using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace ISBS_New
{
    class Connection : IDisposable
    {
        #region INISIASI AWAL
        /********* SQL *********/
        public SqlTransaction Trans;
        public SqlDataReader Dr;
        public SqlDataAdapter Da;
        public SqlCommand Cmd;
        public DataTable Dt;
        public DataSet Ds;
        /********* SQL *********/

        //public string Query;
        //public int Index;
        #endregion

        #region METHOD YANG DIGUNAKAN
        /********* GLOBAL CONNECTION *********/
        public SqlConnection Conn()
        {
            string Conn = "";
            Conn = "Data Source=192.168.0.87;Initial Catalog=ISBS-NEW4;Pwd = sql123;USER ID = sa;MultipleActiveResultSets=true";
            SqlConnection conISBS = new SqlConnection(Conn);
            conISBS.Open();
            return conISBS;
        }
        /********* GLOBAL CONNECTION *********/
      
        /********* GLOBAL ERROR UNTUK SQL ************/
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
        /********* GLOBAL ERROR UNTUK SQL ************/
        #endregion
      
        #region DISPOSE FROM INTERNET
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion
    }
}
