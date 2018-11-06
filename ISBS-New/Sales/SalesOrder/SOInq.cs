using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

//BY: HC
namespace ISBS_New.Sales.SalesOrder
{
    public partial class SOInq : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;


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

        public SOInq()
        {
            InitializeComponent();
        }

        private void SOInq_Load(object sender, EventArgs e)
        {
            TransStatus = "'01', '03', '05', '06', '08', '09', '10', '12'";
            addCmbCrit();
            cmbShowLoad();
            ModeLoad();
            checkSOExp();
            RefreshGrid();

            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void checkSOExp()
        {
            try
            {
                SqlDataReader Dr2;
                SqlDataReader Dr3;
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                Query = "select * from ISBSN.[dbo].[SalesOrderH] where ValidTo < DATEADD(day,-1,GETDATE()) and TransStatus = '01'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    //update for sale qty in Invent_OnHand_Qty when expired
                    Query = "select * from ISBSN.[dbo].[SalesOrderD] where SalesOrderNo = '" + Dr["SalesOrderNo"] + "' and LockID is not null";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr2 = Cmd.ExecuteReader();
                    while (Dr2.Read())
                    {
                        //string FullItemID = Dr2["FullItemID"].ToString();
                        //string LockID = Dr2["LockID"].ToString();
                        decimal LockQty = 0;
                        if (Dr2["LockQty"].ToString() != String.Empty)
                            LockQty = Convert.ToDecimal(Dr2["LockQty"]);
                        decimal AvailableforSale_Qty_UoM = 0;
                        decimal AvailableforSale_Qty_Alt;
                        decimal AvailableforSaleReserved_Qty_UoM = 0;
                        decimal AvailableforSaleReserved_Qty_Alt;
                        decimal Ratio = 0;
                        Query = "select b.Ratio, a.Available_For_Sale_UoM, a.Available_For_Sale_Reserved_UoM from ISBSN.[dbo].[Invent_OnHand_Qty] a left join InventConversion b on a.FullItemId = b.FullItemID where a.FullItemId = '" + Dr2["FullItemID"] + "' and a.InventSiteId = '" + Dr2["LockID"] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr3 = Cmd.ExecuteReader();
                        while (Dr3.Read())
                        {
                            Ratio = Convert.ToDecimal(Dr3["Ratio"]);
                            AvailableforSale_Qty_UoM = Convert.ToDecimal(Dr3["Available_For_Sale_UoM"]);
                            AvailableforSaleReserved_Qty_UoM = Convert.ToDecimal(Dr3["Available_For_Sale_Reserved_UoM"]);
                        }
                        Dr3.Close();
                        AvailableforSale_Qty_UoM = AvailableforSale_Qty_UoM + LockQty;
                        AvailableforSaleReserved_Qty_UoM = AvailableforSaleReserved_Qty_UoM - LockQty;
                        AvailableforSale_Qty_Alt = Ratio * AvailableforSale_Qty_UoM;
                        AvailableforSaleReserved_Qty_Alt = Ratio * AvailableforSaleReserved_Qty_UoM;
                        Query = "update Invent_OnHand_Qty set Available_For_Sale_UoM = '" + AvailableforSale_Qty_UoM + "', Available_For_Sale_Reserved_UoM = '" + AvailableforSaleReserved_Qty_UoM + "', Available_For_Sale_Alt = '" + AvailableforSale_Qty_Alt + "', Available_For_Sale_Reserved_Alt = '" + AvailableforSaleReserved_Qty_Alt + "' where FullItemId = '" + Dr2["FullItemID"] + "' and InventSiteId = '" + Dr2["LockID"] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        //update so pre order qty in Invent_Sales_Qty
                        Query = "SELECT SO_Preordered_UoM from Invent_Sales_Qty where FullItemId = '" + Dr2["FullItemID"] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        decimal tempSOPreorderQty = Convert.ToDecimal(Cmd.ExecuteScalar());
                        tempSOPreorderQty -= LockQty;
                        Query = "update Invent_Sales_Qty set SO_Preordered_UoM = '" + tempSOPreorderQty + "', SO_Preordered_Alt = '" + tempSOPreorderQty * Ratio + "' where FullItemId = '" + Dr2["FullItemID"] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                    Dr2.Close();
                }
                Dr.Close();

                Query = "update SalesOrderH set TransStatus = '02', UpdatedDate = getdate(), UpdatedBy = 'SYSTEM' where ValidTo < DATEADD(day,-1,GETDATE()) and TransStatus != '03' and TransStatus != '04'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                Trans.Commit();
                Conn.Close();
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                MetroFramework.MetroMessageBox.Show(this, "Error to update SO expiry date!\r\n Error message: " + ex, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'SalesOrderH'";

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

            //begin
            //updated by : joshua 
            //updated date : 12 feb 2018 
            //description : add condition trans status on progress or completed
            if (TransStatus == String.Empty)
            {
                TransStatus = "'01', '03', '05', '06', '08', '09', '10', '12'"; Limit1 = 1; Limit2 = dataShow;
            }
            Query = "select * from (select ROW_NUMBER() OVER (order by a.[SalesOrderNo] desc) No, * from (select a.SalesOrderNo, a.OrderDate, a.ValidTo, a.RefTransId, a.Referensi, a.SalesMouNo, a.TransStatus, b.Deskripsi 'TransStatus Deskripsi', a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate from ISBSN.[dbo].[SalesOrderH] a left join TransStatusTable b on a.TransStatus = b.StatusCode where b.TransCode = 'SalesOrder' AND TransStatus IN (" + TransStatus + ") ) a ";
            //end

            if (crit == null)
                Query += ") a ";
            else if (crit.Equals("All"))
            {
                Query += "where [SalesOrderNo] like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' ) a ";
            }
            else if (crit.Equals("SalesOrderNo"))
            {
                Query += "where [SalesOrderNo] like '%" + txtSearch.Text + "%' ) a ";
            }
            else if (crit.Equals("TransStatus"))
            {
                Query += "where [TransStatus] like '%" + txtSearch.Text + "%' ) a ";
            }

            Query += "Where a.No Between " + Limit1 + " and " + Limit2 + " ;";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            buttonSend.Name = "Send Email";
            buttonSend.HeaderText = "Send Email";
            buttonSend.Text = "Send Email";
            buttonSend.UseColumnTextForButtonValue = true;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = Dt;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToDateTime(dataGridView1.Rows[i].Cells["UpdatedDate"].Value) == new DateTime(1753, 1, 1))
                    dataGridView1.Rows[i].Cells["UpdatedDate"].Value = (object)DBNull.Value;
            }

            if (!dataGridView1.Columns.Contains("Send Email"))
                dataGridView1.Columns.Add(buttonSend);
            dataGridView1.Refresh();
            dataGridView1.AutoResizeColumns();

            //begin
            //updated by : joshua 
            //updated date : 12 feb 2018 
            //description : add condition trans status on progress or completed
            Query = "Select Count([SalesOrderNo]) From ( Select [SalesOrderNo] From [dbo].[SalesOrderH] ";

            if (crit == null)
                Query += "WHERE TransStatus IN (" + TransStatus + ")) a;";
            else if (crit.Equals("All"))
            {
                Query += "where TransStatus IN (" + TransStatus + ") AND ([SalesOrderNo] like '%" + txtSearch.Text + "%' or TransStatus like '%" + txtSearch.Text + "%' )) a ";
            }
            else if (crit.Equals("SalesOrderNo"))
            {
                Query += "where TransStatus IN (" + TransStatus + ") AND [SalesOrderNo] like '%" + txtSearch.Text + "%' ) a ";
            }
            else if (crit.Equals("TransStatus"))
            {
                Query += "where TransStatus IN (" + TransStatus + ") AND [TransStatus] like '%" + txtSearch.Text + "%' ) a ";
            }
            //end

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
            if (this.PermissionAccess(Login.New) > 0)
            {
                //SOHeader F = new SOHeader();
                //F.SetParent(this);
                //F.SetMode("New", "");
                //F.Show();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void SelectSO()
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            SOHeader f = new SOHeader();
            if (f.PermissionAccess(Login.View) > 0)
            {
                if (dataGridView1.RowCount > 0)
                {
                    //f.SetMode("BeforeEdit", dataGridView1.CurrentRow.Cells["SalesOrderNo"].Value.ToString());
                    ////STEVEN EDIT S
                    //f.GetDgvAttachmentData();
                    ////STEVEN EDIT E
                    //f.SetParent(this);
                    //f.Show();
                    //RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectSO();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectSO();
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
        private void insertStatusLogDeleteSO(DataGridViewRow a)
        {
            DataGridViewRow r = a;
            Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
            Query += " VALUES ('SO Inq','" + dataGridView1.Rows[r.Index].Cells["SalesOrderNo"].Value.ToString() + "' ,' PK2Test', 'PK3Test', 'PK4Test', '04'";
            Query += ",'Deleted/Closed','" + Login.Username + "' , GetDate())";
            SqlCommand Cmd2 = new SqlCommand(Query, Conn, Trans);
            Cmd2.ExecuteNonQuery();
        }
        //End Steven Edit
        //Begin Steven Edit
        private void insertStatusLogDelete(DataGridViewRow a)
        {
            DataGridViewRow r = a;
            Query = "INSERT INTO [dbo].[StatusLog_Customer] ([StatusLog_FormName],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date])";
            Query += " VALUES ('SO Inq','" + dataGridView1.Rows[r.Index].Cells["SalesOrderNo"].Value.ToString() + "' ,' PK2Test', 'PK3Test', 'PK4Test', '04'";
            Query += ",'Deleted/Closed','" + Login.Username + "' , GetDate())";
            SqlCommand Cmd2 = new SqlCommand(Query, Conn, Trans);
            Cmd2.ExecuteNonQuery();
        }
        //End Steven Edit

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission acce ss
            if (this.PermissionAccess(Login.Delete) > 0)
            {
                char flag = '\0';
                string msg = "";
                int count = 0;
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                {
                    if (count >= 1)
                        msg += ", ";
                    msg += dataGridView1.Rows[r.Index].Cells["SalesOrderNo"].Value.ToString();
                    count++;

                    if (dataGridView1.Rows[r.Index].Cells["TransStatus"].Value.ToString() == "02" || dataGridView1.Rows[r.Index].Cells["TransStatus"].Value.ToString() == "06" || dataGridView1.Rows[r.Index].Cells["TransStatus"].Value.ToString() == "07")
                    {
                        flag = 'X';
                        MetroFramework.MetroMessageBox.Show(this, "Cannot select SO with status " + dataGridView1.Rows[r.Index].Cells["TransStatus Deskripsi"].Value.ToString() + "!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                }
                if (flag != 'X')
                {
                    if (msg == String.Empty)
                    {
                        MetroFramework.MetroMessageBox.Show(this, "Select Row(s)!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        DialogResult result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to delete " + msg + "?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                Conn = ConnectionString.GetConnection();
                                Trans = Conn.BeginTransaction();

                                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                                {
                                    //GET SO LOCK QTY
                                    decimal LockQty = 0;
                                    string LockID = String.Empty;
                                    string FullItemID = String.Empty;
                                    decimal Price = 0;
                                    Query = "select FullItemID, LockID, LockQty, Price from SalesOrderD where SalesOrderNo =  '" + dataGridView1.Rows[r.Index].Cells["SalesOrderNo"].Value.ToString() + "' and LockID is not null";
                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    Dr = Cmd.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        FullItemID = Dr["FullItemID"].ToString();
                                        LockID = Dr["LockID"].ToString();
                                        LockQty = Convert.ToDecimal(Dr["LockQty"]);
                                        Price = Convert.ToDecimal(Dr["Price"]);
                                    }
                                    Dr.Close();

                                    //GET FOR SALE QTY
                                    decimal Ratio = 0;
                                    decimal AvailableforSale_Qty_UoM = 0;
                                    decimal AvailableforSale_Qty_Alt;
                                    decimal AvailableforSaleReserved_Qty_UoM = 0;
                                    decimal AvailableforSaleReserved_Qty_Alt;

                                    Query = "select a.Available_For_Sale_UoM, a.Available_For_Sale_Reserved_UoM, b.Ratio from Invent_OnHand_Qty as a left join InventConversion as b on a.FullItemId = b.FullItemID where a.FullItemId = '" + FullItemID + "' and a.InventSiteId = '" + LockID + "'";
                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    Dr = Cmd.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        Ratio = Convert.ToDecimal(Dr["Ratio"]);
                                        AvailableforSale_Qty_UoM = Convert.ToDecimal(Dr["Available_For_Sale_UoM"]);
                                        AvailableforSaleReserved_Qty_UoM = Convert.ToDecimal(Dr["Available_For_Sale_Reserved_UoM"]);
                                    }
                                    Dr.Close();

                                    //ADD DELETED SO QTY FROM INVENT RESERVED TO INVENT FOR SALE
                                    AvailableforSale_Qty_UoM = AvailableforSale_Qty_UoM + LockQty;
                                    AvailableforSaleReserved_Qty_UoM = AvailableforSaleReserved_Qty_UoM - LockQty;
                                    AvailableforSale_Qty_Alt = AvailableforSale_Qty_UoM * Ratio;
                                    AvailableforSaleReserved_Qty_Alt = AvailableforSaleReserved_Qty_UoM * Ratio;

                                    Query = "update Invent_OnHand_Qty set Available_For_Sale_UoM = '" + AvailableforSale_Qty_UoM + "', Available_For_Sale_Reserved_UoM = '" + AvailableforSaleReserved_Qty_UoM + "', Available_For_Sale_Alt = '" + AvailableforSale_Qty_Alt + "', Available_For_Sale_Reserved_Alt = '" + AvailableforSaleReserved_Qty_Alt + "' where FullItemId = '" + FullItemID + "' and InventSiteId = '" + LockID + "'";
                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    Cmd.ExecuteNonQuery();

                                    //BEGIN STEVEN EDIT
                                    insertStatusLogDelete(r);
                                    //END STEVEN EDIT


                                    //MANAGE DELETED SO PRE ORDER QTY IN Invent_Sales_Qty
                                    if (dataGridView1.Rows[r.Index].Cells["TransStatus"].Value.ToString() == "01")
                                    {
                                        //string FullItemID = String.Empty;
                                        //decimal LockQty = 0;
                                        //Query = "select * from SalesOrderD where SalesOrderNo =  '" + dataGridView1.Rows[r.Index].Cells["SalesOrderNo"].Value.ToString() + "' and LockID is not null";
                                        //Cmd = new SqlCommand(Query, Conn, Trans);
                                        //Dr = Cmd.ExecuteReader();
                                        //while (Dr.Read())
                                        //{
                                        //    FullItemID = Dr["FullItemID"].ToString();
                                        //    LockQty = Convert.ToDecimal(Dr["LockQty"]);
                                        //}
                                        //Dr.Close();

                                        Query = "select a.SO_Preordered_UoM, b.Ratio from Invent_Sales_Qty a left join InventConversion b on a.FullItemId=b.FullItemID where a.FullItemId = '" + FullItemID + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
                                        Dr = Cmd.ExecuteReader();
                                        decimal SO_Preordered_UoM = 0;
                                        decimal SO_Preordered_Alt = 0;
                                        Ratio = 0;
                                        while (Dr.Read())
                                        {
                                            SO_Preordered_UoM = Convert.ToDecimal(Dr["SO_Preordered_UoM"]);
                                            Ratio = Convert.ToDecimal(Dr["Ratio"]);
                                        }
                                        Dr.Close();

                                        SO_Preordered_UoM -= LockQty;
                                        SO_Preordered_Alt = SO_Preordered_UoM * Ratio;

                                        Query = "update Invent_Sales_Qty set SO_Preordered_UoM = '" + SO_Preordered_UoM + "', SO_Preordered_Alt = '" + SO_Preordered_Alt + "' where FullItemId = '" + FullItemID + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
                                        Cmd.ExecuteNonQuery();
                                    }
                                    else if (dataGridView1.Rows[r.Index].Cells["TransStatus"].Value.ToString() == "03")
                                    {
                                        Query = "select a.SO_Confirmed_Outstanding_UoM, b.Ratio, a.SO_Confirmed_Outstanding_Amount from Invent_Sales_Qty a left join InventConversion b on a.FullItemId=b.FullItemID where a.FullItemId = '" + FullItemID + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
                                        Dr = Cmd.ExecuteReader();
                                        decimal SO_Confirmed_Outstanding_UoM = 0;
                                        decimal SO_Confirmed_Outstanding_Alt = 0;
                                        decimal SO_Confirmed_Outstanding_Amount = 0;
                                        Ratio = 0;
                                        while (Dr.Read())
                                        {
                                            SO_Confirmed_Outstanding_UoM = Convert.ToDecimal(Dr["SO_Confirmed_Outstanding_UoM"]);
                                            Ratio = Convert.ToDecimal(Dr["Ratio"]);
                                            SO_Confirmed_Outstanding_Amount = Convert.ToDecimal(Dr["SO_Confirmed_Outstanding_Amount"]);
                                        }
                                        Dr.Close();

                                        SO_Confirmed_Outstanding_UoM -= LockQty;
                                        SO_Confirmed_Outstanding_Alt = SO_Confirmed_Outstanding_UoM * Ratio;
                                        SO_Confirmed_Outstanding_Amount = SO_Confirmed_Outstanding_Amount - (LockQty * Price);

                                        Query = "update Invent_Sales_Qty set SO_Confirmed_Outstanding_UoM = '" + SO_Confirmed_Outstanding_UoM + "', SO_Confirmed_Outstanding_Alt = '" + SO_Confirmed_Outstanding_Alt + "', SO_Confirmed_Outstanding_Amount = '" + SO_Confirmed_Outstanding_Amount + "' where FullItemId = '" + FullItemID + "'";
                                        Cmd = new SqlCommand(Query, Conn, Trans);
                                        Cmd.ExecuteNonQuery();
                                    }

                                    //UPDATE SALES ORDER STATUS
                                    Query = "update [dbo].[SalesOrderH] set [TransStatus] = '04', [UpdatedDate] = getdate(),[UpdatedBy] = '" + Login.Username + "' where [SalesOrderNo] = '" + dataGridView1.Rows[r.Index].Cells["SalesOrderNo"].Value.ToString() + "'";
                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    Cmd.ExecuteNonQuery();
                                    //UPDATE SQ STATUS
                                    if(dataGridView1.Rows[r.Index].Cells["SA_SQ_Id"].Value.ToString().Split('/')[0] == "SO")
                                    Query = "update SalesQuotationH set TransStatus = '03', UpdatedDate = getdate(), UpdatedBy = '" + Login.Username + "' where SalesOrderNo = '" + dataGridView1.Rows[r.Index].Cells["SA_SQ_Id"].Value.ToString() + "'";
                                    else
                                        Query = "update SalesAgreementH set TransStatus = '11', UpdatedDate = getdate(), UpdatedBy = '" + Login.Username + "' where SalesOrderNo = '" + dataGridView1.Rows[r.Index].Cells["SA_SQ_Id"].Value.ToString() + "'";
                                    Cmd = new SqlCommand(Query, Conn, Trans);
                                    Cmd.ExecuteNonQuery();
                                    //STEVEN EDIT BEGIN--------------
                                    insertStatusLogDelete(r);
                                    //STEVEN EDIT END--------------
                                }
                                Trans.Commit();
        
                                Conn.Close();
                            }
                            catch (Exception ex)
                            {
                                Trans.Rollback();
                                MetroFramework.MetroMessageBox.Show(this, "Error: " + ex, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        RefreshGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[dataGridView1.CurrentRow.Index].Selected = true;
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells["TransStatus"].Value.ToString() == "03")
                    {
                        try
                        {
                            //Conn = ConnectionString.GetConnection();
                            //Trans = Conn.BeginTransaction();
                            //Query = "update [dbo].[SalesOrderH] set [TransStatus] = '05',[UpdatedDate] = getdate(),[UpdatedBy] = '" + Login.UserGroup + "' where [SalesOrderNo] = '" + dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString() + "'";
                            //Cmd = new SqlCommand(Query, Conn);
                            //Cmd.ExecuteNonQuery();
                            //Trans.Commit();
                            //Conn.Close();
                            //MetroFramework.MetroMessageBox.Show(this, dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString() + " email send!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            string SOId = dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString();
                            //SendEmailSO s = new SendEmailSO(this);
                            //s.flag(SOId); //,TransType);
                            //s.Show();
                        }
                        catch (Exception ex)
                        {
                            Trans.Rollback();
                            MetroFramework.MetroMessageBox.Show(this, "Error message: " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (dataGridView1.Rows[e.RowIndex].Cells["TransStatus"].Value.ToString() == "05")
                        MetroFramework.MetroMessageBox.Show(this, "Cannot send email more than once to " + dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString() + "\nPlease contact admin!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else
                        MetroFramework.MetroMessageBox.Show(this, dataGridView1.Rows[e.RowIndex].Cells["SalesOrderNo"].Value.ToString() + " email not send!\nPlease check the Trans Status!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    RefreshGrid();
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

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
            //if (dataGridView1.Columns[e.ColumnIndex].Name == "CreatedDate" || dataGridView1.Columns[e.ColumnIndex].Name == "UpdatedDate")
            //{
            //    dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            //}
            //else if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
            //    dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";

            //DATE FORMAT
            if (dataGridView1.Columns[e.ColumnIndex].Name == "CreatedDate" || dataGridView1.Columns[e.ColumnIndex].Name == "UpdatedDate" || dataGridView1.Columns[e.ColumnIndex].Name.Contains("ValidTo"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            else if (dataGridView1.Columns[e.ColumnIndex].Name.Contains("Date"))
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            //begin
            //created by : joshua 
            //created date : 12 feb 2018 
            //description : add button on progress
            TransStatus = "'01', '03', '05', '06', '08', '09', '10', '12'"; //BY: HC 27.02.2018 ADDED STATUS 08
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
            TransStatus = "'02', '04', '07', '11'"; //BY: HC 27.02.2018 REMOVED STATUS 08
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
            //end
        }
    }
}
