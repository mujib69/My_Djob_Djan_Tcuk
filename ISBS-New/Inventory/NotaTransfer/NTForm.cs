using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using ISBS_New.Sales.DeliveryOrder;

namespace ISBS_New.Inventory.NotaTransfer
{
    public partial class NTForm : MetroFramework.Forms.MetroForm
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
        List<NTForm> ListNTForm = new List<NTForm>();
        public string TransferNo = "";

        NT_Inquiry Parent = new NT_Inquiry();
        DOHeader Parent1 = new DOHeader();

        //NT Detail Before Edit Value
        int[] TransNo;
        string[] fullitemid;
        decimal[] TransQty;
        decimal[] TransQtyRes;
        string[] deletedunit;
        string[] Notes;

        //NT-SO Detail Before Edit Value
        string[] TransferNo2;
        int[] Transfer_SeqNo;
        int[] SeqNo;
        string[] SOId;
        int[] SO_SeqNo;
        decimal[] SO_Qty;
        decimal[] Reserved_Qty;
        decimal[] NT_Qty;
        string[] fullitemidSO;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public NTForm()
        {
            InitializeComponent();
            metroTabControl1.SelectedTab = metroTabPage1;
        }

        public NTForm(string reftype, string inventid, string warehouse, bool search)
        {
            InitializeComponent();
            metroTabControl1.SelectedTab = metroTabPage1;
            txtBoxRefType.Text = reftype;
            txtInventSiteIDTo.Text = inventid;
            txtWarehouseTo.Text = warehouse;
            btnSearchTo.Enabled = search;
        }

        public void SetParent(NT_Inquiry F)
        {
            Parent = F;
        }

        public void SetParent(DOHeader F)
        {
            Parent1 = F;
        }

        public void ModeNew()
        {
            Mode = "New";

            txtTransferNo.Enabled = false;

            grpHeader.Enabled = true;
            txtInventSiteIDFrom.Enabled = false;
            txtWarehouseFrom.Enabled = false;
            txtInventSiteIDTo.Enabled = false;
            txtWarehouseTo.Enabled = false;

            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnSave.Enabled = true;
            btnExit.Enabled = true;
            btnApprove.Enabled = false;
            btnUnapprove.Enabled = false;
            btnDelete.Enabled = true;
            btnAdd.Enabled = true;
            AllBlank();
        }

