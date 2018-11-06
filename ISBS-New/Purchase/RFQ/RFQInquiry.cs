using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using CrystalDecisions.ReportSource;
using System.IO;
using CrystalDecisions.Windows.Forms;
//using Microsoft.Exchange.WebServices;
//using Microsoft.Exchange.WebServices.Data;
//using Microsoft.Exchange.WebServices.Autodiscover;

namespace ISBS_New.Purchase.RFQ
{
    public partial class RFQInquiry : MetroFramework.Forms.MetroForm
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
        string TransStatus = ""; string Detail = "";
        string RfqId;

        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public RFQInquiry()
        {
            InitializeComponent();
        }

        private void RFQInquiry_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void gvHeader()
        {
            dgvRFQ.Columns["RfqID"].HeaderText = "RFQ No";
            dgvRFQ.Columns["RfqDate"].HeaderText = "RFQ Date";
            dgvRFQ.Columns["PurchReqId"].HeaderText = "PR No";
            dgvRFQ.Columns["PurchReqDate"].HeaderText = "PR Date";
            dgvRFQ.Columns["TransType"].HeaderText = "PR Type";
            dgvRFQ.Columns["VendId"].HeaderText = "Vendor ID";
            dgvRFQ.Columns["VendName"].HeaderText = "Vendor";
            dgvRFQ.Columns["Deskripsi"].HeaderText = "Status";
            dgvRFQ.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvRFQ.Columns["CreatedBy"].HeaderText = "Created By";
            dgvRFQ.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvRFQ.Columns["UpdatedBy"].HeaderText = "Updated By";
            

            dgvRFQ.Columns["VendId"].Visible = false;

        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            //hasim 11 okt 2018
            int mflag;
            String addquery = null;

            if (TransStatus == String.Empty)
            {
                TransStatus = "'01','02'";
            }

            Query = "Select * From ";
            Query += "(Select ROW_NUMBER() OVER (ORDER BY a.RfqID desc) No, a.RfqID, a.RfqDate, a.PurchReqId, p.OrderDate AS PurchReqDate, a.TransType, a.VendId, a.VendName, c.Deskripsi, a.CreatedDate, cr.FullName[CreatedBy], a.UpdatedDate, up.FullName[UpdatedBy] FROM [dbo].[RequestForQuotationH] a ";
            Query += "LEFT JOIN [dbo].[PurchRequisitionH] b ON a.PurchReqId = b.PurchReqId ";
            Query += "LEFT JOIN [dbo].[TransStatusTable] c ON a.TransStatus = c.StatusCode ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON a.CreatedBy = cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON a.UpdatedBy = up.UserID ";
            Query += "INNER JOIN PurchRequisitionH p ON p.PurchReqId = a.PurchReqId  and c.TransCode='RFQ' ";
            Query += "Where a.TransStatus in (" + TransStatus + ") ";

            addquery = "AND (RfqID LIKE @search OR a.PurchReqId LIKE @search OR a.TransType LIKE @search OR VendName LIKE @search OR a.TransStatus LIKE @search OR cr.FullName LIKE @search OR up.FullName LIKE @search) ";
            mflag = 1;
            if (crit == null){
                mflag = 0;
            }else if (crit.Equals("All")){
                Query += addquery;
            }else{
                crit = cmbCriteria.Text;
                Cmd = new SqlCommand("Select FieldName From [User].[Table] WHERE DisplayName = '" + crit + "' AND TableName = 'RequestForQuotationH'", Conn);
                crit = Cmd.ExecuteScalar().ToString();
                if(crit == "RfqDate" || crit == "OrderDate" || crit == "CreatedDate" || crit == "UpdatedDate" ){
                    mflag = 2;
                    if (crit == "OrderDate")
                    {
                        crit = "p." + crit;
                    }
                    else
                    {
                        crit = "a." + crit;
                    }
                    Query += addquery + "AND (" + crit + " BETWEEN @from AND @to) ";
                }else {
                    if (crit == "CreatedBy"){
                        crit = "cr.FullName";
                    }
                    else if (crit == "UpdatedBy")
                    {
                        crit = "up.FullName";
                    }
                    else
                    {
                        crit = "a." + crit;
                    }
                    Query += "AND " + crit + " LIKE @search";
                }
            }

            Query += ") z WHERE No BETWEEN @limit1 AND @limit2 ;";

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

            dgvRFQ.AutoGenerateColumns = true;
            dgvRFQ.DataSource = Dt;
            gvHeader();
            dgvRFQ.Refresh();
            dgvRFQ.AutoResizeColumns();
            dgvRFQ.Columns["PurchReqDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvRFQ.Columns["RfqDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvRFQ.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";
            dgvRFQ.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm tt";

            if (!dgvRFQ.Columns.Contains("Preview"))
                dgvRFQ.Columns.Add(buttonpreview);
            if (!dgvRFQ.Columns.Contains("Send Email"))
                dgvRFQ.Columns.Add(buttonSend);
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            Query = "Select Count(RfqID) FROM [dbo].[RequestForQuotationH] a ";
            Query += "LEFT JOIN [dbo].[PurchRequisitionH] b ON a.PurchReqId = b.PurchReqId ";
            Query += "LEFT JOIN [dbo].[TransStatusTable] c ON a.TransStatus = c.StatusCode ";
            Query += "LEFT JOIN [dbo].[sysPass] cr ON a.CreatedBy = cr.UserID ";
            Query += "LEFT JOIN [dbo].[sysPass] up ON a.UpdatedBy = up.UserID ";
            Query += "INNER JOIN PurchRequisitionH p ON p.PurchReqId = a.PurchReqId  and c.TransCode='RFQ' ";
            Query += "Where a.TransStatus in (" + TransStatus + ") ";

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
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'RequestForQuotationH' And FieldName <> 'VendID'  order by OrderNo";

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

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                RFQForm f = new RFQForm();
                f.flag("", "New");
                f.Show();
                f.ParentRefreshGrid(this);
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
            
        }

        private void dgvRFQ_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                String RfqId = dgvRFQ.CurrentRow.Cells["RfqId"].Value.ToString();

                RFQForm f = new RFQForm();
                f.flag(RfqId, "Edit");
                f.Show();
                f.ParentRefreshGrid(this);
                RefreshGrid();
            }
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
                    if (dgvRFQ.RowCount > 0)
                    {
                        Conn = ConnectionString.GetConnection();                      

                        Index = dgvRFQ.CurrentRow.Index;
                        string RfqId = dgvRFQ.Rows[Index].Cells["RfqId"].Value == null ? "" : dgvRFQ.Rows[Index].Cells["RfqId"].Value.ToString();

                        Query += "SELECT RfqID FROM PurchQuotationH WHERE RfqID = '" + RfqId + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        string GetDataRFQ = Cmd.ExecuteScalar().ToString();

                        if (GetDataRFQ != "")
                        {
                            MessageBox.Show("Data tidak dapat dihapus karena sudah dibuat Purchase Quotation");
                            return;
                        }
                        else
                        {
                            //string VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                            DialogResult dr = MessageBox.Show("RfqId = " + RfqId + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {

                                string Check = "";

                                //Query = "Select b.TransStatus from [dbo].[RequestForQuotationD] a JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqID] = b.[PurchReqID] where a.[RfqID]='" + dgvRFQ.CurrentRow.Cells["RfqID"].Value.ToString() + "';";
                                //Cmd = new SqlCommand(Query, Conn);
                                //Check = Cmd.ExecuteScalar().ToString();
                                //if (Check == "21")
                                //{
                                //    MessageBox.Show("RFQID = " + dgvRFQ.CurrentRow.Cells["RfqID"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                                //    Conn.Close();
                                //    return;
                                //}

                                //delete header
                                //Query += "Delete from [dbo].[RequestForQuotationH] where RfqId='" + RfqId + "';";

                                //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                                //Cmd = new SqlCommand(Query, Conn, Trans);
                                //Cmd.ExecuteNonQuery();

                                //delete item
                                //Query = "Delete from [dbo].[RequestForQuotationD] where RfqId ='" + RfqId + "'; ";
                                //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                                //Cmd = new SqlCommand(Query, Conn, Trans);
                                //Cmd.ExecuteNonQuery();

                                //Query = "Delete from [dbo].[RequestForQuotation_DtlDtl] where RfqId ='" + RfqId + "'; ";
                                //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                                Query = "UPDATE [dbo].[RequestForQuotationH] SET TransStatus = '05' WHERE RfqID = '" + RfqId + "'";

                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                MessageBox.Show("RFQID = " + RfqId.ToUpper() + "\n" + "Data berhasil dicancel.");

                                ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                                Index = 0;

                                RefreshGrid();
                            }
                        
                        }                       

