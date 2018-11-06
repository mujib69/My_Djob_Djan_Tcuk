using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.DashboardApprovalPR
{
    public partial class DashboardHeaderPRApproval : Form
    {
        //private SqlConnection Conn;
        //private SqlConnection Conn2;
        //private SqlCommand Cmd;
        //private SqlTransaction Trans;
        //private SqlDataReader Dr;
        //private SqlDataAdapter Da;
        //private DataTable Dt;
        //private DataSet Ds;

        //String Mode, Query;
        //int Limit1, Limit2, Total, Page1, Page2, Index;
        //String PurchReqID = null;

        //PopUp.DashboardApprovalPR.DashboardApprovalPR Parent;

        public DashboardHeaderPRApproval()
        {
            //InitializeComponent();
        }

        public void setParent(PopUp.DashboardApprovalPR.DashboardApprovalPR F)
        {
            //Parent = F;
        }

        public void flag(String purchreqid)
        {
            //PurchReqID = purchreqid;
        }

        private void CreateTable()
        {
            //if (dgvPrDetails.RowCount - 1 <= 0)
            //{
            //    dgvPrDetails.ColumnCount = 18;
            //    dgvPrDetails.Columns[0].Name = "No";
            //    dgvPrDetails.Columns[1].Name = "FullItemID";
            //    dgvPrDetails.Columns[2].Name = "ItemName";
            //    dgvPrDetails.Columns[3].Name = "ReffTransID";
            //    dgvPrDetails.Columns[4].Name = "ExpectedDateFrom";
            //    dgvPrDetails.Columns[5].Name = "ExpectedDateTo";
            //    dgvPrDetails.Columns[6].Name = "Delivery Method";
            //    dgvPrDetails.Columns[7].Name = "Qty";
            //    dgvPrDetails.Columns[8].Name = "Unit";
            //    dgvPrDetails.Columns[9].Name = "VendId";
            //    dgvPrDetails.Columns[10].Name = "Base";
            //    dgvPrDetails.Columns[11].Name = "Deskripsi";
            //    dgvPrDetails.Columns[12].Name = "Sales Manager Status";
            //    dgvPrDetails.Columns[13].Name = "Sales Last Approval ID";
            //    dgvPrDetails.Columns[14].Name = "Sales Approval Description";
            //    dgvPrDetails.Columns[15].Name = "Purchase Manager Status";
            //    dgvPrDetails.Columns[16].Name = "Purchase Last Approval ID";
            //    dgvPrDetails.Columns[17].Name = "Purchase Approval Description";

            //    dgvPrDetails.ReadOnly = false;
            //    dgvPrDetails.Columns["No"].ReadOnly = true;
            //    dgvPrDetails.Columns["FullItemID"].ReadOnly = true;
            //    dgvPrDetails.Columns["ItemName"].ReadOnly = true;
            //    dgvPrDetails.Columns["ReffTransID"].ReadOnly = true;
            //    dgvPrDetails.Columns["Delivery Method"].ReadOnly = true;
            //    dgvPrDetails.Columns["ExpectedDateFrom"].ReadOnly = true;
            //    dgvPrDetails.Columns["ExpectedDateTo"].ReadOnly = true;
            //    dgvPrDetails.Columns["Qty"].ReadOnly = true;
            //    dgvPrDetails.Columns["Unit"].ReadOnly = true;
            //    dgvPrDetails.Columns["VendId"].ReadOnly = true;
            //    dgvPrDetails.Columns["Base"].ReadOnly = true;
            //    dgvPrDetails.Columns["Sales Last Approval ID"].ReadOnly = true;
            //    dgvPrDetails.Columns["Purchase Last Approval ID"].ReadOnly = true;

            //    dgvPrDetails.Columns["Qty"].Visible = true;
            //    dgvPrDetails.Columns["Unit"].Visible = true;
            //    dgvPrDetails.Columns["VendId"].Visible = true;
            //    dgvPrDetails.Columns["Sales Manager Status"].Visible = true;
            //    dgvPrDetails.Columns["Sales Last Approval ID"].Visible = true;
            //    dgvPrDetails.Columns["Sales Approval Description"].Visible = true;
            //    dgvPrDetails.Columns["Purchase Manager Status"].Visible = true;
            //    dgvPrDetails.Columns["Purchase Last Approval ID"].Visible = true;
            //    dgvPrDetails.Columns["Purchase Approval Description"].Visible = true;
            //    dgvPrDetails.Columns["ReffTransID"].Visible = false;
            //    dgvPrDetails.Columns["ExpectedDateFrom"].Visible = false;
            //    dgvPrDetails.Columns["ExpectedDateTo"].Visible = false;
            //    dgvPrDetails.Columns["Delivery Method"].Visible = false;
            //    dgvPrDetails.Columns["Base"].Visible = false;
            //    dgvPrDetails.Columns["Deskripsi"].Visible = false;
            //}
        }

        private void DashboardHeaderPRApproval_Load(object sender, EventArgs e)
        {
            //RefreshData();
        }

        private void RefreshData()
        {
        //    Conn = ConnectionString.GetConnection();
        //    Query = "Select *,t.[Deskripsi] From [PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode Where PurchReqID = '" + PurchReqID + "'";

        //    Cmd = new SqlCommand(Query, Conn);
        //    Dr = Cmd.ExecuteReader();

        //    while (Dr.Read())
        //    {
        //        txtPrNumber.Text = Dr["PurchReqID"].ToString();
        //        dtPrDate.Text = Dr["OrderDate"].ToString();
        //        cmbPrType.SelectedItem = Dr["TransType"].ToString();
        //        txtPrStatus.Text = Dr["Deskripsi"].ToString();
        //        cmbPrType.Text = Dr["TransType"].ToString();
        //        txtPrApproved.Text = Dr["ApprovedBy"].ToString();
        //        txtTransStatus.Text = Dr["TransStatus"].ToString();
        //    }
        //    Dr.Close();

        //    CreateTable();

        //    if (Login.UserGroup == "SalesManager")
        //    {
        //        Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Qty],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' order by SeqNo asc";
        //    }
        //    else if (Login.UserGroup == "PurchaseManager")
        //    {
        //        Query = "Select [SeqNoGroup] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' And TransStatus = 'Yes' order by SeqNo asc";

        //        List<string> GroupNo = new List<string>();

        //        Cmd = new SqlCommand(Query,Conn);
        //        Dr = Cmd.ExecuteReader(); 
        //        while (Dr.Read())
        //        {
        //            GroupNo.Add(Dr["SeqNoGroup"].ToString());
        //        }

        //        string temp = "";
        //        foreach (string x in GroupNo)
        //        {
        //            if (temp == "")
        //            {
        //                temp = x;
        //            }
        //            temp += ",'"+x+"'";
        //        }

        //        Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Qty],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' And SeqNoGroup IN (" + temp + ") order by SeqNo asc";
        //    }

        //    Cmd = new SqlCommand(Query, Conn);
        //    Dr = Cmd.ExecuteReader();
        //    int i = 0;
        //    //dgvPrDetails.ReadOnly = true;
        //    while (Dr.Read())
        //    {
        //        this.dgvPrDetails.Rows.Add(Dr["No"], Dr["FullItemID"], Dr["ItemDesc"], Dr["ReffTransID"], Convert.ToDateTime(Dr["ExpectedDateFrom"]).ToString("dd-MM-yyyy"), Convert.ToDateTime(Dr["ExpectedDateTo"]).ToString("dd-MM-yyyy"), Dr["DeliveryMethod"], Dr["Qty"].ToString().Replace(',', '.'), Dr["Unit"], Dr["VendId"], Dr["Base"], Dr["Deskripsi"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"]);
                

        //        DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
        //        combo2.Items.Add("Pending");
        //        combo2.Items.Add("Yes");
        //        combo2.Items.Add("No");
        //        combo2.Items.Add("Revision");

        //        if (Dr["Base"].ToString() == "N")
        //        {
        //            dgvPrDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
        //        }

                

                

        //        if (Login.UserGroup == "SalesManager")
        //        {//kalo dikasi status yes ama purchase manager
        //            if (txtTransStatus.Text == "13" || txtTransStatus.Text == "14" || txtTransStatus.Text == "15")
        //            {
        //                dgvPrDetails.ReadOnly = true;
        //            }
        //            else if (txtTransStatus.Text == "12")
        //            {
                        
        //                    if (dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString() == "Revision")
        //                    {
        //                        dgvPrDetails.Rows[i].Cells["Sales Manager Status"].ReadOnly = false;
        //                    }
        //                    else
        //                    {
        //                        dgvPrDetails.Rows[i].ReadOnly = true;
        //                    }

                        
        //            }



        //            //if (Dr["TransStatus"].ToString() == "13" || Dr["TransStatus"].ToString() == "14" || Dr["TransStatus"].ToString() == "15")
        //            //{
        //            //    dgvPrDetails.ReadOnly = true;
        //            //}

        //            //if (Dr2[13].ToString() == null)//transstatuspurch
        //            //{
                        
        //            //}
        //            //else if (Dr2[13].ToString() == "Yes" || Dr2[13].ToString() == "No" || Dr2[13].ToString() == "Pending")
        //            //{
        //            //    dgvPrDetails.Rows[i].ReadOnly = true;
        //            //}
        //            //else if (Dr2[13].ToString() == "Revision")
        //            //{
                        
        //            //}
        //        }

        //        if (Login.UserGroup == "SalesManager")
        //        {
        //            //matiin yang N
        //            if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
        //            {
        //                dgvPrDetails.Rows[i].ReadOnly = true;
        //            }
        //            else
        //            {
        //                dgvPrDetails.Rows[i].Cells["Sales Manager Status"] = combo2;
        //                dgvPrDetails.Rows[i].ReadOnly = false;
        //                //dgvPrDetails.Rows[i].Cells["Sales Manager Status"].ReadOnly = false;
        //            }

        //            if (Dr["TransStatus"] != null)
        //            {
        //                dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value = Dr["TransStatus"].ToString();
        //            }

        //            dgvPrDetails.Columns["Purchase Manager Status"].ReadOnly = true;
        //            dgvPrDetails.Columns["Purchase Approval Description"].ReadOnly = true;


        //        }
        //        else if (Login.UserGroup == "PurchaseManager")
        //        {

        //            //matiin yang N
        //            if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
        //            {
        //                dgvPrDetails.Rows[i].ReadOnly = true;
        //                dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].ReadOnly = true;
        //            }
        //            else
        //            {
        //                dgvPrDetails.Rows[i].Cells["Purchase Manager Status"] = combo2;
        //            }

        //            if (Dr["TransStatusPurch"] != null)
        //            {
        //                dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value = Dr["TransStatusPurch"].ToString();

        //            }

        //            dgvPrDetails.Columns["Sales Manager Status"].ReadOnly = true;
        //            dgvPrDetails.Columns["Sales Approval Description"].ReadOnly = true;


        //        }
                
        //        //untuk isi combobox via database
                
        //        //Conn = ConnectionString.GetConnection();
        //        //Query = "Select [Deskripsi] From dbo.[TransStatusTable]";
        //        //Cmd = new SqlCommand(Query, Conn);
        //        //SqlDataReader DrCmb2;
        //        //DrCmb2 = Cmd.ExecuteReader();
        //        //DataGridViewComboBoxCell combo2 = new DataGridViewComboBoxCell();
        //        //while (DrCmb2.Read())
        //        //{
        //        //    combo2.Items.Add(DrCmb2[0].ToString());
        //        //}
        //        //DrCmb2.Close();
        //        //if (Dr[10] != null)
        //        //{
        //        //    combo2.Value = Dr[10].ToString();
        //        //}
        //        //dgvPrDetails.Rows[i].Cells[14] = combo2;
        //        dgvPrDetails.AutoResizeColumns();
                
        //        i++;
                
        //    }
        //    Dr.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
        //    this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
        //    Conn = ConnectionString.GetConnection();
        //    Conn2 = ConnectionString.GetConnection();
        //    Trans = Conn.BeginTransaction();
        //    String StatusHeader = "";
        //    int counterWait = 0;
        //    int counterRev = 0;
        //    int counterApp = 0;
        //    int counterNo = 0;
        //    int counterPend = 0;

        //    if (Login.UserGroup == "SalesManager")
        //    {
        //        //header
        //        for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
        //        {
        //            if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "Y")
        //            {
        //                String detailStatus = dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString();

        //                if (string.IsNullOrEmpty(detailStatus))
        //                {
        //                    counterWait++;
        //                }
        //                else if (detailStatus == "Pending")
        //                {
        //                    counterPend++;
        //                }
        //                else if (detailStatus == "Revision")
        //                {
        //                    counterRev++;
        //                }
        //                else if (detailStatus == "Yes")
        //                {
        //                    counterApp++;
        //                }
        //                else if (detailStatus == "No")
        //                {
        //                    counterNo++;
        //                }
                    
        //            }
        //        }

        //        if (counterWait > 0)
        //        {
        //            MessageBox.Show("Status harus diisi.");
        //            return;
        //        }

        //        if (counterRev > 0 || counterPend > 0 )
        //        {
        //            StatusHeader = "02";
        //        }
        //        else
        //        {
        //            if (counterApp == 0)
        //            {
        //                StatusHeader = "05";
        //            }
        //            else if (counterNo == 0)
        //            {
        //                StatusHeader = "03";
        //            }
        //            else
        //            {
        //                StatusHeader = "04";
        //            }
        //        }

        //        String Query2 = "Select [Deskripsi] From [dbo].[TransStatusTable] Where StatusCode = '" + StatusHeader + "'";
        //        String NewStatusDesc = "";
        //        Cmd = new SqlCommand(Query2, Conn, Trans);
        //        NewStatusDesc = Cmd.ExecuteScalar().ToString();


        //        Query = "Update PurchRequisitionH Set TransStatus ='" + StatusHeader + "' Where PurchReqId = '" + PurchReqID + "'";

        //        if (txtPrStatus.Text != NewStatusDesc)
        //        {
        //            Query += "INSERT INTO [dbo].[WorkflowLogTable]([ReffTableName],[ReffID],[ReffDate],[ReffSeqNo],[UserID],[WorkFlow],[LogStatus],[StatusDesc],[LogDate]) ";
        //            Query += "Values ('PurchRequisitionH', '" + PurchReqID + "','" + dtPrDate.Value + "','','" + Login.Username + "','" + Login.UserGroup + "','" + StatusHeader + "','" + NewStatusDesc + "',getDate())";
        //        }

        //        Cmd = new SqlCommand(Query, Conn, Trans);
        //        Cmd.ExecuteNonQuery();

        //        //detail
        //        for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
        //        {
        //            //String ApprovePerson = dgvPrDetails.Rows[i].Cells["Approval ID"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Approval ID"].Value.ToString();
        //            String ApprovalNotes = dgvPrDetails.Rows[i].Cells["Sales Approval Description"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Approval Description"].Value.ToString();

        //            Query = "Update PurchRequisition_Dtl Set TransStatus = '" + ((dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Sales Manager Status"].Value.ToString())) + "', ";
        //            Query += "[ApprovalNotes]='" + ApprovalNotes + "', ";
        //            Query += "[ApprovePerson]='" + Login.Username + "' Where PurchReqID = '" + PurchReqID + "' And SeqNo = '" + (i + 1) + "'";
        //            Cmd = new SqlCommand(Query, Conn, Trans);
        //            Cmd.ExecuteNonQuery();
        //        }
        //    }
        //    else if (Login.UserGroup == "PurchaseManager")
        //    {
        //        //header
        //        for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
        //        {
        //            if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "Y")
        //            {
        //                String detailStatus = dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString();
        //                if (string.IsNullOrEmpty(detailStatus))
        //                {
        //                    counterWait++;
        //                }
        //                else if (detailStatus == "Pending")
        //                {
        //                    counterPend++;
        //                }
        //                else if (detailStatus == "Revision")
        //                {
        //                    counterRev++;
        //                }
        //                else if (detailStatus == "Yes")
        //                {
        //                    counterApp++;
        //                }
        //                else if (detailStatus == "No")
        //                {
        //                    counterNo++;
        //                }
        //            }
        //        }

        //        if (counterWait > 0)
        //        {
        //            MessageBox.Show("Status harus diisi.");
        //            return;
        //        }

        //        if (counterRev > 0 || counterPend > 0)
        //        {
        //            StatusHeader = "12";
        //        }
        //        else
        //        {
        //            if (counterApp == 0)
        //            {
        //                StatusHeader = "15";
        //            }
        //            else if (counterNo == 0)
        //            {
        //                StatusHeader = "13";
        //            }
        //            else
        //            {
        //                StatusHeader = "14";
        //            }
        //        }

        //        String Query2 = "Select [Deskripsi] From [dbo].[TransStatusTable] Where StatusCode = '" + StatusHeader + "'";
        //        String NewStatusDesc = "";
        //        Cmd = new SqlCommand(Query2, Conn, Trans);
        //        NewStatusDesc = Cmd.ExecuteScalar().ToString();

        //        Query = "Update PurchRequisitionH Set TransStatus ='" + StatusHeader + "' Where PurchReqId = '" + PurchReqID + "'";

        //        if (txtPrStatus.Text != NewStatusDesc)
        //        {
        //            Query += "INSERT INTO [dbo].[WorkflowLogTable]([ReffTableName],[ReffID],[ReffDate],[ReffSeqNo],[UserID],[WorkFlow],[LogStatus],[StatusDesc],[LogDate]) ";
        //            Query += "Values ('PurchRequisitionH', '" + PurchReqID + "','" + dtPrDate.Value + "','','" + Login.Username + "','" + Login.UserGroup + "','" + StatusHeader + "','" + NewStatusDesc + "',getDate())";
        //        }
                
        //        Cmd = new SqlCommand(Query, Conn, Trans);
        //        Cmd.ExecuteNonQuery();

        //        //detail
        //        for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
        //        {
        //            //String ApprovePerson = dgvPrDetails.Rows[i].Cells["Last Approval ID"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Last Approval ID"].Value.ToString();
        //            String ApprovalNotes = dgvPrDetails.Rows[i].Cells["Purchase Approval Description"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Approval Description"].Value.ToString();

        //            Query = "Update PurchRequisition_Dtl Set TransStatusPurch = '" + ((dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Purchase Manager Status"].Value.ToString())) + "', ";
        //            Query += "[ApprovalNotesPurch]='" + ApprovalNotes + "', ";
        //            Query += "[ApprovePerson]='" + Login.Username + "' Where PurchReqID = '" + PurchReqID + "' And SeqNo = '" + (i + 1) + "'";
        //            Cmd = new SqlCommand(Query, Conn, Trans);
        //            Cmd.ExecuteNonQuery();
        //        }
        //    }
        //    MessageBox.Show("Data sudah diupdate.");
        //    Trans.Commit();
        //    Conn.Close();
        //    this.Close();
        }

        private void DashboardHeaderPRApproval_FormClosed(object sender, FormClosedEventArgs e)
        {
        //    //Parent.RefreshGrid();
        }

        private void tabDgvControl_SelectedIndexChanged(object sender, EventArgs e)
        {
        //    if (dgvPrDetails.Columns.Count != 0)
        //    {
        //        if (tabDgvControl.SelectedTab.Text == "Approval")
        //        {
        //            dgvPrDetails.Columns["Qty"].Visible = true;
        //            dgvPrDetails.Columns["Unit"].Visible = true;
        //            dgvPrDetails.Columns["VendId"].Visible = true;
        //            dgvPrDetails.Columns["Sales Manager Status"].Visible = true;
        //            dgvPrDetails.Columns["Sales Last Approval ID"].Visible = true;
        //            dgvPrDetails.Columns["Sales Approval Description"].Visible = true;
        //            dgvPrDetails.Columns["Purchase Manager Status"].Visible = true;
        //            dgvPrDetails.Columns["Purchase Last Approval ID"].Visible = true;
        //            dgvPrDetails.Columns["Purchase Approval Description"].Visible = true;
        //            dgvPrDetails.Columns["ReffTransID"].Visible = false;
        //            dgvPrDetails.Columns["ExpectedDateFrom"].Visible = false;
        //            dgvPrDetails.Columns["ExpectedDateTo"].Visible = false;
        //            dgvPrDetails.Columns["Delivery Method"].Visible = false;
        //            dgvPrDetails.Columns["Base"].Visible = false;
        //            dgvPrDetails.Columns["Deskripsi"].Visible = false;
        //        }
        //        else
        //        {
        //            dgvPrDetails.Columns["Qty"].Visible = false;
        //            dgvPrDetails.Columns["Unit"].Visible = false;
        //            dgvPrDetails.Columns["VendId"].Visible = false;
        //            dgvPrDetails.Columns["Sales Manager Status"].Visible = false;
        //            dgvPrDetails.Columns["Sales Last Approval ID"].Visible = false;
        //            dgvPrDetails.Columns["Sales Approval Description"].Visible = false;
        //            dgvPrDetails.Columns["Purchase Manager Status"].Visible = false;
        //            dgvPrDetails.Columns["Purchase Last Approval ID"].Visible = false;
        //            dgvPrDetails.Columns["Purchase Approval Description"].Visible = false;
        //            dgvPrDetails.Columns["ReffTransID"].Visible = true;
        //            dgvPrDetails.Columns["ExpectedDateFrom"].Visible = true;
        //            dgvPrDetails.Columns["ExpectedDateTo"].Visible = true;
        //            dgvPrDetails.Columns["Delivery Method"].Visible = true;
        //            dgvPrDetails.Columns["Base"].Visible = true;
        //            dgvPrDetails.Columns["Deskripsi"].Visible = true;
        //        }
        //    }
        }

        //private bool CheckForm(Form form)
        //{
        //    //form = Application.OpenForms[form.Text];
        //    //if (form != null)
        //    //    return true;
        //    //else
        //    //    return false;
        //}

        private void dgvPrDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
        //    if (e.Button == MouseButtons.Right && e.RowIndex > -1)
        //    {
        //        if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "VendId")
        //        {
        //            String TotalVendor = dgvPrDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        //            String[] VendorSatuan = TotalVendor.Split(';');

        //            PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();

        //            if (!CheckForm(PopUpVendor))//jika tidak ada
        //            {
        //                for (int i = 0; i < VendorSatuan.Count(); i++)
        //                {
        //                    PopUp.Vendor.Vendor PopUpVendor1 = new PopUp.Vendor.Vendor();

        //                    PopUpVendor1.GetData(VendorSatuan[i].ToString());
        //                    PopUpVendor1.Y += 100 * i;
        //                    PopUpVendor1.Show();
        //                }
        //            }

                    

                    
        //        }
        //        if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName")
        //        {
        //            PopUp.Stock.Stock PopUpStock = new PopUp.Stock.Stock();
        //            if (!CheckForm(PopUpStock))
        //            {
        //                PopUpStock.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
        //                PopUpStock.Show();
                        
        //            }
                    
        //        }
        //    }
        }


    }
}
