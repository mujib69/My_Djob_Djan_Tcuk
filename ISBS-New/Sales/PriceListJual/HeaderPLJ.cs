using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ISBS_New.Sales.PriceListJual
{
    public partial class HeaderPLJ : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, crit = null;

        int Index;

        ComboBox DeliveryMethod;

        public string Gelombang;
        public string Bracket;

        public string PLJNumber = "", tmpPrType = "";

        Sales.PriceListJual.InqueryPLJ Parent;

        DateTimePicker dtp;

       // List<Gelombang> ListGelombang = new List<Gelombang>();

        List<DetailPLJ> ListDetailPLJ = new List<DetailPLJ>();
        List<PopUp.PriceListJual.PriceListJual> PriceListJual = new List<PopUp.PriceListJual.PriceListJual>();
        List<PopUp.Customer.Customer> ListCustomer = new List<PopUp.Customer.Customer>();

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public HeaderPLJ()
        {
            InitializeComponent();
           
        }
        
        private void HeaderPLJ2_Load(object sender, EventArgs e)
        {
            lblForm.Location = new Point(16, 11);
            SetCmbCustomer();
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


            //dtp = new DateTimePicker();
            //dtp.Format = DateTimePickerFormat.Custom;
            //dtp.CustomFormat = "yyyy-MM-dd";
            //dtp.Visible = false;
            //dtp.Width = 100;

            dgvPLJDetails.Controls.Add(dtp);
           // dtp.ValueChanged += this.dtp_ValueChanged;
            dgvPLJDetails.CellBeginEdit += this.dgvPLJDetails_CellBeginEdit;
            dgvPLJDetails.CellEndEdit += this.dgvPLJDetails_CellEndEdit;

            DeliveryMethod = new ComboBox();
            DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            DeliveryMethod.Visible = false;
            dgvPLJDetails.Controls.Add(DeliveryMethod);
            DeliveryMethod.DropDownClosed += this.DeliveryMethod_DropDownClosed;
            DeliveryMethod.SelectionChangeCommitted += this.DeliveryMethod_SelectionChangeCommitted;
            Sales.PriceListJual.InqueryPLJ f = new Sales.PriceListJual.InqueryPLJ();
           // f.RefreshGrid();
        }

        private void SetCmbCustomer()
        {
            cmbCustomer.DataSource = null; 
            cmbCustomer.DisplayMember = "Text";
            cmbCustomer.ValueMember = "Value";

            var items = new[] { 
                new { Text = "-select-", Value = "" }, 
                new { Text = "All Customer", Value = "1" }, 
                new { Text = "All Customer Except", Value = "2" },
                new { Text = "Specific Customer", Value = "3" }
            };

            cmbCustomer.DataSource = items;
            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
        }

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
            dtPljDate.Enabled = false;

            dgvPLJDetails.ReadOnly = true;
            BeforeEditColor();
            dgvPLJDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }


        private void dgvPrDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (Mode != "BeforeEdit")
                {

                    if (dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                    {
                        DeliveryMethod.Location = dgvPLJDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        DeliveryMethod.Visible = true;
                        string tmpFullItemId = dgvPLJDetails.Rows[dgvPLJDetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                        string tmpDeliveryMethod = "";
                        Conn = ConnectionString.GetConnection();
                        for (int i = 0; i < dgvPLJDetails.RowCount; i++)
                        {
                            if (dgvPLJDetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                            {
                                if (dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value != null)
                                {
                                    if (tmpDeliveryMethod == "")
                                    {
                                        tmpDeliveryMethod = "'" + dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                                    }
                                    else
                                    {
                                        tmpDeliveryMethod += ",'" + dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
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
                        DeliveryMethod.SelectedIndex = 0;
                        DrCmb.Close();

                        Conn.Close();
                    }

                    if (dgvPLJDetails.RowCount > 0)
                    {
                        if (dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Tolerance")
                        {
                            if (Convert.ToDecimal(dgvPLJDetails.Rows[e.RowIndex].Cells["Tolerance"].Value.ToString() != "" ? dgvPLJDetails.Rows[e.RowIndex].Cells["Tolerance"].Value.ToString() : "0") > 100)
                            {
                                dgvPLJDetails.CurrentCell.Value = "100";
                                MessageBox.Show("Tolerance tidak boleh lebih dari 100");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }

        private void dgvPrDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Mode != "BeforeEdit")
            {


                if (dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Visible = false;
                }

                //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                //{
                //    dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value = Convert.ToDecimal(dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value).ToString("N2");
                //}

                

                dtp.Visible = false;
                dgvPLJDetails.AutoResizeColumns();

            }

        }

        private void DeliveryMethod_DropDownClosed(object sender, EventArgs e)
        {
            DeliveryMethod.Visible = false;
        }

        private void DeliveryMethod_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvPLJDetails.CurrentCell.Value = DeliveryMethod.Text.ToString();
            for (int j = 0; j < dgvPLJDetails.RowCount; j++)
            {
                if (dgvPLJDetails.Rows[j].Cells["SeqNoGroup"].Value == dgvPLJDetails.Rows[dgvPLJDetails.CurrentCell.RowIndex].Cells["No"].Value.ToString())
                {
                    dgvPLJDetails.Rows[j].Cells["DeliveryMethod"].Value = dgvPLJDetails.Rows[dgvPLJDetails.CurrentCell.RowIndex].Cells["DeliveryMethod"].Value.ToString();
                }
            }
        }

        public void SetMode(string tmpMode, string tmpPLJNumber)
        {
            Mode = tmpMode;
            PLJNumber = tmpPLJNumber;
            txtPLJNumber.Text = tmpPLJNumber;
        }

        private void grpDetail_Enter(object sender, EventArgs e)
        {

        }

        public void SetParent(Sales.PriceListJual.InqueryPLJ F)
        {
            Parent = F;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
           // Gelombang = "";
           // Bracket = "";
            DetailPLJ DetailPLJ = new DetailPLJ();

            List<DetailPLJ> ListDetailPLJ = new List<DetailPLJ>();
            DetailPLJ.ParentRefreshGrid(this);
            DetailPLJ.ParamHeader(dgvPLJDetails);
            DetailPLJ.ShowDialog();
            //CallFormGelombang(Gelombang, Bracket);
            //MethodReadOnlyRowBaseN();
            
            EditColor();
        }

        private void EditColor()
        {
            for (int i = 0; i < dgvPLJDetails.RowCount; i++)
            {

                dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price0D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price2D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price3D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price7D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price14D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price21D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price30D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price40D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price45D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price60D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price75D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price90D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price120D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price150D"].Style.BackColor = Color.White;
                dgvPLJDetails.Rows[i].Cells["Price180D"].Style.BackColor = Color.White;

                
            }
        }

        //private void MethodReadOnlyRowBaseN()
        //{
        //    for (int i = 0; i < dgvPLJDetails.RowCount; i++)
        //    {
        //        if (dgvPLJDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
        //        {
        //            dgvPLJDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
        //            dgvPLJDetails.Rows[i].ReadOnly = true;
        //        }
        //        else
        //        {
        //            dgvPLJDetails.Rows[i].ReadOnly = false;
        //        }
        //    }
        //}

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

        //private void CallFormGelombang(string GelombangId, string BracketId)
        //{
        //    if (GelombangId != "")
        //    {
                
        //        Conn = ConnectionString.GetConnection();
        //        Query = "Select count([GelombangId]) From [InventGelombangD] Where GelombangId in (Select GelombangId from InventGelombangD where GelombangId = '" + GelombangId + "' and BracketId='" + BracketId + "')";

        //        Cmd = new SqlCommand(Query, Conn);
        //        int CountChk = Convert.ToInt32(Cmd.ExecuteScalar());

        //        if (CountChk > 0)
        //        {
        //            Gelombang Gelombang = new Gelombang();

        //            ListGelombang.Add(Gelombang);
        //            Gelombang.SetParentForm(this);
        //            Gelombang.GetInventStockId(GelombangId, BracketId);
        //            Gelombang.ShowDialog();
        //        }
                
        //    }
        //}

        //public void AddDataGridGelombang(List<string> GroupId, List<string> SubGroup1Id, List<string> SubGroup2Id, List<string> ItemId, List<string> FullItemId, List<string> ItemName, List<string> GelombangId, List<string> BracketId, List<string> Base, List<string> Price, List<string> VendId, List<string> BracketDesc)
        //{
        //    int SeqNoGroup = CheckSeqNoGroup();

        //    if (dgvPLJDetails.RowCount - 1 <= 0)
        //    {
        //        dgvPLJDetails.ColumnCount = 21;
        //        dgvPLJDetails.Columns[0].Name = "No";
        //        dgvPLJDetails.Columns[1].Name = "FullItemID";
        //        dgvPLJDetails.Columns[2].Name = "ItemName";
        //        dgvPLJDetails.Columns[3].Name = "Qty";
        //        dgvPLJDetails.Columns[4].Name = "Unit";
        //        dgvPLJDetails.Columns[5].Name = "Base";
        //        dgvPLJDetails.Columns[6].Name = "Vendor";
        //        dgvPLJDetails.Columns[7].Name = "DeliveryMethod";
        //        dgvPLJDetails.Columns[8].Name = "SalesSO";
        //        dgvPLJDetails.Columns[9].Name = "ExpectedDateFrom";
        //        dgvPLJDetails.Columns[10].Name = "ExpectedDateTo"; ;
        //        dgvPLJDetails.Columns[11].Name = "Deskripsi";
        //        dgvPLJDetails.Columns[12].Name = "GroupId";
        //        dgvPLJDetails.Columns[13].Name = "SubGroup1Id";
        //        dgvPLJDetails.Columns[14].Name = "SubGroup2Id";
        //        dgvPLJDetails.Columns[15].Name = "ItemId";
        //        dgvPLJDetails.Columns[16].Name = "GelombangId";
        //        dgvPLJDetails.Columns[17].Name = "BracketId";
        //        dgvPLJDetails.Columns[18].Name = "Price";
        //        dgvPLJDetails.Columns[19].Name = "SeqNoGroup";
        //        dgvPLJDetails.Columns[20].Name = "BracketDesc";
        //    }

        //    for (int i = 0; i < FullItemId.Count; i++)
        //    {

        //        this.dgvPLJDetails.Rows.Add((dgvPLJDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], "0", "", Base[i], VendId[i], "", "", "", "", "", GroupId[i], SubGroup1Id[i], SubGroup2Id[i], ItemId[i], GelombangId[i], BracketId[i], Price[i], SeqNoGroup, BracketDesc[i]);

        //        Conn = ConnectionString.GetConnection();
        //        Query = "Select [Uom], [UomAlt] From dbo.[InventTable] where FullItemId = '" + FullItemId[i] + "' ";
        //        Cmd = new SqlCommand(Query, Conn);
        //        Dr = Cmd.ExecuteReader();
        //        DataGridViewComboBoxCell combo = new DataGridViewComboBoxCell();
        //        if (Base[i].ToString() != "N")
        //        {
        //            while (Dr.Read())
        //            {
        //                combo.Items.Add(Dr[0].ToString());
        //                combo.Items.Add(Dr[1].ToString());
        //            }
        //            dgvPLJDetails.Rows[(dgvPLJDetails.Rows.Count - 1)].Cells[4] = combo;
        //            //dgvPrDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.LightPink;
        //            //dgvPrDetails.Rows[i].Cells["Vendor"].Style.BackColor = Color.LightYellow;
        //            //dgvPrDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.LightYellow;
        //            //dgvPrDetails.Rows[i].Cells["SalesSO"].Style.BackColor = Color.LightYellow;
        //            //dgvPrDetails.Rows[i].Cells["ExpectedDateFrom"].Style.BackColor = Color.LightYellow;
        //            //dgvPrDetails.Rows[i].Cells["ExpectedDateTo"].Style.BackColor = Color.LightYellow;
        //            //dgvPrDetails.Rows[i].Cells["Deskripsi"].Style.BackColor = Color.LightYellow;
        //        }
        //        else
        //        {
        //            dgvPLJDetails.Rows[(dgvPLJDetails.Rows.Count - 1)].DefaultCellStyle.BackColor = Color.LightGray;
        //            dgvPLJDetails.Rows[(dgvPLJDetails.Rows.Count - 1)].ReadOnly = true;
        //        }
        //        //Query = "Select DeliveryMethod From dbo.[DeliveryMethod]";
        //        //Cmd = new SqlCommand(Query, Conn);
        //        //SqlDataReader DrDeliveryMethod;
        //        //DrDeliveryMethod = Cmd.ExecuteReader();
        //        ////DataGridViewComboBoxCell DeliveryMethod = new DataGridViewComboBoxCell();
        //        //DeliveryMethod.Items.Clear();

        //        //while (DrDeliveryMethod.Read())
        //        //{
        //        //    DeliveryMethod.Items.Add(DrDeliveryMethod[0].ToString());
        //        //}
        //        //DeliveryMethod.ValueChanged += this.dtp_ValueChanged;
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[3] = DeliveryMethod;


        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[10].Value = "...";
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[11].Value = "...";
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[12].Value = "...";
        //        //dgvPrDetails.Rows[(dgvPrDetails.Rows.Count - 1)].Cells[13].Value = "...";
        //    }


        //    dgvPLJDetails.Columns["GroupId"].Visible = false;
        //    dgvPLJDetails.Columns["SubGroup1Id"].Visible = false;
        //    dgvPLJDetails.Columns["SubGroup2Id"].Visible = false;
        //    dgvPLJDetails.Columns["ItemId"].Visible = false;
        //    dgvPLJDetails.Columns["GelombangId"].Visible = false;
        //    dgvPLJDetails.Columns["BracketId"].Visible = false;
        //    dgvPLJDetails.Columns["Price"].Visible = false;
        //    dgvPLJDetails.Columns["SeqNoGroup"].Visible = false;


        //    dgvPLJDetails.ReadOnly = false;
        //    dgvPLJDetails.Columns["No"].ReadOnly = true;
        //    dgvPLJDetails.Columns["FullItemID"].ReadOnly = true;
        //    dgvPLJDetails.Columns["ItemName"].ReadOnly = true;
        //    dgvPLJDetails.Columns["SalesSO"].ReadOnly = true;
        //    dgvPLJDetails.Columns["Base"].ReadOnly = true;
        //    dgvPLJDetails.Columns["BracketDesc"].Visible = true;
        //    dgvPLJDetails.Columns["BracketDesc"].ReadOnly = true;

        //    //dgvPrDetails.Columns["D1"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D2"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D3"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D4"].ReadOnly = true;
        //    dgvPLJDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLJDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    //dgvPrDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    dgvPLJDetails.AutoResizeColumns();

        //    //InvStockDetail = InvStockId;
        //}

        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvPLJDetails.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvPLJDetails.Rows[i].Cells["SeqNoGroup"].Value) == j)
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

        public void AddDataGridDetail(List<string> GroupId, List<string> SubGroup1Id, List<string> SubGroup2Id, List<string> ItemId, List<string> FullItemId, List<string> ItemName)
        {
            string Price0D = "0.0000";
            string Price2D = "0.0000";
            string Price3D = "0.0000";
            string Price7D = "0.0000";
            string Price14D = "0.0000";
            string Price21D = "0.0000";
            string Price30D = "0.0000";
            string Price40D = "0.0000";
            string Price45D = "0.0000";
            string Price60D = "0.0000";
            string Price75D = "0.0000";
            string Price90D = "0.0000";
            string Price120D = "0.0000";
            string Price150D = "0.0000";
            string Price180D = "0.0000";
            string Tolerance = "0.0000";

            for (int i = 0; i < FullItemId.Count; i++)
            {
                PriceListJualAktif PLJA = new PriceListJualAktif();
                PLJA.SetParent(this);
                PLJA.SetHeader(FullItemId[i]);

                Conn = ConnectionString.GetConnection();
                Query = "SELECT ROW_NUMBER() OVER (ORDER BY FullItemID asc) No,* FROM (SELECT d.PriceListNo, d.DeliveryMethod, d.GroupID AS GroupId, d.SubGroupID AS SubGroup1Id, d.SubGroup2ID AS SubGroup2Id, d.ItemID AS ItemId, d.FullItemID, d.ItemName, ";
                Query += "d.Price0D, d.Price2D, d.Price3D, d.Price7D, d.Price14D, d.Price21 AS Price21D, d.Price30D, d.Price40D, ";
                Query += "d.Price45D, d.Price60D, d.Price75D, d.Price90D, d.Price120D, d.Price150D, d.Price180D, d.Tolerance FROM SalesPriceListDtl d ";
                Query += "INNER JOIN SalesPriceListH h ON d.PriceListNo = h.PriceListNo ";
                Query += "WHERE h.ValidTo >= GETDATE() AND d.FullItemID = '" + FullItemId[i] + "') a ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                PLJA.dgvPriceListJualAktif.Rows.Clear();

                while (Dr.Read())
                {
                    PLJA.dgvPriceListJualAktif.Rows.Add(Convert.ToString(Dr["No"]), Convert.ToString(Dr["FullItemID"]),
                    Convert.ToString(Dr["ItemName"]), Convert.ToString(Dr["DeliveryMethod"]), Convert.ToString(Dr["Price0D"]),
                    Convert.ToString(Dr["Price2D"]), Convert.ToString(Dr["Price3D"]), Convert.ToString(Dr["Price7D"]),
                    Convert.ToString(Dr["Price14D"]), Convert.ToString(Dr["Price21D"]), Convert.ToString(Dr["Price30D"]),
                    Convert.ToString(Dr["Price40D"]), Convert.ToString(Dr["Price45D"]), Convert.ToString(Dr["Price60D"]),
                    Convert.ToString(Dr["Price75D"]), Convert.ToString(Dr["Price90D"]), Convert.ToString(Dr["Price120D"]),
                    Convert.ToString(Dr["Price150D"]), Convert.ToString(Dr["Price180D"]), Convert.ToString(Dr["Tolerance"]),
                    Convert.ToString(Dr["GroupId"]), Convert.ToString(Dr["SubGroup1Id"]), Convert.ToString(Dr["SubGroup2Id"]),
                    Convert.ToString(Dr["ItemId"]), Convert.ToString(Dr["PriceListNo"]));

                }
                Dr.Close();
                Conn.Close();

                if (PLJA.dgvPriceListJualAktif.RowCount == 0)
                {
                    this.dgvPLJDetails.Rows.Add((dgvPLJDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], "", (dgvPLJDetails.RowCount + 1).ToString(), Price0D, Price2D, Price3D, Price7D, Price14D, Price21D, Price30D, Price40D, Price45D, Price60D, Price75D, Price90D, Price120D, Price150D, Price180D, Tolerance, GroupId[i], SubGroup1Id[i], SubGroup2Id[i], ItemId[i]);
                }
                else
                {
                    PLJA.ShowDialog();
                }
           }  
        }

        public void SetGridDetail(string PriceListNo, string FullItemID)
        {
            if (dgvPLJDetails.RowCount - 1 <= 0)
            {
                dgvPLJDetails.ColumnCount = 25;
                dgvPLJDetails.Columns[0].Name = "No";
                dgvPLJDetails.Columns[1].Name = "FullItemID";
                dgvPLJDetails.Columns[2].Name = "ItemName";
                dgvPLJDetails.Columns[3].Name = "DeliveryMethod";
                dgvPLJDetails.Columns[4].Name = "SeqNoGroup";
                dgvPLJDetails.Columns[5].Name = "Price0D";
                dgvPLJDetails.Columns[6].Name = "Price2D";
                dgvPLJDetails.Columns[7].Name = "Price3D";
                dgvPLJDetails.Columns[8].Name = "Price7D";
                dgvPLJDetails.Columns[9].Name = "Price14D";
                dgvPLJDetails.Columns[10].Name = "Price21D";
                dgvPLJDetails.Columns[11].Name = "Price30D";
                dgvPLJDetails.Columns[12].Name = "Price40D";
                dgvPLJDetails.Columns[13].Name = "Price45D";
                dgvPLJDetails.Columns[14].Name = "Price60D";
                dgvPLJDetails.Columns[15].Name = "Price75D";
                dgvPLJDetails.Columns[16].Name = "Price90D";
                dgvPLJDetails.Columns[17].Name = "Price120D";
                dgvPLJDetails.Columns[18].Name = "Price150D";
                dgvPLJDetails.Columns[19].Name = "Price180D";
                dgvPLJDetails.Columns[20].Name = "Tolerance";
                dgvPLJDetails.Columns[21].Name = "GroupId";
                dgvPLJDetails.Columns[22].Name = "SubGroup1Id";
                dgvPLJDetails.Columns[23].Name = "SubGroup2Id";
                dgvPLJDetails.Columns[24].Name = "ItemId";

                dgvPLJDetails.Columns[20].HeaderText = "Tolerance(%)";

            }

            string Price0D = "0.0000";
            string Price2D = "0.0000";
            string Price3D = "0.0000";
            string Price7D = "0.0000";
            string Price14D = "0.0000";
            string Price21D = "0.0000";
            string Price30D = "0.0000";
            string Price40D = "0.0000";
            string Price45D = "0.0000";
            string Price60D = "0.0000";
            string Price75D = "0.0000";
            string Price90D = "0.0000";
            string Price120D = "0.0000";
            string Price150D = "0.0000";
            string Price180D = "0.0000";
            string Tolerance = "0.0000";

            dgvPLJDetails.ReadOnly = false;
            dgvPLJDetails.Columns["No"].ReadOnly = true;
            dgvPLJDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLJDetails.Columns["ItemName"].ReadOnly = true;

            dgvPLJDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPLJDetails.Columns["GroupId"].Visible = false;
            dgvPLJDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPLJDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPLJDetails.Columns["ItemId"].Visible = false;
            dgvPLJDetails.Columns["SeqNoGroup"].Visible = false;
            dgvPLJDetails.AutoResizeColumns();

            int SeqNo = this.dgvPLJDetails.RowCount + 1;
            Conn = ConnectionString.GetConnection();
            string Query = "";
            Query = "SELECT d.DeliveryMethod, d.GroupID AS GroupId, d.SubGroupID AS SubGroup1Id, d.SubGroup2ID AS SubGroup2Id, d.ItemID AS ItemId, d.FullItemID, d.ItemName, ";
            Query += "d.Price0D, d.Price2D, d.Price3D, d.Price7D, d.Price14D, d.Price21 AS Price21D, d.Price30D, d.Price40D, ";
            Query += "d.Price45D, d.Price60D, d.Price75D, d.Price90D, d.Price120D, d.Price150D, d.Price180D, d.Tolerance FROM SalesPriceListDtl d ";
            Query += "WHERE d.PriceListNo = '" + PriceListNo + "' AND FullItemID = '" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            if (Dr.Read())
            {
                this.dgvPLJDetails.Rows.Add(SeqNo, Convert.ToString(Dr["FullItemID"]),
                Convert.ToString(Dr["ItemName"]), Convert.ToString(Dr["DeliveryMethod"]), SeqNo, Convert.ToString(Dr["Price0D"]),
                Convert.ToString(Dr["Price2D"]), Convert.ToString(Dr["Price3D"]), Convert.ToString(Dr["Price7D"]),
                Convert.ToString(Dr["Price14D"]), Convert.ToString(Dr["Price21D"]), Convert.ToString(Dr["Price30D"]),
                Convert.ToString(Dr["Price40D"]), Convert.ToString(Dr["Price45D"]), Convert.ToString(Dr["Price60D"]),
                Convert.ToString(Dr["Price75D"]), Convert.ToString(Dr["Price90D"]), Convert.ToString(Dr["Price120D"]),
                Convert.ToString(Dr["Price150D"]), Convert.ToString(Dr["Price180D"]), Convert.ToString(Dr["Tolerance"]),
                Convert.ToString(Dr["GroupId"]), Convert.ToString(Dr["SubGroup1Id"]), Convert.ToString(Dr["SubGroup2Id"]),
                Convert.ToString(Dr["ItemId"]));

            }
            else
            {
                Query = "SELECT GroupId, SubGroup1Id, SubGroup2Id, ItemId, ItemDeskripsi AS ItemName FROM InventTable WHERE FullItemId = '" + FullItemID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.Read())
                {
                    this.dgvPLJDetails.Rows.Add(SeqNo, FullItemID, Convert.ToString(Dr["ItemName"]), "", SeqNo, Price0D, Price2D, Price3D, Price7D, Price14D, Price21D, Price30D, Price40D, Price45D, Price60D, Price75D, Price90D, Price120D, Price150D, Price180D, Tolerance, Convert.ToString(Dr["GroupId"]), Convert.ToString(Dr["SubGroup1Id"]), Convert.ToString(Dr["SubGroup2Id"]), Convert.ToString(Dr["ItemId"]));
                }
            }
            Dr.Close();
            Conn.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            if (dgvPLJDetails.RowCount > 0)
            {
                Index = dgvPLJDetails.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvPLJDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvPLJDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvPLJDetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string NumberGroupSeq = dgvPLJDetails.Rows[dgvPLJDetails.CurrentCell.RowIndex].Cells["SeqNoGroup"].Value.ToString();

                    //if (dgvPLJDetails.Rows[dgvPLJDetails.CurrentCell.RowIndex].Cells["Base"].Value.ToString() == "Y")
                    //{
                    //    for (int i = 0; i < dgvPLJDetails.RowCount; i++)
                    //    {
                    //        if (dgvPLJDetails.Rows[i].Cells["SeqNoGroup"].Value.ToString() == NumberGroupSeq)
                    //        {
                    //            dgvPLJDetails.Rows.RemoveAt(i);
                    //            i--;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                        dgvPLJDetails.Rows.RemoveAt(Index);
                   // }
                    SortNoDataGrid();
                }
                //GetGelombang();
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvPLJDetails.RowCount; i++)
            {
                dgvPLJDetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void HeaderPLJ2_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void HeaderPLJ2_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < ListDetailPLJ.Count(); i++)
            {
                ListDetailPLJ[i].Close();
            }
            //for (int i = 0; i < ListGelombang.Count(); i++)
            //{
            //    ListGelombang[i].Close();
            //}
            //for (int i = 0; i < ListInfo.Count(); i++)
            //{
            //    ListInfo[i].Close();
            //}
            //for (int i = 0; i < ListVendor.Count(); i++)
            //{
            //    ListVendor[i].Close();
            //}
            Purchase.PurchaseQuotation.FormPQ.reffID = null;
          //  Sales.PriceListJual.InqueryPLJ f = new Sales.PriceListJual.InqueryPLJ();
          //  f.RefreshGrid();
            //if (Mode != "ModeView")
            //{
            //    Parent.RefreshGrid();
            //}
        }

        private void dgvPLJDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "DeliveryMethod")
            {
                if (!char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            if (dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price0D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price2D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price7D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price14D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price21D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price30D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price40D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price45D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price60D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price75D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price90D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price120D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price150D" || dgvPLJDetails.Columns[dgvPLJDetails.CurrentCell.ColumnIndex].Name == "Price180D")
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
        }

        private void dgvPLJDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (Mode != "BeforeEdit")
                {

                    if (dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                    {
                        DeliveryMethod.Location = dgvPLJDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        DeliveryMethod.Visible = true;
                        string tmpFullItemId = dgvPLJDetails.Rows[dgvPLJDetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                        string tmpDeliveryMethod = "";
                        Conn = ConnectionString.GetConnection();
                        for (int i = 0; i < dgvPLJDetails.RowCount; i++)
                        {
                            if (dgvPLJDetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                            {
                                if (dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value != null)
                                {
                                    if (tmpDeliveryMethod == "")
                                    {
                                        tmpDeliveryMethod = "'" + dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                                    }
                                    else
                                    {
                                        tmpDeliveryMethod += ",'" + dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
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
                        DeliveryMethod.SelectedIndex = 0;
                        DrCmb.Close();

                        Conn.Close();
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }

        }

        private void dgvPLJDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Mode != "BeforeEdit")
            {



                if (dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Visible = false;
                }

                //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                //{
                //    dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value = Convert.ToDecimal(dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value).ToString("N2");
                //}

              //  dtp.Visible = false;
                dgvPLJDetails.AutoResizeColumns();

                if (dgvPLJDetails.RowCount > 0)
                {
                    if (dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Tolerance")
                    {
                        if (Convert.ToDecimal(dgvPLJDetails.Rows[e.RowIndex].Cells["Tolerance"].Value.ToString() != "" ? dgvPLJDetails.Rows[e.RowIndex].Cells["Tolerance"].Value.ToString() : "0") > 100)
                        {
                            dgvPLJDetails.CurrentCell.Value = "100";
                            MessageBox.Show("Tolerance tidak boleh lebih dari 100");
                            return;
                        }
                    }
                }


            }
        }

        private void dgvPLJDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPLJDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPLJDetails_KeyPress);
            }
        }

        private void dgvPLJDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price0D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price2D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price3D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price7D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price14D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price21D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price30D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price40D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price45D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price60D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price75D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price90D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price120D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price150D" || dgvPLJDetails.Columns[e.ColumnIndex].Name.ToString() == "Price180D")
                {
                    string FullItemID = dgvPLJDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                    
                  //  PopUp.PriceListJual.PriceListJual PopUpPriceList = new PopUp.PriceListJual.PriceListJual();
                  //  PriceListJual.Add(PopUpPriceList);
                   // PopUpPriceList.GetData(FullItemID);
                   // PopUpPriceList.Y += 100 * i;
                   // PopUpPriceList.Show();

                    PopUp.PriceListJual.PriceListJual tmpSearch = new PopUp.PriceListJual.PriceListJual();
                    tmpSearch.FullItemID = FullItemID;
                    //tmpSearch.RefreshGrid(dgvPrDetails.CurrentCell.Value.ToString());
                    tmpSearch.ShowDialog();
                    //dgvPrDetails.CurrentCell.Value = tmpSearch.VendId;
                    //tmpSearch.VendId = "";
                    
                }
            }
        }

        public void ModeNew()
        {
            PLJNumber = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            dtPljDate.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;

            if (dgvPLJDetails.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }
            else
            {
                for (int i = 0; i <= dgvPLJDetails.RowCount - 1; i++)
                {
                    string Price0D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price0D"].Value);
                    string Price2D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price2D"].Value);
                    string Price3D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price3D"].Value);
                    string Price7D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price7D"].Value);
                    string Price14D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price14D"].Value);
                    string Price21D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price21D"].Value);
                    string Price30D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price30D"].Value);
                    string Price40D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price40D"].Value);
                    string Price45D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price45D"].Value);
                    string Price60D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price60D"].Value);
                    string Price75D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price75D"].Value);
                    string Price90D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price90D"].Value);
                    string Price120D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price120D"].Value);
                    string Price150D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price150D"].Value);
                    string Price180D = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Price180D"].Value);
                    string Tolerance = Convert.ToString(dgvPLJDetails.Rows[i].Cells["Tolerance"].Value);

                    if ((dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) == "")
                    {
                    
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", DeliveryMethod tidak boleh kosong.");
                            return;
                     }
                    else if (Convert.ToDecimal((Price0D == "" ? "0.000" : Price0D)) <= 0)
                    {
                        
                        MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price0D tidak boleh lebih kecil atau sama dengan 0");
                        return; 
                    }
                    else if (Convert.ToDecimal((Price2D == "" ? "0.000" : Price2D)) <= 0)
                    {
                     
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price2D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                       
                    }
                    else if (Convert.ToDecimal((Price3D == "" ? "0.000" : Price3D)) <= 0)
                    {
                     
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price3D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price7D == "" ? "0.000" : Price7D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price7D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                     }
                    else if (Convert.ToDecimal((Price14D == "" ? "0.000" : Price14D)) <= 0)
                    {
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price14D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price21D == "" ? "0.000" : Price21D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price21D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price30D == "" ? "0.000" : Price30D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price30D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price40D == "" ? "0.000" : Price40D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price40D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price45D == "" ? "0.000" : Price45D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price45D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price60D == "" ? "0.000" : Price60D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price60D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price75D == "" ? "0.000" : Price75D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price75D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price90D == "" ? "0.000" : Price90D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price90D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price120D == "" ? "0.000" : Price120D)) <= 0)
                    {
                     
                            MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price120D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price150D == "" ? "0.000" : Price150D)) <= 0)
                    {
                          MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price150D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price180D == "" ? "0.000" : Price180D)) <= 0)
                    {

                        MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Price180D tidak boleh lebih kecil atau sama dengan 0");
                        return;

                    }
                    else if (Convert.ToDecimal((Tolerance == "" ? "0.000" : Tolerance)) <= 0)
                    {

                        MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Tolerance tidak boleh lebih kecil atau sama dengan 0");
                        return;

                    }
                    else if (Convert.ToDecimal(Tolerance) > 100)
                    {

                        MessageBox.Show("Item No = " + dgvPLJDetails.Rows[i].Cells["No"].Value + ", Tolerance tidak boleh lebih dari 100");
                        return;

                    }
                    
                }

                DateTime fromDate = DateTime.ParseExact(dtFrom.Text, "dd/MM/yyyy HH:mm:ss", null);
                DateTime toDate = DateTime.ParseExact(dtTo.Text, "dd/MM/yyyy HH:mm:ss", null);
                DateTime CurrentDate = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), "dd/MM/yyyy HH:mm:ss", null);

                if (fromDate > toDate)
                {

                    MessageBox.Show("Valid From harus lebih kecil dari Valid To");
                    return;
                }
                else if (CurrentDate > toDate)
                {

                    MessageBox.Show("Valid To harus lebih besar dari hari ini");
                    return;

                }

                else if (Convert.ToString(cmbCustomer.SelectedValue) == "2" || Convert.ToString(cmbCustomer.SelectedValue) == "3")
                {
                    if (dgvCustomer.RowCount == 0)
                    {
                        MessageBox.Show("Customer List tidak boleh kosong");
                        return;
                    }
                }
            }

            try
            {
                if (Mode == "New" || txtPLJNumber.Text == "")
                {
                    //Old Code=======================================================================================                   
                    //Query = "Insert into SalesPriceListH (PriceListNo,ValidFrom,ValidTo,StatusCode,CreatedDate,CreatedBy, Criteria, Notes) OUTPUT INSERTED.PriceListNo values ";
                    //Query += "((Select 'PLJ-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(PriceListNo) is null then '1' else substring(Max(PriceListNo),11,5)+1 end), 5) ";
                    //Query += "from [SalesPriceListH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    //Query += "'" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    //Query += "'01',getdate(),'" + ControlMgr.UserId + "', '"+Convert.ToString(cmbCustomer.SelectedValue)+"', '"+txtNotes.Text+"');";
                    //Cmd = new SqlCommand(Query, Conn, Trans);

                    //string PLJNumber = Cmd.ExecuteScalar().ToString();
                    //End Old Code=====================================================================================

                    //begin============================================================================================
                    //updated by : joshua
                    //updated date : 13 Feb 2018
                    //description : change generate sequence number, get from global function and update counter 
                    string Jenis = "PLJ", Kode = "PLJ";
                    string PLJNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    Query = "Insert into SalesPriceListH (PriceListNo,ValidFrom,ValidTo,StatusCode,CreatedDate,CreatedBy, Criteria, Notes) values ";
                    Query += "('" + PLJNumber + "',";
                    Query += "'" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    Query += "'01',getdate(),'" + ControlMgr.UserId + "', '" + Convert.ToString(cmbCustomer.SelectedValue) + "', '" + txtNotes.Text + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //update counter
                    //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                    //end update counter
                    //end=============================================================================================
                    

                    Query = "";
                    for (int i = 0; i <= dgvPLJDetails.RowCount - 1; i++)
                    {
                        Query += "Insert SalesPriceListDtl (PriceListNo,SeqNo,FullItemID,ItemName,DeliveryMethod,GroupID,SubGroupID,SubGroup2ID,ItemID,Price0D,Price2D,Price3D,Price7D,Price14D,Price21,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,Tolerance,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PLJNumber + "',";
                        Query += (dgvPLJDetails.Rows[i].Cells["No"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["No"].Value.ToString()) + ",'";
                        Query += (dgvPLJDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "',";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price0D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price0D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price2D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price2D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price3D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price3D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price7D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price7D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price14D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price14D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price21D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price21D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price30D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price30D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price40D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price40D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price45D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price45D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price60D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price60D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price75D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price75D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price90D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price90D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price120D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price120D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price150D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price150D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price180D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price180D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Tolerance"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Tolerance"].Value.ToString()) + ",";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";

                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }
                    }

                 
                    if (Query != "")
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    for (int i = 0; i <= dgvCustomer.RowCount - 1; i++)
                    {
                        Query = "Insert SalesPriceList_CustList (PriceListNo,CustID,Name,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PLJNumber + "',";
                        Query += "'" + (dgvCustomer.Rows[i].Cells["CustID"].Value == null ? "" : dgvCustomer.Rows[i].Cells["CustID"].Value.ToString()) + "',";
                        Query += "'" + (dgvCustomer.Rows[i].Cells["CustName"].Value == null ? "" : dgvCustomer.Rows[i].Cells["CustName"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    String StatusDesc = null;
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='01' AND TransCode = 'SalesPriceList' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                    }
                    Dr.Close();

                    string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };

                    Query = "Insert into SalesPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES(getdate(), '" + PLJNumber + "', '01', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();


                    Trans.Commit();
                    MessageBox.Show("Data PLJNumber : " + PLJNumber + " berhasil ditambahkan.");
                    txtPLJNumber.Text = PLJNumber;
                    MainMenu f = new MainMenu();
                    //f.refreshTaskList();
                }
                else
                {
                    Query = "Update SalesPriceListH set ";
                    Query += "ValidFrom='" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    Query += "ValidTo='" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    Query += "Criteria='" + Convert.ToString(cmbCustomer.SelectedValue) + "',";
                    Query += "Notes='" + txtNotes.Text + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLJNumber.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "Delete from SalesPriceListDtl where PriceListNo='" + txtPLJNumber.Text.Trim() + "';";
                    for (int i = 0; i <= dgvPLJDetails.RowCount - 1; i++)
                    {
                        Query += "Insert SalesPriceListDtl (PriceListNo,SeqNo,FullItemID,ItemName,DeliveryMethod,GroupID,SubGroupID,SubGroup2ID,ItemID,Price0D,Price2D,Price3D,Price7D,Price14D,Price21,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,Tolerance,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PLJNumber + "',";
                        Query += (dgvPLJDetails.Rows[i].Cells["No"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["No"].Value.ToString()) + ",'";
                        Query += (dgvPLJDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                        Query += (dgvPLJDetails.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPLJDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "',";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price0D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price0D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price2D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price2D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price3D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price3D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price7D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price7D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price14D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price14D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price21D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price21D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price30D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price30D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price40D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price40D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price45D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price45D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price60D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price60D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price75D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price75D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price90D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price90D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price120D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price120D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price150D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price150D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Price180D"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Price180D"].Value.ToString()) + ",";
                        Query += (dgvPLJDetails.Rows[i].Cells["Tolerance"].Value == null ? "0.0000" : dgvPLJDetails.Rows[i].Cells["Tolerance"].Value.ToString()) + ",";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "');";

                        if (i % 5 == 0 && i > 0)
                        {
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }

                    }
                    if (Query != "")
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }

                    Query = "Delete from SalesPriceList_CustList where PriceListNo='" + txtPLJNumber.Text.Trim() + "';";                    
                    for (int i = 0; i <= dgvCustomer.RowCount - 1; i++)
                    {
                        Query += "Insert SalesPriceList_CustList (PriceListNo,CustID,Name,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PLJNumber + "',";
                        Query += "'" + (dgvCustomer.Rows[i].Cells["CustID"].Value == null ? "" : dgvCustomer.Rows[i].Cells["CustID"].Value.ToString()) + "',";
                        Query += "'" + (dgvCustomer.Rows[i].Cells["CustName"].Value == null ? "" : dgvCustomer.Rows[i].Cells["CustName"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    Trans.Commit();
                    MessageBox.Show("Data PriceListNo : " + txtPLJNumber.Text + " berhasil diupdate.");

                }
               
            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
                GetDataHeader();
                Parent.RefreshGrid();
                ModeBeforeEdit();
            }
        }

        public void GetDataHeader()
        {
            if (PLJNumber == "")
            {
                PLJNumber = txtPLJNumber.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            Query = "Select a.[PriceListNo],a.[ValidFrom],a.[ValidTo],a.[StatusCode], a.[CreatedDate],b.Deskripsi, a.Notes, a.Criteria From [SalesPriceListH] a ";
            Query += "left join TransStatusTable b on b.StatusCode = a.StatusCode ";
            Query += "Where PriceListNo = '" + PLJNumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPLJNumber.Text = Dr["PriceListNo"].ToString();
                dtPljDate.Text = Dr["CreatedDate"].ToString();
                dtFrom.Text = Dr["ValidFrom"].ToString();
                dtTo.Text = Dr["ValidTo"].ToString();
                txtNotes.Text = Dr["Notes"].ToString();
                cmbCustomer.SelectedValue = Dr["Criteria"].ToString();
            }
            Dr.Close();

            dgvPLJDetails.Rows.Clear();
            if (dgvPLJDetails.RowCount - 1 <= 0)
            {
                dgvPLJDetails.ColumnCount = 25;
                dgvPLJDetails.Columns[0].Name = "No";
                dgvPLJDetails.Columns[1].Name = "FullItemID";
                dgvPLJDetails.Columns[2].Name = "ItemName";
                dgvPLJDetails.Columns[3].Name = "DeliveryMethod";
                dgvPLJDetails.Columns[4].Name = "SeqNoGroup";
                dgvPLJDetails.Columns[5].Name = "Price0D";
                dgvPLJDetails.Columns[6].Name = "Price2D";
                dgvPLJDetails.Columns[7].Name = "Price3D";
                dgvPLJDetails.Columns[8].Name = "Price7D";
                dgvPLJDetails.Columns[9].Name = "Price14D";
                dgvPLJDetails.Columns[10].Name = "Price21D";
                dgvPLJDetails.Columns[11].Name = "Price30D";
                dgvPLJDetails.Columns[12].Name = "Price40D";
                dgvPLJDetails.Columns[13].Name = "Price45D";
                dgvPLJDetails.Columns[14].Name = "Price60D";
                dgvPLJDetails.Columns[15].Name = "Price75D";
                dgvPLJDetails.Columns[16].Name = "Price90D";
                dgvPLJDetails.Columns[17].Name = "Price120D";
                dgvPLJDetails.Columns[18].Name = "Price150D";
                dgvPLJDetails.Columns[19].Name = "Price180D";
                dgvPLJDetails.Columns[20].Name = "Tolerance";
                dgvPLJDetails.Columns[21].Name = "GroupId";
                dgvPLJDetails.Columns[22].Name = "SubGroup1Id";
                dgvPLJDetails.Columns[23].Name = "SubGroup2Id";
                dgvPLJDetails.Columns[24].Name = "ItemId";

                dgvPLJDetails.Columns[20].HeaderText = "Tolerance(%)";

            }

            Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[DeliveryMethod],Price0D,Price2D,Price3D,Price7D,Price14D,Price21 AS Price21D,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,Tolerance,GroupId,SubGroupID,SubGroup2ID,ItemID From [SalesPriceListDtl] Where PriceListNo = '" + PLJNumber + "' order by SeqNo asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {

                this.dgvPLJDetails.Rows.Add(i+1, Dr[1], Dr[2], Dr[3], Dr[0], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23]);
                i++;
            }
            Dr.Close();

            dgvPLJDetails.ReadOnly = false;
            dgvPLJDetails.Columns["No"].ReadOnly = true;
            dgvPLJDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLJDetails.Columns["ItemName"].ReadOnly = true;
            //dgvPLJDetails.Columns["Base"].ReadOnly = true;

            dgvPLJDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
         //   dgvPLJDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
         //   dgvPLJDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["GroupId"].Visible = false;
            dgvPLJDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPLJDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPLJDetails.Columns["ItemId"].Visible = false;
         //   dgvPLJDetails.Columns["BracketId"].Visible = false;
            dgvPLJDetails.Columns["SeqNoGroup"].Visible = false;

            //dgvPLJDetails.Columns["Base"].Visible = false;

            dgvPLJDetails.AutoResizeColumns();



            //Customer
            this.dgvCustomer.Rows.Clear();
            if (dgvCustomer.RowCount - 1 <= 0)
            {
                dgvCustomer.ColumnCount = 3;
                dgvCustomer.Columns[0].Name = "No";
                dgvCustomer.Columns[1].Name = "CustID";
                dgvCustomer.Columns[2].Name = "CustName";
            }

            Query = "Select CustID, Name AS CustName From [SalesPriceList_CustList] Where PriceListNo = '" + PLJNumber + "' order by CustID asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int j = 0;
            while (Dr.Read())
            {

                dgvCustomer.Rows.Add(j + 1, Dr[0], Dr[1]);
                j++;
            }
            Dr.Close();

            dgvCustomer.ReadOnly = false;
            dgvCustomer.Columns["No"].ReadOnly = true;
            dgvCustomer.Columns["CustID"].ReadOnly = true;
            dgvCustomer.Columns["CustName"].ReadOnly = true;

            dgvCustomer.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvCustomer.AutoResizeColumns();
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
            dtFrom.Enabled = false;
            dtTo.Enabled = false;

            btnAddCustomer.Enabled = false;
            btnDeleteCustomer.Enabled = false;
            cmbCustomer.Enabled = false;
            txtNotes.Enabled = false;
            //dtPLJDate.Enabled = false;
           // cmbPrType.Enabled = false;
           // txtPrStatus.Enabled = false;

            dgvPLJDetails.ReadOnly = true;
            dgvCustomer.ReadOnly = true;
            BeforeEditColor();
            dgvPLJDetails.DefaultCellStyle.BackColor = Color.LightGray;
            dgvCustomer.DefaultCellStyle.BackColor = Color.LightGray;
        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvPLJDetails.RowCount; i++)
            {
                dgvPLJDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price0D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price2D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price3D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price7D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price14D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price21D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price30D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price40D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price45D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price60D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price75D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price90D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price120D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price150D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Price180D"].Style.BackColor = Color.LightGray;
                dgvPLJDetails.Rows[i].Cells["Tolerance"].Style.BackColor = Color.LightGray;
            }

            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {
                dgvCustomer.Rows[i].Cells["CustID"].Style.BackColor = Color.LightGray;
                dgvCustomer.Rows[i].Cells["CustName"].Style.BackColor = Color.LightGray;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                if (txtPLJNumber.Text != "")
                {
                    Query = "Select StatusCode from [dbo].[SalesPriceListH] where [PriceListNo]='" + txtPLJNumber.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Check = Cmd.ExecuteScalar().ToString();
                    if (Check == "02" || Check == "03")
                    {
                        MessageBox.Show("PriceListNo = " + txtPLJNumber.Text + ".\n" + "Tidak bisa diedit karena sudah diproses.");
                        Conn.Close();
                        return;
                    }
                }

                Mode = "Edit";

                btnSave.Visible = true;
                btnExit.Visible = false;
                btnEdit.Visible = false;
                btnCancel.Visible = true;

                dtPljDate.Enabled = false;

                btnAddCustomer.Enabled = true;
                btnDeleteCustomer.Enabled = true;
                cmbCustomer.Enabled = true;
                txtNotes.Enabled = true;

                ModeEdit();
                //MethodReadOnlyRowBaseN();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
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
            dtPljDate.Enabled = false;
            dtFrom.Enabled = true;
            dtTo.Enabled = true;

            dgvPLJDetails.ReadOnly = false;
            dgvPLJDetails.Columns["No"].ReadOnly = true;
            dgvPLJDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLJDetails.Columns["ItemName"].ReadOnly = true;
            //dgvPLJDetails.Columns["Base"].ReadOnly = true;

            dgvPLJDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
           // dgvPLJDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
          //  dgvPLJDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLJDetails.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPLJDetails.AutoResizeColumns();
            EditColor();
            dgvPLJDetails.DefaultCellStyle.BackColor = Color.White;

            dgvCustomer.ReadOnly = false;
            dgvCustomer.Columns["No"].ReadOnly = true;
            dgvCustomer.Columns["CustID"].ReadOnly = true;
            dgvCustomer.Columns["CustName"].ReadOnly = true;
            dgvCustomer.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.AutoResizeColumns();
            dgvCustomer.DefaultCellStyle.BackColor = Color.White;
            EditColorCustomer();

        }

        private void EditColorCustomer()
        {
            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {

                dgvCustomer.Rows[i].Cells["CustID"].Style.BackColor = Color.White;
                dgvCustomer.Rows[i].Cells["CustName"].Style.BackColor = Color.White;
            }
        }

        private void dgvPLJDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == dgvPLJDetails.Columns["Price0D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price2D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price3D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price7D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price14D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price21D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price30D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price40D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price45D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price60D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price75D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price90D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price120D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price150D"].Index || e.ColumnIndex == dgvPLJDetails.Columns["Price180D"].Index) && e.Value != null)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dtPljDate.Enabled = false;

            ModeBeforeEdit();
            GetDataHeader();
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            PopUp.Customer.Customer CS = new PopUp.Customer.Customer();
            List<PopUp.Customer.Customer> ListCustomer = new List<PopUp.Customer.Customer>();
            CS.ParentRefreshGrid(this);
            CS.ParamHeader(dgvCustomer);
            CS.ShowDialog();
            EditColorCustomer();
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (dgvCustomer.RowCount > 0)
            {
                Index = dgvCustomer.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvCustomer.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " CustID = " + dgvCustomer.Rows[Index].Cells["CustID"].Value.ToString() + Environment.NewLine + " CustName = " + dgvCustomer.Rows[Index].Cells["CustName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string NumberGroupSeq = dgvCustomer.Rows[dgvCustomer.CurrentCell.RowIndex].Cells["No"].Value.ToString();
                    dgvCustomer.Rows.RemoveAt(Index);
                    SortNoDataGridCustomer();
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        public void AddDataGridCustomer(List<string> CustID, List<string> CustName)
        {
            if (dgvCustomer.RowCount - 1 <= 0)
            {
                dgvCustomer.ColumnCount = 3;
                dgvCustomer.Columns[0].Name = "No";
                dgvCustomer.Columns[1].Name = "CustID";
                dgvCustomer.Columns[2].Name = "CustName";
            }

            for (int i = 0; i < CustID.Count; i++)
            {
                this.dgvCustomer.Rows.Add((dgvCustomer.RowCount + 1).ToString(), CustID[i], CustName[i]);
            }

            dgvCustomer.ReadOnly = false;
            dgvCustomer.Columns["No"].ReadOnly = true;
            dgvCustomer.Columns["CustID"].ReadOnly = true;
            dgvCustomer.Columns["CustName"].ReadOnly = true;

            dgvCustomer.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvCustomer.Columns["CustName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvCustomer.AutoResizeColumns();
        }

        private void SortNoDataGridCustomer()
        {
            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {
                dgvCustomer.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void Customer_SelectedIndexChange(object sender, EventArgs e)
        {
            if (Convert.ToString(cmbCustomer.SelectedValue) == "1" || Convert.ToString(cmbCustomer.SelectedValue) == "")
            {
                btnAddCustomer.Enabled = false;

                for (int i = 0; i < dgvCustomer.RowCount; i++)
                {
                    dgvCustomer.Rows.Clear();
                    dgvCustomer.Refresh();
                }
            }
            else
            {
                btnAddCustomer.Enabled = true;
            }
        }

    }
}