                        Conn.Close();
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            String RfqId = dgvRFQ.CurrentRow.Cells["RfqId"].Value.ToString();

            RFQForm f = new RFQForm();

            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                f.flag(RfqId, "Edit");
                f.Show();
                f.ParentRefreshGrid(this);
                RefreshGrid();
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
            cmbCriteria.SelectedIndex = 0;
            ModeLoad();
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

        //private void ExportToPdf()
        //{
        //    string RfqId = dgvRFQ.Rows[e.RowIndex].Cells["RfqId"].Value == null ? "" : dgvRFQ.Rows[e.RowIndex].Cells["RfqId"].Value.ToString();
        //    DateTime RfqDate = dgvRFQ.Rows[e.RowIndex].Cells["RfqDate"].Value == null ? default(DateTime) : Convert.ToDateTime(dgvRFQ.Rows[e.RowIndex].Cells["RfqDate"].Value);

        //    PreviewRFQ f = new PreviewRFQ(RfqId, RfqDate);

        //}

        private void dgvRFQ_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                Conn = ConnectionString.GetConnection();

                RfqId = dgvRFQ.Rows[e.RowIndex].Cells["RfqId"].Value == null ? "" : dgvRFQ.Rows[e.RowIndex].Cells["RfqId"].Value.ToString();

