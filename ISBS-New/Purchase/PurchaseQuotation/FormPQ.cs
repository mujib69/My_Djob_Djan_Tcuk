using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace ISBS_New.Purchase.PurchaseQuotation
{
    public partial class FormPQ : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        PopUp.FullItemId.FullItemId FullItemId = new PopUp.FullItemId.FullItemId();

        public static string reffID;

        string Mode, Status, Query, crit = null;
        List<string> sSelectedFile, FileName, Extension;

        int Index;
        List<byte[]> test = new List<byte[]>();

        public string PQNumber = "", tmpPrType = "";

        DateTimePicker dtp;
        ComboBox DeliveryMethod;
        DataGridViewComboBoxCell cell;

        List<string> ListSeqNum;
        List<Purchase.PurchaseRequisition.Info> ListInfo = new List<Purchase.PurchaseRequisition.Info>();
        ContextMenu vendid = new ContextMenu();

        Purchase.PurchaseQuotation.InquiryPQ Parent;
        Purchase.RFQ.RFQInquiry ParentRFQ;

        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormPQ()
        {
            InitializeComponent();
        }

        private void refreshGrey()
        {
            for (int i = 0; i < dgvPqDetails1.RowCount; i++)
            {
                if (dgvPqDetails1.Rows[i].Cells["Base"].Value != null)
                    if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        dgvPqDetails1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                        dgvPqDetails1.Rows[i].ReadOnly = true;
                    }

                if (dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value.ToString() == "Select")
                {
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].Style.BackColor = Color.LightGray;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].Style.BackColor = Color.LightGray;
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].ReadOnly = true;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].ReadOnly = true;
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].Value = 0;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].Value = 0;
                }
                if (dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value.ToString() == "Percentage")
                {
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].Style.BackColor = Color.White;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].Style.BackColor = Color.LightGray;
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].ReadOnly = false;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].ReadOnly = true;
                }
                if (dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value.ToString() == "Amount")
                {
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].Style.BackColor = Color.LightGray;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].Style.BackColor = Color.White;
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].ReadOnly = true;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].ReadOnly = false;
                }
                if (dgvPqDetails1.ReadOnly == true || Mode == "BeforeEdit")
                {
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].Style.BackColor = Color.LightGray;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].Style.BackColor = Color.LightGray;
                }
            }

        }

        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvPqDetails1.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvPqDetails1.Rows[i].Cells["SeqNoGroup"].Value) == j)
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

        protected void txtExchRate_SetText()
        {
            this.txtExchRate.Text = "1";
            txtExchRate.ForeColor = Color.Gray;
        }

        private void FormPQ_Load(object sender, EventArgs e)
        {
            cmbPPh_Load();
            cmbPPn_Load();
            AddCmbCurrency();
            AddCmbPaymentMode();
            AddCmbTermOfPayment();
            AddCmbDPType();

            GetDataHeader();

            if (Mode == "New")
            {
                ModeNew();
                txtExchRate_SetText();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd-MM-yyyy";
            dtp.Visible = false;
            dtp.Width = 100;

            dgvPqDetails1.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dgvPqDetails1.CellBeginEdit += this.dgvPrDetails_CellBeginEdit;
            dgvPqDetails1.CellEndEdit += this.dgvPrDetails_CellEndEdit;

            DeliveryMethod = new ComboBox();
            DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            DeliveryMethod.Visible = false;
            dgvPqDetails1.Controls.Add(DeliveryMethod);

            DeliveryMethod.DropDownClosed += this.DeliveryMethod_DropDownClosed;
            DeliveryMethod.SelectionChangeCommitted += this.DeliveryMethod_SelectionChangeCommitted;

            //DeliveryMethod = new ComboBox();
            //DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            //DeliveryMethod.Visible = false;
            //dgvPqDetails1.Controls.Add(DeliveryMethod);

            //DeliveryMethod.DropDownClosed += this.DeliveryMethod_DropDownClosed;
            //DeliveryMethod.SelectionChangeCommitted += this.DeliveryMethod_SelectionChangeCommitted;

            txtExchRate.TextAlign = HorizontalAlignment.Right;

            this.txtExchRate.Enter += new EventHandler(txtExchRate_Enter);
            this.txtExchRate.Leave += new EventHandler(txtExchRate_Leave);
        }

        private void txtExchRate_Enter(object sender, EventArgs e)
        {
            if (txtExchRate.ForeColor != Color.Black)
                txtExchRate.ForeColor = Color.Black;
        }

        private void txtExchRate_Leave(object sender, EventArgs e)
        {
            if (txtExchRate.Text.Trim() == "")
                txtExchRate_SetText();
        }

        public void SetMode(string tmpMode, string tmpPQNumber)
        {
            Mode = tmpMode;
            txtPqNumber.Text = tmpPQNumber;
        }

        private void btnEditH_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                try
                {
                    if (ControlMgr.GroupName == "Purchase Manager")
                    {
                        
                    }
                    else
                    {
                        string strSql = "SELECT * FROM PurchQuotationH WHERE PurchQuotID IN(SELECT PurchQuotID FROM CanvasSheetD) AND PurchQuotID='" + txtPqNumber.Text + "'";
                        Cmd = new SqlCommand(strSql, Conn);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            MessageBox.Show("PurchQuotID = " + txtPqNumber.Text + ".\n" + "Tidak bisa diedit karena sudah dibuat Canvass Sheet..");
                            return;
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Conn.Close();
                }

                Mode = "Edit";

                readDB = false;

                btnNew.Enabled = true;
                btnDelete.Enabled = true;
                btnUpload.Enabled = true;
                btnDownload.Enabled = true;
                btnDelAttachment.Enabled = true;
                btnSave.Visible = true;
                btnExit.Visible = false;
                btnEdit.Visible = false;
                btnCancel.Visible = true;
                cbByPhone.Enabled = true;

                dtPqDate.Enabled = false;
                txtVendorPqNumber.ReadOnly = true;

                if (ControlMgr.GroupName == "Purchase Manager")
                {
                    ModeBeforeEdit();
                    dtValidTo.Enabled = true;
                    btnSave.Visible = true;
                    btnExit.Visible = false;
                    btnEdit.Visible = false;
                    btnCancel.Visible = true;

                    btnUpload.Enabled = true;
                    btnDownload.Enabled = true;
                    btnDelAttachment.Enabled = true;
                }
                else
                    ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void SelectDgvPR()
        {
            if (txtPRId.Text != "")
            {
                dgvPR.Rows.Clear();
                dgvPR.ColumnCount = 11;
                dgvPR.Columns[0].Name = "No";
                dgvPR.Columns[1].Name = "FullItemId";
                dgvPR.Columns[2].Name = "ItemName";
                dgvPR.Columns[3].Name = "Base";
                dgvPR.Columns[4].Name = "Qty";
                dgvPR.Columns[5].Name = "Amount";
                dgvPR.Columns[6].Name = "Unit";
                dgvPR.Columns[7].Name = "DeliveryMethod";
                dgvPR.Columns[8].Name = "ExpectedDateFrom";
                dgvPR.Columns[9].Name = "ExpectedDateTo";
                dgvPR.Columns[10].Name = "Notes";

                Conn = ConnectionString.GetConnection();
                Query = "Select ROW_NUMBER() OVER (ORDER BY PurchReqId) No, FullItemId, ItemName, Base, Qty, Amount, Unit, DeliveryMethod, ExpectedDateFrom, ExpectedDateTo, Deskripsi [Notes] From [PurchRequisition_Dtl] a ";
                Query += "Where PurchReqId='" + txtPRId.Text + "' ;";

                //Da = new SqlDataAdapter(Query, Conn);
                //Dt = new DataTable();
                //Da.Fill(Dt);

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string ExpectedDateFrom = Convert.ToDateTime(Dr["ExpectedDateFrom"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr["ExpectedDateFrom"]).ToString("dd/MM/yyyy");
                    string ExpectedDateTo = Convert.ToDateTime(Dr["ExpectedDateTo"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr["ExpectedDateTo"]).ToString("dd/MM/yyyy");

                    this.dgvPR.Rows.Add(
                        Dr["No"],
                        Dr["FullItemId"],
                        Dr["ItemName"],
                        Dr["Base"],
                        Dr["Qty"],
                        Dr["Amount"],
                        Dr["Unit"],
                        Dr["DeliveryMethod"],
                        ExpectedDateFrom,//Dr["ExpectedDateFrom"],
                        ExpectedDateTo,//Dr["ExpectedDateTo"],
                        Dr["Notes"]);
                }

                dgvPR.AutoGenerateColumns = true;
                dgvPR.DataSource = Dt;
                dgvPR.Refresh();
                dgvPR.AutoResizeColumns();
                dgvPR.DefaultCellStyle.BackColor = Color.LightGray;

                if (txtTransType.Text == "FIX")
                {
                    dgvPR.Columns["Base"].Visible = false;
                    dgvPR.Columns["Amount"].Visible = false;
                    dgvPR.Columns["Qty"].Visible = true;
                }
                else if (txtTransType.Text == "QTY")
                {
                    dgvPR.Columns["Base"].Visible = true;
                    dgvPR.Columns["Amount"].Visible = false;
                    dgvPR.Columns["Qty"].Visible = true;
                }
                else if (txtTransType.Text == "AMOUNT")
                {

                    dgvPR.Columns["Base"].Visible = true;
                    dgvPR.Columns["Amount"].Visible = true;
                    dgvPR.Columns["Qty"].Visible = false;
                }
            }
        }

        private void dgvPrDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "DeliveryMethod")
                e.Handled = true;

            if (dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Price" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Ratio" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Qty Vendor" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Disc. Type" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "CashBack Amount" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Bonus Amount" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "SubTotal" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "SubTotal_PPN" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "SubTotal_PPH")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                    e.Handled = true;
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    e.Handled = true;
            }
        }

        private void CallFormGelombang(List<string> InvStockId)
        {
            if (InvStockId != null)
            {
                if (InvStockId.Count != 0)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "Select count([GelombangId]) From [InventGelombang] Where GelombangId in (Select GelombangId from InventGelombang where ItemId in (";
                    for (var i = 0; i < InvStockId.Count; i++)
                    {
                        if (i == 0)
                        {
                            Query += "'" + InvStockId[i] + "'";
                        }
                        else
                        {
                            Query += ",'" + InvStockId[i] + "'";
                        }
                    }
                    Query += "))";

                    Cmd = new SqlCommand(Query, Conn);
                    int CountChk = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (CountChk > 0)
                    {

                    }
                }
            }
        }

        public string DetailHeaderItem()
        {
            string InvStockId = "";

            if (dgvPqDetails1.RowCount > 0)
            {
                for (int i = 0; i <= dgvPqDetails1.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        InvStockId += "and FullItemId not in ('";
                        InvStockId += dgvPqDetails1.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["FullItemId"].Value.ToString();
                        InvStockId += "'";
                    }
                    else
                    {
                        InvStockId += ",'";
                        InvStockId += dgvPqDetails1.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["FullItemId"].Value.ToString();
                        InvStockId += "'";
                    }
                }
                InvStockId += ")";
                return InvStockId;
            }
            else
            {
                InvStockId = "";
                return InvStockId;
            }
        }


        public void AddDataGridDetail(List<string> PurchReqID, List<string> SeqNoGroup)
        {
            int TmpSeqNoGroup = CheckSeqNoGroup();
            if (dgvPqDetails1.RowCount - 1 <= 0)
            {
                string[] ListColumn = new string[] { };
                GenerateColumnDataGridView(ref ListColumn);
            }

            for (int i = 0; i < SeqNoGroup.Count; i++)
            {
                if (i > 0)
                {
                    if (Convert.ToInt32(SeqNoGroup[i]) > Convert.ToInt32(SeqNoGroup[i - 1]))
                    {
                        TmpSeqNoGroup++;
                    }
                }

                Conn = ConnectionString.GetConnection();

                //if (txtTransType.Text != "AMOUNT")
                //    Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, [Qty], [Unit],Ratio, Deskripsi,b.PurchReqID,PurchReqSeqNo,b.VendId,b.VendName,b.TransType, GelombangID, BracketId, BracketDesc, Base From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId Where a.RfqID='" + txtRfqNumber.Text.Trim() + "' and a.PurchReqId='" + PurchReqID[i] + "' and SeqNoGroup='" + SeqNoGroup[i] + "' order by RfqSeqNo asc";
                //else if (txtTransType.Text == "AMOUNT")
                //    Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, [Amount], [Unit],Ratio, Deskripsi,b.PurchReqID,PurchReqSeqNo,b.VendId,b.VendName,b.TransType, GelombangID, BracketId, BracketDesc, Base From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId Where a.RfqID='" + txtRfqNumber.Text.Trim() + "' and a.PurchReqId='" + PurchReqID[i] + "' and SeqNoGroup='" + SeqNoGroup[i] + "' order by RfqSeqNo asc";

                Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, Amount, [Qty], [Unit], CASE WHEN Base = 'Y' THEN Price ELSE Price + ";
                Query += "(SELECT c.Price FROM [RequestForQuotationD] c WHERE c.RfqId = a.RfqId AND c.SeqNoGroup = a.SeqNoGroup AND c.Base = 'Y') END AS Price, Ratio, Deskripsi,b.PurchReqID,PurchReqSeqNo,b.VendId,b.VendName,b.TransType, a.DeliveryMethod, GelombangID,BracketID,BracketDesc,Base,SeqNoGroup From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId Where a.RfqId = '" + txtRfqNumber.Text + "' and a.PurchReqId='" + PurchReqID[i] + "' and SeqNoGroup='" + SeqNoGroup[i] + "' order by RfqSeqNo asc";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (txtTransType.Text != "AMOUNT")
                        this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Base"], Dr["Qty"], Dr["Qty"], Dr["Unit"], Dr["Ratio"], Dr["Price"], "", Dr["DeliveryMethod"], "", "0.0000", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "0", "0", "0", "0", "0", "0", "0", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], "");
                    else if (txtTransType.Text == "AMOUNT")
                        this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Base"], Dr["Amount"], Dr["Amount"], Dr["Unit"], Dr["Ratio"], Dr["Price"], "", Dr["DeliveryMethod"], "", "0.0000", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "0", "0", "0", "0", "0", "0", "0", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], "");
                    //if (txtTransType.Text != "AMOUNT")
                    //    this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Qty"], Dr["Unit"], Dr["Ratio"], "", "", "", "", "", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "", "", "", "0", "0", "", "", "", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["Base"], TmpSeqNoGroup);
                    //if (txtTransType.Text == "AMOUNT")
                    //    this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Amount"], Dr["Unit"], Dr["Ratio"], Dr["Amount"], "", "", "", "", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "", "", "", "0", "0", "", "", "", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["Base"], TmpSeqNoGroup);

                    cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                    cell.Value = "Select";
                    dgvPqDetails1.Rows[(dgvPqDetails1.Rows.Count - 1)].Cells["Disc. Type"] = cell;

                }
                Dr.Close();
            }
            dgvPqDetails1.ReadOnly = true;

            dgvPqDetails1.ReadOnly = false;
            dgvPqDetails1.Columns["No"].ReadOnly = true;
            dgvPqDetails1.Columns["FullItemID"].ReadOnly = true;
            dgvPqDetails1.Columns["ItemName"].ReadOnly = true;
            if (dgvPqDetails1.Columns.Contains("Qty"))
                dgvPqDetails1.Columns["Qty"].ReadOnly = true;
            else if (dgvPqDetails1.Columns.Contains("Amount"))
                dgvPqDetails1.Columns["Amount"].ReadOnly = true; ;
            dgvPqDetails1.Columns["PR ID"].ReadOnly = true;
            dgvPqDetails1.Columns["Unit"].ReadOnly = true;
            dgvPqDetails1.Columns["Ratio"].ReadOnly = true;
            dgvPqDetails1.Columns["SubTotal"].ReadOnly = true;
            dgvPqDetails1.Columns["SubTotal_PPN"].ReadOnly = true;
            dgvPqDetails1.Columns["SubTotal_PPH"].ReadOnly = true;
            dgvPqDetails1.Columns["GelombangID"].ReadOnly = true;
            dgvPqDetails1.Columns["BracketID"].ReadOnly = true;
            dgvPqDetails1.Columns["BracketDesc"].ReadOnly = true;
            dgvPqDetails1.Columns["Base"].ReadOnly = true;
            for (int x = 0; x < dgvPqDetails1.RowCount; x++)
            {
                if (dgvPqDetails1.Rows[x].Cells["Base"].Value.ToString() == "N")
                {
                    dgvPqDetails1.Rows[x].Cells["DeliveryMethod"].ReadOnly = true;
                    if (dgvPqDetails1.Columns.Contains("Qty"))
                    {
                        dgvPqDetails1.Rows[x].Cells["Qty"].ReadOnly = true;
                        dgvPqDetails1.Rows[x].Cells["Qty Vendor"].ReadOnly = true;
                    }
                    else if (dgvPqDetails1.Columns.Contains("Amount"))
                    {
                        dgvPqDetails1.Rows[x].Cells["Amount"].ReadOnly = true;
                        dgvPqDetails1.Rows[x].Cells["Amount Vendor"].ReadOnly = true;
                    }
                }
            }

            for (int z = 0; z < dgvPqDetails1.ColumnCount; z++)
            {
                dgvPqDetails1.Columns[z].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            if (dgvPqDetails1.Columns.Contains("Qty"))
            {
                dgvPqDetails1.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Qty Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dgvPqDetails1.Columns.Contains("Amount"))
            {
                dgvPqDetails1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Amount Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvPqDetails1.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal_PPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal_PPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            SetVisibleDatagridView();

            dgvPqDetails1.AutoResizeColumns();

            refreshGrey();

        }

        private void GenerateColumnDataGridView(ref string[] ListColumn)
        {
            if (txtTransType.Text != "AMOUNT")
            {
                ListColumn = new string[] { "No", "GroupId", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "Base", "Qty" 
                    , "Qty Vendor", "Unit", "Ratio", "Price", "Unit2", "DeliveryMethod", "AvailableDate", "Total", "PR ID", "ReffSeqNo", "ReffTransType", "TransStatus"
                    , "Disc. Type", "Disc. %", "Total Discount", "SubTotal" , "SubTotal_PPN", "SubTotal_PPH", "CashBack Amount", "Bonus Amount", "GelombangID"
                    , "BracketID", "BracketDesc" , "SeqNoGroup", "Notes"};
            }
            else if (txtTransType.Text == "AMOUNT")
            {
                ListColumn = new string[] { "No", "GroupId", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", "Base", "Amount" 
                    , "Amount Vendor", "Unit", "Ratio", "Price", "Unit2", "DeliveryMethod", "AvailableDate", "Total", "PR ID", "ReffSeqNo", "ReffTransType", "TransStatus"
                    , "Disc. Type", "Disc. %", "Total Discount", "SubTotal" , "SubTotal_PPN", "SubTotal_PPH", "CashBack Amount", "Bonus Amount", "GelombangID"
                    , "BracketID", "BracketDesc", "SeqNoGroup", "Notes"};
            }

            dgvPqDetails1.ColumnCount = (ListColumn.Length);
            for (int z = 0; z < ListColumn.Length; z++)
            {
                dgvPqDetails1.Columns[z].Name = ListColumn[z].ToString();
            }
        }

        bool readDB = false, EditDM = false;
        public void GetDataHeader()
        {
            if (txtPqNumber.Text != "")
            {
                test.Clear();

                Conn = ConnectionString.GetConnection();
                Query = "Select Top 1 a.PurchReqId,a.[PurchQuotID],a.[RFQId],a.[OrderDate],a.[VendorQuotNo],a.[VendorQuotDate],a.[VendID],a.VendName,a.DeliveryMethod,a.TransStatus,a.ApprovedBy,b.[ReffTransID],a.TermOfPayment,a.PaymentModeId,a.PPN,a.PPH,a.Deskripsi,a.Total,a.Total_PPN,a.Total_PPH,a.TransType, case when a.DP is null then 0 else a.DP end DP, a.CurrencyId, a.ExchRate, case when a.DPAmount is null then 0 else a.DPAmount end DPAmount, a.ValidTo, a.TotalDiscount, a.BonusScheme 'Bonus Amount', a.CashbackScheme 'CashBack Amount', a.DPType From [PurchQuotationH] a left join PurchQuotation_Dtl b on a.PurchQuotID=b.PurchQuotID Where a.PurchQuotID = '" + txtPqNumber.Text + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    readDB = true;
                    dtPqDate.Text = Dr["OrderDate"].ToString();
                    txtPqNumber.Text = Dr["PurchQuotID"].ToString();
                    txtRfqNumber.Text = Dr["RFQId"].ToString();
                    txtPRId.Text = Dr["PurchReqId"].ToString();
                    txtVendorPqNumber.Text = Dr["VendorQuotNo"].ToString();
                    dtVendorPqDate.Text = Dr["VendorQuotDate"].ToString();
                    txtVendorId.Text = Dr["VendID"].ToString();
                    txtVendorName.Text = Dr["VendName"].ToString();
                    cmbTermOfPayment.SelectedItem = Dr["TermOfPayment"].ToString();
                    cmbPPn.SelectedItem = Dr["PPN"].ToString();
                    cmbPPh.SelectedItem = Dr["PPH"].ToString();
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                    txtTotal.Text = Convert.ToDecimal(Dr["Total"]).ToString("N2");
                    txtTotalPPN.Text = Convert.ToDecimal(Dr["Total_PPN"]).ToString("N2");
                    txtTotalPPH.Text = Convert.ToDecimal(Dr["Total_PPH"]).ToString("N2");
                    txtTransType.Text = Dr["TransType"].ToString();
                    //txtDPAmount.Text = Convert.ToDecimal(Dr["DP"]).ToString("N2");
                    txtExchRate.Text = Convert.ToDecimal(Dr["ExchRate"]).ToString("N2");
                    cmbCurrency.Text = Convert.ToString(Dr["CurrencyId"]);
                    //txtDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                    dtValidTo.Text = Dr["ValidTo"].ToString();
                    cmbPaymentMode.SelectedItem = Dr["PaymentModeId"].ToString();
                    txtDPPercent.Text = Dr["DP"].ToString();
                    txtDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                    //if (Dr["DPType"].ToString() == "Amount")
                    //{
                    //    txtDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                    //    txtDPPercent.Text = Dr["DP"].ToString();
                    //}
                    //else
                    //{

                    //}
                    if (Convert.ToDecimal(txtDPPercent.Text) == 0 && Convert.ToDecimal(txtDPAmount.Text) == 0)
                        cmbDPRequired.SelectedIndex = 0;
                    else
                        cmbDPRequired.SelectedIndex = 1;
                    cmbDPType.SelectedItem = Dr["DPType"].ToString();

                    txtDiscount.Text = Convert.ToDecimal(Dr["TotalDiscount"].ToString() == "" ? "0.00" : Dr["TotalDiscount"].ToString()).ToString("N2");
                    txtBonusScheme.Text = Convert.ToDecimal(Dr["Bonus Amount"]).ToString("N2");
                    txtCashBackScheme.Text = Convert.ToDecimal(Dr["CashBack Amount"]).ToString("N2");
                    txtGrandTotal.Text = (Convert.ToDecimal(Dr["Total"]) - Convert.ToDecimal(Dr["TotalDiscount"]) + Convert.ToDecimal(Dr["Total_PPN"]) + Convert.ToDecimal(Dr["Total_PPH"])).ToString("N2");
                    //edited by Thaddaeus 22JUNE2018,EBGIN
                    //if (Dr["TotalDiscount"] == System.DBNull.Value)
                    //{
                    //    txtGrandTotal.Text = (Convert.ToDecimal(Dr["Total"])).ToString("N4");
                    //}
                    //else
                    //{
                    //    txtGrandTotal.Text = (Convert.ToDecimal(Dr["Total"]) - Convert.ToDecimal(Dr["TotalDiscount"])).ToString("N4");
                    //}
                    //END=============================
                }

                dgvPqDetails1.Rows.Clear();

                if (dgvPqDetails1.RowCount - 1 <= 0)
                {
                    string[] ListColumn = new string[] { };
                    GenerateColumnDataGridView(ref ListColumn);
                }

                if (txtTransType.Text != "AMOUNT")
                    Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, [Qty], [Unit], Ratio, [Price], [Qty2], [Unit2], [AvailableDate], [DeliveryMethod], case when Total is null then 0 else Total end Total, ReffTransId, ReffSeqNo, ReffTransType, TransStatus, Deskripsi, DiscType 'Disc. Type', case when DiscPercent is null then 0 else DiscPercent end 'Disc. %', case when DiscAmount is null then 0 else DiscAmount end 'Total Discount', BonusScheme 'Bonus Amount', CashBackScheme 'CashBack Amount', SubTotal, SubTotal_PPN, SubTotal_PPH, GelombangID, BracketID, Base, SeqNoGroup, BracketDesc From [PurchQuotation_Dtl] Where PurchQuotID = '" + txtPqNumber.Text + "' order by SeqNo asc";
                else if (txtTransType.Text == "AMOUNT")
                    Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, Amount, [Unit], Ratio, [Price], [Amount2], [Unit2], [AvailableDate], [DeliveryMethod], case when Total is null then 0 else Total end Total, ReffTransId, ReffSeqNo, ReffTransType, TransStatus, Deskripsi, DiscType 'Disc. Type', case when DiscPercent is null then 0 else DiscPercent end 'Disc. %', case when DiscAmount is null then 0 else DiscAmount end 'Total Discount', BonusScheme 'Bonus Amount', CashBackScheme 'CashBack Amount', SubTotal, SubTotal_PPN, SubTotal_PPH, GelombangID, BracketID, Base, SeqNoGroup, BracketDesc From [PurchQuotation_Dtl] Where PurchQuotID = '" + txtPqNumber.Text + "' order by SeqNo asc";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    string AvailableDate = Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy");

                    if (txtTransType.Text != "AMOUNT")
                        this.dgvPqDetails1.Rows.Add(Dr["SeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["Base"], Dr["Qty"], Dr["Qty2"], Dr["Unit"], Dr["Ratio"], Dr["Price"], Dr["Unit2"], Dr["DeliveryMethod"], AvailableDate, Dr["Total"], Dr["ReffTransId"], Dr["ReffSeqNo"], Dr["ReffTransType"], Dr["TransStatus"], Dr["Disc. Type"], Dr["Disc. %"], Dr["Total Discount"], Dr["SubTotal"], Dr["SubTotal_PPN"], Dr["SubTotal_PPH"], Dr["CashBack Amount"], Dr["Bonus Amount"], Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                    else if (txtTransType.Text == "AMOUNT")
                        this.dgvPqDetails1.Rows.Add(Dr["SeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["Base"], Dr["Amount"], Dr["Amount2"], Dr["Unit"], Dr["Ratio"], Dr["Price"], Dr["Unit2"], Dr["DeliveryMethod"], AvailableDate, Dr["Total"], Dr["ReffTransId"], Dr["ReffSeqNo"], Dr["ReffTransType"], Dr["TransStatus"], Dr["Disc. Type"], Dr["Disc. %"], Dr["Total Discount"], Dr["SubTotal"], Dr["SubTotal_PPN"], Dr["SubTotal_PPH"], Dr["CashBack Amount"], Dr["Bonus Amount"], Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], Dr["Deskripsi"]);

                    cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                    cell.Value = "Select";
                    dgvPqDetails1.Rows[(dgvPqDetails1.Rows.Count - 1)].Cells["Disc. Type"] = cell;
                    if (Dr["Disc. Type"].ToString() != "")
                        cell.Value = Dr["Disc. Type"].ToString();
                    j++;
                }
                Dr.Close();


                dgvAttachment.Rows.Clear();
                if (dgvAttachment.RowCount - 1 <= 0)
                {
                    dgvAttachment.ColumnCount = 5;
                    dgvAttachment.Columns[0].Name = "FileName";
                    dgvAttachment.Columns[1].Name = "ContentType";
                    dgvAttachment.Columns[2].Name = "File Size (kb)";
                    dgvAttachment.Columns[3].Name = "Attachment";
                    dgvAttachment.Columns[4].Name = "Id";

                    dgvAttachment.Columns["Attachment"].Visible = false;
                    dgvAttachment.Columns["Id"].Visible = false;
                }

                Query = "Select * From [tblAttachments] Where ReffTableName = 'PurchQuotationH' And ReffTransId = '" + txtPqNumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                    test.Add((byte[])Dr["Attachment"]);
                }

                dgvAttachment.AutoResizeColumns();

                SetReadOnlyDatagridView();

                if (txtTransType.Text != "AMOUNT")
                {
                    dgvPqDetails1.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPqDetails1.Columns["Qty Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (txtTransType.Text != "AMOUNT")
                {
                    dgvPqDetails1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPqDetails1.Columns["Amount Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                dgvPqDetails1.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["SubTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["SubTotal_PPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["SubTotal_PPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                SetVisibleDatagridView();
            }
            dgvPqDetails1.AutoResizeColumns();
            refreshGrey();
            SelectDgvPR();
        }

        #region MODE
        public void ModeNew()
        {
            txtExchRate.ReadOnly = true;
            txtPqNumber.Text = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            btnSearchPR.Enabled = true;
            cbByPhone.Enabled = true;

            if (cbByPhone.Checked == true)
            {
                txtVendorPqNumber.ReadOnly = true;
            }
            else
            {
                txtVendorPqNumber.ReadOnly = false;
            }
            dtVendorPqDate.Enabled = true;
            dtValidTo.Enabled = true;
            cmbPPn.Enabled = true;
            cmbPPh.Enabled = true;
            cmbTermOfPayment.Enabled = true;
            txtTotal.ReadOnly = true;
            txtTotalPPN.ReadOnly = true;
            txtTotalPPH.ReadOnly = true;
            txtDeskripsi.ReadOnly = false;
            cmbCurrency.Enabled = true;
            cmbDPRequired.Enabled = true;
            cmbPaymentMode.Enabled = true;
            txtDPPercent.ReadOnly = true;
            txtDPAmount.ReadOnly = true;
            txtBonusScheme.ReadOnly = true;
            txtCashBackScheme.ReadOnly = true;

            //edit
            txtDPPercent.Text = "0";
            txtDPAmount.Text = "0";
            cmbPPh.Text = "0.00";
            cmbPPn.Text = "10.00";

        }

        public void ModeEdit()
        {
            Mode = "Edit";

            cbByPhone.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;
            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnDelAttachment.Enabled = true;
            cmbDPType.Enabled = true;
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnSearchPR.Enabled = false;
            //btnSearchVendor.Enabled = true;

            if (cbByPhone.Checked == true)
            {
                txtVendorPqNumber.ReadOnly = true;
            }
            else
            {
                txtVendorPqNumber.ReadOnly = false;
            }
            dtVendorPqDate.Enabled = true;
            dtValidTo.Enabled = true;
            cmbPPn.Enabled = true;
            cmbPPh.Enabled = true;
            cmbTermOfPayment.Enabled = true;
            txtTotal.ReadOnly = true;
            txtTotalPPN.ReadOnly = true;
            txtTotalPPH.ReadOnly = true;

            if (cmbDPRequired.Text == "YES")
            {
                txtDPPercent.ReadOnly = false;
                txtDPAmount.ReadOnly = false;
            }
            else
            {
                txtDPPercent.ReadOnly = true;
                txtDPAmount.ReadOnly = true;
                txtDPPercent.Text = "0.00";
                txtDPAmount.Text = "0.00";
            }
            txtExchRate.ReadOnly = true;
            cmbCurrency.Enabled = true;
            cmbDPRequired.Enabled = true;
            cmbPaymentMode.Enabled = true;
            txtDeskripsi.ReadOnly = false;
            txtBonusScheme.ReadOnly = true;
            txtCashBackScheme.ReadOnly = true;

            SetReadOnlyDatagridView();

            if (dgvPqDetails1.Columns.Contains("Qty"))
            {
                dgvPqDetails1.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Qty Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            }
            if (dgvPqDetails1.Columns.Contains("Amount"))
            {
                dgvPqDetails1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Amount Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvPqDetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal_PPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal_PPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            SetVisibleDatagridView();

            dgvPqDetails1.AutoResizeColumns();
            dgvPqDetails1.DefaultCellStyle.BackColor = Color.White;

            refreshGrey();
        }

        public void ModeBeforeEdit()
        {
            if (ControlMgr.GroupName != "Purchase Manager")
                Mode = "BeforeEdit";
            //hendry
            dtValidTo.Enabled = false;
            //hendry end
            cbByPhone.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            //tia edit
            txtVendorId.Enabled = true;
            txtVendorName.Enabled = true;
            txtRfqNumber.Enabled = true;
            txtPRId.Enabled = true;

            txtVendorId.ReadOnly = true;
            txtVendorName.ReadOnly = true;
            txtRfqNumber.ReadOnly = true;
            txtPRId.ReadOnly = true;

            txtVendorId.ContextMenu = vendid;
            txtVendorName.ContextMenu = vendid;
            txtRfqNumber.ContextMenu = vendid;
            txtPRId.ContextMenu = vendid;
            //tia edit end
            cmbDPType.Enabled = false;

            btnSearchPR.Enabled = false;
            //btnSearchVendor.Enabled = false;

            txtVendorPqNumber.ReadOnly = true;
            dtVendorPqDate.Enabled = false;
            cmbPPn.Enabled = false;
            cmbPPh.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            cmbCurrency.Enabled = false;
            txtCashBackScheme.ReadOnly = false;
            txtTotal.ReadOnly = true;
            txtTotalPPN.ReadOnly = true;
            txtTotalPPH.ReadOnly = true;
            txtDeskripsi.ReadOnly = true;
            cmbDPRequired.Enabled = false;
            txtExchRate.ReadOnly = true;
            cmbDPRequired.Enabled = false;
            cmbPaymentMode.Enabled = false;
            txtDPPercent.ReadOnly = true;
            txtDPAmount.ReadOnly = true;
            txtBonusScheme.ReadOnly = true;
            txtCashBackScheme.ReadOnly = true;

            dgvPqDetails1.ReadOnly = true;
            dgvPqDetails1.DefaultCellStyle.BackColor = Color.LightGray;
            dgvAttachment.ReadOnly = true;
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;
            for (int i = 0; i < dgvPqDetails1.RowCount; i++)
                for (int j = 0; j < dgvPqDetails1.ColumnCount; j++)
                {
                    dgvPqDetails1.Rows[i].Cells["Disc. %"].Style.BackColor = Color.LightGray;
                    dgvPqDetails1.Rows[i].Cells["Total Discount"].Style.BackColor = Color.LightGray;
                }
        }
        #endregion

        private Boolean validasiSave()
        {
            bool vBol = true;

            if (txtTransType.Text != "AMOUNT" && dgvPqDetails1.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                tabControl1.SelectedIndex = 1;
                vBol = false;
            }

            else if (txtVendorId.Text.Trim() == "" || txtVendorName.Text.Trim() == "")
            {
                MessageBox.Show("Vendor Id tidak boleh kosong.");
                vBol = false;
            }

            else if (dgvAttachment.RowCount < 1)
            {
                MessageBox.Show("Attachment harus ada.");
                tabControl1.SelectedIndex = 3;
                vBol = false;
            }

            else if (cbByPhone.Checked == false && txtVendorPqNumber.Text.Trim() == "")
            {
                MessageBox.Show("Vendor Quote Number harus diisi karena tidak by Phone.");
                vBol = false;
            }

            else if (cmbTermOfPayment.Text == "")
            {
                MessageBox.Show("Term of Payment wajib dipilih.");
                tabControl1.SelectedIndex = 0;
                vBol = false;
            }

            else if (cmbPaymentMode.Text == "")
            {
                MessageBox.Show("Payment Mode wajib dipilih.");
                tabControl1.SelectedIndex = 0;
                vBol = false;
            }

            else if (cmbDPRequired.Text == "")
            {
                MessageBox.Show("DP Required wajib dipilih.");
                tabControl1.SelectedIndex = 0;
                vBol = false;
            }

            else if (cmbDPRequired.Text == "YES")
            {
                if (txtDPPercent.Text == "" && txtDPAmount.Text == "" || (Convert.ToDecimal(txtDPPercent.Text) <= 0 && Convert.ToDecimal(txtDPAmount.Text) <= 0))
                {
                    MessageBox.Show("DP Percent atau Amaount wajib diisi dan tidak boleh 0.");
                    tabControl1.SelectedIndex = 0;
                    vBol = false;
                    return vBol;
                }
            }

            else if (txtTransType.Text != "AMOUNT")
            {
                for (int i = 0; i <= dgvPqDetails1.RowCount - 1; i++)
                {
                    Decimal QtyVendor = 0;
                    if (dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value != null && dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value.ToString() != "")
                        QtyVendor = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value.ToString());

                    if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() == "Y" && QtyVendor <= 0)
                    {
                        MessageBox.Show("Item No = " + dgvPqDetails1.Rows[i].Cells["No"].Value + ", Qty tidak boleh 0..");
                        tabControl1.SelectedIndex = 1;
                        vBol = false;
                        return vBol;
                    }
                    else
                    {
                        Decimal Price = 0;
                        if (dgvPqDetails1.Rows[i].Cells["Price"].Value != null && dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString() != "")
                            Price = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString());
                        if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() != "N" && Price <= 0)
                        {
                            MessageBox.Show("Item No = " + dgvPqDetails1.Rows[i].Cells["No"].Value + ", Price tidak boleh lebih kecil atau sama dengan 0");
                            tabControl1.SelectedIndex = 1;
                            vBol = false;
                            return vBol;
                        }
                    }

                    if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        if (dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value == null || dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() == "")
                        {
                            MessageBox.Show("Item No = " + dgvPqDetails1.Rows[i].Cells["No"].Value + ", Delivery Method harus ditentukan.");
                            tabControl1.SelectedIndex = 1;
                            vBol = false;
                            return vBol;
                        }
                        if (dgvPqDetails1.Rows[i].Cells["Price"].Value == null || dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString() == "")
                        {
                            MessageBox.Show("Item No = " + dgvPqDetails1.Rows[i].Cells["No"].Value + ", price tidak boleh nol.");
                            tabControl1.SelectedIndex = 1;
                            vBol = false;
                            return vBol;
                        }
                    }
                }

                for (int i = 0; i <= dgvPqDetails1.RowCount - 1; i++)
                {
                    int cek = 0;

                    if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        for (int j = i + 1; j < dgvPqDetails1.RowCount - i; j++)
                        {
                            if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() != "N")
                            {
                                if (dgvPqDetails1.Rows[i].Cells["FullItemID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["FullItemID"].Value.ToString() && dgvPqDetails1.Rows[i].Cells["GelombangID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["GelombangID"].Value.ToString() && dgvPqDetails1.Rows[i].Cells["BracketID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["BracketID"].Value.ToString())
                                {
                                    if (dgvPqDetails1.Rows[i].Cells["PR ID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["PR ID"].Value.ToString())
                                    {
                                        if (dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["DeliveryMethod"].Value.ToString())
                                        {
                                            cek++;
                                        }
                                    }
                                }
                                if (cek > 0)
                                {
                                    MessageBox.Show("Item no " + dgvPqDetails1.Rows[i].Cells["No"].Value.ToString() + ". Periksa Delivery Method.");
                                    tabControl1.SelectedIndex = 1;
                                    vBol = false;
                                    return vBol;
                                }

                            }
                        }
                    }
                }

            }

            else if (txtTransType.Text == "AMOUNT")
            {
                for (int i = 0; i <= dgvPqDetails1.RowCount - 1; i++)
                {
                    Decimal Price = 0;
                    if (dgvPqDetails1.Rows[i].Cells["Price"].Value != null && dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString() != "")
                        Price = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString());
                    if (Price <= 0)
                    {
                        MessageBox.Show("Item No = " + dgvPqDetails1.Rows[i].Cells["No"].Value + ", Price tidak boleh lebih kecil atau sama dengan 0");
                        tabControl1.SelectedIndex = 1;
                        vBol = false;
                        return vBol;
                    }

                    if (dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value == null || dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() == "")
                    {
                        MessageBox.Show("Item No = " + dgvPqDetails1.Rows[i].Cells["No"].Value + ", Delivery Method harus ditentukan.");
                        tabControl1.SelectedIndex = 1;
                        vBol = false;
                        return vBol;
                    }
                }

                for (int i = 0; i <= dgvPqDetails1.RowCount - 1; i++)
                {
                    int cek = 0;

                    if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        for (int j = i + 1; j < dgvPqDetails1.RowCount - i; j++)
                        {
                            if (dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString() != "N")
                            {
                                if (dgvPqDetails1.Rows[i].Cells["FullItemID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["FullItemID"].Value.ToString() && dgvPqDetails1.Rows[i].Cells["GelombangID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["GelombangID"].Value.ToString() && dgvPqDetails1.Rows[i].Cells["BracketID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["BracketID"].Value.ToString())
                                {
                                    if (dgvPqDetails1.Rows[i].Cells["PR ID"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["PR ID"].Value.ToString())
                                    {
                                        if (dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() == dgvPqDetails1.Rows[j].Cells["DeliveryMethod"].Value.ToString())
                                        {
                                            cek++;
                                        }
                                    }
                                }
                                if (cek > 0)
                                {
                                    MessageBox.Show("Item no " + dgvPqDetails1.Rows[i].Cells["No"].Value.ToString() + ". Periksa Delivery Method.");
                                    vBol = false;
                                    return vBol;
                                }

                            }
                        }
                    }
                }
            }

            return vBol;
        }

        private void saveNew()
        {
            string Jenis = "PQ", Kode = "PQ", type_dp = "";
            string PQNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);

            Query = "Insert into PurchQuotationH (PurchQuotID,PurchReqId,RfqId,TransType,OrderDate,VendorQuotNo,VendorQuotDate,VendID,VendName,TermOfPayment, PaymentModeID,PPN,PPH,Deskripsi,Total,Total_PPN,Total_PPH,CreatedDate,CreatedBy, DP, CurrencyId, ExchRate, ValidTo, DPAmount, BonusScheme, CashbackScheme, TotalDiscount, DPType) values( ";
            Query += "@pqid, @prid, @rfqid, @type, @pqdate, @vendorno, @vendordate, @vendorid, @vendorname, @termofpay, @paymode, @ppn, @pph, @deskripsi, @total, @totalppn, @totalpph, getdate(), @createdby, @dp, @currency, @rate, @validto, @dpamount, @bonus, @cashback, @totaldisc, @dptype)";

            if (Convert.ToDecimal(txtDPPercent.Text) != 0 || Convert.ToDecimal(txtDPAmount.Text) != 0)
                type_dp = cmbDPType.Text;

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@pqid", PQNumber);
            Cmd.Parameters.AddWithValue("@prid", txtPRId.Text);
            Cmd.Parameters.AddWithValue("@rfqid", txtRfqNumber.Text);
            Cmd.Parameters.AddWithValue("@type", txtTransType.Text);
            Cmd.Parameters.AddWithValue("@pqdate", dtPqDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            Cmd.Parameters.AddWithValue("@vendorno", txtVendorPqNumber.Text);
            Cmd.Parameters.AddWithValue("@vendordate", dtVendorPqDate.Value.ToString("yyyy-MM-dd") + " 00:00:00");
            Cmd.Parameters.AddWithValue("@vendorid", txtVendorId.Text);
            Cmd.Parameters.AddWithValue("@vendorname", txtVendorName.Text);
            Cmd.Parameters.AddWithValue("@termofpay", cmbTermOfPayment.Text);
            Cmd.Parameters.AddWithValue("@paymode", cmbPaymentMode.Text);
            Cmd.Parameters.AddWithValue("@ppn", (cmbPPn.Text == "" ? "0.00" : cmbPPn.Text));
            Cmd.Parameters.AddWithValue("@pph", (cmbPPh.Text == "" ? "0.00" : cmbPPh.Text));
            Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text.Trim());
            Cmd.Parameters.AddWithValue("@total", "0.00");
            Cmd.Parameters.AddWithValue("@totalppn", "0.00");
            Cmd.Parameters.AddWithValue("@totalpph", "0.00");
            Cmd.Parameters.AddWithValue("@createdby", ControlMgr.UserId);
            Cmd.Parameters.AddWithValue("@dp", (txtDPPercent.Text == "" ? "0.00" : Convert.ToDecimal(txtDPPercent.Text).ToString()));
            Cmd.Parameters.AddWithValue("@currency", cmbCurrency.Text);
            Cmd.Parameters.AddWithValue("@rate", (txtExchRate.Text == "" ? "0.00" : Convert.ToDecimal(txtExchRate.Text).ToString()));
            Cmd.Parameters.AddWithValue("@validto", dtValidTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
            Cmd.Parameters.AddWithValue("@dpamount", Convert.ToDecimal(txtDPAmount.Text.Trim()));
            Cmd.Parameters.AddWithValue("@bonus", Convert.ToDecimal(txtBonusScheme.Text));
            Cmd.Parameters.AddWithValue("@cashback", Convert.ToDecimal(txtCashBackScheme.Text));
            Cmd.Parameters.AddWithValue("@totaldisc", Convert.ToDecimal(txtDiscount.Text.Trim()));
            Cmd.Parameters.AddWithValue("@dptype", type_dp);

            Cmd.ExecuteNonQuery();

            for (int i = 0; i <= dgvPqDetails1.RowCount - 1; i++)
            {

                if (dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value == null || dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value == "")
                    dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value = "01-01-1900";


                if (txtTransType.Text != "AMOUNT")
                {
                    Query = "Insert PurchQuotation_Dtl (PurchQuotID,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,Qty,Unit,Ratio,Price,Qty2,Unit2,AvailableDate,DeliveryMethod,ReffTransId,ReffSeqNo,ReffTransType,TransStatus,Deskripsi,DiscType, DiscPercent, DiscAmount, BonusScheme, CashBackScheme,SubTotal,SubTotal_PPN,SubTotal_PPH,GelombangID,BracketID,BracketDesc,Base, SeqNoGroup,CreatedDate,CreatedBy,Total,TotalDiscount) Values ";
                    Query += "(@pqid, @pqdate, @seqno, @group, @subgroup1, @subgroup2, @itemid, @fullitemid, @itemname, @qty, @unit, @ratio, @price, @qty2, @unit2, @availdate, @dm, @rtid, @rsno, @rttype, @status, @desc, @disct, @discp, @disca, @bonus, @cbscheme, @st, @stppn, @stpph, @gelombangid, @bracketid, @bracketdesc, @base, @sngroup, getdate(), @crby, @tot, @totdisc)";
                }
                else if (txtTransType.Text == "AMOUNT")
                {
                    Query = "Insert PurchQuotation_Dtl (PurchQuotID,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,Amount,Qty,Unit,Ratio,Price,Amount2,Unit2,AvailableDate,DeliveryMethod,ReffTransId,ReffSeqNo,ReffTransType,TransStatus,Deskripsi, DiscType, DiscPercent, DiscAmount,BonusScheme,CashBackScheme,SubTotal,SubTotal_PPN,SubTotal_PPH,GelombangID,BracketID,BracketDesc,Base, SeqNoGroup,CreatedDate,CreatedBy,Total,TotalDiscount) Values ";
                    Query += "(@pqid, @pqdate, @seqno, @group, @subgroup1, @subgroup2, @itemid, @fullitemid, @itemname, @qty, 1, @unit, @ratio, @price, @qty2, @unit2, @availdate, @dm, @rtid, @rsno, @rttype, @status, @desc, @disct, @discp, @disca, @bonus, @cbscheme, @st, @stppn, @stpph, @gelombangid, @bracketid, @bracketdesc, @base, @sngroup, getdate(), @crby, @tot, @totdisc)";
                }

                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@pqid", PQNumber);
                Cmd.Parameters.AddWithValue("@pqdate", (dtPqDate.Value == null ? "" : dtPqDate.Value.ToString("yyyy-MM-dd")));
                Cmd.Parameters.AddWithValue("@seqno", (dgvPqDetails1.Rows[i].Cells["No"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["No"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@group", (dgvPqDetails1.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["GroupId"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@subgroup1", (dgvPqDetails1.Rows[i].Cells["SubGroup1Id"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["SubGroup1Id"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@subgroup2", (dgvPqDetails1.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["SubGroup2Id"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@itemid", (dgvPqDetails1.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ItemId"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@fullitemid", (dgvPqDetails1.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["FullItemID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@itemname", (dgvPqDetails1.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ItemName"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@unit", (dgvPqDetails1.Rows[i].Cells["Unit"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["Unit"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@ratio", (dgvPqDetails1.Rows[i].Cells["Ratio"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["Ratio"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@price", (dgvPqDetails1.Rows[i].Cells["Price"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString())));
                Cmd.Parameters.AddWithValue("@unit2", (dgvPqDetails1.Rows[i].Cells["Unit2"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["Unit2"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@availdate", (dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value == null ? "" : FormateDateyyyymmdd(dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value.ToString())));
                Cmd.Parameters.AddWithValue("@dm", (dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@rtid", (dgvPqDetails1.Rows[i].Cells["PR ID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["PR ID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@rsno", (dgvPqDetails1.Rows[i].Cells["ReffSeqNo"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ReffSeqNo"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@rttype", (dgvPqDetails1.Rows[i].Cells["ReffTransType"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ReffTransType"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@status", (dgvPqDetails1.Rows[i].Cells["TransStatus"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["TransStatus"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@desc", (dgvPqDetails1.Rows[i].Cells["Notes"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["Notes"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@disct", (dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@discp", (dgvPqDetails1.Rows[i].Cells["Disc. %"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Disc. %"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@disca", (dgvPqDetails1.Rows[i].Cells["Total Discount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Total Discount"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@bonus", (dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@cbscheme", (dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@st", (dgvPqDetails1.Rows[i].Cells["SubTotal"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SubTotal"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@stppn", (dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@stpph", (dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@gelombangid", (dgvPqDetails1.Rows[i].Cells["GelombangID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["GelombangID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@bracketid", (dgvPqDetails1.Rows[i].Cells["BracketID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["BracketID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@bracketdesc", (dgvPqDetails1.Rows[i].Cells["BracketDesc"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["BracketDesc"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@base", (dgvPqDetails1.Rows[i].Cells["Base"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@sngroup", (dgvPqDetails1.Rows[i].Cells["SeqNoGroup"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@crby", ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@tot", (dgvPqDetails1.Rows[i].Cells["Total"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Total"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@totdisc", (dgvPqDetails1.Rows[i].Cells["Total Discount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Total Discount"].Value.ToString()));

                if (txtTransType.Text != "AMOUNT")
                {
                    Cmd.Parameters.AddWithValue("@qty", (dgvPqDetails1.Rows[i].Cells["Qty"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Qty"].Value.ToString())));
                    Cmd.Parameters.AddWithValue("@qty2", (dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value.ToString())));
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@qty", (dgvPqDetails1.Rows[i].Cells["Amount"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Amount"].Value.ToString())));
                    Cmd.Parameters.AddWithValue("@qty2", (dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value.ToString())));
                }

                Cmd.ExecuteNonQuery();
            }

            for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                Query += "( 'PurchQuotationH', @pqnumber, @filename, @contentType, @filesize ,@binaryValue);";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@pqnumber", PQNumber);
                Cmd.Parameters.AddWithValue("@filename", dgvAttachment.Rows[i].Cells["FileName"].Value.ToString());
                Cmd.Parameters.AddWithValue("@contentType", dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString());
                Cmd.Parameters.AddWithValue("@filesize", dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString());
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }

            //Update Status PR
            Query = "Update PurchRequisitionH set TransStatus='22' where PurchReqID in (select ReffTransID from [dbo].[PurchQuotation_Dtl] where PurchQuotID='" + PQNumber + "');";
            Query += "Update [dbo].[RequestForQuotationH] set TransStatus = '03' where RfqID = '" + txtRfqNumber.Text + "';";

            if (Query != "")
            {
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                Query = "";
            }


            MessageBox.Show("Data PQNumber : " + PQNumber + " berhasil ditambahkan.");
            txtPqNumber.Text = PQNumber;
            txtVendorPqNumber.Text = "";
            ModeBeforeEdit();
        }

        private void saveEdit()
        {
            #region NOT New
            Query = "UPDATE PurchQuotationH SET ";
            Query += "OrderDate = @pqdate, RfqID = @rfqid, PurchReqId = @prid, TransType = @ttype,";
            Query += "VendorQuotNo = @vendorquoteno, VendorQuotDate = @vendordate, VendID = @vendorid,";
            Query += "VendName = @vendorname, PaymentModeID = @paymode, TermOfPayment = @termpay, PPN = @ppn, PPH = @pph,";
            Query += "Deskripsi = @descr, DP = @dpp, ExchRate = @excrate, CurrencyId = @curr, Total='0.00', ";
            Query += "Total_PPN='0.00', Total_PPH='0.00', UpdatedDate = getdate(), DPType= @dptype, ";
            Query += "UpdatedBy = @upby, ValidTo = @validto, DPAmount = @dpa,";
            Query += "BonusScheme = @bonusscheme, CashbackScheme = @cbscheme, TotalDiscount = @totdisc OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where PurchQuotID = @pqid";

            Cmd = new SqlCommand(Query, Conn);

            Cmd.Parameters.AddWithValue("@pqdate", dtPqDate.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@rfqid", txtRfqNumber.Text);
            Cmd.Parameters.AddWithValue("@prid", txtPRId.Text);
            Cmd.Parameters.AddWithValue("@ttype", txtTransType.Text);
            Cmd.Parameters.AddWithValue("@vendorquoteno", txtVendorPqNumber.Text);
            Cmd.Parameters.AddWithValue("@vendordate", dtVendorPqDate.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@vendorid", txtVendorId.Text);
            Cmd.Parameters.AddWithValue("@vendorname", txtVendorName.Text);
            Cmd.Parameters.AddWithValue("@paymode", cmbPaymentMode.Text);
            Cmd.Parameters.AddWithValue("@termpay", (cmbTermOfPayment.Text == "" ? "1" : cmbTermOfPayment.Text));
            Cmd.Parameters.AddWithValue("@ppn", (cmbPPn.Text == "" ? "0.00" : cmbPPn.Text));
            Cmd.Parameters.AddWithValue("@pph", (cmbPPh.Text == "" ? "0.00" : cmbPPh.Text));
            Cmd.Parameters.AddWithValue("@descr", txtDeskripsi.Text.Trim());
            Cmd.Parameters.AddWithValue("@dpp", (txtDPPercent.Text == "" ? "0.00" : Convert.ToDecimal(txtDPPercent.Text).ToString()));
            Cmd.Parameters.AddWithValue("@excrate", (txtExchRate.Text == "" ? "0.00" : Convert.ToDecimal(txtExchRate.Text).ToString()));
            Cmd.Parameters.AddWithValue("@curr", cmbCurrency.Text);
            if (Convert.ToDecimal(txtDPPercent.Text) == 0 && Convert.ToDecimal(txtDPAmount.Text) == 0)
                Cmd.Parameters.AddWithValue("@dptype", "");
            else
                Cmd.Parameters.AddWithValue("@dptype", cmbDPType.Text);
            Cmd.Parameters.AddWithValue("@upby", ControlMgr.UserId);
            Cmd.Parameters.AddWithValue("@validto", dtValidTo.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@dpa", Convert.ToDecimal(txtDPAmount.Text.Trim()));
            Cmd.Parameters.AddWithValue("@bonusscheme", Convert.ToDecimal(txtBonusScheme.Text));
            Cmd.Parameters.AddWithValue("@cbscheme", Convert.ToDecimal(txtCashBackScheme.Text));
            Cmd.Parameters.AddWithValue("@totdisc", Convert.ToDecimal(txtDiscount.Text.Trim()));
            Cmd.Parameters.AddWithValue("@pqid", txtPqNumber.Text.Trim());

            Cmd.ExecuteNonQuery();
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                CreatedBy = Dr["CreatedBy"].ToString();
            }
            Dr.Close();

            Query = "Delete from PurchQuotation_Dtl where PurchQuotID='" + txtPqNumber.Text.Trim() + "';";
            Query += "Delete from tblAttachments where ReffTableName='PurchQuotationH' And ReffTransId='" + txtPqNumber.Text.Trim() + "';";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            for (int i = 0; i <= dgvPqDetails1.RowCount - 1; i++)
            {
                if (dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value == null || dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value == "")
                    dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value = "01-01-1900";


                if (txtTransType.Text != "AMOUNT")
                {
                    Query = "Insert PurchQuotation_Dtl (PurchQuotID,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,Qty,Unit,Ratio,Price,Qty2,Unit2,AvailableDate,DeliveryMethod,ReffTransId,ReffSeqNo,ReffTransType,TransStatus,Deskripsi,DiscType, DiscPercent, DiscAmount, BonusScheme, CashBackScheme,SubTotal,SubTotal_PPN,SubTotal_PPH,GelombangID,BracketID,BracketDesc,Base, SeqNoGroup,CreatedDate,CreatedBy,Total,TotalDiscount) Values ";
                    Query += "(@pqid, @pqdate, @seqno, @group, @subgroup1, @subgroup2, @itemid, @fullitemid, @itemname, @qty, @unit, @ratio, @price, @qty2, @unit2, @availdate, @dm, @rtid, @rsno, @rttype, @status, @desc, @disct, @discp, @disca, @bonus, @cbscheme, @st, @stppn, @stpph, @gelombangid, @bracketid, @bracketdesc, @base, @sngroup, @crdate, @crby, @tot, @totdisc)";
                }
                else if (txtTransType.Text == "AMOUNT")
                {
                    Query = "Insert PurchQuotation_Dtl (PurchQuotID,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,Amount,Qty,Unit,Ratio,Price,Amount2,Unit2,AvailableDate,DeliveryMethod,ReffTransId,ReffSeqNo,ReffTransType,TransStatus,Deskripsi, DiscType, DiscPercent, DiscAmount,BonusScheme,CashBackScheme,SubTotal,SubTotal_PPN,SubTotal_PPH,GelombangID,BracketID,BracketDesc,Base, SeqNoGroup,CreatedDate,CreatedBy,Total,TotalDiscount) Values ";
                    Query += "(@pqid, @pqdate, @seqno, @group, @subgroup1, @subgroup2, @itemid, @fullitemid, @itemname, @qty, 1, @unit, @ratio, @price, @qty2, @unit2, @availdate, @dm, @rtid, @rsno, @rttype, @status, @desc, @disct, @discp, @disca, @bonus, @cbscheme, @st, @stppn, @stpph, @gelombangid, @bracketid, @bracketdesc, @base, @sngroup, @crdate, @crby, @tot, @totdisc)";
                }

                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@pqid", txtPqNumber.Text.Trim());
                Cmd.Parameters.AddWithValue("@pqdate", (dtPqDate.Value == null ? "" : dtPqDate.Value.ToString("yyyy-MM-dd")));
                Cmd.Parameters.AddWithValue("@seqno", (dgvPqDetails1.Rows[i].Cells["No"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["No"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@group", (dgvPqDetails1.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["GroupId"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@subgroup1", (dgvPqDetails1.Rows[i].Cells["SubGroup1Id"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["SubGroup1Id"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@subgroup2", (dgvPqDetails1.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["SubGroup2Id"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@itemid", (dgvPqDetails1.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ItemId"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@fullitemid", (dgvPqDetails1.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["FullItemID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@itemname", (dgvPqDetails1.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ItemName"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@unit", (dgvPqDetails1.Rows[i].Cells["Unit"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["Unit"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@ratio", (dgvPqDetails1.Rows[i].Cells["Ratio"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["Ratio"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@price", (dgvPqDetails1.Rows[i].Cells["Price"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString())));
                Cmd.Parameters.AddWithValue("@unit2", (dgvPqDetails1.Rows[i].Cells["Unit2"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["Unit2"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@availdate", (dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value == null ? "" : FormateDateyyyymmdd(dgvPqDetails1.Rows[i].Cells["AvailableDate"].Value.ToString())));
                Cmd.Parameters.AddWithValue("@dm", (dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@rtid", (dgvPqDetails1.Rows[i].Cells["PR ID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["PR ID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@rsno", (dgvPqDetails1.Rows[i].Cells["ReffSeqNo"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ReffSeqNo"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@rttype", (dgvPqDetails1.Rows[i].Cells["ReffTransType"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["ReffTransType"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@status", (dgvPqDetails1.Rows[i].Cells["TransStatus"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["TransStatus"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@desc", (dgvPqDetails1.Rows[i].Cells["Notes"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["Notes"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@disct", (dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@discp", (dgvPqDetails1.Rows[i].Cells["Disc. %"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Disc. %"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@disca", (dgvPqDetails1.Rows[i].Cells["Total Discount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Total Discount"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@bonus", (dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@cbscheme", (dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@st", (dgvPqDetails1.Rows[i].Cells["SubTotal"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SubTotal"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@stppn", (dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@stpph", (dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@gelombangid", (dgvPqDetails1.Rows[i].Cells["GelombangID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["GelombangID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@bracketid", (dgvPqDetails1.Rows[i].Cells["BracketID"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["BracketID"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@bracketdesc", (dgvPqDetails1.Rows[i].Cells["BracketDesc"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["BracketDesc"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@base", (dgvPqDetails1.Rows[i].Cells["Base"].Value == "" ? "" : dgvPqDetails1.Rows[i].Cells["Base"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@sngroup", (dgvPqDetails1.Rows[i].Cells["SeqNoGroup"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@crdate", CreatedDate);
                Cmd.Parameters.AddWithValue("@crby", CreatedBy);
                Cmd.Parameters.AddWithValue("@tot", (dgvPqDetails1.Rows[i].Cells["Total"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Total"].Value.ToString()));
                Cmd.Parameters.AddWithValue("@totdisc", (dgvPqDetails1.Rows[i].Cells["Total Discount"].Value == "" ? "0.00" : dgvPqDetails1.Rows[i].Cells["Total Discount"].Value.ToString()));

                if (txtTransType.Text != "AMOUNT")
                {
                    Cmd.Parameters.AddWithValue("@qty", (dgvPqDetails1.Rows[i].Cells["Qty"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Qty"].Value.ToString())));
                    Cmd.Parameters.AddWithValue("@qty2", (dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value.ToString())));
                }
                else
                {
                    Cmd.Parameters.AddWithValue("@qty", (dgvPqDetails1.Rows[i].Cells["Amount"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Amount"].Value.ToString())));
                    Cmd.Parameters.AddWithValue("@qty2", (dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value == "" ? 0.00 : Double.Parse(dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value.ToString())));
                }

                Cmd.ExecuteNonQuery();
            }

            for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                Query += "( 'PurchQuotationH', @pqnumber, @filename, @contentType, @filesize ,@binaryValue);";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@pqnumber", txtPqNumber.Text);
                Cmd.Parameters.AddWithValue("@filename", dgvAttachment.Rows[i].Cells["FileName"].Value.ToString());
                Cmd.Parameters.AddWithValue("@contentType", dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString());
                Cmd.Parameters.AddWithValue("@filesize", dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString());
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Data PQNumber : " + txtPqNumber.Text + " berhasil diupdate.");
            ModeBeforeEdit();
            #endregion
        }

        private void Save_PM()
        {
            Query = "UPDATE PurchQuotationH SET ValidTo = @validto, UpdatedBy = @updateby, UpdatedDate = GETDATE() WHERE PurchQuotID = @PQID";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@validto", dtValidTo.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@updateby", ControlMgr.UserId);
            Cmd.Parameters.AddWithValue("@PQID", txtPqNumber.Text.Trim());
            Cmd.ExecuteNonQuery();

            Query = "Delete from tblAttachments where ReffTableName = 'PurchQuotationH' And ReffTransId = @pqnumber;";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@pqnumber", txtPqNumber.Text.Trim());
            Cmd.ExecuteNonQuery();

            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                Query += "( 'PurchQuotationH', @pqnumber, @filename, @contentType, @filesize ,@binaryValue);";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@pqnumber", txtPqNumber.Text.Trim());
                Cmd.Parameters.AddWithValue("@filename", dgvAttachment.Rows[i].Cells["FileName"].Value.ToString());
                Cmd.Parameters.AddWithValue("@contentType", dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString());
                Cmd.Parameters.AddWithValue("@filesize", dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString());
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Data PQNumber : " + txtPqNumber.Text + " berhasil diupdate.");
            ModeBeforeEdit();
        }


        string CreatedBy;
        DateTime CreatedDate;

        private void btnSave_Click(object sender, EventArgs e)
        {
            CreatedDate = DateTime.Now;
            CreatedBy = "";

            if (!validasiSave())
                return;

            //try
            //{
            using (TransactionScope scope = new TransactionScope())
            {
                Conn = ConnectionString.GetConnection();
                if (ControlMgr.GroupName == "Purchase Manager")
                    Save_PM();
                else
                {
                    if (Mode == "New" && txtPqNumber.Text.Trim() == "")
                        saveNew();
                    else
                        saveEdit();
                }
                scope.Complete();
            }

            GetDataHeader();
            if (Parent != null)
            {
                Parent.RefreshGrid();
            }

            if (ParentRFQ != null)
            {
                ParentRFQ.RefreshGrid();
            }

            MainMenu f = new MainMenu();
            f.refreshTaskList();
            refreshGrey();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return;
            //}
            //finally
            //{
            Conn.Close();
            //this.Close();
            //}
        }

        private string FormateDateyyyymmdd(string tmpDate)
        {
            //string reformat="";
            tmpDate = tmpDate.Replace("\\", "-");
            tmpDate = tmpDate.Replace("/", "-");
            string[] data = tmpDate.Split('-');
            return data[2] + "-" + data[1] + "-" + data[0];
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            cbByPhone.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dtPqDate.Enabled = false;
            dtValidTo.Enabled = false;
            txtVendorPqNumber.ReadOnly = false;

            ModeBeforeEdit();
            //FormPQ_Load(sender, e);
            GetDataHeader();
            refreshGrey();
        }

        private void dgvPrDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (Mode != "BeforeEdit")
                {
                    if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                    {
                        DeliveryMethod.Location = dgvPqDetails1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        DeliveryMethod.Visible = true;
                        string tmpFullItemId = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["FullItemID"].Value.ToString();
                        string tmpGelombangId = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["GelombangID"].Value.ToString();
                        string tmpBracketId = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["BracketID"].Value.ToString();
                        string tmpDeliveryMethod = "";
                        Conn = ConnectionString.GetConnection();
                        for (int i = 0; i < dgvPqDetails1.RowCount; i++)
                        {
                            if (dgvPqDetails1.Rows[i].Cells["FullItemID"].Value.ToString() == tmpFullItemId && dgvPqDetails1.Rows[i].Cells["GelombangID"].Value.ToString() == tmpGelombangId && dgvPqDetails1.Rows[i].Cells["BracketID"].Value.ToString() == tmpBracketId)
                            {
                                if (dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value != null)
                                {
                                    if (tmpDeliveryMethod == "")
                                        tmpDeliveryMethod = "'" + dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                                    else
                                        tmpDeliveryMethod += ",'" + dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                                }
                            }
                        }

                        Query = "SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod] ";

                        if (tmpDeliveryMethod != "")
                            Query += "Where DeliveryMethod not in (" + tmpDeliveryMethod + ");";

                        Cmd = new SqlCommand(Query, Conn);
                        SqlDataReader DrCmb;
                        DrCmb = Cmd.ExecuteReader();

                        DeliveryMethod.Items.Clear();
                        DeliveryMethod.Items.Add("");
                        while (DrCmb.Read())
                        {
                            DeliveryMethod.Items.Add(DrCmb[0].ToString());
                        }
                        DeliveryMethod.SelectedIndex = 0;
                        DrCmb.Close();

                        Conn.Close();
                    }

                    if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
                    {
                        dtp.Location = dgvPqDetails1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        dtp.Visible = true;

                        if (dgvPqDetails1.CurrentCell.Value != "" && dgvPqDetails1.CurrentCell.Value != null)
                        {
                            DateTime dDate;
                            if (!DateTime.TryParse(dgvPqDetails1.CurrentCell.Value.ToString(), out dDate))
                            {
                                dtp.Value = Convert.ToDateTime(FormateDateyyyymmdd(dgvPqDetails1.CurrentCell.Value.ToString()));
                            }
                            else
                            {
                                dtp.Value = Convert.ToDateTime(dgvPqDetails1.CurrentCell.Value);
                            }
                        }
                        else
                        {
                            dtp.Value = DateTime.Now;
                        }
                    }
                    else
                    {
                        dtp.Visible = false;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }

        void cbo_Validating(object sender, CancelEventArgs e)
        {

            DataGridViewComboBoxEditingControl cbo = sender as DataGridViewComboBoxEditingControl;

            DataGridView grid = cbo.EditingControlDataGridView;

            object value = cbo.Text;

            // Add value to list if not there

            if (cbo.Items.IndexOf(value) == -1)
            {

                DataGridViewComboBoxCell cboCol = (DataGridViewComboBoxCell)grid.CurrentCell;

                // Must add to both the current combobox as well as the template, to avoid duplicate entries...

                cbo.Items.Add(value);

                cboCol.Items.Add(value);

                grid.CurrentCell.Value = value;

            }

        }

        private void dgvPrDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Index = dgvPqDetails1.CurrentRow.Index;
            if (Mode != "BeforeEdit")
            {
                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Total Discount")
                {
                    if (dgvPqDetails1.Rows[Index].Cells["Total Discount"].Value.ToString() == "")
                    {
                        dgvPqDetails1.Rows[Index].Cells["Total Discount"].Value = "0";
                    }
                    SumFooter();
                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Disc. %")
                {
                    if (dgvPqDetails1.Rows[Index].Cells["Disc. %"].Value.ToString() == "")
                    {
                        dgvPqDetails1.Rows[Index].Cells["Disc. %"].Value = "0";
                    }
                    else
                    {
                        dgvPqDetails1.Rows[Index].Cells["Total Discount"].Value = Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Total"].Value) * Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Disc. %"].Value) / 100;
                    }
                    SumFooter();
                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "CashBack Amount")
                {
                    if (dgvPqDetails1.Rows[Index].Cells["CashBack Amount"].Value.ToString() == "")
                    {
                        dgvPqDetails1.Rows[Index].Cells["CashBack Amount"].Value = "0";
                    }
                    SumFooter();
                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Bonus Amount")
                {
                    if (dgvPqDetails1.Rows[Index].Cells["Bonus Amount"].Value.ToString() == "")
                    {
                        dgvPqDetails1.Rows[Index].Cells["Bonus Amount"].Value = "0";
                    }
                    SumFooter();
                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Visible = false;
                    string TmpNoGroup = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString();
                    string TmpDeliveryMethod = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["DeliveryMethod"].Value.ToString();

                    for (int i = 0; i < dgvPqDetails1.RowCount; i++)
                    {
                        if (TmpNoGroup == dgvPqDetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                        {
                            dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value = TmpDeliveryMethod;
                        }
                    }
                }


                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
                {
                    if (dgvPqDetails1.CurrentCell.Value != "" && dgvPqDetails1.CurrentCell.Value != null)
                    {
                        dgvPqDetails1.CurrentCell.Value = dtp.Value.Date.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        dtp.Value = DateTime.Now;
                    }
                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Amount Vendor")
                {
                    if (dgvPqDetails1.CurrentCell.Value != "" && dgvPqDetails1.CurrentCell.Value != null)
                    {
                        if (Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Amount Vendor"].Value) > Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Amount"].Value))
                        {
                            MessageBox.Show("Amount Vendor tidak boleh lebih besar dari Amount = '" + dgvPqDetails1.Rows[Index].Cells["Amount"].Value.ToString() + "'");
                            dgvPqDetails1.Rows[Index].Cells["Amount Vendor"].Value = dgvPqDetails1.Rows[Index].Cells["Amount"].Value;
                        }
                    }
                    else
                    {
                        dgvPqDetails1.Rows[Index].Cells["Amount Vendor"].Value = dgvPqDetails1.Rows[Index].Cells["Amount"].Value;
                    }

                }


                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Qty Vendor")
                {
                    SumFooter();
                }
                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Price")
                {
                    endEditPrice();
                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Amount Vendor")
                {
                    if (dgvPqDetails1.CurrentCell.Value == null || dgvPqDetails1.CurrentCell.Value.ToString() == "")
                    {
                        dgvPqDetails1.CurrentCell.Value = "0.00";
                    }

                    if (dgvPqDetails1.CurrentCell.Value.ToString() != "" && dgvPqDetails1.CurrentCell.Value != null)
                    {
                        //if (txtTransType.Text != "AMOUNT")
                        //{
                        //    dgvPqDetails1.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails1.CurrentCell.Value.ToString()) * Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Qty"].Value.ToString() == "" ? "0" : dgvPqDetails1.Rows[Index].Cells["Qty"].Value.ToString());
                        //}
                        if (txtTransType.Text == "AMOUNT")
                        {
                            dgvPqDetails1.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Amount Vendor"].Value.ToString() == "" ? "0" : dgvPqDetails1.Rows[Index].Cells["Amount Vendor"].Value.ToString());
                        }
                    }
                    SumFooter();
                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Disc. Type")
                {
                    //if (dgvPqDetails1.Rows[Index].Cells["DiscType"].Value.ToString() == "Select")
                    //{
                    //    dgvPqDetails1.Rows[Index].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscAmount"].Style.BackColor = Color.LightGray;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscPercent"].ReadOnly = true;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscAmount"].ReadOnly = true;
                    //}
                    //if (dgvPqDetails1.Rows[Index].Cells["DiscType"].Value.ToString() == "Percentage")
                    //{
                    //    dgvPqDetails1.Rows[Index].Cells["DiscPercent"].Style.BackColor = Color.White;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscAmount"].Style.BackColor = Color.LightGray;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscPercent"].ReadOnly = false;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscAmount"].ReadOnly = true;
                    //}
                    //if (dgvPqDetails1.Rows[Index].Cells["DiscType"].Value.ToString() == "Amount")
                    //{
                    //    dgvPqDetails1.Rows[Index].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscAmount"].Style.BackColor = Color.White;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscPercent"].ReadOnly = true;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscAmount"].ReadOnly = false;
                    //}
                    //if (dgvPqDetails1.Enabled == false)
                    //{
                    //    dgvPqDetails1.Rows[Index].Cells["DiscPercent"].Style.BackColor = Color.LightGray;
                    //    dgvPqDetails1.Rows[Index].Cells["DiscAmount"].Style.BackColor = Color.LightGray;
                    //}
                    refreshGrey();
                    SumFooter();
                }

                dtp.Visible = false;
                dgvPqDetails1.AutoResizeColumns();

            }
        }

        private void endEditPrice()
        {
            if (dgvPqDetails1.CurrentCell.Value == null || dgvPqDetails1.CurrentCell.Value.ToString() == "")
            {
                dgvPqDetails1.CurrentCell.Value = "0.00";
            }

            if (dgvPqDetails1.CurrentCell.Value != "" && dgvPqDetails1.CurrentCell.Value != null)
            {
                if (txtTransType.Text != "AMOUNT")
                {
                    dgvPqDetails1.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails1.CurrentCell.Value.ToString()) * Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Qty"].Value.ToString() == "" ? "0" : dgvPqDetails1.Rows[Index].Cells["Qty"].Value.ToString());
                }
                //else if (txtTransType.Text == "AMOUNT")
                //{
                //    dgvPqDetails1.Rows[Index].Cells["SubTotal"].Value = Convert.ToDecimal(dgvPqDetails1.CurrentCell.Value.ToString()) * Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Amount"].Value.ToString() == "" ? "0" : dgvPqDetails1.Rows[Index].Cells["Amount"].Value.ToString());
                //}
            }

            if (txtTransType.Text != "FIX")
            {
                for (int j = dgvPqDetails1.CurrentRow.Index; j < dgvPqDetails1.RowCount; j++)
                {
                    decimal TmpPriceInduk = Convert.ToDecimal(dgvPqDetails1.CurrentCell.Value);
                    if (dgvPqDetails1.Rows[j].Cells["SeqNoGroup"].Value.ToString() == dgvPqDetails1.Rows[dgvPqDetails1.CurrentCell.RowIndex].Cells["SeqNoGroup"].Value.ToString())
                    {
                        Conn = ConnectionString.GetConnection();
                        //Query = "Select Price From dbo.[InventGelombangD] where ItemId='" + dgvPqDetails1.Rows[j].Cells["FullItemID"].Value.ToString() + "' and GelombangID='" + dgvPqDetails1.Rows[j].Cells["GelombangID"].Value.ToString() + "' and BracketID='" + dgvPqDetails1.Rows[j].Cells["BracketID"].Value.ToString() + "'";
                        Query = "Select Price From dbo.[PurchRequisition_Dtl] where PurchReqId='" + txtPRId.Text + "' and SeqNo='" + dgvPqDetails1.Rows[j].Cells["ReffSeqNo"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal TmpPriceAnak = Convert.ToDecimal(Cmd.ExecuteScalar());
                        Conn.Close();
                        dgvPqDetails1.Rows[j].Cells["Price"].Value = TmpPriceInduk + TmpPriceAnak;

                        if (j == dgvPqDetails1.RowCount - 1)
                            break;
                        if (dgvPqDetails1.Rows[j + 1].Cells["Base"].Value.ToString().ToUpper() == "Y")
                            break;
                    }
                }
            }
            dgvPqDetails1.Rows[Index].Cells["Total Discount"].Value = Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Total"].Value) * Convert.ToDecimal(dgvPqDetails1.Rows[Index].Cells["Disc. %"].Value) / 100;

            SumFooter();
        }

        private void DeliveryMethod_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //dgvPqDetails1.CurrentCell.Value = DeliveryMethod.Text.ToString();
            //for (int j = 0; j < dgvPqDetails1.RowCount; j++)
            //{
            //    if (dgvPqDetails1.Rows[j].Cells["SeqNoGroup"].Value == dgvPqDetails1.Rows[dgvPqDetails1.CurrentCell.RowIndex].Cells["No"].Value.ToString())
            //    {
            //        dgvPqDetails1.Rows[j].Cells["DeliveryMethod"].Value = dgvPqDetails1.Rows[dgvPqDetails1.CurrentCell.RowIndex].Cells["DeliveryMethod"].Value.ToString();
            //    }
            //}
        }


        private void DeliveryMethod_DropDownClosed(object sender, EventArgs e)
        {
            string isi = DeliveryMethod.Text.ToString();
            int dm = dgvPqDetails1.CurrentCell.RowIndex;
            string[] answer = new string[dgvPqDetails1.RowCount];

            for (int j = 0; j < dgvPqDetails1.RowCount; j++)
            {
                answer[j] = dgvPqDetails1.Rows[j].Cells["DeliveryMethod"].Value.ToString();

            }
            DeliveryMethod.Visible = false;
            for (int j = 0; j < dgvPqDetails1.RowCount; j++)
            {
                if (j == dm)
                {
                    dgvPqDetails1.Rows[j].Cells["DeliveryMethod"].Value = isi;
                }
                else
                {
                    dgvPqDetails1.Rows[j].Cells["DeliveryMethod"].Value = answer[j];
                }
            }
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dgvPqDetails1.CurrentCell.Value = dtp.Text;
        }

        private void dgvPrDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);
            }
        }

        public void SetParent(Purchase.PurchaseQuotation.InquiryPQ F)
        {
            Parent = F;
        }

        public void SetParentRFQ(Purchase.RFQ.RFQInquiry F)
        {
            ParentRFQ = F;
        }

        private void FormPQ_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < ListInfo.Count(); i++)
            {
                ListInfo[i].Close();
            }
        }


        private void FormPQ_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 23);
        }

        private void btnSearchPR_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.Text = "Search Request For Quotation";
            tmpSearch.Order = "CreatedDate Desc";
            tmpSearch.PrimaryKey = "RfqId";
            tmpSearch.QuerySearch = "Select RfqId, RfqDate, RFQ.PurchReqId, OrderDate, RFQ.TransType, VendName, DeliveryMethod, RFQ.CreatedDate, RFQ.CreatedBy, RFQ.UpdatedDate, RFQ.UpdatedBy From RequestForQuotationH RFQ ";
            tmpSearch.QuerySearch += "Left Join [PurchRequisitionH] PR ON RFQ.PurchReqId = PR.PurchReqId ";
            tmpSearch.FilterText = new string[] { "RfqId", "RfqDate", "PurchReqId", "OrderDate", "TransType", "VendName", "DeliveryMethod", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "RFQ No", "RFQ Date", "PR No", "PR Date", "PR Type", "Vendor", "Delivery", "Created Date", "Created By", "Updated Date", "Updated By" };
            tmpSearch.Select = new string[] { "RfqId", "PurchReqId", "TransType" };
            tmpSearch.WherePlus = " And RfqID not in (select case when RfqID is null then '' else RfqID end RfqId from [PurchQuotationH]) and PurchReqId not in(select PurchReqId from CanvasSheetH)";
            tmpSearch.ShowDialog();


            if (ConnectionString.Kodes != null)
            {
                txtRfqNumber.Text = ConnectionString.Kodes[0];
                txtPRId.Text = ConnectionString.Kodes[1];
                txtTransType.Text = ConnectionString.Kodes[2];

                ConnectionString.Kodes = null;
                dgvPqDetails1.Rows.Clear();
                dgvPqDetails1.Columns.Clear();


                ResetFooter();

                if (dgvPqDetails1.RowCount - 1 <= 0)
                {
                    string[] ListColumn = new string[] { };
                    GenerateColumnDataGridView(ref ListColumn);
                }

                Conn = ConnectionString.GetConnection();

                Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, Amount, [Qty], [Unit], CASE WHEN Base = 'Y' THEN Price ELSE Price + ";
                Query += "(SELECT c.Price FROM [RequestForQuotationD] c WHERE c.RfqId = a.RfqId AND c.SeqNoGroup = a.SeqNoGroup AND c.Base = 'Y') END AS Price, Ratio, Deskripsi,b.PurchReqID,PurchReqSeqNo,b.VendId,b.VendName,b.TransType, a.DeliveryMethod, GelombangID,BracketID,BracketDesc,Base,SeqNoGroup From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId Where a.RfqId = '" + txtRfqNumber.Text + "' order by RfqSeqNo asc";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int i = 0;
                while (Dr.Read())
                {
                    if (txtTransType.Text != "AMOUNT")
                        this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Base"], Dr["Qty"], Dr["Qty"], Dr["Unit"], Dr["Ratio"], Dr["Price"], "", Dr["DeliveryMethod"], "", "0.0000", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "0", "0", "0", "0", "0", "0", "0", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], "");
                    else if (txtTransType.Text == "AMOUNT")
                        this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Base"], Dr["Amount"], Dr["Amount"], Dr["Unit"], Dr["Ratio"], Dr["Price"], "", Dr["DeliveryMethod"], "", "0.0000", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "0", "0", "0", "0", "0", "0", "0", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], "");

                    txtVendorId.Text = Dr["VendId"].ToString();
                    txtVendorName.Text = Dr["VendName"].ToString();

                    cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                    cell.Value = "Select";
                    dgvPqDetails1.Rows[(dgvPqDetails1.Rows.Count - 1)].Cells["Disc. Type"] = cell;

                    i++;
                }

                Dr.Close();

                if (dgvAttachment.RowCount - 1 <= 0)
                {
                    dgvAttachment.ColumnCount = 5;
                    dgvAttachment.Columns[0].Name = "FileName";
                    dgvAttachment.Columns[1].Name = "ContentType";
                    dgvAttachment.Columns[2].Name = "File Size (kb)";
                    dgvAttachment.Columns[3].Name = "Attachment";
                    dgvAttachment.Columns[4].Name = "Id";

                    dgvAttachment.Columns["Attachment"].Visible = false;
                    dgvAttachment.Columns["Id"].Visible = false;
                }

                SetReadOnlyDatagridView();

                for (int z = 0; z < dgvPqDetails1.ColumnCount; z++)
                {
                    dgvPqDetails1.Columns[z].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                if (dgvPqDetails1.Columns.Contains("Qty"))
                {
                    dgvPqDetails1.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPqDetails1.Columns["Qty Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (dgvPqDetails1.Columns.Contains("Amount"))
                {
                    dgvPqDetails1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPqDetails1.Columns["Amount Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                dgvPqDetails1.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["SubTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["SubTotal_PPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["SubTotal_PPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                SetVisibleDatagridView();

                tabDgvControl.SelectedIndex = 0;
                refreshGrey();
                dgvPqDetails1.AutoResizeColumns();
                SelectDgvPR();
            }
        }

        private void SetReadOnlyDatagridView()
        {
            dgvPqDetails1.ReadOnly = false;
            if (txtTransType.Text != "AMOUNT")
            {
                dgvPqDetails1.ReadOnly = false;
                dgvPqDetails1.Columns["No"].ReadOnly = true;
                dgvPqDetails1.Columns["FullItemID"].ReadOnly = true;
                dgvPqDetails1.Columns["ItemName"].ReadOnly = true;
                dgvPqDetails1.Columns["Qty"].ReadOnly = true;
                dgvPqDetails1.Columns["Price"].ReadOnly = false;
                dgvPqDetails1.Columns["PR ID"].ReadOnly = true;
                dgvPqDetails1.Columns["Unit"].ReadOnly = true;
                dgvPqDetails1.Columns["Ratio"].ReadOnly = true;
                dgvPqDetails1.Columns["Total"].ReadOnly = true;
                dgvPqDetails1.Columns["SubTotal"].ReadOnly = true;
                dgvPqDetails1.Columns["SubTotal_PPN"].ReadOnly = true;
                dgvPqDetails1.Columns["SubTotal_PPH"].ReadOnly = true;
                dgvPqDetails1.Columns["GelombangID"].ReadOnly = true;
                dgvPqDetails1.Columns["BracketID"].ReadOnly = true;
                dgvPqDetails1.Columns["BracketDesc"].ReadOnly = true;
                dgvPqDetails1.Columns["Base"].ReadOnly = true;
                dgvPqDetails1.Columns["Total"].ReadOnly = true;
            }
            else
            {
                dgvPqDetails1.ReadOnly = false;
                dgvPqDetails1.Columns["No"].ReadOnly = true;
                dgvPqDetails1.Columns["FullItemID"].ReadOnly = true;
                dgvPqDetails1.Columns["ItemName"].ReadOnly = true;
                dgvPqDetails1.Columns["Amount"].ReadOnly = true;
                dgvPqDetails1.Columns["Price"].ReadOnly = false;
                dgvPqDetails1.Columns["PR ID"].ReadOnly = true;
                dgvPqDetails1.Columns["Unit"].ReadOnly = true;
                dgvPqDetails1.Columns["Ratio"].ReadOnly = true;
                dgvPqDetails1.Columns["Total"].ReadOnly = true;
                dgvPqDetails1.Columns["SubTotal"].ReadOnly = true;
                dgvPqDetails1.Columns["SubTotal_PPN"].ReadOnly = true;
                dgvPqDetails1.Columns["SubTotal_PPH"].ReadOnly = true;
                dgvPqDetails1.Columns["GelombangID"].ReadOnly = true;
                dgvPqDetails1.Columns["BracketID"].ReadOnly = true;
                dgvPqDetails1.Columns["BracketDesc"].ReadOnly = true;
                dgvPqDetails1.Columns["Base"].ReadOnly = true;
                dgvPqDetails1.Columns["Total"].ReadOnly = true;
            }

            for (int x = 0; x < dgvPqDetails1.RowCount; x++)
            {
                if (dgvPqDetails1.Rows[x].Cells["Base"].Value == null)
                    dgvPqDetails1.Rows[x].Cells["Base"].Value = "";
                if (dgvPqDetails1.Rows[x].Cells["Base"].Value.ToString() == "N")
                {
                    dgvPqDetails1.Rows[x].Cells["DeliveryMethod"].ReadOnly = true;
                    if (dgvPqDetails1.Columns.Contains("Qty"))
                    {
                        dgvPqDetails1.Rows[x].Cells["Qty"].ReadOnly = true;
                        dgvPqDetails1.Rows[x].Cells["Qty Vendor"].ReadOnly = true;
                    }
                    if (dgvPqDetails1.Columns.Contains("Amount"))
                    {
                        dgvPqDetails1.Rows[x].Cells["Amount"].ReadOnly = true;
                        dgvPqDetails1.Rows[x].Cells["Amount Vendor"].ReadOnly = true;
                    }
                }
            }

            //not sortable
            for (int z = 0; z < dgvPqDetails1.ColumnCount; z++)
            {
                dgvPqDetails1.Columns[z].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void SetVisibleDatagridView()
        {
            if (txtTransType.Text == "FIX")
            {
                dgvPqDetails1.Columns["GroupId"].Visible = false;
                dgvPqDetails1.Columns["SubGroup1ID"].Visible = false;
                dgvPqDetails1.Columns["SubGroup2ID"].Visible = false;
                dgvPqDetails1.Columns["ItemID"].Visible = false;
                dgvPqDetails1.Columns["ReffSeqNo"].Visible = false;
                dgvPqDetails1.Columns["ReffTransType"].Visible = false;
                dgvPqDetails1.Columns["TransStatus"].Visible = false;
                dgvPqDetails1.Columns["Unit2"].Visible = false;
                dgvPqDetails1.Columns["DeliveryMethod"].Visible = true;
                dgvPqDetails1.Columns["SubTotal"].Visible = true;
                dgvPqDetails1.Columns["SubTotal_PPN"].Visible = true;
                dgvPqDetails1.Columns["SubTotal_PPH"].Visible = true;
                dgvPqDetails1.Columns["Qty"].Visible = false;
                dgvPqDetails1.Columns["Unit"].Visible = true;
                dgvPqDetails1.Columns["Qty Vendor"].Visible = true;
                dgvPqDetails1.Columns["Ratio"].Visible = false;
                dgvPqDetails1.Columns["Price"].Visible = true;
                dgvPqDetails1.Columns["AvailableDate"].Visible = true;
                dgvPqDetails1.Columns["Disc. Type"].Visible = true;
                dgvPqDetails1.Columns["Bonus Amount"].Visible = true;
                dgvPqDetails1.Columns["CashBack Amount"].Visible = true;
                dgvPqDetails1.Columns["SeqNoGroup"].Visible = false;
                dgvPqDetails1.Columns["GelombangID"].Visible = false;
                dgvPqDetails1.Columns["BracketID"].Visible = false;
                dgvPqDetails1.Columns["BracketDesc"].Visible = false;
                dgvPqDetails1.Columns["PR ID"].Visible = false;
            }
            else if (txtTransType.Text == "QTY")
            {
                dgvPqDetails1.Columns["GroupId"].Visible = false;
                dgvPqDetails1.Columns["SubGroup1ID"].Visible = false;
                dgvPqDetails1.Columns["SubGroup2ID"].Visible = false;
                dgvPqDetails1.Columns["ItemID"].Visible = false;
                dgvPqDetails1.Columns["ReffSeqNo"].Visible = false;
                dgvPqDetails1.Columns["ReffTransType"].Visible = false;
                dgvPqDetails1.Columns["TransStatus"].Visible = false;
                dgvPqDetails1.Columns["Unit2"].Visible = false;
                dgvPqDetails1.Columns["DeliveryMethod"].Visible = true;
                dgvPqDetails1.Columns["SubTotal"].Visible = true;
                dgvPqDetails1.Columns["SubTotal_PPN"].Visible = true;
                dgvPqDetails1.Columns["SubTotal_PPH"].Visible = true;
                dgvPqDetails1.Columns["Qty"].Visible = false;
                dgvPqDetails1.Columns["Unit"].Visible = true;
                dgvPqDetails1.Columns["Qty Vendor"].Visible = true;
                dgvPqDetails1.Columns["Ratio"].Visible = false;
                dgvPqDetails1.Columns["Price"].Visible = true;
                dgvPqDetails1.Columns["AvailableDate"].Visible = true;
                dgvPqDetails1.Columns["Disc. Type"].Visible = true;
                dgvPqDetails1.Columns["Bonus Amount"].Visible = true;
                dgvPqDetails1.Columns["CashBack Amount"].Visible = true;
                dgvPqDetails1.Columns["SeqNoGroup"].Visible = false;
                dgvPqDetails1.Columns["GelombangID"].Visible = false;
                dgvPqDetails1.Columns["BracketID"].Visible = false;
                dgvPqDetails1.Columns["BracketDesc"].Visible = false;
                dgvPqDetails1.Columns["PR ID"].Visible = false;
            }
            else if (txtTransType.Text == "AMOUNT")
            {
                dgvPqDetails1.Columns["GroupId"].Visible = false;
                dgvPqDetails1.Columns["SubGroup1ID"].Visible = false;
                dgvPqDetails1.Columns["SubGroup2ID"].Visible = false;
                dgvPqDetails1.Columns["ItemID"].Visible = false;
                dgvPqDetails1.Columns["ReffSeqNo"].Visible = false;
                dgvPqDetails1.Columns["ReffTransType"].Visible = false;
                dgvPqDetails1.Columns["TransStatus"].Visible = false;
                dgvPqDetails1.Columns["Unit2"].Visible = false;
                dgvPqDetails1.Columns["DeliveryMethod"].Visible = true;
                dgvPqDetails1.Columns["SubTotal"].Visible = true;
                dgvPqDetails1.Columns["SubTotal_PPN"].Visible = true;
                dgvPqDetails1.Columns["SubTotal_PPH"].Visible = true;
                dgvPqDetails1.Columns["Amount"].Visible = false;
                dgvPqDetails1.Columns["Unit"].Visible = false;
                dgvPqDetails1.Columns["Amount Vendor"].Visible = true;
                dgvPqDetails1.Columns["Ratio"].Visible = false;
                dgvPqDetails1.Columns["Price"].Visible = true;
                dgvPqDetails1.Columns["AvailableDate"].Visible = true;
                dgvPqDetails1.Columns["Disc. Type"].Visible = true;
                dgvPqDetails1.Columns["Bonus Amount"].Visible = true;
                dgvPqDetails1.Columns["CashBack Amount"].Visible = true;
                dgvPqDetails1.Columns["SeqNoGroup"].Visible = false;
                dgvPqDetails1.Columns["GelombangID"].Visible = false;
                dgvPqDetails1.Columns["BracketID"].Visible = false;
                dgvPqDetails1.Columns["BracketDesc"].Visible = false;
                dgvPqDetails1.Columns["PR ID"].Visible = false;
                SumFooter();
            }
        }

        private void SumFooter()
        {
            txtBonusScheme.Text = "0";
            txtCashBackScheme.Text = "0";
            //txtTotal.Text = "0";
            //txtDiscount.Text = "0";
            //txtTotalPPN.Text = "0";
            //txtTotalPPH.Text = "0";
            //txtGrandTotal.Text = "0";
            for (int i = 0; i < dgvPqDetails1.RowCount; i++)
            {
                if (dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value == null || dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value.ToString() == "")
                    dgvPqDetails1.Rows[i].Cells["Disc. Type"].Value = "0";
                if (dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value == null || dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value.ToString() == "")
                    dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value = "0";
                if (dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value == null || dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value.ToString() == "")
                    dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value = "0";
                if (dgvPqDetails1.Rows[i].Cells["SubTotal"].Value == null || dgvPqDetails1.Rows[i].Cells["SubTotal"].Value.ToString() == "")
                    dgvPqDetails1.Rows[i].Cells["SubTotal"].Value = "0";
                if (dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value == null || dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value.ToString() == "")
                    dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value = "0";
                if (dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value == null || dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value.ToString() == "")
                    dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value = "0";
                if (dgvPqDetails1.Rows[i].Cells["Price"].Value == null || dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString() == "")
                    dgvPqDetails1.Rows[i].Cells["Price"].Value = "0";
                if (txtTransType.Text != "AMOUNT")
                {
                    if (dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value == null || dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value.ToString() == "")
                        dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value = "0";
                }
                else if (txtTransType.Text == "AMOUNT")
                {
                    if (dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value == null || dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value.ToString() == "")
                        dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value = "0";
                }

                decimal PPh = 0;
                decimal PPN = 0;
                decimal Qty = 0;
                decimal Price = 0;
                decimal Amount = 0;


                if (cmbPPh.SelectedIndex == 0 || cmbPPh.Text == "")
                {
                    PPh = 0;
                }
                else
                {
                    PPh = Convert.ToDecimal(cmbPPh.Text);
                }
                if (cmbPPn.SelectedIndex == 0 || cmbPPn.Text == "")
                {
                    PPN = 0;
                }
                else
                {
                    PPN = Convert.ToDecimal(cmbPPn.Text);
                }

                if (txtTransType.Text != "AMOUNT")
                {
                    Qty = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value == "" || dgvPqDetails1.Rows[i].Cells["Qty"].Value == null ? "0" : dgvPqDetails1.Rows[i].Cells["Qty Vendor"].Value.ToString());
                    Price = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Price"].Value == "" || dgvPqDetails1.Rows[i].Cells["Price"].Value == null ? "0" : dgvPqDetails1.Rows[i].Cells["Price"].Value.ToString());

                    decimal DiscAmount = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Total Discount"].Value);
                    dgvPqDetails1.Rows[i].Cells["Total"].Value = (Qty * Price);
                    dgvPqDetails1.Rows[i].Cells["SubTotal"].Value = (Qty * Price) - DiscAmount;
                    dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value = ((Qty * Price) - (DiscAmount)) * PPh / 100;
                    dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value = ((Qty * Price) - (DiscAmount)) * PPN / 100;
                    txtDiscount.Text = (Convert.ToDecimal(txtDiscount.Text) + DiscAmount).ToString("N2");
                }
                else if (txtTransType.Text == "AMOUNT")
                {
                    Amount = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value == "" || dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value == null ? "0" : dgvPqDetails1.Rows[i].Cells["Amount Vendor"].Value.ToString());
                    decimal DiscAmount = Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Total Discount"].Value);
                    dgvPqDetails1.Rows[i].Cells["Total"].Value = Amount;
                    dgvPqDetails1.Rows[i].Cells["SubTotal"].Value = (Amount) - DiscAmount;
                    dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value = ((Amount) - (DiscAmount)) * PPh / 100;
                    dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value = ((Amount) - (DiscAmount)) * PPN / 100;
                    txtDiscount.Text = (Convert.ToDecimal(txtDiscount.Text) + DiscAmount).ToString("N2");
                }
                //txtDiscScheme.Text = (Convert.ToDecimal(txtDiscScheme.Text) + Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["DiscType"].Value.ToString())).ToString("N4");
                //txtTotal.Text = (Convert.ToDecimal(txtTotal.Text) + Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Total"].Value.ToString())).ToString("N2");
                //txtBonusScheme.Text = (Convert.ToDecimal(txtBonusScheme.Text) + Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["Bonus Amount"].Value.ToString())).ToString("N2");
                //txtCashBackScheme.Text = (Convert.ToDecimal(txtCashBackScheme.Text) + Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["CashBack Amount"].Value.ToString())).ToString("N2");
                //txtTotalPPN.Text = (Convert.ToDecimal(txtTotalPPN.Text) + Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["SubTotal_PPN"].Value.ToString())).ToString("N2");
                //txtTotalPPH.Text = (Convert.ToDecimal(txtTotalPPH.Text) + Convert.ToDecimal(dgvPqDetails1.Rows[i].Cells["SubTotal_PPH"].Value.ToString())).ToString("N2");

            }
            //txtGrandTotal.Text = (Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(txtDiscount.Text) + Convert.ToDecimal(txtTotalPPN.Text) + Convert.ToDecimal(txtTotalPPH.Text)).ToString("N2");
        }

        private void ResetFooter()
        {
            txtBonusScheme.Text = "0";
            txtCashBackScheme.Text = "0";
            // txtTotal.Text = "0";
            //txtTotalPPH.Text = "0";
            //txtTotalPPN.Text = "0";
        }

        private void cmbPPh_TextChanged(object sender, EventArgs e)
        {
            SumFooter();
        }

        private void cmbPPn_TextChanged(object sender, EventArgs e)
        {
            SumFooter();
        }

        private void cmbPPn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void cmbPPh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtTermOfPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDiscScheme_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtTotalPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtTotalPPH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        public string ReffID
        {
            get { return reffID; }
            set { reffID = value; }
        }

        private void tabDgvControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvPqDetails1.Columns.Count != 0)
            {
                if (tabDgvControl.SelectedTab.Text == "Detail PR")
                {
                    dgvPR.AutoResizeColumns();
                }
            }
        }

        private void cmbPPn_SelectedIndexChanged(object sender, EventArgs e)
        {
            SumFooter();
        }

        private void cmbPPh_Load()
        {
            Query = "Select [TaxStatusCode], [TaxStatusName], [TaxPercent] From dbo.[TaxGroup] where TaxStatusCode like '%PPH%' or TaxStatusCode like ''";
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPPh.Items.Add(Dr["TaxPercent"].ToString());
            }
            Dr.Close();
        }

        private void cmbPPn_Load()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select [TaxStatusCode], [TaxStatusName], [TaxPercent] From dbo.[TaxGroup] where TaxStatusCode like '%PPN%' or TaxStatusCode like ''";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPPn.Items.Add(Dr["TaxPercent"].ToString());
            }
            Dr.Close();
        }

        public void CreateNew(string RfqNumber, string PRNumber, string TransType)
        {
            Mode = "New";
            Status = "Generate";
            txtRfqNumber.Text = RfqNumber;
            txtPRId.Text = PRNumber;
            txtTransType.Text = TransType;
            txtExchRate.Text = "1";
            btnSearchPR.Visible = false;

            dgvPqDetails1.Rows.Clear();
            if (dgvPqDetails1.RowCount - 1 <= 0)
            {
                string[] ListColumn = new string[] { };
                GenerateColumnDataGridView(ref ListColumn);
            }

            Conn = ConnectionString.GetConnection();

            if (txtTransType.Text != "AMOUNT")
                Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, [Qty], [Unit], Ratio, [Price], Deskripsi,b.PurchReqID,PurchReqSeqNo,b.VendId,b.VendName,b.TransType, a.DeliveryMethod, GelombangID,BracketID,BracketDesc,Base,SeqNoGroup From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId Where a.RfqId = '" + txtRfqNumber.Text + "' order by RfqSeqNo asc";
            else if (txtTransType.Text == "AMOUNT")
                Query = "Select GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemDeskripsi, [Amount], [Unit], Ratio, [Price], Deskripsi,b.PurchReqID,PurchReqSeqNo,b.VendId,b.VendName,b.TransType, a.DeliveryMethod, GelombangID,BracketID,BracketDesc,Base,SeqNoGroup From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId Where a.RfqId = '" + txtRfqNumber.Text + "' order by RfqSeqNo asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                if (txtTransType.Text != "AMOUNT")
                    this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Base"], Dr["Qty"], Dr["Qty"], Dr["Unit"], Dr["Ratio"], Dr["Price"], "", Dr["DeliveryMethod"], "", "0.0000", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "0", "0", "0", "0", "0", "0", "0", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], "");
                else if (txtTransType.Text == "AMOUNT")
                    this.dgvPqDetails1.Rows.Add(dgvPqDetails1.Rows.Count + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["Base"], Dr["Amount"], Dr["Amount"], Dr["Unit"], Dr["Ratio"], Dr["Price"], "", Dr["DeliveryMethod"], "", "0.0000", Dr["PurchReqID"], Dr["PurchReqSeqNo"], Dr["TransType"], "", "", "0", "0", "0", "0", "0", "0", "0", Dr["GelombangID"], Dr["BracketID"], Dr["BracketDesc"], Dr["SeqNoGroup"], "");

                txtVendorId.Text = Dr["VendId"].ToString();
                txtVendorName.Text = Dr["VendName"].ToString();
                txtTransType.Text = Dr["TransType"].ToString();

                cellValue("Select [Deskripsi] from [ISBS-NEW4].[dbo].[DiskonScheme]");
                cell.Value = "Select";
                dgvPqDetails1.Rows[(dgvPqDetails1.Rows.Count - 1)].Cells["Disc. Type"] = cell;

                i++;

            }
            Dr.Close();

            dgvAttachment.Rows.Clear();
            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 5;
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "ContentType";
                dgvAttachment.Columns[2].Name = "File Size (kb)";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";

                dgvAttachment.Columns["Attachment"].Visible = false;
                dgvAttachment.Columns["Id"].Visible = false;
            }

            SetReadOnlyDatagridView();

            if (txtTransType.Text != "AMOUNT")
            {
                dgvPqDetails1.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Qty Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else if (txtTransType.Text == "AMOUNT")
            {
                dgvPqDetails1.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPqDetails1.Columns["Amount Vendor"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvPqDetails1.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal_PPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPqDetails1.Columns["SubTotal_PPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            SetVisibleDatagridView();

            dgvPqDetails1.AutoResizeColumns();
            tabDgvControl.SelectedIndex = 0;

            refreshGrey();
            SelectDgvPR();
        }

        private void txtRfqNumber_TextChanged(object sender, EventArgs e)
        {
            tabDgvControl.SelectedIndex = 0;
        }

        private void cmbPPh_SelectedIndexChanged(object sender, EventArgs e)
        {
            SumFooter();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtRfqNumber.Text == "" || txtRfqNumber.Text == null)
            {
                MessageBox.Show("Pilih RFQ terlebih dahulu.");
                return;
            }

            Purchase.PurchaseQuotation.AddItem F = new Purchase.PurchaseQuotation.AddItem();
            F.flag(txtRfqNumber.Text, txtTransType.Text);
            F.setParent(this);
            F.ShowDialog();
            SumFooter();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPqDetails1.RowCount > 0)
            {
                Index = dgvPqDetails1.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvPqDetails1.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvPqDetails1.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvPqDetails1.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (dgvPqDetails1.CurrentRow.Cells["Base"].Value.ToString() == "N")
                    {
                        dgvPqDetails1.Rows.RemoveAt(Index);

                        for (int i = 0; i < dgvPqDetails1.RowCount; i++)
                        {
                            dgvPqDetails1.Rows[i].Cells["No"].Value = i + 1;
                        }


                    }
                    else if (dgvPqDetails1.CurrentRow.Cells["Base"].Value.ToString() != "N" && txtTransType.Text.ToUpper().Trim() != "FIX")
                    {
                        DialogResult dialogResult2 = MessageBox.Show("Data tersebut adalah Base. Sehingga akan menghapus gelombangnya. Teruskan menghapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                        if (dialogResult2 == DialogResult.Yes)
                        {
                            String Group = dgvPqDetails1.CurrentRow.Cells["SeqNoGroup"].Value.ToString();
                            for (int x = 0; x < dgvPqDetails1.RowCount; x++)
                            {
                                if (dgvPqDetails1.Rows[x].Cells["SeqNoGroup"].Value.ToString() == Group)
                                {
                                    dgvPqDetails1.Rows.RemoveAt(x);
                                    x--;
                                }
                            }

                            for (int i = 0; i < dgvPqDetails1.RowCount; i++)
                            {
                                dgvPqDetails1.Rows[i].Cells["No"].Value = i + 1;
                            }
                        }
                    }
                    else
                    {
                        Index = dgvPqDetails1.CurrentRow.Index;
                        dgvPqDetails1.Rows.RemoveAt(Index);
                        for (int i = 0; i < dgvPqDetails1.RowCount; i++)
                        {
                            dgvPqDetails1.Rows[i].Cells["No"].Value = i + 1;
                        }
                    }
                    SumFooter();
                }
            }
        }

        private void txtTransType_TextChanged(object sender, EventArgs e)
        {
            //if (txtTransType.Text == "FIX")
            //{
            //    dgvPqDetails1.Columns["Base"].Visible = false;
            //}
            //else
            //{
            //    //5dgvPqDetails1.Columns["Base"].Visible = true;
            //}
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (txtRfqNumber.Text == "" || txtRfqNumber.Text == null)
            {
                MessageBox.Show("Pilih RFQ terlebih dahulu.");
                return;
            }

            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            choofdlog.FilterIndex = 3;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                FileName = new List<string>();
                Extension = new List<string>();
                sSelectedFile = new List<string>();


                int i = 0;

                foreach (string file in choofdlog.FileNames)
                {
                    FileStream objFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    int filelength = Convert.ToInt32(objFileStream.Length);
                    byte[] data = new byte[filelength];

                    objFileStream.Read(data, 0, filelength);
                    objFileStream.Close();

                    string tempFullName = Path.GetFileName(file);
                    string[] tempSplit = tempFullName.Split('.');

                    FileName.Add(tempSplit[0]);
                    Extension.Add(tempSplit[tempSplit.Count() - 1]);
                    int filesize = filelength / 1024;
                    this.dgvAttachment.Rows.Add(FileName[i], Extension[i], filesize.ToString(), System.Text.Encoding.UTF8.GetString(data));
                    test.Add(data);
                    i++;
                }

            }


        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (txtRfqNumber.Text == "" || txtRfqNumber.Text == null)
            {
                MessageBox.Show("Pilih RFQ terlebih dahulu.");
                return;
            }

            if (dgvAttachment.RowCount > 0)
            {
                String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
                String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
                String ContentType = dgvAttachment.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["ContentType"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ContentType;
                sfd.FileName = fileName + "." + ContentType;
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.AddExtension = true;

                if (ContentType == "pdf")
                {
                    sfd.FilterIndex = 1;
                }
                else if (ContentType == "txt")
                {
                    sfd.FilterIndex = 2;
                }
                else
                {
                    sfd.FilterIndex = 3;
                }

                if (String.IsNullOrEmpty(fileid))
                {
                    MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
                    return;
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                Cmd = new SqlCommand(Query, Conn);

                byte[] data = (byte[])Cmd.ExecuteScalar();

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                    objFileStream.Write(data, 0, data.Length);
                    objFileStream.Close();
                    MessageBox.Show("Data tersimpan!");
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk didownload");
                return;
            }

        }

        private void btnDelAttachment_Click(object sender, EventArgs e)
        {
            if (txtRfqNumber.Text == "" || txtRfqNumber.Text == null)
            {
                MessageBox.Show("Pilih RFQ terlebih dahulu.");
                return;
            }
            if (dgvAttachment.RowCount > 0)
            {
                if (dgvAttachment.CurrentRow.Index > -1)
                {
                    test.RemoveAt(dgvAttachment.CurrentRow.Index);
                    dgvAttachment.Rows.RemoveAt(dgvAttachment.CurrentRow.Index);
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }

        }

        private void cbByPhone_CheckedChanged(object sender, EventArgs e)
        {
            if (cbByPhone.Checked == true)
            {
                txtVendorPqNumber.Text = "By Phone";
                txtVendorPqNumber.ReadOnly = true;
            }
            else
            {
                txtVendorPqNumber.Text = "";
                txtVendorPqNumber.ReadOnly = false;
            }
        }

        private void txtVendorPqNumber_TextChanged(object sender, EventArgs e)
        {
            if (txtVendorPqNumber.Text == "By Phone")
            {
                cbByPhone.Checked = true;
            }
            else
            {
                cbByPhone.Checked = false;
            }
        }

        private void dgvPqDetails1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {
                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                {
                    PopUp.Stock.Stock PopUpStock = new PopUp.Stock.Stock();
                    if (!CheckForm(PopUpStock))
                    {
                        PopUpStock.GetData(dgvPqDetails1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvPqDetails1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        PopUpStock.Show();

                    }

                }

                if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "ReffTransID")
                {
                    Purchase.PurchaseRequisition.HeaderPR f = new Purchase.PurchaseRequisition.HeaderPR();
                    f.Close();
                    pRID = dgvPqDetails1.Rows[e.RowIndex].Cells["PR ID"].Value.ToString();
                    f = new Purchase.PurchaseRequisition.HeaderPR();
                    f.Show();
                }
            }
        }

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private bool CheckForm(Form form)
        {
            form = Application.OpenForms[form.Text];
            if (form != null)
                return true;
            else
                return false;
        }

        public static string pRID;
        public string PRId
        {
            get { return pRID; }
            set { pRID = value; }
        }

        private void dgvPqDetails1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPqDetails1.Columns.Contains("Qty"))
            {
                if (e.ColumnIndex == dgvPqDetails1.Columns["Qty"].Index)
                {
                    if (e.Value == null || e.Value.ToString() == "")
                    {
                        e.Value = "0.00";
                        return;
                    }
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N2");
                }
            }
            if (dgvPqDetails1.Columns.Contains("Amount"))
            {
                if (e.ColumnIndex == dgvPqDetails1.Columns["Amount"].Index)
                {
                    if (e.Value == null || e.Value.ToString() == "")
                    {
                        e.Value = "0.00";
                        return;
                    }
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N2");
                }
            }
            if (e.ColumnIndex == dgvPqDetails1.Columns["Price"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }

            if (e.ColumnIndex == dgvPqDetails1.Columns["SubTotal"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            if (e.ColumnIndex == dgvPqDetails1.Columns["SubTotal_PPN"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            if (e.ColumnIndex == dgvPqDetails1.Columns["SubTotal_PPH"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
        }

        private void textExchangeRate_KeyPress(object sender, KeyPressEventArgs e)
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
        }

        private void textExchangeRate_Leave(object sender, EventArgs e)
        {
            if (txtExchRate.Text != "")
            {
                double d = double.Parse(txtExchRate.Text);
                txtExchRate.Text = d.ToString("N2");
            }
        }

        private void DP_KeyPress(object sender, KeyPressEventArgs e)
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
        }

        private void DP_Leave(object sender, EventArgs e)
        {
            if (txtDPAmount.Text != "")
            {
                double d = double.Parse(txtDPAmount.Text);
                txtDPAmount.Text = d.ToString("N2");

                if (Convert.ToDecimal(txtDPAmount.Text) > 100)
                {
                    d = double.Parse("100");
                    txtDPAmount.Text = d.ToString("N2");
                }
            }
        }

        #region ListFunction
        private void AddCmbCurrency()
        {
            cmbCurrency.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select CurrencyID from [dbo].[CurrencyTable]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();

            if (cmbCurrency.Items.Contains("IDR"))
            {
                cmbCurrency.SelectedItem = "IDR";
            }
        }

        private void AddCmbTermOfPayment()
        {
            cmbTermOfPayment.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [TermOfPayment] from TermOfPayment", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbTermOfPayment.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
        }

        private void AddCmbPaymentMode()
        {
            cmbPaymentMode.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [PaymentModeName] from [dbo].[PaymentMode]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPaymentMode.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
        }

        private void AddCmbDPType()
        {
            cmbDPType.Items.Add("Percentage");
            cmbDPType.Items.Add("Amount");
            cmbDPType.SelectedIndex = 0;

            if (cmbDPRequired.Text == "")
            {
                label16.Visible = false;
                cmbDPType.Visible = false;
                txtDPPercent.Visible = false;
                label32.Visible = false;
                cbHitung.Visible = false;
                txtHitung.Visible = false;
            }
        }

        private DataGridViewComboBoxCell cellValue(string query)
        {
            cell = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            cell = new DataGridViewComboBoxCell();
            cell.Items.Add("Select");
            while (Dr2.Read())
                cell.Items.Add(Dr2[0].ToString());
            return cell;
        }
        #endregion

        private void txtDPPercent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtBonusScheme_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtCashBackScheme_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDPPercent_TextChanged(object sender, EventArgs e)
        {
            if (readDB == false)
            {
                if (txtDPPercent.Visible == true)
                {
                    if (txtDPPercent.Text == "")
                        txtDPPercent.Text = "0";
                    if (Convert.ToDecimal(txtDPPercent.Text) > 100)
                        txtDPPercent.Text = "100";
                    txtDPAmount.Text = (Convert.ToDecimal(txtGrandTotal.Text) * Convert.ToDecimal(txtDPPercent.Text) / 100).ToString("N2");
                    txtHitung.Text = txtDPAmount.Text;
                }
            }
        }

        private void txtDPAmount_TextChanged(object sender, EventArgs e)
        {
            if (readDB == false)
            {
                if (txtDPAmount.Text == "")
                    txtDPAmount.Text = "0";
                if (txtDPAmount.Visible == true)
                {
                    if (cmbDPType.Text == "Percentage")
                        txtDPPercent.Text = ((Convert.ToDecimal(txtDPAmount.Text) / Convert.ToDecimal(txtGrandTotal.Text)) * 100).ToString();
                }
            }
        }

        private void txtGrandTotal_TextChanged(object sender, EventArgs e)
        {
            if (readDB == false)
            {
                txtDPAmount.Text = (Convert.ToDecimal(txtGrandTotal.Text) * Convert.ToDecimal(txtDPPercent.Text) / 100).ToString("N2");
            }
        }

        private void cmbDPRequired_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDPRequired.Text == "NO")
            {
                cmbDPType.SelectedIndex = 0;
                label16.Visible = false;
                cmbDPType.Visible = false;
                txtDPPercent.Visible = false;
                label32.Visible = false;
                cbHitung.Visible = false;

                txtDPPercent.ReadOnly = true;
                txtDPAmount.ReadOnly = true;
                txtDPPercent.Text = "0.00";
                txtDPAmount.Text = "0.00";
            }
            else
            {
                label16.Visible = true;
                cmbDPType.Visible = true;
                txtDPPercent.Visible = true;
                label32.Visible = true;
                cbHitung.Visible = false;

                txtDPPercent.ReadOnly = false;
                txtDPAmount.ReadOnly = false;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                EditDM = true;
                dgvPqDetails1.AutoResizeColumns();
            }
            else
            {
                EditDM = false;
            }
        }

        private void dgvPqDetails1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Qty Vendor" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Price" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Amount Vendor" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Disc. %" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Total Discount" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "Bonus Amount" ||
                dgvPqDetails1.Columns[dgvPqDetails1.CurrentCell.ColumnIndex].Name == "CashBack Amount"
                )
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

        private void dgvPqDetails1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPqDetails1_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPqDetails1_KeyPress);
            }
        }

        private void dgvPqDetails1_CellFormatting_1(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value.ToString() != "")
            {
                if (dgvPqDetails1.Columns.Contains("Qty Vendor"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["Qty Vendor"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("Amount Vendor"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["Amount Vendor"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("Disc. %"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["Disc. %"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("Total Discount"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["Total Discount"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("Price"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["Price"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("Total"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["Total"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("Bonus Amount"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["Bonus Amount"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("CashBack Amount"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["CashBack Amount"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }

                if (dgvPqDetails1.Columns.Contains("SubTotal"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["SubTotal"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }

                if (dgvPqDetails1.Columns.Contains("SubTotal_PPN"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["SubTotal_PPN"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
                if (dgvPqDetails1.Columns.Contains("SubTotal_PPH"))
                {
                    if (e.ColumnIndex == dgvPqDetails1.Columns["SubTotal_PPH"].Index && e.Value != null)
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }

            }
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCurrency.Text == "IDR")
            {
                txtExchRate.Text = "1";
                txtExchRate.ReadOnly = true;
            }
            else
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("select [ExchRate] from [dbo].[ExchRate] where CurrencyId='" + cmbCurrency.Text + "' and convert(varchar(10), CreatedDate, 102)=convert(varchar(10), getdate(), 102)", Conn);
                if (Cmd.ExecuteScalar() != null)
                {
                    txtExchRate.Text = Cmd.ExecuteScalar().ToString();
                    txtExchRate.ReadOnly = true;
                }
                else
                {
                    MessageBox.Show("ExchRate (" + cmbCurrency.Text + ") untuk hari ini belum di set, silahkan diisi sendiri.");
                    txtExchRate.ReadOnly = false;
                }
                Conn.Close();
            }

        }

        private void txtDPAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDPAmount_Leave(object sender, EventArgs e)
        {
            txtDPAmount.Text = Convert.ToDecimal(txtDPAmount.Text).ToString("N2");
        }

        private void txtDPPercent_Leave(object sender, EventArgs e)
        {
            txtDPPercent.Text = Convert.ToDecimal(txtDPPercent.Text).ToString("N2");
        }

        private void txtDPAmount_Enter(object sender, EventArgs e)
        {
            txtDPAmount.Text = Convert.ToDecimal(txtDPAmount.Text).ToString();
        }

        private void txtDPPercent_Enter(object sender, EventArgs e)
        {
            txtDPPercent.Text = Convert.ToDecimal(txtDPPercent.Text).ToString();
        }

        private void dgvPqDetails1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 1)
                return;

            if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "Price")
            {
                string tmpFullItemId = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["FullItemID"].Value.ToString();
                string tmpGelombangId = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["GelombangID"].Value.ToString();
                string tmpBracketId = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["BracketID"].Value.ToString();
                string tmpDeliveryMethod = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["DeliveryMethod"].Value.ToString();

                if (tmpDeliveryMethod == "") //user harus pilih deliv method baru bisa dapat harga
                {
                    MessageBox.Show("Pilih Delivery Method dahulu..");
                    return;
                }

                SearchQueryV1 tmpSearch = new SearchQueryV1();
                tmpSearch.Text = "Search Price List";
                tmpSearch.Order = "CreatedDate Desc";
                tmpSearch.PrimaryKey = "PriceListNo";

                tmpSearch.QuerySearch = "Select a.PriceListNo, a.PricelistDate, FullItemId, Price, b.ValidFrom, b.ValidTo , a.CreatedBy ,a.CreatedDate, a.UpdatedBy, a.UpdatedDate, a.Type From [dbo].[Pricelist_Dtl] a ";
                tmpSearch.QuerySearch += "LEFT JOIN PricelistH b ON b.PriceListNo = a.PriceListNo ";
                tmpSearch.QuerySearch += "LEFT JOIN PriceList_AccountList c ON a.PriceListNo = c.PriceListNo ";
                tmpSearch.QuerySearch += " WHERE FullItemId = '" + tmpFullItemId + "' AND GelombangId = '" + tmpGelombangId + "' AND BracketID='" + tmpBracketId + "' AND a.Type = 'PURCHASE' AND b.Active=1 AND ";
                tmpSearch.QuerySearch += " CAST(ValidFrom AS DATE) <= '" + dtPqDate.Value.Date + "' AND CAST(ValidTo AS DATE) >= '" + dtPqDate.Value.Date + "' AND ";
                tmpSearch.QuerySearch += " a.[DeliveryMethod] = '" + tmpDeliveryMethod + "' AND";
                tmpSearch.QuerySearch += "((b.Criteria = 1)OR(b.Criteria = 3 AND c.AccountId = '" + txtVendorId.Text + "')OR(b.Criteria= 2 AND c.AccountId != '" + txtVendorId.Text + "') )";

                tmpSearch.FilterText = new string[] { "PriceListNo", "FullItemId" };
                tmpSearch.Select = new string[] { "Price" };
                //tmpSearch.WherePlus = " and FullItemId = '" + tmpFullItemId +"' and a.Type = 'PURCHASE' ";
                //tmpSearch.WherePlus += " and (b.ValidFrom <= getdate() And b.ValidTo >= getdate())";
                tmpSearch.ShowDialog();

                //if (ConnectionString.Kodes != null)
                //{
                //    dgvPqDetails1.CurrentRow.Cells["Price"].Value = ConnectionString.Kodes[0];

                //    ConnectionString.Kodes = null;
                //} 

                decimal PriceKG = 0;
                decimal PriceAlt = 0;
                string tmpUnit = dgvPqDetails1.Rows[dgvPqDetails1.CurrentRow.Index].Cells["Unit"].Value.ToString();
                string tmpFromUnit = "";
                string tmpToUnit = "";
                decimal tmpRatio = 0;


                Conn = ConnectionString.GetConnection();
                Query = "Select FromUnit, ToUnit, Ratio from dbo.[InventConversion] Where FullItemId = '" + tmpFullItemId + "'";
                Cmd = new SqlCommand(Query, ConnectionString.GetConnection());
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    tmpFromUnit = Dr["FromUnit"].ToString();
                    tmpToUnit = Dr["ToUnit"].ToString();
                    tmpRatio = Convert.ToDecimal(Dr["Ratio"].ToString());
                }
                Dr.Close();


                if (ConnectionString.Kodes != null)
                {
                    PriceKG = Convert.ToDecimal(ConnectionString.Kodes[0]);
                    if (tmpUnit == "KG")
                    {
                        dgvPqDetails1.CurrentRow.Cells["Price"].Value = PriceKG;
                        //dgvPqDetails1.CurrentRow.Cells["Total"].Value = PriceKG * Convert.ToDecimal(dgvPqDetails1.CurrentRow.Cells["Qty"].Value);
                    }
                    else
                    {
                        if (tmpFromUnit == "KG")
                        {
                            PriceAlt = PriceKG * tmpRatio;
                        }
                        else if (tmpToUnit == "KG")
                        {
                            PriceAlt = PriceKG / tmpRatio;
                        }
                        dgvPqDetails1.CurrentRow.Cells["Price"].Value = PriceAlt;
                    }
                    ConnectionString.Kodes = null;
                    endEditPrice();
                }

            }


        }
        //tia edit
        //klik kanan
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.CustomerID.Customer Cust = null;
        PopUp.Vendor.Vendor Vendor = null;
        Purchase.RFQ.RFQForm RFQId = null;
        Purchase.PurchaseRequisition.HeaderPR PRid = null;


        //public static string itemID;
        //public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvPqDetails1_CellMouseDown_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {

                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPqDetails1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        //itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvPqDetails1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
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


        private void txtVendorId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {

                    txtVendorId.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendorId.Text);

                    Vendor.Show();
                    //}
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void txtVendorName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {

                    txtVendorName.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendorId.Text);
                    Vendor.Show();
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void txtPRId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (PRid == null || PRid.Text == "")
                {
                    txtPRId.Enabled = true;
                    PRid = new Purchase.PurchaseRequisition.HeaderPR();
                    PRid.SetMode("PopUp", txtPRId.Text);
                    PRid.ParentRefreshGrid(this);
                    PRid.Show();
                }
                else if (CheckOpened(PRid.Name))
                {
                    PRid.WindowState = FormWindowState.Normal;
                    PRid.Show();
                    PRid.Focus();
                }
            }
        }


        private void txtRfqNumber_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (RFQId == null || RFQId.Text == "")
                {
                    txtRfqNumber.Enabled = true;
                    RFQId = new Purchase.RFQ.RFQForm();
                    //RFQId.GetData(txtPRId.Text);
                    RFQId.flag(txtRfqNumber.Text, "PopUp");
                    RFQId.Show();
                    RFQId.ParentRefreshGrid(this);

                }
                else if (CheckOpened(RFQId.Name))
                {
                    RFQId.WindowState = FormWindowState.Normal;
                    RFQId.Show();
                    RFQId.Focus();
                }
            }
        }

        private void cmbDPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDPType.Text == "Percentage")
            {
                txtDPPercent.Visible = true;
                label32.Visible = true;
                txtDPAmount.Visible = false;
                cbHitung.Visible = false;

                if (Mode != "BeforeEdit")
                {
                    txtDPAmount.Text = "0.00";
                    txtDPPercent.Text = "0.00";
                }
            }
            else
            {
                cbHitung.Checked = false;
                txtDPPercent.Visible = false;
                label32.Visible = false;
                txtDPAmount.Visible = true;
                cbHitung.Visible = false;

                if (Mode != "BeforeEdit")
                {
                    txtDPAmount.Text = "0.00";
                    txtDPPercent.Text = "0.00";
                }
            }
        }

        private void cbHitung_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHitung.Checked == true)
            {
                txtHitung.Text = txtDPAmount.Text;
                txtHitung.Visible = true;
            }
            else
            {
                txtHitung.Text = "0";
                txtHitung.Visible = false;
            }
        }

        private void dgvPqDetails1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (EditDM == true)
            {
                if (txtTransType.Text != "FIX")
                {
                    for (int i = dgvPqDetails1.CurrentCell.RowIndex; i < dgvPqDetails1.RowCount; i++)
                    {
                        if (dgvPqDetails1.Columns[e.ColumnIndex].Name.ToString().Contains("DeliveryMethod"))
                        {
                            if (dgvPqDetails1.Rows[i].Cells["SeqNoGroup"].Value.ToString() == dgvPqDetails1.Rows[dgvPqDetails1.CurrentCell.RowIndex].Cells["SeqNoGroup"].Value.ToString())
                            {
                                dgvPqDetails1.Rows[i].Cells["DeliveryMethod"].Value = dgvPqDetails1.Rows[dgvPqDetails1.CurrentCell.RowIndex].Cells["DeliveryMethod"].Value.ToString();

                                if (i == dgvPqDetails1.RowCount - 1)
                                    break;
                                if (dgvPqDetails1.Rows[i + 1].Cells["Base"].Value.ToString().ToUpper() == "Y")
                                    break;
                            }
                        }
                    }
                }
            }
        }
        //end
    }
}
