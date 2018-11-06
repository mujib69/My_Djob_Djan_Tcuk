using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.TaskList.Purchase.PurchaseAgreement
{
    public partial class TaskListPA : MetroFramework.Forms.MetroForm
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
      
        private string TransStatus = String.Empty;
        string Detail = "";

        public TaskListPA()
        {
            InitializeComponent();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10 * 1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        private void gvHeader()
        {
            dgvPA.Columns["AgreementID"].HeaderText = "PA ID";
            dgvPA.Columns["OrderDate"].HeaderText = "Order Date";
            dgvPA.Columns["DueDate"].HeaderText = "Due Date";
            dgvPA.Columns["TransType"].HeaderText = "Transaction Type";
            dgvPA.Columns["StatusName"].HeaderText = "StatusName";
            dgvPA.Columns["CanvasId"].HeaderText = "CS ID";
            dgvPA.Columns["PurchQuotID"].HeaderText = "PQ ID";
            dgvPA.Columns["CurrencyID"].HeaderText = "Currency";
            dgvPA.Columns["ExchRate"].HeaderText = "Exchange Rate";
            dgvPA.Columns["VendID"].HeaderText = "Vendor ID";
            dgvPA.Columns["Total_Disk"].HeaderText = "Total Discount";
            dgvPA.Columns["Total_PPN"].HeaderText = "Total PPN";
            dgvPA.Columns["Total_PPH"].HeaderText = "Total PPH";
            dgvPA.Columns["Deskripsi"].HeaderText = "Description";
        }

        private void TaskListPA_Load(object sender, EventArgs e)
        {
            addCmbStatusName();
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
            //Menampilkan data
            Conn = ConnectionString.GetConnection();

            // if (btnmode == String.Empty)
            // {
            //     btnmode = "Progress";
            // }
            //if (btnmode == "Progress")
            //{
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'02'";
                    //StClose = "0";
                }

                //Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where ";
                //Query += "StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0";
                //Query += ") a ";
                //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, h.DueDate, h.TransType, t.Deskripsi AS StatusName, h.CanvasId, h.PurchQuotID, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode Where ";
                Query += "(h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' ";
                if (TransStatus.Contains("05"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {

                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
                Query += ") a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.AgreementID DESC";
            }
            else if (crit.Equals("All"))
            {
                //Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where (h.AgreementID Like '%" + txtSearch.Text + "%' or TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' ";
                //Query += "or CurrencyID Like '%" + txtSearch.Text + "%' or ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%') And ";
                //Query += "StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0";
                //Query += ") a ";
                //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, h.DueDate, h.TransType, t.Deskripsi AS StatusName, h.CanvasId, h.PurchQuotID, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, h.PPN, h.Total_PPN, h.PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode ";
                Query += "Where ((h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or t.Deskripsi Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or h.PurchQuotID Like '%" + txtSearch.Text + "%' ";
                Query += "or h.VendID Like '%" + txtSearch.Text + "%') And ";
                Query += "h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or t.Deskripsi Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or h.PurchQuotID Like '%" + txtSearch.Text + "%' ";
                Query += "or h.VendID Like '%" + txtSearch.Text + "%') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
                Query += ") a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.AgreementID DESC ";
            }
            else if (crit.Equals("Status Name"))
            {
                // Query = "Select Count(AgreementID) From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID) No, h.AgreementID From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where (h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' ";
                // Query += "or CurrencyID Like '%" + txtSearch.Text + "%' or ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%') And StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0 ) a ;";
                string StatusName = string.Empty;
                if (Convert.ToString(cmbStatusName.SelectedItem) == "All")
                {
                    StatusName = "";
                }
                else
                {
                    StatusName = Convert.ToString(cmbStatusName.SelectedItem);
                }
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, h.DueDate, h.TransType, t.Deskripsi AS StatusName, h.CanvasId, h.PurchQuotID, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, h.PPN, h.Total_PPN, h.PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode ";
                Query += "Where ((t.Deskripsi Like '%" + StatusName + "%') And ";
                Query += "h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(t.Deskripsi Like '%" + StatusName + "%') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
                Query += ") a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.AgreementID DESC ";
            }
            else if (crit.Equals("Order Date"))
            {
                //Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID where ";
                //Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (h.AgreementID Like '%" + txtSearch.Text + "%' or TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or CurrencyID Like '%" + txtSearch.Text + "%' or ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%' ) And ";
                //Query += "StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0";
                //Query += ") a ";
                //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, h.DueDate, h.TransType, t.Deskripsi AS StatusName, h.CanvasId, h.PurchQuotID, h.CurrencyID, h.ExchRate, h.VendID, h.Total, h.Total_Disk, h.PPN, h.Total_PPN, h.PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode where( ";
                Query += "(CONVERT(VARCHAR(10),h.OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(CONVERT(VARCHAR(10),h.OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
                Query += ") a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.AgreementID DESC ";
            }
            else if (crit.Equals("Due Date"))
            {
                //Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID where ";
                //Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (h.AgreementID Like '%" + txtSearch.Text + "%' or TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or CurrencyID Like '%" + txtSearch.Text + "%' or ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%' ) And ";
                //Query += "StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0";
                //Query += ") a ";
                //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, h.DueDate, h.TransType, t.Deskripsi AS StatusName, h.CanvasId, h.PurchQuotID, h.CurrencyID, h.ExchRate, h.VendID, h.Total, h.Total_Disk, h.PPN, h.Total_PPN, h.PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode where( ";
                Query += "(CONVERT(VARCHAR(10),h.DueDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.DueDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                Query += "h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(CONVERT(VARCHAR(10),h.DueDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.DueDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
                Query += ") a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.AgreementID DESC ";
            }
            else
            {
                if (crit.Equals("PA ID"))
                {
                    crit = "AgreementID";
                }
                if (crit.Equals("Transaction Type"))
                {
                    crit = "TransType";
                }
                if (crit.Equals("CS ID"))
                {
                    crit = "CanvasId";
                }
                if (crit.Equals("PQ ID"))
                {
                    crit = "PurchQuotID";
                }
                if (crit.Equals("Vendor ID"))
                {
                    crit = "VendID";
                }

                //Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where";
                //Query += crit + " Like '%" + txtSearch.Text + "%' And ";
                //Query += "StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0";
                //Query += ") a ";
                //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID Desc) No, h.AgreementID, h.OrderDate, h.DueDate, h.TransType, t.Deskripsi AS StatusName, h.CanvasId, h.PurchQuotID, h.CurrencyID, h.ExchRate, h.VendID, h.Total, h.Total_Disk, h.PPN, h.Total_PPN, h.PPH, h.Total_PPH, h.Deskripsi From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode Where( ";
                Query += crit + " Like '%" + txtSearch.Text + "%' And ";
                Query += "h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += crit + " Like '%" + txtSearch.Text + "%'";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
                Query += ") a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ORDER BY a.AgreementID DESC  ";
            }
        
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            DataGridViewButtonColumn buttonpreview = new DataGridViewButtonColumn();
            buttonpreview.Name = "Preview";
            buttonpreview.HeaderText = "Preview";
            buttonpreview.Text = "Preview";
            buttonpreview.UseColumnTextForButtonValue = true;
            //Dt.Columns.Add(new DataColumn("colStatus", typeof(System.Windows.Forms.Button)));

            DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            buttonSend.Name = "Send Email";
            buttonSend.HeaderText = "Send Email";
            buttonSend.Text = "Send Email";
            buttonSend.UseColumnTextForButtonValue = true;

            dgvPA.AutoGenerateColumns = true;
            dgvPA.DataSource = Dt;
            gvHeader();
            dgvPA.Refresh();
            dgvPA.AutoResizeColumns();
            dgvPA.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPA.Columns["DueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            if (!dgvPA.Columns.Contains("Preview"))
                dgvPA.Columns.Add(buttonpreview);
            if (!dgvPA.Columns.Contains("Send Email"))
                dgvPA.Columns.Add(buttonSend);
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            // if (btnmode == "Progress")
            // {
            if (crit == null)
            {
                //Query = "Select Count(AgreementID) From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID) No, h.AgreementID From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0) a ;";
                Query = "Select Count(h.AgreementID) From [dbo].[PurchAgreementH] h Where( h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) ";
                }
            }
            else if (crit.Equals("All"))
            {
                // Query = "Select Count(AgreementID) From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID) No, h.AgreementID From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where (h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' ";
                // Query += "or CurrencyID Like '%" + txtSearch.Text + "%' or ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%') And StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0 ) a ;";

                Query = "Select Count(h.AgreementID) From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode Where( (h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or t.Deskripsi Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or h.PurchQuotID Like '%" + txtSearch.Text + "%' ";
                Query += "or h.VendID Like '%" + txtSearch.Text + "%') AND h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or t.Deskripsi Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or h.PurchQuotID Like '%" + txtSearch.Text + "%' ";
                Query += "or h.VendID Like '%" + txtSearch.Text + "%') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
            }
            else if (crit.Equals("Status Name"))
            {
                // Query = "Select Count(AgreementID) From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID) No, h.AgreementID From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where (h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' ";
                // Query += "or CurrencyID Like '%" + txtSearch.Text + "%' or ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%') And StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0 ) a ;";
                string StatusName = string.Empty;
                if (Convert.ToString(cmbStatusName.SelectedItem) == "All")
                {
                    StatusName = "";
                }
                else
                {
                    StatusName = Convert.ToString(cmbStatusName.SelectedItem);
                }
                Query = "Select Count(h.AgreementID) From [dbo].[PurchAgreementH] h Left JOIN TransStatusTable t ON h.TransStatus = t.StatusCode Where( (t.Deskripsi Like '%" + StatusName + "%') AND h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(t.Deskripsi Like '%" + StatusName + "%') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) AND t.TransCode = 'PA' ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) AND t.TransCode = 'PA' ";
                }
            }
            else if (crit.Equals("Order Date"))
            {
                //Query = "Select Count(AgreementID) From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID) No, h.AgreementID From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where ";
                //Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or h.CurrencyID Like '%" + txtSearch.Text + "%' or h.ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%' ) And StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0) a ;";

                Query = "Select Count(h.AgreementID) From [dbo].[PurchAgreementH] h Where( ";
                Query += "(CONVERT(VARCHAR(10),h.OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(CONVERT(VARCHAR(10),h.OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) ";
                }
            }
            else if (crit.Equals("Due Date"))
            {
                //Query = "Select Count(AgreementID) From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID) No, h.AgreementID From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where ";
                //Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (h.AgreementID Like '%" + txtSearch.Text + "%' or h.TransType Like '%" + txtSearch.Text + "%' or h.CanvasId Like '%" + txtSearch.Text + "%' or h.CurrencyID Like '%" + txtSearch.Text + "%' or h.ExchRate Like '%" + txtSearch.Text + "%' or VendID Like '%" + txtSearch.Text + "%' or h.Deskripsi Like '%" + txtSearch.Text + "%' ) And StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0) a ;";

                Query = "Select Count(h.AgreementID) From [dbo].[PurchAgreementH] h Where( ";
                Query += "(CONVERT(VARCHAR(10),h.DueDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.DueDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += "(CONVERT(VARCHAR(10),h.DueDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),h.DueDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                if (TransStatus.Contains("02"))
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) ";
                }
            }
            else
            {
                // Query = "Select Count(AgreementID) From (Select ROW_NUMBER() OVER (ORDER BY h.AgreementID) No, h.AgreementID From [dbo].[PurchAgreementH] h Left JOIN PurchAgreementDtl d ON h.AgreementID = d.AgreementID Where ";
                //Query += crit + " Like '%" + txtSearch.Text + "%' And StClose = 0 and d.Base='Y' group by h.AgreementID, h.OrderDate, DueDate, TransType, h.CanvasId, CurrencyID, ExchRate, VendID, h.Total, h.Total_Disk, PPN, h.Total_PPN, PPH, h.Total_PPH, h.Deskripsi having SUM(d.RemainingQty) >0) a ";
                if (crit.Equals("PA ID"))
                {
                    crit = "AgreementID";
                }
                if (crit.Equals("Transaction Type"))
                {
                    crit = "TransType";
                }
                if (crit.Equals("CS ID"))
                {
                    crit = "CanvasId";
                }
                if (crit.Equals("PQ ID"))
                {
                    crit = "PurchQuotID";
                }
                if (crit.Equals("Vendor ID"))
                {
                    crit = "VendID";
                }

                Query = "Select Count(h.AgreementID) From [dbo].[PurchAgreementH] h Where( ";
                Query += crit + " Like '%" + txtSearch.Text + "%' AND h.TransStatus IN (" + TransStatus + ") ";
                Query += "OR h.AgreementID IN ";
                Query += "(SELECT d.AgreementID ";
                Query += "FROM PurchAgreementDtl d WHERE h.TransStatus = '02' AND ";
                Query += crit + " Like '%" + txtSearch.Text + "%'";
                if (TransStatus.Contains("02"))
                {

                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0)) ";
                }
                else
                {
                    Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) > 0)) ";
                }

            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            if (Page2 == 0)
            {
                Page2 = 1;
            }
            lblPage.Text = "/ " + Page2;
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchAgreementH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void addCmbStatusName()
        {
            cmbStatusName.Items.Clear();
            cmbStatusName.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select Deskripsi From TransStatusTable WHERE TransCode = 'PA' And StatusCode <> '01'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbStatusName.Items.Add(Dr[0]);
            }
            cmbStatusName.SelectedIndex = 0;
            Conn.Close();
        }

        public void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            //cmbCriteria.SelectedIndex = 0;

            RefreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > Convert.ToInt32((decimal)Total / Int32.Parse(cmbShow.Text)))
            {
                txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            }
            if (Convert.ToInt32(txtPage.Text) < 1)
            {
                txtPage.Text = "1";
            }
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 2) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text);
                if (Convert.ToInt32(txtPage.Text) > 1)
                {
                    txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
                }
            }
            RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > Convert.ToInt32((decimal)Total / Int32.Parse(cmbShow.Text)))
            {
                txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            }
            if (Convert.ToInt32(txtPage.Text) < 1)
            {
                txtPage.Text = "1";
            }
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 = (Int32.Parse(txtPage.Text)) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text) + 1) * Int32.Parse(cmbShow.Text);
                if (Convert.ToInt32(txtPage.Text) < Convert.ToInt32((decimal)Total / Int32.Parse(cmbShow.Text)))
                {
                    txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
                }
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
            cmbStatusName.SelectedIndex = 0;
            ModeLoad();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
                txtSearch.Text = "";
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }

            if (cmbCriteria.Text.Contains("StatusName"))
            {
                cmbStatusName.Enabled = true;
                txtSearch.Text = "";
            }
            else
            {
                cmbStatusName.Enabled = false;
                cmbStatusName.SelectedIndex = 0;
            }

            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("StatusName"))
            {
                txtSearch.Enabled = false;
            }

            else
            {
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvPA_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           ISBS_New.Purchase.PurchaseAgreement.PAForm PAForm = new ISBS_New.Purchase.PurchaseAgreement.PAForm();

            if (PAForm.PermissionAccess(ControlMgr.View) > 0)
            {
                String AgreementId = dgvPA.CurrentRow.Cells["AgreementId"].Value.ToString();
                PAForm.SetParent2(this);
                PAForm.SetMode("View", "", AgreementId);
                PAForm.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }   
        }

        private void btnAmmend_Click(object sender, EventArgs e)
        {
           ISBS_New.Purchase.PurchaseAgreement.PAForm PAForm = new ISBS_New.Purchase.PurchaseAgreement.PAForm();
            if (PAForm.PermissionAccess(ControlMgr.New) > 0)
            {
                String AgreementId = dgvPA.CurrentRow.Cells["AgreementId"].Value.ToString();

                Conn = ConnectionString.GetConnection();
                Query = "Select StClose From PurchAgreementH Where AgreementId = '" + AgreementId + "'";
                Cmd = new SqlCommand(Query, Conn);

                if ((bool)Cmd.ExecuteScalar() == true)
                {
                    Conn.Close();
                    MessageBox.Show("Purchase Agreement telah selesai.");
                    return;
                }

                Query = "Select TransStatus From PurchAgreementH Where AgreementId = '" + AgreementId + "'";
                Cmd = new SqlCommand(Query, Conn);

                if (Convert.ToString(Cmd.ExecuteScalar()) != "03")
                {
                    Conn.Close();
                    MessageBox.Show("Purchase Agreement belum di approve.");
                    return;
                }

                Query = "SELECT COUNT(d.AgreementID) FROM PurchAgreementH h ";
                Query += "INNER JOIN PurchAgreementDtl d ";
                Query += "ON d.AgreementID = h.AgreementID ";
                Query += "WHERE h.TransStatus = '03' AND h.AgreementID = '" + AgreementId + "' ";
                Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0 ";
                Cmd = new SqlCommand(Query, Conn);

                if (Convert.ToInt32(Cmd.ExecuteScalar()) != 0)
                {
                    Conn.Close();
                    MessageBox.Show("Purchase Agreement telah selesai.");
                    return;
                }

                Conn.Close();


                PAForm.SetParent2(this);
                PAForm.SetMode("Ammend", "", AgreementId);
                PAForm.oldPAID = AgreementId;
                PAForm.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (timerRefresh == null)
            {

            }
            else
            {
                // RefreshGrid();
            }
        }

        private void TaskListPA_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu f = new MainMenu();
            f.refreshTaskList();
            timerRefresh = null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvPA.RowCount > 0)
                    {
                        Index = dgvPA.CurrentRow.Index;
                        string AgreementID = dgvPA.Rows[Index].Cells["AgreementID"].Value == null ? "" : dgvPA.Rows[Index].Cells["AgreementID"].Value.ToString();
                        //string TransType = dgvPR.Rows[Index].Cells["TransType"].Value == null ? "" : dgvPR.Rows[Index].Cells["TransType"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        Conn = ConnectionString.GetConnection();
                        Trans = Conn.BeginTransaction();

                        Query = "SELECT COUNT(*) COUNTDATA FROM PurchH WHERE UPPER(ReffTableName) = 'PA' AND ReffId = '" + AgreementID + "';";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountDataPO = Convert.ToInt32(Cmd.ExecuteScalar());

                        Query = "SELECT COUNT(*) COUNTDATA FROM PurchAgreementH WHERE RefTransId ='" + AgreementID + "';";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountDataAmmend = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountDataAmmend == 0 && CountDataPO == 0)
                        {
                            DialogResult dr = MessageBox.Show("PurchAgreement ID = " + AgreementID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                Query = "UPDATE PurchAgreementDtl SET DeletedDate = getdate(), DeletedBy = '" + ControlMgr.UserId + "' WHERE AgreementID = '" + AgreementID + "';";
                                Query += "UPDATE PurchAgreementH SET TransStatus = 'XX', DeletedDate = getdate(), DeletedBy = '" + ControlMgr.UserId + "' WHERE AgreementID = '" + AgreementID + "';";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show("PurchAgreementID = " + AgreementID.ToUpper() + "\n" + "tidak dapat dihapus karena sudah diproses.");
                            return;
                        }

                        Trans.Commit();
                        MessageBox.Show("PurchAgreementID = " + AgreementID.ToUpper() + "\n" + "Data berhasil dihapus.");
                        RefreshGrid();
                                         
                    }
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
        }

        private int CheckStatus(string prmAgreementID)
        {
            int result = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(h.AgreementID) FROM PurchAgreementH H INNER JOIN PurchAgreementDtl D ";
            Query += "ON D.AgreementID = H.AgreementID ";
            Query += "WHERE H.AgreementID = '" + prmAgreementID + "' AND (H.StClose = 1 OR (D.Qty <>  D.RemainingQty))";
            Cmd = new SqlCommand(Query, Conn);
            result = Convert.ToInt32(Cmd.ExecuteScalar());
            Conn.Close();

            return result;
        }

        private string GetRefTransID(string prmAgreementID)
        {
            string result = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT RefTransId FROM PurchAgreementH WHERE AgreementID = '" + prmAgreementID + "'";
            Cmd = new SqlCommand(Query, Conn);
            result = Convert.ToString(Cmd.ExecuteScalar());
            Conn.Close();

            return result;
        }

        private void dgvPA_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex > -1)
            {
                Conn = ConnectionString.GetConnection();
                string PAId = dgvPA.CurrentRow.Cells["AgreementID"].Value.ToString();

                Cmd = new SqlCommand("SELECT [VendId] FROM [PurchAgreementH] WHERE [AgreementID] = '" + PAId + "'", Conn);
                string VendId = Cmd.ExecuteScalar().ToString();

                if (dgvPA.Columns[e.ColumnIndex].Name == "Preview")
                {
                    DialogResult res = MessageBox.Show("Rinci / Tidak", "Rinci / Tidak", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.Yes)
                    {
                        Detail = "Rinci";
                    }
                    else
                    {
                        Detail = "Tidak Rinci";
                    }

                    //ISBS_New.Purchase.PurchaseAgreement.PreviewPA f = new ISBS_New.Purchase.PurchaseAgreement.PreviewPA(PAId, Detail);
                    //f.Show();

                    GlobalPreview f = new GlobalPreview("Purchase Agreement", PAId);
                    f.SetMode(Detail);
                    f.Show();
                }
                else if (dgvPA.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    DialogResult res = MessageBox.Show("Rinci / Tidak", "Rinci / Tidak", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (res == DialogResult.Yes)
                    {
                        Detail = "Rinci";
                    }
                    else
                    {
                        Detail = "Tidak Rinci";
                    }

                    Query = "Select EmailId From PurchAgreementH Where AgreementId = '" + PAId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    var emailid = Cmd.ExecuteScalar();

                    if (emailid == null)
                    {
                        MessageBox.Show("Email sudah pernah dikirim.");
                        return;
                    }

                    //ISBS_New.Purchase.PurchaseAgreement.SendEmailPA s = new ISBS_New.Purchase.PurchaseAgreement.SendEmailPA();
                    //s.flag(PAId, Detail);
                    //s.Show();

                    GlobalSendEmail f = new GlobalSendEmail("Purchase Agreement", PAId, VendId);
                    f.SetMode(Detail);
                    f.Show();
                }

                Conn.Close();
            }
        }

        public static string vendID;

        public string VendID { get { return vendID; } set { vendID = value; } }

        private void dgvPA_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvPA.Columns["Total"].Index || e.ColumnIndex == dgvPA.Columns["Total_Disk"].Index || e.ColumnIndex == dgvPA.Columns["Total_PPN"].Index || e.ColumnIndex == dgvPA.Columns["Total_PPH"].Index || e.ColumnIndex == dgvPA.Columns["ExchRate"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.0000";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                dgvPA.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N4");
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            //btnmode = "Progress";
            TransStatus = "'02'";
            // StClose = "0";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            // btnmode = "Completed";
            TransStatus = "'05', '06'";

            // StClose = "1";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }

        private void insertstatuslogDelete()
        {
            Index = dgvPA.CurrentRow.Index;
            string AgreementID = dgvPA.Rows[Index].Cells["AgreementID"].Value == null ? "" : dgvPA.Rows[Index].Cells["AgreementID"].Value.ToString();
            string VendorID = dgvPA.Rows[Index].Cells["VendID"].Value == null ? "" : dgvPA.Rows[Index].Cells["VendID"].Value.ToString();
            string CSID = dgvPA.Rows[Index].Cells["CanvasId"].Value == null ? "" : dgvPA.Rows[Index].Cells["CanvasId"].Value.ToString();
            string PQID = dgvPA.Rows[Index].Cells["PurchQuotID"].Value == null ? "" : dgvPA.Rows[Index].Cells["PurchQuotID"].Value.ToString();

            Query = "INSERT INTO [dbo].[StatusLog_Vendor] VALUES "; //[StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]
            Query += " ('PAInquiry', '" + AgreementID + "', '" + VendorID + "', '" + CSID + "', '" + PQID + "', 'XX', 'PA Data Deleted', '" + ControlMgr.UserId + "', getdate()) ";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            ISBS_New.Purchase.PurchaseAgreement.PAForm PAForm = new ISBS_New.Purchase.PurchaseAgreement.PAForm();

            if (PAForm.PermissionAccess(ControlMgr.View) > 0)
            {
                String AgreementId = dgvPA.CurrentRow.Cells["AgreementId"].Value.ToString();
                PAForm.SetParent2(this);
                PAForm.SetMode("View", "", AgreementId);
                PAForm.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            } 
        }



    }
}
