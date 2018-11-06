using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace ISBS_New
{
    class CheckAuthDB
    {
        private static string qry;
        private static string connectionString = "Data Source=192.168.0.87;Initial Catalog=ISBS-New4;Pwd = sql123;USER ID = sa;MultipleActiveResultSets=true";

        public static List<CheckAuth>getAllAuths(string userGroup)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            qry = "select b.groupname, C.MenuName, C.SubMenu1, C.SubMenu2, A.AuthName from usergroupauthority as A left join [dbo].[Group] as b on a.GroupCode = b.GroupCode left join [dbo].[Menu] as C on C.MenuID = A.MenuID where b.groupname = @userGroup";
            SqlCommand com = new SqlCommand(qry, conn);
            com.Parameters.AddWithValue("@userGroup", userGroup);
            conn.Open();
            SqlDataReader reader = com.ExecuteReader();
            List<CheckAuth> Auths = new List<CheckAuth>();
            while (reader.Read())
            {
                CheckAuth a = new CheckAuth(reader["UserGroupAuthID"].ToString(),
                                            reader["groupCode"].ToString(),
                                            reader["authName"].ToString(),
                                            reader["menuID"].ToString());
                Auths.Add(a);
            }
            reader.Close();
            conn.Close();
            return Auths;
        }
    }
}
