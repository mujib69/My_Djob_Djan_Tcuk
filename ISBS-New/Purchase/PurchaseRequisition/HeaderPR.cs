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

namespace ISBS_New.Purchase.PurchaseRequisition
{
    public partial class HeaderPR : MetroFramework.Forms.MetroForm
    {
        #region Initialization
        private SqlConnection Conn;
        private SqlCommand Cmd, Cmd2;
        private SqlDataReader Dr, Dr2;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        
        String statussm = "";
        String statuspm = "";

        string Mode, Query,Query2, crit = null;
        decimal QtyAwal = 0, QtyInput = 0;
        String sm = "", pm = "";
        int Index;
        decimal step = 0;
            
        public string PRNumber = "", tmpPrType = "";

        DateTimePicker dtp;
        ComboBox DeliveryMethod;

        public string Gelombang;
        public string Bracket;

        public List<string> Gelombang1 = new List<string>();
        public List<string> Bracket1 = new List<string>();
        List<DetailPR> ListDetailPR = new List<DetailPR>();
        List<Gelombang> ListGelombang = new List<Gelombang>();
        List<Info> ListInfo = new List<Info>();
        List<PopUp.Vendor.Vendor> ListVendor = new List<PopUp.Vendor.Vendor>();
        List<PopUp.SalesOrder.SalesOrder> ListSO = new List<PopUp.SalesOrder.SalesOrder>();
        PopUp.Stock.Stock PopUpItemName = new PopUp.Stock.Stock();
        PopUp.FullItemId.FullItemId FullItemId = new PopUp.FullItemId.FullItemId();
       // PopUp.SalesOrder.SalesOrder SalesOrder = new PopUp.SalesOrder.SalesOrder();

        Purchase.PurchaseRequisition.InquiryPR Parent;
        #endregion
        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public HeaderPR()
        {
            InitializeComponent();
            if (Purchase.PurchaseQuotation.FormPQ.reffID != null)
                txtPrNumber.Text = Purchase.PurchaseQuotation.FormPQ.reffID;
            if (Purchase.PurchaseQuotation.FormPQ.pRID != null)
                txtPrNumber.Text = Purchase.PurchaseQuotation.FormPQ.pRID;

        }

        private void HeaderPR2_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
            GetDataHeader();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
            else if (Mode == "ModeView")
            {
                ModeView();
            }
            else if (Mode=="PopUp")//tia edit
            {
                ModePopUp();
            }//tia edit end

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            //dtp.CustomFormat = "dd/MM/yyyy";
            if (Mode == "New")
            { }
            else
                dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Visible = false;
            dtp.Width = 100;

            dgvPrDetails.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;
            dgvPrDetails.CellBeginEdit += this.dgvPrDetails_CellBeginEdit;
            dgvPrDetails.CellEndEdit += this.dgvPrDetails_CellEndEdit;

            DeliveryMethod = new ComboBox();
            DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            DeliveryMethod.Visible = false;
            dgvPrDetails.Controls.Add(DeliveryMethod);
            DeliveryMethod.DropDownClosed += this.DeliveryMethod_DropDownClosed;
            DeliveryMethod.SelectionChangeCommitted += this.DeliveryMethod_SelectionChangeCommitted;
            Purchase.PurchaseRequisition.InquiryPR f = new Purchase.PurchaseRequisition.InquiryPR();
            //f.RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (cmbPrType.SelectedIndex > -1)
            {
                if (cmbPrType.SelectedItem.ToString() == "")
                {
                    MessageBox.Show("Pilih PR Type dahulu..");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Pilih PR Type dahulu..");
                return;
            }

            Gelombang = "";
            Bracket = "";
            DetailPR DetailPR = new DetailPR();

            List<DetailPR> ListDetailPR = new List<DetailPR>();
            DetailPR.ParentRefreshGrid(this);
            DetailPR.ShowDialog();
            if (cmbPrType.Text !="FIX")
            {
                CallFormGelombang(Gelombang, Bracket);
                MethodReadOnlyRowBaseN();
            }
            EditColor();
            //}
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPrDetails.RowCount > 0)
                if (dgvPrDetails.RowCount > 0)
                {
                    Index = dgvPrDetails.CurrentRow.Index;
                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvPrDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvPrDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvPrDetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        string NumberGroupSeq = dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["SeqNoGroup"].Value.ToString();

                        if (dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Base"].Value.ToString() == "Y")
                        {
                            for (int i = 0; i < dgvPrDetails.RowCount; i++)
                            {
                                if (dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString() == NumberGroupSeq)
                                {
                                    dgvPrDetails.Rows.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                        else
                        {
                            dgvPrDetails.Rows.RemoveAt(Index);
                        }
                        SortNoDataGrid();
                    }
                    //GetGelombang();
                }
        }

        public void SetMode(string tmpMode, string tmpPRNumber)
        {
            Mode = tmpMode;
            PRNumber = tmpPRNumber;
            txtPrNumber.Text = tmpPRNumber;
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

                if (txtPrNumber.Text != "")
                {
                    Query = "Select TransStatus from [dbo].[PurchRequisitionH] where [PurchReqID]='" + txtPrNumber.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Check = Cmd.ExecuteScalar().ToString();
                    if (Check != "01" && Check != "02" && Check != "12")
                    {
                        MessageBox.Show("PurchReqID = " + txtPrNumber.Text + ".\n" + "Tidak bisa diedit karena sudah diproses.");
                        Conn.Close();
                        return;
                    }
                }

                Mode = "Edit";

                btnSave.Visible = true;
                btnExit.Visible = false;
                btnEdit.Visible = false;
                btnCancel.Visible = true;

                dtPrDate.Enabled = false;
                cmbPrType.Enabled = false;
                txtPrStatus.Enabled = true;

                ModeEdit();
                MethodReadOnlyRowBaseN();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
            
        }

        private void dgvPrDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "Qty")
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
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "Amount")
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
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "DeliveryMethod")
            {
                if (!char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            
        }

        public void AddDataGridDetail(string GroupId, string SubGroup1Id, string SubGroup2Id, string ItemId, string FullItemId, string ItemName, string GelombangId, string BracketId, string Base, string Price)
        {
            if (dgvPrDetails.RowCount - 1 <= 0)
            {
                dgvPrDetails.ColumnCount = 24;
                dgvPrDetails.Columns[0].Name = "No";
                dgvPrDetails.Columns[1].Name = "FullItemID";
                dgvPrDetails.Columns[2].Name = "ItemName";
                dgvPrDetails.Columns[3].Name = "Qty";
                dgvPrDetails.Columns[4].Name = "Unit";
                dgvPrDetails.Columns[5].Name = "Base";
                dgvPrDetails.Columns[6].Name = "Vendor"; //VendId
                dgvPrDetails.Columns[7].Name = "DeliveryMethod";
                dgvPrDetails.Columns[8].Name = "SalesSO"; //ReffTransID
                dgvPrDetails.Columns[9].Name = "ExpectedDateFrom";
                dgvPrDetails.Columns[10].Name = "ExpectedDateTo";;
                dgvPrDetails.Columns[11].Name = "Deskripsi";
                dgvPrDetails.Columns[12].Name = "GroupId";
                dgvPrDetails.Columns[13].Name = "SubGroup1Id";
                dgvPrDetails.Columns[14].Name = "SubGroup2Id";
                dgvPrDetails.Columns[15].Name = "ItemId";
                dgvPrDetails.Columns[16].Name = "GelombangId";
                dgvPrDetails.Columns[17].Name = "BracketId";
                dgvPrDetails.Columns[18].Name = "Price";
                dgvPrDetails.Columns[19].Name = "SeqNoGroup";
                dgvPrDetails.Columns[20].Name = "BracketDesc";
                dgvPrDetails.Columns[21].Name = "StatusSM";
                dgvPrDetails.Columns[22].Name = "StatusPM";
                dgvPrDetails.Columns[23].Name = "step";
            }
            int SeqNoGroup = CheckSeqNoGroup();
            this.dgvPrDetails.Rows.Add((dgvPrDetails.RowCount + 1).ToString(), FullItemId, ItemName, "0", "", Base, "", "", "", "", "", "", GroupId, SubGroup1Id, SubGroup2Id, ItemId, GelombangId, BracketId, Price, SeqNoGroup);
            
            DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
            combo.Items.Clear();
           
            Conn = ConnectionString.GetConnection();
            Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + FullItemId + "' ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
           

            while (Dr.Read())
            {
                combo.Items.Add(Dr[0].ToString());
                combo.Items.Add(Dr[1].ToString());

                if (cmbPrType.Text == "QTY")
                {
                    combo.Value = Dr[1].ToString();
                }
            }
            Dr.Close();
    
           
            dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[4] = combo;
           
            dgvPrDetails.ReadOnly = false;
            dgvPrDetails.Columns["No"].ReadOnly = true;
            dgvPrDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPrDetails.Columns["ItemName"].ReadOnly = true;
            dgvPrDetails.Columns["SalesSO"].ReadOnly = true;
            dgvPrDetails.Columns["Base"].ReadOnly = true;
            dgvPrDetails.Columns["BracketDesc"].ReadOnly = true;
            dgvPrDetails.Columns["Vendor"].ReadOnly = true;
            
            dgvPrDetails.Columns["StatusSM"].Visible = false;
            dgvPrDetails.Columns["StatusPM"].Visible = false;
            dgvPrDetails.Columns["step"].Visible = false;
            
            if (cmbPrType.Text == "FIX")
            {
                dgvPrDetails.Columns["Unit"].ReadOnly = false;
            }
            else
            {
                dgvPrDetails.Columns["Unit"].ReadOnly = true;
            }
            
            dgvPrDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["GelombangId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;

            //dgvPrDetails.Columns["SalesSO"].Visible = true;
            //dgvPrDetails.Columns["Unit"].Visible = true;
            dgvPrDetails.Columns["GroupId"].Visible = false;
            dgvPrDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPrDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPrDetails.Columns["ItemId"].Visible = false;
            dgvPrDetails.Columns["GelombangId"].Visible = false;
            dgvPrDetails.Columns["BracketId"].Visible = false;
            dgvPrDetails.Columns["Price"].Visible = false;
            dgvPrDetails.Columns["SeqNoGroup"].Visible = false;
            if (cmbPrType.Text == "FIX")
            {
                dgvPrDetails.Columns["Base"].Visible = false;
                dgvPrDetails.Columns["BracketDesc"].Visible = false;
            }
            else
            {
                dgvPrDetails.Columns["Base"].Visible = true;
                dgvPrDetails.Columns["BracketDesc"].Visible = true;
            }

            //dgvPrDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPrDetails.AutoResizeColumns();
            //InvStockDetail.Clear();
            //InvStockDetail = GelombangId;
        }

        //Check SeqNoGroup yang belum digunakan.
        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvPrDetails.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value) == j)
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

