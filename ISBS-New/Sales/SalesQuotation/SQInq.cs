using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;

//BY: HC
namespace ISBS_New.Sales.SalesQuotation
{
    public partial class SQInq : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private TransactionScope scope;

        //begin
        //created by : joshua 
        //created date : 12 feb 2018 
        //description : inisialisasi variable
        private string TransStatus = String.Empty;
        //end

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        public static int dataShow;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public SQInq()
        {
            InitializeComponent();
        }

        private void SQInq_Load(object sender, EventArgs e)
        {
            TransStatus = "'01', '02', '03', '05', '06', '22', '23'";
            addCmbCrit();
            cmbShowLoad();
            ModeLoad();
            checkSQValidity();
            RefreshGrid();
            lblForm.Location = new Point(16, 11);

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void checkSQValidity()
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                Query = "update SalesQuotationH set TransStatus = '11', updatedDate = getdate(), updatedBy = 'SYSTEM' where ValidTo < DATEADD(day,-1,GETDATE()) and TransStatus in ('01', '02', '05', '07', '21', '22')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
                Trans.Commit();
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Conn.Close();
            }
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'SalesQuotationH'";

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
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;
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

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            //Query = "select count(FieldName) from [User].[Table] where TableName = 'SalesQuotationH'";
            //Cmd = new SqlCommand(Query, Conn);
            //int countField = (Int32)Cmd.ExecuteScalar();

            //Query = "select FieldName from [User].[Table] where TableName = 'SalesQuotationH'";
            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();
            //int x = 1;
            //string search = "";
            //while (Dr.Read())
            //{
            //    if (x == 1)
            //        search += "where ";
            //    if (countField > 1 && x != 1)
            //        search += "or ";
            //    search += Dr[0].ToString() + " like '%" + txtSearch.Text + "%' ";
            //    if (x == countField)
            //        search += " ) a ";
            //    x++;
            //}

            //begin
            //updated by : joshua 
            //updated date : 12 feb 2018 
            //description : add condition trans status on progress or completed
            if (TransStatus == String.Empty)
            {
                TransStatus = "'01', '02', '03', '05', '06', '22', '23'"; Limit1 = 1; Limit2 = dataShow;
            }
            Query = "select * from (select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) No, * from (select [SalesQuotationNo] ,[OrderDate] ,[TransType] ,[RefTransId], [SalesMouNo], b.Deskripsi 'TransStatus Deskripsi',[CustName],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate] from [ISBS-NEW4].[dbo].[SalesQuotationH] left join TransStatusTable b on SalesQuotationH.TransStatus = b.StatusCode where [TransStatus] != '08' and b.TransCode = 'SalesQuotation' AND TransStatus IN (" + TransStatus + ") ";
            //end

            if (crit == null)
                Query += ")a )a ";
            else if (crit.Equals("All"))
            {
                Query += " and (SalesQuotationNo like '%" + txtSearch.Text + "%' or OrderDate like '%" + txtSearch.Text + "%' or SalesMouNo like '%" + txtSearch.Text + "%' or CustName like '%" + txtSearch.Text + "%' or b.Deskripsi like '%" + txtSearch.Text + "%') )a )a ";
            }
            else if (crit.Equals("SalesQuotationNo"))
            {
                Query += " and (SalesQuotationNo like '%" + txtSearch.Text + "%') )a )a ";
            }
            else if (crit.Equals("OrderDate"))
            {
                Query += " and (OrderDate like '%" + txtSearch.Text + "%') )a )a ";
            }
            else if (crit.Equals("SalesMouNo"))
            {
                Query += " and (SalesMouNo like '%" + txtSearch.Text + "%') )a )a ";
            }
            else if (crit.Equals("CustID"))
            {
                Query += " and (CustName like '%" + txtSearch.Text + "%') )a )a ";
            }
            else if (crit.Equals("TransStatus"))
            {
                Query += " and (b.Deskripsi like '%" + txtSearch.Text + "%') )a )a ";
            }

            Query += "Where a.No Between " + Limit1 + " and " + Limit2 + " ;";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            //STEVEN EDIT START
            DataGridViewButtonColumn buttonpreview = new DataGridViewButtonColumn();
            buttonpreview.Name = "Preview";
            buttonpreview.HeaderText = "Preview";
            buttonpreview.Text = "Preview";
            buttonpreview.UseColumnTextForButtonValue = true;
            //STEVEN EDIT END

            DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            buttonSend.Name = "Send Email";
            buttonSend.HeaderText = "Send Email";
            buttonSend.Text = "Send Email";
            buttonSend.UseColumnTextForButtonValue = true;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = Dt;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells["UpdatedDate"].Value != (object)DBNull.Value)
                {
                    if (Convert.ToDateTime(dataGridView1.Rows[i].Cells["UpdatedDate"].Value) == new DateTime(1753, 1, 1))
                        dataGridView1.Rows[i].Cells["UpdatedDate"].Value = (object)DBNull.Value;
                }
            }

            //STEVEN EDIT START
            if (!dataGridView1.Columns.Contains("Preview"))
                dataGridView1.Columns.Add(buttonpreview);
            //STEVEN EDIT END
            if (!dataGridView1.Columns.Contains("Send Email"))
                dataGridView1.Columns.Add(buttonSend);
            dataGridView1.Refresh();
            dataGridView1.AutoResizeColumns();

            //begin
            //updated by : joshua 
            //updated date : 12 feb 2018 
            //description : add condition trans status on progress or completed
            Query = "Select Count([SalesQuotationNo]) From ( Select [SalesQuotationNo] From [dbo].[SalesQuotationH] where [TransStatus] != '08' AND TransStatus IN (" + TransStatus + ") ";
            //end

            if (crit == null)
                Query += ") a;";
            else if (crit.Equals("All"))
            {
                Query += "and ( SalesQuotationNo like '%" + txtSearch.Text + "%' or OrderDate like '%" + txtSearch.Text + "%' or SalesMouNo like '%" + txtSearch.Text + "%' or CustID like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' ) ) a ";
            }
            else if (crit.Equals("SalesQuotationNo"))
            {
                Query += "and SalesQuotationNo like '%" + txtSearch.Text + "%' ) a ";
            }
            else if (crit.Equals("OrderDate"))
            {
                Query += "and OrderDate like '%" + txtSearch.Text + "%' ) a ";
            }
            else if (crit.Equals("SalesMouNo"))
            {
                Query += "and SalesMouNo like '%" + txtSearch.Text + "%' ) a ";
            }
            else if (crit.Equals("CustID"))
            {
                Query += "and CustID like '%" + txtSearch.Text + "%' ) a ";
            }
            else if (crit.Equals("TransStatus"))
            {
                Query += "and TransStatus like '%" + txtSearch.Text + "%' ) a ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (dataShow != 0)
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
            Conn.Close();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                //SQHeader F = new SQHeader();
                SQHeader2 F = new SQHeader2();
                F.SetParent(this);
                F.SetMode("New", "");
                F.Show();
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPR();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //crit = null;
            txtSearch.Text = "";
            ModeLoad();

            cmbCriteria.SelectedItem = "All";
            //if (cmbCriteria.SelectedIndex == -1)
            //    crit = "All";
            //else
            //    crit = cmbCriteria.SelectedItem.ToString();

            RefreshGrid();

        }


        #region SelectPR
        private void SelectPR()
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            //SQHeader f = new SQHeader();                   
            SQHeader2 f = new SQHeader2();
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dataGridView1.RowCount > 0)
                {
                    f.SetMode("BeforeEdit", dataGridView1.CurrentRow.Cells["SalesQuotationNo"].Value.ToString());
                    f.SetParent(this);
                    f.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }
        #endregion

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectPR();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
                crit = "All";
            else
                crit = cmbCriteria.SelectedItem.ToString();

            RefreshGrid();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            cmbShow_SelectedIndexChanged(new object(), new EventArgs());
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

        //Begin Steven Edit
        private void insertStatusLogDelete(DataGridViewRow a)
        {
            DataGridViewRow r = a;
            Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
            Query += " VALUES ('SO Inq','" + dataGridView1.Rows[r.Index].Cells["SalesQuotationNo"].Value.ToString() + "' ,' PK2Test', 'PK3Test', 'PK4Test', '04'";
            Query += ",'Deleted/Closed','" + ControlMgr.UserId + "' , GetDate())";
            SqlCommand Cmd2 = new SqlCommand(Query, Conn, Trans);
            Cmd2.ExecuteNonQuery();
        }
        //End Steven Edit

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("Select [TransStatus] from [ISBS-NEW4].[dbo].[SalesQuotationH] where [SalesQuotationNo] = '" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["SalesQuotationNo"].Value + "'", Conn);
                string transStatus = Cmd.ExecuteScalar().ToString();

                if (transStatus == "03" || transStatus == "23" || transStatus == "04" || transStatus == "07" || transStatus == "21" || transStatus == "11" || transStatus == "09" || transStatus == "10")
                {
                    MetroFramework.MetroMessageBox.Show(this, "Cannot delete data!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to delete " + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["SalesQuotationNo"].Value.ToString() + "?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                    {
                        Query = "update [dbo].[SalesQuotationH] set [TransStatus] = '08' where [SalesQuotationNo] = '" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["SalesQuotationNo"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();

                        insertStatusLogDelete(r);
                    }
                }
                Conn.Close();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private Boolean previewable()
        {
            if (Status == "01" || Status == "03" || Status == "07" || Status == "09" || Status == "10" || Status == "21" || Status == "23")
                return true;
            else
            {
                MessageBox.Show("File cannot be previewed, check TransStatus");
                return false;
            }
        }

        private Boolean checkStatusPreviewEmail(string Action)
        {
            bool vBol = false;
            if (Action == "Preview")
                if (Status == "01" || Status == "03" || Status == "07" || Status == "09" || Status == "10" || Status == "21" || Status == "23")
                    vBol = true;
                else
                {
                    vBol = false;
                    MessageBox.Show("Cannot preview, check status");
                }


            if (Action == "Email")
                if (Status == "01" || Status == "03" || Status == "07" || Status == "22")
                    vBol = true;
                else
                {
                    vBol = false;
                    MessageBox.Show("Cannot send email, check status");
                }

            return vBol;
        }

        //int PreviewC;
        string Status;
        string CustID;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                Conn = ConnectionString.GetConnection();
                string SQId = dataGridView1.Rows[e.RowIndex].Cells["SalesQuotationNo"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["SalesQuotationNo"].Value.ToString();

                Cmd = new SqlCommand("SELECT [CustID] FROM [SalesQuotationH] WHERE [SalesQuotationNo] = '" + SQId + "'", Conn);
                CustID = Cmd.ExecuteScalar().ToString();

                Cmd = new SqlCommand("Select TransStatus From [SalesQuotationH] Where [SalesQuotationNo] = '" + SQId + "'", Conn);
                Status = Cmd.ExecuteScalar().ToString();

                if (dataGridView1.Columns[e.ColumnIndex].Name == "Preview")
                {
                    if(!(checkStatusPreviewEmail("Preview")))
                        return;

                    ListMethod.restrictedPreviewEmail("Preview", "Sales Quotation", "SalesQuotationH", "SalesQuotationNo", SQId, "");
                }

                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    if (!(checkStatusPreviewEmail("Email")))
                        return;

                    ListMethod.restrictedPreviewEmail("Email", "Sales Quotation", "SalesQuotationH", "SalesQuotationNo", SQId, CustID);
               
                    RefreshGrid();
                }
            }
            Conn.Close();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("PPH") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("PPN") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DPPercent"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Total") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExchRate") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DPAmount"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "CreatedDate" || dataGridView1.Columns[e.ColumnIndex].Name == "UpdatedDate")
            {
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            }
            else if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            //begin
            //created by : joshua 
            //created date : 12 feb 2018 
            //description : add button on progress
            TransStatus = "'01', '02', '03', '05', '06', '22', '23'"; //BY: HC stats 01 on progress ke completed 21.02.2018
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
            //end
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            //begin
            //created by : joshua 
            //created date : 12 feb 2018 
            //description : add button completed
            TransStatus = "'04', '07', '08', '09', '10', '11', '21'"; //BY: HC stats 01 on progress ke completed 21.02.2018
            RefreshGrid();
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
            //end
        }

        private void cmbCriteria_TextChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text == null)
            {
            }
            else if (cmbCriteria.Text.Contains("Date"))
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
                txtSearch.Enabled = true;
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
                crit = "All";
            else
                crit = cmbCriteria.SelectedItem.ToString();


            if (e.KeyChar == (char)13)
                RefreshGrid();
        }
    }
}
