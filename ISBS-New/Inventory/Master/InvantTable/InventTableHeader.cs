using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.Master.InvantTable
{
    public partial class InventTableHeader : MetroFramework.Forms.MetroForm
    {
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;

        string UkuranSectionB = "";
        string Mode = "";
        string ori_itemid = "";

        string tempPlus = "";
        string[] tempCmb = new string[] { "", "", "", "", "", "", "", "" };

        InquiryInventTable Parent;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public InventTableHeader()
        {
            InitializeComponent();
        }

        #region Load
        private void InventTableHeader_Load(object sender, EventArgs e)
        {
            GlobalSetting();
            LoadAll();
            CheckUkuran();
            this.metroTabControl1.SelectedTab = metroTabPage1;
        }

        public void ModeNew()
        {
            Mode = "New";
            btnCancel.Enabled = false;
            btnEdit.Enabled = false;
            BeforeAddItemID();
            SetToZero();
        }        

        public void ModeBeforeEdit()
        {
            btnResize.Enabled = false; //hendry tambah resize
            Mode = "BeforeEdit";
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;

            //sectionGeneral
            cmbType.Enabled = false;
            cmbUoM.Enabled = false;
            cmbAlt.Enabled = false;
            chkGroupCheck.Enabled = false;
            chkSubGroup1.Enabled = false;
            chkSubGroup2.Enabled = false;
            btnSearchGroupID.Enabled = false;
            btnSearchTagSize.Enabled = false;
            //sectionA
            cmbManufacturer.Enabled = false;
            cmbMerek.Enabled = false;
            cmbGolongan.Enabled = false;
            cmbKodeBerat.Enabled = false;
            cmbSpec.Enabled = false;
            cmbBentuk.Enabled = false;
            //sectionB
            txtSatuan1.Enabled = false;
            txtSatuan2.Enabled = false;
            txtSatuan3.Enabled = false;
            txtSatuan4.Enabled = false;
            txtSatuan5.Enabled = false;
            txtUkuran1Value.Enabled = false;
            txtUkuran2Value.Enabled = false;
            txtUkuran3Value.Enabled = false;
            txtUkuran4Value.Enabled = false;
            txtUkuran5Value.Enabled = false;
            ChkUkuran1.Enabled = false;
            ChkUkuran2.Enabled = false;
            ChkUkuran3.Enabled = false;
            ChkUkuran4.Enabled = false;
            ChkUkuran5.Enabled = false;
            //sectionC
            cbxPlus1.Enabled = false;
            cbxPlus2.Enabled = false;
            cbxPlus3.Enabled = false;
            cbxPlus4.Enabled = false;
            cbxPlus5.Enabled = false;
            cbxPlus6.Enabled = false;
            cbxPlus7.Enabled = false;

            cmbPlus1.Enabled = false;
            cmbPlus2.Enabled = false;
            cmbPlus3.Enabled = false;
            cmbPlus4.Enabled = false;
            cmbPlus5.Enabled = false;
            cmbPlus6.Enabled = false;
            cmbPlus7.Enabled = false;

            //sectionD
            txtToleransiBerat.Enabled = false;
            txtRatioKonversiTabel.Enabled = false;
            cbxCertificate.Enabled = false;
            cbxResize.Enabled = false;
            //sectionE
            txtLeadTime.Enabled = false;
            txtMinQtyUoM.Enabled = false;
            txtMinQtyAlt.Enabled = false;
            txtReorderQtyUom.Enabled = false;
            txtReorderQtyAlt.Enabled = false;
            cbxRoutine.Enabled = false;
            btnVendor1.Enabled = false;
            btnVendor2.Enabled = false;
            btnVendor3.Enabled = false;

        }

        public void ModeEdit()
        {
            //hendry tambah resize
            if (cbxResize.Checked) { btnResize.Enabled = true; } else { btnResize.Enabled = false; }
            Mode = "Edit";
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnCancel.Enabled = true;
            btnExit.Enabled = false;

            //sectionGeneral
            cmbType.Enabled = false;
            cmbUoM.Enabled = false;
            cmbAlt.Enabled = false;
            chkGroupCheck.Enabled = false;
            chkSubGroup1.Enabled = false;
            chkSubGroup2.Enabled = false;
            btnSearchGroupID.Enabled = false;
            btnSearchTagSize.Enabled = false;
            //sectionA
            cmbManufacturer.Enabled = true;
            cmbMerek.Enabled = true;
            cmbGolongan.Enabled = true;
            cmbKodeBerat.Enabled = true;
            cmbSpec.Enabled = true;
            cmbBentuk.Enabled = true;
            //sectionB
            //sectionC
            cbxPlus1.Enabled = true;
            cbxPlus2.Enabled = true;
            cbxPlus3.Enabled = true;
            cbxPlus4.Enabled = true;
            cbxPlus5.Enabled = true;
            cbxPlus6.Enabled = true;
            cbxPlus7.Enabled = true;

            cmbPlus1.Enabled = true;
            cmbPlus2.Enabled = true;
            cmbPlus3.Enabled = true;
            cmbPlus4.Enabled = true;
            cmbPlus5.Enabled = true;
            cmbPlus6.Enabled = true;
            cmbPlus7.Enabled = true;
            //sectionD
            txtToleransiBerat.Enabled = true;
            txtRatioKonversiTabel.Enabled = true;
            cbxCertificate.Enabled = true;
            cbxResize.Enabled = true;
            //sectionE
            txtLeadTime.Enabled = true;
            txtMinQtyUoM.Enabled = true;
            //txtMinQtyAlt.Enabled = true; Hendry Comment
            txtReorderQtyUom.Enabled = true;
            //txtReorderQtyAlt.Enabled = true; hendry comment
            cbxRoutine.Enabled = true;
            btnVendor1.Enabled = true;
            btnVendor2.Enabled = true;
            btnVendor3.Enabled = true;

            CheckUkuran();
        }

        public void SetParent(InquiryInventTable F)
        {
            Parent = F;
        }

        private void SetToZero()
        {
            txtLeadTime.Text = Convert.ToString(0);
            txtMinQtyUoM.Text = Convert.ToString(0);
            txtMinQtyAlt.Text = Convert.ToString(0);
            txtReorderQtyUom.Text = Convert.ToString(0);
            txtReorderQtyAlt.Text = Convert.ToString(0);
            txtRatioKonversiTabel.Text = Convert.ToString(0);
            txtToleransiBerat.Text = Convert.ToString(0);

            txtRatioKonversiTabel.Text = Convert.ToString(0);
        }

        private void LoadAll()
        {
            Conn = ConnectionString.GetConnection();
            LoadGeneral();
            LoadSectionA();
            LoadSectionC();
            Conn.Close();
        }

        private void LoadGeneral()
        {
            Query = "SELECT [InventTypeID] FROM [dbo].[InventType]";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string InventTypeID = Dr["InventTypeID"].ToString();
                    cmbType.Items.Add(InventTypeID);
                }
                Dr.Close();
            }

            Query = "SELECT [UnitID] FROM [dbo].[InventUnit]";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string UnitID = Dr["UnitID"].ToString();
                    cmbUoM.Items.Add(UnitID);
                    cmbAlt.Items.Add(UnitID);
                }
                Dr.Close();
            }
        }

        private void LoadSectionA()
        {
            cmbManufacturer.Items.Add("");
            Query = "SELECT [ManufacturerID] FROM [dbo].[InventManufacturer]";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string ManufacturerID = Dr["ManufacturerID"].ToString();                    
                    cmbManufacturer.Items.Add(ManufacturerID);
                }
                Dr.Close();
            }

            cmbMerek.Items.Add("");
            Query = "SELECT [MerekID] FROM [dbo].[InventMerek]";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string MerekID = Dr["MerekID"].ToString();
                    cmbMerek.Items.Add(MerekID);
                }
                Dr.Close();
            }

            cmbGolongan.Items.Add("");
            Query = "SELECT [GolonganID] FROM [dbo].[InventGolongan]";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string GolonganID = Dr["GolonganID"].ToString();
                    cmbGolongan.Items.Add(GolonganID);
                }
                Dr.Close();
            }

            cmbKodeBerat.Items.Add("");
            Query = "SELECT [BeratId] FROM [dbo].[InventBerat]";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string BeratId = Dr["BeratId"].ToString();
                    cmbKodeBerat.Items.Add(BeratId);
                }
                Dr.Close();
            }

            cmbSpec.Items.Add("");
            Query = "SELECT [SpecID] FROM [dbo].[InventSpec]";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string SpecID = Dr["SpecID"].ToString();
                    cmbSpec.Items.Add(SpecID);
                }
                Dr.Close();
            }

            cmbBentuk.Items.Add("");
            Query = "SELECT Bentuk FROM InventBentuk";
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    string Bentuk = Dr["Bentuk"].ToString();
                    cmbBentuk.Items.Add(Bentuk);
                }
                Dr.Close();
            }
        }

        private void LoadSectionC()
        {
            for (int i = 0; i < tempCmb.Count(); i++)
            {
                tempCmb[i] = "";
            }
            List<string> pluses = new List<string>();
            pluses.Clear();
            cmbPlus1.Items.Clear();
            cmbPlus2.Items.Clear();
            cmbPlus3.Items.Clear();
            cmbPlus4.Items.Clear();
            cmbPlus5.Items.Clear();
            cmbPlus6.Items.Clear();
            cmbPlus7.Items.Clear();

            pluses.Add("");
            pluses.Add("MANUFACTURER");
            pluses.Add("MEREK");
            pluses.Add("GOLONGAN");
            pluses.Add("UKURAN");
            //if (!(UkuranSectionB == "" || UkuranSectionB == null))
            //{
            //    pluses.Add(UkuranSectionB);
            //}
            pluses.Add("KODE BERAT");
            pluses.Add("SPEC");
            pluses.Add("TAG SIZE");

            //foreach (string plus in pluses) // Display for verification.
            for (int i = 0; i < pluses.Count(); i++)
            {
                cmbPlus1.Items.Add(pluses[i]);
                cmbPlus2.Items.Add(pluses[i]);
                cmbPlus3.Items.Add(pluses[i]);
                cmbPlus4.Items.Add(pluses[i]);
                cmbPlus5.Items.Add(pluses[i]);
                cmbPlus6.Items.Add(pluses[i]);
                cmbPlus7.Items.Add(pluses[i]);
            }

            //cmbPlus1.SelectedItem = "MANUFACTURER";
            //cmbPlus1.Enabled = false;

            //cmbPlus2.SelectedItem = "MEREK";
            //cmbPlus2.Enabled = false;

            //cmbPlus3.SelectedItem = "GOLONGAN";
            //cmbPlus3.Enabled = false;

            //cmbPlus4.SelectedItem = UkuranSectionB;
            //cmbPlus4.Enabled = false;

            //cmbPlus5.SelectedItem = "KODE BERAT";
            //cmbPlus5.Enabled = false;

            //cmbPlus6.SelectedItem = "SPEC";
            //cmbPlus6.Enabled = false;
        }

        private void GlobalSetting()
        {
            btnResize.Enabled = false; //hendry tambah resize

            txtGroupID.ReadOnly = true;
            txtGroupDeskripsi.ReadOnly = true;
            txtSubGroup1ID.ReadOnly = true;
            txtSubGroup1Desk.ReadOnly = true;
            txtSubGroup2ID.ReadOnly = true;
            txtSubGroup2Desk.ReadOnly = true;
            txtItemID.ReadOnly = true;
            txtItemDeskripsi.ReadOnly = true;
            txtTagSizeID.ReadOnly = true;
            txtTagSizeDeskripsi.ReadOnly = true;
            txtHasilPlus.ReadOnly = true;
            txtFullItemID.ReadOnly = true;
            cbxPartOfGelombangPurchase.Enabled = false;
            cbxPartOfGelombangSales.Enabled = false;
            txtSatuan1.Enabled = false;
            txtSatuan2.Enabled = false;
            txtSatuan3.Enabled = false;
            txtSatuan4.Enabled = false;
            txtSatuan5.Enabled = false;
            txtVendorPreferenceID1.Enabled = false;
            txtVendorPreferenceID2.Enabled = false;
            txtVendorPreferenceID3.Enabled = false;
            txtVendorP1Desk.Enabled = false;
            txtVendorP2Desk.Enabled = false;
            txtVendorP3Desk.Enabled = false;
        }

        public void GetDataHeader(string FullItemID)
        {
            Conn = ConnectionString.GetConnection();

            Query = "SELECT * FROM [dbo].[InventTable] where FullItemID = '" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                //sectionGeneral
                txtFullItemID.Text = Dr["FullItemID"].ToString();
                ori_itemid = Dr["FullItemID"].ToString();
                txtGroupID.Text = Dr["GroupID"].ToString();
                txtGroupDeskripsi.Text = Dr["GroupDeskripsi"].ToString();
                chkGroupCheck.Checked = Dr["GroupDeskripsiDispC"] == null ? false : Convert.ToBoolean(Dr["GroupDeskripsiDispC"]);

                txtSubGroup1ID.Text = Dr["SubGroup1ID"].ToString();
                txtSubGroup1Desk.Text = Dr["SubGroup1Deskripsi"].ToString();
                chkSubGroup1.Checked = Dr["SubGroup1DeskripsiDispC"] == null ? false : Convert.ToBoolean(Dr["SubGroup1DeskripsiDispC"]);

                txtSubGroup2ID.Text = Dr["SubGroup2ID"].ToString();
                txtSubGroup2Desk.Text = Dr["SubGroup2Deskripsi"].ToString();
                chkSubGroup2.Checked = Dr["SubGroup2DeskripsiDispC"] == null ? false : Convert.ToBoolean(Dr["SubGroup2DeskripsiDispC"]);

                txtItemID.Text = Dr["ItemID"].ToString();

                txtTagSizeID.Text = Dr["TagSizeID"].ToString();
                txtTagSizeDeskripsi.Text = Dr["TagSizeDeskripsi"].ToString();

                cmbType.SelectedItem = Dr["InventTypeID"].ToString();
                cmbUoM.SelectedItem = Dr["UoM"].ToString();
                cmbAlt.SelectedItem = Dr["UoMAlt"].ToString();

                //sectionA
                if (Dr["ManufacturerID"].ToString() == "")
                {
                    cmbManufacturer.SelectedIndex = -1;
                }
                else
                {
                    cmbManufacturer.SelectedItem = Dr["ManufacturerID"].ToString();
                }

                if (Dr["MerekID"].ToString() == "")
                {
                    cmbMerek.SelectedIndex = -1;
                }
                else
                {
                    cmbMerek.SelectedItem = Dr["MerekID"].ToString();
                }

                if (Dr["GolonganID"].ToString() == "")
                {
                    cmbGolongan.SelectedIndex = -1;
                }
                else
                {
                    cmbGolongan.SelectedItem = Dr["GolonganID"].ToString();
                }

                if (Dr["KodeBerat"].ToString() == "")
                {
                    cmbKodeBerat.SelectedIndex = -1;
                }
                else
                {
                    cmbKodeBerat.SelectedItem = Dr["KodeBerat"].ToString();
                }

                if (Dr["SpecID"].ToString() == "")
                {
                    cmbSpec.SelectedIndex = -1;
                }
                else
                {
                    cmbSpec.SelectedItem = Dr["SpecID"].ToString();
                }

                if (Dr["Bentuk"].ToString() == "")
                {
                    cmbBentuk.SelectedIndex = -1;
                }
                else
                {
                    cmbBentuk.SelectedItem = Dr["Bentuk"].ToString();
                }

                //cmbManufacturer.SelectedItem = Dr["ManufacturerID"].ToString();
                //cmbMerek.SelectedItem = Dr["MerekID"].ToString();
                //cmbGolongan.SelectedItem = Dr["GolonganID"].ToString();
                //cmbKodeBerat.SelectedItem = Dr["KodeBerat"].ToString();
                //cmbSpec.SelectedItem = Dr["SpecID"].ToString();

                //sectionB
                txtSatuan1.Text = Dr["Ukuran1"].ToString();
                decimal Value1 = (decimal)Dr["Ukuran1Value"];
                txtUkuran1Value.Text = Value1.ToString("0.##");
                txtUkuran1MeasurementID.Text = Dr["Ukuran1MeasurementID"].ToString();
                ChkUkuran1.Checked = bool.Parse(Dr["Ukuran1Chk"].ToString());

                txtSatuan2.Text = Dr["Ukuran2"].ToString();
                decimal Value2 = (decimal)Dr["Ukuran2Value"];
                txtUkuran2Value.Text = Value2.ToString("0.##");
                txtUkuran2MeasurementID.Text = Dr["Ukuran2MeasurementID"].ToString();
                ChkUkuran2.Checked = bool.Parse(Dr["Ukuran2Chk"].ToString());

                txtSatuan3.Text = Dr["Ukuran3"].ToString();
                decimal Value3 = (decimal)Dr["Ukuran3Value"];
                txtUkuran3Value.Text = Value3.ToString("0.##");
                txtUkuran3MeasurementID.Text = Dr["Ukuran3MeasurementID"].ToString();
                ChkUkuran3.Checked = bool.Parse(Dr["Ukuran3Chk"].ToString());

                txtSatuan4.Text = Dr["Ukuran4"].ToString();
                decimal Value4 = (decimal)Dr["Ukuran4Value"];
                txtUkuran4Value.Text = Value4.ToString("0.##");
                txtUkuran4MeasurementID.Text = Dr["Ukuran4MeasurementID"].ToString();
                ChkUkuran4.Checked = bool.Parse(Dr["Ukuran4Chk"].ToString());

                txtSatuan5.Text = Dr["Ukuran5"].ToString();
                decimal Value5 = (decimal)Dr["Ukuran5Value"];
                txtUkuran5Value.Text = Value5.ToString("0.##");
                txtUkuran5MeasurementID.Text = Dr["Ukuran5MeasurementID"].ToString();
                ChkUkuran5.Checked = bool.Parse(Dr["Ukuran5Chk"].ToString());

                CreateFullItemId();

                //sectionC
                LoadSectionC();
                //txtItemDeskripsi.Text = Dr["ItemDeskripsi"].ToString();
                CreateFullItemId();
                //if (Dr["plus1"].ToString() == "MANUFACTURER")
                //{
                //    cbxPlus1.Checked = true;                  
                //}
                //if (Dr["plus2"].ToString() == "MEREK")
                //{
                //    cbxPlus2.Checked = true;
                //}
                //if (Dr["plus3"].ToString() == "GOLONGAN")
                //{
                //    cbxPlus3.Checked = true;
                //}
                //if (Dr["plus4"].ToString() != "")
                //{
                //    cbxPlus4.Checked = true;
                //}
                //if (Dr["plus5"].ToString() == "KODE BERAT")
                //{
                //    cbxPlus5.Checked = true;
                //}
                //if (Dr["plus6"].ToString() == "SPEC")
                //{
                //    cbxPlus6.Checked = true;
                //}

                cmbPlus1.SelectedItem = Dr["plus1"].ToString();
                cmbPlus2.SelectedItem = Dr["plus2"].ToString();
                cmbPlus3.SelectedItem = Dr["plus3"].ToString();
                cmbPlus4.SelectedItem = Dr["plus4"].ToString();
                cmbPlus5.SelectedItem = Dr["plus5"].ToString();
                cmbPlus6.SelectedItem = Dr["plus6"].ToString();
                cmbPlus7.SelectedItem = Dr["plus6"].ToString();

                if (Dr["plus1"].ToString() != "")
                {
                    cbxPlus1.Checked = true;
                }
                if (Dr["plus2"].ToString() != "")
                {
                    cbxPlus2.Checked = true;
                }
                if (Dr["plus3"].ToString() != "")
                {
                    cbxPlus3.Checked = true;
                }
                if (Dr["plus4"].ToString() != "")
                {
                    cbxPlus4.Checked = true;
                }
                if (Dr["plus5"].ToString() != "")
                {
                    cbxPlus5.Checked = true;
                }
                if (Dr["plus6"].ToString() != "")
                {
                    cbxPlus6.Checked = true;
                }
                if (Dr["plus7"].ToString() != "")
                {
                    cbxPlus7.Checked = true;
                }

                txtHasilPlus.Text = Dr["FullItemDeskripsi"].ToString();

                //sectionD
                txtToleransiBerat.Text = Dr["Toleransi_Berat"].ToString();
                cbxCertificate.Checked = Dr["Certificate"] == null ? false : Convert.ToBoolean(Dr["Certificate"]);
                cbxResize.Checked = Dr["Resize"] == null ? false : Convert.ToBoolean(Dr["Resize"]);
                txtRatioKonversiTabel.Text = Dr["Ratio"].ToString();



                //sectionE
                txtLeadTime.Text = Dr["LeadTime"].ToString();
                txtMinQtyUoM.Text = Dr["Min_Qty_UoM"].ToString();
                txtMinQtyAlt.Text = Dr["Min_Qty_Alt"].ToString();
                txtReorderQtyUom.Text = Dr["ReorderUoMQty"].ToString();
                txtReorderQtyAlt.Text = Dr["ReorderUoMAltQty"].ToString();
                cbxRoutine.Checked = Dr["Routine"] == null ? false : Convert.ToBoolean(Dr["Routine"]);

                txtVendorPreferenceID1.Text = Dr["VendorPreferenceID1"].ToString();
                txtVendorPreferenceID2.Text = Dr["VendorPreferenceID2"].ToString();
                txtVendorPreferenceID3.Text = Dr["VendorPreferenceID3"].ToString();

                //sectionZ error
                cbxPartOfGelombangPurchase.Checked = Dr["Part_of_Gelombang_Purchase"] == null ? false : Convert.ToBoolean(Dr["Part_of_Gelombang_Purchase"]);
                cbxPartOfGelombangSales.Checked = Dr["Part_of_Gelombang_Sales"] == null ? false : Convert.ToBoolean(Dr["Part_of_Gelombang_Sales"]);

            }
            Dr.Close();
        }


        #endregion Load

        #region CheckBox / ComboBox Changed
        private void chkGroupCheck_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
        }

        private void chkSubGroup1_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
        }

        private void chkSubGroup2_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
        }

        private void cbxPlus1_CheckedChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cbxPlus2_CheckedChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cbxPlus3_CheckedChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cbxPlus4_CheckedChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cbxPlus5_CheckedChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cbxPlus6_CheckedChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cbxPlus7_CheckedChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtItemDeskripsi_TextChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void ChkUkuran1_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void ChkUkuran2_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void ChkUkuran3_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void ChkUkuran4_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void ChkUkuran5_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void txtUkuran1Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void txtUkuran2Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void txtUkuran3Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void txtUkuran4Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void txtUkuran5Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
            FunctionChangePlus();
            //LoadSectionC();
        }

        private void cmbUoM_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblUoMMin.Text = cmbUoM.Text;
            lblUoMReorder.Text = cmbUoM.Text;
        }

        private void cmbAlt_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblAltMin.Text = cmbAlt.Text;
            lblAltReorder.Text = cmbAlt.Text;
        }

        private void txtVendorPreferenceID1_TextChanged(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT VendName FROM [dbo].[VendTable] WHERE VendId = '" + txtVendorPreferenceID1.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            var VendName = Cmd.ExecuteScalar();
            if (VendName != null)
            {
                txtVendorP1Desk.Text = VendName.ToString();
            }
            Conn.Close();
        }

        private void txtVendorPreferenceID2_TextChanged(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT VendName FROM [dbo].[VendTable] WHERE VendId = '" + txtVendorPreferenceID2.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            var VendName = Cmd.ExecuteScalar();
            if (VendName != null)
            {
                txtVendorP2Desk.Text = VendName.ToString();
            }
            Conn.Close();
        }

        private void txtVendorPreferenceID3_TextChanged(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT VendName FROM [dbo].[VendTable] WHERE VendId = '" + txtVendorPreferenceID3.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            var VendName = Cmd.ExecuteScalar();
            if (VendName != null)
            {
                txtVendorP3Desk.Text = VendName.ToString();
            }
            Conn.Close();
        }
        #endregion

        #region KeyPress + Leave
        private void txtUkuran1Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtUkuran1Value.Text = String.Format("{0:0.##}", txtUkuran1Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran1Value.Text));
            }
        }

        private void txtUkuran2Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtUkuran2Value.Text = String.Format("{0:0.##}", txtUkuran2Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran2Value.Text));
            }
        }

        private void txtUkuran3Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtUkuran3Value.Text = String.Format("{0:0.##}", txtUkuran3Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran3Value.Text));
            }
        }

        private void txtUkuran4Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtUkuran4Value.Text = String.Format("{0:0.##}", txtUkuran4Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran4Value.Text));
            }
        }

        private void txtUkuran5Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtUkuran5Value.Text = String.Format("{0:0.##}", txtUkuran5Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran5Value.Text));
            }
        }

        private void txtUkuran1Value_Leave(object sender, EventArgs e)
        {
            txtUkuran1Value.Text = String.Format("{0:0.##}", txtUkuran1Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran1Value.Text));
        }

        private void txtUkuran2Value_Leave(object sender, EventArgs e)
        {
            txtUkuran2Value.Text = String.Format("{0:0.##}", txtUkuran2Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran2Value.Text));
        }

        private void txtUkuran3Value_Leave(object sender, EventArgs e)
        {
            txtUkuran3Value.Text = String.Format("{0:0.##}", txtUkuran3Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran3Value.Text));
        }

        private void txtUkuran4Value_Leave(object sender, EventArgs e)
        {
            txtUkuran4Value.Text = String.Format("{0:0.##}", txtUkuran4Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran4Value.Text));
        }

        private void txtUkuran5Value_Leave(object sender, EventArgs e)
        {
            txtUkuran5Value.Text = String.Format("{0:0.##}", txtUkuran5Value.Text == "" ? 0 : Convert.ToDecimal(txtUkuran5Value.Text));
        }

        private void txtToleransiBerat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtToleransiBerat.Text = String.Format("{0:0.##}", txtToleransiBerat.Text == "" ? 0 : Convert.ToDecimal(txtToleransiBerat.Text));
            }
        }

        private void txtToleransiBerat_Leave(object sender, EventArgs e)
        {
            txtToleransiBerat.Text = String.Format("{0:0.##}", txtToleransiBerat.Text == "" ? 0 : Convert.ToDecimal(txtToleransiBerat.Text));
        }

        private void txtRatioKonversiTabel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtRatioKonversiTabel.Text = String.Format("{0:0.##}", txtRatioKonversiTabel.Text == "" ? 0 : Convert.ToDecimal(txtRatioKonversiTabel.Text));
            }
        }

        private void txtRatioKonversiTabel_Leave(object sender, EventArgs e)
        {
            txtRatioKonversiTabel.Text = String.Format("{0:0.##}", txtRatioKonversiTabel.Text == "" ? 0 : Convert.ToDecimal(txtRatioKonversiTabel.Text));

        }

        private void txtMinQtyUoM_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtMinQtyUoM.Text = String.Format("{0:0.##}", txtMinQtyUoM.Text == "" ? 0 : Convert.ToDecimal(txtMinQtyUoM.Text));
            }
        }

        private void txtMinQtyAlt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtMinQtyAlt.Text = String.Format("{0:0.##}", txtMinQtyAlt.Text == "" ? 0 : Convert.ToDecimal(txtMinQtyAlt.Text));
            }
        }

        private void txtReorderQtyUom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtReorderQtyUom.Text = String.Format("{0:0.##}", txtReorderQtyUom.Text == "" ? 0 : Convert.ToDecimal(txtReorderQtyUom.Text));
            }
        }

        private void txtReorderQtyAlt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                txtReorderQtyAlt.Text = String.Format("{0:0.##}", txtReorderQtyAlt.Text == "" ? 0 : Convert.ToDecimal(txtReorderQtyAlt.Text));
            }
        }

        private void txtMinQtyUoM_Leave(object sender, EventArgs e)
        {
            txtMinQtyUoM.Text = String.Format("{0:0.##}", txtMinQtyUoM.Text == "" ? 0 : Convert.ToDecimal(txtMinQtyUoM.Text));

        }

        private void txtMinQtyAlt_Leave(object sender, EventArgs e)
        {
            txtMinQtyAlt.Text = String.Format("{0:0.##}", txtMinQtyAlt.Text == "" ? 0 : Convert.ToDecimal(txtMinQtyAlt.Text));
        }

        private void txtReorderQtyUom_Leave(object sender, EventArgs e)
        {
            txtReorderQtyUom.Text = String.Format("{0:0.##}", txtReorderQtyUom.Text == "" ? 0 : Convert.ToDecimal(txtReorderQtyUom.Text));
        }

        private void txtReorderQtyAlt_Leave(object sender, EventArgs e)
        {
            txtReorderQtyAlt.Text = String.Format("{0:0.##}", txtReorderQtyAlt.Text == "" ? 0 : Convert.ToDecimal(txtReorderQtyAlt.Text));
        }
        #endregion

        #region Function
        private void CreateFullItemId()
        {
            //if (Mode != "BeforeEdit")
            //{
            txtItemDeskripsi.Text = "";
            txtFullItemID.Text = "";
            UkuranSectionB = "";
            Boolean ukuran = false;

            if (txtGroupID.Text != "")
            {
                txtFullItemID.Text += txtGroupID.Text;
            }
            if (txtSubGroup1ID.Text != "")
            {
                txtFullItemID.Text += "." + txtSubGroup1ID.Text;
            }
            if (txtSubGroup2ID.Text != "")
            {
                txtFullItemID.Text += "." + txtSubGroup2ID.Text;
            }
            if (txtItemID.Text != "")
            {
                txtFullItemID.Text += "." + txtItemID.Text;

                if (txtTagSizeID.Text != "")
                {
                    txtFullItemID.Text += "." + txtTagSizeID.Text;
                }
            }
            if (chkGroupCheck.Checked == true)
            {
                txtItemDeskripsi.Text = txtGroupDeskripsi.Text + " ";
            }
            if (chkSubGroup1.Checked == true)
            {
                txtItemDeskripsi.Text += txtSubGroup1Desk.Text + " ";
            }
            if (chkSubGroup2.Checked == true)
            {
                txtItemDeskripsi.Text += txtSubGroup2Desk.Text + " ";
            }
            if (txtItemDeskripsi.Text != "")
            {
                txtItemDeskripsi.Text = txtItemDeskripsi.Text.Remove(txtItemDeskripsi.Text.Length - 1);
            }

            if (ChkUkuran1.Checked == true)
            {
                UkuranSectionB += txtUkuran1Value.Text + " " + txtUkuran1MeasurementID.Text;
                ukuran = true;
            }
            if (ChkUkuran2.Checked == true)
            {
                if (ukuran == true)
                    UkuranSectionB += " x ";

                UkuranSectionB += txtUkuran2Value.Text + " " + txtUkuran2MeasurementID.Text;
                ukuran = true;
            }
            if (ChkUkuran3.Checked == true)
            {
                if (ukuran == true)
                    UkuranSectionB += " x ";

                UkuranSectionB += txtUkuran3Value.Text + " " + txtUkuran2MeasurementID.Text;
                ukuran = true;
            }
            if (ChkUkuran4.Checked == true)
            {
                if (ukuran == true)
                    UkuranSectionB += " x ";

                UkuranSectionB += txtUkuran4Value.Text + " " + txtUkuran2MeasurementID.Text;
                ukuran = true;
            }
            if (ChkUkuran5.Checked == true)
            {
                if (ukuran == true)
                    UkuranSectionB += " x ";

                UkuranSectionB += txtUkuran5Value.Text + " " + txtUkuran2MeasurementID.Text;
                ukuran = true;
            }

            //}
        }

        private void CheckUkuran()
        {
            if (txtGroupID.Text.Trim() == "" || txtSubGroup1ID.Text.Trim() == "" || txtSubGroup2ID.Text.Trim() == "")
            {
                txtUkuran1Value.Text = "0";
                txtUkuran1MeasurementID.Text = "";
                txtUkuran2Value.Text = "0";
                txtUkuran2MeasurementID.Text = "";
                txtUkuran3Value.Text = "0";
                txtUkuran3MeasurementID.Text = "";
                txtUkuran4Value.Text = "0";
                txtUkuran4MeasurementID.Text = "";
                txtUkuran5Value.Text = "0";
                txtUkuran5MeasurementID.Text = "";

                txtUkuran1Value.Enabled = false;
                txtUkuran1MeasurementID.Enabled = false;
                ChkUkuran1.Enabled = false;
                txtUkuran2Value.Enabled = false;
                txtUkuran2MeasurementID.Enabled = false;
                ChkUkuran2.Enabled = false;
                txtUkuran3Value.Enabled = false;
                txtUkuran3MeasurementID.Enabled = false;
                ChkUkuran3.Enabled = false;
                txtUkuran4Value.Enabled = false;
                txtUkuran4MeasurementID.Enabled = false;
                ChkUkuran4.Enabled = false;
                txtUkuran5Value.Enabled = false;
                txtUkuran5MeasurementID.Enabled = false;
                ChkUkuran5.Enabled = false;
            }
            else
            {
                if (Mode != "Edit")
                {
                    txtUkuran1Value.Text = "0";
                    txtUkuran1MeasurementID.Text = "";
                    txtUkuran2Value.Text = "0";
                    txtUkuran2MeasurementID.Text = "";
                    txtUkuran3Value.Text = "0";
                    txtUkuran3MeasurementID.Text = "";
                    txtUkuran4Value.Text = "0";
                    txtUkuran4MeasurementID.Text = "";
                    txtUkuran5Value.Text = "0";
                    txtUkuran5MeasurementID.Text = "";
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Ukuran1,DimensionID1,Measurement1,Ukuran2,DimensionID2,Measurement2,Ukuran3,DimensionID3,Measurement3,Ukuran4,DimensionID4,Measurement4,Ukuran5,DimensionID5,Measurement5 From [dbo].[InventConfig] ";
                Query += "Where GroupId+SubGroup1Id+SubGroup2Id='" + (txtGroupID.Text.Trim() + txtSubGroup1ID.Text.Trim() + txtSubGroup2ID.Text.Trim()) + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {

                    //Ukuran1
                    if (Convert.ToBoolean(Dr["Ukuran1"]) == false)
                    {
                        txtUkuran1Value.Enabled = false;
                        ChkUkuran1.Enabled = false;
                    }
                    else
                    {
                        txtUkuran1Value.Enabled = true;
                        ChkUkuran1.Enabled = true;
                        //txtUkuran1MeasurementID.Text = Dr["DimensionID1"].ToString();
                        //txtSatuan1.Text = Dr["Measurement1"].ToString();

                        txtUkuran1MeasurementID.Text = Dr["Measurement1"].ToString();
                        txtSatuan1.Text = Dr["DimensionID1"].ToString();
                    }

                    //Ukuran2
                    if (Convert.ToBoolean(Dr["Ukuran2"]) == false)
                    {
                        txtUkuran2Value.Enabled = false;
                        ChkUkuran2.Enabled = false;
                    }
                    else
                    {
                        txtUkuran2Value.Enabled = true;
                        ChkUkuran2.Enabled = true;
                        txtUkuran2MeasurementID.Text = Dr["Measurement2"].ToString();
                        txtSatuan2.Text = Dr["DimensionID2"].ToString();
                    }

                    //Ukuran3
                    if (Convert.ToBoolean(Dr["Ukuran3"]) == false)
                    {
                        txtUkuran3Value.Enabled = false;
                        ChkUkuran3.Enabled = false;
                    }
                    else
                    {
                        txtUkuran3Value.Enabled = true;
                        ChkUkuran3.Enabled = true;
                        txtUkuran3MeasurementID.Text = Dr["Measurement3"].ToString();
                        txtSatuan3.Text = Dr["DimensionID3"].ToString();
                    }

                    //Ukuran4
                    if (Convert.ToBoolean(Dr["Ukuran4"]) == false)
                    {
                        txtUkuran4Value.Enabled = false;
                        ChkUkuran4.Enabled = false;
                    }
                    else
                    {
                        txtUkuran4Value.Enabled = true;
                        ChkUkuran4.Enabled = true;
                        txtUkuran4MeasurementID.Text = Dr["Measurement4"].ToString();
                        txtSatuan4.Text = Dr["DimensionID4"].ToString();
                    }

                    //Ukuran5
                    if (Convert.ToBoolean(Dr["Ukuran5"]) == false)
                    {
                        txtUkuran5Value.Enabled = false;
                        ChkUkuran5.Enabled = false;
                    }
                    else
                    {
                        txtUkuran5Value.Enabled = true;
                        ChkUkuran5.Enabled = true;
                        txtUkuran5MeasurementID.Text = Dr["Measurement5"].ToString();
                        txtSatuan5.Text = Dr["DimensionID5"].ToString();
                    }
                }
                Conn.Close();
            }
        }

        private void FunctionChangePlus()
        {
            txtHasilPlus.Text = "";
            txtHasilPlus.Text += txtItemDeskripsi.Text;
            if (cbxPlus1.Checked == true)
            {
                if (CheckChangePlus(cmbPlus1.Text) != "")
                    txtHasilPlus.Text += " " + CheckChangePlus(cmbPlus1.Text);
            }
            if (cbxPlus2.Checked == true)
            {
                if (CheckChangePlus(cmbPlus2.Text) != "")
                    txtHasilPlus.Text += " " + CheckChangePlus(cmbPlus2.Text);
            }
            if (cbxPlus3.Checked == true)
            {
                if (CheckChangePlus(cmbPlus3.Text) != "")
                    txtHasilPlus.Text += " " + CheckChangePlus(cmbPlus3.Text);
            }
            if (cbxPlus4.Checked == true)
            {
                if (CheckChangePlus(cmbPlus4.Text) != "")
                    txtHasilPlus.Text += " " + CheckChangePlus(cmbPlus4.Text);
            }
            if (cbxPlus5.Checked == true)
            {
                if (CheckChangePlus(cmbPlus5.Text) != "")
                    txtHasilPlus.Text += " " + CheckChangePlus(cmbPlus5.Text);
            }
            if (cbxPlus6.Checked == true)
            {
                if (CheckChangePlus(cmbPlus6.Text) != "")
                    txtHasilPlus.Text += " " + CheckChangePlus(cmbPlus6.Text);
            }
            if (cbxPlus7.Checked == true)
            {
                if (CheckChangePlus(cmbPlus7.Text) != "")
                    txtHasilPlus.Text += " " + CheckChangePlus(cmbPlus7.Text);
            }
        }

        private string CheckChangePlus(string Plus)
        {
            if (Plus == "MANUFACTURER")
                return cmbManufacturer.Text;
            else if (Plus == "MEREK")
                return cmbMerek.Text;
            else if (Plus == "GOLONGAN")
                return cmbGolongan.Text;
            else if (Plus == "KODE BERAT")
                return cmbKodeBerat.Text;
            else if (Plus == "SPEC")
                return cmbSpec.Text;
            else if (Plus == "UKURAN")
                return UkuranSectionB;
            else if (Plus == "TAG SIZE")
                return txtTagSizeDeskripsi.Text;
            //else if (Plus == UkuranSectionB)
            //    return UkuranSectionB;
            else
                return "";
        }

        private Boolean ValidasiUkuran()
        {
            if (txtUkuran1Value.Enabled == true && txtUkuran1Value.Text.Trim() == "0")
            {
                MessageBox.Show("Ukuran1Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran2Value.Enabled == true && txtUkuran2Value.Text.Trim() == "0")
            {
                MessageBox.Show("Ukuran2Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran3Value.Enabled == true && txtUkuran3Value.Text.Trim() == "0")
            {
                MessageBox.Show("Ukuran3Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran4Value.Enabled == true && txtUkuran4Value.Text.Trim() == "0")
            {
                MessageBox.Show("Ukuran4Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran5Value.Enabled == true && txtUkuran5Value.Text.Trim() == "0")
            {
                MessageBox.Show("Ukuran5Value tidak boleh kosong.");
                return false;
            }
            return true;
        }
        #endregion Function

        #region Vendor Search
        private void btnVendor1_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "VendTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();

            txtVendorPreferenceID1.Text = ConnectionString.Kode;
            txtVendorP1Desk.Text = ConnectionString.Kode2;
        }

        private void btnVendor2_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "VendTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();

            txtVendorPreferenceID2.Text = ConnectionString.Kode;
            txtVendorP2Desk.Text = ConnectionString.Kode2;
        }

        private void btnVendor3_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "VendTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();

            txtVendorPreferenceID3.Text = ConnectionString.Kode;
            txtVendorP3Desk.Text = ConnectionString.Kode2;
        }
        #endregion Vendor Search

        private void btnSearchGroupID_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "GroupID";
            tmpSearch.Order = "GroupID Asc";
            tmpSearch.Table = "[dbo].[InventSubGroup2]";
            tmpSearch.QuerySearch = "SELECT a.[GroupID],b.[Deskripsi] AS GroupDesk,a.[SubGroup1ID], c.[Deskripsi] AS Sub1Desk,a.[SubGroup2ID], a.[Deskripsi] AS Sub2Desk,a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM [dbo].[InventSubGroup2] a ";
            tmpSearch.QuerySearch += "INNER JOIN [InventGroup] b ON a.GroupID = b.GroupID ";
            tmpSearch.QuerySearch += "INNER JOIN [InventSubGroup1] c ON a.SubGroup1ID = c.SubGroup1ID ";
            tmpSearch.FilterText = new string[] { "GroupID", "GroupDesk", "SubGroup1ID","Sub1Desk","SubGroup2ID", "Sub2Desk", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            tmpSearch.Mask = new string[] { "GroupID", "Group", "SubGroup1ID", "Sub Group", "SubGroup2ID", "Sub Group 2", "Created Date", "Created By", "Updated Date", "Updated By" };
            tmpSearch.Hide = new string[] { "GroupID", "SubGroup1ID", "SubGroup2ID" };
            tmpSearch.Select = new string[] { "GroupID", "GroupDesk", "SubGroup1ID", "Sub1Desk", "SubGroup2ID", "Sub2Desk" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtGroupID.Text = ConnectionString.Kodes[0];
                txtGroupDeskripsi.Text = ConnectionString.Kodes[1];
                txtSubGroup1ID.Text = ConnectionString.Kodes[2];
                txtSubGroup1Desk.Text = ConnectionString.Kodes[3];
                txtSubGroup2ID.Text = ConnectionString.Kodes[4];
                txtSubGroup2Desk.Text = ConnectionString.Kodes[5];      
                ConnectionString.Kodes = null;
                
                if (CheckItemConfig() == false)
                {
                    return;
                }
                else
                {
                    CreateFullItemId();
                    CheckUkuran();
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
            GetDataHeader(txtFullItemID.Text.Trim());
            ModeBeforeEdit();
        }

        private void InventTableHeader_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Hendry tambah jika ada inquiry yg aktif
            Form Inquiry = Application.OpenForms["InquiryInventTable"];
            if (Inquiry != null)
                Parent.RefreshGrid();
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            #region Validasi

            if (txtFullItemID.Text.Trim() == "")
            {
                ErrMsg = "Data FullItemID tidak boleh kosong..";
                vBol = false;
            }
            else if (txtTagSizeID.Text=="")
            {
                ErrMsg = "Tag Size tidak boleh kosong." + Environment.NewLine + "Pilih Tag Size pada bagian atas..";
                vBol = false;
            }
            else if (txtItemDeskripsi.Text.Trim() == "")
            {
                metroTabControl1.SelectedIndex = 2;
                ErrMsg = "Item Deskripsi tidak boleh kosong." + Environment.NewLine + "Centang Disp.C pada bagian atas untuk menambah Item Deskripsi..";
                vBol = false;
            }
            else if (cmbType.SelectedIndex == -1)
            {
                ErrMsg = "Type tidak boleh kosong..";
                vBol = false;
            }
            else if (cmbUoM.SelectedIndex == -1 || cmbAlt.SelectedIndex == -1)
            {
                ErrMsg = "Satuan UoM dan Alt tidak boleh kosong..";
                vBol = false;
            }
            else if (ValidasiUkuran() == false)
            {
                metroTabControl1.SelectedIndex = 1;
                vBol = false;
            }
            else if (txtRatioKonversiTabel.Text == "0")
            {
                metroTabControl1.SelectedIndex = 3;
                ErrMsg = "Konversi Ratio Tabel tidak boleh kosong..";
                vBol = false;
            }

            if (vBol == true && Mode == "Edit")
            {
                try
                {
                    Query = "Select * From InventTable Where FullItemID=@txtFullItemID ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtFullItemID", txtFullItemID.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Item tidak ditemukan..";
                            vBol = false;
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Conn.Close();
                }
            }
            #endregion Validasi

            if (vBol == false && ErrMsg!="") { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cekValidasi() == false)
            {
                return;
            }

            try
            {                
                Conn = ConnectionString.GetConnection();

                if (Mode == "New")
                {
                    #region Make New Full Item ID
                    string NewGroupSub1Sub2 = txtGroupID.Text.Trim() + "." + txtSubGroup1ID.Text.Trim() + "." + txtSubGroup2ID.Text.Trim() + ".";

                    string QueryTemp = "SELECT RIGHT('00000'+ISNULL(CAST((Max(Right(ItemID,5)+1)) AS VARCHAR(5)),1),5) from InventTable Where FullItemID like '%" + NewGroupSub1Sub2 + "%'";
                    Cmd = new SqlCommand(QueryTemp, Conn);
                    string NewItemID = Cmd.ExecuteScalar().ToString();
                    string NewFullItemID = NewGroupSub1Sub2 + NewItemID.ToString();
                    if (txtTagSizeID.Text != "")
                    {
                        NewFullItemID += "." + txtTagSizeID.Text;
                    }
                    txtFullItemID.Text = NewFullItemID;
                    txtItemID.Text = NewItemID;
                    #endregion

                    #region Query Insert to InventTable

                    Query = "INSERT INTO [dbo].[InventTable] (";

                    //sectionGeneral
                    Query += "[FullItemID],[ItemID]";
                    Query += ",[GroupID],[GroupDeskripsi],[GroupDeskripsiDispC]";
                    Query += ",[SubGroup1ID],[SubGroup1Deskripsi],[SubGroup1DeskripsiDispC]";
                    Query += ",[SubGroup2ID],[SubGroup2Deskripsi],[SubGroup2DeskripsiDispC]";
                    Query += ",[TagSizeID],[TagSizeDeskripsi]";
                    Query += ",[UoM],[UoMAlt],[InventTypeID]";

                    //sectionA
                    Query += ",[ManufacturerID],[MerekID],[GolonganID],[KodeBerat],[SpecID],[Bentuk]";

                    //sectionB
                    Query += ",[Ukuran1Value],[Ukuran1MeasurementID],[Ukuran1Chk],[Ukuran1],[Ukuran1C]";
                    Query += ",[Ukuran2Value],[Ukuran2MeasurementID],[Ukuran2Chk],[Ukuran2],[Ukuran2C]";
                    Query += ",[Ukuran3Value],[Ukuran3MeasurementID],[Ukuran3Chk],[Ukuran3],[Ukuran3C]";
                    Query += ",[Ukuran4Value],[Ukuran4MeasurementID],[Ukuran4Chk],[Ukuran4],[Ukuran4C]";
                    Query += ",[Ukuran5Value],[Ukuran5MeasurementID],[Ukuran5Chk],[Ukuran5],[Ukuran5C]";

                    //sectionC
                    Query += ",[ItemDeskripsi],[FullItemDeskripsi]";
                    Query += ",[plus1],[plus2],[plus3],[plus4],[plus5],[plus6],[plus7]";

                    //sectionD
                    Query += ",[Toleransi_Berat],[Certificate],[Resize],[Ratio]";

                    //sectionE
                    Query += ",[LeadTime],[Min_Qty_UoM],[Min_Qty_Alt],[ReorderUoMQty],[ReorderUoMAltQty],[Routine]";
                    Query += ",[VendorPreferenceID1],[VendorPreferenceID2],[VendorPreferenceID3]";

                    //Other
                    Query += ",[CreatedDate],[CreatedBy]) VALUES (";

                    //=============VALUE====================
                    //sectionGeneral
                    Query += "'" + NewFullItemID + "', '" + NewItemID + "'";
                    Query += ",'" + txtGroupID.Text.Trim().ToUpper() + "','" + txtGroupDeskripsi.Text.Trim().ToUpper() + "','" + chkGroupCheck.Checked.ToString() + "'";
                    Query += ",'" + txtSubGroup1ID.Text.Trim().ToUpper() + "','" + txtSubGroup1Desk.Text.Trim().ToUpper() + "','" + chkSubGroup1.Checked.ToString() + "'";
                    Query += ",'" + txtSubGroup2ID.Text.Trim().ToUpper() + "','" + txtSubGroup2Desk.Text.Trim().ToUpper() + "','" + chkSubGroup2.Checked.ToString() + "'";
                    Query += ",'" + txtTagSizeID.Text.Trim().ToUpper() + "','" + txtTagSizeDeskripsi.Text.Trim().ToUpper() + "'";
                    Query += ",'" + cmbUoM.Text.Trim().ToUpper() + "','" + cmbAlt.Text.Trim().ToUpper() + "','" + cmbType.Text.Trim().ToUpper() + "'";

                    //sectionA
                    Query += ",'" + cmbManufacturer.Text.Trim().ToUpper() + "','" + cmbMerek.Text.Trim().ToUpper() + "','" + cmbGolongan.Text.Trim().ToUpper() + "', '" + cmbKodeBerat.Text.Trim().ToUpper() + "', '" + cmbSpec.Text.Trim().ToUpper() + "', '" + cmbBentuk.Text.Trim().ToUpper() + "'";

                    //sectionB
                    Query += ",'" + txtUkuran1Value.Text.Trim().ToUpper() + "','" + txtUkuran1MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran1.Checked.ToString() + "','" + txtSatuan1.Text.Trim().ToUpper() + "','" + ChkUkuran1.Checked.ToString() + "'";
                    Query += ",'" + txtUkuran2Value.Text.Trim().ToUpper() + "','" + txtUkuran2MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran2.Checked.ToString() + "','" + txtSatuan2.Text.Trim().ToUpper() + "','" + ChkUkuran2.Checked.ToString() + "'";
                    Query += ",'" + txtUkuran3Value.Text.Trim().ToUpper() + "','" + txtUkuran3MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran3.Checked.ToString() + "','" + txtSatuan3.Text.Trim().ToUpper() + "','" + ChkUkuran3.Checked.ToString() + "'";
                    Query += ",'" + txtUkuran4Value.Text.Trim().ToUpper() + "','" + txtUkuran4MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran4.Checked.ToString() + "','" + txtSatuan4.Text.Trim().ToUpper() + "','" + ChkUkuran4.Checked.ToString() + "'";
                    Query += ",'" + txtUkuran5Value.Text.Trim().ToUpper() + "','" + txtUkuran5MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran5.Checked.ToString() + "','" + txtSatuan5.Text.Trim().ToUpper() + "','" + ChkUkuran5.Checked.ToString() + "'";
                    //sectionC
                    //Query += ",'" + txtItemDeskripsi.Text.Trim().ToUpper() + "','" + txtHasilPlus.Text.Trim().ToUpper() + "'";
                    Query += ",'" + txtHasilPlus.Text.Trim().ToUpper() + "','" + txtHasilPlus.Text.Trim().ToUpper() + "'";

                    if (cbxPlus1.Checked)
                    {
                        Query += ",'" + cmbPlus1.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus1.Checked)
                    {
                        Query += ",''";
                    }

                    if (cbxPlus2.Checked)
                    {
                        Query += ",'" + cmbPlus2.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus2.Checked)
                    {
                        Query += ",''";
                    }

                    if (cbxPlus3.Checked)
                    {
                        Query += ",'" + cmbPlus3.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus3.Checked)
                    {
                        Query += ",''";
                    }

                    if (cbxPlus4.Checked)
                    {
                        Query += ",'" + cmbPlus4.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus4.Checked)
                    {
                        Query += ",''";
                    }

                    if (cbxPlus5.Checked)
                    {
                        Query += ",'" + cmbPlus5.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus5.Checked)
                    {
                        Query += ",''";
                    }

                    if (cbxPlus6.Checked)
                    {
                        Query += ",'" + cmbPlus6.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus6.Checked)
                    {
                        Query += ",''";
                    }

                    if (cbxPlus7.Checked)
                    {
                        Query += ",'" + cmbPlus7.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus7.Checked)
                    {
                        Query += ",''";
                    }

                    //sectionD
                    Query += ",'" + txtToleransiBerat.Text.Trim().ToUpper() + "', '" + cbxCertificate.Checked.ToString() + "', '" + cbxResize.Checked.ToString() + "', '" + txtRatioKonversiTabel.Text.Trim().ToUpper() + "'";

                    //sectionE
                    Query += ",@txtLeadTime,'" + txtMinQtyUoM.Text.Trim().ToUpper() + "', '" + txtMinQtyAlt.Text.Trim().ToUpper() + "', '" + txtReorderQtyUom.Text.Trim().ToUpper() + "', '" + txtReorderQtyAlt.Text.Trim().ToUpper() + "', '" + cbxRoutine.Checked.ToString() + "'";
                    Query += ",'" + txtVendorPreferenceID1.Text.Trim().ToUpper() + "','" + txtVendorPreferenceID2.Text.Trim().ToUpper() + "','" + txtVendorPreferenceID3.Text.Trim().ToUpper() + "'";

                    //Other
                    Query += ",getdate(),'" + ControlMgr.UserId.ToUpper() + "')";
                    #endregion InventTable
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.AddWithValue("@txtLeadTime", txtLeadTime.Text.Trim().ToUpper());
                    Cmd.ExecuteNonQuery();

                    #region Other than InventTable
                    string strSql = "INSERT INTO InventConversion(FullItemID,ItemDeskripsi,FromUnit,ToUnit,Ratio,CreatedBy) ";
                    strSql += "VALUES('" + txtFullItemID.Text + "','" + txtHasilPlus.Text + "','" + cmbUoM.Text.Trim().ToUpper() + "','" + cmbAlt.Text.Trim().ToUpper() + "','" + txtRatioKonversiTabel.Text.Trim().ToUpper() + "','" + ControlMgr.UserId + "')";
                    using (SqlCommand Cmd1 = new SqlCommand(strSql, Conn))
                    {
                        Cmd1.ExecuteNonQuery();
                    }
                                        
                    strSql = "INSERT INTO Invent_Purchase_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtHasilPlus.Text + "')";
                    using (SqlCommand Cmd1 = new SqlCommand(strSql, Conn))
                    {
                        Cmd1.ExecuteNonQuery();
                    }

                    strSql = "INSERT INTO Invent_Sales_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtHasilPlus.Text + "')";
                    using (SqlCommand Cmd1 = new SqlCommand(strSql, Conn))
                    {
                        Cmd1.ExecuteNonQuery();
                    }

                    strSql = "INSERT INTO Invent_Movement_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtHasilPlus.Text + "')";
                    using (SqlCommand Cmd1 = new SqlCommand(strSql, Conn))
                    {
                        Cmd1.ExecuteNonQuery();
                    }

                    strSql = "INSERT INTO Invent_OnHand_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtHasilPlus.Text + "')";
                    using (SqlCommand Cmd1 = new SqlCommand(strSql, Conn))
                    {
                        Cmd1.ExecuteNonQuery();
                    }             
                    #endregion

                    MessageBox.Show("FullItemID = " + NewFullItemID + Environment.NewLine + "Item Deskripsi = " + txtItemDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                    ModeBeforeEdit();
                }

                if (Mode == "Edit")
                {
                    #region Query Update InventTable
                    Query = "UPDATE [dbo].[InventTable] SET ";

                    //PS General cannot be edited
                    //sectionA
                    Query += "[ManufacturerID] = '" + cmbManufacturer.Text.Trim().ToUpper() + "'";
                    Query += ",[MerekID] = '" + cmbMerek.Text.Trim().ToUpper() + "'";
                    Query += ",[GolonganID] = '" + cmbGolongan.Text.Trim().ToUpper() + "'";
                    Query += ",[KodeBerat] = '" + cmbKodeBerat.Text.Trim().ToUpper() + "'";
                    Query += ",[SpecID] = '" + cmbSpec.Text.Trim().ToUpper() + "'";
                    Query += ",[Bentuk] = '" + cmbBentuk.Text.Trim().ToUpper() + "'";

                    //sectionB
                    Query += ",[Ukuran1Value] = '" + txtUkuran1Value.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran1MeasurementID] = '" + txtUkuran1MeasurementID.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran1Chk] = '" + ChkUkuran1.Checked.ToString() + "'";
                    Query += ",[Ukuran1] = '" + txtSatuan1.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran1C] = '" + ChkUkuran1.Checked.ToString() + "'";

                    Query += ",[Ukuran2Value] = '" + txtUkuran2Value.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran2MeasurementID] = '" + txtUkuran2MeasurementID.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran2Chk] = '" + ChkUkuran2.Checked.ToString() + "'";
                    Query += ",[Ukuran2] = '" + txtSatuan2.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran2C] = '" + ChkUkuran2.Checked.ToString() + "'";

                    Query += ",[Ukuran3Value] = '" + txtUkuran3Value.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran3MeasurementID] = '" + txtUkuran3MeasurementID.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran3Chk] = '" + ChkUkuran3.Checked.ToString() + "'";
                    Query += ",[Ukuran3] = '" + txtSatuan3.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran3C] = '" + ChkUkuran3.Checked.ToString() + "'";

                    Query += ",[Ukuran4Value] = '" + txtUkuran4Value.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran4MeasurementID] = '" + txtUkuran4MeasurementID.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran4Chk] = '" + ChkUkuran4.Checked.ToString() + "'";
                    Query += ",[Ukuran4] = '" + txtSatuan4.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran4C] = '" + ChkUkuran4.Checked.ToString() + "'";

                    Query += ",[Ukuran5Value] = '" + txtUkuran5Value.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran5MeasurementID] = '" + txtUkuran5MeasurementID.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran5Chk] = '" + ChkUkuran5.Checked.ToString() + "'";
                    Query += ",[Ukuran5] = '" + txtSatuan4.Text.Trim().ToUpper() + "'";
                    Query += ",[Ukuran5C] = '" + ChkUkuran5.Checked.ToString() + "'";  

                    //sectionC
                    Query += ",[ItemDeskripsi] = '" + txtHasilPlus.Text.Trim().ToUpper() + "'";
                    Query += ",[FullItemDeskripsi] = '" + txtHasilPlus.Text.Trim().ToUpper() + "'";

                    if (cbxPlus1.Checked)
                    {
                        Query += ",[plus1] = '" + cmbPlus1.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus1.Checked)
                    {
                        Query += ",[plus1] = ''";
                    }

                    if (cbxPlus2.Checked)
                    {
                        Query += ",[plus2] = '" + cmbPlus2.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus2.Checked)
                    {
                        Query += ",[plus2] = ''";
                    }

                    if (cbxPlus3.Checked)
                    {
                        Query += ",[plus3] = '" + cmbPlus3.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus3.Checked)
                    {
                        Query += ",[plus3] = ''";
                    }

                    if (cbxPlus4.Checked)
                    {
                        Query += ",[plus4] = '" + cmbPlus4.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus4.Checked)
                    {
                        Query += ",[plus4] = ''";
                    }

                    if (cbxPlus5.Checked)
                    {
                        Query += ",[plus5] = '" + cmbPlus5.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus5.Checked)
                    {
                        Query += ",[plus5] = ''";
                    }

                    if (cbxPlus6.Checked)
                    {
                        Query += ",[plus6] = '" + cmbPlus6.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus6.Checked)
                    {
                        Query += ",[plus6] = ''";
                    }

                    if (cbxPlus7.Checked)
                    {
                        Query += ",[plus7] = '" + cmbPlus7.Text.Trim().ToUpper() + "'";
                    }
                    if (!cbxPlus7.Checked)
                    {
                        Query += ",[plus7] = ''";
                    }

                    //sectionD
                    Query += ",[Toleransi_Berat] = '" + txtToleransiBerat.Text.Trim().ToUpper() + "'";
                    Query += ",[Certificate] =  '" + cbxCertificate.Checked.ToString() + "'";
                    Query += ",[Resize] = '" + cbxResize.Checked.ToString() + "'";
                    Query += ",[Ratio] = '" + txtRatioKonversiTabel.Text.Trim().ToUpper() + "'";

                    //sectionE
                    Query += ",[LeadTime] = @txtLeadTime ";
                    Query += ",[Min_Qty_UoM] = '" + txtMinQtyUoM.Text.Trim().ToUpper() + "'";
                    Query += ",[Min_Qty_Alt] = '" + txtMinQtyAlt.Text.Trim().ToUpper() + "'";
                    Query += ",[ReorderUoMQty] = '" + txtReorderQtyUom.Text.Trim().ToUpper() + "'";
                    Query += ",[ReorderUoMAltQty] = '" + txtReorderQtyAlt.Text.Trim().ToUpper() + "'";
                    Query += ",[Routine] = '" + cbxRoutine.Checked.ToString() + "'";

                    Query += ",[VendorPreferenceID1] = '" + txtVendorPreferenceID1.Text.Trim().ToUpper() + "'";
                    Query += ",[VendorPreferenceID2] = '" + txtVendorPreferenceID2.Text.Trim().ToUpper() + "'";
                    Query += ",[VendorPreferenceID3] = '" + txtVendorPreferenceID3.Text.Trim().ToUpper() + "'";

                    //Other
                    Query += ",[UpdatedDate] = getdate()";
                    Query += ",[UpdatedBy] = '" + ControlMgr.UserId.ToUpper() + "'";
                    Query += "WHERE FullItemId = '" + txtFullItemID.Text + "'";
                    #endregion
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.AddWithValue("@txtLeadTime", txtLeadTime.Text.Trim().ToUpper());
                    Cmd.ExecuteNonQuery();

                    #region Other than InventTable
                    Query = "UPDATE InventConversion SET ";
                    Query += "ItemDeskripsi='" + txtHasilPlus.Text.Trim().ToUpper() + "',";
                    Query += "Ratio='" + txtRatioKonversiTabel.Text.Trim().ToUpper() + "',";
                    Query += "UpdatedBy='" + ControlMgr.UserId.ToUpper() + "' ";
                    Query += "WHERE FullItemID='" + txtFullItemID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_Purchase_Qty SET ";
                    Query += "ItemName='" + txtHasilPlus.Text.Trim().ToUpper() + "' ";
                    Query += "WHERE FullItemID='" + txtFullItemID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_Sales_Qty SET ";
                    Query += "ItemName='" + txtHasilPlus.Text.Trim().ToUpper() + "' ";
                    Query += "WHERE FullItemID='" + txtFullItemID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_Movement_Qty SET ";
                    Query += "ItemName='" + txtHasilPlus.Text.Trim().ToUpper() + "' ";
                    Query += "WHERE FullItemID='" + txtFullItemID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    Query = "UPDATE Invent_OnHand_Qty SET ";
                    Query += "ItemName='" + txtHasilPlus.Text.Trim().ToUpper() + "' ";
                    Query += "WHERE FullItemID='" + txtFullItemID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    #endregion

                    MessageBox.Show("FullItemID = " + txtFullItemID.Text + Environment.NewLine + "Item Deskripsi = " + txtItemDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
                    ModeBeforeEdit();
                }
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
                //Hendry tambah jika ada inquiry yg aktif
                Form Inquiry = Application.OpenForms["InquiryInventTable"];
                if (Inquiry != null)
                    Parent.RefreshGrid();
            }
        }


        private void cmbPlus1_Enter(object sender, EventArgs e)
        {
            //if (cmbPlus1.SelectedIndex <= 0)
            //    return;
            //if (cmbPlus1.SelectedIndex >= 1)
            //    if (cmbPlus1.Text != "" || cmbPlus1.Text != null)
            //    {
            //        tempPlus = cmbPlus1.SelectedItem.ToString();
            //    }
            if (cmbPlus1.Text != "" && cmbPlus1.Text != null)
            {
                tempPlus = cmbPlus1.SelectedItem.ToString();
            }
        }

        private void cmbPlus2_Enter(object sender, EventArgs e)
        {
            //if (cmbPlus2.SelectedIndex <= 0)
            //    return;
            //if (cmbPlus2.SelectedIndex >= 1)
            if (cmbPlus2.Text != "" && cmbPlus2.Text != null)
            {
                tempPlus = cmbPlus2.SelectedItem.ToString();
            }
        }

        private void cmbPlus3_Enter(object sender, EventArgs e)
        {
            if (cmbPlus3.Text != "" && cmbPlus3.Text != null)
            {
                tempPlus = cmbPlus3.SelectedItem.ToString();
            }
        }

        private void cmbPlus4_Enter(object sender, EventArgs e)
        {
            if (cmbPlus4.Text != "" && cmbPlus4.Text != null)
            {
                tempPlus = cmbPlus4.SelectedItem.ToString();
            }
        }

        private void cmbPlus5_Enter(object sender, EventArgs e)
        {
            if (cmbPlus5.Text != "" && cmbPlus5.Text != null)
            {
                tempPlus = cmbPlus5.SelectedItem.ToString();
            }
        }

        private void cmbPlus6_Enter(object sender, EventArgs e)
        {
            if (cmbPlus6.Text != "" && cmbPlus6.Text != null)
            {
                tempPlus = cmbPlus6.SelectedItem.ToString();
            }
        }

        private void cmbPlus7_Enter(object sender, EventArgs e)
        {
            if (cmbPlus7.Text != "" && cmbPlus7.Text != null)
            {
                tempPlus = cmbPlus7.SelectedItem.ToString();
            }
        }

        private void cmbPlus1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();

            string PlusSelect = cmbPlus1.SelectedItem.ToString();
            if (PlusSelect != tempCmb[1])
            {
                cmbPlus2.Items.Remove(PlusSelect);
                cmbPlus3.Items.Remove(PlusSelect);
                cmbPlus4.Items.Remove(PlusSelect);
                cmbPlus5.Items.Remove(PlusSelect);
                cmbPlus6.Items.Remove(PlusSelect);
                cmbPlus7.Items.Remove(PlusSelect);

                if (tempCmb[1] != "")
                {
                    cmbPlus2.Items.Add(tempCmb[1]);
                    cmbPlus3.Items.Add(tempCmb[1]);
                    cmbPlus4.Items.Add(tempCmb[1]);
                    cmbPlus5.Items.Add(tempCmb[1]);
                    cmbPlus6.Items.Add(tempCmb[1]);
                    cmbPlus7.Items.Add(tempCmb[1]);
                }
            }
            tempCmb[1] = cmbPlus1.SelectedItem.ToString(); ;
        }




        private void cmbPlus2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
            string PlusSelect = cmbPlus2.SelectedItem.ToString();
            if (PlusSelect != tempCmb[2])
            {
                cmbPlus1.Items.Remove(PlusSelect);
                cmbPlus3.Items.Remove(PlusSelect);
                cmbPlus4.Items.Remove(PlusSelect);
                cmbPlus5.Items.Remove(PlusSelect);
                cmbPlus6.Items.Remove(PlusSelect);
                cmbPlus7.Items.Remove(PlusSelect);

                if (tempCmb[2] != "")
                {
                    cmbPlus1.Items.Add(tempCmb[2]);
                    cmbPlus3.Items.Add(tempCmb[2]);
                    cmbPlus4.Items.Add(tempCmb[2]);
                    cmbPlus5.Items.Add(tempCmb[2]);
                    cmbPlus6.Items.Add(tempCmb[2]);
                    cmbPlus7.Items.Add(tempCmb[2]);
                }
            }
            tempCmb[2] = cmbPlus2.SelectedItem.ToString(); ;
        }

        private void cmbPlus3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
            string PlusSelect = cmbPlus3.SelectedItem.ToString();
            if (PlusSelect != tempCmb[3])
            {
                cmbPlus1.Items.Remove(PlusSelect);
                cmbPlus2.Items.Remove(PlusSelect);
                cmbPlus4.Items.Remove(PlusSelect);
                cmbPlus5.Items.Remove(PlusSelect);
                cmbPlus6.Items.Remove(PlusSelect);
                cmbPlus7.Items.Remove(PlusSelect);

                if (tempCmb[3] != "")
                {
                    cmbPlus1.Items.Add(tempCmb[3]);
                    cmbPlus2.Items.Add(tempCmb[3]);
                    cmbPlus4.Items.Add(tempCmb[3]);
                    cmbPlus5.Items.Add(tempCmb[3]);
                    cmbPlus6.Items.Add(tempCmb[3]);
                    cmbPlus7.Items.Add(tempCmb[3]);
                }
            }
            tempCmb[3] = cmbPlus3.SelectedItem.ToString(); ;
        }

        private void cmbPlus4_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();

            string PlusSelect = cmbPlus4.SelectedItem.ToString();
            if (PlusSelect != tempCmb[4])
            {
                cmbPlus2.Items.Remove(PlusSelect);
                cmbPlus3.Items.Remove(PlusSelect);
                cmbPlus1.Items.Remove(PlusSelect);
                cmbPlus5.Items.Remove(PlusSelect);
                cmbPlus6.Items.Remove(PlusSelect);
                cmbPlus7.Items.Remove(PlusSelect);

                if (tempCmb[4] != "")
                {
                    cmbPlus2.Items.Add(tempCmb[4]);
                    cmbPlus3.Items.Add(tempCmb[4]);
                    cmbPlus1.Items.Add(tempCmb[4]);
                    cmbPlus5.Items.Add(tempCmb[4]);
                    cmbPlus6.Items.Add(tempCmb[4]);
                    cmbPlus7.Items.Add(tempCmb[4]);
                }
            }
            tempCmb[4] = cmbPlus4.SelectedItem.ToString(); ;
        }

        private void cmbPlus5_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();

            string PlusSelect = cmbPlus5.SelectedItem.ToString();
            if (PlusSelect != tempCmb[5])
            {
                cmbPlus2.Items.Remove(PlusSelect);
                cmbPlus3.Items.Remove(PlusSelect);
                cmbPlus4.Items.Remove(PlusSelect);
                cmbPlus1.Items.Remove(PlusSelect);
                cmbPlus6.Items.Remove(PlusSelect);
                cmbPlus7.Items.Remove(PlusSelect);

                if (tempCmb[5] != "")
                {
                    cmbPlus2.Items.Add(tempCmb[5]);
                    cmbPlus3.Items.Add(tempCmb[5]);
                    cmbPlus4.Items.Add(tempCmb[5]);
                    cmbPlus1.Items.Add(tempCmb[5]);
                    cmbPlus7.Items.Add(tempCmb[5]);
                }
            }
            tempCmb[5] = cmbPlus5.SelectedItem.ToString(); ;
        }

        private void cmbPlus6_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();

            string PlusSelect = cmbPlus6.SelectedItem.ToString();
            if (PlusSelect != tempCmb[6])
            {
                cmbPlus2.Items.Remove(PlusSelect);
                cmbPlus3.Items.Remove(PlusSelect);
                cmbPlus4.Items.Remove(PlusSelect);
                cmbPlus5.Items.Remove(PlusSelect);
                cmbPlus1.Items.Remove(PlusSelect);
                cmbPlus7.Items.Remove(PlusSelect);

                if (tempCmb[6] != "")
                {
                    cmbPlus2.Items.Add(tempCmb[6]);
                    cmbPlus3.Items.Add(tempCmb[6]);
                    cmbPlus4.Items.Add(tempCmb[6]);
                    cmbPlus5.Items.Add(tempCmb[6]);
                    cmbPlus1.Items.Add(tempCmb[6]);
                    cmbPlus7.Items.Add(tempCmb[6]);
                }
            }
            tempCmb[6] = cmbPlus6.SelectedItem.ToString(); ;
        }

        private void cmbPlus7_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();

            string PlusSelect = cmbPlus7.SelectedItem.ToString();
            if (PlusSelect != tempCmb[7])
            {
                cmbPlus2.Items.Remove(PlusSelect);
                cmbPlus3.Items.Remove(PlusSelect);
                cmbPlus4.Items.Remove(PlusSelect);
                cmbPlus5.Items.Remove(PlusSelect);
                cmbPlus1.Items.Remove(PlusSelect);
                cmbPlus6.Items.Remove(PlusSelect);

                if (tempCmb[7] != "")
                {
                    cmbPlus2.Items.Add(tempCmb[6]);
                    cmbPlus3.Items.Add(tempCmb[6]);
                    cmbPlus4.Items.Add(tempCmb[6]);
                    cmbPlus5.Items.Add(tempCmb[6]);
                    cmbPlus1.Items.Add(tempCmb[6]);
                    cmbPlus6.Items.Add(tempCmb[6]);
                }
            }
            tempCmb[7] = cmbPlus7.SelectedItem.ToString(); ;
        }

        private void btnSearchTagSize_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "TagSize";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();

            if (ConnectionString.Kode == "")
                return;

            txtTagSizeID.Text = ConnectionString.Kode;
            txtTagSizeDeskripsi.Text = ConnectionString.Kode2;

            CreateFullItemId();
        }

        private Boolean CheckItemConfig()
        {
            string QueryIConfig = "SELECT * FROM [InventConfig] Where GroupID = '" + txtGroupID.Text + "' AND [SubGroup1ID] = '" + txtSubGroup1ID.Text + "' AND [SubGroup2ID] = '" + txtSubGroup2ID.Text + "'";
            using (Cmd = new SqlCommand(QueryIConfig, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    AfterAddItemID();
                    return true;                    
                }
                else
                {
                    BeforeAddItemID();
                    Reset();
                    MessageBox.Show("Item belum terdaftar di Item Config");
                    return false;
                }
            }
        }

        private void BeforeAddItemID()
        {
            //sectionGeneral
            cmbType.Enabled = false;
            cmbUoM.Enabled = false;
            cmbAlt.Enabled = false;
            chkGroupCheck.Enabled = false;
            chkSubGroup1.Enabled = false;
            chkSubGroup2.Enabled = false;
            btnSearchGroupID.Enabled = true;
            btnSearchTagSize.Enabled = false;
            //sectionA
            cmbManufacturer.Enabled = false;
            cmbMerek.Enabled = false;
            cmbGolongan.Enabled = false;
            cmbKodeBerat.Enabled = false;
            cmbSpec.Enabled = false;
            cmbBentuk.Enabled = false;
            //sectionB
            txtSatuan1.Enabled = false;
            txtSatuan2.Enabled = false;
            txtSatuan3.Enabled = false;
            txtSatuan4.Enabled = false;
            txtSatuan5.Enabled = false;
            txtUkuran1Value.Enabled = false;
            txtUkuran2Value.Enabled = false;
            txtUkuran3Value.Enabled = false;
            txtUkuran4Value.Enabled = false;
            txtUkuran5Value.Enabled = false;
            ChkUkuran1.Enabled = false;
            ChkUkuran2.Enabled = false;
            ChkUkuran3.Enabled = false;
            ChkUkuran4.Enabled = false;
            ChkUkuran5.Enabled = false;
            //sectionC
            cbxPlus1.Enabled = false;
            cbxPlus2.Enabled = false;
            cbxPlus3.Enabled = false;
            cbxPlus4.Enabled = false;
            cbxPlus5.Enabled = false;
            cbxPlus6.Enabled = false;
            cbxPlus7.Enabled = false;

            cmbPlus1.Enabled = false;
            cmbPlus2.Enabled = false;
            cmbPlus3.Enabled = false;
            cmbPlus4.Enabled = false;
            cmbPlus5.Enabled = false;
            cmbPlus6.Enabled = false;
            cmbPlus7.Enabled = false;

            //sectionD
            txtToleransiBerat.Enabled = false;
            txtRatioKonversiTabel.Enabled = false;
            cbxCertificate.Enabled = false;
            cbxResize.Enabled = false;
            //sectionE
            txtLeadTime.Enabled = false;
            txtMinQtyUoM.Enabled = false;
            txtMinQtyAlt.Enabled = false;
            txtReorderQtyUom.Enabled = false;
            txtReorderQtyAlt.Enabled = false; 
            cbxRoutine.Enabled = false;
            btnVendor1.Enabled = false;
            btnVendor2.Enabled = false;
            btnVendor3.Enabled = false;
        }

        private void AfterAddItemID()
        {
            //sectionGeneral
            cmbType.Enabled = true;
            cmbUoM.Enabled = true;
            cmbAlt.Enabled = true;
            chkGroupCheck.Enabled = true;
            chkSubGroup1.Enabled = true;
            chkSubGroup2.Enabled = true;
            btnSearchGroupID.Enabled = true;
            btnSearchTagSize.Enabled = true;
            //sectionA
            cmbManufacturer.Enabled = true;
            cmbMerek.Enabled = true;
            cmbGolongan.Enabled = true;
            cmbKodeBerat.Enabled = true;
            cmbSpec.Enabled = true;
            cmbBentuk.Enabled = true;
            //sectionB
            //txtSatuan1.Enabled = true;
            //txtSatuan2.Enabled = true;
            //txtSatuan3.Enabled = true;
            //txtSatuan4.Enabled = true;
            //txtSatuan5.Enabled = true;
            txtUkuran1Value.Enabled = true;
            txtUkuran2Value.Enabled = true;
            txtUkuran3Value.Enabled = true;
            txtUkuran4Value.Enabled = true;
            txtUkuran5Value.Enabled = true;
            ChkUkuran1.Enabled = true;
            ChkUkuran2.Enabled = true;
            ChkUkuran3.Enabled = true;
            ChkUkuran4.Enabled = true;
            ChkUkuran5.Enabled = true;
            //sectionC
            cbxPlus1.Enabled = true;
            cbxPlus2.Enabled = true;
            cbxPlus3.Enabled = true;
            cbxPlus4.Enabled = true;
            cbxPlus5.Enabled = true;
            cbxPlus6.Enabled = true;
            cbxPlus7.Enabled = true;

            cmbPlus1.Enabled = true;
            cmbPlus2.Enabled = true;
            cmbPlus3.Enabled = true;
            cmbPlus4.Enabled = true;
            cmbPlus5.Enabled = true;
            cmbPlus6.Enabled = true;
            cmbPlus7.Enabled = true;

            //sectionD
            txtToleransiBerat.Enabled = true;
            txtRatioKonversiTabel.Enabled = true;
            cbxCertificate.Enabled = true;
            cbxResize.Enabled = true;
            //sectionE
            txtLeadTime.Enabled = true;
            txtMinQtyUoM.Enabled = true;
            //txtMinQtyAlt.Enabled = true; Hendry Comment
            txtReorderQtyUom.Enabled = true;
            //txtReorderQtyAlt.Enabled = true; Hendry Comment
            cbxRoutine.Enabled = true;
            btnVendor1.Enabled = true;
            btnVendor2.Enabled = true;
            btnVendor3.Enabled = true;
        }

        private void Reset()
        {
            //sectionGeneral
            cmbType.SelectedIndex = -1;
            cmbUoM.SelectedIndex = -1;
            cmbAlt.SelectedIndex = -1;
            chkGroupCheck.Checked = false;
            chkSubGroup1.Checked = false;
            chkSubGroup2.Checked = false;
            //sectionA
            cmbManufacturer.SelectedIndex = -1;
            cmbMerek.SelectedIndex = -1;
            cmbGolongan.SelectedIndex = -1;
            cmbKodeBerat.SelectedIndex = -1;
            cmbSpec.SelectedIndex = -1;
            cmbBentuk.SelectedIndex = -1;
            //sectionB
            txtSatuan1.Text = "";
            txtSatuan2.Text = "";
            txtSatuan3.Text = "";
            txtSatuan4.Text = "";
            txtSatuan5.Text = "";
            txtUkuran1Value.Text = "";
            txtUkuran2Value.Text = "";
            txtUkuran3Value.Text = "";
            txtUkuran4Value.Text = "";
            txtUkuran5Value.Text = "";
            ChkUkuran1.Checked = false;
            ChkUkuran2.Checked = false;
            ChkUkuran3.Checked = false;
            ChkUkuran4.Checked = false;
            ChkUkuran5.Checked = false;
            //sectionC
            cbxPlus1.Checked = false;
            cbxPlus2.Checked = false;
            cbxPlus3.Checked = false;
            cbxPlus4.Checked = false;
            cbxPlus5.Checked = false;
            cbxPlus6.Checked = false;
            cbxPlus7.Checked = false;

            cmbPlus1.SelectedIndex = -1;
            cmbPlus2.SelectedIndex = -1;
            cmbPlus3.SelectedIndex = -1;
            cmbPlus4.SelectedIndex = -1;
            cmbPlus5.SelectedIndex = -1;
            cmbPlus6.SelectedIndex = -1;
            cmbPlus7.SelectedIndex = -1;

            //sectionD
            txtToleransiBerat.Text = "";
            txtRatioKonversiTabel.Text = "0.00";
            cbxCertificate.Checked = false;
            cbxResize.Checked = false;
            //sectionE
            txtLeadTime.Text = "";
            txtMinQtyUoM.Text = "0.00";
            txtMinQtyAlt.Text = "0.00";
            txtReorderQtyUom.Text = "0.00";
            txtReorderQtyAlt.Text = "0.00";        
            cbxRoutine.Checked = false;
        }

        //Hendry Tambah
        private void txtMinQtyUoM_TextChanged(object sender, EventArgs e)
        {
            //hasim edit
            if (txtReorderQtyUom.Text != "")
            {
                if (txtMinQtyUoM.Text != "")
                {
                    txtMinQtyAlt.Text = Convert.ToString(Convert.ToDecimal(txtMinQtyUoM.Text) * Convert.ToDecimal(txtRatioKonversiTabel.Text));
                }
            }
        }

        private void txtReorderQtyUom_TextChanged(object sender, EventArgs e)
        {
            if (txtReorderQtyUom.Text != "")
            {
                txtReorderQtyAlt.Text = Convert.ToString(Convert.ToDecimal(txtReorderQtyUom.Text) * Convert.ToDecimal(txtRatioKonversiTabel.Text));
            }
        }

        private void txtRatioKonversiTabel_TextChanged(object sender, EventArgs e)
        {
            if (txtRatioKonversiTabel.Text != "")
            {
                txtMinQtyAlt.Text = Convert.ToString(Convert.ToDecimal(txtMinQtyUoM.Text) * Convert.ToDecimal(txtRatioKonversiTabel.Text));
                txtReorderQtyAlt.Text = Convert.ToString(Convert.ToDecimal(txtReorderQtyUom.Text) * Convert.ToDecimal(txtRatioKonversiTabel.Text));
            }
        }
     
        private void btnResize_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                MessageBox.Show("Simpan Item terlebih dahulu..");
            }
            else
            {
                InvantTable.FrmM_ResizeMapping f = new InvantTable.FrmM_ResizeMapping();
                f.ItemID = ori_itemid;
                f.ItemName = txtHasilPlus.Text;
                f.ShowDialog();
            }
        }

        private void cbxResize_Click(object sender, EventArgs e)
        {
            if (cbxResize.Checked)
                btnResize.Enabled = true;
            else btnResize.Enabled = false;
        }
    }
}
