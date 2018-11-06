using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Windows.Forms;

namespace ISBS_New.Purchase.CanvasSheet
{
    public partial class FormCanvasSheet2 : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt; private DataSet Ds;

        //string[] SubHeader = { "Price", "Order\nQty" };

        string Mode, Query, crit, CSNumber = null;

        List<string> HitungQtyPrNama = new List<string>();
        List<decimal> HitungQtyPrNilai = new List<decimal>();
        List<decimal> HitungQtyPrOrder = new List<decimal>();

        int Index;
        private int[] daysInMonths;

        public Boolean ApproveStatus = false;

        public string PQNumber = "", tmpPrType = "";

        string flag;

        DateTimePicker dtp;

        List<string> ListSeqNum;
        List<Purchase.PurchaseRequisition.Info> ListInfo = new List<Purchase.PurchaseRequisition.Info>();

        public List<string> VendorId = new List<string>(); //list jumlah vendor
        public List<string> VendorName = new List<string>(); //list nama vendor
        List<string> TmpVendorId = new List<string>(); //list jumlah vendor temporary
        List<string> Columns = new List<string>(); //list jumlah column di datagrid price, pr qty, qty, check
        public List<string> PurchQuotId = new List<string>();
        public string PurchQuotIdString = "";
        public List<string> NotInPurchQuotIdString = new List<string>();
        List<Color> ListColor = new List<Color>();
        //tia12062018
        List<PopUp.Vendor.Vendor> ListVendor = new List<PopUp.Vendor.Vendor>();
        PopUp.FullItemId.FullItemId FullItemID = new PopUp.FullItemId.FullItemId();
        ContextMenu vendid = new ContextMenu();
        //end

        Purchase.CanvasSheet.InquiryCanvasSheet Parent;
       
        Boolean CanvasSheetType = false;
        //tia edit
        ISBS_New.TaskList.Purchase.TaskListCanvasSheet Parent2;
        //edit end
        //Tampung
        string TmpCreatedDate;
        string TmpCreatedBy = "";

        //Update Invent_Purch_Qty variable
        string FullItemId = "";
        string Unit = "";
        string UoM = "";
        decimal ConvRatio = 0;
        string QueryTemp = "";
        decimal QtyUoM = 0;
        decimal QtyAlt = 0;
        decimal QtyInput = 0;
        decimal QtyOld = 0;

        bool GetIn_dgvPqDetails_CellEndEdit = false, dgvPqDetailsClick = false;
 

        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormCanvasSheet2()
        {
            InitializeComponent();
        }

