using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.CanvasSheet
{
    public partial class ApproveCanvasSheet : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string[] SubHeader = { "Price", "Order\nQty", "Check" };

        string Mode, Query, crit, CSNumber = null;

        int Index;
        private int[] daysInMonths;

        public string PQNumber = "", tmpPrType = "";

        DateTimePicker dtp;

        List<string> ListSeqNum;
        List<Purchase.PurchaseRequisition.Info> ListInfo = new List<Purchase.PurchaseRequisition.Info>();

        List<string> VendorId = new List<string>(); //list jumlah vendor
        List<string> Columns = new List<string>(); //list jumlah column di datagrid price, pr qty, qty, check
        List<string> PurchQuotId = new List<string>();
        
        Purchase.PurchaseQuotation.InquiryPQ Parent;
        Boolean CanvasSheetType = false;

        public ApproveCanvasSheet()
        {
            InitializeComponent();
        }

        private void ApproveCanvasSheet_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
        }

        public void ModeBeforeEdit(string TmpCSNumber)
        {
            txtCSNumber.Text = TmpCSNumber;

            txtPrNumber.Text = "";

            btnSave.Visible = true;
            btnExit.Visible = true;

            dgvPqDetails.AutoResizeColumns();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("CanvasId = " + txtCSNumber.Text.Trim() + "\n" + "Apakah CanvasSheet akan di approve ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                Query = "Update [CanvasSheetH] ";
                Query += " set StatusApproval='YES'";
                Query += " where CanvasId='" + txtCSNumber.Text.Trim() + "';";

                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteScalar();
                Trans.Commit();
                MessageBox.Show("Data CanvasSheetNumber : " + CSNumber + " berhasil diapproved.");
                txtApproved.Text = "YES";
                }
            }

            catch (Exception)
            {
                Trans.Rollback();
            }
            finally
            {
                GetDataHeader();
            }
        }

        public void GetDataHeader()
        {
            if (txtCSNumber.Text.Trim() != "")
            {
                Conn = ConnectionString.GetConnection();
                Query = "SELECT a.[CanvasId],a.[CanvasDate],a.[PurchReqId],a.[TransStatus], a.StatusApproval,b.[Deskripsi] FROM [dbo].[CanvasSheetH] a left join TransStatusTable b on a.TransStatus=b.StatusCode where [CanvasId]='" + txtCSNumber.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtCanvasDate.Text = Dr["CanvasDate"].ToString();
                    txtCSNumber.Text = Dr["CanvasId"].ToString();
                    txtPrNumber.Text = Dr["PurchReqId"].ToString();
                    txtTransStatusCode.Text = Dr["TransStatus"].ToString();
                    txtApproved.Text = Dr["StatusApproval"].ToString();
                    txtTransStatusDesc.Text = Dr["Deskripsi"].ToString();
                }
                Dr.Close();

                if (dgvPqDetails.Rows.Count >= 1)
                {
                    dgvPqDetails.Rows.Clear();
                    dgvPqDetails.Columns.Clear();
                    VendorId.Clear();
                    PurchQuotId.Clear();
                    Columns.Clear();
                }

                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                chk.HeaderText = "By Row";
                chk.Name = "By Row";
                dgvPqDetails.Columns.Add(chk);

                dgvPqDetails.ColumnCount = 6;
                dgvPqDetails.Columns[1].Name = "No";
                dgvPqDetails.Columns[2].Name = "Delivery\nMethod";
                dgvPqDetails.Columns[3].Name = "FullItemID";
                dgvPqDetails.Columns[4].Name = "ItemName";
                dgvPqDetails.Columns[5].Name = "SeqNo";

                Conn = ConnectionString.GetConnection();

                //List Item PR
                //ROW_NUMBER() OVER (ORDER BY PurchReqSeqNo) No
                Query = "Select a.CheckLine, DeliveryMethod, a.FullItemID, a.ItemName, a.PurchReqSeqNo , a.CanvasSeqNo From (Select distinct CheckLine,DeliveryMethod,FullItemID, ItemName, PurchReqSeqNo, CanvasSeqNo From CanvasSheetD a where CanvasId='" + txtCSNumber.Text.Trim() + "') a  order by CanvasSeqNo asc";// and a.TransStatus=''";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int x = 1;
                while (Dr.Read())
                {
                    this.dgvPqDetails.Rows.Add(Dr[0].ToString(), x++, Dr[1], Dr[2], Dr[3], Dr[4]);
                    //this.dgvPqDetails.Rows.Add(Dr[0].ToString(), x++, Dr[1], Dr[2], Dr[3], Dr[4]);
                }
                Dr.Close();

                //this.dgvPqDetails.Rows.Add("*", "By Column", "");

                //List Quotation Vendor Id
                Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where ReffTransID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                dgvPqDetails.Columns["SeqNo"].Visible = false;

                //int i = dgvPqDetails.ColumnCount;
                
                while (Dr.Read())
                {
                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice").ToString());
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice"].ReadOnly = true;

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty").ToString());
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty"].ReadOnly = true;

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty").ToString());
                    //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty"].ReadOnly = false;
                
                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty").ToString());
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty"].ReadOnly = false;

                    VendorId.Add(Dr["VendId"].ToString());

                    PurchQuotId.Add(Dr["PurchQuotId"].ToString());

                    chk = new DataGridViewCheckBoxColumn();
                    chk.HeaderText = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck";
                    chk.Name = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck";
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck").ToString());
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck"].ReadOnly = false;

                    dgvPqDetails.Columns.Add(chk);
                    //dgvPqDetails.Columns.Add();
                }

               if (VendorId.Count > 0)
               {
                   int col = 6;
                   for (int j = 0; j < VendorId.Count; j++)
                   {
                       for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                       {
                           //Obtain a reference to the newly created DataGridViewRow 
                           //if (dgvPqDetails.Rows[i].Cells[1].Value != "By Column")
                           //{
                           var row = this.dgvPqDetails.Rows[i];

                           Query = "Select CheckLine,Price, PRQty, PQQty, Qty, CheckVendorD From CanvasSheetD where FullItemId='" + dgvPqDetails.Rows[i].Cells[3].Value + "' and CanvasId='" + txtCSNumber.Text.Trim() + "' and PurchQuotId='" + PurchQuotId[j] + "' and DeliveryMethod='" + dgvPqDetails.Rows[i].Cells["Delivery\nMethod"].Value.ToString() + "';";
                           Cmd = new SqlCommand(Query, Conn);
                           //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                           Dr = Cmd.ExecuteReader();
                           while (Dr.Read())
                           {
                               dgvPqDetails.Rows[i].Cells[0].Value = Dr[0].ToString();
                               dgvPqDetails.Rows[i].Cells[col].Value = Dr[1].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 1].Value = Dr[2].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 2].Value = Dr[3].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 3].Value = Dr[4].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 4].Value = Dr[5].ToString();
                           }
                           //}
                       }
                       col += 5;
                   }
               }

               dgvPqDetails.AutoResizeColumns();

                //GetGelombang
               Query = "Select Count(PurchReqID) from PurchRequisition_DtlDtl where PurchReqID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
               Cmd = new SqlCommand(Query, Conn);
               int CheckCount = Convert.ToInt32(Cmd.ExecuteScalar());

               if (CheckCount > 0)
               {

                   if (dgvPqDetails.Rows.Count >= 1)
                   {
                       dgvCsGel.Rows.Clear();
                       dgvCsGel.Columns.Clear();
                       VendorId.Clear();
                       PurchQuotId.Clear();
                       Columns.Clear();
                   }

                   CanvasSheetType = true;
                   //CheckCollapse();

                   chk = new DataGridViewCheckBoxColumn();
                   chk.HeaderText = "By Row";
                   chk.Name = "By Row";
                   dgvCsGel.Columns.Add(chk);

                   dgvCsGel.ColumnCount = 6;
                   dgvCsGel.Columns[1].Name = "No";
                   dgvCsGel.Columns[2].Name = "FullItemID";
                   dgvCsGel.Columns[3].Name = "ItemName";
                   dgvCsGel.Columns[4].Name = "SeqNo";
                   dgvCsGel.Columns[5].Name = "Base";

                   Conn = ConnectionString.GetConnection();

                   //List Item PR
                   Query = "Select ROW_NUMBER() OVER (ORDER BY SeqNoDtl) No, FullItemID, ItemName, SeqNoDtl, Base From PurchRequisition_DtlDtl a where PurchReqID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
                   Cmd = new SqlCommand(Query, Conn);
                   Dr = Cmd.ExecuteReader();
                   while (Dr.Read())
                   {
                       this.dgvCsGel.Rows.Add("true", Dr[0], Dr[1], Dr[2], Dr[3], Dr[4]);
                   }
                   Dr.Close();

                   //this.dgvCsGel.Rows.Add("*", "By Column", "");

                   //List Quotation Vendor Id
                   Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where ReffTransID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
                   Cmd = new SqlCommand(Query, Conn);
                   Dr = Cmd.ExecuteReader();
                   dgvCsGel.Columns["SeqNo"].Visible = false;

                   //int i = dgvCsGel.ColumnCount;

                   while (Dr.Read())
                   {
                       dgvCsGel.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice");
                       Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice").ToString());
                       //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice"].ReadOnly = true;

                       dgvCsGel.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty");
                       Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty").ToString());
                       //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty"].ReadOnly = true;

                       dgvCsGel.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty");
                       Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty").ToString());
                       //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty"].ReadOnly = false;

                       VendorId.Add(Dr["VendId"].ToString());
                       PurchQuotId.Add(Dr["PurchQuotId"].ToString());

                       chk = new DataGridViewCheckBoxColumn();
                       chk.HeaderText = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck";
                       chk.Name = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck";
                       Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck").ToString());
                       //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck"].ReadOnly = false;

                       dgvCsGel.Columns.Add(chk);
                       //dgvCsGel.Columns.Add();
                   }

                   if (VendorId.Count > 0)
                   {
                       int col = 6;
                       for (int j = 0; j < VendorId.Count; j++)
                       {
                           for (int i = 0; i < dgvCsGel.Rows.Count; i++)
                           {
                               
                               var row = this.dgvCsGel.Rows[i];

                               Query = "Select CheckLine, Price, Qty, CheckVendorD FROM [dbo].[CanvasSheetD_Dtl] where canvasid='" + txtCSNumber.Text.Trim() + "' and FullItemId='" + dgvCsGel.Rows[i].Cells["FullItemId"].Value.ToString() + "' and PurchQuotID='" + PurchQuotId[j].ToString() + "';";
                               Cmd = new SqlCommand(Query, Conn);
                               //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                               Dr = Cmd.ExecuteReader();
                               while (Dr.Read())
                               {
                                   dgvCsGel.Rows[i].Cells[0].Value = Dr[0].ToString();
                                   dgvCsGel.Rows[i].Cells[col].Value = Dr[1].ToString();
                                   dgvCsGel.Rows[i].Cells[col + 1].Value = Dr[2].ToString();
                                   dgvCsGel.Rows[i].Cells[col + 3].Value = Dr[3].ToString();
                                   //dgvCsGel.Rows[i].Cells[col + 3].Value = Dr[3].ToString();
                               }

                               //}
                           }
                           col += 4;
                       }
                   }

                   dgvCsGel.ReadOnly = false;
                   dgvCsGel.Columns["No"].ReadOnly = true;
                   dgvCsGel.Columns["FullItemID"].ReadOnly = true;
                   dgvCsGel.Columns["ItemName"].ReadOnly = true;
                   dgvCsGel.Columns["SeqNo"].ReadOnly = true;
                   dgvCsGel.Columns["Base"].ReadOnly = true;
                   for (int i = 0; i < Columns.Count; i++)
                   {
                       if (Columns[i].Contains("Price") || Columns[i].Contains("PR"))
                       {
                           dgvCsGel.Columns[Columns[i]].ReadOnly = true;
                       }
                   }

                   dgvCsGel.AutoResizeColumns();
               }
               Conn.Close();

               foreach (DataGridViewColumn column in dgvPqDetails.Columns)
               {
                   column.SortMode = DataGridViewColumnSortMode.NotSortable;
               }

               foreach (DataGridViewColumn column in dgvCsGel.Columns)
               {
                   column.SortMode = DataGridViewColumnSortMode.NotSortable;
               }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
