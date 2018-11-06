using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.Purchase.CanvasSheet
{
    public partial class InquiryCanvasSheet : MetroFramework.Forms.MetroForm
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
        List<FormCanvasSheet2> ListFormCanvasSheet = new List<FormCanvasSheet2>();
        public List<string> VendorId = new List<string>();
        public List<string> PQId = new List<string>();

        string TransStatus = "";

        private static int countCS;

        public static int showData;

        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //List<FormPQ> ListFormPQ = new List<FormPQ>();

        //timer autorefresh
        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10 * 1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        public InquiryCanvasSheet()
        {
            InitializeComponent();
        }

        private void gvHeader()
        {
            //dgvPQ.Columns["CanvasId"].HeaderText = "CS No";
            //dgvPQ.Columns["CanvasDate"].HeaderText = "Date";
            //dgvPQ.Columns["PurchReqId"].HeaderText = "PR ID";
            //dgvPQ.Columns["TransType"].HeaderText = "Transaction Type";
            //dgvPQ.Columns["Status"].HeaderText = "Status";
            //dgvPQ.Columns["CreatedDate"].HeaderText = "Created Date";
            //dgvPQ.Columns["CreatedBy"].HeaderText = "Created By";
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (TransStatus == String.Empty)
            {
                TransStatus = "'01'"; Limit1 = 1; Limit2 = ShowData;
            }

            int mflag;
            String addquery = null;

            Query = "Select a.No,a.CanvasId 'CS No',CONVERT(varchar,a.CanvasDate,3) 'CS Date',a.PurchReqId 'PR No', a.OrderDate 'PR Date', a.TransType 'PR Type',UPPER(a.Deskripsi) 'Status',a.CreatedBy 'Created By',a.CreatedDate 'Created Date', a.UpdatedBy 'Updated By', a.UpdatedDate 'Updated Date' From ";
            Query += "(Select ROW_NUMBER() OVER (ORDER BY CanvasDate DESC, CanvasId desc) No, CanvasId, CanvasDate, a.TransType, a.PurchReqId, a.TransStatus,a.CreatedDate, cr.FullName[CreatedBy], a.UpdatedDate, up.FullName[UpdatedBy], c.OrderDate, b.Deskripsi ";
            Query += "From [dbo].[CanvasSheetH] a ";
            Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON a.CreatedBy = cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON a.UpdatedBy = up.UserID ";
            Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where a.TransStatus in (" + TransStatus + ") ";


            addquery = "AND (CanvasId LIKE @search OR a.PurchReqId LIKE @search OR a.TransType LIKE @search OR b.Deskripsi LIKE @search OR cr.FullName LIKE @search OR up.FullName LIKE @search) ";
            mflag = 1;
            if (crit == null)
            {
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += addquery;
            }
            else
            {
                crit = cmbCriteria.Text;
                Cmd = new SqlCommand("Select FieldName From [User].[Table] WHERE DisplayName = '" + crit + "' AND TableName = 'CanvasSheet'", Conn);
                crit = Cmd.ExecuteScalar().ToString();
                if (crit == "CanvasDate" || crit == "OrderDate" || crit == "CreatedDate" || crit == "UpdatedDate")
                {
                    mflag = 2;
                    if (crit == "OrderDate")
                    {
                        crit = "c." + crit;
                    }
                    else
                    {
                        crit = "a." + crit;
                    }
                    Query += addquery + "AND (" + crit + " BETWEEN @from AND @to) ";
                }
                else
                {
                    if (crit == "CreatedBy")
                    {
                        crit = "cr.FullName";
                    }
                    else if (crit == "UpdatedBy")
                    {
                        crit = "up.FullName";
                    }
                    else if (crit == "Status")
                    {
                        crit = "b.Deskripsi";
                    }
                    else
                    {
                        crit = "a." + crit;
                    }
                    Query += "AND " + crit + " LIKE @search";
                }
            }

            Query += ") a WHERE No BETWEEN @limit1 AND @limit2 ;";

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

            dgvPQ.AutoGenerateColumns = true;
            dgvPQ.DataSource = Dt;
            dgvPQ.Refresh();
            gvHeader();
            dgvPQ.Columns["CS Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["PR Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvPQ.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            Query = "Select Count(CanvasId) From [dbo].[CanvasSheetH] a ";
            Query += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and 'CanvasSheet' = b.TransCode ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON a.CreatedBy = cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON a.UpdatedBy = up.UserID ";
            Query += "left join PurchRequisitionH c on a.PurchReqId=c.PurchReqId Where a.TransStatus in (" + TransStatus + ") ";

            if (crit == null)
            {
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += addquery;
            }
            else
            {
                if (mflag == 2)
                {
                    Query += addquery + "AND (" + crit + " BETWEEN @from AND @to) ";
                }
                else
                {
                    Query += "AND " + crit + " LIKE @search";
                }
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
            if (cmbShow.Text == String.Empty)
                cmbShow.SelectedItem = ShowData.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            dgvPQ.AutoResizeColumns();

        }

        public static int ShowData { get { return showData; } set { showData = value; } }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select [DisplayName] From [User].[Table] Where SchemaName = 'dbo' And TableName = 'CanvasSheet' order by OrderNo";

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
            ShowData = Int32.Parse(cmbShow.Text);
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
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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

        //private void btnNew_Click(object sender, EventArgs e)
        //{
        //    FormPQ FormPQ = new FormPQ();
        //    //header.flag("", "New");
        //    ListFormPQ.Add(FormPQ);
        //    FormPQ.SetMode("New", "");
        //    FormPQ.SetParent(this);
        //    FormPQ.Show();
        //    RefreshGrid();
        //}

        //private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex > -1)
        //    {
        //        FormPQ header = new FormPQ();
        //        header.SetMode("BeforeEdit", dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString());
        //        header.Show();
        //        header.SetParent(this);
        //        RefreshGrid();
        //    }
        //}

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
                    if (dgvPQ.RowCount > 0)
                    {
                        if (dgvPQ.CurrentRow.Cells["Status"].Value.ToString().ToUpper() != "WAITING FOR APPROVAL")
                        {
                            MessageBox.Show("Maaf dokumen Canvas Sheet tidak bisa di Cancel.");
                            return;
                        }

                        Index = dgvPQ.CurrentRow.Index;
                        string CanvasId = dgvPQ.Rows[Index].Cells["CS No"].Value == null ? "" : dgvPQ.Rows[Index].Cells["CS No"].Value.ToString();
                        //string TransType = dgvPR.Rows[Index].Cells["TransType"].Value == null ? "" : dgvPR.Rows[Index].Cells["TransType"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        DialogResult dr = MessageBox.Show("CanvasId = " + CanvasId + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            string Check = "";
                            Conn = ConnectionString.GetConnection();

                            //Query = "Select TransStatus from [dbo].[CanvasSheetH] where [CanvasId]='" + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + "';";
                            //Cmd = new SqlCommand(Query, Conn);
                            //Check = Cmd.ExecuteScalar().ToString();
                            //if (Check != "01")
                            //{
                            //    MessageBox.Show("CanvasId = " + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                            //    Conn.Close();
                            //    return;
                            //}

                            #region Delete Invent_Purch_Qty
                            string QueryTemp = "";
                            string FullItemId = "";
                            string Unit = "";                            
                            string UoM = "";
                            decimal ConvRatio = 0;
                            decimal QtyDeleted = 0;
                            decimal QtyUoMDeleted = 0;
                            decimal QtyAltDeleted = 0;
                            string PRType = "";
                            string CSNo = "";
                            int JumlahCS = 0;
                            VendorId.Clear();

                            PRType = dgvPQ.CurrentRow.Cells["PR Type"].Value == null ? "" : dgvPQ.CurrentRow.Cells["PR Type"].Value.ToString();
                            CSNo = dgvPQ.CurrentRow.Cells["CS No"].Value == null ? "" : dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();

                            Query = "Select distinct PurchQuotId from [ISBS-NEW4].[dbo].[CanvasSheetD] where CanvasId = '" + CSNo + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                //VendorId.Add(Dr["VendId"].ToString());
                                PQId.Add(Dr["PurchQuotId"].ToString());
                            }

                            for (int i = 0; i < PQId.Count; i++)
                            {
                                Query = "SELECT COUNT (CanvasId) FROM [ISBS-NEW4].[dbo].[CanvasSheetD] WHERE [CanvasId]='" + CSNo + "' and PurchQuotId = '" + PQId[i] + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                JumlahCS = (int)Cmd.ExecuteScalar();

                                if (PRType != "AMOUNT")
                                {
                                    for (int j = 1; j <= JumlahCS; j++)
                                    {
                                        Query = "Select FullItemID, Qty, Unit From CanvasSheetD Where CanvasId = '" + CSNo + "' and PurchQuotId = '" + PQId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
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

                                            Query = "Update Invent_Purchase_Qty Set [PR_Approved2_UoM] = PR_Approved2_UoM + " + QtyUoMDeleted + ", PR_Approved2_Alt = PR_Approved2_Alt + " + QtyAltDeleted + ", [PR_CS_Issued_UoM] = [PR_CS_Issued_UoM] - " + QtyUoMDeleted + ", [PR_CS_Issued_Alt] = [PR_CS_Issued_Alt] + " + QtyAltDeleted + " Where FullItemID = '" + FullItemId + "'";
                                            Cmd = new SqlCommand(Query, Conn, Trans);
                                            Cmd.ExecuteNonQuery();
                                            Query = "";
                                        }
                                        Dr.Close();
                                    }
                                }
                                if (PRType == "AMOUNT")
                                {
                                    for (int j = 1; j <= JumlahCS; j++)
                                    {
                                        Query = "Select FullItemID, CSAmount From CanvasSheetD Where CanvasId = '" + CSNo + "' and PurchQuotId = '" + PQId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
                                        Dr = Cmd.ExecuteReader();
                                        while (Dr.Read())
                                        {
                                            FullItemId = Dr["FullItemID"].ToString();
                                            QtyDeleted = decimal.Parse(Dr["CSAmount"].ToString());

                                            Query = "Update Invent_Purchase_Qty Set [PR_Approved2_Amount] = PR_Approved2_Amount + " + QtyDeleted + ", [PR_CS_Issued_Amount] = [PR_CS_Issued_Amount] - " + QtyDeleted + " Where FullItemID = '" + FullItemId + "'";
                                            Cmd = new SqlCommand(Query, Conn, Trans);
                                            Cmd.ExecuteNonQuery();
                                            Query = "";
                                        }
                                        Dr.Close();
                                    }
                                }

                                Query = "UPDATE PurchQuotationH SET TransStatus = '' WHERE PurchQuotID = '" + PQId[i] + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                            }
                            #endregion

                            //delete header
                            Conn = ConnectionString.GetConnection();
                            //Query = "Delete from [dbo].[CanvasSheetH] where [CanvasId]='" + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + "';";

                            //delete detail
                            //Query += "Delete from [dbo].[CanvasSheetD] where CanvasId ='" + CanvasId + "';";

                            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                            Query = "UPDATE CanvasSheetH SET TransStatus = '04' WHERE CanvasId = '" + CanvasId + "'";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("CanvasId = " + CanvasId.ToUpper() + "\n" + "Data berhasil dicancel.");

                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
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
            //end
        }

        //private void btnSelect_Click(object sender, EventArgs e)
        //{
        //    //Simpen HeaderId
        //    if (dgvPQ.RowCount > 0)
        //    {
        //        FormPQ header = new FormPQ();
        //        header.SetMode("BeforeEdit", dgvPQ.CurrentRow.Cells["PurchQuotID"].Value.ToString());
        //        header.SetParent(this);
        //        header.Show();
        //        RefreshGrid();
        //    }
        //}

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
            backtopageone();
            RefreshGrid();
        }

        //hasim 10 oct 2018
        private void backtopageone()
        {
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
        }
        //end hasim 10 oct 2018

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
           //     RefreshGrid();
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

        //private void InquiryPR_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    timerRefresh = null;
        //    for (int i = 0; i < ListFormPQ.Count(); i++)
        //    {
        //        ListFormPQ[i].Close();
        //    }
        //}

        private void InquiryPR_Load(object sender, EventArgs e)
        {
            TransStatus = "'01'";
            addCmbCrit();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);
            //setTimer();

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void InquiryPR_Shown(object sender, EventArgs e)
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

        private void btnApprove_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            FormCanvasSheet2 FormCanvasSheet = new FormCanvasSheet2();
            if (FormCanvasSheet.PermissionAccess(ControlMgr.View) > 0)
            {
                try
                {
                    if (dgvPQ.RowCount > 0)
                    {
                        if (dgvPQ.CurrentRow.Cells["Status"].Value.ToString().ToUpper() == "APPROVED")
                        {
                            MessageBox.Show("Dokumen Canvas Sheet sudah di Approved.");
                            return;
                        }
                        else if (dgvPQ.CurrentRow.Cells["Status"].Value.ToString().ToUpper() == "REJECTED")
                        {
                            MessageBox.Show("Dokumen Canvas Sheet sudah di Rejected.");
                            return;
                        }
                        else if (dgvPQ.CurrentRow.Cells["Status"].Value.ToString().ToUpper() == "CANCELLED")
                        {
                            MessageBox.Show("Dokumen Canvas Sheet sudah di Cancelled.");
                            return;
                        }

                        if (ControlMgr.GroupName == "Purchase Manager")
                        {
                            FormCanvasSheet.SetParent(this);
                            //FormCanvasSheet.Show();                          
                            FormCanvasSheet.ApproveStatus = true;
                            FormCanvasSheet.txtCSNumber.Text = dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();
                            FormCanvasSheet.GetDataHeader();
                            FormCanvasSheet.ModeEdit();
                            FormCanvasSheet.Show();

                            if (ConnectionString.GetConnection().State == ConnectionState.Closed)
                            {
                                Conn.Open();
                            }
                            else
                                Conn.Close();
                            Query = "select count(*) from [dbo].[CanvasSheetH] where [TransStatus] = '01'";
                            Cmd = new SqlCommand(Query, Conn);
                            Conn.Open();
                            CountCS = Int32.Parse(Cmd.ExecuteScalar().ToString());
                            lblTotal.Text = "Total Rows : " + CountCS.ToString();
                            RefreshGrid();
                        }
                        else
                        {
                            MessageBox.Show("User yang bisa melakukan approve hanya (Purchase Manager).");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Data belum dipilih.");
                    }
                    //if (dgvPQ.RowCount > 0)
                    //{
                    //    FormCanvasSheet2 FormCanvasSheet = new FormCanvasSheet2();
                    //    //header.SetParent(this);
                    //    FormCanvasSheet.Show();
                    //    FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                    //    FormCanvasSheet.GetDataHeader();

                    //    //RefreshGrid();
                    //}

                    //if (dgvPQ.RowCount > 0)
                    //{
                    //    Query = "Select StatusApproval from [dbo].[CanvasSheetH] where CanvasId = '" + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + "'";
                    //    Conn = ConnectionString.GetConnection();
                    //    SqlCommand cmd = new SqlCommand(Query, Conn);
                    //    if(cmd.ExecuteScalar().ToString() == "YES")
                    //    {
                    //        MessageBox.Show("CanvasSheet " + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + " sudah di approve");
                    //    }
                    //    else
                    //    {
                    //        ApproveCanvasSheet FormCanvasSheet = new ApproveCanvasSheet();
                    //        FormCanvasSheet.Show();
                    //        FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                    //        FormCanvasSheet.GetDataHeader();
                    //    }
                    //}
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
            //end            
        }

        public static int CountCS { get { return countCS; } set { countCS = value; } }

        private void btnNew_Click(object sender, EventArgs e)
        {


            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                FormCanvasSheet2 FormCanvasSheet = new FormCanvasSheet2();
                FormCanvasSheet.SetParent(this);
                //FormCanvasSheet.Show();
                FormCanvasSheet.Show();
                ListFormCanvasSheet.Add(FormCanvasSheet);
                FormCanvasSheet.ModeNew();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
           
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            SelectData();
            //end
            
        }

        private void SelectData()
        {
            FormCanvasSheet2 FormCanvasSheet = new FormCanvasSheet2();
            if (FormCanvasSheet.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPQ.RowCount > 0)
                {
                    if (TransStatus == "'01'")
                    {
                        FormCanvasSheet.SetParent(this);
                        //FormCanvasSheet.Show();                   
                        FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                        FormCanvasSheet.GetDataHeader();
                        FormCanvasSheet.Show();

                        //RefreshGrid();
                    }
                    else
                    {
                        FormCanvasSheet.SetParent(this);
                        FormCanvasSheet.ApproveStatus = true;
                        FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
                        FormCanvasSheet.GetDataHeader();
                        FormCanvasSheet.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void dgvPQ_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectData();
            //if (dgvPQ.RowCount > 0)
            //{
            //    Purchase.CanvasSheet.FormCanvasSheet2 FormCanvasSheet = new Purchase.CanvasSheet.FormCanvasSheet2();
            //    FormCanvasSheet.SetParent(this);
            //    FormCanvasSheet.Show();
            //    FormCanvasSheet.ModeBeforeEdit(dgvPQ.CurrentRow.Cells["CS No"].Value.ToString());
            //    FormCanvasSheet.GetDataHeader();

            //    //RefreshGrid();
            //}
        }

        private Boolean checkReleasePOPA(string checkCSRelease)
        {
            bool vbol = true;
            
            //Query = "SELECT ReffId from [PurchH] Where ReffId = '" + checkCSRelease + "'";
            //using (Conn = ConnectionString.GetConnection())
            //using (Cmd = new SqlCommand(Query, Conn))
            //{
            //    Dr = Cmd.ExecuteReader();
            //    if (Dr.HasRows)
            //    {
            //        MessageBox.Show("Canvas Sheets sudah direlease PO/PA");
            //        vbol = false;
            //    }
            //}

            
            #region hasim , check canvas sheet PQ yg belum dijadikan PA / PO
            
            decimal PQapprCS, PA_atau_PO;
            Conn = ConnectionString.GetConnection();

            Query = "SELECT COUNT(a.PQid) FROM (SELECT DISTINCT(PurchQuotId) AS PQid FROM CanvasSheetD WHERE CanvasId='" + checkCSRelease + "' AND StatusApproval='Yes' GROUP BY PurchQuotId) AS a;";
            Cmd = new SqlCommand(Query, Conn);
            PQapprCS = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());

            
            Query = "SELECT COUNT(a.PQid) FROM (SELECT DISTINCT(ReffId2) AS PQid from [PurchH] Where ReffId = 'PA/1805/00020') AS a;";
            Cmd = new SqlCommand(Query, Conn);
            PA_atau_PO = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());
            if (PQapprCS == PA_atau_PO)
            {
                MessageBox.Show("Canvas Sheets sudah direlease PO");
                vbol = false;
            }

            Query = "SELECT COUNT(PurchQuotId) from [PurchAgreementH] Where CanvasId = '" + checkCSRelease + "'";
            Cmd = new SqlCommand(Query, Conn);
            PA_atau_PO = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());
            if (PQapprCS == PA_atau_PO)
            {
                MessageBox.Show("Canvas Sheets sudah direlease PA");
                vbol = false;
            }

            Conn.Close();

            #endregion

            //Query = "SELECT CanvasId from [PurchAgreementH] Where CanvasId = '" + checkCSRelease + "'";
            //using (Conn = ConnectionString.GetConnection())
            //using (Cmd = new SqlCommand(Query, Conn))
            //{
            //    Dr = Cmd.ExecuteReader();
            //    if (Dr.HasRows)
            //    {
            //        MessageBox.Show("Canvas Sheets sudah direlease PO/PA");
            //        vbol = false;
            //    }
            //}

            return vbol;
        }

        private void btnGeneratePOPA_Click(object sender, EventArgs e)
        {
            string CanvasId = dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();

            if (!checkReleasePOPA(CanvasId))
                return;

            Conn = ConnectionString.GetConnection();

            if (dgvPQ.RowCount > 0)
            {
                if (dgvPQ.CurrentRow.Cells["Status"].Value.ToString() != "APPROVED")
                {
                    //MessageBox.Show("Canves Sheet : " + dgvPQ.CurrentRow.Cells["CS No"].Value.ToString() + " belum di approve..");
                    MessageBox.Show("Canvass Sheet harus diapprove terlebih dahulu");
                }             
                else
                {

                    CanvasId = dgvPQ.CurrentRow.Cells["CS No"].Value.ToString();

                    if (dgvPQ.CurrentRow.Cells["PR Type"].Value.ToString() == "FIX")
                    {
                        Purchase.PurchaseOrderNew.POForm POForm = new Purchase.PurchaseOrderNew.POForm();

                        //begin
                        //updated by : joshua
                        //updated date : 21 feb 2018
                        //description : check permission access
                        if (POForm.PermissionAccess(ControlMgr.New) > 0)
                        {
                            string QuotID = "";

                            Query = "Select ReffId2 From PurchH Where ReffID = '" + CanvasId + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();

                            while (Dr.Read())
                            {
                                if (QuotID == "")
                                {
                                    QuotID = "'" + Dr[0].ToString() + "'";
                                }
                                else
                                {
                                    QuotID += ",";
                                    QuotID += "'" + Dr[0].ToString() + "'";
                                }

                            }
                            Dr.Close();
                            if (QuotID == "")
                            {
                                QuotID = "''";
                            }

                            Query = "Select DISTINCT PurchQuotId from [dbo].[CanvasSheetD] Where CanvasId = '" + CanvasId + "' and StatusApproval = 'YES' and Qty > '0' and PurchQuotId NOT IN(" + QuotID + ")";

                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();

                            if (Dr.Read())
                            {
                                POForm.SetMode("Generate", "", CanvasId);
                                POForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("Tidak ada Purchase Quotation yang sudah di Approve");
                            }
                            Dr.Close();
                            //if (Purchase.PurchaseOrderNew.PQ.PQID1.Count == 0)
                            //POForm.Close();
                        }
                        else
                        {
                            MessageBox.Show(ControlMgr.PermissionDenied);
                        }
                        //end                        
                    }
                    else
                    {
                        

                        Purchase.PurchaseAgreement.PAForm PAForm = new Purchase.PurchaseAgreement.PAForm();

                        //begin
                        //updated by : joshua
                        //updated date : 21 feb 2018
                        //description : check permission access
                        if (PAForm.PermissionAccess(ControlMgr.New) > 0)
                        {
                            PAForm.SetMode("New", CanvasId, "");
                            if (Purchase.PurchaseAgreement.SelectPQ.PQId != "" && Purchase.PurchaseAgreement.SelectPQ.VendId != "")
                            {
                                Purchase.PurchaseAgreement.PAInq PAInq = new Purchase.PurchaseAgreement.PAInq();
                                PAForm.SetParent(PAInq);
                                PAForm.Show();
                            }
                            else
                            {
                                Purchase.PurchaseAgreement.SelectPQ f = new Purchase.PurchaseAgreement.SelectPQ();
                                f.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show(ControlMgr.PermissionDenied);
                        }
                        //end
                    }
                }
            }
            Conn.Close();
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            backtopageone();
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'02','03','04'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            backtopageone();
            RefreshGrid();
        }

        private void txtPage_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblPage_Click(object sender, EventArgs e)
        {

        }

    }
}
