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
    public partial class FormInventTable : MetroFramework.Forms.MetroForm
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
        private int Index;

        bool FlVendorP1 = false, FlPlus1 = false, FlTotal1 = false, FlAverage1 = false, FlRoutine1 = false, FlUkuran1 = false, FlPacking1 = false, FlSubResize = false;
        int Left = 0, Right = 0;

        InquiryInventTable Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public FormInventTable()
        {
            InitializeComponent();
        }

        private void FormInventTable_Load(object sender, EventArgs e)
        {
            AddCmbPlus();
            lblForm.Location = new Point(16, 11);
            CollapseAll();
        }

        //Mode
        string Mode = "";

        public void ModeNew()
        {
            Mode = "New";

            btnSearchGroupID.Enabled = true;
            btnSearchSubGroup1.Enabled = true;
            btnSearchSubGroup2.Enabled = true;
            btnInventTypeId.Enabled = true;
            btnManufacturerId.Enabled = true;
            btnMerekId.Enabled = true;
            btnGolonganId.Enabled = true;
            btnQualityId.Enabled = true;
            btnSpecId.Enabled = true;

            txtFullItemID.Enabled = false;
            txtItemDeskripsi.Enabled = false;
            txtGroupID.Enabled = false;
            txtGroupDeskripsi.Enabled = false;
            chkGroupCheck.Enabled = true;
            txtSubGroup1ID.Enabled = false;
            txtSubGroup1Desk.Enabled = false;
            chkSubGroup1.Enabled = true;
            txtSubGroup2ID.Enabled = false;
            txtSubGroup2Desk.Enabled = false;
            chkSubGroup2.Enabled = true;
            txtItemID.Enabled = false;
            txtInventTypeID.Enabled = false;
            txtManufacturerID.Enabled = false;
            txtMerekID.Enabled = false;
            txtGolonganID.Enabled = false;
            txtQualityID.Enabled = false;
            txtSpecID.Enabled = false;
            

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

            txtPackingListUoM.Enabled = false;
            txtPackingListUoMRatio.Enabled = false;
            txtUoMQty.Enabled = false;
            txtUoMAltQty.Enabled = false;
            txtUoMAlt1Qty.Enabled = false;
            txtUoMAlt2Qty.Enabled = false;
            txtToleransiAlt2Value.Enabled = false;
            txtUoMAlt3Qty.Enabled = false;
            txtToleransiAlt3Value.Enabled = false;
            txtUoMAlt4Qty.Enabled = false;
            txtToleransiAlt4Value.Enabled = false;
            txtUoMAltExtQty.Enabled = false;
            txtToleransiExtValue.Enabled = false;
            txtKursOriginal.Enabled = false;
            txtKursDasar.Enabled = false;

            txtUoMNilaiTotal.Enabled = false;
            txtUoMAltNilaiTotal.Enabled = false;
            txtUoMNilaiRataRata.Enabled = false;
            txtUoMAltRataRata.Enabled = false;
            txtMinimumUoMQtyManual.Enabled = true;
            txtMinimumUoMAltQtyManual.Enabled = true;
            txtMinimumUoMQtyPredictive.Enabled = true;
            txtMinimumUoMAltQtyPredictive.Enabled = true;

            txtUoM.Enabled = false;
            txtUoMAlt.Enabled = false;
            txtUoMAlt1.Enabled = true;
            txtUoMAlt1Conversion.Enabled = true;
            chkUoMAlt1.Enabled = true;
            txtUoMAlt2.Enabled = true;
            chkUoMAlt2.Enabled = true;
            txtToleransiAlt2.Enabled = true;
            txtUoMAlt3.Enabled = true;
            txtToleransiAlt3.Enabled = true;
            chkUoMAlt3.Enabled = true;
            txtUoMAlt4.Enabled = true;
            txtToleransiAlt4.Enabled = true;
            chkUoMAlt4.Enabled = true;
            txtUoMAltExt.Enabled = true;
            chkUoMAltExt.Enabled = true;
            txtToleransiExt.Enabled = true;
            txtBentuk.Enabled = true;

            txtVendorPreferenceID1.Enabled = true;
            txtVendorPreferenceID2.Enabled = true;
            txtVendorPreferenceID3.Enabled = true;

            cmbPlus1.Enabled = true;
            cmbPlus2.Enabled = true;
            cmbPlus3.Enabled = true;
            cmbPlus4.Enabled = true;
            cmbPlus5.Enabled = true;
            cmbPlus6.Enabled = true;

            txtTotalUoMQtyIn.Enabled = false;
            txtTotalUoMQtyOut.Enabled = false;
            txtTotalUoMAltQtyIn.Enabled = false;
            txtTotalUoMAltQtyOut.Enabled = false;
            txtTotalUoMNilaiIn.Enabled = false;
            txtTotalUoMNilaiQtyOut.Enabled = false;
            txtTotalUoMAltNilaiIn.Enabled = false;
            txtTotalUoMAltNilaiOut.Enabled = false;

            txtAverageBuyUoMQty.Enabled = false;
            txtAverageBuyUoMAltQty.Enabled = false;
            txtAverageBuyUoMNilai.Enabled = false;
            txtAverageBuyUoMAltNilai.Enabled = false;
            txtAverageSalesUoMQty.Enabled = false;
            txtAverageSalesUoMNilai.Enabled = false;
            txtAverageSalesUoMAltQty.Enabled = false;
            txtAverageSalesUoMAltNilai.Enabled = false;

            cmbRoutineNonRoutine.Enabled = true;
            cmbFastNSlowNmedium.Enabled = true;
            txtLeadTime.Enabled = true;
            txtReorderUoMQty.Enabled = true;
            txtReorderUoMAltQty.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;

            CheckUkuran();
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSearchGroupID.Enabled = false;
            btnSearchSubGroup1.Enabled = false;
            btnSearchSubGroup2.Enabled = false;
            btnInventTypeId.Enabled = true;
            btnManufacturerId.Enabled = true;
            btnMerekId.Enabled = true;
            btnGolonganId.Enabled = true;
            btnQualityId.Enabled = true;
            btnSpecId.Enabled = true;

            txtFullItemID.Enabled = false;
            txtItemDeskripsi.Enabled = false;
            txtGroupID.Enabled = false;
            txtGroupDeskripsi.Enabled = false;
            chkGroupCheck.Enabled = true;
            txtSubGroup1ID.Enabled = false;
            txtSubGroup1Desk.Enabled = false;
            chkSubGroup1.Enabled = true;
            txtSubGroup2ID.Enabled = false;
            txtSubGroup2Desk.Enabled = false;
            chkSubGroup2.Enabled = true;
            txtItemID.Enabled = false;
            txtInventTypeID.Enabled = false;
            txtManufacturerID.Enabled = false;
            txtMerekID.Enabled = false;
            txtGolonganID.Enabled = false;
            txtQualityID.Enabled = false;
            txtSpecID.Enabled = false;

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

            txtPackingListUoM.Enabled = false;
            txtPackingListUoMRatio.Enabled = false;
            txtUoMQty.Enabled = false;
            txtUoMAltQty.Enabled = false;
            txtUoMAlt1Qty.Enabled = false;
            txtUoMAlt2Qty.Enabled = false;
            txtToleransiAlt2Value.Enabled = false;
            txtUoMAlt3Qty.Enabled = false;
            txtToleransiAlt3Value.Enabled = false;
            txtUoMAlt4Qty.Enabled = false;
            txtToleransiAlt4Value.Enabled = false;
            txtUoMAltExtQty.Enabled = false;
            txtToleransiExtValue.Enabled = false;
            txtKursOriginal.Enabled = false;
            txtKursDasar.Enabled = false;

            txtUoMNilaiTotal.Enabled = false;
            txtUoMAltNilaiTotal.Enabled = false;
            txtUoMNilaiRataRata.Enabled = false;
            txtUoMAltRataRata.Enabled = false;
            txtMinimumUoMQtyManual.Enabled = true;
            txtMinimumUoMAltQtyManual.Enabled = true;
            txtMinimumUoMQtyPredictive.Enabled = true;
            txtMinimumUoMAltQtyPredictive.Enabled = true;

            txtUoM.Enabled = false;
            txtUoMAlt.Enabled = false;
            txtUoMAlt1.Enabled = true;
            txtUoMAlt1Conversion.Enabled = true;
            chkUoMAlt1.Enabled = true;
            txtUoMAlt2.Enabled = true;
            chkUoMAlt2.Enabled = true;
            txtToleransiAlt2.Enabled = true;
            txtUoMAlt3.Enabled = true;
            txtToleransiAlt3.Enabled = true;
            chkUoMAlt3.Enabled = true;
            txtUoMAlt4.Enabled = true;
            txtToleransiAlt4.Enabled = true;
            chkUoMAlt4.Enabled = true;
            txtUoMAltExt.Enabled = true;
            chkUoMAltExt.Enabled = true;
            txtToleransiExt.Enabled = true;
            txtBentuk.Enabled = true;

            txtVendorPreferenceID1.Enabled = true;
            txtVendorPreferenceID2.Enabled = true;
            txtVendorPreferenceID3.Enabled = true;

            cmbPlus1.Enabled = true;
            cmbPlus2.Enabled = true;
            cmbPlus3.Enabled = true;
            cmbPlus4.Enabled = true;
            cmbPlus5.Enabled = true;
            cmbPlus6.Enabled = true;

            txtTotalUoMQtyIn.Enabled = false;
            txtTotalUoMQtyOut.Enabled = false;
            txtTotalUoMAltQtyIn.Enabled = false;
            txtTotalUoMAltQtyOut.Enabled = false;
            txtTotalUoMNilaiIn.Enabled = false;
            txtTotalUoMNilaiQtyOut.Enabled = false;
            txtTotalUoMAltNilaiIn.Enabled = false;
            txtTotalUoMAltNilaiOut.Enabled = false;

            txtAverageBuyUoMQty.Enabled = false;
            txtAverageBuyUoMAltQty.Enabled = false;
            txtAverageBuyUoMNilai.Enabled = false;
            txtAverageBuyUoMAltNilai.Enabled = false;
            txtAverageSalesUoMQty.Enabled = false;
            txtAverageSalesUoMNilai.Enabled = false;
            txtAverageSalesUoMAltQty.Enabled = false;
            txtAverageSalesUoMAltNilai.Enabled = false;

            cmbRoutineNonRoutine.Enabled = true;
            cmbFastNSlowNmedium.Enabled = true;
            txtLeadTime.Enabled = true;
            txtReorderUoMQty.Enabled = true;
            txtReorderUoMAltQty.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;

            CheckUkuran();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSearchGroupID.Enabled = false;
            btnSearchSubGroup1.Enabled = false;
            btnSearchSubGroup2.Enabled = false;
            btnInventTypeId.Enabled = false;
            btnManufacturerId.Enabled = false;
            btnMerekId.Enabled = false;
            btnGolonganId.Enabled = false;
            btnQualityId.Enabled = false;
            btnSpecId.Enabled = false;

            txtFullItemID.Enabled = false;
            txtItemDeskripsi.Enabled = false;
            txtGroupID.Enabled = false;
            txtGroupDeskripsi.Enabled = false;
            chkGroupCheck.Enabled = false;
            txtSubGroup1ID.Enabled = false;
            txtSubGroup1Desk.Enabled = false;
            chkSubGroup1.Enabled = false;
            txtSubGroup2ID.Enabled = false;
            txtSubGroup2Desk.Enabled = false;
            chkSubGroup2.Enabled = false;
            txtItemID.Enabled = false;
            txtInventTypeID.Enabled = false;
            txtManufacturerID.Enabled = false;
            txtMerekID.Enabled = false;
            txtGolonganID.Enabled = false;
            txtQualityID.Enabled = false;
            txtSpecID.Enabled = false;

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

            txtPackingListUoM.Enabled = false;
            txtPackingListUoMRatio.Enabled = false;
            txtUoMQty.Enabled = false;
            txtUoMAltQty.Enabled = false;
            txtUoMAlt1Qty.Enabled = false;
            txtUoMAlt2Qty.Enabled = false;
            txtToleransiAlt2Value.Enabled = false;
            txtUoMAlt3Qty.Enabled = false;
            txtToleransiAlt3Value.Enabled = false;
            txtUoMAlt4Qty.Enabled = false;
            txtToleransiAlt4Value.Enabled = false;
            txtUoMAltExtQty.Enabled = false;
            txtToleransiExtValue.Enabled = false;
            txtKursOriginal.Enabled = false;
            txtKursDasar.Enabled = false;

            txtUoMNilaiTotal.Enabled = false;
            txtUoMAltNilaiTotal.Enabled = false;
            txtUoMNilaiRataRata.Enabled = false;
            txtUoMAltRataRata.Enabled = false;
            txtMinimumUoMQtyManual.Enabled = false;
            txtMinimumUoMAltQtyManual.Enabled = false;
            txtMinimumUoMQtyPredictive.Enabled = false;
            txtMinimumUoMAltQtyPredictive.Enabled = false;

            txtUoM.Enabled = false;
            txtUoMAlt.Enabled = false;
            txtUoMAlt1.Enabled = false;
            txtUoMAlt1Conversion.Enabled = false;
            chkUoMAlt1.Enabled = false;
            txtUoMAlt2.Enabled = false;
            chkUoMAlt2.Enabled = false;
            txtToleransiAlt2.Enabled = false;
            txtUoMAlt3.Enabled = false;
            txtToleransiAlt3.Enabled = false;
            chkUoMAlt3.Enabled = false;
            txtUoMAlt4.Enabled = false;
            txtToleransiAlt4.Enabled = false;
            chkUoMAlt4.Enabled = false;
            txtUoMAltExt.Enabled = false;
            chkUoMAltExt.Enabled = false;
            txtToleransiExt.Enabled = false;
            txtBentuk.Enabled = false;

            txtVendorPreferenceID1.Enabled = false;
            txtVendorPreferenceID2.Enabled = false;
            txtVendorPreferenceID3.Enabled = false;

            cmbPlus1.Enabled = false;
            cmbPlus2.Enabled = false;
            cmbPlus3.Enabled = false;
            cmbPlus4.Enabled = false;
            cmbPlus5.Enabled = false;
            cmbPlus6.Enabled = false;

            txtTotalUoMQtyIn.Enabled = false;
            txtTotalUoMQtyOut.Enabled = false;
            txtTotalUoMAltQtyIn.Enabled = false;
            txtTotalUoMAltQtyOut.Enabled = false;
            txtTotalUoMNilaiIn.Enabled = false;
            txtTotalUoMNilaiQtyOut.Enabled = false;
            txtTotalUoMAltNilaiIn.Enabled = false;
            txtTotalUoMAltNilaiOut.Enabled = false;

            txtAverageBuyUoMQty.Enabled = false;
            txtAverageBuyUoMAltQty.Enabled = false;
            txtAverageBuyUoMNilai.Enabled = false;
            txtAverageBuyUoMAltNilai.Enabled = false;
            txtAverageSalesUoMQty.Enabled = false;
            txtAverageSalesUoMNilai.Enabled = false;
            txtAverageSalesUoMAltQty.Enabled = false;
            txtAverageSalesUoMAltNilai.Enabled = false;

            cmbRoutineNonRoutine.Enabled = false;
            cmbFastNSlowNmedium.Enabled = false;
            txtLeadTime.Enabled = false;
            txtReorderUoMQty.Enabled = false;
            txtReorderUoMAltQty.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
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
            GetDataHeader(txtFullItemID.Text.Trim());
            ModeBeforeEdit();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Cek Data apakah sudah ada
                string CekData;
                Conn = ConnectionString.GetConnection();

                //Validasi jika kosong
                if (txtFullItemID.Text == "")
                {
                    MessageBox.Show("Data FullItemID tidak boleh kosong.");
                    return;
                }
                else if (txtItemDeskripsi.Text == "")
                {
                    MessageBox.Show("Data ItemDeskripsi tidak boleh kosong.");
                    return;
                }
                else if(txtUoM.Text.Trim()=="" || txtUoMAlt.Text.Trim()=="")
                {
                    MessageBox.Show("Satuan UoM dan Alt Tidak boleh kosong..");
                    return;
                }

                Query = "Select FullItemID from InventTable where FullItemID='" + txtFullItemID.Text.Trim().ToUpper() + "'";
                using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                {
                    CekData = Command.ExecuteScalar() == null ? "" : Command.ExecuteScalar().ToString();
                }

                if (CekData != "" && Mode != "Edit")
                {
                    MessageBox.Show("FullItemID " + txtFullItemID.Text.Trim().ToUpper() + " sudah digunakan, silahkan diganti dengan yang lain.");
                    Conn.Close();
                    return;
                }
                if (ValidasiUkuran() == false)
                {
                    return;
                }

                string GroupSub1Sub2 = txtGroupID.Text.Trim() + "." + txtSubGroup1ID.Text.Trim() + "." + txtSubGroup2ID.Text.Trim() +".";

                //Jika New
                if (Mode=="New")
                {
                    Query = "Insert into dbo.InventTable ([FullItemID],[ItemDeskripsi],[GroupID],[GroupDeskripsi],[GroupDeskripsiDispC]";
                    Query += ",[SubGroup1ID],[SubGroup1Deskripsi],[SubGroup1DeskripsiDispC],[SubGroup2ID],[SubGroup2Deskripsi],[SubGroup2DeskripsiDispC]";
                    Query += ",[UoM],[UoMAlt],[UoMAlt1],[UoMAlt1C],[UoMAlt1ConversionID],[UoMAlt2],[UoMAlt2C],[ToleransiAlt2],[UoMAlt3]";
                    Query += ",[UoMAlt3C],[ToleransiAlt3],[UoMAlt4],[UoMAlt4C],[ToleransiAlt4],[UoMAltExt],[UoMAltExtC],[ToleransiExt]";
                    Query += ",[ItemID],[InventTypeID],[ManufacturerID],[MerekID],[GolonganID],[QualityID],[SpecID],[VendorPreferenceID1]";
                    Query += ",[VendorPreferenceID2],[VendorPreferenceID3],[Bentuk],[Ukuran1Value],[Ukuran1MeasurementID],[Ukuran1Chk],[Ukuran2Value]";
                    Query += ",[Ukuran2MeasurementID],[Ukuran2Chk],[Ukuran3Value],[Ukuran3MeasurementID],[Ukuran3Chk],[Ukuran4Value],[Ukuran4MeasurementID]";
                    Query += ",[Ukuran4Chk],[Ukuran5Value],[Ukuran5MeasurementID],[Ukuran5Chk],[PackingListUoM],[PackingListUoMRatio],[plus1],[plus2],[plus3]";
                    Query += ",[plus4],[plus5],[plus6],[UoMQty],[UoMAltQty],[UoMAlt1Qty],[UoMAlt2Qty],[ToleransiAlt2Value],[UoMAlt3Qty]";
                    Query += ",[ToleransiAlt3Value],[UoMAlt4Qty],[ToleransiAlt4Value],[UoMAltExtQty],[ToleransiExtValue],[KursOriginal]";
                    Query += ",[KursDasar],[UoMNilaiTotal],[UoMAltNilaiTotal],[UoMNilaiRataRata],[UoMAltRataRata],[TotalUoMQtyIn]";
                    Query += ",[TotalUoMQtyOut],[TotalUoMAltQtyIn],[TotalUoMAltQtyOut],[TotalUoMNilaiIn],[TotalUoMNilaiQtyOut]";
                    Query += ",[TotalUoMAltNilaiIn],[TotalUoMAltNilaiOut],[AverageBuyUoMQty],[AverageBuyUoMNilai],[AverageBuyUoMAltQty]";
                    Query += ",[AverageBuyUoMAltNilai],[AverageSalesUoMQty],[AverageSalesUoMNilai],[AverageSalesUoMAltQty]";
                    Query += ",[AverageSalesUoMAltNilai],[RoutineNonRoutine],[FastNSlowNmedium],[LeadTime],[MinimumUoMQtyManual]";
                    Query += ",[MinimumUoMAltQtyManual],[MinimumUoMQtyPredictive],[MinimumUoMAltQtyPredictive],[ReorderUoMQty]";
                    Query += ",[ReorderUoMAltQty],[CreatedDate],[CreatedBy],Resize,ResizeType) OUTPUT Inserted.FullItemID values ";
                    Query += "((SELECT '" + GroupSub1Sub2 + "' +RIGHT('00000'+ISNULL(CAST((Max(Right(FullItemID,5)+1)) AS VARCHAR(5)),1),5) from InventTable Where FullItemID like '%" + GroupSub1Sub2 + "%'),'" + txtItemDeskripsi.Text.Trim().ToUpper() + "','" + txtGroupID.Text.Trim().ToUpper() + "','" + txtGroupDeskripsi.Text.Trim().ToUpper() + "','" + chkGroupCheck.Checked.ToString() + "',";
                    Query += "'" + txtSubGroup1ID.Text.Trim().ToUpper() + "','" + txtSubGroup1Desk.Text.Trim().ToUpper() + "','" + chkSubGroup1.Checked.ToString() + "','" + txtSubGroup2ID.Text.Trim().ToUpper() + "','" + txtSubGroup2Desk.Text.Trim().ToUpper() + "','" + chkSubGroup2.Checked.ToString() + "',";
                    Query += "'" + txtUoM.Text.Trim().ToUpper() + "','" + txtUoMAlt.Text.Trim().ToUpper() + "','" + txtUoMAlt1.Text.Trim().ToUpper() + "','" + chkUoMAlt1.Checked.ToString() + "','" + txtUoMAlt1Conversion.Text.Trim().ToUpper() + "','" + txtUoMAlt2.Text.Trim().ToUpper() + "','" + chkUoMAlt2.Checked.ToString() + "',";
                    Query += "'" + (txtToleransiAlt2.Text.Trim() == "" ? "0.00" : txtToleransiAlt2.Text.Trim()) + "',";
                    Query += "'" + txtUoMAlt3.Text.Trim().ToUpper() + "',";
                    Query += "'" + chkUoMAlt3.Checked.ToString() + "',";
                    Query += "'" + (txtToleransiAlt3.Text.Trim() == "" ? "0.00" : txtToleransiAlt3.Text.Trim()) + "',";
                    Query += "'" + txtUoMAlt4.Text.ToUpper() + "','" + chkUoMAlt4.Checked.ToString() + "',";
                    Query += "'" + (txtToleransiAlt4.Text.Trim() == "" ? "0.00" : txtToleransiAlt4.Text.Trim()) + "',";
                    Query += "'" + txtUoMAltExt.Text.Trim().ToUpper() + "','" + chkUoMAltExt.Checked.ToString() + "',";
                    Query += "'" + (txtToleransiExt.Text.Trim() == "" ? "0.00" : txtToleransiExt.Text.Trim()) + "',";
                    Query += "(SELECT RIGHT('00000'+ISNULL(CAST((Max(Right(FullItemID,5)+1)) AS VARCHAR(5)),1),5) from InventTable Where FullItemID like '%" + GroupSub1Sub2 + "%'),'" + txtInventTypeID.Text.Trim().ToUpper() + "','" + txtManufacturerID.Text.Trim().ToUpper() + "','" + txtMerekID.Text.Trim().ToUpper() + "','" + txtGolonganID.Text.Trim().ToUpper() + "','" + txtQualityID.Text.Trim().ToUpper() + "','" + txtSpecID.Text.Trim().ToUpper() + "','" + txtVendorPreferenceID1.Text.Trim().ToUpper() + "',";
                    Query += "'" + txtVendorPreferenceID2.Text.Trim().ToUpper() + "','" + txtVendorPreferenceID3.Text.Trim().ToUpper() + "','" + txtBentuk.Text.Trim().ToUpper() + "','" + txtUkuran1Value.Text.Trim().ToUpper() + "','" + txtUkuran1MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran1.Checked.ToString() + "','" + txtUkuran2Value.Text.Trim().ToUpper() + "',";
                    Query += "'" + txtUkuran2MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran2.Checked.ToString() + "','" + txtUkuran3Value.Text.Trim().ToUpper() + "','" + txtUkuran3MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran3.Checked.ToString() + "','" + txtUkuran4Value.Text.Trim().ToUpper() + "','" + txtUkuran4MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran4.Checked.ToString() + "',";
                    Query += "'" + txtUkuran5Value.Text.Trim().ToUpper() + "','" + txtUkuran5MeasurementID.Text.Trim().ToUpper() + "','" + ChkUkuran5.Checked.ToString() + "','" + txtPackingListUoM.Text.Trim().ToUpper() + "',";
                    Query += "'" + (txtPackingListUoMRatio.Text.Trim() == "" ? "0.00" : txtPackingListUoMRatio.Text.Trim()) + "',";
                    Query += "'" + cmbPlus1.Text.Trim().ToUpper() + "','" + cmbPlus2.Text.Trim().ToUpper() + "','" + cmbPlus3.Text.Trim().ToUpper() + "',";
                    Query += "'" + cmbPlus4.Text.Trim().ToUpper() + "','" + cmbPlus5.Text.Trim().ToUpper() + "','" + cmbPlus6.Text.Trim().ToUpper() + "',";
                    Query += "'" + (txtUoMQty.Text.Trim() == "" ? "0.00" : txtUoMQty.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAltQty.Text.Trim() == "" ? "0.00" : txtUoMAltQty.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAlt1Qty.Text.Trim() == "" ? "0.00" : txtUoMAlt1Qty.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAlt2Qty.Text.Trim() == "" ? "0.00" : txtUoMAlt2Qty.Text.Trim()) + "',";
                    Query += "'" + (txtToleransiAlt2Value.Text.Trim() == "" ? "0.00" : txtToleransiAlt2Value.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAlt3Qty.Text.Trim() == "" ? "0.00" : txtUoMAlt3Qty.Text.Trim()) + "',";
                    Query += "'" + (txtToleransiAlt3Value.Text.Trim() == "" ? "0.00" : txtToleransiAlt3Value.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAlt4Qty.Text.Trim() == "" ? "0.00" : txtUoMAlt4Qty.Text.Trim()) + "',";
                    Query += "'" + (txtToleransiAlt4Value.Text.Trim() == "" ? "0.00" : txtToleransiAlt4Value.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAltExtQty.Text.Trim() == "" ? "0.00" : txtUoMAltExtQty.Text.Trim()) + "',";
                    Query += "'" + (txtToleransiExtValue.Text.Trim() == "" ? "0.00" : txtToleransiExtValue.Text.Trim()) + "',";
                    Query += "'" + txtKursOriginal.Text.Trim().ToUpper() + "',";
                    Query += "'" + txtKursDasar.Text.Trim().ToUpper() + "',";
                    Query += "'" + (txtUoMNilaiTotal.Text.Trim() == "" ? "0.00" : txtUoMAltNilaiTotal.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAltNilaiTotal.Text.Trim() == "" ? "0.00" : txtUoMAltNilaiTotal.Text.Trim()) + "',";
                    Query += "'" + (txtUoMNilaiRataRata.Text.Trim() == "" ? "0.00" : txtUoMNilaiRataRata.Text.Trim()) + "',";
                    Query += "'" + (txtUoMAltRataRata.Text.Trim() == "" ? "0.00" : txtUoMAltRataRata.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMQtyIn.Text.Trim() == "" ? "0.00" : txtTotalUoMQtyIn.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMQtyOut.Text.Trim() == "" ? "0.00" : txtTotalUoMQtyOut.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMAltQtyIn.Text.Trim() == "" ? "0.00" : txtTotalUoMAltQtyIn.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMAltQtyOut.Text.Trim() == "" ? "0.00" : txtTotalUoMAltQtyOut.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMNilaiIn.Text.Trim() == "" ? "0.00" : txtTotalUoMNilaiIn.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMNilaiQtyOut.Text.Trim() == "" ? "0.00" : txtTotalUoMNilaiQtyOut.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMAltNilaiIn.Text.Trim() == "" ? "0.00" : txtTotalUoMAltNilaiIn.Text.Trim()) + "',";
                    Query += "'" + (txtTotalUoMAltNilaiOut.Text.Trim() == "" ? "0.00" : txtTotalUoMAltNilaiOut.Text.Trim()) + "',";
                    Query += "'" + (txtAverageBuyUoMQty.Text.Trim() == "" ? "0.00" : txtAverageBuyUoMQty.Text.Trim()) + "',";
                    Query += "'" + (txtAverageBuyUoMNilai.Text.Trim() == "" ? "0.00" : txtAverageBuyUoMNilai.Text.Trim()) + "',";
                    Query += "'" + (txtAverageBuyUoMAltQty.Text.Trim() == "" ? "0.00" : txtAverageBuyUoMAltQty.Text.Trim()) + "',";
                    Query += "'" + (txtAverageBuyUoMAltNilai.Text.Trim() == "" ? "0.00" : txtAverageBuyUoMAltNilai.Text.Trim()) + "',";
                    Query += "'" + (txtAverageSalesUoMQty.Text.Trim() == "" ? "0.00" : txtAverageSalesUoMQty.Text.Trim()) + "',";
                    Query += "'" + (txtAverageSalesUoMNilai.Text.Trim() == "" ? "0.00" : txtAverageSalesUoMNilai.Text.Trim()) + "',";
                    Query += "'" + (txtAverageSalesUoMAltQty.Text.Trim() == "" ? "0.00" : txtAverageSalesUoMAltQty.Text.Trim()) + "',";
                    Query += "'" + (txtAverageSalesUoMAltNilai.Text.Trim() == "" ? "0.00" : txtAverageSalesUoMAltNilai.Text.Trim()) + "',";
                    Query += "'" + cmbRoutineNonRoutine.Text.Trim().ToUpper() + "',";
                    Query += "'" + cmbFastNSlowNmedium.Text.Trim().ToUpper() + "',";
                    Query += "'" + txtLeadTime.Text.Trim().ToUpper() + "',";
                    Query += "'" + (txtMinimumUoMQtyManual.Text.Trim() == "" ? "0.00" : txtMinimumUoMQtyManual.Text.Trim()) + "',";
                    Query += "'" + (txtMinimumUoMAltQtyManual.Text.Trim() == "" ? "0.00" : txtMinimumUoMAltQtyManual.Text.Trim()) + "',";
                    Query += "'" + (txtMinimumUoMQtyPredictive.Text.Trim() == "" ? "0.00" : txtMinimumUoMQtyPredictive.Text.Trim()) + "',";
                    Query += "'" + (txtMinimumUoMAltQtyPredictive.Text.Trim() == "" ? "0.00" : txtMinimumUoMAltQtyPredictive.Text.Trim()) + "',";
                    Query += "'" + (txtReorderUoMQty.Text.Trim() == "" ? "0.00" : txtReorderUoMQty.Text.Trim()) + "',";
                    Query += "'" + (txtReorderUoMAltQty.Text.Trim() == "" ? "0.00" : txtUoMAlt2Qty.Text.Trim()) + "',getdate(),'" + ControlMgr.UserId.ToUpper() + "','" + cbResize.Checked.ToString() +"','" + cmbResizeType.Text + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        txtFullItemID.Text = Command.ExecuteScalar().ToString();
                    }
                    Trans.Commit();

                    string strSql = "INSERT INTO InventConversion(FullItemID,ItemDeskripsi,FromUnit,ToUnit,CreatedBy) ";
                    strSql += "VALUES('" + txtFullItemID.Text + "','" + txtItemDeskripsi.Text + "','" + txtUoM.Text.Trim() + "','" + txtUoMAlt.Text.Trim() + "','" + ControlMgr.UserId + "')";
                    using (SqlCommand Cmd = new SqlCommand(strSql,Conn))
                    {
                        Cmd.ExecuteNonQuery();
                    }

                    strSql = "INSERT INTO Invent_Purchase_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtItemDeskripsi.Text + "')";
                    using (SqlCommand Cmd = new SqlCommand(strSql, Conn))
                    {
                        Cmd.ExecuteNonQuery();
                    }

                    strSql = "INSERT INTO Invent_Sales_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtItemDeskripsi.Text + "')";
                    using (SqlCommand Cmd = new SqlCommand(strSql, Conn))
                    {
                        Cmd.ExecuteNonQuery();
                    }

                    strSql = "INSERT INTO Invent_Movement_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtItemDeskripsi.Text + "')";
                    using (SqlCommand Cmd = new SqlCommand(strSql, Conn))
                    {
                        Cmd.ExecuteNonQuery();
                    }

                    strSql = "INSERT INTO Invent_OnHand_Qty(GroupId,SubGroupId,SubGroup2Id,ItemId,FullItemID,ItemName) ";
                    strSql += "VALUES('" + txtGroupID.Text + "','" + txtSubGroup1ID.Text + "','" + txtSubGroup2ID.Text + "','" + txtItemID.Text + "','" + txtFullItemID.Text + "','" + txtItemDeskripsi.Text + "')";
                    using (SqlCommand Cmd = new SqlCommand(strSql, Conn))
                    {
                        Cmd.ExecuteNonQuery();
                    }               

                    MessageBox.Show("FullItemID = " + txtFullItemID.Text.Trim().ToUpper() + Environment.NewLine + "Item Deskripsi = " + txtItemDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");                                  
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventTable set ";
                    Query += "ItemDeskripsi='" + txtItemDeskripsi.Text.Trim().ToUpper() + "',";
                    Query += "UoM='" + txtUoM.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt='" + txtUoMAlt.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt1='" + txtUoMAlt1.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt1C='" + chkUoMAlt1.Checked.ToString() + "',";
                    Query += "UoMAlt1ConversionID='" + txtUoMAlt1Conversion.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt2='" + txtUoMAlt2.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt2C='" + chkUoMAlt2.Checked.ToString() + "',";
                    Query += "ToleransiAlt2='" + txtToleransiAlt2.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt3='" + txtUoMAlt3.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt3C='" + chkUoMAlt3.Checked.ToString() + "',";
                    Query += "ToleransiAlt3='" + txtToleransiAlt3.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt4='" + txtUoMAlt4.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt4C='" + chkUoMAlt4.Checked.ToString() + "',";
                    Query += "ToleransiAlt4='" + txtToleransiAlt4.Text.Trim().ToUpper() + "',";
                    Query += "UoMAltExt='" + txtUoMAltExt.Text.Trim().ToUpper() + "',";
                    Query += "UoMAltExtC='" + chkUoMAltExt.Checked.ToString().ToUpper() + "',";
                    Query += "ToleransiExt='" + txtToleransiExt.Text.Trim().ToUpper() + "',";
                    Query += "ItemID='" + txtItemID.Text.Trim().ToUpper() + "',";
                    Query += "InventTypeID='" + txtInventTypeID.Text.Trim().ToUpper() + "',";
                    Query += "ManufacturerID='" + txtManufacturerID.Text.Trim().ToUpper() + "',";
                    Query += "MerekID='" + txtMerekID.Text.Trim().ToUpper() + "',";
                    Query += "GolonganID='" + txtGolonganID.Text.Trim().ToUpper() + "',";
                    Query += "QualityID='" + txtQualityID.Text.Trim().ToUpper() + "',";
                    Query += "SpecID='" + txtSpecID.Text.Trim().ToUpper() + "',";
                    Query += "VendorPreferenceID1='" + txtVendorPreferenceID1.Text.Trim().ToUpper() + "',";
                    Query += "VendorPreferenceID2='" + txtVendorPreferenceID2.Text.Trim().ToUpper() + "',";
                    Query += "VendorPreferenceID3='" + txtVendorPreferenceID3.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran1='" + txtUkuran1.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran1C='" + txtUkuran1C.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran2='" + txtUkuran2.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran2C='" + txtUkuran2C.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran3='" + txtUkuran3.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran3C='" + txtUkuran3C.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran4='" + txtUkuran4.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran4C='" + txtUkuran4C.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran5='" + txtUkuran5.Text.Trim().ToUpper() + "',";
                    //Query += "Ukuran5C='" + txtUkuran5C.Text.Trim().ToUpper() + "',";
                    //Query += "Bentuk='" + txtBentuk.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran1Value='" + txtUkuran1Value.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran1MeasurementID='" + txtUkuran1MeasurementID.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran1Chk='" + ChkUkuran1.Checked.ToString() + "',";
                    Query += "Ukuran2Value='" + txtUkuran2Value.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran2MeasurementID='" + txtUkuran2MeasurementID.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran2Chk='" + ChkUkuran2.Checked.ToString() + "',";
                    Query += "Ukuran3Value='" + txtUkuran3Value.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran3MeasurementID='" + txtUkuran3MeasurementID.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran3Chk='" + ChkUkuran3.Checked.ToString() + "',";
                    Query += "Ukuran4Value='" + txtUkuran4Value.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran4MeasurementID='" + txtUkuran4MeasurementID.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran4Chk='" + ChkUkuran4.Checked.ToString() + "',";
                    Query += "Ukuran5Value='" + txtUkuran5Value.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran5MeasurementID='" + txtUkuran5MeasurementID.Text.Trim().ToUpper() + "',";
                    Query += "Ukuran5Chk='" + ChkUkuran5.Checked.ToString() + "',";
                    Query += "PackingListUoM='" + txtPackingListUoM.Text.Trim().ToUpper() + "',";
                    Query += "PackingListUoMRatio='" + txtPackingListUoMRatio.Text.Trim().ToUpper() + "',";
                    Query += "plus1='" + cmbPlus1.Text.Trim().ToUpper() + "',";
                    Query += "plus2='" + cmbPlus2.Text.Trim().ToUpper() + "',";
                    Query += "plus3='" + cmbPlus3.Text.Trim().ToUpper() + "',";
                    Query += "plus4='" + cmbPlus4.Text.Trim().ToUpper() + "',";
                    Query += "plus5='" + cmbPlus5.Text.Trim().ToUpper() + "',";
                    Query += "plus6='" + cmbPlus6.Text.Trim().ToUpper() + "',";
                    Query += "UoMQty='" + txtUoMQty.Text.Trim().ToUpper() + "',";
                    Query += "UoMAltQty='" + txtUoMAltQty.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt1Qty='" + txtUoMAlt1Qty.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt2Qty='" + txtUoMAlt2Qty.Text.Trim().ToUpper() + "',";
                    Query += "ToleransiAlt2Value='" + txtToleransiAlt2Value.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt3Qty='" + txtUoMAlt3Qty.Text.Trim().ToUpper() + "',";
                    Query += "ToleransiAlt3Value='" + txtToleransiAlt3Value.Text.Trim().ToUpper() + "',";
                    Query += "UoMAlt4Qty='" + txtUoMAlt4Qty.Text.Trim().ToUpper() + "',";
                    Query += "ToleransiAlt4Value='" + txtToleransiAlt4Value.Text.Trim().ToUpper() + "',";
                    Query += "UoMAltExtQty='" + txtUoMAltExtQty.Text.Trim().ToUpper() + "',";
                    Query += "ToleransiExtValue='" + txtToleransiExtValue.Text.Trim().ToUpper() + "',";
                    Query += "KursOriginal='" + txtKursOriginal.Text.Trim().ToUpper() + "',";
                    Query += "KursDasar='" + txtKursDasar.Text.Trim().ToUpper() + "',";
                    Query += "UoMNilaiTotal='" + txtUoMNilaiTotal.Text.Trim().ToUpper() + "',";
                    Query += "UoMAltNilaiTotal='" + txtUoMAltNilaiTotal.Text.Trim().ToUpper() + "',";
                    Query += "UoMNilaiRataRata='" + txtUoMNilaiRataRata.Text.Trim().ToUpper() + "',";
                    Query += "UoMAltRataRata='" + txtUoMAltRataRata.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMQtyIn='" + txtTotalUoMQtyIn.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMQtyOut='" + txtTotalUoMQtyOut.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMAltQtyIn='" + txtTotalUoMAltQtyIn.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMAltQtyOut='" + txtTotalUoMAltQtyOut.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMNilaiIn='" + txtTotalUoMNilaiIn.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMNilaiQtyOut='" + txtTotalUoMNilaiQtyOut.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMAltNilaiIn='" + txtTotalUoMAltNilaiIn.Text.Trim().ToUpper() + "',";
                    Query += "TotalUoMAltNilaiOut='" + txtTotalUoMAltNilaiOut.Text.Trim().ToUpper() + "',";
                    Query += "AverageBuyUoMQty='" + txtAverageBuyUoMQty.Text.Trim().ToUpper() + "',";
                    Query += "AverageBuyUoMNilai='" + txtAverageBuyUoMNilai.Text.Trim().ToUpper() + "',";
                    Query += "AverageBuyUoMAltQty='" + txtAverageBuyUoMAltQty.Text.Trim().ToUpper() + "',";
                    Query += "AverageBuyUoMAltNilai='" + txtAverageBuyUoMAltNilai.Text.Trim().ToUpper() + "',";
                    Query += "AverageSalesUoMQty='" + txtAverageSalesUoMQty.Text.Trim().ToUpper() + "',";
                    Query += "AverageSalesUoMNilai='" + txtAverageSalesUoMNilai.Text.Trim().ToUpper() + "',";
                    Query += "AverageSalesUoMAltQty='" + txtAverageSalesUoMAltQty.Text.Trim().ToUpper() + "',";
                    Query += "AverageSalesUoMAltNilai='" + txtAverageSalesUoMAltNilai.Text.Trim().ToUpper() + "',";
                    Query += "RoutineNonRoutine='" + cmbRoutineNonRoutine.Text.Trim().ToUpper() + "',";
                    Query += "FastNSlowNmedium='" + cmbFastNSlowNmedium.Text.Trim().ToUpper() + "',";
                    Query += "LeadTime='" + txtLeadTime.Text.Trim().ToUpper() + "',";
                    Query += "MinimumUoMQtyManual='" + txtMinimumUoMQtyManual.Text.Trim().ToUpper() + "',";
                    Query += "MinimumUoMAltQtyManual='" + txtMinimumUoMAltQtyManual.Text.Trim().ToUpper() + "',";
                    Query += "MinimumUoMQtyPredictive='" + txtMinimumUoMQtyPredictive.Text.Trim().ToUpper() + "',";
                    Query += "MinimumUoMAltQtyPredictive='" + txtMinimumUoMAltQtyPredictive.Text.Trim().ToUpper() + "',";
                    Query += "ReorderUoMQty='" + txtReorderUoMQty.Text.Trim().ToUpper() + "',";
                    Query += "ReorderUoMAltQty='" + txtReorderUoMAltQty.Text.Trim().ToUpper() + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "',";
                    Query += "Resize='" + cbResize.Checked.ToString() + "',";
                    Query += "ResizeType='" + cmbResizeType.Text + "'";
                    Query += " Where FullItemID='" + txtFullItemID.Text.Trim().ToUpper() + "'";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("FullItemID = " + txtFullItemID.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
                }
                Conn.Close();
                this.Close();
                
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
            }  
        }

        public void SetParent(InquiryInventTable F)
        {
            Parent = F;
        }

        public void GetDataHeader(string FullItemID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select [FullItemID],[ItemDeskripsi],[GroupID],[GroupDeskripsi],[GroupDeskripsiDispC]";
            Query += ",[SubGroup1ID],[SubGroup1Deskripsi],[SubGroup1DeskripsiDispC],[SubGroup2ID],[SubGroup2Deskripsi],[SubGroup2DeskripsiDispC]";
            Query += ",[UoM],[UoMAlt],[UoMAlt1],[UoMAlt1C],[UoMAlt1ConversionID],[UoMAlt2],[UoMAlt2C],[ToleransiAlt2],[UoMAlt3]";
            Query += ",[UoMAlt3C],[ToleransiAlt3],[UoMAlt4],[UoMAlt4C],[ToleransiAlt4],[UoMAltExt],[UoMAltExtC],[ToleransiExt]";
            Query += ",[ItemID],[InventTypeID],[ManufacturerID],[MerekID],[GolonganID],[QualityID],[SpecID],[VendorPreferenceID1]";
            Query += ",[VendorPreferenceID2],[VendorPreferenceID3],[Bentuk],[Ukuran1Value],[Ukuran1MeasurementID],[Ukuran2Value]";
            Query += ",[Ukuran2MeasurementID],[Ukuran3Value],[Ukuran3MeasurementID],[Ukuran4Value],[Ukuran4MeasurementID]";
            Query += ",[Ukuran5Value],[Ukuran5MeasurementID],[PackingListUoM],[PackingListUoMRatio],[plus1],[plus2],[plus3]";
            Query += ",[plus4],[plus5],[plus6],[UoMQty],[UoMAltQty],[UoMAlt1Qty],[UoMAlt2Qty],[ToleransiAlt2Value],[UoMAlt3Qty]";
            Query += ",[ToleransiAlt3Value],[UoMAlt4Qty],[ToleransiAlt4Value],[UoMAltExtQty],[ToleransiExtValue],[KursOriginal]";
            Query += ",[KursDasar],[UoMNilaiTotal],[UoMAltNilaiTotal],[UoMNilaiRataRata],[UoMAltRataRata],[TotalUoMQtyIn]";
            Query += ",[TotalUoMQtyOut],[TotalUoMAltQtyIn],[TotalUoMAltQtyOut],[TotalUoMNilaiIn],[TotalUoMNilaiQtyOut]";
            Query += ",[TotalUoMAltNilaiIn],[TotalUoMAltNilaiOut],[AverageBuyUoMQty],[AverageBuyUoMNilai],[AverageBuyUoMAltQty]";
            Query += ",[AverageBuyUoMAltNilai],[AverageSalesUoMQty],[AverageSalesUoMNilai],[AverageSalesUoMAltQty]";
            Query += ",[AverageSalesUoMAltNilai],[RoutineNonRoutine],[FastNSlowNmedium],[LeadTime],[MinimumUoMQtyManual]";
            Query += ",[MinimumUoMAltQtyManual],[MinimumUoMQtyPredictive],[MinimumUoMAltQtyPredictive],[ReorderUoMQty]";
            Query += ",[ReorderUoMAltQty],[Ukuran1Chk],[Ukuran2Chk],[Ukuran3Chk],[Ukuran4Chk],[Ukuran5Chk],[Resize],[ResizeType] From InventTable Where FullItemId='" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtFullItemID.Text = Dr["FullItemID"].ToString();
                txtItemDeskripsi.Text = Dr["ItemDeskripsi"].ToString();
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
                txtInventTypeID.Text = Dr["InventTypeID"].ToString();
                txtManufacturerID.Text = Dr["ManufacturerID"].ToString();
                txtMerekID.Text = Dr["MerekID"].ToString();
                txtGolonganID.Text = Dr["GolonganID"].ToString();
                txtQualityID.Text = Dr["QualityID"].ToString();
                txtSpecID.Text = Dr["SpecID"].ToString();
                txtVendorPreferenceID1.Text = Dr["VendorPreferenceID1"].ToString();
                txtVendorPreferenceID2.Text = Dr["VendorPreferenceID2"].ToString();
                txtVendorPreferenceID3.Text = Dr["VendorPreferenceID3"].ToString();
                cmbPlus1.SelectedItem = Dr["plus1"].ToString();
                cmbPlus2.SelectedItem = Dr["plus2"].ToString();
                cmbPlus3.SelectedItem = Dr["plus3"].ToString();
                cmbPlus4.SelectedItem = Dr["plus4"].ToString();
                cmbPlus5.SelectedItem = Dr["plus5"].ToString();
                cmbPlus6.SelectedItem = Dr["plus6"].ToString();
                txtAverageBuyUoMQty.Text = Dr["AverageBuyUoMQty"].ToString();
                txtAverageBuyUoMNilai.Text = Dr["AverageBuyUoMNilai"].ToString();
                txtAverageBuyUoMAltQty.Text = Dr["AverageBuyUoMAltQty"].ToString();
                txtAverageBuyUoMAltNilai.Text = Dr["AverageBuyUoMAltNilai"].ToString();
                txtAverageSalesUoMQty.Text = Dr["AverageSalesUoMQty"].ToString();
                txtAverageSalesUoMNilai.Text = Dr["AverageSalesUoMNilai"].ToString();
                txtAverageSalesUoMAltQty.Text = Dr["AverageSalesUoMAltQty"].ToString();
                txtAverageSalesUoMAltNilai.Text = Dr["AverageSalesUoMAltNilai"].ToString();
                txtTotalUoMQtyIn.Text = Dr["TotalUoMQtyIn"].ToString();
                txtTotalUoMQtyOut.Text = Dr["TotalUoMQtyOut"].ToString();
                txtTotalUoMAltQtyIn.Text = Dr["TotalUoMAltQtyIn"].ToString();
                txtTotalUoMAltQtyOut.Text = Dr["TotalUoMAltQtyOut"].ToString();
                txtTotalUoMNilaiIn.Text = Dr["TotalUoMNilaiIn"].ToString();
                txtTotalUoMNilaiQtyOut.Text = Dr["TotalUoMNilaiQtyOut"].ToString();
                txtTotalUoMAltNilaiIn.Text = Dr["TotalUoMAltNilaiIn"].ToString();
                txtTotalUoMAltNilaiOut.Text = Dr["TotalUoMAltNilaiOut"].ToString();
                cmbRoutineNonRoutine.SelectedItem = Dr["RoutineNonRoutine"].ToString();
                cmbFastNSlowNmedium.SelectedItem = Dr["FastNSlowNmedium"].ToString();
                txtLeadTime.Text = Dr["LeadTime"].ToString();
                txtReorderUoMQty.Text = Dr["ReorderUoMQty"].ToString();
                txtReorderUoMAltQty.Text = Dr["ReorderUoMAltQty"].ToString();

                txtUoM.Text = Dr["UoM"].ToString();
                txtUoMAlt.Text = Dr["UoMAlt"].ToString();
                txtUoMAlt1.Text = Dr["UoMAlt1"].ToString();
                txtUoMAlt1Conversion.Text = Dr["UoMAlt1ConversionID"].ToString();
                chkUoMAlt1.Checked = Dr["UoMAlt1C"] == null ? false : Convert.ToBoolean(Dr["UoMAlt1C"]);
                txtUoMAlt2.Text = Dr["UoMAlt2"].ToString();
                chkUoMAlt2.Checked = Dr["UoMAlt2C"] == null ? false : Convert.ToBoolean(Dr["UoMAlt2C"]);
                txtToleransiAlt2.Text = Dr["ToleransiAlt2"].ToString();
                txtUoMAlt3.Text = Dr["UoMAlt3"].ToString();
                txtToleransiAlt3.Text = Dr["ToleransiAlt3"].ToString();
                chkUoMAlt3.Checked = Dr["UoMAlt3C"] == null ? false : Convert.ToBoolean(Dr["UoMAlt3C"]);
                txtUoMAlt4.Text = Dr["UoMAlt4"].ToString();
                txtToleransiAlt4.Text = Dr["ToleransiAlt4"].ToString();
                chkUoMAlt4.Checked = Dr["UoMAlt4C"] == null ? false : Convert.ToBoolean(Dr["UoMAlt4C"]);
                txtUoMAltExt.Text = Dr["UoMAltExt"].ToString();
                chkUoMAltExt.Checked = Dr["UoMAltExtC"] == null ? false : Convert.ToBoolean(Dr["UoMAltExtC"]);
                txtToleransiExt.Text = Dr["ToleransiExt"].ToString();
                txtBentuk.Text = Dr["Bentuk"].ToString();
                txtUkuran1Value.Text = Dr["Ukuran1Value"].ToString();
                txtUkuran1MeasurementID.Text = Dr["Ukuran1MeasurementID"].ToString();
                ChkUkuran1.Checked = bool.Parse(Dr["Ukuran1Chk"].ToString());
                txtUkuran2Value.Text = Dr["Ukuran2Value"].ToString();
                txtUkuran2MeasurementID.Text = Dr["Ukuran2MeasurementID"].ToString();
                ChkUkuran2.Checked = bool.Parse(Dr["Ukuran2Chk"].ToString());
                txtUkuran3Value.Text = Dr["Ukuran3Value"].ToString();
                txtUkuran3MeasurementID.Text = Dr["Ukuran3MeasurementID"].ToString();
                ChkUkuran3.Checked = bool.Parse(Dr["Ukuran3Chk"].ToString());
                txtUkuran4Value.Text = Dr["Ukuran4Value"].ToString();
                txtUkuran4MeasurementID.Text = Dr["Ukuran4MeasurementID"].ToString();
                ChkUkuran4.Checked = bool.Parse(Dr["Ukuran4Chk"].ToString());
                txtUkuran5Value.Text = Dr["Ukuran5Value"].ToString();
                txtUkuran5MeasurementID.Text = Dr["Ukuran5MeasurementID"].ToString();
                ChkUkuran5.Checked = bool.Parse(Dr["Ukuran5Chk"].ToString());

                txtPackingListUoM.Text = Dr["PackingListUoM"].ToString();
                txtPackingListUoMRatio.Text = Dr["PackingListUoMRatio"].ToString();
                txtUoMQty.Text = Dr["UoMAltQty"].ToString();
                txtUoMAltQty.Text = Dr["UoMAltQty"].ToString();
                txtUoMAlt1Qty.Text = Dr["UoMAlt1Qty"].ToString();
                txtUoMAlt2Qty.Text = Dr["UoMAlt2Qty"].ToString();
                txtToleransiAlt2Value.Text = Dr["ToleransiAlt2Value"].ToString();
                txtUoMAlt3Qty.Text = Dr["UoMAlt3Qty"].ToString();
                txtToleransiAlt3Value.Text = Dr["ToleransiAlt3Value"].ToString();
                txtUoMAlt4Qty.Text = Dr["UoMAlt4Qty"].ToString();
                txtToleransiAlt4Value.Text = Dr["ToleransiAlt4Value"].ToString();
                txtUoMAltExtQty.Text = Dr["UoMAltExtQty"].ToString();
                txtToleransiExtValue.Text = Dr["ToleransiExtValue"].ToString();
                txtKursOriginal.Text = Dr["KursOriginal"].ToString();
                txtKursDasar.Text = Dr["KursDasar"].ToString();
                txtUoMNilaiTotal.Text = Dr["UoMNilaiTotal"].ToString();
                txtUoMAltNilaiTotal.Text = Dr["UoMAltNilaiTotal"].ToString();
                txtUoMNilaiRataRata.Text = Dr["UoMNilaiRataRata"].ToString();
                txtUoMAltRataRata.Text = Dr["UoMAltRataRata"].ToString();
                txtMinimumUoMQtyManual.Text = Dr["MinimumUoMQtyManual"].ToString();
                txtMinimumUoMAltQtyManual.Text = Dr["MinimumUoMAltQtyManual"].ToString();
                txtMinimumUoMQtyPredictive.Text = Dr["MinimumUoMQtyPredictive"].ToString();
                txtMinimumUoMAltQtyPredictive.Text = Dr["MinimumUoMAltQtyPredictive"].ToString();

                cbResize.Checked = bool.Parse(Dr["Resize"].ToString());
                cmbResizeType.SelectedItem = Dr["ResizeType"].ToString();

            }
            Dr.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchGroupID_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventGroup";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtGroupID.Text = ConnectionString.Kode;
            txtGroupDeskripsi.Text = ConnectionString.Kode2;
            txtSubGroup1ID.Text = "";
            txtSubGroup1Desk.Text = "";
            txtSubGroup2ID.Text = "";
            txtSubGroup2Desk.Text = "";
            if (txtGroupID.Text != "")
                chkGroupCheck.Checked = true;
            else
                chkGroupCheck.Checked = false;
            CreateFullItemId();
            CheckUkuran();
        }

        private void btnSearchSubGroup1_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSubGroup1";
            string Where = " AND GROUPID='" + txtGroupID.Text + "'";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            tmpSearch.ShowDialog();
            txtSubGroup1ID.Text = ConnectionString.Kode2;
            txtSubGroup1Desk.Text = ConnectionString.Kode3;
            txtSubGroup2ID.Text = "";
            txtSubGroup2Desk.Text = "";
            if (txtSubGroup1ID.Text != "")
                chkSubGroup1.Checked = true;
            else
                chkSubGroup1.Checked = false;
            CreateFullItemId();
            CheckUkuran();
        }

        private void FormSubGroup2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        private void btnSearchSubGroup2_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSubGroup2";
            string Where = " AND GROUPID='" + txtGroupID.Text + "' AND SUBGROUP1ID='" + txtSubGroup1ID.Text + "'";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            tmpSearch.ShowDialog();
            txtSubGroup2ID.Text = ConnectionString.Kode3;
            txtSubGroup2Desk.Text = ConnectionString.Kode4;
            if (txtSubGroup2ID.Text != "")
                chkSubGroup2.Checked = true;
            else
                chkSubGroup2.Checked = false;
            CreateFullItemId();
            CheckUkuran();
        }


        private void chkGroupCheck_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void chkSubGroup1_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void chkSubGroup2_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void btnItemId_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventItem";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtItemID.Text = ConnectionString.Kode;
            CreateFullItemId();
        }

        private void btnInventTypeId_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventType";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtInventTypeID.Text = ConnectionString.Kode;
        }

        private void btnManufacturerId_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventManufacturer";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtManufacturerID.Text = ConnectionString.Kode;
        }

        private void btnMerekId_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventMerek";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtMerekID.Text = ConnectionString.Kode;
        }

        private void btnGolonganId_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventGolongan";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtGolonganID.Text = ConnectionString.Kode2;
        }

        private void btnQualityId_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventQuality";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtQualityID.Text = ConnectionString.Kode;
        }

        private void btnSpecId_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSpec";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtSpecID.Text = ConnectionString.Kode;

        }

        private void CreateFullItemId()
        {
            if (Mode != "BeforeEdit")
            {
                txtItemDeskripsi.Text = "";
                txtFullItemID.Text = "";
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
                if (ChkUkuran1.Checked == true)
                {
                    txtItemDeskripsi.Text += txtUkuran1Value.Text + " " + txtSatuan1.Text;
                    ukuran = true;
                }
                if (ChkUkuran2.Checked == true)
                {
                    if (ukuran == true)
                        txtItemDeskripsi.Text += " x ";

                    txtItemDeskripsi.Text += txtUkuran2Value.Text + " " + txtSatuan2.Text;
                    ukuran = true;
                }
                if (ChkUkuran3.Checked == true)
                {
                    if (ukuran == true)
                        txtItemDeskripsi.Text += " x ";

                    txtItemDeskripsi.Text += txtUkuran3Value.Text + " " + txtSatuan3.Text;
                    ukuran = true;
                }
                if (ChkUkuran4.Checked == true)
                {
                    if (ukuran == true)
                        txtItemDeskripsi.Text += " x ";

                    txtItemDeskripsi.Text += txtUkuran4Value.Text + " " + txtSatuan4.Text;
                    ukuran = true;
                }
                if (ChkUkuran5.Checked == true)
                {
                    if (ukuran == true)
                        txtItemDeskripsi.Text += " x ";

                    txtItemDeskripsi.Text += txtUkuran5Value.Text + " " + txtSatuan5.Text;
                    ukuran = true;
                }

            }
        }

        #region CheckUkuran
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
                        txtUkuran1MeasurementID.Text = Dr["DimensionID1"].ToString();
                        txtSatuan1.Text = Dr["Measurement1"].ToString();
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
                        txtUkuran2MeasurementID.Text = Dr["DimensionID2"].ToString();
                        txtSatuan2.Text = Dr["Measurement2"].ToString();
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
                        txtUkuran3MeasurementID.Text = Dr["DimensionID3"].ToString();
                        txtSatuan3.Text = Dr["Measurement3"].ToString();
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
                        txtUkuran4MeasurementID.Text = Dr["DimensionID4"].ToString();
                        txtSatuan4.Text = Dr["Measurement4"].ToString();
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
                        txtUkuran5MeasurementID.Text = Dr["DimensionID5"].ToString();
                        txtSatuan5.Text = Dr["Measurement5"].ToString();
                    }
                }
                Conn.Close();
            }
        }

        private Boolean ValidasiUkuran()
        {
            if (txtUkuran1Value.Enabled == true && txtUkuran1Value.Text.Trim() == "")
            {
                MessageBox.Show("Ukuran1Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran2Value.Enabled == true && txtUkuran2Value.Text.Trim() == "")
            {
                MessageBox.Show("Ukuran2Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran3Value.Enabled == true && txtUkuran3Value.Text.Trim() == "")
            {
                MessageBox.Show("Ukuran3Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran4Value.Enabled == true && txtUkuran4Value.Text.Trim() == "")
            {
                MessageBox.Show("Ukuran4Value tidak boleh kosong.");
                return false;
            }
            else if (txtUkuran5Value.Enabled == true && txtUkuran5Value.Text.Trim() == "")
            {
                MessageBox.Show("Ukuran5Value tidak boleh kosong.");
                return false;
            }
            return true;
        }

        #endregion




        #region

            private void CollapseAll()
            {
                CollapseFlVendor();
                CollapseFlPlus();
                CollapseFlTotal();
                CollapseFlAverage();
                CollapseFlRoutine();
                CollapseFlUkuran();
                CollapseFlPacking();
                CollapseFlSubResize();
            }
            
            private void CollapseFlVendor()
            {
                //Form Invent = 1008, 788 (X,Y)

                if (FlVendorP1 == false)
                {
                    //340, 85
                    grpVendorPreference.Height = 0;
                    grpVendorPreference.Width = 0;

                    lblPacking.Top -= 85;
                    grpPacking.Top -= 85;
                    lblAverage.Top -= 85;
                    grpAverage.Top -= 85;
                    lblSubResize.Top -= 85;
                    grpSubResize.Top -= 85;
                   
                }
                else
                {
                    grpVendorPreference.Width = 372;
                    grpVendorPreference.Height = 85;

                    lblPacking.Top += 85;
                    grpPacking.Top += 85;
                    lblAverage.Top += 85;
                    grpAverage.Top += 85;
                    lblSubResize.Top += 85;
                    grpSubResize.Top += 85;
                }
            }

            private void CollapseFlPacking()
            {
                //Right Packing = 360, 551
                if (FlPacking1 == false)
                {
                    grpPacking.Height = 0;
                    grpPacking.Width = 0;

                    lblAverage.Top -= 551;
                    grpAverage.Top -= 551;

                    lblSubResize.Top -= 551;
                    grpSubResize.Top -= 551;
                }
                else
                {
                    grpPacking.Width = 360;
                    grpPacking.Height = 551;

                    lblAverage.Top += 551;
                    grpAverage.Top += 551;

                    lblSubResize.Top += 551;
                    grpSubResize.Top += 551;
                }
            }

            private void CollapseFlAverage()
            {
                //Left Group Average = 340,  200

                if (FlAverage1 == false)
                {
                    grpAverage.Height = 0;
                    grpAverage.Width = 0;

                    lblSubResize.Top -= 200;
                    grpSubResize.Top -= 200;
                }
                else
                {
                    grpAverage.Width = 340;
                    grpAverage.Height = 200;

                    lblSubResize.Top += 200;
                    grpSubResize.Top += 200;
                }
            }

            private void CollapseFlSubResize()
            {
                //Left Group Average = 340,  200

                if (FlSubResize == false)
                {
                    grpSubResize.Height = 0;
                    grpSubResize.Width = 0;
                }
                else
                {
                    grpSubResize.Width = 340;
                    grpSubResize.Height = 99;
                }
            }

            private void CollapseFlPlus()
            {
                //Left Group Plus = 340, 204
                if (FlPlus1 == false)
                {
                    grpPlus.Height = 0;
                    grpPlus.Width = 0;

                    lblRoutine.Top -= 204;
                    grpRoutine.Top -= 204;
                    lblTotal.Top -= 204;
                    grpTotal.Top -= 204;

                    btnSave.Top -= 204;
                    btnEdit.Top -= 204;
                    btnCancel.Top -= 204;
                    btnExit.Top -= 204;

                }
                else
                {
                    grpPlus.Width = 465;
                    grpPlus.Height = 204;

                    lblRoutine.Top += 204;
                    grpRoutine.Top += 204;
                    lblTotal.Top += 204;
                    grpTotal.Top += 204;

                    btnSave.Top += 204;
                    btnEdit.Top += 204;
                    btnCancel.Top += 204;
                    btnExit.Top += 204;

                }
            }

            private void CollapseFlTotal()
            {
                //Left Group Total = 340, 204

                if (FlTotal1 == false)
                {
                    grpTotal.Height = 0;
                    grpTotal.Width = 0;

                    btnSave.Top -= 204;
                    btnEdit.Top -= 204;
                    btnCancel.Top -= 204;
                    btnExit.Top -= 204;

                }
                else
                {
                    grpTotal.Width = 340;
                    grpTotal.Height = 204;

                    btnSave.Top += 204;
                    btnEdit.Top += 204;
                    btnCancel.Top += 204;
                    btnExit.Top += 204;

                }
            }

            private void CollapseFlRoutine()
            {
                //Left Group Routine = 340, 135

                if (FlRoutine1 == false)
                {
                    grpRoutine.Height = 0;
                    grpRoutine.Width = 0;

                    lblTotal.Top -= 135;
                    grpTotal.Top -= 135;

                    btnSave.Top -= 135;
                    btnEdit.Top -= 135;
                    btnCancel.Top -= 135;
                    btnExit.Top -= 135;

                }
                else
                {
                    grpRoutine.Width = 340;
                    grpRoutine.Height = 135;

                    lblTotal.Top += 135;
                    grpTotal.Top += 135;

                    btnSave.Top += 135;
                    btnEdit.Top += 135;
                    btnCancel.Top += 135;
                    btnExit.Top += 135;

                }
            }

            private void CollapseFlUkuran()
            {
                //Right Ukuran = 465,224
                if (FlUkuran1 == false)
                {
                    grpUkuran.Height = 0;
                    grpUkuran.Width = 0;

                    lblPlus.Top -= 244;
                    grpPlus.Top -= 244;
                    lblRoutine.Top -= 244;
                    grpRoutine.Top -= 244;
                    lblTotal.Top -= 244;
                    grpTotal.Top -= 244;

                    btnSave.Top -= 244;
                    btnEdit.Top -= 244;
                    btnCancel.Top -= 244;
                    btnExit.Top -= 244;
                }
                else
                {
                    grpUkuran.Width = 465;
                    grpUkuran.Height = 244;

                    lblPlus.Top += 244;
                    grpPlus.Top += 244;
                    lblRoutine.Top += 244;
                    grpRoutine.Top += 244;
                    lblTotal.Top += 244;
                    grpTotal.Top += 244;

                    btnSave.Top += 244;
                    btnEdit.Top += 244;
                    btnCancel.Top += 244;
                    btnExit.Top += 244;

                }
            }

           

            private void lblVendorPreference_Click(object sender, EventArgs e)
            {
                if (FlVendorP1 == false)
                    FlVendorP1 = true;
                else
                    FlVendorP1 = false;

                CollapseFlVendor();
            }

            private void lblPlus_Click(object sender, EventArgs e)
            {
                if (FlPlus1 == false)
                    FlPlus1 = true;
                else
                    FlPlus1 = false;

                CollapseFlPlus();
            }

            private void lblTotal_Click(object sender, EventArgs e)
            {
                if (FlTotal1 == false)
                    FlTotal1 = true;
                else
                    FlTotal1 = false;

                CollapseFlTotal();
            }
                
            private void lblAverage_Click(object sender, EventArgs e)
            {
                if (FlAverage1 == false)
                    FlAverage1 = true;
                else
                    FlAverage1 = false;

                CollapseFlAverage();
            }

            private void lblRoutine_Click(object sender, EventArgs e)
            {
                if (FlRoutine1 == false)
                    FlRoutine1 = true;
                else
                    FlRoutine1 = false;

                CollapseFlRoutine();
            }

            private void lblUkuran_Click(object sender, EventArgs e)
            {
                if (FlUkuran1 == false)
                    FlUkuran1 = true;
                else
                    FlUkuran1 = false;

                CollapseFlUkuran();
            }

            private void lblPacking_Click(object sender, EventArgs e)
            {
                if (FlPacking1 == false)
                    FlPacking1 = true;
                else
                    FlPacking1 = false;

                CollapseFlPacking();
            }

        #endregion

        private void txtUkuran1Value_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtUkuran2Value_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtUkuran3Value_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtUkuran4Value_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtUkuran5Value_KeyPress(object sender, KeyPressEventArgs e)
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

        private void FormInventTable_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void AddCmbPlus()
        {
            cmbPlus1.Items.Add("");
            cmbPlus1.Items.Add("TYPE");
            cmbPlus1.Items.Add("MANUFACTURER");
            cmbPlus1.Items.Add("MEREK");
            cmbPlus1.Items.Add("GOLONGAN");
            cmbPlus1.Items.Add("QUALITY");
            cmbPlus1.Items.Add("SPEC");
            cmbPlus1.SelectedIndex = 0;

            cmbPlus2.Items.Add("");
            cmbPlus2.Items.Add("TYPE");
            cmbPlus2.Items.Add("MANUFACTURER");
            cmbPlus2.Items.Add("MEREK");
            cmbPlus2.Items.Add("GOLONGAN");
            cmbPlus2.Items.Add("QUALITY");
            cmbPlus2.Items.Add("SPEC");
            cmbPlus2.SelectedIndex = 0;

            cmbPlus3.Items.Add("");
            cmbPlus3.Items.Add("TYPE");
            cmbPlus3.Items.Add("MANUFACTURER");
            cmbPlus3.Items.Add("MEREK");
            cmbPlus3.Items.Add("GOLONGAN");
            cmbPlus3.Items.Add("QUALITY");
            cmbPlus3.Items.Add("SPEC");
            cmbPlus3.SelectedIndex = 0;

            cmbPlus4.Items.Add("");
            cmbPlus4.Items.Add("TYPE");
            cmbPlus4.Items.Add("MANUFACTURER");
            cmbPlus4.Items.Add("MEREK");
            cmbPlus4.Items.Add("GOLONGAN");
            cmbPlus4.Items.Add("QUALITY");
            cmbPlus4.Items.Add("SPEC");
            cmbPlus4.SelectedIndex = 0;

            cmbPlus5.Items.Add("");
            cmbPlus5.Items.Add("TYPE");
            cmbPlus5.Items.Add("MANUFACTURER");
            cmbPlus5.Items.Add("MEREK");
            cmbPlus5.Items.Add("GOLONGAN");
            cmbPlus5.Items.Add("QUALITY");
            cmbPlus5.Items.Add("SPEC");
            cmbPlus5.SelectedIndex = 0;

            cmbPlus6.Items.Add("");
            cmbPlus6.Items.Add("TYPE");
            cmbPlus6.Items.Add("MANUFACTURER");
            cmbPlus6.Items.Add("MEREK");
            cmbPlus6.Items.Add("GOLONGAN");
            cmbPlus6.Items.Add("QUALITY");
            cmbPlus6.Items.Add("SPEC");
            cmbPlus6.SelectedIndex = 0;
        }

        private void FunctionChangePlus()
        {
            txtHasilPlus.Text = txtItemDeskripsiCopy.Text + " " + CheckChangePlus(cmbPlus1.Text) + " " + CheckChangePlus(cmbPlus2.Text) + " " + CheckChangePlus(cmbPlus3.Text) + " " + CheckChangePlus(cmbPlus4.Text) + " " + CheckChangePlus(cmbPlus5.Text) + " " + CheckChangePlus(cmbPlus6.Text);
        }

        private string CheckChangePlus(string Plus)
        {
            if (Plus == "TYPE")
                return txtInventTypeID.Text;
            else if (Plus == "MANUFACTURER")
                return txtManufacturerID.Text;
            else if (Plus == "MEREK")
                return txtMerekID.Text;
            else if (Plus == "GOLONGAN")
                return txtGolonganID.Text;
            else if (Plus == "QUALITY")
                return txtQualityID.Text;
            else if (Plus == "SPEC")
                return txtSpecID.Text;
            else
                return "";
        }

        private void txtItemDeskripsi_TextChanged(object sender, EventArgs e)
        {
            txtItemDeskripsiCopy.Text = txtItemDeskripsi.Text;
            FunctionChangePlus();
        }

        private void cmbPlus1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cmbPlus2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cmbPlus3_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cmbPlus4_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cmbPlus5_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void cmbPlus6_SelectedIndexChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtInventTypeID_TextChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtManufacturerID_TextChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtMerekID_TextChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtGolonganID_TextChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtQualityID_TextChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtSpecID_TextChanged(object sender, EventArgs e)
        {
            FunctionChangePlus();
        }

        private void txtUoM_TextChanged(object sender, EventArgs e)
        {
            lblUoM.Text = txtUoM.Text;
        }

        private void txtUoMAlt_TextChanged(object sender, EventArgs e)
        {
            lblUoMAlt.Text = txtUoMAlt.Text;
        }

        private void ChkUkuran1_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void ChkUkuran2_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void ChkUkuran3_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void ChkUkuran4_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void ChkUkuran5_CheckedChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void txtUkuran1Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void txtUkuran2Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void txtUkuran3Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void txtUkuran4Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void txtUkuran5Value_TextChanged(object sender, EventArgs e)
        {
            CreateFullItemId();
        }

        private void btnSearchUoM_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventUnit";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtUoM.Text = ConnectionString.Kode;
        }

        private void btnSearchUoMAlt_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventUnit";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtUoMAlt.Text = ConnectionString.Kode;
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            if (txtItemID.Text == "")
            {
                MessageBox.Show("Save Item terlebih dahulu..");
            }
            else {
                if (cbResize.Checked == false)
                {
                    MessageBox.Show("Bukan Item Resize");
                }
                else
                {
                    InvantTable.HdrResize f = new InvantTable.HdrResize();
                    f.ItemID = txtFullItemID.Text;
                    f.ItemName = txtItemDeskripsi.Text;
                    f.ResizeType = cmbResizeType.Text;
                    f.ShowDialog();
                }
            }
        }

        private void lblSubResize_Click(object sender, EventArgs e)
        {
            if (FlSubResize == false)
                FlSubResize = true;
            else
                FlSubResize = false;

            CollapseFlSubResize();
        }

        private void btnConversion_Click(object sender, EventArgs e)
        {
            if (txtItemID.Text == "")
            {
                MessageBox.Show("Save Item terlebih dahulu..");
            }
            else
            {
                InvantTable.PopUpConversion tmpConversion = new InvantTable.PopUpConversion();
                tmpConversion.FullItemId = txtFullItemID.Text.Trim();
                tmpConversion.ShowDialog();       
            }
        }

        private void btnVendor1_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "VendID";
            tmpSearch.Order = "VendId Asc";
            tmpSearch.QuerySearch = "SELECT [VendID],[VendName],[TaxName],[NPWP],[PKP],[Type],[TempoBayar],[PaymentModeID],[TaxGroup],[CurrencyID],[ReffVendID],[VendGroupID] FROM [VendTable]";
            tmpSearch.FilterText = new string[] { "VendID", "VendName", "TaxName", "NPWP" };
            tmpSearch.Select = new string[] { "VendID", "VendName" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtVendorPreferenceID1.Text = ConnectionString.Kodes[0];
                ConnectionString.Kodes = null;
            }
        }

        private void btnVendor2_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "VendID";
            tmpSearch.Order = "VendId Asc";
            tmpSearch.QuerySearch = "SELECT [VendID],[VendName],[TaxName],[NPWP],[PKP],[Type],[TempoBayar],[PaymentModeID],[TaxGroup],[CurrencyID],[ReffVendID],[VendGroupID] FROM [VendTable]";
            tmpSearch.FilterText = new string[] { "VendID", "VendName", "TaxName", "NPWP" };
            tmpSearch.Select = new string[] { "VendID", "VendName" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtVendorPreferenceID2.Text = ConnectionString.Kodes[0];
                ConnectionString.Kodes = null;
            }
        }

        private void btnVendor3_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "VendID";
            tmpSearch.Order = "VendId Asc";
            tmpSearch.QuerySearch = "SELECT [VendID],[VendName],[TaxName],[NPWP],[PKP],[Type],[TempoBayar],[PaymentModeID],[TaxGroup],[CurrencyID],[ReffVendID],[VendGroupID] FROM [VendTable]";
            tmpSearch.FilterText = new string[] { "VendID", "VendName", "TaxName", "NPWP" };
            tmpSearch.Select = new string[] { "VendID", "VendName" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtVendorPreferenceID3.Text = ConnectionString.Kodes[0];
                ConnectionString.Kodes = null;
            }
        }

        private void cbResize_CheckedChanged(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select Count(From_FullItemId) from [InventResize] where From_FullItemId='" + txtFullItemID.Text.Trim().ToUpper() + "'";
            int Cek;
            using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
            {
                Cek = int.Parse(Command.ExecuteScalar().ToString()) == 0 ? 0 : int.Parse(Command.ExecuteScalar().ToString());
            }
            Conn.Close();
            if (Cek > 0 && cbResize.Checked == false)
            {
                MessageBox.Show("Data sudah terdapat di InventResize.");
                cbResize.Checked = true;
            }
            //else if (Cek < 1 && cbResize.Checked == true)
            //{
            //    MessageBox.Show("Data sudah terdapat di InventResize.");
            //    cbResize.Checked = false;
            //}
        }

        private void cmbResizeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbResizeType.SelectedItem.ToString().ToUpper() == "AUTO")
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select Count(From_FullItemId) from [InventResize] where From_FullItemId='" + txtFullItemID.Text.Trim().ToUpper() + "'";
                int Cek;
                using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                {
                    Cek = int.Parse(Command.ExecuteScalar().ToString()) == 0 ? 0 : int.Parse(Command.ExecuteScalar().ToString());
                }
                Conn.Close();
                //if (Cek > 0 && Cek < 2)
                //{
                //    cmbResizeType.SelectedItem = "Auto";
                //}
                if (Cek > 1)
                {
                    MessageBox.Show("Data lebih dari 1, sehingga tidak bisa auto.");
                    cmbResizeType.SelectedItem = "Manual";
                }
                //else if (Cek < 1)
                //{
                //    MessageBox.Show("Data belum dibuat, tidak bisa Auto / Manual.");
                //    cmbResizeType.SelectedItem = "";
                //}
            }
        } 
    }
}
