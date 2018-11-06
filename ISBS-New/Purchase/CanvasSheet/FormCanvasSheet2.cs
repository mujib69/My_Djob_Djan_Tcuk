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
    public partial class FormCanvasSheet2 : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string[] SubHeader = { "Price", "Order\nQty" };

        string Mode, Query, crit, CSNumber = null;

        int Index;
        private int[] daysInMonths;

        public Boolean ApproveStatus = false;

        public string PQNumber = "", tmpPrType = "";

        DateTimePicker dtp;

        List<string> ListSeqNum;
        List<Purchase.PurchaseRequisition.Info> ListInfo = new List<Purchase.PurchaseRequisition.Info>();

        List<string> VendorId = new List<string>(); //list jumlah vendor
        List<string> TmpVendorId = new List<string>(); //list jumlah vendor temporary
        List<string> Columns = new List<string>(); //list jumlah column di datagrid price, pr qty, qty, check
        public List<string> PurchQuotId = new List<string>();
        public string PurchQuotIdString = "";
        public List<string> NotInPurchQuotIdString = new List<string>();
        List<Color> ListColor = new List<Color>();

        Purchase.PurchaseQuotation.InquiryPQ Parent;
        Boolean CanvasSheetType = false;

        public FormCanvasSheet2()
        {
            InitializeComponent();
        }

        private void FormCanvasSheet2_Load(object sender, EventArgs e)
        {
            lblForm.Location = new Point(16, 11);
        }

        public void ModeNew()
        {
            //CheckCollapse();
            txtCSNumber.Text = "";
            txtPrNumber.Text = "";
            //dtCanvasDate.Value = Convert.ToDateTime("01-01-1990");
            dtCanvasDate.Value = DateTime.Today;

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
            btnAddQuotation.Enabled = true;
            btnDeleteQuotation.Enabled = true;

            //if(txtCSNumber.Text.Trim()!="")
            btnSearchPR.Enabled = false ;
            //dgvPqDetails.Enabled = true;
            dgvPqDetails.DefaultCellStyle.BackColor = Color.White;

            dgvPqDetails.ReadOnly = false;
            dgvPqDetails.Columns["No"].ReadOnly = true;
            dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPqDetails.Columns["ItemName"].ReadOnly = true;
            dgvPqDetails.Columns["SeqNo"].ReadOnly = true;
            dgvPqDetails.Columns["PR Qty"].ReadOnly = true;
            dgvPqDetails.Columns["BracketDesc"].ReadOnly = true;

            dgvPqDetails.Columns["No"].Frozen = true;
            dgvPqDetails.Columns["Delivery\nMethod"].Frozen = true;
            dgvPqDetails.Columns["FullItemID"].Frozen = true;
            dgvPqDetails.Columns["ItemName"].Frozen = true;
            dgvPqDetails.Columns["SeqNo"].Frozen = true;
            dgvPqDetails.Columns["PR Qty"].Frozen = true;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Vendor") || Columns[i].Contains("Attachment") || Columns[i].Contains("Approval"))
                {
                    if (Columns[i].Contains("Approval"))
                    {
                        for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                        {
                            if (dgvPqDetails.Rows[j].Cells["Base"].Value.ToString() == "N")
                            {
                                dgvPqDetails.Rows[j].ReadOnly = true;
                            }
                        }
                    }
                    else
                    {
                        dgvPqDetails.Columns[Columns[i]].ReadOnly = true;
                    }
                }
            }
            dgvPqDetails.Rows[0].Frozen = true;

            dgvPqDetails.AutoResizeColumns();
            for (int j = 11; j < dgvPqDetails.ColumnCount; j+=9)
            {
                for (int i = 0; i < dgvPqDetails.RowCount; i++)
                {
                    if (txtPrType.Text.Trim() != "FIX")
                    {
                        if (dgvPqDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                        {
                            dgvPqDetails.Rows[i].Cells[j-1].ReadOnly = true;
                            //dgvPqDetails.Rows[i].Cells[0].Style.BackColor = Color.LightYellow;
                            //dgvPqDetails.Rows[i].Cells[j-1].Style.BackColor = Color.LightYellow;
                            dgvPqDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            //dgvPqDetails.Rows[i].Cells[j-1].Style.BackColor = Color.LightYellow;
                            //dgvPqDetails.Rows[i].Cells[0].Style.BackColor = Color.LightYellow;
                            //dgvPqDetails.Rows[i].Cells[j].Style.BackColor = Color.LightYellow;
                        }
                    }
                    else
                    {
                        //dgvPqDetails.Rows[i].Cells[j-1].Style.BackColor = Color.LightYellow;
                        //dgvPqDetails.Rows[i].Cells[0].Style.BackColor = Color.LightYellow;
                        //dgvPqDetails.Rows[i].Cells[j].Style.BackColor = Color.LightYellow;
                    }
                }
            }
            dgvPqDetails.AutoResizeColumns();
            EditColor();
        }

        public void ModeBeforeEdit(string TmpCSNumber)
        {
            txtCSNumber.Text = TmpCSNumber;

            txtPrNumber.Text = "";
            ModeBeforeEdit();
        }

        public void ModeBeforeEdit()
        {
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            btnDeleteQuotation.Enabled = false;
            btnAddQuotation.Enabled = false;

            btnSearchPR.Enabled = false;
            //dgvPqDetails.Enabled = false;

            dgvPqDetails.ReadOnly = true;
            dgvPqDetails.AutoResizeColumns();
            dgvPqDetails.DefaultCellStyle.BackColor = Color.LightGray;
            int x = 0;//membaca column
            int i = 9;//start pewarnaan column
            for (int j = 0; j < ListColor.Count; j++)
            {
                if (i + 1 >= dgvPqDetails.ColumnCount)
                {
                    break;
                }
                x = 0;
                while (x < 9)
                {
                    dgvPqDetails.Columns[x + i].DefaultCellStyle.BackColor = Color.LightGray;
                    x++;
                }
                i += 9;
            }
        }

        private void EditColor()
        {
            ListColor.Add(Color.LightBlue);
            ListColor.Add(Color.LightCoral);
            ListColor.Add(Color.LightCyan);
            ListColor.Add(Color.LightGoldenrodYellow);
            ListColor.Add(Color.LightGray);
            ListColor.Add(Color.LightGreen);
            ListColor.Add(Color.LightPink);
            ListColor.Add(Color.LightSalmon);
            ListColor.Add(Color.LightSeaGreen);
            ListColor.Add(Color.LightSkyBlue);
            ListColor.Add(Color.LightSlateGray);

            int x = 0;//membaca column
            int i = 9;//start pewarnaan column
            for (int j = 0; j < ListColor.Count; j++)
            {
                if (i+1 >= dgvPqDetails.ColumnCount)
                {
                    break;
                }
                x = 0;
                while(x<9)
                {
                    dgvPqDetails.Columns[x + i].DefaultCellStyle.BackColor = ListColor[j];
                    x++;
                }
                i += 9;
            }
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
            NotInPurchQuotIdString.Clear();
            PurchQuotIdString = "";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);//, Where);
            tmpSearch.ShowDialog();
            txtPrNumber.Text = ConnectionString.Kode;
            txtPrType.Text = ConnectionString.Kode2;

            if (dgvPqDetails.Rows.Count >= 1)
            {
                dgvPqDetails.Rows.Clear();
                dgvPqDetails.Columns.Clear();
                //VendorId.Clear();
            }

            //DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            //chk.HeaderText = "By Row";
            //chk.Name = "By Row";
            //dgvPqDetails.Columns.Add(chk);

            dgvPqDetails.ColumnCount = 9;
            dgvPqDetails.Columns[0].Name = "No";
            dgvPqDetails.Columns[1].Name = "Delivery\nMethod";
            dgvPqDetails.Columns[2].Name = "FullItemID";
            dgvPqDetails.Columns[3].Name = "ItemName";
            dgvPqDetails.Columns[4].Name = "SeqNo";
            dgvPqDetails.Columns[5].Name = "Base";
            dgvPqDetails.Columns[6].Name = "SeqNoGroup";
            dgvPqDetails.Columns[7].Name = "PR Qty";
            dgvPqDetails.Columns[8].Name = "BracketDesc";

            Conn = ConnectionString.GetConnection();

            //List Item PR
            Query = "Select distinct a.DeliveryMethod, a.FullItemID, a.ItemName, a.SeqNo, a.Base, a.SeqNoGroup,b.[Qty],a.BracketDesc  From PurchQuotation_Dtl a left join PurchRequisition_Dtl b on b.PurchReqID=a.ReffTransID and a.ReffSeqNo=b.SeqNo where a.ReffTransID='" + txtPrNumber.Text + "' order by a.SeqNoGroup asc, a.SeqNo asc, a.Base desc";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int x = 1;
            while (Dr.Read())
            {
                this.dgvPqDetails.Rows.Add(x++, Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7]);
                if(Dr[4].ToString() == "N")
                {
                    dgvPqDetails.Rows[(x - 2)].DefaultCellStyle.BackColor = Color.LightGray;
                }
                //dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells["PR Qty"].Value = Convert.ToDecimal(dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells["PR Qty"].Value).ToString("N2");
            }
            Dr.Close();

            dgvPqDetails.Columns["SeqNo"].Visible = false;
            dgvPqDetails.Columns["SeqNoGroup"].Visible = false;
            dgvPqDetails.Columns["Base"].Visible = false;
            dgvPqDetails.Columns["BracketDesc"].Visible = false;

            dgvPqDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Delivery\nMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SeqNo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["PR Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPqDetails.Columns["BracketDesc"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPqDetails.Columns["PR Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvPqDetails.AutoResizeColumns();
            Conn.Close();
            //dgvPqDetails.Columns["By Row"].DefaultCellStyle.ForeColor = Color.LightYellow;
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int CountItem = 0, Yes = 0, No = 0;
                if (ApproveStatus == true)
                {
                    for (int i = 0; i < VendorId.Count; i++)
                    {
                        for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                        {
                            if (txtPrType.Text != "FIX")
                            {
                                if (dgvPqDetails.Rows[j].Cells["BASE"].Value.ToString() == "Y")
                                {
                                    CountItem++;
                                    if (dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value != null)
                                    {
                                        if (dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString() == "Yes")
                                        {
                                            Yes++;
                                        }
                                        else if (dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString() == "No")
                                        {
                                            No++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CountItem++;
                                if (dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString() == "Yes")
                                {
                                    Yes++;
                                }
                                else if (dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString() == "No")
                                {
                                    No++;
                                }
                            }
                        }
                    }

                    if (CountItem != (Yes + No))
                    {
                        MessageBox.Show("Approval tidak boleh ada yang kosong.");
                        goto Outer;
                    }
                }
                
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                string CSNumber = "";
                //IF New
                if (txtCSNumber.Text.Trim() == "")
                {
                     Query = "Insert into [CanvasSheetH] (CanvasId,CanvasDate,PurchReqId,TransType,TransStatus,CreatedDate,CreatedBy) OUTPUT INSERTED.CanvasId values ";
                    Query += "((Select 'CS-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(CanvasId) is null then '1' else substring(Max(CanvasId),11,4)+1 end), 5) ";
                    Query += "from [CanvasSheetH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    Query += "'" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "',";
                    Query += "'" + txtPrNumber.Text + "',";
                    Query += "(select top 1 TransType from [PurchRequisitionH] a where a.PurchReqId='" + txtPrNumber.Text + "'),";
                    Query += "'CS01',";
                    //Query += "'Pending',";
                    Query += "getdate(),'" + Login.Username + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    CSNumber = Cmd.ExecuteScalar().ToString();

                    Query = "";
                    int CountGel = 1;
                    for (int i = 0; i < VendorId.Count; i ++)
                    {
                        for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                        {
                            Query += "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,FullItemId,ItemName,PurchReqSeqNo,Base,SeqNoGroup,PRQty,BracketDesc,Price,PQQty,Qty,VendID,PPN,PPH,Unit,Ratio,StatusApproval,CreatedDate,CreatedBy) Values ";
                            Query += "('" + CSNumber + "','";
                            Query += j + 1 + "','";
                            Query += dgvPqDetails.Rows[j].Cells["Delivery\nMethod"].Value.ToString() + "','";
                            Query += PurchQuotId[i] + "','";
                            Query += txtPrNumber.Text.Trim() + "','";
                            //Query += dgvPqDetails.Rows[j].Cells["SeqNo"].Value == null ? "','" : (dgvPqDetails.Rows[j].Cells["SeqNo"].Value.ToString() + "','");
                            Query += dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells["ItemName"].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[4].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[4].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[5].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[5].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[6].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[6].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[7].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[7].Value.ToString().Replace(",", "") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[8].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[8].Value.ToString() + "','";
                            //(i * 9) digunakan untuk jeda setiap ganti vendor 
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 9].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 9].Value.ToString().Replace(",", "") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value.ToString().Replace(",", "") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value.ToString().Replace(",", "") + "','";
                            Query += VendorId[i] + "','";
                            //Query += dgvPqDetails.Rows[j].Cells[0].Value.ToString() == null ? "false'," : dgvPqDetails.Rows[j].Cells[0].Value.ToString() + "','";// "false','"; //dgvPqDetails.Rows[j].Cells["By Row"].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 13].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 13].Value.ToString().Replace(",", "") + "','";//"false',"; //dgvPqDetails.Rows[j].Cells[Columns[i + 2]].Value.ToString() + "','";

                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value.ToString().Replace(",", "") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value == null ? "Null','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value.ToString().Replace(",", "") + "','";
                            //Query += dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value + "',";
                            Query += "getdate(),'" + Login.Username + "');";
                        }
                    }
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
                    Trans.Commit();
                    MessageBox.Show("Data CanvasSheetNumber : " + CSNumber + " berhasil ditambahkan.");
                    txtCSNumber.Text = CSNumber;
                }
                else
                {
                    Query = "Update [CanvasSheetH] ";
                    Query += " set CanvasDate='" + dtCanvasDate.Value + "',";
                    Query += " PurchReqId='" + txtPrNumber.Text.Trim() + "',";
                    Query += " TransType=(select top 1 TransType from [CanvasSheetH] a where a.PurchReqId='" + txtPrNumber.Text.Trim() + "'),";
                    if (ApproveStatus == true)
                    {
                        if (Yes > 0)
                        {
                            Query += "TransStatus='CS02',";
                        }
                        else
                        {
                            Query += "TransStatus='CS03',";
                        }
                    }
                    else
                    {
                        Query += "TransStatus='CS01',";
                    }
                    Query += " UpdatedDate=getdate(),UpdatedBy='" + Login.Username + "' where CanvasId='" + txtCSNumber.Text.Trim() + "';";

                    Query += "Delete From CanvasSheetD where CanvasId='" + txtCSNumber.Text.Trim() + "';";
                    Query += "Delete From CanvasSheetD_Dtl where CanvasId='" + txtCSNumber.Text.Trim() + "';";

                    int CountGel = 1;
                    for (int i = 0; i < VendorId.Count; i++)
                    {
                        for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                        {
                            Query += "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,FullItemId,ItemName,PurchReqSeqNo,Base,SeqNoGroup,PRQty,BracketDesc,Price,PQQty,Qty,VendID,PPN,PPH,Unit,Ratio,StatusApproval,UpdatedDate,UpdatedBy) Values ";
                            Query += "('" + txtCSNumber.Text.Trim() + "','";
                            Query += j + 1 + "','";
                            Query += dgvPqDetails.Rows[j].Cells["Delivery\nMethod"].Value.ToString() + "','";
                            Query += PurchQuotId[i] + "','";
                            Query += txtPrNumber.Text.Trim() + "','";
                            //Query += dgvPqDetails.Rows[j].Cells["SeqNo"].Value == null ? "','" : (dgvPqDetails.Rows[j].Cells["SeqNo"].Value.ToString() + "','");
                            Query += dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells["ItemName"].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[4].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[4].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[5].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[5].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[6].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[6].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[7].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[7].Value.ToString().Replace(",","") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[8].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[8].Value.ToString() + "','";
                            //(i * 9) digunakan untuk jeda setiap ganti vendor 
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 9].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 9].Value.ToString().Replace(",","") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value.ToString().Replace(",","") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value.ToString().Replace(",","") + "','";
                            Query += VendorId[i] + "','";
                            //Query += dgvPqDetails.Rows[j].Cells[0].Value.ToString() == null ? "false'," : dgvPqDetails.Rows[j].Cells[0].Value.ToString() + "','";// "false','"; //dgvPqDetails.Rows[j].Cells["By Row"].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 13].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 13].Value.ToString().Replace(",", "") + "','";//"false',"; //dgvPqDetails.Rows[j].Cells[Columns[i + 2]].Value.ToString() + "','";

                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value.ToString().Replace(",", "") + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value == null ? "Null','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value.ToString().Replace(",", "") + "','";
                            //Query += dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value.ToString() + "','";
                            Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value + "',";
                            Query += "getdate(),'" + Login.Username + "');";
                        }
                    }

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
                    Trans.Commit();
                    MessageBox.Show("Data CanvasSheetNumber : " + txtCSNumber.Text.Trim() + " berhasil diupdate.");
                    txtCSNumber.Text = txtCSNumber.Text.Trim();
                }
                GetDataHeader();
                EditColor();
                ModeBeforeEdit();
                Outer:;
            }
            catch (Exception)
            {
                Trans.Rollback();
            }
        }

        public void GetDataHeader()
        {
            if (txtCSNumber.Text.Trim() != "")
            {
                Conn = ConnectionString.GetConnection();
                Query = "SELECT a.[CanvasId],a.[CanvasDate],a.[PurchReqId],a.[TransType],a.[TransStatus], a.StatusApproval,b.[Deskripsi] FROM [dbo].[CanvasSheetH] a left join TransStatusTable b on a.TransStatus=b.TransCode where [CanvasId]='" + txtCSNumber.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtCanvasDate.Text = Dr["CanvasDate"].ToString();
                    txtCSNumber.Text = Dr["CanvasId"].ToString();
                    txtPrNumber.Text = Dr["PurchReqId"].ToString();
                    txtTransStatusCode.Text = Dr["TransStatus"].ToString();
                    txtTransStatusDesc.Text = Dr["Deskripsi"].ToString();
                    txtPrType.Text = Dr["TransType"].ToString();
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

                //DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                //chk.HeaderText = "By Row";
                //chk.Name = "By Row";
                //dgvPqDetails.Columns.Add(chk);

                dgvPqDetails.ColumnCount = 9;
                dgvPqDetails.Columns[0].Name = "No";
                dgvPqDetails.Columns[1].Name = "Delivery\nMethod";
                dgvPqDetails.Columns[2].Name = "FullItemID";
                dgvPqDetails.Columns[3].Name = "ItemName";
                dgvPqDetails.Columns[4].Name = "SeqNo";
                dgvPqDetails.Columns[5].Name = "Base";
                dgvPqDetails.Columns[6].Name = "SeqNoGroup";
                dgvPqDetails.Columns[7].Name = "PR Qty";
                dgvPqDetails.Columns[8].Name = "BracketDesc";

                Conn = ConnectionString.GetConnection();

                //List Item PR
                //ROW_NUMBER() OVER (ORDER BY PurchReqSeqNo) No
                Query = "Select a.CanvasSeqNo, DeliveryMethod, a.FullItemID, a.ItemName, a.PurchReqSeqNo, a.Base, a.SeqNoGroup, a.PRQty, a.BracketDesc From (Select distinct DeliveryMethod,FullItemID, ItemName, PurchReqSeqNo, CanvasSeqNo, Base, SeqNoGroup, PRQty, BracketDesc From CanvasSheetD a where CanvasId='" + txtCSNumber.Text.Trim() + "') a  order by CanvasSeqNo asc";// and a.TransStatus=''";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int x = 1;
                while (Dr.Read())
                {
                    this.dgvPqDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8]);
                    //this.dgvPqDetails.Rows.Add(Dr[0].ToString(), x++, Dr[1], Dr[2], Dr[3], Dr[4]);
                    if (Dr[4].ToString() == "N")
                    {
                        dgvPqDetails.Rows[(x - 3)].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    //dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells["PR Qty"].Value = Convert.ToDecimal(dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells["PR Qty"].Value).ToString("N2");
                }
                Dr.Close();

                //this.dgvPqDetails.Rows.Add("*", "By Column", "");

                //List Quotation Vendor Id
                Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID inner join CanvasSheetD c on c.VendId=b.VendId where ReffTransID='" + txtPrNumber.Text + "' and c.CanvasId='" + txtCSNumber.Text.Trim() + "'";// and a.TransStatus=''";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                dgvPqDetails.Columns["SeqNo"].Visible = false;
                dgvPqDetails.Columns["Base"].Visible = false;
                dgvPqDetails.Columns["SeqNoGroup"].Visible = false;
                dgvPqDetails.Columns["BracketDesc"].Visible = false;

                //int i = dgvPqDetails.ColumnCount;
                
                while (Dr.Read())
                {
                    //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice");
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice").ToString());
                    ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].ReadOnly = true;

                    //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty");
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty").ToString());
                    ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].ReadOnly = true;

                    //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty");
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty").ToString());
                    ////dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].ReadOnly = false;
                
                    //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty");
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty").ToString());
                    ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].ReadOnly = false;

                    //VendorId.Add(Dr["VendId"].ToString());

                    //PurchQuotId.Add(Dr["PurchQuotId"].ToString());

                    //chk = new DataGridViewCheckBoxColumn();
                    //chk.HeaderText = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck";
                    //chk.Name = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck";
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck").ToString());
                    ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck"].ReadOnly = false;

                    //dgvPqDetails.Columns.Add(chk);
                    ////dgvPqDetails.Columns.Add();
                    
                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice").ToString());
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].ReadOnly = true;
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty");
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty").ToString());
                    ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].ReadOnly = true;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty").ToString());
                    //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].ReadOnly = false;
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    x = dgvPqDetails.ColumnCount;
                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty").ToString());
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    VendorId.Add(Dr["VendId"].ToString());

                    PurchQuotId.Add(Dr["PurchQuotId"].ToString());
                    NotInPurchQuotIdString.Add("'"+Dr["PurchQuotId"].ToString()+"'");

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment").ToString());
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    
                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN").ToString());
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN"].Visible = false;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH").ToString());
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH"].Visible = false;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit").ToString());
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit"].Visible = false;

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio").ToString());
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio"].Visible = false;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval").ToString());
                    dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (ApproveStatus == false)
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval"].Visible = false;

                    //DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    //chk.HeaderText = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck";
                    //chk.Name = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck";
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck").ToString());
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck"].ReadOnly = false;

                    //dgvPqDetails.Columns.Add(chk);
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck"].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

               if (VendorId.Count > 0)
               {
                   int col = 9;
                   for (int j = 0; j < VendorId.Count; j++)
                   {
                       for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                       {
                           //Obtain a reference to the newly created DataGridViewRow 
                           //if (dgvPqDetails.Rows[i].Cells[1].Value != "By Column")
                           //{
                           var row = this.dgvPqDetails.Rows[i];

                           Query = "Select Price, PQQty, Qty, 'Files', PPN, PPH, Unit, Ratio, StatusApproval  From CanvasSheetD where FullItemId='" + dgvPqDetails.Rows[i].Cells[2].Value + "' and CanvasId='" + txtCSNumber.Text.Trim() + "' and PurchQuotId='" + PurchQuotId[j] + "' and DeliveryMethod='" + dgvPqDetails.Rows[i].Cells["Delivery\nMethod"].Value.ToString() + "';";
                           Cmd = new SqlCommand(Query, Conn);
                           //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                           Dr = Cmd.ExecuteReader();
                           while (Dr.Read())
                           {
                               dgvPqDetails.Rows[i].Cells[col].Value = Dr[0].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 1].Value = Dr[1].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 2].Value = Dr[2].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 3].Value = Dr[3].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 4].Value = Dr[4].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 5].Value = Dr[5].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 6].Value = Dr[6].ToString();
                               dgvPqDetails.Rows[i].Cells[col + 7].Value = Dr[7].ToString();


                               DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                               combo.Items.Add("");
                               combo.Items.Add("Yes");
                               combo.Items.Add("No");
                               combo.Value = Dr[8].ToString();

                               if (txtPrType.Text.Trim() != "FIX")
                               {
                                   if (dgvPqDetails.Rows[i].Cells["Base"].Value.ToString() == "Y")
                                   {
                                       dgvPqDetails.Rows[i].Cells[col + 8] = combo;
                                   }
                               }
                               else
                               {
                                   dgvPqDetails.Rows[i].Cells[col + 8] = combo;
                               }
                                    
                               //dgvPqDetails.Rows[i].Cells[col + 8].Value = Dr[8].ToString();
                           }
                           //}
                       }
                       col += 9;
                   }
               }

               

               Conn.Close();

               foreach (DataGridViewColumn column in dgvPqDetails.Columns)
               {
                   column.SortMode = DataGridViewColumnSortMode.NotSortable;
               }

               //foreach (DataGridViewColumn column in dgvCsGel.Columns)
               //{
               //    column.SortMode = DataGridViewColumnSortMode.NotSortable;
               //}
            }
            dgvPqDetails.AutoResizeColumns();
            //dgvPqDetails.Columns["PR Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
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
                if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value != null)
                {
                    if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString()))
                    {
                        decimal TmpQty = 0;
                        for (int i = 2; i < Columns.Count(); i += 9)
                        {
                            TmpQty += Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[Columns[i]].Value);
                        }

                        if (TmpQty > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value))
                        {
                            MessageBox.Show("Qty Order tidak boleh lebih besar dari PR Qty.");
                            TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString()) - TmpQty;
                            if (TmpQty < 0)
                            {
                                TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value.ToString()) + TmpQty;
                            }
                            else
                            {
                                TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value.ToString());
                            }
                            dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                        }
                        else
                        {
                            MessageBox.Show("Qty Order tidak boleh lebih besar dari Vendor Qty.");
                            dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString();
                        }
                    }
                    else
                    {
                        decimal TmpQty = 0;
                        for (int i = 2; i < Columns.Count(); i += 9)
                        {
                            TmpQty += Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[Columns[i]].Value);
                        }

                        if (TmpQty > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value))
                        {
                            MessageBox.Show("Qty Order tidak boleh lebih besar dari PR Qty.");
                            TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString()) - TmpQty;
                            if (TmpQty < 0)
                            {
                                TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value.ToString()) + TmpQty;
                            }
                            else
                            {
                                TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value.ToString());
                            }
                            dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                        }
                    }
                    //dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells[dgvPqDetails.CurrentCell.ColumnIndex].Value = Convert.ToDecimal(dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells[dgvPqDetails.CurrentCell.ColumnIndex].Value).ToString("N2");
                }
            }

            //if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString().Contains("Order"))
            //{
            //    if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value != null)
            //    {
            //        if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value.ToString()))
            //        {
            //            MessageBox.Show("Qty Order tidak boleh lebih besar dari Vendor Qty.");
            //            dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value.ToString();
            //        }
            //    }
            //}

        }

        //private void DatagridColor()
        //{
        //    for (int i = 0; i < dgvPqDetails.RowCount; i++)
        //    {
        //        if (dgvPqDetails.Rows[Index].Cells["Base"].Value.ToString() == "N")
        //        {
        //            dgvPqDetails.Rows[Index].Cells[].ReadOnly = true;
        //        }
        //    }
        //}


        private void btnAddQuotation_Click(object sender, EventArgs e)
        {
            //if (dgvPqDetails.ColumnCount < (8 + (8 * 3)))
            //{
                int TmpColumnCount = dgvPqDetails.ColumnCount;
                SearchQuotation Quotation = new SearchQuotation();

                List<SearchQuotation> ListQuotation = new List<SearchQuotation>();
                Quotation.SetParent(this);
                ListQuotation.Add(Quotation);
                Quotation.ShowDialog();

                //this.dgvPqDetails.Rows.Add("*", "By Column", "");
                //List Quotation Vendor Id
                //int x = 0;
                if (PurchQuotIdString != "")
                {

                    Conn = ConnectionString.GetConnection();
                    Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId, b.PPH, b.PPN From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.PurchQuotID in (" + PurchQuotIdString + ") order by a.PurchQuotID";// and a.TransStatus=''";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    dgvPqDetails.Columns["SeqNo"].Visible = false;

                    //int i = dgvPqDetails.ColumnCount;
                    TmpVendorId.Clear();

                    while (Dr.Read())
                    {

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice").ToString());
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].ReadOnly = true;
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty");
                        //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty").ToString());
                        ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].ReadOnly = true;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty").ToString());
                        //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].ReadOnly = false;
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        //x = dgvPqDetails.ColumnCount;
                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        VendorId.Add(Dr["VendId"].ToString());
                        TmpVendorId.Add(Dr["VendId"].ToString());

                        PurchQuotId.Add(Dr["PurchQuotId"].ToString());

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment"].SortMode = DataGridViewColumnSortMode.NotSortable;

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN"].Visible = false;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH"].Visible = false;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nUnit"].Visible = false;

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio"].Visible = false;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nRatio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        if(ApproveStatus == false)
                            dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval"].Visible = false;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval"].Visible = false;
                       
                        //DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                        //chk.HeaderText = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck";
                        //chk.Name = Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck";
                        //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck").ToString());
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck"].ReadOnly = false;

                        //dgvPqDetails.Columns.Add(chk);
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nCheck"].SortMode = DataGridViewColumnSortMode.NotSortable;

                        //dgvPqDetails.Columns.Add();
                    }

                    if (VendorId.Count > 0)
                    {
                        int col = TmpColumnCount;
                        //if (TmpColumnCount - 8 > 1)
                        //{
                        //    col += (NotInPurchQuotIdString.Count - 1) * 8;
                        //}
                        for (int j = 0; j < TmpVendorId.Count; j++)
                        {
                            for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                            {
                                //Obtain a reference to the newly created DataGridViewRow 
                                //if (dgvPqDetails.Rows[i].Cells[1].Value != "By Column")
                                //{
                                var row = this.dgvPqDetails.Rows[i];

                                Query = "Select Price, Qty2, a.PurchQuotID, SeqNo, FullItemID, a.DeliveryMethod, b.PPN, b.PPH, a.Unit, a.Ratio From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.DeliveryMethod='" + dgvPqDetails.Rows[i].Cells[1].Value + "' and a.FullItemId='" + dgvPqDetails.Rows[i].Cells[2].Value + "' and ReffTransID='" + txtPrNumber.Text + "' and b.VendId='" + VendorId[j].ToString() + "';";
                                Cmd = new SqlCommand(Query, Conn);
                                //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    dgvPqDetails.Rows[i].Cells[col].Value = (Dr[0].ToString() == "" ? "0" : Dr[0].ToString());
                                    //dgvPqDetails.Rows[i].Cells[col + 1].Value = (Dr[1].ToString() == "" ? "0" : Dr[1].ToString());
                                    dgvPqDetails.Rows[i].Cells[col + 1].Value = (Dr[1].ToString() == "" ? "0" : Dr[1].ToString());
                                    dgvPqDetails.Rows[i].Cells[col + 2].Value = "0";
                                    dgvPqDetails.Rows[i].Cells[col + 3].Value = "Files";
                                    dgvPqDetails.Rows[i].Cells[col + 4].Value = (Dr["PPN"].ToString() == "" ? "0" : Dr["PPN"].ToString());
                                    dgvPqDetails.Rows[i].Cells[col + 5].Value = (Dr["PPH"].ToString() == "" ? "0" : Dr["PPH"].ToString());
                                    dgvPqDetails.Rows[i].Cells[col + 6].Value = (Dr["Unit"].ToString() == "" ? "0" : Dr["Unit"].ToString());
                                    dgvPqDetails.Rows[i].Cells[col + 7].Value = (Dr["Ratio"].ToString() == "" ? "0" : Dr["Ratio"].ToString());
                                    
                                    DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                                    combo.Items.Add("");
                                    combo.Items.Add("Yes");
                                    combo.Items.Add("No");

                                    if (txtPrType.Text.Trim() != "FIX")
                                    {
                                        if (dgvPqDetails.Rows[i].Cells["Base"].Value.ToString() == "Y")
                                        {
                                            dgvPqDetails.Rows[i].Cells[col + 8] = combo;
                                        }
                                    }
                                    else
                                    {
                                        dgvPqDetails.Rows[i].Cells[col + 8] = combo;
                                    }
                                    
                                    //dgvPqDetails.Rows[i].Cells[col + 8].Value = false;

                                    //Query = "Select (Price+" + (Dr["Price"] == null ? "0" : Dr["Price"].ToString()) + "), " + (Dr["Qty2"] == null ? "0" : Dr["Qty2"].ToString()) + " From PurchQuotation_DtlDtl a where PurchQuotID='" + Dr["PurchQuotID"].ToString() + "' and FullItemId='" + dgvPqDetails.Rows[i].Cells[3].Value.ToString() + "' and DeliveryMethod='" + Dr[5].ToString() + "' order by SeqNoDtl asc, SeqNo asc";// and a.TransStatus=''";
                                    //Cmd = new SqlCommand(Query, Conn);
                                    //SqlDataReader DrDtl;
                                    //DrDtl = Cmd.ExecuteReader();
                                    //while (DrDtl.Read())
                                    //{
                                    //    //this.dgvPqDetails.Rows.Add("true", x++, DrDtl[0], DrDtl[1], DrDtl[2], DrDtl[3]);
                                    //    i++;
                                    //    dgvPqDetails.Rows[i].Cells[col].Value = (DrDtl[0].ToString() == "" ? "0" : DrDtl[0].ToString());
                                    //    //dgvPqDetails.Rows[i].Cells[col + 1].Value = (DrDtl[1].ToString() == "" ? "0" : DrDtl[1].ToString());
                                    //    dgvPqDetails.Rows[i].Cells[col + 1].Value = (DrDtl[1].ToString() == "" ? "0" : DrDtl[1].ToString());
                                    //    dgvPqDetails.Rows[i].Cells[col + 2].Value = "0";
                                    //    dgvPqDetails.Rows[i].Cells[col + 3].Value = "Files";
                                    //    dgvPqDetails.Rows[i].Cells[col + 4].Value = false;
                                    //}
                                }
                                //}
                            }
                            col += 9;
                        }
                    }

                    dgvPqDetails.ReadOnly = false;
                    dgvPqDetails.Columns["No"].ReadOnly = true;
                    dgvPqDetails.Columns["Delivery\nMethod"].ReadOnly = true;
                    dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
                    dgvPqDetails.Columns["ItemName"].ReadOnly = true;
                    dgvPqDetails.Columns["SeqNo"].ReadOnly = true;
                    dgvPqDetails.Columns["PR Qty"].ReadOnly = true;
                    dgvPqDetails.Columns["BracketDesc"].ReadOnly = true;

                    dgvPqDetails.Columns["No"].Frozen = true;
                    dgvPqDetails.Columns["Delivery\nMethod"].Frozen = true;
                    dgvPqDetails.Columns["FullItemID"].Frozen = true;
                    dgvPqDetails.Columns["ItemName"].Frozen = true;
                    dgvPqDetails.Columns["SeqNo"].Frozen = true;
                    dgvPqDetails.Columns["PR Qty"].Frozen = true;
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Vendor") || Columns[i].Contains("Attachment"))
                        {
                            dgvPqDetails.Columns[Columns[i]].ReadOnly = true;
                            //dgvPqDetails.Columns["PR Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    }
                    dgvPqDetails.Rows[0].Frozen = true;

                    dgvPqDetails.AutoResizeColumns();
                    for (int x = 11; x < dgvPqDetails.ColumnCount;x+=9)
                    for (int i = 0; i < dgvPqDetails.RowCount; i++)
                    {
                        if (txtPrType.Text.Trim() != "FIX")
                        {
                            if (dgvPqDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                            {
                                dgvPqDetails.Rows[i].Cells[x-1].ReadOnly = true;
                                //dgvPqDetails.Rows[i].Cells[0].Style.BackColor = Color.LightYellow;
                                //dgvPqDetails.Rows[i].Cells[x + 1].Style.BackColor = Color.LightYellow;
                            }
                            else
                            {
                                //dgvPqDetails.Rows[i].Cells[x - 1].Style.BackColor = Color.LightYellow;
                                //dgvPqDetails.Rows[i].Cells[0].Style.BackColor = Color.LightYellow;
                                //dgvPqDetails.Rows[i].Cells[x + 1].Style.BackColor = Color.LightYellow;
                            }
                        }
                        else
                        {
                            //dgvPqDetails.Rows[i].Cells[x-1].Style.BackColor = Color.LightYellow;
                            //dgvPqDetails.Rows[i].Cells[0].Style.BackColor = Color.LightYellow;
                            //dgvPqDetails.Rows[i].Cells[x + 1].Style.BackColor = Color.LightYellow;
                        }
                    }
                }

                EditColor();
            //}
            //else
            //{
            //    MessageBox.Show("Data Vendor tidak boleh lebih dari 3.");
            //}
        }

        private void btnDeleteQuotation_Click(object sender, EventArgs e)
        {
            if (dgvPqDetails.RowCount > 0)
                if (dgvPqDetails.RowCount > 0 && dgvPqDetails.CurrentCell.ColumnIndex >=8)
                {
                    Index = dgvPqDetails.CurrentRow.Index;
                    int Column = Convert.ToInt32(dgvPqDetails.CurrentCell.ColumnIndex);
                    string[] Vendor = dgvPqDetails.Columns[Column].Name.ToString().Split('\n');

                    //dgvPqDetails.Columns[].Name.ToString();
                    DialogResult dialogResult = MessageBox.Show("Apakah Vendor : " + Vendor[0] + " akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int tmpColumn = ((dgvPqDetails.CurrentCell.ColumnIndex - 8)/9);
                        NotInPurchQuotIdString.RemoveAt(tmpColumn);
                            VendorId.RemoveAt(VendorId.IndexOf(Vendor[0].ToString()));
                        //NotInPurchQuotIdString.RemoveAt(VendorId.IndexOf(Vendor[0].ToString()));
                        PurchQuotId.RemoveAt(PurchQuotId.IndexOf(Vendor[2].ToString()));
                        
                        //Columns.RemoveAt(tmpColumn);
                        //PurchQuotId.RemoveAt(tmpColumn);
                        int min = (tmpColumn * 9) + 8 ;
                        int max = min + 8;

                        for (int i = max; i >= min; i--)
                        {
                            dgvPqDetails.Columns.RemoveAt(i);
                        }

                        //delete Column
                        for (int i = tmpColumn; i <= tmpColumn + 8; i++)
                        {
                            Columns.RemoveAt(0);
                        }
                    }
                }
        }

        private void dgvPqDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name.Contains("Qty"))
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
        }

        private void dgvPqDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(dgvPqDetails_KeyPress);
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {

        }

        private void dgvPqDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex >-1)
            {
                //if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "VendId")
                //{
                //    string TmpListVendor = dgvPqDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                //    string[] SplitVendor = TmpListVendor.Split(';');

                //    for (int i = 0; i < SplitVendor.Count(); i++)
                //    {
                //        PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();
                //        ListVendor.Add(PopUpVendor);
                //        PopUpVendor.GetData(SplitVendor[i].ToString());
                //        PopUpVendor.Y += 100 * i;
                //        PopUpVendor.Show();
                //    }
                //}

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                {

                    PopUp.Stock.Stock PopUpStock = new PopUp.Stock.Stock();
                    PopUpStock.GetData(dgvPqDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    itemID = dgvPqDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                    PopUpStock.Show();
                }
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Attachment"))
                {
                    PopUp.Attachment.Attachment PopUpStock = new PopUp.Attachment.Attachment();
                    string[] dgvHeader = dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Split('\n');
                    PopUpStock.RefreshGrid(dgvHeader[2]);
                    PopUpStock.Show();
                }
            }
        
        }

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvPqDetails_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {

        }

        private void dgvPqDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("Qty"))
            {
                double d = double.Parse(e.Value.ToString());
                dgvPqDetails.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N2");
            }
            if (dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("Ratio") || dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("Price") || dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("PPN") || dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("PPH"))
            {
                double d = double.Parse(e.Value.ToString());
                dgvPqDetails.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N4");
            }
        }
   

    }
}
