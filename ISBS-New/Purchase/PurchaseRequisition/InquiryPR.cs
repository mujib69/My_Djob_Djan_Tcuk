using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.Purchase.PurchaseRequisition
{
    public partial class InquiryPR : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr, Dr2;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        public static int dataShow;
        string TransStatus = "";

        List<HeaderPR> ListHeaderPR = new List<HeaderPR>();

        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //timer autorefresh
        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10 * 1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        public InquiryPR()
        {
            InitializeComponent();
        }
        public void RefreshGrid()
        {
            //Menampilkan data
            if (TransStatus == String.Empty)
            {
                TransStatus = "'01','02','03','04','12'";
            }
            Conn = ConnectionString.GetConnection();

            #region inquery OLD
            //if (crit == null)
            //{
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, Case When (a.TransStatus<5 or a.TransStatus = 12 ) then 'ON-PROGRESS' else 'COMPLETED' end StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by joshua
            //    //updated date 08 Mar 2018
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, b.Deskripsi AS StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by steven
            //    Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, UPPER(b.Deskripsi) StatusName, a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate ";
            //    Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate From [dbo].[PurchRequisitionH] Where TransStatus in (" + TransStatus + ")) a ";
            //    Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            //}
            //else if (crit.Equals("All"))
            //{               
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, Case When (a.TransStatus<5 or a.TransStatus = 12 ) then 'ON-PROGRESS' else 'COMPLETED' end StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                
            //    //updated by joshua
            //    //updated date 08 Mar 2018
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, b.Deskripsi AS StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by steven
            //    Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, UPPER(b.Deskripsi) StatusName, a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate ";
            //    Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate From [dbo].[PurchRequisitionH] where ";
            //    Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            //}
            //else if (crit.Equals("PR Date"))
            //{
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, Case When (a.TransStatus<5 or a.TransStatus = 12 ) then 'ON-PROGRESS' else 'COMPLETED' end StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by joshua
            //    //updated date 08 Mar 2018
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, b.Deskripsi AS StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by steven
            //    Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus,  UPPER(b.Deskripsi) StatusName,a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate ";
            //    Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate From [dbo].[PurchRequisitionH] where ";
            //    Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            //}
            //else if (crit.Equals("Created Date"))
            //{                
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, Case When (a.TransStatus<5 or a.TransStatus = 12 ) then 'ON-PROGRESS' else 'COMPLETED' end StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by joshua
            //    //updated date 08 Mar 2018
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, b.Deskripsi AS StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by steven
            //    Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus,  UPPER(b.Deskripsi) StatusName,a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate ";
            //    Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate From [dbo].[PurchRequisitionH] where ";
            //    Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            //}
            //else if (crit.Equals("Updated Date"))
            //{
            //    //updated by steven
            //    Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus,  UPPER(b.Deskripsi) StatusName,a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate ";
            //    Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate From [dbo].[PurchRequisitionH] where ";
            //    Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') And TransStatus in (" + TransStatus + ")) a ";
            //    Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            //}
            //else
            //{
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, Case When (a.TransStatus<5 or a.TransStatus = 12 ) then 'ON-PROGRESS' else 'COMPLETED' end StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += crit + " Like '%" + txtSearch.Text + "%' And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by joshua
            //    //updated date 08 Mar 2018
            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, b.Deskripsi AS StatusName, a.CreatedDate, a.CreatedBy ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] where ";
            //    //Query += crit + " Like '%" + txtSearch.Text + "%' And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    //updated by steven
            //    if (crit == "PR No")
            //    {
            //        crit = "PurchReqID";
            //    }
            //    else if (crit == "PR Type")
            //    {
            //        crit = "TransType";
            //    }
            //    else if (crit == "Status")
            //    {
            //        //crit = "TransStatus";
            //        crit = "b.Deskripsi";
            //    }
            //    else if (crit == "Created By")
            //    {
            //        crit = "CreatedBy";
            //    }
            //    else if (crit == "Updated By")
            //    {
            //        crit = "UpdatedBy";
            //    }

            //    //Query = "Select a.[No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, UPPER(b.Deskripsi) StatusName, a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate  ";
            //    //Query += "From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID desc) [No], PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate From [dbo].[PurchRequisitionH] where ";
            //    //Query += crit + " Like '%" + txtSearch.Text + "%' And TransStatus in (" + TransStatus + ")) a ";
            //    //Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

            //    Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY PurchReqID DESC) [No], a.PurchReqID, a.OrderDate, a.TransType, a.TransStatus, UPPER(b.Deskripsi) StatusName, a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate ";
            //    Query += "FROM PurchRequisitionH a LEFT JOIN TransStatusTable b ON a.TransStatus = b.StatusCode and TransCode='PR' ";
            //    Query += "WHERE " + crit + " LIKE '%" + txtSearch.Text + "%' AND TransStatus IN (" + TransStatus + ")) a WHERE a.No BETWEEN " + Limit1 + " AND " + Limit2 + " ; ";
            //}
            #endregion

            //update start: Mujib 08/10/2018
            //Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY PurchReqID DESC) [No], PRH.PurchReqID, PRH.OrderDate, PRH.TransType, ";
            //Query += "PRH.TransStatus, UPPER(TST.Deskripsi) StatusName, PRH.CreatedBy, PRH.CreatedDate, PRH.UpdatedBy, PRH.UpdatedDate ";
            //Query += "FROM PurchRequisitionH PRH LEFT JOIN TransStatusTable TST ON PRH.TransStatus = TST.StatusCode AND TST.TransCode = 'PR' ";
            //Query += "WHERE TransStatus IN (" + TransStatus + ") ";


            //hasim 10 okt 2018
            int mflag;
            String addquery = null;

            Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY PurchReqID DESC) [No], PRH.PurchReqID, PRH.OrderDate, PRH.TransType, ";
            Query += "PRH.TransStatus, UPPER(TST.Deskripsi) StatusName, cr.FullName[CreatedBy], PRH.CreatedDate, up.FullName[UpdatedBy], PRH.UpdatedDate ";
            Query += "FROM PurchRequisitionH PRH LEFT JOIN TransStatusTable TST ON PRH.TransStatus = TST.StatusCode AND TST.TransCode = 'PR' ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON PRH.CreatedBy=cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON PRH.UpdatedBy=up.UserID WHERE TransStatus IN (" + TransStatus + ") ";

            addquery = "AND (PurchReqId LIKE @search OR TransType LIKE @search OR TST.Deskripsi LIKE @search OR cr.FullName LIKE @search OR up.FullName LIKE @search) ";
            mflag = 1;
            if (crit == null)
            {
                Query += "";
                mflag = 0;
            }
            else if(crit.Equals("All"))
            {
                Query += addquery;
            }
            else if (crit.Equals("PR No"))
            {
                Query += "AND PurchReqId LIKE @search";
            }
            else if (crit.Equals("PR Date"))
            {
                Query += addquery + "AND (OrderDate BETWEEN @from AND @to) ";
                mflag = 2;
            }
            else if (crit.Equals("PR Type"))
            {
                Query += "AND TransType LIKE @search";
            }
            else if (crit.Equals("Status"))
            {
                Query += "AND TST.Deskripsi LIKE @search";
            }
            else if (crit.Equals("Created Date"))
            {
                Query += addquery + "AND (CreatedDate BETWEEN @from AND @to) ";
                mflag = 2;
            }
            else if (crit.Equals("Created By"))
            {
                Query += "AND cr.FullName LIKE @search";
            }
            else if (crit.Equals("Updated By"))
            {
                Query += "AND up.FullName LIKE @search";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query += addquery + "AND (UpdatedDate BETWEEN @from AND @to) ";
                mflag = 2;
            }
            Query += ") A WHERE [No] BETWEEN @limit1 AND @limit2 ;" ;
            //update end

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@limit1", Limit1);
            Da.SelectCommand.Parameters.AddWithValue("@limit2", Limit2);
            if (mflag > 0)
            {
                Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                if (mflag == 2)
                {
                    Da.SelectCommand.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd") + " 00:00:00");
                    Da.SelectCommand.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd") + " 23:59:59");
                }
            }
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPR.AutoGenerateColumns = true;
            dgvPR.DataSource = Dt;
            dgvPR.Refresh();
            dgvPR.AutoResizeColumns();
            dgvPR.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvPR.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvPR.Columns["TransStatus"].Visible = false;
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            #region paging OLD
            //if (crit == null)
            //{
            //    Query = "Select Count(PurchReqID) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where TransStatus in (" + TransStatus + ") ) a ;";
            //}
            //else if (crit.Equals("All"))
            //{
            //    Query = "Select Count(PurchReqID) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
            //    Query += "(PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') and TransStatus in (" + TransStatus + ")) a ;";
            //}
            //else if (crit.Equals("PR Date"))
            //{
            //    Query = "Select Count(PurchReqID) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
            //    Query += "(CONVERT(VARCHAR(10),OrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),OrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') and TransStatus in (" + TransStatus + ")) a ;";
            //}
            //else if (crit.Equals("Created Date"))
            //{
            //    Query = "Select Count(PurchReqID) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
            //    Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') and TransStatus in (" + TransStatus + ")) a ;";
            //}
            //else if (crit.Equals("Updated Date"))
            //{
            //    Query = "Select Count(PurchReqID) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
            //    Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (PurchReqID like '%" + txtSearch.Text + "%' or TransType like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%'  or ApprovedBy like '%" + txtSearch.Text + "%') and TransStatus in (" + TransStatus + ")) a ;";
            //}
            //else
            //{
            //    //updated by steven
            //    if (crit == "PR No")
            //    {
            //        crit = "PurchReqID";
            //    }
            //    else if (crit == "PR Type")
            //    {
            //        crit = "TransType";
            //    }
            //    else if (crit == "Status")
            //    {
            //        //crit = "TransStatus";
            //        crit = "b.Deskripsi";
            //    }
            //    else if (crit == "Created By")
            //    {
            //        crit = "CreatedBy";
            //    }
            //    else if (crit == "Updated By")
            //    {
            //        crit = "Updated By";
            //    }

            //    //Query = "Select Count(PurchReqID) From (Select ROW_NUMBER() OVER (ORDER BY PurchReqID) No, PurchReqID, OrderDate, TransType, TransStatus, CreatedDate, CreatedBy From [dbo].[PurchRequisitionH] Where ";
            //    //Query += crit + " Like '%" + txtSearch.Text + "%' and TransStatus in (" + TransStatus + ")) a ";

            //    Query = "SELECT Count(PurchReqID) FROM PurchRequisitionH a LEFT JOIN TransStatusTable b on a.TransStatus = b.StatusCode AND TransCode='PR' ";
            //    Query += "WHERE " + crit + " LIKE '%" + txtSearch.Text + "%' AND TransStatus IN (" + TransStatus + ");  ";
            //}

            //Query = "";

            //update start: Mujib 08/10/2018
            //Query = "SELECT COUNT(PurchReqID) FROM PurchRequisitionH PRH LEFT JOIN TransStatusTable TST ON PRH.TransStatus = TST.StatusCode AND TST.TransCode = 'PR' WHERE TransStatus IN (" + TransStatus + ") ";
            
            #endregion

            Query = "SELECT COUNT(PurchReqID) FROM PurchRequisitionH PRH ";
            Query += "LEFT JOIN TransStatusTable TST ON PRH.TransStatus = TST.StatusCode AND TST.TransCode = 'PR' ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON PRH.CreatedBy=cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON PRH.UpdatedBy=up.UserID WHERE TransStatus IN (" + TransStatus + ") ";
            
            
            if (crit == null)
            {
                Query += "";
            }
            else if (crit.Equals("All"))
            {
                Query += addquery;
            }
            else if (crit.Equals("PR No"))
            {
                Query += "AND PurchReqId LIKE @search";
            }
            else if (crit.Equals("PR Date"))
            {
                Query += addquery + "AND (OrderDate BETWEEN @from AND @to) ";
            }
            else if (crit.Equals("PR Type"))
            {
                Query += "AND TransType LIKE @search";
            }
            else if (crit.Equals("Status"))
            {
                Query += "AND TST.Deskripsi LIKE @search";
            }
            else if (crit.Equals("Created Date"))
            {
                Query += addquery + "AND (CreatedDate BETWEEN @from AND @to) ";
            }
            else if (crit.Equals("Created By"))
            {
                Query += "AND cr.FullName LIKE @search";
            }
            else if (crit.Equals("Updated By"))
            {
                Query += "AND up.FullName LIKE @search";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query += addquery + "AND (UpdatedDate BETWEEN @from AND @to) ";
            }
            
            Cmd = new SqlCommand(Query, Conn);
            if (mflag > 0)
            {
                Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                if (mflag == 2)
                {
                    Cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd") + " 00:00:00");
                    Cmd.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd") + " 23:59:59");
                }
            }
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        public int DataShow { get { return dataShow; } set { dataShow = value; } }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchRequisitionH'";

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
                Limit1 = (Int32.Parse(txtPage.Text)-2) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = (Int32.Parse(txtPage.Text)-1) * Int32.Parse(cmbShow.Text);
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
                Limit2 = (Int32.Parse(txtPage.Text)+1) * Int32.Parse(cmbShow.Text);
                if (Convert.ToInt32(txtPage.Text) < ((decimal)Total / Int32.Parse(cmbShow.Text)))
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

        //hasim update, maksimal page tidak boleh lebih dari page yang ada
        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Int32.Parse(txtPage.Text) <= Page2)
            {
                if (e.KeyChar == (char)13)
                {
                    Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                    Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                    RefreshGrid();
                }
            }
            else
            {
                txtPage.Text = Page2.ToString();
                if (e.KeyChar == (char)13)
                {
                    Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                    Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                    RefreshGrid();
                }
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin old code
            //if (ControlMgr.GroupName != "Purchasing Manager")
            //{
            //    HeaderPR HeaderPR = new HeaderPR();
            //    //header.flag("", "New");
            //    ListHeaderPR.Add(HeaderPR);
            //    HeaderPR.SetMode("New", "");
            //    HeaderPR.SetParent(this);
            //    HeaderPR.Show();
            //    RefreshGrid();
            //}
            //else
            //{
            //    MessageBox.Show("User Group : Purchasing Manager \nTidak bisa melakukan create PR.");
            //}
            //end old code

            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                HeaderPR HeaderPR = new HeaderPR();
                //header.flag("", "New");
                ListHeaderPR.Add(HeaderPR);
                HeaderPR.SetMode("New", "");
                HeaderPR.SetParent(this);
                HeaderPR.Show();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end


        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPR();
        }

        //HENDRY VALIDASI
        private Boolean ValidGeneral()
        {
            Boolean vBol = true;
            string PRID = "";
            string TransStatus = "";

            if (dgvPR.RowCount > 0)
            {
                PRID = dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString();
                Conn = ConnectionString.GetConnection();
                Query = "Select TransStatus from [dbo].[PurchRequisitionH] where [PurchReqID]='" + PRID + "'";
                Cmd = new SqlCommand(Query, Conn);
                TransStatus = Cmd.ExecuteScalar().ToString();
                if (TransStatus != "01" && TransStatus != "02" && TransStatus != "12")
                {
                    vBol = false;
                    MessageBox.Show("PurchReqID = " + PRID + ".\n" + "Tidak bisa delete karena sudah diproses.");
                }
                Conn.Close();
            }                      
            return vBol;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvPR.RowCount > 0)
                    {
                        if (dgvPR.CurrentRow.Cells["StatusName"].Value.ToString().ToUpper() != "REQUEST – WAITING FOR APPROVAL")
                        {
                            MessageBox.Show("Maaf dokumen Purchase Requisition tidak bisa di Cancel.");
                            return;
                        }

                        Index = dgvPR.CurrentRow.Index;
                        string PurchReqID = dgvPR.Rows[Index].Cells["PurchReqID"].Value == null ? "" : dgvPR.Rows[Index].Cells["PurchReqID"].Value.ToString();
                        string TransType = dgvPR.Rows[Index].Cells["TransType"].Value == null ? "" : dgvPR.Rows[Index].Cells["TransType"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        DialogResult dr = MessageBox.Show("PurchReqID = " + PurchReqID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string Check = "";
                            Conn = ConnectionString.GetConnection();

                            Query = "Select TransStatus from [dbo].[PurchRequisitionH] where [PurchReqID]='" + dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString() + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Check = Cmd.ExecuteScalar().ToString();
                            if (Check != "01")
                            {
                                MessageBox.Show("PurchReqID = " + dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                                Conn.Close();
                                return;
                            }


                            Conn = ConnectionString.GetConnection();

                            #region Delete Invent_Purch_Qty
                            #region variable
                            string FullItemId = "";
                            string Unit = "";
                            string UoM = "";
                            decimal ConvRatio = 0;
                            string QueryTemp = "";
                            decimal QtyDeleted = 0;
                            decimal QtyUoMDeleted = 0;
                            decimal QtyAltDeleted = 0;
                            string PRType = "";
                            int JumlahPR = 0;
                            string PRNo = "";
                            #endregion

                            PRType = dgvPR.CurrentRow.Cells["TransType"].Value == null ? "" : dgvPR.CurrentRow.Cells["TransType"].Value.ToString();
                            PRNo = dgvPR.CurrentRow.Cells["PurchReqID"].Value == null ? "" : dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString();

                            Query = "EXEC [dbo].[stock_pr] @pr_id, @amount_or_qty, 'delete'  ; ";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.AddWithValue("@pr_id", PurchReqID);
                            Cmd.Parameters.AddWithValue("@amount_or_qty", PRType);
                            Cmd.ExecuteNonQuery();

                            Query = "SELECT COUNT (PurchReqId) FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + PRNo + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            JumlahPR = (int)Cmd.ExecuteScalar();

                            if (PRType != "AMOUNT")
                            {
                                #region NOT AMOUNT
                                for (int x = 1; x <= JumlahPR; x++)
                                {
                                    QueryTemp = "Select FullItemID, Qty, Unit From PurchRequisition_Dtl Where PurchReqID = '" + PRNo + "' and [SeqNo] = '" + x + "'";
                                    Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                                    Dr = Cmd.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        FullItemId = Dr["FullItemID"].ToString();
                                        Unit = Dr["Unit"].ToString();
                                        QtyDeleted = decimal.Parse(Dr["Qty"].ToString());
                                        ConvRatio = 0;

                                        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                                        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                                        UoM = Cmd.ExecuteScalar().ToString();

                                        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                                        Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                                        ConvRatio = (decimal)Cmd.ExecuteScalar();

                                        if (Unit == UoM)
                                        {
                                            QtyUoMDeleted = QtyDeleted;
                                            QtyAltDeleted = QtyDeleted * ConvRatio;
                                        }
                                        else
                                        {
                                            QtyAltDeleted = QtyDeleted;
                                            QtyUoMDeleted = QtyDeleted / ConvRatio;
                                        }

                                        //Query = "Update Invent_Purchase_Qty Set PR_Issued_UoM = PR_Issued_UoM - " + QtyUoMDeleted + ", PR_Issued_Alt = PR_Issued_Alt - " + QtyAltDeleted + "  Where FullItemID = '" + FullItemId + "'";
                                        //Cmd = new SqlCommand(Query, Conn, Trans);
                                        //Cmd.ExecuteNonQuery();
                                        Query = "";
                                    }
                                    Dr.Close();
                                }
                                #endregion
                            }

                            if (PRType == "AMOUNT")
                            {
                                #region AMOUNT
                                for (int x = 1; x <= JumlahPR; x++)
                                {
                                    QueryTemp = "Select FullItemID, Amount, Unit From PurchRequisition_Dtl Where PurchReqID = '" + PRNo + "' and [SeqNo] = '" + x + "'";
                                    Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                                    Dr = Cmd.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        FullItemId = Dr["FullItemID"].ToString();
                                        QtyDeleted = decimal.Parse(Dr["Amount"].ToString());

                                        //Query = "Update Invent_Purchase_Qty Set PR_Issued_Amount = PR_Issued_Amount - " + QtyDeleted + " Where FullItemID = '" + FullItemId + "'";
                                        //Cmd = new SqlCommand(Query, Conn, Trans);
                                        //Cmd.ExecuteNonQuery();
                                        Query = "";
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            //QueryTemp = "Delete From [PurchRequisition_LogTable] where [PurchReqID]='" + dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString() + "'";
                            //Cmd = new SqlCommand(QueryTemp, Conn, Trans);
                            //Cmd.ExecuteNonQuery();

                            //delete detail

                            //if (TransType == "Fix")
                            //{
                            //    Query = "Delete from [dbo].[PurchRequisition_Dtl] where PurchReqID ='" + PurchReqID + "';";
                            //}
                            //else
                            //{
                            //    Query = "Delete from [dbo].[PurchRequisition_DtlDtl] where PurchReqID ='" + PurchReqID + "';";
                            //    Query += "Delete from [dbo].[PurchRequisition_Dtl] where PurchReqID ='" + PurchReqID + "';";
                            //}

                            //delete header
                            //Query += "Delete from [dbo].[PurchRequisitionH] where PurchReqID='" + PurchReqID + "';";

                            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                            Query = "UPDATE PurchRequisitionH SET TransStatus = '99' WHERE PurchReqId = '" + PurchReqID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("PurchReqID = " + PurchReqID.ToUpper() + "\n" + "Data berhasil dicancel.");

                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                            

                            Index = 0;
                            Conn.Close();
                            RefreshGrid();

                        }
                    }
                }
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
                finally { };
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end

            
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectPR();
        }

        private void SelectPR()
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            HeaderPR header = new HeaderPR();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPR.RowCount > 0)
                {
                    
                    header.SetMode("BeforeEdit", dgvPR.CurrentRow.Cells["PurchReqID"].Value.ToString());
                    header.SetParent(this);
                    header.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
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
            backtopageone();

            RefreshGrid();
        }

        private void backtopageone()
        {
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
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


        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }

        }

        private void InquiryPR_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
            for (int i = 0; i < ListHeaderPR.Count(); i++)
            {
                ListHeaderPR[i].Close();
            }
        }

        private void InquiryPR_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            //setTimer();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            gvheader();
        }

        private void gvheader()
        {
            dgvPR.Columns["PurchReqId"].HeaderText = "PR No";
            dgvPR.Columns["OrderDate"].HeaderText = "PR Date";
            dgvPR.Columns["TransType"].HeaderText = "PR Type";
            //dgvPR.Columns["TransStatus"].HeaderText = "PR Status";
            dgvPR.Columns["StatusName"].HeaderText = "PR Status";           
            //dgvPR.Columns["StatusName"].HeaderText = "Status Name";
            dgvPR.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvPR.Columns["CreatedBy"].HeaderText = "Created By";
            dgvPR.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvPR.Columns["UpdatedBy"].HeaderText = "Updated By";
        }

        private void InquiryPR_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (txtSearch.Text == null || txtSearch.Text.Equals(""))
                {
                    MessageBox.Show("Masukkan Kata Kunci");
                }
                else if (cmbCriteria.SelectedIndex == -1)
                {
                    crit = "All";
                }
                else
                {
                    crit = cmbCriteria.SelectedItem.ToString();
                }
                backtopageone();
                RefreshGrid();
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01','02','03','04','12'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            backtopageone();
            RefreshGrid();            
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'05','13','14','15','21','22','99','33'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            backtopageone();
            RefreshGrid();
        }
    }
}
