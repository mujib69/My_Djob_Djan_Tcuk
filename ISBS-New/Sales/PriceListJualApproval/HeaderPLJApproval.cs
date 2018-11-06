using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ISBS_New.Sales.PriceListJualApproval
{
    public partial class HeaderPLJApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;

        String Query;
        String PriceListNo = null;
        String StatusCode = null;
        String UserApproved = null;

        Sales.PriceListJualApproval.InquiryPLJApproval Parent;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public HeaderPLJApproval()
        {
            InitializeComponent();
            
        }

        public void setParent(Sales.PriceListJualApproval.InquiryPLJApproval F)
        {
            Parent = F;
        }

        public void flag(String pricelistno, String statuscode)
        {
            PriceListNo = pricelistno;
            StatusCode = statuscode;
        }

        private void HeaderPLJApproval_Load(object sender, EventArgs e)
        {
            SetCmbCustomer();
            dgvPLJDetails.ReadOnly = true;
            dgvCustomer.ReadOnly = true;
            cmbCustomer.Enabled = false;
            txtNotes.Enabled = false;
            BeforeEditColor();

            dgvPLJDetails.DefaultCellStyle.BackColor = Color.LightGray;
            dgvCustomer.DefaultCellStyle.BackColor = Color.LightGray;

            if (StatusCode == "02")
            {
                btnApproved.Enabled = false;
                btnRequestApprovedByManagement.Enabled = false;

                if (ControlMgr.UserId == GetUserApproved(PriceListNo, StatusCode))
                {
                    btnCancelapproved.Enabled = true;
                }
                else
                {
                    btnCancelapproved.Enabled = false; 
                }
            }
            else {
                btnApproved.Enabled = true;
                btnCancelapproved.Enabled = false;
                btnRequestApprovedByManagement.Enabled = true;

                if (StatusCode == "05")
                {
                  // ControlMgr.GroupName = "Management";
                    if (ControlMgr.GroupName == "Management")
                    {
                        btnRequestApprovedByManagement.Enabled = false;
                   }
                  else
                  {
                      btnRequestApprovedByManagement.Enabled = false;
                        btnApproved.Enabled = false;
                  }
                }
            }          

            RefreshData();
        }

        private void SetCmbCustomer()
        {
            cmbCustomer.DisplayMember = "Text";
            cmbCustomer.ValueMember = "Value";

            var items = new[] { 
                new { Text = "-select-", Value = "" }, 
                new { Text = "All Customer", Value = "1" }, 
                new { Text = "All Customer Except", Value = "2" },
                new { Text = "Specific Customer", Value = "3" }
            };

            cmbCustomer.DataSource = items;
            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void RefreshData()
        {
            if (PriceListNo == "")
            {
                PriceListNo = txtPLJNumber.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            Query = "Select a.[PriceListNo],a.[ValidFrom],a.[ValidTo],a.[StatusCode], a.[CreatedDate],b.Deskripsi, a.Criteria, a.Notes From [SalesPriceListH] a ";
            Query += "left join TransStatusTable b on b.StatusCode = a.StatusCode ";
            Query += "Where PriceListNo = '" + PriceListNo + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPLJNumber.Text = Dr["PriceListNo"].ToString();
                dtPljDate.Text = Dr["CreatedDate"].ToString();
                dtFrom.Text = Dr["ValidFrom"].ToString();
                dtTo.Text = Dr["ValidTo"].ToString();
                txtNotes.Text = Dr["Notes"].ToString();
                cmbCustomer.SelectedValue = Dr["Criteria"].ToString();
            }
            Dr.Close();

            dgvPLJDetails.Rows.Clear();
            if (dgvPLJDetails.RowCount - 1 <= 0)
            {
                dgvPLJDetails.ColumnCount = 26;
                dgvPLJDetails.Columns[0].Name = "No";
                dgvPLJDetails.Columns[1].Name = "FullItemID";
                dgvPLJDetails.Columns[2].Name = "ItemName";
                dgvPLJDetails.Columns[3].Name = "DeliveryMethod";
                dgvPLJDetails.Columns[4].Name = "SeqNoGroup";
                dgvPLJDetails.Columns[5].Name = "Base";
                dgvPLJDetails.Columns[6].Name = "Price0D";
                dgvPLJDetails.Columns[7].Name = "Price2D";
                dgvPLJDetails.Columns[8].Name = "Price3D";
                dgvPLJDetails.Columns[9].Name = "Price7D";
                dgvPLJDetails.Columns[10].Name = "Price14D";
                dgvPLJDetails.Columns[11].Name = "Price21D";
                dgvPLJDetails.Columns[12].Name = "Price30D";
                dgvPLJDetails.Columns[13].Name = "Price40D";
                dgvPLJDetails.Columns[14].Name = "Price45D";
                dgvPLJDetails.Columns[15].Name = "Price60D";
                dgvPLJDetails.Columns[16].Name = "Price75D";
                dgvPLJDetails.Columns[17].Name = "Price90D";
                dgvPLJDetails.Columns[18].Name = "Price120D";
                dgvPLJDetails.Columns[19].Name = "Price150D";
                dgvPLJDetails.Columns[20].Name = "Price180D";
                dgvPLJDetails.Columns[21].Name = "BracketId";
                dgvPLJDetails.Columns[22].Name = "GroupId";
                dgvPLJDetails.Columns[23].Name = "SubGroup1Id";
                dgvPLJDetails.Columns[24].Name = "SubGroup2Id";
                dgvPLJDetails.Columns[25].Name = "ItemId";
            }

            Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[DeliveryMethod],Base,Price0D,Price2D,Price3D,Price7D,Price14D,Price21 AS Price21D,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,BracketID,GroupId,SubGroupID,SubGroup2ID,ItemID From [SalesPriceListDtl] Where PriceListNo = '" + PriceListNo + "' order by SeqNo asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {

                this.dgvPLJDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], "", Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23], Dr[24]);
                i++;
            }
            Dr.Close();

            dgvPLJDetails.ReadOnly = false;
            dgvPLJDetails.Columns["No"].ReadOnly = true;
            dgvPLJDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLJDetails.Columns["ItemName"].ReadOnly = true;
            dgvPLJDetails.Columns["Base"].ReadOnly = true;

            dgvPLJDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPLJDetails.Columns["GroupId"].Visible = false;
            dgvPLJDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPLJDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPLJDetails.Columns["ItemId"].Visible = false;
            dgvPLJDetails.Columns["BracketId"].Visible = false;
            dgvPLJDetails.Columns["SeqNoGroup"].Visible = false;

            dgvPLJDetails.Columns["Base"].Visible = false;

            dgvPLJDetails.AutoResizeColumns();



            //Customer
            this.dgvCustomer.Rows.Clear();
            if (dgvCustomer.RowCount - 1 <= 0)
            {
                dgvCustomer.ColumnCount = 3;
                dgvCustomer.Columns[0].Name = "No";
                dgvCustomer.Columns[1].Name = "CustID";
                dgvCustomer.Columns[2].Name = "CustName";
            }

            Query = "Select CustID, Name AS CustName From [SalesPriceList_CustList] Where PriceListNo = '" + PriceListNo + "' order by CustID asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int j = 0;
            while (Dr.Read())
            {

                dgvCustomer.Rows.Add(j + 1, Dr[0], Dr[1]);
                j++;
            }
            Dr.Close();

            dgvCustomer.ReadOnly = false;
            dgvCustomer.Columns["No"].ReadOnly = true;
            dgvCustomer.Columns["CustID"].ReadOnly = true;
            dgvCustomer.Columns["CustName"].ReadOnly = true;

            dgvCustomer.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvCustomer.AutoResizeColumns();


        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvPLJDetails.RowCount; i++)
            {
                dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price0D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price2D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price3D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price7D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price14D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price21D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price30D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price40D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price45D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price60D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price75D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price90D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price120D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price150D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price180D"].Style.BackColor = Color.LightGray;
            }

            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {
                dgvCustomer.Rows[i].Cells["CustID"].Style.BackColor = Color.LightGray;
                dgvCustomer.Rows[i].Cells["CustName"].Style.BackColor = Color.LightGray;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HeaderPLJApproval_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        private void btnApproved_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "PriceListNo = " + PriceListNo + Environment.NewLine + "Akan approved ? ", "Approved Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();
                    try
                    {
                        Query = "Update SalesPriceListH set ";
                        Query += "StatusCode='02', ";
                        Query += "UpdatedDate=getdate(),";
                        Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLJNumber.Text.Trim() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        String StatusDesc = null;
                        Query = "SELECT Deskripsi FROM TransStatusTable ";
                        Query += "WHERE StatusCode='02' AND TransCode = 'SalesPriceList' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                        }
                        Dr.Close();

                        string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                        Query = "Insert into SalesPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                        Query += "VALUES('" + dtPljDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPLJNumber.Text.Trim() + "', '02', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data PriceListNo : " + txtPLJNumber.Text + " berhasil approved.");

                    }


                    catch (Exception)
                    {
                        Trans.Rollback();
                        return;
                    }
                    finally
                    {
                        Conn.Close();
                        RefreshData();
                        Parent.RefreshGrid();
                        ModeAfterApproved();
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end              
        }

        private void ModeAfterApproved()
        {
            btnCancelapproved.Enabled = true;
            btnRequestApprovedByManagement.Enabled = false;
            btnApproved.Enabled = false;
        }

        private String GetUserApproved(String PriceListNo, String StatusCode)
        {
            Conn = ConnectionString.GetConnection();
            UserApproved = null;
            Query = "SELECT TOP 1 UserID FROM SalesPriceList_LogTable ";
            Query += "WHERE PriceListNo='" + PriceListNo + "' AND LogStatusCode = '02' ORDER BY LogDate DESC "; Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                UserApproved = Convert.ToString(Dr["UserID"]);
            }
            Dr.Close();

            return UserApproved;
        }

        private void btnCancelapproved_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "PriceListNo = " + PriceListNo + Environment.NewLine + "Akan di cancel approved ? ", "Cancel Approved Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();
                    try
                    {
                        Query = "Update SalesPriceListH set ";
                        Query += "StatusCode='01', ";
                        Query += "UpdatedDate=getdate(),";
                        Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLJNumber.Text.Trim() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        String StatusDesc = null;
                        Query = "SELECT Deskripsi FROM TransStatusTable ";
                        Query += "WHERE StatusCode='01' AND TransCode = 'SalesPriceList' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                        }
                        Dr.Close();

                        string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                        Query = "Insert into SalesPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                        Query += "VALUES('" + dtPljDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPLJNumber.Text.Trim() + "', '01', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data PriceListNo : " + txtPLJNumber.Text + " berhasil cancel approved.");

                    }


                    catch (Exception)
                    {
                        Trans.Rollback();
                        return;
                    }
                    finally
                    {
                        Conn.Close();
                        RefreshData();
                        Parent.RefreshGrid();
                        ModeAfterCancelApproved();
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void ModeAfterCancelApproved()
        {
            btnCancelapproved.Enabled = false;
            btnRequestApprovedByManagement.Enabled = true;
            btnApproved.Enabled = true;
        }

        private void btnApprovedByManagement_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "PriceListNo = " + PriceListNo + Environment.NewLine + "Akan request approved by management ? ", "Request Approved By Management Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();
                    try
                    {
                        Query = "Update SalesPriceListH set ";
                        Query += "StatusCode='05', ";
                        Query += "UpdatedDate=getdate(),";
                        Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLJNumber.Text.Trim() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        String StatusDesc = null;
                        Query = "SELECT Deskripsi FROM TransStatusTable ";
                        Query += "WHERE StatusCode='05' AND TransCode = 'SalesPriceList' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                        }
                        Dr.Close();

                        string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                        Query = "Insert into SalesPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                        Query += "VALUES('" + dtPljDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPLJNumber.Text.Trim() + "', '05', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Trans.Commit();
                        MessageBox.Show("Data PriceListNo : " + txtPLJNumber.Text + " berhasil request approved by management.");
                    }

                    catch (Exception)
                    {
                        Trans.Rollback();
                        return;
                    }
                    finally
                    {
                        Conn.Close();
                        RefreshData();
                        Parent.RefreshGrid();
                        ModeAfterApprovedByManagement();
                    }
                }   
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end  
        }

        private void ModeAfterApprovedByManagement()
        {
            btnCancelapproved.Enabled = false;
            btnRequestApprovedByManagement.Enabled = false;
            btnApproved.Enabled = false;
        }
    }
}
