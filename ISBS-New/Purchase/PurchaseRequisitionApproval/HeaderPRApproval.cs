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

namespace ISBS_New.Purchase.PurchaseRequisitionApproval
{
    public partial class HeaderPRApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlConnection Conn2;
        private SqlCommand Cmd;
        private SqlDataReader Dr, Dr2;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String PurchReqID = null;
        String complete = "yes";

        Purchase.PurchaseRequisitionApproval.InquiryPRApproval Parent;
        //tia edit
        TaskList.Purchase.PurchaseRequisition.TaskListPR Parent2;
        List<PopUp.Vendor.Vendor> ListVendor = new List<PopUp.Vendor.Vendor>();
        List<PopUp.SalesOrder.SalesOrder> ListSO = new List<PopUp.SalesOrder.SalesOrder>();
        //tia edit end
        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public HeaderPRApproval()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.PurchaseRequisitionApproval.InquiryPRApproval F)
        {
            Parent = F;
        }

        public void setParent2(TaskList.Purchase.PurchaseRequisition.TaskListPR F2)
        {
            Parent2 = F2;
        }

        public void flag(String purchreqid)
        {
            PurchReqID = purchreqid;
        }

        private void CreateTable()
        {
            if (dgvPrDetails.RowCount - 1 <= 0)
            {
                dgvPrDetails.ColumnCount = 19;
                dgvPrDetails.Columns[0].Name = "No";
                dgvPrDetails.Columns[1].Name = "FullItemID";
                dgvPrDetails.Columns[2].Name = "ItemName";
                dgvPrDetails.Columns[3].Name = "Sales SO";
                dgvPrDetails.Columns[4].Name = "ExpectedDateFrom";
                dgvPrDetails.Columns[5].Name = "ExpectedDateTo";
                dgvPrDetails.Columns[6].Name = "Delivery Method";
                if(cmbPrType.Text != "AMOUNT")
                    dgvPrDetails.Columns[7].Name = "Qty";
                else if (cmbPrType.Text == "AMOUNT")
                    dgvPrDetails.Columns[7].Name = "Amount";
                dgvPrDetails.Columns[8].Name = "Unit";
                dgvPrDetails.Columns[9].Name = "VendId";
                dgvPrDetails.Columns[10].Name = "Base";
                dgvPrDetails.Columns[11].Name = "Deskripsi";
                dgvPrDetails.Columns[12].Name = "Sales Manager Status";
                dgvPrDetails.Columns[13].Name = "Sales Last Approval ID";
                dgvPrDetails.Columns[14].Name = "Sales Approval Description";
                dgvPrDetails.Columns[15].Name = "Purchase Manager Status";
                dgvPrDetails.Columns[16].Name = "Purchase Last Approval ID";
                dgvPrDetails.Columns[17].Name = "Purchase Approval Description";
                dgvPrDetails.Columns[18].Name = "SeqNo";

                dgvPrDetails.ReadOnly = false;
                dgvPrDetails.Columns["No"].ReadOnly = true;
                dgvPrDetails.Columns["FullItemID"].ReadOnly = true;
                dgvPrDetails.Columns["ItemName"].ReadOnly = true;
                dgvPrDetails.Columns["Sales SO"].ReadOnly = true;
                dgvPrDetails.Columns["Delivery Method"].ReadOnly = true;
                dgvPrDetails.Columns["ExpectedDateFrom"].ReadOnly = true;
                dgvPrDetails.Columns["ExpectedDateTo"].ReadOnly = true;
                if (cmbPrType.Text != "AMOUNT")
                {
                    dgvPrDetails.Columns["Qty"].ReadOnly = true;
                }
                else if (cmbPrType.Text == "AMOUNT")
                {
                    dgvPrDetails.Columns["Amount"].ReadOnly = true;
                }
                dgvPrDetails.Columns["Unit"].ReadOnly = true;
                dgvPrDetails.Columns["VendId"].ReadOnly = true;
                dgvPrDetails.Columns["Base"].ReadOnly = true;
                dgvPrDetails.Columns["Sales Last Approval ID"].ReadOnly = true;
                dgvPrDetails.Columns["Purchase Last Approval ID"].ReadOnly = true;
                dgvPrDetails.Columns["SeqNo"].ReadOnly = true;

                if (cmbPrType.Text != "AMOUNT")
                {
                    dgvPrDetails.Columns["Qty"].Visible = true;
                    dgvPrDetails.Columns["Unit"].Visible = true;
                }
                else if (cmbPrType.Text == "AMOUNT")
                {
                    dgvPrDetails.Columns["Amount"].Visible = true;
                    dgvPrDetails.Columns["Unit"].Visible = false;
                }
                dgvPrDetails.Columns["VendId"].Visible = true;
                dgvPrDetails.Columns["Sales Manager Status"].Visible = true;
                dgvPrDetails.Columns["Sales Last Approval ID"].Visible = true;
                dgvPrDetails.Columns["Sales Approval Description"].Visible = true;
                dgvPrDetails.Columns["Purchase Manager Status"].Visible = true;
                dgvPrDetails.Columns["Purchase Last Approval ID"].Visible = true;
                dgvPrDetails.Columns["Purchase Approval Description"].Visible = true;
                dgvPrDetails.Columns["Sales SO"].Visible = false;
                dgvPrDetails.Columns["ExpectedDateFrom"].Visible = false;
                dgvPrDetails.Columns["ExpectedDateTo"].Visible = false;
                dgvPrDetails.Columns["Delivery Method"].Visible = false;
                dgvPrDetails.Columns["Base"].Visible = false;
                dgvPrDetails.Columns["Deskripsi"].Visible = false;
                dgvPrDetails.Columns["SeqNo"].Visible = false;
            }
        }

        private void HeaderPRApproval_Load(object sender, EventArgs e)
        {
          
            RefreshData();
        }

        private void RefreshData()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select h.PurchReqId, h.OrderDate, h.TransType, t.[Deskripsi], h.ApprovedBy, h.TransStatus From [PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode And TransCode = 'PR' Where PurchReqID = '" + PurchReqID + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPrNumber.Text = Dr["PurchReqID"].ToString();
                dtPrDate.Text = Dr["OrderDate"].ToString();
                cmbPrType.SelectedItem = Dr["TransType"].ToString();
                txtPrStatus.Text = Dr["Deskripsi"].ToString();
                cmbPrType.Text = Dr["TransType"].ToString();
                txtPrApproved.Text = Dr["ApprovedBy"].ToString();
                txtTransStatus.Text = Dr["TransStatus"].ToString();
            }
            Dr.Close();

            CreateTable();

            if (ControlMgr.GroupName == "Sales Manager")
            {
                if(cmbPrType.Text !="AMOUNT")
                    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Qty],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' order by SeqNo asc";
                else if (cmbPrType.Text == "AMOUNT")
                    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Amount],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' order by SeqNo asc";
            }
            else if (ControlMgr.GroupName == "Purchase Manager")
            {
                string tempQuery = "Select [SeqNoGroup] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' And TransStatus = 'Yes' order by SeqNo asc";

                List<string> GroupNo = new List<string>();

                Cmd = new SqlCommand(tempQuery, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    GroupNo.Add(Dr[0].ToString());
                }

                string addIn = "";

                for (int x = 0; x < GroupNo.Count; x++)
                {
                    addIn += "'" + GroupNo[x] + "'";
                    if (x == GroupNo.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        addIn += ",";
                    }
                }
                //string temp = "";
                //foreach (string x in GroupNo)
                //{
                //    if (temp == "")
                //    {
                //        temp = "'" + x + "'";
                //    }
                //    else
                //    {
                //        temp += ",'" + x + "'";
                //    }
                //}
                //Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Qty],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' And SeqNoGroup IN (" + temp + ") order by SeqNo asc";
                if (cmbPrType.Text != "AMOUNT")
                {
                    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Qty],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' "; 
                   
                }
                else if (cmbPrType.Text == "AMOUNT")
                {
                    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Amount],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' ";
                }
                if (addIn != "")
                    Query += "And SeqNoGroup IN (" + addIn + ") order by SeqNo asc";

            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            //dgvPrDetails.ReadOnly = true;
            while (Dr.Read())
            {
                string ExpectedDateFrom = Convert.ToDateTime(Dr["ExpectedDateFrom"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr["ExpectedDateFrom"]).ToString("dd-MM-yyyy");
                string ExpectedDateTo = Convert.ToDateTime(Dr["ExpectedDateTo"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr["ExpectedDateTo"]).ToString("dd-MM-yyyy");
                if (cmbPrType.Text == "FIX")
                {

                    this.dgvPrDetails.Rows.Add(Dr["No"], Dr["FullItemID"], Dr["ItemDesc"], Dr["ReffTransID"], ExpectedDateFrom, ExpectedDateTo, Dr["DeliveryMethod"], Dr["Qty"].ToString().Replace(',', '.'), Dr["Unit"], Dr["VendId"], Dr["Base"], Dr["Deskripsi"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"], Dr["No"]);
                }
                else
                {
                    if (Dr["Base"].ToString() == "N")
                    {
                        if (cmbPrType.Text != "AMOUNT")
                            this.dgvPrDetails.Rows.Add(Dr["No"], Dr["FullItemID"], Dr["ItemDesc"], "", "", "", Dr["DeliveryMethod"], Dr["Qty"].ToString().Replace(',', '.'), Dr["Unit"], Dr["VendId"], Dr["Base"], Dr["Deskripsi"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"], Dr["No"]);
                        else if (cmbPrType.Text == "AMOUNT")
                            this.dgvPrDetails.Rows.Add(Dr["No"], Dr["FullItemID"], Dr["ItemDesc"], "", "", "", Dr["DeliveryMethod"], Dr["Amount"].ToString().Replace(',', '.'), Dr["Amount"], Dr["VendId"], Dr["Base"], Dr["Deskripsi"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"], Dr["No"]);
                        
                        dgvPrDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    else
                    {
                        if(cmbPrType.Text != "AMOUNT")
                            this.dgvPrDetails.Rows.Add(Dr["No"], Dr["FullItemID"], Dr["ItemDesc"], Dr["ReffTransID"], ExpectedDateFrom, ExpectedDateTo, Dr["DeliveryMethod"], Dr["Qty"].ToString().Replace(',', '.'), Dr["Unit"], Dr["VendId"], Dr["Base"], Dr["Deskripsi"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"], Dr["No"]);
                        else if (cmbPrType.Text == "AMOUNT")
                            this.dgvPrDetails.Rows.Add(Dr["No"], Dr["FullItemID"], Dr["ItemDesc"], Dr["ReffTransID"], ExpectedDateFrom, ExpectedDateTo, Dr["DeliveryMethod"], Dr["Amount"].ToString().Replace(',', '.'), Dr["Unit"], Dr["VendId"], Dr["Base"], Dr["Deskripsi"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"], Dr["No"]);
                        
                    }
                }
                
                DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                combo2.Items.Add("Pending");
                combo2.Items.Add("Yes");
                combo2.Items.Add("No");
                combo2.Items.Add("Revision");

                if (ControlMgr.GroupName == "Sales Manager")
                {//kalo dikasi status yes ama purchase manager
                    dgvPrDetails.Columns["Purchase Manager Status"].Visible = false;
                    dgvPrDetails.Columns["Purchase Last Approval ID"].Visible = false;
                    dgvPrDetails.Columns["Purchase Approval Description"].Visible = false;

                    if (txtTransStatus.Text == "13" || txtTransStatus.Text == "14" || txtTransStatus.Text == "15")
                    {
                        dgvPrDetails.ReadOnly = true;
                        
                    }
                    else if (txtTransStatus.Text == "12")
                    {
                        if (dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString() == "Revision" || dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString() == "Pending")
                        {
                            dgvPrDetails.Rows[i].Cells["Sales Manager Status"].ReadOnly = false;
                        }
                        else
                        {
                            dgvPrDetails.Rows[i].ReadOnly = true;
                        }
                    }



                    //if (Dr["TransStatus"].ToString() == "13" || Dr["TransStatus"].ToString() == "14" || Dr["TransStatus"].ToString() == "15")
                    //{
                    //    dgvPrDetails.ReadOnly = true;
                    //}

                    //if (Dr2[13].ToString() == null)//transstatuspurch
                    //{

                    //}
                    //else if (Dr2[13].ToString() == "Yes" || Dr2[13].ToString() == "No" || Dr2[13].ToString() == "Pending")
                    //{
                    //    dgvPrDetails.Rows[i].ReadOnly = true;
                    //}
                    //else if (Dr2[13].ToString() == "Revision")
                    //{

                    //}
                }

                if (ControlMgr.GroupName == "Sales Manager")
                {
                    if (cmbPrType.Text == "FIX")
                    {
                        dgvPrDetails.Rows[i].Cells["Sales Manager Status"] = combo2;
                        dgvPrDetails.Rows[i].ReadOnly = false;
                        //dgvPrDetails.Rows[i].Cells["Sales Manager Status"].ReadOnly = false;
                    }
                    else
                    {
                        //matiin yang N
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                        {
                            dgvPrDetails.Rows[i].ReadOnly = true;
                        }
                        else
                        {
                            dgvPrDetails.Rows[i].Cells["Sales Manager Status"] = combo2;
                            dgvPrDetails.Rows[i].ReadOnly = false;
                            //dgvPrDetails.Rows[i].Cells["Sales Manager Status"].ReadOnly = false;
                        }
                    }
                    

                    if (Dr["TransStatus"] != null)
                    {
                        if (dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString() == "Revision" || dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString() == "Pending")
                            dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value = "";
                        else
                        dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value = Dr["TransStatus"].ToString();
                    }

                    dgvPrDetails.Columns["Purchase Manager Status"].ReadOnly = true;
                    dgvPrDetails.Columns["Purchase Approval Description"].ReadOnly = true;


                }
                else if (ControlMgr.GroupName == "Purchase Manager")
                {
                    if (cmbPrType.Text == "FIX")
                    {
                        dgvPrDetails.Rows[i].Cells["Purchase Manager Status"] = combo2;
                    }
                    else
                    {
                        //matiin yang N
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                        {
                            dgvPrDetails.Rows[i].ReadOnly = true;
                            dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].ReadOnly = true;
                        }
                        else
                        {
                            dgvPrDetails.Rows[i].Cells["Purchase Manager Status"] = combo2;
                        }
                    }
                    

                    if (Dr["TransStatusPurch"] != null)
                    {
                       dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value = Dr["TransStatusPurch"].ToString();

                    }

                    dgvPrDetails.Columns["Sales Manager Status"].ReadOnly = true;
                    dgvPrDetails.Columns["Sales Approval Description"].ReadOnly = true;


                }

                //untuk isi combobox via database

                //Conn = ConnectionString.GetConnection();
                //Query = "Select [Deskripsi] From dbo.[TransStatusTable]";
                //Cmd = new SqlCommand(Query, Conn);
                //SqlDataReader DrCmb2;
                //DrCmb2 = Cmd.ExecuteReader();
                //DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
                //while (DrCmb2.Read())
                //{
                //    combo2.Items.Add(DrCmb2[0].ToString());
                //}
                //DrCmb2.Close();
                //if (Dr[10] != null)
                //{
                //    combo2.Value = Dr[10].ToString();
                //}
                //dgvPrDetails.Rows[i].Cells[14] = combo2;
                dgvPrDetails.AutoResizeColumns();

                i++;

            }
            Dr.Close();
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void counterSaveSalesManager()
        {
            //Check Counter FIX/QTY/AMOUNT PENDING/REV/YES/NO
            //header
            for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            {
                if (cmbPrType.Text == "FIX")
                {
                    String detailStatus = dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString();
                    //salesGridStatus = detailStatus;
                    if (string.IsNullOrEmpty(detailStatus))
                        counterWait++;
                    else if (detailStatus == "Pending")
                        counterPend++;
                    else if (detailStatus == "Revision")
                        counterRev++;
                    else if (detailStatus == "Yes")
                        counterApp++;
                    else if (detailStatus == "No")
                        counterNo++;
                }
                else
                {
                    if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        if (cmbPrType.Text == "FIX" || cmbPrType.Text == "QTY" || cmbPrType.Text == "AMOUNT")
                        {
                            String detailStatus = dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString();
                            //salesGridStatus = detailStatus;
                            if (string.IsNullOrEmpty(detailStatus))
                                counterWait++;
                            else if (detailStatus == "Pending")
                                counterPend++;
                            else if (detailStatus == "Revision")
                                counterRev++;
                            else if (detailStatus == "Yes")
                                counterApp++;
                            else if (detailStatus == "No")
                                counterNo++;
                        }
                    }
                }

                ////int x = i + 1;
                ////Query = "SELECT TransStatus FROM [PurchRequisition_Dtl] where [PurchReqId]= '" + PurchReqID + "' and SeqNo = '"+ x +"' ";
                ////Cmd = new SqlCommand(Query, Conn);
                ////string salesGridStatusOld = Cmd.ExecuteScalar().ToString();

                //if (salesGridStatusOld == "Yes" && salesGridStatus == "No")
                //{
                //    salesGridStatus = "YesToNo";
                //}


            }

            if (counterWait > 0)
            {
                MessageBox.Show("Status harus diisi.");
                counterWait = 0;
                update = false;
                return;
            }

            if (counterRev > 0 || counterPend > 0)
            {
                StatusHeader = "02";
                complete = "no";
            }
            else
            {
                if (counterApp == 0)
                {
                    StatusHeader = "05";
                    
                }
                else if (counterNo == 0)
                {
                    StatusHeader = "03";
                }
                else
                {
                    StatusHeader = "04";
                }
            }

            #region Set TransStatus
            String Query2 = "Select [Deskripsi] From [dbo].[TransStatusTable] Where StatusCode = '" + StatusHeader + "'";
            String NewStatusDesc = "";
            Cmd = new SqlCommand(Query2, Conn);
            NewStatusDesc = Cmd.ExecuteScalar().ToString();

            if (StatusHeader == "03" || StatusHeader == "04")
            {
                Query = "Update PurchRequisitionH Set TransStatus ='" + StatusHeader + "', ApprovedBy = '" + ControlMgr.UserId + "' Where PurchReqId = '" + PurchReqID + "'";
            }
            else
            {
                Query = "Update PurchRequisitionH Set TransStatus ='" + StatusHeader + "' Where PurchReqId = '" + PurchReqID + "'";
            }

            if (txtPrStatus.Text != NewStatusDesc)
            {
                Query += "INSERT INTO [dbo].[WorkflowLogTable]([ReffTableName],[ReffID],[ReffDate],[ReffSeqNo],[UserID],[WorkFlow],[LogStatus],[StatusDesc],[LogDate]) ";
                Query += "Values ('PurchRequisitionH', '" + PurchReqID + "','" + dtPrDate.Value.ToString("yyyy-MM-dd") + "','','" + ControlMgr.UserId + "','" + ControlMgr.GroupName + "','" + StatusHeader + "','" + NewStatusDesc + "',getDate())";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            ListMethod ListMethod1 = new ListMethod();
            #endregion
        }

        private void saveSalesManager()
        {
            counterSaveSalesManager();          

            updateInventPurchaseSM();            
        }

        private void updateInventPurchaseSM()
        {
            #region delete berdasarkan jumlah PR yg Yes
            int JumlahPR = 0;
            int JumlahRow = 0;
            string QueryTemp = "";
            string TransStatus = "";
            string FullItemId = "";
            string Unit = "";
            string UoM = "";
            decimal QtyOld = 0;
            decimal QtyUoM = 0;
            decimal QtyAlt = 0;
            decimal ConvRatio = 0;
            string PRType = "";

            Query = "SELECT COUNT (PurchReqId) FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            JumlahPR = (int)Cmd.ExecuteScalar();
            JumlahRow = (int)dgvPrDetails.RowCount;

            //PRType = dgvPrDetails.CurrentRow.Cells["TransType"].Value == null ? "" : dgvPrDetails.CurrentRow.Cells["TransType"].Value.ToString();
            for (int x = 1; x <= JumlahPR; x++)
            {
                QueryTemp = "SELECT TransStatus FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + x + "'";
                Cmd = new SqlCommand(QueryTemp, Conn);
                TransStatus = Cmd.ExecuteScalar().ToString();

                QueryTemp = "SELECT [TransStatusPurch] FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + x + "'";
                Cmd = new SqlCommand(QueryTemp, Conn);
                string checkStatusPM = Cmd.ExecuteScalar().ToString();

                if (checkStatusPM == "Revision")
                    TransStatus = "Revision";

                QueryTemp = "SELECT FullItemId FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + x + "'";
                Cmd = new SqlCommand(QueryTemp, Conn);
                FullItemId = Cmd.ExecuteScalar().ToString();

                if (cmbPrType.Text != "AMOUNT")
                {
                    QueryTemp = "Select [Unit] From [PurchRequisition_Dtl] Where PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + x + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    Unit = Cmd.ExecuteScalar().ToString();

                    QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    UoM = Cmd.ExecuteScalar().ToString();

                    QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    ConvRatio = (decimal)Cmd.ExecuteScalar();

                    QueryTemp = "SELECT Qty FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + x + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    QtyOld = (decimal)Cmd.ExecuteScalar();

                    if (Unit == UoM)
                    {
                        QtyUoM = QtyOld;
                        QtyAlt = QtyOld * ConvRatio;
                    }
                    else
                    {
                        QtyAlt = QtyOld;
                        QtyUoM = QtyOld / ConvRatio;
                    }

                    
                }

                if (cmbPrType.Text == "AMOUNT")
                {
                    QueryTemp = "SELECT Amount FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + x + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    QtyOld = (decimal)Cmd.ExecuteScalar();

                    
                }
            }
            #endregion

            //detail
            for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            {
                #region ADD berdasarkan grid
                #region Variable
                //String ApprovePerson = dgvPrDetails.Rows[i].Cells["Approval ID"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Approval ID"].Value.ToString();
                String ApprovalNotes = dgvPrDetails.Rows[i].Cells["Sales Approval Description"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Approval Description"].Value.ToString().Trim();

                string TmpStatus = dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString();
                double TmpQty = 0;
                if (cmbPrType.Text != "AMOUNT")
                    TmpQty = dgvPrDetails.Rows[i].Cells["Qty"].Value == null ? 0 : Convert.ToDouble(dgvPrDetails.Rows[i].Cells["Qty"].Value);
                else if (cmbPrType.Text == "AMOUNT")
                    TmpQty = dgvPrDetails.Rows[i].Cells["Amount"].Value == null ? 0 : Convert.ToDouble(dgvPrDetails.Rows[i].Cells["Amount"].Value);

                string TmpFullItemId = dgvPrDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString();
                Unit = dgvPrDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString();
                //if (false == ListMethod1.PRApproved1TM(Conn, PurchReqID, (i + 1).ToString(), TmpStatus, TmpQty, Unit, TmpFullItemId, cmbPrType.Text.ToUpper()))
                //{
                //    MessageBox.Show(ListMethod1.MessageBox.ToString());
                //    return;
                //}
                salesGridStatus = dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString();
                #endregion

                #region Update PurchRequisition_Dtl
                Query = "Update PurchRequisition_Dtl Set TransStatus = '" + ((dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString())) + "', ";
                Query += "[ApprovalNotes]='" + ApprovalNotes + "', ";
                Query += "[ApprovePerson]='" + ControlMgr.UserId + "', ";
                Query += "[TransStatusPurch] = ''";
                Query += "Where PurchReqID = '" + PurchReqID + "' AND TransStatusPurch NOT IN('Yes','No') And SeqNo = '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                String StatusDesc = null;
                Query = "SELECT Deskripsi FROM TransStatusTable ";
                Query += "WHERE StatusCode='" + StatusHeader + "' AND TransCode = 'PR' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                }
                Dr.Close();
                #endregion

                #region Variable
                //[PurchRequisition_LogTable]
                FullItemId = dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString();
                decimal QtyInput = 0;
                if (cmbPrType.Text != "AMOUNT")
                    QtyInput = decimal.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString());
                if (cmbPrType.Text == "AMOUNT")
                    QtyInput = decimal.Parse(dgvPrDetails.Rows[i].Cells["Amount"].Value.ToString());
                Unit = dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString();
                ConvRatio = 0;
                QtyUoM = 0;
                QtyAlt = 0;
                #endregion

                #region Convert
                QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                Cmd = new SqlCommand(QueryTemp, Conn);
                UoM = Cmd.ExecuteScalar().ToString();

                QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                Cmd = new SqlCommand(QueryTemp, Conn);
                ConvRatio = (decimal)Cmd.ExecuteScalar();

                if (ConvRatio == 0)
                {
                    ConvRatio = 1;
                }
                if (Unit == UoM)
                {
                    QtyUoM = QtyInput;
                    QtyAlt = QtyInput * ConvRatio;
                }
                else
                {
                    QtyAlt = QtyInput;
                    QtyUoM = QtyInput / ConvRatio;
                }
                #endregion

                string Status = ((dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString()));

                
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                #endregion

                //Cek apakah data sudah ada di PurchRequisition_LogTable
                Query = "Select [PurchReqID] from PurchRequisition_LogTable where LogStatusCode in ('02','03','04','05') and PurchReqId='" + txtPrNumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                String CheckData = (Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString());

                if (CheckData == "")
                {
                    if (cmbPrType.Text != "AMOUNT")
                    {
                        Query = "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[Qty_UoM],[Qty_Alt]) ";
                        Query += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPrNumber.Text.Trim() + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "', '" + StatusHeader + "' ,'" + Status + StatusDesc + "' ,'" + Status + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + QtyUoM + "','" + QtyAlt + "');";

                    }
                    else if (cmbPrType.Text == "AMOUNT")
                    {
                        Query = "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],Amount) ";
                        Query += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPrNumber.Text.Trim() + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "', '" + StatusHeader + "' ,'" + Status + " - " + StatusDesc + "' ,'" + Status + " - " + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + QtyInput + "');";
                    }
                }
                else
                {
                    if (cmbPrType.Text != "AMOUNT")
                    {
                        Query = "Update [PurchRequisition_LogTable] Set [PurchReqDate]='" + dtPrDate.Value.ToString("yyyy-MM-dd") + "',";
                        Query += "[PurchReqType]='" + cmbPrType.Text.Trim() + "',";
                        Query += "[LogStatusCode]='" + StatusHeader + "',";
                        Query += "[LogStatusDesc]='" + Status + StatusDesc + "',";
                        Query += "[LogDescription]='" + Status + StatusDesc + " By User " + ControlMgr.UserId + "',";
                        Query += "[UserID]='" + ControlMgr.UserId + "',";
                        Query += "[LogDate]=getdate(),";
                        Query += "[Qty_UoM]='" + QtyUoM + "',";
                        Query += "[Qty_Alt]='" + QtyAlt + "' where [PurchReqID]='" + txtPrNumber.Text.Trim() + "' and [PurchReqSeqNo]='" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "';";
                    }
                    else if (cmbPrType.Text == "AMOUNT")
                    {
                        Query = "Update [PurchRequisition_LogTable] Set [PurchReqDate]='" + dtPrDate.Value.ToString("yyyy-MM-dd") + "',";
                        Query += "[PurchReqType]='" + cmbPrType.Text.Trim() + "',";
                        Query += "[LogStatusCode]='" + StatusHeader + "',";
                        Query += "[LogStatusDesc]='" + Status + StatusDesc + "',";
                        Query += "[LogDescription]='" + Status + StatusDesc + " By User " + ControlMgr.UserId + "',";
                        Query += "[UserID]='" + ControlMgr.UserId + "',";
                        Query += "[LogDate]=getdate(),";
                        Query += "Amount='" + QtyInput + "' where [PurchReqID]='" + txtPrNumber.Text.Trim() + "' and [PurchReqSeqNo]='" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "';";
                    }
                }
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }

            Query = "EXEC [dbo].[stockview_pr] @pr_id, @complete ,@amount_or_qty , 'sm'  ; ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@pr_id", txtPrNumber.Text.Trim());
            Cmd.Parameters.AddWithValue("@complete", complete);
            Cmd.Parameters.AddWithValue("@amount_or_qty", cmbPrType.Text);
            Cmd.ExecuteNonQuery();
        }

        private void counterSavePurchaseManager()
        {
            for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            {
                if (cmbPrType.Text == "FIX")
                {
                    String detailStatus = dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString();

                    if (string.IsNullOrEmpty(detailStatus))
                        counterWait++;
                    else if (detailStatus == "Pending")
                        counterPend++;
                    else if (detailStatus == "Revision")
                        counterRev++;
                    else if (detailStatus == "Yes")
                        counterApp++;
                    else if (detailStatus == "No")
                        counterNo++;
                }
                else
                {
                    if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        String detailStatus = dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString();

                        if (string.IsNullOrEmpty(detailStatus))
                            counterWait++;
                        else if (detailStatus == "Pending")
                            counterPend++;
                        else if (detailStatus == "Revision")
                            counterRev++;
                        else if (detailStatus == "Yes")
                            counterApp++;
                        else if (detailStatus == "No")
                            counterNo++;
                    }
                }
            }

            if (counterWait > 0)
            {
                MessageBox.Show("Status harus diisi.");
                counterWait = 0;
                update = false;
                return;
            }

            if (counterRev > 0 || counterPend > 0)
            {
                StatusHeader = "12";   // Purchase Manager Approval Status = Pending atau Revision
                complete = "no";
            }
            else
            {
                if (counterApp == 0)
                {
                    StatusHeader = "15";  // Purchase Manager Approval Status = Yes
                    
                }
                else if (counterNo == 0)
                {
                    StatusHeader = "13";   // Purchase Manager Approval Status = No
                }
                else
                {
                    StatusHeader = "14";  // Purchase Manager Approval Status = blank
                }
            }

            String Query2 = "Select [Deskripsi] From [dbo].[TransStatusTable] Where StatusCode = '" + StatusHeader + "'";
            String NewStatusDesc = "";
            Cmd = new SqlCommand(Query2, Conn);
            NewStatusDesc = Cmd.ExecuteScalar().ToString();

            if (StatusHeader == "13" || StatusHeader == "14")
            {
                Query = "Update PurchRequisitionH Set TransStatus ='" + StatusHeader + "', ApprovedBy ='" + ControlMgr.UserId + "' Where PurchReqId = '" + PurchReqID + "'";
            }
            else
            {
                Query = "Update PurchRequisitionH Set TransStatus ='" + StatusHeader + "' Where PurchReqId = '" + PurchReqID + "'";
            }

            if (txtPrStatus.Text != NewStatusDesc)
            {
                Query += "INSERT INTO [dbo].[WorkflowLogTable]([ReffTableName],[ReffID],[ReffDate],[ReffSeqNo],[UserID],[WorkFlow],[LogStatus],[StatusDesc],[LogDate]) ";
                Query += "Values ('PurchRequisitionH', '" + PurchReqID + "','" + dtPrDate.Value + "','','" + ControlMgr.UserId + "','" + ControlMgr.GroupName + "','" + StatusHeader + "','" + NewStatusDesc + "',getDate())";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
        }

        private void savePurchaseManager()
        {
            counterSavePurchaseManager();

            updateInventPurchasePM();
        }

        string purchaseGridStatus;
        decimal QtyInput;
        private void updateInventPurchasePM()
        {
            ListMethod ListMethod1 = new ListMethod();

            int JumlahPR = 0;
            int JumlahRow = 0;
            string QueryTemp = "";
            string TransStatus = "";
            string FullItemId = "";
            string Unit = "";
            string UoM = "";
            decimal QtyOld = 0;
            decimal QtyUoM = 0;
            decimal QtyAlt = 0;
            decimal ConvRatio = 0;

            #region Add berdasarkan grid
            //detail
            for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            {
                #region Variable
                String ApprovalNotes = dgvPrDetails.Rows[i].Cells["Purchase Approval Description"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Approval Description"].Value.ToString().Trim();

                #endregion

                #region Set TransStatus Purch
                Query = "Update PurchRequisition_Dtl Set TransStatusPurch = '" + ((dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString())) + "', ";
                Query += "[ApprovalNotesPurch]='" + ApprovalNotes + "', ";
                Query += "[ApprovePerson]='" + ControlMgr.UserId + "' Where PurchReqID = '" + PurchReqID + "' And SeqNo = '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();

                String StatusDesc = null;
                Query = "SELECT Deskripsi FROM TransStatusTable ";
                Query += "WHERE StatusCode='" + StatusHeader + "' AND TransCode = 'PR' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                }
                Dr.Close();
                #endregion

               
                

                //Cek apakah data sudah ada di PurchRequisition_LogTable
                Query = "Select [PurchReqID] from PurchRequisition_LogTable where LogStatusCode in ('12','13','14','15') and PurchReqId='" + txtPrNumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                String CheckData = (Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString());
            #endregion

                #region uom,alt,amount
                if (cmbPrType.Text != "AMOUNT")
                {
                    QueryTemp = "Select [Unit] From [PurchRequisition_Dtl] Where PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    Unit = Cmd.ExecuteScalar().ToString();

                    QueryTemp = "SELECT FullItemId FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    FullItemId = Cmd.ExecuteScalar().ToString();

                    QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    UoM = Cmd.ExecuteScalar().ToString();

                    QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    ConvRatio = (decimal)Cmd.ExecuteScalar();

                    QueryTemp = "SELECT Qty FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    QtyOld = (decimal)Cmd.ExecuteScalar();

                    if (Unit == UoM)
                    {
                        QtyUoM = QtyOld;
                        QtyAlt = QtyOld * ConvRatio;
                    }
                    else
                    {
                        QtyAlt = QtyOld;
                        QtyUoM = QtyOld / ConvRatio;
                    }
                }

                if (cmbPrType.Text == "AMOUNT")
                {
                    QueryTemp = "SELECT Amount FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "' and [SeqNo] = '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    QtyInput = (decimal)Cmd.ExecuteScalar();
                }
                #endregion

                string Status = ((dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString()));



                //Jika data belum ada di LogsTable
                if (CheckData == "")
                {
                    if (cmbPrType.Text != "AMOUNT")
                    {
                        Query = "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[Qty_UoM],[Qty_Alt]) ";
                        Query += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPrNumber.Text.Trim() + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "', '" + StatusHeader + "' ,'" + Status + " - " + StatusDesc + "' ,'" + Status + " - " + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + QtyUoM + "','" + QtyAlt + "');";
                    }
                    else if (cmbPrType.Text == "AMOUNT")
                    {
                        Query = "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],Amount) ";
                        Query += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + txtPrNumber.Text.Trim() + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "', '" + StatusHeader + "' ,'" + Status + " - " + StatusDesc + "' ,'" + Status + " - " + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + QtyInput + "')";
                    }
                }
                else
                {
                    if (cmbPrType.Text != "AMOUNT")
                    {
                        Query = "";
                        Query = "Update [PurchRequisition_LogTable] Set [PurchReqDate]='" + dtPrDate.Value.ToString("yyyy-MM-dd") + "',";
                        Query += "[PurchReqType]='" + cmbPrType.Text.Trim() + "',";
                        Query += "[LogStatusCode]='" + StatusHeader + "',";
                        Query += "[LogStatusDesc]='" + Status + StatusDesc + "',";
                        Query += "[LogDescription]='" + Status + StatusDesc + " By User " + ControlMgr.UserId + "',";
                        Query += "[UserID]='" + ControlMgr.UserId + "',";
                        Query += "[LogDate]=getdate(),";
                        Query += "[Qty_UoM]='" + QtyUoM + "',";
                        Query += "[Qty_Alt]='" + QtyAlt + "' where [PurchReqID]='" + txtPrNumber.Text.Trim() + "' and [PurchReqSeqNo]='" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "';";
                    }
                    else if (cmbPrType.Text == "AMOUNT")
                    {
                        Query = "";
                        Query = "Update [PurchRequisition_LogTable] Set [PurchReqDate]='" + dtPrDate.Value.ToString("yyyy-MM-dd") + "',";
                        Query += "[PurchReqType]='" + cmbPrType.Text.Trim() + "',";
                        Query += "[LogStatusCode]='" + StatusHeader + "',";
                        Query += "[LogStatusDesc]='" + Status + StatusDesc + "',";
                        Query += "[LogDescription]='" + Status + StatusDesc + " By User " + ControlMgr.UserId + "',";
                        Query += "[UserID]='" + ControlMgr.UserId + "',";
                        Query += "[LogDate]=getdate(),";
                        Query += "Amount='" + QtyInput + "' where [PurchReqID]='" + txtPrNumber.Text.Trim() + "' and [PurchReqSeqNo]='" + dgvPrDetails.Rows[i].Cells["SeqNo"].Value.ToString() + "';";
                    }
                }
                if (Query != "")
                {
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                }

            }
            Query = "EXEC [dbo].[stockview_pr] @pr_id, @complete ,@amount_or_qty , 'pm'  ; ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@pr_id", txtPrNumber.Text.Trim());
            Cmd.Parameters.AddWithValue("@complete", complete);
            Cmd.Parameters.AddWithValue("@amount_or_qty", cmbPrType.Text);
            Cmd.ExecuteNonQuery();
        }

        String StatusHeader = "";
        int counterWait = 0;
        int counterRev = 0;
        int counterApp = 0;
        int counterNo = 0;
        int counterPend = 0;
        string salesGridStatus = "";
        bool update = true;

        private void btnSave_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                using (TransactionScope Scope = new TransactionScope())
                {
                    try
                    {
                        Conn = ConnectionString.GetConnection();
                        //Conn2 = ConnectionString.GetConnection();
                        //String StatusHeader = "";
                        //int counterWait = 0;
                        //int counterRev = 0;
                        //int counterApp = 0;
                        //int counterNo = 0;
                        //int counterPend = 0;
                        //string salesGridStatus = "";
                        //int JumlahPR = 0;

                        if (!ValidasiPR())
                            return;

                        if (ControlMgr.GroupName == "Sales Manager")
                        {
                            saveSalesManager();
                        }
                        else if (ControlMgr.GroupName == "Purchase Manager")
                        {
                            savePurchaseManager();
                        }
                        Scope.Complete();
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end

            if (update == true)
            {
                MessageBox.Show("Data : " + txtPrNumber.Text.Trim() + " berhasil diupdate.");
                this.Close();
            }
            update = true;
        }

        private void HeaderPRApproval_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Parent2==null)
            {
                Parent.RefreshGrid();
            }
            else
            {
                Parent2.RefreshGrid();
            }
           
        }

        private void tabDgvControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dgvPrDetails.Columns.Count != 0)
            {
                if (tabDgvControl.SelectedTab.Text == "Approval")
                {
                    if (cmbPrType.Text.ToUpper() != "AMOUNT")
                        dgvPrDetails.Columns["Qty"].Visible = true;
                    else
                        dgvPrDetails.Columns["AMOUNT"].Visible = true;
                    dgvPrDetails.Columns["Unit"].Visible = true;
                    dgvPrDetails.Columns["VendId"].Visible = true;
                    dgvPrDetails.Columns["Sales Manager Status"].Visible = true;
                    dgvPrDetails.Columns["Sales Last Approval ID"].Visible = true;
                    dgvPrDetails.Columns["Sales Approval Description"].Visible = true;
                    dgvPrDetails.Columns["Purchase Manager Status"].Visible = true;
                    dgvPrDetails.Columns["Purchase Last Approval ID"].Visible = true;
                    dgvPrDetails.Columns["Purchase Approval Description"].Visible = true;
                    dgvPrDetails.Columns["Sales SO"].Visible = false;
                    dgvPrDetails.Columns["ExpectedDateFrom"].Visible = false;
                    dgvPrDetails.Columns["ExpectedDateTo"].Visible = false;
                    dgvPrDetails.Columns["Delivery Method"].Visible = false;
                    dgvPrDetails.Columns["Base"].Visible = false;
                    dgvPrDetails.Columns["Deskripsi"].Visible = false;
                    dgvPrDetails.Columns["SeqNo"].Visible = false;
                }
                else
                {
                    if (cmbPrType.Text.ToUpper() != "AMOUNT")
                        dgvPrDetails.Columns["Qty"].Visible = true;
                    else
                        dgvPrDetails.Columns["AMOUNT"].Visible = true;
                    dgvPrDetails.Columns["Unit"].Visible = false;
                    dgvPrDetails.Columns["VendId"].Visible = false;
                    dgvPrDetails.Columns["Sales Manager Status"].Visible = false;
                    dgvPrDetails.Columns["Sales Last Approval ID"].Visible = false;
                    dgvPrDetails.Columns["Sales Approval Description"].Visible = false;
                    dgvPrDetails.Columns["Purchase Manager Status"].Visible = false;
                    dgvPrDetails.Columns["Purchase Last Approval ID"].Visible = false;
                    dgvPrDetails.Columns["Purchase Approval Description"].Visible = false;
                    dgvPrDetails.Columns["Sales SO"].Visible = true;
                    dgvPrDetails.Columns["ExpectedDateFrom"].Visible = true;
                    dgvPrDetails.Columns["ExpectedDateTo"].Visible = true;
                    dgvPrDetails.Columns["Delivery Method"].Visible = true;
                    dgvPrDetails.Columns["Base"].Visible = true;
                    dgvPrDetails.Columns["Deskripsi"].Visible = true;
                    dgvPrDetails.Columns["SeqNo"].Visible = false;
                }
            }
        }

        private bool CheckForm(Form form)
        {
            form = Application.OpenForms[form.Text];
            if (form != null)
                return true;
            else
                return false;
        }
        //tia edit
        //klik kanan
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.FullItemId.FullItemId FID = null;
        Sales.SalesOrder.SOHeader PUSO = null;

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvPrDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {

                if (FID == null || FID.Text == "")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvPrDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
                if (Vendor == null || Vendor.Text == "")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "VendId")
                    {
                        Vendor = new PopUp.Vendor.Vendor();
                        Vendor.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["VendId"].Value.ToString());

                        Vendor.Show();
                     
                    }
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["VendId"].Value.ToString());
                    Vendor.Show();
                    Vendor.Focus();
                }
                if (PUSO == null || PUSO.Text == "")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Sales SO")
                    {
                        PUSO = new Sales.SalesOrder.SOHeader();
                        PUSO.SetMode("PopUp", dgvPrDetails.Rows[e.RowIndex].Cells["Sales SO"].Value.ToString());
                        PUSO.ParentRefreshGrid2(this);
                        PUSO.Show();
                    }
                }
                else if (CheckOpened(PUSO.Name))
                {
                    PUSO.WindowState = FormWindowState.Normal;
                    PUSO.SetMode("PopUp", dgvPrDetails.Rows[e.RowIndex].Cells["Sales SO"].Value.ToString());
                    PUSO.ParentRefreshGrid2(this);
                    PUSO.Show();
                    PUSO.Focus();
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
        //tia edit end

        //Validasi PR jika sudah di approve / sudah di create RFQ 
        private bool ValidasiPR()
        {
            if (txtPrNumber.Text != "")
            {
                string Query = "SELECT [TransStatus] FROM [PurchRequisitionH] where PurchReqID='" + txtPrNumber.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, ConnectionString.GetConnection());
                string TmpTransStatus = Cmd.ExecuteScalar().ToString();
                if (ControlMgr.GroupName == "Sales Manager")
                {
                    if (int.Parse(TmpTransStatus) == 1 || int.Parse(TmpTransStatus) == 2 || int.Parse(TmpTransStatus) == 3 || int.Parse(TmpTransStatus) == 4 || int.Parse(TmpTransStatus) == 5 || int.Parse(TmpTransStatus) == 12)
                    {
                        return true;
                    }
                    //MessageBox.Show("Approval tidak dapat karena status document telah berubah (" + TmpTransStatus + ").");
                    //return false;
                }
                else if (ControlMgr.GroupName == "Purchase Manager")
                {
                    if (int.Parse(TmpTransStatus) == 12 || int.Parse(TmpTransStatus) == 13 || int.Parse(TmpTransStatus) == 14 || int.Parse(TmpTransStatus) == 3 || int.Parse(TmpTransStatus) == 4 || int.Parse(TmpTransStatus) == 5 || int.Parse(TmpTransStatus) == 15)
                    {
                        return true;
                    }
                    //MessageBox.Show("Approval tidak dapat karena status document telah berubah (" + TmpTransStatus + ")."); 
                    //return false;
                }
                else
                {
                    MessageBox.Show("Approval tidak dapat dilakukan karena user group bukan Sales Manager / Purchase Manager.");
                    return false;
                }
            }
            MessageBox.Show("PR Number masih kosong sehingga approval tidak dapat dilakukan.");
            return false;
        }

        private void dgvPrDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPrDetails.Columns.Contains("Qty"))
            {
                if (e.ColumnIndex == dgvPrDetails.Columns["Qty"].Index && e.Value != null)
                {
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N2");
                }
            }
            if (dgvPrDetails.Columns.Contains("Amount"))
            {
                if (e.ColumnIndex == dgvPrDetails.Columns["Amount"].Index && e.Value != null)
                {
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N4");
                }
            }
        }


    }
}