        public void ModeView()
        {
            Mode = "View";

            grpHeader.Enabled = false;

            dgvNotaTransfer.ReadOnly = true;

            btnEdit.Enabled = true;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnExit.Enabled = true;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;
            btnApprove.Enabled = false;
            btnUnapprove.Enabled = false;
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                string TransNo = (TransferNo == null) || (TransferNo.ToString() == "") ? txtTransferNo.Text : TransferNo;
                Query = "SELECT [TransStatus] FROM [dbo].[NotaTransferH] WHERE [TransferNo] = '" + TransNo + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    if (Cmd.ExecuteScalar() != System.DBNull.Value)
                    {
                        if (Cmd.ExecuteScalar().ToString() == "02")
                        {
                            btnUnapprove.Enabled = true;
                        }
                        else if (Cmd.ExecuteScalar().ToString() == "01")
                        {
                            btnApprove.Enabled = true;
                        }
                    }
                }
            }
            GetDataHeader();
        }

        public void ModeApprove()
        {
            Mode = "Approve";

            grpHeader.Enabled = false;

            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnExit.Enabled = true;

            btnApprove.Enabled = false;
            btnUnapprove.Enabled = false;
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                string TransNo = (TransferNo == null) || (TransferNo.ToString() == "") ? txtTransferNo.Text : TransferNo;
                Query = "SELECT [TransStatus] FROM [dbo].[NotaTransferH] WHERE [TransferNo] = '" + TransNo + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    if (Cmd.ExecuteScalar() != System.DBNull.Value)
                    {
                        if (Cmd.ExecuteScalar().ToString() == "02")
                        {
                            btnUnapprove.Enabled = true;
                        }
                        else if (Cmd.ExecuteScalar().ToString() == "01")
                        {
                            btnApprove.Enabled = true;
                        }
                    }
                }
            }

            GetDataHeader();
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtTransferNo.Enabled = false;

            grpHeader.Enabled = true;
            txtInventSiteIDFrom.Enabled = false;
            txtWarehouseFrom.Enabled = false;
            txtInventSiteIDTo.Enabled = false;
            txtWarehouseTo.Enabled = false;

            btnEdit.Enabled = false;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
            btnApprove.Enabled = false;
            btnExit.Enabled = false;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            btnUnapprove.Enabled = false;

            dgvNotaTransfer.ReadOnly = false;
            string[] read = new string[] { "No", "FullItemId", "ItemName", "Available", "Reserved", "Unit", "Transfer Qty Reserved" };
            for (int i = 0; i < read.Length; i++)
            {
                dgvNotaTransfer.Columns[read[i]].ReadOnly = true;
            }

            dgvDetailSO.ReadOnly = false;
            string[] read2 = new string[] { "No", "TransferNo", "Transfer_SeqNo", "FullItemId", "ItemName", "SO Id", "SO Seq No", "SO Qty", "Unit", "Remaining Reserved Qty" };
            for (int i = 0; i < read2.Length; i++)
            {
                dgvDetailSO.Columns[read2[i]].ReadOnly = true;
            }
            
        }

        private void AllBlank()
        {
            txtTransferNo.Text = "";
            txtInventSiteIDFrom.Text = "";
            txtWarehouseFrom.Text = "";
            txtInventSiteIDTo.Text = "";
            txtWarehouseTo.Text = "";
            txtVehicleType.Text = "";
            txtVehicleNumber.Text = "";
            txtDriverName.Text = "";
            dtTransferDate.Value = DateTime.Now; //Convert.ToDateTime("01-01-1900"); hendry
            
        }

        private void GetDataHeader()
        {
            if (TransferNo == "")
            {
                TransferNo = txtTransferNo.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            //Input Header
            Query = "Select a.[ReferenceType],a.[TransferNo],a.[TransferDate],a.[InventSiteFrom],a.[InventSiteFromName],a.InventSiteTo,a.InventSiteToName,a.[VehicleOwner],a.VehicleType,a.VehicleNo,a.DriverName,a.TransStatus,a.Notes,b.Deskripsi,c.[VendName] From [NotaTransferH] a left join TransStatustable b on a.TransStatus=b.StatusCode and b.TransCode='TransferNote' ";
            Query += " LEFT JOIN [dbo].[VendTable] c ON a.VehicleOwner=c.[VendId] ";
            Query += "Where TransferNo = '" + TransferNo + "';";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtBoxRefType.Text = Dr["ReferenceType"].ToString();
                txtTransferNo.Text = Dr["TransferNo"].ToString();
                txtInventSiteIDFrom.Text = Dr["InventSiteFrom"].ToString();
                txtWarehouseFrom.Text = Dr["InventSiteFromName"].ToString();
                txtInventSiteIDTo.Text = Dr["InventSiteTo"].ToString();
                txtWarehouseTo.Text = Dr["InventSiteToName"].ToString();
                txtBoxVehicleOwnerId.Text = Dr["VehicleOwner"].ToString();
                txtBoxVehicleOwnerName.Text = Dr["VendName"].ToString();
                txtVehicleType.Text = Dr["VehicleType"].ToString();
                txtVehicleNumber.Text = Dr["VehicleNo"].ToString();
                txtDriverName.Text = Dr["DriverName"].ToString();
                txtTransStatus.Text = Dr["TransStatus"].ToString();
                txtTransStatusDesc.Text = Dr["Deskripsi"].ToString();
            }
            Dr.Close();

            //INPUT DETAIL ITEM
            dgvNotaTransfer.Rows.Clear();
            if (dgvNotaTransfer.RowCount - 1 <= 0)
            {
                dgvNotaTransfer.ColumnCount = 14;
                dgvNotaTransfer.Columns[0].Name = "No";
                dgvNotaTransfer.Columns[1].Name = "SeqNo";
                dgvNotaTransfer.Columns[2].Name = "GroupId";
                dgvNotaTransfer.Columns[3].Name = "SubGroupId";
                dgvNotaTransfer.Columns[4].Name = "SubGroup2Id";
                dgvNotaTransfer.Columns[5].Name = "ItemId";
                dgvNotaTransfer.Columns[6].Name = "FullItemId";
                dgvNotaTransfer.Columns[7].Name = "ItemName";
                dgvNotaTransfer.Columns[8].Name = "Available";
                dgvNotaTransfer.Columns[9].Name = "Transfer Qty";
                dgvNotaTransfer.Columns[10].Name = "Reserved";
                dgvNotaTransfer.Columns[11].Name = "Transfer Qty Reserved";
                dgvNotaTransfer.Columns[12].Name = "Unit";
                dgvNotaTransfer.Columns[13].Name = "Notes";
            }

            //INPUT SO RESERVE
            dgvDetailSO.Rows.Clear();
            if (dgvDetailSO.Rows.Count <= 0)
            {
                dgvDetailSO.ColumnCount = 13;
                dgvDetailSO.Columns[0].Name = "No";
                dgvDetailSO.Columns[1].Name = "TransferNo";
                dgvDetailSO.Columns[2].Name = "Transfer_SeqNo";
                dgvDetailSO.Columns[3].Name = "SeqNo";
                dgvDetailSO.Columns[4].Name = "FullItemId";
                dgvDetailSO.Columns[5].Name = "ItemName";
                dgvDetailSO.Columns[6].Name = "SO Id";
                dgvDetailSO.Columns[7].Name = "SO Seq No";
                dgvDetailSO.Columns[8].Name = "SO Qty";
                dgvDetailSO.Columns[9].Name = "SO Reserved";
                dgvDetailSO.Columns[10].Name = "Remaining Reserved Qty";
                dgvDetailSO.Columns[11].Name = "NT Reserved";
                dgvDetailSO.Columns[12].Name = "Unit";

            }

            Query = "SELECT * FROM [dbo].[NotaTransfer_SO_List] WHERE [TransferNo] = '" + txtTransferNo.Text + "';";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    this.dgvDetailSO.Rows.Add((dgvDetailSO.Rows.Count + 1), Dr["TransferNo"], Dr["Transfer_SeqNo"], Dr["SeqNo"], Dr["FullItemId"], Dr["ItemName"], Dr["SOId"], Dr["SO_SeqNo"], Dr["SO_Qty"], Dr["Reserved_Qty"], 0.00, Dr["NT_Qty"], Dr["Unit"]);
                }
                Dr.Close();
            }

            TransferNo2 = new string[dgvDetailSO.Rows.Count];
            Transfer_SeqNo = new int[dgvDetailSO.Rows.Count];
            SeqNo = new int[dgvDetailSO.Rows.Count];
            SOId = new string[dgvDetailSO.Rows.Count];
            SO_SeqNo = new int[dgvDetailSO.Rows.Count];
            SO_Qty = new decimal[dgvDetailSO.Rows.Count];
            Reserved_Qty = new decimal[dgvDetailSO.Rows.Count];
            NT_Qty = new decimal[dgvDetailSO.Rows.Count];
            fullitemidSO = new string[dgvDetailSO.Rows.Count];

            for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
            {
                TransferNo2[i] = dgvDetailSO.Rows[i].Cells["TransferNo"].Value.ToString();
                Transfer_SeqNo[i] = Convert.ToInt32(dgvDetailSO.Rows[i].Cells["Transfer_SeqNo"].Value);
                SeqNo[i] = Convert.ToInt32(dgvDetailSO.Rows[i].Cells["SeqNo"].Value);
                SOId[i] = dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString();
                SO_SeqNo[i] = Convert.ToInt32(dgvDetailSO.Rows[i].Cells["SO Seq No"].Value);
                SO_Qty[i] = Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["SO Qty"].Value);
                Reserved_Qty[i] = Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["SO Reserved"].Value);
                NT_Qty[i] = Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["NT Reserved"].Value);
                fullitemidSO[i] = dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString();
            }

            Query = "Select SeqNo,[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],FullItemID,ItemName,[NT_Available_Qty],[NT_Reserved_Qty],UoM,Notes from NotaTransferD Where TransferNo = '" + txtTransferNo.Text + "' order by SeqNo asc";
            int No = 0;
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                this.dgvNotaTransfer.Rows.Add(Dr[0], Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], "Available", Dr[7], "Reserved", Dr[8], Dr[9], Dr[10]);
            }
            Dr.Close();

            TransNo = new int[dgvNotaTransfer.Rows.Count];
            TransQty = new decimal[dgvNotaTransfer.Rows.Count];
            TransQtyRes = new decimal[dgvNotaTransfer.Rows.Count];
            fullitemid = new string[dgvNotaTransfer.Rows.Count];
            deletedunit = new string[dgvNotaTransfer.Rows.Count];
            Notes = new string[dgvNotaTransfer.Rows.Count];

            for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
            {
                decimal qtyAvailable = 0;
                decimal qtyReserved = 0;
                Query = "SELECT a.Available_For_Sale_UoM,a.Available_For_Sale_Reserved_UoM FROM [dbo].[Invent_OnHand_Qty] a WHERE a.[FullItemId]='" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND a.[InventSiteId] ='" + txtInventSiteIDFrom.Text + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        qtyAvailable = Convert.ToDecimal(Dr["Available_For_Sale_UoM"]);
                        dgvNotaTransfer.Rows[i].Cells["Available"].Value = qtyAvailable;
                        qtyReserved = Convert.ToDecimal(Dr["Available_For_Sale_Reserved_UoM"]);
                        dgvNotaTransfer.Rows[i].Cells["Reserved"].Value = qtyReserved;
                    }
                    Dr.Close();
                }

                TransNo[i] = Convert.ToInt32(dgvNotaTransfer.Rows[i].Cells["SeqNo"].Value);
                TransQty[i] = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
                TransQtyRes[i] = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);
                fullitemid[i] = dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString();
                deletedunit[i] = dgvNotaTransfer.Rows[i].Cells["Unit"].Value.ToString();
                Notes[i] = dgvNotaTransfer.Rows[i].Cells["Notes"].Value.ToString();
            }

            string[] visible = new string[] { "SeqNo", "GroupId", "SubGroupId", "SubGroup2Id", "ItemId" };
            for (int i = 0; i < visible.Length; i++)
            {
                dgvNotaTransfer.Columns[visible[i]].Visible = false;
            }

            string[] color = new string[] { "No", "FullItemId", "ItemName", "Available", "Reserved", "Unit", "Transfer Qty Reserved" };
            for (int i = 0; i < color.Length; i++)
            {
                dgvNotaTransfer.Columns[color[i]].DefaultCellStyle.BackColor = Color.LightGray;
            }

            dgvNotaTransfer.AutoResizeColumns();
            dgvNotaTransfer.Refresh();
            foreach (DataGridViewColumn column1 in dgvNotaTransfer.Columns)
            {
                column1.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dgvDetailSO.AutoResizeColumns();
            dgvDetailSO.Refresh();
            foreach (DataGridViewColumn column2 in dgvDetailSO.Columns)
            {
                column2.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            string[] visible2 = new string[] { "SeqNo", "Transfer_SeqNo" };
            for (int i = 0; i < visible2.Length; i++)
            {
                dgvDetailSO.Columns[visible2[i]].Visible = false;
            }

            string[] color2 = new string[] { "No", "TransferNo", "Transfer_SeqNo", "FullItemId", "ItemName", "SO Id", "SO Seq No", "SO Reserved", "SO Qty", "Unit", "Remaining Reserved Qty" };
            for (int i = 0; i < color2.Length; i++)
            {
                dgvDetailSO.Columns[color2[i]].DefaultCellStyle.BackColor = Color.LightGray;
            }

            dgvNotaTransfer.Columns["Available"].DefaultCellStyle.Format = "0.00";
            dgvNotaTransfer.Columns["Transfer Qty"].DefaultCellStyle.Format = "0.00";
            dgvNotaTransfer.Columns["Reserved"].DefaultCellStyle.Format = "0.00";
            dgvNotaTransfer.Columns["Transfer Qty Reserved"].DefaultCellStyle.Format = "0.00";

            dgvDetailSO.Columns["SO Qty"].DefaultCellStyle.Format = "0.00";
            dgvDetailSO.Columns["SO Reserved"].DefaultCellStyle.Format = "0.00";
            dgvDetailSO.Columns["Remaining Reserved Qty"].DefaultCellStyle.Format = "0.00";
            dgvDetailSO.Columns["NT Reserved"].DefaultCellStyle.Format = "0.00";
            

        }

        private void NTForm_Load(object sender, EventArgs e)
        {

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtTransStatus.Text == "01")
            {
                if (txtBoxRefType.Text.ToUpper() != "SALES ORDER")
                {
                    MessageBox.Show("Hanya Reference Type jenis SALES ORDER yang dapat diedit.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Hanya TransStatus 01 yang dapat diedit");
                return;
            }
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end              
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;

            //Validasi Items
            if (validate() == false)
            {
                return;
            }
            try
            {
                if (txtTransferNo.Text.Trim() == "")
                {
                    //Old Code=======================================================================================  
                    //Query = "Insert into NotaTransferH (TransferNo,TransferDate,InventSiteFrom,InventSiteFromName,InventSiteTo,InventSiteToName,Referensi,VehicleType,VehicleNo,DriverName,TransStatus,Notes,CreatedDate,CreatedBy) OUTPUT INSERTED.TransferNo values ";
                    //Query += "((Select 'NT-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max([TransferNo]) is null then '1' else substring(Max([TransferNo]),11,4)+1 end), 5) ";
                    //Query += "from [NotaTransferH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    //Query += "'" + dtTransferDate.Value.ToString("yyyy-MM-dd") + "',";
                    //Query += "'" + txtInventSiteIDFrom.Text+ "',";
                    //Query += "'" + txtWarehouseFrom.Text + "',";
                    //Query += "'" + txtInventSiteIDTo.Text + "',";
                    //Query += "'" + txtWarehouseTo.Text + "',";
                    //Query += "'" + txtReferensi.Text + "',";
                    //Query += "'" + txtVehicleType.Text + "',";
                    //Query += "'" + txtVehicleNumber.Text + "',";
                    //Query += "'" + txtDriverName.Text + "',";
                    //Query += "'01',";
                    //Query += "'" + txtNote.Text + "',";
                    //Query += "getdate(),'" + ControlMgr.UserId + "');";
                    //Cmd = new SqlCommand(Query, Conn, Trans);

                    //string TransferNumber = Cmd.ExecuteScalar().ToString();
                    //End Old Code=====================================================================================

                    //begin============================================================================================
                    //updated by : joshua
                    //updated date : 14 Feb 2018
                    //description : change generate sequence number, get from global function and update counter 
                    string Jenis = "NT", Kode = "NT";
                    string TransferNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "Insert into NotaTransferH (ReferenceType,ReferenceId,TransferNo,TransferDate,InventSiteFrom,InventSiteFromName,InventSiteTo,InventSiteToName,VehicleType,VehicleNo,DriverName,TransStatus,CreatedDate,CreatedBy,VehicleOwner) values ";
                    //BY: HC (S)
                    if (tbxRefID.Text != String.Empty && tbxRefID.Text.Split('/')[0] == "DO")
                        Query += "(@ReferenceType,";
                    //BY: HC (E)
                    else
                        Query += "('SALES ORDER',";
                    Query += "@ReferenceId,"; //BY: HC
                    Query += "@TransferNo,";
                    Query += "@TransferDate,";
                    Query += "@InventSiteFrom,";
                    Query += "@InventSiteFromName,";
                    Query += "@InventSiteTo,";
                    Query += "@InventSiteToName,";
                    Query += "@VehicleType,";
                    Query += "@VehicleNo,";
                    Query += "@DriverName,";
                    Query += "'01',";
                    Query += "getdate(),@CreatedBy, ";
                    Query += " @VehicleOwner) ";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@ReferenceType", txtBoxRefType.Text);
                        Cmd.Parameters.AddWithValue("@ReferenceId", tbxRefID.Text);
                        Cmd.Parameters.AddWithValue("@TransferNo", TransferNumber);
                        Cmd.Parameters.AddWithValue("@TransferDate", dtTransferDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.AddWithValue("@InventSiteFrom", txtInventSiteIDFrom.Text);
                        Cmd.Parameters.AddWithValue("@InventSiteFromName", txtWarehouseFrom.Text);
                        Cmd.Parameters.AddWithValue("@InventSiteTo", txtInventSiteIDTo.Text);
                        Cmd.Parameters.AddWithValue("@InventSiteToName", txtWarehouseTo.Text);
                        Cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text.Trim());
                        Cmd.Parameters.AddWithValue("@VehicleNo", txtVehicleNumber.Text.Trim());
                        Cmd.Parameters.AddWithValue("@DriverName", txtDriverName.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@VehicleOwner", txtBoxVehicleOwnerId.Text.Trim());
                        Cmd.ExecuteNonQuery();
                    }

                    //update counter
                    //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                    //end update counter
                    //end=============================================================================================

                    for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
                    {
                        insertTableDtl(TransferNumber, Trans, i);
                        updateSeqNo(Trans, i);
                        //insertNotaLog(TransferNumber, "01", Trans, "Created", i);
                        insertInventTrans(Trans, "-", dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString(), TransferNumber, i, i, "Created");
                    }
                    for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
                    {
                        insertTableSONT(TransferNumber, Trans, i);
                        insertLockTable(TransferNumber, Mode, Trans, i);
                    }
                    insertInventMovement(Trans);
                    insertInventOnHand(Trans, TransferNo);
                    txtTransferNo.Text = TransferNumber;

                    InsertNTLog(TransferNumber, "01", "Waiting for Approval", Trans, "N");

                    Trans.Commit();
                    MessageBox.Show("Data TransferNumber : " + TransferNumber + " berhasil ditambahkan.");

                }
                else
                {
                    Query = "Update NotaTransferH set ";
                    Query += "TransferDate=@TransferDate,";
                    Query += "[InventSiteTo]=@InventSiteTo,";
                    Query += "[InventSiteToName]=@InventSiteToName,";
                    Query += "[VehicleOwner]=@VehicleOwner,";
                    Query += "[VehicleType]=@VehicleType,";
                    Query += "[VehicleNo]=@VehicleNo,";
                    Query += "[DriverName]=@DriverName,";
                    //Query += "[TransStatus]='',";
                    //Query += "[Notes]='" + txtNote.Text + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where TransferNo=@TransferNo";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@TransferDate", dtTransferDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.AddWithValue("@InventSiteTo", txtInventSiteIDTo.Text);
                        Cmd.Parameters.AddWithValue("@InventSiteToName", txtWarehouseTo.Text);
                        Cmd.Parameters.AddWithValue("@VehicleOwner", txtBoxVehicleOwnerId.Text.Trim());
                        Cmd.Parameters.AddWithValue("@VehicleType", txtVehicleType.Text.Trim());
                        Cmd.Parameters.AddWithValue("@VehicleNo", txtVehicleNumber.Text.Trim());
                        Cmd.Parameters.AddWithValue("@DriverName", txtDriverName.Text.Trim());
                        Cmd.Parameters.AddWithValue("@TransferNo", txtTransferNo.Text.Trim());
                        Cmd.ExecuteNonQuery();
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                            CreatedBy = Dr["CreatedBy"].ToString();
                        }
                        Dr.Close();
                    }
                    checkEditDeleteSOList(Trans);
                    checkEditDelete(Trans);

                    insertInventMovement(Trans);
                    insertInventOnHand(Trans, TransferNo);

                    InsertNTLog(TransferNo, "01", "Waiting for Approval", Trans, "E");

                    Trans.Commit();
                    MessageBox.Show("Data TransferNumber : " + txtTransferNo.Text + " berhasil diupdate.");
                    for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
                    {
                        Query = "SELECT SUM(Lock_Qty) FROM [dbo].[InventLockTable] ";
                        Query += " WHERE  SiteId =@SiteId AND FullItemId = @FullItemId AND [RefTransId] = @RefTransId AND [RefTrans_SeqNo] = @RefTrans_SeqNo ";
                        Query += " GROUP BY  [SiteId],[FullItemId],[RefTransId],[Unit],[RefTransType],[RefTrans_SeqNo] ";
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            Cmd.Parameters.AddWithValue("@SiteId",txtInventSiteIDFrom.Text);
                            Cmd.Parameters.AddWithValue("@FullItemId",dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() );
                            Cmd.Parameters.AddWithValue("@RefTransId",dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@RefTrans_SeqNo", dgvDetailSO.Rows[i].Cells["SO Seq No"].Value);
                            dgvDetailSO.Rows[i].Cells["Remaining Reserved Qty"].Value = (Cmd.ExecuteScalar());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                MessageBox.Show(ex.Message);
                Conn.Close();
                return;
            }
            finally
            {
                Conn.Close();
                Parent.RefreshGrid();
                ModeView();
                GetDataHeader();
                //this.Close();
            }
        }

        private void CreateJournal(SqlTransaction Trans)
        {
            decimal AdjustInProgress = 0, Available = 0;
            decimal Price = 0;
            for (int i = 0; i < dgvNotaTransfer.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value);
                string QtyUoM = Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value) == "" ? "0" : Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
                string QtyUoMReserved = Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value) == "" ? "0" : Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);
                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + FullItemID + "';";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Price = Cmd.ExecuteScalar() == System.DBNull.Value ? 1 : Convert.ToDecimal(Cmd.ExecuteScalar());
                }

                AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM)) + (Price * Convert.ToDecimal(QtyUoMReserved.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM)) + (Price * Convert.ToDecimal(QtyUoMReserved.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
            }

            if (AdjustInProgress != 0 || Available != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "IN31";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn,Trans, Cmd);
                string Notes = "";

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtTransferNo.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                using (Cmd = new SqlCommand(Query, Conn,Trans))
                {
                    Cmd.ExecuteNonQuery();
                }
                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                using (Cmd = new SqlCommand(Query, Conn,Trans))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                        Type = Convert.ToString(Dr["Type"]);
                        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                        AmountValue = 0;

                        if (JournalHID == "IN31")
                        {
                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = Available;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = AdjustInProgress;
                            }
                        }

                        if (AmountValue == 0)
                        {
                            continue;
                        }

                        //Insert Detail GLJournal
                        Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                        Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                        Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                        using (Cmd = new SqlCommand(Query, Conn,Trans))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        SeqNo++;
                    }
                    Dr.Close();
                }
            }
        }

        private void CreateJournalNotApproval(SqlTransaction Trans)
        {
            decimal AdjustInProgress = 0, Available = 0;
            decimal Price = 0;
            for (int i = 0; i < dgvNotaTransfer.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value);
                string QtyUoM = Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value) == "" ? "0" : Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
                string QtyUoMReserved = Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value) == "" ? "0" : Convert.ToString(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);
                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + FullItemID + "';";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Price = Cmd.ExecuteScalar() == System.DBNull.Value ? 1 : Convert.ToDecimal(Cmd.ExecuteScalar());
                }

                AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM)) + (Price * Convert.ToDecimal(QtyUoMReserved.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM)) + (Price * Convert.ToDecimal(QtyUoMReserved.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
            }

            if (AdjustInProgress != 0 || Available != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "IN39";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);
                string Notes = "";

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtTransferNo.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                using (Cmd = new SqlCommand(Query, Conn,Trans))
                {
                    Cmd.ExecuteNonQuery();
                }
                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                using (Cmd = new SqlCommand(Query, Conn,Trans))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                        Type = Convert.ToString(Dr["Type"]);
                        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                        AmountValue = 0;

                        if (JournalHID == "IN39")
                        {
                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = Available;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = AdjustInProgress;
                            }
                        }

                        if (AmountValue == 0)
                        {
                            continue;
                        }

                        //Insert Detail GLJournal
                        Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                        Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                        Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                        using (Cmd = new SqlCommand(Query, Conn,Trans))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        SeqNo++;
                    }
                    Dr.Close();
                }
            }
        }

        private bool validate()
        {
            if (dgvNotaTransfer.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return false;
            }
            else if (txtVehicleType.Text.Trim() == "" || txtVehicleType.Text.Trim() == "")
            {
                MessageBox.Show("Vehicle Type tidak boleh kosong.");
                return false;
            }
            else if (txtVehicleNumber.Text.Trim() == "" || txtVehicleNumber.Text.Trim() == "")
            {
                MessageBox.Show("Vehicle Number tidak boleh kosong.");
                return false;
            }
            else if (txtDriverName.Text.Trim() == "" || txtDriverName.Text.Trim() == "")
            {
                MessageBox.Show("Driver Name tidak boleh kosong.");
                return false;
            }
            else if (txtInventSiteIDFrom.Text.Trim() == "")
            {
                MessageBox.Show("Warehouse From tidak boleh kosong.");
                return false;
            }
            else if (txtInventSiteIDTo.Text.Trim() == "")
            {
                MessageBox.Show("Warehouse To tidak boleh kosong.");
                return false;
            }
            else if (txtVehicleType.Text.Trim() == "")
            {
                MessageBox.Show("Vehicle Type tidak boleh kosong.");
                return false;
            }
            else if (txtVehicleNumber.Text.Trim() == "")
            {
                MessageBox.Show("Vehicle Number tidak boleh kosong.");
                return false;
            }
            else if (txtDriverName.Text.Trim() == "")
            {
                MessageBox.Show("Driver Name tidak boleh kosong.");
                return false;
            }
            else if (txtInventSiteIDFrom.Text == txtInventSiteIDTo.Text)
            {
                MessageBox.Show("Invent Site tidak boleh sama.");
                return false;
            }
            else if (txtBoxVehicleOwnerId.Text == "")
            {
                MessageBox.Show("Invent Site tidak boleh kosong.");
                return false;
            }
            else if (statusQty() == false)
            {
                MessageBox.Show("Quantity melebihi stock.");
                return false;
            }
            else if (statusQty2() == false)
            {
                MessageBox.Show("Qty Available for Sale dan Reserved Item bernilai 0, tidak boleh bernilai 0 untuk keduanya secara bersamaan.");
                return false;
            }
            else if (statusQty3() == false)
            {
                MessageBox.Show("Item Pada NT Reserved tidak boleh 0.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void checkEditDeleteSOList(SqlTransaction Trans)
        {
            if (dgvDetailSO.Rows.Count != 0)
            {
                for (int i = 0; i < SOId.Length; i++)
                {
                    int del = 0;
                    for (int x = 0; x < dgvDetailSO.Rows.Count; x++)
                    {
                        if (SeqNo[i] == Convert.ToInt32(dgvDetailSO.Rows[x].Cells["SeqNo"].Value))
                        {
                            if (Convert.ToDecimal(dgvDetailSO.Rows[x].Cells["NT Reserved"].Value) != NT_Qty[i])
                            {
                                Query = "UPDATE [dbo].[NotaTransfer_SO_List] SET [NT_Qty] = " + dgvDetailSO.Rows[x].Cells["NT Reserved"].Value + ", [UpdatedDate]='" + DateTime.Now + "',[UpdatedBy]='" + ControlMgr.UserId + "' WHERE [TransferNo]='" + txtTransferNo.Text + "' AND [SeqNo]=" + dgvDetailSO.Rows[x].Cells["SeqNo"].Value + " ";
                                using (Cmd = new SqlCommand(Query, Conn, Trans))
                                {
                                    Cmd.ExecuteNonQuery();
                                }
                                updateLockTable(txtTransferNo.Text, Mode, Trans, x, "Edit");
                            }
                        }
                        else
                        {
                            del++;
                        }
                        if (del == dgvDetailSO.Rows.Count)
                        {
                            Query = "DELETE FROM [dbo].[NotaTransfer_SO_List] WHERE [TransferNo]='" + txtTransferNo.Text + "' AND [SeqNo]=" + SeqNo[i] + " ";
                            using (Cmd = new SqlCommand(Query, Conn, Trans))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                            updateLockTable(txtTransferNo.Text, Mode, Trans, i, "DELETE");
                        }
                    }
                }
            }
            else if (dgvDetailSO.Rows.Count == 0)
            {
                for (int i = 0; i < SeqNo.Length; i++)
                {
                    Query = "DELETE FROM [dbo].[NotaTransfer_SO_List] WHERE [TransferNo]='" + txtTransferNo.Text + "' AND [SeqNo]=" + SeqNo[i] + " ";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                    updateLockTable(txtTransferNo.Text, Mode, Trans, i, "DELETE");
                }
            }
            for (int h = 0; h < dgvDetailSO.Rows.Count; h++)
            {
                bool create = false;
                if (SeqNo.Length > 0)
                {
                    if (Convert.ToInt32(dgvDetailSO.Rows[h].Cells["SeqNo"].Value) > SeqNo.Max())
                    {
                        Query = "SELECT * FROM [dbo].[InventLockTable] WHERE [RefTransType]='SALES ORDER' AND [RefTransId]='" + dgvDetailSO.Rows[h].Cells["SO Id"].Value.ToString() + "' AND [RefTrans_SeqNo]="+dgvDetailSO.Rows[h].Cells["SO Seq No"].Value+" AND [RefTrans2Id]='" + txtTransferNo.Text + "' AND [Lock_Qty]=0 ";
                        using (Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                insertTableSONT(txtTransferNo.Text, Trans, h);
                                updateLockTable(txtTransferNo.Text, Mode, Trans, h, "Edit");
                            }
                            else
                            {
                                create = true;
                            }
                            Dr.Close();
                        }
                        if (create == true)
                        {
                            insertTableSONT(txtTransferNo.Text, Trans, h);
                            insertLockTable(txtTransferNo.Text, "New", Trans, h);
                        }
                    }
                }
                else
                {
                    insertTableSONT(txtTransferNo.Text, Trans, h);
                    insertLockTable(txtTransferNo.Text, "New", Trans, h);
                }
            }
        }

        private void checkEditDelete(SqlTransaction Trans)
        {
            for (int i = 0; i < fullitemid.Length; i++)
            {
                int del = 0;
                for (int x = 0; x < dgvNotaTransfer.Rows.Count; x++)
                {
                    if (TransNo[i] == Convert.ToInt32(dgvNotaTransfer.Rows[x].Cells["SeqNo"].Value))
                    {
                        if (Convert.ToDecimal(dgvNotaTransfer.Rows[x].Cells["Transfer Qty"].Value) != TransQty[i]  || Convert.ToDecimal(dgvNotaTransfer.Rows[x].Cells["Transfer Qty Reserved"].Value) != TransQtyRes[i])
                        {
                            updateTableDtl(Trans, x);
                            //insertNotaLog(txtTransferNo.Text, "01", Trans, "Edited", x);
                            insertInventTrans(Trans, "-", fullitemid[x], txtTransferNo.Text, x, i, "Edit");
                        }
                        else if (dgvNotaTransfer.Rows[x].Cells["Notes"].Value.ToString() != Notes[i].ToString())
                        {
                            updateTableDtl(Trans, x);
                        }
                    }
                    else
                    {
                        del++;
                    }
                    if (del == dgvNotaTransfer.Rows.Count)
                    {
                        Query = "DELETE FROM [dbo].[NotaTransferD] WHERE [TransferNo]='" + txtTransferNo.Text + "' AND [SeqNo]=" + TransNo[i] + " ";
                        using (Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        //insertNotaLog(txtTransferNo.Text, "XX", Trans, "Deleted", i);
                        insertInventTrans(Trans, "+", fullitemid[i], txtTransferNo.Text, x, i, "Delete");
                    }
                }
            }
            for (int h = 0; h < dgvNotaTransfer.Rows.Count; h++)
            {
                if (Convert.ToInt32(dgvNotaTransfer.Rows[h].Cells["SeqNo"].Value) > TransNo.Max())
                {
                    insertTableDtl(txtTransferNo.Text, Trans, h);
                    //insertNotaLog(txtTransferNo.Text, "01", Trans, "Created", h);
                    insertInventTrans(Trans, "-", dgvNotaTransfer.Rows[h].Cells["FullItemId"].Value.ToString(), txtTransferNo.Text, h, h, "Created");
                }
            }
        }

        private void updateTableDtl(SqlTransaction Trans, int x)
        {
            decimal ratio = 0;
            Query = "SELECT Ratio FROM [dbo].[InventConversion] WHERE [FullItemID] = '" + dgvNotaTransfer.Rows[x].Cells["FullItemId"].Value.ToString() + "'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            decimal qty = Convert.ToDecimal(dgvNotaTransfer.Rows[x].Cells["Transfer Qty"].Value);
            decimal qtyres = Convert.ToDecimal(dgvNotaTransfer.Rows[x].Cells["Transfer Qty Reserved"].Value);

            Query = "UPDATE [dbo].[NotaTransferD] ";
            Query += "SET [Qty]=" + (qty + qtyres) + ",[Qty_Alt]=" + ((qty + qtyres)*ratio) + ",[NT_Available_Qty] = " + qty + ", ";
            Query += "[Notes] = '" + dgvNotaTransfer.Rows[x].Cells["Notes"].Value.ToString().Trim() + "', ";
            Query += "[UpdatedDate]='" + DateTime.Now + "',[UpdatedBy]='" + ControlMgr.UserId + "', ";
            Query += " [RemainingQty]=" + ((qty + qtyres)) + ", ";
            Query += " [NT_Available_Qty_Alt] =" + (qty * ratio) + ", ";
            Query += " [NT_Reserved_Qty] =" + qtyres + ", ";
            Query += " [NT_Reserved_Qty_Alt] = " + (qtyres * ratio) + ", ";
            Query += " RemainingAvailable = "+qty+", ";
            Query += " RemainingReserved = "+qtyres+" ";
            Query += "WHERE [TransferNo]='" + txtTransferNo.Text + "' AND [SeqNo]=" + dgvNotaTransfer.Rows[x].Cells["SeqNo"].Value + " ";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertTableSONT(string TransferNumber, SqlTransaction Trans, int i)
        {
            int ntseq = 0;
            Query = "SELECT [SeqNo] FROM [dbo].[NotaTransferD] WHERE [FullItemId]='" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value + "' AND [TransferNo] = '" + TransferNumber + "'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                ntseq = Convert.ToInt32(Cmd.ExecuteScalar());
            }

            Query = "INSERT INTO [dbo].[NotaTransfer_SO_List] VALUES ('" + TransferNumber + "'," + ntseq + "," + dgvDetailSO.Rows[i].Cells["SeqNo"].Value.ToString() + ",'" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() + "','" + dgvDetailSO.Rows[i].Cells["ItemName"].Value.ToString() + "', ";
            Query += " '" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "'," + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value + ", ";
            Query += " " + Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["SO Qty"].Value) + ", ";
            Query += " " + Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["SO Reserved"].Value) + ", " + Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["NT Reserved"].Value) + ",'" + dgvDetailSO.Rows[i].Cells["Unit"].Value.ToString() + "', ";
            Query += " '" + ControlMgr.UserId + "','" + DateTime.Now + "','','') ";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }


        private void insertInventMovement(SqlTransaction Trans)
        {
            if (txtTransferNo.Text != null && txtTransferNo.Text.Trim() != "")
            {
                for (int i = 0; i < fullitemid.Length; i++)
                {
                    decimal Ratio = 0;
                    Query = "SELECT Ratio FROM InventConversion WHERE [FullItemID] ='" + fullitemid[i] + "' AND [FromUnit]='" + deletedunit[i] + "' AND [ToUnit]='KG' ";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }

                    decimal price = 0;
                    Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + fullitemid[i] + "'";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                    Query = "UPDATE [dbo].[Invent_Movement_Qty] SET ";
                    Query += " [Transfer_In_Progress_UoM] = [Transfer_In_Progress_UoM] - " + (TransQty[i] + TransQtyRes[i]) + ", ";
                    Query += " [Transfer_In_Progress_Alt] = [Transfer_In_Progress_Alt] - " + ((TransQty[i] + TransQtyRes[i]) * Ratio) + ", ";
                    Query += " [Transfer_In_Progress_Amount] = [Transfer_In_Progress_Amount] - " + ((TransQty[i] + TransQtyRes[i]) * price) + " ";
                    Query += " WHERE [FullItemId] = '" + fullitemid[i] + "'  ";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
            {
                decimal Ratio = 0;
                Query = "SELECT Ratio FROM InventConversion WHERE [FullItemID] ='" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [FromUnit]='" + dgvNotaTransfer.Rows[i].Cells["Unit"].Value.ToString() + "' AND [ToUnit]='KG' ";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
                }

                decimal price = 0;
                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "'";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                decimal qty = dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
                decimal qtyres = dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);

                Query = "UPDATE [dbo].[Invent_Movement_Qty] SET ";
                Query += " [Transfer_In_Progress_UoM] = [Transfer_In_Progress_UoM] + " + (qty + qtyres) + ", ";
                Query += " [Transfer_In_Progress_Alt] = [Transfer_In_Progress_Alt] + " + ((qty + qtyres) * Ratio) + ", ";
                Query += " [Transfer_In_Progress_Amount] = [Transfer_In_Progress_Amount] + " + ((qty + qtyres) * price) + " ";
                Query += " WHERE [FullItemId] = '" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' ";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void insertInventOnHand(SqlTransaction Trans, string TransferNo)
        {
            if (txtTransferNo.Text != null && txtTransferNo.Text.Trim() != "")
            {
                for (int i = 0; i < fullitemid.Length; i++)
                {
                    int tes = fullitemid.Length;
                    decimal Ratio = 0;
                    Query = "SELECT Ratio FROM InventConversion WHERE [FullItemID] ='" + fullitemid[i] + "' AND [FromUnit]='" + deletedunit[i] + "' AND [ToUnit]='KG' ";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }

                    decimal price = 0;
                    Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + fullitemid[i] + "'";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }

                    Query = "UPDATE [dbo].[Invent_OnHand_Qty] SET ";
                    if (TransQty[i] != null && TransQty[i].ToString().Trim() != "")
                    {
                        Query += " [Available_For_Sale_UoM] = [Available_For_Sale_UoM] + " + TransQty[i] + ", ";
                        Query += " [Available_For_Sale_Alt] = [Available_For_Sale_Alt] + " + (TransQty[i] * Ratio) + ", ";
                        Query += " [Available_For_Sale_Amount] = [Available_For_Sale_Amount] + " + (TransQty[i] * price) + ", ";
                    }
                    if (TransQtyRes[i] != null && TransQtyRes[i].ToString().Trim() != "")
                    {
                        Query += " [Available_For_Sale_Reserved_UoM] = [Available_For_Sale_Reserved_UoM] + " + TransQtyRes[i] + ", ";
                        Query += " [Available_For_Sale_Reserved_Alt] = [Available_For_Sale_Reserved_Alt] + " + (TransQtyRes[i] * Ratio) + ", ";
                        Query += " [Available_For_Sale_Reserved_Amount] = [Available_For_Sale_Reserved_Amount] + " + (TransQtyRes[i] * price) + ", ";
                    }
                    Query += " [Available_UoM] = [Available_UoM] ";//+ " + (TransQty[i] + TransQtyRes[i]) + ", ";
                    //Query += " [Available_Alt] = [Available_Alt] + " + ((TransQty[i] + TransQtyRes[i]) * Ratio) + ", ";
                    //Query += " [Available_Amount] = [Available_Amount] + " + ((TransQty[i] + TransQtyRes[i]) * price) + " ";
                    Query += " WHERE [FullItemId] = '" + fullitemid[i] + "' AND [InventSiteId]='" + txtInventSiteIDFrom.Text + "' ";
                    using (Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
            {
                decimal Ratio = 0;
                Query = "SELECT Ratio FROM InventConversion WHERE [FullItemID] ='" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [FromUnit]='" + dgvNotaTransfer.Rows[i].Cells["Unit"].Value.ToString() + "' AND [ToUnit]='KG' ";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
                }

                decimal price = 0;
                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "'";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                decimal qty = dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
                decimal qtyres = dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value == "" ? Convert.ToDecimal(0) : Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);

                Query = "UPDATE [dbo].[Invent_OnHand_Qty] SET ";
                Query += " [Available_For_Sale_UoM] = [Available_For_Sale_UoM] - " + qty + ", ";
                Query += " [Available_For_Sale_Alt] = [Available_For_Sale_Alt] - " + (qty * Ratio) + ", ";
                Query += " [Available_For_Sale_Amount] = [Available_For_Sale_Amount] - " + (qty * price) + ", ";
                Query += " [Available_For_Sale_Reserved_UoM] = [Available_For_Sale_Reserved_UoM] - " + qtyres + ", ";
                Query += " [Available_For_Sale_Reserved_Alt] = [Available_For_Sale_Reserved_Alt] - " + (qtyres * Ratio) + ", ";
                Query += " [Available_For_Sale_Reserved_Amount] = [Available_For_Sale_Reserved_Amount] - " + (qtyres * price) + " ";
                //Query += " [Available_UoM] = [Available_UoM] - " + (qty + qtyres) + ", ";
                //Query += " [Available_Alt] = [Available_Alt] - " + ((qty + qtyres) * Ratio) + ", ";
                //Query += " [Available_Amount] = [Available_Amount] - " + ((qty + qtyres) * price) + " ";
                Query += " WHERE [FullItemId] = '" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [InventSiteId]='" + txtInventSiteIDFrom.Text + "' ";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void insertInventTrans(SqlTransaction Trans, string opera, string FullItemId, string Transfer, int i, int x, string status)
        {
            string GroupId = "";
            string SubGroupId = "";
            string SubGroup2Id = "";
            string ItemId = "";
            string ItemName = "";
            decimal price = 0;
            decimal ratio = 0;
            decimal qty = 0;
            decimal qtyres = 0;
            int No = Convert.ToInt32(dgvNotaTransfer.Rows[i].Cells["SeqNo"].Value);
            Query = "SELECT a.*, b.* FROM InventTable a INNER JOIN InventConversion b ON a.FullItemID=b.FullItemID WHERE a.FullItemId = '" + FullItemId + "' AND b.ToUnit='KG'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    GroupId = Dr["GroupId"].ToString();
                    SubGroup2Id = Dr["SubGroup2Id"].ToString();
                    SubGroupId = Dr["SubGroup1Id"].ToString();
                    ItemId = Dr["ItemId"].ToString();
                    ItemName = Dr["ItemDeskripsi"].ToString();
                    price = Dr["UoM_AvgPrice"] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr["UoM_AvgPrice"]);
                    ratio = Dr["Ratio"] == System.DBNull.Value ? 0: Convert.ToDecimal(Dr["Ratio"]);
                }
                Dr.Close();
            }
            string notes = "";
            if (No != 0)
            {
                notes = dgvNotaTransfer.Rows[i].Cells["Notes"].Value.ToString().Trim();
            }
            //if (opera == "-")
            //{
            //    notes = "";
            //}
            qty = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
            qtyres = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);
            //if (status.ToUpper() == "EDIT")
            //{
            //    qty = qty - Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value) + TransQty[x];
            //    qtyres = qtyres - Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value) + TransQtyRes[x];
            //    opera = "";
            //}
            //else
            //{
            //    qty = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
            //    qtyres = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);
            //}
            //Query = "INSERT INTO InventTrans VALUES ( ";
            //Query += "'" + GroupId + "','" + SubGroupId + "','" + SubGroup2Id + "','" + ItemId + "','" + FullItemId + "','" + ItemName + "','" + txtInventSiteIDFrom.Text + "','" + Transfer + "', ";
            //Query += " " + No + ",'" + dtTransferDate.Value.Date + "','','',0,'','', ";
            //Query += " " + opera + (qty + qtyres) + "," + opera + ((qty + qtyres) * ratio) + "," + opera + ((qty + qtyres) * price) + "," + opera + (qty) + "," + opera + (qty * ratio) + ", ";
            //Query += " " + opera + (qty + price) + "," + opera + (qtyres) + "," + opera + (qtyres * ratio) + "," + opera + (qtyres * price) + ",'" + notes + "')";
            if (opera == "-")
            {
                Query = "INSERT INTO InventTrans VALUES ( ";
                Query += "'" + GroupId + "','" + SubGroupId + "','" + SubGroup2Id + "','" + ItemId + "','" + FullItemId + "',@ItemName,'" + txtInventSiteIDFrom.Text + "','" + Transfer + "', ";
                Query += " " + No + ",'" + dtTransferDate.Value.Date + "','','',0,'','', ";
                Query += " " + (qty + qtyres) + "," + ((qty + qtyres) * ratio) + "," + ((qty + qtyres) * price) + "," + (qty) + "," + (qty * ratio) + ", ";
                Query += " " + (qty + price) + "," + (qtyres) + "," + (qtyres * ratio) + "," + (qtyres * price) + ",@notes)";
            }
            else
            {
                Query = "DELETE FROM InventTrans WHERE [TransId] = '" + Transfer + "' AND [SeqNo]="+TransNo[x]+" ";
            }
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.Parameters.AddWithValue("@ItemName", ItemName);
                Cmd.Parameters.AddWithValue("@notes", notes);
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertLockTable(string TransferNumber, string Mode, SqlTransaction Trans, int i)
        {
            decimal Ratio = 0;
            decimal qty = 0;
            decimal qtyalt = 0;

            Query = "SELECT Ratio FROM InventConversion WHERE [FullItemID] ='" + dgvDetailSO.Rows[i].Cells["FullItemID"].Value.ToString() + "' AND [FromUnit]='" + dgvDetailSO.Rows[i].Cells["Unit"].Value.ToString() + "' AND [ToUnit]='KG' ";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            qty = Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["NT Reserved"].Value) * Convert.ToDecimal(-1);
            qtyalt = qty * Ratio;

            if (Mode.ToUpper() == "NEW")
            {
                Query = "INSERT INTO [dbo].[InventLockTable] VALUES ('SALES ORDER','" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "','" + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value.ToString() + "','" + TransferNumber + "'," + dgvDetailSO.Rows[i].Cells["Transfer_SeqNo"].Value + ",'" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() + "','" + txtInventSiteIDFrom.Text + "'," + Ratio + "," + qty + ",'" + dgvDetailSO.Rows[i].Cells["Unit"].Value.ToString() + "'," + qtyalt + ",'KG','" + DateTime.Now + "','" + ControlMgr.UserId + "','" + DateTime.Now + "','')";
            }
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void updateLockTable(string TransferNumber, string Mode, SqlTransaction Trans, int i, string status)
        {
            decimal Ratio = 0;
            decimal qty = 0;
            decimal qtyalt = 0;

            if (status.ToUpper() != "DELETE")
            {
                Query = "SELECT Ratio FROM InventConversion WHERE [FullItemID] ='" + dgvDetailSO.Rows[i].Cells["FullItemID"].Value.ToString() + "' AND [FromUnit]='" + dgvDetailSO.Rows[i].Cells["Unit"].Value.ToString() + "' AND [ToUnit]='KG' ";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
                }

                qty = Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["NT Reserved"].Value) * Convert.ToDecimal(-1);
                if (status.ToUpper() == "DELETE")
                {
                    qty = 0;
                }
                qtyalt = qty * Ratio;
            }
            if (Mode.ToUpper() == "EDIT")
            {
                int recid = 0;
                if (status.ToUpper() == "DELETE")
                {
                    Query = "SELECT TOP 1 RecId FROM [dbo].[InventLockTable] WHERE [RefTransType]='" + txtBoxRefType.Text + "' AND [RefTransId]='" + SOId[i] + "' AND [RefTrans_SeqNo]=" + SO_SeqNo[i] + " AND [FullItemId]='" + fullitemidSO[i] + "' AND [RefTrans2Id] = '" + TransferNo2[i] + "' AND [RefTrans2_SeqNo]='" + Transfer_SeqNo[i] + "' ORDER BY RecId DESC";
                }
                else
                {
                    Query = "SELECT TOP 1 RecId FROM [dbo].[InventLockTable] WHERE [RefTransType]='" + txtBoxRefType.Text + "' AND [RefTransId]='" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "' AND [RefTrans_SeqNo]=" + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value + " AND [FullItemId]='" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [RefTrans2Id] = '" + dgvDetailSO.Rows[i].Cells["TransferNo"].Value.ToString() + "' AND [RefTrans2_SeqNo]='" + dgvDetailSO.Rows[i].Cells["Transfer_SeqNo"].Value.ToString() + "' ORDER BY RecId DESC";
                }

                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    recid = Convert.ToInt32(Cmd.ExecuteScalar());
                }

                Query = "UPDATE [dbo].[InventLockTable] SET Lock_Qty=" + qty + ",[Lock_Qty_Alt]=" + qtyalt + ",UpdatedBy='" + ControlMgr.UserId + "',UpdatedDate='" + DateTime.Now + "' WHERE RecId= " + recid + "";
            }
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void InsertNTLog(string TransferNumber, string transstatus,string statusdesk, SqlTransaction Trans, string action)
        {
            Query = "INSERT INTO [dbo].[NotaTransfer_LogTable] ([NTDate],[NTId],[FromSiteId],[ToSiteId],[LogStatusCode],[LogStatusDesc],[Action],[UserID],[LogDate]) ";
            Query += "VALUES (@NTDate,@NTId,@FromSiteId,@ToSiteId,@LogStatusCode,@LogStatusDesc,@Action,@UserID,@LogDate)";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.Parameters.AddWithValue("@NTDate",dtTransferDate.Value);
                Cmd.Parameters.AddWithValue("@NTId",txtTransferNo.Text);
                Cmd.Parameters.AddWithValue("@FromSiteId",txtInventSiteIDFrom.Text);
                Cmd.Parameters.AddWithValue("@ToSiteId",txtInventSiteIDTo.Text);
                Cmd.Parameters.AddWithValue("@LogStatusCode",transstatus);
                Cmd.Parameters.AddWithValue("@LogStatusDesc",statusdesk);
                Cmd.Parameters.AddWithValue("@Action",action);
                Cmd.Parameters.AddWithValue("@UserID",ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@LogDate",DateTime.Now);
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertNotaLog(string TransferNumber, string transstatus, SqlTransaction Trans, string status, int i)
        {
            string fullitemid2;
            decimal qty;
            int No;

            if (status.ToUpper() == "DELETED")
            {
                fullitemid2 = fullitemid[i];
                qty = Convert.ToDecimal(TransQty[i] + TransQtyRes[i]);
                No = TransNo[i];
            }
            else
            {
                fullitemid2 = dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString();
                qty = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value) + Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);
                No = Convert.ToInt32(dgvNotaTransfer.Rows[i].Cells["SeqNo"].Value);
            }

            decimal price = 0;
            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + fullitemid2 + "'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                price = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            decimal ratio = 0;
            Query = "SELECT Ratio FROM [dbo].[InventConversion] WHERE [FullItemID] = '" + fullitemid2 + "'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            string statusdeskripsi = "";
            Query = "SELECT Deskripsi FROM [dbo].[TransStatusTable] WHERE StatusCode= '" + transstatus + "' AND TransCode ='TransferNote'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                statusdeskripsi = Cmd.ExecuteScalar().ToString();
            }

            decimal amount = qty * price;
            decimal UoMAlt = qty * ratio;
            Query = "INSERT INTO [dbo].[NotaTransfer_LogTable] VALUES ('" + dtTransferDate.Value.Date + "','" + TransferNumber + "','','','','" + txtInventSiteIDFrom.Text + "','" + txtInventSiteIDTo.Text + "','" + fullitemid2 + "','" + No + "','',''," + qty + "," + UoMAlt + "," + amount + ",'" + transstatus + "','" + statusdeskripsi + "','" + status + "','" + ControlMgr.UserId + "','" + DateTime.Now + "')";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertTableDtl(string TransferNumber, SqlTransaction Trans, int i)
        {
            decimal ratio=0;
            decimal cogs = 0;
            Query = "SELECT a.Ratio, b.[UoM_AvgPrice] FROM InventConversion a LEFT JOIN [InventTable] b ON a.FullItemID = b.FullItemID WHERE a.[FullItemID] ='" + dgvNotaTransfer.Rows[i].Cells["FullItemID"].Value.ToString() + "' AND a.[FromUnit]='" + dgvNotaTransfer.Rows[i].Cells["Unit"].Value.ToString() + "' AND a.[ToUnit]='KG'";
            using(Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Dr = Cmd .ExecuteReader();
                while(Dr.Read())
                {
                    ratio = Convert.ToDecimal(Dr["Ratio"]);
                    cogs = Convert.ToDecimal(Dr["Uom_AvgPrice"]);
                }
                Dr.Close();
            }

            decimal qty = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value);
            decimal qtyReserved = Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value);

            Query = "";
            Query += "Insert [NotaTransferD] (TransferNo,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemId,ItemName,Qty,UoM,Qty_Alt,UoM_Alt,Ratio,InventSite,Notes,CreatedDate,CreatedBy,LockDocument,[NT_Available_Qty],NT_Available_Qty_Alt,NT_Reserved_Qty ,NT_Reserved_Qty_Alt,ReferenceId,Reference_SeqNo,RemainingQty,[COGS],RemainingAvailable,RemainingReserved ) Values ";
            Query += "(@TransferNo,'";
            Query += (dgvNotaTransfer.Rows[i].Cells["SeqNo"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["SeqNo"].Value.ToString()) + "','";
            Query += (dgvNotaTransfer.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
            Query += (dgvNotaTransfer.Rows[i].Cells["SubGroupId"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["SubGroupId"].Value.ToString()) + "','";
            Query += (dgvNotaTransfer.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
            Query += (dgvNotaTransfer.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
            Query += (dgvNotaTransfer.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["FullItemID"].Value.ToString()) + "',";
            Query += "@ItemName,'";
            Query += (qty + qtyReserved) + "','";
            Query += (dgvNotaTransfer.Rows[i].Cells["Unit"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["Unit"].Value.ToString()) + "','";
            Query += ((qtyReserved + qty) * ratio) + "','";
            Query += "KG','";
            Query += (ratio) + "',";
            Query += "@InventSiteIDFrom,";
            Query += "@Notes,";
            Query += "getdate(),";
            Query += "@CreatedBy,'";
            Query += "',";
            Query += (qty) + ",";
            Query += (qty * ratio) + ",";
            Query += (qtyReserved) + ",";
            Query += (qtyReserved * ratio) + ",'";
            Query += "','";
            Query += "0'," + (qty + qtyReserved) + ","+cogs+", "+qty+","+qtyReserved+");";

            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
            Cmd.Parameters.AddWithValue("@ItemName", (dgvNotaTransfer.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["ItemName"].Value.ToString()));
            Cmd.Parameters.AddWithValue("@TransferNo", TransferNumber);
            Cmd.Parameters.AddWithValue("@InventSiteIDFrom", txtInventSiteIDFrom.Text.Trim());
            Cmd.Parameters.AddWithValue("@Notes", (dgvNotaTransfer.Rows[i].Cells["Notes"].Value == "" ? "" : dgvNotaTransfer.Rows[i].Cells["Notes"].Value.ToString().Trim()));
            Cmd.ExecuteNonQuery();
            Query = "";
        }

        private bool statusQty()
        {

            for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
            {
                decimal qtyAvailable = 0;
                //Query = "SELECT a.[Available_For_Sale_UoM], b.[Qty] FROM [dbo].[Invent_OnHand_Qty] a INNER JOIN (SELECT FullItemID,Qty FROM [dbo].[SalesOrderD] WHERE [SalesOrderNo] ='" + dgvNotaTransfer.Rows[i].Cells["ReferenceId"].Value.ToString() + "') b ON a.[FullItemId]=b.[FullItemID] WHERE a.[FullItemId]='" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND a.[InventSiteId] ='" + txtInventSiteIDFrom.Text + "'";
                Query = "SELECT a.[Available_For_Sale_UoM] FROM [dbo].[Invent_OnHand_Qty] a WHERE a.[FullItemId]='" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND a.[InventSiteId] ='" + txtInventSiteIDFrom.Text + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    qtyAvailable = Convert.ToDecimal(Cmd.ExecuteScalar());
                }

                if (Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value) > qtyAvailable)
                {
                    return false;
                }
            }
            for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
            {
                decimal qtyReserved = 0;
                Query = "SELECT SUM([Lock_Qty]) as Lock_Qty, FullItemId FROM [dbo].[InventLockTable] WHERE [RefTransId] ='" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "' AND [SiteId] = '" + txtInventSiteIDFrom.Text + "' AND [FullItemId]='" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [RefTrans_SeqNo]=" + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value + "  GROUP BY [SiteId],[FullItemId],[RefTransId],[Unit],[RefTransType],[RefTrans_SeqNo] ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    qtyReserved = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                decimal resqty = 0;
                Query = "SELECT [NT_Qty] FROM [dbo].[NotaTransfer_SO_List] WHERE [TransferNo] = '" + txtTransferNo.Text + "' AND [SOId]='" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "' AND [SO_SeqNo]=" + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value + " ";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    resqty = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                if (Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["NT Reserved"].Value) > (qtyReserved + resqty))
                {
                    dgvDetailSO.Rows[i].Cells["Transfer Qty"].Value = resqty;
                    return false;
                }
            }

            return true;
        }

        private bool statusQty3()
        {
            decimal qtyzero = Convert.ToDecimal(0.00);
            for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dgvDetailSO.Rows[i].Cells["NT Reserved"].Value) == qtyzero)
                {
                    return false;
                }
            }
            return true;
        }

        private bool statusQty2()
        {
            decimal qtyzero = Convert.ToDecimal(0.00);
            for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
            {
                if (Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty"].Value) == qtyzero && Convert.ToDecimal(dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value) == qtyzero)
                {
                    return false;
                }
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeView();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (metroTabControl1.SelectedIndex == 0)
            {
                if (txtInventSiteIDFrom.Text.Trim() != "" && txtInventSiteIDFrom.Text != null)
                {
                    string Table = "[dbo].[Invent_OnHand_Qty]";
                    string QuerySearch = "SELECT a.[GroupId],a.[SubGroupId],a.[SubGroup2Id],a.[ItemId],a.[FullItemId], a.[ItemName], a.[InventSiteId],a.[Available_For_Sale_UoM],a.[Available_For_Sale_Reserved_UoM],b.[UoM]  FROM [dbo].[Invent_OnHand_Qty] a LEFT JOIN [dbo].[InventTable] b ON a.[FullItemId]=b.[FullItemID]WHERE a.[InventSiteId] = '" + txtInventSiteIDFrom.Text + "'";
                    //BY: HC (S)
                    if (tbxRefID.Text != String.Empty && tbxRefID.Text.Split('/')[0] == "DO")
                    {
                        QuerySearch += " and a.FullItemId in (select b.FullItemId from DeliveryOrderH a left join DeliveryOrderD b on a.DeliveryOrderId = b.DeliveryOrderId where a.DeliveryOrderId = '" + tbxRefID.Text + "' and a.InventSiteID = '" + txtInventSiteIDTo.Text + "')";
                    }
                    //BY: HC (E)
                    for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
                    {
                        QuerySearch += " AND NOT a.FullItemId = '" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "'";
                    }
                    if (fullitemid != null)
                    {
                        for (int i = 0; i < fullitemid.Length; i++)
                        {
                            QuerySearch += " AND NOT a.FullitemId = '" + fullitemid[i] + "' ";
                        }
                    }
                    string[] FilterText = { "FullItemId", "ItemName" };
                    string[] Select = { "FullItemId", "ItemName", "Available_For_Sale_UoM", "UoM", "Available_For_Sale_Reserved_UoM", "GroupId", "SubGroupId", "SubGroup2Id", "ItemId" };
                    string PrimaryKey = "FullItemId";
                    string[] HideField = { "GroupId", "SubGroupId", "SubGroup2Id", "ItemId" };
                    callSearchQueryV2Form(Table, QuerySearch, FilterText, Select, PrimaryKey, HideField);
                }
                else
                {
                    MessageBox.Show("Invent Id belom terpilih!");
                }
            }
            else if (metroTabControl1.SelectedIndex == 1)
            {
                if (dgvNotaTransfer.Rows.Count > 0)
                {
                    string Table = "[dbo].[InventLockTable]";
                    string QuerySearch = "SELECT RefTransType,RefTransId,RefTrans_SeqNo,FullItemId,SiteId,SUM(Lock_Qty) as Lock_Qty,Unit FROM [dbo].[InventLockTable]  WHERE  SiteId ='" + txtInventSiteIDFrom.Text + "' AND FullItemId IN ( ";
                    for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
                    {
                        QuerySearch += " '" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "' ";
                        if (i + 1 != dgvNotaTransfer.Rows.Count)
                        {
                            QuerySearch += ",";
                        }
                    }
                    QuerySearch += " ) ";
                    for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
                    {
                        QuerySearch += " AND NOT (RefTransId = '" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "' AND [FullItemId]='" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [RefTrans_SeqNo] = " + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value + ") ";
                    }
                    if (TransferNo2 != null)
                    {
                        for (int i = 0; i < TransferNo2.Length; i++)
                        {
                            QuerySearch += " AND NOT (RefTransId='" + SOId[i] + "'AND FullItemId='" + fullitemidSO[i] + "' AND RefTrans_SeqNo=" + SO_SeqNo[i] + ") ";
                        }
                    }
                    QuerySearch += " GROUP BY  [SiteId],[FullItemId],[RefTransId],[Unit],[RefTransType],[RefTrans_SeqNo] HAVING SUM(Lock_Qty) != 0 ";

                    string[] FilterText = { "RefTransId", "FullItemId", "Lock_Qty", "Unit", "RefTransType" };
                    string[] Select = { "FullItemId", "RefTransId", "RefTrans_SeqNo", "Lock_Qty", "Unit" };
                    string PrimaryKey = "RefTransId";
                    string[] HideField = { };
                    callSearchQueryV2Form(Table, QuerySearch, FilterText, Select, PrimaryKey, HideField);
                }
                else
                {
                    MessageBox.Show("Item pada Item Detail Belom terpilih.");
                }
            }
        }

        private void callSearchQueryV2Form(string Table, string QuerySearch, string[] FilterText, string[] Select, string PrimaryKey, string[] HideField)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();

            populateAfterSearch(Table);
        }

        private void populateAfterSearch(string Table)
        {
            if (Variable.Kode2 == null)
            {
                return;
            }
            if (Table == "[dbo].[Invent_OnHand_Qty]")
            {
                using (Method C = new Method())
                {
                    if (dgvNotaTransfer.Rows.Count <= 0)
                    {
                        dgvNotaTransfer.ColumnCount = 14;
                        dgvNotaTransfer.Columns[0].Name = "No";
                        dgvNotaTransfer.Columns[1].Name = "SeqNo";
                        dgvNotaTransfer.Columns[2].Name = "GroupId";
                        dgvNotaTransfer.Columns[3].Name = "SubGroupId";
                        dgvNotaTransfer.Columns[4].Name = "SubGroup2Id";
                        dgvNotaTransfer.Columns[5].Name = "ItemId";
                        dgvNotaTransfer.Columns[6].Name = "FullItemId";
                        dgvNotaTransfer.Columns[7].Name = "ItemName";
                        dgvNotaTransfer.Columns[8].Name = "Available";
                        dgvNotaTransfer.Columns[9].Name = "Transfer Qty";
                        dgvNotaTransfer.Columns[10].Name = "Reserved";
                        dgvNotaTransfer.Columns[11].Name = "Transfer Qty Reserved";
                        dgvNotaTransfer.Columns[12].Name = "Unit";
                        dgvNotaTransfer.Columns[13].Name = "Notes";
                    }

                    int seqno = 0;
                    if (dgvNotaTransfer.Rows.Count > 0 || TransNo != null)
                    {
                        if (TransNo != null)
                        {
                            seqno = TransNo.Length;
                            for (int i = 0; i < TransNo.Length; i++)
                            {
                                if (seqno < TransNo[i])
                                {
                                    seqno = TransNo[i];
                                }
                            }
                        }
                        else
                        {
                            seqno = dgvNotaTransfer.Rows.Count;
                        }
                        if (dgvNotaTransfer.Rows.Count > 0)
                        {
                            for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
                            {
                                if (seqno < Convert.ToInt32(dgvNotaTransfer.Rows[i].Cells["SeqNo"].Value))
                                {
                                    seqno = Convert.ToInt32(dgvNotaTransfer.Rows[i].Cells["SeqNo"].Value);
                                }
                            }
                        }
                    }

                    for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                    {
                        dgvNotaTransfer.Rows.Add(dgvNotaTransfer.Rows.Count + 1, (seqno + i + 1), Variable.Kode2[i, 5], Variable.Kode2[i, 6], Variable.Kode2[i, 7], Variable.Kode2[i, 8], Variable.Kode2[i, 0], Variable.Kode2[i, 1], Variable.Kode2[i, 2], "0.00", Variable.Kode2[i, 4], "0.00 ", Variable.Kode2[i, 3], "");
                    }

                    dgvNotaTransfer.ReadOnly = false;
                    string[] read = new string[] { "No", "FullItemId", "ItemName", "Available", "Reserved", "Unit", "Transfer Qty Reserved" };
                    for (int i = 0; i < read.Length; i++)
                    {
                        dgvNotaTransfer.Columns[read[i]].ReadOnly = true;
                    }

                    string[] visible = new string[] { "SeqNo", "GroupId", "SubGroupId", "SubGroup2Id", "ItemId" };
                    for (int i = 0; i < visible.Length; i++)
                    {
                        dgvNotaTransfer.Columns[visible[i]].Visible = false;
                    }

                    string[] color = new string[] { "No", "FullItemId", "ItemName", "Available", "Reserved", "Unit", "Transfer Qty Reserved" };
                    for (int i = 0; i < color.Length; i++)
                    {
                        dgvNotaTransfer.Columns[color[i]].DefaultCellStyle.BackColor = Color.LightGray;
                    }

                    dgvNotaTransfer.AutoResizeColumns();
                    Variable.Kode2 = null;
                }
            }
            else if (Table == "[dbo].[InventLockTable]")
            {
                if (Variable.Kode2 != null)
                {
                    if (dgvDetailSO.Rows.Count == 0)
                    {

                        dgvDetailSO.Columns.Clear();
                        dgvDetailSO.ColumnCount = 13;
                        dgvDetailSO.Columns[0].Name = "No";
                        dgvDetailSO.Columns[1].Name = "TransferNo";
                        dgvDetailSO.Columns[2].Name = "Transfer_SeqNo";
                        dgvDetailSO.Columns[3].Name = "SeqNo";
                        dgvDetailSO.Columns[4].Name = "FullItemId";
                        dgvDetailSO.Columns[5].Name = "ItemName";
                        dgvDetailSO.Columns[6].Name = "SO Id";
                        dgvDetailSO.Columns[7].Name = "SO Seq No";
                        dgvDetailSO.Columns[8].Name = "SO Qty";
                        dgvDetailSO.Columns[9].Name = "SO Reserved";
                        dgvDetailSO.Columns[10].Name = "Remaining Reserved Qty";
                        dgvDetailSO.Columns[11].Name = "NT Reserved";
                        dgvDetailSO.Columns[12].Name = "Unit";
                    }

                    int seqno = 0;
                    if (dgvDetailSO.Rows.Count > 0 || SeqNo != null)
                    {
                        if (SeqNo != null)
                        {
                            seqno = SeqNo.Length;
                            for (int i = 0; i < SeqNo.Length; i++)
                            {
                                if (seqno < SeqNo[i])
                                {
                                    seqno = SeqNo[i];
                                }
                            }
                        }
                        else
                        {
                            seqno = dgvDetailSO.Rows.Count;
                        }
                        if (dgvDetailSO.Rows.Count > 0)
                        {
                            for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
                            {
                                if (seqno < Convert.ToInt32(dgvDetailSO.Rows[i].Cells["SeqNo"].Value))
                                {
                                    seqno = Convert.ToInt32(dgvDetailSO.Rows[i].Cells["SeqNo"].Value);
                                }
                            }
                        }
                    }

                    for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                    {
                        decimal Qty = 0;
                        decimal QtyTotal = 0;
                        string itemname = "";
                        Query = "SELECT TOP 1 [Lock_Qty] FROM [dbo].[InventLockTable] WHERE [RefTransId] = '" + Variable.Kode2[i, 1] + "' AND [FullItemId]='" + Variable.Kode2[i, 0] + "' AND [SiteId]='" + txtInventSiteIDFrom.Text + "' AND (RefTrans2Id IS NULL OR RefTrans2Id='')";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Qty = Convert.ToDecimal(Cmd.ExecuteScalar());
                        }
                        Query = "SELECT [ItemDeskripsi] FROM [dbo].[InventTable] WHERE [FullItemId]='" + Variable.Kode2[i, 0] + "' ";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            if (Cmd.ExecuteScalar() != null)
                            {
                                itemname = Cmd.ExecuteScalar().ToString();
                            }
                            else
                            {
                                MessageBox.Show("Daftar Item dengan Full Item ID = "+Variable.Kode2[i,0].ToString()+" tidak ada pada Invent Table.");
                            }
                        }
                        if (txtBoxRefType.Text.ToUpper() == "SALES ORDER" || Mode.ToUpper() == "NEW")
                        {
                            Query = "SELECT Qty FROM [dbo].[SalesOrderD] WHERE [SalesOrderNo] = '" + Variable.Kode2[i, 1] + "' AND [FullItemID]='" + Variable.Kode2[i, 0] + "' AND [SeqNo]=" + Variable.Kode2[i, 2] + " ";
                            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            {
                                QtyTotal = Convert.ToDecimal(Cmd.ExecuteScalar());
                            }
                        }
                        else if (txtBoxRefType.Text.ToUpper() == "DELIVERY ORDER")
                        {
                            Query = "SELECT Qty FROM [dbo].[DeliveryOrderD] WHERE [DeliveryOrderId] = '" + Variable.Kode2[i, 1] + "' AND [FullItemID]='" + Variable.Kode2[i, 0] + "' AND [SeqNo]=" + Variable.Kode2[i, 2] + " ";
                            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            {
                                QtyTotal = Convert.ToDecimal(Cmd.ExecuteScalar());
                            }
                        }
                        dgvDetailSO.Rows.Add(dgvDetailSO.Rows.Count + 1, txtTransferNo.Text, 0, (seqno + 1 + i), Variable.Kode2[i, 0], itemname, Variable.Kode2[i, 1], Variable.Kode2[i, 2], QtyTotal, Qty, 0.00, 0.00, Variable.Kode2[i, 4]);

                    }

                    dgvDetailSO.AutoResizeColumns();
                    Variable.Kode2 = null;

                    dgvDetailSO.ReadOnly = false;
                    string[] read2 = new string[] { "No", "TransferNo", "Transfer_SeqNo", "FullItemId", "ItemName", "SO Id", "SO Seq No", "SO Qty", "Unit", "Remaining Reserved Qty" };
                    for (int i = 0; i < read2.Length; i++)
                    {
                        dgvDetailSO.Columns[read2[i]].ReadOnly = true;
                    }

                    string[] visible2 = new string[] { "SeqNo" };
                    for (int i = 0; i < visible2.Length; i++)
                    {
                        dgvDetailSO.Columns[visible2[i]].Visible = false;
                    }
                    string[] color2 = new string[] { "No", "TransferNo", "Transfer_SeqNo", "FullItemId", "ItemName", "SO Id", "SO Seq No", "SO Reserved", "SO Qty", "Unit", "Remaining Reserved Qty" };
                    for (int i = 0; i < color2.Length; i++)
                    {
                        dgvDetailSO.Columns[color2[i]].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
            }
            else if (Table == "[dbo].[VendTable]")
            {
                txtBoxVehicleOwnerId.Text = Variable.Kode2[0, 0];
                txtBoxVehicleOwnerName.Text = Variable.Kode2[0,1];
                Variable.Kode2 = null;
            }
        }

        private void updateSeqNo(SqlTransaction Trans, int i)
        {
            Query = "UPDATE [dbo].[NotaTransferD] SET [SeqNo] =" + (i + 1) + " WHERE [TransferNo] ='" + txtTransferNo.Text + "' AND [FullItemId] ='" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "'";
            using (Cmd = new SqlCommand(Query, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvNotaTransfer.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvNotaTransfer.Rows[i].Cells["No"].Value) == j)
                    {
                        goto Outer;
                    }
                }
                return j;
            Outer:
                continue;
            }
            return 1000000;
        }

        public void AddDataGridGelombang(List<string> FullItemID, List<string> GroupId, List<string> SubGroup1Id, List<string> SubGroup2Id, List<string> ItemId, List<string> ItemDeskripsi, List<string> UoM, List<string> UoMAlt, List<string> Ratio)
        {

            int SeqNoGroup = CheckSeqNoGroup();

            if (dgvNotaTransfer.RowCount - 1 <= 0)
            {
                dgvNotaTransfer.ColumnCount = 14;
                dgvNotaTransfer.Columns[0].Name = "No";
                dgvNotaTransfer.Columns[1].Name = "FullItemID";
                dgvNotaTransfer.Columns[2].Name = "ItemName";
                dgvNotaTransfer.Columns[3].Name = "QtyUoM";
                dgvNotaTransfer.Columns[4].Name = "UoM";
                dgvNotaTransfer.Columns[5].Name = "QtyAlt";
                dgvNotaTransfer.Columns[6].Name = "UoMAlt";
                dgvNotaTransfer.Columns[7].Name = "Ratio";
                dgvNotaTransfer.Columns[8].Name = "Blok";
                dgvNotaTransfer.Columns[9].Name = "Notes";
                dgvNotaTransfer.Columns[10].Name = "GroupId";
                dgvNotaTransfer.Columns[11].Name = "SubGroup1Id";
                dgvNotaTransfer.Columns[12].Name = "SubGroup2Id";
                dgvNotaTransfer.Columns[13].Name = "ItemId";
            }

            for (int i = 0; i < FullItemID.Count; i++)
            {

                this.dgvNotaTransfer.Rows.Add((dgvNotaTransfer.RowCount + 1).ToString(), FullItemID[i], ItemDeskripsi[i], "0", UoM[i], "0", UoMAlt[i], Ratio[i], "", "", GroupId[i], SubGroup1Id[i], SubGroup2Id[i], ItemId[i]);

            }


            dgvNotaTransfer.ReadOnly = false;
            dgvNotaTransfer.Columns["No"].ReadOnly = true;
            dgvNotaTransfer.Columns["FullItemID"].ReadOnly = true;
            dgvNotaTransfer.Columns["ItemName"].ReadOnly = true;
            dgvNotaTransfer.Columns["QtyUoM"].ReadOnly = false;
            dgvNotaTransfer.Columns["UoM"].ReadOnly = true;
            dgvNotaTransfer.Columns["QtyAlt"].ReadOnly = false;
            dgvNotaTransfer.Columns["UoMAlt"].ReadOnly = true;
            dgvNotaTransfer.Columns["Ratio"].ReadOnly = true;
            dgvNotaTransfer.Columns["Blok"].ReadOnly = false;
            dgvNotaTransfer.Columns["Notes"].ReadOnly = false;

            dgvNotaTransfer.Columns["GroupId"].Visible = false;
            dgvNotaTransfer.Columns["SubGroup1Id"].Visible = false;
            dgvNotaTransfer.Columns["SubGroup2Id"].Visible = false;
            dgvNotaTransfer.Columns["ItemId"].Visible = false;

            dgvNotaTransfer.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["QtyUoM"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["UoM"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["QtyAlt"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["UoMAlt"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["Blok"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvNotaTransfer.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvNotaTransfer.AutoResizeColumns();
            dgvNotaTransfer.Refresh();
        }

        private void btnSearchFrom_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSite";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode.ToString() == txtInventSiteIDTo.Text && txtInventSiteIDTo.Text.Trim() != "")
            {
                MessageBox.Show("Site Id usul tidak boleh sama dengan site Id Tujuan.");
            }
            else
            {
                txtInventSiteIDFrom.Text = ConnectionString.Kode;
                txtWarehouseFrom.Text = ConnectionString.Kode2;
                dgvNotaTransfer.Columns.Clear();
                dgvDetailSO.Columns.Clear();
            }
        }

        private void btnSearchTo_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSite";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode.ToString() == txtInventSiteIDFrom.Text && txtInventSiteIDFrom.Text.Trim() != "")
            {
                MessageBox.Show("Site Id tujuan tidak boleh sama dengan site Id usul.");
            }
            else
            {
                txtInventSiteIDTo.Text = ConnectionString.Kode;
                txtWarehouseTo.Text = ConnectionString.Kode2;
            }
        }

        private void dgvNotaTransfer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvNotaTransfer.Columns[dgvNotaTransfer.CurrentCell.ColumnIndex].Name == "Transfer Qty")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                //hendry end

            }
        }

        private void dgvNotaTransfer_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvNotaTransfer_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvNotaTransfer_KeyPress);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (metroTabControl1.SelectedIndex == 0)
            {
                if (dgvNotaTransfer.RowCount > 0)
                    if (dgvNotaTransfer.RowCount > 0)
                    {
                        Index = dgvNotaTransfer.CurrentRow.Index;
                        DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvNotaTransfer.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvNotaTransfer.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvNotaTransfer.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            for (int i = (dgvDetailSO.Rows.Count - 1); i >= 0; i--)
                            {
                                if (dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() == dgvNotaTransfer.Rows[Index].Cells["FullItemId"].Value.ToString())
                                {
                                    dgvDetailSO.Rows.RemoveAt(i);
                                }
                            }
                            dgvNotaTransfer.Rows.RemoveAt(Index);
                            SortNoDataGrid();
                        }
                        //GetGelombang();
                    }
            }
            else if (metroTabControl1.SelectedIndex == 1)
            {
                if (dgvDetailSO.RowCount > 0)
                {
                    Index = dgvDetailSO.CurrentRow.Index;
                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvDetailSO.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvDetailSO.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvDetailSO.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        dgvDetailSO.Rows.RemoveAt(Index);
                        SortNoDataGrid();
                    }
                }
            }
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvNotaTransfer.RowCount; i++)
            {
                dgvNotaTransfer.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) <= 0)
            {
                MessageBox.Show("Access Denied.");
                return;
            }
            if (txtTransStatus.Text == "02")
            {
                MessageBox.Show("Data sudah diapprove.");
                return;
            }
            else if (txtTransStatus.Text != "01")
            {
                MessageBox.Show("Data Sudah Diproses.");
                return;
            }
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                Query = "Update NotaTransferH set ";
                Query += "[TransStatus]='02',";
                Query += "[Notes]=@DriverName,";
                Query += "UpdatedDate=getdate(),";
                Query += "UpdatedBy='' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where TransferNo='" + txtTransferNo.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.AddWithValue("@DriverName",txtDriverName.Text.Trim());
                Cmd.ExecuteNonQuery();

                if (Query != "")
                {
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@DriverName", txtDriverName.Text.Trim());
                    Cmd.ExecuteNonQuery();
                    Query = "";
                }

                string action = "";
                Query = "SELECT TOP 1 [Action] FROM [dbo].[NotaTransfer_LogTable] WHERE [NTId] = @NTId ORDER BY [LogDate] DESC";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Cmd.Parameters.AddWithValue("@NTId", txtTransferNo.Text);
                    action = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                }
                InsertNTLog(txtTransferNo.Text, "02", "Approved by Stock Manager", Trans, action);

                CreateJournal(Trans);

                Trans.Commit();
                MessageBox.Show("Data TransferNumber : " + txtTransferNo.Text + " berhasil diapprove.");
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
                Parent.RefreshGrid();
                GetDataHeader();
                ModeApprove();
                //this.Close();
            }
        }

        private void btnSearchRef_Click(object sender, EventArgs e)
        {
            string Table = "[dbo].[VendTable]";
            string QuerySearch = "SELECT [VendId], [VendName] FROM [dbo].[VendTable] WHERE [Gol_Prsh] = 'EXPEDISI'";
            string[] FilterText = { "VendId", "VendName" };
            string[] Select = { "VendId", "VendName" };
            string PrimaryKey = "VendId";
            string[] HideField = { "Check" };
            callSearchQueryV2Form(Table, QuerySearch, FilterText, Select, PrimaryKey, HideField);
        }

        private void dgvNotaTransfer_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvNotaTransfer.Columns[e.ColumnIndex].Name == "ReferenceId")
                {
                    string Table = "[dbo].[InventLockTable]";
                    string QuerySearch = "SELECT RefTransType,RefTransId,[RefTrans_SeqNo],FullItemId,SiteId,SUM(Lock_Qty) as Lock_Qty,Unit FROM [dbo].[InventLockTable]  WHERE FullItemId='" + dgvNotaTransfer.Rows[dgvNotaTransfer.CurrentRow.Index].Cells["FullItemId"].Value.ToString() + "' AND SiteId ='" + txtInventSiteIDFrom.Text + "'  ";
                    for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
                    {
                        if (dgvNotaTransfer.Rows[i].Cells["ReferenceId"].Value.ToString() != null && dgvNotaTransfer.Rows[i].Cells["ReferenceId"].Value.ToString().Trim() != "")
                        {
                            QuerySearch += " AND NOT (RefTransId = '" + dgvNotaTransfer.Rows[i].Cells["ReferenceId"].Value.ToString() + "' AND [FullItemId]='" + dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString() + "') ";
                        }
                    }
                    QuerySearch += " GROUP BY  [SiteId],[FullItemId],[RefTransId],[Unit],[RefTransType],[RefTrans_SeqNo] HAVING SUM(Lock_Qty) != 0 ";

                    string[] FilterText = { "RefTransId", "FullItemId", "Lock_Qty", "Unit", "RefTransType" };
                    string[] Select = { "RefTransId", "FullItemId", "Lock_Qty", "Unit", "RefTransType", "RefTrans_SeqNo" };
                    string PrimaryKey = "RefTransId";
                    string[] HideField = { "Check", "RefTrans_SeqNo" };
                    callSearchQueryV2Form(Table, QuerySearch, FilterText, Select, PrimaryKey, HideField);
                    //dgvNotaTransfer.Columns.Clear(); ;
                }
            }
        }

        private void dgvNotaTransfer_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNotaTransfer.Rows[dgvNotaTransfer.CurrentRow.Index].Cells["Transfer Qty"].Value != null || dgvNotaTransfer.Rows[dgvNotaTransfer.CurrentRow.Index].Cells["Transfer Qty"].Value != "")
            {
                if (Convert.ToDecimal(dgvNotaTransfer.Rows[dgvNotaTransfer.CurrentRow.Index].Cells["Transfer Qty"].Value) > Convert.ToDecimal(dgvNotaTransfer.Rows[dgvNotaTransfer.CurrentRow.Index].Cells["Available"].Value))
                {
                    MessageBox.Show("Transfer tidak boleh melebihi stock Available.");
                    dgvNotaTransfer.Rows[dgvNotaTransfer.CurrentRow.Index].Cells["Transfer Qty"].Value = 0.00;
                }
            }
        }

        private void dgvNotaTransfer_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvNotaTransfer.Rows.Count > 0)
            {
                for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
                {
                    dgvNotaTransfer.Rows[i].Cells["No"].Value = i + 1;
                    decimal sum = 0;
                    for (int x = 0; x < dgvDetailSO.Rows.Count; x++)
                    {
                        if (dgvDetailSO.Rows[x].Cells["FullItemId"].Value.ToString() == dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString())
                        {
                            sum += Convert.ToDecimal(dgvDetailSO.Rows[x].Cells["NT Reserved"].Value);
                        }
                    }
                    dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value = sum;
                }
            }

        }

        private void dgvDetailSO_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
            {
                for (int x = 0; x < dgvNotaTransfer.Rows.Count; x++)
                {
                    if (dgvNotaTransfer.Rows[x].Cells["FullItemId"].Value.ToString() == dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString())
                    {
                        dgvDetailSO.Rows[i].Cells["Transfer_SeqNo"].Value = dgvNotaTransfer.Rows[x].Cells["SeqNo"].Value;
                        break;
                    }
                }
            }

        }

        private void dgvDetailSO_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
            {
                Query = "SELECT SUM(Lock_Qty) FROM [dbo].[InventLockTable] ";
                Query += " WHERE  SiteId ='" + txtInventSiteIDFrom.Text + "' AND FullItemId = '" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [RefTransId] = '" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "' AND [RefTrans_SeqNo] = " + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value + " ";
                Query += " GROUP BY  [SiteId],[FullItemId],[RefTransId],[Unit],[RefTransType],[RefTrans_SeqNo] ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    dgvDetailSO.Rows[i].Cells["Remaining Reserved Qty"].Value = (Cmd.ExecuteScalar());
                }
            }
            if (dgvNotaTransfer.Rows.Count > 0)
            {
                for (int i = 0; i < dgvNotaTransfer.Rows.Count; i++)
                {
                    dgvNotaTransfer.Rows[i].Cells["No"].Value = i + 1;
                    decimal sum = 0;
                    for (int x = 0; x < dgvDetailSO.Rows.Count; x++)
                    {
                        if (dgvDetailSO.Rows[x].Cells["FullItemId"].Value.ToString() == dgvNotaTransfer.Rows[i].Cells["FullItemId"].Value.ToString())
                        {
                            sum += Convert.ToDecimal(dgvDetailSO.Rows[x].Cells["NT Reserved"].Value);
                        }
                    }
                    dgvNotaTransfer.Rows[i].Cells["Transfer Qty Reserved"].Value = sum;
                }
            }

            decimal resqty = 0;
            Query = "SELECT [NT_Qty] FROM [dbo].[NotaTransfer_SO_List] WHERE [TransferNo] = '" + txtTransferNo.Text + "' AND [SOId]='" + dgvDetailSO.Rows[dgvDetailSO.CurrentRow.Index].Cells["SO Id"].Value.ToString() + "' AND [SO_SeqNo]=" + dgvDetailSO.Rows[dgvDetailSO.CurrentRow.Index].Cells["SO Seq No"].Value + " ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                resqty = Convert.ToDecimal(Cmd.ExecuteScalar());
            }
            if (Convert.ToDecimal(dgvDetailSO.Rows[dgvDetailSO.CurrentRow.Index].Cells["NT Reserved"].Value) > (Convert.ToDecimal(dgvDetailSO.Rows[dgvDetailSO.CurrentRow.Index].Cells["Remaining Reserved Qty"].Value) + resqty))
            {
                MessageBox.Show("Transfer tidak boleh melebihi stock Reserved.");
                dgvDetailSO.Rows[dgvDetailSO.CurrentRow.Index].Cells["NT Reserved"].Value = resqty;
            }
        }

        private void dgvDetailSO_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < dgvDetailSO.Rows.Count; i++)
            {
                Query = "SELECT SUM(Lock_Qty) FROM [dbo].[InventLockTable] ";
                Query += " WHERE  SiteId ='" + txtInventSiteIDFrom.Text + "' AND FullItemId = '" + dgvDetailSO.Rows[i].Cells["FullItemId"].Value.ToString() + "' AND [RefTransId] = '" + dgvDetailSO.Rows[i].Cells["SO Id"].Value.ToString() + "' AND [RefTrans_SeqNo] = " + dgvDetailSO.Rows[i].Cells["SO Seq No"].Value + " ";
                Query += " GROUP BY  [SiteId],[FullItemId],[RefTransId],[Unit],[RefTransType],[RefTrans_SeqNo] ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    dgvDetailSO.Rows[i].Cells["Remaining Reserved Qty"].Value = (Cmd.ExecuteScalar());
                }
            }
        }

        private void dgvDetailSO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvDetailSO.Columns[dgvDetailSO.CurrentCell.ColumnIndex].Name == "NT Reserved")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
            }
        }

        private void dgvDetailSO_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvDetailSO_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvDetailSO_KeyPress);
            }
        }

        private void btnUnapprove_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) <= 0)
            {
                MessageBox.Show("Access Denied.");
                return;
            }
            if (txtTransStatus.Text != "02")
            {
                MessageBox.Show("Data sudah tidak bisa di-unapprove.");
                return;
            }
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                Query = "Update NotaTransferH set ";
                Query += "[TransStatus]='01',";
                Query += "[Notes]=@DriverName,";
                Query += "UpdatedDate=getdate(),";
                Query += "UpdatedBy='' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where TransferNo='" + txtTransferNo.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.AddWithValue("@DriverName",txtDriverName.Text.Trim());
                Cmd.ExecuteNonQuery();

                if (Query != "")
                {
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@DriverName", txtDriverName.Text.Trim());
                    Cmd.ExecuteNonQuery();
                    Query = "";
                }

                string action = "N";
                Query = "SELECT TOP 1 [Action] FROM [dbo].[NotaTransfer_LogTable] WHERE [NTId] = @NTId ORDER BY [LogDate] DESC";
                using (Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Cmd.Parameters.AddWithValue("@NTId", txtTransferNo.Text);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            action = Dr["Action"] == System.DBNull.Value ? "" : Dr["Action"].ToString();
                        }
                    }
                    Dr.Close();
                }
                InsertNTLog(txtTransferNo.Text, "01", "Waiting for Approval", Trans, action);

                CreateJournalNotApproval(Trans);

                Trans.Commit();
                MessageBox.Show("Data TransferNumber : " + txtTransferNo.Text + " berhasil diunapprove.");
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
                Parent.RefreshGrid();
                GetDataHeader();
                ModeApprove();
                //this.Close();
            }
        }

    }
}
