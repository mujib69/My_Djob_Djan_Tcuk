using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PriceListBeli
{
    public partial class HeaderPLB : MetroFramework.Forms.MetroForm
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
        public string PLBNumber = "", tmpPrType = "";
        Purchase.PriceListBeli.InqueryPLB Parent;
        DateTimePicker dtp;

       // List<Gelombang> ListGelombang = new List<Gelombang>();

        List<DetailPLB> ListDetailPLB = new List<DetailPLB>();
        List<PopUp.PriceListBeli.PriceListBeli> PriceListBeli = new List<PopUp.PriceListBeli.PriceListBeli>();
        List<PopUp.VendorReference.VendorReference> ListVendorReference = new  List<PopUp.VendorReference.VendorReference>();

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public HeaderPLB()
        {
            InitializeComponent();
           
        }
        
        private void HeaderPLB2_Load(object sender, EventArgs e)
        {          
            SetCmbVendorReference();
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

            dgvPLBDetails.Controls.Add(dtp);
           // dtp.ValueChanged += this.dtp_ValueChanged;
            dgvPLBDetails.CellBeginEdit += this.dgvPLBDetails_CellBeginEdit;
            dgvPLBDetails.CellEndEdit += this.dgvPLBDetails_CellEndEdit;

            DeliveryMethod = new ComboBox();
            DeliveryMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            DeliveryMethod.Visible = false;
            dgvPLBDetails.Controls.Add(DeliveryMethod);
            DeliveryMethod.DropDownClosed += this.DeliveryMethod_DropDownClosed;
            DeliveryMethod.SelectionChangeCommitted += this.DeliveryMethod_SelectionChangeCommitted;
            Purchase.PriceListBeli.InqueryPLB f = new Purchase.PriceListBeli.InqueryPLB();
           // f.RefreshGrid();
        }
        
        private void SetCmbVendorReference()
        {
            cmbVendorReference.DisplayMember = "Text";
            cmbVendorReference.ValueMember = "Value";

            var items = new[] { 
                new { Text = "-select-", Value = "" }, 
                new { Text = "All Vendor", Value = "1" }, 
                new { Text = "All Vendor Except", Value = "2" },
                new { Text = "Specific Vendor", Value = "3" }
            };

            cmbVendorReference.DataSource = items;
            cmbVendorReference.DropDownStyle = ComboBoxStyle.DropDownList;
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
            dtPlbDate.Enabled = false;

            dgvPLBDetails.ReadOnly = true;
            BeforeEditColor();
            dgvPLBDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }

        private void dgvPrDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (Mode != "BeforeEdit")
                {

                    if (dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                    {
                        DeliveryMethod.Location = dgvPLBDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        DeliveryMethod.Visible = true;
                        string tmpFullItemId = dgvPLBDetails.Rows[dgvPLBDetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                        string tmpDeliveryMethod = "";
                        Conn = ConnectionString.GetConnection();
                        for (int i = 0; i < dgvPLBDetails.RowCount; i++)
                        {
                            if (dgvPLBDetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                            {
                                if (dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value != null)
                                {
                                    if (tmpDeliveryMethod == "")
                                    {
                                        tmpDeliveryMethod = "'" + dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                                    }
                                    else
                                    {
                                        tmpDeliveryMethod += ",'" + dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
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

        private void dgvPrDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Mode != "BeforeEdit")
            {


                if (dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Visible = false;
                }

                //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                //{
                //    dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value = Convert.ToDecimal(dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value).ToString("N2");
                //}

                dtp.Visible = false;
                dgvPLBDetails.AutoResizeColumns();

            }

        }

        private void DeliveryMethod_DropDownClosed(object sender, EventArgs e)
        {
            DeliveryMethod.Visible = false;
        }

        private void DeliveryMethod_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvPLBDetails.CurrentCell.Value = DeliveryMethod.Text.ToString();
            for (int j = 0; j < dgvPLBDetails.RowCount; j++)
            {
                if (dgvPLBDetails.Rows[j].Cells["SeqNoGroup"].Value == dgvPLBDetails.Rows[dgvPLBDetails.CurrentCell.RowIndex].Cells["No"].Value.ToString())
                {
                    dgvPLBDetails.Rows[j].Cells["DeliveryMethod"].Value = dgvPLBDetails.Rows[dgvPLBDetails.CurrentCell.RowIndex].Cells["DeliveryMethod"].Value.ToString();
                }
            }
        }

        public void SetMode(string tmpMode, string tmpPLBNumber)
        {
            Mode = tmpMode;
            PLBNumber = tmpPLBNumber;
            txtPLBNumber.Text = tmpPLBNumber;
        }

        public void SetParent(Purchase.PriceListBeli.InqueryPLB F)
        {
            Parent = F;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
           // Gelombang = "";
           // Bracket = "";
            DetailPLB DetailPLB = new DetailPLB();

            List<DetailPLB> ListDetailPLB = new List<DetailPLB>();
            DetailPLB.ParentRefreshGrid(this);
            DetailPLB.ParamHeader(dgvPLBDetails);
            DetailPLB.ShowDialog();
            //CallFormGelombang(Gelombang, Bracket);
            //MethodReadOnlyRowBaseN();
            
            EditColor();
        }

        private void EditColor()
        {
            for (int i = 0; i < dgvPLBDetails.RowCount; i++)
            {

                dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price0D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price2D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price3D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price7D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price14D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price21D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price30D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price40D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price45D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price60D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price75D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price90D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price120D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price150D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Price180D"].Style.BackColor = Color.White;
                dgvPLBDetails.Rows[i].Cells["Tolerance"].Style.BackColor = Color.White;

                
            }
        }

        private void EditColorVendor()
        {
            for (int i = 0; i < dgvVendorReference.RowCount; i++)
            {

                dgvVendorReference.Rows[i].Cells["VendID"].Style.BackColor = Color.White;
                dgvVendorReference.Rows[i].Cells["VendName"].Style.BackColor = Color.White;
            }
        }

        //private void MethodReadOnlyRowBaseN()
        //{
        //    for (int i = 0; i < dgvPLBDetails.RowCount; i++)
        //    {
        //        if (dgvPLBDetails.Rows[i].Cells["Base"].Value.ToString() == "N")
        //        {
        //            dgvPLBDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
        //            dgvPLBDetails.Rows[i].ReadOnly = true;
        //        }
        //        else
        //        {
        //            dgvPLBDetails.Rows[i].ReadOnly = false;
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

        //    if (dgvPLBDetails.RowCount - 1 <= 0)
        //    {
        //        dgvPLBDetails.ColumnCount = 21;
        //        dgvPLBDetails.Columns[0].Name = "No";
        //        dgvPLBDetails.Columns[1].Name = "FullItemID";
        //        dgvPLBDetails.Columns[2].Name = "ItemName";
        //        dgvPLBDetails.Columns[3].Name = "Qty";
        //        dgvPLBDetails.Columns[4].Name = "Unit";
        //        dgvPLBDetails.Columns[5].Name = "Base";
        //        dgvPLBDetails.Columns[6].Name = "Vendor";
        //        dgvPLBDetails.Columns[7].Name = "DeliveryMethod";
        //        dgvPLBDetails.Columns[8].Name = "SalesSO";
        //        dgvPLBDetails.Columns[9].Name = "ExpectedDateFrom";
        //        dgvPLBDetails.Columns[10].Name = "ExpectedDateTo"; ;
        //        dgvPLBDetails.Columns[11].Name = "Deskripsi";
        //        dgvPLBDetails.Columns[12].Name = "GroupId";
        //        dgvPLBDetails.Columns[13].Name = "SubGroup1Id";
        //        dgvPLBDetails.Columns[14].Name = "SubGroup2Id";
        //        dgvPLBDetails.Columns[15].Name = "ItemId";
        //        dgvPLBDetails.Columns[16].Name = "GelombangId";
        //        dgvPLBDetails.Columns[17].Name = "BracketId";
        //        dgvPLBDetails.Columns[18].Name = "Price";
        //        dgvPLBDetails.Columns[19].Name = "SeqNoGroup";
        //        dgvPLBDetails.Columns[20].Name = "BracketDesc";
        //    }

        //    for (int i = 0; i < FullItemId.Count; i++)
        //    {

        //        this.dgvPLBDetails.Rows.Add((dgvPLBDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], "0", "", Base[i], VendId[i], "", "", "", "", "", GroupId[i], SubGroup1Id[i], SubGroup2Id[i], ItemId[i], GelombangId[i], BracketId[i], Price[i], SeqNoGroup, BracketDesc[i]);

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
        //            dgvPLBDetails.Rows[(dgvPLBDetails.Rows.Count - 1)].Cells[4] = combo;
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
        //            dgvPLBDetails.Rows[(dgvPLBDetails.Rows.Count - 1)].DefaultCellStyle.BackColor = Color.LightGray;
        //            dgvPLBDetails.Rows[(dgvPLBDetails.Rows.Count - 1)].ReadOnly = true;
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


        //    dgvPLBDetails.Columns["GroupId"].Visible = false;
        //    dgvPLBDetails.Columns["SubGroup1Id"].Visible = false;
        //    dgvPLBDetails.Columns["SubGroup2Id"].Visible = false;
        //    dgvPLBDetails.Columns["ItemId"].Visible = false;
        //    dgvPLBDetails.Columns["GelombangId"].Visible = false;
        //    dgvPLBDetails.Columns["BracketId"].Visible = false;
        //    dgvPLBDetails.Columns["Price"].Visible = false;
        //    dgvPLBDetails.Columns["SeqNoGroup"].Visible = false;


        //    dgvPLBDetails.ReadOnly = false;
        //    dgvPLBDetails.Columns["No"].ReadOnly = true;
        //    dgvPLBDetails.Columns["FullItemID"].ReadOnly = true;
        //    dgvPLBDetails.Columns["ItemName"].ReadOnly = true;
        //    dgvPLBDetails.Columns["SalesSO"].ReadOnly = true;
        //    dgvPLBDetails.Columns["Base"].ReadOnly = true;
        //    dgvPLBDetails.Columns["BracketDesc"].Visible = true;
        //    dgvPLBDetails.Columns["BracketDesc"].ReadOnly = true;

        //    //dgvPrDetails.Columns["D1"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D2"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D3"].ReadOnly = true;
        //    //dgvPrDetails.Columns["D4"].ReadOnly = true;
        //    dgvPLBDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["SalesSO"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["ExpectedDateFrom"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["ExpectedDateTo"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["Vendor"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["Deskripsi"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    dgvPLBDetails.Columns["DeliveryMethod"].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    //dgvPrDetails.Columns["D1"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D2"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D3"].SortMode = DataGridViewColumnSortMode.NotSortable;
        //    //dgvPrDetails.Columns["D4"].SortMode = DataGridViewColumnSortMode.NotSortable;

        //    dgvPLBDetails.AutoResizeColumns();

        //    //InvStockDetail = InvStockId;
        //}

        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvPLBDetails.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvPLBDetails.Rows[i].Cells["SeqNoGroup"].Value) == j)
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
                PriceListBeliAktif PLBA = new PriceListBeliAktif();
                PLBA.SetParent(this);
                PLBA.SetHeader(FullItemId[i]);

                Conn = ConnectionString.GetConnection();
                Query = "SELECT ROW_NUMBER() OVER (ORDER BY FullItemID asc) No,* FROM (SELECT d.PriceListNo, d.DeliveryMethod, d.GroupID AS GroupId, d.SubGroupID AS SubGroup1Id, d.SubGroup2ID AS SubGroup2Id, d.ItemID AS ItemId, d.FullItemID, d.ItemName, ";
	            Query += "d.Price0D, d.Price2D, d.Price3D, d.Price7D, d.Price14D, d.Price21 AS Price21D, d.Price30D, d.Price40D, ";
	            Query += "d.Price45D, d.Price60D, d.Price75D, d.Price90D, d.Price120D, d.Price150D, d.Price180D, d.Tolerance FROM PurchPriceListDtl d ";
                Query += "INNER JOIN PurchPriceListH h ON d.PriceListNo = h.PriceListNo ";
                Query += "WHERE h.ValidTo >= GETDATE() AND d.FullItemID = '" + FullItemId [i]+ "') a ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                PLBA.dgvPriceListBeliAktif.Rows.Clear();

                while (Dr.Read())
                {
                    PLBA.dgvPriceListBeliAktif.Rows.Add(Convert.ToString(Dr["No"]), Convert.ToString(Dr["FullItemID"]), 
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

                if (PLBA.dgvPriceListBeliAktif.RowCount == 0)
                {
                    this.dgvPLBDetails.Rows.Add((dgvPLBDetails.RowCount + 1).ToString(), FullItemId[i], ItemName[i], "", (dgvPLBDetails.RowCount + 1).ToString(), Price0D, Price2D, Price3D, Price7D, Price14D, Price21D, Price30D, Price40D, Price45D, Price60D, Price75D, Price90D, Price120D, Price150D, Price180D, Tolerance, GroupId[i], SubGroup1Id[i], SubGroup2Id[i], ItemId[i]);
                }
                else
                {
                    PLBA.ShowDialog();
                }
            }  
        }

        public void SetGridDetail(string PriceListNo, string FullItemID)
        {
            if (dgvPLBDetails.RowCount - 1 <= 0)
            {
                dgvPLBDetails.ColumnCount = 25;
                dgvPLBDetails.Columns[0].Name = "No";
                dgvPLBDetails.Columns[1].Name = "FullItemID";
                dgvPLBDetails.Columns[2].Name = "ItemName";
                dgvPLBDetails.Columns[3].Name = "DeliveryMethod";
                dgvPLBDetails.Columns[4].Name = "SeqNoGroup";
                dgvPLBDetails.Columns[5].Name = "Price0D";
                dgvPLBDetails.Columns[6].Name = "Price2D";
                dgvPLBDetails.Columns[7].Name = "Price3D";
                dgvPLBDetails.Columns[8].Name = "Price7D";
                dgvPLBDetails.Columns[9].Name = "Price14D";
                dgvPLBDetails.Columns[10].Name = "Price21D";
                dgvPLBDetails.Columns[11].Name = "Price30D";
                dgvPLBDetails.Columns[12].Name = "Price40D";
                dgvPLBDetails.Columns[13].Name = "Price45D";
                dgvPLBDetails.Columns[14].Name = "Price60D";
                dgvPLBDetails.Columns[15].Name = "Price75D";
                dgvPLBDetails.Columns[16].Name = "Price90D";
                dgvPLBDetails.Columns[17].Name = "Price120D";
                dgvPLBDetails.Columns[18].Name = "Price150D";
                dgvPLBDetails.Columns[19].Name = "Price180D";
                dgvPLBDetails.Columns[20].Name = "Tolerance";
                dgvPLBDetails.Columns[21].Name = "GroupId";
                dgvPLBDetails.Columns[22].Name = "SubGroup1Id";
                dgvPLBDetails.Columns[23].Name = "SubGroup2Id";
                dgvPLBDetails.Columns[24].Name = "ItemId";

                dgvPLBDetails.Columns[20].HeaderText = "Tolerance(%)";
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

            dgvPLBDetails.ReadOnly = false;
            dgvPLBDetails.Columns["No"].ReadOnly = true;
            dgvPLBDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLBDetails.Columns["ItemName"].ReadOnly = true;
       

            dgvPLBDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;      
            dgvPLBDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPLBDetails.Columns["GroupId"].Visible = false;
            dgvPLBDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPLBDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPLBDetails.Columns["ItemId"].Visible = false;
            dgvPLBDetails.Columns["SeqNoGroup"].Visible = false;            
            dgvPLBDetails.AutoResizeColumns();

            int SeqNo = this.dgvPLBDetails.RowCount + 1;
            Conn = ConnectionString.GetConnection();
            string Query = "";
            Query = "SELECT d.DeliveryMethod, d.GroupID AS GroupId, d.SubGroupID AS SubGroup1Id, d.SubGroup2ID AS SubGroup2Id, d.ItemID AS ItemId, d.FullItemID, d.ItemName, ";
            Query += "d.Price0D, d.Price2D, d.Price3D, d.Price7D, d.Price14D, d.Price21 AS Price21D, d.Price30D, d.Price40D, ";
            Query += "d.Price45D, d.Price60D, d.Price75D, d.Price90D, d.Price120D, d.Price150D, d.Price180D, d.Tolerance FROM PurchPriceListDtl d ";
            Query += "WHERE d.PriceListNo = '" + PriceListNo + "' AND FullItemID = '"+FullItemID+"'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            if (Dr.Read())
            {
                this.dgvPLBDetails.Rows.Add(SeqNo, Convert.ToString(Dr["FullItemID"]),
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
                Query = "SELECT GroupId, SubGroup1Id, SubGroup2Id, ItemId, ItemDeskripsi AS ItemName FROM InventTable WHERE FullItemId = '"+FullItemID+"'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.Read())
                {
                    this.dgvPLBDetails.Rows.Add(SeqNo, FullItemID, Convert.ToString(Dr["ItemName"]), "", SeqNo, Price0D, Price2D, Price3D, Price7D, Price14D, Price21D, Price30D, Price40D, Price45D, Price60D, Price75D, Price90D, Price120D, Price150D, Price180D, Tolerance, Convert.ToString(Dr["GroupId"]), Convert.ToString(Dr["SubGroup1Id"]), Convert.ToString(Dr["SubGroup2Id"]), Convert.ToString(Dr["ItemId"]));
                }
            }
            Dr.Close();
            Conn.Close(); 
        }

        public void AddDataGridVendor(List<string> VendID, List<string> VendName)
        {
            if (dgvVendorReference.RowCount - 1 <= 0)
            {
                dgvVendorReference.ColumnCount = 3;
                dgvVendorReference.Columns[0].Name = "No";
                dgvVendorReference.Columns[1].Name = "VendID";
                dgvVendorReference.Columns[2].Name = "VendName";
            }

            for (int i = 0; i < VendID.Count; i++)
            {
                this.dgvVendorReference.Rows.Add((dgvVendorReference.RowCount + 1).ToString(), VendID[i], VendName[i]);               
            }

            dgvVendorReference.ReadOnly = false;
            dgvVendorReference.Columns["No"].ReadOnly = true;
            dgvVendorReference.Columns["VendID"].ReadOnly = true;
            dgvVendorReference.Columns["VendName"].ReadOnly = true;

            dgvVendorReference.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvVendorReference.AutoResizeColumns();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            
            if (dgvPLBDetails.RowCount > 0)
            {
                Index = dgvPLBDetails.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvPLBDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvPLBDetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvPLBDetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string NumberGroupSeq = dgvPLBDetails.Rows[dgvPLBDetails.CurrentCell.RowIndex].Cells["SeqNoGroup"].Value.ToString();
                    dgvPLBDetails.Rows.RemoveAt(Index);                 
                    SortNoDataGrid();
                }                
            }
            else {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvPLBDetails.RowCount; i++)
            {
                dgvPLBDetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void SortNoDataGridVendor()
        {
            for (int i = 0; i < dgvVendorReference.RowCount; i++)
            {
                dgvVendorReference.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void HeaderPLB2_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void HeaderPLB2_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < ListDetailPLB.Count(); i++)
            {
                ListDetailPLB[i].Close();
            }

            for (int j = 0; j < ListVendorReference.Count(); j++)
            {
                ListVendorReference[j].Close();
            }
            
           // Purchase.PriceListBeli.InqueryPLB f = new Purchase.PriceListBeli.InqueryPLB();
           // f.RefreshGrid();
        }

        private void dgvPLBDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "DeliveryMethod")
            {
                if (!char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }

            //hendry price3D
            if (dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price0D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price2D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price3D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price7D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price14D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price21D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price30D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price40D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price45D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price60D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price75D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price90D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price120D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price150D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Price180D" || dgvPLBDetails.Columns[dgvPLBDetails.CurrentCell.ColumnIndex].Name == "Tolerance")
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

        private void dgvPLBDetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (Mode != "BeforeEdit")
                {

                    if (dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                    {
                        DeliveryMethod.Location = dgvPLBDetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                        DeliveryMethod.Visible = true;
                        string tmpFullItemId = dgvPLBDetails.Rows[dgvPLBDetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                        string tmpDeliveryMethod = "";
                        Conn = ConnectionString.GetConnection();
                        for (int i = 0; i < dgvPLBDetails.RowCount; i++)
                        {
                            if (dgvPLBDetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                            {
                                if (dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value != null)
                                {
                                    if (tmpDeliveryMethod == "")
                                    {
                                        tmpDeliveryMethod = "'" + dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
                                    }
                                    else
                                    {
                                        tmpDeliveryMethod += ",'" + dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "'";
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

        private void dgvPLBDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Mode != "BeforeEdit")
            {



                if (dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "DeliveryMethod")
                {
                    DeliveryMethod.Visible = false;
                }

                //if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "Qty")
                //{
                //    dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value = Convert.ToDecimal(dgvPrDetails.Rows[dgvPrDetails.CurrentCell.RowIndex].Cells["Qty"].Value).ToString("N2");
                //}

              //  dtp.Visible = false;
                dgvPLBDetails.AutoResizeColumns();

                if (dgvPLBDetails.RowCount > 0)
                {
                    if (dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Tolerance")
                    {
                        if (Convert.ToDecimal(dgvPLBDetails.Rows[e.RowIndex].Cells["Tolerance"].Value.ToString() != "" ? dgvPLBDetails.Rows[e.RowIndex].Cells["Tolerance"].Value.ToString() : "0") > 100)
                        {
                            dgvPLBDetails.CurrentCell.Value = "100";
                            MessageBox.Show("Tolerance tidak boleh lebih dari 100");
                            return;
                        }
                    }
                } 

            }
        }

        private void dgvPLBDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPLBDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPLBDetails_KeyPress);
            }
        }

        private void dgvPLBDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price0D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price2D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price3D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price7D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price14D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price21D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price30D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price40D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price45D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price60D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price75D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price90D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price120D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price150D" || dgvPLBDetails.Columns[e.ColumnIndex].Name.ToString() == "Price180D")
                {
                    string FullItemID = dgvPLBDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                    PopUp.PriceListBeli.PriceListBeli tmpSearch = new PopUp.PriceListBeli.PriceListBeli();
                    tmpSearch.FullItemID = FullItemID;
                    tmpSearch.ShowDialog();
                    
                }
            }
        }

        public void ModeNew()
        {
            PLBNumber = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            dtPlbDate.Enabled = false;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();
            DateTime CreatedDate = DateTime.Now;

            if (dgvPLBDetails.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }
            else
            {
                for (int i = 0; i <= dgvPLBDetails.RowCount - 1; i++)
                {
                    string Price0D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price0D"].Value);
                    string Price2D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price2D"].Value);
                    string Price3D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price3D"].Value);
                    string Price7D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price7D"].Value);
                    string Price14D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price14D"].Value);
                    string Price21D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price21D"].Value);
                    string Price30D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price30D"].Value);
                    string Price40D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price40D"].Value);
                    string Price45D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price45D"].Value);
                    string Price60D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price60D"].Value);
                    string Price75D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price75D"].Value);
                    string Price90D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price90D"].Value);
                    string Price120D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price120D"].Value);
                    string Price150D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price150D"].Value);
                    string Price180D = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Price180D"].Value);
                    string Tolerance = Convert.ToString(dgvPLBDetails.Rows[i].Cells["Tolerance"].Value);


                    if ((dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) == "")
                    {
                    
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", DeliveryMethod tidak boleh kosong.");
                            return;
                     }
                    else if (Convert.ToDecimal((Price0D == "" ? "0.000" : Price0D)) <= 0)
                    {
                        
                        MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price0D tidak boleh lebih kecil atau sama dengan 0");
                        return; 
                    }
                    else if (Convert.ToDecimal((Price2D == "" ? "0.000" : Price2D)) <= 0)
                    {
                     
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price2D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                       
                    }
                    else if (Convert.ToDecimal((Price3D == "" ? "0.000" : Price3D)) <= 0)
                    {
                     
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price3D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price7D == "" ? "0.000" : Price7D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price7D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                     }
                    else if (Convert.ToDecimal((Price14D == "" ? "0.000" : Price14D)) <= 0)
                    {
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price14D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price21D == "" ? "0.000" : Price21D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price21D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price30D == "" ? "0.000" : Price30D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price30D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price40D == "" ? "0.000" : Price40D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price40D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price45D == "" ? "0.000" : Price45D)) <= 0)
                    {
                       
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price45D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price60D == "" ? "0.000" : Price60D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price60D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price75D == "" ? "0.000" : Price75D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price75D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price90D == "" ? "0.000" : Price90D)) <= 0)
                    {
                      
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price90D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price120D == "" ? "0.000" : Price120D)) <= 0)
                    {
                     
                            MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price120D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price150D == "" ? "0.000" : Price150D)) <= 0)
                    {
                          MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price150D tidak boleh lebih kecil atau sama dengan 0");
                            return;
                        
                    }
                    else if (Convert.ToDecimal((Price180D == "" ? "0.000" : Price180D)) <= 0)
                    {

                        MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Price180D tidak boleh lebih kecil atau sama dengan 0");
                        return;

                    }
                    else if (Convert.ToDecimal((Tolerance == "" ? "0.000" : Tolerance)) <= 0)
                    {

                        MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Tolerance tidak boleh lebih kecil atau sama dengan 0");
                        return;

                    }
                    else if (Convert.ToDecimal(Tolerance) > 100)
                    {
                        MessageBox.Show("Item No = " + dgvPLBDetails.Rows[i].Cells["No"].Value + ", Tolerance tidak boleh lebih dari 100");
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
                    //hendry
                else if (Convert.ToString(cmbVendorReference.SelectedValue) == "")
                {                    
                        MessageBox.Show("Vendor Reference Type harus dipilih");
                        return;                    
                }
                    //hendry end
                else if (Convert.ToString(cmbVendorReference.SelectedValue) == "2" || Convert.ToString(cmbVendorReference.SelectedValue) == "3")
                {
                    if (dgvVendorReference.RowCount == 0)
                    {
                        MessageBox.Show("Vendor Reference tidak boleh kosong");
                        return;
                    }
                }
            }

            try
            {
                if (Mode == "New" || txtPLBNumber.Text == "")
                {
                    //Old Code=======================================================================================
                    //Query = "Insert into PurchPriceListH (PriceListNo,ValidFrom,ValidTo, Criteria, Notes,StatusCode,CreatedDate,CreatedBy) OUTPUT INSERTED.PriceListNo values ";
                    //Query += "((Select 'PLB-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(PriceListNo) is null then '1' else substring(Max(PriceListNo),11,4)+1 end), 5) ";
                    //Query += "from [PurchPriceListH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                    //Query += "'" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    //Query += "'"+Convert.ToString(cmbVendorReference.SelectedValue) +"', '"+txtNotes.Text+"', '01',getdate(),'" + ControlMgr.UserId + "');";
                    //Cmd = new SqlCommand(Query, Conn, Trans);

                    //string PLBNumber = Cmd.ExecuteScalar().ToString();
                    //End Old Code=====================================================================================


                    //begin============================================================================================
                    //updated by : joshua
                    //updated date : 13 Feb 2018
                    //description : change generate sequence number, get from global function and update counter 
                    string Jenis = "PLB", Kode = "PLB";
                    string PLBNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                    
                    Query = "Insert into PurchPriceListH (PriceListNo,ValidFrom,ValidTo, Criteria, Notes,StatusCode,CreatedDate,CreatedBy) values ";
                    Query += "('" + PLBNumber + "',";
                    Query += "'" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    Query += "'" + Convert.ToString(cmbVendorReference.SelectedValue) + "', '" + txtNotes.Text + "', '01',getdate(),'" + ControlMgr.UserId + "');";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //update counter
                    //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                    //end update counter
                    //end=============================================================================================
                    
                    Query = "";
                    for (int i = 0; i <= dgvPLBDetails.RowCount - 1; i++)
                    {
                        Query += "Insert PurchPriceListDtl (PriceListNo,SeqNo,FullItemID,ItemName,DeliveryMethod,GroupID,SubGroupID,SubGroup2ID,ItemID,Price0D,Price2D,Price3D,Price7D,Price14D,Price21,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,Tolerance,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PLBNumber + "',";
                        Query += (dgvPLBDetails.Rows[i].Cells["No"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["No"].Value.ToString()) + ",'";
                        Query += (dgvPLBDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "',";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price0D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price0D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price2D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price2D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price3D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price3D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price7D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price7D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price14D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price14D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price21D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price21D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price30D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price30D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price40D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price40D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price45D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price45D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price60D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price60D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price75D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price75D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price90D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price90D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price120D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price120D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price150D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price150D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price180D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price180D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Tolerance"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Tolerance"].Value.ToString()) + ",";
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

                    for (int i = 0; i <= dgvVendorReference.RowCount - 1; i++)
                    {
                        Query = "Insert PurchPriceList_VendorList (PriceListNo,VendID,Name,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PLBNumber + "',";
                        Query += "'" + (dgvVendorReference.Rows[i].Cells["VendID"].Value == null ? "" : dgvVendorReference.Rows[i].Cells["VendID"].Value.ToString()) + "',";
                        Query += "'" + (dgvVendorReference.Rows[i].Cells["VendName"].Value == null ? "" : dgvVendorReference.Rows[i].Cells["VendName"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'"+ControlMgr.UserId+"')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    String StatusDesc = null;
                    Query = "SELECT Deskripsi FROM TransStatusTable ";
                    Query += "WHERE StatusCode='01' AND TransCode = 'PurchPriceList' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        StatusDesc = Convert.ToString(Dr["Deskripsi"]);
                    }
                    Dr.Close();

                   // string[] formats = { "dd/MM/yyyy", "dd-MMM-yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "M/d/yyyy", "dd MMM yyyy" };


                    Query = "Insert into PurchPriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate) ";
                    Query += "VALUES(getdate(), '" + PLBNumber + "', '01', '" + StatusDesc + "', '" + StatusDesc + " By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate()) ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();


                    Trans.Commit();
                    MessageBox.Show("Data PLBNumber : " + PLBNumber + " berhasil ditambahkan.");
                    txtPLBNumber.Text = PLBNumber;
                    MainMenu f = new MainMenu();
                    //f.refreshTaskList();
                }
                else
                {
                    Query = "Update PurchPriceListH set ";
                    Query += "ValidFrom='" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    Query += "ValidTo='" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                    Query += "Criteria='" + Convert.ToString(cmbVendorReference.SelectedValue) + "',";
                    Query += "Notes='" + txtNotes.Text + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where PriceListNo='" + txtPLBNumber.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Query = "Delete from PurchPriceListDtl where PriceListNo='" + txtPLBNumber.Text.Trim() + "';";
                    for (int i = 0; i <= dgvPLBDetails.RowCount - 1; i++)
                    {
                        Query += "Insert PurchPriceListDtl (PriceListNo,SeqNo,FullItemID,ItemName,DeliveryMethod,GroupID,SubGroupID,SubGroup2ID,ItemID,Price0D,Price2D,Price3D,Price7D,Price14D,Price21,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,Tolerance,CreatedDate,CreatedBy) Values ";
                        Query += "('" + txtPLBNumber.Text.Trim() + "',";
                        Query += (dgvPLBDetails.Rows[i].Cells["No"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["No"].Value.ToString()) + ",'";
                        Query += (dgvPLBDetails.Rows[i].Cells["FullItemID"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["GroupId"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["SubGroup1Id"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["SubGroup2Id"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                        Query += (dgvPLBDetails.Rows[i].Cells["ItemId"].Value == null ? "" : dgvPLBDetails.Rows[i].Cells["ItemId"].Value.ToString()) + "',";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price0D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price0D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price2D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price2D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price3D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price3D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price7D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price7D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price14D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price14D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price21D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price21D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price30D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price30D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price40D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price40D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price45D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price45D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price60D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price60D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price75D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price75D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price90D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price90D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price120D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price120D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price150D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price150D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Price180D"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Price180D"].Value.ToString()) + ",";
                        Query += (dgvPLBDetails.Rows[i].Cells["Tolerance"].Value == null ? "0.0000" : dgvPLBDetails.Rows[i].Cells["Tolerance"].Value.ToString()) + ",";
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

                    Query = "";
                    Query = "Delete from PurchPriceList_VendorList where PriceListNo='" + txtPLBNumber.Text.Trim() + "';";
                    for (int i = 0; i <= dgvVendorReference.RowCount - 1; i++)
                    {
                        Query += "Insert PurchPriceList_VendorList (PriceListNo,VendID,Name,CreatedDate,CreatedBy) Values ";
                        Query += "('" + PLBNumber + "',";
                        Query += "'" + (dgvVendorReference.Rows[i].Cells["VendID"].Value == null ? "" : dgvVendorReference.Rows[i].Cells["VendID"].Value.ToString()) + "',";
                        Query += "'" + (dgvVendorReference.Rows[i].Cells["VendName"].Value == null ? "" : dgvVendorReference.Rows[i].Cells["VendName"].Value.ToString()) + "',";
                        Query += "getdate(),";
                        Query += "'" + ControlMgr.UserId + "')";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }

                    Trans.Commit();
                    MessageBox.Show("Data PriceListNo : " + txtPLBNumber.Text + " berhasil diupdate.");

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
            if (PLBNumber == "")
            {
                PLBNumber = txtPLBNumber.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            Query = "Select a.[PriceListNo],a.[ValidFrom],a.[ValidTo], a.[StatusCode], a.[CreatedDate], a.Notes, a.Criteria, b.Deskripsi From [PurchPriceListH] a ";
            Query += "left join TransStatusTable b on b.StatusCode = a.StatusCode ";
            Query += "Where PriceListNo = '" + PLBNumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPLBNumber.Text = Dr["PriceListNo"].ToString();
                dtPlbDate.Text = Dr["CreatedDate"].ToString();
                dtFrom.Text = Dr["ValidFrom"].ToString();
                dtTo.Text = Dr["ValidTo"].ToString();
                txtNotes.Text = Dr["Notes"].ToString();
                cmbVendorReference.SelectedValue = Dr["Criteria"].ToString();
            }
            Dr.Close();

            dgvPLBDetails.Rows.Clear();
            if (dgvPLBDetails.RowCount - 1 <= 0)
            {
                dgvPLBDetails.ColumnCount = 25;
                dgvPLBDetails.Columns[0].Name = "No";
                dgvPLBDetails.Columns[1].Name = "FullItemID";
                dgvPLBDetails.Columns[2].Name = "ItemName";
                dgvPLBDetails.Columns[3].Name = "DeliveryMethod";
                dgvPLBDetails.Columns[4].Name = "SeqNoGroup";
               // dgvPLBDetails.Columns[5].Name = "Base";
                dgvPLBDetails.Columns[5].Name = "Price0D";
                dgvPLBDetails.Columns[6].Name = "Price2D";
                dgvPLBDetails.Columns[7].Name = "Price3D";
                dgvPLBDetails.Columns[8].Name = "Price7D";
                dgvPLBDetails.Columns[9].Name = "Price14D";
                dgvPLBDetails.Columns[10].Name = "Price21D";
                dgvPLBDetails.Columns[11].Name = "Price30D";
                dgvPLBDetails.Columns[12].Name = "Price40D";
                dgvPLBDetails.Columns[13].Name = "Price45D";
                dgvPLBDetails.Columns[14].Name = "Price60D";
                dgvPLBDetails.Columns[15].Name = "Price75D";
                dgvPLBDetails.Columns[16].Name = "Price90D";
                dgvPLBDetails.Columns[17].Name = "Price120D";
                dgvPLBDetails.Columns[18].Name = "Price150D";
                dgvPLBDetails.Columns[19].Name = "Price180D";
                dgvPLBDetails.Columns[20].Name = "Tolerance";
               // dgvPLBDetails.Columns[22].Name = "BracketId";
                dgvPLBDetails.Columns[21].Name = "GroupId";
                dgvPLBDetails.Columns[22].Name = "SubGroup1Id";
                dgvPLBDetails.Columns[23].Name = "SubGroup2Id";
                dgvPLBDetails.Columns[24].Name = "ItemId";

                dgvPLBDetails.Columns[20].HeaderText = "Tolerance(%)";

            }

            Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[DeliveryMethod],Price0D,Price2D,Price3D,Price7D,Price14D,Price21 AS Price21D,Price30D,Price40D,Price45D,Price60D,Price75D,Price90D,Price120D,Price150D,Price180D,Tolerance,GroupId,SubGroupID,SubGroup2ID,ItemID From [PurchPriceListDtl] Where PriceListNo = '" + PLBNumber + "' order by SeqNo asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {

                this.dgvPLBDetails.Rows.Add(i+1, Dr[1], Dr[2], Dr[3], Dr[0], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23]);
                i++;
            }
            Dr.Close();

            dgvPLBDetails.ReadOnly = false;
            dgvPLBDetails.Columns["No"].ReadOnly = true;
            dgvPLBDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLBDetails.Columns["ItemName"].ReadOnly = true;
           // dgvPLBDetails.Columns["Base"].ReadOnly = true;

            dgvPLBDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
           // dgvPLBDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
           // dgvPLBDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["GroupId"].Visible = false;
            dgvPLBDetails.Columns["SubGroup1Id"].Visible = false;
            dgvPLBDetails.Columns["SubGroup2Id"].Visible = false;
            dgvPLBDetails.Columns["ItemId"].Visible = false;
         //   dgvPLBDetails.Columns["BracketId"].Visible = false;
            dgvPLBDetails.Columns["SeqNoGroup"].Visible = false;

           // dgvPLBDetails.Columns["Base"].Visible = false;

            dgvPLBDetails.AutoResizeColumns();



            //Vendor
            this.dgvVendorReference.Rows.Clear();
            if (dgvVendorReference.RowCount - 1 <= 0)
            {
                dgvVendorReference.ColumnCount = 3;
                dgvVendorReference.Columns[0].Name = "No";
                dgvVendorReference.Columns[1].Name = "VendID";
                dgvVendorReference.Columns[2].Name = "VendName";                
            }

            Query = "Select VendID, Name AS VendName From [PurchPriceList_VendorList] Where PriceListNo = '" + PLBNumber + "' order by VendID asc";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int j = 0;
            while (Dr.Read())
            {

                dgvVendorReference.Rows.Add(j + 1, Dr[0], Dr[1]);
                j++;
            }
            Dr.Close();

            dgvVendorReference.ReadOnly = false;
            dgvVendorReference.Columns["No"].ReadOnly = true;
            dgvVendorReference.Columns["VendID"].ReadOnly = true;
            dgvVendorReference.Columns["VendName"].ReadOnly = true;

            dgvVendorReference.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvVendorReference.AutoResizeColumns();


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

            btnAddVendor.Enabled = false;
            btnDeleteVendor.Enabled = false;
            cmbVendorReference.Enabled = false;
            txtNotes.Enabled = false;
            //dtPLJDate.Enabled = false;
           // cmbPrType.Enabled = false;
           // txtPrStatus.Enabled = false;

            dgvPLBDetails.ReadOnly = true;
            dgvVendorReference.ReadOnly = true;
            BeforeEditColor();
            dgvPLBDetails.DefaultCellStyle.BackColor = Color.LightGray;
            dgvVendorReference.DefaultCellStyle.BackColor = Color.LightGray;
        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvPLBDetails.RowCount; i++)
            {
                dgvPLBDetails.Rows[i].Cells["DeliveryMethod"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price0D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price2D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price3D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price7D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price14D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price21D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price30D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price40D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price45D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price60D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price75D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price90D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price120D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price150D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Price180D"].Style.BackColor = Color.LightGray;
                dgvPLBDetails.Rows[i].Cells["Tolerance"].Style.BackColor = Color.LightGray;
            }

            for (int i = 0; i < dgvVendorReference.RowCount; i++)
            {
                dgvVendorReference.Rows[i].Cells["VendID"].Style.BackColor = Color.LightGray;
                dgvVendorReference.Rows[i].Cells["VendName"].Style.BackColor = Color.LightGray;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                if (txtPLBNumber.Text != "")
                {
                    Query = "Select StatusCode from [dbo].[PurchPriceListH] where [PriceListNo]='" + txtPLBNumber.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Check = Cmd.ExecuteScalar().ToString();
                    if (Check == "02" || Check == "03")
                    {
                        MessageBox.Show("PriceList No = " + txtPLBNumber.Text + ".\n" + "Tidak bisa diedit karena sudah diproses.");
                        Conn.Close();
                        return;
                    }
                }

                Mode = "Edit";

                btnSave.Visible = true;
                btnExit.Visible = false;
                btnEdit.Visible = false;
                btnCancel.Visible = true;

                dtPlbDate.Enabled = false;

                btnAddVendor.Enabled = true;
                btnDeleteVendor.Enabled = true;
                cmbVendorReference.Enabled = true;
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
            dtPlbDate.Enabled = false;
            dtFrom.Enabled = true;
            dtTo.Enabled = true;

            dgvPLBDetails.ReadOnly = false;
            dgvPLBDetails.Columns["No"].ReadOnly = true;
            dgvPLBDetails.Columns["FullItemID"].ReadOnly = true;
            dgvPLBDetails.Columns["ItemName"].ReadOnly = true;           
            //dgvPLBDetails.Columns["Base"].ReadOnly = true;

            dgvPLBDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;          
            //dgvPLBDetails.Columns["Base"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["GroupId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup1Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SubGroup2Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["ItemId"].SortMode = DataGridViewColumnSortMode.NotSortable;
          //  dgvPLBDetails.Columns["BracketId"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["SeqNoGroup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price0D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price2D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price3D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price7D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price14D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price21D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price30D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price40D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price45D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price60D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price75D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price90D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price120D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price150D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Price180D"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvPLBDetails.Columns["Tolerance"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvPLBDetails.AutoResizeColumns();
            EditColor();
            dgvPLBDetails.DefaultCellStyle.BackColor = Color.White;


            dgvVendorReference.ReadOnly = false;
            dgvVendorReference.Columns["No"].ReadOnly = true;
            dgvVendorReference.Columns["VendID"].ReadOnly = true;
            dgvVendorReference.Columns["VendName"].ReadOnly = true;
            dgvVendorReference.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.Columns["VendName"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvVendorReference.AutoResizeColumns();
            dgvVendorReference.DefaultCellStyle.BackColor = Color.White;
            EditColorVendor();
        }

        private void dgvPLBDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == dgvPLBDetails.Columns["Price0D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price2D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price3D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price7D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price14D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price21D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price30D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price40D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price45D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price60D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price75D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price90D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price120D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price150D"].Index || e.ColumnIndex == dgvPLBDetails.Columns["Price180D"].Index) && e.Value != null)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }

            if (e.ColumnIndex == dgvPLBDetails.Columns["Tolerance"].Index)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            dtPlbDate.Enabled = false;

            ModeBeforeEdit();
            GetDataHeader();
        }

        private void VendorReference_SelectedIndexChange(object sender, EventArgs e)
        {
            if (Convert.ToString(cmbVendorReference.SelectedValue) == "1" || Convert.ToString(cmbVendorReference.SelectedValue) == "")
            {
                btnAddVendor.Enabled = false;

                //hendry
                //for (int i = 0; i < dgvVendorReference.RowCount; i++)
                //{
                //    dgvVendorReference.Rows.Clear();
                //    dgvVendorReference.Refresh();
                //}
                if (dgvVendorReference.Rows.Count > 0)
                {   
                    dgvVendorReference.Rows.Clear();                    
                    dgvVendorReference.Refresh();
                }
                //hendry end
            }
            else
            {
                btnAddVendor.Enabled = true;            
            }
        }

        private void btnDeleteVendor_Click(object sender, EventArgs e)
        {
            if (dgvVendorReference.RowCount > 0)
            {
                Index = dgvVendorReference.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvVendorReference.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " VendID = " + dgvVendorReference.Rows[Index].Cells["VendID"].Value.ToString() + Environment.NewLine + " VendName = " + dgvVendorReference.Rows[Index].Cells["VendName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string NumberGroupSeq = dgvVendorReference.Rows[dgvVendorReference.CurrentCell.RowIndex].Cells["No"].Value.ToString();
                    dgvVendorReference.Rows.RemoveAt(Index);
                    SortNoDataGridVendor();
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void btnAddVendor_Click(object sender, EventArgs e)
        {
           // DetailPLB DetailPLB = new DetailPLB();
           // List<DetailPLB> ListDetailPLB = new List<DetailPLB>();

            PopUp.VendorReference.VendorReference VR = new PopUp.VendorReference.VendorReference();
            List<PopUp.VendorReference.VendorReference> ListVendorReference = new List<PopUp.VendorReference.VendorReference>();
            VR.ParentRefreshGrid(this);
            VR.ParamHeader(dgvVendorReference);
            VR.ShowDialog();
            EditColorVendor();
        }           
    }
}
