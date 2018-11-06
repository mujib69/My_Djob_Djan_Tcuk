using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PriceListBeliApproval
{
    public partial class HeaderPLBApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;

        string Query;
        
        public string Gelombang;
        public string Bracket;
        public string PLBNumber = "", tmpPrType = "";
        Purchase.PriceListBeliApproval.InquiryPLBApproval Parent;
        

        String StatusCode = null;
        String UserApproved = null;

        public HeaderPLBApproval()
        {
            InitializeComponent();
           
        }
        
        private void HeaderPLBApproval_Load(object sender, EventArgs e)
        {
            SetCmbVendorReference();        

            dgvPLBDetails.ReadOnly = true;
            dgvVendorReference.ReadOnly = true;
            cmbVendorReference.Enabled = false;
            txtNotes.Enabled = false;
            BeforeEditColor();

            dgvPLBDetails.DefaultCellStyle.BackColor = Color.LightGray;
            dgvVendorReference.DefaultCellStyle.BackColor = Color.LightGray;

            if (StatusCode == "02")
            {
                btnApproved.Enabled = false;
                btnRequestApprovedByManagement.Enabled = false;

                if (ControlMgr.UserId == GetUserApproved(PLBNumber, StatusCode))
                {
                    btnCancelapproved.Enabled = true;
                }
                else
                {
                    btnCancelapproved.Enabled = false;
                }
            }
            else
            {
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
        
        private void SetCmbVendorReference()
        {
            cmbVendorReference.DisplayMember = "Text";
            cmbVendorReference.ValueMember = "Value";

            var items = new[] { 
                new { Text = "-select-", Value = "" }, 
                new { Text = "All Vendor", Value = "1" }, 
                new { Text = "All Vendor Except", Value = "2" },
                new { Text = "Specific Vendor", Value = "3" }
            };

            cmbVendorReference.DataSource = items;
            cmbVendorReference.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void SetParent(Purchase.PriceListBeliApproval.InquiryPLBApproval F)
        {
            Parent = F;
        }

        public void flag(String pricelistno, String statuscode)
        {
            PLBNumber = pricelistno;
            StatusCode = statuscode;
            txtPLBNumber.Text = PLBNumber;
        }

        private void HeaderPLBApproval_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        public void RefreshData()
        {
            if (PLBNumber == "")
            {
                PLBNumber = txtPLBNumber.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            Query = "Select a.[PriceListNo],a.[ValidFrom],a.[ValidTo], a.[StatusCode], a.[CreatedDate], a.Notes, a.Criteria, b.Deskripsi From [PurchPriceListH] a ";
            Query += "left join TransStatusTable b on b.StatusCode = a.StatusCode ";
            Query += "Where PriceListNo = '" + PLBNumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPLBNumber.Text = Dr["PriceListNo"].ToString();
                dtPlbDate.Text = Dr["CreatedDate"].ToString();
                dtFrom.Text = Dr["ValidFrom"].ToString();
                dtTo.Text = Dr["ValidTo"].ToString();
                txtNotes.Text = Dr["Notes"].ToString();
                cmbVendorReference.SelectedValue = Dr["Criteria"].ToString();
            }
            Dr.Close();

            dgvPLBDetails.Rows.Clear();
            if (dgvPLBDetails.RowCount - 1 <= 0)
            {
                dgvPLBDetails.ColumnCount = 25;
                dgvPLBDetails.Columns[0].Name = "No";
                dgvPLBDetails.Columns[1].Name = "FullItemID";
                dgvPLBDetails.Columns[2].Name = "ItemName";
                dgvPLBDetails.Columns[3].Name = "DeliveryMethod";
                dgvPLBDetails.Columns[4].Name = "SeqNoGroup";
               // dgvPLBDetails.Columns[5].Name = "Base";
                dgvPLBDetails.Columns[5].Name = "Price0D";
                dgvPLBDetails.Columns[6].Name = "Price2D";
                dgvPLBDetails.Columns[7].Name = "Price3D";
                dgvPLBDetails.Columns[8].Name = "Price7D";
                dgvPLBDetails.Columns[9].Name = "Price14D";
                dgvPLBDetails.Columns[10].Name = "Price21D";
                dgvPLBDetails.Columns[11].Name = "Price30D";
                dgvPLBDetails.Columns[12].Name = "Price40D";
                dgvPLBDetails.Columns[13].Name = "Price45D";
                dgvPLBDetails.Columns[14].Name = "Price60D";
                dgvPLBDetails.Columns[15].Name = "Price75D";
                dgvPLBDetails.Columns[16].Name = "Price90D";
                dgvPLBDetails.Columns[17].Name = "Price120D";
                dgvPLBDetails.Columns[18].Name = "Price150D";
                dgvPLBDetails.Columns[19].Name = "Price180D";
                dgvPLBDetails.Columns[20].Name = "Tolerance";
               // dgvPLBDetails.Columns[22].Name = "BracketId";
                dgvPLBDetails.Columns[21].Name = "GroupId";
                dgvPLBDetails.Columns[22].Name = "SubGroup1Id";
                dgvPLBDetails.Columns[23].Name = "SubGroup2Id";
                dgvPLBDetails.Columns[24].Name = "ItemId";
            }

            Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[DeliveryMethod],Price0D,Price2D,Price3D,Price7D,Price14D,Price21 AS Price21D,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,Tolerance,GroupId,SubGroupID,SubGroup2ID,ItemID From [PurchPriceListDtl] Where PriceListNo = '" + PLBNumber + "' order by SeqNo asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {

                this.dgvPLBDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[0], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23]);
                i++;
            }
            Dr.Close();

            dgvPLBDetails.ReadOnly = false;
            dgvPLBDetails.Columns["No"].ReadOnly = true;
            dgvPLBDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLBDetails.Columns["ItemName"].ReadOnly = true;
           // dgvPLBDetails.Columns["Base"].ReadOnly = true;

            dgvPLBDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
           // dgvPLBDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
           // dgvPLBDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["GroupId"].Visible = false;
            dgvPLBDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPLBDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPLBDetails.Columns["ItemId"].Visible = false;
         //   dgvPLBDetails.Columns["BracketId"].Visible = false;
            dgvPLBDetails.Columns["SeqNoGroup"].Visible = false;

           // dgvPLBDetails.Columns["Base"].Visible = false;

            dgvPLBDetails.AutoResizeColumns();



            //Vendor
            this.dgvVendorReference.Rows.Clear();
            if (dgvVendorReference.RowCount - 1 <= 0)
            {
                dgvVendorReference.ColumnCount = 3;
                dgvVendorReference.Columns[0].Name = "No";
                dgvVendorReference.Columns[1].Name = "VendID";
                dgvVendorReference.Columns[2].Name = "VendName";                
            }

            Query = "Select VendID, Name AS VendName From [PurchPriceList_VendorList] Where PriceListNo = '" + PLBNumber + "' order by VendID asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int j = 0;
            while (Dr.Read())
            {

                dgvVendorReference.Rows.Add(j + 1, Dr[0], Dr[1]);
                j++;
            }
            Dr.Close();

            dgvVendorReference.ReadOnly = false;
            dgvVendorReference.Columns["No"].ReadOnly = true;
            dgvVendorReference.Columns["VendID"].ReadOnly = true;
            dgvVendorReference.Columns["VendName"].ReadOnly = true;

            dgvVendorReference.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvVendorReference.AutoResizeColumns();


        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvPLBDetails.RowCount; i++)
            {
                dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price0D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price2D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price3D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price7D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price14D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price21D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price30D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price40D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price45D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price60D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price75D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price90D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price120D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price150D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price180D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Tolerance"].Style.BackColor = Color.LightGray;
            }

            for (int i = 0; i < dgvVendorReference.RowCount; i++)
            {
                dgvVendorReference.Rows[i].Cells["VendID"].Style.BackColor = Color.LightGray;
                dgvVendorReference.Rows[i].Cells["VendName"].Style.BackColor = Color.LightGray;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private String GetUserApproved(String PriceListNo, String StatusCode)
        {
            Conn = ConnectionString.GetConnection();
            UserApproved = null;
            Query = "SELECT TOP 1 UserID FROM PurchPriceList_LogTable ";
            Query += "WHERE PriceListNo='" + PriceListNo + "' AND LogStatusCode = '02' ORDER BY LogDate DESC ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                UserApproved = Convert.ToString(Dr["UserID"]);
            }
            Dr.Close();

            return UserApproved;
        }

        private void btnApproved_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "PriceListNo = " + PLBNumber + Environment.NewLine + "Akan approved ? ", "Approved Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                try
                {
                    Query = "Update PurchPriceListH set ";
                    Query += "StatusCode='02', ";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLBNumber.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    String StatusDesc = null;
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='02' AND TransCode = 'PurchPriceList' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                    }
                    Dr.Close();

                    Query = "Insert into PurchPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES('" + dtPlbDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPLBNumber.Text.Trim() + "', '02', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data PriceListNo : " + txtPLBNumber.Text + " berhasil approved.");

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

        private void ModeAfterApproved()
        {
            btnCancelapproved.Enabled = true;
            btnRequestApprovedByManagement.Enabled = false;
            btnApproved.Enabled = false;
        }

        private void btnCancelapproved_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "PriceListNo = " + PLBNumber + Environment.NewLine + "Akan di cancel approved ? ", "Cancel Approved Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                try
                {
                    Query = "Update PurchPriceListH set ";
                    Query += "StatusCode='01', ";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLBNumber.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    String StatusDesc = null;
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='01' AND TransCode = 'PurchPriceList' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                    }
                    Dr.Close();

                    Query = "Insert into PurchPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES('" + dtPlbDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPLBNumber.Text.Trim() + "', '01', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data PriceListNo : " + txtPLBNumber.Text + " berhasil cancel approved.");

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

        private void ModeAfterCancelApproved()
        {
            btnCancelapproved.Enabled = false;
            btnRequestApprovedByManagement.Enabled = true;
            btnApproved.Enabled = true;
        }

        private void btnRequestApprovedByManagement_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "PriceListNo = " + PLBNumber + Environment.NewLine + "Akan request approved by management ? ", "Request Approved By Management Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                try
                {
                    Query = "Update PurchPriceListH set ";
                    Query += "StatusCode='05', ";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLBNumber.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    String StatusDesc = null;
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='05' AND TransCode = 'PurchPriceList' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                    }
                    Dr.Close();

                    Query = "Insert into PurchPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES('" + dtPlbDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPLBNumber.Text.Trim() + "', '05', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data PriceListNo : " + txtPLBNumber.Text + " berhasil request approved by management.");
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

        private void ModeAfterApprovedByManagement()
        {
            btnCancelapproved.Enabled = false;
            btnRequestApprovedByManagement.Enabled = false;
            btnApproved.Enabled = false;
        }

    }
}
