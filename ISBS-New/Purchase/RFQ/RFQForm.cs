using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Transactions;

namespace ISBS_New.Purchase.RFQ
{
    public partial class RFQForm : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, Query1;
        int Limit1, Limit2, Total, Page1, Page2, Index;

        public string tmpTransType = "";

        List<InfoRFQ> ListInfo = new List<InfoRFQ>();
        ComboBox DeliveryMethod;
        String RFQId = null;
        String PurchReqId = null;
        String TransType = null;
        Purchase.RFQ.RFQInquiry Parent;
        Purchase.PurchaseRequisitionApproval.InquiryPRApproval Parent2;
        //tia edit
        Purchase.PurchaseQuotation.FormPQ Parent3;
        ContextMenu vendid = new ContextMenu();
        //tia edit end

        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public RFQForm()
        {
            InitializeComponent();
        }

        public void flag(String rfqid, String mode)
        {
            RFQId = rfqid;
            Mode = mode;
        }

        public void flag2(String purhcreqid, String transtype, String mode)
        {
            PurchReqId = purhcreqid;
            TransType = transtype;
            Mode = mode;
        }

        private void RFQForm_Load(object sender, EventArgs e)
        {
            this.Location = new Point(40, 7);

            if (Mode == "New")
            {
                ModeNew();
                //AddCmbType();
            }
            else if (Mode == "Generate")
            {
                ModeNew();
                txtPurchReqID.Text = PurchReqId;
                txtPurchReqID.Enabled = false;
                btnSearchPurchReqID.Visible = false;

               
                GeneratePR();
                GenerateVendor();
                //AddCmbType();
            }
            else
            {
                RefreshGrid();
                GetDataHeader();
            }
            if (Mode != "New")
                gvHeader();
            DeliveryMethod = new ComboBox();
            DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            DeliveryMethod.Visible = false;
            dgvDetails.Controls.Add(DeliveryMethod);
            DeliveryMethod.SelectedIndexChanged += this.DeliveryMethod_SelectedIndexChanged;
            //tia edit
            txtPurchReqID.Enabled = true;
            txtPurchReqID.ReadOnly = true;
            txtPurchReqID.ContextMenu = vendid;
            //tia edit end
            CheckCollapse();
        }

