using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.PurchaseRequisition
{
    public partial class PRNo : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;
        private string PRId;
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.FullItemId.FullItemId FID = null;


        public PRNo()
        {
            InitializeComponent();
        }

        private void PRNo_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            dtPRDate.Enabled = false;
           
        }

        public void GetData(string PRId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select h.PurchReqId, h.OrderDate, h.TransType, t.[Deskripsi], h.ApprovedBy, h.TransStatus From [PurchRequisitionH] h JOIN TransStatusTable t ON h.TransStatus = t.StatusCode And TransCode = 'PR' Where PurchReqID = '" + PRId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtPRDate.Text=" " + Dr["OrderDate"].ToString();
                lblPRNumber.Text=" " + Dr["PurchReqId"].ToString();
                lblPRType.Text=Dr["TransType"].ToString();
                lblPRStatus.Text=" " + Dr["TransStatus"].ToString();
                lblPRApproved.Text=" " + Dr["ApprovedBy"].ToString();
            }
            Dr.Close();

            dgvApproval.DataSource = null;
            if (dgvApproval.RowCount==0)
            {
                dgvApproval.Rows.Clear();
                dgvApproval.ColumnCount = 12;
                dgvApproval.Columns[0].Name = "No";
                dgvApproval.Columns[1].Name = "FullItemID";
                dgvApproval.Columns[2].Name = "ItemName";
              
                if (lblPRType.Text != "AMOUNT")
                {
                   dgvApproval.Columns[3].Name = "Qty";
                   dgvApproval.Columns[4].Name = "Unit";
                }
                else if (lblPRType.Text == "AMOUNT")
                {
                    dgvApproval.Columns[3].Name = "Amount";
                     dgvApproval.Columns[4].Name = "Unit";
                }
               
                dgvApproval.Columns[5].Name = "VendId";
                dgvApproval.Columns[6].Name = "Sales Manager Status";
                dgvApproval.Columns[7].Name = "Sales Last Approval ID";
                dgvApproval.Columns[8].Name = "Sales Approval Description";
                dgvApproval.Columns[9].Name = "Purchase Manager Status";
                dgvApproval.Columns[10].Name = "Purchase Last Approval ID";
                dgvApproval.Columns[11].Name = "Purchase Approval Description";
               
            }

            dgvDetail.DataSource = null;
            if (dgvDetail.RowCount==0)
            {
                dgvDetail.Rows.Clear();
                dgvDetail.ColumnCount = 9;
                dgvDetail.Columns[0].Name="No";
                dgvDetail.Columns[1].Name = "FullItemID";
                dgvDetail.Columns[2].Name = "ItemName";
                dgvDetail.Columns[3].Name = "Sales SO";
                dgvDetail.Columns[4].Name = "ExpectedDateFrom";
                dgvDetail.Columns[5].Name = "ExpectedDateTo";
                dgvDetail.Columns[6].Name = "Delivery Method";
                dgvDetail.Columns[7].Name = "Base";
                dgvDetail.Columns[8].Name = "Deskripsi";
            }

            if (lblPRType.Text != "AMOUNT")
            {
                Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Qty],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PRId + "' ";

            }
            else if (lblPRType.Text == "AMOUNT")
            {
                Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[DeliveryMethod],[Amount],[Unit],[VendID],[Deskripsi],[Base],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PRId + "' ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int j = 1;
            int i = 1;
            while (Dr.Read())
            {
                if (lblPRType.Text != "AMOUNT")
                {
                    this.dgvApproval.Rows.Add(j, Dr["FullItemID"], Dr["ItemDesc"], Dr["Qty"], Dr["Unit"], Dr["VendID"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"]);
                    j++;
                    this.dgvDetail.Rows.Add(i, Dr["FullItemID"], Dr["ItemDesc"], Dr["ReffTransID"], Dr["ExpectedDateFrom"], Dr["ExpectedDateTo"], Dr["DeliveryMethod"], Dr["Base"], Dr["Deskripsi"]);
                    i++;
                }
                else if (lblPRType.Text == "AMOUNT")
                {
                    this.dgvApproval.Rows.Add(j, Dr["FullItemID"], Dr["ItemDesc"], Dr["Amount"], Dr["Unit"], Dr["VendID"], Dr["TransStatus"], Dr["ApprovePerson"], Dr["ApprovalNotes"], Dr["TransStatusPurch"], Dr["ApprovePersonPurch"], Dr["ApprovalNotesPurch"]);
                    j++;
                    this.dgvDetail.Rows.Add(i, Dr["FullItemID"], Dr["ItemDesc"], Dr["ReffTransID"], Dr["ExpectedDateFrom"], Dr["ExpectedDateTo"], Dr["DeliveryMethod"], Dr["Base"], Dr["Deskripsi"]);
                    i++;
                }
            }
            Dr.Close();
            dgvApproval.AutoResizeColumns();
            dgvDetail.AutoResizeColumns();
            dgvApproval.ReadOnly = true;
            dgvDetail.ReadOnly = true;
        }
        
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvApproval_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    if (dgvApproval.Columns[e.ColumnIndex].Name.ToString() == "VendId")
                    {
                        Vendor = new PopUp.Vendor.Vendor();
                        Vendor.GetData(dgvApproval.Rows[e.RowIndex].Cells["VendId"].Value.ToString());
                        Vendor.Show();
                    }
                   

                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.GetData(dgvApproval.Rows[e.RowIndex].Cells["VendId"].Value.ToString());
                    Vendor.Show();
                    Vendor.Focus();
                }
                if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    if (FID == null || FID.Text == "")
                    {
                        if (dgvApproval.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvApproval.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                        {
                            FID = new PopUp.FullItemId.FullItemId();
                            FID.GetData(dgvApproval.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                           // itemID = dgvApproval.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                            FID.Show();
                        }
                    }
                    else if (CheckOpened(FID.Name))
                    {
                        FID.WindowState = FormWindowState.Normal;
                        FID.GetData(dgvApproval.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        FID.Show();
                        FID.Focus();
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

        private void dgvDetail_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvDetail.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvDetail.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }
    }
}
