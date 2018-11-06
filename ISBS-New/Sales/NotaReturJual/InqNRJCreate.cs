using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Sales.NotaReturJual
{
    public partial class InqNRJCreate : MetroFramework.Forms.MetroForm
    {
        public InqNRJCreate()
        {
            InitializeComponent();
        }

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query, crit = null;
        int Total, Limit1, Limit2, Page1, Page2, Index;
        public static int dataShow;
        private string TransStatus = String.Empty;

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

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (timerRefresh == null)
            {

            }
            else
            {
                RefreshGrid();
            }
        }

        private void InqNRJCreate_Load(object sender, EventArgs e)
        {
            btnDelete.Visible = false;
            addCmbStatusCode();
            addCmbCriteria();
            ModeLoad();

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName, DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'NotaReturJualH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[1]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Clear();
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01', '02', '03', '04', '05', '06'";
                }
            }
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='NotaReturJual' AND StatusCode IN (" + TransStatus + ")";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbStatusCode.DisplayMember = "Text";
            cmbStatusCode.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbStatusCode.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[1] + "" });
            }
            cmbStatusCode.SelectedIndex = 0;
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

            cmbCriteria.SelectedIndex = 0;
            cmbStatusCode.SelectedIndex = 0;

            cmbStatusCode.Enabled = false;

            RefreshGrid();
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox]";

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

        public void RefreshGrid()
        {
            string cmbStatusSelected = cmbStatusCode.Text;

            if (cmbStatusSelected == "Waiting for Approval")
            {
                cmbStatusSelected = "01";
            }
            else if (cmbStatusSelected == "Rejected by Sales Manager")
            {
                cmbStatusSelected = "02";
            }
            else if (cmbStatusSelected == "Approved by Sales Manager")
            {
                cmbStatusSelected = "03";
            }
            else if (cmbStatusSelected == "Waiting for Approval by Stock Manager")
            {
                cmbStatusSelected = "04";
            }
            else if (cmbStatusSelected == "Approved by Stock Manager")
            {
                cmbStatusSelected = "05";
            }
            else if (cmbStatusSelected == "Rejected by Stock Manager")
            {
                cmbStatusSelected = "06";
            }
            else if (cmbStatusSelected == "GR in Progress")
            {
                cmbStatusSelected = "07";
            }
            else if (cmbStatusSelected == "GR Completed")
            {
                cmbStatusSelected = "08";
            }
            else
            {
                cmbStatusSelected = "";
            }

            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01', '02', '03', '04', '05', '06'";
                }
            }
             
            Conn = ConnectionString.GetConnection();
            Query = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY NRJ.CreatedDate DESC) AS [No], NRJ.NRJId AS [NRJ No], NRJ.NRJDate AS [NRJ Date], NRJ.NRJMode AS [Mode], ";
            Query += "NRJ.GoodsIssuedId AS [GI No], NRJ.GoodsIssuedDate AS [GI Date], CT.CustName AS [Customer], NRJ.SiteName AS [Warehouse], TST.Deskripsi AS [Status], ";
            Query += "NRJ.CreatedDate AS [Created Date], NRJ.CreatedBy AS [Created By], NRJ.UpdatedDate AS [Updated Date], NRJ.UpdatedBy AS [Updated By] ";
            Query += "FROM NotaReturJualH NRJ LEFT JOIN TransStatusTable TST ON NRJ.TransStatusId = TST.StatusCode LEFT JOIN CustTable CT ON NRJ.CustId = CT.CustId WHERE TransCode = 'NotaReturJual' AND NRJ.TransStatusId IN (" + TransStatus + ") ";
            if (crit == null)
            {
                Query += ") A ";
            }
            else if (crit == "All")
            {
                Query += "AND (NRJ.NRJId LIKE '%" + txtSearch.Text + "%' OR CT.CustName LIKE '%" + txtSearch.Text + "%' OR NRJ.GoodsIssuedId LIKE '%" + txtSearch.Text + "%' OR NRJ.SiteName LIKE '%" + txtSearch.Text + "%' OR NRJ.CreatedBy LIKE '%" + txtSearch.Text + "%')) A ";
            }
            else if (crit == "NRJ No")
            {
                Query += "AND NRJ.NRJId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "NRJ Date")
            {
                Query += "AND (CONVERT(VARCHAR(10),NRJ.NRJDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRJ.NRJDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit == "Customer")
            {
                Query += "AND CT.CustName LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "GI No")
            {
                Query += "AND NRJ.GoodsIssuedId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "Warehouse")
            {
                Query += "AND NRJ.SiteName LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "Created Date")
            {
                Query += "AND (CONVERT(VARCHAR(10),NRJ.CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRJ.CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit == "Created By")
            {
                Query += "AND NRJ.CreatedBy LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit == "Status Name")
            {
                Query += "AND NRJ.TransStatusId LIKE '%" + cmbStatusSelected + "%') A ";
            }
            Query += "WHERE A.[No] BETWEEN " + Limit1 + " AND " + Limit2;

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

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

            dgvNRJ.AutoGenerateColumns = true;
            dgvNRJ.DataSource = Dt;
            dgvNRJ.Refresh();
            dgvNRJ.AutoResizeColumns();
            dgvNRJ.Columns["NRJ Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRJ.Columns["GI Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRJ.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRJ.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            if (!dgvNRJ.Columns.Contains("Preview"))
                dgvNRJ.Columns.Add(buttonpreview);
            if (!dgvNRJ.Columns.Contains("Send Email"))
                dgvNRJ.Columns.Add(buttonSend);

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE TransStatusId IN (" + TransStatus + ") ";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE ";
                Query += "(NRJId LIKE '%" + txtSearch.Text + "%' OR SiteName LIKE '%" + txtSearch.Text + "%' OR CustName LIKE '%" + txtSearch.Text + "%' OR GoodsIssuedId LIKE '%" + txtSearch.Text + "%' OR CreatedBy LIKE '%" + txtSearch.Text + "%') AND TransStatusId IN (" + TransStatus + ") ";
            }
            else if (crit.Equals("NRJ Date"))
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE ";
                Query += "(CONVERT(VARCHAR(10),NRJDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRJDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId IN (" + TransStatus + ") ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatusId IN (" + TransStatus + ") ;";
            }
            else
            {
                Query = "SELECT COUNT(NRJId) FROM NotaReturJualH WHERE TransStatusId IN (" + TransStatus + ") ";
                if (crit.Equals("NRJ No"))
                    Query += "AND NRJId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Warehouse"))
                    Query += "AND SiteName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Customer"))
                    Query += "AND CustName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("GI No"))
                    Query += "AND GoodsIssuedId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Created By"))
                    Query += "AND CreatedBy Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Status Name"))
                    Query += "AND TransStatusId LIKE '%" + cmbStatusSelected + "%' ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            crit = null;
            ModeLoad();   
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if(Application.OpenForms[i].Name == "NRJHeader")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddGI")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddItem")
                    Application.OpenForms[i].Close();
                else if (Application.OpenForms[i].Name == "AddWH")
                    Application.OpenForms[i].Close();
            }
            MainMenu f = new MainMenu();
            f.refreshTaskList();
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

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                try
                {
                    if (dgvNRJ.RowCount > 0)
                    {
                        Index = dgvNRJ.CurrentRow.Index;
                        string NRJId = dgvNRJ.Rows[Index].Cells["NRJ No"].Value == null ? "" : dgvNRJ.Rows[Index].Cells["NRJ No"].Value.ToString();

                        DialogResult dr = MessageBox.Show("NRJ No = " + NRJId + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            //masih belum tahu apa yg harus diupdate
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        NRJHeader H = null;
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                if (H == null || H.Text == "")
                {
                    H = new NRJHeader();
                    H.SetMode("New", "");
                    H.setParent(this);
                    H.Show();
                }
                else if (CheckOpened(H.Name))
                {
                    H.WindowState = FormWindowState.Normal;
                    H.Show();
                    H.Focus();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectNRJ();
        }

        private void SelectNRJ()
        {
            NRJHeader f = new NRJHeader();
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvNRJ.CurrentRow == null)
                {
                    MessageBox.Show("Maaf List Masih Kosong");
                    return;
                }
                else
                {
                    string NRJID = dgvNRJ.CurrentRow.Cells["NRJ No"].Value.ToString();
                    f.SetMode("BeforeEdit", NRJID);
                    f.setParent(this);
                    f.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
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

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }

        private void dgvNRJ_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectNRJ();
            }
        }

        private void dgvNRJ_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.ColumnIndex > -1)
            {
                string NRJId = dgvNRJ.Rows[e.RowIndex].Cells["NRJ No"].Value == null ? "" : dgvNRJ.Rows[e.RowIndex].Cells["NRJ No"].Value.ToString();

                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("SELECT [CustID] FROM [NotaReturJualH] WHERE [NRJId] = '" + NRJId + "'", Conn);
                string CustID = Cmd.ExecuteScalar().ToString();

                if (dgvNRJ.Columns[e.ColumnIndex].Name == "Preview")
                {
                    #region Preview Method

                    Conn = ConnectionString.GetConnection();
                    Query = "Select PreviewC From [NotaReturJualH] Where [NRJId] = '" + NRJId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    int PreviewC = Convert.ToInt32(Cmd.ExecuteScalar());
                    Conn.Close();

                    if (PreviewC == 0)
                    {
                        //PreviewNRJ f = new PreviewNRJ(NRJId);
                        //f.Show();

                        ISBS_New.GlobalPreview f = new ISBS_New.GlobalPreview("Nota Retur Jual", NRJId);
                        f.Show();

                        //Set PreviewC to 1
                        Conn = ConnectionString.GetConnection();
                        Query = "Update [dbo].[NotaReturJualH] Set [PreviewC] = '1' , PreviewCtr = PreviewCtr + 1 Where NRJId = '" + NRJId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult resPreview = MessageBox.Show(NRJId + Environment.NewLine + "Document already previewed!" + Environment.NewLine + "Allow document to be previewed again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (resPreview == DialogResult.Yes)
                            {
                                Conn = ConnectionString.GetConnection();
                                Query = "Update [dbo].[NotaReturJualH] Set [PreviewC] = '0' Where NRJId = '" + NRJId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                                Conn.Close();

                                #region Ask Resend Email Now?
                                DialogResult resPreviewPrompt = MessageBox.Show("Do you want to preview document now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (resPreviewPrompt == DialogResult.Yes)
                                {
                                    //PreviewNRJ f = new PreviewNRJ(NRJId);
                                    //f.Show();

                                    ISBS_New.GlobalPreview f = new ISBS_New.GlobalPreview("Nota Retur Jual", NRJId);
                                    f.Show();

                                    //Set PreviewC to 1
                                    Conn = ConnectionString.GetConnection();
                                    Query = "Update [dbo].[NotaReturJualH] Set [PreviewC] = '1', PreviewCtr = PreviewCtr + 1 Where NRJId = '" + NRJId + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();
                                    Conn.Close();
                                }
                                if (resPreviewPrompt == DialogResult.No)
                                {
                                    //Some task…  
                                }
                                #endregion
                            }
                            if (resPreview == DialogResult.No) //Action not allow email to be sent
                            {
                                //Some task…  
                            }
                        }
                        else
                        {
                            MessageBox.Show("Document already previed!" + Environment.NewLine + "Contact Administrator");
                        }
                    }
                    #endregion
                }

                else if (dgvNRJ.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "Select SendEmailC From [NotaReturJualH] Where [NRJId] = '" + NRJId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    int SendEmailC = Convert.ToInt32(Cmd.ExecuteScalar());
                    Conn.Close();

                    if (SendEmailC == 0)
                    {
                        GlobalSendEmail f = new GlobalSendEmail("Nota Retur Jual", NRJId, CustID);
                        f.Show();

                        //SendEmail s = new SendEmail(this);
                        //s.flag(NRJId);
                        //s.Show();
                    }
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult resSendEmail = MessageBox.Show(NRJId + Environment.NewLine + "Email already been Sent!" + Environment.NewLine + "Allow Email to be sent again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (resSendEmail == DialogResult.Yes)
                            {
                                Conn = ConnectionString.GetConnection();
                                Query = "Update [dbo].[NotaReturJualH] Set [SendEmailC] = '0' Where NRJId = '" + NRJId + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                                Conn.Close();

                                #region Ask Resend Email Now?
                                DialogResult resSendEmailPrompt = MessageBox.Show("Do you want to resend email now?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                if (resSendEmailPrompt == DialogResult.Yes)
                                {
                                    GlobalSendEmail f = new GlobalSendEmail("Nota Retur Jual", NRJId, CustID);
                                    f.Show();

                                    //SendEmail s = new SendEmail(this);
                                    //s.flag(NRJId);
                                    //s.Show();
                                }
                                if (resSendEmailPrompt == DialogResult.No)//Action not sending email immidiately
                                {
                                    //Some task…  
                                }
                                #endregion
                            }
                            if (resSendEmail == DialogResult.No) //Action not allow email to be sent
                            {
                                //Some task…  
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

        private bool CheckOpened(string name)
        {
            FormCollection FC = Application.OpenForms;
            foreach (Form frm in FC)
            {
                if (frm.Name == name)
                {
                    return true;
                }
            }
            return false;
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

            if (cmbCriteria.Text.Contains("Status"))
            {
                cmbStatusCode.Enabled = true;
            }
            else
            {
                cmbStatusCode.Enabled = false;
                cmbStatusCode.SelectedIndex = 0;
            }

            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("Status"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01', '02', '03', '04', '05', '06'";
            addCmbStatusCode();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'07', '08'";
            addCmbStatusCode();
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }
    }
}