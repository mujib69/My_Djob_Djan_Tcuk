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

using System.Threading;

//BY: HC
namespace ISBS_New
{
    public partial class GlobalInquiry : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private TransactionScope scope;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        public static int dataShow;

        string where2;
        private string PK, SchemaName, TableName, Where, fromTableQuery, fieldName, ReferenceType;
        private List<string> headerText;
        string FormName;
        string TransStatus = String.Empty; //FOR BUTTON COMPLETE & ON PROGRESS
        private List<string> HideField;
        bool Distinct = false;
        bool Journal = false;
        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(FormName, Authority);
        }
        //end

        //SET MODE V1.0
        public void SetMode2(string PK, string SchemaName, string TableName, string fromTableQuery, string fieldName, string Where, string Title, string FormName)
        {
            this.PK = PK;
            this.SchemaName = SchemaName;
            this.TableName = TableName;
            this.fromTableQuery = fromTableQuery;
            this.fieldName = fieldName;
            this.Where = Where;
            this.Text = Title;
            lblForm.Text = Title;
            this.FormName = FormName;
        }

        //SET MODE V2.0
        public void SetMode(string PK, string SchemaName, string TableName, string fromTableQuery, string fieldName, string Where, string Title, string FormName, List<string> headerText)
        {
            this.PK = PK;
            this.SchemaName = SchemaName;
            this.TableName = TableName;
            this.fromTableQuery = fromTableQuery;
            this.fieldName = fieldName;
            this.Where = Where;
            this.Text = Title;
            lblForm.Text = Title;
            this.FormName = FormName;
            this.headerText = headerText;
        }

        //SET MODE V3.0
        public void SetMode3(string PK, string SchemaName, string TableName, string fromTableQuery, string fieldName, string Where, string Title, string FormName, List<string> headerText, string Referencetype)
        {
            this.PK = PK;
            this.SchemaName = SchemaName;
            this.TableName = TableName;
            this.fromTableQuery = fromTableQuery;
            this.fieldName = fieldName;
            this.Where = Where;
            this.Text = Title;
            lblForm.Text = Title;
            this.FormName = FormName;
            this.headerText = headerText;
            this.ReferenceType = Referencetype;
        }
        //SET MODE v4.0 + HideField
        public void SetMode4(string PK, string SchemaName, string TableName, string fromTableQuery, string fieldName, string Where, string Title, string FormName, List<string> headerText, string Referencetype, List<string> Hide)
        {
            this.PK = PK;
            this.SchemaName = SchemaName;
            this.TableName = TableName;
            this.fromTableQuery = fromTableQuery;
            this.fieldName = fieldName;
            this.Where = Where;
            this.Text = Title;
            lblForm.Text = Title;
            this.FormName = FormName;
            this.headerText = headerText;
            this.ReferenceType = Referencetype;
            this.HideField = Hide;
        }

        //SET MODE v5.0 + Use Distinct
        public void SetMode5(string PK, string SchemaName, string TableName, string fromTableQuery, string fieldName, string Where, string Title, string FormName, List<string> headerText, string Referencetype, List<string> Hide, bool distinct)
        {
            this.PK = PK;
            this.SchemaName = SchemaName;
            this.TableName = TableName;
            this.fromTableQuery = fromTableQuery;
            this.fieldName = fieldName;
            this.Where = Where;
            this.Text = Title;
            lblForm.Text = Title;
            this.FormName = FormName;
            this.headerText = headerText;
            this.ReferenceType = Referencetype;
            this.HideField = Hide;
            this.Distinct = distinct;
        }

        
        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            //NOTE : Delete for simple form tanpa onprogress/completed.
            string temp = "";
            temp = dataGridView1.CurrentRow.Cells[PK].Value.ToString();

            DialogResult dr = MessageBox.Show(temp + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {              
                Conn = ConnectionString.GetConnection();
                try
                {
                    Query = "Delete from " + SchemaName + "." + TableName + " where " + PK + " = @temp";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.AddWithValue("@temp", temp);
                    Cmd.ExecuteScalar();

                    MessageBox.Show(temp + " berhasil di delete");
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                }
                Conn.Close();                
                RefreshGrid();
            }
        }

        public GlobalInquiry()
        {
            InitializeComponent();
        }

        private void GlobalInquiry_Load(object sender, EventArgs e)
        {
            if (FormName == "DOInq")
                TransStatus = "'01', '02', '03', '05', '06', '09'";
            else if (FormName == "BBKInq")
                TransStatus = "'01', '02', '05', '06'";
            else if (FormName == "SAInq")
                TransStatus = "'01', '03', '05', '06', '08', '09', '11', '12'";
            else if (FormName == "InquiryNotaResize")
                TransStatus = "0";
            else if (FormName == "SOInq")
                TransStatus = "'01', '03', '05', '06', '08', '09', '10', '12'";
            else if (FormName == "RVInq")
                TransStatus = "'01','03'";
            else if (FormName == "FQA" || FormName == "COA" || FormName == "MASTERCOA" || FormName == "FQA2" || FormName == "JOURNAL")
                TransStatus = "'Gunakan'";
            else if (FormName == "GLJournal")
                TransStatus = "'0'";
            else if (FormName == "CreditLimitInq")
                TransStatus = "'01'";

            addCmbCrit();
            cmbShowLoad();
            ModeLoad();
            checkFormValidity(); //SALES AGREEMENT
            RefreshGrid();
            //RefreshGrid2();
            metroGrid1.CellBorderStyle= DataGridViewCellBorderStyle.Single;

            btnOnProgress.BackColor = Color.Black;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void checkFormValidity()
        {
            try
            {
                using (scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    if (FormName == "SAInq")
                    {
                        Query = "update SalesAgreementH set TransStatus = '02', UpdatedDate = getdate(), UpdatedBy = 'SYSTEM' where ValidTo < DATEADD(day,-1,GETDATE()) and TransStatus in ('01', '03', '05', '09', '11')";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    else if (FormName == "SOInq")
                    {
                        Query = "update SalesOrderH set TransStatus = '02', UpdatedDate = getdate(), UpdatedBy = 'SYSTEM' where ValidTo < DATEADD(day,-1,GETDATE()) and TransStatus in ('01', '05', '09', '10', '12')";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    Conn.Close();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();

            if (FormName == "InquiryNotaResize" || FormName == "Payment Mode" || FormName == "Term of Payment" || FormName == "Delivery Method" || FormName == "Sales" || FormName == "Counter" || FormName == "Bank Group" || FormName == "Bank" || FormName == "Currency" || FormName == "Exchange Rate" || FormName == "RVInq" || FormName == "JOURNAL" || FormName == "CreditLimitInq")
            {
                Query = "Select DisplayName From [User].[Table] Where SchemaName = '" + SchemaName + "' And TableName = '" + TableName + "'";
            }
            else
                Query = "Select FieldName From [User].[Table] Where SchemaName = '" + SchemaName + "' And TableName = '" + TableName + "'";

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

            if (FormName == "InquiryNotaResize")
            {
                btnCompleted.Text = "Posted";
                btnOnProgress.Text = "Unposted";
                btnNew.Visible = false;
                btnClosed.Visible = false;
            }

            if (FormName == "BBKInq")
            {
                btnUbahReceiptDate.Visible = true;
            }
            else
            {
                btnUbahReceiptDate.Visible = false;
            }

            if (FormName == "Payment Mode" || FormName == "Term of Payment" || FormName == "Delivery Method" || FormName == "Sales" || FormName == "Counter" || FormName == "Bank Group" || FormName == "Bank" || FormName == "Currency" || FormName == "Exchange Rate")
            {
                btnDelete.Visible = true;
            }
            else
            {
                btnDelete.Visible = false;
            }

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date.AddDays(1);
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

        public void RefreshGrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            Conn = ConnectionString.GetConnection();
            if (FormName == "DOInq")
            {
                where2 = " and a.DeliveryOrderStatus in (" + TransStatus + ") ";
            }
            else if (FormName == "BBKInq")
            {
                where2 = " and a.StatusCode in (" + TransStatus + ") ";
            }
            else if (FormName == "SAInq")
            {
                where2 = " and a.TransStatus in (" + TransStatus + ") ";
            }
            else if (FormName == "InquiryNotaResize")
            {
                where2 = " and Posted in (" + TransStatus + ") ";
            }
            else if (FormName == "SOInq")
                where2 = " and a.TransStatus in (" + TransStatus + ") ";
            else if (FormName == "RVInq")
            {
                where2 = " and a.StatusCode in ("+TransStatus+") ";
            }
            else if (FormName == "FQA" || FormName == "COA" || FormName == "MASTERCOA" || FormName == "FQA2" || FormName == "JOURNAL")
            {
                where2 = " and a.Status in (" + TransStatus + ") ";
            }
            else if (FormName == "GLJournal")
            {
                where2 = " and a.Posting in (" + TransStatus + ") ";
            }
            else if (FormName == "CreditLimitInq")
            {
                where2 = "AND a.StatusCode in (" + TransStatus + ") ";
            }

            if (Distinct == true)
            {
                Query = "select * from (select ROW_NUMBER() OVER (order by "+PK+" DESC) No, a.* from (select DISTINCT " + fieldName + " from " + fromTableQuery + " where " + Where + where2 + " ";
            }
            else
            {
                Query = "select * from (select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) No, a.* from (select " + fieldName + " from " + fromTableQuery + " where " + Where + where2 + " ";
            }

            if (FormName == "InquiryNotaResize" || FormName == "Payment Mode" || FormName == "Term of Payment" || FormName == "Delivery Method" || FormName == "Sales" || FormName == "Counter" || FormName == "Bank Group" || FormName == "Bank" || FormName == "Currency" || FormName == "Exchange Rate" || FormName == "RVInq")
            {              
                if (crit == null)
                {
                    Query += ") a ";
                }
                else if (crit.Equals("All"))
                {
                    for (int i = 1; i < cmbCriteria.Items.Count; i++)
                    {
                        string criteria = "";
                        string CriteriaQuery = "SELECT FieldName From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.Items[i] + "' ";
                        using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
                        {
                            criteria = Cmd3.ExecuteScalar().ToString();
                        }
                        if (i == 1)
                            Query += "and ( ";
                        if (i > 1 && i != 1)
                            Query += " or ";
                        Query += " " + criteria + " like @Search ";
                    }
                    Query += ") ) a ";
                }
                else if (crit.Contains("Date"))
                {
                    Query += "and [" + getFieldName(crit) + "] BETWEEN '" + dtFrom.Value.ToString() + "' AND '" + dtTo.Value.ToString() + "' ) a ";
                }
                //Untuk Nota Resize
                else if (crit.Equals("SiteID"))
                {
                    Query = "select * from (select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) No, a.* from (select " + fieldName + " from " + fromTableQuery + " ";
                    Query += " LEFT JOIN [dbo].[InventSite] b ON a.SiteID = b.[InventSiteID] ";
                    Query += "where " + Where + where2 + " ";
                    Query += "AND b.InventSiteName LIKE @Search ";
                    Query += " ) a ";
                }
                else
                {
                    Query += "and [" + getFieldName(crit) + "] like @Search ) a ";
                }
            }
            else
            {
                if (crit == null)
                    Query += ") a ";
                else if (crit.Equals("All"))
                {
                    //Query += "where [" + PK + "] like @Search ) a ";
                    for (int i = 1; i < cmbCriteria.Items.Count; i++)
                    {
                        string criteria = "";
                        string CriteriaQuery = "";
                        if (Distinct == true)
                        {
                            CriteriaQuery = "SELECT DISTINCT(FieldName) From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.Items[i] + "' ";
                        }
                        else
                        {
                            CriteriaQuery = "SELECT FieldName From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.Items[i] + "' ";
                        }
                        using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
                        {
                            criteria = Cmd3.ExecuteScalar().ToString();
                        }
                        if (i == 1)
                            Query += "and ( ";
                        if (i > 1 && i != 1)
                            Query += " or ";
                        Query += " " + criteria + " like @Search ";
                    }
                    Query += ") ) a ";
                }
                else if (crit.Contains("Date"))
                {
                    Query += "and [" + getFieldName(crit) + "] BETWEEN '" + dtFrom.Value.ToString() + "' AND '" + dtTo.Value.ToString() + "' ) a ";
                }
                else
                {
                    Query += "and [" + getFieldName(crit) + "] like @Search ) a ";
                }
            }

            Query += ") a ";
            Query += "Where a.No Between " + Limit1 + " and " + Limit2 + " ;";

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@Search","%"+txtSearch.Text+"%");
            Dt = new DataTable();
            Da.Fill(Dt);

            //STEVEN EDIT START
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
            //STEVEN EDIT END

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = Dt;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Distinct == false)
                {
                    if (dataGridView1.Rows[i].Cells["UpdatedDate"].Value != System.DBNull.Value && dataGridView1.Rows[i].Cells["UpdatedDate"].Value != null)
                    {
                        if (Convert.ToDateTime(dataGridView1.Rows[i].Cells["UpdatedDate"].Value) == new DateTime(1753, 1, 1))
                            dataGridView1.Rows[i].Cells["UpdatedDate"].Value = (object)DBNull.Value;
                    }
                }
            }

            if (FormName != "InquiryNotaResize")
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].HeaderText = headerText[i];
                }

                //STEVEN EDIT START
                if (!dataGridView1.Columns.Contains("Preview"))
                    dataGridView1.Columns.Add(buttonpreview);
                if (!dataGridView1.Columns.Contains("Send Email"))
                    dataGridView1.Columns.Add(buttonSend);
                //STEVEN EDIT END
            }

            dataGridView1.Refresh();
            dataGridView1.AutoResizeColumns();

            //REMARKED BY: HC (S)
            //if (FormName == "DOInq")
            //{
            //    PK = "DeliveryOrderId";
            //}
            //REMARKED BY: HC (E)
            Query = "Select Count([" + PK + "]) From ( Select [" + PK + "] From " + fromTableQuery + " where " + Where + where2;
            if (FormName == "DOInq")
            {
                Query += "and a.DeliveryOrderStatus in (" + TransStatus + ") ";

            }
            else if (FormName == "BBKInq")
            {
                Query += "and a.StatusCode in (" + TransStatus + ") ";
            }
            else if (FormName == "SAInq")
            {
                Query += "and a.TransStatus in (" + TransStatus + ") ";
            }
            else if (FormName == "InquiryNotaResize")
            {
                Query += "and Posted in (" + TransStatus + ") ";
            }
            else if (FormName == "SOInq")
                Query += "and a.TransStatus in (" + TransStatus + ") ";
            else if (FormName == "FQA" || FormName == "COA" || FormName == "MASTERCOA" || FormName == "FQA2" || FormName == "JOURNAL")
                Query += "AND a.Status in (" + TransStatus + ") ";
            else if (FormName == "GLJournal")
                Query += "AND a.Posting in (" + TransStatus + ") ";
            else if (FormName == "CreditLimitInq")
                Query += "AND a.StatusCode in (" + TransStatus + ") ";

            if (crit == null)
                Query += ") a;";
            else if (crit.Equals("All"))
            {
                //Query += "and [" + PK + "] like @Search ) a ";
                for (int i = 1; i < cmbCriteria.Items.Count; i++)
                {
                    string criteria = "";
                    string CriteriaQuery = "SELECT FieldName From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.Items[i] + "' ";
                    using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
                    {
                        criteria = Cmd3.ExecuteScalar().ToString();
                    }

                    if (i == 1)
                        Query += "and ( ";
                    if (i > 1 && i != 1)
                        Query += " or ";
                    Query += " " + criteria + " like @Search ";
                }
                Query += ") ) a ";
            }
            else if (crit.Equals("SiteID"))
            {
                Query = "Select Count([" + PK + "]) From ( Select [" + PK + "], SiteID From " + fromTableQuery + " where " + Where + where2;
                Query += "and Posted in (" + TransStatus + ") ";
                Query += ") a ";
                Query += " LEFT JOIN [dbo].[InventSite] b ON a.[SiteID] = b.[InventSiteID] ";
                Query += "WHERE b.InventSiteName LIKE @Search ;";
            }
            else if (crit.Contains("Date"))
            {
                Query += "and " + getFieldName(crit) + " between '" + dtFrom.Value + "' and '" + dtTo.Value + "' ) a ";
            }
            else
                Query += "and " + getFieldName(crit) + " like @Search ) a ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@Search","%"+txtSearch.Text+"%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (dataShow != 0)
            {
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
                dataShow = 0;
            }
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
            Conn.Close();

            if (FormName == "InquiryNotaResize")
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    string Warehouse = "";
                    string QueryWarehouse = "SELECT [InventSiteName] FROM [dbo].[InventSite] WHERE [InventSiteID] = '" + dataGridView1.Rows[i].Cells["Warehouse"].Value.ToString() + "'";
                    using (Conn = ConnectionString.GetConnection())
                    using (SqlCommand cmd4 = new SqlCommand(QueryWarehouse, Conn))
                    {
                        if (dataGridView1.Rows[i].Cells["Warehouse"].Value != null && dataGridView1.Rows[i].Cells["Warehouse"].Value.ToString() != "")
                        {
                            Warehouse = cmd4.ExecuteScalar().ToString();
                        }
                    }
                    dataGridView1.Rows[i].Cells["Warehouse"].Value = Warehouse;
                }
            }
            dataGridView1.AutoResizeColumns();
            if (FormName == "InquiryNotaResize" && cmbCriteria.Text == "Warehouse" && dataGridView1.Columns.Count > 10 && dataGridView1.Columns["InventSiteName"] != null)
            {
                dataGridView1.Columns["InventSiteName"].Visible = false;
            }

            //Hide
            if (HideField != null)
            {
                if (HideField.Count > 0)
                {
                    for (int i = 0; i < HideField.Count; i++)
                    {
                        dataGridView1.Columns[HideField[i]].Visible = false;
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPR();
        }

        private string getFieldName(string crit)
        {
            string criteria = "";
            string CriteriaQuery = "SELECT FieldName From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + crit + "' ";
            using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
            {
                Dr = Cmd3.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        criteria = Dr["FieldName"].ToString();
                    }
                }
                Dr.Close();
            }
            if (criteria == "")
            {
                criteria = crit;
            }
            return criteria;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectPR();
        }

        public void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                CallForm("New");
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void CallForm(string tmpMode)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access

            string temp = "";

            //REMARKED BY: HC (S) 02.05.18
            //Digunakan saat HeaderGridView Name tidak sesuai dengan nama field table
            //if (FormName == "DOInq")
            //{
            //    PK = "DO No";
            //}
            //Digunakan saat HeaderGridView Name tidak sesuai dengan nama field table
            //REMARKED BY: HC (E)

            if (dataGridView1.RowCount != 0 && FormName != "InquiryNotaResize")
                temp = dataGridView1.CurrentRow.Cells[PK].Value.ToString();
            else if (FormName == "InquiryNotaResize" && dataGridView1.RowCount != 0)
                temp = dataGridView1.CurrentRow.Cells["NRZ No"].Value.ToString();

            if (tmpMode == "New")
                temp = "";
            if (FormName == "DOInq")
            {
                Sales.DeliveryOrder.DOHeader F = new Sales.DeliveryOrder.DOHeader();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "BBKInq")
            {
                Sales.BBK.BBKHeader F;
                if (ReferenceType == "NOTA TRANSFER")
                {
                    F = new Sales.BBK.BBKHeader(ReferenceType);
                }
                else
                {
                    F = new Sales.BBK.BBKHeader();
                }

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "SAInq")
            {
                Sales.SalesAgreement.SAHeader F = new Sales.SalesAgreement.SAHeader();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "InquiryNotaResize")
            {
                ISBS_New.Purchase.GoodsReceipt.Resize.FormResizeGR F = new ISBS_New.Purchase.GoodsReceipt.Resize.FormResizeGR();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    string refnumber = dataGridView1.CurrentRow.Cells["GR No"].Value.ToString();
                    DateTime date = new DateTime();
                    date = DateTime.ParseExact(dataGridView1.CurrentRow.Cells["NRZ Date"].Value.ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    F.SetParent(this);
                    F.SetMode("VIEW", temp, refnumber, date);
                    F.dtTransDate.Enabled = false;
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "SOInq")
            {
                Sales.SalesOrder.SOHeader F = new Sales.SalesOrder.SOHeader();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "RVInq")
            {
                AccountsReceivable.ReceiptVoucher.HeaderReceiptVoucher F = new ISBS_New.AccountsReceivable.ReceiptVoucher.HeaderReceiptVoucher();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "FQA")
            {
                AccountAssignment.FormFQA F = new AccountAssignment.FormFQA();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "COA")
            {
                AccountAssignment.FormCOA F = new AccountAssignment.FormCOA();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "MASTERCOA")
            {
                AccountAssignment.FormCOAMaster F = new AccountAssignment.FormCOAMaster();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "FQA2")
            {
                AccountAssignment.FormFQA2 F = new ISBS_New.AccountAssignment.FormFQA2();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "GLJournal")
            {
                AccountAssignment.GLJournal.FormGLJournalHeader F = new ISBS_New.AccountAssignment.GLJournal.FormGLJournalHeader();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "JOURNAL")
            {
                ISBS_New.JournalType.FormJournal.FormJournal F = new ISBS_New.JournalType.FormJournal.FormJournal();
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "Payment Mode")
            {
                Master.Payment_Mode.PaymentMode F = new Master.Payment_Mode.PaymentMode();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "Term of Payment")
            {
                Master.TermOfPayment.ToP F = new Master.TermOfPayment.ToP();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }                
            }
            else if (FormName == "Delivery Method")
            {
                Master.DeliveryMethod.DeliveryForm F = new Master.DeliveryMethod.DeliveryForm();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "Sales")
            {
                Master.Sales.Sales F = new Master.Sales.Sales();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "Counter")
            {
                Master.Counter.Counter F = new Master.Counter.Counter();

                string Kode = dataGridView1.CurrentRow.Cells["Kode"].Value.ToString();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp, Kode);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }           
            else if (FormName == "Exchange Rate")
            {
                Master.ExchangeRate.ExchangeRate F = new Master.ExchangeRate.ExchangeRate();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "Currency")
            {
                Master.Currency.Currency F = new Master.Currency.Currency();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else if (FormName == "CreditLimitInq")
            {
                Sales.CreditLimit.CreditLimitHeader F = new Sales.CreditLimit.CreditLimitHeader();

                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.SetParent(this);
                    F.SetMode(tmpMode, temp);
                    F.Show();
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
        }

        private void SelectPR()
        {
            if (dataGridView1.RowCount > 0)
            {
                CallForm("BeforeEdit");
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            crit = null;
            cmbCriteria.SelectedItem = "All";
            ModeLoad();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
                crit = "All";
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();

                if (FormName == "InquiryNotaResize" || FormName == "Payment Mode" || FormName == "Term of Payment" || FormName == "Delivery Method" || FormName == "Sales" || FormName == "Counter" || FormName == "Bank Group" || FormName == "Bank")
                {
                    string criteria = "";
                    if (crit != "All")
                    {
                        string CriteriaQuery = "SELECT FieldName From [User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.SelectedItem.ToString() + "'";
                        using (Conn = ConnectionString.GetConnection())
                        using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
                        {
                            criteria = Cmd3.ExecuteScalar().ToString();
                        }
                        crit = criteria;
                    }
                }
            }
            txtPage.Text = "1";
            Limit1 = 1;
            Limit2 = 10;
            RefreshGrid();
            //RefreshGrid2();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
            //RefreshGrid2();
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
            //RefreshGrid2();
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
            //RefreshGrid2();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
            //RefreshGrid2();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Status"))
                dataGridView1.Columns[e.ColumnIndex].Visible = false;

            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("PPH") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("PPN") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DPPercent") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Total") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExchRate") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DPAmount"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            //if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Total") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExchRate") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DPAmount"))
            //{
            //    if (e.Value == "" || e.Value == null)
            //        e.Value = "0";
            //    double d = double.Parse(e.Value.ToString());
            //    e.Value = d.ToString("N4");
            //}

            //ALLIGNMENT
            if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("PPH") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("PPN") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DPPercent") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Total") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ExchRate") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("DPAmount") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Timbang1Weight") || dataGridView1.Columns[e.ColumnIndex].Name.Contains("Timbang2Weight"))
            {
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            //DATE FORMAT
            if (dataGridView1.Columns[e.ColumnIndex].Name == "CreatedDate" || dataGridView1.Columns[e.ColumnIndex].Name == "UpdatedDate" || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ValidTo"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            else if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                DialogResult result = DialogResult.Ignore;
                if (FormName == "DOInq")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select DeliveryOrderStatus from DeliveryOrderH where DeliveryOrderId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar().ToString() == "06")
                        result = MetroFramework.MetroMessageBox.Show(this, "Cannot close printed DO! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    Conn.Close();
                }
                else if (FormName == "BBKInq")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select StatusCode from GoodsIssuedH where GoodsIssuedId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar().ToString() == "03" || Cmd.ExecuteScalar().ToString() == "04" || Cmd.ExecuteScalar().ToString() == "06")
                        result = MetroFramework.MetroMessageBox.Show(this, "Cannot close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    Conn.Close();
                }
                else if (FormName == "SAInq")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select TransStatus from SalesAgreementH where SalesAgreementNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar().ToString() == "02" || Cmd.ExecuteScalar().ToString() == "04" || Cmd.ExecuteScalar().ToString() == "06" || Cmd.ExecuteScalar().ToString() == "07" || Cmd.ExecuteScalar().ToString() == "10")
                        result = MetroFramework.MetroMessageBox.Show(this, "Cannot close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    Conn.Close();
                }
                else if (FormName == "SOInq")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select TransStatus from SalesOrderH where SalesOrderNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar().ToString() == "02" || Cmd.ExecuteScalar().ToString() == "04" || Cmd.ExecuteScalar().ToString() == "07" || Cmd.ExecuteScalar().ToString() == "11")
                        result = MetroFramework.MetroMessageBox.Show(this, "Cannot close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    Conn.Close();
                }
                else if (FormName == "RVInq")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select [StatusCode] from [ReceiptVoucher_H] where [RV_No] = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar().ToString() == "02" || Cmd.ExecuteScalar().ToString() == "03" )
                        result = MetroFramework.MetroMessageBox.Show(this, "Cannot close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "! ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to close " + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + " ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    Conn.Close();
                }
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (FormName == "RVInq")
                        {
                            
                            using (scope = new TransactionScope())
                            {
                                Conn = ConnectionString.GetConnection();
                                //Check status rv
                                Query = "SELECT [StatusCode] FROM [ReceiptVoucher_H] WHERE [RV_No] = @RV_No;";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Cmd.Parameters.AddWithValue("@RV_No", dataGridView1.CurrentRow.Cells[PK].Value.ToString());
                                    //Jika status RV 01, maka boleh di close
                                    if (Cmd.ExecuteScalar().ToString() == "01")
                                    {
                                        //Close RV
                                        Query = "UPDATE [ReceiptVoucher_H] SET StatusCode='XX',UpdatedDate = getdate(), UpdatedBy = '"+ControlMgr.UserId+"' WHERE [RV_No] = @RV_No;";
                                        using (Cmd = new SqlCommand(Query, Conn))
                                        {
                                            Cmd.Parameters.AddWithValue("@RV_No", dataGridView1.CurrentRow.Cells[PK].Value.ToString());
                                            Cmd.ExecuteNonQuery();
                                        }

                                        //Begin
                                        //Created By : Joshua
                                        //Created Date : 06 Sep 2018
                                        //Desc : Closed Journal
                                        BatalJournal(Conn, dataGridView1.CurrentRow.Cells[PK].Value.ToString());
                                        if (Journal == true)
                                        {
                                            Journal = false;
                                            Conn.Close();
                                            goto Outer;
                                        }
                                        //End
                                    }
                                    else
                                    {
                                        MessageBox.Show("Receipt Voucher sudah tidak bisa diclose.");
                                        return;
                                    }
                                }
                                Conn.Close();
                                scope.Complete();
                            }
                            
                        }
                        else if (FormName == "DOInq")
                        {
                            Conn = ConnectionString.GetConnection();
                            Trans = Conn.BeginTransaction();
                            //MAINTAIN SO REMAINING QTY FROM DELETED DO
                            Query = "select SalesOrderId, SalesOrderSeqNo, Qty from DeliveryOrderD where DeliveryOrderId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            string SalesOrderId = "", SalesOrderSeqNo = "";
                            decimal qty = 0, SOQty = 0;
                            while (Dr.Read())
                            {
                                SalesOrderId = Dr["SalesOrderId"].ToString();
                                SalesOrderSeqNo = Dr["SalesOrderSeqNo"].ToString();
                                qty = (Decimal)Dr["Qty"];

                                Query = "Select RemainingQty from SalesOrderD where SalesOrderNo = '" + SalesOrderId + "' and SeqNo = '" + SalesOrderSeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                SOQty = (Decimal)Cmd.ExecuteScalar();
                                qty = SOQty + qty;

                                Query = "update SalesOrderD set RemainingQty = '" + qty + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + SalesOrderId + "' and SeqNo = '" + SalesOrderSeqNo + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                            }
                            Dr.Close();

                            //GET SO STATUS BY COMPARING QTY & REMAINING QTY
                            string status = "";
                            Query = "Select Qty, RemainingQty from SalesOrderD where SalesOrderNo = '" + SalesOrderId + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                if ((Decimal)Dr["Qty"] != (Decimal)Dr["RemainingQty"])
                                {
                                    status = "06"; //DO Partial
                                    break;
                                }
                                else
                                    status = "08"; //Created - DO Deleted
                            }
                            Dr.Close();

                            Query = "update SalesOrderH set TransStatus = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + SalesOrderId + "'";
                            Query += "update DeliveryOrderH set DeliveryOrderStatus = '07', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where DeliveryOrderId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            updateInventSalesQty();
                            updateInventOnHand();
                            updateInventLockTable();
                            deleteInventTrans();
                            updateDOIssuedLogTable();

                            ListMethod.StatusLogCustomer("GlobalInquiry", "DO", dataGridView1.CurrentRow.Cells["Customer"].Value.ToString(), "07", "", dataGridView1.CurrentRow.Cells[PK].Value.ToString(), "", "", "");


                            Trans.Commit();
                            Conn.Close();
                        }
                        else if (FormName == "BBKInq")
                        {
                            using (scope = new TransactionScope())
                            {
                                Conn = ConnectionString.GetConnection();
                                Query = "select b.RefTransID, b.RefTransSeqNo, b.Qty, b.Qty_Actual, a.StatusCode from GoodsIssuedH a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId where a.GoodsIssuedId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Dr = Cmd.ExecuteReader();
                                decimal oldQty = 0, newQty = 0;
                                string pk = "", tableName = "";
                                while (Dr.Read())
                                {
                                    switch (Dr["RefTransID"].ToString().Split('/')[0])
                                    {
                                        case "DO": //DELIVERY ORDER
                                            pk = "DeliveryOrderId";
                                            tableName = "DeliveryOrderD";
                                            Query = "select RemainingQty from " + tableName + " where " + pk + " = '" + Dr["RefTransID"].ToString() + "' and SeqNo = '" + Dr["RefTransSeqNo"].ToString() + "'";
                                            break;
                                        case "NT": //NOTA TRANSFER
                                            pk = "TransferNo";
                                            tableName = "NotaTransferD";
                                            Query = "select RemainingQty from " + tableName + " where " + pk + " = '" + Dr["RefTransID"].ToString() + "' and SeqNo = '" + Dr["RefTransSeqNo"].ToString() + "'";
                                            break;
                                        case "NRB": //Nota Retur Beli
                                            pk = "RTBId";
                                            tableName = "ReturTukarBarangD";
                                            Query = "select RemainingQty from " + tableName + " where " + pk + " = '" + Dr["RefTransID"].ToString() + "' and SeqNo = '" + Dr["RefTransSeqNo"].ToString() + "'";
                                            break;
                                    }
                                    Cmd = new SqlCommand(Query, Conn);
                                    oldQty = Convert.ToDecimal(Cmd.ExecuteScalar());
                                    if (Dr["StatusCode"].ToString() == "01")
                                        newQty = oldQty + Convert.ToDecimal(Dr["Qty"]);
                                    else
                                        newQty = oldQty + Convert.ToDecimal(Dr["Qty_Actual"]);
                                    Query = "update " + tableName + " set RemainingQty = '" + newQty + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where " + pk + " = '" + Dr["RefTransID"].ToString() + "' and SeqNo = '" + Dr["RefTransSeqNo"].ToString() + "'; ";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();
                                }
                                Dr.Close();

                                Query = "update GoodsIssuedH set StatusCode = '04', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.GroupName + "' where GoodsIssuedId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                ListMethod.StatusLogCustomer("GlobalInquiry", "GI", "", "04", "", dataGridView1.CurrentRow.Cells[PK].Value.ToString(), "", "", "");

                                Conn.Close();
                                scope.Complete();
                            }
                        }
                        else if (FormName == "SAInq")
                        {
                            using (scope = new TransactionScope())
                            {
                                Conn = ConnectionString.GetConnection();
                                //GET SQ ID FROM SA
                                Query = "select SalesQuotationNo from SalesAgreementH where SalesAgreementNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                string SQID = "";
                                if (Cmd.ExecuteScalar() != null)
                                {
                                    //UPDATE SQ STATUS TO WAITING FOR APPROVAL
                                    SQID = Cmd.ExecuteScalar().ToString();
                                    Query = "update SalesQuotationH set TransStatus = '02' where SalesQuotationNo = '" + Cmd.ExecuteScalar().ToString() + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();
                                }

                                //UPDATE SA STATUS TO DELETED/CLOSED
                                Query = "update SalesAgreementH set TransStatus = '04', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                                Conn.Close();
                                scope.Complete();
                            }
                        }
                        else if (FormName == "SOInq")
                        {
                            using (scope = new TransactionScope())
                            {
                                Conn = ConnectionString.GetConnection();

                                //Update Credit Limit on Customer Table Database
                                string CUSTId = "";
                                Query = "SELECT [CustID] FROM  [dbo].[SalesOrderH] WHERE [SalesOrderNo] = '" + dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["SalesOrderNo"].Value.ToString() + "'  ";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Dr = Cmd.ExecuteReader();
                                    if (Dr.HasRows)
                                    {
                                        while (Dr.Read())
                                        {
                                            CUSTId = Dr["CustId"].ToString();
                                        }
                                    }
                                }
                                Query = "UPDATE [dbo].[CustTable] SET [Sisa_Limit_Total] -= " + Convert.ToDecimal(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["Total_Nett"].Value) + " WHERE [CustId] = '" + CUSTId + "' ";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                                //=============================================

                                Query = "select SA_SQ_Id from SalesOrderH where SalesOrderNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                string SA_SQ_ID = "";
                                if (Cmd.ExecuteScalar() != null)
                                {
                                    //UPDATE SQ STATUS TO WAITING FOR APPROVAL
                                    SA_SQ_ID = Cmd.ExecuteScalar().ToString();
                                    if (SA_SQ_ID.Split('/')[0] == "SQ")
                                    {
                                        Query = "update SalesQuotationH set TransStatus = '02', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesQuotationNo = '" + Cmd.ExecuteScalar().ToString() + "'";
                                        Cmd = new SqlCommand(Query, Conn);
                                        Cmd.ExecuteNonQuery();
                                    }
                                    else if (SA_SQ_ID.Split('/')[0] == "SA")
                                    {
                                        Query = "update SalesAgreementH set TransStatus = '08', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + Cmd.ExecuteScalar().ToString() + "'";
                                        Cmd = new SqlCommand(Query, Conn);
                                        Cmd.ExecuteNonQuery();

                                        Query = "select TransType from SalesAgreementH where SalesAgreementNo = '" + SA_SQ_ID + "'";
                                        Cmd = new SqlCommand(Query, Conn);
                                        string TransType = Cmd.ExecuteScalar().ToString();

                                        if (TransType == "QUANTITY")
                                        {
                                            Query = "select * from SalesOrderD where SalesOrderNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                            Cmd = new SqlCommand(Query, Conn);
                                            Dr = Cmd.ExecuteReader();
                                            while (Dr.Read())
                                            {
                                                decimal qty = Convert.ToDecimal(Dr["Qty"]);

                                                Query = "select top 1 seqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + SA_SQ_ID + "' and SeqNo < '" + Dr["SA_SQ_SeqNo"] + "' and Base = 'Y' order by SeqNo desc";
                                                Cmd = new SqlCommand(Query, Conn);
                                                int BaseY_SeqNo = Convert.ToInt32(Cmd.ExecuteScalar());

                                                Query = "select RemainingQty from SalesAgreement_Dtl where SalesAgreementNo = '" + SA_SQ_ID + "' and SeqNo = '" + BaseY_SeqNo + "'";
                                                Cmd = new SqlCommand(Query, Conn);
                                                decimal SARemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                                                SARemainingQty += qty;

                                                Query = "update SalesAgreement_Dtl set RemainingQty = '" + SARemainingQty + "', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + SA_SQ_ID + "' and SeqNo = '" + BaseY_SeqNo + "'";
                                                Cmd = new SqlCommand(Query, Conn);
                                                Cmd.ExecuteNonQuery();
                                            }
                                        }
                                        else if (TransType == "AMOUNT")
                                        {
                                            Query = "select * from SalesOrderD where SalesOrderNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                            Cmd = new SqlCommand(Query, Conn);
                                            Dr = Cmd.ExecuteReader();
                                            while (Dr.Read())
                                            {
                                                decimal subtotal = Convert.ToDecimal(Dr["SubTotal"]);
                                                decimal ppn = Convert.ToDecimal(Dr["SubTotal_PPN"]);
                                                decimal pph = Convert.ToDecimal(Dr["SubTotal_PPH"]);
                                                decimal subtotal_Nett = subtotal + ppn + pph;

                                                Query = "select top 1 seqNo from SalesAgreement_Dtl where SalesAgreementNo = '" + SA_SQ_ID + "' and SeqNo < '" + Dr["SA_SQ_SeqNo"] + "' and Base = 'Y' order by SeqNo desc";
                                                Cmd = new SqlCommand(Query, Conn);
                                                int BaseY_SeqNo = Convert.ToInt32(Cmd.ExecuteScalar());

                                                Query = "select RemainingQty from SalesAgreement_Dtl where SalesAgreementNo = '" + SA_SQ_ID + "' and SeqNo = '" + BaseY_SeqNo + "'";
                                                Cmd = new SqlCommand(Query, Conn);
                                                decimal SARemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                                                SARemainingQty += subtotal_Nett;

                                                Query = "update SalesAgreement_Dtl set RemainingQty = '" + SARemainingQty + "', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesAgreementNo = '" + SA_SQ_ID + "' and SeqNo = '" + BaseY_SeqNo + "'";
                                                Cmd = new SqlCommand(Query, Conn);
                                                Cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }

                                //ADJUST QTY
                                Query = "select * from InventLockTable where RefTransId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "' and RefTrans2Id = (select SA_SQ_Id from SalesOrderH where SalesOrderNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "')";
                                Cmd = new SqlCommand(Query, Conn);
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    //DEDUCT INVENT LOCK TABLE
                                    Query = "INSERT INTO [dbo].[InventLockTable] ([RefTransType] ,[RefTransId] ,[RefTrans_SeqNo] ,[RefTrans2Id] ,[RefTrans2_SeqNo] ,[FullItemId] ,[SiteId] ,[Ratio] ,[Lock_Qty] ,[Unit] ,[Lock_Qty_Alt] ,[Unit_Alt] ,[CreatedDate] ,[CreatedBy] ,[UpdatedDate] ,[UpdatedBy]) VALUES ";
                                    Query += "('SALES ORDER', '" + Dr["RefTransId"] + "', '" + Dr["RefTrans_SeqNo"] + "', '" + Dr["RefTrans2Id"] + "', '" + Dr["RefTrans2_SeqNo"] + "', '" + Dr["FullItemId"] + "', '" + Dr["SiteId"] + "', '" + Dr["Ratio"] + "', '" + Convert.ToInt32(Dr["Lock_Qty"]) * -1 + "', '" + Dr["Unit"] + "', '" + Convert.ToInt32(Dr["Lock_Qty_Alt"]) * -1 + "', '" + Dr["Unit_Alt"] + "', getdate(), '" + ControlMgr.UserId + "', '1753-01-01', NULL)";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();

                                    //INVENT ON HAND QTY
                                    decimal Available_For_Sale_UoM = 0;
                                    decimal Available_For_Sale_Reserved_UoM = 0;
                                    decimal Available_For_Sale_Alt = 0;
                                    decimal Available_For_Sale_Reserved_Alt = 0;
                                    Query = "select Available_For_Sale_UoM, Available_For_Sale_Reserved_UoM, Available_For_Sale_Alt, Available_For_Sale_Reserved_Alt from Invent_OnHand_Qty where FullItemId = '" + Dr["FullItemId"] + "' and InventSiteId = '" + Dr["SiteId"] + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    SqlDataReader Dr2 = Cmd.ExecuteReader();
                                    while (Dr2.Read())
                                    {
                                        Available_For_Sale_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]);
                                        Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_UoM"]);
                                        Available_For_Sale_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Alt"]);
                                        Available_For_Sale_Reserved_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Alt"]);
                                    }
                                    Dr2.Close();

                                    Available_For_Sale_UoM += Convert.ToDecimal(Dr["Lock_Qty"]);
                                    Available_For_Sale_Alt += Convert.ToDecimal(Dr["Lock_Qty_Alt"]);
                                    Available_For_Sale_Reserved_UoM -= Convert.ToDecimal(Dr["Lock_Qty"]);
                                    Available_For_Sale_Reserved_Alt -= Convert.ToDecimal(Dr["Lock_Qty_Alt"]);

                                    Query = "update Invent_OnHand_Qty set Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "'";
                                    Query += ", Available_For_Sale_Reserved_UoM = '" + Available_For_Sale_Reserved_UoM + "'";
                                    Query += ", Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "'";
                                    Query += ", Available_For_Sale_Reserved_Alt = '" + Available_For_Sale_Reserved_Alt + "'";
                                    Query += " where FullItemId = '" + Dr["FullItemId"] + "' and InventSiteId = '" + Dr["SiteId"] + "'";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();

                                    //INVENT TRANS
                                    //INVENT TRANS
                                    Cmd = new SqlCommand("select * from InventTrans where TransId = '" + Dr["RefTransId"] + "' and SeqNo = '" + Dr["RefTrans_SeqNo"] + "'", Conn);
                                    Dr2 = Cmd.ExecuteReader();
                                    while (Dr2.Read())
                                    {
                                        Query = "INSERT INTO [dbo].[InventTrans] ([GroupId],[SubGroupId],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteId],[TransId],[SeqNo],[TransDate],[Ref_TransId],[Ref_TransDate],[Ref_Trans_SeqNo],[AccountId],[AccountName],[Available_UoM],[Available_Alt],[Available_Amount],[Available_For_Sale_UoM],[Available_For_Sale_Alt],[Available_For_Sale_Amount],[Available_For_Sale_Reserved_UoM],[Available_For_Sale_Reserved_Alt],[Available_For_Sale_Reserved_Amount],[Notes]) ";
                                        Query += "VALUES (@GroupId ,@SubGroupId ,@SubGroup2Id ,@ItemId ,@FullItemId ,@ItemName ,@InventSiteId ,@TransId ,@SeqNo ,@TransDate ,@Ref_TransId ,@Ref_TransDate ,@Ref_Trans_SeqNo ,@AccountId ,@AccountName ,@Available_UoM ,@Available_Alt ,@Available_Amount ,@Available_For_Sale_UoM ,@Available_For_Sale_Alt ,@Available_For_Sale_Amount ,@Available_For_Sale_Reserved_UoM ,@Available_For_Sale_Reserved_Alt ,@Available_For_Sale_Reserved_Amount ,@Notes)";
                                        Cmd = new SqlCommand(Query, Conn);
                                        Cmd.Parameters.AddWithValue("@GroupId", Dr2["GroupId"]);
                                        Cmd.Parameters.AddWithValue("@SubGroupId", Dr2["SubGroupId"]);
                                        Cmd.Parameters.AddWithValue("@SubGroup2Id", Dr2["SubGroup2Id"]);
                                        Cmd.Parameters.AddWithValue("@ItemId", Dr2["ItemId"]);
                                        Cmd.Parameters.AddWithValue("@FullItemId", Dr2["FullItemId"]);
                                        Cmd.Parameters.AddWithValue("@ItemName", Dr2["ItemName"]);
                                        Cmd.Parameters.AddWithValue("@InventSiteId", Dr2["InventSiteId"]);
                                        Cmd.Parameters.AddWithValue("@TransId", Dr2["TransId"]);
                                        Cmd.Parameters.AddWithValue("@SeqNo", Dr2["SeqNo"]);
                                        Cmd.Parameters.AddWithValue("@TransDate", Dr2["TransDate"]);
                                        Cmd.Parameters.AddWithValue("@Ref_TransId", Dr2["Ref_TransId"]);
                                        Cmd.Parameters.AddWithValue("@Ref_TransDate", Dr2["Ref_TransDate"]);
                                        Cmd.Parameters.AddWithValue("@Ref_Trans_SeqNo", Dr2["Ref_Trans_SeqNo"]);
                                        Cmd.Parameters.AddWithValue("@AccountId", Dr2["AccountId"]);
                                        Cmd.Parameters.AddWithValue("@AccountName", Dr2["AccountName"]);
                                        Cmd.Parameters.AddWithValue("@Available_UoM", Convert.ToDecimal(Dr2["Available_UoM"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_Alt", Convert.ToDecimal(Dr2["Available_Alt"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_Amount", Convert.ToDecimal(Dr2["Available_Amount"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", Convert.ToDecimal(Dr2["Available_For_Sale_Alt"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", Convert.ToDecimal(Dr2["Available_For_Sale_Amount"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM", Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_UoM"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt", Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Alt"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount", Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Amount"]) * -1);
                                        Cmd.Parameters.AddWithValue("@Notes", Dr2["Notes"]);
                                    }
                                    Dr2.Close();
                                    Cmd.ExecuteNonQuery();
                                }
                                Dr.Close();

                                Query = "update SalesOrderH set TransStatus = '04', updatedDate = getdate(), updatedBy = '" + ControlMgr.UserId + "' where SalesOrderNo = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();

                                ListMethod.StatusLogCustomer("GlobalInquiry", "SalesOrder", "", "04", "", dataGridView1.CurrentRow.Cells[PK].Value.ToString(), "", "", "");

                                Conn.Close();
                                scope.Complete();
                            }
                        }
                        MetroFramework.MetroMessageBox.Show(this, dataGridView1.CurrentRow.Cells[PK].Value.ToString() + " Closed!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Outer: ;
                    }
                    catch (Exception ex)
                    {
                        MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end  
        }

        private void BatalJournal(SqlConnection Conn, string RVNo)
        {
            //Begin
            //Created By : Joshua
            //Created Date : 08 Sept 2018
            //Desc : Batal Journal

            SqlCommand Cmd;

            //Get GLJournalHID
            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + RVNo + "' ";
            Cmd = new SqlCommand(Query, Conn);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
            Cmd = new SqlCommand(Query, Conn);
            int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

            if (CountData == 1)
            {
                //Delete Journal Detail
                Query = "UPDATE GLJournalH SET Status = 'Batal' WHERE GLJournalHID = '" + GLJournalHID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this, "Tidak dapat Closed karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Journal = true;
                return;
            }

            //End
        }        

        private void deleteInventTrans()
        {
            Query = "delete InventTrans where TransId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
        }

        private void updateDOIssuedLogTable()
        {
            DateTime DeliveryOrderDate = new DateTime(1753, 1, 1);
            string DOID = "";
            string CustID = "";
            int SeqNo = 0;
            string InventSiteID = "";
            decimal Qty = 0;
            decimal Ratio = 0;
            decimal price = 0;
            string SOID = "";
            DateTime SalesOrderDate = new DateTime(1753, 1, 1);
            string DOStats = "07";
            string DOStatsDesc = "Deleted";

            Query = "select a.DeliveryOrderDate, b.DeliveryOrderId, a.CustID, b.SeqNo, a.InventSiteID, b.Qty, b.ConvertionRatio, d.Price, b.SalesOrderId, c.OrderDate from DeliveryOrderH a left join DeliveryOrderD b on a.DeliveryOrderId = b.DeliveryOrderId left join SalesOrderH c on c.SalesOrderNo = b.SalesOrderId left join SalesOrderD d on b.SalesOrderId = d.SalesOrderNo and b.SalesOrderSeqNo = d.SeqNo where a.DeliveryOrderId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                DeliveryOrderDate = Convert.ToDateTime(Dr["DeliveryOrderDate"]);
                DOID = Dr["DeliveryOrderId"].ToString();
                CustID = Dr["CustID"].ToString();
                SeqNo = Convert.ToInt32(Dr["SeqNo"]);
                InventSiteID = Dr["InventSiteID"].ToString();
                Qty = Convert.ToDecimal(Dr["Qty"]);
                Ratio = Convert.ToDecimal(Dr["ConvertionRatio"]);
                price = Convert.ToDecimal(Dr["Price"]);
                SOID = Dr["SalesOrderId"].ToString();
                SalesOrderDate = Convert.ToDateTime(Dr["OrderDate"]);
            }
            Dr.Close();

            Query = "INSERT INTO [dbo].[DO_Issued_LogTable] ([DeliveryOrderDate],[DeliveryOrderId],[CustID],[SeqNo],[InventSiteID],[Qty_UoM],[Qty_Alt],[Amount],[SalesOrderId],[SalesOrderDate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES (@DeliveryOrderDate,@DeliveryOrderId,@CustID,@SeqNo,@InventSiteID,@Qty_UoM,@Qty_Alt,@Amount,@SalesOrderId,@SalesOrderDate,@LogStatusCode,@LogStatusDesc,@LogDescription,'" + ControlMgr.UserId + "',getdate())";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.AddWithValue("@DeliveryOrderDate", DeliveryOrderDate);
            Cmd.Parameters.AddWithValue("@DeliveryOrderId", DOID);
            Cmd.Parameters.AddWithValue("@CustID", CustID);
            Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            Cmd.Parameters.AddWithValue("@InventSiteID", InventSiteID);
            Cmd.Parameters.AddWithValue("@Qty_UoM", Qty);
            Cmd.Parameters.AddWithValue("@Qty_Alt", Qty * Ratio);
            Cmd.Parameters.AddWithValue("@Amount", price);
            Cmd.Parameters.AddWithValue("@SalesOrderId", SOID);
            Cmd.Parameters.AddWithValue("@SalesOrderDate", SalesOrderDate);
            Cmd.Parameters.AddWithValue("@LogStatusCode", DOStats);
            Cmd.Parameters.AddWithValue("@LogStatusDesc", DOStatsDesc);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + DOStats + ". " + DOStatsDesc);
            Cmd.ExecuteNonQuery();
        }

        private void updateInventLockTable()
        {
            Query = "select * from DeliveryOrderH a left join DeliveryOrderD b on a.DeliveryOrderId = b.DeliveryOrderId where a.DeliveryOrderId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                Query = "update InventLockTable set Lock_Qty = '0', Lock_Qty_Alt = '0' where RefTransId = '" + Dr["DeliveryOrderId"] + "' and RefTrans_SeqNo = '" + Dr["SeqNo"] + "' and RefTrans2Id = '" + Dr["SalesOrderId"] + "' and RefTrans2_SeqNo = '" + Dr["SalesOrderSeqNo"] + "' and SiteId = '" + Dr["InventSiteID"] + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }
        }

        private void updateInventOnHand()
        {

            decimal LockQty = 0;
            decimal Available_For_Sale_UoM = 0;
            decimal Available_For_Sale_Alt = 0;
            decimal Available_For_Sale_Amount = 0;
            decimal Available_For_Sale_Reserved_UoM = 0;
            decimal Available_For_Sale_Reserved_Alt = 0;
            decimal Available_For_Sale_Reserved_Amount = 0;
            decimal avgPrice = 0;
            Query = "select b.SalesOrderId, b.SalesOrderSeqNo, a.InventSiteID, b.FullItemID, b.Qty, b.ConvertionRatio from DeliveryOrderH a left join DeliveryOrderD b on a.DeliveryOrderId = b.DeliveryOrderId where a.DeliveryOrderId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                Query = "select Lock_Qty from InventLockTable where RefTransId = '" + Dr["SalesOrderId"] + "' and RefTrans_SeqNo = '" + Dr["SalesOrderSeqNo"] + "' and SiteId = '" + Dr["InventSiteID"] + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                LockQty = Convert.ToDecimal(Cmd.ExecuteScalar());

                Query = "select a.Available_For_Sale_UoM, a.Available_For_Sale_Alt, a.Available_For_Sale_Amount, a.Available_For_Sale_Reserved_UoM, a.Available_For_Sale_Reserved_Alt, a.Available_For_Sale_Reserved_Amount, b.UoM_AvgPrice from Invent_OnHand_Qty a left join InventTable b on a.FullItemId = b.FullItemID where a.FullItemId = '" + Dr["FullItemID"] + "' and a.InventSiteId = '" + Dr["InventSiteID"] + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                SqlDataReader Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    Available_For_Sale_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]);
                    Available_For_Sale_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Alt"]);
                    Available_For_Sale_Amount = Convert.ToDecimal(Dr2["Available_For_Sale_Amount"]);
                    Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_UoM"]);
                    Available_For_Sale_Reserved_Alt = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Alt"]);
                    Available_For_Sale_Reserved_Amount = Convert.ToDecimal(Dr2["Available_For_Sale_Reserved_Amount"]);
                    avgPrice = Convert.ToDecimal(Dr2["UoM_AvgPrice"]);
                }
                Dr2.Close();

                decimal Qty = Convert.ToDecimal(Dr["Qty"]);
                decimal Ratio = Convert.ToDecimal(Dr["ConvertionRatio"]);
                if (Qty > LockQty)
                {
                    Available_For_Sale_Reserved_UoM += LockQty;
                    Available_For_Sale_Reserved_Alt += (LockQty * Ratio);
                    Available_For_Sale_Reserved_Amount += (LockQty * avgPrice);

                    Available_For_Sale_UoM += Qty - LockQty;
                    Available_For_Sale_Alt += ((Qty - LockQty) * Ratio);
                    Available_For_Sale_Amount += ((Qty - LockQty) * avgPrice);
                }
                else
                {
                    Available_For_Sale_Reserved_UoM += Qty;
                    Available_For_Sale_Reserved_Alt += (Qty * Ratio);
                    Available_For_Sale_Reserved_Amount += (Qty * avgPrice);
                }

                Query = "update Invent_OnHand_Qty set Available_For_Sale_Reserved_UoM = '" + Available_For_Sale_Reserved_UoM + "', Available_For_Sale_Reserved_Alt = '" + Available_For_Sale_Reserved_Alt + "', Available_For_Sale_Reserved_Amount = '" + Available_For_Sale_Reserved_Amount + "', Available_For_Sale_UoM = '" + Available_For_Sale_UoM + "'";
                Query += ", Available_For_Sale_Alt = '" + Available_For_Sale_Alt + "', Available_For_Sale_Amount = '" + Available_For_Sale_Amount + "' where FullItemId = '" + Dr["FullItemId"] + "' and InventSiteId = '" + Dr["InventSiteID"] + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }
        }

        private void updateInventSalesQty()
        {
            decimal SO_Confirmed_Outstanding_UoM = 0;
            decimal SO_Confirmed_Outstanding_Alt = 0;
            decimal SO_Confirmed_Outstanding_Amount = 0;
            decimal DO_Issued_Outstanding_UoM = 0;
            decimal DO_Issued_Outstanding_Alt = 0;
            decimal DO_Issued_Outstanding_Amount = 0;
            decimal avgPrice = 0;
            Query = "select * from DeliveryOrderD where DeliveryOrderId = '" + dataGridView1.CurrentRow.Cells[PK].Value.ToString() + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                Query = "select a.SO_Confirmed_Outstanding_UoM, a.SO_Confirmed_Outstanding_Alt, a.SO_Confirmed_Outstanding_Amount, a.DO_Issued_Outstanding_UoM, a.DO_Issued_Outstanding_Alt, a.DO_Issued_Outstanding_Amount, b.UoM_AvgPrice from Invent_Sales_Qty a left join InventTable b on a.FullItemId = b.FullItemID where a.FullItemId = '" + Dr["FullItemID"] + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                SqlDataReader Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    SO_Confirmed_Outstanding_UoM = Convert.ToDecimal(Dr2["SO_Confirmed_Outstanding_UoM"]);
                    SO_Confirmed_Outstanding_Alt = Convert.ToDecimal(Dr2["SO_Confirmed_Outstanding_Alt"]);
                    SO_Confirmed_Outstanding_Amount = Convert.ToDecimal(Dr2["SO_Confirmed_Outstanding_Amount"]);
                    DO_Issued_Outstanding_UoM = Convert.ToDecimal(Dr2["DO_Issued_Outstanding_UoM"]);
                    DO_Issued_Outstanding_Alt = Convert.ToDecimal(Dr2["DO_Issued_Outstanding_Alt"]);
                    DO_Issued_Outstanding_Amount = Convert.ToDecimal(Dr2["DO_Issued_Outstanding_Amount"]);
                    avgPrice = Convert.ToDecimal(Dr2["UoM_AvgPrice"]);
                }
                Dr2.Close();

                Query = "select Price from SalesOrderD where SalesOrderNo = '" + Dr["SalesOrderId"] + "' and SeqNo = '" + Dr["SalesOrderSeqNo"] + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());

                decimal Qty = Convert.ToDecimal(Dr["Qty"]);
                decimal Ratio = Convert.ToDecimal(Dr["ConvertionRatio"]);
                SO_Confirmed_Outstanding_UoM += Qty;
                SO_Confirmed_Outstanding_Alt = SO_Confirmed_Outstanding_Alt + (Qty * Ratio);
                SO_Confirmed_Outstanding_Amount = SO_Confirmed_Outstanding_Amount + (Qty * price);

                DO_Issued_Outstanding_UoM -= Qty;
                DO_Issued_Outstanding_Alt -= (Qty * Ratio);
                DO_Issued_Outstanding_Amount -= (Qty * avgPrice);

                Query = "update Invent_Sales_Qty set SO_Confirmed_Outstanding_UoM = '" + SO_Confirmed_Outstanding_UoM + "', SO_Confirmed_Outstanding_Alt = '" + SO_Confirmed_Outstanding_Alt + "'";
                Query += ", SO_Confirmed_Outstanding_Amount = '" + SO_Confirmed_Outstanding_Amount + "', DO_Issued_Outstanding_UoM = '" + DO_Issued_Outstanding_UoM + "', DO_Issued_Outstanding_Alt = '" + DO_Issued_Outstanding_Alt + "'";
                Query += ", DO_Issued_Outstanding_Amount = '" + DO_Issued_Outstanding_Amount + "' where FullItemId = '" + Dr["FullItemID"] + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }
            Dr.Close();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            if (FormName == "DOInq")
            {
                TransStatus = "'07', '08'";
            }
            else if (FormName == "BBKInq")
            {
                TransStatus = "'03', '04'";
            }
            else if (FormName == "SAInq")
            {
                TransStatus = "'02', '04', '07', '10'";
            }
            else if (FormName == "InquiryNotaResize")
            {
                TransStatus = "1";
            }
            else if(FormName == "SOInq")
                TransStatus = "'02', '04', '07', '11'";
            else if (FormName == "RVInq")
            {
                TransStatus = "'02'";
            }
            else if (FormName == "FQA" || FormName == "FQA2" || FormName == "COA" || FormName == "MASTERCOA" || FormName == "JOURNAL")
            {
                TransStatus = "'Batal'";
            }
            else if (FormName == "GLJournal")
            {
                TransStatus = "'1'";
            }
            else if (FormName == "CreditLimitInq")
            {
                TransStatus = "'02','XX'";
            }
            btnMPrev_Click(sender, e);
            //btnOnProgress.Theme = MetroFramework.MetroThemeStyle.Light;
            //btnCompleted.Theme = MetroFramework.MetroThemeStyle.Dark;
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.Black;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
            //RefreshGrid2();
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            if (FormName == "DOInq")
            {
                TransStatus = "'01', '02', '03', '05', '06', '09'";
            }
            else if (FormName == "BBKInq")
            {
                TransStatus = "'01', '02', '05', '06'";
            }
            else if (FormName == "SAInq")
            {
                TransStatus = "'01', '03', '05', '06', '08', '09', '11', '12'";
            }
            else if (FormName == "InquiryNotaResize")
            {
                TransStatus = "0";
            }
            else if (FormName == "SOInq")
                TransStatus = "'01', '03', '05', '06', '08', '09', '10', '12'";
            else if (FormName == "RVInq")
            {
                TransStatus = "'01','03'";
            }
            else if (FormName == "FQA" || FormName == "FQA2" || FormName == "COA" || FormName == "MASTERCOA"  || FormName == "JOURNAL")
            {
                TransStatus = "'Gunakan'";
            }
            else if (FormName == "GLJournal")
            {
                TransStatus = "'0'";
            }
            else if (FormName == "CreditLimitInq")
            {
                TransStatus = "'01'";
            }
            btnMPrev_Click(sender, e);
            btnOnProgress.Theme = MetroFramework.MetroThemeStyle.Dark;
            btnCompleted.Theme = MetroFramework.MetroThemeStyle.Light;
            btnOnProgress.BackColor = Color.Black;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
            //RefreshGrid2();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(this, new EventArgs());
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if (FormName == "DOInq")
            {
                if (e.ColumnIndex != -1 && e.RowIndex != -1)
                {
                    string DOId = dataGridView1.Rows[e.RowIndex].Cells["DeliveryOrderId"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["DeliveryOrderId"].Value.ToString();

                    Cmd = new SqlCommand("SELECT [CustID] FROM [DeliveryOrderH] WHERE [DeliveryOrderId] = '" + DOId + "'", Conn);
                    string CustID = Cmd.ExecuteScalar().ToString();

                    if (e.RowIndex > -1 && dataGridView1.Columns[e.ColumnIndex].Name == "Preview")
                    {
                        GlobalPreview f = new GlobalPreview("Delivery Order", DOId);
                        f.Show();
                    }
                    else if (e.RowIndex > -1 && dataGridView1.Columns[e.ColumnIndex].Name == "Send Email")
                    {
                        GlobalSendEmail f = new GlobalSendEmail("Delivery Order", DOId, CustID);
                        f.Show();
                    }
                }
            }
            else if (FormName == "SAInq")
            {
                //cuman status preorder yang bisa send email

                if (e.ColumnIndex > -1 && e.RowIndex > -1)
                {
                    string SAId = dataGridView1.Rows[e.RowIndex].Cells["SalesAgreementNo"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["SalesAgreementNo"].Value.ToString();

                    Cmd = new SqlCommand("SELECT [CustID] FROM [SalesAgreementH] WHERE [SalesAgreementNo] = '" + SAId + "'", Conn);
                    string CustID = Cmd.ExecuteScalar().ToString();
                    
                    if (dataGridView1.Columns[e.ColumnIndex].Name == "Preview")
                    {
                        //GlobalPreview f = new GlobalPreview("Sales Agreement", SAId);
                        //f.Show();

                        ListMethod.restrictedPreviewEmail("Preview", "Sales Agreement", "SalesAgreementH", "SalesAgreementNo", SAId, CustID);

                    }
                    else if (dataGridView1.Columns[e.ColumnIndex].Name == "Send Email")
                    {
                        //GlobalSendEmail f = new GlobalSendEmail("Sales Agreement", SAId, CustID);
                        //f.Show();

                        ListMethod.restrictedPreviewEmail("Email", "Sales Agreement", "SalesAgreementH", "SalesAgreementNo", SAId, CustID);

                    }
                }
            }
            else if (FormName == "SOInq")
            {
                if (e.ColumnIndex > -1 && e.RowIndex > -1)
                {
                    string SOId = dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString();

                    Cmd = new SqlCommand("SELECT [CustID] FROM [SalesOrderH] WHERE [SalesOrderNo] = '" + SOId + "'", Conn);
                    string CustID = Cmd.ExecuteScalar().ToString();

                    if (dataGridView1.Columns[e.ColumnIndex].Name == "Preview")
                    {
                        //GlobalPreview f = new GlobalPreview("Sales Order", SOId);
                        //f.Show();

                        ListMethod.restrictedPreviewEmail("Preview", "Sales Order", "SalesOrderH", "SalesOrderNo", SOId, "");
                    }

                    else if (dataGridView1.Columns[e.ColumnIndex].Name == "Send Email")
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells["TransStatus"].Value.ToString() == "01")
                        {
                            try
                            {
                                //GlobalSendEmail f = new GlobalSendEmail("Sales Order", SOId, CustID);
                                //f.Show();

                                ListMethod.restrictedPreviewEmail("Email", "Sales Order", "SalesOrderH", "SalesOrderNo", SOId, CustID);

                            }
                            catch (Exception ex)
                            {
                                Trans.Rollback();
                                MetroFramework.MetroMessageBox.Show(this, "Error message: " + ex, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else if (dataGridView1.Rows[e.RowIndex].Cells["TransStatus"].Value.ToString() == "05")
                            MetroFramework.MetroMessageBox.Show(this, "Cannot send email more than once to " + dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString() + "\nPlease contact admin!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        else
                            MetroFramework.MetroMessageBox.Show(this, dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString() + " email not send!\nPlease check SO Status!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        RefreshGrid();
                    }
                }
            }
            else if (FormName == "BBKInq")
            {
                if (e.ColumnIndex > -1 && e.RowIndex > -1)
                {
                    string BBKid = dataGridView1.Rows[e.RowIndex].Cells["GoodsIssuedId"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["GoodsIssuedId"].Value.ToString();

                    Cmd = new SqlCommand("SELECT [AccountNum] FROM [GoodsIssuedH] WHERE [GoodsIssuedId] = '" + BBKid + "'", Conn);
                    string recipientId = Cmd.ExecuteScalar().ToString();

                    if (dataGridView1.Columns[e.ColumnIndex].Name == "Preview")
                    {
                        GlobalPreview f = new GlobalPreview("Goods Issued", BBKid);
                        f.SetMode("Surat Jalan"); //Inquiry buka surat jalan
                        f.Show();
                    }
                    else if (dataGridView1.Columns[e.ColumnIndex].Name == "Send Email")
                    {
                        GlobalSendEmail f = new GlobalSendEmail("Goods Issued", BBKid, recipientId);
                        f.Show();
                    }
                }
            }
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

        //public void RefreshGrid2()
        //{
        //    metroGrid1.DataSource = null;
        //    metroGrid1.Columns.Clear();
        //    metroGrid1.Rows.Clear();
        //    Conn = ConnectionString.GetConnection();
        //    if (FormName == "DOInq")
        //    {
        //        if (TransStatus == String.Empty)
        //        {
        //            TransStatus = "'01', '02', '05', '06', '09'";
        //            //PK = "DO No";
        //            //PK = "DeliveryOrderId"; //REMARKED BY: HC
        //        }
        //        where2 = " and a.DeliveryOrderStatus in (" + TransStatus + ") ";
        //    }
        //    else if (FormName == "BBKInq")
        //    {
        //        if (TransStatus == String.Empty)
        //        {
        //            TransStatus = "'01', '02', '05', '06'";
        //        }
        //        where2 = " and a.StatusCode in (" + TransStatus + ") ";
        //    }
        //    else if (FormName == "SAInq")
        //    {
        //        if (TransStatus == String.Empty)
        //        {
        //            TransStatus = "'01', '03', '05', '06', '08', '09', '11'";
        //        }
        //        where2 = " and a.TransStatus in (" + TransStatus + ") ";
        //    }
        //    else if (FormName == "InquiryNotaResize")
        //    {
        //        if (TransStatus == String.Empty)
        //        {
        //            TransStatus = "0";
        //        }
        //        where2 = " and Posted in (" + TransStatus + ") ";
        //    }

        //    Query = "select * from (select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) No, a.* from (select " + fieldName + " from " + fromTableQuery + " where " + Where + where2 + " ";

        //    if (FormName != "InquiryNotaResize")
        //    {
        //        if (crit == null)
        //            Query += ") a ";
        //        else if (crit.Equals("All"))
        //        {
        //            //Query += "where [" + PK + "] like '%" + txtSearch.Text + "%' ) a ";
        //            for (int i = 1; i < cmbCriteria.Items.Count; i++)
        //            {
        //                string criteria = "";
        //                string CriteriaQuery = "SELECT FieldName From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.Items[i] + "' ";
        //                using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
        //                {
        //                    criteria = Cmd3.ExecuteScalar().ToString();
        //                }
        //                if (i == 1)
        //                    Query += "and ( ";
        //                if (i > 1 && i != 1)
        //                    Query += " or ";
        //                Query += " " + criteria + " like '%" + txtSearch.Text + "%' ";
        //            }
        //            Query += ") ) a ";
        //        }
        //        else
        //        {
        //            Query += "and [" + crit + "] like '%" + txtSearch.Text + "%' ) a ";
        //        }
        //    }
        //    else if (FormName == "InquiryNotaResize")
        //    {
        //        if (crit == null)
        //            Query += ") a ";
        //        else if (crit.Equals("All"))
        //        {
        //            for (int i = 1; i < cmbCriteria.Items.Count; i++)
        //            {
        //                string criteria = "";
        //                string CriteriaQuery = "SELECT FieldName From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.Items[i] + "' ";
        //                using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
        //                {
        //                    criteria = Cmd3.ExecuteScalar().ToString();
        //                }
        //                if (i == 1)
        //                    Query += "and ( ";
        //                if (i > 1 && i != 1)
        //                    Query += " or ";
        //                Query += " " + criteria + " like '%" + txtSearch.Text + "%' ";
        //            }
        //            Query += ") ) a ";
        //        }
        //        else if (crit.Contains("Date"))
        //        {
        //            Query += "and [" + crit + "] BETWEEN '" + dtFrom.Value.ToString() + "' AND '" + dtTo.Value.ToString() + "' ) a ";
        //        }
        //        else if (crit.Equals("SiteID"))
        //        {
        //            Query = "select * from (select ROW_NUMBER() OVER (order by CASE WHEN CreatedDate >= UpdatedDate THEN  CreatedDate ELSE  UpdatedDate END DESC) No, a.* from (select " + fieldName + " from " + fromTableQuery + " ";
        //            Query += " LEFT JOIN [dbo].[InventSite] b ON a.SiteID = b.[InventSiteID] ";
        //            Query += "where " + Where + where2 + " ";
        //            Query += "AND b.InventSiteName LIKE '%" + txtSearch.Text + "%' ";
        //            Query += " ) a ";
        //        }
        //        else
        //        {
        //            Query += "and [" + crit + "] like '%" + txtSearch.Text + "%' ) a ";
        //        }
        //    }

        //    Query += ") a ";
        //    Query += "Where a.No Between " + Limit1 + " and " + Limit2 + " ;";

        //    Da = new SqlDataAdapter(Query, Conn);
        //    Dt = new DataTable();
        //    Da.Fill(Dt);

        //    //STEVEN EDIT START
        //    DataGridViewButtonColumn buttonpreview = new DataGridViewButtonColumn();
        //    buttonpreview.Name = "Preview";
        //    buttonpreview.HeaderText = "Preview";
        //    buttonpreview.Text = "Preview";
        //    buttonpreview.UseColumnTextForButtonValue = true;

        //    DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
        //    buttonSend.Name = "Send Email";
        //    buttonSend.HeaderText = "Send Email";
        //    buttonSend.Text = "Send Email";
        //    buttonSend.UseColumnTextForButtonValue = true;
        //    //STEVEN EDIT END

        //    //DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
        //    //buttonSend.Name = "Send Email";
        //    //buttonSend.HeaderText = "Send Email";
        //    //buttonSend.Text = "Send Email";
        //    //buttonSend.UseColumnTextForButtonValue = true;

        //    metroGrid1.AutoGenerateColumns = true;
        //    metroGrid1.DataSource = Dt;

        //    for (int i = 0; i < metroGrid1.RowCount; i++)
        //    {
        //        if (Convert.ToDateTime(metroGrid1.Rows[i].Cells["UpdatedDate"].Value) == new DateTime(1753, 1, 1))
        //            metroGrid1.Rows[i].Cells["UpdatedDate"].Value = (object)DBNull.Value;
        //    }

        //    if (FormName != "InquiryNotaResize")
        //    {
        //        for (int i = 0; i < metroGrid1.ColumnCount; i++)
        //        {
        //            metroGrid1.Columns[i].HeaderText = headerText[i];
        //        }

        //        //STEVEN EDIT START
        //        if (!metroGrid1.Columns.Contains("Preview"))
        //            metroGrid1.Columns.Add(buttonpreview);
        //        if (!metroGrid1.Columns.Contains("Send Email"))
        //            metroGrid1.Columns.Add(buttonSend);
        //        //STEVEN EDIT END
        //    }

        //    metroGrid1.Refresh();
        //    metroGrid1.AutoResizeColumns();

        //    //REMARKED BY: HC (S)
        //    //if (FormName == "DOInq")
        //    //{
        //    //    PK = "DeliveryOrderId";
        //    //}
        //    //REMARKED BY: HC (E)
        //    Query = "Select Count([" + PK + "]) From ( Select [" + PK + "] From " + fromTableQuery + " where " + Where + where2;
        //    if (FormName == "DOInq")
        //    {
        //        if (TransStatus == String.Empty)
        //            TransStatus = "'01', '02', '05', '06', '09'";
        //        Query += "and a.DeliveryOrderStatus in (" + TransStatus + ") ";

        //    }
        //    else if (FormName == "BBKInq")
        //    {
        //        if (TransStatus == String.Empty)
        //            TransStatus = "'01', '02', '05', '06'";
        //        Query += "and a.StatusCode in (" + TransStatus + ") ";
        //    }
        //    else if (FormName == "SAInq")
        //    {
        //        if (TransStatus == String.Empty)
        //            TransStatus = "'01', '03', '05', '06', '08', '09', '11'";
        //        Query += "and a.TransStatus in (" + TransStatus + ") ";
        //    }
        //    else if (FormName == "InquiryNotaResize")
        //    {
        //        if (TransStatus == String.Empty)
        //            TransStatus = "0";
        //        Query += "and Posted in (" + TransStatus + ") ";
        //    }
        //    if (crit == null)
        //        Query += ") a;";
        //    else if (crit.Equals("All"))
        //    {
        //        //Query += "and [" + PK + "] like '%" + txtSearch.Text + "%' ) a ";
        //        for (int i = 1; i < cmbCriteria.Items.Count; i++)
        //        {
        //            string criteria = "";
        //            string CriteriaQuery = "SELECT FieldName From [ISBS-NEW4].[User].[Table] WHERE TableName = '" + TableName + "' AND DisplayName = '" + cmbCriteria.Items[i] + "' ";
        //            using (SqlCommand Cmd3 = new SqlCommand(CriteriaQuery, Conn))
        //            {
        //                criteria = Cmd3.ExecuteScalar().ToString();
        //            }

        //            if (i == 1)
        //                Query += "and ( ";
        //            if (i > 1 && i != 1)
        //                Query += " or ";
        //            Query += " " + criteria + " like '%" + txtSearch.Text + "%' ";
        //        }
        //        Query += ") ) a ";
        //    }
        //    else if (crit.Equals("SiteID"))
        //    {
        //        Query = "Select Count([" + PK + "]) From ( Select [" + PK + "], SiteID From " + fromTableQuery + " where " + Where + where2;
        //        Query += "and Posted in (" + TransStatus + ") ";
        //        Query += ") a ";
        //        Query += " LEFT JOIN [dbo].[InventSite] b ON a.[SiteID] = b.[InventSiteID] ";
        //        Query += "WHERE b.InventSiteName LIKE '%" + txtSearch.Text + "%' ;";
        //    }
        //    else
        //        Query += "and " + crit + " like '%" + txtSearch.Text + "%' ) a ";

        //    Cmd = new SqlCommand(Query, Conn);
        //    Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
        //    Conn.Close();

        //    lblTotal.Text = "Total Rows : " + Total.ToString();
        //    if (dataShow != 0)
        //        Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
        //    else
        //        Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
        //    lblPage.Text = "/ " + Page2;
        //    Conn.Close();

        //    if (FormName == "InquiryNotaResize")
        //    {
        //        for (int i = 0; i < metroGrid1.Rows.Count; i++)
        //        {
        //            string Warehouse = "";
        //            string QueryWarehouse = "SELECT [InventSiteName] FROM [dbo].[InventSite] WHERE [InventSiteID] = '" + metroGrid1.Rows[i].Cells["Warehouse"].Value.ToString() + "'";
        //            using (Conn = ConnectionString.GetConnection())
        //            using (SqlCommand cmd4 = new SqlCommand(QueryWarehouse, Conn))
        //            {
        //                Warehouse = cmd4.ExecuteScalar().ToString();
        //            }
        //            metroGrid1.Rows[i].Cells["Warehouse"].Value = Warehouse;
        //        }
        //    }
        //    metroGrid1.AutoResizeColumns();
        //    if (FormName == "InquiryNotaResize" && cmbCriteria.Text == "Warehouse" && metroGrid1.Columns.Count > 10 && metroGrid1.Columns["InventSiteName"] != null)
        //    {
        //        metroGrid1.Columns["InventSiteName"].Visible = false;
        //    }
        //}

        //private void CallForm2(string tmpMode)
        //{
        //    //begin
        //    //updated by : joshua
        //    //updated date : 23 feb 2018
        //    //description : check permission access

        //    string temp = "";

        //    //REMARKED BY: HC (S) 02.05.18
        //    //Digunakan saat HeaderGridView Name tidak sesuai dengan nama field table
        //    //if (FormName == "DOInq")
        //    //{
        //    //    PK = "DO No";
        //    //}
        //    //Digunakan saat HeaderGridView Name tidak sesuai dengan nama field table
        //    //REMARKED BY: HC (E)

        //    if (metroGrid1.RowCount != 0 && FormName != "InquiryNotaResize")
        //        temp = metroGrid1.CurrentRow.Cells[PK].Value.ToString();
        //    else if (FormName == "InquiryNotaResize" && metroGrid1.RowCount != 0)
        //        temp = metroGrid1.CurrentRow.Cells["NRZ No"].Value.ToString();

        //    if (tmpMode == "New")
        //        temp = "";
        //    if (FormName == "DOInq")
        //    {
        //        Sales.DeliveryOrder.DOHeader F = new Sales.DeliveryOrder.DOHeader();
        //        if (F.PermissionAccess(ControlMgr.View) > 0)
        //        {
        //            F.SetParent(this);
        //            F.SetMode(tmpMode, temp);
        //            F.Show();
        //        }
        //        else
        //        {
        //            MessageBox.Show(ControlMgr.PermissionDenied);
        //        }
        //    }
        //    else if (FormName == "BBKInq")
        //    {
        //        Sales.BBK.BBKHeader F = new Sales.BBK.BBKHeader();
        //        if (F.PermissionAccess(ControlMgr.View) > 0)
        //        {
        //            F.SetParent(this);
        //            F.SetMode(tmpMode, temp);
        //            F.Show();
        //        }
        //        else
        //        {
        //            MessageBox.Show(ControlMgr.PermissionDenied);
        //        }
        //    }
        //    else if (FormName == "SAInq")
        //    {
        //        Sales.SalesAgreement.SAHeader F = new Sales.SalesAgreement.SAHeader();
        //        if (F.PermissionAccess(ControlMgr.View) > 0)
        //        {
        //            F.SetParent(this);
        //            F.SetMode(tmpMode, temp);
        //            F.Show();
        //        }
        //        else
        //        {
        //            MessageBox.Show(ControlMgr.PermissionDenied);
        //        }
        //    }
        //    else if (FormName == "InquiryNotaResize")
        //    {
        //        ISBS_New.Purchase.GoodsReceipt.Resize.FormResizeGR F = new ISBS_New.Purchase.GoodsReceipt.Resize.FormResizeGR();
        //        if (F.PermissionAccess(ControlMgr.View) > 0)
        //        {
        //            string refnumber = metroGrid1.CurrentRow.Cells["GR No"].Value.ToString();
        //            DateTime date = new DateTime();
        //            date = Convert.ToDateTime(metroGrid1.CurrentRow.Cells["NRZ Date"].Value.ToString());
        //            F.SetParent(this);
        //            F.SetMode(tmpMode, temp, refnumber, date);
        //            F.dtTransDate.Enabled = false;
        //            F.Show();
        //        }
        //        else
        //        {
        //            MessageBox.Show(ControlMgr.PermissionDenied);
        //        }
        //    }
        //}

        //private void SelectPR2()
        //{
        //    if (metroGrid1.RowCount > 0)
        //    {
        //        CallForm("BeforeEdit");
        //    }
        //}

        private void metroGrid1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (metroGrid1.Columns[e.ColumnIndex].Name.Contains("Status"))
                metroGrid1.Columns[e.ColumnIndex].Visible = false;

            if (metroGrid1.Columns[e.ColumnIndex].Name.Contains("PPH") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("PPN") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("DPPercent"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            if (metroGrid1.Columns[e.ColumnIndex].Name.Contains("Total") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("ExchRate") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("DPAmount"))
            {
                if (e.Value == "" || e.Value == null)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }

            //ALLIGNMENT
            if (metroGrid1.Columns[e.ColumnIndex].Name.Contains("PPH") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("PPN") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("DPPercent") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("Total") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("ExchRate") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("DPAmount") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("Timbang1Weight") || metroGrid1.Columns[e.ColumnIndex].Name.Contains("Timbang2Weight"))
            {
                metroGrid1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            //DATE FORMAT
            if (metroGrid1.Columns[e.ColumnIndex].Name == "CreatedDate" || metroGrid1.Columns[e.ColumnIndex].Name == "UpdatedDate" || metroGrid1.Columns[e.ColumnIndex].Name.Contains("ValidTo"))
                metroGrid1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            else if (metroGrid1.Columns[e.ColumnIndex].Name.Contains("Date"))
                metroGrid1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnUbahReceiptDate_Click(object sender, EventArgs e)
        {
            Sales.BBK.UbahReceiptDate F = new Sales.BBK.UbahReceiptDate();

            string ginumber = dataGridView1.CurrentRow.Cells["GoodsIssuedId"].Value.ToString();
            DateTime date = new DateTime();
            date = DateTime.ParseExact(dataGridView1.CurrentRow.Cells["GoodsIssuedDate"].Value.ToString(), "dd/MM/yyyy H:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            //F.SetParent(this);
            F.SetMode(ginumber, date);
            F.Show();

        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            } 
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
        }

        private void txtPage_Leave(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > Page2)
            {
                txtPage.Text = Page2.ToString();
            }
        }
    }
}
