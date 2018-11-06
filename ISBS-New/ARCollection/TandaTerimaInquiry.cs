using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Data.SqlClient;
using System.Collections.Generic;
using CrystalDecisions.ReportSource;
using System.IO;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace ISBS_New.ARCollection
{
    public partial class TandaTerimaInquiry : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        string TransStatus = "01";

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public TandaTerimaInquiry()
        {
            InitializeComponent();
        }

        private void TandaTerimaInquiry_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        public void RefreshGrid()
        { 
            //Menampilkan Data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") ";
                Query += ") a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.Kwt_Id DESC";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%') ";
                Query += " ) a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.Kwt_Id DESC";
            }
            else if (crit.Equals("TT Date"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND ((CONVERT(VARCHAR(10),Kwt_Date,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),Kwt_Date,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%')) ";
                Query += " ) a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.Kwt_Id DESC";
            }
            else if(crit.Equals("Due Date From"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND ((CONVERT(VARCHAR(10),(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%')) ";
                Query += " ) a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.Kwt_Id DESC";
            }
            else if (crit.Equals("Due Date To"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND ((CONVERT(VARCHAR(10),(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%')) ";
                Query += " ) a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.Kwt_Id DESC";
            }
            
            else 
            {
                if(crit == "TT No")
                {
                    crit = "Kwt_Id";
                }
                else if (crit == "Customer")
                {
                    crit = "(SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id])";
                }
                else if (crit == "Customer Id")
                {
                    crit = "Cust_Id";
                }
                else if (crit == "Jumlah Invoice")
                {
                    crit = "(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id])";
                }
                else if (crit == "Total Invoice Amount")
                {
                    crit = "(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id])";
                }

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id WHERE kw.StatusCode IN (" + TransStatus + ") AND ";
                Query += crit + " like '%" + txtSearch.Text + "%' ";
                Query += " ) a ";
                Query += " Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.Kwt_Id DESC";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvTandaTerima.AutoGenerateColumns = true;
            dgvTandaTerima.DataSource = Dt;
            dgvTandaTerima.Refresh();
            dgvTandaTerima.AutoResizeColumns();

            dataPaging();
            dgvSetting();           
        }

        private void dataPaging()
        {
            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw WHERE kw.StatusCode IN (" + TransStatus + ")";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ) a ";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%') ";
                Query += " ) a ";
            }
            else if (crit.Equals("TT Date"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND ((CONVERT(VARCHAR(10),Kwt_Date,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),Kwt_Date,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%')) ";
                Query += " ) a ";
            }
            else if (crit.Equals("Due Date From"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND ((CONVERT(VARCHAR(10),(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%')) ";
                Query += " ) a ";
            }
            else if (crit.Equals("Due Date To"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id ";
                Query += " Where kw.StatusCode IN (" + TransStatus + ") AND ((CONVERT(VARCHAR(10),(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]),120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND (Kwt_Id like '%" + txtSearch.Text + "%' or Kwt_Date like '%" + txtSearch.Text + "%' or Cust_Id like '%" + txtSearch.Text + "%' or (SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id]) like '%" + txtSearch.Text + "%' or (SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%' or (SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) like '%" + txtSearch.Text + "%')) ";
                Query += " ) a ";
            }
            else
            {
                if (crit == "TT No")
                {
                    crit = "Kwt_Id";
                }
                else if (crit == "Customer")
                {
                    crit = "(SELECT (CustName) FROM [CustTable] WHERE CustId = [Cust_Id])";
                }
                else if (crit == "Customer Id")
                {
                    crit = "Cust_Id";
                }
                else if (crit == "Jumlah Invoice")
                {
                    crit = "(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id])";
                }
                else if (crit == "Total Invoice Amount")
                {
                    crit = "(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id])";
                }

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY Kwt_Id DESC) No, [Kwt_Id], [Kwt_Date], [Cust_Id], [CustName] ";
                Query += ",(SELECT COUNT (Invoice_Id) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) JumlahInvoice ";
                Query += ",(SELECT SUM (Invoice_Amount) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) TotalInvoiceAmount ";
                Query += ",(SELECT MIN (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateFrom ";
                Query += ",(SELECT MAX (Invoice_Date) FROM [CustInvoice_H] WHERE Kwitansi_No = [Kwt_Id]) DueDateTo ";
                Query += ",kw.[StatusCode],[Deskripsi] ";
                Query += " From [dbo].[Kwitansi] kw ";
                Query += " LEFT JOIN [TransStatusTable] ts ON ts.StatusCode = kw.StatusCode AND TransCode = 'Kwitansi' ";
                Query += " LEFT JOIN [CustTable] ct ON ct.CustId = kw.Cust_Id Where kw.StatusCode IN (" + TransStatus + ") AND ";
                Query += crit + " like '%" + txtSearch.Text + "%' ";
                Query += " ) a ";
            }
       
            Cmd = new SqlCommand(Query, Conn);
            try
            {
                Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            }
            catch
            {
                Total = 0;
            }
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void dgvSetting()
        {
            dgvTandaTerima.Columns["Kwt_Id"].HeaderText = "TT No";
            dgvTandaTerima.Columns["Kwt_Date"].HeaderText = "TT Date";
            dgvTandaTerima.Columns["CustName"].HeaderText = "Customer";
            dgvTandaTerima.Columns["JumlahInvoice"].HeaderText = "Jumlah Invoice";
            dgvTandaTerima.Columns["TotalInvoiceAmount"].HeaderText = "Total Invoice Amount";
            dgvTandaTerima.Columns["DueDateFrom"].HeaderText = "Due Date From";
            dgvTandaTerima.Columns["DueDateTo"].HeaderText = "Due Date To ";

            dgvTandaTerima.Columns["Cust_Id"].Visible = false;
            dgvTandaTerima.Columns["StatusCode"].Visible = false;

            dgvTandaTerima.Columns["DueDateFrom"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvTandaTerima.Columns["DueDateTo"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'TandaTerima' Order By OrderNo";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;
            cmbCriteria.SelectedIndex = 0;
            crit = "All";
            RefreshGrid();
        }

        #region page navigation

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }
            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                RefreshGrid();
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShow.Items.Clear();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr.GetInt32(0));
            }
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }
        #endregion page navigation

        private void btnNew_Click(object sender, EventArgs e)
        {
            Kwitansi F = new Kwitansi();
            F.Show();
            F.ModeNew();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {            
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    Index = dgvTandaTerima.CurrentRow.Index;
                    string deletedKWNO = dgvTandaTerima.Rows[Index].Cells["Kwt_Id"].Value == null ? "" : dgvTandaTerima.Rows[Index].Cells["Kwt_Id"].Value.ToString();

                    Conn = ConnectionString.GetConnection();

                    Conn = ConnectionString.GetConnection();
                    Query = "Select [StatusCode] from [Kwitansi] Where [Kwt_Id] = '" + deletedKWNO + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    var tmpStatusCode = Cmd.ExecuteScalar();

                    if (tmpStatusCode.ToString() == "05")
                    {
                        MessageBox.Show("Tanda Terima tidak bisa di-edit karena sudah diproses");
                        return;
                    }
                    else
                    {
                        DialogResult dr = MessageBox.Show("Apakah Anda Ingin Reject Tanda Terima " + deletedKWNO + " ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            Query = "Update [Kwitansi] SET [StatusCode] = '05' Where [Kwt_Id] = '" + deletedKWNO + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                            MessageBox.Show("Tanda Terima " + deletedKWNO + " rejected");
                        }                    
                    }
                    RefreshGrid();
                    Conn.Close();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectData();
        }

        private void SelectData()
        {
            Kwitansi f = new Kwitansi();
            if (dgvTandaTerima.RowCount > 0)
            {
                f.Show();
                f.ModeBeforeEdit();
                f.GetDataHeader(dgvTandaTerima.CurrentRow.Cells["Kwt_Id"].Value.ToString());
                f.RefreshGrid();
                RefreshGrid();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }

            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            cmbCriteria.SelectedIndex = 0;
            ModeLoad();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
                txtSearch.Text = "";
                txtSearch.Enabled = false;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
                txtSearch.Text = "";
                txtSearch.Enabled = true;
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (cmbCriteria.SelectedIndex == -1)
                {
                    crit = "All";
                }
                else
                {
                    crit = cmbCriteria.SelectedItem.ToString();
                }
                RefreshGrid();
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'05'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }     

        //Created by Thaddaeus Matthias, 15 March 2018
        //insert status log deleted data
        //========================================begin=========================================

        //private void insertstatuslogDelete()
        //{
        //    SqlConnection Conn = ConnectionString.GetConnection();
        //    Index = dgvTandaTerima.CurrentRow.Index;
        //    string PurchID = dgvTandaTerima.Rows[Index].Cells["PurchID"].Value == null ? "" : dgvTandaTerima.Rows[Index].Cells["PurchID"].Value.ToString();
        //    string VendorID = dgvTandaTerima.Rows[Index].Cells["VendID"].Value == null ? "" : dgvTandaTerima.Rows[Index].Cells["VendID"].Value.ToString();
        //    string PK3 = dgvTandaTerima.Rows[Index].Cells["ReffID"].Value == null ? "" : dgvTandaTerima.Rows[Index].Cells["ReffID"].Value.ToString();

        //    Query = "INSERT INTO [dbo].[StatusLog_Vendor] VALUES "; //[StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]
        //    Query += " ('POInquiry', '" + PurchID + "', '" + VendorID + "', '" + PK3 + "', '', 'XX', 'PO Data Deleted', '" + ControlMgr.UserId + "', getdate()) ";
        //    SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
        //    cmd2.ExecuteNonQuery();

        //    Query = "UPDATE [dbo].[StatusLog_Vendor] SET StatusLog_PK4 = (SELECT TOP 1 [ReffId2] FROM [ISBS-NEW4].[dbo].[PurchH] WHERE [PurchID] LIKE '%" + PurchID.ToString() + "%' ORDER BY [PurchID] DESC) WHERE StatusLog_PK1 =  '" + PurchID + "' ";
        //    SqlCommand cmd3 = new SqlCommand(Query, Conn, Trans);
        //    cmd3.ExecuteNonQuery();
        //}

        private void dgvTandaTerima_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectData();
        }
    }
}