        public void AddDataGridDetail(List<string> GroupId, List<string> SubGroup1Id, List<string> SubGroup2Id, List<string> ItemId, List<string> FullItemId, List<string> ItemName, List<string> GelombangId, List<string> BracketId, List<string> Base, List<string> Price)
        {
            int SeqNoGroup = CheckSeqNoGroup();
            if (dgvPrDetails.RowCount - 1 <= 0)
            {
                dgvPrDetails.ColumnCount = 24;
                dgvPrDetails.Columns[0].Name = "No";
                dgvPrDetails.Columns[1].Name = "FullItemID";
                dgvPrDetails.Columns[2].Name = "ItemName";
                dgvPrDetails.Columns[3].Name = "Qty";
                dgvPrDetails.Columns[4].Name = "Unit";
                dgvPrDetails.Columns[5].Name = "Base";
                dgvPrDetails.Columns[6].Name = "Vendor";
                dgvPrDetails.Columns[7].Name = "DeliveryMethod";
                dgvPrDetails.Columns[8].Name = "SalesSO";
                dgvPrDetails.Columns[9].Name = "ExpectedDateFrom";
                dgvPrDetails.Columns[10].Name = "ExpectedDateTo"; ;
                dgvPrDetails.Columns[11].Name = "Deskripsi";
                dgvPrDetails.Columns[12].Name = "GroupId";
                dgvPrDetails.Columns[13].Name = "SubGroup1Id";
                dgvPrDetails.Columns[14].Name = "SubGroup2Id";
                dgvPrDetails.Columns[15].Name = "ItemId";
                dgvPrDetails.Columns[16].Name = "GelombangId";
                dgvPrDetails.Columns[17].Name = "BracketId";
                dgvPrDetails.Columns[18].Name = "Price";
                dgvPrDetails.Columns[19].Name = "SeqNoGroup";
                dgvPrDetails.Columns[20].Name = "BracketDesc";
                dgvPrDetails.Columns[21].Name = "StatusSM";
                dgvPrDetails.Columns[22].Name = "StatusPM";
                dgvPrDetails.Columns[23].Name = "step";
            }

            for (int i = 0; i < FullItemId.Count; i++)
            {
                this.dgvPrDetails.Rows.Add((dgvPrDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], "0", "", Base[i], "", "", "", "", "", "", GroupId[i], SubGroup1Id[i], SubGroup2Id[i], ItemId[i], GelombangId[i], BracketId[i], Price[i], SeqNoGroup);
                SeqNoGroup += 1;

                Conn = ConnectionString.GetConnection();
                Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + FullItemId[i] + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                while (Dr.Read())
                {
                    combo.Items.Add(Dr[0].ToString());
                    combo.Items.Add(Dr[1].ToString());
                }
                Dr.Close();
                dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[4] = combo;

                //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Qty"].Style.BackColor = Color.LightPink;
                //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Vendor"].Style.BackColor = Color.LightYellow;
                //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["DeliveryMethod"].Style.BackColor = Color.LightYellow;
                //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["SalesSO"].Style.BackColor = Color.LightYellow;
                //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateFrom"].Style.BackColor = Color.LightYellow;
                //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateTo"].Style.BackColor = Color.LightYellow;
                //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Deskripsi"].Style.BackColor = Color.LightYellow;
            }

            dgvPrDetails.ReadOnly = false;
            dgvPrDetails.Columns["No"].ReadOnly = true;
            dgvPrDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPrDetails.Columns["ItemName"].ReadOnly = true;
            dgvPrDetails.Columns["SalesSO"].ReadOnly = true;
            dgvPrDetails.Columns["Base"].ReadOnly = true;
            dgvPrDetails.Columns["BracketDesc"].ReadOnly = true;
            dgvPrDetails.Columns["Vendor"].ReadOnly = true;
            
            dgvPrDetails.Columns["StatusSM"].Visible = false;
            dgvPrDetails.Columns["StatusPM"].Visible = false;
            dgvPrDetails.Columns["step"].Visible = false;

            dgvPrDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["GelombangId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPrDetails.Columns["Unit"].Visible = true;
            dgvPrDetails.Columns["SalesSO"].Visible = true;
            dgvPrDetails.Columns["GroupId"].Visible = false;
            dgvPrDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPrDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPrDetails.Columns["ItemId"].Visible = false;
            dgvPrDetails.Columns["GelombangId"].Visible = false;
            dgvPrDetails.Columns["BracketId"].Visible = false;
            dgvPrDetails.Columns["Price"].Visible = false;
            dgvPrDetails.Columns["SeqNoGroup"].Visible = false;
            if (cmbPrType.Text == "FIX")
            {
                dgvPrDetails.Columns["Base"].Visible = false;
                dgvPrDetails.Columns["BracketDesc"].Visible = false;
            }
            else
            {
                dgvPrDetails.Columns["Base"].Visible = true;
                dgvPrDetails.Columns["BracketDesc"].Visible = true;
            }
            dgvPrDetails.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


            dgvPrDetails.AutoResizeColumns();
        }

        public void AddDataGridGelombang(List<string> GroupId, List<string> SubGroup1Id, List<string> SubGroup2Id, List<string> ItemId, List<string> FullItemId, List<string> ItemName, List<string> GelombangId, List<string> BracketId, List<string> Base, List<string> Price, List<string> VendId, List<string> BracketDesc)
        {
            int SeqNoGroup = CheckSeqNoGroup();

            if (dgvPrDetails.RowCount - 1 <= 0)
            {
                dgvPrDetails.ColumnCount = 24;
                dgvPrDetails.Columns[0].Name = "No";
                dgvPrDetails.Columns[1].Name = "FullItemID";
                dgvPrDetails.Columns[2].Name = "ItemName";
                if(cmbPrType.Text != "AMOUNT")
                    dgvPrDetails.Columns[3].Name = "Qty";
                else if (cmbPrType.Text == "AMOUNT")
                    dgvPrDetails.Columns[3].Name = "Amount";
                dgvPrDetails.Columns[4].Name = "Unit";
                dgvPrDetails.Columns[5].Name = "Base";
                dgvPrDetails.Columns[6].Name = "Vendor";
                dgvPrDetails.Columns[7].Name = "DeliveryMethod";
                dgvPrDetails.Columns[8].Name = "SalesSO";
                dgvPrDetails.Columns[9].Name = "ExpectedDateFrom";
                dgvPrDetails.Columns[10].Name = "ExpectedDateTo"; ;
                dgvPrDetails.Columns[11].Name = "Deskripsi";
                dgvPrDetails.Columns[12].Name = "GroupId";
                dgvPrDetails.Columns[13].Name = "SubGroup1Id";
                dgvPrDetails.Columns[14].Name = "SubGroup2Id";
                dgvPrDetails.Columns[15].Name = "ItemId";
                dgvPrDetails.Columns[16].Name = "GelombangId";
                dgvPrDetails.Columns[17].Name = "BracketId";
                dgvPrDetails.Columns[18].Name = "Price";
                dgvPrDetails.Columns[19].Name = "SeqNoGroup";
                dgvPrDetails.Columns[20].Name = "BracketDesc";
                dgvPrDetails.Columns[21].Name = "StatusSM";
                dgvPrDetails.Columns[22].Name = "StatusPM"; 
                dgvPrDetails.Columns[23].Name = "step";
            }

            for (int i = 0; i < FullItemId.Count; i++)
            {

                this.dgvPrDetails.Rows.Add((dgvPrDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], "0", "", Base[i], VendId[i], "", "", "", "", "", GroupId[i], SubGroup1Id[i], SubGroup2Id[i], ItemId[i], GelombangId[i], BracketId[i], Price[i], SeqNoGroup, BracketDesc[i]);

                Conn = ConnectionString.GetConnection();
                Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + FullItemId[i] + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                if (Base[i].ToString() != "N")
                {

                    while (Dr.Read())
                    {
                        combo.Items.Add(Dr[0].ToString());
                        combo.Items.Add(Dr[1].ToString());

                        if (cmbPrType.Text == "QTY")
                        {
                            combo.Value = Dr[1].ToString();
                        }
                    }

                    
                   
                    dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[4] = combo;
                    
                }
                else
                {
                    dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].ReadOnly = true;
                }
                
            }

            dgvPrDetails.Columns["StatusSM"].Visible = false;
            dgvPrDetails.Columns["StatusPM"].Visible = false;
            dgvPrDetails.Columns["step"].Visible = false;

            dgvPrDetails.Columns["GroupId"].Visible = false;
            dgvPrDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPrDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPrDetails.Columns["ItemId"].Visible = false;
            dgvPrDetails.Columns["GelombangId"].Visible = false;
            dgvPrDetails.Columns["BracketId"].Visible = false;
            dgvPrDetails.Columns["Price"].Visible = false;
            dgvPrDetails.Columns["SeqNoGroup"].Visible = false;
            dgvPrDetails.Columns["BracketDesc"].Visible = true;
            dgvPrDetails.Columns["SalesSO"].Visible = false;
            if (cmbPrType.Text == "AMOUNT")
                dgvPrDetails.Columns["Unit"].Visible = false;
            else
                dgvPrDetails.Columns["Unit"].Visible = true;

            dgvPrDetails.ReadOnly = false;
            dgvPrDetails.Columns["No"].ReadOnly = true;
            dgvPrDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPrDetails.Columns["ItemName"].ReadOnly = true;
            dgvPrDetails.Columns["SalesSO"].ReadOnly = true;
            dgvPrDetails.Columns["Base"].ReadOnly = true;
            dgvPrDetails.Columns["Vendor"].ReadOnly = true;
            dgvPrDetails.Columns["BracketDesc"].ReadOnly = true;

            if (cmbPrType.Text == "QTY")
            {
                dgvPrDetails.Columns["Unit"].ReadOnly = true;
            }
            else
            {
                dgvPrDetails.Columns["Unit"].ReadOnly = false;            
            }

            //dgvPrDetails.Columns["D1"].ReadOnly = true;
            //dgvPrDetails.Columns["D2"].ReadOnly = true;
            //dgvPrDetails.Columns["D3"].ReadOnly = true;
            //dgvPrDetails.Columns["D4"].ReadOnly = true;
            dgvPrDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            if (cmbPrType.Text != "AMOUNT")
                dgvPrDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            else if (cmbPrType.Text == "AMOUNT")
                dgvPrDetails.Columns["AMOUNT"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;

            //dgvPrDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPrDetails.AutoResizeColumns();

            //InvStockDetail = InvStockId;
        }

        private void CallFormGelombang(string GelombangId, string BracketId)
        {
            if (GelombangId != "")
            {
                if (cmbPrType.Text != "FIX")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "Select count([GelombangId]) From [InventGelombangD] Where GelombangId in (Select GelombangId from InventGelombangD where GelombangId = '" + GelombangId + "' and BracketId='" + BracketId + "')";

                    Cmd = new SqlCommand(Query, Conn);
                    int CountChk = Convert.ToInt32(Cmd.ExecuteScalar());

                    if (CountChk > 0)
                    {
                        Gelombang Gelombang = new Gelombang();

                        ListGelombang.Add(Gelombang);
                        Gelombang.SetParentForm(this);
                        Gelombang.GetInventStockId(GelombangId, BracketId);
                        Gelombang.ShowDialog();
                    }
                }
            }
        }

