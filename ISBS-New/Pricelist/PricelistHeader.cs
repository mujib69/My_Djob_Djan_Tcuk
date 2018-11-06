using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Pricelist
{
    public partial class PricelistHeader : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr, Dr2;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        private int Index;       
        private string Mode, Query, crit = null;
        private string PricelistNo = "", tmpPrType = "", PricelistType = "", Type = "";
        private string CallType = "";

        List<string> ListSubGroup2 = new List<string>();
        List<PopUp.Vendor.Vendor> ListVendor = new List<PopUp.Vendor.Vendor>();
        Dictionary<string, string> DataCheckPricelist = new Dictionary<string, string>();

        Pricelist.PricelistInquiry ParentBeforeEdit;
        Pricelist.LookupReferenceDoc ParentView;
        
        DateTimePicker dtp;
        Regex strPattern = new Regex("^[0-9.-]*$");

        //begin
        //created by : joshua
        //created date : 26 apr 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public PricelistHeader()
        {
            InitializeComponent();

        }

        private void PricelistHeader_Load(object sender, EventArgs e)
        {
            lblForm.Location = new Point(16, 11);

            if (PricelistType.ToUpper() == "JUAL")
            {
                SetCmbCustomer();
            }
            else
            {
                SetCmbVendorReference();
            }            
           
            SetCmbDeliveryMethod();           

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
                GetDataHeader();
                ModeBeforeEdit();
            }
            else if (Mode == "View")
            {
                GetDataHeader();
                ModeView();
            }

            dgvPricelistDetails.Controls.Add(dtp);
        }

        private void SetCmbCustomer()
        {
            cmbAccountlist.DataSource = null;
            cmbAccountlist.DisplayMember = "Text";
            cmbAccountlist.ValueMember = "Value";

            var items = new[] { 
                new { Text = "-select-", Value = "" }, 
                new { Text = "All Customer", Value = "1" }, 
                new { Text = "All Customer Except", Value = "2" },
                new { Text = "Specific Customer", Value = "3" }
            };

            cmbAccountlist.DataSource = items;
            cmbAccountlist.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void SetCmbVendorReference()
        {
            cmbAccountlist.DisplayMember = "Text";
            cmbAccountlist.ValueMember = "Value";

            var items = new[] { 
                new { Text = "-select-", Value = "" }, 
                new { Text = "All Vendor", Value = "1" }, 
                new { Text = "All Vendor Except", Value = "2" },
                new { Text = "Specific Vendor", Value = "3" }
            };

            cmbAccountlist.DataSource = items;
            cmbAccountlist.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void SetCmbDeliveryMethod()
        {
            cmbDeliveryMethod.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT DeliveryMethod FROM DeliveryMethod WHERE DeliveryMethod <> ''";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbDeliveryMethod.DisplayMember = "Text";
            cmbDeliveryMethod.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbDeliveryMethod.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[0] + "" });
            }
            cmbDeliveryMethod.SelectedIndex = 0;
            Conn.Close();
        }

        public void ModeView()
        {
            Mode = "ModeView";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            btnInactive.Visible = false;
            btnActive.Visible = false;
            btnApprove.Visible = false;
            btnReject.Visible = false;

            dtFrom.Enabled = false;
            dtTo.Enabled = false;
            dtPricelist.Enabled = false;

            btnAddAccountList.Enabled = false;
            btnDeleteAccountList.Enabled = false;
            btnReferenceDoc.Enabled = false;
            btnAddPricelistDetail.Enabled = false;
            btnDeletePricelistDetail.Enabled = false;
            cmbAccountlist.Enabled = false;
            cmbDeliveryMethod.Enabled = false;
            txtNotes.Enabled = false;

            dgvPricelistDetails.DefaultCellStyle.BackColor = Color.LightGray;
            EditColorAccountList();
        }

        public void SetMode(string tmpMode, string tmpPricelistNo, string tmpPricelistType)
        {
            Mode = tmpMode;
            PricelistNo = tmpPricelistNo;
            txtPricelistNo.Text = tmpPricelistNo;
            PricelistType = tmpPricelistType;
            lblForm.Text = Text = lblForm.Text + " " + tmpPricelistType;
            if(PricelistType.ToUpper() == "JUAL")
            {
                lblAccountList.Text = "Customer List";
                Type = "SALES";
            }
            else
            {
                lblAccountList.Text = "Vendor List";
                Type = "PURCHASE";
            }
        }

        public void SetParent(Pricelist.PricelistInquiry F)
        {
            ParentBeforeEdit = F;
            CallType = "BeforeEdit";
        }

        public void SetParentView(Pricelist.LookupReferenceDoc F)
        {
            ParentView = F;
            CallType = "View";
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvPricelistDetails.RowCount; i++)
            {
                dgvPricelistDetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void PricelistHeader_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void PricelistHeader_FormClosed(object sender, FormClosedEventArgs e)
        {
            var PricelistInquiry = Application.OpenForms.OfType<PricelistInquiry>().FirstOrDefault();
            if (PricelistInquiry != null)
            {
                if (CallType == "BeforeEdit")
                {
                    PricelistInquiry.Activate();
                    PricelistInquiry.RefreshGrid();
                }
            }

            //if (CallType == "BeforeEdit")
            //{
            //    ParentBeforeEdit.RefreshGrid();
            //}           
        }

        public void ModeNew()
        {
            PricelistNo = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            dtPricelist.Enabled = false;

            btnApprove.Visible = false;
            btnReject.Visible = false;
            btnInactive.Visible = false;
            btnActive.Visible = false;
        }

        private Boolean Validasi()
        {
            if (cmbDeliveryMethod.Text.ToUpper() != "LOCO" && txtHIO.Text == "")
            {
                MessageBox.Show("HIO harus diisi karena bukan LOCO");
                return false;
            }
            else if (dtFrom.Value.Date < DateTime.Now.Date)
            {
                MessageBox.Show("Valid From harus lebih besar atau sama dengan dari hari ini");
                return false;
            }
            else if (dtTo.Value.Date <= dtFrom.Value.Date)
            {
                MessageBox.Show("Valid To harus lebih besar dari Valid From");
                return false;
            }
            else if (Convert.ToString(cmbAccountlist.SelectedValue) == "")
            {
                if (PricelistType.ToUpper() == "JUAL")
                {
                    MessageBox.Show("Customer List harus diisi");
                }
                else
                {
                    MessageBox.Show("Vendor List harus diisi");
                }
                return false;
            }
            else if ((Convert.ToString(cmbAccountlist.SelectedValue) == "2" || Convert.ToString(cmbAccountlist.SelectedValue) == "3") && dgvAccountList.RowCount == 0)
            {
                if (PricelistType.ToUpper() == "JUAL")
                {
                    MessageBox.Show("Customer List tidak boleh kosong");
                }
                else
                {
                    MessageBox.Show("Vendor List tidak boleh kosong");
                }
                return false;
            }

            else if (cmbDeliveryMethod.SelectedIndex == 0)
            {
                MessageBox.Show("Delivery Method harus diisi");
                return false;
            }
            else if (dgvPricelistDetails.RowCount == 0)
            {
                MessageBox.Show("Data Pricelist Detail tidak boleh kosong");
                return false;
            }
            else return true;
        }

        private void SaveNew()
        {
            //GET DATA SUBGROUP FROM GRIDVIEW PRICELIST DETAILS
            var DistinctSubGroup2ID = dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
               .Select(x => x.Cells["SubGroup2ID"].Value.ToString())
               .Distinct()
               .ToList();
            //END GET DATA SUBGROUP FROM GRIDVIEW PRICELIST DETAILS

            //INSERT PRICELIST HEADER
            string Jenis = "", Kode = "";
            if (PricelistType.ToUpper() == "JUAL")
            {
                Jenis = "PLJ";
                Kode = "PLJ";
            }
            else
            {
                Jenis = "PLB";
                Kode = "PLB";
            }

            PricelistNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
            Query = "INSERT INTO PricelistH(PriceListNo, PricelistDate, Type, RefId, Ref2Id, ValidFrom, ValidTo, DeliveryMethod, Active, TransStatus, Notes, Criteria, CreatedDate, CreatedBy, HIO) VALUES ";
            Query += "('" + PricelistNo + "', '" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + Type + "', ";
            Query += "'" + ReferenceDoc1 + "', '" + ReferenceDoc2 + "', ";
            Query += "'" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
            Query += "'" + cmbDeliveryMethod.Text + "', 1, '01', '" + txtNotes.Text + "', '" + cmbAccountlist.SelectedValue + "', ";
            Query += "getdate(),'" + ControlMgr.UserId + "', " + Convert.ToDecimal(txtHIO.Text) + ");";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();


            //END INSERT HEADER PRICELISTH

            //INSERT PRICELIST DETAILS
            int SeqNo = 1;

            for (int i = 0; i < DistinctSubGroup2ID.Count; i++)
            {
                string SubGroup2ID = DistinctSubGroup2ID[i];
                int SeqSubGroup2ID = i;

                //GET DATA GRIDVIEW PRICELIST DETAILS 
                var DistinctPricelistDetails = from Pricelist in dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
                                               where Pricelist.Cells["SubGroup2ID"].Value.ToString() == SubGroup2ID
                                               select Pricelist;
                //END GET DATA GRIDVIEW PRICELIST DETAILS 

                //INSERT BASE PRICE
                foreach (var DataPricelist in DistinctPricelistDetails)
                {
                    string GroupID = DataPricelist.Cells["GroupID"].Value.ToString();
                    string SubGroup1ID = DataPricelist.Cells["SubGroup1ID"].Value.ToString();
                    string SubGroup2Name = DataPricelist.Cells["SubGroup2"].Value.ToString() == "" ? "" : DataPricelist.Cells["SubGroup2"].Value.ToString();
                    string Ukuran1_Value = DataPricelist.Cells["Ukuran1_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value"].Value.ToString();
                    string Ukuran1_Value_To = DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString();
                    string Ukuran2_Value = DataPricelist.Cells["Ukuran2_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value"].Value.ToString();
                    string Ukuran2_Value_To = DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString();
                    string Ukuran3_Value = DataPricelist.Cells["Ukuran3_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value"].Value.ToString();
                    string Ukuran3_Value_To = DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString();
                    string Ukuran4_Value = DataPricelist.Cells["Ukuran4_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value"].Value.ToString();
                    string Ukuran4_Value_To = DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString();
                    string Ukuran5_Value = DataPricelist.Cells["Ukuran5_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value"].Value.ToString();
                    string Ukuran5_Value_To = DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString();
                    string ItemID = DataPricelist.Cells["ItemID"].Value.ToString();
                    string FullItemID = DataPricelist.Cells["FullItemID"].Value.ToString();
                    string ItemName = DataPricelist.Cells["ItemName"].Value.ToString();
                    decimal Tolerance = Convert.ToDecimal(DataPricelist.Cells["Tolerance(%)"].Value);
                    decimal Price = Convert.ToDecimal(DataPricelist.Cells["Price[0]"].Value);
                    int SeqGroupNo = Convert.ToInt32(DataPricelist.Cells["SeqGroupNo"].Value);
                    string ManufacturerID = DataPricelist.Cells["ManufacturerID"].Value.ToString();
                    string MerekID = DataPricelist.Cells["MerekID"].Value.ToString();
                    string GolonganID = DataPricelist.Cells["GolonganID"].Value.ToString();
                    string KodeBerat = DataPricelist.Cells["KodeBerat"].Value.ToString();
                    string SpecID = DataPricelist.Cells["SpecID"].Value.ToString();
                    string Bentuk = DataPricelist.Cells["Bentuk"].Value.ToString();

                    Query = "INSERT INTO Pricelist_Dtl(PricelistNo, PricelistDate, Type, SeqGroupNo, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ";
                    Query += "Ukuran1_Value, Ukuran1_Value_To, Ukuran2_Value, Ukuran2_Value_To, Ukuran3_Value, Ukuran3_Value_To, ";
                    Query += "Ukuran4_Value, Ukuran4_Value_To, Ukuran5_Value, Ukuran5_Value_To, ItemId, FullItemId, ItemName, DeliveryMethod, ";
                    Query += "PriceType, Factor, Price, Tolerance, Ref_Config_RecId, CreatedDate, CreatedBy, ManufacturerID, MerekID, GolonganID, KodeBerat, SpecID, Bentuk) ";
                    Query += "VALUES('" + PricelistNo + "', '" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + Type + "', ";
                    Query += "" + SeqGroupNo + "," + SeqNo + ", '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', ";
                    Query += "" + Ukuran1_Value + ", " + Ukuran1_Value_To + ", " + Ukuran2_Value + ", " + Ukuran2_Value_To + ", " + Ukuran3_Value + ", " + Ukuran3_Value_To + ", ";
                    Query += "" + Ukuran4_Value + ", " + Ukuran4_Value_To + ", " + Ukuran5_Value + ", " + Ukuran5_Value_To + ", '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', '" + cmbDeliveryMethod.Text + "', ";
                    Query += "" + 0 + ", " + 0 + ", " + Price + ", " + Tolerance + ", " + 0 + ", getdate(), '" + ControlMgr.UserId + "', '" + ManufacturerID + "', '" + MerekID + "', '" + GolonganID + "', '" + KodeBerat + "', '" + SpecID + "', '" + Bentuk + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;

                    //INSERT LOG
                    //if (i == SeqSubGroup2ID)
                    //{
                    //    Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, PK4, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                    //    Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + SubGroup2ID + " " + SubGroup2Name + "', ";
                    //    Query += "'Ukuran 1 From " + Ukuran1_Value + " To " + Ukuran1_Value_To + ", Ukuran 2 From " + Ukuran2_Value + " To " + Ukuran2_Value_To + ", ";
                    //    Query += "Ukuran 3 From " + Ukuran3_Value + " To " + Ukuran3_Value_To + ", Ukuran 4 From " + Ukuran4_Value + " To " + Ukuran4_Value_To + ", ";
                    //    Query += "Ukuran 5 From " + Ukuran5_Value + " To " + Ukuran5_Value_To + "', '" + cmbDeliveryMethod.Text + "', '" + cmbAccountlist.SelectedValue + "', '01', 'Waiting for Approval', 'Created By User " + ControlMgr.UserId + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                    //    Cmd = new SqlCommand(Query, Conn, Trans);
                    //    Cmd.ExecuteNonQuery();
                    //    SeqSubGroup2ID++;
                    //}
                    //END INSERT LOG
                }
                //END INSERT BASE PRICE

                if (PricelistType.ToUpper() == "JUAL")
                { //INSERT BY PRICE LIST CONFIG
                    Query = "SELECT PriceType, Factor, RecId FROM PricelistConfig WHERE SubGroup2Id = '" + SubGroup2ID + "' ORDER BY PriceType ASC";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        int PriceType = Convert.ToInt32(Dr["PriceType"]);
                        decimal Factor = Convert.ToDecimal(Dr["Factor"]);
                        long RecId = Convert.ToInt64(Dr["RecId"]);

                        foreach (var DataPricelist in DistinctPricelistDetails)
                        {
                            string GroupID = DataPricelist.Cells["GroupID"].Value.ToString();
                            string SubGroup1ID = DataPricelist.Cells["SubGroup1ID"].Value.ToString();
                            string Ukuran1_Value = DataPricelist.Cells["Ukuran1_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value"].Value.ToString();
                            string Ukuran1_Value_To = DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString();
                            string Ukuran2_Value = DataPricelist.Cells["Ukuran2_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value"].Value.ToString();
                            string Ukuran2_Value_To = DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString();
                            string Ukuran3_Value = DataPricelist.Cells["Ukuran3_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value"].Value.ToString();
                            string Ukuran3_Value_To = DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString();
                            string Ukuran4_Value = DataPricelist.Cells["Ukuran4_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value"].Value.ToString();
                            string Ukuran4_Value_To = DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString();
                            string Ukuran5_Value = DataPricelist.Cells["Ukuran5_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value"].Value.ToString();
                            string Ukuran5_Value_To = DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString();
                            string ItemID = DataPricelist.Cells["ItemID"].Value.ToString();
                            string FullItemID = DataPricelist.Cells["FullItemID"].Value.ToString();
                            string ItemName = DataPricelist.Cells["ItemName"].Value.ToString();
                            decimal Tolerance = Convert.ToDecimal(DataPricelist.Cells["Tolerance(%)"].Value);
                            string ManufacturerID = DataPricelist.Cells["ManufacturerID"].Value.ToString();
                            string MerekID = DataPricelist.Cells["MerekID"].Value.ToString();
                            string GolonganID = DataPricelist.Cells["GolonganID"].Value.ToString();
                            string KodeBerat = DataPricelist.Cells["KodeBerat"].Value.ToString();
                            string SpecID = DataPricelist.Cells["SpecID"].Value.ToString();
                            string Bentuk = DataPricelist.Cells["Bentuk"].Value.ToString();
                            decimal Price = 0;
                            int SeqGroupNo = Convert.ToInt32(DataPricelist.Cells["SeqGroupNo"].Value);

                            if (PriceType == 2)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[2]"].Value);
                            }
                            else if (PriceType == 3)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[3]"].Value);
                            }
                            else if (PriceType == 7)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[7]"].Value);
                            }
                            else if (PriceType == 14)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[14]"].Value);
                            }
                            else if (PriceType == 21)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[21]"].Value);
                            }
                            else if (PriceType == 30)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[30]"].Value);
                            }
                            else if (PriceType == 40)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[40]"].Value);
                            }
                            else if (PriceType == 45)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[45]"].Value);
                            }
                            else if (PriceType == 60)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[60]"].Value);
                            }
                            else if (PriceType == 75)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[75]"].Value);
                            }
                            else if (PriceType == 90)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[90]"].Value);
                            }
                            else if (PriceType == 120)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[120]"].Value);
                            }
                            else if (PriceType == 150)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[150]"].Value);
                            }
                            else if (PriceType == 180)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[180]"].Value);
                            }

                            Query = "INSERT INTO Pricelist_Dtl(PricelistNo, PricelistDate, Type, SeqGroupNo, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ";
                            Query += "Ukuran1_Value, Ukuran1_Value_To, Ukuran2_Value, Ukuran2_Value_To, Ukuran3_Value, Ukuran3_Value_To, ";
                            Query += "Ukuran4_Value, Ukuran4_Value_To, Ukuran5_Value, Ukuran5_Value_To, ItemId, FullItemId, ItemName, DeliveryMethod, ";
                            Query += "PriceType, Factor, Price, Tolerance, Ref_Config_RecId, CreatedDate, CreatedBy, ManufacturerID, MerekID, GolonganID, KodeBerat, SpecID, Bentuk) ";
                            Query += "VALUES('" + PricelistNo + "', '" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + Type + "', ";
                            Query += "" + SeqGroupNo + "," + SeqNo + ", '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', ";
                            Query += "" + Ukuran1_Value + ", " + Ukuran1_Value_To + ", " + Ukuran2_Value + ", " + Ukuran2_Value_To + ", " + Ukuran3_Value + ", " + Ukuran3_Value_To + ", ";
                            Query += "" + Ukuran4_Value + ", " + Ukuran4_Value_To + ", " + Ukuran5_Value + ", " + Ukuran5_Value_To + ", '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', '" + cmbDeliveryMethod.Text + "', ";
                            Query += "" + PriceType + ", " + Factor + ", " + Price + ", " + Tolerance + ", " + RecId + ", getdate(), '" + ControlMgr.UserId + "', '" + ManufacturerID + "', '" + MerekID + "', '" + GolonganID + "', '" + KodeBerat + "', '" + SpecID + "', '" + Bentuk + "')";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                    }
                    Dr.Close();
                    //END INSERT BY PRICE LIST CONFIG                            
                }
            }
            //END INSERT PRICELIST DETAILS

            //INSERT ACCOUNT LIST (VENDOR LIST ATAU CUSTOMER LIST)
            for (int j = 0; j < dgvAccountList.RowCount; j++)
            {
                Query = "INSERT INTO PriceList_AccountList (PricelistNo, AccountId, Name, CreatedDate, CreatedBy) VALUES ";
                Query += "('" + PricelistNo + "', '" + dgvAccountList.Rows[j].Cells[1].Value.ToString() + "', '" + dgvAccountList.Rows[j].Cells[2].Value.ToString() + "', getdate(), '" + ControlMgr.UserId + "' )";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }
            //END INSERT ACCOUNT LIST (VENDOR LIST ATAU CUSTOMER LIST)   


            InsertLog(PricelistNo, "Pricelist", "01", "N", Conn, Trans, Cmd);

            Trans.Commit();
            txtPricelistNo.Text = PricelistNo;
            MessageBox.Show("Data PriceListNo : " + PricelistNo + " berhasil disimpan.");
        }

        private void SaveEdit()
        {
            //GET DATA SUBGROUP FROM GRIDVIEW PRICELIST DETAILS
            var DistinctSubGroup2ID = dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
               .Select(x => x.Cells["SubGroup2ID"].Value.ToString())
               .Distinct()
               .ToList();
            //END GET DATA SUBGROUP FROM GRIDVIEW PRICELIST DETAILS

            //INSERT LOG   
            //INSERT CHANGE DELIVERY METHOD
            Query = "SELECT DeliveryMethod FROM PricelistH WHERE PricelistNo = '" + txtPricelistNo.Text + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string OldDeliveryMethod = Convert.ToString(Cmd.ExecuteScalar());
            //if (OldDeliveryMethod != cmbDeliveryMethod.Text)
            //{
            //    Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
            //    Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldDeliveryMethod + "', '" + cmbDeliveryMethod.Text + "', '01', 'Waiting for Approval', 'Edit Change Delivery Method From : " + OldDeliveryMethod + " To " + cmbDeliveryMethod.Text + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();

            //}
            //END INSERT CHANGE DELIVERY METHOD     

            //INSERT CHANGE CRITERIA 
            Query = "SELECT Criteria FROM PricelistH WHERE PricelistNo = '" + txtPricelistNo.Text + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string OldCriteria = Convert.ToString(Cmd.ExecuteScalar());
            //if (OldCriteria != Convert.ToString(cmbAccountlist.SelectedValue))
            //{
            //    Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
            //    Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldCriteria + "', '" + cmbAccountlist.SelectedValue + "', '01', 'Waiting for Approval', 'Edit Change Criteria From : " + OldCriteria + " To " + cmbAccountlist.SelectedValue + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();
            //}
            //END INSERT CHANGE CRITERIA 

            //INSERT CHANGE VALID FROM
            Query = "SELECT CONVERT(VARCHAR, ValidFrom, 120), CONVERT(VARCHAR, ValidTo, 120) FROM PricelistH WHERE PricelistNo = '" + txtPricelistNo.Text + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                string OldValidFrom = Convert.ToString(Dr[0]);
                string OldValidTo = Convert.ToString(Dr[1]);

                //if (OldValidFrom != dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") && OldValidTo != dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                //{
                //    Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, PK4, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                //    Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldValidFrom + "', '" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + OldValidTo + "', '" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '01', 'Waiting for Approval', 'Edit Change Valid Date From : " + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + " To " + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //}
                //else if (OldValidFrom != dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                //{
                //    Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                //    Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldValidFrom + "', '" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '01', 'Waiting for Approval', 'Edit Change Valid Date From : " + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //}
                //else if (OldValidTo != dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                //{
                //    Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                //    Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldValidTo + "', '" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '01', 'Waiting for Approval', 'Edit Change Valid Date To : " + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //}
            }
            Dr.Close();
            //END INSERT CHANGE VALID TO

            //INSERT ADD & DELETE ACCOUNT LIST
            string AccountListCategory = "";
            if (PricelistType.ToUpper() == "JUAL")
            {
                AccountListCategory = "Customer";
            }
            else
            {
                AccountListCategory = "Vendor";
            }

            if (OldCriteria != Convert.ToString(cmbAccountlist.SelectedValue))
            {
                Query = "SELECT AccountId, Name FROM PriceList_AccountList WHERE PricelistNo = '" + txtPricelistNo.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    int CheckAccountList = 0;
                    string OldAccountId = Convert.ToString(Dr[0]);
                    string OldAccountName = Convert.ToString(Dr[1]);

                    for (int a = 0; a < dgvAccountList.RowCount; a++)
                    {
                        string AccountId = dgvAccountList.Rows[a].Cells[1].Value.ToString();
                        if (OldAccountId == AccountId)
                        {
                            CheckAccountList = 1;
                        }
                    }
                    if (CheckAccountList == 0)
                    {
                        //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                        //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldCriteria + "', '" + OldAccountId + "', '" + OldAccountName + "', '01', 'Waiting for Approval', 'Edit Delete " + AccountListCategory + " : " + OldAccountId + " - " + OldAccountName + " ', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                        //Cmd = new SqlCommand(Query, Conn, Trans);
                        //Cmd.ExecuteNonQuery();

                        Query = "DELETE FROM PriceList_AccountList WHERE PricelistNo='" + txtPricelistNo.Text + "' AND AccountId = '" + OldAccountId + "';";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                }
                Dr.Close();
            }

            if (OldCriteria != Convert.ToString(cmbAccountlist.SelectedValue) && Convert.ToString(cmbAccountlist.SelectedValue) != "1")
            {
                for (int i = 0; i < dgvAccountList.RowCount; i++)
                {
                    string AccountId = dgvAccountList.Rows[i].Cells[1].Value.ToString();
                    string AccountName = dgvAccountList.Rows[i].Cells[2].Value.ToString();

                    Query = "SELECT AccountId FROM PriceList_AccountList WHERE PricelistNo = '" + txtPricelistNo.Text + "' AND AccountId = '" + AccountId + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    string OldAccountId = Convert.ToString(Cmd.ExecuteScalar());

                    if (OldAccountId == "")
                    {
                        //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                        //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + cmbAccountlist.SelectedValue + "', '" + AccountId + "', '" + AccountName + "', '01', 'Waiting for Approval', 'Edit Add " + AccountListCategory + " : " + AccountId + " - " + AccountName + " ', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                        //Cmd = new SqlCommand(Query, Conn, Trans);
                        //Cmd.ExecuteNonQuery();

                        Query = "INSERT INTO PriceList_AccountList (PricelistNo, AccountId, Name, CreatedDate, CreatedBy) VALUES ";
                        Query += "('" + PricelistNo + "', '" + AccountId + "', '" + AccountName + "', getdate(), '" + ControlMgr.UserId + "' )";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }

            if (OldCriteria == Convert.ToString(cmbAccountlist.SelectedValue) && Convert.ToString(cmbAccountlist.SelectedValue) != "1")
            {
                Query = "SELECT AccountId, Name FROM PriceList_AccountList WHERE PricelistNo = '" + txtPricelistNo.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string OldAccountId = Convert.ToString(Dr[0]);
                    string OldAccountName = Convert.ToString(Dr[1]);
                    int CheckAccountId = 0;

                    for (int i = 0; i < dgvAccountList.RowCount; i++)
                    {
                        string NewAccountId = dgvAccountList.Rows[i].Cells[1].Value.ToString();

                        if (OldAccountId == NewAccountId)
                        {
                            CheckAccountId = 1;
                        }
                    }

                    if (CheckAccountId == 0)
                    {
                        //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                        //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldCriteria + "', '" + OldAccountId + "', '" + OldAccountName + "', '01', 'Waiting for Approval', 'Edit Delete " + AccountListCategory + " : " + OldAccountId + " - " + OldAccountName + " ', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                        //Cmd = new SqlCommand(Query, Conn, Trans);
                        //Cmd.ExecuteNonQuery();

                        Query = "DELETE FROM PriceList_AccountList WHERE PricelistNo='" + txtPricelistNo.Text + "' AND AccountId = '" + OldAccountId + "';";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                }
                Dr.Close();

                for (int i = 0; i < dgvAccountList.RowCount; i++)
                {
                    string NewAccountId = dgvAccountList.Rows[i].Cells[1].Value.ToString();
                    string NewAccountName = dgvAccountList.Rows[i].Cells[2].Value.ToString();

                    Query = "SELECT COUNT(AccountId) AS DataCount FROM PriceList_AccountList WHERE PricelistNo = '" + txtPricelistNo.Text + "' AND AccountId = '" + NewAccountId + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    int DataCount = Convert.ToInt32(Cmd.ExecuteScalar());
                    if (DataCount == 0)
                    {
                        //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                        //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + cmbAccountlist.SelectedValue + "', '" + NewAccountId + "', '" + NewAccountName + "', '01', 'Waiting for Approval', 'Edit Add " + AccountListCategory + " : " + NewAccountId + " - " + NewAccountName + " ', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                        //Cmd = new SqlCommand(Query, Conn, Trans);
                        //Cmd.ExecuteNonQuery();

                        Query = "INSERT INTO PriceList_AccountList (PricelistNo, AccountId, Name, CreatedDate, CreatedBy) VALUES ";
                        Query += "('" + PricelistNo + "', '" + NewAccountId + "', '" + NewAccountName + "', getdate(), '" + ControlMgr.UserId + "' )";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }

            if (Convert.ToString(cmbAccountlist.SelectedValue) == "1")
            {
                Query = "SELECT AccountId, Name FROM PriceList_AccountList WHERE PricelistNo = '" + txtPricelistNo.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string OldAccountId = Convert.ToString(Dr[0]);
                    string OldAccountName = Convert.ToString(Dr[1]);

                    //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                    //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + PricelistNo + "', '" + OldCriteria + "', '" + OldAccountId + "', '" + OldAccountName + "', '01', 'Waiting for Approval', 'Edit Delete " + AccountListCategory + " : " + OldAccountId + " - " + OldAccountName + " ', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                    //Cmd = new SqlCommand(Query, Conn, Trans);
                    //Cmd.ExecuteNonQuery();
                }

                Query = "DELETE FROM PriceList_AccountList WHERE PricelistNo='" + txtPricelistNo.Text + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }
            //END INSERT ADD & DELETE ACCOUNT LIST

            //PRICE LIST DETAILS 
            Query = "SELECT DISTINCT d.SubGroup2Id, d.Ukuran1_Value, d.Ukuran1_Value_To, d.Ukuran2_Value, d.Ukuran2_Value_To, ";
            Query += "d.Ukuran3_Value, d.Ukuran3_Value_To, d.Ukuran4_Value, d.Ukuran4_Value_To, ";
            Query += "d.Ukuran5_Value, d.Ukuran5_Value_To, d.DeliveryMethod,  ";
            Query += "d.Price, d.Tolerance, h.Criteria, i.SubGroup2Deskripsi, d.ManufacturerID, d.MerekID, d.GolonganID, d.KodeBerat, d.SpecID, d.Bentuk FROM Pricelist_Dtl d INNER JOIN PricelistH h ON h.PricelistNo = d.PricelistNo INNER JOIN InventTable i ON i.SubGroup2ID = d.SubGroup2Id WHERE d.PricelistNo = '" + txtPricelistNo.Text + "' AND d.PriceType = 0 ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                string OldSubGroup2Id = Convert.ToString(Dr[0]);
                string OldUkuran1_Value = Convert.ToString(Dr[1]);
                string OldUkuran1_Value_To = Convert.ToString(Dr[2]);
                string OldUkuran2_Value = Convert.ToString(Dr[3]);
                string OldUkuran2_Value_To = Convert.ToString(Dr[4]);
                string OldUkuran3_Value = Convert.ToString(Dr[5]);
                string OldUkuran3_Value_To = Convert.ToString(Dr[6]);
                string OldUkuran4_Value = Convert.ToString(Dr[7]);
                string OldUkuran4_Value_To = Convert.ToString(Dr[8]);
                string OldUkuran5_Value = Convert.ToString(Dr[9]);
                string OldUkuran5_Value_To = Convert.ToString(Dr[10]);
                string OldDeliveryMethods = Convert.ToString(Dr[11]);
                string OldPrices = Convert.ToString(Dr[12]);
                string OldTolerance = Convert.ToString(Dr[13]);
                string OldCriterias = Convert.ToString(Dr[14]);
                string OldSubGroup2Name = Convert.ToString(Dr[15]);
                string OldManufacturerID = Convert.ToString(Dr[16]);
                string OldMerekID = Convert.ToString(Dr[17]);
                string OldGolonganID = Convert.ToString(Dr[18]);
                string OldKodeBerat = Convert.ToString(Dr[19]);
                string OldSpecID = Convert.ToString(Dr[20]);
                string OldBentuk = Convert.ToString(Dr[21]);

                //for (int j = 0; j < DistinctSubGroup2ID.Count; j++)
                // {
                //      string SubGroup2ID = DistinctSubGroup2ID[j];
                int CheckPricelistDetails = 0;

                //GET DATA GRIDVIEW PRICELIST DETAILS 
                var DistinctPricelistDetails = from Pricelist in dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
                                               where Pricelist.Cells["SubGroup2ID"].Value.ToString() == OldSubGroup2Id
                                               select Pricelist;
                //END GET DATA GRIDVIEW PRICELIST DETAILS 

                //BASE PRICE
                foreach (var DataPricelist in DistinctPricelistDetails)
                {
                    string GroupID = DataPricelist.Cells["GroupID"].Value.ToString();
                    string SubGroup1ID = DataPricelist.Cells["SubGroup1ID"].Value.ToString();
                    string SubGroup2Name = DataPricelist.Cells["SubGroup2"].Value.ToString() == "" ? "" : DataPricelist.Cells["SubGroup2"].Value.ToString();
                    string Ukuran1_Value = DataPricelist.Cells["Ukuran1_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran1_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran1_Value_To = DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran2_Value = DataPricelist.Cells["Ukuran2_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran2_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran2_Value_To = DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran3_Value = DataPricelist.Cells["Ukuran3_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran3_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran3_Value_To = DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran4_Value = DataPricelist.Cells["Ukuran4_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran4_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran4_Value_To = DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran5_Value = DataPricelist.Cells["Ukuran5_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran5_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran5_Value_To = DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString()).ToString("N2"));
                    string Tolerance = Convert.ToString(DataPricelist.Cells["Tolerance(%)"].Value);
                    string Price = Convert.ToString(DataPricelist.Cells["Price[0]"].Value);
                    string ManufacturerID = DataPricelist.Cells["ManufacturerID"].Value.ToString();
                    string MerekID = DataPricelist.Cells["MerekID"].Value.ToString();
                    string GolonganID = DataPricelist.Cells["GolonganID"].Value.ToString();
                    string KodeBerat = DataPricelist.Cells["KodeBerat"].Value.ToString();
                    string SpecID = DataPricelist.Cells["SpecID"].Value.ToString();
                    string Bentuk = DataPricelist.Cells["Bentuk"].Value.ToString();
                  

                    if ((OldUkuran1_Value == Ukuran1_Value) && (OldUkuran1_Value_To == Ukuran1_Value_To) && (OldUkuran2_Value == Ukuran2_Value) && (OldUkuran2_Value_To == Ukuran2_Value_To)
                        && (OldUkuran3_Value == Ukuran3_Value) && (OldUkuran3_Value_To == Ukuran3_Value_To) && (OldUkuran4_Value == Ukuran4_Value) && (OldUkuran4_Value_To == Ukuran4_Value_To)
                        && (OldUkuran5_Value == Ukuran5_Value) && (OldUkuran5_Value_To == Ukuran5_Value_To) && (OldDeliveryMethods == cmbDeliveryMethod.Text) && (OldPrices == Price) && (OldTolerance == Tolerance) && (OldCriterias == Convert.ToString(cmbAccountlist.SelectedValue))
                        && (OldManufacturerID == ManufacturerID) && (OldMerekID == MerekID) && (OldGolonganID == GolonganID) && (OldKodeBerat == KodeBerat) && (OldSpecID == SpecID) && (OldBentuk == Bentuk))
                    {
                        CheckPricelistDetails = 1;
                    }
                }
                //END BASE PRICE   

                //if (CheckPricelistDetails == 0)
                //{
                //    Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, PK4, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                //    Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtPricelistNo.Text + "', '" + OldSubGroup2Id + " " + OldSubGroup2Name + "', ";
                //    Query += "'Ukuran 1 From " + OldUkuran1_Value + " To " + OldUkuran1_Value_To + ", Ukuran 2 From " + OldUkuran2_Value + " To " + OldUkuran2_Value_To + ", ";
                //    Query += "Ukuran 3 From " + OldUkuran3_Value + " To " + OldUkuran3_Value_To + ", Ukuran 4 From " + OldUkuran4_Value + " To " + OldUkuran4_Value_To + ", ";
                //    Query += "Ukuran 5 From " + OldUkuran5_Value + " To " + OldUkuran5_Value_To + "', '" + OldDeliveryMethods + "', '" + OldCriterias + "', '01', 'Waiting for Approval', 'Edit Delete Line', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                //    Cmd = new SqlCommand(Query, Conn, Trans);
                //    Cmd.ExecuteNonQuery();
                //}
                //  }     
            }
            Dr.Close();

            for (int j = 0; j < DistinctSubGroup2ID.Count; j++)
            {
                string SubGroup2ID = DistinctSubGroup2ID[j];
                int SeqSubGroup2ID = j;

                //GET DATA GRIDVIEW PRICELIST DETAILS 
                var DistinctPricelistDetails = from Pricelist in dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
                                               where Pricelist.Cells["SubGroup2ID"].Value.ToString() == SubGroup2ID
                                               select Pricelist;
                //END GET DATA GRIDVIEW PRICELIST DETAILS 

                //BASE PRICE
                foreach (var DataPricelist in DistinctPricelistDetails)
                {
                    string GroupID = DataPricelist.Cells["GroupID"].Value.ToString();
                    string SubGroup1ID = DataPricelist.Cells["SubGroup1ID"].Value.ToString();
                    string SubGroup2Name = DataPricelist.Cells["SubGroup2"].Value.ToString() == "" ? "" : DataPricelist.Cells["SubGroup2"].Value.ToString();
                    string Ukuran1_Value = DataPricelist.Cells["Ukuran1_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran1_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran1_Value_To = DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran2_Value = DataPricelist.Cells["Ukuran2_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran2_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran2_Value_To = DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran3_Value = DataPricelist.Cells["Ukuran3_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran3_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran3_Value_To = DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran4_Value = DataPricelist.Cells["Ukuran4_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran4_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran4_Value_To = DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString()).ToString("N2"));
                    string Ukuran5_Value = DataPricelist.Cells["Ukuran5_Value"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran5_Value"].Value.ToString()).ToString("N2"));
                    string Ukuran5_Value_To = DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString() == "" ? "0.00" : Convert.ToString(Convert.ToDouble(DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString()).ToString("N2"));
                    decimal Tolerance = Convert.ToDecimal(DataPricelist.Cells["Tolerance(%)"].Value);
                    decimal Price = Convert.ToDecimal(DataPricelist.Cells["Price[0]"].Value);
                    string ManufacturerID = DataPricelist.Cells["ManufacturerID"].Value.ToString();
                    string MerekID = DataPricelist.Cells["MerekID"].Value.ToString();
                    string GolonganID = DataPricelist.Cells["GolonganID"].Value.ToString();
                    string KodeBerat = DataPricelist.Cells["KodeBerat"].Value.ToString();
                    string SpecID = DataPricelist.Cells["SpecID"].Value.ToString();
                    string Bentuk = DataPricelist.Cells["Bentuk"].Value.ToString();

                    Query = "SELECT COUNT(d.PricelistNo) AS DataCount  FROM Pricelist_Dtl d INNER JOIN PricelistH h ON h.PricelistNo = d.PricelistNo WHERE d.SubGroup2Id = '" + SubGroup2ID + "' ";
                    Query += "AND d.Ukuran1_Value = " + Ukuran1_Value + " AND d.Ukuran1_Value_To = " + Ukuran1_Value_To + " AND d.Ukuran2_Value = " + Ukuran2_Value + " AND d.Ukuran2_Value_To = " + Ukuran2_Value_To + " ";
                    Query += "AND d.Ukuran3_Value = " + Ukuran3_Value + " AND d.Ukuran3_Value_To = " + Ukuran3_Value_To + " AND d.Ukuran4_Value = " + Ukuran4_Value + " AND d.Ukuran4_Value_To = " + Ukuran4_Value_To + " ";
                    Query += "AND d.Ukuran5_Value = " + Ukuran5_Value + " AND d.Ukuran5_Value_To = " + Ukuran5_Value_To + " AND d.DeliveryMethod = '" + cmbDeliveryMethod.Text + "' ";
                    Query += "AND d.PriceType = 0 AND d.Price = " + Price + " AND d.Tolerance = " + Tolerance + " AND d.PricelistNo = '" + txtPricelistNo.Text + "' AND h.Criteria = '" + cmbAccountlist.SelectedValue + "' ";
                    Query += "AND d.ManufacturerID = '" + ManufacturerID + "' AND d.MerekID = '" + MerekID + "' AND d.GolonganID = '" + GolonganID + "' AND d.KodeBerat = '" + KodeBerat + "' AND d.SpecID = '" + SpecID + "' AND d.Bentuk = '" + Bentuk + "'";


                    Cmd = new SqlCommand(Query, Conn, Trans);
                    int DataCount = Convert.ToInt32(Cmd.ExecuteScalar());
                    //if (DataCount == 0)
                    //{
                    //    if (j == SeqSubGroup2ID)
                    //    {
                    //        Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, PK1, PK2, PK3, PK4, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                    //        Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtPricelistNo.Text + "', '" + SubGroup2ID + " " + SubGroup2Name + "', ";
                    //        Query += "'Ukuran 1 From " + Ukuran1_Value + " To " + Ukuran1_Value_To + ", Ukuran 2 From " + Ukuran2_Value + " To " + Ukuran2_Value_To + ", ";
                    //        Query += "Ukuran 3 From " + Ukuran3_Value + " To " + Ukuran3_Value_To + ", Ukuran 4 From " + Ukuran4_Value + " To " + Ukuran4_Value_To + ", ";
                    //        Query += "Ukuran 5 From " + Ukuran5_Value + " To " + Ukuran5_Value_To + "', '" + cmbDeliveryMethod.Text + "', '" + cmbAccountlist.SelectedValue + "', '01', 'Waiting for Approval', 'Edit New Line', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                    //        Cmd = new SqlCommand(Query, Conn, Trans);
                    //        Cmd.ExecuteNonQuery();
                    //        SeqSubGroup2ID++;
                    //    }
                    //}
                }
                //END BASE PRICE                               
            }
            //END INSERT LOG

            //UPDATE PRICELIST HEADER
            Query = "UPDATE PricelistH SET ";
            Query += "ValidFrom='" + dtFrom.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
            Query += "ValidTo='" + dtTo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
            Query += "PricelistDate='" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',";
            Query += "RefId='" + ReferenceDoc1 + "',";
            Query += "Ref2Id='" + ReferenceDoc2 + "',";
            Query += "Criteria='" + Convert.ToString(cmbAccountlist.SelectedValue) + "',";
            Query += "DeliveryMethod='" + Convert.ToString(cmbDeliveryMethod.Text) + "',";
            Query += "Notes='" + txtNotes.Text + "',";
            Query += "UpdatedDate=getdate(),";
            Query += "UpdatedBy='" + ControlMgr.UserId + "', ";
            Query += "HIO = " + Convert.ToDecimal(txtHIO.Text) + " WHERE PricelistNo='" + txtPricelistNo.Text.Trim() + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
            //END UPDATE PRICELIST HEADER  

            //DELETE PRICE LIST DETAIL
            Query = "DELETE FROM Pricelist_Dtl WHERE PricelistNo='" + txtPricelistNo.Text.Trim() + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
            //END DELETE PRICE LIST DETAIL

            //INSERT PRICELIST DETAILS
            int SeqNo = 1;

            for (int i = 0; i < DistinctSubGroup2ID.Count; i++)
            {
                string SubGroup2ID = DistinctSubGroup2ID[i];
                int SeqSubGroup2ID = i;

                //GET DATA GRIDVIEW PRICELIST DETAILS 
                var DistinctPricelistDetails = from Pricelist in dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
                                               where Pricelist.Cells["SubGroup2ID"].Value.ToString() == SubGroup2ID
                                               select Pricelist;
                //END GET DATA GRIDVIEW PRICELIST DETAILS 

                //INSERT BASE PRICE
                foreach (var DataPricelist in DistinctPricelistDetails)
                {
                    string GroupID = DataPricelist.Cells["GroupID"].Value.ToString();
                    string SubGroup1ID = DataPricelist.Cells["SubGroup1ID"].Value.ToString();
                    string SubGroup2Name = DataPricelist.Cells["SubGroup2"].Value.ToString() == "" ? "" : DataPricelist.Cells["SubGroup2"].Value.ToString();
                    string Ukuran1_Value = DataPricelist.Cells["Ukuran1_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value"].Value.ToString();
                    string Ukuran1_Value_To = DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString();
                    string Ukuran2_Value = DataPricelist.Cells["Ukuran2_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value"].Value.ToString();
                    string Ukuran2_Value_To = DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString();
                    string Ukuran3_Value = DataPricelist.Cells["Ukuran3_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value"].Value.ToString();
                    string Ukuran3_Value_To = DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString();
                    string Ukuran4_Value = DataPricelist.Cells["Ukuran4_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value"].Value.ToString();
                    string Ukuran4_Value_To = DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString();
                    string Ukuran5_Value = DataPricelist.Cells["Ukuran5_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value"].Value.ToString();
                    string Ukuran5_Value_To = DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString();
                    string ItemID = DataPricelist.Cells["ItemID"].Value.ToString();
                    string FullItemID = DataPricelist.Cells["FullItemID"].Value.ToString();
                    string ItemName = DataPricelist.Cells["ItemName"].Value.ToString();
                    decimal Tolerance = Convert.ToDecimal(DataPricelist.Cells["Tolerance(%)"].Value);
                    decimal Price = Convert.ToDecimal(DataPricelist.Cells["Price[0]"].Value);
                    int SeqGroupNo = Convert.ToInt32(DataPricelist.Cells["SeqGroupNo"].Value);
                    string ManufacturerID = DataPricelist.Cells["ManufacturerID"].Value.ToString();
                    string MerekID = DataPricelist.Cells["MerekID"].Value.ToString();
                    string GolonganID = DataPricelist.Cells["GolonganID"].Value.ToString();
                    string KodeBerat = DataPricelist.Cells["KodeBerat"].Value.ToString();
                    string SpecID = DataPricelist.Cells["SpecID"].Value.ToString();
                    string Bentuk = DataPricelist.Cells["Bentuk"].Value.ToString();

                    Query = "INSERT INTO Pricelist_Dtl(PricelistNo, PricelistDate, Type, SeqGroupNo, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ";
                    Query += "Ukuran1_Value, Ukuran1_Value_To, Ukuran2_Value, Ukuran2_Value_To, Ukuran3_Value, Ukuran3_Value_To, ";
                    Query += "Ukuran4_Value, Ukuran4_Value_To, Ukuran5_Value, Ukuran5_Value_To, ItemId, FullItemId, ItemName, DeliveryMethod, ";
                    Query += "PriceType, Factor, Price, Tolerance, Ref_Config_RecId, CreatedDate, CreatedBy, ManufacturerID, MerekID, GolonganID, KodeBerat, SpecID, Bentuk) ";
                    Query += "VALUES('" + PricelistNo + "', '" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + Type + "', ";
                    Query += "" + SeqGroupNo + "," + SeqNo + ", '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', ";
                    Query += "" + Ukuran1_Value + ", " + Ukuran1_Value_To + ", " + Ukuran2_Value + ", " + Ukuran2_Value_To + ", " + Ukuran3_Value + ", " + Ukuran3_Value_To + ", ";
                    Query += "" + Ukuran4_Value + ", " + Ukuran4_Value_To + ", " + Ukuran5_Value + ", " + Ukuran5_Value_To + ", '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', '" + cmbDeliveryMethod.Text + "', ";
                    Query += "" + 0 + ", " + 0 + ", " + Price + ", " + Tolerance + ", " + 0 + ", getdate(), '" + ControlMgr.UserId + "', '" + ManufacturerID + "', '" + MerekID + "', '" + GolonganID + "', '" + KodeBerat + "', '" + SpecID + "', '" + Bentuk + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                //END INSERT BASE PRICE

                if (PricelistType.ToUpper() == "JUAL")
                {
                    //INSERT BY PRICE LIST CONFIG
                    Query = "SELECT PriceType, Factor, RecId FROM PricelistConfig WHERE SubGroup2Id = '" + SubGroup2ID + "' ORDER BY PriceType ASC";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        int PriceType = Convert.ToInt32(Dr["PriceType"]);
                        decimal Factor = Convert.ToDecimal(Dr["Factor"]);
                        long RecId = Convert.ToInt64(Dr["RecId"]);

                        foreach (var DataPricelist in DistinctPricelistDetails)
                        {
                            string GroupID = DataPricelist.Cells["GroupID"].Value.ToString();
                            string SubGroup1ID = DataPricelist.Cells["SubGroup1ID"].Value.ToString();
                            string Ukuran1_Value = DataPricelist.Cells["Ukuran1_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value"].Value.ToString();
                            string Ukuran1_Value_To = DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString();
                            string Ukuran2_Value = DataPricelist.Cells["Ukuran2_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value"].Value.ToString();
                            string Ukuran2_Value_To = DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString();
                            string Ukuran3_Value = DataPricelist.Cells["Ukuran3_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value"].Value.ToString();
                            string Ukuran3_Value_To = DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString();
                            string Ukuran4_Value = DataPricelist.Cells["Ukuran4_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value"].Value.ToString();
                            string Ukuran4_Value_To = DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString();
                            string Ukuran5_Value = DataPricelist.Cells["Ukuran5_Value"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value"].Value.ToString();
                            string Ukuran5_Value_To = DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString() == "" ? "0" : DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString();
                            string ItemID = DataPricelist.Cells["ItemID"].Value.ToString();
                            string FullItemID = DataPricelist.Cells["FullItemID"].Value.ToString();
                            string ItemName = DataPricelist.Cells["ItemName"].Value.ToString();
                            decimal Tolerance = Convert.ToDecimal(DataPricelist.Cells["Tolerance(%)"].Value);
                            decimal Price = 0;
                            int SeqGroupNo = Convert.ToInt32(DataPricelist.Cells["SeqGroupNo"].Value);
                            string ManufacturerID = DataPricelist.Cells["ManufacturerID"].Value.ToString();
                            string MerekID = DataPricelist.Cells["MerekID"].Value.ToString();
                            string GolonganID = DataPricelist.Cells["GolonganID"].Value.ToString();
                            string KodeBerat = DataPricelist.Cells["KodeBerat"].Value.ToString();
                            string SpecID = DataPricelist.Cells["SpecID"].Value.ToString();
                            string Bentuk = DataPricelist.Cells["Bentuk"].Value.ToString();

                            if (PriceType == 2)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[2]"].Value);
                            }
                            else if (PriceType == 3)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[3]"].Value);
                            }
                            else if (PriceType == 7)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[7]"].Value);
                            }
                            else if (PriceType == 14)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[14]"].Value);
                            }
                            else if (PriceType == 21)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[21]"].Value);
                            }
                            else if (PriceType == 30)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[30]"].Value);
                            }
                            else if (PriceType == 40)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[40]"].Value);
                            }
                            else if (PriceType == 45)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[45]"].Value);
                            }
                            else if (PriceType == 60)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[60]"].Value);
                            }
                            else if (PriceType == 75)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[75]"].Value);
                            }
                            else if (PriceType == 90)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[90]"].Value);
                            }
                            else if (PriceType == 120)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[120]"].Value);
                            }
                            else if (PriceType == 150)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[150]"].Value);
                            }
                            else if (PriceType == 180)
                            {
                                Price = Convert.ToDecimal(DataPricelist.Cells["Price[180]"].Value);
                            }

                            Query = "INSERT INTO Pricelist_Dtl(PricelistNo, PricelistDate, Type, SeqGroupNo, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ";
                            Query += "Ukuran1_Value, Ukuran1_Value_To, Ukuran2_Value, Ukuran2_Value_To, Ukuran3_Value, Ukuran3_Value_To, ";
                            Query += "Ukuran4_Value, Ukuran4_Value_To, Ukuran5_Value, Ukuran5_Value_To, ItemId, FullItemId, ItemName, DeliveryMethod, ";
                            Query += "PriceType, Factor, Price, Tolerance, Ref_Config_RecId, CreatedDate, CreatedBy, ManufacturerID, MerekID, GolonganID, KodeBerat, SpecID, Bentuk) ";
                            Query += "VALUES('" + PricelistNo + "', '" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + Type + "', ";
                            Query += "" + SeqGroupNo + "," + SeqNo + ", '" + GroupID + "', '" + SubGroup1ID + "', '" + SubGroup2ID + "', ";
                            Query += "" + Ukuran1_Value + ", " + Ukuran1_Value_To + ", " + Ukuran2_Value + ", " + Ukuran2_Value_To + ", " + Ukuran3_Value + ", " + Ukuran3_Value_To + ", ";
                            Query += "" + Ukuran4_Value + ", " + Ukuran4_Value_To + ", " + Ukuran5_Value + ", " + Ukuran5_Value_To + ", '" + ItemID + "', '" + FullItemID + "', '" + ItemName + "', '" + cmbDeliveryMethod.Text + "', ";
                            Query += "" + PriceType + ", " + Factor + ", " + Price + ", " + Tolerance + ", " + RecId + ", getdate(), '" + ControlMgr.UserId + "', '" + ManufacturerID + "', '" + MerekID + "', '" + GolonganID + "', '" + KodeBerat + "', '" + SpecID + "', '" + Bentuk + "')";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                    }
                    Dr.Close();
                    //END INSERT BY PRICE LIST CONFIG
                }
            }
            //END INSERT PRICELIST DETAILS

            InsertLog(PricelistNo, "Pricelist", "01", "E", Conn, Trans, Cmd);

            Trans.Commit();
            txtPricelistNo.Text = PricelistNo;
            MessageBox.Show("Data PriceListNo : " + PricelistNo + " berhasil diedit.");
        }

        string ReferenceDoc1;
        string ReferenceDoc2;
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Validasi())
                return;

            try
            {
                ReferenceDoc1 = lblReferenceDoc1.Text;
                ReferenceDoc2 = lblReferenceDoc2.Text;

                if (ReferenceDoc1 == "1.")
                {
                    ReferenceDoc1 = "";
                }
                else
                {
                    ReferenceDoc1 = ReferenceDoc1.Remove(0, 3);
                }

                if (ReferenceDoc2 == "2.")
                {
                    ReferenceDoc2 = "";
                }
                else
                {
                    ReferenceDoc2 = ReferenceDoc2.Remove(0, 3);
                }                

                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (txtHIO.Text == "")
                {
                    txtHIO.Text = "0.00";
                }

                txtNotes.Text = txtNotes.Text.Replace("'", "").Trim();

                if (Mode == "New" || txtPricelistNo.Text == "")
                {
                    SaveNew();
                }
                else
                {
                    SaveEdit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();                
            }
            if (CallType == "BeforeEdit")
            {
                ParentBeforeEdit.RefreshGrid();
            }

            ModeBeforeEdit();
        }

        public void GetDataHeader()
        {
            if (PricelistNo == "")
            {
                PricelistNo = txtPricelistNo.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            Query = "SELECT PricelistDate, PricelistNo, RefId, Ref2Id, ValidFrom, ValidTo, DeliveryMethod, Notes, Criteria, HIO ";
            Query += "FROM PricelistH WHERE PricelistNo = '"+PricelistNo+"' AND Type = '"+Type+"'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPricelistNo.Text = Dr["PriceListNo"].ToString();
                dtPricelist.Text = Dr["PricelistDate"].ToString();
                dtFrom.Text = Dr["ValidFrom"].ToString();
                dtTo.Text = Dr["ValidTo"].ToString();
                txtNotes.Text = Dr["Notes"].ToString();
                txtHIO.Text = Dr["HIO"].ToString();
                cmbDeliveryMethod.Text = Dr["DeliveryMethod"].ToString();
                cmbAccountlist.SelectedValue = Dr["Criteria"].ToString();
                if (Dr["RefId"].ToString() == "")
                {
                    lblReferenceDoc1.Text = "1.";
                }
                else
                {
                    lblReferenceDoc1.Text = "1. " + Dr["RefId"].ToString();
                }

                if (Dr["Ref2Id"].ToString() == "")
                {
                    lblReferenceDoc2.Text = "2.";
                }
                else
                {
                    lblReferenceDoc2.Text = "2. " + Dr["Ref2Id"].ToString();
                }
            }
            Dr.Close();

            dgvPricelistDetails.Rows.Clear();
            if(dgvPricelistDetails.RowCount == 0)
            {
                setHeaderDgvPricelistDetails();
            }            
            
            Query = "SELECT DISTINCT a.FullItemID, a.ItemName, 'KG' AS Unit, a.Tolerance, ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 0) IS NULL THEN 0 ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 0) END AS 'Price[0]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 2) IS NULL THEN 0 ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 2) END AS 'Price[2]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 3) IS NULL THEN 0 ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 3) END AS 'Price[3]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 7) IS NULL THEN 0 ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 7) END AS 'Price[7]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 14) IS NULL THEN 0 ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 14) END AS 'Price[14]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 21) IS NULL THEN 0 ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 21) END AS 'Price[21]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 30) IS NULL THEN 0 ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 30) END AS 'Price[30]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 40) IS NULL THEN 0  ";
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 40) END AS 'Price[40]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 45) IS NULL THEN 0 "; 
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 45) END AS 'Price[45]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 60) IS NULL THEN 0 "; 
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 60) END AS 'Price[60]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 75) IS NULL THEN 0 "; 
            Query += "ELSE  (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 75) END AS 'Price[75]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 90) IS NULL THEN 0 "; 
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 90) END AS 'Price[90]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 120) IS NULL THEN 0 "; 
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 120) END AS 'Price[120]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 150) IS NULL THEN 0 "; 
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 150) END AS 'Price[150]', ";
            Query += "CASE WHEN (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 180) IS NULL THEN 0 "; 
            Query += "ELSE (SELECT DISTINCT b.Price FROM Pricelist_Dtl b WHERE b.PricelistNo = a.PricelistNo AND b.FullItemId = a.FullItemId AND PriceType = 180) END AS 'Price[180]', ";
            Query += "a.GroupId AS GroupID, a.SubGroup1Id AS SubGroup1ID, a.SubGroup2Id AS SubGroup2ID, (SELECT Deskripsi FROM InventSubGroup2 WHERE SubGroup2ID = a.SubGroup2Id) AS SubGroup2, ";
            Query += "a.ItemId AS ItemID, a.Ukuran1_Value, a.Ukuran1_Value_To, a.Ukuran2_Value, a.Ukuran2_Value_To, a.Ukuran3_Value, a.Ukuran3_Value_To, ";
            Query += "a.Ukuran4_Value, a.Ukuran4_Value_To, a.Ukuran5_Value, a.Ukuran5_Value_To, a.SeqGroupNo, a.ManufacturerID, a.MerekID, a.GolonganID, a.KodeBerat, a.SpecID, a.Bentuk ";
            Query += "FROM Pricelist_Dtl a ";
            Query += "WHERE a.PricelistNo = '"+PricelistNo+"'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {

                this.dgvPricelistDetails.Rows.Add(i + 1, Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5], Dr[6], Dr[7], Dr[8], Dr[9], Dr[10], Dr[11], Dr[12], Dr[13], Dr[14], Dr[15], Dr[16], Dr[17], Dr[18], Dr[19], Dr[20], Dr[21], Dr[22], Dr[23], Dr[24], Dr[25], Dr[26], Dr[27], Dr[28], Dr[29], Dr[30], Dr[31], Dr[32], Dr[33], Dr[34], Dr[35], Dr[36], Dr[37], Dr[38], Dr[39], Dr[40]);
                i++;
            }
            Dr.Close();

            dgvPricelistDetails.ReadOnly = false;
            dgvPricelistDetails.AutoResizeColumns();
            dgvPricelistDetails.Refresh();
            FormatDataNumeric();  

            //Account List
            this.dgvAccountList.Rows.Clear();
            if (dgvAccountList.RowCount - 1 <= 0)
            {
                dgvAccountList.ColumnCount = 3;

                if (PricelistType.ToUpper() == "JUAL")
                {
                    dgvAccountList.Columns[0].Name = "No";
                    dgvAccountList.Columns[1].Name = "CustID";
                    dgvAccountList.Columns[2].Name = "CustName";
                }
                else
                {
                    dgvAccountList.Columns[0].Name = "No";
                    dgvAccountList.Columns[1].Name = "VendID";
                    dgvAccountList.Columns[2].Name = "VendName";
                 }     
            }

            Query = "Select AccountId, Name From PriceList_AccountList Where PricelistNo = '" + PricelistNo + "' ORDER BY AccountId ASC";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int j = 0;
            while (Dr.Read())
            {
                dgvAccountList.Rows.Add(j + 1, Dr[0], Dr[1]);
                j++;
            }
            Dr.Close();

            dgvAccountList.ReadOnly = false;
            dgvAccountList.Columns[0].ReadOnly = true;
            dgvAccountList.Columns[1].ReadOnly = true;
            dgvAccountList.Columns[2].ReadOnly = true;

            dgvAccountList.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAccountList.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAccountList.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAccountList.AutoResizeColumns();   

            Conn.Close();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;           
            btnCancel.Visible = false;

            if (CheckEditAccess() == "")
            {
                btnEdit.Visible = false;
            }
            else
            {
                btnEdit.Visible = true;
            }            

            dtFrom.Enabled = false;
            dtTo.Enabled = false;
            dtPricelist.Enabled = false;

            btnAddAccountList.Enabled = false;
            btnDeleteAccountList.Enabled = false;
            btnReferenceDoc.Enabled = false;
            btnAddPricelistDetail.Enabled = false;
            btnDeletePricelistDetail.Enabled = false;
            cmbAccountlist.Enabled = false;
            cmbDeliveryMethod.Enabled = false;
            txtNotes.Enabled = false;
            txtHIO.Enabled = false;
            
          
            dgvPricelistDetails.DefaultCellStyle.BackColor = Color.LightGray;
            dgvPricelistDetails.ReadOnly = true;
            EditColorAccountList();

            CheckApprovedAccess();
            CheckActiveAccess();
            SetActiveInactive();
        }

        private void CheckApprovedAccess()
        {
            //begin
            //updated by : joshua
            //updated date : 08 mei 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                SetApproveReject();                                
            }
            else
            {
                btnApprove.Visible = false;
                btnReject.Visible = false;
            }
            //end
        }

        private void CheckActiveAccess()
        {
            //begin
            //updated by : joshua
            //updated date : 15 mei 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                btnActive.Visible = true;
                btnInactive.Visible = true;
            }
            else
            {
                btnActive.Visible = false;
                btnInactive.Visible = false;
            }
            //end
        }

        private string CheckEditAccess()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT CreatedBy FROM PricelistH WHERE PricelistNo = '" + txtPricelistNo.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string CreatedBy = Convert.ToString(Cmd.ExecuteScalar());
            Conn.Close();

            string result = "";
            if (CreatedBy.ToUpper() == ControlMgr.UserId.ToUpper())
            {
                result = "success";
            }
            else if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                result = "success";
            }

            return result;
        }

        private void SetApproveReject()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT PricelistNo FROM PricelistH WHERE PricelistNo = '" + txtPricelistNo.Text + "' AND TransStatus IN ('02', '03', '04') ";
            Cmd = new SqlCommand(Query, Conn);
            string GetDataExisting = Convert.ToString(Cmd.ExecuteScalar());
            Conn.Close();
            if (GetDataExisting != "")
            {
                btnReject.Enabled = false;
                btnApprove.Enabled = false;
            }
            else
            {
                if (Mode == "BeforeEdit")
                {
                    btnApprove.Visible = true;
                    btnReject.Visible = true;
                }
                else if (Mode == "Edit")
                {
                    btnApprove.Visible = false;
                    btnReject.Visible = false;
                }
            }            
        }

        private void SetActiveInactive()
        {
            string Active = "", TransStatus = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT Active, TransStatus FROM PricelistH WHERE PricelistNo = '" + txtPricelistNo.Text + "' ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();   
            while (Dr.Read())
            {
                Active = Convert.ToString(Dr[0]);
                TransStatus = Convert.ToString(Dr[1]);
            }
            Dr.Close();
            Conn.Close();
            if (Active.ToUpper() == "FALSE")
            {
                if (Mode == "BeforeEdit")
                {
                    if (TransStatus != "03")
                    {
                        btnActive.Enabled = false;
                        btnInactive.Enabled = false;
                    }
                    else
                    {
                        btnActive.Enabled = true;
                        btnInactive.Enabled = false;
                    }                 

                    btnActive.Visible = true;
                    btnInactive.Visible = true;
                }
                else if(Mode == "Edit")
                {
                    btnActive.Enabled = false;
                    btnInactive.Enabled = false;

                    btnActive.Visible = false;
                    btnInactive.Visible = false;
                }                
            }
            else
            {               
                if (Mode == "BeforeEdit")
                {
                    if (TransStatus != "03")
                    {
                        btnActive.Enabled = false;
                        btnInactive.Enabled = false;
                    }
                    else
                    {
                        btnActive.Enabled = false;
                        btnInactive.Enabled = true;
                    }    

                    btnActive.Visible = true;
                    btnInactive.Visible = true;
                }
                else if (Mode == "Edit")
                {
                    btnActive.Enabled = false;
                    btnInactive.Enabled = false;

                    btnActive.Visible = false;
                    btnInactive.Visible = false;
                } 
            }
        }

        private void EditColorAccountList()
        {
            if (Mode == "BeforeEdit")
            {
                for (int i = 0; i < dgvAccountList.RowCount; i++)
                {
                    dgvAccountList.Rows[i].Cells[0].Style.BackColor = Color.LightGray;
                    dgvAccountList.Rows[i].Cells[1].Style.BackColor = Color.LightGray;
                    dgvAccountList.Rows[i].Cells[2].Style.BackColor = Color.LightGray;
                }
            }
            else
            {
                for (int i = 0; i < dgvAccountList.RowCount; i++)
                {
                    dgvAccountList.Rows[i].Cells[0].Style.BackColor = Color.White;
                    dgvAccountList.Rows[i].Cells[1].Style.BackColor = Color.White;
                    dgvAccountList.Rows[i].Cells[2].Style.BackColor = Color.White;
                }
            }                   
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 05 Mei 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                if (txtPricelistNo.Text != "")
                {
                    Query = "SELECT TransStatus FROM PricelistH WHERE PricelistNo='" + txtPricelistNo.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Check = Cmd.ExecuteScalar().ToString();
                    if (Check == "02" || Check == "03" || Check == "04")
                    {
                        MessageBox.Show("PricelistNo = " + txtPricelistNo.Text + ".\n" + "Tidak dapat diedit karena sudah diproses.");
                        Conn.Close();
                        return;
                    }
                }

                Mode = "Edit";              
                ModeEdit();
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

            dtFrom.Enabled = true;
            dtTo.Enabled = true;
            dtPricelist.Enabled = false;

            btnAddAccountList.Enabled = true;
            btnDeleteAccountList.Enabled = true;
            btnReferenceDoc.Enabled = true;
            btnAddPricelistDetail.Enabled = true;
            btnDeletePricelistDetail.Enabled = true;
            cmbAccountlist.Enabled = true;   
            if (Convert.ToString(cmbAccountlist.SelectedValue) == "" || Convert.ToString(cmbAccountlist.SelectedValue) == "1")
            {
                btnAddAccountList.Enabled = false;
            }
            else
            {
                btnAddAccountList.Enabled = true;           
            }
           


            cmbDeliveryMethod.Enabled = true;
            txtNotes.Enabled = true;
            txtHIO.Enabled = true;

           
            dgvPricelistDetails.DefaultCellStyle.BackColor = Color.White;
            dgvPricelistDetails.ReadOnly = true;
            EditColorAccountList();

            CheckApprovedAccess();
            CheckActiveAccess();
            SetActiveInactive();
            HIOsetting();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";           
            GetDataHeader();
            ModeBeforeEdit();
        }

        private void btnAddAccountList_Click(object sender, EventArgs e)
        {
            if (PricelistType.ToUpper() == "JUAL")
            {
                LookupCustomer LC = new LookupCustomer();
                LC.ParentRefreshGrid(this);
                LC.ParamHeader(dgvAccountList);
                LC.ShowDialog();
                EditColorAccountList();
            }
            else
            {
                LookupVendorReference LV = new LookupVendorReference();
                LV.ParentRefreshGrid(this);
                LV.ParamHeader(dgvAccountList);
                LV.ShowDialog();
                EditColorAccountList();
            } 
        }

        private void btnDeleteAccountList_Click(object sender, EventArgs e)
        {
            if (dgvAccountList.RowCount > 0)
            {
                Index = dgvAccountList.CurrentRow.Index;
                DialogResult dialogResult;
                if (PricelistType.ToUpper() == "JUAL")
                {
                     dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvAccountList.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " CustID = " + dgvAccountList.Rows[Index].Cells["CustID"].Value.ToString() + Environment.NewLine + " CustName = " + dgvAccountList.Rows[Index].Cells["CustName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                }
                else
                { 
                    dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvAccountList.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " VendID = " + dgvAccountList.Rows[Index].Cells["VendID"].Value.ToString() + Environment.NewLine + " VendName = " + dgvAccountList.Rows[Index].Cells["VendName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                }

                if (dialogResult == DialogResult.Yes)
                {
                    string NumberGroupSeq = dgvAccountList.Rows[dgvAccountList.CurrentCell.RowIndex].Cells["No"].Value.ToString();
                    dgvAccountList.Rows.RemoveAt(Index);
                    SortNoDataGridAccountList();
                    
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
            if (dgvAccountList.RowCount - 1 <= 0)
            {
                dgvAccountList.ColumnCount = 3;
                dgvAccountList.Columns[0].Name = "No";
                dgvAccountList.Columns[1].Name = "CustID";
                dgvAccountList.Columns[2].Name = "CustName";
            }

            for (int i = 0; i < CustID.Count; i++)
            {
                this.dgvAccountList.Rows.Add((dgvAccountList.RowCount + 1).ToString(), CustID[i], CustName[i]);
            }

            dgvAccountList.ReadOnly = false;
            dgvAccountList.Columns["No"].ReadOnly = true;
            dgvAccountList.Columns["CustID"].ReadOnly = true;
            dgvAccountList.Columns["CustName"].ReadOnly = true;

            dgvAccountList.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAccountList.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAccountList.Columns["CustName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvAccountList.AutoResizeColumns();
        }

        public void AddDataGridVendor(List<string> VendID, List<string> VendName)
        {
            if (dgvAccountList.RowCount - 1 <= 0)
            {
                dgvAccountList.ColumnCount = 3;
                dgvAccountList.Columns[0].Name = "No";
                dgvAccountList.Columns[1].Name = "VendID";
                dgvAccountList.Columns[2].Name = "VendName";
            }

            for (int i = 0; i < VendID.Count; i++)
            {
                this.dgvAccountList.Rows.Add((dgvAccountList.RowCount + 1).ToString(), VendID[i], VendName[i]);
            }

            dgvAccountList.ReadOnly = false;
            dgvAccountList.Columns["No"].ReadOnly = true;
            dgvAccountList.Columns["VendID"].ReadOnly = true;
            dgvAccountList.Columns["VendName"].ReadOnly = true;

            dgvAccountList.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAccountList.Columns["VendID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvAccountList.Columns["VendName"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvAccountList.AutoResizeColumns();
        }

        private void SortNoDataGridAccountList()
        {
            for (int i = 0; i < dgvAccountList.RowCount; i++)
            {
                dgvAccountList.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void SortNoDataGridPricelistDetails()
        {
            for (int i = 0; i < dgvPricelistDetails.RowCount; i++)
            {
                dgvPricelistDetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void AccountList_SelectedIndexChange(object sender, EventArgs e)
        {
            if (Convert.ToString(cmbAccountlist.SelectedValue) == "1" || Convert.ToString(cmbAccountlist.SelectedValue) == "")
            {
                btnAddAccountList.Enabled = false;

                for (int i = 0; i < dgvAccountList.RowCount; i++)
                {
                    dgvAccountList.Rows.Clear();
                    dgvAccountList.Refresh();
                }
            }
            else
            {
                btnAddAccountList.Enabled = true;
            }

            dgvPricelistDetails.Rows.Clear();
        }

        private void btnAddPricelistDetail_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 30 apr 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                if (Convert.ToString(cmbAccountlist.SelectedValue) == "")
                {
                    if (PricelistType.ToUpper() == "JUAL")
                        {
                            MessageBox.Show("Customer List harus diisi");
                        }
                        else
                        {
                            MessageBox.Show("Vendor List harus diisi");
                        }
                        return;
                }

                if (Convert.ToString(cmbAccountlist.SelectedValue) == "2" || Convert.ToString(cmbAccountlist.SelectedValue) == "3")
                {
                    if (dgvAccountList.RowCount == 0)
                    {
                        if (PricelistType.ToUpper() == "JUAL")
                        {
                            MessageBox.Show("Customer List tidak boleh kosong");
                        }
                        else
                        {
                            MessageBox.Show("Vendor List tidak boleh kosong");
                        }
                        return;
                    }
                }
                else if (cmbDeliveryMethod.SelectedIndex == 0)
                {
                    MessageBox.Show("Delivery Method harus diisi");
                    return;
                }

                SubGroupHeader PH = new SubGroupHeader();
                PH.SetMode("New", PricelistType, cmbDeliveryMethod.Text, dgvAccountList, cmbAccountlist.SelectedValue.ToString());
                PH.SetParent(this);
                PH.ShowDialog();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void btnReferenceDoc_Click(object sender, EventArgs e)
        {
            LookupReferenceDoc LRD = new LookupReferenceDoc();
            LRD.ParamHeader(PricelistType);
            LRD.ParentRefreshGrid(this);
            LRD.ShowDialog();
        }

        public void setHeaderDgvPricelistDetails()
        {
            if (dgvPricelistDetails.RowCount - 1 <= 0)
            {
                dgvPricelistDetails.ColumnCount = 42;
                dgvPricelistDetails.Columns[0].Name = "No";
                dgvPricelistDetails.Columns[1].Name = "FullItemID";
                dgvPricelistDetails.Columns[2].Name = "ItemName";
                dgvPricelistDetails.Columns[3].Name = "Unit";
                dgvPricelistDetails.Columns[4].Name = "Tolerance(%)";
                dgvPricelistDetails.Columns[5].Name = "Price[0]";
                dgvPricelistDetails.Columns[6].Name = "Price[2]";
                dgvPricelistDetails.Columns[7].Name = "Price[3]";
                dgvPricelistDetails.Columns[8].Name = "Price[7]";
                dgvPricelistDetails.Columns[9].Name = "Price[14]";
                dgvPricelistDetails.Columns[10].Name = "Price[21]";
                dgvPricelistDetails.Columns[11].Name = "Price[30]";
                dgvPricelistDetails.Columns[12].Name = "Price[40]";
                dgvPricelistDetails.Columns[13].Name = "Price[45]";
                dgvPricelistDetails.Columns[14].Name = "Price[60]";
                dgvPricelistDetails.Columns[15].Name = "Price[75]";
                dgvPricelistDetails.Columns[16].Name = "Price[90]";
                dgvPricelistDetails.Columns[17].Name = "Price[120]";
                dgvPricelistDetails.Columns[18].Name = "Price[150]";
                dgvPricelistDetails.Columns[19].Name = "Price[180]";
                dgvPricelistDetails.Columns[20].Name = "GroupID";
                dgvPricelistDetails.Columns[21].Name = "SubGroup1ID";
                dgvPricelistDetails.Columns[22].Name = "SubGroup2ID";
                dgvPricelistDetails.Columns[23].Name = "SubGroup2";
                dgvPricelistDetails.Columns[24].Name = "ItemID";
                dgvPricelistDetails.Columns[25].Name = "Ukuran1_Value";
                dgvPricelistDetails.Columns[26].Name = "Ukuran1_Value_To";
                dgvPricelistDetails.Columns[27].Name = "Ukuran2_Value";
                dgvPricelistDetails.Columns[28].Name = "Ukuran2_Value_To";
                dgvPricelistDetails.Columns[29].Name = "Ukuran3_Value";
                dgvPricelistDetails.Columns[30].Name = "Ukuran3_Value_To";
                dgvPricelistDetails.Columns[31].Name = "Ukuran4_Value";
                dgvPricelistDetails.Columns[32].Name = "Ukuran4_Value_To";
                dgvPricelistDetails.Columns[33].Name = "Ukuran5_Value";
                dgvPricelistDetails.Columns[34].Name = "Ukuran5_Value_To";
                dgvPricelistDetails.Columns[35].Name = "SeqGroupNo";
                dgvPricelistDetails.Columns[36].Name = "ManufacturerID";
                dgvPricelistDetails.Columns[37].Name = "MerekID";
                dgvPricelistDetails.Columns[38].Name = "GolonganID";
                dgvPricelistDetails.Columns[39].Name = "KodeBerat";
                dgvPricelistDetails.Columns[40].Name = "SpecID";
                dgvPricelistDetails.Columns[41].Name = "Bentuk";

                if (PricelistType.ToUpper() == "JUAL")
                {
                    for (int i = 20; i <= 41; i++)
                    {
                        dgvPricelistDetails.Columns[i].Visible = false;
                    }                   
                }               
                else
                {
                    for (int i = 6; i <= 41; i++)
                    {
                        dgvPricelistDetails.Columns[i].Visible = false;
                    } 
                }
            }
        }

        private void btnDeletePricelistDetail_Click(object sender, EventArgs e)
        {
            if (dgvPricelistDetails.RowCount > 0)
            {
                string No = dgvPricelistDetails.Rows[dgvPricelistDetails.CurrentCell.RowIndex].Cells["No"].Value.ToString();
                string SubGroup2ID = dgvPricelistDetails.Rows[dgvPricelistDetails.CurrentCell.RowIndex].Cells["SubGroup2ID"].Value.ToString();
                string SubGroup2 = dgvPricelistDetails.Rows[dgvPricelistDetails.CurrentCell.RowIndex].Cells["SubGroup2"].Value.ToString();
                string SeqGroupNo = dgvPricelistDetails.Rows[dgvPricelistDetails.CurrentCell.RowIndex].Cells["SeqGroupNo"].Value.ToString();

                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "No = " + No + Environment.NewLine + "Sub Group 2 = " + SubGroup2 + Environment.NewLine + "Seq Group No = " + SeqGroupNo + Environment.NewLine + "Akan dihapus?" + Environment.NewLine + Environment.NewLine + "Warning : Semua data Sub Group 2 = " + SubGroup2 + " dengan Seq Group No = " + SeqGroupNo + " akan terhapus", "Delete Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    for (int i = dgvPricelistDetails.Rows.Count - 1; i >= 0; i--)
                    {
                        if (dgvPricelistDetails.Rows[i].Cells["SubGroup2ID"].Value != null)
                        {
                            if (dgvPricelistDetails.Rows[i].Cells["SubGroup2ID"].Value.ToString() == SubGroup2ID && dgvPricelistDetails.Rows[i].Cells["SeqGroupNo"].Value.ToString() == SeqGroupNo)
                            {
                                dgvPricelistDetails.Rows.RemoveAt(i);
                            }
                        }
                    }
                    SortNoDataGridPricelistDetails();
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        public void FormatDataNumeric()
        {
            for (int i = 5; i < 20; i++)
            {
                dgvPricelistDetails.Columns[i].DefaultCellStyle.Format = "N2";
            }
        }

        private void cmbDeliveryMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvPricelistDetails.Rows.Clear();
            HIOsetting();
        }

        private void HIOsetting()
        {
            if (cmbDeliveryMethod.Text.ToUpper() != "LOCO")
            {
                txtHIO.Enabled = true;
                //txtHIO.Text = "";
            }
            if (cmbDeliveryMethod.Text.ToUpper() == "LOCO")
            {
                txtHIO.Enabled = false;
                txtHIO.Text = "0.00";
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
             DialogResult dr = MessageBox.Show("PriceListNo = " + txtPricelistNo.Text + "\n" + "Apakah data diatas akan diapprove ?", "Konfirmasi", MessageBoxButtons.YesNo);
             if (dr == DialogResult.Yes)
             {
                 if (CheckOldPricelistNo() != "")
                 {
                     return;
                 }
                 else
                 {
                     try
                     {
                         Conn = ConnectionString.GetConnection();
                         Trans = Conn.BeginTransaction();

                         Query = "UPDATE PricelistH SET TransStatus = '03' WHERE PricelistNo = '" + txtPricelistNo.Text + "' ";
                         Cmd = new SqlCommand(Query, Conn, Trans);
                         Cmd.ExecuteNonQuery();

                         //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                         //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtPricelistNo.Text + "', '03', 'Approved', 'Approved By : " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                         //Cmd = new SqlCommand(Query, Conn);
                         //Cmd.ExecuteNonQuery();

                         InsertLog(txtPricelistNo.Text, "Pricelist", "03", "", Conn, Trans, Cmd);

                         Trans.Commit();

                         MessageBox.Show("Data PriceListNo : " + txtPricelistNo.Text + " berhasil diapprove.");
                         SetApproveReject();
                         btnInactive.Enabled = true;
                     }
                     catch (Exception)
                     {
                         Trans.Rollback();
                         return;
                     }
                     finally
                     {
                         Conn.Close();
                     }
                 }
             }           
        }

        private void InsertLog(string TransaksiID, string TransCode, string StatusCode, string Action, SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            Query = "SELECT Deskripsi FROM TransStatusTable WHERE TransCode = '" + TransCode + "' AND StatusCode = '" + StatusCode + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string StatusTransaksi = Convert.ToString(Cmd.ExecuteScalar());

            if (Action == "")
            {
                Query = "SELECT TOP 1 Action FROM PriceList_LogTable WHERE TransaksiID = '" + TransaksiID + "' ORDER BY LogDatetime DESC";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Action = Convert.ToString(Cmd.ExecuteScalar());
            }

            if (StatusCode == "Active")
            {
                StatusTransaksi = "Active";
            }
            else if (StatusCode == "Inactive")
            {
                StatusTransaksi = "Inactive";
            }

            Query = "INSERT INTO PriceList_LogTable (TransaksiID, StatusTransaksi, Action, UserID, LogDatetime) ";
            Query += "VALUES ('" + TransaksiID + "', '" + StatusTransaksi + "', '" + Action + "', '" + ControlMgr.UserId + "', GETDATE())";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
        }


        private string CheckOldPricelistNo()
        {
            string result = "";
            string AccountList = "";
            if (dgvAccountList.RowCount > 0)
            {
                for (int i = 0; i < dgvAccountList.RowCount; i++)
                {
                    AccountList = AccountList + "" + dgvAccountList.Rows[i].Cells[1].Value + "-";
                }
                AccountList = AccountList.Remove(AccountList.Length - 1);
            }

            try
            {
                DataCheckPricelist.Clear();
                ListSubGroup2.Clear();

                Conn = ConnectionString.GetConnection();
                Query = "SELECT DISTINCT d.SubGroup2Id, d.Ukuran1_Value, d.Ukuran1_Value_To, d.Ukuran2_Value, d.Ukuran2_Value_To, ";
                Query += "d.Ukuran3_Value, d.Ukuran3_Value_To, d.Ukuran4_Value, d.Ukuran4_Value_To, d.Ukuran5_Value, d.Ukuran5_Value_To, i.SubGroup2Deskripsi, ";
                Query += "d.ManufacturerID, d.MerekID, d.GolonganID ,d.KodeBerat ,d.SpecID ,d.Bentuk ";
                Query += "FROM Pricelist_Dtl d INNER JOIN InventTable i ON i.SubGroup2ID = d.SubGroup2Id WHERE d.PricelistNo = '" + txtPricelistNo.Text + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string SubGroup2Id = Convert.ToString(Dr[0]);
                    string Ukuran1_Value = Convert.ToString(Dr[1]);
                    string Ukuran1_Value_To = Convert.ToString(Dr[2]);
                    string Ukuran2_Value = Convert.ToString(Dr[3]);
                    string Ukuran2_Value_To = Convert.ToString(Dr[4]);
                    string Ukuran3_Value = Convert.ToString(Dr[5]);
                    string Ukuran3_Value_To = Convert.ToString(Dr[6]);
                    string Ukuran4_Value = Convert.ToString(Dr[7]);
                    string Ukuran4_Value_To = Convert.ToString(Dr[8]);
                    string Ukuran5_Value = Convert.ToString(Dr[9]);
                    string Ukuran5_Value_To = Convert.ToString(Dr[10]);
                    string SubGroup2 = Convert.ToString(Dr[11]);
                    string ManufacturerID = Convert.ToString(Dr[12]);
                    string MerekID = Convert.ToString(Dr[13]);
                    string GolonganID = Convert.ToString(Dr[14]);
                    string KodeBerat = Convert.ToString(Dr[15]);
                    string SpecID = Convert.ToString(Dr[16]);
                    string Bentuk = Convert.ToString(Dr[17]);

                    //GET DATA GRIDVIEW PRICELIST DETAILS 
                    string OldFullItemID = "";
                    var DistinctPricelistDetails = from Pricelist in dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
                                                   where Pricelist.Cells["SubGroup2ID"].Value.ToString() == SubGroup2Id
                                                   select Pricelist;

                    foreach (var DataPricelist in DistinctPricelistDetails)
                    {
                        OldFullItemID = OldFullItemID + "" + DataPricelist.Cells["FullItemID"].Value.ToString() + "-";
                    }
                    OldFullItemID = OldFullItemID.Remove(OldFullItemID.Length - 1);

                    //END GET DATA GRIDVIEW PRICELIST DETAILS 

                    Cmd = new SqlCommand("SP_CheckPricelist", Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@PricelistType", PricelistType);
                    Cmd.Parameters.AddWithValue("@SubGroup2Id", SubGroup2Id);
                    Cmd.Parameters.AddWithValue("@Ukuran1From", Ukuran1_Value);
                    Cmd.Parameters.AddWithValue("@Ukuran1To", Ukuran1_Value_To);
                    Cmd.Parameters.AddWithValue("@Ukuran2From", Ukuran2_Value);
                    Cmd.Parameters.AddWithValue("@Ukuran2To", Ukuran2_Value_To);
                    Cmd.Parameters.AddWithValue("@Ukuran3From", Ukuran3_Value);
                    Cmd.Parameters.AddWithValue("@Ukuran3To", Ukuran3_Value_To);
                    Cmd.Parameters.AddWithValue("@Ukuran4From", Ukuran4_Value);
                    Cmd.Parameters.AddWithValue("@Ukuran4To", Ukuran4_Value_To);
                    Cmd.Parameters.AddWithValue("@Ukuran5From", Ukuran5_Value);
                    Cmd.Parameters.AddWithValue("@Ukuran5To", Ukuran5_Value_To);
                    Cmd.Parameters.AddWithValue("@DeliveryMethod", cmbDeliveryMethod.Text);
                    Cmd.Parameters.AddWithValue("@AccountList", AccountList);
                    Cmd.Parameters.AddWithValue("@Criteria", cmbAccountlist.SelectedValue);
                    Cmd.Parameters.AddWithValue("@OldFullItemID", OldFullItemID);
                    Cmd.Parameters.AddWithValue("ManufacturerID", ManufacturerID);
                    Cmd.Parameters.AddWithValue("MerekID", MerekID);
                    Cmd.Parameters.AddWithValue("GolonganID", GolonganID);
                    Cmd.Parameters.AddWithValue("KodeBerat", KodeBerat);
                    Cmd.Parameters.AddWithValue("SpecID", SpecID);
                    Cmd.Parameters.AddWithValue("Bentuk", Bentuk);

                    Dr2 = Cmd.ExecuteReader();                 
                    while (Dr2.Read())
                    {
                        if (dgvAccountList.RowCount > 0)
                        {
                            DataCheckPricelist.Add(Convert.ToString(Dr2[0]), Convert.ToString(Dr2[1]));
                        }
                        else
                        {
                            DataCheckPricelist.Add(Convert.ToString(Dr2[0]), "");
                        }

                        ListSubGroup2.Add(SubGroup2);
                    }
                }

                int i = 0;
                if (DataCheckPricelist.Count != 0)
                {
                    string ErrorMessage = "\nMasih ada data yang aktif pada \n";
                    if (dgvAccountList.RowCount > 0)
                    {
                        string AccountListCategory = "";
                        if (PricelistType.ToUpper() == "JUAL")
                        {
                            AccountListCategory = "Customer ID : ";
                        }
                        else
                        {
                            AccountListCategory = "Vendor ID : ";
                        }

                        foreach (var ExistingData in DataCheckPricelist)
                        {
                            ErrorMessage = ErrorMessage + "PricelistNo : " + ExistingData.Key + "\n" + "Sub Group 2 : " + ListSubGroup2[i] + "\n" + AccountListCategory + ExistingData.Value + "\n";
                            i++;
                        }
                    }
                    else
                    {
                        foreach (var ExistingData in DataCheckPricelist)
                        {
                            ErrorMessage = ErrorMessage + "PricelistNo : " + ExistingData.Key + "\n" + "Sub Group 2 : " + ListSubGroup2[i] + "\n";
                            i++;
                        }
                    }

                    MessageBox.Show("Data dengan Delivery Method : " + cmbDeliveryMethod.Text + ErrorMessage);
                    result = "error";
                }
                             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Dr.Close();
                Dr2.Close();   
                Conn.Close();
            }

            return result;
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
             DialogResult dr = MessageBox.Show("PriceListNo = " + txtPricelistNo.Text + "\n" + "Apakah data diatas akan direject ?", "Konfirmasi", MessageBoxButtons.YesNo);
             if (dr == DialogResult.Yes)
             {
                 try
                 {
                     Conn = ConnectionString.GetConnection();
                     Trans = Conn.BeginTransaction();

                     Query = "UPDATE PricelistH SET TransStatus = '02', Active = 0 WHERE PricelistNo = '" + txtPricelistNo.Text + "' ";
                     Cmd = new SqlCommand(Query, Conn, Trans);
                     Cmd.ExecuteNonQuery();

                     //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                     //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtPricelistNo.Text + "', '02', 'Rejected', 'Rejected By : " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                     //Cmd = new SqlCommand(Query, Conn);
                     //Cmd.ExecuteNonQuery();


                     InsertLog(txtPricelistNo.Text, "Pricelist", "02", "", Conn, Trans, Cmd);

                     Trans.Commit();

                     MessageBox.Show("Data PriceListNo : " + txtPricelistNo.Text + " berhasil direject.");
                     SetApproveReject();
                 }
                 catch (Exception)
                 {
                     Trans.Rollback();
                     return;
                 }
                 finally
                 {
                     Conn.Close();
                 }
             }
           
        }

        private void btnInactive_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("PriceListNo = " + txtPricelistNo.Text + "\n" + "Apakah data diatas akan dinonaktifkan ?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "UPDATE PricelistH SET Active = 0 WHERE PricelistNo = '" + txtPricelistNo.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    string TransStatus = "", Deskripsi = "";
                    Query = "SELECT p.TransStatus, t.Deskripsi FROM PricelistH p INNER JOIN TransStatusTable t ON t.StatusCode = p.TransStatus  WHERE t.TransCode = 'Pricelist' AND PricelistNo = '" + txtPricelistNo.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        TransStatus = Convert.ToString(Dr[0]);
                        Deskripsi = Convert.ToString(Dr[1]);
                    }
                    Dr.Close();

                    //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                    //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtPricelistNo.Text + "', '" + TransStatus + "', '" + Deskripsi + "', 'Change Active Status From Active to Inactive By " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                    //Cmd = new SqlCommand(Query, Conn);
                    //Cmd.ExecuteNonQuery();

                    InsertLog(txtPricelistNo.Text, "Pricelist", "Inactive", "", Conn, Trans, Cmd);

                    Trans.Commit();

                    MessageBox.Show("Data PriceListNo : " + txtPricelistNo.Text + " berhasil dinonaktif.");
                    SetActiveInactive();
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return;
                }
                finally
                {
                    Conn.Close();
                } 
            }
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("PriceListNo = " + txtPricelistNo.Text + "\n" + "Apakah data diatas akan diaktifkan ?", "Konfirmasi", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                if (CheckOldPricelistNo() != "")
                {
                    return;
                }
                else
                {
                    try
                    {
                        Conn = ConnectionString.GetConnection();
                        Trans = Conn.BeginTransaction();

                        Query = "UPDATE PricelistH SET Active = 1 WHERE PricelistNo = '" + txtPricelistNo.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        string TransStatus = "", Deskripsi = "";
                        Query = "SELECT p.TransStatus, t.Deskripsi FROM PricelistH p INNER JOIN TransStatusTable t ON t.StatusCode = p.TransStatus  WHERE t.TransCode = 'Pricelist' AND PricelistNo = '" + txtPricelistNo.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            TransStatus = Convert.ToString(Dr[0]);
                            Deskripsi = Convert.ToString(Dr[1]);
                        }
                        Dr.Close();

                        //Query = "INSERT INTO PriceList_LogTable (PriceListDate, PriceListNo, LogStatusCode, LogStatusDesc, LogDescription, UserID, LogDate, Type) VALUES ";
                        //Query += "('" + dtPricelist.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + txtPricelistNo.Text + "', '" + TransStatus + "', '" + Deskripsi + "', 'Change Active Status From Inactive to Active By " + ControlMgr.GroupName + "', '" + ControlMgr.UserId + "', getdate(), '" + Type + "')";
                        //Cmd = new SqlCommand(Query, Conn);
                        //Cmd.ExecuteNonQuery();


                        InsertLog(txtPricelistNo.Text, "Pricelist", "Active", "", Conn, Trans, Cmd);

                        Trans.Commit();

                        MessageBox.Show("Data PriceListNo : " + txtPricelistNo.Text + " berhasil diaktifkan.");
                        SetActiveInactive();
                    }
                    catch (Exception)
                    {
                        Trans.Rollback();
                        return;
                    }
                    finally
                    {
                        Conn.Close();
                    }  
                }
            }  
        }

        private void txtHIO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        //Tia 26062018
        //klik kanan
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.CustomerID.Customer Cust = null;

        TaskList.Pricelist.TasklistPricelist ParentToTlPl;

        public void SetParent2(TaskList.Pricelist.TasklistPricelist TlPl)
        {
            ParentToTlPl = TlPl;
        }

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }
      
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

        private void dgvPricelistDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvPricelistDetails.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPricelistDetails.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPricelistDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvPricelistDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvPricelistDetails.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }

        private void dgvAccountList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                 if (Vendor == null || Vendor.Text == "")
                 {
                     if (dgvAccountList.Columns[e.ColumnIndex].Name.ToString() == "VendID" || dgvAccountList.Columns[e.ColumnIndex].Name.ToString() == "VendName")
                     {
                         Vendor = new PopUp.Vendor.Vendor();
                         Vendor.GetData(dgvAccountList.Rows[e.RowIndex].Cells["VendID"].Value.ToString());

                         Vendor.Show();
                     }
                 }
                 else if (CheckOpened(Vendor.Name))
                 {
                     Vendor.WindowState = FormWindowState.Normal;
                     Vendor.GetData(dgvAccountList.Rows[e.RowIndex].Cells["VendID"].Value.ToString());
                     Vendor.Show();
                     Vendor.Focus();
                 }
                 if (Cust == null || Cust.Text == "")
                 {
                     if (dgvAccountList.Columns[e.ColumnIndex].Name.ToString() == "CustID" || dgvAccountList.Columns[e.ColumnIndex].Name.ToString() == "CustName")
                     {
                         Cust = new PopUp.CustomerID.Customer();
                         Cust.GetData(dgvAccountList.Rows[e.RowIndex].Cells["CustID"].Value.ToString());
                         Cust.Show();
                     }
                 }
                 else if (CheckOpened(Cust.Name))
                 {
                     Cust.WindowState = FormWindowState.Normal;
                     Cust.GetData(dgvAccountList.Rows[e.RowIndex].Cells["CustID"].Value.ToString());
                     Cust.Show();
                     Cust.Focus();
                 }
            }
        }

        private void txtHIO_Leave(object sender, EventArgs e)
        {
            if (txtHIO.Text != "")
            {
                double d = double.Parse(txtHIO.Text);
                txtHIO.Text = d.ToString("N2");
            }
        }

       
        //end    
    }
}
