using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.NotaReturJual
{
    public partial class ReturJualHeader : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd, cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Status, Query, crit, NRJNumber, BBKNumber = null;
        Purchase.Retur.NotaReturJual.InqReturJual Parent;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public ReturJualHeader()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.Retur.NotaReturJual.InqReturJual f)
        {
            Parent = f;
        }

        private void ReturJualHeader_Load(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName.Contains("Manager"))
            {
                gbApprove.Visible = true;
            }
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

        public void SetMode(string tmpMode, string tmpNRJNumber)
        {
            Mode = tmpMode;
            NRJNumber = tmpNRJNumber;
        }

        public void CreateGrid()
        {
            if (dgvNRJ.RowCount - 1 <= 0)
            {
                dgvNRJ.ColumnCount = 18;
                dgvNRJ.Columns[0].Name = "No";
                dgvNRJ.Columns[1].Name  = "ItemID";
                dgvNRJ.Columns[2].Name = "FullItemID"; dgvNRJ.Columns["FullItemID"].HeaderText = "Item ID";
                dgvNRJ.Columns[3].Name = "ItemName"; dgvNRJ.Columns["ItemName"].HeaderText = "Name";
                dgvNRJ.Columns[4].Name = "GroupId";
                dgvNRJ.Columns[5].Name = "SubGroup1ID";
                dgvNRJ.Columns[6].Name = "SubGroup2ID";
                dgvNRJ.Columns[7].Name = "RemainingQty";
                dgvNRJ.Columns[8].Name = "UoM_Qty";
                dgvNRJ.Columns[9].Name = "UoM_Unit";
                dgvNRJ.Columns[10].Name = "Alt_Qty";
                dgvNRJ.Columns[11].Name = "Alt_Unit";
                dgvNRJ.Columns[12].Name = "Ratio";
                dgvNRJ.Columns[13].Name = "ActionCode";
                dgvNRJ.Columns[14].Name = "GoodsIssuedID";
                dgvNRJ.Columns[15].Name = "GoodsIssued_SeqNo";
                dgvNRJ.Columns[16].Name = "Notes";
                dgvNRJ.Columns[17].Name = "OldQty";
            }
        }

        public void ModeNew()
        {
            NRJNumber = "";
            dtNRJ.Value = DateTime.Now;
            dtExpectedReturnDate.Value = DateTime.Now;
            dtExpectedReturnDate.Enabled = true;
            txtVehicleType.Enabled = true;
            txtVehicleNumber.Enabled = true;
            txtDriverName.Enabled = true;
            txtNotes.Enabled = true;
            dgvNRJ.ReadOnly = false;

            btnSearchBBK.Enabled = true;
            btnSearchInventSite.Enabled = true;
            btnSearchVehicleOwner.Enabled = true;
            cbSame.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            GetDataHeader();

            dtExpectedReturnDate.Enabled = false;
            txtVehicleType.Enabled = false;
            txtVehicleNumber.Enabled = false;
            txtDriverName.Enabled = false;
            txtNotes.Enabled = false;
            dgvNRJ.ReadOnly = true;

            btnSearchBBK.Enabled = false;
            btnSearchInventSite.Enabled = false;
            btnSearchVehicleOwner.Enabled = false;
            cbSame.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dgvNRJ.ReadOnly = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            dtExpectedReturnDate.Enabled = true;
            txtVehicleType.Enabled = true;
            txtVehicleNumber.Enabled = true;
            txtDriverName.Enabled = true;
            txtNotes.Enabled = true;
            dgvNRJ.ReadOnly = false;

            btnSearchBBK.Enabled = true;
            btnSearchInventSite.Enabled = true;
            btnSearchVehicleOwner.Enabled = true;
            cbSame.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            dgvNRJ.ReadOnly = false;
            dgvNRJ.Columns["No"].ReadOnly = true;
            dgvNRJ.Columns["FullItemID"].ReadOnly = true;
            dgvNRJ.Columns["ItemName"].ReadOnly = true;
            dgvNRJ.Columns["RemainingQty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Qty"].ReadOnly = false;
            dgvNRJ.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Ratio"].ReadOnly = true;
            dgvNRJ.Columns["ActionCode"].ReadOnly = false;
            dgvNRJ.Columns["Notes"].ReadOnly = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
                if (ControlMgr.GroupName.Contains("Manager"))
                {
                    if (gbApprove.Visible == true)
                    {
                        gbApprove.Visible = false;
                    }
                }
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
            if (ControlMgr.GroupName.Contains("Manager"))
            {
                if (gbApprove.Visible == false)
                {
                    gbApprove.Visible = true;
                }
            }
        }

        public void GetDataHeader()
        {
            if (NRJNumber != "")
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select Top 1 NRJDate, CustID, CustName, GoodsIssuedID, GoodsIssuedDate, InventSiteID, InventSiteName, VehicleOwnerID, SameAsCustID,  VehicleOwnerName, VehicleType, VehicleNumber, DriverName, ExpectedReturnDate, Notes, b.Deskripsi, ApprovedBy From [NotaReturJualH] a Left JOIN [TransStatusTable] b ON a.TransStatus = b.StatusCode And b.TransCode = 'NotaReturJual' Where NRJID = '" + NRJNumber + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtNRJ.Text = Dr["NRJDate"].ToString();
                    txtNRJNum.Text = NRJNumber;
                    dtBBK.Text = Dr["GoodsIssuedDate"].ToString();
                    txtBBKNum.Text = Dr["GoodsIssuedID"].ToString();
                    txtCustID.Text = Dr["CustID"].ToString();
                    txtCustName.Text = Dr["CustName"].ToString();
                    dtExpectedReturnDate.Text = Dr["ExpectedReturnDate"].ToString();
                    txtVehicleOwnerID.Text = Dr["VehicleOwnerID"].ToString();
                    txtVehicleOwnerName.Text = Dr["VehicleOwnerName"].ToString();
                    cbSame.Checked = (bool)Dr["SameAsCustID"];
                    txtVehicleType.Text = Dr["VehicleType"].ToString();
                    txtVehicleNumber.Text = Dr["VehicleNumber"].ToString();
                    txtDriverName.Text = Dr["DriverName"].ToString();
                    txtInventSiteID.Text = Dr["InventSiteID"].ToString();
                    txtInventSiteName.Text = Dr["InventSiteName"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString();
                    txtApproved.Text = Dr["ApprovedBy"].ToString();
                    txtNotes.Text = Dr["Notes"].ToString();
                }
                Dr.Close();

                dgvNRJ.Rows.Clear();
                CreateGrid();
                Query = "Select SeqNo, ItemId, [FullItemID], ItemName, GroupId, SubGroup1Id, SubGroup2Id, [UoM_Qty], [UoM_Unit], [Alt_Qty], [Alt_Unit], Ratio, ActionCode, [GoodsIssuedID], [GoodsIssued_SeqNo], [Notes] From [NotaReturJual_Dtl] Where NRJID = '" + NRJNumber + "' order by SeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                string action = "";
                int j = 0;
                while (Dr.Read())
                {
                    Query = "Select Remaining_Qty From [GoodsIssuedD] Where GoodsIssuedId = '" + txtBBKNum.Text + "' And FullItemID = '" + Dr["FullItemID"] + "'";
                    cmd = new SqlCommand(Query, Conn);

                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        action = "Retur Credit Note";
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        action = "Retur Tukar Barang";
                    }

                    this.dgvNRJ.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], action, Dr["GoodsIssuedID"], Dr["GoodsIssued_SeqNo"], Dr["Notes"], Dr["UoM_Qty"]);

                    DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                    combo2.Items.Add("Retur Credit Note");
                    combo2.Items.Add("Retur Tukar Barang");
                    dgvNRJ.Rows[j].Cells["ActionCode"] = combo2;

                    dgvNRJ.Rows[j].Cells["ActionCode"].Value = action;

                    j++;
                }
                Dr.Close();

                dgvNRJ.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["RemainingQty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["ActionCode"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRJ.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvNRJ.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRJ.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRJ.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvNRJ.Columns["ItemID"].Visible = false;
                dgvNRJ.Columns["GroupId"].Visible = false;
                dgvNRJ.Columns["SubGroup1ID"].Visible = false;
                dgvNRJ.Columns["SubGroup2ID"].Visible = false;
                dgvNRJ.Columns["GoodsIssued_SeqNo"].Visible = false;
                dgvNRJ.Columns["OldQty"].Visible = false;

                dgvNRJ.AutoResizeColumns();

                Conn.Close();
            }
        }

        private void btnSearchBBK_Click(object sender, EventArgs e)
        {
            AddBBK f = new AddBBK();
            f.setParent(this);
            f.Show();
        }

        public void AddBBK(String bbknum)
        {
            txtBBKNum.Text = bbknum;

            Conn = ConnectionString.GetConnection();
            Query = "Select Top 1 GoodsIssuedDate, GoodsIssuedId, AccountNum, AccountName From [GoodsIssuedH] Where GoodsIssuedId = '" + txtBBKNum.Text + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtBBK.Text = Dr["GoodsIssuedDate"].ToString();
                txtBBKNum.Text = Dr["GoodsIssuedId"].ToString();
                txtCustID.Text = Dr["AccountNum"].ToString();
                txtCustName.Text = Dr["AccountName"].ToString();
            }
            Dr.Close();

            dgvNRJ.Rows.Clear();
        }

        private void btnSearchVehicleOwner_Click(object sender, EventArgs e)
        {
            AddVehicle f = new AddVehicle();
            f.setParent(this);
            f.Show();
        }

        public void AddVehicle(String vehicleownerid, String vehicleownername)
        {
            cbSame.Checked = false;
            txtVehicleOwnerID.Text = vehicleownerid;
            txtVehicleOwnerName.Text = vehicleownername;
        }

        private void cbSame_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSame.Checked == true)
            {
                txtVehicleOwnerID.Text = txtCustID.Text;
                txtVehicleOwnerName.Text = txtCustName.Text;
            }
            else
            {
                txtVehicleOwnerID.Text = "";
                txtVehicleOwnerName.Text = "";
            }
        }

        private void btnSearchInventSite_Click(object sender, EventArgs e)
        {
            AddInventSite f = new AddInventSite();
            f.setParent(this);
            f.Show();
        }

        public void AddSite(String siteid, String sitename)
        {
            txtInventSiteID.Text = siteid;
            txtInventSiteName.Text = sitename;
        }

        public void AddDataGridDetail(List<string> SeqNo)
        {
            Conn = ConnectionString.GetConnection();
            
            CreateGrid();

            int rowcount = dgvNRJ.Rows.Count;
            for (int i = 0; i < SeqNo.Count; i++)
            {
                Query = "Select * From [GoodsIssuedD]a INNER JOIN [InventTable]b ON a.FullItemID = b.FullItemID Where a.[GoodsIssuedId] = '" + txtBBKNum.Text + "' AND a.GoodsIssuedSeqNo = '" + SeqNo[i] + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    Query = "Select Remaining_Qty From [GoodsIssuedD] Where GoodsIssuedId = '" + txtBBKNum.Text + "' And GoodsIssuedSeqNo = '" + Dr["GoodsIssuedSeqNo"] + "'";
                    cmd = new SqlCommand(Query, Conn);

                    this.dgvNRJ.Rows.Add(dgvNRJ.RowCount + 1, Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], cmd.ExecuteScalar(), Dr["Qty"], Dr["UoM"], Dr["TotalBerat"], Dr["UoMAlt"], Dr["Ratio"], "", txtBBKNum.Text, Dr["GoodsIssuedSeqNo"], Dr["Notes"]);

                    

                    j++;
                }
                Dr.Close();
            }

            for (int x = rowcount; x < dgvNRJ.Rows.Count; x++)
            {
                DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                combo2.Items.Add("Retur Credit Note");
                combo2.Items.Add("Retur Tukar Barang");
                dgvNRJ.Rows[x].Cells["ActionCode"] = combo2;
            }

            dgvNRJ.ReadOnly = false;
            //dgvNRJ.DefaultCellStyle.BackColor = Color.White;
            dgvNRJ.Columns["No"].ReadOnly = true;
            dgvNRJ.Columns["FullItemID"].ReadOnly = true;
            dgvNRJ.Columns["ItemName"].ReadOnly = true;
            dgvNRJ.Columns["RemainingQty"].ReadOnly = true;
            dgvNRJ.Columns["UoM_Qty"].ReadOnly = false;
            dgvNRJ.Columns["UoM_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Qty"].ReadOnly = true;
            dgvNRJ.Columns["Alt_Unit"].ReadOnly = true;
            dgvNRJ.Columns["Ratio"].ReadOnly = true;
            dgvNRJ.Columns["ActionCode"].ReadOnly = false;
            dgvNRJ.Columns["Notes"].ReadOnly = true;

            dgvNRJ.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["RemainingQty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["ActionCode"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNRJ.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvNRJ.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRJ.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRJ.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvNRJ.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvNRJ.Columns["ItemID"].Visible = false;
            dgvNRJ.Columns["GroupId"].Visible = false;
            dgvNRJ.Columns["SubGroup1ID"].Visible = false;
            dgvNRJ.Columns["SubGroup2ID"].Visible = false;
            dgvNRJ.Columns["GoodsIssued_SeqNo"].Visible = false;
            dgvNRJ.Columns["OldQty"].Visible = false;

            dgvNRJ.AutoResizeColumns();

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtBBKNum.Text == "")
            {
                MessageBox.Show("Pilih BBK terlebih dahulu.");
            }
            else
            {
                List<string> seqno = new List<string>();

                if (dgvNRJ.RowCount > 0)
                {
                    for (int i = 0; i < dgvNRJ.RowCount; i++)
                    {
                        seqno.Add(dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString());
                    }
                }

                AddItem add = new AddItem();
                add.setParent(this);
                add.setMode(txtBBKNum.Text, seqno);
                add.Show();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvNRJ.RowCount > 0)
            {
                dgvNRJ.Rows.RemoveAt(dgvNRJ.CurrentRow.Index);

                for (int i = 0; i < dgvNRJ.RowCount; i++)
                {
                    dgvNRJ.Rows[i].Cells["No"].Value = i + 1;
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvNRJ_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNRJ.RowCount > 0)
            {
                if (dgvNRJ.Columns[e.ColumnIndex].Name == "UoM_Qty")
                {
                    if (dgvNRJ.CurrentRow.Cells["UoM_Qty"].Value == null)
                    {
                        dgvNRJ.CurrentRow.Cells["Alt_Qty"].Value = "";
                    }
                    else
                    {
                        dgvNRJ.CurrentRow.Cells["Alt_Qty"].Value = decimal.Parse(dgvNRJ.CurrentRow.Cells["UoM_Qty"].Value.ToString()) * decimal.Parse(dgvNRJ.CurrentRow.Cells["Ratio"].Value.ToString());
                    }
                }
            }
        }

        private void dgvNRJ_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (dgvNRJ.CurrentCell.ColumnIndex == dgvNRJ.Columns["UoM_Qty"].Index) //Desired Column
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            decimal QtyNew = 0;
            string ActionCodeNumber;

            if (dgvNRJ.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }

            try
            {
                if (Mode == "New" && txtNRJNum.Text.Trim() == "")
                {
                    //Old Code======================================================================================= 
                    //Query = "Insert into [NotaReturJualH] (NRJID,NRJDate,CustID,CustName,GoodsIssuedID,GoodsIssuedDate,InventSiteID,InventSiteName,VehicleOwnerID,SameAsCustID, VehicleOwnerName, VehicleType, VehicleNumber, DriverName, ExpectedReturnDate, Notes,TransStatus,CreatedDate,CreatedBy) OUTPUT INSERTED.NRJID values ";
                    //Query += "((Select 'NRJ-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(NRJID) is null then '1' else substring(Max(NRJID),11,4)+1 end), 5) ";
                    //Query += "from [NotaReturJualH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    //Query += "'" + dtNRJ.Value.ToString("yyyy-MM-dd") + "',";
                    //Query += "'" + txtCustID.Text + "',";
                    //Query += "'" + txtCustName.Text + "',";
                    //Query += "'" + txtBBKNum.Text + "',";
                    //Query += "'" + dtBBK.Value.ToString("yyyy-MM-dd") + "',";
                    //Query += "'" + txtInventSiteID.Text + "',";
                    //Query += "'" + txtInventSiteName.Text + "',";
                    //Query += "'" + txtVehicleOwnerID.Text + "',";
                    //Query += "'" + cbSame.Checked + "',";
                    //Query += "'" + txtVehicleOwnerName.Text + "',";
                    //Query += "'" + txtVehicleType.Text + "',";
                    //Query += "'" + txtVehicleNumber.Text + "',";
                    //Query += "'" + txtDriverName.Text + "',";
                    //Query += "'" + dtExpectedReturnDate.Value.ToString("yyyy-MM-dd") + "',";
                    //Query += "'" + txtNotes.Text + "',";
                    //Query += "'01',";
                    //Query += "getdate(),'" + ControlMgr.UserId + "');";
                    //Cmd = new SqlCommand(Query, Conn, Trans);

                    //NRJNumber = Cmd.ExecuteScalar().ToString();
                    //End Old Code=====================================================================================

                    //begin============================================================================================
                    //updated by : joshua
                    //updated date : 14 Feb 2018
                    //description : change generate sequence number, get from global function and update counter 
                    string Jenis = "NRJ", Kode = "NRJ";
                    NRJNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "Insert into [NotaReturJualH] (NRJID,NRJDate,CustID,CustName,GoodsIssuedID,GoodsIssuedDate,InventSiteID,InventSiteName,VehicleOwnerID,SameAsCustID, VehicleOwnerName, VehicleType, VehicleNumber, DriverName, ExpectedReturnDate, Notes,TransStatus,CreatedDate,CreatedBy) values ";
                    Query += "('" + NRJNumber  + "',";
                    Query += "'" + dtNRJ.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "'" + txtCustID.Text + "',";
                    Query += "'" + txtCustName.Text + "',";
                    Query += "'" + txtBBKNum.Text + "',";
                    Query += "'" + dtBBK.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "'" + txtInventSiteID.Text + "',";
                    Query += "'" + txtInventSiteName.Text + "',";
                    Query += "'" + txtVehicleOwnerID.Text + "',";
                    Query += "'" + cbSame.Checked + "',";
                    Query += "'" + txtVehicleOwnerName.Text + "',";
                    Query += "'" + txtVehicleType.Text + "',";
                    Query += "'" + txtVehicleNumber.Text + "',";
                    Query += "'" + txtDriverName.Text + "',";
                    Query += "'" + dtExpectedReturnDate.Value.ToString("yyyy-MM-dd") + "',";
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
                    
                    for (int i = 0; i <= dgvNRJ.RowCount - 1; i++)
                    {
                        if (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == null)
                        {
                            MessageBox.Show("Terdapat quantity yang belum diisi.");
                            return;
                        }
                        if (decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) < decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Terdapat quantity yang melebihi batas.");
                            return;
                        }
                        if (dgvNRJ.Rows[i].Cells["ActionCode"].Value == "" || dgvNRJ.Rows[i].Cells["ActionCode"].Value == null)
                        {
                            MessageBox.Show("Action Code harus dipilih.");
                            return;
                        }
                        else if (dgvNRJ.Rows[i].Cells["ActionCode"].Value.ToString() == "Retur Credit Note")
                        {
                            ActionCodeNumber = "01";
                        }
                        else
                        {
                            ActionCodeNumber = "02";
                        }

                        Query += "Insert [NotaReturJual_Dtl] (NRJID,SeqNo,ItemId,FullItemId,ItemName,GroupId,SubGroup1Id,SubGroup2Id,UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,ActionCode,GoodsIssuedID,GoodsIssued_SeqNo,Notes,CreatedDate,CreatedBy) Values ";
                        Query += "('" + NRJNumber + "','";

                        Query += (dgvNRJ.Rows[i].Cells["No"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["No"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Qty"].Value == null ? "0" : dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Ratio"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["Ratio"].Value.ToString()) + "','";
                        Query += ActionCodeNumber + "','";
                        Query += txtBBKNum.Text + "','";
                        Query += (dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Notes"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";

                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) - decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString());

                        Query = "Update [GoodsIssuedD] set ";
                        Query += "Remaining_Qty='" + QtyNew + "' ";
                        Query += "where GoodsIssuedId='" + txtBBKNum.Text.Trim() + "' And GoodsIssuedSeqNo='" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "'";
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
                    MessageBox.Show("Data NRJ Number : " + NRJNumber + " berhasil ditambahkan.");
                    txtNRJNum.Text = NRJNumber;
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
                else
                {
                    DateTime CreatedDate = DateTime.Now;
                    String CreatedBy = "";

                    Query = "Update [NotaReturJualH] set ";
                    Query += "CustID='" + txtCustID.Text + "',";
                    Query += "CustName='" + txtCustName.Text + "',";
                    Query += "GoodsIssuedID='" + txtBBKNum.Text + "',";
                    Query += "GoodsIssuedDate='" + dtBBK.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "InventSiteID='" + txtInventSiteID.Text + "',";
                    Query += "InventSiteName='" + txtInventSiteName.Text + "',";
                    Query += "VehicleOwnerID='" + txtVehicleOwnerID.Text + "',";
                    Query += "SameAsCustID='" + cbSame.Checked + "',";
                    Query += "VehicleOwnerName='" + txtVehicleOwnerName.Text + "',";
                    Query += "VehicleType='" + txtVehicleType.Text + "',";
                    Query += "VehicleNumber='" + txtVehicleNumber.Text + "',";
                    Query += "DriverName='" + txtDriverName.Text + "',";
                    Query += "ExpectedReturnDate='" + dtExpectedReturnDate.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "Notes='" + txtNotes.Text + "',";
                    Query += "TransStatus = '01',";
                    Query += "ApprovedBy = '',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where NRJID='" + txtNRJNum.Text.Trim() + "'";
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

                    Query = "Delete from [NotaReturJual_Dtl] where NRJID='" + txtNRJNum.Text.Trim() + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    Query = "";

                    for (int i = 0; i <= dgvNRJ.RowCount - 1; i++)
                    {
                        if (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == null)
                        {
                            MessageBox.Show("Terdapat quantity yang belum diisi.");
                            return;
                        }
                        if ((decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) + decimal.Parse(dgvNRJ.Rows[i].Cells["OldQty"].Value.ToString())) < decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()))
                        {
                            MessageBox.Show("Terdapat quantity yang melebihi batas.");
                            return;
                        }

                        if (dgvNRJ.Rows[i].Cells["ActionCode"].Value == "")
                        {
                            MessageBox.Show("Action Code harus dipilih.");
                            return;
                        }
                        else if (dgvNRJ.Rows[i].Cells["ActionCode"].Value.ToString() == "Retur Credit Note")
                        {
                            ActionCodeNumber = "01";
                        }
                        else
                        {
                            ActionCodeNumber = "02";
                        }

                        Query += "Insert [NotaReturJual_Dtl] (NRJID,SeqNo,ItemId,FullItemId,ItemName,GroupId,SubGroup1Id,SubGroup2Id,UoM_Qty,UoM_Unit,Alt_Qty,Alt_Unit,Ratio,ActionCode,GoodsIssuedID,GoodsIssued_SeqNo,Notes,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) Values ";
                        Query += "('" + NRJNumber + "','";

                        Query += (dgvNRJ.Rows[i].Cells["No"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["No"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["ItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup1ID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["SubGroup2ID"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Qty"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["UoM_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["UoM_Unit"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Qty"].Value == null ? "0" : dgvNRJ.Rows[i].Cells["Alt_Qty"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Alt_Unit"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Alt_Unit"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Ratio"].Value == "" ? "0" : dgvNRJ.Rows[i].Cells["Ratio"].Value.ToString()) + "','";
                        Query += ActionCodeNumber + "','";
                        Query += txtBBKNum.Text + "','";
                        Query += (dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString()) + "','";
                        Query += (dgvNRJ.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNRJ.Rows[i].Cells["Notes"].Value.ToString()) + "',";
                        Query += "'" + CreatedDate + "',";
                        Query += "'" + CreatedBy + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";

                        //Update Remaining Qty
                        QtyNew = decimal.Parse(dgvNRJ.Rows[i].Cells["RemainingQty"].Value.ToString()) + decimal.Parse(dgvNRJ.Rows[i].Cells["OldQty"].Value.ToString()) - decimal.Parse(dgvNRJ.Rows[i].Cells["UoM_Qty"].Value.ToString());

                        Query = "Update [GoodsIssuedD] set ";
                        Query += "Remaining_Qty='" + QtyNew + "' ";
                        Query += "where GoodsIssuedId='" + txtBBKNum.Text.Trim() + "' And GoodsIssuedSeqNo='" + dgvNRJ.Rows[i].Cells["GoodsIssued_SeqNo"].Value.ToString() + "'";
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
                    MessageBox.Show("Data NRJ Number : " + txtNRJNum.Text + " berhasil diupdate.");
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

        private void btnApprove_Click(object sender, EventArgs e)
        {
            
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (ControlMgr.GroupName == "Sales Manager")
                {
                    Query = "Update [NotaReturJualH] set ";
                    Query += "TransStatus = '03',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where NRJId='" + txtNRJNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }
                else if (ControlMgr.GroupName == "Stock Manager")
                {
                    Query = "Update [NotaReturJualH] set ";
                    Query += "TransStatus = '05',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where NRJId='" + txtNRJNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }

                

                Trans.Commit();
                MessageBox.Show("Data NRJ Number : " + txtNRJNum.Text + " berhasil diupdate.");
                this.Close();
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

        private void btnReject_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (ControlMgr.GroupName == "Sales Manager")
                {
                    Query = "Update [NotaReturJualH] set ";
                    Query += "TransStatus = '02',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where NRJId='" + txtNRJNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }
                else if (ControlMgr.GroupName == "Stock Manager")
                {
                    Query = "Update [NotaReturJualH] set ";
                    Query += "TransStatus = '06',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where NRJId='" + txtNRJNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }

                //Update Qty
                List<string> GoodsIssued_SeqNo = new List<string>();
                List<string> GoodsIssuedID = new List<string>();
                List<decimal> Qty = new List<decimal>();
                decimal RemainingQty, QtyNew = 0;
                Query = "Select GoodsIssued_SeqNo,UoM_Qty,[GoodsIssuedID] From [NotaReturJual_Dtl] Where NRJID='" + txtNRJNum.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    GoodsIssued_SeqNo.Add(Dr["GoodsIssued_SeqNo"].ToString());
                    GoodsIssuedID.Add(Dr["GoodsIssuedID"].ToString());
                    Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                }
                Dr.Close();

                for (int i = 0; i < GoodsIssued_SeqNo.Count; i++)
                {
                    Query = "Select Remaining_Qty From [GoodsIssuedD] Where [GoodsIssuedId] = '" + GoodsIssuedID[i] + "' AND GoodsIssuedSeqNo = '" + GoodsIssued_SeqNo[i] + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                    QtyNew = RemainingQty + Qty[i];

                    Query = "Update [GoodsIssuedD] set ";
                    Query += "Remaining_Qty='" + QtyNew + "' ";
                    Query += "where GoodsIssuedId='" + GoodsIssuedID[i] + "' And GoodsIssuedSeqNo='" + GoodsIssued_SeqNo[i] + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    Query = "";
                }


                Trans.Commit();
                MessageBox.Show("Data NRJ Number : " + txtNRJNum.Text + " berhasil diupdate.");
                this.Close();
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
    }
}
