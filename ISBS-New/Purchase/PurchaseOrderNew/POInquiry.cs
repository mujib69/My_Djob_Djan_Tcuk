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

namespace ISBS_New.Purchase.PurchaseOrderNew
{
    public partial class POInquiry : MetroFramework.Forms.MetroForm
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
        List<POForm> ListPOForm = new List<POForm>();
        string TransStatus = "";

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        private void setTimer()
        {
            //Timer timerRefresh = new Timer();
            //timerRefresh.Interval = (10 * 1000);//milisecond
            //timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            //timerRefresh.Start();
        }

        public POInquiry()
        {
            InitializeComponent();
            if (ControlMgr.GroupName == "Staff" )
            {
                btnPreview.Visible = false;
                btnEmail.Visible = false;
            }    
        }

        private void POInquiry_Load(object sender, EventArgs e)
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

            //hasim 12 okt 2018
            int mflag = 1;

            if (TransStatus == String.Empty)
            {
                TransStatus = "'01','04','05','06','08'";
            }

            Query = "Select * From (Select ROW_NUMBER() OVER (order by CASE WHEN h.CreatedDate >= h.UpdatedDate THEN  h.CreatedDate ELSE  h.UpdatedDate END DESC) No, PurchID, OrderDate, DueDate, TransType, ReffTableName, ReffID, h.CurrencyID, ExchRate, h.VendID, VendName, TransStatus, b.Deskripsi, h.CreatedBy, h.CreatedDate, h.UpdatedBy, CASE WHEN h.UpdatedBy = '' then null else h.UpdatedDate END UpdatedDate From [dbo].[PurchH] h LEFT JOIN [dbo].[TransStatusTable] b ON h.TransStatus = b.StatusCode left join [VendTable] c on h.VendId=c.VendId ";
            if (crit == null)
            {
                Query += "Where h.TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
                
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += "Where (h.PurchID like @search or h.VendID like @search or b.Deskripsi like @search) And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }
            else if(crit.Equals("OrderDate"))
            {
                Query += "where (OrderDate BETWEEN @from AND @to) And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
                mflag = 2;
            }
            else if (crit.Equals("PurchID"))
            {
                Query += "Where h.PurchID Like @search And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }
            else if (crit.Equals("VendID"))
            {
                Query += "Where h.VendID Like @search And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }
            else if (crit.Equals("TransStatus"))
            {
                Query += "Where b.Deskripsi Like @search And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }

            if (TransStatus.Contains("07"))
            {
                Query += "and h.PurchID IN (SELECT distinct(d.PurchID) FROM PurchDtl d GROUP BY d.PurchID HAVING SUM(d.RemainingQty) = 0) ";
            }
            else
            {
                Query += "and h.PurchID IN (SELECT distinct(d.PurchID) FROM PurchDtl d GROUP BY d.PurchID HAVING SUM(d.RemainingQty) > 0) ";
            }
            Query += ") a ";
            Query += "Where No Between @limit1 and @limit2";

            DataGridViewButtonColumn buttonpreview = new DataGridViewButtonColumn();
            buttonpreview.Name = "Preview";
            buttonpreview.HeaderText = "Preview";
            buttonpreview.Text = "Preview";
            buttonpreview.UseColumnTextForButtonValue = true;
            
            DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            buttonSend.Name = "Send Email";
            buttonSend.HeaderText = "Send Email";
            buttonSend.Text = "Send Email";
            buttonSend.UseColumnTextForButtonValue = true;

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

            dgvPO.AutoGenerateColumns = true;
            dgvPO.DataSource = Dt;
            dgvPO.Refresh();
            dgvPO.AutoResizeColumns();
            dgvPO.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPO.Columns["DueDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPO.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvPO.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";

            if (!dgvPO.Columns.Contains("Preview"))
                dgvPO.Columns.Add(buttonpreview);
            if (!dgvPO.Columns.Contains("Send Email"))
                dgvPO.Columns.Add(buttonSend);

            

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            Query = "Select Count(PurchID) From [dbo].[PurchH] h LEFT JOIN [dbo].[TransStatusTable] b ON h.TransStatus = b.StatusCode left join [VendTable] c on h.VendId=c.VendId ";
            if (crit == null)
            {
                Query += "Where (h.TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }
            else if (crit.Equals("All"))
            {

                Query += "Where( (b.Deskripsi like @search or h.VendID like @search or h.PurchID Like @search) And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO'";
            }
            else if (crit.Equals("OrderDate"))
            {
                Query += "Where (OrderDate BETWEEN @from AND @to AND b.TransCode = 'PO' and h.TransStatus in (" + TransStatus + ") And (PurchID like @search or TransType like @search or ReffTableName like @search or ReffID like @search) ";
            }
            else if (crit.Equals("PurchID"))
            {
                Query += "Where(h.PurchID Like @search And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }
            else if (crit.Equals("VendID"))
            {
                Query += "Where(h.VendID Like @search And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }
            else if (crit.Equals("TransStatus"))
            {
                Query += "Where(b.Deskripsi Like @search And TransStatus in (" + TransStatus + ") AND b.TransCode = 'PO' ";
            }


            if (TransStatus.Contains("07"))
            {
                Query += "and h.PurchID IN (SELECT distinct(d.PurchID) FROM PurchDtl d GROUP BY d.PurchID HAVING SUM(d.RemainingQty) = 0)) ";
            }
            else
            {
                Query += "and h.PurchID IN (SELECT distinct(d.PurchID) FROM PurchDtl d GROUP BY d.PurchID HAVING SUM(d.RemainingQty) > 0)) ";
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

            //No, PurchID, OrderDate, DueDate, TransType, ReffTableName, ReffID, CurrencyID, ExchRate, VendID, TransStatus, b.Deskripsi

            dgvPO.Columns["PurchID"].HeaderText = "PO No";
            dgvPO.Columns["OrderDate"].HeaderText = "PO Date";
            dgvPO.Columns["ReffID"].HeaderText = "Referensi";
            dgvPO.Columns["VendName"].HeaderText = "Vendor Name";
            dgvPO.Columns["Deskripsi"].HeaderText = "Status";

            dgvPO.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvPO.Columns["CreatedBy"].HeaderText = "Created By";
            dgvPO.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvPO.Columns["UpdatedBy"].HeaderText = "Updated By";

            dgvPO.Columns["DueDate"].Visible = false;
            dgvPO.Columns["TransType"].Visible = false;
            dgvPO.Columns["ReffTableName"].Visible = false;
            dgvPO.Columns["CurrencyId"].Visible = false;
            dgvPO.Columns["ExchRate"].Visible = false;
            dgvPO.Columns["VendId"].Visible = true;
            dgvPO.Columns["TransStatus"].Visible = false;

            //Please configure the permission
            dgvPO.Columns["Preview"].Visible = true;
            dgvPO.Columns["Send Email"].Visible = true;
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchH'";

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
           // cmbCriteria.SelectedIndex = 0;
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
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            if (e.KeyChar == (char)13)
            {
                if (Convert.ToInt32(txtPage.Text) > Convert.ToInt32(lblPage.Text.Substring(2, lblPage.Text.Length - 2)))
                    txtPage.Text = lblPage.Text.Substring(2, lblPage.Text.Length - 2);
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                POForm POForm = new POForm();
                ListPOForm.Add(POForm);
                POForm.SetMode("New", "", "");
                POForm.SetParent(this);
                POForm.Show();
                RefreshGrid();

            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void dgvPO_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            POForm header = new POForm();

            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (e.RowIndex > -1)
                {
                    header.SetMode("BeforeEdit", dgvPO.CurrentRow.Cells["PurchID"].Value.ToString(), "");
                    header.Show();
                    header.SetParent(this);
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvPO.RowCount > 0)
                    {
                        Index = dgvPO.CurrentRow.Index;
                        string PurchID = dgvPO.Rows[Index].Cells["PurchID"].Value == null ? "" : dgvPO.Rows[Index].Cells["PurchID"].Value.ToString();

                        Conn = ConnectionString.GetConnection();
                        Trans = Conn.BeginTransaction();

                        Query = "SELECT COUNT(*) FROM ReceiptOrderH WHERE PurchaseOrderId = '" + PurchID + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountDataRO = Convert.ToInt32(Cmd.ExecuteScalar());

                        Query = "SELECT COUNT(*) COUNTDATA FROM PurchH WHERE ReffId ='" + PurchID + "';";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        int CountDataAmmend = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountDataAmmend == 0 && CountDataRO == 0)
                        {
                            DialogResult dr = MessageBox.Show("Purch ID = " + PurchID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                //update credit limit, created By Thaddaeus 21 JULY2018
                                string VendId = "";
                                decimal TotalNettCredit = 0;
                                Query = "SELECT [VendID],[Total_Nett] FROM  [dbo].[PurchH] WHERE [PurchID] = '" + dgvPO.Rows[dgvPO.CurrentCell.RowIndex].Cells["PurchID"].Value.ToString() + "'  ";
                                using (Cmd = new SqlCommand(Query, Conn, Trans))
                                {
                                    Dr = Cmd.ExecuteReader();
                                    if (Dr.HasRows)
                                    {
                                        while (Dr.Read())
                                        {
                                            VendId = Dr["VendID"].ToString();
                                            TotalNettCredit = Convert.ToDecimal(Dr["Total_Nett"]);
                                        }
                                    }
                                }
                                Query = "UPDATE [dbo].[VendTable] SET [Sisa_Limit_Total] -= " + TotalNettCredit + " WHERE [VendId] = '" + VendId + "' ";
                                using (Cmd = new SqlCommand(Query, Conn, Trans))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                                //END===================================================

                                Query = "UPDATE PurchH SET DeletedDate = getdate(), DeletedBy = '" + ControlMgr.UserId + "' WHERE PurchID = '" + PurchID + "';";
                                Query += "UPDATE PurchH SET TransStatus = 'XX', DeletedDate = getdate(), DeletedBy = '" + ControlMgr.UserId + "' WHERE PurchID = '" + PurchID + "';";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                Query = "SELECT h.ReffId, h.TransType, d.Qty_KG, d.RemainingQty, d.ReffSeqNo, d.ReffBaseSeqNo, d.Total_Nett FROM PurchDtl d INNER JOIN PurchH h ";
                                Query += "ON d.PurchID = h.PurchID WHERE h.ReffTableName = 'PA' AND d.PurchID = '" + PurchID + "'";
                                using (SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans))
                                {
                                    Dr = cmd2.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        decimal ReffSeqNo = Convert.ToDecimal(Dr["ReffSeqNo"]);
                                        decimal ReffBaseSeqNo = Convert.ToDecimal(Dr["ReffBaseSeqNo"]);
                                        decimal Qty_KG = Convert.ToDecimal(Dr["Qty_KG"]);
                                        decimal RemainingQty = Convert.ToDecimal(Dr["RemainingQty"]);
                                        string PAID = Convert.ToString(Dr["ReffId"]);
                                        decimal AmountPO = Convert.ToDecimal(Dr["Total_Nett"]);

                                        string Query2 = "";
                                        //string Query2 = "UPDATE PurchAgreementDtl SET RemainingQty = (RemainingQty + " + Qty_KG + "), UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE AgreementID = '" + PAID + "' AND SeqNo = '" + ReffSeqNo + "' and Base='Y';";
                                        if (Convert.ToString(Dr["TransType"]) == "AMOUNT")
                                        {
                                            Query2 = "UPDATE PurchAgreementDtl SET RemainingAmount = (RemainingAmount + " + AmountPO + "), UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE AgreementID = '" + PAID + "' AND SeqNoGroup = '" + ReffBaseSeqNo + "' and Base='Y';";
                                        }
                                        else
                                        {
                                            Query2 = "UPDATE PurchAgreementDtl SET RemainingQty = (RemainingQty + " + Qty_KG + "), UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' WHERE AgreementID = '" + PAID + "' AND SeqNoGroup = '" + ReffBaseSeqNo + "' and Base='Y';";
                                        }
                                        Cmd = new SqlCommand(Query2, Conn, Trans);
                                        Cmd.ExecuteNonQuery();
                                    }
                                }
                                Dr.Close();

                                //BY: HC (S)
                                DateTime dtReff = new DateTime(1900, 1, 1);
                                if (dgvPO.Rows[Index].Cells["ReffTableName"].Value.ToString() == "Canvass Sheet")
                                {
                                    Query = "select CanvasDate from CanvasSheetH where CanvasId = '" + dgvPO.Rows[Index].Cells["ReffID"].Value + "'";
                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    dtReff = Convert.ToDateTime(Cmd.ExecuteScalar());
                                }
                                else if (dgvPO.Rows[Index].Cells["ReffTableName"].Value.ToString() == "Purchase Agreement")
                                {
                                    Query = "select OrderDate from PurchAgreementH where AgreementID = '" + dgvPO.Rows[Index].Cells["ReffID"].Value + "'";
                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    dtReff = Convert.ToDateTime(Cmd.ExecuteScalar());
                                }

                                Query = "INSERT INTO [dbo].[PO_Issued_LogTable] ([PODate],[POId],[VendId],[Qty_UoM],[Qty_Alt],[Amount],[PAId],[PADate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[POSeqNo]) VALUES (@PODate, '" + dgvPO.Rows[Index].Cells["PurchID"].Value + "', '" + dgvPO.Rows[Index].Cells["VendID"].Value + "', 0, 0, 0, '" + dgvPO.Rows[Index].Cells["ReffID"].Value + "', @dtReff, 'XX', 'PO Deleted', 'PO Deleted', '" + ControlMgr.UserId + "', getdate(), 0);";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.Parameters.AddWithValue("PODate", Convert.ToDateTime(dgvPO.Rows[Index].Cells["OrderDate"].Value));
                                Cmd.Parameters.AddWithValue("dtReff", dtReff);
                                Cmd.ExecuteNonQuery();

                                Query = "INSERT INTO [dbo].[StatusLog_Vendor] ([StatusLog_FormName],[Vendor_Id],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]) VALUES ('POForm', '" + dgvPO.Rows[Index].Cells["VendID"].Value + "', '" + dgvPO.Rows[Index].Cells["PurchID"].Value + "', '" + dgvPO.Rows[Index].Cells["ReffID"].Value + "', '', '', 'XX', 'PO Deleted', '" + ControlMgr.UserId + "', getdate()";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                                //BY: HC (E)

                                MessageBox.Show("PurchID = " + PurchID.ToUpper() + "\n" + "Data berhasil dihapus.");
                            }
                            if (dr == DialogResult.No)
                                return;
                        }
                        else
                        {
                            MessageBox.Show("Purch ID = " + PurchID.ToUpper() + "\n" + "tidak dapat dihapus karena sudah diproses.");
                            return;
                        }
                        Trans.Commit();
                        RefreshGrid();


                        //if (CheckStatus(PurchID) == 0)
                        //{
                        //    string RefTransID = GetRefTransID(PurchID);
                        //    if (RefTransID == "")
                        //    {
                        //        DialogResult dr = MessageBox.Show("PurchID = " + PurchID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        //        if (dr == DialogResult.Yes)
                        //        {
                        //            Conn = ConnectionString.GetConnection();
                        //            Trans = Conn.BeginTransaction();
                        //           // string Check = "";

                        //            //Query = "Select b.TransStatus from [dbo].[RequestForQuotationD] a JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqID] = b.[PurchReqID] where a.[RfqID]='" + dgvRFQ.CurrentRow.Cells["RfqID"].Value.ToString() + "';";
                        //            //Cmd = new SqlCommand(Query, Conn);
                        //            //Check = Cmd.ExecuteScalar().ToString();
                        //            //if (Check == "21")
                        //            //{
                        //            //    MessageBox.Show("RFQID = " + dgvRFQ.CurrentRow.Cells["RfqID"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                        //            //    Conn.Close();
                        //            //    return;
                        //            //}

                        //            //delete header


                        //            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                        //            //Cmd = new SqlCommand(Query, Conn, Trans);
                        //            //Cmd.ExecuteNonQuery();

                        //            //delete item
                        //            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                        //            //created by Thaddaeus Matthias, 15 March 2018
                        //            // insertin status log delete data
                        //            //===================================Begin=====================================
                        //            insertstatuslogDelete();
                        //            //====================================end======================================

                        //            Query = "Delete from [dbo].[PurchDtl] where PurchId ='" + PurchID + "'; ";
                        //            Query += "Delete from [dbo].[PurchH] where PurchId ='" + PurchID + "';";
                        //            Cmd = new SqlCommand(Query, Conn, Trans);
                        //            Cmd.ExecuteNonQuery();
                        //            Trans.Commit();

                        //            MessageBox.Show("PurchID = " + PurchID.ToUpper() + "\n" + "Data berhasil dihapus.");

                        //            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                        //            //Index = 0;

                        //            RefreshGrid();
                        //        }
                        //    }
                        //    else
                        //    {
                        //        DialogResult dr = MessageBox.Show("PurchID = " + PurchID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        //        if (dr == DialogResult.Yes)
                        //        {
                        //            Conn = ConnectionString.GetConnection();
                        //            Trans = Conn.BeginTransaction();
                        //           // string Check = "";

                        //            //Query = "Select b.TransStatus from [dbo].[RequestForQuotationD] a JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqID] = b.[PurchReqID] where a.[RfqID]='" + dgvRFQ.CurrentRow.Cells["RfqID"].Value.ToString() + "';";
                        //            //Cmd = new SqlCommand(Query, Conn);
                        //            //Check = Cmd.ExecuteScalar().ToString();
                        //            //if (Check == "21")
                        //            //{
                        //            //    MessageBox.Show("RFQID = " + dgvRFQ.CurrentRow.Cells["RfqID"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                        //            //    Conn.Close();
                        //            //    return;
                        //            //}

                        //            //delete header


                        //            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                        //            //Cmd = new SqlCommand(Query, Conn, Trans);
                        //            //Cmd.ExecuteNonQuery();

                        //            //delete item
                        //            //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                        //            Query = "Delete from [dbo].[PurchDtl] where PurchId ='" + PurchID + "'; ";
                        //            Query += "Delete from [dbo].[PurchH] where PurchId ='" + PurchID + "';";

                        //            Cmd = new SqlCommand(Query, Conn, Trans);
                        //            Cmd.ExecuteNonQuery();

                        //            Query = "UPDATE PurchH SET StClose = 0, TransStatus = '05' WHERE PurchId = '" + RefTransID + "'";
                        //            Cmd = new SqlCommand(Query, Conn, Trans);
                        //            Cmd.ExecuteNonQuery();

                        //            Trans.Commit();

                        //            MessageBox.Show("PurchID = " + PurchID.ToUpper() + "\n" + "Data berhasil dihapus.");

                        //            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                        //            //Index = 0;

                        //            RefreshGrid();
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    MessageBox.Show("PurchID = " + PurchID + "\n" + "tidak dapat dihapus karena sudah diproses.");
                        //}                          
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
            //end
        }

        private int CheckStatus(string prmPurchID)
        {
            int result = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(h.PurchID) FROM PurchH H INNER JOIN PurchDtl D ";
            Query += "ON D.PurchID = H.PurchID ";
            Query += "WHERE H.PurchID = '" + prmPurchID + "' AND (H.StClose = 1 OR (D.Qty <>  D.RemainingQty))";
            Cmd = new SqlCommand(Query, Conn);
            result = Convert.ToInt32(Cmd.ExecuteScalar());
            Conn.Close();

            return result;
        }

        private string GetRefTransID(string prmPurchID)
        {
            string result = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT ReffId FROM PurchH WHERE PurchID = '" + prmPurchID + "'";
            Cmd = new SqlCommand(Query, Conn);
            result = Convert.ToString(Cmd.ExecuteScalar());
            Conn.Close();

            return result;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            POForm header = new POForm();

            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPO.RowCount > 0)
                {
                    header.SetMode("BeforeEdit", dgvPO.CurrentRow.Cells["PurchID"].Value.ToString(), "");
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

        private void POInquiry_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu f = new MainMenu();
            f.refreshTaskList();
            //timerRefresh = null;
            //for (int i = 0; i < ListFormPQ.Count(); i++)
            //{
            //    ListFormPQ[i].Close();
            //}
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

        private void dgvPO_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvPO.Rows[dgvPO.CurrentRow.Index].Selected = true;
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                string PurchID = dgvPO.Rows[e.RowIndex].Cells["PurchID"].Value == null ? "" : dgvPO.Rows[e.RowIndex].Cells["PurchID"].Value.ToString();
                string Check = "";
                string VendId = dgvPO.Rows[e.RowIndex].Cells["VendId"].Value == null ? "" : dgvPO.Rows[e.RowIndex].Cells["VendId"].Value.ToString();

                Conn = ConnectionString.GetConnection();
                if (dgvPO.Columns[e.ColumnIndex].Name == "Preview")
                {
                    if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Purchase Manager")
                    {
                        GlobalPreview f = new GlobalPreview("Purchase Order", PurchID);
                        f.Show();
                    }
                    else if (ControlMgr.GroupName == "Staff")
                    {

                        Query = "Select PurchH_Print from [dbo].[PurchH] where [PurchID]='" + PurchID + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Check = Cmd.ExecuteScalar().ToString();
                        if (Check == "True")
                        {
                            GlobalPreview f = new GlobalPreview("Purchase Order", PurchID);
                            f.Show();

                            Trans = Conn.BeginTransaction();
                            Query = "Update [dbo].[PurchH] set PurchH_Print = '0' Where PurchId = '" + PurchID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            Trans.Commit();
                            Conn.Close();
                        }
                        else
                        {
                            MessageBox.Show("Don't have permission to Preview");
                        }
                    }
                    else MessageBox.Show("You don't have permission to Preview");
                }

                if (dgvPO.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Purchase Manager")
                    {
                        GlobalSendEmail f = new GlobalSendEmail("Purchase Order", PurchID, VendId);
                        f.Show();
                    }
                    else if (ControlMgr.GroupName == "Staff")
                    {
                        Query = "Select PurchH_SendEmail from [dbo].[PurchH] where [PurchID]='" + PurchID + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Check = Cmd.ExecuteScalar().ToString();
                        if (Check == "False")
                        {
                            GlobalSendEmail f = new GlobalSendEmail("Purchase Order", PurchID, VendId);
                            f.Show();
                        }
                        else
                        {
                            MessageBox.Show("Don't have permission to send again");
                        }
                    }
                    else MessageBox.Show("You don't have permission to send again");
                }
                Conn.Close();
            }
        }
        

        private void dgvPO_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1 )
            {
                if (dgvPO.Columns[e.ColumnIndex].Name.ToString() == "VendID")
                {
                    PopUp.Vendor.Vendor f = new PopUp.Vendor.Vendor();
                    vendID = dgvPO.Rows[e.RowIndex].Cells["VendID"].Value.ToString();
                    f.Show();
                }
            }
        }

        public static string vendID;
        
        public string VendID { get { return vendID; } set { vendID = value; } }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            Index = dgvPO.CurrentRow.Index;
            string PurchID = dgvPO.Rows[Index].Cells["PurchID"].Value == null ? "" : dgvPO.Rows[Index].Cells["PurchID"].Value.ToString();

            Query = "Update [dbo].[PurchH] set PurchH_SendEmail = '0' Where PurchId = '" + PurchID + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
            Trans.Commit();
            Conn.Close();
            MessageBox.Show("Approved Access to Sending E-mail");
        }

        //private void btnPreview_Click(object sender, EventArgs e)
        //{
        //    Conn = ConnectionString.GetConnection();
        //    Trans = Conn.BeginTransaction();

        //    Index = dgvPO.CurrentRow.Index;
        //    string PurchID = dgvPO.Rows[Index].Cells["PurchID"].Value == null ? "" : dgvPO.Rows[Index].Cells["PurchID"].Value.ToString();

        //    Query = "Update [dbo].[PurchH] set PurchH_Print = '1' Where PurchId = '" + PurchID + "'";
        //    Cmd = new SqlCommand(Query, Conn, Trans);
        //    Cmd.ExecuteNonQuery();
        //    Trans.Commit();
        //    MessageBox.Show("Approved Access to Preview");
        //    Conn.Close();
        //}

        private void dgvPO_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvPO.Columns["ExchRate"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.0000";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                dgvPO.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N4");
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01','04','05','06','08'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            backtopageone();
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'02','03','07'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            backtopageone();
            RefreshGrid();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            String PONumber = dgvPO.CurrentRow.Cells["PurchId"].Value.ToString();

            Conn = ConnectionString.GetConnection();
            Query = "Select TransStatus From PurchH Where PurchId = '" + PONumber+ "'";
            Cmd = new SqlCommand(Query, Conn);
            string TransStatus = Convert.ToString(Cmd.ExecuteScalar());
           
            if (TransStatus == "02" || TransStatus == "03" || TransStatus == "07")
            {
                Conn.Close();
                MessageBox.Show("Purchase Order telah selesai.");
                return;
            }
            else if (TransStatus == "04" || TransStatus == "08")
            {
                Conn.Close();
                MessageBox.Show("Purchase Order belum di approve.");
                return;
            }
            else if (TransStatus == "06")
            {
                Conn.Close();
                MessageBox.Show("Purchase Order membutuhkan revisi terlebih dahulu.");
                return;
            }
            else if (TransStatus == "05")
            {
                Query = "SELECT COUNT(d.PurchID) FROM PurchH h ";
                Query += "INNER JOIN PurchDtl d ";
                Query += "ON d.PurchID = h.PurchID ";
                Query += "WHERE h.TransStatus = '05' AND h.PurchID = '" + PONumber + "' ";
                Query += "GROUP BY d.PurchID HAVING SUM(d.RemainingQty) = 0 ";
                Cmd = new SqlCommand(Query, Conn);

                if (Convert.ToInt32(Cmd.ExecuteScalar()) != 0)
                {
                    Conn.Close();
                    MessageBox.Show("Purchase Order telah selesai.");
                    return;
                }                
            }

            Conn.Close();
            
            
            Purchase.PurchaseOrderNew.POForm POForm = new Purchase.PurchaseOrderNew.POForm();
            POForm.SetParent(this);
            POForm.SetMode("Amend", PONumber, "");
            POForm.Show();
        }

        //Created by Thaddaeus Matthias, 15 March 2018
        //insert status log deleted data
        //========================================begin=========================================

        private void insertstatuslogDelete()
        {
            SqlConnection Conn = ConnectionString.GetConnection();
            Index = dgvPO.CurrentRow.Index;
            string PurchID = dgvPO.Rows[Index].Cells["PurchID"].Value == null ? "" : dgvPO.Rows[Index].Cells["PurchID"].Value.ToString();
            string VendorID = dgvPO.Rows[Index].Cells["VendID"].Value == null ? "" : dgvPO.Rows[Index].Cells["VendID"].Value.ToString();
            string PK3 = dgvPO.Rows[Index].Cells["ReffID"].Value == null ? "" : dgvPO.Rows[Index].Cells["ReffID"].Value.ToString();

            Query = "INSERT INTO [dbo].[StatusLog_Vendor] VALUES "; //[StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]
            Query += " ('POInquiry', '" + PurchID + "', '" + VendorID + "', '" + PK3 + "', '', 'XX', 'PO Data Deleted', '" + ControlMgr.UserId + "', getdate()) ";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();

            Query = "UPDATE [dbo].[StatusLog_Vendor] SET StatusLog_PK4 = (SELECT TOP 1 [ReffId2] FROM [ISBS-NEW4].[dbo].[PurchH] WHERE [PurchID] LIKE '%" + PurchID.ToString() + "%' ORDER BY [PurchID] DESC) WHERE StatusLog_PK1 =  '" + PurchID + "' ";
            SqlCommand cmd3 = new SqlCommand(Query, Conn, Trans);
            cmd3.ExecuteNonQuery();
        }
        //=========================================end==========================================*/

    }
}
