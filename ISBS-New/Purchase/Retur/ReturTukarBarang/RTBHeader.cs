using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.ReturTukarBarang
{
    public partial class RTBHeader : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd,cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Status, Query, crit, RTBNumber, GRID = null;
        Purchase.Retur.ReturTukarBarang.InqRTB Parent;

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public RTBHeader()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.ReturTukarBarang.InqRTB f)
        {
            Parent = f;

        }

        private void RTBHeader_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
            GetDataHeader();
        }

        public void SetMode(string tmpMode, string tmpRTBNumber)
        {
            Mode = tmpMode;
            RTBNumber = tmpRTBNumber;
        }

        public void CreateGrid()
        {
            if (dgvRTB.RowCount == 0)
            {
                dgvRTB.Rows.Clear();
                dgvRTB.ColumnCount = 19;
                dgvRTB.Columns[0].Name = "No";
                dgvRTB.Columns[1].Name = "ItemID";
                dgvRTB.Columns[2].Name = "FullItemID"; dgvRTB.Columns["FullItemID"].HeaderText = "Item ID";
                dgvRTB.Columns[3].Name = "ItemName"; dgvRTB.Columns["ItemName"].HeaderText = "Name";
                dgvRTB.Columns[4].Name = "GroupId";
                dgvRTB.Columns[5].Name = "SubGroup1ID";
                dgvRTB.Columns[6].Name = "SubGroup2ID";
                dgvRTB.Columns[7].Name = "RemainingQty";
                dgvRTB.Columns[8].Name = "UoM_Qty";
                dgvRTB.Columns[9].Name = "UoM_Unit";
                dgvRTB.Columns[10].Name = "Alt_Qty";
                dgvRTB.Columns[11].Name = "Alt_Unit";
                dgvRTB.Columns[12].Name = "Ratio";
                dgvRTB.Columns[13].Name = "Ratio_Actual";
                dgvRTB.Columns[14].Name = "GoodsReceivedSeqNo";
                dgvRTB.Columns[15].Name = "InventSiteBlokID";
                dgvRTB.Columns[16].Name = "Quality";
                dgvRTB.Columns[17].Name = "Notes";
                dgvRTB.Columns[18].Name = "OldQty";
            }
        }

        public void ModeNew()
        {
            RTBNumber = "";
            dtRTB.Value = DateTime.Now;
            txtNotes.Enabled = true;
            dgvRTB.ReadOnly = false;

            btnSearchGR.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

        }

        public void ModeGenerate()
        {
            RTBNumber = "";
            dtRTB.Value = DateTime.Now;
            txtNotes.Enabled = true;
            dgvRTB.ReadOnly = false;

            btnSearchGR.Visible = false;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtNotes.Enabled = true;
            dgvRTB.ReadOnly = false;

            btnSearchGR.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            dgvRTB.ReadOnly = false;
            dgvRTB.Columns["No"].ReadOnly = true;
            dgvRTB.Columns["FullItemID"].ReadOnly = true;
            dgvRTB.Columns["ItemName"].ReadOnly = true;
            dgvRTB.Columns["RemainingQty"].ReadOnly = true;
            dgvRTB.Columns["UoM_Qty"].ReadOnly = false;
            dgvRTB.Columns["UoM_Unit"].ReadOnly = true;
            dgvRTB.Columns["Alt_Qty"].ReadOnly = true;
            dgvRTB.Columns["Alt_Unit"].ReadOnly = true;
            dgvRTB.Columns["Ratio"].ReadOnly = true;
            dgvRTB.Columns["Ratio_Actual"].ReadOnly = true;
            dgvRTB.Columns["InventSiteBlokID"].ReadOnly = true;
            dgvRTB.Columns["Quality"].ReadOnly = true;
            dgvRTB.Columns["Notes"].ReadOnly = true;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            GetDataHeader();

            txtNotes.Enabled = false;
            dgvRTB.ReadOnly = true;

            btnSearchGR.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;


            dgvRTB.ReadOnly = true;
            dgvRTB.Columns["No"].ReadOnly = true;
            dgvRTB.Columns["FullItemID"].ReadOnly = true;
            dgvRTB.Columns["ItemName"].ReadOnly = true;
            dgvRTB.Columns["RemainingQty"].ReadOnly = true;
            dgvRTB.Columns["UoM_Qty"].ReadOnly = false;
            dgvRTB.Columns["UoM_Unit"].ReadOnly = true;
            dgvRTB.Columns["Alt_Qty"].ReadOnly = true;
            dgvRTB.Columns["Alt_Unit"].ReadOnly = true;
            dgvRTB.Columns["Ratio"].ReadOnly = true;
            dgvRTB.Columns["Ratio_Actual"].ReadOnly = true;
            dgvRTB.Columns["InventSiteBlokID"].ReadOnly = true;
            dgvRTB.Columns["Quality"].ReadOnly = true;
            dgvRTB.Columns["Notes"].ReadOnly = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                if (txtStatusName.Text.Contains("Approve") || txtStatusName.Text.Contains("Reject"))
                {
                    MessageBox.Show("Retur tukar barang sudah diapprove/reject.");
                    return;
                }
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            GetDataHeader();
        }

        public void GetDataHeader()
        {
            if (RTBNumber != "")
            {
                dgvRTB.Rows.Clear();
                Conn = ConnectionString.GetConnection();
                Query = "Select Top 1 RTBDate, GoodsReceivedID, VendID, VendName, SiteID, SiteName, SiteLocation, Notes, b.Deskripsi, ApprovedBy From [ReturTukarBarangH] a Left JOIN [TransStatusTable] b ON a.TransStatus = b.StatusCode And b.TransCode = 'ReturTukarBarang' Where RTBId = '" + RTBNumber + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtRTB.Text = Dr["RTBDate"].ToString();
                    txtRTBNum.Text = RTBNumber;
                    txtGRNum.Text = Dr["GoodsReceivedID"].ToString();
                    txtVendID.Text = Dr["VendID"].ToString();
                    txtVendName.Text = Dr["VendName"].ToString();
                    txtSiteID.Text = Dr["SiteID"].ToString();
                    txtSiteName.Text = Dr["SiteName"].ToString();
                    txtSiteLocation.Text = Dr["SiteLocation"].ToString();
                    txtNotes.Text = Dr["Notes"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString();
                    txtApproved.Text = Dr["ApprovedBy"].ToString();
                }
                Dr.Close();

                CreateGrid();

                Query = "Select SeqNo, ItemId, [FullItemID], ItemName, GroupId, SubGroup1Id, SubGroup2Id, [UoM_Qty], [UoM_Unit], [Alt_Qty], [Alt_Unit], Ratio, Ratio_Actual, [GoodsReceivedID], [GoodsReceivedSeqNo], [InventSiteId], [InventSiteBlokID], [Quality], Notes From [ReturTukarBarangD] Where RTBId = '" + RTBNumber + "' order by SeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    Query = "Select Remaining_Qty From [GoodsReceivedD] Where GoodsReceivedId = '" + txtGRNum.Text + "' And FullItemID = '" + Dr["FullItemID"] + "'";
                    cmd = new SqlCommand(Query, Conn);

                    this.dgvRTB.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceivedSeqNo"], Dr["InventSiteBlokID"], Dr["Quality"], Dr["Notes"], Dr["UoM_Qty"]);
                    j++;
                }
                Dr.Close();
                
                dgvRTB.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["InventSiteBlokID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Quality"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvRTB.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvRTB.Columns["ItemID"].Visible = false;
                dgvRTB.Columns["GroupId"].Visible = false;
                dgvRTB.Columns["SubGroup1ID"].Visible = false;
                dgvRTB.Columns["SubGroup2ID"].Visible = false;
                dgvRTB.Columns["Ratio"].Visible = false;
                dgvRTB.Columns["GoodsReceivedSeqNo"].Visible = false;
                dgvRTB.Columns["OldQty"].Visible = false;
                
                dgvRTB.AutoResizeColumns();

                Conn.Close();
            }
        }

        private void btnSearchGR_Click(object sender, EventArgs e)
        {
            AddGR f = new AddGR();
            f.setParent(this);
            f.Show();
        }

        public void AddGR(String grnumber)
        {
            txtGRNum.Text = grnumber;

            Conn = ConnectionString.GetConnection();
            Query = "Select Top 1 VendID, VendorName, SiteID, SiteName, SiteLocation From [GoodsReceivedH] Where GoodsReceivedId = '" + grnumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtVendID.Text = Dr["VendID"].ToString();
                txtVendName.Text = Dr["VendorName"].ToString();
                txtSiteID.Text = Dr["SiteID"].ToString();
                txtSiteName.Text = Dr["SiteName"].ToString();
                txtSiteLocation.Text = Dr["SiteLocation"].ToString();
            }
            Dr.Close();

            dgvRTB.Rows.Clear();
        }

        public void AddDataGridDetail(List<string> SeqNo)
        {
            Conn = ConnectionString.GetConnection();
            CreateGrid();

            for (int i = 0; i < SeqNo.Count; i++)
            {
                Query = "Select * From [GoodsReceivedD]a INNER JOIN [InventTable]b ON a.FullItemID = b.FullItemID Where a.[GoodsReceivedId] = '" + txtGRNum.Text + "' AND a.GoodsReceivedSeqNo = '" + SeqNo[i] + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    Query = "Select Remaining_Qty From [GoodsReceivedD] Where GoodsReceivedId = '" + txtGRNum.Text + "' And GoodsReceivedSeqNo = '" + Dr["GoodsReceivedSeqNo"] + "'";
                    cmd = new SqlCommand(Query, Conn);

                    this.dgvRTB.Rows.Add(dgvRTB.RowCount + 1, Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), Dr["Qty_Actual"], Dr["UoM"], Dr["TotalBerat_Actual"], Dr["UoMAlt"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceivedSeqNo"], Dr["InventSiteBlokID"], Dr["Quality"], Dr["Notes"], Dr["Qty_Actual"]);
                }
                Dr.Close();
            }

            dgvRTB.ReadOnly = false;
            //dgvRTB.DefaultCellStyle.BackColor = Color.White;
            dgvRTB.Columns["No"].ReadOnly = true;
            dgvRTB.Columns["FullItemID"].ReadOnly = true;
            dgvRTB.Columns["ItemName"].ReadOnly = true;
            dgvRTB.Columns["RemainingQty"].ReadOnly = true;
            dgvRTB.Columns["UoM_Qty"].ReadOnly = false;
            dgvRTB.Columns["UoM_Unit"].ReadOnly = true;
            dgvRTB.Columns["Alt_Qty"].ReadOnly = true;
            dgvRTB.Columns["Alt_Unit"].ReadOnly = true;
            dgvRTB.Columns["Ratio"].ReadOnly = true;
            dgvRTB.Columns["Ratio_Actual"].ReadOnly = true;
            dgvRTB.Columns["InventSiteBlokID"].ReadOnly = true;
            dgvRTB.Columns["Quality"].ReadOnly = true;
            dgvRTB.Columns["Notes"].ReadOnly = true;

            dgvRTB.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["InventSiteBlokID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["Quality"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvRTB.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvRTB.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRTB.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRTB.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRTB.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvRTB.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvRTB.Columns["ItemID"].Visible = false;
            dgvRTB.Columns["GroupId"].Visible = false;
            dgvRTB.Columns["SubGroup1ID"].Visible = false;
            dgvRTB.Columns["SubGroup2ID"].Visible = false;
            dgvRTB.Columns["Ratio"].Visible = false;
            dgvRTB.Columns["GoodsReceivedSeqNo"].Visible = false;
            dgvRTB.Columns["OldQty"].Visible = false;

            dgvRTB.AutoResizeColumns();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtGRNum.Text == "")
            {
                MessageBox.Show("Pilih Good Received terlebih dahulu.");
            }
            else
            {
                List<string> seqno = new List<string>();

                if (dgvRTB.RowCount > 0)
                {
                    for (int i = 0; i < dgvRTB.RowCount; i++)
                    {
                        seqno.Add(dgvRTB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString());
                    }
                }

                AddItem add = new AddItem();
                add.setParent(this);
                add.setMode(txtGRNum.Text, seqno);
                add.Show();
            }
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRTB.RowCount > 0)
            {
                dgvRTB.Rows.RemoveAt(dgvRTB.CurrentRow.Index);

                for (int i = 0; i < dgvRTB.RowCount; i++)
                {
                    dgvRTB.Rows[i].Cells["No"].Value = i + 1;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            decimal QtyNew = 0;

            if (dgvRTB.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }

            try
            {
                if (Mode == "New" && txtRTBNum.Text.Trim() == "")
                {
                    //Old Code=======================================================================================                   
                    //Query = "Insert into [ReturTukarBarangH] (RTBId,RTBDate,TransType,VendId,VendName,GoodsReceivedID,SiteID,SiteName,SiteLocation,Notes,TransStatus,CreatedDate,CreatedBy) OUTPUT INSERTED.RTBId values ";
                    //Query += "((Select 'RTB-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(RTBId) is null then '1' else substring(Max(RTBId),11,4)+1 end), 5) ";
                    //Query += "from [ReturTukarBarangH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    //Query += "'" + dtRTB.Value.ToString("yyyy-MM-dd") + "',";
                    //Query += "'MANUAL',";
                    //Query += "'" + txtVendID.Text +  "',";
                    //Query += "'" + txtVendName.Text + "',";
                    //Query += "'" + txtGRNum.Text + "',";
                    //Query += "'" + txtSiteID.Text + "',";
                    //Query += "'" + txtSiteName.Text + "',";
                    //Query += "'" + txtSiteLocation.Text + "',";
                    //Query += "'" + txtNotes.Text + "',";
                    //Query += "'01',";
                    //Query += "getdate(),'" + ControlMgr.UserId + "');";
                    //Cmd = new SqlCommand(Query, Conn, Trans);

                    //RTBNumber = Cmd.ExecuteScalar().ToString();
                    //End Old Code=====================================================================================

                    //begin============================================================================================
                    //updated by : joshua
                    //updated date : 14 Feb 2018
                    //description : change generate sequence number, get from global function and update counter 
                    string Jenis = "NRB", Kode = "NRB";
                    RTBNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "Insert into [ReturTukarBarangH] (RTBId,RTBDate,TransType,VendId,VendName,GoodsReceivedID,SiteID,SiteName,SiteLocation,Notes,TransStatus,CreatedDate,CreatedBy) values ";
                    Query += "('" + RTBNumber + "',";
                    Query += "'" + dtRTB.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "'MANUAL',";
                    Query += "'" + txtVendID.Text + "',";
                    Query += "'" + txtVendName.Text + "',";
                    Query += "'" + txtGRNum.Text + "',";
                    Query += "'" + txtSiteID.Text + "',";
                    Query += "'" + txtSiteName.Text + "',";
                    Query += "'" + txtSiteLocation.Text + "',";
                    Query += "'" + txtNotes.Text + "',";
                    Query += "'01',";
                    Query += "getdate(),'" + ControlMgr.UserId + "');";                   
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //update counter
                    //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                    //end update counter
                    //end=============================================================================================
                    

                    Query = "";
                    
                    for (int i = 0; i <= dgvRTB.RowCount - 1; i++)
                    {
                        if (dgvRTB.Rows[i].Cells["UoM_Qty"].Value == null)
                        {
                            MessageBox.Show("Terdapat quantity yang belum diisi.");
                            return;
                        }
                        if (decimal.Parse(dgvRTB.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvRTB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Terdapat quantity yang melebihi batas.");
                            return;
                        }

                        Query += "Insert [ReturTukarBarangD] (RTBId,SeqNo,ItemId,FullItemId,ItemName,GroupId,SubGroup1Id,SubGroup2Id,UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,Ratio_Actual,GoodsReceivedID,GoodsReceivedSeqNo,InventSiteId,InventSiteBlokID,Quality,Notes,CreatedDate,CreatedBy) Values ";
                        Query += "('" + RTBNumber + "','";

                        Query += (dgvRTB.Rows[i].Cells["No"].Value == "" ? "" : dgvRTB.Rows[i].Cells["No"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["ItemID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvRTB.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvRTB.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvRTB.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvRTB.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Alt_Qty"].Value == null ? "0" : dgvRTB.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Ratio"].Value == "" ? "0" : dgvRTB.Rows[i].Cells["Ratio"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Ratio_Actual"].Value == "" ? "0" : dgvRTB.Rows[i].Cells["Ratio_Actual"].Value.ToString()) + "','";
                        Query += txtGRNum.Text + "','";
                        Query += (dgvRTB.Rows[i].Cells["GoodsReceivedSeqNo"].Value == "" ? "" : dgvRTB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString()) + "','";
                        Query += txtSiteID.Text + "','";
                        Query += (dgvRTB.Rows[i].Cells["InventSiteBlokID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["InventSiteBlokID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Quality"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Quality"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Notes"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Notes"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";

                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvRTB.Rows[i].Cells["RemainingQty"].Value.ToString()) - decimal.Parse(dgvRTB.Rows[i].Cells["UoM_Qty"].Value.ToString());

                        Query = "Update [GoodsReceivedD] set ";
                        Query += "Remaining_Qty='" + QtyNew + "' ";
                        Query += "where GoodsReceivedId='" + txtGRNum.Text.Trim() + "' And FullItemID='" + dgvRTB.Rows[i].Cells["FullItemID"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";

                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            Query = "";
                        }
                    }

                    if (Query != "")
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                    }

                    

                    Trans.Commit();
                    MessageBox.Show("Data RTBNumber : " + RTBNumber + " berhasil ditambahkan.");
                    txtRTBNum.Text = RTBNumber;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
                else
                {
                    DateTime CreatedDate = DateTime.Now;
                    String CreatedBy = "";

                    Query = "Update [ReturTukarBarangH] set ";
                    Query += "VendId='" + txtVendID.Text + "',";
                    Query += "VendName='" + txtVendName.Text + "',";
                    Query += "GoodsReceivedID='" + txtGRNum.Text + "',";
                    Query += "SiteID='" + txtSiteID.Text + "',";
                    Query += "SiteName='" + txtSiteName.Text + "',";
                    Query += "SiteLocation='" + txtSiteLocation.Text + "',";
                    Query += "Notes='" + txtNotes.Text + "',";
                    Query += "TransStatus = '01',";
                    Query += "ApprovedBy = '',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where RTBId='" + txtRTBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "";

                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                        CreatedBy = Dr["CreatedBy"].ToString();
                    }
                    Dr.Close();

                    //Update Qty
                    List<string> GoodsReceivedSeqNo = new List<string>();
                    List<string> GoodsReceivedID = new List<string>();
                    List<decimal> Qty = new List<decimal>();
                    decimal RemainingQty, QtyNew2 = 0;
                    Query = "Select GoodsReceivedSeqNo,UoM_Qty,[GoodsReceivedID] From [ReturTukarBarangD] Where RTBId='" + txtRTBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn,Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        GoodsReceivedSeqNo.Add(Dr["GoodsReceivedSeqNo"].ToString());
                        GoodsReceivedID.Add(Dr["GoodsReceivedID"].ToString());
                        Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                    }
                    Dr.Close();

                    for (int i = 0; i < GoodsReceivedSeqNo.Count; i++)
                    {
                        Query = "Select Remaining_Qty From [GoodsReceivedD] Where [GoodsReceivedId] = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn,Trans);
                        RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                        QtyNew2 = RemainingQty + Qty[i];

                        Query = "Update [GoodsReceivedD] set ";
                        Query += "Remaining_Qty='" + QtyNew2 + "' ";
                        Query += "where GoodsReceivedId='" + GoodsReceivedID[i] + "' And GoodsReceivedSeqNo='" + GoodsReceivedSeqNo[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    Query = "Delete from [ReturTukarBarangD] where RTBId='" + txtRTBNum.Text.Trim() + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    Query = "";

                    for (int i = 0; i <= dgvRTB.RowCount - 1; i++)
                    {
                        if (dgvRTB.Rows[i].Cells["UoM_Qty"].Value == null)
                        {
                            MessageBox.Show("Terdapat quantity yang belum diisi.");
                            return;
                        }
                        if ((decimal.Parse(dgvRTB.Rows[i].Cells["RemainingQty"].Value.ToString()) + decimal.Parse(dgvRTB.Rows[i].Cells["OldQty"].Value.ToString())) < decimal.Parse(dgvRTB.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Terdapat quantity yang melebihi batas.");
                            return;
                        }

                        Query += "Insert [ReturTukarBarangD] (RTBId,SeqNo,ItemId,FullItemId,ItemName,GroupId,SubGroup1Id,SubGroup2Id,UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,Ratio_Actual,GoodsReceivedID,GoodsReceivedSeqNo,InventSiteId,InventSiteBlokID,Quality,Notes,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) Values ";
                        Query += "('" + RTBNumber + "','";

                        Query += (dgvRTB.Rows[i].Cells["No"].Value == "" ? "" : dgvRTB.Rows[i].Cells["No"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["ItemID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvRTB.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvRTB.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvRTB.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvRTB.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Alt_Qty"].Value == "" ? "0" : dgvRTB.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Ratio"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Ratio"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Ratio_Actual"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Ratio_Actual"].Value.ToString()) + "','";
                        Query += txtGRNum.Text + "','";
                        Query += (dgvRTB.Rows[i].Cells["GoodsReceivedSeqNo"].Value == "" ? "" : dgvRTB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString()) + "','";
                        Query += txtSiteID.Text + "','";
                        Query += (dgvRTB.Rows[i].Cells["InventSiteBlokID"].Value == "" ? "" : dgvRTB.Rows[i].Cells["InventSiteBlokID"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Quality"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Quality"].Value.ToString()) + "','";
                        Query += (dgvRTB.Rows[i].Cells["Notes"].Value == "" ? "" : dgvRTB.Rows[i].Cells["Notes"].Value.ToString()) + "',";
                        Query += "'" + CreatedDate + "',";
                        Query += "'" + CreatedBy + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";

                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvRTB.Rows[i].Cells["RemainingQty"].Value.ToString()) + decimal.Parse(dgvRTB.Rows[i].Cells["OldQty"].Value.ToString()) - decimal.Parse(dgvRTB.Rows[i].Cells["UoM_Qty"].Value.ToString());

                        Query = "Update [GoodsReceivedD] set ";
                        Query += "Remaining_Qty='" + QtyNew + "' ";
                        Query += "where GoodsReceivedId='" + txtGRNum.Text.Trim() + "' And GoodsReceivedSeqNo='" + dgvRTB.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";

                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            Query = "";
                        }
                    }

                    if (Query != "")
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        Query = "";
                    }

                    Trans.Commit();
                    MessageBox.Show("Data RTBNumber : " + txtRTBNum.Text + " berhasil diupdate.");
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
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
                //this.Close();
            }
        }

        private void dgvRTB_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvRTB.RowCount >0)
            {
                if (dgvRTB.Columns[e.ColumnIndex].Name == "UoM_Qty")
                {
                    if (dgvRTB.CurrentRow.Cells["UoM_Qty"].Value == null)
                    {
                        dgvRTB.CurrentRow.Cells["Alt_Qty"].Value = "";
                    }
                    else
                    {
                        dgvRTB.CurrentRow.Cells["Alt_Qty"].Value = decimal.Parse(dgvRTB.CurrentRow.Cells["UoM_Qty"].Value.ToString()) * decimal.Parse(dgvRTB.CurrentRow.Cells["Ratio_Actual"].Value.ToString());
                    }
                }
            }
        }

        private void dgvRTB_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dgvRTB.CurrentCell.ColumnIndex == dgvRTB.Columns["UoM_Qty"].Index) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }
    }
}