        public string GetInvStockId()
        {
            //string InvStockId = "";

            //if (dgvPrDetails.RowCount > 0)
            //{
            //    for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            //    {
            //        if (i == 0)
            //        {
            //            InvStockId += "and FullItemID not in ('";
            //            InvStockId += dgvPrDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString();
            //            InvStockId += "'";
            //        }
            //        else
            //        {
            //            InvStockId += ",'";
            //            InvStockId += dgvPrDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString();
            //            InvStockId += "'";
            //        }
            //    }
            //    InvStockId += ")";
            //    return InvStockId;
            //}
            //else
            //{
            //    InvStockId = "";
            //    return InvStockId;
            //}
            return "";
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvPrDetails.RowCount ; i++)
            {
                dgvPrDetails.Rows[i].Cells["No"].Value = i+1;
            }
        }

        public void ModeNew()
        {
            PRNumber = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            dtPrDate.Enabled = false;
            cmbPrType.Enabled = true;
            txtPrStatus.Enabled = false;

        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnNew.Enabled = true;
            btnDelete.Enabled = true;
            dtPrDate.Enabled = false;
            cmbPrType.Enabled = false;
            txtPrStatus.Enabled = false;

            dgvPrDetails.ReadOnly = false;
            dgvPrDetails.Columns["No"].ReadOnly = true;
            dgvPrDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPrDetails.Columns["ItemName"].ReadOnly = true;
            dgvPrDetails.Columns["SalesSO"].ReadOnly = true;
            dgvPrDetails.Columns["Base"].ReadOnly = true;
            dgvPrDetails.Columns["BracketDesc"].ReadOnly = true;
            dgvPrDetails.Columns["Vendor"].ReadOnly = true;
            dgvPrDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["PRDate"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            if(cmbPrType.Text!="AMOUNT")
                dgvPrDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPrDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvPrDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;
            if (cmbPrType.Text == "FIX")
            {
                dgvPrDetails.Columns["Unit"].ReadOnly = false;
            }
            else
            {
                dgvPrDetails.Columns["Unit"].ReadOnly = true;
            }
            dgvPrDetails.AutoResizeColumns();
            EditColor();
            dgvPrDetails.DefaultCellStyle.BackColor = Color.White;
        }

        private void EditColor()
        {
            int jgnedit = 0;
            for (int i = 0; i < dgvPrDetails.RowCount; i++)
            {
                sm = dgvPrDetails.Rows[i].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusSM"].Value.ToString();
                pm = dgvPrDetails.Rows[i].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusPM"].Value.ToString();
                if (sm == "Yes" || sm == "No")
                {
                    jgnedit = jgnedit + 1;
                }
                if (pm == "Yes" || pm == "No")
                {
                    jgnedit = jgnedit + 1;
                }
                
                if (pm == "Revision")
                {
                    jgnedit = jgnedit - 1;
                }


                if (jgnedit < 1)
                {
                    if (cmbPrType.Text != "FIX")
                    {
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                        {
                            if (cmbPrType.Text != "AMOUNT")
                                dgvPrDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.White;
                            else if (cmbPrType.Text != "AMOUNT")
                                dgvPrDetails.Rows[i].Cells["Amount"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["Vendor"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["SalesSO"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["Deskripsi"].Style.BackColor = Color.White;
                        }
                        else
                        {
                            if (cmbPrType.Text != "AMOUNT")
                                dgvPrDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.LightGray;
                            else if (cmbPrType.Text != "AMOUNT")
                                dgvPrDetails.Rows[i].Cells["Amount"].Style.BackColor = Color.LightGray;
                            dgvPrDetails.Rows[i].Cells["Vendor"].Style.BackColor = Color.LightGray;
                            dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.LightGray;
                            dgvPrDetails.Rows[i].Cells["SalesSO"].Style.BackColor = Color.LightGray;
                            dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Style.BackColor = Color.LightGray;
                            dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Style.BackColor = Color.LightGray;
                            dgvPrDetails.Rows[i].Cells["Deskripsi"].Style.BackColor = Color.LightGray;
                        }
                    }
                    else
                    {
                        dgvPrDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.White;
                        dgvPrDetails.Rows[i].Cells["Vendor"].Style.BackColor = Color.White;
                        dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.White;
                        dgvPrDetails.Rows[i].Cells["SalesSO"].Style.BackColor = Color.White;
                        dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Style.BackColor = Color.White;
                        dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Style.BackColor = Color.White;
                        dgvPrDetails.Rows[i].Cells["Deskripsi"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    dgvPrDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvPrDetails.Rows[i].ReadOnly = true;
                }
                jgnedit = 0;
            }
        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvPrDetails.RowCount; i++)
            {
                if (cmbPrType.Text != "AMOUNT")
                    dgvPrDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.LightGray;
                else if (cmbPrType.Text != "AMOUNT")
                    dgvPrDetails.Rows[i].Cells["Amount"].Style.BackColor = Color.LightGray;
                dgvPrDetails.Rows[i].Cells["Vendor"].Style.BackColor = Color.LightGray;
                dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.LightGray;
                dgvPrDetails.Rows[i].Cells["SalesSO"].Style.BackColor = Color.LightGray;
                dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Style.BackColor = Color.LightGray;
                dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Style.BackColor = Color.LightGray;
                dgvPrDetails.Rows[i].Cells["Deskripsi"].Style.BackColor = Color.LightGray;
            }
        }
        
        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            dtPrDate.Enabled = false;
            cmbPrType.Enabled = false;
            txtPrStatus.Enabled = false;

            dgvPrDetails.ReadOnly = true;
            BeforeEditColor();
            dgvPrDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }
        //tia edit
        public void ModePopUp()
        {
            Mode = "PopUp";

            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnNew.Visible = false;
            btnDelete.Visible = false;


            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            dtPrDate.Enabled = false;
            cmbPrType.Enabled = false;
            txtPrStatus.Enabled = false;

            dgvPrDetails.ReadOnly = true;
            BeforeEditColor();
            dgvPrDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }
        //tia edit end

        public void ModeView()
        {
            Mode = "ModeView";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnNew.Visible = false;
            btnDelete.Visible = false;


            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            dtPrDate.Enabled = false;
            cmbPrType.Enabled = false;
            txtPrStatus.Enabled = false;

            dgvPrDetails.ReadOnly = true;
            BeforeEditColor();
            dgvPrDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }

        public void GetDataHeader()
        {
            if (PRNumber == "")
            {
                PRNumber = txtPrNumber.Text.Trim();
            }
            else
            {
                Conn = ConnectionString.GetConnection();

                Query = "Select a.[PurchReqID],a.[OrderDate],a.[TransType],a.[TransStatus],b.Deskripsi,a.[ApprovedBy] From [PurchRequisitionH] a ";
                Query += "left join TransStatusTable b on a.TransStatus = b.StatusCode And TransCode = 'PR' ";
                Query += "Where PurchReqID = '" + PRNumber + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtPrNumber.Text = Dr["PurchReqID"].ToString();
                    dtPrDate.Text = Dr["OrderDate"].ToString();
                    cmbPrType.SelectedItem = Dr["TransType"].ToString();
                    txtPrStatus.Text = Dr["TransStatus"].ToString();
                    txtPrApproved.Text = Dr["ApprovedBy"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString().ToUpper();
                }
                Dr.Close();

                dgvPrDetails.Rows.Clear();
                if (dgvPrDetails.RowCount - 1 <= 0)
                {
                    dgvPrDetails.ColumnCount = 24;
                    dgvPrDetails.Columns[0].Name = "No";
                    dgvPrDetails.Columns[1].Name = "FullItemID";
                    dgvPrDetails.Columns[2].Name = "ItemName";
                    if (cmbPrType.Text != "AMOUNT")
                    {
                        dgvPrDetails.Columns[3].Name = "Qty";
                    }
                    else if (cmbPrType.Text == "AMOUNT")
                    {
                        dgvPrDetails.Columns[3].Name = "Amount";
                    }
                    dgvPrDetails.Columns[4].Name = "Unit";
                    dgvPrDetails.Columns[5].Name = "Base";
                    dgvPrDetails.Columns[6].Name = "Vendor";
                    dgvPrDetails.Columns[7].Name = "DeliveryMethod";
                    dgvPrDetails.Columns[8].Name = "SalesSO";
                    dgvPrDetails.Columns[9].Name = "ExpectedDateFrom";
                    dgvPrDetails.Columns[10].Name = "ExpectedDateTo"; ;
                    dgvPrDetails.Columns[11].Name = "Deskripsi";
                    dgvPrDetails.Columns[12].Name = "GroupId";
                    dgvPrDetails.Columns[13].Name = "SubGroup1Id";
                    dgvPrDetails.Columns[14].Name = "SubGroup2Id";
                    dgvPrDetails.Columns[15].Name = "ItemId";
                    dgvPrDetails.Columns[16].Name = "GelombangId";
                    dgvPrDetails.Columns[17].Name = "BracketId";
                    dgvPrDetails.Columns[18].Name = "Price";
                    dgvPrDetails.Columns[19].Name = "SeqNoGroup";
                    dgvPrDetails.Columns[20].Name = "BracketDesc";
                    dgvPrDetails.Columns[21].Name = "StatusSM";
                    dgvPrDetails.Columns[22].Name = "StatusPM";
                    dgvPrDetails.Columns[23].Name = "step";
                    //dgvPrDetails.Columns[10].Name = "D1";
                    //dgvPrDetails.Columns[11].Name = "D2";
                    //dgvPrDetails.Columns[12].Name = "D3";
                    //dgvPrDetails.Columns[13].Name = "D4";
                }

                if (cmbPrType.Text != "AMOUNT")
                    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[Qty],[Unit],[Base],[VendID] Vendor,[DeliveryMethod],[ReffTransID] SalesSO,[ExpectedDateFrom],[ExpectedDateTo],[Deskripsi],GroupId,SubGroup1Id,SubGroup2Id,ItemId,GelombangId,BracketId,Price,SeqNoGroup,BracketDesc,TransStatus,TransStatusPurch,step From [PurchRequisition_Dtl] Where PurchReqID = '" + PRNumber + "' order by SeqNo asc";
                else if (cmbPrType.Text == "AMOUNT")
                    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[Amount],[Unit],[Base],[VendID] Vendor,[DeliveryMethod],[ReffTransID] SalesSO,[ExpectedDateFrom],[ExpectedDateTo],[Deskripsi],GroupId,SubGroup1Id,SubGroup2Id,ItemId,GelombangId,BracketId,Price,SeqNoGroup,BracketDesc,TransStatus,TransStatusPurch,step From [PurchRequisition_Dtl] Where PurchReqID = '" + PRNumber + "' order by SeqNo asc";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int i = 0;
                while (Dr.Read())
                {

                    string ExpectedDateFrom = Convert.ToDateTime(Dr[9]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr[9]).ToString("dd-MM-yyyy");
                    string ExpectedDateTo = Convert.ToDateTime(Dr[10]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(Dr[10]).ToString("dd-MM-yyyy");

                    if (cmbPrType.Text != "AMOUNT")
                        this.dgvPrDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], ExpectedDateFrom, ExpectedDateTo, Dr[11].ToString().Replace(',', '.'), Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23]);
                    else if (cmbPrType.Text == "AMOUNT")
                        this.dgvPrDetails.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], ExpectedDateFrom, ExpectedDateTo, Dr[11].ToString().Replace(',', '.'), Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23]);

                    Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemID = '" + Dr[1].ToString() + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader DrCmb;
                    DrCmb = Cmd.ExecuteReader();
                    DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
                    if (cmbPrType.Text.Trim() == "FIX")
                    {
                        while (DrCmb.Read())
                        {
                            if (DrCmb[0] != null)
                                combo.Items.Add(DrCmb[0].ToString());
                            if (DrCmb[1] != null)
                                combo.Items.Add(DrCmb[1].ToString());
                        }
                        if (Dr[4] != null)
                        {
                            combo.Value = Dr[4].ToString();
                        }
                        dgvPrDetails.Rows[i].Cells[4] = combo;
                        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Qty"].Style.BackColor = Color.LightPink;
                        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Vendor"].Style.BackColor = Color.LightYellow;
                        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["DeliveryMethod"].Style.BackColor = Color.LightYellow;
                        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["SalesSO"].Style.BackColor = Color.LightYellow;
                        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateFrom"].Style.BackColor = Color.LightYellow;
                        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateTo"].Style.BackColor = Color.LightYellow;
                        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Deskripsi"].Style.BackColor = Color.LightYellow;
                    }
                    else
                    {
                        if (Dr[5].ToString() != "N")
                        {
                            while (DrCmb.Read())
                            {
                                if (DrCmb[0] != null)
                                    combo.Items.Add(DrCmb[0].ToString());
                                if (DrCmb[1] != null)
                                    combo.Items.Add(DrCmb[1].ToString());
                            }
                            if (Dr[4] != null)
                            {
                                combo.Value = Dr[4].ToString();
                            }
                            dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[4] = combo;
                            //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Qty"].Style.BackColor = Color.LightPink;
                            //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Vendor"].Style.BackColor = Color.LightYellow;
                            //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["DeliveryMethod"].Style.BackColor = Color.LightYellow;
                            //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["SalesSO"].Style.BackColor = Color.LightYellow;
                            //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateFrom"].Style.BackColor = Color.LightYellow;
                            //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["ExpectedDateTo"].Style.BackColor = Color.LightYellow;
                            //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells["Deskripsi"].Style.BackColor = Color.LightYellow;
                        }
                        else
                        {
                            dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].DefaultCellStyle.BackColor = Color.LightGray;
                            dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].ReadOnly = true;
                        }
                    }
                    //Query = "Select DeliveryMethod From dbo.[DeliveryMethod]";
                    //Cmd = new SqlCommand(Query, Conn);
                    //SqlDataReader DrDeliveryMethod;
                    //DrDeliveryMethod = Cmd.ExecuteReader();
                    //DataGridViewComboBoxCell DeliveryMethod = new DataGridViewComboBoxCell();
                    //while (DrCmb.Read())
                    //{
                    //    DeliveryMethod.Items.Add(DrCmb[0].ToString());
                    //}
                    //if (Dr[3] != null)
                    //{
                    //    DeliveryMethod.Value = Dr[3].ToString();
                    //}

                    //dgvPrDetails.Rows[i].Cells[3] = DeliveryMethod;
                    //dgvPrDetails.Rows[i].Cells[10].Value = "...";
                    //dgvPrDetails.Rows[i].Cells[11].Value = "...";
                    //dgvPrDetails.Rows[i].Cells[12].Value = "...";
                    //dgvPrDetails.Rows[i].Cells[13].Value = "...";
                    //dgvPrDetails.Rows[i].Cells[7].Value = Dr[7];
                    i++;

                    
                }
                Dr.Close();

                dgvPrDetails.ReadOnly = false;
                dgvPrDetails.Columns["No"].ReadOnly = true;
                dgvPrDetails.Columns["FullItemID"].ReadOnly = true;
                dgvPrDetails.Columns["ItemName"].ReadOnly = true;
                dgvPrDetails.Columns["SalesSO"].ReadOnly = true;
                dgvPrDetails.Columns["Base"].ReadOnly = true;
                dgvPrDetails.Columns["BracketDesc"].ReadOnly = true;
                //dgvPrDetails.Columns["D1"].ReadOnly = true;
                //dgvPrDetails.Columns["D2"].ReadOnly = true;
                //dgvPrDetails.Columns["D3"].ReadOnly = true;
                //dgvPrDetails.Columns["D4"].ReadOnly = true;
                dgvPrDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
                if (cmbPrType.Text != "AMOUNT")
                    dgvPrDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                else if (cmbPrType.Text == "AMOUNT")
                    dgvPrDetails.Columns["Amount"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["GelombangId"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPrDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvPrDetails.Columns["StatusSM"].Visible = false;
                dgvPrDetails.Columns["StatusPM"].Visible = false;
                dgvPrDetails.Columns["step"].Visible = false;

                dgvPrDetails.Columns["GroupId"].Visible = false;
                dgvPrDetails.Columns["SubGroup1Id"].Visible = false;
                dgvPrDetails.Columns["SubGroup2Id"].Visible = false;
                dgvPrDetails.Columns["ItemId"].Visible = false;
                dgvPrDetails.Columns["GelombangId"].Visible = false;
                dgvPrDetails.Columns["BracketId"].Visible = false;
                dgvPrDetails.Columns["Price"].Visible = false;
                dgvPrDetails.Columns["SeqNoGroup"].Visible = false;
                if (cmbPrType.Text == "AMOUNT")
                    dgvPrDetails.Columns["Unit"].Visible = false;
                if (Purchase.PurchaseQuotation.FormPQ.reffID != null || Purchase.PurchaseQuotation.FormPQ.pRID != null)
                {
                    dgvPrDetails.ReadOnly = true;
                    dgvPrDetails.DefaultCellStyle.BackColor = Color.LightGray;
                }
                if (cmbPrType.Text == "FIX")
                {
                    dgvPrDetails.Columns["Base"].Visible = false;
                    dgvPrDetails.Columns["BracketDesc"].Visible = false;
                }
                else
                {
                    dgvPrDetails.Columns["Base"].Visible = true;
                    dgvPrDetails.Columns["BracketDesc"].Visible = true;
                }

                if (cmbPrType.Text != "AMOUNT")
                    dgvPrDetails.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                else if (cmbPrType.Text == "AMOUNT")
                    dgvPrDetails.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvPrDetails.AutoResizeColumns();
            }
            
        }

        string Notification = "";
        string CreatedBy = "";
        DateTime CreatedDate = DateTime.Now;

        private void saveNew()
        {
            #region Create new PR Number
            string Jenis = "PR", Kode = "PR";
            string PRNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);
            Query = "Insert into PurchRequisitionH (PurchReqID,OrderDate,TransType,[TransStatus],ApprovedBy,CreatedDate,CreatedBy) values( ";
            Query += "'" + PRNumber + "',";
            Query += "'" + dtPrDate.Value.ToString("yyyy-MM-dd") + "','" + cmbPrType.Text + "',";
            Query += "'01','" + txtPrApproved.Text + "',getdate(),'" + ControlMgr.UserId + "');";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();
            #endregion

            #region Insert PurchRequisition_Dtl
            Query = "";
            for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            {
                if (dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == null || dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == "")
                    dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value = "01-01-1900";
                if (dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == null || dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == "")
                    dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value = "01-01-1900";

                if (cmbPrType.Text != "AMOUNT")
                    Query += "Insert PurchRequisition_Dtl (PurchReqID,SeqNo,FullItemID,ItemName,DeliveryMethod,OrderDate,ReffTransID,ExpectedDateFrom,ExpectedDateTo,VendID,Deskripsi,Qty,Unit,Ratio, GroupId, SubGroup1Id, SubGroup2Id, ItemId, GelombangId, BracketId, Base, Price, SeqNoGroup,BracketDesc,TransStatus,CreatedDate,CreatedBy) Values ";
                if (cmbPrType.Text == "AMOUNT")
                    Query += "Insert PurchRequisition_Dtl (PurchReqID,SeqNo,FullItemID,ItemName,DeliveryMethod,OrderDate,ReffTransID,ExpectedDateFrom,ExpectedDateTo,VendID,Deskripsi,Amount,Qty,Unit,Ratio, GroupId, SubGroup1Id, SubGroup2Id, ItemId, GelombangId, BracketId, Base, Price, SeqNoGroup,BracketDesc,TransStatus,CreatedDate,CreatedBy) Values ";
                Query += "('" + PRNumber + "','";
                Query += (dgvPrDetails.Rows[i].Cells["No"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["No"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                Query += (dtPrDate.Value == null ? "" : dtPrDate.Value.ToString("yyyy-MM-dd")) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SalesSO"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SalesSO"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == null ? "" : FormateDateddmmyyyy(dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value.ToString())) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == null ? "" : FormateDateddmmyyyy(dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value.ToString())) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Vendor"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Vendor"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Deskripsi"].Value.ToString().Trim()) + "','";
                if (cmbPrType.Text != "AMOUNT")
                {
                    Query += ((dgvPrDetails.Rows[i].Cells["Qty"].Value == "" ? 0.00 : Double.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString()))) + "','";
                    Query += (dgvPrDetails.Rows[i].Cells["Unit"].Value == null ? "'," : dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString()) + "',";
                }
                else if (cmbPrType.Text == "AMOUNT")
                {
                    Query += ((dgvPrDetails.Rows[i].Cells["Amount"].Value == "" ? 0.0000 : Double.Parse(dgvPrDetails.Rows[i].Cells["Amount"].Value.ToString()))) + "','";
                    Query += "1','";//(dgvPrDetails.Rows[i].Cells["Qty"].Value == "" ? 0.00 : Double.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString())) + "','";
                    Query += "KG',";//(dgvPrDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString()) + "',";
                }
                Query += "(select top 1 [Ratio] from [dbo].[InventConversion] where FullItemID='" + dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString() + "' ),'";
                Query += (dgvPrDetails.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["GelombangId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["GelombangId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["BracketId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["BracketId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Base"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Base"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Price"].Value == null ? "0.00" : dgvPrDetails.Rows[i].Cells["Price"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["BracketDesc"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["BracketDesc"].Value.ToString()) + "',";
                Query += "'',getdate(),";
                Query += "'" + ControlMgr.UserId + "');";

                if (i % 5 == 0 && i > 0)
                {
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    Query = "";
                }

            }
            #endregion            

            #region Return to zero
            if (Query != "")
            {
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                Query = "";
            }

            string FullItemId = "";
            string Unit = "";
            string UoM = "";
            decimal ConvRatio = 0;
            string QueryTemp = "";
            decimal QtyUoM = 0;
            decimal QtyAlt = 0;
            decimal QtyPRIssued_UoM = 0;
            decimal QtyPRIssued_Alt = 0;
            #endregion

            #region Update Invent_Purch_Qty (QtyPRIssued)
            //Update QtyPRIssued
            if (cmbPrType.Text != "AMOUNT")
            {
                #region NOT Amount
                for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
                {
                    FullItemId = dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString();
                    QtyInput = decimal.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString());
                    Unit = dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString();
                    ConvRatio = 0;
                    step = dgvPrDetails.Rows[i].Cells["step"].Value == null ? 0 : decimal.Parse(dgvPrDetails.Rows[i].Cells["step"].Value.ToString());
                    
                    QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            if (Dr["UoM"] == System.DBNull.Value)
                            {
                                MessageBox.Show("UoM Item belom ada dalam tabel InventTable");
                                Dr.Close();
                                return;
                            }
                            else
                            {
                                UoM = Dr["UoM"].ToString();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Item belom terdaftar dalam tabel InventTable");
                        Dr.Close();
                        return;
                    }
                    Dr.Close();

                    QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            if (Dr["Ratio"] == System.DBNull.Value)
                            {
                                MessageBox.Show("Ratio Item belom ada dalam tabel InventConversion");
                                Dr.Close();
                                return;
                            }
                            else
                            {
                                ConvRatio = Convert.ToDecimal(Dr["Ratio"]);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Item belom terdaftar dalam tabel InventConversion");
                        Dr.Close();
                        return;
                    }
                    Dr.Close();

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


                    QueryTemp = "Select PR_Issued_UoM, PR_Issued_Alt From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            QtyPRIssued_UoM = decimal.Parse(Dr["PR_Issued_UoM"].ToString());
                            QtyPRIssued_Alt = decimal.Parse(Dr["PR_Issued_Alt"].ToString());
                        }
                    }
                    else
                    {
                            Cmd = new SqlCommand("INSERT INTO Invent_Purchase_Qty ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName]) VALUES ( '" + dgvPrDetails.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["ItemName"].Value.ToString() + "')", Conn);
                            Cmd.ExecuteNonQuery();
                    }

                    Dr.Close();

                    QtyUoM = QtyUoM + QtyPRIssued_UoM;
                    QtyAlt = QtyAlt + QtyPRIssued_Alt;

                    if (step == 0)
                    {
                        Query = "Update Invent_Purchase_Qty Set PR_Issued_UoM = " + QtyUoM + ", PR_Issued_Alt = " + QtyAlt + "  Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    #region Insert to PurchRequisition_LogTable
                    Query2 = "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],Qty_Uom,Qty_Alt) ";
                    Query2 += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + PRNumber + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["No"].Value.ToString() + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + QtyUoM + "','" + QtyAlt + "')";
                    Cmd = new SqlCommand(Query2, Conn);
                    Cmd.ExecuteNonQuery();
                    #endregion

                    Query = "";
                }
                #endregion
            }

            if (cmbPrType.Text == "AMOUNT")
            {
                Double Amount = 0.0000;
                #region Amount
                for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
                {
                    Amount = dgvPrDetails.Rows[i].Cells["Amount"].Value == "" ? 0.0000 : Double.Parse(dgvPrDetails.Rows[i].Cells["Amount"].Value.ToString());
                    step = dgvPrDetails.Rows[i].Cells["step"].Value == null ? 0 : decimal.Parse(dgvPrDetails.Rows[i].Cells["step"].Value.ToString());

                    if (step == 0)
                    {
                        //TmpQty = Amount
                        Query = "Update Invent_Purchase_Qty Set PR_Issued_Amount = (PR_Issued_Amount + " + Amount + ")  Where FullItemID = '" + (dgvPrDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    //Steven Edit save to PurchRequisition_LogTable
                    Query2 = "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],Amount) ";
                    Query2 += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + PRNumber + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["No"].Value.ToString() + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + Amount + "')";
                    Cmd = new SqlCommand(Query2, Conn);
                    Cmd.ExecuteNonQuery();
                    //Steven Edit save to PurchRequisition_LogTable
                }
                #endregion
            }
            #endregion

            #region Insert WorkFlowLogTable
            Query = "Insert WorkflowLogTable (ReffTableName,ReffID,ReffDate,ReffSeqNo,UserID,WorkFlow,LogStatus,StatusDesc,LogDate) Values ";
            Query += "('PurchRequisitionH','";
            Query += PRNumber + "','";
            Query += dtPrDate.Value.ToString("yyyy-MM-dd") + "','";
            Query += "0','";
            Query += ControlMgr.UserId + "','";
            Query += "Sales','";
            Query += "01','";
            Query += "Created by Requestor',";
            Query += "getdate());";
            Query += "Update PurchRequisitionH set TransStatus='01' where PurchReqID='" + PRNumber + "';";
            #endregion


            if (Query != "")
            {
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
            Notification = "Data PRNumber : " + PRNumber + " berhasil ditambahkan.";
            txtPrNumber.Text = PRNumber;
            GetDataHeader();
            MainMenu f = new MainMenu();
            f.refreshTaskList();                
        }

        private void saveEdit()
        {
            #region declare object, set to 0
            //Sebelum di delete, hrs bandingin qty lama dan baru
            string FullItemId = "";
            string Unit = "";
            string UoM = "";
            decimal ConvRatio = 0;
            string QueryTemp = "";
            decimal QtyOld = 0;
            decimal QtyUoMOld = 0;
            decimal QtyAltOld = 0;

            decimal QtyNew = 0;
            decimal QtyUoMNew = 0;
            decimal QtyAltNew = 0;
            decimal QtyPRIssued_UoM = 0;
            decimal QtyPRIssued_Alt = 0;
            string TransStatusPurch = "";

            //edit
            string UnitOld = "";
            int JumlahPR = 0;
            int JumlahRow = 0;
            int k = 0;
            #endregion

            #region Update Invent_Purch_Qty
            if (cmbPrType.Text != "AMOUNT")
            {
                #region NOT Amount
                Query = "SELECT COUNT (PurchReqId) FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                JumlahPR = (int)Cmd.ExecuteScalar();
                JumlahRow = (int)dgvPrDetails.RowCount;

                #region Delete Old InventPurchQty based on PR Detail

                for (int i = 0; i < JumlahPR; i++)
                {
                    k = i + 1;
                    step = dgvPrDetails.Rows[i].Cells["step"].Value == null ? 0 : decimal.Parse(dgvPrDetails.Rows[i].Cells["step"].Value.ToString());
                    
                    //sm adalah status approval sales manager, pm adalah status approval dari purchase manager, bila status masih kosong atau status revisi maka qty akan diupdate di stockview
                    sm = dgvPrDetails.Rows[i].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusSM"].Value.ToString();
                    pm = dgvPrDetails.Rows[i].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusPM"].Value.ToString();
                    
                    //step=1 apabila ini lemparan dari revisi, bila barang baru maka step = 0
                    //barang baru tidak perlu hapus qty lama, sehingga langsung ditambahkan qty baru ke stock view
                    if ((sm == "" || sm == "Revision" || pm == "" || pm == "Revision") && step == 1)
                    {
                        #region detail data yg diperlukan untuk update stockview
                        
                        FullItemId = "";
                        QueryTemp = "Select [FullItemId] From [PurchRequisition_Dtl] Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        FullItemId = Cmd.ExecuteScalar().ToString();

                        UnitOld = "";
                        QueryTemp = "Select [Unit] From [PurchRequisition_Dtl] Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        UnitOld = Cmd.ExecuteScalar().ToString();

                        QtyOld = 0;
                        QueryTemp = "Select Qty From PurchRequisition_Dtl Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        QtyOld = (decimal)Cmd.ExecuteScalar();

                        ConvRatio = 0;
                        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();

                        UoM = "";
                        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "';";
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

                        QueryTemp = "select TransStatus from PurchRequisitionH Where PurchReqID = '" + PRNumber + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        string TransStatus = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select TransStatusPurch From [PurchRequisition_Dtl] Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        TransStatusPurch = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select TransStatus From [PurchRequisition_Dtl] Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        string TransStatusSales = Cmd.ExecuteScalar().ToString();
                        
                        #endregion
                        
                        #region Add new item to InventPurchQty based on Datagrid


                        Query = "Update Invent_Purchase_Qty Set PR_Issued_UoM = PR_Issued_UoM - " + QtyUoMOld + " , PR_Issued_Alt = PR_Issued_Alt - " + QtyAltOld + "  Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        //if (TransStatusSales == "Yes" && TransStatus == "02")
                        //{
                        //    Query = "Update Invent_Purchase_Qty Set [PR_Approved_UoM] = (PR_Approved_UoM - " + QtyUoMOld + ") , PR_Approved_Alt = (PR_Approved_Alt - " + QtyAltOld + ") Where FullItemID = '" + FullItemId + "'";
                        //    Cmd = new SqlCommand(Query, Conn);
                        //    Cmd.ExecuteNonQuery();
                        //}
                        //else if (TransStatusPurch == "Yes" && TransStatus == "12")
                        //{
                        //    Query = "Update Invent_Purchase_Qty Set [PR_Approved2_UoM] = (PR_Approved2_UoM - " + QtyUoMOld + ") , PR_Approved2_Alt = (PR_Approved2_Alt - " + QtyAltOld + ") Where FullItemID = '" + FullItemId + "'";
                        //    Cmd = new SqlCommand(Query, Conn);
                        //    Cmd.ExecuteNonQuery();
                        //}
                        //else
                        //{
                        //    Query = "Update Invent_Purchase_Qty Set PR_Issued_UoM = PR_Issued_UoM - " + QtyUoMOld + " , PR_Issued_Alt = PR_Issued_Alt - " + QtyAltOld + "  Where FullItemID = '" + FullItemId + "'";
                        //    Cmd = new SqlCommand(Query, Conn);
                        //    Cmd.ExecuteNonQuery();
                        //}
                        #endregion
                    }//hasim breakpoint 1
                }
                #endregion

                #region tambah qty ke table
                for (int i = 0; i < dgvPrDetails.RowCount ; i++)
                {
                    //sm adalah status approval sales manager, pm adalah status approval dari purchase manager, bila status masih kosong atau status revisi maka qty akan ditambah di stockview
                    sm = dgvPrDetails.Rows[i].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusSM"].Value.ToString();
                    pm = dgvPrDetails.Rows[i].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusPM"].Value.ToString();

                    step = dgvPrDetails.Rows[i].Cells["step"].Value == null ? 0 : decimal.Parse(dgvPrDetails.Rows[i].Cells["step"].Value.ToString());
                    
                    #region Add new item to InventPurchQty table based on Datagrid
                    if ((sm == "" || sm == "Revision" || pm == "" || pm == "Revision") && step < 2)
                    {
                        #region inisiasi data 

                        FullItemId = "";
                        Unit = "";
                        UoM = "";
                        QtyUoMNew = 0;
                        QtyAltNew = 0;
                        ConvRatio = 0;

                        FullItemId = dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString();
                        Unit = dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString();


                        QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        UoM = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        ConvRatio = (decimal)Cmd.ExecuteScalar();
                        if (ConvRatio == 0)
                        {
                            ConvRatio = 1;
                            MessageBox.Show("Ratio item " + FullItemId + " bernilai 0 pada tabel InventConversion. Ratio tersebut akan dijadikan nilai 1.");
                        }

                        //Ambil Nilai Qty yang baru dimasukan
                        if (Unit == UoM)
                        {
                            QtyUoMNew = decimal.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString());
                            QtyAltNew = decimal.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString()) * ConvRatio;
                        }
                        else
                        {
                            QtyAltNew = decimal.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString());
                            QtyUoMNew = decimal.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString()) / ConvRatio;
                        }

                        #endregion

                        #region Check ItemID is already in Invent Purchase (if not create new)
                        QueryTemp = "Select PR_Issued_UoM,PR_Issued_Alt From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {

                        }
                        else
                        {
                            Cmd = new SqlCommand("INSERT INTO Invent_Purchase_Qty {[GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName]) VALUES ( '" + dgvPrDetails.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["ItemName"].Value.ToString() + "');", Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        Dr.Close();
                        #endregion

                        #region tambah qty ke table Update Invent_Purchase_Qty
                        Query = "Update Invent_Purchase_Qty Set PR_Issued_UoM = PR_Issued_UoM + " + QtyUoMNew + " , PR_Issued_Alt = PR_Issued_Alt + " + QtyAltNew + "  Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        #endregion

                        Query = "Select PR_Issued_UoM,PR_Issued_Alt From Invent_Purchase_Qty Where FullItemID = '" + dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn);
                    
                        #region Save to PurchRequisition_LogTable
                        Query += "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],Qty_Uom,Qty_Alt) ";
                        Query += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + PRNumber + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["No"].Value.ToString() + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + QtyUoMNew + "','" + QtyAltNew + "');";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        #endregion

                    }
                    #endregion
                    
                }
                #endregion
            }
            else
            {
                #region declare object, set to 0
                FullItemId = "";
                UoM = "";
                QueryTemp = "";
                QtyOld = 0;
                QtyNew = 0;
                JumlahPR = 0;
                JumlahRow = 0;
                #endregion

                Query = "Delete From PurchRequisition_LogTable where PurchReqID='" + PRNumber + "';";

                Query = "SELECT COUNT (PurchReqId) FROM [ISBS-NEW4].[dbo].[PurchRequisition_Dtl] WHERE PurchReqId='" + txtPrNumber.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                JumlahPR = (int)Cmd.ExecuteScalar();
                JumlahRow = (int)dgvPrDetails.RowCount;

                #region hapus qty lama dari table Invent_Purchase_Qty
                for (int i = 0; i < JumlahPR; i++)
                {
                    k = i + 1;
                    step = dgvPrDetails.Rows[i].Cells["step"].Value == null ? 0 : decimal.Parse(dgvPrDetails.Rows[i].Cells["step"].Value.ToString());
                    //sm dan pm sama dengan comment di atas
                    sm = dgvPrDetails.Rows[i].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusSM"].Value.ToString();
                    pm = dgvPrDetails.Rows[i].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusPM"].Value.ToString();
                    
                    if ((sm == "" || sm == "Revision" || pm == "" || pm == "Revision") && step == 1)
                    {
                        
                        #region persiapan variable qty new
                        FullItemId = "";
                        QueryTemp = "Select [FullItemId] From [PurchRequisition_Dtl] Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        FullItemId = Cmd.ExecuteScalar().ToString();

                        QtyOld = 0;
                        QueryTemp = "Select Amount From PurchRequisition_Dtl Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        QtyOld = (decimal)Cmd.ExecuteScalar();

                        QueryTemp = "Select TransStatusPurch From [PurchRequisition_Dtl] Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        TransStatusPurch = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "Select TransStatus From [PurchRequisition_Dtl] Where [SeqNo] = '" + k + "' and PurchReqID = '" + PRNumber + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        string TransStatusSales = Cmd.ExecuteScalar().ToString();

                        QueryTemp = "select TransStatus from PurchRequisitionH Where PurchReqID = '" + PRNumber + "'";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        string TransStatus = Cmd.ExecuteScalar().ToString();
                        #endregion

                        
                        Query = "Update Invent_Purchase_Qty Set PR_Issued_Amount = PR_Issued_Amount - " + QtyOld + " Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        
                    }
                }
                #endregion

                #region tambah qty baru ke table invent_purchase_qty
                //last edit by Hasim 19 okt 2018
                for (int i = 0; i < dgvPrDetails.RowCount ; i++)
                {
                    #region tambah qty bila memenuhi kriteria
                    step = dgvPrDetails.Rows[i].Cells["step"].Value == null ? 0 : decimal.Parse(dgvPrDetails.Rows[i].Cells["step"].Value.ToString());
                    //sm dan pm sama dengan comment di atas
                    sm = dgvPrDetails.Rows[i].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusSM"].Value.ToString();
                    pm = dgvPrDetails.Rows[i].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusPM"].Value.ToString();

                    if ((sm == "" || sm == "Revision" || pm == "" || pm == "Revision") && step < 2)
                    {
                        #region inisiasi data
                        Unit = "";
                        UoM = "";
                        QtyNew = 0;
                        ConvRatio = 0;

                        FullItemId = dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString();
                        QtyNew = decimal.Parse(dgvPrDetails.Rows[i].Cells["Amount"].Value.ToString());
                        #endregion

                        #region Check ItemID is already in Invent Purchase (if not create new)
                        QueryTemp = "Select PR_Issued_Amount From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "';";
                        Cmd = new SqlCommand(QueryTemp, Conn);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {

                        }
                        else
                        {
                            Cmd = new SqlCommand("INSERT INTO Invent_Purchase_Qty {[GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName]) VALUES ( '" + dgvPrDetails.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dgvPrDetails.Rows[i].Cells["ItemName"].Value.ToString() + "');", Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        Dr.Close();
                        #endregion

                        #region tambah qty
                        Query = "Update Invent_Purchase_Qty Set PR_Issued_Amount = PR_Issued_Amount + " + QtyNew + "  Where FullItemID = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        #endregion

                        #region Save to PurchRequisition_LogTable
                        Query = "Select PR_Issued_Amount From Invent_Purchase_Qty Where FullItemID = '" + dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString() + "' ";
                        Cmd = new SqlCommand(Query, Conn);
                        
                        Query += "Insert into [PurchRequisition_LogTable] ([PurchReqDate],[PurchReqID],[PurchReqType],[PurchReqSeqNo],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],Qty_Uom,Qty_Alt) ";
                        Query += "VALUES('" + dtPrDate.Value.ToString("yyyy-MM-dd") + "', '" + PRNumber + "', '" + cmbPrType.Text.Trim() + "', '" + dgvPrDetails.Rows[i].Cells["No"].Value.ToString() + "', '01' ,'Request – waiting for approval' ,'Request – waiting for approval By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(),'" + QtyUoMNew + "','" + QtyAltNew + "');";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        #endregion
                    }
                    #endregion

                }
                #endregion
            }
            #endregion

            #region Update PurchRequisitionH
            Query = "Update PurchRequisitionH set ";
            Query += "OrderDate='" + dtPrDate.Value.ToString("yyyy-MM-dd") + "',";
            Query += "TransType='" + cmbPrType.Text + "',";
            Query += "TransStatus='01',";
            Query += "ApprovedBy='" + txtPrApproved.Text + "',";
            Query += "UpdatedDate=getdate(),";
            Query += "UpdatedBy='" + ControlMgr.UserId + "' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy  where PurchReqID='" + txtPrNumber.Text.Trim() + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                CreatedBy = Dr["CreatedBy"].ToString();
            }
            Dr.Close();
            #endregion

            #region Delete Qty from PurchRequisition_Dtl
            //Delete Qty Lama
            QueryTemp = "Select FullItemID, Qty, Unit From PurchRequisition_Dtl Where PurchReqID = '" + txtPrNumber.Text + "'";
            Cmd = new SqlCommand(QueryTemp, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                FullItemId = Dr["FullItemID"].ToString();
                Unit = Dr["Unit"].ToString();
                QtyOld = decimal.Parse(Dr["Qty"].ToString());
                ConvRatio = 0;

                QueryTemp = "Select UoM From InventTable Where FullItemID = '" + FullItemId + "'";
                Cmd2 = new SqlCommand(QueryTemp, Conn);
                UoM = Cmd2.ExecuteScalar().ToString();

                QueryTemp = "Select Ratio From InventConversion Where FullItemID = '" + FullItemId + "'";
                Cmd2 = new SqlCommand(QueryTemp, Conn);
                ConvRatio = (decimal)Cmd2.ExecuteScalar();

                if (Unit == UoM)
                {
                    QtyUoMOld = QtyOld;
                    QtyAltOld = QtyOld * ConvRatio;
                }
                else
                {
                    QtyAltOld = QtyOld;
                    QtyUoMOld = QtyOld / ConvRatio;
                }

                QueryTemp = "Select PR_Issued_UoM, PR_Issued_Alt From Invent_Purchase_Qty Where FullItemID = '" + FullItemId + "'";
                Cmd2 = new SqlCommand(QueryTemp, Conn);
                Dr2 = Cmd2.ExecuteReader();
                while (Dr2.Read())
                {
                    QtyPRIssued_UoM = decimal.Parse(Dr2["PR_Issued_UoM"].ToString());
                    QtyPRIssued_Alt = decimal.Parse(Dr2["PR_Issued_Alt"].ToString());
                }
                Dr2.Close();

                QtyUoMOld = QtyPRIssued_UoM - QtyUoMOld;
                QtyAltOld = QtyPRIssued_Alt - QtyAltOld;
            }
            Dr.Close();
            #endregion

            #region Delete from PurchRequisition_Dtl
            Query = "Delete from PurchRequisition_Dtl where PurchReqID='" + txtPrNumber.Text.Trim() + "';";
            for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            {
                if (dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == null || dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == "")
                    dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value = "01-01-1900";
                if (dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == null || dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == "")
                    dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value = "01-01-1900";

                if (cmbPrType.Text != "AMOUNT")
                    Query += "Insert PurchRequisition_Dtl (PurchReqID,SeqNo,FullItemID,ItemName,DeliveryMethod,OrderDate,ReffTransID,ExpectedDateFrom,ExpectedDateTo,VendID,Deskripsi,Qty,Unit,Ratio, GroupId, SubGroup1Id, SubGroup2Id, ItemId, GelombangId, BracketId, Base, Price, SeqNoGroup,BracketDesc,TransStatus,TransStatusPurch,step,CreatedDate,CreatedBy) Values ";
                if (cmbPrType.Text == "AMOUNT")
                    Query += "Insert PurchRequisition_Dtl (PurchReqID,SeqNo,FullItemID,ItemName,DeliveryMethod,OrderDate,ReffTransID,ExpectedDateFrom,ExpectedDateTo,VendID,Deskripsi,Amount,Qty,Unit,Ratio, GroupId, SubGroup1Id, SubGroup2Id, ItemId, GelombangId, BracketId, Base, Price, SeqNoGroup,BracketDesc,TransStatus,TransStatusPurch,step,CreatedDate,CreatedBy) Values ";
                Query += "('" + PRNumber + "','";
                Query += (dgvPrDetails.Rows[i].Cells["No"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["No"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                Query += (dtPrDate.Value == null ? "" : dtPrDate.Value.ToString("yyyy-MM-dd")) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SalesSO"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SalesSO"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == null ? "" : FormateDateddmmyyyy(dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value.ToString())) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == null ? "" : FormateDateddmmyyyy(dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value.ToString())) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Vendor"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Vendor"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Deskripsi"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Deskripsi"].Value.ToString().Trim()) + "','";
                if (cmbPrType.Text != "AMOUNT")
                {
                    Query += ((dgvPrDetails.Rows[i].Cells["Qty"].Value == "" ? 0.00 : Double.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString()))) + "','";
                    Query += (dgvPrDetails.Rows[i].Cells["Unit"].Value == null ? "'," : dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString()) + "',";
                }
                else if (cmbPrType.Text == "AMOUNT")
                {
                    Query += ((dgvPrDetails.Rows[i].Cells["Amount"].Value == "" ? 0.0000 : Double.Parse(dgvPrDetails.Rows[i].Cells["Amount"].Value.ToString()))) + "','";
                    Query += "1','";//(dgvPrDetails.Rows[i].Cells["Qty"].Value == "" ? 0.00 : Double.Parse(dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString())) + "','";
                    Query += "KG',";//(dgvPrDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString()) + "',";
                }
                Query += "(select top 1 [Ratio] from [dbo].[InventConversion] where FullItemID='" + dgvPrDetails.Rows[i].Cells["FullItemID"].Value.ToString() + "' ),'";
                Query += (dgvPrDetails.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["GelombangId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["GelombangId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["BracketId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["BracketId"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Base"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Base"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["Price"].Value == null ? "0.00" : dgvPrDetails.Rows[i].Cells["Price"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["BracketDesc"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["BracketDesc"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusSM"].Value.ToString()) + "','";
                Query += (dgvPrDetails.Rows[i].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusPM"].Value.ToString()) + "',";
                Query += (dgvPrDetails.Rows[i].Cells["step"].Value == null ? "1" : dgvPrDetails.Rows[i].Cells["step"].Value.ToString()) + ",";
                Query += "'" + CreatedDate + "',";
                Query += "'" + ControlMgr.UserId + "');";

                if (i % 5 == 0 && i > 0)
                {
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    Query = "";
                }                
            }
            if (Query != "")
            {
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
            #endregion

            Notification = "Data PurchReqID : " + txtPrNumber.Text + " berhasil diupdate.";
        }

        private Boolean Validasi()
        {
            Boolean vBol = true;
            if (dgvPrDetails.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                vBol = false;
            }
            else
            {
                for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
                {
                    if (cmbPrType.Text != "AMOUNT" && (Convert.ToDecimal((dgvPrDetails.Rows[i].Cells["Qty"].Value == "" ? "0.000" : dgvPrDetails.Rows[i].Cells["Qty"].Value.ToString())) <= 0) || dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                        {
                            MessageBox.Show("Item No = " + dgvPrDetails.Rows[i].Cells["No"].Value + ", Qty tidak boleh lebih kecil atau sama dengan 0");
                            vBol = false;
                        }
                    }
                    if (cmbPrType.Text == "AMOUNT" && (Convert.ToDecimal((dgvPrDetails.Rows[i].Cells["Amount"].Value == "" ? "0.000" : dgvPrDetails.Rows[i].Cells["Amount"].Value.ToString())) <= 0) || dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                        {
                            MessageBox.Show("Item No = " + dgvPrDetails.Rows[i].Cells["No"].Value + ", Amount tidak boleh lebih kecil atau sama dengan 0");
                            vBol = false;
                        }
                    }
                    if (cmbPrType.Text != "AMOUNT" && ((dgvPrDetails.Rows[i].Cells["Unit"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString()) == "") || dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                        {
                            MessageBox.Show("Item No = " + dgvPrDetails.Rows[i].Cells["No"].Value + ", Unit tidak boleh kosong.");
                            vBol = false;
                        }
                    }
                    if (dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value != "")
                    {
                        if (DateTime.Parse(FormateDateddmmyyyy(dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value.ToString())) < DateTime.Parse(FormateDateddmmyyyy(dtPrDate.Value.ToString("dd-MM-yyyy"))))
                        {
                            MessageBox.Show("Item No = " + dgvPrDetails.Rows[i].Cells["No"].Value + ", ExpectedDateFrom tidak boleh lebih kecil dari PR Date.");
                            vBol = false;
                        }
                    }
                    if (DateTime.Parse(dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == null ? "01-01-1900" : FormateDateddmmyyyy(dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value.ToString())) > DateTime.Parse(dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == null ? "01-01-1900" : FormateDateddmmyyyy(dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value.ToString())))
                    {
                        MessageBox.Show("Item No = " + dgvPrDetails.Rows[i].Cells["No"].Value + ", ExpectedDateTo tidak boleh lebih kecil dari ExpectedDateFrom.");
                        vBol = false;
                    }
                    if ((dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value == null && dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value != null) || dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                        {
                            MessageBox.Show("Item No = " + dgvPrDetails.Rows[i].Cells["No"].Value + ", ExpectedDateFrom tidak boleh kosong.");
                            vBol = false;
                        }
                    }
                    if ((dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Value == null && dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Value != null) || dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
                    {
                        if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() != "N")
                        {
                            MessageBox.Show("Item No = " + dgvPrDetails.Rows[i].Cells["No"].Value + ", ExpectedDateTo tidak boleh kosong.");
                            vBol = false;
                        }
                    }
                }
            }
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Validasi())
                return;

            string PRType = cmbPrType.SelectedItem.ToString();
            
            //try
            //{
                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();           
                    if (Mode == "New" || txtPrNumber.Text == "")
                    {
                        saveNew();
                    }
                    
                    else
                    {
                        saveEdit();
                    }                    
                    scope.Complete();
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return;
            //}
            //finally
           // {
                
            //}
            

            MessageBox.Show(Notification);
            GetDataHeader();
            //Conn.Close();
            Parent.RefreshGrid();
            ModeBeforeEdit();
        }

        private string GetSeqDtlNo(string Gelombang, string Bracket)
        {
            SqlConnection Con1 = ConnectionString.GetConnection();
            for (int i = 0; i < dgvPrDetails.RowCount; i++)
            {
                string Query123 = "Select GelombangID, BracketID from [dbo].[InventGelombangD] where [ItemId]='" + dgvPrDetails.Rows[i].Cells[1].Value.ToString() + "';";
                Cmd = new SqlCommand(Query123, Con1);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (Gelombang == Dr[0].ToString() && Bracket == Dr[1].ToString())
                    {
                        Dr.Close();
                        Con1.Close();
                        return (i + 1).ToString();
                    }
                }
                Dr.Close();
            }
            return "";
        }

        private string FormateDateddmmyyyy(string tmpDate)
        {
            if (tmpDate == "")
            {
                tmpDate = "01-01-1900";
            }
            //string reformat="";
            string [] data = tmpDate.Split('-');
            return data[2] + "-" + data[1] + "-" + data[0];
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dtPrDate.Enabled = false;
            cmbPrType.Enabled = false;
            txtPrStatus.Enabled = false;

            ModeBeforeEdit();
            GetDataHeader();
        }

        private void dgvPrDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (Mode != "BeforeEdit" && dgvPrDetails.Rows[e.RowIndex].Cells["Base"].Value.ToString() != "N")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "OrderDate" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
                    {
                        dtp.Location = dgvPrDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        dtp.Visible = true;

                        if (dgvPrDetails.CurrentCell.Value != "" && dgvPrDetails.CurrentCell.Value != null)
                        {
                            DateTime dDate;
                            if (!DateTime.TryParse(dgvPrDetails.CurrentCell.Value.ToString(), out dDate))
                            {
                                //dtp.Value = Convert.ToDateTime(FormateDateddmmyyyy(dgvPrDetails.CurrentCell.Value.ToString()));
                            }
                            else
                            {
                                dtp.Value = Convert.ToDateTime(dgvPrDetails.CurrentCell.Value);
                            }
                        }
                        else
                        {
                            //dtp.Value = Convert.ToDateTime(DateTime.Now.ToString("dd-MM-yyyy"));
                        }
                    }
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                     {
                        DeliveryMethod.Location = dgvPrDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        DeliveryMethod.Visible = true;
                        string tmpFullItemId = dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                        string tmpDeliveryMethod = "";
                        Conn = ConnectionString.GetConnection();
                        for (int i = 0; i < dgvPrDetails.RowCount; i++)
                        {
                            if (dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                            {
                                if (dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value != null)
                                {
                                    if (tmpDeliveryMethod == "")
                                    {
                                        tmpDeliveryMethod = "'" + dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";                                       
                                    }
                                    else
                                    {
                                        tmpDeliveryMethod += ",'" + dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                                    }
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
                        DeliveryMethod.SelectedIndex=0;
                        DrCmb.Close();

                        Conn.Close();
                    }
                }
                if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "OrderDate" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
                {
                    if (dgvPrDetails.CurrentCell.Value != "" && dgvPrDetails.CurrentCell.Value != null)
                    {
                        //dgvPrDetails.CurrentCell.Value = dtp.Value.Date.ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        dtp.Value = DateTime.Now;
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
            if (Mode != "BeforeEdit" && dgvPrDetails.Rows[e.RowIndex].Cells["Base"].Value.ToString() != "N")
            {
                if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "OrderDate" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateFrom" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ExpectedDateTo" && dgvPrDetails.Rows[e.RowIndex].Cells["SalesSO"].Value != null)
                    {
                        DateTime date;
                        if (dgvPrDetails.Rows[e.RowIndex].Cells["SalesSO"].Value.ToString() != "")
                        {
                            date = Convert.ToDateTime("01/01/1900");
                        
                            Conn = ConnectionString.GetConnection();
                            Query = "Select [ExpectedDateTo] From dbo.[SalesOrderD] where [SalesOrderNo] = '" + dgvPrDetails.Rows[e.RowIndex].Cells["SalesSO"].Value + "' and FullItemId='" + dgvPrDetails.Rows[e.RowIndex].Cells["FullItemId"].Value + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            date = Convert.ToDateTime(Cmd.ExecuteScalar());

                            if (dtp.Value.Date > date)
                            {
                                dtp.Value = date;
                                MessageBox.Show("ExpectedDate tidak boleh lebih besar dari DeliveryDate SO (" + date.ToString("dd-MMM-yyyy") + ")");
                                return;
                            }

                        }
                        
                       
                    }

                    if (dgvPrDetails.CurrentCell.Value != "" && dgvPrDetails.CurrentCell.Value != null)
                    {
                        dgvPrDetails.CurrentCell.Value = dtp.Value.Date.ToString("dd-MM-yyyy");
                    }
                    else
                    {
                        dtp.Value = DateTime.Now;
                    }
                }

                if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "BracketDesc")
                {

                    for (int i = 0; i < dgvPrDetails.RowCount; i++)
                    {
                        if (dgvPrDetails.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString() == dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                        {
                            dgvPrDetails.Rows[i].Cells["BracketDesc"].Value = dgvPrDetails.Rows[e.RowIndex].Cells["BracketDesc"].Value;
                        }
                    }
                    //if (dgvPrDetails.CurrentCell.Value != "" && dgvPrDetails.CurrentCell.Value != null)
                    //{
                    //    dgvPrDetails.CurrentCell.Value = dtp.Value.Date.ToString("dd-MM-yyyy");
                    //}
                    //else
                    //{
                    //    dtp.Value = DateTime.Now;
                    //}
                }

                if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Visible = false;
                    string TmpNoGroup = dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString();
                    string TmpDeliveryMethod = dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["DeliveryMethod"].Value.ToString();

                    for (int i = 0; i < dgvPrDetails.RowCount; i++)
                    {
                        if (TmpNoGroup == dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                        {
                            dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Value = TmpDeliveryMethod;
                        }
                    }
                }

                if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Unit")
                {
                    string TmpNoGroup = dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["SeqNoGroup"].Value.ToString();
                    //string TmpUnit = dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["Unit"].Value.ToString();
                    string TmpUnit = dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["Unit"].Value == null ? "" : dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["Unit"].Value.ToString();
                    //hendry is null

                    for (int i = 0; i < dgvPrDetails.RowCount; i++)
                    {
                        if (TmpNoGroup == dgvPrDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString())
                        {
                            dgvPrDetails.Rows[i].Cells["Unit"].Value = TmpUnit;
                        }
                    }
                }

                //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                //{
                //    dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value = Convert.ToDecimal(dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value).ToString("N2");
                //}

                dtp.Visible = false;
                dgvPrDetails.AutoResizeColumns();

            }
            
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dgvPrDetails.CurrentCell.Value = dtp.Text;
        }

        private void cmbPrType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetGelombang();
            tmpPrType = cmbPrType.Text;
        }

        private void GetGelombang()
        {
            if (dgvPrDetails.RowCount > 0 && tmpPrType != cmbPrType.Text)
            {
                //if ((tmpPrType == "FIX" && cmbPrType.Text != "FIX") || (tmpPrType != "FIX" && cmbPrType.Text == "FIX"))//&& dgvPrDetails.RowCount > 1
                //{
                DialogResult dr = MessageBox.Show("Details item akan dikosongkan. " + Environment.NewLine + "Apakah anda setuju ? ", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    ChangeGelombang();
                    dgvPrDetails.Rows.Clear();
                    dgvPrDetails.Columns.Clear();
                }
                else
                {
                    cmbPrType.Text = tmpPrType;
                }
                //}
                //else
                //{
                //    ChangeGelombang();
                //}
            }
        }

        private void ChangeGelombang()
        {
            //Conn = ConnectionString.GetConnection();
            //string InvStockId = "";

            //for (int i = 0; i <= dgvPrDetails.RowCount - 1; i++)
            //{

            //    if (i == 0)
            //    {
            //        InvStockId += " ItemIdInduk in ('";
            //        InvStockId += dgvPrDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString();
            //        InvStockId += "'";
            //    }
            //    else
            //    {
            //        InvStockId += ",'";
            //        InvStockId += dgvPrDetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["FullItemId"].Value.ToString();
            //        InvStockId += "'";
            //    }
            //}
            //InvStockId += ")";

            ////Conn = ConnectionString.GetConnection();

            //Query = "Select ROW_NUMBER() OVER (ORDER BY ItemIdAnak) No, ItemIdAnak InvStockId, NamaItemAnak ItemDesc, Base, Price From Gelombang a ";
            //Query += "Where " + InvStockId + ";";

            //if (dgvGelombang.RowCount - 1 <= 0)
            //{
            //    dgvGelombang.ColumnCount = 5;
            //    dgvGelombang.Columns[0].Name = "No";
            //    dgvGelombang.Columns[1].Name = "FullItemId";
            //    dgvGelombang.Columns[2].Name = "ItemDesc";
            //    dgvGelombang.Columns[3].Name = "Base";
            //    dgvGelombang.Columns[4].Name = "Price";
            //}

            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();
            //dgvGelombang.Rows.Clear();

            //while (Dr.Read())
            //{
            //    this.dgvGelombang.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4].ToString().Replace(',', '.'));
            //}

            //dgvGelombang.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvGelombang.Columns["FullItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvGelombang.Columns["ItemDesc"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvGelombang.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            //dgvGelombang.Columns["Price"].SortMode = DataGridViewColumnSortMode.NotSortable;

            //dgvGelombang.AutoResizeColumns();
            //dgvGelombang.DefaultCellStyle.BackColor = Color.LightGray;
            //Conn.Close();
        }

        private void dgvPrDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                statussm = dgvPrDetails.Rows[e.RowIndex].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[e.RowIndex].Cells["StatusSM"].Value.ToString();
                statuspm = dgvPrDetails.Rows[e.RowIndex].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[e.RowIndex].Cells["StatusPM"].Value.ToString();
                
                if (sm == "" || sm == "Revision" || pm == "" || pm == "Revision")
                {
                    if (dgvPrDetails.Rows[e.RowIndex].Cells["Base"].Value.ToString() != "N")
                    {
                        if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "SalesSO" && Mode != "BeforeEdit")
                        {
                            SearchQueryV1 tmpSearch = new SearchQueryV1();
                            tmpSearch.Text = "Search Sales Order No";
                            tmpSearch.PrimaryKey = "SalesOrderNo";
                            tmpSearch.Order = "SalesOrderNo Desc";
                            tmpSearch.QuerySearch = "SELECT Distinct a.[SalesOrderNo],[OrderDate],[SalesQuotationNo],[CustName] FROM [SalesOrderH] a Left JOIN [SalesOrderD] b ON a.SalesOrderNo = b.SalesOrderNo Where b.FullItemID = '" + dgvPrDetails.CurrentRow.Cells["FullItemID"].Value.ToString() + "' And a.TransStatus IN ('01', '03', '05', '06', '08', '09') And b.RemainingQty > 0";
                            tmpSearch.FilterText = new string[] { "SalesOrderNo" };
                            tmpSearch.Select = new string[] { "SalesOrderNo" };
                            tmpSearch.ShowDialog();

                            if (ConnectionString.Kodes != null)
                            {
                                dgvPrDetails.CurrentCell.Value = ConnectionString.Kodes[0];

                                ConnectionString.Kodes = null;
                                dgvPrDetails.AutoResizeColumns();
                            }
                        }


                        if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Vendor" && Mode != "BeforeEdit")
                        {
                            //hasim 19 Okt 2018

                            #region search vendor
                            SearchQueryV2 tmpSearch = new SearchQueryV2();

                            tmpSearch.Text = "Search Vendor";
                            tmpSearch.PrimaryKey = "VendId";
                            tmpSearch.QuerySearch = "Select VendId,VendName,TaxName,NPWP,PKP,SIUP,TermOfPayment,PaymentModeId,CurrencyId,Sisa_Limit_Total,Deposito From VendTable";
                            tmpSearch.FilterText = new string[] { "VendId", "VendName" };
                            tmpSearch.Select = new string[] { "VendId" };
                            //tmpSearch.HideField = new string[] { "From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId" };
                            tmpSearch.Parent = this;
                            tmpSearch.Notes = "";

                            tmpSearch.ShowDialog();

                            string TmpVendor = "";
                            if (Variable.Kode2 != null)
                            {
                                for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
                                {
                                    TmpVendor += Variable.Kode2[i, 0].ToString();

                                    if (i < Variable.Kode2.GetLength(0) - 1)
                                    {
                                        TmpVendor += ";";
                                    }
                                }
                            }

                            dgvPrDetails.CurrentCell.Value = TmpVendor;
                            Variable.Kode2 = null;
                            #endregion

                        }

                        //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Vendor" && Mode != "BeforeEdit")
                        //{
                        //    PopUpSelect.Vendor tmpSearch = new PopUpSelect.Vendor();
                        //    tmpSearch.VendId = dgvPrDetails.CurrentCell.Value.ToString();
                        //    //tmpSearch.RefreshGrid(dgvPrDetails.CurrentCell.Value.ToString());
                        //    tmpSearch.ShowDialog();
                        //    dgvPrDetails.CurrentCell.Value = tmpSearch.VendId;
                        //    tmpSearch.VendId = "";
                        //    //dgvPrDetails.CurrentCell.Value = ConnectionString.Kode;
                        //    //dgvPrDetails.Columns[e.ColumnIndex].Name = ConnectionString.Kode2;
                        //}

                    }


                    //if (Mode != "BeforeEdit")
                    //{
                    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D1")
                    //{
                    //    Info tmpInfo = new Info();

                    //    ListInfo.Add(tmpInfo);
                    //    //tmpInfo.SetParent(this);
                    //    tmpInfo.Show();
                    //}
                    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D2")
                    //{
                    //    Info tmpInfo = new Info();

                    //    ListInfo.Add(tmpInfo);
                    //    //tmpInfo.SetParent(this);
                    //    tmpInfo.Show();
                    //}
                    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D3")
                    //{
                    //    Info tmpInfo = new Info();

                    //    ListInfo.Add(tmpInfo);
                    //    //tmpInfo.SetParent(this);
                    //    tmpInfo.Show();
                    //}
                    //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "D4")
                    //{
                    //    Info tmpInfo = new Info();

                    //    ListInfo.Add(tmpInfo);
                    //    //tmpInfo.SetParent(this);
                    //    tmpInfo.Show();
                    //}
                    //}
                }
                //dgvPrDetails.CurrentRow.Cells["ItemFullID"].Value.ToString();
            }
        }

        private void dgvPrDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);
            }
            //hendry 
            //if (e.Control.AccessibilityObject.Role.ToString() == "Qty")
            //{
            //    int count = 0;
            //    foreach (char c in dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value.ToString())
            //    {
            //        if (c == '.') count++;
            //    }
            //}
            //hendry end
        }

        public void SetParent(Purchase.PurchaseRequisition.InquiryPR F)
        {
            Parent = F;
        }

        private void HeaderPR2_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < ListDetailPR.Count(); i++)
            {
                ListDetailPR[i].Close();
            }
            for (int i = 0; i < ListGelombang.Count(); i++)
            {
                ListGelombang[i].Close();
            }
            for (int i = 0; i < ListInfo.Count(); i++)
            {
                ListInfo[i].Close();
            }
            for (int i = 0; i < ListVendor.Count(); i++)
            {
                ListVendor[i].Close();
            }
            Purchase.PurchaseQuotation.FormPQ.reffID = null;
           // Purchase.PurchaseRequisition.InquiryPR f = new Purchase.PurchaseRequisition.InquiryPR();
           // f.RefreshGrid();
            //if (Mode != "ModeView")
            //{
            //    Parent.RefreshGrid();
            //}
        }

        private void HeaderPR2_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void MethodReadOnlyRowBaseN()
        {
            for (int i = 0; i < dgvPrDetails.RowCount; i++)
            {
                sm = dgvPrDetails.Rows[i].Cells["StatusSM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusSM"].Value.ToString();
                pm = dgvPrDetails.Rows[i].Cells["StatusPM"].Value == null ? "" : dgvPrDetails.Rows[i].Cells["StatusPM"].Value.ToString();
                
                if (dgvPrDetails.Rows[i].Cells["Base"].Value.ToString() == "N" && cmbPrType.Text.Trim() != "FIX")
                {
                    dgvPrDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvPrDetails.Rows[i].ReadOnly = true;
                }
                else if ((sm == "Yes" || sm == "No" || sm == "Pending" || pm == "Yes" || pm == "No" || pm == "Pending") && (sm == "Yes" && pm != "Revision"))
                {
                    dgvPrDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dgvPrDetails.Rows[i].ReadOnly = true;
                }
                else
                {
                    dgvPrDetails.Rows[i].ReadOnly = false;
                }

                
            }
        }


        private void DeliveryMethod_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvPrDetails.CurrentCell.Value = DeliveryMethod.Text.ToString();
            for (int j = 0; j < dgvPrDetails.RowCount; j++)
            {
                if (dgvPrDetails.Rows[j].Cells["SeqNoGroup"].Value == dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["No"].Value.ToString())
                {
                    dgvPrDetails.Rows[j].Cells["DeliveryMethod"].Value = dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["DeliveryMethod"].Value.ToString();
                }
            }
        }

        private void DeliveryMethod_DropDownClosed(object sender, EventArgs e)
        {
            DeliveryMethod.Visible = false;
        }
        //tia edit
        //klik kanan
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.FullItemId.FullItemId FID = null;
        Sales.SalesOrder.SOHeader PUSO = null;

        Purchase.PurchaseQuotation.FormPQ ParentToPQ;
        Purchase.CanvasSheet.FormCanvasSheet2 ParentToCS;
        Purchase.RFQ.RFQForm ParentToRFQ;

        public void ParentRefreshGrid(Purchase.PurchaseQuotation.FormPQ pq)
        {
            ParentToPQ = pq;
        }
        public void ParentRefreshGrid2(Purchase.CanvasSheet.FormCanvasSheet2 cs)
        {
            ParentToCS = cs;
        }
        public void ParentRefreshGrid3(Purchase.RFQ.RFQForm rfq)
        {
            ParentToRFQ = rfq;
        }


        private void dgvPrDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex>-1 && e.ColumnIndex>-1)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Vendor")
                    {
                        string TmpListVendor = dgvPrDetails.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                        string [] SplitVendor = TmpListVendor.Split(';');

                        for (int i = 0; i < ListVendor.Count(); i++)
                        {
                            ListVendor[i].Close();
                        }

                        for (int i = 0; i < SplitVendor.Count(); i++)
                        {
                            PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();
                            ListVendor.Add(PopUpVendor);
                            PopUpVendor.GetData(SplitVendor[i].ToString());
                            PopUpVendor.Y += 100 * i;
                            PopUpVendor.Show();
                        }
                    }
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
                if (FID==null || FID.Text=="")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        //PopUpItemName.Close();
                        // PopUpItemName = new PopUp.Stock.Stock();
                         //PopUpItemName = new PopUp.FullItemId.FullItemId();
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
                if (PUSO == null || PUSO.Text == "")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "SalesSO")
                    {
                        PUSO = new Sales.SalesOrder.SOHeader();
                        PUSO.SetMode("PopUp", dgvPrDetails.Rows[e.RowIndex].Cells["SalesSO"].Value.ToString());
                        PUSO.ParentRefreshGrid(this);
                        PUSO.Show();
                    }
                }
                else if (CheckOpened(PUSO.Name))
                {
                    PUSO.WindowState = FormWindowState.Normal;
                    PUSO.SetMode("PopUp", dgvPrDetails.Rows[e.RowIndex].Cells["SalesSO"].Value.ToString());
                    PUSO.ParentRefreshGrid(this);
                    PUSO.Show();
                    PUSO.Focus();
                }
            }
        }
        private bool CheckOpened(string name)
        {
            // FormCollection FC = Application.OpenForms;
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
        //end

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

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
                    e.Value = d.ToString("N2");
                }
            }
        }

        private void cmbPrType_Click(object sender, EventArgs e)
        {

        }

    }
}
            #endregion