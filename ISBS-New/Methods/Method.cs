using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace ISBS_New
{
    class Method : IDisposable
    {
        #region INISIASI AWAL VARIABEL
        //SQL
        public SqlConnection Conn = ConnectionString.GetConnection();
        public SqlTransaction Trans;
        public SqlDataReader Dr;
        public SqlDataAdapter Da;
        public SqlCommand Cmd;
        public DataTable Dt;
        public DataSet Ds;

        //UNTUK METHOD VALIDASI
        public bool ValValidasi = true;
        #endregion

        #region VALIDASI,MANDATORY UNTUK WARNA TEXTBOX
            
            //MEREPLACE SINGLE QUOTE (')
            public void RepQuot(ref string TmpStr)
            {
                TmpStr.Replace("'", "''");
            }
            
            //VALIDASI MEMBERIKAN WARNA MERAH
            public System.Windows.Forms.Control Validasi(System.Windows.Forms.Control TmpControl)
            {
                TmpControl.BackColor = Color.White;
                if (TmpControl.GetType() == typeof(System.Windows.Forms.TextBox) && TmpControl.Text == String.Empty)
                {
                    ValValidasi = false;
                    TmpControl.BackColor = Color.LightPink;
                    return TmpControl;
                }
                return TmpControl;
            }
            
            //MEMBERIKAN WARNA KUNING
            public System.Windows.Forms.Control Mandatory(System.Windows.Forms.Control TmpControl)
            {
                TmpControl.BackColor = Color.LightYellow;
                return TmpControl;
            }
            
        #endregion

        #region METHOD YANG DIGUNAKAN UNTUK GLOBAL DATAGRID
            //MENAMPILKAN DATAGRID STORED PROCEDURE
            public DataTable DgvAutoSp(string TmpSpName)
            {
                Cmd = new SqlCommand(TmpSpName, Conn);
                Cmd.CommandType = CommandType.StoredProcedure;
                Da = new SqlDataAdapter();
                Da.SelectCommand = Cmd;
                Dt = new DataTable();
                Da.Fill(Dt);
                return Dt;
            }

            //MENAMPILKAN DATAGRID DENGAN QUERY
            public DataTable DgvAutoQuery(string TmpSpName)
            {
                Cmd = new SqlCommand(TmpSpName, Conn);
                Da = new SqlDataAdapter();
                Da.SelectCommand = Cmd;
                Dt = new DataTable();
                Da.Fill(Dt);
                return Dt;
            }
        #endregion

        #region DATAREADER, EXECUTE SCALAR, EXECUTE NON SCALAR
        //MENGEMBALIKAN DATAREADER
        public SqlDataReader ReturnDr(string TmpQuery)
        {
            Cmd = new SqlCommand(TmpQuery, Conn);
            return Cmd.ExecuteReader();
        }

        //MENGEMBALIKAN EXECUTE SCALAR
        public object ReturnScalar(string TmpQuery)
        {
            Cmd = new SqlCommand(TmpQuery, Conn);
            return Cmd.ExecuteScalar();
        }
        
        //MELAKUKAN EXECUTE NON QUERY
        public void ExecuteNonQuery(string TmpQuery)
        {
            Cmd = new SqlCommand(TmpQuery, Conn);
            Cmd.ExecuteNonQuery();
        }
        #endregion 

        #region LIST INSERT UPDATE DELETE SELECT
        //METHOD UNTUK MELAKUKAN DELETE DATABASE TRANS
        public void DeleteDBTrans(string TmpTabel, string[] TmpParam, string[] TmpData)
        {
            //DialogResult dr = MessageBox.Show(DisplayParam(TmpParam, TmpData) + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //if (dr == DialogResult.Yes)
            //{
         
                string Query = "Delete From " + TmpTabel + " Where " + DeleteQueryWhere(TmpParam, TmpData);

                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

            //}
        }

        //METHOD UNTUK MELAKUKAN DELETE DATABASE TRANS
        public void DeleteDB(string TmpTabel, string[] TmpParam, string[] TmpData)
        {
            DialogResult dr = MessageBox.Show(DisplayParam(TmpParam, TmpData) + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {

                string Query = "Delete From " + TmpTabel + " Where " + DeleteQueryWhere(TmpParam, TmpData);

                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                MessageBox.Show(DisplayParam(TmpParam, TmpData) + "Data berhasil dihapus.");
            }
        }

        //MENAMPILKAN NAMA YANG AKAN DIDELETE --> DIGUNAKAN OLEH METHOD DELETEDB
        public string DisplayParam(string[] TmpParam, string[] TmpData)
        {
            string TmpReturn = "";
            for (int i = 0; i < TmpParam.Count(); i++)
            {
                TmpReturn = TmpParam[i] + " = " + TmpData[i] + "\n";
            }
            return TmpReturn;
        }

        //MENAMPILKAN NAMA YANG AKAN DIDELETE --> DIGUNAKAN OLEH METHOD DELETEDB
        public string DisplayDeleteParam(System.Windows.Forms.DataGridView TmpDgv, int Index, string[] TmpParams)
        {
            string TmpReturn = "";
            for (int i = 0; i < TmpParams.Count(); i++)
            {
                TmpReturn += TmpParams[i] + " = " + (TmpDgv.Rows[Index].Cells[TmpParams[i]].Value == null ? "" : TmpDgv.Rows[Index].Cells[TmpParams[i]].Value.ToString()) + "\n";
            }
            return TmpReturn;
        }

        //MELAKUKAN SINGLE SAVE TABLE
        public void SaveTbl(string TmpTabelName, string[] TmpField, string[] TmpData, string[] TmpDisplayField, string[] TmpDisplayData)
        {
            string Query = "Insert into " + TmpTabelName + " (";
            for (int i = 0; i < TmpField.Count(); i++)
            {
                Query += TmpField[i];
                if (i < TmpField.Count() - 1)
                {
                    Query += ",";
                }
            }
            Query += ") values (";
            for (int i = 0; i < TmpData.Count(); i++)
            {
                if (TmpData[i].ToString().Contains("getdate()"))
                {
                    Query += TmpData[i]; 
                }
                else
                {
                    Query += "'" + TmpData[i] + "'";
                }

                if (i < TmpData.Count() - 1)
                {
                    Query += ",";
                }
            }
            Query += ")";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            MessageBox.Show(DisplayParam(TmpDisplayField, TmpDisplayData) + "Data berhasil ditambahkan.");
    
        }

        //MELAKUKAN SINGLE UPDATE TABLE
        public void UpdateTbl(string TmpTabelName, string[] TmpField, string[] TmpData, string[] TmpDisplayField, string[] TmpDisplayData)
        {
            string Query = "Update into " + TmpTabelName + " (";
            for (int i = 0; i < TmpField.Count(); i++)
            {
                Query += TmpField[i];
                if (i < TmpField.Count() - 1)
                {
                    Query += ",";
                }
            }
            Query += ")";
            for (int i = 0; i < TmpData.Count(); i++)
            {
                Query += "'" + TmpData[i] + "'";
                if (i < TmpData.Count() - 1)
                {
                    Query += ",";
                }
            }
            Query += ")";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            MessageBox.Show(DisplayParam(TmpDisplayField, TmpDisplayData) + "Data berhasil ditambahkan.");
        }

        //MELAKUKAN SINGLE UPDATE TABLE
        public void UpdateTblTrans(string TmpTabelName, string[] TmpField, string[] TmpData, string[] TmpWhereField, string[] TmpWhereData)
        {
            string Query = "Update " + TmpTabelName + " SET ";
            for (int i = 0; i < TmpField.Count(); i++)
            {
                if (TmpData[i] != null)
                {
                    if (TmpData[i].ToString().Contains("getdate()"))
                    {
                        Query += TmpField[i].ToString() + "=" + TmpData[i].ToString() + " ";
                    }
                    else
                    {
                        Query += TmpField[i].ToString() + "='" + TmpData[i].ToString() + "' ";
                    }
                    if (i < TmpField.Count() - 1)
                    {
                        Query += ", ";
                    }
                }
                else
                {
                    Query += TmpField[i].ToString() + "='' ";
                    if (i < TmpField.Count() - 1)
                    {
                        Query += ", ";
                    }
                }
            }
            for (int i = 0; i < TmpWhereField.Count(); i++)
            {
                Query += "Where " + TmpWhereField[i].ToString() + "='" + TmpWhereData[i].ToString() + "'";
                if (i < TmpWhereField.Count() - 1)
                {
                    Query += "And ";
                }
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        
        }

        //SAVE TABLE DENGAN KONDISI TRANS
        public void SaveTblTrans(string TmpTabelName, string[] TmpField, string[] TmpData)
        {
            string Query = "Insert into " + TmpTabelName + " (";
            for (int i = 0; i < TmpField.Count(); i++)
            {
                Query += TmpField[i];
                if (i < TmpField.Count() - 1)
                {
                    Query += ",";
                }
            }
            Query += ") values (";
            for (int i = 0; i < TmpData.Count(); i++)
            {
                if (TmpField[i].ToUpper().Contains("CREATEDDATE"))
                {
                    Query += TmpData[i];
                }
                else
                {
                    Query += "'" + TmpData[i] + "'";
                }

                if (i < TmpData.Count() - 1)
                {
                    Query += ",";
                }
            }
            Query += ")";
            
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            //MessageBox.Show(DisplayParam(TmpDisplayField, TmpDisplayData) + "Data berhasil ditambahkan.");
        }

        //DIGUNAKAN UNTUK CEK PARAMETER YANG AKAN DIDELETE --> DIGUNAKAN OLEH METHOD DELETEDB
        private string DeleteQueryWhere(string[] TmpParam, string[] TmpData)
        {
            string TmpReturn = "";
            for (int i = 0; i < TmpParam.Count(); i++)
            {
                TmpReturn += TmpParam[i] + " = '" + TmpData[i] + "'";
                if (i < (TmpParam.Count() - 1))
                {
                    TmpReturn += " and ";
                }
            }
            return TmpReturn;
        }
        #endregion

        #region DATAGRID METHOD
        //METHOD UNTUK MENDELETE 1 ROW DI DATAGRID (TEMPORARY)
        public void DeleteDgv1(System.Windows.Forms.DataGridView TmpDgv, int Index, string[] TmpParam)
        {
            if (TmpDgv.RowCount > 0)
            {
                DialogResult dr = MessageBox.Show(DisplayDeleteParam(TmpDgv,Index,TmpParam) + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    TmpDgv.Rows.RemoveAt(Index);
                }
            }
            SortNoDataGrid(TmpDgv);
        }

        //METHOD UNTUK MENGURUTKAN NO DATAGRID
        private void SortNoDataGrid(System.Windows.Forms.DataGridView TmpDgv)
        {
            for (int i = 0; i < TmpDgv.RowCount; i++)
            {
                TmpDgv.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        //METHOD UNTUK MELAKUKAN PENAMAAN HEADER DATAGRID (DIGUNAKAN DIAWAL PEMBUATAN DATAGRID)
        public void DgvCreate(System.Windows.Forms.DataGridView TmpDgv, string[] TmpHeader)
        {
            if (TmpHeader != null)
            {
                TmpDgv.ColumnCount = TmpHeader.Count();
                for (int i = 0; i < TmpHeader.Count(); i++)
                {
                    TmpDgv.Columns[i].Name = TmpHeader[i];
                }
            }
        }

        //SET KOLOM DATAGRID MENJADI VISIBLE
        public void DgvVisible(System.Windows.Forms.DataGridView TmpDgv, string[] TmpHeader)
        {
            if (TmpHeader != null)
            {
                for (int i = 0; i < TmpHeader.Count(); i++)
                {
                    TmpDgv.Columns[TmpHeader[i]].Visible = false;
                }
            }
        }

        //SET KOLOM DATAGRID MENJADI READONLY
        public void DgvReadOnly(System.Windows.Forms.DataGridView TmpDgv, string[] TmpHeader)
        {
            if (TmpHeader != null)
            {
                for (int i = 0; i < TmpHeader.Count(); i++)
                {
                    TmpDgv.Columns[TmpHeader[i]].ReadOnly = true;
                }
            }
        }

        //SET KOLOM DATAGRID SELAIN CHECK MENJADI READONLY
        public void DgvCheckReadOnly(System.Windows.Forms.DataGridView TmpDgv)
        {
            for (int i = 0; i < TmpDgv.ColumnCount ; i++)
            {
                if (TmpDgv.Columns[i].Name != "Check")
                {
                TmpDgv.Columns[i].ReadOnly = true;
                }
            }
        }


        //SET KOLOM DATAGRID WARNA
        public void DgvColor( System.Windows.Forms.DataGridView TmpDgv, string[] TmpHeader , Color TmpColor)
        {
            for (int i = 0; i < TmpHeader.Count(); i++)
            {
                TmpDgv.Columns[TmpHeader[i]].DefaultCellStyle.BackColor = TmpColor;
            }
        }

        //SET KOLOM DATAGRID MENJADI NONSORT
        public void DgvNotSort(System.Windows.Forms.DataGridView TmpDgv, string[] TmpHeader)
        {
            for (int i = 0; i < TmpHeader.Count(); i++)
            {
                TmpDgv.Columns[TmpHeader[i]].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        //SET KOLOM DATAGRID MENJADI RATA KANAN
        public void DgvAlignRight(System.Windows.Forms.DataGridView TmpDgv, string[] TmpHeader)
        {
            for (int i = 0; i < TmpHeader.Count(); i++)
            {
                TmpDgv.Columns[TmpHeader[i]].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }
        #endregion

        #region GLOBAL ENABLE TEXTBOX,DATETIPICKER,BUTTON,DATAGRID 
            public void ControlNonActivation(System.Windows.Forms.Form TmpForm, bool TmpBool)
            {
                foreach (System.Windows.Forms.Control TmpControlTab in TmpForm.Controls)
                {
                    if (TmpControlTab is TabControl && TmpControlTab.HasChildren)
                    {
                        foreach (System.Windows.Forms.Control TmpControlPage in TmpControlTab.Controls)
                        {
                            if (TmpControlTab is TabControl && TmpControlTab.HasChildren)
                            {
                                foreach (System.Windows.Forms.Control TmpControl in TmpControlTab.Controls)
                                {
                                    if (TmpControl is GroupBox && TmpControl.HasChildren)
                                    {
                                        foreach (Control TmpControl1 in TmpControl.Controls)
                                        {
                                            if (TmpControl1.GetType() == typeof(DateTimePicker))
                                            {
                                                TmpControl1.Enabled = TmpBool;
                                            }
                                            else if (TmpControl1.GetType() == typeof(ComboBox))
                                            {
                                                TmpControl1.Enabled = TmpBool;
                                            }
                                            else if (TmpControl1.GetType() == typeof(Button))
                                            {
                                                TmpControl1.Enabled = TmpBool;
                                            }
                                            else if (TmpControl1.GetType() == typeof(DataGridView))
                                            {
                                                TmpControl1.Enabled = TmpBool;
                                            }
                                            else if (TmpControl1.GetType() == typeof(CheckBox))
                                            {
                                                TmpControl1.Enabled = TmpBool;
                                            }
                                        }
                                        foreach (TextBoxBase TmpControl1 in TmpControl.Controls.OfType<TextBoxBase>())
                                        {
                                            TmpControl1.ReadOnly = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (TmpControlTab is TabControl && TmpControlTab.HasChildren)
                    {
                        foreach (System.Windows.Forms.Control TmpControl in TmpControlTab.Controls)
                        {
                            if (TmpControl is GroupBox && TmpControl.HasChildren)
                            {
                                foreach (Control TmpControl1 in TmpControl.Controls)
                                {
                                    if (TmpControl1.GetType() == typeof(DateTimePicker))
                                    {
                                        TmpControl1.Enabled = TmpBool;
                                    }
                                    else if (TmpControl1.GetType() == typeof(ComboBox))
                                    {
                                        TmpControl1.Enabled = TmpBool;
                                    }
                                    else if (TmpControl1.GetType() == typeof(Button))
                                    {
                                        TmpControl1.Enabled = TmpBool;
                                    }
                                    else if (TmpControl1.GetType() == typeof(DataGridView))
                                    {
                                        TmpControl1.Enabled = TmpBool;
                                    }
                                    else if (TmpControl1.GetType() == typeof(CheckBox))
                                    {
                                        TmpControl1.Enabled = TmpBool;
                                    }

                                }
                                foreach (TextBoxBase TmpControl1 in TmpControl.Controls.OfType<TextBoxBase>())
                                {
                                    TmpControl1.ReadOnly = true;
                                }
                            }
                        }
                    }
                    else if (TmpControlTab is GroupBox && TmpControlTab.HasChildren)
                    {
                        foreach (Control TmpControl1 in TmpControlTab.Controls)
                        {
                            if (TmpControl1.GetType() == typeof(DateTimePicker))
                            {
                                TmpControl1.Enabled = TmpBool;
                            }
                            else if (TmpControl1.GetType() == typeof(ComboBox))
                            {
                                TmpControl1.Enabled = TmpBool;
                            }
                            else if (TmpControl1.GetType() == typeof(Button))
                            {
                                TmpControl1.Enabled = TmpBool;
                            }
                            else if (TmpControl1.GetType() == typeof(DataGridView))
                            {
                                TmpControl1.Enabled = TmpBool;
                            }
                            else if (TmpControl1.GetType() == typeof(CheckBox))
                            {
                                TmpControl1.Enabled = TmpBool;
                            }

                        }
                        foreach (TextBoxBase TmpControl1 in TmpControlTab.Controls.OfType<TextBoxBase>())
                        {
                            TmpControl1.ReadOnly = true;
                        }
                    }
                    else if (TmpControlTab.GetType() == typeof(DateTimePicker))
                    {
                        TmpControlTab.Enabled = TmpBool;
                    }
                    else if (TmpControlTab.GetType() == typeof(ComboBox))
                    {
                        TmpControlTab.Enabled = TmpBool;
                    }
                    else if (TmpControlTab.GetType() == typeof(Button))
                    {
                        TmpControlTab.Enabled = TmpBool;
                    }
                    else if (TmpControlTab.GetType() == typeof(DataGridView))
                    {
                        TmpControlTab.Enabled = TmpBool;
                    }
                    else if (TmpControlTab.GetType() == typeof(CheckBox))
                    {
                        TmpControlTab.Enabled = TmpBool;
                    }
                }

            }
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
