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

namespace ISBS_New.Purchase.CanvasSheet
{
    public partial class FormCanvasSheet : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
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

        public FormCanvasSheet()
        {
            InitializeComponent();
        }

        private void FormCanvasSheet_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
        }

        public void ModeNew()
        {
            //CheckCollapse();
            txtCSNumber.Text = "";
            txtPrNumber.Text = "";
            dtCanvasDate.Value = Convert.ToDateTime("01-01-1990");

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            btnSearchPR.Enabled = true;

        }

        public void ModeEdit()
        {
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnSearchPR.Enabled = true;
            dgvPqDetails.Enabled = true;

            dgvPqDetails.ReadOnly = false;
            dgvPqDetails.Columns["No"].ReadOnly = true;
            dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPqDetails.Columns["ItemName"].ReadOnly = true;
            dgvPqDetails.Columns["SeqNo"].ReadOnly = true;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Vendor"))
                {
                    dgvPqDetails.Columns[Columns[i]].ReadOnly = true;
                }
            }
            dgvPqDetails.AutoResizeColumns();

        }

        public void ModeBeforeEdit(string TmpCSNumber)
        {
            txtCSNumber.Text = TmpCSNumber;

            txtPrNumber.Text = "";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            btnSearchPR.Enabled = false;
            dgvPqDetails.Enabled = false;

            dgvPqDetails.ReadOnly = true;
            dgvPqDetails.AutoResizeColumns();
        }

        public void ModeBeforeEdit()
        {
            //txtPrNumber.Text = "";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            btnSearchPR.Enabled = false;
            dgvPqDetails.Enabled = false;

            dgvPqDetails.ReadOnly = true;
            dgvPqDetails.AutoResizeColumns();
        }

        //private void CheckCollapse()
        //{
        //    if (CanvasSheetType == false)
        //    {
        //        grpGelombang.Visible = false;

        //        grpDetail.Height = 213;
        //        dgvPqDetails.Height = 183;

        //        btnSave.Top -= 149;
        //        btnEdit.Top -= 149;
        //        btnCancel.Top -= 149;
        //        btnExit.Top -= 149;

        //        groupPRH.Height -= 149;
        //        this.Height -= 149;
        //    }
        //    else
        //    {
        //        grpGelombang.Visible = true;

        //        grpDetail.Height = 170;
        //        dgvPqDetails.Height = 149;

        //        grpGelombang.Height = 200;
        //        dgvCsGel.Height = 183;

        //        grpGelombang.Top -= 149;
        //        btnSave.Top += 149;
        //        btnEdit.Top += 149;
        //        btnCancel.Top += 149;
        //        btnExit.Top += 149;

        //        groupPRH.Height += 149;
        //        this.Height += 149;
        //    }
        //}

        private void btnSearchPR_Click(object sender, EventArgs e)
        {
            //Details
            string SchemaName = "dbo";
            string TableName = "PurchRequisitionH";
            //string Where = " AND TransStatus='13'";
            VendorId.Clear();
            Columns.Clear();
            PurchQuotId.Clear();

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);//, Where);
            tmpSearch.ShowDialog();
            txtPrNumber.Text = ConnectionString.Kode;

            if (dgvPqDetails.Rows.Count >= 1)
            {
                dgvPqDetails.Rows.Clear();
                dgvPqDetails.Columns.Clear();
                VendorId.Clear();
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
            //ROW_NUMBER() OVER (ORDER BY SeqNo) No,
            Query = "Select FullItemID, ItemName, SeqNo From PurchRequisition_Dtl a where PurchReqID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int x = 1;
            while (Dr.Read())
            {
                this.dgvPqDetails.Rows.Add("true", x++, "LOCO", Dr[0], Dr[1], Dr[2]);
                //x++;
                this.dgvPqDetails.Rows.Add("true", x++, "FRANCO", Dr[0], Dr[1], Dr[2]);
                //x++;
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

                dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty");
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

                            Query = "Select Price, Qty, Qty2 From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.DeliveryMethod='" + dgvPqDetails.Rows[i].Cells[2].Value + "' and a.FullItemId='" + dgvPqDetails.Rows[i].Cells[3].Value + "' and ReffTransID='" + txtPrNumber.Text + "' and b.VendId='" + VendorId[j].ToString() + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                dgvPqDetails.Rows[i].Cells[col].Value = (Dr[0].ToString() == "" ? "0" : Dr[0].ToString());
                                dgvPqDetails.Rows[i].Cells[col + 1].Value = (Dr[1].ToString() == "" ? "0" : Dr[1].ToString());
                                dgvPqDetails.Rows[i].Cells[col + 2].Value = (Dr[2].ToString() == "" ? "0" : Dr[2].ToString());
                                dgvPqDetails.Rows[i].Cells[col + 3].Value = "0"; 
                                dgvPqDetails.Rows[i].Cells[col + 4].Value = false;
                            }
                        //}
                    }
                    col += 5;
                }
            }

           dgvPqDetails.ReadOnly =false;
           dgvPqDetails.Columns["No"].ReadOnly = true;
           dgvPqDetails.Columns["Delivery\nMethod"].ReadOnly = true;
           dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
           dgvPqDetails.Columns["ItemName"].ReadOnly = true;
           dgvPqDetails.Columns["SeqNo"].ReadOnly = true;
           for (int i = 0; i < Columns.Count; i++)
           {
               if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Vendor"))
               {
                   dgvPqDetails.Columns[Columns[i]].ReadOnly = true;
               }  
           }

           dgvPqDetails.AutoResizeColumns();

           //Gelombang 794,44
           Query = "Select Count(PurchReqID) from PurchRequisition_DtlDtl where PurchReqID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
           Cmd = new SqlCommand(Query, Conn);
           int CheckCount = Convert.ToInt32(Cmd.ExecuteScalar());
            
           if (CheckCount > 0)
           {
               dgvCsGel.Rows.Clear();
               dgvCsGel.Columns.Clear();
               VendorId.Clear();

               CanvasSheetType = true;
               //CheckCollapse();

               chk = new DataGridViewCheckBoxColumn();
               chk.HeaderText = "By Row";
               chk.Name = "By Row";
               dgvCsGel.Columns.Add(chk);

               dgvCsGel.ColumnCount = 7;
               dgvCsGel.Columns[1].Name = "No";
               dgvCsGel.Columns[2].Name = "GelombangId";
               dgvCsGel.Columns[3].Name = "FullItemID";
               dgvCsGel.Columns[4].Name = "ItemName";
               dgvCsGel.Columns[5].Name = "SeqNo";
               dgvCsGel.Columns[6].Name = "Base";

               Conn = ConnectionString.GetConnection();

               //List Item PR
               Query = "Select ROW_NUMBER() OVER (ORDER BY SeqNoDtl) No, GelombangId, FullItemID, ItemName, SeqNoDtl, Base From PurchRequisition_DtlDtl a where PurchReqID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
               Cmd = new SqlCommand(Query, Conn);
               Dr = Cmd.ExecuteReader();
               while (Dr.Read())
               {
                   this.dgvCsGel.Rows.Add("true", Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5]);
               }
               Dr.Close();

               //this.dgvCsGel.Rows.Add("*", "By Column", "");

               //List Quotation Vendor Id
               Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where ReffTransID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
               Cmd = new SqlCommand(Query, Conn);
               Dr = Cmd.ExecuteReader();
               dgvCsGel.Columns["SeqNo"].Visible = false;

               //int i = dgvCsGel.ColumnCount;
               int k = 1;
               while (Dr.Read())
               {
                   dgvCsGel.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice");
                   Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice").ToString());
                   //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPrice"].ReadOnly = true;

                   dgvCsGel.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty");
                   Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty").ToString());
                   //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nPR Qty"].ReadOnly = true;

                   //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty");
                   //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty").ToString());
                   //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty"].ReadOnly = false;

                   dgvCsGel.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty");
                   Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty").ToString());
                   //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nOrder Qty"].ReadOnly = false;
                   
                   VendorId.Add(Dr["VendId"].ToString());
                   PurchQuotId.Add(Dr["PurchQuotId"].ToString());

                   chk = new DataGridViewCheckBoxColumn();
                   chk.HeaderText = k + " " + Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck";
                   chk.Name = k + " " + Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck";
                   Columns.Add((k + " " + Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck").ToString());
                   //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nCheck"].ReadOnly = false;

                   dgvCsGel.Columns.Add(chk);
                   //dgvCsGel.Columns.Add();
               }

               if (VendorId.Count > 0)
               {
                   int col = 7;
                   for (int j = 0; j < VendorId.Count; j++)
                   {
                       for (int i = 0; i < dgvCsGel.Rows.Count; i++)
                       {
                           //Obtain a reference to the newly created DataGridViewRow 
                           //if (dgvCsGel.Rows[i].Cells[1].Value != "By Column")
                           //{
                           var row = this.dgvCsGel.Rows[i];

                           Query = "Select a.Price + case when d.Price is null then 0 end price2, Qty From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID left join PurchQuotation_DtlDtl c on a.FullItemID=c.FullItemID left join PurchQuotation_DtlDtl d on c.GelombangID=d.GelombangID and c.BracketId=d.BracketId and c.GelombangId='Y' where a.FullItemId='" + dgvCsGel.Rows[i].Cells[2].Value + "' and ReffTransID='" + txtPrNumber.Text + "' and b.VendId='" + VendorId[j].ToString() + "';";
                           Cmd = new SqlCommand(Query, Conn);
                           //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                           Dr = Cmd.ExecuteReader();
                           while (Dr.Read())
                           {
                               dgvCsGel.Rows[i].Cells[col].Value = (Dr[0].ToString() == "" ? "0" : Dr[0].ToString());
                               dgvCsGel.Rows[i].Cells[col + 2].Value = (Dr[1].ToString() == "" ? "0" : Dr[1].ToString());
                               dgvCsGel.Rows[i].Cells[col + 3].Value = "0";
                               dgvCsGel.Rows[i].Cells[col + 4].Value = false;
                           }

                           //}
                       }
                       col += 4;
                   }
               }

               dgvCsGel.ReadOnly = false;
               dgvCsGel.Columns["No"].ReadOnly = true;
               dgvCsGel.Columns["GelombangId"].ReadOnly = true;
               dgvCsGel.Columns["FullItemID"].ReadOnly = true;
               dgvCsGel.Columns["ItemName"].ReadOnly = true;
               dgvCsGel.Columns["SeqNo"].ReadOnly = true;
               dgvCsGel.Columns["Base"].ReadOnly = true;
               dgvCsGel.Columns["GelombangId"].Visible = false;

               for (int i = 0; i < Columns.Count; i++)
               {
                   if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Attachment"))
                   {
                       dgvCsGel.Columns[Columns[i]].ReadOnly = true;
                   }
               }

               foreach (DataGridViewColumn column in dgvPqDetails.Columns)
               {
                   column.SortMode = DataGridViewColumnSortMode.NotSortable;
               }

               foreach (DataGridViewColumn column in dgvCsGel.Columns)
               {
                   column.SortMode = DataGridViewColumnSortMode.NotSortable;
               }

               dgvCsGel.AutoResizeColumns();
           }
           Conn.Close();
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();
                    string CSNumber = "";
                    //IF New
                    if (txtCSNumber.Text.Trim() == "")
                    {
                        Query = "Insert into [CanvasSheetH] (CanvasId,CanvasDate,PurchReqId,TransType,TransStatus, StatusApproval,CreatedDate,CreatedBy) OUTPUT INSERTED.CanvasId values ";
                        Query += "((Select 'CS-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(CanvasId) is null then '1' else substring(Max(CanvasId),11,4)+1 end), 5) ";
                        Query += "from [CanvasSheetH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                        Query += "'" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "',";
                        Query += "'" + txtPrNumber.Text + "',";
                        Query += "(select top 1 TransType from [CanvasSheetH] a where a.PurchReqId='" + txtPrNumber.Text + "'),";
                        Query += "'01',";
                        Query += "'NO',";
                        Query += "getdate(),'');";
                        Cmd = new SqlCommand(Query, Conn);
                        CSNumber = Cmd.ExecuteScalar().ToString();

                        Query = "";
                        int CountGel = 1;
                        for (int i = 0; i < VendorId.Count; i++)
                        {
                            for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                            {
                                Query += "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,PurchReqSeqNo,FullItemId,ItemName,Price,PRQty,PQQty,Qty,VendID,CheckLine,CheckVendorD,CreatedDate,CreatedBy) Values ";
                                Query += "('" + CSNumber + "','";
                                Query += j + 1 + "','";
                                Query += dgvPqDetails.Rows[j].Cells["Delivery\nMethod"].Value.ToString() + "','";
                                Query += PurchQuotId[i] + "','";
                                Query += txtPrNumber.Text.Trim() + "','";
                                Query += dgvPqDetails.Rows[j].Cells["SeqNo"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells["ItemName"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 6 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 6 + i].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 7 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 7 + i].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 8 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 8 + i].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 9 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 9 + i].Value.ToString() + "','";
                                Query += VendorId[i] + "','";
                                Query += dgvPqDetails.Rows[j].Cells[0].Value.ToString() == null ? "false'," : dgvPqDetails.Rows[j].Cells[0].Value.ToString() + "','";// "false','"; //dgvPqDetails.Rows[j].Cells["By Row"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 10 + i].Value == null ? "false'," : dgvPqDetails.Rows[j].Cells[(i * 4) + 10 + i].Value.ToString() + "',";//"false',"; //dgvPqDetails.Rows[j].Cells[Columns[i + 2]].Value.ToString() + "','";
                                Query += "getdate(),'');";
                            }
                            if (CanvasSheetType == true)
                            {
                                for (int j = 0; j < dgvCsGel.Rows.Count; j++)
                                {
                                    Query += "Insert CanvasSheetD_Dtl ([CanvasID],PurchQuotId,[CanvasDate],[SeqNo],[SeqNoDtl],[VendID],[GroupID],[SubGroup1ID],[SubGroup2ID],[ItemID],[FullItemID],[ItemName],[Base],[Price],[GelombangID],[BracketID],CheckLine,CheckVendorD,[CreatedDate],[CreatedBy]) ";
                                    Query += "SELECT  distinct '" + CSNumber + "' CanvasID, a.PurchQuotID, '" + dtCanvasDate.Text.Trim() + "' CanvasDate,'" + (CountGel) + "',a.[SeqNoDtl],a.[VendID],a.[GroupID],a.[SubGroup1ID],a.[SubGroup2ID],a.[ItemID],a.[FullItemID],a.[ItemName],a.[Base],a.[Price],a.[GelombangID],a.[BracketID],'";
                                    Query += dgvCsGel.Rows[j].Cells[0].Value == null ? "false" : dgvCsGel.Rows[j].Cells[0].Value.ToString() + "','";
                                    Query += (dgvCsGel.Rows[j].Cells[((i + 1) * 4) + 6].Value == null ? "false" : dgvCsGel.Rows[j].Cells[((i + 1) * 4) + 6].Value.ToString()) + "',";
                                    Query += "getdate(),'' FROM PurchQuotation_DtlDtl a LEFT JOIN PurchQuotation_Dtl b on a.PurchQuotID=b.PurchQuotID and a.FullItemID=b.FullItemID where a.SeqNoDtl='" + dgvCsGel.Rows[j].Cells[5].Value.ToString() + "' and a.PurchQuotID='" + PurchQuotId[i].ToString() + "';";
                                    CountGel++;
                                }
                            }
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteScalar();
                        MessageBox.Show("Data CanvasSheetNumber : " + CSNumber + " berhasil ditambahkan.");
                        txtCSNumber.Text = CSNumber;
                    }
                    else
                    {
                        Query = "Update [CanvasSheetH] ";
                        Query += " set CanvasDate='" + dtCanvasDate.Value + "',";
                        Query += " PurchReqId='" + txtPrNumber.Text.Trim() + "',";
                        Query += " TransType=(select TransType from [CanvasSheetH] a where a.RfqID='" + txtPrNumber.Text.Trim() + "'),";
                        Query += " TransStatus='01',";
                        Query += " UpdatedDate=getdate(),UpdatedBy='' where CanvasId='" + txtCSNumber.Text.Trim() + "';";

                        Query = "Delete From CanvasSheetD where CanvasId='" + txtCSNumber.Text.Trim() + "';";
                        Query += "Delete From CanvasSheetD_Dtl where CanvasId='" + txtCSNumber.Text.Trim() + "';";

                        int CountGel = 1;
                        for (int i = 0; i < VendorId.Count; i++)
                        {
                            for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                            {
                                Query += "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,PurchReqSeqNo,FullItemId,ItemName,Price,PRQty,PQQty,Qty,VendID,CheckLine,CheckVendorD,CreatedDate,CreatedBy) Values ";
                                Query += "('" + txtCSNumber.Text.Trim() + "','";
                                Query += j + 1 + "','";
                                Query += dgvPqDetails.Rows[j].Cells["Delivery\nMethod"].Value.ToString() + "','";
                                Query += PurchQuotId[i] + "','";
                                Query += txtPrNumber.Text.Trim() + "','";
                                Query += dgvPqDetails.Rows[j].Cells["SeqNo"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells["ItemName"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 6 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 6 + i].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 7 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 7 + i].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 8 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 8 + i].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 9 + i].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 4) + 9 + i].Value.ToString() + "','";
                                Query += VendorId[i] + "','";
                                Query += dgvPqDetails.Rows[j].Cells[0].Value.ToString() == null ? "false'," : dgvPqDetails.Rows[j].Cells[0].Value.ToString() + "','";// "false','"; //dgvPqDetails.Rows[j].Cells["By Row"].Value.ToString() + "','";
                                Query += dgvPqDetails.Rows[j].Cells[(i * 4) + 10 + i].Value == null ? "false'," : dgvPqDetails.Rows[j].Cells[(i * 4) + 10 + i].Value.ToString() + "',";//"false',"; //dgvPqDetails.Rows[j].Cells[Columns[i + 2]].Value.ToString() + "','";
                                Query += "getdate(),'');";
                            }
                            if (CanvasSheetType == true)
                            {
                                for (int j = 0; j < dgvCsGel.Rows.Count; j++)
                                {
                                    Query += "Insert CanvasSheetD_Dtl ([CanvasID],PurchQuotId,[CanvasDate],[SeqNo],[SeqNoDtl],[VendID],[GroupID],[SubGroup1ID],[SubGroup2ID],[ItemID],[FullItemID],[ItemName],[Base],[Price],[GelombangID],[BracketID],CheckLine,CheckVendorD,[CreatedDate],[CreatedBy]) ";
                                    Query += "SELECT distinct '" + txtCSNumber.Text.Trim() + "' CanvasID, a.PurchQuotID, '" + dtCanvasDate.Text.Trim() + "' CanvasDate,'" + (CountGel) + "',a.[SeqNoDtl],a.[VendID],a.[GroupID],a.[SubGroup1ID],a.[SubGroup2ID],a.[ItemID],a.[FullItemID],a.[ItemName],a.[Base],a.[Price],a.[GelombangID],a.[BracketID],'";
                                    Query += dgvCsGel.Rows[j].Cells[0].Value == null ? "false" : dgvCsGel.Rows[j].Cells[0].Value.ToString() + "','";
                                    Query += (dgvCsGel.Rows[j].Cells[((i + 1) * 4) + 6].Value == null ? "false" : dgvCsGel.Rows[j].Cells[((i + 1) * 4) + 6].Value.ToString()) + "',";
                                    Query += "getdate(),'' FROM PurchQuotation_DtlDtl a LEFT JOIN PurchQuotation_Dtl b on a.PurchQuotID=b.PurchQuotID and a.FullItemID=b.FullItemID where a.SeqNoDtl='" + dgvCsGel.Rows[j].Cells[5].Value.ToString() + "' and a.PurchQuotID='" + PurchQuotId[i].ToString() + "';";
                                    CountGel++;
                                }
                            }
                        }

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteScalar();
                        scope.Complete();
                        MessageBox.Show("Data CanvasSheetNumber : " + txtCSNumber.Text.Trim() + " berhasil diupdate.");
                        txtCSNumber.Text = txtCSNumber.Text.Trim();
                    }
                }
            }

            catch (Exception)
            {
            }
            finally
            {
                GetDataHeader();
                ModeBeforeEdit();
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

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n---------\nVendor Qty");
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

                   dgvCsGel.ColumnCount = 7;
                   dgvCsGel.Columns[1].Name = "No";
                   dgvCsGel.Columns[2].Name = "GelombangId";
                   dgvCsGel.Columns[3].Name = "FullItemID";
                   dgvCsGel.Columns[4].Name = "ItemName";
                   dgvCsGel.Columns[5].Name = "SeqNo";
                   dgvCsGel.Columns[6].Name = "Base";

                   Conn = ConnectionString.GetConnection();

                   //List Item PR
                   Query = "Select ROW_NUMBER() OVER (ORDER BY SeqNoDtl) No, GelombangId, FullItemID, ItemName, SeqNoDtl, Base From PurchRequisition_DtlDtl a where PurchReqID='" + txtPrNumber.Text + "'";// and a.TransStatus=''";
                   Cmd = new SqlCommand(Query, Conn);
                   Dr = Cmd.ExecuteReader();
                   while (Dr.Read())
                   {
                       this.dgvCsGel.Rows.Add("true", Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5]);
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
                       int col = 7;
                       for (int j = 0; j < VendorId.Count; j++)
                       {
                           for (int i = 0; i < dgvCsGel.Rows.Count; i++)
                           {
                               //Obtain a reference to the newly created DataGridViewRow 
                               //if (dgvCsGel.Rows[i].Cells[1].Value != "By Column")
                               //{
                               var row = this.dgvCsGel.Rows[i];

                               Query = "Select CheckLine, Price, Qty, CheckVendorD FROM [dbo].[CanvasSheetD_Dtl] where canvasid='" + txtCSNumber.Text.Trim() + "' and FullItemId='" + dgvCsGel.Rows[i].Cells["FullItemId"].Value.ToString() + "' and PurchQuotID='" + PurchQuotId[j].ToString() + "';";
                               Cmd = new SqlCommand(Query, Conn);
                               //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                               Dr = Cmd.ExecuteReader();
                               while (Dr.Read())
                               {
                                   dgvCsGel.Rows[i].Cells[0].Value = Dr[0].ToString();
                                   dgvCsGel.Rows[i].Cells[col ].Value = Dr[1].ToString();
                                   dgvCsGel.Rows[i].Cells[col + 2].Value = Dr[2].ToString();
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
                   dgvCsGel.Columns["GelombangId"].ReadOnly = true;
                   dgvCsGel.Columns["FullItemID"].ReadOnly = true;
                   dgvCsGel.Columns["ItemName"].ReadOnly = true;
                   dgvCsGel.Columns["SeqNo"].ReadOnly = true;
                   dgvCsGel.Columns["Base"].ReadOnly = true;

                   dgvCsGel.Columns["GelombangId"].Visible = false;
                   for (int i = 0; i < Columns.Count; i++)
                   {
                       if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Attachment"))
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            GetDataHeader();
        }


        private void dgvPqDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Index = dgvPqDetails.CurrentRow.Index;

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString().Contains("Order"))
            {
                if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex-1].Value.ToString()))
                {
                    MessageBox.Show("Qty Order tidak boleh lebih besar dari Vendor Qty.");
                    dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value.ToString();
                }
            }

        }

        private void dgvPqDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //ListGelombangSelectItem();
        }

        //private void ListGelombangSelectItem()
        //{
        //    for (int i = 0; i < dgvCsGel.RowCount; i++)
        //    {
        //        dgvCsGel.Rows[i].Visible = false;
        //    }

        //    Conn = ConnectionString.GetConnection();
        //    Query = "Select [GelombangId] From [InventGelombangD] Where GelombangId = (Select GelombangId from InventGelombangD where ItemId = '" + dgvPqDetails.CurrentRow.Cells["FullItemId"].Value.ToString() + "') and BracketId = (Select BracketId from InventGelombangD where ItemId = '" + dgvPqDetails.CurrentRow.Cells["FullItemId"].Value.ToString() + "')";
        //    Cmd = new SqlCommand(Query, Conn);
        //    Dr = Cmd.ExecuteReader();
        //    while (Dr.Read())
        //    {
        //        for (int i = 0; i < dgvCsGel.RowCount; i++)
        //        {
        //            if (dgvCsGel.Rows[i].Cells["GelombangId"].Value.ToString() == Dr[0].ToString())
        //            {
        //                dgvCsGel.Rows[i].Visible = true;
        //            }
        //        }
        //    }
        //    Conn.Close();
        //}
        //private void dgvPqDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        //{
        //    List<string> TmpColumns = new List<string>();
            
        //    for(int i=0; i<Columns.Count(); i++)
        //    {
        //        if (Columns[i].Contains("Check"))
        //            TmpColumns.Add(Columns[i].ToString());
        //    }

        //    Columns.Contains("Check");

        //    if (Mode != "BeforeEdit")
        //    {
        //         for(int i=0; i<TmpColumns.Count(); i++)
        //        {
        //            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == TmpColumns[i])
        //            {
        //                if (dgvPqDetails.CurrentCell.Value.ToString().ToLower() == "true")
        //                {
        //                    dgvPqDetails.CurrentCell.Value = false;
        //                }
        //                else
        //                {
        //                    dgvPqDetails.CurrentCell.Value = true;
        //                }
        //            }
        //        }
        //    }

        //    dgvPqDetails.AutoResizeColumns();

            
        //}
    }
}
