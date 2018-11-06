using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace ISBS_New
{
    class ConnectionString
    {
        public string TblName = "";
        public string tmpSort = "";
        public static string Kode = "";
        public static string Kode2 = "";
        public static string Kode3 = "";
        public static string Kode4 = "";
        public static string Kode5 = "";
        public static string[] Kodes;
        public string tmpWhere = "";
        public string tmpNamaFormSearch = "";
        public Boolean tmpAmbilKodeNo = false;
        public static string IdSearchPK = ""; //Global untuk menampung primary key pada saat buka menu baru
        //public static string[] Kode;

        public static SqlConnection GetConnection()
        {
            string Conn = "";
            //Conn = "Data Source=192.168.0.87;Initial Catalog=ISBS-New4;Pwd = sql123;USER ID = sa;MultipleActiveResultSets=true";
            //Conn = "Data Source=192.168.30.127;Initial Catalog=ISBS-New4;Pwd = sql123;USER ID = sa;MultipleActiveResultSets=true";
            //Conn = "Data Source=ITDIVISI14-PC\\DB_ISBS;Initial Catalog=ISBS-New4;Pwd = sql123;USER ID = sa;MultipleActiveResultSets=true";
            Conn = "Data Source=DESKTOP-3USGKTD\\MY_DB_COLLECTION;Initial Catalog=ISBS-New4;Pwd = CIntabulan123;USER ID = sa;MultipleActiveResultSets=true";
            SqlConnection conISBS = new SqlConnection(Conn);
            conISBS.Open();
            return conISBS;
        }

        

        public static string GlobalException(Exception Ex)
        {
            if (Ex is SqlException)
            {
                SqlException ExSql = Ex as SqlException;
                if (ExSql.Number.Equals(547))
                    return "Data tidak bisa dihapus karena masih digunakan.";
                else
                {
                    return ExSql.Message;
                }
            }
            else
                return "Terjadi error : " + Ex;
            return "";
        }

        public void ClearKode()
        {
            Kode = "";
            Kode2 = "";
            Kode3 = "";
            Kode4 = "";
            Kode5 = "";
        }

        public static string Encrypt(string input)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(input)));
        }

        public static byte[] Encrypt(byte[] input)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84 });
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        public static string Decrypt(string input)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(input)));
        }

        public static byte[] Decrypt(byte[] input)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84 });
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

        //public static string GenerateID(string pk, string tableName, string startID)
        //{
        //    string id = null;
        //    SqlConnection Conn = GetConnection();
        //    SqlCommand Cmd = new SqlCommand("select count(*) from [dbo].[" + tableName + "]", Conn);
        //    int count = (Int32)Cmd.ExecuteScalar();
        //    if (count == 0)
        //        count++;
        //    else
        //    {
        //        Cmd = new SqlCommand("SELECT TOP 1 " + pk + " FROM [dbo].[" + tableName + "] order by [CreatedDate] desc", Conn);
        //        string[] lastID = Cmd.ExecuteScalar().ToString().Split('/');
        //        if (lastID[1] != DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM"))
        //            count = 1;
        //        else
        //            count = Convert.ToInt32(lastID[2]) + 1;
        //    }
        //    if (count.ToString().Length == 1)
        //        id += "0000" + count;
        //    else if (count.ToString().Length == 2)
        //        id += "000" + count;
        //    else if (count.ToString().Length == 3)
        //        id += "00" + count;
        //    else if (count.ToString().Length == 4)
        //        id += "0" + count;
        //    else if (count.ToString().Length == 5)
        //        id += "" + count;
        //    Conn.Close();
        //    count = 0;
        //    return startID + "-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-" + id;
        //}

        //public static string GenerateID2(string fieldName, string tableName, string startID)
        //{
        //    int count;
        //    string id = null;
        //    SqlConnection Conn = GetConnection();
        //    SqlCommand Cmd = new SqlCommand("Select max(" + fieldName + ") from " + tableName + " where " + fieldName + " LIKE '%" + startID + "%'", Conn);
        //    if (Cmd.ExecuteScalar().ToString() == String.Empty)
        //        count = 1;
        //    else
        //    {
        //        if (Cmd.ExecuteScalar().ToString().Split('-')[1] != DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM"))
        //            count = 1;
        //        else
        //            count = Convert.ToInt32(Cmd.ExecuteScalar().ToString().Split('/')[2]) + 1;
        //    }
        //    if (count.ToString().Length == 1)
        //        id += "0000" + count;
        //    else if (count.ToString().Length == 2)
        //        id += "000" + count;
        //    else if (count.ToString().Length == 3)
        //        id += "00" + count;
        //    else if (count.ToString().Length == 4)
        //        id += "0" + count;
        //    else if (count.ToString().Length == 5)
        //        id += "" + count;
        //    return startID + "-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-" + id;
        //}

        //format id contoh JN/18/000001 > KODE/Tahun 2 digit/Running number 6 digit

        public static string GenerateSequenceNo(int NumberOfDigit, string TableName,string FieldIdName, SqlConnection Conn, SqlCommand Cmd)
        {
            string ID = "1";
            string Query = "SELECT MAX(CAST("+FieldIdName+" AS int)) FROM "+TableName+"";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                if (Cmd.ExecuteScalar() == System.DBNull.Value)
                {
                    ID = Convert.ToInt32(ID).ToString("D" + NumberOfDigit + "");
                }
                else
                {
                    ID = (Convert.ToInt32(Cmd.ExecuteScalar()) + 1).ToString("D"+NumberOfDigit+"");
                }
                if (ID.Length > NumberOfDigit)
                {
                    ID = "";
                    MessageBox.Show("Jumlah ID sudah melebihi batas maksimum ID.");
                }
            }
            return ID;
        }

        //GENERATE ID with format like JN/18/000001 > JN is the kode,18 is the last 2 digit of the current year, 00000001 > the number of digits is set by the NumberOfDigit
        //the counter use the normal counter from table counter
        public static string GenerateSeqID(int NumberOfDigit,string Kode,string Jenis, SqlConnection Conn, SqlCommand Cmd)
        {
            int counter = 0;
            string ID = "";
            string Query = "SELECT Counter FROM Counter WHERE Jenis='" + Jenis + "' AND Kode = '" + Kode + "'";
            Cmd = new SqlCommand(Query, Conn);
            ID = (Convert.ToInt32(Cmd.ExecuteScalar()) + 1).ToString("D" + NumberOfDigit + "");
            
            if (ID.Length > NumberOfDigit)
            {
                ID = "";
                MessageBox.Show("Jumlah ID sudah melebihi batas maksimum ID.");
            }
            else
            {
                ID = Kode + "/" + DateTime.Now.ToString("yy") + "/" + ID;

                Query = "UPDATE Counter SET Counter = (Counter + 1),";
                Query += "UpdatedDate=getdate(),";
                Query += "UpdatedBy='SYSTEM' WHERE Jenis='" + Jenis + "' AND Kode = '" + Kode + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }

            return ID;
        }

        public static string GenerateSeqID(int NumberOfDigit, string Kode, string Jenis, SqlConnection Conn, SqlTransaction Trans,SqlCommand Cmd)
        {
            int counter = 0;
            string ID = "";
            string Query = "SELECT Counter FROM Counter WHERE Jenis='" + Jenis + "' AND Kode = '" + Kode + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            ID = (Convert.ToInt32(Cmd.ExecuteScalar()) + 1).ToString("D" + NumberOfDigit + "");

            if (ID.Length > NumberOfDigit)
            {
                ID = "";
                MessageBox.Show("Jumlah ID sudah melebihi batas maksimum ID.");
            }
            else
            {
                ID = Kode + "/" + DateTime.Now.ToString("yy") + "/" + ID;

                Query = "UPDATE Counter SET Counter = (Counter + 1),";
                Query += "UpdatedDate=getdate(),";
                Query += "UpdatedBy='SYSTEM' WHERE Jenis='" + Jenis + "' AND Kode = '" + Kode + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }

            return ID;
        }

        public static string GenerateSequenceNo(string Jenis, string Kode, string TableName, string ID, SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            string result = "0";

            Cmd = new SqlCommand("sp_generate_sequence_number", Conn, Trans);
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Jenis", Jenis);
            Cmd.Parameters.AddWithValue("@Kode", Kode);
            Cmd.Parameters.AddWithValue("@TableName", TableName);
            Cmd.Parameters.AddWithValue("@Id", ID);
            Cmd.Parameters.Add("@SequenceNo", SqlDbType.VarChar, 25).Direction = ParameterDirection.Output;
            Cmd.ExecuteNonQuery();
            result = Convert.ToString(Cmd.Parameters["@SequenceNo"].Value);

            if (TableName == "")
            {
                string Query = "";

                Query = "UPDATE Counter SET Counter = (Counter + 1),";
                Query += "UpdatedDate=getdate(),";
                Query += "UpdatedBy='SYSTEM' WHERE Jenis='" + Jenis + "' AND Kode = '" + Kode + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery(); 
            }
           
            return result;
        }

        public static string GenerateSequenceNo(string Jenis, string Kode, string TableName, string ID, SqlConnection Conn, SqlCommand Cmd)
        {
            string result = "0";

            Cmd = new SqlCommand("sp_generate_sequence_number", Conn);
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@Jenis", Jenis);
            Cmd.Parameters.AddWithValue("@Kode", Kode);
            Cmd.Parameters.AddWithValue("@TableName", TableName);
            Cmd.Parameters.AddWithValue("@Id", ID);
            Cmd.Parameters.Add("@SequenceNo", SqlDbType.VarChar, 25).Direction = ParameterDirection.Output;
            Cmd.ExecuteNonQuery();
            result = Convert.ToString(Cmd.Parameters["@SequenceNo"].Value);

            if (TableName == "")
            {
                string Query = "";

                Query = "UPDATE Counter SET Counter = (Counter + 1),";
                Query += "UpdatedDate=getdate(),";
                Query += "UpdatedBy='SYSTEM' WHERE Jenis='" + Jenis + "' AND Kode = '" + Kode + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }

            return result;
        }

        //public static string UpdateCounter(string Jenis, string Kode, SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        //{
        //    string result = "success";
        //    string Query = "";

        //    Query = "UPDATE Counter SET Counter = (Counter + 1),";
        //    Query += "UpdatedDate=getdate(),";
        //    Query += "UpdatedBy='SYSTEM' WHERE Jenis='" + Jenis + "' AND Kode = '" + Kode + "'";
        //    Cmd = new SqlCommand(Query, Conn, Trans);
        //    Cmd.ExecuteNonQuery();         

        //    return result;
        //}

        public static List<string> GetPermissionAccess(string ClassName, string GroupName)
        {
            List<string> AuthorityName = new List<string>();
            string Query = "";
            AuthorityName.Clear();

            SqlDataReader Dr;
            SqlConnection Conn = GetConnection();
            Query = "SELECT AuthorityName FROM sysGroupAuthority WHERE GroupName = '" + GroupName + "' AND SubMenuNameClass = '" + ClassName + "'";
            using (SqlCommand Command = new SqlCommand(Query, Conn))
            {
                Dr = Command.ExecuteReader();
                while (Dr.Read())
                {
                    AuthorityName.Add(Dr["AuthorityName"].ToString());
                    
                }
            }
            Dr.Close();
            Conn.Close();

            return AuthorityName;
        }

        public static int CheckPermissionAccess(string ClassName, string Authority)
        {
            //hendry tambah group Administrator tidak perlu cek akses
            List<string> AuthorityName = new List<string>();
            int AccessCount = 0;

            if (ControlMgr.GroupName == "Administrator")
            {
                AccessCount = 1;
            }
            else
            {
                AuthorityName = GetPermissionAccess(ClassName, ControlMgr.GroupName);

                AccessCount = 0;
                foreach (string access in AuthorityName)
                {
                    if (access.ToUpper() == Authority.ToUpper())
                    {
                        AccessCount = 1;
                    }
                }
            }
            return AccessCount;
        }

        public static void DisableOrEnableControls(Control con, bool boolean)
        {
            //BY: HC (S)
            foreach (Control c in con.Controls)
            {
                DisableOrEnableControls(c, boolean);
            }
            con.Enabled = boolean;
            //BY: HC (E)
        }
        public static void EnableControls(Control con)
        {
            //BY: HC (S)
            if (con != null)
            {
                con.Enabled = true;
                EnableControls(con.Parent);
            }
            //BY: HC (E)
        }
    }
}