                Cmd = new SqlCommand("SELECT [VendID] FROM [RequestForQuotationH] WHERE [RfqID] = '" + RfqId + "'", Conn);
                string VendID = Cmd.ExecuteScalar().ToString();

                if (dgvRFQ.Columns[e.ColumnIndex].Name == "Preview")
                {
                    string TransType = dgvRFQ.Rows[e.RowIndex].Cells["TransType"].Value == null ? "" : dgvRFQ.Rows[e.RowIndex].Cells["TransType"].Value.ToString();

                    Query = "Select PreviewC From [RequestForQuotationH] Where [RfqID] = '" + RfqId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    int PreviewC = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (PreviewC == 0)
                    {
                        if (TransType == "FIX")
                        {
                            Detail = "Fix";
                        }
                        else if (TransType != "FIX")
                        {
                            DialogResult resPreview = MessageBox.Show("Rinci / Tidak", "Rinci / Tidak", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (resPreview == DialogResult.Yes)
                            {
                                Detail = "Rinci";
                            }
                            else
                            {
                                Detail = "Tidak Rinci";
                            }
                        }

                        GlobalPreview f = new GlobalPreview("Request For Quotation", RfqId);
                        f.SetMode(Detail);
                        f.Show();

                        //Set PreviewC to 1
                        Query = "Update [dbo].[RequestForQuotationH] Set [PreviewC] = '1' , PreviewCtr = PreviewCtr + 1 Where RfqID = '" + RfqId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult resPreview = MessageBox.Show(RfqId + Environment.NewLine + "Document already previewed!" + Environment.NewLine + "Allow document to be previewed again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (resPreview == DialogResult.Yes)
                            {
                                Query = "Update [dbo].[RequestForQuotationH] Set [PreviewC] = '0' Where RfqID = '" + RfqId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                DialogResult resPreviewPrompt = MessageBox.Show("Do you want to preview document now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (resPreviewPrompt == DialogResult.Yes)
                                {
                                    if (TransType == "FIX")
                                    {
                                        Detail = "Fix";
                                    }
                                    else if (TransType != "FIX")
                                    {
                                        DialogResult resPreviewDetail = MessageBox.Show("Rinci / Tidak", "Rinci / Tidak", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                        if (resPreviewDetail == DialogResult.Yes)
                                        {
                                            Detail = "Rinci";
                                        }
                                        else
                                        {
                                            Detail = "Tidak Rinci";
                                        }
                                    }

                                    GlobalPreview f = new GlobalPreview("Request For Quotation", RfqId);
                                    f.SetMode(Detail);
                                    f.Show();

                                    //Set PreviewC to 1
                                    Query = "Update [dbo].[RequestForQuotationH] Set [PreviewC] = '1', PreviewCtr = PreviewCtr + 1 Where RfqID = '" + RfqId + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Document already previed!" + Environment.NewLine + "Contact Administrator");
                        }
                    }

                }
                else if (dgvRFQ.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    Query = "Select SendEmailC From [RequestForQuotationH] Where [RfqID] = '" + RfqId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    int SendEmailC = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (SendEmailC == 0)
                    {
                        //SendEmail s = new SendEmail(this);
                        //s.flag(RfqId);
                        //s.Show();

                        GlobalSendEmail f = new GlobalSendEmail("Request For Quotation", RfqId, VendID);
                        f.Show();
                    }
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult resSendEmail = MessageBox.Show(RfqId + Environment.NewLine + "Email already been Sent!" + Environment.NewLine + "Allow Email to be sent again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (resSendEmail == DialogResult.Yes)
                            {
                                Query = "Update [dbo].[RequestForQuotationH] Set [SendEmailC] = '0' Where RfqID = '" + RfqId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                DialogResult resSendEmailPrompt = MessageBox.Show("Do you want to resend email now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (resSendEmailPrompt == DialogResult.Yes)
                                {
                                    //SendEmail s = new SendEmail(this);
                                    //s.flag(RfqId);
                                    //s.Show();

                                    GlobalSendEmail f = new GlobalSendEmail("Request For Quotation", RfqId, VendID);
                                    f.Show();
                                }                                
                            }                            
                        }
                        else
                        {
                            MessageBox.Show("Email already been Sent!" + Environment.NewLine + "Contact Administrator");
                        }
                    }
                }
            }
        }


        private void btnGenerateQuot_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            Purchase.PurchaseQuotation.FormPQ x = new Purchase.PurchaseQuotation.FormPQ();
            if (x.PermissionAccess(ControlMgr.New) > 0)
            {
                if (dgvRFQ.Rows.Count == 0) { return; }
                string RfqId = dgvRFQ.CurrentRow.Cells["RfqId"].Value.ToString();
                string PurchReqId = dgvRFQ.CurrentRow.Cells["PurchReqId"].Value.ToString();
                string TransType = dgvRFQ.CurrentRow.Cells["TransType"].Value.ToString();

                Query = "Select RfqId from [dbo].[PurchQuotationH] where RfqID = '" + RfqId + "'";

                Conn = ConnectionString.GetConnection();

                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())
                    {
                        MessageBox.Show("RFQID sudah pernah input Quotation..");
                    }
                    else
                    {
                       
                        //CreatePQ x = new CreatePQ();
                       // x.ModeNew();
                        x.SetMode("New", "");
                        x.SetParentRFQ(this);
                        x.Show();
                        x.CreateNew(RfqId, PurchReqId,TransType);
                        
                    }
                    Dr.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
            
        }

        private void dgvRFQ_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dgvRFQ.Columns[e.ColumnIndex].Name.ToString() == "VendId")
                {
                    vendID = dgvRFQ.Rows[e.RowIndex].Cells["VendId"].Value.ToString();
                    PopUp.Vendor.Vendor f = new PopUp.Vendor.Vendor();
                    f.Close();
                    f = new PopUp.Vendor.Vendor();
                    f.Show();
                }

            }
        }

        public static string vendID;
        public string VendID
        {
            get { return vendID; }
            set { vendID = value; }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01','02'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'03','04','05'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }
    }
}
