using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.Purchase.ReceiptOrder
{
    public partial class InquiryReceiptOrder : MetroFramework.Forms.MetroForm
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
        public static int dataShow;
        string TransStatus = "";

        List<HeaderReceiptOrder> ListHeaderReceiptOrder = new List<HeaderReceiptOrder>();

        Purchase.ReceiptOrder.HeaderReceiptOrder Child;

        //begin
        //created by : joshua
        //created date : 22 feb 2018
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

        public InquiryReceiptOrder()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01','02', '04','09'";
                }
                Query = "Select * From(Select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) ";
                Query += "No, ReceiptOrderId 'ReceiptNumber', ReceiptOrderDate 'ReceiptDate', DeliveryDate,a.PurchaseOrderId, VendId 'VendorId', VendorName 'VendorName', InventSiteName, InventSiteID 'WarehouseCode', ReceiptOrderStatus, b.Deskripsi, a.CreatedDate, a.CreatedBy, CASE WHEN a.UpdatedBy = '' then null else a.UpdatedDate END UpdatedDate, a.UpdatedBy ";
                Query += "From [dbo].[ReceiptOrderH] a ";
                Query += "Left join TransStatusTable b on a.ReceiptOrderStatus = b.StatusCode and b.Transcode ='RO' ";
                Query += "Where ReceiptOrderStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select * From(Select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) ";
                Query += "No, ReceiptOrderId 'ReceiptNumber', ReceiptOrderDate 'ReceiptDate', DeliveryDate,a.PurchaseOrderId, VendId 'VendorId', VendorName 'VendorName', InventSiteName, InventSiteID 'WarehouseCode', ReceiptOrderStatus, a.CreatedDate, a.CreatedBy, CASE WHEN a.UpdatedBy = '' then null else a.UpdatedDate END UpdatedDate, a.UpdatedBy ";
                Query += "From [dbo].[ReceiptOrderH] a ";
                Query += "Left join TransStatusTable b on a.ReceiptOrderStatus = b.StatusCode and b.Transcode ='RO' ";
                Query += "Where (ReceiptOrderId like '%@search%' or VendId like '%@search%'  or VendorName like '%@search%' or InventSiteID like '%@search%') And ";
                Query += "ReceiptOrderStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "Select * From(Select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) ";
                Query += "No, ReceiptOrderId 'ReceiptNumber', ReceiptOrderDate 'ReceiptDate', DeliveryDate,a.PurchaseOrderId, VendId 'VendorId', VendorName 'VendorName', InventSiteName, InventSiteID 'WarehouseCode', ReceiptOrderStatus, a.CreatedDate, a.CreatedBy, CASE WHEN a.UpdatedBy = '' then null else a.UpdatedDate END UpdatedDate, a.UpdatedBy ";
                Query += "From [dbo].[ReceiptOrderH] a ";
                Query += "Left join TransStatusTable b on a.ReceiptOrderStatus = b.StatusCode and b.Transcode ='RO' ";
                Query += "Where (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (ReceiptOrderId like '%@search%' or VendId like '%@search%'  or VendorName like '%@search%' or InventSiteID like '%@search%') And ";
                Query += "ReceiptOrderStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            else if (crit.Equals("Updated Date"))
            {
                Query = "Select * From(Select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) ";
                Query += "No, ReceiptOrderId 'ReceiptNumber', ReceiptOrderDate 'ReceiptDate', DeliveryDate,a.PurchaseOrderId, VendId 'VendorId', VendorName 'VendorName', InventSiteName, InventSiteID 'WarehouseCode', ReceiptOrderStatus, a.CreatedDate, a.CreatedBy, CASE WHEN a.UpdatedBy = '' then null else a.UpdatedDate END UpdatedDate, a.UpdatedBy ";
                Query += "From [dbo].[ReceiptOrderH] a ";
                Query += "Left join TransStatusTable b on a.ReceiptOrderStatus = b.StatusCode and b.Transcode ='RO' ";
                Query += "Where (CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (ReceiptOrderId like '%@search%' or VendId like '%@search%'  or VendorName like '%@search%' or InventSiteID like '%@search%') And ";
                Query += "ReceiptOrderStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            else if (crit.Equals("RO Date"))
            {
                Query = "Select * From(Select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) ";
                Query += "No, ReceiptOrderId 'ReceiptNumber', ReceiptOrderDate 'ReceiptDate', DeliveryDate,a.PurchaseOrderId, VendId 'VendorId', VendorName 'VendorName', InventSiteName, InventSiteID 'WarehouseCode', ReceiptOrderStatus, a.CreatedDate, a.CreatedBy, CASE WHEN a.UpdatedBy = '' then null else a.UpdatedDate END UpdatedDate, a.UpdatedBy ";
                Query += "From [dbo].[ReceiptOrderH] a ";
                Query += "Left join TransStatusTable b on a.ReceiptOrderStatus = b.StatusCode and b.Transcode ='RO' ";
                Query += "Where (CONVERT(VARCHAR(10),ReceiptOrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),ReceiptOrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (ReceiptOrderId like '%@search%' or VendId like '%@search%'  or VendorName like '%@search%' or InventSiteID like '%@search%') And ";
                Query += "ReceiptOrderStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                string QueryTemp = "Select FieldName from [User].[Table] where DisplayName = '" + cmbCriteria.Text.ToString() + "' AND TableName = 'ReceiptOrderH'";
                Cmd = new SqlCommand(QueryTemp, Conn);
                crit = Cmd.ExecuteScalar().ToString();

                Query = "Select * From(Select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) ";
                Query += "No, ReceiptOrderId 'ReceiptNumber', ReceiptOrderDate 'ReceiptDate', DeliveryDate,a.PurchaseOrderId, VendId 'VendorId', VendorName 'VendorName', InventSiteName, InventSiteID 'WarehouseCode', ReceiptOrderStatus, a.CreatedDate, a.CreatedBy, CASE WHEN a.UpdatedBy = '' then null else a.UpdatedDate END UpdatedDate, a.UpdatedBy ";
                Query += "From [dbo].[ReceiptOrderH] a ";
                Query += "Left join TransStatusTable b on a.ReceiptOrderStatus = b.StatusCode and b.Transcode ='RO' Where ";
                Query += crit + " like '%@search%' And ";
                Query += "ReceiptOrderStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search", txtSearch.Text);
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
            
            dgvPR.AutoGenerateColumns = true;
            dgvPR.DataSource = Dt;
            dgvPR.Refresh();
            dgvPR.AutoResizeColumns();
            
            Conn.Close();

            if (!dgvPR.Columns.Contains("Preview"))
                dgvPR.Columns.Add(buttonpreview);
            if (!dgvPR.Columns.Contains("Send Email"))
                dgvPR.Columns.Add(buttonSend);
            dgvSetting();
            dgvPaging();
        }

        private void dgvSetting()
        {
            dgvPR.Columns["ReceiptDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPR.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvPR.Columns["DeliveryDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPR.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";

            dgvPR.Columns["ReceiptNumber"].HeaderText = "RO No";
            dgvPR.Columns["ReceiptDate"].HeaderText = "RO Date";
            dgvPR.Columns["PurchaseOrderId"].HeaderText = "PO No";
            dgvPR.Columns["VendorName"].HeaderText = "Vendor";
            dgvPR.Columns["InventSiteName"].HeaderText = "Warehouse";
            dgvPR.Columns["ReceiptOrderStatus"].HeaderText = "Status";
            dgvPR.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvPR.Columns["CreatedBy"].HeaderText = "Created By";
            dgvPR.Columns["DeliveryDate"].HeaderText = "Delivery Date";
            dgvPR.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvPR.Columns["UpdatedBy"].HeaderText = "Updated By";

            dgvPR.Columns["VendorId"].Visible = false;
            dgvPR.Columns["WarehouseCode"].Visible = false;
            dgvPR.Columns["ReceiptOrderStatus"].Visible = false;

        }

        private void dgvPaging()
        {
            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(ReceiptOrderId) From [dbo].[ReceiptOrderH] Where ReceiptOrderStatus in (" + TransStatus + ") ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(ReceiptOrderId) From [dbo].[ReceiptOrderH] Where ";
                Query += "(ReceiptOrderId like '%@search%' or VendId like '%@search%'  or VendorName like '%@search%' or InventSiteID like '%@search%') And ReceiptOrderStatus in (" + TransStatus + ")";
            }
            else if (crit.Equals("RO Date"))
            {
                Query = "Select Count(ReceiptOrderId) From [dbo].[ReceiptOrderH] Where ";
                Query += "(CONVERT(VARCHAR(10),ReceiptOrderDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),ReceiptOrderDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ReceiptOrderStatus in (" + TransStatus + ") ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "Select Count (ReceiptOrderId) From [dbo].[ReceiptOrderH] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ReceiptOrderStatus in (" + TransStatus + ") ;";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query = "Select Count (ReceiptOrderId) From [dbo].[ReceiptOrderH] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ReceiptOrderStatus in (" + TransStatus + ") ;";
            }
            else
            {
                Query = "Select Count(ReceiptOrderId) From [dbo].[ReceiptOrderH] Where ";
                Query += crit + " Like '%@search%' And ReceiptOrderStatus in (" + TransStatus + ") ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search", txtSearch.Text);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (dataShow != 0)
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        public int DataShow { get { return dataShow; } set { dataShow = value; } }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'ReceiptOrderH' order by OrderNo";

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
            dataShow = Int32.Parse(cmbShow.Text);
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
                if (Convert.ToInt32(txtPage.Text) > Convert.ToInt32(lblPage.Text.Substring(2, lblPage.Text.Length - 2)))
                    txtPage.Text = lblPage.Text.Substring(2, lblPage.Text.Length - 2);
                else if (Convert.ToInt32(txtPage.Text) < 1)
                    txtPage.Text = "1";
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
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
            
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                //if (ControlMgr.GroupName != "PurchaseManager")
                //{
                HeaderReceiptOrder HeaderReceiptOrder = new HeaderReceiptOrder();
                //header.flag("", s"New");
                ListHeaderReceiptOrder.Add(HeaderReceiptOrder);
                HeaderReceiptOrder.SetMode("New", "");
                HeaderReceiptOrder.SetParent(this);
                HeaderReceiptOrder.Show();
                RefreshGrid();
                //}
                //else
                //{
                //    MessageBox.Show("User Group : PurchaseManager \nTidak bisa melakukan create PR.");
                //}
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                decimal QtyBefore = 0;
                decimal PurchOrderSeqNo = 0;
                String POID = null;

                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                try
                {
                    if (dgvPR.RowCount > 0)
                    {
                        Index = dgvPR.CurrentRow.Index;
                        string ReceiptOrderID = dgvPR.Rows[Index].Cells["ReceiptNumber"].Value == null ? "" : dgvPR.Rows[Index].Cells["ReceiptNumber"].Value.ToString();
                        //string TransType = dgvPR.Rows[Index].Cells["TransType"].Value == null ? "" : dgvPR.Rows[Index].Cells["TransType"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();
                        
                        DialogResult dr = MessageBox.Show("ReceiptNumber = " + ReceiptOrderID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string Check = "";
                            Query = "Select ReceiptOrderStatus from [dbo].[ReceiptOrderH] where [ReceiptOrderId]='" + dgvPR.CurrentRow.Cells["ReceiptNumber"].Value.ToString() + "';";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Check = Cmd.ExecuteScalar().ToString();
                            if (Check != "01")
                            {
                                MessageBox.Show("ReceiptOrderId = " + dgvPR.CurrentRow.Cells["ReceiptNumber"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                                Conn.Close();
                                return;
                            }

                            //delete Hedaer
                            //Query = "Delete from [dbo].[ReceiptOrderH]  where ReceiptOrderId ='" + ReceiptOrderID + "';";
                            Query = "UPDATE [dbo].[ReceiptOrderH] SET [ReceiptOrderStatus] = 'XX', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where ReceiptOrderId ='" + ReceiptOrderID + "'";
                            using (Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                            //delete detail
                            //Query += "Delete from [dbo].[ReceiptOrderD] OUTPUT Deleted.Qty, Deleted.PurchaseOrderSeqNo, Deleted.PurchaseOrderId  where ReceiptOrderId='" + ReceiptOrderID + "';";
                            Query = "SELECT * FROM [dbo].[ReceiptOrderD] where ReceiptOrderId='" + ReceiptOrderID + "';";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                QtyBefore = decimal.Parse(Dr["Qty"].ToString());
                                PurchOrderSeqNo = decimal.Parse(Dr["PurchaseOrderSeqNo"].ToString());
                                POID = Dr["PurchaseOrderId"].ToString();

                                Query = "Update [dbo].[PurchDtl] set RemainingQty = (RemainingQty + " + QtyBefore + ") where PurchID = '" + POID + "' and SeqNo = '" + PurchOrderSeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }

                            Dr.Close();

                            //created by Thaddaeus Matthias, 15 March 2018
                            //insert status log 
                            //=====================================begin=======================================
                            //insertstatuslogDelete();
                            Query = "insert into StatusLog_Vendor ([StatusLog_FormName] ,[Vendor_Id] ,[StatusLog_PK1] ,[StatusLog_PK2] ,[StatusLog_PK3] ,[StatusLog_PK4] ,[StatusLog_Status] ,[StatusLog_Description] ,[StatusLog_UserID] ,[StatusLog_Date]) select top 1 [StatusLog_FormName] ,[Vendor_Id] ,[StatusLog_PK1] ,[StatusLog_PK2] ,[StatusLog_PK3] ,[StatusLog_PK4] ,'XX' ,'Deleted' ,'" + ControlMgr.UserId + "' ,getdate() from StatusLog_Vendor where StatusLog_PK1 = '" + ReceiptOrderID + "' order by StatusLog_Date desc"; //BY: HC 
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //======================================end========================================

                            //BY: HC (S) | INSERT KE RO LOG TABLE
                            Query = "insert into ReceiptOrder_LogTable select top 1 [ReceiptOrderDate],[ReceiptOrderNo] ,[PurchaseOrderNo] ,[PurchaseOrderDate] ,[VendorID] ,[InventSiteID] ,[FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount] ,[GoodsReceivedId] ,'XX' ,'Deleted' ,'Deleted' ,'" + ControlMgr.UserId + "' ,getdate() from ReceiptOrder_LogTable where ReceiptOrderNo = '" + ReceiptOrderID + "' order by LogDate desc";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //BY: HC (E)

                            Query = "SELECT [FullItemId] FROM [dbo].[ReceiptOrderD] WHERE [ReceiptOrderId] = '" + ReceiptOrderID + "'";
                            using (Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    updateInventPurchaseQtyRO_PO(ReceiptOrderID, Dr["FullItemId"].ToString(), QtyBefore, "-", "+", Conn, Trans);
                                }
                                Dr.Close();
                            }
                            
                            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";


                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                            Index = 0;
                            Trans.Commit();
                            Conn.Close(); 
                            MessageBox.Show("ReceiptNumber = " + ReceiptOrderID.ToUpper() + "\n" + "Data berhasil dihapus.");
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    Conn.Close();
                    MessageBox.Show(ex.Message);
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
            SelectPR();
        }

        private void SelectPR()
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            HeaderReceiptOrder header = new HeaderReceiptOrder();

            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPR.RowCount > 0)
                {
                    header.SetMode("BeforeEdit", dgvPR.CurrentRow.Cells["ReceiptNumber"].Value.ToString());
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

        private void backtopageone()
        {
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            cmbCriteria.SelectedIndex = 0;
            ModeLoad();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (timerRefresh == null)
            {

            }
            else
            {
                //RefreshGrid();
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

        private void InquiryReceiptOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
            for (int i = 0; i < ListHeaderReceiptOrder.Count(); i++)
            {
                //ListHeaderReceiptOrder[i].Close();
            }
        }

        private void InquiryReceiptOrder_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            //lblForm.Location = new Point(16, 11);
            //setTimer();
            //gvheader();
        }

        //private void gvheader()
        //{

        //    dgvPR.Columns["ReceiptNumber"].HeaderText = "ReceiptNumber";
        //    dgvPR.Columns["ReceiptDate"].HeaderText = "ReceiptDate";
        //    dgvPR.Columns["VendorId"].HeaderText = "VendorId";
        //    dgvPR.Columns["VendorName"].HeaderText = "VendorName";
        //    dgvPR.Columns["WarehouseCode"].HeaderText = "WarehouseCode";
        //    dgvPR.Columns["CreatedDate"].HeaderText = "CreatedDate";
        //    dgvPR.Columns["CreatedBy"].HeaderText = "CreatedBy";
        //}

        private void InquiryReceiptOrder_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
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

        private void dgvPR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex > -1)
            {
                string ROId = dgvPR.Rows[e.RowIndex].Cells["ReceiptNumber"].Value == null ? "" : dgvPR.Rows[e.RowIndex].Cells["ReceiptNumber"].Value.ToString();
                string VendorId = dgvPR.Rows[e.RowIndex].Cells["VendorId"].Value == null ? "" : dgvPR.Rows[e.RowIndex].Cells["VendorId"].Value.ToString();

                if (dgvPR.Columns[e.ColumnIndex].Name == "Preview")
                {                
                    GlobalPreview f = new GlobalPreview("Receipt Order", ROId);
                    f.Show();
                }
                else if (dgvPR.Columns[e.ColumnIndex].Name == "Send Email")
                {                    
                    //SendEmail s = new SendEmail();
                    //s.flag(ROId); //,TransType);
                    //s.Show();

                    GlobalSendEmail f = new GlobalSendEmail("Receipt Order", ROId, VendorId);
                    f.Show();
                }
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01','02', '04','09'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            backtopageone();
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'03','05','XX'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            backtopageone();
            RefreshGrid();
        }

        //created by Thaddaeus Matthias, 15 March 2018
        //insert status log 
        //=====================================begin=======================================
        private void insertstatuslogDelete()
        {
            Index = dgvPR.CurrentRow.Index;
            string ReceiptOrderID = dgvPR.Rows[Index].Cells["ReceiptNumber"].Value == null ? "" : dgvPR.Rows[Index].Cells["ReceiptNumber"].Value.ToString();

            //REMARKED BY: HC
            //Query = "INSERT INTO [dbo].[StatusLog_Vendor] VALUES ";
            //Query += "('ROInquiry', '" + ReceiptOrderID + "', '', '', '', 'XX', 'RO Deleted', '" + ControlMgr.UserId + "', getdate() )";
            Query = "insert into StatusLog_Vendor ([StatusLog_FormName] ,[Vendor_Id] ,[StatusLog_PK1] ,[StatusLog_PK2] ,[StatusLog_PK3] ,[StatusLog_PK4] ,[StatusLog_Status] ,[StatusLog_Description] ,[StatusLog_UserID] ,[StatusLog_Date]) select top 1 [StatusLog_FormName] ,[Vendor_Id] ,[StatusLog_PK1] ,[StatusLog_PK2] ,[StatusLog_PK3] ,[StatusLog_PK4] ,'XX' ,'Deleted' ,'" + Login.UserId + "' ,getdate() from StatusLog_Vendor where StatusLog_PK1 = '" + ReceiptOrderID + "' order by StatusLog_Date desc"; //BY: HC 
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }
        //======================================end========================================

        private void btnGunakan_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                decimal Qty = 0;
                decimal PurchOrderSeqNo = 0;
                String POID = null;

                try
                {
                    if (dgvPR.RowCount > 0)
                    {
                        Index = dgvPR.CurrentRow.Index;
                        string ReceiptOrderID = dgvPR.Rows[Index].Cells["ReceiptNumber"].Value == null ? "" : dgvPR.Rows[Index].Cells["ReceiptNumber"].Value.ToString();
                        DialogResult dr = MessageBox.Show("ReceiptNumber = " + ReceiptOrderID + "\n" + "Apakah data diatas akan digunakan kembali ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string Check = "";
                            Conn = ConnectionString.GetConnection();

                            Query = "Select ReceiptOrderStatus from [dbo].[ReceiptOrderH] where [ReceiptOrderId]='" + dgvPR.CurrentRow.Cells["ReceiptNumber"].Value.ToString() + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Check = Cmd.ExecuteScalar().ToString();
                            if (Check != "XX")
                            {
                                MessageBox.Show("ReceiptOrderId = " + dgvPR.CurrentRow.Cells["ReceiptNumber"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa digunakan kembali karena belum dibatalkan.");
                                Conn.Close();
                                return;
                            }

                            Conn = ConnectionString.GetConnection();

                            Query = "UPDATE [dbo].[ReceiptOrderH] SET [ReceiptOrderStatus] = '01', UpdatedDate = getdate(), updatedBy = '" + Login.UserId + "' where ReceiptOrderId ='" + ReceiptOrderID + "'";
                            using (Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Cmd.ExecuteNonQuery();
                            }

                            //BY: HC (S)
                            //INSERT KE TABLE STATUS LOG VENDOR
                            Query = "select VendId, PurchaseOrderId, InventSiteID from ReceiptOrderH where ReceiptOrderId = '" + ReceiptOrderID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                ListMethod.StatusLogVendor("HeaderReceiptOrder", "RO", Dr["VendId"].ToString(), "01", "Created", ReceiptOrderID, Dr["PurchaseOrderId"].ToString(), Dr["InventSiteID"].ToString(), "");
                            }
                            Dr.Close();

                            //INSERT KE RO LOG TABLE
                            Query = "insert into ReceiptOrder_LogTable ([ReceiptOrderDate] ,[ReceiptOrderNo] ,[PurchaseOrderNo] ,[PurchaseOrderDate] ,[VendorID] ,[InventSiteID] ,[FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount] ,[GoodsReceivedId] ,[LogStatusCode] ,[LogStatusDesc] ,[LogDescription] ,[UserID] ,[LogDate]) select top 1 [ReceiptOrderDate] ,[ReceiptOrderNo] ,[PurchaseOrderNo] ,[PurchaseOrderDate] ,[VendorID] ,[InventSiteID] ,[FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount] ,[GoodsReceivedId] ,'01' ,'Created' ,'Created' , '" + ControlMgr.UserId + "' ,getdate() from ReceiptOrder_LogTable where ReceiptOrderNo = '" + ReceiptOrderID + "' order by LogDate desc";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            //BY: HC (E)

                            Query = "SELECT * FROM [dbo].[ReceiptOrderD] where ReceiptOrderId='" + ReceiptOrderID + "';";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();

                            while (Dr.Read())
                            {
                                Qty = decimal.Parse(Dr["Qty"].ToString());
                                PurchOrderSeqNo = decimal.Parse(Dr["PurchaseOrderSeqNo"].ToString());
                                POID = Dr["PurchaseOrderId"].ToString();

                                Query = "Update [dbo].[PurchDtl] set RemainingQty = (RemainingQty - " + Qty + ") where PurchID = '" + POID + "' and SeqNo = '" + PurchOrderSeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }

                            Dr.Close();

                            Query = "SELECT [FullItemId] FROM [dbo].[ReceiptOrderD] WHERE [ReceiptOrderId] = '" + ReceiptOrderID + "'";
                            using(Cmd = new SqlCommand(Query,Conn,Trans))
                            {
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    updateInventPurchaseQtyRO_PO(ReceiptOrderID, Dr["FullItemId"].ToString(), Qty, "+", "-", Conn, Trans);
                                }
                                Dr.Close();
                            }
                            
                            MessageBox.Show("ReceiptNumber = " + ReceiptOrderID.ToUpper() + "\n" + "Data berhasil digunakan kembali.");
                            Index = 0;
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void updateInventPurchaseQtyRO_PO(string ROID,string fullItemId,decimal Qty, string ROStatus, string POStatus, SqlConnection con, SqlTransaction trans )
        {
            decimal Ratio = 0;
            decimal Price = 0;
            Query = "SELECT * FROM [dbo].[InventTable] WHERE [FullItemID] = '"+fullItemId+"'";
            using (Cmd = new SqlCommand(Query, con, trans))
            {
                SqlDataReader Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    Price = Convert.ToDecimal(Dr2["UoM_AvgPrice"]);
                    Ratio = Convert.ToDecimal(Dr2["Ratio"]);
                }
                Dr2.Close();
            }

            Query = "SELECT Price, Unit FROM [ReceiptOrderD] WHERE [ReceiptOrderId] = '" + ROID + "' AND [FullItemId] = '"+fullItemId+"'";
            using (Cmd = new SqlCommand(Query, con, trans))
            {
                SqlDataReader Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    if (Dr2["Unit"].ToString() == "KG")
                    {
                        Qty = Qty / Ratio;
                    }
                    if (Dr2["Price"] != null && Convert.ToDecimal(Dr2["Price"]) != 0)
                    {
                        Price = Convert.ToDecimal(Dr2["Price"]);
                    }
                }
                Dr2.Close();
            }

            Query = "UPDATE [dbo].[Invent_Purchase_Qty] SET ";
            //REMARKED BY: HC (S) | GA ADA KOLUM ITU 3
            //Query += " [PO_From_PA_Approved_Alt] " + POStatus + "= " + Qty * Ratio + " ";
            //Query += "  ,[PO_From_PA_Approved2_UoM] " + POStatus + "= " + Qty + " ";
            //Query += "  ,[PO_From_PA_Approved2_Alt] " + POStatus + "= " + Qty * Price+ " ";
            //REMARKED BY: HC (E)
            //BY: HC (S)
            Query += "PO_Issued_Outstanding_UoM " + POStatus + "= " + Qty + ", ";
            Query += "PO_Issued_Outstanding_Alt " + POStatus + "= " + Qty * Ratio + ", ";
            Query += "PO_Issued_Outstanding_Amount " + POStatus + "= " + Qty * Price;
            //BY: HC (E)
            Query += "  ,[RO_Issued_UoM] " + ROStatus + "= " + Qty + " ";
            Query += "  ,[RO_Issued_Alt] " + ROStatus + "= " + Qty * Ratio + " ";
            Query += "  ,[RO_Issued_Amount] " + ROStatus + "= " + Qty * Price + " ";
            Query += " WHERE [FullItemID] = '"+fullItemId+"'";
            using (Cmd = new SqlCommand(Query, con, trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

    }
}