        private void FormCanvasSheet2_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
        }

        public void ModeNew()
        {
            Mode = "New";
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
            Mode = "Edit";
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnAddQuotation.Enabled = true;
            btnDeleteQuotation.Enabled = true;

            //if(txtCSNumber.Text.Trim()!="")
            btnSearchPR.Enabled = false;
            //dgvPqDetails.Enabled = true;
            dgvPqDetails.DefaultCellStyle.BackColor = Color.White;

            dgvPqDetails.ReadOnly = false;
            dgvPqDetails.Columns["No"].ReadOnly = true;
            dgvPqDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPqDetails.Columns["ItemName"].ReadOnly = true;
            dgvPqDetails.Columns["SeqNo"].ReadOnly = true;
            if(txtPrType.Text != "AMOUNT")
                dgvPqDetails.Columns["PR Qty"].ReadOnly = true;
            if (txtPrType.Text == "AMOUNT")
                dgvPqDetails.Columns["PR Amount"].ReadOnly = true;
            dgvPqDetails.Columns["BracketDesc"].ReadOnly = true;
            dgvPqDetails.Columns["Unit"].ReadOnly = true;

            dgvPqDetails.Columns["No"].Frozen = true;
            dgvPqDetails.Columns["Delivery\nMethod"].Frozen = true;
            dgvPqDetails.Columns["FullItemID"].Frozen = true;
            dgvPqDetails.Columns["ItemName"].Frozen = true;
            dgvPqDetails.Columns["SeqNo"].Frozen = true;
            if (txtPrType.Text != "AMOUNT")
                dgvPqDetails.Columns["PR Qty"].Frozen = true;
            else if (txtPrType.Text == "AMOUNT")
                dgvPqDetails.Columns["PR Amount"].Frozen = true;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Vendor") || Columns[i].Contains("Attachment") || Columns[i].Contains("Approval") || Columns[i].Contains("Order Qty") || Columns[i].Contains("Amount Qty"))
                {
                    if (Columns[i].Contains("Approval"))
                    {
                        for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                        {
                            if (dgvPqDetails.Rows[j].Cells["Base"].Value.ToString() == "N")
                            {
                                dgvPqDetails.Rows[j].ReadOnly = true;
                            }
                            if (dgvPqDetails.Rows[j].Cells[i - 7 + 10].Value != null)
                            {
                                if (txtPrType.Text!="AMOUNT" && Decimal.Parse(dgvPqDetails.Rows[j].Cells[i - 7 + 10].Value.ToString()) == 0)
                                {
                                    dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                                }
                                if (txtPrType.Text == "AMOUNT" && Decimal.Parse(dgvPqDetails.Rows[j].Cells[i - 7 + 11].Value.ToString()) == 0)
                                {
                                    dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                                }
                            }
                            if (ApproveStatus == true && Decimal.Parse(dgvPqDetails.Rows[j].Cells[i-6+10].Value.ToString()) == 0)
                            {
                                dgvPqDetails.Rows[j].Cells[i - 6+ 10].ReadOnly = true;
                                dgvPqDetails.Rows[j].Cells[i+10].ReadOnly = true;
                            }
                        }
                    }
                    if (Columns[i].Contains("Order Qty"))
                    {
                        for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                        {
                            if (dgvPqDetails.Rows[j].Cells[i + 10].Value == null || dgvPqDetails.Rows[j].Cells[i + 9].Value == null)
                            {
                                dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                            }
                            //else
                            //{
                            //    if (decimal.Parse(dgvPqDetails.Rows[j].Cells[i + 10].Value.ToString()) == 0)
                            //    {
                            //        dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                            //    }
                            //}
                        }
                    }
                    if (Columns[i].Contains("Amount Qty"))
                    {
                        for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                        {
                            if (dgvPqDetails.Rows[j].Cells[i + 10].Value == null || dgvPqDetails.Rows[j].Cells[i + 9].Value == null)
                            {
                                dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                            }
                            //else
                            //{
                            //    if (decimal.Parse(dgvPqDetails.Rows[j].Cells[i + 10].Value.ToString()) == 0)
                            //    {
                            //        dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                            //    }
                            //}
                        }
                    }
                    if (Columns[i].Contains("Price"))
                        dgvPqDetails.Columns[i + 10].ReadOnly = true;
                    if (Columns[i].Contains("PR"))
                        dgvPqDetails.Columns[i + 10].ReadOnly = true;
                    if (Columns[i].Contains("Vendor"))
                        dgvPqDetails.Columns[i + 10].ReadOnly = true;
                    if (Columns[i].Contains("Attachment"))
                        dgvPqDetails.Columns[i + 10].ReadOnly = true;
                    if (ApproveStatus == true && Columns[i].Contains("Order Qty"))
                    {
                        dgvPqDetails.Columns[i + 10].ReadOnly = true;
                    }
                    if (ApproveStatus == true && Columns[i].Contains("Amount Qty"))
                    {
                        dgvPqDetails.Columns[i + 10].ReadOnly = true;
                    }
                    //dgvPqDetails.Columns[Columns[i]].ReadOnly = true;
                }
            }
            dgvPqDetails.Rows[0].Frozen = true;

            dgvPqDetails.AutoResizeColumns();
            for (int j = 11; j < dgvPqDetails.ColumnCount; j += 9)
            {
                for (int i = 0; i < dgvPqDetails.RowCount; i++)
                {
                    if (txtPrType.Text.Trim() != "FIX")
                    {
                        if (dgvPqDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                        {
                            dgvPqDetails.Rows[i].Cells[j - 1].ReadOnly = true;
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
       //tia edit
        public void ModePopUp(string TmpCSNumber)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }

            txtCSNumber.Text = TmpCSNumber;
            txtPrNumber.Text = "";
            ModePopUp();
        }

        public void ModePopUp()
        {
            GetDataHeader();
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            btnDeleteQuotation.Enabled = false;
            btnAddQuotation.Enabled = false;

            btnSearchPR.Enabled = false;
            
            dgvPqDetails.ReadOnly = true;
            dgvPqDetails.AutoResizeColumns();
            dgvPqDetails.DefaultCellStyle.BackColor = Color.LightGray;
            txtPrNumber.Enabled = true;
            txtPrNumber.ReadOnly = true;
            txtPrNumber.ContextMenu = vendid;
           
            int x = 0;//membaca column
            int i = 10;//start pewarnaan column
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
        public void SetParent2(ISBS_New.TaskList.Purchase.TaskListCanvasSheet F2)
        {
            Parent2 = F2;
        }
        //tia edit end
        public void SetParent(Purchase.CanvasSheet.InquiryCanvasSheet F)
        {
            Parent = F;
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
            //tia edit
            txtPrNumber.Enabled = true;
            txtPrNumber.ReadOnly = true;
            txtPrNumber.ContextMenu = vendid;
            //tia edit end
            int x = 0;//membaca column
            int i = 10;//start pewarnaan column
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
            int i = 10;//start pewarnaan column
            for (int j = 0; j < ListColor.Count; j++)
            {
                if (i + 1 >= dgvPqDetails.ColumnCount)
                {
                    break;
                }
                x = 0;
                while (x < 9)
                {
                    dgvPqDetails.Columns[x + i].DefaultCellStyle.BackColor = ListColor[j];
                    dgvPqDetails.Columns[x + i].HeaderCell.Style.BackColor = ListColor[j];
                    x++;
                }
                i += 9;
            }

            dgvPqDetails.EnableHeadersVisualStyles = false;
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
            //Hendry Revisi : 5 April 2018, ganti informasi yang nampak ketika search 
            //SearchQueryV1 tmpSearch = new SearchQueryV1();
            //tmpSearch.PrimaryKey = "PurchReqID";
            //tmpSearch.Order = "PurchReqID desc";
            //tmpSearch.QuerySearch = "SELECT PR.PurchReqId 'PR Id', PR.OrderDate 'PR Date', PR.TransType 'Type', PQ.PurchQuotID 'PQ Id', PQ.OrderDate 'PQ Date', PQ.VendName 'Vendor Name'  From PurchRequisitionH PR INNER JOIN PurchQuotationH PQ ON PR.PurchReqId=PQ.PurchReqId WHERE PR.PurchReqID NOT IN(SELECT DISTINCT PurchReqId FROM CanvasSheetH)";
            //tmpSearch.FilterText = new string[] { "PR Id", "Type" };
            //tmpSearch.Select = new string[] { "PR Id", "Type" };
            //tmpSearch.ShowDialog();
            string TmpCSNumber = txtCSNumber.Text;
            
            ////before 31/08/2018
            //ControlMgr.TblName = "PurchRequisitionH PR INNER JOIN PurchQuotationH PQ ON PR.PurchReqId=PQ.PurchReqId";
            //Methods.ControlMgr.tmpWhere = "WHERE PR.PurchReqID NOT IN(SELECT DISTINCT PurchReqId FROM CanvasSheetH)";
            //ControlMgr.tmpSort = "ORDER BY PR.OrderDate DESC";
            //Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();
            //FrmSearch.Text = "Search PR";
            //FrmSearch.ShowDialog();

            ////STV ganti search 31/08/2018
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "PRNo";
            tmpSearch.Order = "PRDate DESC";
            tmpSearch.Table = "[dbo].[PurchRequisitionH]";
            tmpSearch.QuerySearch = "SELECT a.[PurchReqId] AS PRNo, a.[OrderDate] AS PRDate, a.[TransType] AS PRType, b.[PurchQuotID] AS PQNo, b.[OrderDate] AS PQDate, b.[VendName] FROM PurchRequisitionH a ";
            tmpSearch.QuerySearch += "INNER JOIN PurchQuotationH b ON a.PurchReqId = b.PurchReqId ";
            tmpSearch.QuerySearch += "WHERE a.PurchReqID NOT IN(SELECT DISTINCT PurchReqId FROM CanvasSheetH WHERE TransStatus <> '04') AND b.TransStatus <> '01'";
            tmpSearch.FilterText = new string[] { "PRNo", "PRDate", "PRType", "PQNo", "PQDate", "VendName" };
            tmpSearch.Mask = new string[] { "PR No", "PR Date", "PR Type", "PQ No", "PQ Date", "Vendor" };
            tmpSearch.Select = new string[] { "PRNo", "PRType" };
            tmpSearch.ShowDialog();            

            if (ConnectionString.Kodes != null)
            {
                txtPrNumber.Text = ConnectionString.Kodes[0];
                txtPrType.Text = ConnectionString.Kodes[1];

                if (TmpCSNumber != txtPrNumber.Text)
                {
                    VendorId = new List<string>();
                    Columns = new List<string>();
                }

                if (dgvPqDetails.Rows.Count >= 1)
                {
                    dgvPqDetails.Rows.Clear();
                    dgvPqDetails.Columns.Clear();
                    //VendorId.Clear();
                }

                dgvPqDetails.ColumnCount = 10;
                dgvPqDetails.Columns[0].Name = "No";
                dgvPqDetails.Columns[1].Name = "Delivery\nMethod";
                dgvPqDetails.Columns[2].Name = "FullItemID";
                dgvPqDetails.Columns[3].Name = "ItemName";
                dgvPqDetails.Columns[4].Name = "SeqNo";
                dgvPqDetails.Columns[5].Name = "Base";
                dgvPqDetails.Columns[6].Name = "SeqNoGroup";
                if (txtPrType.Text != "AMOUNT")
                {
                    dgvPqDetails.Columns[7].Name = "PR Qty";
                }
                else if (txtPrType.Text == "AMOUNT")
                {
                    dgvPqDetails.Columns[7].Name = "PR Amount";
                }
                dgvPqDetails.Columns[8].Name = "BracketDesc";
                dgvPqDetails.Columns[9].Name = "Unit";

                Conn = ConnectionString.GetConnection();

                //List Item PR
                if (txtPrType.Text != "AMOUNT")
                {
                    Query = "Select distinct a.DeliveryMethod, a.FullItemID, a.ItemName, a.ReffSeqNo, a.Base, b.SeqNoGroup,b.[Qty],a.BracketDesc, a.Unit From PurchQuotation_Dtl a left join PurchRequisition_Dtl b on b.PurchReqID=a.ReffTransID and a.ReffSeqNo=b.SeqNo where a.ReffTransID='" + txtPrNumber.Text + "' order by b.SeqNoGroup asc, a.ReffSeqNo asc, a.Base desc";
                }
                else if (txtPrType.Text == "AMOUNT")
                {
                    Query = "Select distinct a.DeliveryMethod, a.FullItemID, a.ItemName, a.ReffSeqNo, a.Base, b.SeqNoGroup,b.[Amount],a.BracketDesc, a.Unit From PurchQuotation_Dtl a left join PurchRequisition_Dtl b on b.PurchReqID=a.ReffTransID and a.ReffSeqNo=b.SeqNo where a.ReffTransID='" + txtPrNumber.Text + "' order by b.SeqNoGroup asc, a.ReffSeqNo asc, a.Base desc";
                }
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int x = 1;
                int z = 0;

                while (Dr.Read())
                {
                    //int TmpIndexArray = -1;
                    //if (HitungQtyPrNama != null)
                    //{
                    //    TmpIndexArray = HitungQtyPrNama.Contains(Dr["FullItemID"].ToString() + Dr["ReffSeqNo"].ToString());
                    //}
                    if (HitungQtyPrNama.Contains(Dr["SeqNoGroup"].ToString()) == false)
                    {
                        HitungQtyPrNama.Add(Dr["SeqNoGroup"].ToString());
                        HitungQtyPrOrder.Add(0);
                        HitungQtyPrNilai.Add(0);//Convert.ToDecimal(Dr["Qty"]);
                    }
                    this.dgvPqDetails.Rows.Add(x++, Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8]);
                    if (Dr[4].ToString() == "N")
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
                if (txtPrType.Text != "AMOUNT")
                {
                    dgvPqDetails.Columns["PR Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                if (txtPrType.Text == "AMOUNT")
                {
                    dgvPqDetails.Columns["PR Amount"].ReadOnly = true;
                    dgvPqDetails.Columns["PR Amount"].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                dgvPqDetails.Columns["BracketDesc"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPqDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                //dgvPqDetails.Columns["PR Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvPqDetails.AutoResizeColumns();
                Conn.Close();  
            }
            
            //if (ControlMgr.Kode != null)
            //{
                          
            //}

            //ControlMgr.TblName = "";
            //ControlMgr.tmpSort = "";
            //Methods.ControlMgr.tmpWhere = "";
            //ControlMgr.Kode = "";
            //ControlMgr.Kode2 = "";
        }

        private void saveNewCS()
        {
            #region Create New CS Number
            string Jenis = "CS", Kode = "CS";
            CSNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);
            Query = "Insert into [CanvasSheetH] (CanvasId,CanvasDate,PurchReqId,TransType,TransStatus,CreatedDate,CreatedBy) values( ";
            Query += "@csnum, @csdate, @prnum, (select top 1 TransType from [PurchRequisitionH] a where a.PurchReqId=@prnum),";
            Query += "'01', getdate(), @crby);";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@csnum", CSNumber);
            Cmd.Parameters.AddWithValue("@csdate", dtCanvasDate.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@prnum", txtPrNumber.Text);
            Cmd.Parameters.AddWithValue("@crby", ControlMgr.UserId);
            Cmd.ExecuteNonQuery();
            #endregion

            Query = "";
            int CountGel = 1;

            #region Add CS Issued
            for (int i = 0; i < VendorId.Count; i++)
            {
                for (int j = 0; j <= dgvPqDetails.RowCount - 1; j++)
                {
                    FullItemId = dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString();

                    #region Insert CanvasSheetD
                    if (txtPrType.Text != "AMOUNT")
                    {
                        Query = "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,FullItemId,ItemName,PurchReqSeqNo,Base,SeqNoGroup,PRQty,BracketDesc,Price,PQQty,Qty,VendID,PPN,PPH,Unit,Ratio,StatusApproval,CreatedDate,CreatedBy) Values ";
                    }
                    else if (txtPrType.Text == "AMOUNT")
                    {
                        Query = "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,FullItemId,ItemName,PurchReqSeqNo,Base,SeqNoGroup,PRAmount,BracketDesc,Price,PQQty,CSAmount,VendID,PPN,PPH,Unit,Ratio,StatusApproval,CreatedDate,CreatedBy) Values ";
                    }
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
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value.ToString()) + "','";
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value.ToString()) + "','";
                    //QtyInput = decimal.Parse(dgvPqDetails.Rows[j].Cells["PR Qty"].Value.ToString());
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value.ToString()) + "','";

                    Query += VendorId[i] + "','";
                    //Query += dgvPqDetails.Rows[j].Cells[0].Value.ToString() == null ? "false'," : dgvPqDetails.Rows[j].Cells[0].Value.ToString() + "','";// "false','"; //dgvPqDetails.Rows[j].Cells["By Row"].Value.ToString() + "','";
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value.ToString()) + "','";//"false',"; //dgvPqDetails.Rows[j].Cells[Columns[i + 2]].Value.ToString() + "','";

                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value.ToString()) + "','";

                    //Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value == null ? "Null','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value.ToString() + "','";
                    // (i * 9) + 16 diganti menjadi 9 (Unit)
                    Query += dgvPqDetails.Rows[j].Cells[9].Value == null ? "Null','" : dgvPqDetails.Rows[j].Cells[9].Value.ToString() + "','";

                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString()) + "','";
                    //Query += dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value.ToString() + "','";
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value + "',";
                    Query += "getdate(),'" + ControlMgr.UserId + "');";

                    string TmpStatus = dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value == null ? "" : dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString();
                    decimal TmpQty = dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value == null ? 0 : Convert.ToDecimal(dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value);
                    string TmpFullItemId = dgvPqDetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString();
                    string Unit = dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value == null ? "" : dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value.ToString();
                    int PurchReqSeqNo = dgvPqDetails.Rows[j].Cells[4].Value == null ? 0 : Convert.ToInt16(dgvPqDetails.Rows[j].Cells[4].Value);
                    decimal TmpRatio = dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value == null ? 0 : Decimal.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString());
                    string POUnit = Unit;
                    //if (false == ListMethod1.CSIssued1TM(Conn, Trans, txtCSNumber.Text.Trim(), (j + 1).ToString(), TmpQty, Unit, TmpFullItemId, PurchReqSeqNo))
                    //{
                    //    Trans.Rollback();
                    //    MessageBox.Show(ListMethod1.MessageBox.ToString());
                    //    return;
                    //}

                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    #endregion

                    #region Update Invent_Purchase_Qty

                    if (txtPrType.Text != "AMOUNT")
                    {
                        FullItemId = dgvPqDetails.Rows[j].Cells["FullItemID"].Value.ToString();

                        #region NOT AMOUNT
                        //QtyInput = decimal.Parse(dgvPqDetails.Rows[j].Cells["Order Qty"].Value.ToString());
                        //QtyInput = decimal.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value == null ? "0" : dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value.ToString());
                        
                        Unit = dgvPqDetails.Rows[j].Cells["Unit"].Value.ToString();

                        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        UoM = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();

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

                        #endregion Add CS Issued

                        #region Insert CanvassSheets_LogTable
                        Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Qty_UoM],[Qty_Alt],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                        Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + CSNumber + "','" + PurchReqSeqNo + "', '" + QtyUoM + "', '" + QtyAlt + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "')";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        #endregion
                    #endregion
                    }else{
                        #region Insert CanvassSheets_LogTable
                        Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                        Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + CSNumber + "','" + PurchReqSeqNo + "', '" + TmpQty + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "')";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        #endregion
                    }
            #endregion
                }
            }

            if (txtPrType.Text != "AMOUNT")
            {
                Query = "EXEC [dbo].[update_purchaseqty_cs] '" + CSNumber + "' ; ";
            }
            else if (txtPrType.Text == "AMOUNT")
            {
                Query = "EXEC [dbo].[update_purchaseqty_csa] '" + CSNumber + "' ; ";
            }

            #region Set TransStatus PurchQuotationH
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            Query = "";
            for (int i = 0; i < PurchQuotId.Count; i++)
            {
                Query += "Update PurchQuotationH set TransStatus='01' where PurchQuotID='" + PurchQuotId[i].ToString() + "';";
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            #endregion


            MessageBox.Show("Data CanvasSheetNumber : " + CSNumber + " berhasil ditambahkan.");
            txtCSNumber.Text = CSNumber;
        }

        private void saveEditCS()
        {
            #region Delete Old CanvassSheets_LogTable
            //Query = "Delete From [CanvassSheets_LogTable] Where [CSID]='txtCSNumber.Text';";
            //Cmd = new SqlCommand(Query, Conn);
            //Cmd.ExecuteNonQuery();
            #endregion
            string PrType = txtPrType.Text;
            string csid = txtCSNumber.Text.Trim();

            #region balikkin qty ke prapproved2
            if (ApproveStatus == true)
            {
                if (PrType != "AMOUNT")
                {
                    Query = "EXEC [dbo].[empty_cs_pr] '" + csid + "' ; ";
                }
                else
                {
                    Query = "EXEC [dbo].[empty_cs_pra] '" + csid + "' ; ";
                }
            }
            else
            {
                if (PrType != "AMOUNT")
                {
                    Query = "EXEC [dbo].[reverse_purchaseqty_cs] '" + csid + "' ; ";
                }
                else
                {
                    Query = "EXEC [dbo].[reverse_purchaseqty_csa] '" + csid + "' ; ";
                }
            }
            
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            #endregion

            #region Update CanvasSheetH
            Query = "Update [CanvasSheetH] ";
            Query += " set CanvasDate='" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "',";
            Query += " PurchReqId='" + txtPrNumber.Text.Trim() + "',";
            Query += " TransType=(select top 1 TransType from [CanvasSheetH] a where a.PurchReqId='" + txtPrNumber.Text.Trim() + "'),";
            if (ApproveStatus == true)
            {
                if (Yes > 0)
                {
                    Query += "TransStatus='02',";
                }
                else
                {
                    Query += "TransStatus='03',";
                }
            }
            else
            {
                Query += "TransStatus='01',";
            }
            Query += " UpdatedDate=getdate(),UpdatedBy='" + ControlMgr.UserId + "' where CanvasId='" + txtCSNumber.Text.Trim() + "';";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            #endregion

            //Update Invent_Purchase_Qty RO
            for (int i = 0; i < VendorId.Count; i++)
            {
                #region Variable
                string FullItemId = "";
                string Unit = "";
                string UoM = "";
                decimal Qty = 0;
                decimal QtyUoM = 0;
                decimal QtyAlt = 0;
                decimal QtyAmount = 0;
                decimal QtyUoMOld = 0;
                decimal QtyAltOld = 0;
                decimal ConvRatio = 0;
                string QueryTemp = "";
                decimal QtyOld = 0;
                string UnitOld = "";
                int JumlahCSD = 0;
                int JumlahRow = 0;

                #endregion

                Query = "SELECT COUNT (CanvasId) FROM [ISBS-NEW4].[dbo].[CanvasSheetD] WHERE [CanvasId]='" + txtCSNumber.Text + "' and VendID = '" + VendorId[i] + "'";
                Cmd = new SqlCommand(Query, Conn);
                JumlahCSD = (int)Cmd.ExecuteScalar();
                JumlahRow = (int)dgvPqDetails.RowCount;

                if (txtPrType.Text != "AMOUNT")
                {
                    #region NOT AMOUNT
                    for (int j = 1; j <= JumlahCSD; j++)
                    {
                        #region Delete Old InventPurchQty based on CS Detail
                        QueryTemp = "Select [FullItemId] From [CanvasSheetD] Where CanvasId = '" + txtCSNumber.Text + "' and VendID = '" + VendorId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        FullItemId = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select [Unit] From [CanvasSheetD] Where CanvasId = '" + txtCSNumber.Text + "' and VendID = '" + VendorId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        UnitOld = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select [Qty] From [CanvasSheetD] Where CanvasId = '" + txtCSNumber.Text + "' and VendID = '" + VendorId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        QtyOld = (decimal)Cmd.ExecuteScalar();

                        ConvRatio = 0;
                        QueryTemp = "Select [Ratio] From [InventConversion] Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();

                        UoM = "";
                        QueryTemp = "Select [UoM] From [InventTable] Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        UoM = Cmd.ExecuteScalar().ToString();

                        QtyUoMOld = 0;
                        QtyAltOld = 0;
                        if (UnitOld == UoM)
                        {
                            QtyUoMOld = QtyOld;
                            QtyAltOld = QtyOld * ConvRatio;
                        }
                        else
                        {
                            QtyAltOld = QtyOld;
                            QtyUoMOld = QtyOld / ConvRatio;
                        }

                        
                        //MessageBox.Show(VendorId[i] + " urutan " + j + Environment.NewLine + QtyUoMOld + Environment.NewLine + QtyAltOld);
                        #endregion
                    }

                    for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                    {
                        #region Add yang baru berdasarkan Grid
                        Qty = dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value == null ? 0 : Convert.ToDecimal(dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value);
                        FullItemId = dgvPqDetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString();
                        Unit = dgvPqDetails.Rows[j].Cells["Unit"].Value == null ? "" : dgvPqDetails.Rows[j].Cells["Unit"].Value.ToString();
                        //string POUnit = Unit;
                        int PurchReqSeqNo = dgvPqDetails.Rows[j].Cells[4].Value == null ? 0 : Convert.ToInt16(dgvPqDetails.Rows[j].Cells[4].Value);
                        string Status = dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value == null ? "" : dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString();

                        Query = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        UoM = Cmd.ExecuteScalar().ToString();

                        Query = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();

                        Query = "Select Qty From CanvasSheetD Where CanvasId='" + txtCSNumber.Text + "' and FullItemID = '" + FullItemId + "' and DeliveryMethod = '" + dgvPqDetails.Rows[j].Cells["Delivery\nMethod"].Value.ToString() + "' and PurchReqSeqNo ='" + dgvPqDetails.Rows[j].Cells[6].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn);
                        QtyOld = Convert.ToDecimal(Cmd.ExecuteScalar());

                        if (Unit == UoM)
                        {
                            QtyUoM = Convert.ToDecimal(Qty);
                            QtyAlt = Convert.ToDecimal(Qty) * ConvRatio;
                        }
                        else
                        {
                            QtyAlt = Convert.ToDecimal(Qty);
                            QtyUoM = Convert.ToDecimal(Qty) / ConvRatio;
                        }

                        #region UpdateInvent_Purchase_Qty
                        if (ApproveStatus == false)
                        {
                            #region Insert into [CanvassSheets_LogTable]
                            Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Qty_UoM],[Qty_Alt],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                            Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + txtCSNumber.Text + "','" + PurchReqSeqNo + "', '" + QtyUoM + "', '" + QtyAlt + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "')";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                            #endregion

                            
                        }

                        if (ApproveStatus == true && txtTransStatusCode.Text == "01")
                        {
                            if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString() == "Yes")
                            {
                                #region Insert into [CanvassSheets_LogTable]
                                Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Qty_UoM],[Qty_Alt],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                                Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + txtCSNumber.Text + "','" + PurchReqSeqNo + "', '" + QtyUoM + "', '" + QtyAlt + "', '02' ,'Approved' ,'Approved By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "')";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                                #endregion

                                   //Query += "Update Invent_Purchase_Qty Set PR_CS_Issued_UoM = (PR_CS_Issued_UoM - " + QtyUoM + "), PR_CS_Issued_Alt = (PR_CS_Issued_Alt - " + QtyAlt + ")  Where FullItemID = '" + FullItemId + "';";
                            }
                            else
                            {
                                #region Insert into [CanvassSheets_LogTable]
                                Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Qty_UoM],[Qty_Alt],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                                Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + txtCSNumber.Text + "','" + PurchReqSeqNo + "', '" + QtyUoM + "', '" + QtyAlt + "', '03' ,'Rejected' ,'Rejected By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "')";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                                #endregion

                                //Query = "Update Invent_Purchase_Qty Set PR_CS_Approved_UoM = (PR_CS_Approved_UoM - " + QtyUoM + "), PR_CS_Approved_Alt = (PR_Approved_Alt - " + QtyAlt + ")  Where FullItemID = '" + FullItemId + "';";
                            }
                        }

                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                        #endregion
                        #endregion
                    }
                    #endregion
                }

                if (txtPrType.Text == "AMOUNT")
                {
                    #region AMOUNT

                    for (int j = 1; j < JumlahCSD; j++)
                    {
                        QueryTemp = "Select [FullItemId] From [CanvasSheetD] Where CanvasId = '" + txtCSNumber.Text + "' and VendID = '" + VendorId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        FullItemId = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select [CSAmount] From [CanvasSheetD] Where CanvasId = '" + txtCSNumber.Text + "' and VendID = '" + VendorId[i] + "' and [CanvasSeqNo] = '" + j + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        QtyOld = (decimal)Cmd.ExecuteScalar();

                        //Query = "Update Invent_Purchase_Qty Set [PR_Approved2_Amount] = (PR_Approved2_Amount + " + QtyOld + "), ";
                        //Query += " [PR_CS_Issued_Amount] = (PR_CS_Issued_Amount - " + QtyOld + ")";
                        //Query += " Where FullItemID = '" + FullItemId + "'";

                        
                    }

                    for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                    {
                        #region Add data based on Grid
                        QtyAmount = dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value == null ? 0 : Convert.ToDecimal(dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value);
                        FullItemId = dgvPqDetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString();
                        string Status = dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value == null ? "" : dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString();
                        int PurchReqSeqNo = dgvPqDetails.Rows[j].Cells[4].Value == null ? 0 : Convert.ToInt16(dgvPqDetails.Rows[j].Cells[4].Value);

                        if (ApproveStatus == false)
                        {
                            #region Insert CanvassSheets_LogTable
                            Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                            Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + txtCSNumber.Text + "','" + PurchReqSeqNo + "', '" + QtyAmount + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "');";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                            #endregion

                            //Query += "Update Invent_Purchase_Qty Set PR_Approved2_Amount = (PR_Approved2_Amount - " + QtyAmount + ") Where FullItemID = '" + FullItemId + "';";
                        }
                        if (ApproveStatus == true && txtTransStatusCode.Text == "01")
                        {
                            if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString() == "Yes")
                            {
                                #region Insert CanvassSheets_LogTable
                                Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                                Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + txtCSNumber.Text + "','" + PurchReqSeqNo + "', '" + QtyAmount + "', '02' ,'Approved' ,'Approved By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "');";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                                #endregion

                                //Query += "Update Invent_Purchase_Qty Set PR_CS_Issued_Amount = (PR_CS_Issued_Amount - " + QtyAmount + ") Where FullItemID = '" + FullItemId + "';";
                            }
                            else
                            {
                                #region Insert CanvassSheets_LogTable
                                Query = "Insert into [CanvassSheets_LogTable] ([CSDate],[CSID],[PurchReqSeqNo],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[CanvasSeqNo]) ";
                                Query += "VALUES('" + dtCanvasDate.Value.ToString("yyyy-MM-dd") + "','" + txtCSNumber.Text + "','" + PurchReqSeqNo + "', '" + QtyAmount + "', '03' ,'Rejected.' ,'Rejected By Purchase Manager', '" + ControlMgr.UserId + "', getdate(),'" + (j + 1) + "');";
                                Cmd = new SqlCommand(Query, Conn);
                                Cmd.ExecuteNonQuery();
                                #endregion

                                //Query = "Update Invent_Purchase_Qty Set PR_CS_Approved_Amount = (PR_CS_Approved_Amount - " + QtyAmount + ") Where FullItemID = '" + FullItemId + "';";
                            }
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        #endregion
                    }
                    #endregion
                }
            }
            

            #region Delete From CanvasSheetD & CanvasSheetD_Dtl
            Query = "Delete From CanvasSheetD where CanvasId='" + txtCSNumber.Text.Trim() + "';";
            Query += "Delete From CanvasSheetD_Dtl where CanvasId='" + txtCSNumber.Text.Trim() + "';";

            int CountGel = 1;
            for (int i = 0; i < VendorId.Count; i++)
            {
                for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                {
                    if (txtPrType.Text != "AMOUNT")
                    {
                        Query += "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,FullItemId,ItemName,PurchReqSeqNo,Base,SeqNoGroup,PRQty,BracketDesc,Price,PQQty,Qty,VendID,PPN,PPH,Unit,Ratio,StatusApproval,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) Values ";
                    }
                    else if (txtPrType.Text == "AMOUNT")
                    {
                        Query += "Insert CanvasSheetD (CanvasId,CanvasSeqNo,DeliveryMethod,PurchQuotId,PurchReqId,FullItemId,ItemName,PurchReqSeqNo,Base,SeqNoGroup,PRAmount,BracketDesc,Price,PQQty,CSAmount,VendID,PPN,PPH,Unit,Ratio,StatusApproval,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) Values ";
                    }
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
                    Query += dgvPqDetails.Rows[j].Cells[7].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[7].Value.ToString().Replace(",", "") + "','";
                    Query += dgvPqDetails.Rows[j].Cells[8].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[8].Value.ToString() + "','";
                    //(i * 9) digunakan untuk jeda setiap ganti vendor 
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 10].Value.ToString()) + "','";
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value.ToString()) + "','";
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 12].Value.ToString()) + "','";
                    Query += VendorId[i] + "','";
                    //Query += dgvPqDetails.Rows[j].Cells[0].Value.ToString() == null ? "false'," : dgvPqDetails.Rows[j].Cells[0].Value.ToString() + "','";// "false','"; //dgvPqDetails.Rows[j].Cells["By Row"].Value.ToString() + "','";
                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 14].Value.ToString()) + "','";//"false',"; //dgvPqDetails.Rows[j].Cells[Columns[i + 2]].Value.ToString() + "','";

                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value.ToString()) + "','";

                    //Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value == null ? "Null','" : dgvPqDetails.Rows[j].Cells[(i * 9) + 16].Value.ToString() + "','";
                    // (i * 9) + 16 diganti menjadi 9 (Unit)
                    Query += dgvPqDetails.Rows[j].Cells[9].Value == null ? "Null','" : dgvPqDetails.Rows[j].Cells[9].Value.ToString() + "','";

                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value == null ? "0','" : Double.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString()) + "','";
                    //Query += dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value == null ? "0','" : dgvPqDetails.Rows[j].Cells[(i * 7) + 17].Value.ToString() + "','";

                    Query += dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value + "','";
                    Query += TmpCreatedDate + "','" + TmpCreatedBy + "',"; //TmpCreatedDate.ToString("yyyy-MM-dd HH:mm:ss")
                    Query += "getdate(),'" + ControlMgr.UserId + "');";

                    //string TmpStatus = dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value == null ? "" : dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString();
                    decimal TmpQty = dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value == null ? 0 : Convert.ToDecimal(dgvPqDetails.Rows[j].Cells[(i * 9) + 11].Value);
                    //string TmpFullItemId = dgvPqDetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvPqDetails.Rows[j].Cells["FullItemId"].Value.ToString();
                    string Unit = dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value == null ? "" : dgvPqDetails.Rows[j].Cells[(i * 9) + 15].Value.ToString();
                    int PurchReqSeqNo = dgvPqDetails.Rows[j].Cells[4].Value == null ? 0 : Convert.ToInt16(dgvPqDetails.Rows[j].Cells[4].Value);
                    decimal TmpRatio = dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value == null ? 1 : decimal.Parse(dgvPqDetails.Rows[j].Cells[(i * 9) + 17].Value.ToString());
                    //string POUnit = Unit;

                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                }
            }

            if (ApproveStatus == true)
            {
                if (PrType != "AMOUNT")
                {
                    Query = "EXEC [dbo].[update_csappr] '" + csid + "' ; ";
                }
                else
                {
                    Query = "EXEC [dbo].[update_csappra] '" + csid + "' ; ";
                }
            }
            else
            {
                if (PrType != "AMOUNT")
                {
                    Query = "EXEC [dbo].[update_purchaseqty_cs] '" + csid + "' ; ";
                }
                else
                {
                    Query = "EXEC [dbo].[update_purchaseqty_csa] '" + csid + "' ; ";
                }
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            
            #endregion

            #region Update TransStatus PurchQuotationH
            Query = "";
            for (int i = 0; i < PurchQuotId.Count; i++)
            {
                Query += "Update PurchQuotationH set TransStatus='01' where PurchQuotID='" + PurchQuotId[i].ToString() + "';";
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            #endregion

            MessageBox.Show("Data CanvasSheetNumber : " + txtCSNumber.Text.Trim() + " berhasil diupdate.");
            txtCSNumber.Text = txtCSNumber.Text.Trim();
            //Purchase.CanvasSheet.InquiryCanvasSheet f = new Purchase.CanvasSheet.InquiryCanvasSheet();
            //f.RefreshGrid();    
        }

        int CountItem = 0, Yes = 0, No = 0, TmpNull = 0;

        private void btnSave_Click(object sender, EventArgs e)
        {
            //try
            //{
                #region CountItem Yes No
                CountItem = 0; Yes = 0; No = 0; TmpNull = 0;
                if (ApproveStatus == true)
                {
                    //begin
                    //updated by : joshua
                    //updated date : 26 feb 2018
                    //description : check permission access
                    if (this.PermissionAccess(ControlMgr.Approve) > 0)
                    {
                        for (int i = 0; i < VendorId.Count; i++)
                        {
                            for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                            {
                                if (txtPrType.Text != "FIX")
                                {
                                    if (dgvPqDetails.Rows[j].Cells["BASE"].Value.ToString() == "Y" || dgvPqDetails.Rows[j].Cells["BASE"].Value.ToString() == "")
                                    {
                                        CountItem++;
                                        if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value != null)
                                        {
                                            if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString() == "Yes")
                                            {
                                                Yes++;
                                            }
                                            else if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString() == "No")
                                            {
                                                No++;
                                            }
                                        }
                                        else
                                        {
                                            TmpNull++;
                                        }
                                    }
                                }
                                else
                                {
                                    CountItem++;
                                    if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value != null)
                                    {
                                        if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString() == "Yes")
                                        {
                                            Yes++;
                                        }
                                        else if (dgvPqDetails.Rows[j].Cells[(i * 9) + 18].Value.ToString() == "No")
                                        {
                                            No++;
                                        }
                                    }
                                    else
                                    {
                                        TmpNull++;
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
                    else
                    {
                        MessageBox.Show(ControlMgr.PermissionDenied);
                    }
                    //end                     
                }
                #endregion

                if (dgvPqDetails.ColumnCount <=10)
                {
                    MessageBox.Show("Quotation harus dipilih.");
                    goto Outer;
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    ListMethod ListMethod1 = new ListMethod();
                    Conn = ConnectionString.GetConnection();
                    string CSNumber = "";

                    if (txtCSNumber.Text.Trim() == "")
                    {
                        saveNewCS();
                    }
                    else
                    {
                        saveEditCS();
                    }
                    scope.Complete();
                }
                

                #region Set TransStatus CanvasSheetH
                Conn = ConnectionString.GetConnection();
                Query = "select count(*) from [dbo].[CanvasSheetH] where [TransStatus] = '01'";
                Cmd = new SqlCommand(Query, Conn);
                #endregion

                #region Refresh dan Ganti Mode
                Purchase.CanvasSheet.InquiryCanvasSheet.CountCS = Int32.Parse(Cmd.ExecuteScalar().ToString());
                //GetDataHeader();
                Parent.RefreshGrid();
                EditColor();
                ModeBeforeEdit();
                GetDataHeader();
                #endregion
            Outer: ;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        public void GetDataHeader()
        {
            if (txtCSNumber.Text.Trim() != "")
            {
                Conn = ConnectionString.GetConnection();
                Query = "SELECT a.[CanvasId],a.[CanvasDate],a.[PurchReqId],a.[TransType],a.[TransStatus], a.StatusApproval,b.[Deskripsi],a.CreatedDate,a.CreatedBy FROM [dbo].[CanvasSheetH] a left join TransStatusTable b on a.TransStatus=b.StatusCode and b.TransCode ='CanvasSheet' where [CanvasId]='" + txtCSNumber.Text.Trim() + "'";
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
                    TmpCreatedDate = Dr["CreatedDate"].ToString();
                    TmpCreatedBy = Dr["CreatedBy"].ToString();
                       
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

                dgvPqDetails.ColumnCount = 10;
                dgvPqDetails.Columns[0].Name = "No"; dgvPqDetails.Columns[0].HeaderText = "No";
                dgvPqDetails.Columns[1].Name = "Delivery\nMethod"; dgvPqDetails.Columns[1].HeaderText = "Delivery Method";
                dgvPqDetails.Columns[2].Name = "FullItemID"; dgvPqDetails.Columns[2].HeaderText = "Item ID";
                dgvPqDetails.Columns[3].Name = "ItemName"; dgvPqDetails.Columns[3].HeaderText = "Item Name";
                dgvPqDetails.Columns[4].Name = "SeqNo"; dgvPqDetails.Columns[4].HeaderText = "Seq No";
                dgvPqDetails.Columns[5].Name = "Base"; dgvPqDetails.Columns[5].HeaderText = "Base";
                dgvPqDetails.Columns[6].Name = "SeqNoGroup"; dgvPqDetails.Columns[6].HeaderText = "Seq Group No";
                if (txtPrType.Text != "AMOUNT")
                {   
                    dgvPqDetails.Columns[7].Name = "PR Qty";
                    dgvPqDetails.Columns[7].HeaderText = "PR Qty";
                }
                if (txtPrType.Text == "AMOUNT")
                {
                    dgvPqDetails.Columns[7].Name = "PR Amount";
                    dgvPqDetails.Columns[7].HeaderText = "PR Amount";
                }
                dgvPqDetails.Columns[8].Name = "BracketDesc"; dgvPqDetails.Columns[8].HeaderText = "Bracket Desccription";
                dgvPqDetails.Columns[9].Name = "Unit"; dgvPqDetails.Columns[9].HeaderText = "Unit";

                Conn = ConnectionString.GetConnection();

                //List Item PR
                //ROW_NUMBER() OVER (ORDER BY PurchReqSeqNo) No
                if (txtPrType.Text != "AMOUNT")
                {
                    Query = "Select a.CanvasSeqNo, DeliveryMethod, a.FullItemID, a.ItemName, a.PurchReqSeqNo, a.Base, a.SeqNoGroup, a.PRQty, a.BracketDesc, a.Unit From (Select distinct DeliveryMethod,FullItemID, ItemName, PurchReqSeqNo, CanvasSeqNo, Base, SeqNoGroup, PRQty, BracketDesc, Unit From CanvasSheetD a where CanvasId='" + txtCSNumber.Text.Trim() + "') a  order by CanvasSeqNo asc";// and a.TransStatus=''";
                }
                else if (txtPrType.Text == "AMOUNT")
                {
                    Query = "Select a.CanvasSeqNo, DeliveryMethod, a.FullItemID, a.ItemName, a.PurchReqSeqNo, a.Base, a.SeqNoGroup, ISNULL(a.PRAmount, 0) as PRAmount, a.BracketDesc, a.Unit From (Select distinct DeliveryMethod,FullItemID, ItemName, PurchReqSeqNo, CanvasSeqNo, Base, SeqNoGroup, PRAmount, BracketDesc, Unit From CanvasSheetD a where CanvasId='" + txtCSNumber.Text.Trim() + "') a  order by CanvasSeqNo asc";// and a.TransStatus=''";
                }
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int x = 1;
                int z = 0;

                //HitungQtyPrNama = new string[100];
                //HitungQtyPrNilai = new decimal[100];
                while (Dr.Read())
                {
                    //int TmpIndexArray = -1;
                    //if (HitungQtyPrNama != null)
                    //{
                    //    TmpIndexArray = Array.IndexOf(HitungQtyPrNama, Equals(Dr["FullItemID"].ToString() + Dr["PurchReqSeqNo"].ToString()));
                    //}
                    if (HitungQtyPrNama.Contains(Dr["SeqNoGroup"].ToString()) == false)
                    {
                        HitungQtyPrNama.Add(Dr["SeqNoGroup"].ToString());
                        HitungQtyPrOrder.Add(0);
                        HitungQtyPrNilai.Add(0);//Convert.ToDecimal(Dr["PRQty"]);
                        z++;
                    }
                    this.dgvPqDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9]);
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
                Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId From PurchQuotation_Dtl a inner join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID inner join CanvasSheetD c on c.VendId=b.VendId and c.PurchQuotId=b.PurchQuotID where ReffTransID='" + txtPrNumber.Text + "' and c.CanvasId='" + txtCSNumber.Text.Trim() + "'";// and a.TransStatus=''";
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

                    dgvPqDetails.Columns.Add("Price", "Price");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice").ToString());
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].ReadOnly = true;
                    dgvPqDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;


                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty");
                    //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty").ToString());
                    ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].ReadOnly = true;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if (txtPrType.Text != "AMOUNT")
                    {
                    dgvPqDetails.Columns.Add("Vendor Qty", "Vendor Qty");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty").ToString());
                    //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].ReadOnly = false;
                    dgvPqDetails.Columns["Vendor Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                    else if (txtPrType.Text == "AMOUNT")
                    {
                        dgvPqDetails.Columns.Add("Vendor Qty", "Vendor Qty");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty").ToString());
                        //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].ReadOnly = false;
                        dgvPqDetails.Columns["Vendor Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgvPqDetails.Columns[dgvPqDetails.Columns.Count -1].Visible = false;
                    }


                    x = dgvPqDetails.ColumnCount;
                    if (txtPrType.Text != "AMOUNT")
                    {
                        dgvPqDetails.Columns.Add("Order Qty", "Order Qty");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty").ToString());
                        dgvPqDetails.Columns["Order Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    else if (txtPrType.Text == "AMOUNT")
                    {
                        dgvPqDetails.Columns.Add("Amount Qty", "Amount");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAmount Qty").ToString());
                        dgvPqDetails.Columns["Amount Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    VendorId.Add(Dr["VendId"].ToString());
                    VendorName.Add(Dr["VendName"].ToString());

                    PurchQuotId.Add(Dr["PurchQuotId"].ToString());
                    NotInPurchQuotIdString.Add("'" + Dr["PurchQuotId"].ToString() + "'");

                    dgvPqDetails.Columns.Add("Attachment", "Attachment");
                    Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment").ToString());
                    dgvPqDetails.Columns["Attachment"].SortMode = DataGridViewColumnSortMode.NotSortable;

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

                    dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval", "Approval");
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
                    int col = 10;
                    for (int j = 0; j < VendorId.Count; j++)
                    {
                        for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                        {
                            //Obtain a reference to the newly created DataGridViewRow 
                            //if (dgvPqDetails.Rows[i].Cells[1].Value != "By Column")
                            //{
                            var row = this.dgvPqDetails.Rows[i];

                            if(txtPrType.Text != "AMOUNT")
                                Query = "Select Price, PQQty, Qty, 'Files', PPN, PPH, Unit, Ratio, StatusApproval  From CanvasSheetD where SeqNoGroup='" + dgvPqDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "' AND FullItemId='" + dgvPqDetails.Rows[i].Cells[2].Value + "' and CanvasId='" + txtCSNumber.Text.Trim() + "' and PurchQuotId='" + PurchQuotId[j] + "' and DeliveryMethod='" + dgvPqDetails.Rows[i].Cells["Delivery\nMethod"].Value.ToString() + "';";
                            else if (txtPrType.Text == "AMOUNT")
                                Query = "Select Price, PQQty, case when CSAmount is null then 0 else CSAmount end CSAmount, 'Files', PPN, PPH, Unit, Ratio, StatusApproval  From CanvasSheetD where SeqNoGroup='" + dgvPqDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "' AND FullItemId='" + dgvPqDetails.Rows[i].Cells[2].Value + "' and CanvasId='" + txtCSNumber.Text.Trim() + "' and PurchQuotId='" + PurchQuotId[j] + "' and DeliveryMethod='" + dgvPqDetails.Rows[i].Cells["Delivery\nMethod"].Value.ToString() + "';";
                            
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

                                if ((Decimal.Parse(Dr[1].ToString()) == 0 || Decimal.Parse(Dr[2].ToString()) == 0) && ApproveStatus == true && txtPrType.Text != "AMOUNT")
                                {
                                    combo.Value = "No";
                                    dgvPqDetails.Rows[i].Cells[col + 8].ReadOnly = true;
                                }

                                if (Decimal.Parse(Dr[2].ToString()) == 0 && ApproveStatus == true && txtPrType.Text == "AMOUNT")
                                {
                                    combo.Value = "No";
                                    dgvPqDetails.Rows[i].Cells[col + 8].ReadOnly = true;
                                }

                                if (txtPrType.Text.Trim() != "FIX")
                                {
                                    //if (dgvPqDetails.Rows[i].Cells["Base"].Value.ToString() == "Y")
                                    //{
                                        dgvPqDetails.Rows[i].Cells[col + 8] = combo;
                                    //}
                                }
                                else if (txtPrType.Text.Trim() == "FIX")
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
            if (flag == null)
            {
                gvAddHeaderRows();
                for (int i = 0; i < dgvPqDetails.Columns.Count; i++)
                {
                    //dgvPqDetails.Columns[i].Visible = true;
                    dgvPqDetails.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                flag = "X";
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
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                if (txtTransStatusDesc.Text.ToUpper()=="APPROVED")
                {
                    MessageBox.Show("Canvas Sheet tidak bisa diedit karena sudah di approved.");
                    return;
                }
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end         
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            dgvPqDetailsClick = false;
            ModeBeforeEdit();
            GetDataHeader();
        }

        private void dgvPqDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            GetIn_dgvPqDetails_CellEndEdit = true;
            string msg = null;
            Index = dgvPqDetails.CurrentRow.Index;

            for (int k = 0; k < HitungQtyPrNama.Count(); k++)
            {
                 HitungQtyPrNilai[k] = 0;
                 HitungQtyPrOrder[k] = 0;
            }
            int TmpIndex=-1;
            decimal TmpQty;
            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString().Contains("Order"))
            {
                if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value != null)
                {
                    for (int j = 0; j < VendorId.Count; j++)
                    {
                        for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                        {
                            for (int k=0; k < HitungQtyPrNama.Count();k++)
                            {
                                if (HitungQtyPrNama[k] == dgvPqDetails.Rows[Index].Cells["SeqNoGroup"].Value.ToString() && HitungQtyPrNama[k] == dgvPqDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                                {
                                    HitungQtyPrOrder[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);
                                    HitungQtyPrNilai[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);
                                    
                                    TmpIndex = k;
                                }
                            }
                        }
                    }

                    if(Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) != 0)
                    {
                        if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value))
                        {
                            MessageBox.Show("Maaf Input Qty Order Melebihi batas PR Order.");
                            dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = 0;
                        }
                        else
                        {
                            if (HitungQtyPrNilai[TmpIndex] > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value))
                            {
                                msg += "Qty Order tidak boleh lebih besar dari PR Qty.";
                                TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString()) - (HitungQtyPrNilai[TmpIndex] - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value));
                                if (TmpQty <= 0)
                                {
                                    TmpQty = 0;
                                }
                                else
                                {
                                    TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString()) - (HitungQtyPrNilai[TmpIndex] - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value));
                                }
                                dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                            }
                            else
                            {
                                if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value))
                                {
                                    msg = "Qty Order tidak boleh lebih besar dari Vendor Qty.\n";
                                    TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value);// -Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value);
                                    if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 2].Value) == 0)
                                    {
                                        TmpQty = 0;
                                    }
                                    dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                                }
                            }
                        }

                        if (msg != null)
                        {
                            MessageBox.Show(msg);
                        }
                    }
                    //else
                    //{
                    //    decimal TmpQty = 0;
                    //    for (int i = 2; i < Columns.Count(); i += 9)
                    //    {
                    //        if (Columns[i].Contains("Order Qty"))
                    //        {
                    //            TmpQty += Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[i + 10].Value);
                    //        }
                    //        //else
                    //        //TmpQty += Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[Columns[i]].Value);
                    //    }
                    //    //if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value))
                    //    //{
                    //    //    msg = "Qty Order tidak boleh lebih besar dari Vendor Qty.\n";
                    //    //    dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = 0;
                    //    //}

                    //    if (TmpQty > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value))
                    //    {
                    //        msg += "Qty Order tidak boleh lebih besar dari PR Qty.";
                    //        TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString()) - TmpQty;
                    //        if (TmpQty < 0)
                    //        {
                    //            TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value.ToString()) + TmpQty;
                    //        }
                    //        else
                    //        {
                    //            TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value.ToString());
                    //        }
                    //        dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                    //        //dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = 0;
                    //    }

                    //    if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value))
                    //    {
                    //        msg = "Qty Order tidak boleh lebih besar dari Vendor Qty.\n";
                            
                    //        dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value);
                    //    }

                    //    if (msg != null)
                    //    {
                    //        MessageBox.Show(msg);
                    //    }
                    //}
                    //dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells[dgvPqDetails.CurrentCell.ColumnIndex].Value = Convert.ToDecimal(dgvPqDetails.Rows[dgvPqDetails.CurrentCell.RowIndex].Cells[dgvPqDetails.CurrentCell.ColumnIndex].Value).ToString("N2");
                }
            }

            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString().Contains("Amount Qty"))
            {
                if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value != null)
                {
                    for (int j = 0; j < VendorId.Count; j++)
                    {
                        for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                        {
                            for (int k = 0; k < HitungQtyPrNama.Count(); k++)
                            {
                                if (HitungQtyPrNama[k] == dgvPqDetails.Rows[Index].Cells["SeqNoGroup"].Value.ToString() && HitungQtyPrNama[k] == dgvPqDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                                {
                                    HitungQtyPrOrder[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);
                                    HitungQtyPrNilai[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);

                                    TmpIndex = k;
                                }
                            }
                        }
                    }

                    if ((Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) != 0) && dgvPqDetails.Rows[Index].Cells["PR Amount"].Value!=null)
                    {
                        if (HitungQtyPrNilai[TmpIndex] > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Amount"].Value))
                        {
                            msg += "Amount Order tidak boleh lebih besar dari PR Amount.";
                            TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Amount"].Value.ToString()) - (HitungQtyPrNilai[TmpIndex] - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value));
                            if (TmpQty <= 0)
                            {
                                TmpQty = 0;
                            }
                            else
                            {
                                TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Amount"].Value.ToString()) - (HitungQtyPrNilai[TmpIndex] - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value));
                            }
                            dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                        }
                        //else
                        //{
                        //    if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value))
                        //    {
                        //        msg = "Qty Order tidak boleh lebih besar dari Vendor Qty.\n";
                        //        TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 1].Value) - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value);
                        //        if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex - 2].Value) == 0)
                        //        {
                        //            TmpQty = 0;
                        //        }
                        //        dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                        //    }
                        //}

                        if (msg != null)
                        {
                            MessageBox.Show(msg);
                        }
                    }
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
            Conn = ConnectionString.GetConnection();
            if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString().Contains("Approval"))
            {
                if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value.ToString() == "Yes")
                {
                    int i = Convert.ToInt32(Convert.ToInt32(e.ColumnIndex) / 9);
                    Query = "SELECT CASE WHEN ISNULL(ValidTo,'1900-01-01') >= (cast(GETDATE()-6 as date)) THEN 'V' ELSE 'N' END FROM PurchQuotationH WHERE PurchQuotID = @PQId";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.AddWithValue("@PQId", PurchQuotId[i - 2]);
                    String ValidPQ = Cmd.ExecuteScalar().ToString();
                    if (ValidPQ == "N")
                    {
                        MessageBox.Show("Maaf tanggal PQ sudah tidak valid");
                        dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = "No";
                    }
                }

                if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value != null)
                {
                    string SeqNoGroup = dgvPqDetails.Rows[Index].Cells["SeqNoGroup"].Value.ToString();
                    string DeliveryMethod = dgvPqDetails.Rows[Index].Cells["Delivery\nMethod"].Value.ToString();
                    for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                    {
                        if (SeqNoGroup == dgvPqDetails.Rows[j].Cells["SeqNoGroup"].Value.ToString() && DeliveryMethod == dgvPqDetails.Rows[j].Cells["Delivery\nMethod"].Value.ToString() && j!=Index)
                        {
                            dgvPqDetails.Rows[j].Cells[e.ColumnIndex].Value = dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value;
                            //dgvPqDetails.Rows[j].Cells[e.ColumnIndex] = combo;
                        }
                    }
                }
            }
            Conn.Close();
            dgvPqDetails.AutoResizeColumns();
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
                string msg = "";
                for (int i = 0; i < PurchQuotId.Count; i++)
                {
                    if (i >= 1)
                        msg += ",";
                    msg += "'" + PurchQuotId[i] + "'";
                }
                if (!(msg.Contains(PurchQuotIdString)))
                {
                    Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId, b.PPH, b.PPN From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.PurchQuotID in (" + PurchQuotIdString + ") order by a.PurchQuotID";// and a.TransStatus=''";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    dgvPqDetails.Columns["SeqNo"].Visible = false;

                    //int i = dgvPqDetails.ColumnCount;
                    TmpVendorId.Clear();

                    while (Dr.Read())
                    {

                        dgvPqDetails.Columns.Add("Price", "Price");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice").ToString());
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].ReadOnly = true;
                        dgvPqDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        //dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty", Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty");
                        //Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty").ToString());
                        ////dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].ReadOnly = true;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nPR Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;

                        dgvPqDetails.Columns.Add("Vendor Qty", "Vendor Qty");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty").ToString());
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].ReadOnly = true;
                        dgvPqDetails.Columns["Vendor Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        if(txtPrType.Text =="AMOUNT")
                            dgvPqDetails.Columns[dgvPqDetails.Columns.Count-1].Visible = false;
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nVendor Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


                        //x = dgvPqDetails.ColumnCount;
                        if (txtPrType.Text != "AMOUNT")
                        {
                            dgvPqDetails.Columns.Add("Order Qty", "Order Qty");
                            Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty").ToString());
                            dgvPqDetails.Columns["Order Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        else if (txtPrType.Text == "AMOUNT")
                        {
                            dgvPqDetails.Columns.Add("Amount Qty", "Amount");
                            Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAmount Qty").ToString());
                            dgvPqDetails.Columns["Amount Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        }
                        //dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nOrder Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        VendorId.Add(Dr["VendId"].ToString());
                        VendorName.Add(Dr["VendName"].ToString());
                        TmpVendorId.Add(Dr["VendId"].ToString());

                        PurchQuotId.Add(Dr["PurchQuotId"].ToString());

                        dgvPqDetails.Columns.Add("Attachment", "Attachment");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment").ToString());
                        dgvPqDetails.Columns["Attachment"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        //dgvCsGel.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nAttachment"].ReadOnly = true;

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

                        dgvPqDetails.Columns.Add(Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval", "Approval");
                        Columns.Add((Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval").ToString());
                        dgvPqDetails.Columns[Dr["VendId"].ToString() + "\n" + Dr["VendName"].ToString() + "\n" + Dr["PurchQuotId"].ToString() + "\nApproval"].SortMode = DataGridViewColumnSortMode.NotSortable;
                        if (ApproveStatus == false)
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

                                Query = "Select Price, Qty2, a.PurchQuotID, SeqNo, FullItemID, a.DeliveryMethod, b.PPN, b.PPH, a.Unit, a.Ratio From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.DeliveryMethod='" + dgvPqDetails.Rows[i].Cells[1].Value + "' and a.FullItemId='" + dgvPqDetails.Rows[i].Cells[2].Value + "' and ReffTransID='" + txtPrNumber.Text + "' and b.VendId='" + VendorId[VendorId.Count - TmpVendorId.Count + j].ToString() + "' and a.Unit='" + dgvPqDetails.Rows[i].Cells[9].Value + "';";
                                Cmd = new SqlCommand(Query, Conn);
                                //decimal tmpPrice = Convert.ToDecimal(Cmd.ExecuteScalar() == null ? "0.00" : Cmd.ExecuteScalar().ToString());
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    dgvPqDetails.Rows[i].Cells[col].Value = (Dr[0].ToString() == "" ? "0" : Dr[0].ToString());
                                    //dgvPqDetails.Rows[i].Cells[col + 1].Value = (Dr[1].ToString() == "" ? "0" : Dr[1].ToString());
                                    dgvPqDetails.Rows[i].Cells[col + 1].Value = (Dr[1].ToString() == "" ? "0" : Dr[1].ToString());
                                    if (txtPrType.Text != "AMOUNT")
                                        dgvPqDetails.Rows[i].Cells[col + 2].Value = "0";
                                    if (txtPrType.Text == "AMOUNT")
                                        dgvPqDetails.Rows[i].Cells[col + 2].Value = "0";//(Dr[0].ToString() == "" ? "0" : Dr[0].ToString());
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
                    if(txtPrType.Text!="AMOUNT")
                        dgvPqDetails.Columns["PR Qty"].ReadOnly = true;
                    else if(txtPrType.Text=="AMOUNT")
                        dgvPqDetails.Columns["PR Amount"].ReadOnly = true;
                    dgvPqDetails.Columns["BracketDesc"].ReadOnly = true;
                    dgvPqDetails.Columns["Unit"].ReadOnly = true;

                    if (txtPrType.Text != "AMOUNT")
                        dgvPqDetails.Columns["Unit"].Visible = true;
                    if (txtPrType.Text == "AMOUNT")
                        dgvPqDetails.Columns["Unit"].Visible = false;

                    dgvPqDetails.Columns["No"].Frozen = true;
                    dgvPqDetails.Columns["Delivery\nMethod"].Frozen = true;
                    dgvPqDetails.Columns["FullItemID"].Frozen = true;
                    dgvPqDetails.Columns["ItemName"].Frozen = true;
                    dgvPqDetails.Columns["SeqNo"].Frozen = true;
                    if (txtPrType.Text != "AMOUNT")
                        dgvPqDetails.Columns["PR Qty"].Frozen = true;
                    else if (txtPrType.Text == "AMOUNT")
                        dgvPqDetails.Columns["PR Amount"].Frozen = true;
                    dgvPqDetails.Columns["Unit"].Frozen = true;
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        if (Columns[i].Contains("Price") || Columns[i].Contains("PR") || Columns[i].Contains("Vendor") || Columns[i].Contains("Order Qty") || Columns[i].Contains("Amount Qty") || Columns[i].Contains("Attachment"))
                        {
                            if (Columns[i].Contains("Order Qty"))
                            {
                                for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                                {
                                    if (dgvPqDetails.Rows[j].Cells[i + 10].Value == null || dgvPqDetails.Rows[j].Cells[i + 9].Value == null)
                                    {
                                        dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                                    }
                                    //else
                                    //{
                                    //    if (decimal.Parse(dgvPqDetails.Rows[j].Cells[i + 10].Value.ToString()) == 0)
                                    //    {
                                    //        dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                                    //    }
                                    //}
                                }
                            }
                            if (Columns[i].Contains("Amount Qty"))
                            {
                                for (int j = 0; j < dgvPqDetails.Rows.Count; j++)
                                {
                                    if (dgvPqDetails.Rows[j].Cells[i + 10].Value == null || dgvPqDetails.Rows[j].Cells[i + 9].Value == null)
                                    {
                                        dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                                    }
                                    //else
                                    //{
                                    //    if (decimal.Parse(dgvPqDetails.Rows[j].Cells[i + 10].Value.ToString()) == 0)
                                    //    {
                                    //        dgvPqDetails.Rows[j].Cells[i + 10].ReadOnly = true;
                                    //    }
                                    //}
                                }
                            }  
                            if (Columns[i].Contains("Price"))
                                dgvPqDetails.Columns[i+10].ReadOnly = true;
                            if (Columns[i].Contains("PR"))
                                dgvPqDetails.Columns[i + 10].ReadOnly = true;
                            if (Columns[i].Contains("Vendor"))
                                dgvPqDetails.Columns[i + 10].ReadOnly = true;
                            if (Columns[i].Contains("Attachment"))
                                dgvPqDetails.Columns[i + 10].ReadOnly = true;
                            //dgvPqDetails.Columns[Columns[i]].ReadOnly = true;
                            //dgvPqDetails.Columns["PR Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    }
                    dgvPqDetails.Rows[0].Frozen = true;

                    dgvPqDetails.AutoResizeColumns();
                    for (int x = 11; x < dgvPqDetails.ColumnCount; x += 9)
                        for (int i = 0; i < dgvPqDetails.RowCount; i++)
                        {
                            if (txtPrType.Text.Trim() != "FIX")
                            {
                                if (dgvPqDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                                {
                                    dgvPqDetails.Rows[i].Cells[x+1].ReadOnly = true;
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
            }
            if (flag == null)
            {
                gvAddHeaderRows();
                for (int i = 0; i < dgvPqDetails.Columns.Count; i++)
                {
                    //dgvPqDetails.Columns[i].Visible = true;
                    dgvPqDetails.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                flag = "X";
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
            if (txtCSNumber.Text != "")
            {
                MessageBox.Show("Vendor tidak bisa dihapus karena nomor canvas sheet sudah tersimpan.");
                return;
            }

            if (dgvPqDetails.RowCount > 0 && dgvPqDetails.ColumnCount>10)
                if (dgvPqDetails.RowCount > 0 && dgvPqDetails.CurrentCell.ColumnIndex >= 9)
                {
                    Index = dgvPqDetails.CurrentRow.Index;
                    int Column = Convert.ToInt32(dgvPqDetails.CurrentCell.ColumnIndex);
                    //string[] Vendor = dgvPqDetails.Columns[Column].Name.ToString().Split('\n');

                    int a = dgvPqDetails.CurrentCell.ColumnIndex;
                    int index = 0;
                    for (int i = 10; i < dgvPqDetails.ColumnCount; i += 9)
                    {
                        if (a >= i && a < i + 9)
                        {
                            break;
                        }
                        index++;
                    }

                    //dgvPqDetails.Columns[].Name.ToString();
                    DialogResult dialogResult = MessageBox.Show("Apakah Vendor : " + /*Vendor[0]*/VendorId[index] + " akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        int tmpColumn = ((dgvPqDetails.CurrentCell.ColumnIndex - 9) / 9);
                        NotInPurchQuotIdString.RemoveAt(tmpColumn);
                        VendorId.RemoveAt(VendorId.IndexOf(VendorId[index]/*Vendor[0]*/.ToString()));
                        //NotInPurchQuotIdString.RemoveAt(VendorId.IndexOf(Vendor[0].ToString()));
                        PurchQuotId.RemoveAt(/*PurchQuotId.IndexOf(*/index/*Vendor[2].ToString())*/);

                        //Columns.RemoveAt(tmpColumn);
                        //PurchQuotId.RemoveAt(tmpColumn);
                        //int min = (tmpColumn * 9) + 8;
                        int min = (tmpColumn * 9) + 10;
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
            if (dgvPqDetails.Columns[dgvPqDetails.CurrentCell.ColumnIndex].Name == "Delivery\nMethod")
            {
                if (!char.IsControl(e.KeyChar))
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

        //private void dgvPqDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    //hendry
        //    if (dgvPqDetails.RowCount > 0)
        //        //if (dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Attachment"))
        //        //{
        //        //    if (dgvPqDetails.RowCount > 0 && dgvPqDetails.CurrentCell.ColumnIndex >= 8)
        //        //    {
        //        //        Index = dgvPqDetails.CurrentRow.Index;
        //        //        int Column = Convert.ToInt32(dgvPqDetails.CurrentCell.ColumnIndex);
        //        //        int a = dgvPqDetails.CurrentCell.ColumnIndex;
        //        //        int index = 0;
        //        //        for (int i = 10; i < dgvPqDetails.ColumnCount; i += 9)
        //        //        {
        //        //            if (a >= i && a < i + 9)
        //        //            {
        //        //                break;
        //        //            }
        //        //            index++;
        //        //        }

        //        //        PopUp.Attachment.Attachment PopUpStock = new PopUp.Attachment.Attachment();
        //        //        string[] dgvHeader = dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Split('\n');
        //        //        PopUpStock.RefreshGrid(PurchQuotId[index]);
        //        //        PopUpStock.Show();
        //        //    }
        //        //}
        //    //hendry end

        //    if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
        //    {
        //        //if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "VendId")
        //        //{
        //        //    string TmpListVendor = dgvPqDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        //        //    string[] SplitVendor = TmpListVendor.Split(';');

        //        //    for (int i = 0; i < SplitVendor.Count(); i++)
        //        //    {
        //        //        PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();
        //        //        ListVendor.Add(PopUpVendor);
        //        //        PopUpVendor.GetData(SplitVendor[i].ToString());
        //        //        PopUpVendor.Y += 100 * i;
        //        //        PopUpVendor.Show();
        //        //    }
        //        //}

        //        if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
        //        {

        //            PopUp.Stock.Stock PopUpStock = new PopUp.Stock.Stock();
        //            PopUpStock.GetData(dgvPqDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
        //            itemID = dgvPqDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
        //            PopUpStock.Show();
        //        }
        //        /*hendry comment
        //        if (dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Attachment"))
        //        {
        //            PopUp.Attachment.Attachment PopUpStock = new PopUp.Attachment.Attachment();
        //            string[] dgvHeader = dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Split('\n');
        //            PopUpStock.RefreshGrid(dgvHeader[2]);
        //            PopUpStock.Show();
        //        }
        //        hendry comment*/
        //    }

        //}

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvPqDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if(e.Value != null)
            {
                if (dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("Qty"))
                {
                    double d = double.Parse(e.Value.ToString());
                    dgvPqDetails.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    e.Value = d.ToString("N2");
                }
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Amount Qty"))
                {
                    double d = double.Parse(e.Value.ToString());
                    dgvPqDetails.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    e.Value = d.ToString("N2");
                }
                if (dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("Ratio") || dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("Price") || dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("PPN") || dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("PPH") || dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Contains("PR Amount"))
                {
                    double d = double.Parse(e.Value.ToString());
                    dgvPqDetails.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    e.Value = d.ToString("N2");
                }
            }
        }

        private void InvalidateHeader()
        {
            Rectangle rtHeader = this.dgvPqDetails.DisplayRectangle;
            rtHeader.Height = this.dgvPqDetails.ColumnHeadersHeight * 3;
            this.dgvPqDetails.Invalidate(rtHeader);
        }

        private void gvAddHeaderRows()
        {
            this.dgvPqDetails.Paint += dgvPqDetails_Paint_1;
            this.dgvPqDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvPqDetails.ColumnHeadersHeight = this.dgvPqDetails.ColumnHeadersHeight * 3;
            this.dgvPqDetails.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.BottomCenter;
            this.dgvPqDetails.Scroll += dgvPqDetails_Scroll_1;
            this.dgvPqDetails.ColumnWidthChanged += dgvPqDetails_ColumnWidthChanged;
            this.dgvPqDetails.Resize += dgvPqDetails_Resize;
        }

        int w;
        int count = 0;
        private void dgvPqDetails_Paint_1(object sender, PaintEventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            string listID = null;
            if (txtCSNumber.Text != String.Empty)
                Query = "Select count(distinct a.PurchQuotId) From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID inner join CanvasSheetD c on c.VendId=b.VendId where ReffTransID='" + txtPrNumber.Text + "' and c.CanvasId='" + txtCSNumber.Text.Trim() + "'";
            else if (PurchQuotId.Count != 0)//(PurchQuotIdString != String.Empty)
            {
                for (int j = 0; j < PurchQuotId.Count; j++)
                {
                    if (j >= 1)
                        listID += " ,";
                    listID += "'" + PurchQuotId[j] + "'";
                }
                //Query = "Select count(distinct a.PurchQuotId) From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.PurchQuotID in (" + PurchQuotIdString + ")";
                Query = "Select count(distinct a.PurchQuotId) From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.PurchQuotID in (" + listID /*PurchQuotIdString*/ + ")";
            }
            else
                Query = "Select count(distinct a.PurchQuotId) From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID inner join CanvasSheetD c on c.VendId=b.VendId where ReffTransID='" + txtPrNumber.Text + "' and c.CanvasId='" + txtCSNumber.Text.Trim() + "'";
            Cmd = new SqlCommand(Query, Conn);
            count = (Int32)Cmd.ExecuteScalar();

            if (txtCSNumber.Text != String.Empty)
            {
                Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId From PurchQuotation_Dtl a inner join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID inner join CanvasSheetD c on c.VendId=b.VendId and c.PurchQuotId=b.PurchQuotID where ReffTransID='" + txtPrNumber.Text + "' and c.CanvasId='" + txtCSNumber.Text.Trim() + "'";
            }
            else if (PurchQuotId.Count != 0)//(PurchQuotIdString != String.Empty)
            {
                //Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId, b.PPH, b.PPN From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.PurchQuotID in (" + PurchQuotIdString + ")";
                Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId, b.PPH, b.PPN From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.PurchQuotID in (" + listID /*PurchQuotIdString*/ + ")";
            }
            else
            {
                Query = "Select distinct b.VendId, b.VendName, a.PurchQuotId From PurchQuotation_Dtl a inner join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID inner join CanvasSheetD c on c.VendId=b.VendId and c.PurchQuotId=b.PurchQuotID where ReffTransID='" + txtPrNumber.Text + "' and c.CanvasId='" + txtCSNumber.Text.Trim() + "'";
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int col = 10, i = 0;
            int loop = 0;

            if(VendorId != null)
            for(int k=0; k<VendorId.Count(); k++)
            //while (Dr.Read())
            {
                Rectangle r1 = this.dgvPqDetails.GetCellDisplayRectangle(col, -1, true); //atur kotak mau per brp kolom
                int w2 = this.dgvPqDetails.GetCellDisplayRectangle(col + 1, -1, true).Width;
                int w3 = this.dgvPqDetails.GetCellDisplayRectangle(col + 2, -1, true).Width;
                int w4 = this.dgvPqDetails.GetCellDisplayRectangle(col + 3, -1, true).Width;
                int w8 = this.dgvPqDetails.GetCellDisplayRectangle(col + 8, -1, true).Width;
                //for(i = 1; i<=3;i++)
                //    w += this.dgvPqDetails.GetCellDisplayRectangle(col + i, -1, true).Width;
                r1.X += 1;
                r1.Y += 1;
                r1.Width = r1.Width + w2 + w3 + w4 + w8 - 2;//r1.Width + w1 + w2 + w3 + w4 - 2; //atur size kotak
                r1.Height = r1.Height / 2 - 2;
                Pen pen = new Pen(Color.DarkGray);
                e.Graphics.DrawLine(pen, r1.X, r1.Y + r1.Height, r1.X + r1.Width, r1.Y + r1.Height);
                EditColor();
                e.Graphics.FillRectangle(new SolidBrush(ListColor[i]), r1);
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                //Cmd = new SqlCommand("Select distinct b.VendName From PurchQuotation_Dtl a left join PurchQuotationH b on a.PurchQuotID=b.PurchQuotID where a.PurchQuotID = '" + PurchQuotId[loop] + "' and b.VendId = '" + VendorId[loop] + "'", Conn);
                //e.Graphics.DrawString(VendorId[loop]/*Dr[0].ToString()*/ + " ( " + VendorId[k].ToString()/*Dr[1].ToString()*/ + " )\n" + PurchQuotId[loop]/*Dr[2].ToString()*/,
                e.Graphics.DrawString(VendorName[loop] + "\n" + PurchQuotId[loop],
                    this.dgvPqDetails.ColumnHeadersDefaultCellStyle.Font,
                    new SolidBrush(this.dgvPqDetails.ColumnHeadersDefaultCellStyle.ForeColor),
                    r1,
                    format);
                loop++;
                if (col >= count * 9)
                    break;
                col += 9; i++;//atur kotak mau per brp kolom
            }
        }

        private void FormCanvasSheet2_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            flag = null;
        
            if (ParentToPA != null || ParentToPO !=null)
            {
                //Parent.RefreshGrid();
            }
            else
            {
                Parent.RefreshGrid();
            }
        }

        private void dgvPqDetails_Scroll_1(object sender, ScrollEventArgs e)
        {
            InvalidateHeader();
        }

        private void dgvPqDetails_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            InvalidateHeader();
        }

        private void dgvPqDetails_Resize(object sender, EventArgs e)
        {
            InvalidateHeader();
        }

        private void dgvPqDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPqDetails.RowCount > 0)
            {
                //if (dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Attachment"))
                //{
                    if (dgvPqDetails.RowCount > 0 && dgvPqDetails.CurrentCell.ColumnIndex >= 8)
                    {
                        Index = dgvPqDetails.CurrentRow.Index;
                        int Column = Convert.ToInt32(dgvPqDetails.CurrentCell.ColumnIndex);
                        int a = dgvPqDetails.CurrentCell.ColumnIndex;
                        int index = 0;
                        for (int i = 10; i < dgvPqDetails.ColumnCount; i += 9)
                        {
                            if (a >= i && a < i + 9)
                            {
                                break;
                            }
                            index++;
                        }

                        if (dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Attachment"))
                        {
                            PopUp.Attachment.Attachment PopUpStock = new PopUp.Attachment.Attachment();
                            string[] dgvHeader = dgvPqDetails.Columns[e.ColumnIndex].HeaderText.Split('\n');
                            PopUpStock.RefreshGrid(PurchQuotId[index]);
                            PopUpStock.Show();
                        }
                    }
                //}
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
        //tia edit
        //klik kanan
        PopUp.FullItemId.FullItemId FID = null;
        Purchase.PurchaseRequisition.HeaderPR PRid = null;
        PopUp.Vendor.Vendor Vendor = null;

        Purchase.PurchaseAgreement.PAForm ParentToPA;
        Purchase.PurchaseOrderNew.POForm ParentToPO;

        public void ParentRefreshGrid(Purchase.PurchaseAgreement.PAForm pa)
        {
            ParentToPA = pa;
        }

        public void ParentRefreshGrid2(Purchase.PurchaseOrderNew.POForm Po)
        {
            ParentToPO = Po;
        }

        private void dgvPqDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            if ((Mode == "New" || Mode == "Edit") && ApproveStatus != true)
            {
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    if (dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Order Qty") || dgvPqDetails.Columns[e.ColumnIndex].Name.Contains("Amount Qty"))
                    {
                        int i = Convert.ToInt32(Convert.ToInt32(e.ColumnIndex) / 9);
                        Query = "SELECT CASE WHEN ISNULL(ValidTo,'1900-01-01') >= (cast(GETDATE()-6 as date)) THEN 'V' ELSE 'N' END FROM PurchQuotationH WHERE PurchQuotID = @PQId";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.AddWithValue("@PQId", PurchQuotId[i - 1]);
                        String ValidPQ = Cmd.ExecuteScalar().ToString();
                        if (ValidPQ == "N")
                        {
                            dgvPqDetails.Columns[e.ColumnIndex].ReadOnly = true;
                            MessageBox.Show("Maaf tanggal PQ sudah tidak valid");
                            return;
                        }
                        else
                            dgvPqDetailsClick = true;
                    }
                    else
                        dgvPqDetailsClick = false;
                }
            }

            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                //if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "Price")
                //{
                //    string TmpListVendor = dgvPqDetails.Columns[e.ColumnIndex].Name.ToString();
                //    string[] SplitVendor = TmpListVendor.Split(';');

                //    for (int i = 0; i < ListVendor.Count(); i++)
                //    {
                //        ListVendor[i].Close();
                //    }

                //    for (int i = 0; i < SplitVendor.Count(); i++)
                //    {
                //        PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();
                //        ListVendor.Add(PopUpVendor);
                //        PopUpVendor.GetData(SplitVendor[i].ToString());
                //        PopUpVendor.Y += 100 * i;
                //        PopUpVendor.Show();
                //    }
                //}
                if (FID==null || FID.Text=="")
                {
                    if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPqDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        //PopUpItemName.Close();
                        // PopUpItemName = new PopUp.Stock.Stock();
                        //PopUpItemName = new PopUp.FullItemId.FullItemId();
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPqDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvPqDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvPqDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }

            }
            //canvasheets
            if (e.Button == MouseButtons.Right && e.RowIndex == -1 && e.ColumnIndex > 9)
            {
                if (Vendor==null||Vendor.Text=="")
                {
                    int i = Convert.ToInt32(Convert.ToInt32(e.ColumnIndex) / 9);
                    //MessageBox.Show(VendorId[i-1]);
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(VendorId[i - 1]);
                    Vendor.Show();
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }

            }
            Conn.Close();
        }

        private void txtPrNumber_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (PRid == null || PRid.Text == "")
                {
                    txtPrNumber.Enabled = true;
                    PRid = new  Purchase.PurchaseRequisition.HeaderPR();
                    PRid.SetMode("PopUp", txtPrNumber.Text);
                    PRid.ParentRefreshGrid2(this);
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
        //tia edit end

        private void dgvPqDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPqDetailsClick == true)
            {
                string msg = null;
                Index = dgvPqDetails.CurrentRow.Index;

                for (int k = 0; k < HitungQtyPrNama.Count(); k++)
                {
                    HitungQtyPrNilai[k] = 0;
                    HitungQtyPrOrder[k] = 0;
                }
                int TmpIndex = -1;
                decimal TmpQty;
                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString().Contains("Order"))
                {
                    if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value != null)
                    {
                        for (int j = 0; j < VendorId.Count; j++)
                        {
                            for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                            {
                                for (int k = 0; k < HitungQtyPrNama.Count(); k++)
                                {
                                    if (HitungQtyPrNama[k] == dgvPqDetails.Rows[Index].Cells["SeqNoGroup"].Value.ToString() && HitungQtyPrNama[k] == dgvPqDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                                    {
                                        HitungQtyPrOrder[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);
                                        HitungQtyPrNilai[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);

                                        TmpIndex = k;
                                    }
                                }
                            }
                        }

                        if (Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) != 0)
                        {
                            if (HitungQtyPrNilai[TmpIndex] > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value))
                            {
                                if (GetIn_dgvPqDetails_CellEndEdit == true)
                                {
                                    msg = "Qty Order tidak boleh lebih besar dari PR Qty dan Vendor Qty.";
                                    TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Qty"].Value.ToString()) - (HitungQtyPrNilai[TmpIndex] - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value));
                                    if (TmpQty <= 0)
                                    {
                                        TmpQty = 0;
                                    }
                                    dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                                }
                            }

                            if (msg != null)
                            {
                                MessageBox.Show(msg);
                            }
                        }
                    }
                }

                if (dgvPqDetails.Columns[e.ColumnIndex].Name.ToString().Contains("Amount Qty"))
                {
                    if (dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value != null)
                    {
                        for (int j = 0; j < VendorId.Count; j++)
                        {
                            for (int i = 0; i < dgvPqDetails.Rows.Count; i++)
                            {
                                for (int k = 0; k < HitungQtyPrNama.Count(); k++)
                                {
                                    if (HitungQtyPrNama[k] == dgvPqDetails.Rows[Index].Cells["SeqNoGroup"].Value.ToString() && HitungQtyPrNama[k] == dgvPqDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                                    {
                                        HitungQtyPrOrder[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);
                                        HitungQtyPrNilai[k] += Convert.ToDecimal(dgvPqDetails.Rows[i].Cells[12 + ((j) * 9)].Value);

                                        TmpIndex = k;
                                    }
                                }
                            }
                        }

                        if ((Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value) != 0) && dgvPqDetails.Rows[Index].Cells["PR Amount"].Value != null)
                        {
                            if (HitungQtyPrNilai[TmpIndex] > Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Amount"].Value))
                            {
                                msg += "Amount Order tidak boleh lebih besar dari PR Amount.";
                                TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Amount"].Value.ToString()) - (HitungQtyPrNilai[TmpIndex] - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value));
                                if (TmpQty <= 0)
                                {
                                    TmpQty = 0;
                                }
                                else
                                {
                                    TmpQty = Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells["PR Amount"].Value.ToString()) - (HitungQtyPrNilai[TmpIndex] - Convert.ToDecimal(dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value));
                                }
                                dgvPqDetails.Rows[Index].Cells[e.ColumnIndex].Value = TmpQty;
                            }
                            if (msg != null)
                            {
                                MessageBox.Show(msg);
                            }
                        }
                    }
                }
            }
            GetIn_dgvPqDetails_CellEndEdit = false;
            dgvPqDetailsClick = false;
        }
    }
}