        private void DeliveryMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvDetails.CurrentCell.Value = DeliveryMethod.Text.ToString();
        }    

        private void gvHeader()
        {
            dgvDetails.Columns["PurchReqID"].HeaderText = "PR ID";
            dgvDetails.Columns["FullItemID"].HeaderText = "Item ID";
            dgvDetails.Columns["ItemDeskripsi"].HeaderText = "Item Description";
            dgvDetails.Columns["DeliveryMethod"].HeaderText = "Delivery Method";
            if(dgvDetails.Columns.Contains("Qty"))
                dgvDetails.Columns["Qty"].HeaderText = "Quantity";
            dgvDetails.Columns["Deskripsi"].HeaderText = "Description";
            dgvDetails.Columns["GelombangId"].HeaderText = "Gelombang ID";
            dgvDetails.Columns["BracketID"].HeaderText = "Bracket ID";
        }

        private void GeneratePR()
        {
            txtTransType.Text = TransType;

            if (dgvDetails.RowCount - 1 <= 0)
            {
                dgvDetails.ColumnCount = 19;
                dgvDetails.Columns[0].Name = "No";
                dgvDetails.Columns[1].Name = "RfqSeqNo";
                dgvDetails.Columns[2].Name = "PurchReqID";
                dgvDetails.Columns[3].Name = "GroupId";
                dgvDetails.Columns[4].Name = "SubGroup1Id";
                dgvDetails.Columns[5].Name = "SubGroup2ID";
                dgvDetails.Columns[6].Name = "ItemID";
                dgvDetails.Columns[7].Name = "FullItemID";
                dgvDetails.Columns[8].Name = "ItemDeskripsi";
                dgvDetails.Columns[9].Name = "DeliveryMethod";
                if (txtTransType.Text != "AMOUNT")
                    dgvDetails.Columns[10].Name = "Qty";
                else if (txtTransType.Text == "AMOUNT")
                    dgvDetails.Columns[10].Name = "Amount";
                dgvDetails.Columns[11].Name = "Unit";
                dgvDetails.Columns[12].Name = "Deskripsi";
                dgvDetails.Columns[13].Name = "GelombangId";
                dgvDetails.Columns[14].Name = "BracketId";
                dgvDetails.Columns[15].Name = "Base";
                dgvDetails.Columns[16].Name = "Price";
                dgvDetails.Columns[17].Name = "SeqNoGroup";
                dgvDetails.Columns[18].Name = "Ratio";
                dgvDetails.Columns[0].Width = 40;
                dgvDetails.Columns[7].Width = 110;
                dgvDetails.Columns[8].Width = 200;
                dgvDetails.Columns[10].Width = 60;
                dgvDetails.Columns[11].Width = 50;
                dgvDetails.Columns[13].Width = 90;
                dgvDetails.Columns[14].Width = 60;
                dgvDetails.Columns[15].Width = 40;

            }
            Query = "Select SeqNoGroup from PurchRequisition_Dtl where PurchReqId = '" + PurchReqId + "' and TransStatus = 'Yes'";
            Conn = ConnectionString.GetConnection();
            string TmpSeqNoGroup = "";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (TmpSeqNoGroup == "")
                        TmpSeqNoGroup = "'" + Dr[0].ToString() + "'";
                    else
                        TmpSeqNoGroup += ",'" + Dr[0].ToString() + "'";
                }
            }
            Dr.Close();

            Query = "Select SeqNo, PurchReqId, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi, GelombangID, BracketID, Base, Price, SeqNoGroup, Ratio From [dbo].[PurchRequisition_Dtl] Where PurchReqId = '" + PurchReqId + "' and SeqNoGroup in (" + TmpSeqNoGroup + ")";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    String PurchReqId1 = Dr["PurchReqId"] == null ? "" : Dr["PurchReqId"].ToString();
                    String RfqSeqNo1 = Dr["SeqNo"] == null ? "" : Dr["SeqNo"].ToString();
                    String GroupId = Dr["GroupId"] == null ? "" : Dr["GroupId"].ToString();
                    String SubGroup1Id = Dr["SubGroup1Id"] == null ? "" : Dr["SubGroup1Id"].ToString();
                    String SubGroup2Id = Dr["SubGroup2Id"] == null ? "" : Dr["SubGroup2Id"].ToString();
                    String ItemId = Dr["ItemId"] == null ? "" : Dr["ItemId"].ToString();
                    String FullItemId = Dr["FullItemId"] == null ? "" : Dr["FullItemId"].ToString();
                    String ItemDeskripsi = Dr["ItemName"] == null ? "" : Dr["ItemName"].ToString();
                    String DeliveryMethod = Dr["DeliveryMethod"] == null ? "" : Dr["DeliveryMethod"].ToString();
                    String Qty ="";
                    if (txtTransType.Text != "AMOUNT")
                        Qty = Dr["Qty"] == null ? "" : Dr["Qty"].ToString();
                    if (txtTransType.Text == "AMOUNT")
                        Qty = Dr["Amount"] == null ? "" : Dr["Amount"].ToString();
                    String Unit = Dr["Unit"] == null ? "" : Dr["Unit"].ToString();
                    String Deskripsi = Dr["Deskripsi"] == null ? "" : Dr["Deskripsi"].ToString();
                    String GelombangId = Dr["GelombangID"] == null ? "" : Dr["GelombangID"].ToString();
                    String BracketId = Dr["BracketID"] == null ? "" : Dr["BracketID"].ToString();
                    String Base = Dr["Base"] == null ? "" : Dr["Base"].ToString();
                    String Price = Dr["Price"] == null ? "" : Dr["Price"].ToString();
                    String SeqNoGroup = Dr["SeqNoGroup"] == null ? "" : Dr["SeqNoGroup"].ToString();
                    String Ratio = Dr["Ratio"] == null ? "" : Dr["Ratio"].ToString();

                    this.dgvDetails.Rows.Add((dgvDetails.RowCount + 1).ToString(), RfqSeqNo1, PurchReqId1, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemDeskripsi, DeliveryMethod, Qty, Unit, Deskripsi, GelombangId, BracketId, Base, Price, SeqNoGroup, Ratio);
                    if (dgvDetails.Rows[(dgvDetails.Rows.Count - 1)].Cells["Base"].Value.ToString() == "N")
                    {
                        dgvDetails.Rows[(dgvDetails.Rows.Count - 1)].DefaultCellStyle.BackColor = Color.LightGray;
                        dgvDetails.Rows[(dgvDetails.Rows.Count - 1)].ReadOnly = true;
                    }
                }
            }
            Conn.Close();

            dgvDetails.Columns["RfqSeqNo"].Visible = false;
            dgvDetails.Columns["GroupId"].Visible = false;
            dgvDetails.Columns["SubGroup1Id"].Visible = false;
            dgvDetails.Columns["SubGroup2Id"].Visible = false;
            dgvDetails.Columns["ItemId"].Visible = false;
            dgvDetails.Columns["SeqNoGroup"].Visible = false;
            dgvDetails.Columns["Ratio"].Visible = false;

            dgvDetails.ReadOnly = false;
            dgvDetails.Columns["No"].ReadOnly = true;
            dgvDetails.Columns["PurchReqID"].ReadOnly = true;
            dgvDetails.Columns["FullItemID"].ReadOnly = true;
            dgvDetails.Columns["DeliveryMethod"].ReadOnly = true;
            dgvDetails.Columns["ItemDeskripsi"].ReadOnly = true;
            dgvDetails.Columns["Unit"].ReadOnly = true;
            dgvDetails.Columns["Deskripsi"].ReadOnly = true;
            dgvDetails.Columns["GelombangID"].ReadOnly = true;
            dgvDetails.Columns["BracketID"].ReadOnly = true;
            dgvDetails.Columns["Base"].ReadOnly = true;
            dgvDetails.Columns["Price"].ReadOnly = true;
            if (txtTransType.Text != "AMOUNT")
            {
                dgvDetails.Columns["Qty"].ReadOnly = true;
                dgvDetails.Columns["Qty"].DefaultCellStyle.Format = "N2";
            }
            if (txtTransType.Text == "AMOUNT")
            {
                dgvDetails.Columns["Amount"].ReadOnly = true;
                dgvDetails.Columns["Amount"].DefaultCellStyle.Format = "N2";
            }
            dgvDetails.Columns["Price"].DefaultCellStyle.Format = "N4";
            //Conn = ConnectionString.GetConnection();

            //if (dgvDetails.RowCount - 1 <= 0)
            //{
            //    dgvDetails.ColumnCount = 9;
            //    dgvDetails.Columns[0].Name = "No";
            //    dgvDetails.Columns[1].Name = "PurchReqID";
            //    dgvDetails.Columns[2].Name = "PurchReqSeqNo";
            //    dgvDetails.Columns[3].Name = "FullItemID";
            //    dgvDetails.Columns[4].Name = "ItemDeskripsi";
            //    dgvDetails.Columns[5].Name = "DeliveryMethod";
            //    dgvDetails.Columns[6].Name = "Qty";
            //    dgvDetails.Columns[7].Name = "Unit";
            //    dgvDetails.Columns[8].Name = "Deskripsi";
            //    dgvDetails.Columns[0].Width = 40;
            //}

            //Query = "Select * From(Select ROW_NUMBER() OVER (ORDER BY PurchReqId) No, PurchReqId, SeqNo, FullItemId, ItemName, DeliveryMethod, Qty, Unit, Deskripsi From [dbo].[PurchRequisition_Dtl] Where PurchReqId = '" + PurchReqId + "' And TransStatusPurch = 'Yes') a";
            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();

            //while (Dr.Read())
            //{
            //    this.dgvDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8]);
            //}

            //dgvDetails.ReadOnly = false;
            //dgvDetails.Columns["No"].ReadOnly = true;
            //dgvDetails.Columns["PurchReqID"].ReadOnly = true;
            //dgvDetails.Columns["PurchReqSeqNo"].ReadOnly = true;
            //dgvDetails.Columns["FullItemID"].ReadOnly = true;
            //dgvDetails.Columns["DeliveryMethod"].ReadOnly = true;
            //dgvDetails.Columns["ItemDeskripsi"].ReadOnly = true;
            //dgvDetails.Columns["Unit"].ReadOnly = true;
            //dgvDetails.Columns["Deskripsi"].ReadOnly = true;
        }

        private void GenerateVendor()
        {

            if (dgvVendor.RowCount - 1 <= 0)
            {
                dgvVendor.ColumnCount = 3;
                dgvVendor.Columns[0].Name = "No";
                dgvVendor.Columns[1].Name = "Vendor";
                dgvVendor.Columns[2].Name = "VendName";
                dgvVendor.Columns[0].Width = 40;
            }

            Query = "Select [VendId] From [dbo].[PurchRequisition_Dtl] where [PurchReqId] = '" + PurchReqId + "' ";
            string TmpVendor = "";
            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();


                while (Dr.Read())
                {
                    if (TmpVendor == "")
                        TmpVendor = Dr[0].ToString();
                    else
                        TmpVendor += ";" + Dr[0].ToString();
                }
            }
            Dr.Close();

            string[] TmpVendId = TmpVendor.Split(';');
            for (int i = 0; i < TmpVendId.Count(); i++)
            {
                if (i == 0)
                    TmpVendor = "'" + TmpVendId[i].ToString() + "'";
                else
                    TmpVendor += ",'" + TmpVendId[i].ToString() + "'";
            }

            Query = "Select [VendId],VendName From [dbo].[VendTable] where [VendId] in (" + TmpVendor + ") ";

            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {

                    this.dgvVendor.Rows.Add((dgvVendor.RowCount + 1).ToString(), Dr[0].ToString(), Dr[1].ToString());

                }
            }
            Conn.Close();
            //Conn = ConnectionString.GetConnection();

            //if (dgvVendor.RowCount - 1 <= 0)
            //{
            //    dgvVendor.ColumnCount = 3;
            //    dgvVendor.Columns[0].Name = "No";
            //    dgvVendor.Columns[1].Name = "VendId";
            //    dgvVendor.Columns[2].Name = "VendName";
            //    dgvVendor.Columns[0].Width = 40;
            //}

            //Query = "Select * From(Select ROW_NUMBER() OVER (ORDER BY a.[VendId]) No, a.[VendId], b.[VendName] From [dbo].[PurchRequisition_Dtl] a JOIN [dbo].[VendTable] b ON a.[VendID] = b.[VendID]  where a.[PurchReqId] = '"+ PurchReqId +"' )c";
            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();

            //while (Dr.Read())
            //{
            //    this.dgvVendor.Rows.Add(Dr[0], Dr[1], Dr[2]);
            //}
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[RequestForQuotationH] Where RfqId = '" + RFQId + "' ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtRFQID.Text = RFQId.ToString();
                    dtRFQDate.Value = (DateTime)Dr["RfqDate"];
                    txtVendorID.Text = Dr["VendID"].ToString();
                    txtVendorName.Text = Dr["VendName"].ToString();
                    txtPurchReqID.Text = Dr["PurchReqId"].ToString();
                    txtTransType.Text = Dr["TransType"].ToString();
                    rtxtNotes.Text = Dr["Notes"].ToString();
                }
            }
            Conn.Close();

            if (Mode == "Edit" || Mode == "View")
            {
                grpVendor.Visible = false;
                ////tia edit
                //txtPurchReqID.Enabled = true;
                //txtPurchReqID.ReadOnly = true;
                //txtVendorID.Enabled = true;
                //txtVendorID.ReadOnly = true;
                //txtVendorName.Enabled = true;
                //txtVendorName.ReadOnly = true;

                //txtVendorID.ContextMenu = vendid;
                //txtVendorName.ContextMenu = vendid;
                //txtPurchReqID.ContextMenu = vendid;

            }
            else if (Mode =="PopUp")
            {
        //tia edit
                ModePopUp();
            }
            
        }
        private void ModePopUp()
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            grpVendor.Visible = false;
            
            txtPurchReqID.Enabled = true;
            txtPurchReqID.ReadOnly = true;
            txtVendorID.Enabled = true;
            txtVendorID.ReadOnly = true;
            txtVendorName.Enabled = true;
            txtVendorName.ReadOnly = true;
            //btnExit.Visible = false;
            btnEdit.Visible = false;

            txtVendorID.ContextMenu = vendid;
            txtVendorName.ContextMenu = vendid;
            txtPurchReqID.ContextMenu = vendid;
        }

        public void ParentRefreshGrid(Purchase.PurchaseQuotation.FormPQ pq)
        {
            Parent3 = pq;
        }
        //tia edit end
        public void ParentRefreshGrid(Purchase.RFQ.RFQInquiry f)
        {
            Parent = f;
        }

        public void setParent(Purchase.PurchaseRequisitionApproval.InquiryPRApproval G)
        {
            Parent2 = G;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Purchase.RFQ.AddPR F = new Purchase.RFQ.AddPR();
            F.ParentRefreshGrid(this);
            F.flag(TransType);
            F.ShowDialog();
        }

        public string getPurchReqId()
        {
            string PurchReqId = "";

            if (dgvDetails.RowCount > 0)
            {
                for (int i = 0; i <= dgvDetails.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        PurchReqId += "(a.PurchReqId <> '";
                        PurchReqId += dgvDetails.Rows[i].Cells["PurchReqId"].Value == null ? "" : dgvDetails.Rows[i].Cells["PurchReqId"].Value.ToString();
                        PurchReqId += "' or a.SeqNo <> '";
                        PurchReqId += dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value.ToString());
                        PurchReqId += "')";
                    }
                    else
                    {
                        PurchReqId += " and (a.PurchReqId <> '";
                        PurchReqId += dgvDetails.Rows[i].Cells["PurchReqId"].Value == null ? "" : dgvDetails.Rows[i].Cells["PurchReqId"].Value.ToString();
                        PurchReqId += "' or a.SeqNo <> '";
                        PurchReqId += dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value.ToString());
                        PurchReqId += "')";
                    }
                }
                //PurchReqId += ")";
                return PurchReqId;
            }
            else
            {
                PurchReqId = "(a.PurchReqId <> '' or a.SeqNo <> '0')";
                return PurchReqId;
            }
        }

        //public string getSeqNo()
        //{
        //    string PurchReqSeqNo = "";

        //    if (dgvDetails.RowCount > 0)
        //    {
        //        for (int i = 0; i <= dgvDetails.RowCount - 1; i++)
        //        {
        //            if (i == 0)
        //            {
        //                PurchReqSeqNo += "a.SeqNo not in ('";
        //                PurchReqSeqNo += dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value.ToString());
        //                PurchReqSeqNo += "'";
        //            }
        //            else
        //            {
        //                PurchReqSeqNo += ",'";
        //                PurchReqSeqNo += dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[i].Cells["PurchReqSeqNo"].Value.ToString());
        //                PurchReqSeqNo += "'";
        //            }
        //        }
        //        PurchReqSeqNo += ")";
        //        return PurchReqSeqNo;
        //    }
        //    else
        //    {
        //        PurchReqSeqNo = "a.SeqNo not in('0')";
        //        return PurchReqSeqNo;
        //    }
        //}

        public string getVendId()
        {
            string VendId = "";

            if (dgvVendor.RowCount > 0)
            {
                for (int i = 0; i <= dgvVendor.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        VendId += "VendId not in ('";
                        VendId += dgvVendor.Rows[i].Cells["Vendor"].Value == null ? "" : dgvVendor.Rows[i].Cells["Vendor"].Value.ToString();
                        VendId += "'";
                    }
                    else
                    {
                        VendId += ",'";
                        VendId += dgvVendor.Rows[i].Cells["Vendor"].Value == null ? "" : dgvVendor.Rows[i].Cells["Vendor"].Value.ToString();
                        VendId += "'";
                    }
                }
                VendId += ")";
                return VendId;
            }
            else
            {
                VendId = "VendId not in('')";
                return VendId;
            }
        }

        public void GetDataHeader()
        {
            if (RFQId != "")
            {
                Conn = ConnectionString.GetConnection();

                if (dgvDetails.RowCount - 1 <= 0)
                {
                    dgvDetails.ColumnCount = 19;
                    dgvDetails.Columns[0].Name = "No";
                    dgvDetails.Columns[1].Name = "RfqSeqNo";
                    dgvDetails.Columns[2].Name = "PurchReqID";
                    dgvDetails.Columns[3].Name = "GroupId";
                    dgvDetails.Columns[4].Name = "SubGroup1Id";
                    dgvDetails.Columns[5].Name = "SubGroup2ID";
                    dgvDetails.Columns[6].Name = "ItemID";
                    dgvDetails.Columns[7].Name = "FullItemID";
                    dgvDetails.Columns[8].Name = "ItemDeskripsi";
                    dgvDetails.Columns[9].Name = "DeliveryMethod";
                    if(txtTransType.Text !="AMOUNT")
                        dgvDetails.Columns[10].Name = "Qty";
                    else if (txtTransType.Text == "AMOUNT")
                        dgvDetails.Columns[10].Name = "Amount";
                    dgvDetails.Columns[11].Name = "Unit";
                    dgvDetails.Columns[12].Name = "Deskripsi";
                    dgvDetails.Columns[13].Name = "GelombangId";
                    dgvDetails.Columns[14].Name = "BracketId";
                    dgvDetails.Columns[15].Name = "Base";
                    dgvDetails.Columns[16].Name = "Price";
                    dgvDetails.Columns[17].Name = "SeqNoGroup";
                    dgvDetails.Columns[18].Name = "Ratio";
                    //dgvDetails.Columns[0].Width = 40;
                    //dgvDetails.Columns[7].Width = 110;
                    //dgvDetails.Columns[8].Width = 200;
                    //dgvDetails.Columns[10].Width = 60;
                    //dgvDetails.Columns[11].Width = 50;
                    //dgvDetails.Columns[13].Width = 90;
                    //dgvDetails.Columns[14].Width = 60;
                    //dgvDetails.Columns[15].Width = 40;
                    dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }

                if (txtTransType.Text != "AMOUNT")
                    Query = "Select * From(Select ROW_NUMBER() OVER (ORDER BY PurchReqId) No, RfqSeqNo, PurchReqId, GroupId, SubGroup1ID, SubGroup2Id, ItemID, FullItemId, ItemDeskripsi, DeliveryMethod, ISNULL(CAST(Qty as varchar),'')  Qty, Unit, Deskripsi, GelombangID, BracketID, Base, Price, SeqNoGroup, Ratio From [dbo].[RequestForQuotationD] Where RfqId = '" + RFQId + "') a";
                else if(txtTransType.Text =="AMOUNT")
                    Query = "Select * From(Select ROW_NUMBER() OVER (ORDER BY PurchReqId) No, RfqSeqNo, PurchReqId, GroupId, SubGroup1ID, SubGroup2Id, ItemID, FullItemId, ItemDeskripsi, DeliveryMethod, Amount, Unit, Deskripsi, GelombangID, BracketID, Base, Price, SeqNoGroup, Ratio From [dbo].[RequestForQuotationD] Where RfqId = '" + RFQId + "') a";
                
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    this.dgvDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18]);
                }
            }

            if(txtTransType.Text != "AMOUNT")
                dgvDetails.Columns["Unit"].Visible = true;
            else if (txtTransType.Text == "AMOUNT")
                dgvDetails.Columns["Unit"].Visible = false;
            dgvDetails.Columns["RfqSeqNo"].Visible = false;
            dgvDetails.Columns["GroupId"].Visible = false;
            dgvDetails.Columns["SubGroup1Id"].Visible = false;
            dgvDetails.Columns["SubGroup2Id"].Visible = false;
            dgvDetails.Columns["ItemId"].Visible = false;
            dgvDetails.Columns["SeqNoGroup"].Visible = false;
            if(txtTransType.Text != "AMOUNT")
                dgvDetails.Columns["Qty"].DefaultCellStyle.Format = "N2";
            else if (txtTransType.Text == "AMOUNT")
                dgvDetails.Columns["Amount"].DefaultCellStyle.Format = "N2";
            dgvDetails.Columns["Price"].DefaultCellStyle.Format = "N2";
            if (Mode == "View")
                dgvDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }

        public void AddDataGridFromDetail2(List<string> VendId)
        {
            if (dgvVendor.RowCount - 1 <= 0)
            {
                dgvVendor.ColumnCount = 3;
                dgvVendor.Columns[0].Name = "No";
                dgvVendor.Columns[1].Name = "Vendor";
                dgvVendor.Columns[2].Name = "VendName";
                dgvVendor.Columns[0].Width = 40;
            }

            for (int i = 0; i < VendId.Count; i++)
            {
                Query = "Select VendID, VendName From [dbo].[VendTable] Where VendID = '" + VendId[i] + "' ";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        String VendId1 = Dr["VendID"] == null ? "" : Dr["VendID"].ToString();
                        String VendName = Dr["VendName"] == null ? "" : Dr["VendName"].ToString();

                        this.dgvVendor.Rows.Add((dgvVendor.RowCount + 1).ToString(), VendId1, VendName);
                    }
                }
                Conn.Close();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //if (Mode == "New" || Mode == "Generate")
            //{
            //    if (dgvDetails.RowCount > 0)
            //    {
            //        Index = dgvDetails.CurrentRow.Index;
            //        DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvDetails.Rows[Index].Cells["ItemDeskripsi"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
            //        if (dialogResult == DialogResult.Yes)
            //        {
            //            //Delete Gelombang Yang Gelomband dan Bracket nya sesuai dengan FullItemId.
            //            Conn = ConnectionString.GetConnection();
            //            Query = "Select [PurchReqId],[SeqNoDtl] From [dbo].[PurchRequisition_DtlDtl] where PurchReqId = '" + dgvDetails.CurrentRow.Cells[1].Value.ToString() + "' and SeqNoDtl = '" + dgvDetails.CurrentRow.Cells[2].Value.ToString() + "' ";

            //            Cmd = new SqlCommand(Query, Conn);
            //            Dr = Cmd.ExecuteReader();
            //            while (Dr.Read())
            //            {
            //                int tmp = dgvGelombang.RowCount;
            //                for (int i = 0; i < tmp; i++)
            //                {
            //                    if (dgvGelombang.Rows[i].Cells["PurchReqId"].Value.ToString() == Dr[0].ToString() && dgvGelombang.Rows[i].Cells["SeqNoDtl"].Value.ToString() == Dr[1].ToString())
            //                    {
            //                        dgvGelombang.Rows.RemoveAt(i);
            //                        tmp--;
            //                        i--;
            //                    }
            //                }

            //            }
            //            Conn.Close();
            //            dgvDetails.Rows.RemoveAt(Index);
            //        }

            //    }
            //    SortNoDataGrid();
            //}
            //else if (Mode == "Edit")
            //{
            //    if (dgvDetails.RowCount > 0)
            //    {
            //        Index = dgvDetails.CurrentRow.Index;
            //        DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvDetails.Rows[Index].Cells["ItemDeskripsi"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
            //        if (dialogResult == DialogResult.Yes)
            //        {
            //            //Delete Gelombang Yang Gelomband dan Bracket nya sesuai dengan FullItemId.
            //            Conn = ConnectionString.GetConnection();
            //            Query = "Select [RfqID],[SeqNoDtl] From [dbo].[RequestForQuotation_DtlDtl] where RfqID = '" + txtRFQID.Text + "' and SeqNoDtl = '" + dgvDetails.CurrentRow.Cells[2].Value.ToString() + "' ";

            //            Cmd = new SqlCommand(Query, Conn);
            //            Dr = Cmd.ExecuteReader();
            //            while (Dr.Read())
            //            {
            //                int tmp = dgvGelombang.RowCount;
            //                for (int i = 0; i < tmp; i++)
            //                {
            //                    if (txtRFQID.Text == Dr[0].ToString() && dgvGelombang.Rows[i].Cells["SeqNoDtl"].Value.ToString() == Dr[1].ToString())
            //                    {
            //                        dgvGelombang.Rows.RemoveAt(i);
            //                        tmp--;
            //                        i--;
            //                    }
            //                }

            //            }
            //            Conn.Close();
            //            dgvDetails.Rows.RemoveAt(Index);
            //        }

            //    }
            //    SortNoDataGrid();
            //}
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvDetails.RowCount; i++)
            {
                dgvDetails.Rows[i].Cells["No"].Value = i + 1;
            }

            //for (int i = 0; i < dgvGelombang.RowCount; i++)
            //{
            //    dgvGelombang.Rows[i].Cells["No"].Value = i + 1;
            //}
        }

        private void ModeNew()
        {
            dgvDetails.Enabled = true;
            btnNewV.Enabled = true;
            btnDeleteV.Enabled = true;

            label7.Visible = false;
            label5.Visible = false;
            txtVendorID.Visible = false;
            txtVendorName.Visible = false;
            rtxtNotes.Enabled = true;

            txtPurchReqID.Enabled = true;
            btnSearchPurchReqID.Enabled = true;
            btnSearchVendor.Visible = false;

            btnEdit.Visible = false;
            btnSave.Visible = true;

            dgvDetails.ReadOnly = false;
        }

        private void ModeEdit()
        {
            txtRFQID.Enabled = false;
            dtRFQDate.Enabled = false;
            txtVendorID.Enabled = true;
            txtVendorName.Enabled = true;
            txtPurchReqID.Enabled = true;
            rtxtNotes.Enabled = true;
            btnNewV.Enabled = true;
            btnDeleteV.Enabled = true;
            btnSearchPurchReqID.Enabled = true;


            btnSearchVendor.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnCancel.Visible = true;

            dgvDetails.ReadOnly = false;
            dgvDetails.Columns["No"].ReadOnly = true;
            dgvDetails.Columns["PurchReqID"].ReadOnly = true;
            dgvDetails.Columns["GroupId"].ReadOnly = true;
            dgvDetails.Columns["SubGroup1Id"].ReadOnly = true;
            dgvDetails.Columns["SubGroup2Id"].ReadOnly = true;
            dgvDetails.Columns["ItemId"].ReadOnly = true;
            dgvDetails.Columns["FullItemID"].ReadOnly = true;
            dgvDetails.Columns["DeliveryMethod"].ReadOnly = true;
            dgvDetails.Columns["ItemDeskripsi"].ReadOnly = true;
            dgvDetails.Columns["Unit"].ReadOnly = true;
            dgvDetails.Columns["Deskripsi"].ReadOnly = true;
            if(txtTransType.Text != "AMOUNT")
                dgvDetails.Columns["Qty"].ReadOnly = true;
            else if (txtTransType.Text == "AMOUNT")
                dgvDetails.Columns["Amount"].ReadOnly = true;
            dgvDetails.Columns["GelombangID"].ReadOnly = true;
            dgvDetails.Columns["BracketID"].ReadOnly = true;
            dgvDetails.Columns["Base"].ReadOnly = true;
            dgvDetails.Columns["Price"].ReadOnly = true;
            dgvDetails.Columns["Ratio"].ReadOnly = true;
        }

       

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select [PurchQuotID] From [dbo].[PurchQuotationH] where RfqId = '" + txtRFQID.Text + "'";
                SqlCommand cmd = new SqlCommand(Query, Conn);

                var TmpStr = cmd.ExecuteScalar();

                if (TmpStr == null)
                {
                    ModeEdit();
                }
                else 
                {
                    MessageBox.Show("RFQ sudah digunakan oleh Purchase Quotation = " +TmpStr.ToString()+ ".");
                }
                Conn.Close();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
            
        }

        private void ModeCancel()
        {
            txtRFQID.Enabled = false;
            dtRFQDate.Enabled = false;
            txtVendorID.Enabled = false;
            txtVendorName.Enabled = false;
            txtPurchReqID.Enabled = false;
            rtxtNotes.Enabled = false;
            btnSearchPurchReqID.Enabled = false;
            btnSearchVendor.Enabled = false;

            dgvDetails.ReadOnly = true;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeCancel();
            dgvDetails.Rows.Clear();
            GetDataHeader();
            RefreshGrid();
        }

        private void btnNewV_Click(object sender, EventArgs e)
        {
            Purchase.RFQ.AddVendor F = new Purchase.RFQ.AddVendor();
            F.ParentRefreshGrid(this);
            F.ShowDialog();
        }

        private void btnDeleteV_Click(object sender, EventArgs e)
        {
            if (dgvVendor.RowCount > 0)
            {
                Index = dgvVendor.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvVendor.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " VendId = " + dgvVendor.Rows[Index].Cells["Vendor"].Value.ToString() + Environment.NewLine + " VendName = " + dgvVendor.Rows[Index].Cells["VendName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvVendor.Rows.RemoveAt(Index);
                    for (int i = 0; i < dgvVendor.RowCount; i++)
                    {
                        dgvVendor.Rows[i].Cells["No"].Value = i + 1;
                    }
                }
            }
        }

        //HENDRY VALIDASI : 7 APRIL 2018
        private Boolean ValidGeneral()
        {
            Boolean vBol = true;

            if (dgvDetails.RowCount == 0)
            {
                MessageBox.Show("Isikan Detail terlebih dahulu..");
                vBol = false;
            }

            if (vBol == true)
            {
                if ((Mode == "New" || Mode == "Generate") && dgvVendor.RowCount == 0)
                {
                    MessageBox.Show("Pilih Vendor terlebih dahulu..");
                    vBol = false;
                }
            }

            if (vBol == true)
            {
                if (Mode == "Edit" && txtVendorID.Text.Trim()=="")
                {
                    MessageBox.Show("Pilih Vendor terlebih dahulu..");
                    vBol = false;
                }
            }
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //HENDRY VALIDASI            
            if (!ValidGeneral())
            {
                return;
            }

            List<String> RfqId = new List<string>();
            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;

            //HENDRY VALIDASI : 7 APRIL 2018
            //if ((Mode == "New" || Mode == "Generate") && dgvDetails.Rows.Count > 0 && dgvVendor.Rows.Count > 0)
            if ((Mode == "New" || Mode == "Generate"))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    for (int x = 0; x <= dgvVendor.Rows.Count - 1; x++)
                    {
                        String VendorId1 = dgvVendor.Rows[x].Cells["Vendor"].Value == null ? "" : dgvVendor.Rows[x].Cells["Vendor"].Value.ToString();
                        Query = "Select * From [dbo].[RequestForQuotationH] where VendID = '" + VendorId1 + "' and PurchReqId = '" + txtPurchReqID.Text + "'";
                        using (SqlCommand cmd = new SqlCommand(Query, Conn))
                        {
                            Dr = cmd.ExecuteReader();
                            if (Dr.Read())
                            {
                                MessageBox.Show("PRNumber = '" + txtPurchReqID.Text + "' dengan VendorID = '" + VendorId1 + "' sudah ada di database");
                                return;
                            }
                            Dr.Close();
                        }
                    }

                    try
                    {
                            if (dgvVendor.Rows.Count > 0)
                            {
                                for (int i = 0; i <= dgvVendor.Rows.Count - 1; i++)
                                {
                                    String VendorId = dgvVendor.Rows[i].Cells["Vendor"].Value == null ? "" : dgvVendor.Rows[i].Cells["Vendor"].Value.ToString();
                                    String VendorName = dgvVendor.Rows[i].Cells["VendName"].Value == null ? "" : dgvVendor.Rows[i].Cells["VendName"].Value.ToString();

                                    //Old Code=======================================================================================
                                    //Query = "Declare @tmp table (Id varchar (50)) ";
                                    //Query += "Insert into [dbo].[RequestForQuotationH] (RfqID,";
                                    //Query += "RfqDate,";
                                    //Query += "VendID,";
                                    //Query += "VendName,";
                                    //Query += "PurchReqId,";
                                    //Query += "TransType,";
                                    //Query += "Notes,";
                                    //Query += "CreatedDate,";
                                    //Query += "CreatedBy) Output (Inserted.RfqId) into @tmp values (";
                                    //Query += "(SELECT 'RFQ-'+FORMAT(GETDATE(), 'yyMM')+'-'+RIGHT('00000' + CONVERT(NVARCHAR,CASE WHEN Max(RfqId) is null THEN '1' ELSE RIGHT(Max(RfqId),5)+1 END),5) FROM [dbo].[RequestForQuotationH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6) and RfqId like ('RFQ-%'))" + ",'";
                                    //Query += dtRFQDate.Value.Date + "','";
                                    //Query += VendorId + "','";
                                    //Query += VendorName + "','";
                                    //Query += txtPurchReqID.Text + "','";
                                    //Query += txtTransType.Text + "','";
                                    //Query += rtxtNotes.Text + "',";
                                    //Query += "getdate(),'";
                                    //Query += ControlMgr.UserId + "') select * from @tmp;";

                                    //using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                                    //{
                                    //    RfqId.Add(Cmd.ExecuteScalar().ToString());
                                    //}
                                    //End Old Code=====================================================================================

                                    //begin============================================================================================
                                    //updated by : joshua
                                    //updated date : 14 Feb 2018
                                    //description : change generate sequence number, get from global function and update counter 
                                    string Jenis = "RFQ", Kode = "RFQ";
                                    RfqId.Add(ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd));

                                    Query = "Insert into [dbo].[RequestForQuotationH] (RfqID,";
                                    Query += "RfqDate,";
                                    Query += "VendID,";
                                    Query += "VendName,";
                                    Query += "PurchReqId,";
                                    Query += "TransType,";
                                    Query += "Notes,";
                                    Query += "CreatedDate,";
                                    Query += "CreatedBy) values (";
                                    Query += "'" + RfqId[i] + "','";
                                    Query += dtRFQDate.Value.Date + "','";
                                    Query += VendorId + "', @vendor,'";
                                    Query += txtPurchReqID.Text + "','";
                                    Query += txtTransType.Text + "', @notes, ";
                                    Query += "getdate(),'";
                                    Query += ControlMgr.UserId + "');";
                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.Parameters.AddWithValue("@notes", rtxtNotes.Text.ToString().Trim());
                                    Cmd.Parameters.AddWithValue("@vendor", VendorName);
                                    Cmd.ExecuteNonQuery();

                                    //update counter
                                    //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                                    //end update counter
                                    //end=============================================================================================


                                    if (dgvDetails.Rows.Count > 0)
                                    {
                                        for (int j = 0; j <= dgvDetails.Rows.Count - 1; j++)
                                        {
                                            String PurchReqId = dgvDetails.Rows[j].Cells["PurchReqId"].Value == null ? "" : dgvDetails.Rows[j].Cells["PurchReqId"].Value.ToString();
                                            decimal RfqSeqNo = dgvDetails.Rows[j].Cells["No"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["No"].Value.ToString());
                                            decimal PurchReqSeqNo = dgvDetails.Rows[j].Cells["RfqSeqNo"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["RfqSeqNo"].Value.ToString());

                                            String GroupId = dgvDetails.Rows[j].Cells["GroupId"].Value == null ? "" : dgvDetails.Rows[j].Cells["GroupId"].Value.ToString();
                                            String SubGroup1Id = dgvDetails.Rows[j].Cells["SubGroup1Id"].Value == null ? "" : dgvDetails.Rows[j].Cells["SubGroup1Id"].Value.ToString();
                                            String SubGroup2Id = dgvDetails.Rows[j].Cells["SubGroup1Id"].Value == null ? "" : dgvDetails.Rows[j].Cells["SubGroup1Id"].Value.ToString();
                                            String ItemId = dgvDetails.Rows[j].Cells["ItemId"].Value == null ? "" : dgvDetails.Rows[j].Cells["ItemId"].Value.ToString();
                                            String FullItemId = dgvDetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvDetails.Rows[j].Cells["FullItemId"].Value.ToString();
                                            String ItemDeskripsi = dgvDetails.Rows[j].Cells["ItemDeskripsi"].Value == null ? "" : dgvDetails.Rows[j].Cells["ItemDeskripsi"].Value.ToString();
                                            String DeliveryMethod = dgvDetails.Rows[j].Cells["DeliveryMethod"].Value == null ? "" : dgvDetails.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                                            decimal Qty=0;
                                            decimal Amount = 0;
                                            if (txtTransType.Text != "AMOUNT")
                                            {
                                                Qty = dgvDetails.Rows[j].Cells["Qty"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Qty"].Value.ToString());
                                            }
                                            else if (txtTransType.Text == "AMOUNT")
                                            {
                                                Amount = dgvDetails.Rows[j].Cells["Amount"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Amount"].Value.ToString());
                                                Qty = 1;//dgvDetails.Rows[j].Cells["Amount"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Amount"].Value.ToString());
                                            }
                                            String Unit = dgvDetails.Rows[j].Cells["Unit"].Value == null ? "" : dgvDetails.Rows[j].Cells["Unit"].Value.ToString();
                                            String Deskripsi = dgvDetails.Rows[j].Cells["Deskripsi"].Value == null ? "" : dgvDetails.Rows[j].Cells["Deskripsi"].Value.ToString();
                                            String GelombangId = dgvDetails.Rows[j].Cells["GelombangId"].Value == null ? "" : dgvDetails.Rows[j].Cells["GelombangId"].Value.ToString();
                                            String BracketId = dgvDetails.Rows[j].Cells["BracketId"].Value == null ? "" : dgvDetails.Rows[j].Cells["BracketId"].Value.ToString();
                                            String Base = dgvDetails.Rows[j].Cells["Base"].Value == null ? "" : dgvDetails.Rows[j].Cells["Base"].Value.ToString();
                                            decimal Price = dgvDetails.Rows[j].Cells["Price"].Value == "" ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Price"].Value.ToString());
                                            decimal SeqNoGroup = dgvDetails.Rows[j].Cells["SeqNoGroup"].Value == "" ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["SeqNoGroup"].Value.ToString());
                                            decimal Ratio = dgvDetails.Rows[j].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Ratio"].Value.ToString());

                                            if(txtTransType.Text != "AMOUNT")
                                            {
                                                Query = "Insert into [dbo].[RequestForQuotationD] (RfqId, PurchReqId, RfqSeqNo, PurchReqSeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemDeskripsi, DeliveryMethod, Qty, Unit, Deskripsi, GelombangId, BracketId, Base, Price, SeqNoGroup, Ratio, CreatedDate, CreatedBy) ";
                                                Query += "values ('" + RfqId[i] + "',  '" + PurchReqId + "', '" + RfqSeqNo + "', '" + PurchReqSeqNo + "','" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "', @itemdesc,'" + DeliveryMethod + "','" + Qty + "','" + Unit + "', @descr, '" + GelombangId + "', '" + BracketId + "', '" + Base + "', '" + Price + "', '" + SeqNoGroup + "', '" + Ratio + "',getdate(), '" + ControlMgr.UserId + "');";
                                            }
                                            else if (txtTransType.Text == "AMOUNT")
                                            {
                                                Query = "Insert into [dbo].[RequestForQuotationD] (RfqId, PurchReqId, RfqSeqNo, PurchReqSeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemDeskripsi, DeliveryMethod, Amount, Qty, Unit, Deskripsi, GelombangId, BracketId, Base, Price, SeqNoGroup, Ratio, CreatedDate, CreatedBy) ";
                                                Query += "values ('" + RfqId[i] + "',  '" + PurchReqId + "', '" + RfqSeqNo + "', '" + PurchReqSeqNo + "','" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "', @itemdesc,'" + DeliveryMethod + "','" + Amount + "','" + Qty + "','" + Unit + "', @descr, '" + GelombangId + "', '" + BracketId + "', '" + Base + "', '" + Price + "', '" + SeqNoGroup + "', '" + Ratio + "',getdate(), '" + ControlMgr.UserId + "');";
                                            }
                                            Query += "Update [dbo].[PurchRequisitionH] set TransStatus = '21' where PurchReqId = '" + PurchReqId + "' ";
                                            Cmd = new SqlCommand(Query, Conn);
                                            Cmd.Parameters.AddWithValue("@itemdesc", ItemDeskripsi);
                                            Cmd.Parameters.AddWithValue("@descr", Deskripsi);
                                            Cmd.ExecuteNonQuery();
                                            Query = "";
                                        }
                                    }

                                    //insert status

                                    Query = "Update [dbo].[RequestForQuotationH] set TransStatus = '01' where RfqID = '" + RfqId[i] + "' ";

                                    Cmd = new SqlCommand(Query, Conn);
                                    Cmd.ExecuteNonQuery();
                                }
                            }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }

                    string Tmp = "";

                    for (int i = 0; i < RfqId.Count; i++)
                    {
                        Tmp += RfqId[i].ToString() + "=" + dgvVendor.Rows[i].Cells["Vendor"].Value.ToString() + "\n";
                    }
                    Conn.Close();
                    scope.Complete();
                    MessageBox.Show("Data : \n" + Tmp + " \n Berhasil ditambahkan."); 
                }
               
                this.Close();
                if (Mode != "Generate")
                {
                    Parent.RefreshGrid();
                }
            }
            //else if (Mode == "Edit" && dgvDetails.Rows.Count > 0)
            else if (Mode == "Edit" )
            {                
                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        Conn = ConnectionString.GetConnection();

                        Query = "update [dbo].[RequestForQuotationH] set VendID ='" + txtVendorID.Text + "', VendName ='" + txtVendorName.Text + "', DeliveryMethod = '" + txtTransType.Text + "', Notes = @notes, UpdatedDate = getDate(), UpdatedBy = '" + ControlMgr.UserId + "' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy where RfqID ='" + txtRFQID.Text + "' ;";

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.AddWithValue("@notes", rtxtNotes.Text.ToString().Trim());
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                            CreatedBy = Dr["CreatedBy"].ToString();
                        }

                        Dr.Close();

                        if (dgvDetails.Rows.Count > 0)
                        {
                            Query1 = "Delete from [dbo].[RequestForQuotationD] where RfqID ='" + txtRFQID.Text.Trim() + "';";
                            Cmd = new SqlCommand(Query1, Conn);
                            Cmd.ExecuteNonQuery();
                            for (int j = 0; j <= dgvDetails.RowCount - 1; j++)
                            {
                                String PurchReqId = dgvDetails.Rows[j].Cells["PurchReqId"].Value == null ? "" : dgvDetails.Rows[j].Cells["PurchReqId"].Value.ToString();
                                decimal RfqSeqNo = dgvDetails.Rows[j].Cells["No"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["No"].Value.ToString());
                                decimal PurchReqSeqNo = dgvDetails.Rows[j].Cells["No"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["No"].Value.ToString());
                                decimal GroupId = dgvDetails.Rows[j].Cells["GroupId"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["GroupId"].Value.ToString());
                                decimal SubGroup1Id = dgvDetails.Rows[j].Cells["SubGroup1Id"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["SubGroup1Id"].Value.ToString());
                                decimal SubGroup2Id = dgvDetails.Rows[j].Cells["SubGroup1Id"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["SubGroup1Id"].Value.ToString());
                                decimal ItemId = dgvDetails.Rows[j].Cells["ItemId"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["ItemId"].Value.ToString());
                                String FullItemId = dgvDetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvDetails.Rows[j].Cells["FullItemId"].Value.ToString();
                                String ItemDeskripsi = dgvDetails.Rows[j].Cells["ItemDeskripsi"].Value == null ? "" : dgvDetails.Rows[j].Cells["ItemDeskripsi"].Value.ToString();
                                String DeliveryMethod = dgvDetails.Rows[j].Cells["DeliveryMethod"].Value == null ? "" : dgvDetails.Rows[j].Cells["DeliveryMethod"].Value.ToString();
                                decimal Qty = 0;
                                if(txtTransType.Text!="AMOUNT")
                                    Qty = dgvDetails.Rows[j].Cells["Qty"].Value == null || dgvDetails.Rows[j].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Qty"].Value.ToString());
                                else if (txtTransType.Text == "AMOUNT")
                                    Qty = dgvDetails.Rows[j].Cells["Amount"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Amount"].Value.ToString());
                                String Unit = dgvDetails.Rows[j].Cells["Unit"].Value == null ? "" : dgvDetails.Rows[j].Cells["Unit"].Value.ToString();
                                String Deskripsi = dgvDetails.Rows[j].Cells["Deskripsi"].Value == null ? "" : dgvDetails.Rows[j].Cells["Deskripsi"].Value.ToString();
                                String GelombangId = dgvDetails.Rows[j].Cells["GelombangId"].Value == null ? "" : dgvDetails.Rows[j].Cells["GelombangId"].Value.ToString();
                                String BracketId = dgvDetails.Rows[j].Cells["BracketId"].Value == null ? "" : dgvDetails.Rows[j].Cells["GelombangId"].Value.ToString();
                                String Base = dgvDetails.Rows[j].Cells["Base"].Value == null ? "" : dgvDetails.Rows[j].Cells["Base"].Value.ToString();
                                decimal Price = dgvDetails.Rows[j].Cells["Price"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Price"].Value.ToString());
                                decimal SeqNoGroup = dgvDetails.Rows[j].Cells["SeqNoGroup"].Value == null ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["SeqNoGroup"].Value.ToString());
                                decimal Ratio = dgvDetails.Rows[j].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvDetails.Rows[j].Cells["Ratio"].Value.ToString());

                                if (txtTransType.Text != "AMOUNT")
                                    Query = "Insert into [dbo].[RequestForQuotationD] (RfqId, PurchReqId, RfqSeqNo, PurchReqSeqNo, GroupId, SubGroup1ID, SUbGroup2ID, ItemId, FullItemId, ItemDeskripsi, DeliveryMethod, Qty, Unit, Deskripsi, GelombangId, BracketId, Base, Price, SeqNoGroup, Ratio, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) ";
                                else if (txtTransType.Text == "AMOUNT")
                                    Query = "Insert into [dbo].[RequestForQuotationD] (RfqId, PurchReqId, RfqSeqNo, PurchReqSeqNo, GroupId, SubGroup1ID, SUbGroup2ID, ItemId, FullItemId, ItemDeskripsi, DeliveryMethod, Amount, Unit, Deskripsi, GelombangId, BracketId, Base, Price, SeqNoGroup, Ratio, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) ";

                                Query += "values ('" + txtRFQID.Text + "',  '" + PurchReqId + "', '" + RfqSeqNo + "', '" + PurchReqSeqNo + "', '" + GroupId + "', '" + SubGroup1Id + "', '" + SubGroup2Id + "', '" + ItemId + "', '" + FullItemId + "', @itemdesc, '" + DeliveryMethod + "','" + Qty + "','" + Unit + "', @descr, '" + GelombangId + "', '" + BracketId + "', '" + Base + "', '" + Price + "', '" + SeqNoGroup + "', '" + Ratio + "','" + CreatedDate + "', '" + CreatedBy + "',getdate(), '" + ControlMgr.UserId + "');";

                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.Parameters.AddWithValue("@itemdesc", ItemDeskripsi);
                                Cmd.Parameters.AddWithValue("@descr", Deskripsi);
                                Cmd.ExecuteNonQuery();
                            }
                        }
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                Conn.Close();
                MessageBox.Show("Data RFQID = " + txtRFQID.Text + " , berhasil di Update."); 
                ModeCancel();
                Parent.RefreshGrid();
            }
            //HENDRY VALIDASI
            //else
            //{
            //    MessageBox.Show("Input Details First");
            //}
            RefreshGrid();
        }

        private void btnSearchVendor_Click(object sender, EventArgs e)
        {
            //Hendry ubah form search : 7 April 2018
            /*
            string SchemaName = "dbo";
            string TableName = "VendTable";
            string Where = " And BlackList = '0' ";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            tmpSearch.ShowDialog();
            txtVendorID.Text = ConnectionString.Kode;
            txtVendorName.Text = ConnectionString.Kode2;
             */
            ControlMgr.TblName = "VendTable";
            ControlMgr.tmpSort = "ORDER BY VendId";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search Vendor File";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtVendorID.Text = ControlMgr.Kode;
                txtVendorName.Text = ControlMgr.Kode2;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            ControlMgr.tmpWhere = "";
            ControlMgr.Kode = "";
            ControlMgr.Kode2 = "";
        }

        private void txtVendorID_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (txtVendorID.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[VendTable] Where VendId = '" + txtVendorID.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Vendor ID tidak ada di database.");
                        btnSearchVendor_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void txtVendorName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (txtVendorName.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[VendTable] Where VendName = '" + txtVendorName.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Vendor Name tidak ada di database.");
                        btnSearchVendor_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void cmbTransType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckPrType();
            //tmpTransType = cmbTransType.Text;
            CheckCollapse();
            //dgvGelombang.Rows.Clear();
            dgvDetails.Rows.Clear();
            dgvVendor.Rows.Clear();
        }

        private void CheckPrType()
        {
            //if (cmbTransType.Text == "FIX")
            //{
            //    grpGelombang.Visible = false;
            //}
            //else
            //{
            //    grpGelombang.Visible = true;
            //    //GetGelombang();
            //}
        }

        private void CheckCollapse()
        {
            //if (btnSave.Top == 511 && Mode == "Edit")
            //{
            //    btnSave.Top -= 176;
            //    btnEdit.Top -= 176;
            //    btnCancel.Top -= 176;
            //    btnExit.Top -= 176;

            //    grpVN.Visible = false;
            //    groupRFQ.Height -= 176;

            //    this.Height -= 176;   
            //}
        }

        private void dgvDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvDetails.Columns[dgvDetails.CurrentCell.ColumnIndex].Name == "Qty")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
        }

        private void dgvDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvDetails_KeyPress);
            }
        }

        private void dgvDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
            {
                DeliveryMethod.Location = dgvDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                DeliveryMethod.Visible = true;
                string tmpFullItemId = dgvDetails.Rows[dgvDetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                string tmpDeliveryMethod = "";
                Conn = ConnectionString.GetConnection();
                for (int i = 0; i < dgvDetails.RowCount; i++)
                {
                    if (dgvDetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                    {
                        if (dgvDetails.Rows[i].Cells["DeliveryMethod"].Value != null)
                        {
                            if (tmpDeliveryMethod == "")
                            {
                                tmpDeliveryMethod = "'" + dgvDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                            }
                            else
                            {
                                tmpDeliveryMethod += ",'" + dgvDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                            }
                        }
                    }
                }

                Query = "SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod] ";

                if (tmpDeliveryMethod != "")
                {
                    Query += "Where DeliveryMethod not in (" + tmpDeliveryMethod + ");";
                }

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
        }

        private void dgvDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
            {
                DeliveryMethod.Visible = false;
            }
        }

        private void SearchPR()
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "PurchReqId";
            tmpSearch.Order = "CreatedDate Desc";
            tmpSearch.Table = "[dbo].[PurchRequisitionH]";
            //update by hasim karena Selama PR belum dibuat canvass sheet, tetap boleh dibuat rfq
            tmpSearch.QuerySearch = "SELECT [PurchReqId], [OrderDate] AS 'PRDate', [TransType], [CreatedDate],[CreatedBy] FROM [dbo].[PurchRequisitionH] WHERE PurchReqId NOT IN(SELECT PurchReqId FROM CanvasSheetH) and (TransStatus IN ('13','14','21','22','33'))";
            //tmpSearch.QuerySearch = "SELECT [PurchReqId], [OrderDate] AS 'PRDate', [TransType], [CreatedDate],[CreatedBy] FROM [dbo].[PurchRequisitionH] WHERE PurchReqId NOT IN(SELECT PurchReqId FROM CanvasSheetH) and (TransStatus='13' OR TransStatus='14')";

            tmpSearch.FilterText = new string[] { "PurchReqId", "PRDate", "TransType", "CreatedDate", "CreatedBy" };
            tmpSearch.Mask = new string[] { "PR No", "PR Date", "PR Type", "Created Date", "Created By" };
            tmpSearch.Select = new string[] { "PurchReqId", "TransType" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtPurchReqID.Text = ConnectionString.Kodes[0];
                txtTransType.Text = ConnectionString.Kodes[1];
            }
        }

        private void btnSearchPurchReqID_Click(object sender, EventArgs e)
        {
            /*Hendry Ganti Search PR : 2 April 2018
            string SchemaName = "dbo";
            string TableName = "PurchRequisitionH";
            string Where = "And (TransStatus = '21' or TransStatus = '13' or TransStatus = '14')";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);//, Where);
            tmpSearch.ShowDialog();
            txtPurchReqID.Text = ConnectionString.Kode;
            txtTransType.Text = ConnectionString.Kode2;
            */

             /* before 31/08/2018
            ControlMgr.TblName = "PurchRequisitionH";
            Methods.ControlMgr.tmpWhere = "WHERE PurchReqId NOT IN(SELECT PurchReqId FROM CanvasSheetH) and (TransStatus='13' OR TransStatus='14')";
            ControlMgr.tmpSort = "ORDER BY CreatedDate Desc";

            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            FrmSearch.Text = "Search PR";
            FrmSearch.ShowDialog();

            if (ControlMgr.Kode != "")
            {
                txtPurchReqID.Text = ControlMgr.Kode;
                txtTransType.Text = ControlMgr.Kode2;
            }

            ControlMgr.TblName = "";
            ControlMgr.tmpSort = "";
            Methods.ControlMgr.tmpWhere = "";
            ControlMgr.Kode = "";
            ControlMgr.Kode2 = "";
            */

            SearchPR(); //STV ganti search 31/08/2018
            dgvDetails.Rows.Clear();
            dgvVendor.Rows.Clear();

            dgvDetails.Columns.Clear();
            if (dgvDetails.RowCount - 1 <= 0)
            {
                dgvDetails.ColumnCount = 19;
                dgvDetails.Columns[0].Name = "No";
                dgvDetails.Columns[1].Name = "RfqSeqNo"; dgvDetails.Columns[1].HeaderText = "RFQ No";
                dgvDetails.Columns[2].Name = "PurchReqID"; dgvDetails.Columns[2].HeaderText = "PR ID";
                dgvDetails.Columns[3].Name = "GroupId"; dgvDetails.Columns[3].HeaderText = "Group ID";
                dgvDetails.Columns[4].Name = "SubGroup1Id"; dgvDetails.Columns[4].HeaderText = "Sub Group 1 ID";
                dgvDetails.Columns[5].Name = "SubGroup2ID"; dgvDetails.Columns[5].HeaderText = "Sub Group 2 ID";
                dgvDetails.Columns[6].Name = "ItemID"; dgvDetails.Columns[6].HeaderText = "Item ID";
                dgvDetails.Columns[7].Name = "FullItemID"; dgvDetails.Columns[7].HeaderText= "Full Item ID";
                dgvDetails.Columns[8].Name = "ItemDeskripsi"; dgvDetails.Columns[8].HeaderText = "Item Description";
                dgvDetails.Columns[9].Name = "DeliveryMethod"; dgvDetails.Columns[9].HeaderText= "Delivery Method";
                if (txtTransType.Text != "AMOUNT")
                {
                    dgvDetails.Columns[10].Name = "Qty"; dgvDetails.Columns[10].HeaderText = "Quantity";
                }
                else if (txtTransType.Text == "AMOUNT")
                {
                    dgvDetails.Columns[10].Name = "Amount"; //5dgvDetails.Columns[10].HeaderText = "Quantity";
                }
                dgvDetails.Columns[11].Name = "Unit";
                dgvDetails.Columns[12].Name = "Deskripsi";
                dgvDetails.Columns[13].Name = "GelombangId"; dgvDetails.Columns[13].HeaderText = "Wave ID";
                dgvDetails.Columns[14].Name = "BracketId"; dgvDetails.Columns[14].HeaderText = "Bracket ID";
                dgvDetails.Columns[15].Name = "Base";
                dgvDetails.Columns[16].Name = "Price";
                dgvDetails.Columns[17].Name = "SeqNoGroup"; dgvDetails.Columns[17].HeaderText = "Seq No Group";
                dgvDetails.Columns[18].Name = "Ratio";

                dgvDetails.Columns[0].Width = 40;
                dgvDetails.Columns[7].Width = 110;
                dgvDetails.Columns[8].Width = 200;
                dgvDetails.Columns[10].Width = 60;
                dgvDetails.Columns[11].Width = 50;
                dgvDetails.Columns[13].Width = 90;
                dgvDetails.Columns[14].Width = 60;
                dgvDetails.Columns[15].Width = 40;
            }

            Query = "Select SeqNoGroup from PurchRequisition_Dtl where PurchReqId = '" + txtPurchReqID.Text + "' and TransStatusPurch = 'Yes'";
            Conn = ConnectionString.GetConnection();
            string TmpSeqNoGroup = "0";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (TmpSeqNoGroup == "")
                        TmpSeqNoGroup = "'" + Dr[0].ToString() + "'";
                    else
                        TmpSeqNoGroup += ",'" + Dr[0].ToString() + "'";
                }
            }
            Dr.Close();

            if(txtTransType.Text != "AMOUNT")
                Query = "Select SeqNo, PurchReqId, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi, GelombangID, BracketID, Base, Price, SeqNoGroup, Ratio From [dbo].[PurchRequisition_Dtl] Where PurchReqId = '" + txtPurchReqID.Text + "' and SeqNoGroup in (" + TmpSeqNoGroup + ") ;";
            else if (txtTransType.Text == "AMOUNT")
                Query = "Select SeqNo, PurchReqId, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemID, ItemName, DeliveryMethod, Amount, Unit, Deskripsi, GelombangID, BracketID, Base, Price, SeqNoGroup, Ratio From [dbo].[PurchRequisition_Dtl] Where PurchReqId = '" + txtPurchReqID.Text + "' and SeqNoGroup in (" + TmpSeqNoGroup + ") ;";
            
            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    String PurchReqId1 = Dr["PurchReqId"] == null ? "" : Dr["PurchReqId"].ToString();
                    String RfqSeqNo1 = Dr["SeqNo"] == null ? "" : Dr["SeqNo"].ToString();
                    String GroupId = Dr["GroupId"] == null ? "" : Dr["GroupId"].ToString();
                    String SubGroup1Id = Dr["SubGroup1Id"] == null ? "" : Dr["SubGroup1Id"].ToString();
                    String SubGroup2Id = Dr["SubGroup2Id"] == null ? "" : Dr["SubGroup2Id"].ToString();
                    String ItemId = Dr["ItemId"] == null ? "" : Dr["ItemId"].ToString();
                    String FullItemId = Dr["FullItemId"] == null ? "" : Dr["FullItemId"].ToString();
                    String ItemDeskripsi = Dr["ItemName"] == null ? "" : Dr["ItemName"].ToString();
                    String DeliveryMethod = Dr["DeliveryMethod"] == null ? "" : Dr["DeliveryMethod"].ToString();
                    String Qty = "0";
                    if(txtTransType.Text != "AMOUNT")
                    {
                        Qty = Dr["Qty"] == null ? "" : Dr["Qty"].ToString();
                    }
                    else if(txtTransType.Text == "AMOUNT")
                    {
                        Qty = Dr["Amount"] == null ? "" : Dr["Amount"].ToString();
                    }
                    String Unit = Dr["Unit"] == null ? "" : Dr["Unit"].ToString();
                    String Deskripsi = Dr["Deskripsi"] == null ? "" : Dr["Deskripsi"].ToString();
                    String GelombangId = Dr["GelombangID"] == null ? "" : Dr["GelombangID"].ToString();
                    String BracketId = Dr["BracketID"] == null ? "" : Dr["BracketID"].ToString();
                    String Base = Dr["Base"] == null ? "" : Dr["Base"].ToString();
                    String Price = Dr["Price"] == null ? "" : Dr["Price"].ToString();
                    String SeqNoGroup = Dr["SeqNoGroup"] == null ? "" : Dr["SeqNoGroup"].ToString();
                    String Ratio = Dr["Ratio"] == null ? "" : Dr["Ratio"].ToString();

                    this.dgvDetails.Rows.Add((dgvDetails.RowCount + 1).ToString(), RfqSeqNo1, PurchReqId1, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemDeskripsi, DeliveryMethod, Qty, Unit, Deskripsi, GelombangId, BracketId, Base, Price, SeqNoGroup, Ratio);
                    if (dgvDetails.Rows[(dgvDetails.Rows.Count - 1)].Cells["Base"].Value.ToString() == "N")
                    {
                        dgvDetails.Rows[(dgvDetails.Rows.Count - 1)].DefaultCellStyle.BackColor = Color.LightGray;
                        dgvDetails.Rows[(dgvDetails.Rows.Count - 1)].ReadOnly = true;
                    }
                }
            }
            Conn.Close();

            dgvDetails.Columns["RfqSeqNo"].Visible = false;
            dgvDetails.Columns["GroupId"].Visible = false;
            dgvDetails.Columns["SubGroup1Id"].Visible = false;
            dgvDetails.Columns["SubGroup2Id"].Visible = false;
            dgvDetails.Columns["ItemId"].Visible = false;
            dgvDetails.Columns["SeqNoGroup"].Visible = false;
            
            if (dgvVendor.RowCount - 1 <= 0)
            {
                dgvVendor.ColumnCount = 3;
                dgvVendor.Columns[0].Name = "No";
                dgvVendor.Columns[1].Name = "Vendor";
                dgvVendor.Columns[2].Name = "VendName"; dgvVendor.Columns[2].HeaderText = "Vendor Name";
                dgvVendor.Columns[0].Width = 40;
            }

            Query = "Select [VendId] From [dbo].[PurchRequisition_Dtl] where [PurchReqId] = '" + txtPurchReqID.Text + "' AND VendID NOT IN (SELECT VendID FROM [dbo].[RequestForQuotationH] WHERE PurchReqId = '" + txtPurchReqID.Text + "')";
            
            string TmpVendor = "";
            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    if (TmpVendor == "")
                        TmpVendor = Dr[0].ToString();
                    else
                        TmpVendor += ";" + Dr[0].ToString();
                    
                }
            }
            Dr.Close();

            string[] TmpVendId = TmpVendor.Split(';');
            for (int i = 0; i < TmpVendId.Count(); i++)
            {
                if (i == 0)
                    TmpVendor = "'" + TmpVendId[i].ToString() + "'";
                else
                    TmpVendor += ",'" + TmpVendId[i].ToString() + "'";
            }

            Query = "Select [VendId],VendName From [dbo].[VendTable] where [VendId] in (" + TmpVendor + ") ";

            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    this.dgvVendor.Rows.Add((dgvVendor.RowCount + 1).ToString(), Dr[0].ToString(), Dr[1].ToString());
                }
            }
            Conn.Close();

            //Query = "Select a.[VendId], b.[VendName] From [dbo].[PurchRequisition_Dtl] a JOIN [dbo].[VendTable] b ON a.[VendID] = b.[VendID]  where a.[PurchReqId] = '" + txtPurchReqID.Text + "' ";

            //Conn = ConnectionString.GetConnection();
            //using (SqlCommand cmd = new SqlCommand(Query, Conn))
            //{
            //    Dr = cmd.ExecuteReader();

            //    while (Dr.Read())
            //    {
            //        String VendId1 = Dr["VendID"] == null ? "" : Dr["VendID"].ToString();
            //        String VendName = Dr["VendName"] == null ? "" : Dr["VendName"].ToString();

            //        this.dgvVendor.Rows.Add((dgvVendor.RowCount + 1).ToString(), VendId1, VendName);
            //    }
            //}
            //Conn.Close();

            dgvDetails.ReadOnly = false;
            dgvDetails.Columns["No"].ReadOnly = true;
            dgvDetails.Columns["PurchReqID"].ReadOnly = true;
            dgvDetails.Columns["FullItemID"].ReadOnly = true;
            dgvDetails.Columns["DeliveryMethod"].ReadOnly = true;
            dgvDetails.Columns["ItemDeskripsi"].ReadOnly = true;
            dgvDetails.Columns["Unit"].ReadOnly = true;
            dgvDetails.Columns["Deskripsi"].ReadOnly = true;
            if(txtTransType.Text!="AMOUNT")
                dgvDetails.Columns["Qty"].ReadOnly = true;
            else if (txtTransType.Text == "AMOUNT")
                dgvDetails.Columns["Amount"].ReadOnly = true;
            dgvDetails.Columns["GelombangID"].ReadOnly = true;
            dgvDetails.Columns["BracketID"].ReadOnly = true;
            dgvDetails.Columns["Base"].ReadOnly = true;
            dgvDetails.Columns["Price"].ReadOnly = true;
            dgvDetails.Columns["Ratio"].ReadOnly = true;

            if(txtTransType.Text != "AMOUNT")
                dgvDetails.Columns["Unit"].Visible = true;
            else if(txtTransType.Text == "AMOUNT")
                dgvDetails.Columns["Unit"].Visible = false;

            if (dgvDetails.Columns.Contains("Qty"))
                dgvDetails.Columns["Qty"].DefaultCellStyle.Format = "N2";
            else if (dgvDetails.Columns.Contains("Amount"))
                dgvDetails.Columns["Amount"].DefaultCellStyle.Format = "N2";
            dgvDetails.Columns["Price"].DefaultCellStyle.Format = "N2";
            

        }

        private void txtPurchReqID_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (txtPurchReqID.Text == "")
            {

            }
            else
            {
                Query = "Select * From [dbo].[PurchRequisitionH] Where PurchReqID = '" + txtPurchReqID.Text + "'";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    if (Dr.Read())//sama dengan di database
                    {
                        Conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Purchase Requisition ID tidak ada di database.");
                        btnSearchPurchReqID_Click(sender, e);
                        Conn.Close();
                    }
                }
            }
        }

        private void dgvDetails_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //ContextMenu m = new ContextMenu();
                //m.MenuItems.Add(new MenuItem("Cut"));
                //m.MenuItems.Add(new MenuItem("Copy"));
                //m.MenuItems.Add(new MenuItem("Paste"));

                //int currentMouseOverRow = dgvDetails.HitTest(e.X, e.Y).RowIndex;

                //if (currentMouseOverRow >= 0)
                //{
                //    m.MenuItems.Add(new MenuItem(string.Format("Do something to row {0}", currentMouseOverRow.ToString())));
                //}

                //m.Show(dgvDetails, new Point(e.X, e.Y));

                //InfoRFQ tmpInfo = new InfoRFQ();

                //ListInfo.Add(tmpInfo);
                ////tmpInfo.SetParent(this);
                //tmpInfo.Show();
            }
        }

        private void dgvDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (Mode == "New" || Mode == "Generate")
            //{
            //    for (int i = 0; i < dgvGelombang.RowCount; i++)
            //    {
            //        dgvGelombang.Rows[i].Visible = false;
            //    }

            //    Conn = ConnectionString.GetConnection();
            //    Query = "Select [PurchReqId],[SeqNoDtl] From [dbo].[PurchRequisition_DtlDtl] where PurchReqId = '" + dgvDetails.CurrentRow.Cells[1].Value.ToString() + "' and SeqNoDtl = '" + dgvDetails.CurrentRow.Cells[2].Value.ToString() + "' ";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    while (Dr.Read())
            //    {
            //        for (int i = 0; i < dgvGelombang.RowCount; i++)
            //        {
            //            if (dgvGelombang.Rows[i].Cells["PurchReqId"].Value.ToString() == Dr[0].ToString() && dgvGelombang.Rows[i].Cells["SeqNoDtl"].Value.ToString() == Dr[1].ToString())
            //            {
            //                dgvGelombang.Rows[i].Visible = true;
            //            }
            //        }
            //    }
            //    Conn.Close();
            //}
            //else if (Mode == "Edit")
            //{
            //    for (int i = 0; i < dgvGelombang.RowCount; i++)
            //    {
            //        dgvGelombang.Rows[i].Visible = false;
            //    }

            //    Conn = ConnectionString.GetConnection();
            //    Query = "Select [RfqID],[SeqNoDtl] From [dbo].[RequestForQuotation_DtlDtl] where RfqID = '" + txtRFQID.Text + "' and SeqNoDtl = '" + dgvDetails.CurrentRow.Cells[2].Value.ToString() + "' ";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    while (Dr.Read())
            //    {
            //        for (int i = 0; i < dgvGelombang.RowCount; i++)
            //        {
            //            if (txtRFQID.Text == Dr[0].ToString() && dgvGelombang.Rows[i].Cells["SeqNoDtl"].Value.ToString() == Dr[1].ToString())
            //            {
            //                dgvGelombang.Rows[i].Visible = true;
            //            }
            //        }
            //    }
            //    Conn.Close();


            //}

            //if (dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
            //{
            //    InfoRFQ tmpInfo = new InfoRFQ();

            //    ListInfo.Add(tmpInfo);
            //    //tmpInfo.SetParent(this);
            //    tmpInfo.Show();
            //}
        }
        //tia edit
        //klik kanan
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.Vendor.Vendor Vendor = null;
        Purchase.PurchaseRequisition.HeaderPR PRid = null;

        private void dgvDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemDeskripsi" || dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                {
                    if (FID == null || FID.Text == "")
                    {
                        if (dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemDeskripsi" || dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                        {

                            FID = new PopUp.FullItemId.FullItemId();
                            FID.GetData(dgvDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                            itemID = dgvDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                            FID.Show();
                        }
                    }
                    else if (CheckOpened(FID.Name))
                    {
                        //update
                        FID.WindowState = FormWindowState.Normal;
                        FID.GetData(dgvDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        FID.Show();
                        FID.Focus();
                    }
                }
                //PRID
                if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    if (PRid == null || PRid.Text == "")
                    {
                        if (dgvDetails.Columns[e.ColumnIndex].Name.ToString() == "PurchReqID")
                        {
                            PRid = new Purchase.PurchaseRequisition.HeaderPR();
                            PRid.SetMode("PopUp", dgvDetails.Rows[e.RowIndex].Cells["PurchReqID"].Value.ToString());
                            PRid.ParentRefreshGrid3(this);
                            PRid.Show();
                        }
                    }
                    else if (CheckOpened(PRid.Name))
                    {
                        PRid.WindowState = FormWindowState.Normal;
                         PRid.SetMode("PopUp",dgvDetails.Rows[e.RowIndex].Cells["PurchReqID"].Value.ToString());
                         PRid.ParentRefreshGrid3(this);
                        PRid.Show();
                        PRid.Focus();
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
       
        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvVendor_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dgvVendor.Columns[e.ColumnIndex].Name.ToString() == "Vendor" || dgvVendor.Columns[e.ColumnIndex].Name.ToString() == "VendorName")
                {

                    PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();
                    PopUpVendor.GetData(dgvVendor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    PopUpVendor.ShowDialog();

                }
            }
        }
      
        private void txtVendorID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    txtVendorID.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendorID.Text);

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
                    Vendor.GetData(txtVendorID.Text);

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

        private void txtPurchReqID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (PRid == null || PRid.Text == "")
                {
                    txtPurchReqID.Enabled = true;
                    PRid = new Purchase.PurchaseRequisition.HeaderPR();
                    PRid.SetMode("PopUp", txtPurchReqID.Text);
                    PRid.ParentRefreshGrid3(this);
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

        private void dgvDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvDetails.Columns.Contains("Qty"))
            {
                if (e.ColumnIndex == dgvDetails.Columns["Qty"].Index && e.Value != null)
                {
                    if (e.Value.ToString() != "")
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                    else
                    {
                        e.Value = 0;
                    }
                }
            }
            if (dgvDetails.Columns.Contains("Amount"))
            {
                if (e.ColumnIndex == dgvDetails.Columns["Amount"].Index && e.Value != null)
                {
                    if (e.Value.ToString() != "")
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                    else
                    {
                        e.Value = 0;
                    }
                }
            }
            if (dgvDetails.Columns.Contains("Price"))
            {
                if (e.ColumnIndex == dgvDetails.Columns["Price"].Index && e.Value != null)
                {
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N4");
                }
            }
        }
        //end
    }
}
