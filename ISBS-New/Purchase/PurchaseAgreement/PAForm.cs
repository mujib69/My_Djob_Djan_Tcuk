using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ISBS_New.Purchase.PurchaseAgreement
{
    public partial class PAForm : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Status, Query, crit = null;
        int Index;

        public string CanvasId = "";
        public string PurchQuotId = "";
        public string VendId = "";
        public string AgreementId = "";
        public string oldPAID = "";

        Purchase.PurchaseAgreement.PAInq Parent;
        List<string> sSelectedFile, FileName, Extension;
        List<byte[]> test = new List<byte[]>();

        DataGridViewComboBoxCell cell;
        DataGridViewComboBoxCell DeliveryMethod;

        DateTimePicker dtp;
        DateTime adate;

        //tia edit
        ContextMenu Cm = new ContextMenu();
        PopUp.Vendor.Vendor VendorId = new PopUp.Vendor.Vendor();
        TaskList.Purchase.PurchaseAgreement.TaskListPA ParentToTaskListPA;
        public void SetParent2(TaskList.Purchase.PurchaseAgreement.TaskListPA f2)
        {
            ParentToTaskListPA = f2;
        }
        //tia edit end


        //begin
        //created by : joshua
        //created date : 21 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void SetParent(Purchase.PurchaseAgreement.PAInq f)
        {
            Parent = f;
        }

        public PAForm()
        {
            InitializeComponent();
        }

        private void dtp_ValueChanged(object sender, EventArgs e)
        {
            dgvPA.CurrentCell.Value = dtp.Text;
        }

        private void PAForm_Load(object sender, EventArgs e)
        {
            this.Location = new Point(100, 11);
            cmbPPn_Load();
            cmbPPh_Load();
            AddCmbCurrency();
            AddCmbPaymentMode();
            AddCmbTermOfPayment();
            GenerateAttachment();

            dtp = new DateTimePicker();
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "dd/MM/yyyy";
            dtp.Visible = false;
            dtp.Width = 100;
            dtOrderDate.Enabled = false;
            dtDueDate.Enabled = false;
            dtDueDate.Enabled = false;

            dgvPA.Controls.Add(dtp);
            dtp.ValueChanged += this.dtp_ValueChanged;

            RefreshData();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "View")
            {
                ModeView();
            }
            else if (Mode == "Ammend")
            {
                ModeAmmend();
            }
            if (Mode != "New")
            {
                ModeApprove();
            }

            txtExchangeRate.TextAlign = HorizontalAlignment.Right;


            this.txtExchangeRate.Enter += new EventHandler(txtExchangeRate_Enter);
            this.txtExchangeRate.Leave += new EventHandler(txtExchangeRate_Leave);

        }

        protected void txtExchangeRate_SetText()
        {
            this.txtExchangeRate.Text = "1";
            txtExchangeRate.ForeColor = Color.Gray;
        }
        private void txtExchangeRate_Enter(object sender, EventArgs e)
        {

            if (txtExchangeRate.ForeColor != Color.Black)
                //txtExchRate.Text = "";
                txtExchangeRate.ForeColor = Color.Black;
        }


        private void txtExchangeRate_Leave(object sender, EventArgs e)
        {
            if (txtExchangeRate.Text.Trim() == "")
                txtExchangeRate_SetText();
        }

        private void CheckAmmend()
        {
            Conn = ConnectionString.GetConnection();

            //Query = "Select RefTransId From [PurchAgreementH] h ";
            //Query += "Where h.AgreementId = '" + txtPAID.Text + "'";

            Query = "SELECT COUNT(h.AgreementID) FROM PurchAgreementH H INNER JOIN PurchAgreementDtl D ";
            Query += "ON D.AgreementID = H.AgreementID WHERE (H.StClose = 1 OR (D.Qty <> D.RemainingQty)) AND H.AgreementID = '" + AgreementId + "'";

            Cmd = new SqlCommand(Query, Conn);
            int countData = Convert.ToInt32(Cmd.ExecuteScalar());

            if (countData == 0)
            {
                ModeApprove();
            }
            Conn.Close();
        }

        private int CheckApprove()
        {
            Conn = ConnectionString.GetConnection();
            int result = 0;
            Query = "SELECT COUNT(h.AgreementID) FROM PurchAgreementH H INNER JOIN PurchAgreementDtl D ";
            Query += "ON D.AgreementID = H.AgreementID WHERE H.StClose = 1 AND H.AgreementID = '" + AgreementId + "'";
            //Query += "ON D.AgreementID = H.AgreementID WHERE (H.StClose = 1 OR (D.Qty <> D.RemainingQty)) AND H.AgreementID = '" + AgreementId + "'";

            Cmd = new SqlCommand(Query, Conn);
            result = Convert.ToInt32(Cmd.ExecuteScalar());
            Conn.Close();
            return result;
        }

        private void ModeLoad()
        {
            RefreshData();
        }

        private void ModeApprove()
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {

                if (CheckApprove() == 0)
                {
                    btnAmmend.Visible = false;
                    gbApprove.Visible = true;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                    txtDeskripsi.ReadOnly = true;

                    if (CheckStatus(AgreementId) == "03")
                    {
                        btnApprove.Enabled = false;
                        btnReject.Enabled = false;
                        btnRevision.Enabled = false;
                        btnCancelApproved.Enabled = true;
                    }
                    else
                    {
                        btnApprove.Enabled = true;
                        btnReject.Enabled = true;
                        btnRevision.Enabled = true;
                        btnCancelApproved.Enabled = false;
                    }
                }
                else
                {
                    btnAmmend.Visible = false;
                    gbApprove.Visible = false;
                    btnEdit.Visible = false;
                    btnCancel.Visible = false;
                    txtDeskripsi.ReadOnly = false;
                }
            }
        }

        public void SetMode(string tmpMode, string tmpCanvasId, string agreementid)
        {
            Mode = tmpMode;
            CanvasId = tmpCanvasId;
            AgreementId = agreementid;
            if (Mode == "New")
            {
                SelectPQ s = new SelectPQ();
                s.GetCSId(CanvasId);
                s.SetParentForm(this);
                s.ShowDialog();
            }

        }

        public void AddQuotation(string pqid, string vendid)
        {
            PurchQuotId = pqid;
            VendId = vendid;
        }

        public void ModeNew()
        {
            dtDueDate.Value = DateTime.Now;

            //dtDueDate.Enabled = true;
            txtDeskripsi.ReadOnly = false;

            //dtOrderDate.Enabled = true;
            //dtDueDate.Enabled = true;
            cmbCurrency.Enabled = true;
            cmbTermOfPayment.Enabled = true;
            cmbPaymentMode.Enabled = true;
            cmbDPRequired.Enabled = true;
            cmbPPh.Enabled = true;
            cmbPPn.Enabled = true;
            txtDeskripsi.ReadOnly = false;

            btnSave.Visible = true;
            btnAmmend.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
        }

        public void ModeAmmend()
        {
            //dgvPA.Rows.Clear();
            txtPAID.Text = "";
            txtRefID.Text = oldPAID;
            txtExchangeRate.ReadOnly = true;
            txtExchangeRate.ForeColor = Color.Gray;

            cmbCurrency.Enabled = true;
            cmbTermOfPayment.Enabled = true;
            cmbPaymentMode.Enabled = true;
            cmbDPRequired.Enabled = true;
            cmbPPh.Enabled = true;
            cmbPPn.Enabled = true;
            txtDeskripsi.ReadOnly = false;

            btnNew.Visible = true;
            btnDelete.Visible = true;

            //BY: HC (S)
            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            //BY: HC (E)

            btnSave.Visible = true;
            btnAmmend.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            dtDueDate.Value = DateTime.Now;
            //dtOrderDate.Enabled = true;
            //dtDueDate.Enabled = true;

        }

        public void ModeView()
        {
            txtExchangeRate.ReadOnly = true;
            //REMARKED BY: HC (S) soalnya pas view masi bisa edit2 cmb nya
            //cmbCurrency.Enabled = true;
            //cmbTermOfPayment.Enabled = true;
            //cmbPaymentMode.Enabled = true;
            //cmbDPRequired.Enabled = true;
            //cmbPPh.Enabled = true;
            //cmbPPn.Enabled = true;
            //REMARKED BY: HC (E)
            //BY: HC (S)
            cmbCurrency.Enabled = false;
            cmbTermOfPayment.Enabled = false;
            cmbPaymentMode.Enabled = false;
            cmbDPRequired.Enabled = false;
            cmbPPh.Enabled = false;
            cmbPPn.Enabled = false;
            btnNew.Visible = true; btnNew.Enabled = false;
            btnDelete.Visible = true; btnDelete.Enabled = false;
            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnDelAttachment.Enabled = false;
            label29.Visible = false;
            label30.Visible = false;
            label31.Visible = false;
            cmbDPType.Enabled = false;
            tbxDPAmount.Enabled = false;
            tbxDPPercent.Enabled = false;
            cbxHitung.Enabled = false;
            //BY: HC (E)

            txtDeskripsi.ReadOnly = true;
            //tia
            txtRefID.Enabled = true;
            txtRefID.ReadOnly = true;
            txtVendID.ContextMenu = Cm;
            txtVendorName.ContextMenu = Cm;
            txtRefID.ContextMenu = Cm;
            //tia end

            btnSave.Visible = false;

            btnCancel.Visible = false;
            dgvPA.ReadOnly = true;

            // dtDueDate.Value = DateTime.Now;
            //dtDueDate.Enabled = false;
            //dtOrderDate.Enabled = true;
            //dtDueDate.Enabled = true;

            string TransStatus = CheckStatus(txtPAID.Text);
            if (TransStatus == "03")
            {
                if (CheckDataComplete(AgreementId) != 0)
                {
                    btnAmmend.Visible = false;
                    btnEdit.Visible = false;
                }
                else
                {
                    btnAmmend.Visible = true;
                    btnEdit.Visible = false;
                }

            }
            else if (TransStatus == "02" || TransStatus == "04")
            {
                btnAmmend.Visible = false;
                btnEdit.Visible = true;
            }
            else
            {
                btnAmmend.Visible = false;
                btnEdit.Visible = false;
            }
        }

        private string CheckStatus(string prmAgreementID)
        {
            string result = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT TransStatus FROM PurchAgreementH WHERE AgreementID = '" + prmAgreementID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToString(Dr["TransStatus"]);
            }
            Dr.Close();
            Conn.Close();

            return result;
        }

        private int CheckDataComplete(string prmAgreementID)
        {
            int result = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(d.AgreementID) AS CountData FROM PurchAgreementH h ";
            Query += "INNER JOIN PurchAgreementDtl d ";
            Query += "ON d.AgreementID = h.AgreementID ";
            Query += "WHERE h.TransStatus = '03' AND h.AgreementID = '" + prmAgreementID + "' ";
            Query += "GROUP BY d.AgreementID HAVING SUM(d.RemainingQty) = 0 ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToInt32(Dr["CountData"]);
            }
            Dr.Close();
            Conn.Close();

            return result;
        }

        private DateTime FormateDateddmmyyyy(string tmpDate)
        {
            DateTime dtFinaldate; string sDateTime;
            if (tmpDate == "")
            {
                tmpDate = "01/01/1900";
            }
            try { dtFinaldate = Convert.ToDateTime(tmpDate); }
            catch (Exception e)
            {
                string[] data;
                if (tmpDate.Contains(' '))
                {
                    string[] data1 = tmpDate.Split(' ');
                    data = data1[0].Split('/');
                }
                else
                {
                    data = tmpDate.Split('/');
                }
                sDateTime = data[2] + "-" + data[1] + "-" + data[0];
                dtFinaldate = Convert.ToDateTime(sDateTime);
            }
            return dtFinaldate;
        }

        private int CheckSeqNoGroup()
        {
            for (int j = 1; j <= 1000000; j++)
            {
                for (int i = 0; i < dgvPA.RowCount; i++)
                {
                    if (Convert.ToInt32(dgvPA.Rows[i].Cells["SeqNoGroup"].Value) == j)
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

        public void AddDataGridDetail(String AgreementId, List<string> SeqNoGroup)
        {
            int TmpSeqNoGroup = CheckSeqNoGroup();
            // dgvPA.Rows.Clear();

            if (dgvPA.RowCount - 1 <= 0)
            {
                string[] DataGridViewHeader = new string[] { };
                GenerateDatagridViewHeader(ref DataGridViewHeader);
            }
            int rowno = 0;
            for (int i = 0; i < SeqNoGroup.Count; i++)
            {
                if (i > 0)
                {
                    if (Convert.ToInt32(SeqNoGroup[i]) > Convert.ToInt32(SeqNoGroup[i - 1]))
                    {
                        TmpSeqNoGroup++;
                    }
                }

                Conn = ConnectionString.GetConnection();

                Query = "Select * From [PurchAgreementH] h LEFT JOIN [PurchAgreementDtl] d ON h.AgreementId = d.AgreementId ";
                Query += "Where h.AgreementId = '" + AgreementId + "' And d.SeqNoGroup = '" + SeqNoGroup[i] + "' Order By d.SeqNo Asc";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    //Done
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        //in remaining account column the value is available date // old add row
                        //this.dgvPA.Rows.Add(rowno + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["RemainingQty"]/*Dr["Qty"]*/, Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy"), Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"],/*DelivMethod*/"", Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy"), /*Total*/ "", /*DiscType*/"",/*Disc.%*/"",/*Disc.Amount*/"",/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                        this.dgvPA.Rows.Add(rowno + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Qty"]/*Dr["Qty"]*/, Dr["RemainingQty"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"],/*DelivMethod*/"", Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), /*Total*/ "", /*DiscType*/"",/*Disc.%*/"",/*Disc.Amount*/"",/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        //in remaining account column the value is available date // old add row
                        //this.dgvPA.Rows.Add(rowno + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["RemainingAmount"]/*Dr["Amount"]*/, Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy"), Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"],/*DelivMethod*/"", Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy"), /*Total*/ "", /*DiscType*/"",/*Disc.%*/"",/*Disc.Amount*/"",/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                        this.dgvPA.Rows.Add(rowno + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Amount"]/*Dr["Amount"]*/, Dr["RemainingAmount"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"],/*DelivMethod*/"", Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), /*Total*/ "", /*DiscType*/"",/*Disc.%*/"",/*Disc.Amount*/"",/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                    }

                    cellValue("Select [Deskripsi] from [dbo].[DiskonScheme]");
                    cell.Value = "Select";
                    dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["Disc. Type"] = cell;

                    DeliveryMethodValue("SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod]");
                    DeliveryMethod.Value = "Select";
                    dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["DeliveryMethod"] = DeliveryMethod;

                    rowno++;
                }
                Dr.Close();
            }

            SumFooter();

            SetDataGridReadOnly();
            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(*) COUNTDATA FROM PurchH WHERE UPPER(ReffTableName) = 'PA' AND ReffId = '" + txtRefID.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            int CountDataPO = Convert.ToInt32(Cmd.ExecuteScalar());
            if (CountDataPO == 0)
            {
                dgvPA.Columns["Price"].ReadOnly = false;
            }
            else
            {
                dgvPA.Columns["Price"].ReadOnly = true;
            }

            for (int z = 0; z < dgvPA.ColumnCount; z++)
            {
                dgvPA.Columns[z].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            SetMiddleRightDatagrid();
            SetVisibleDatagrid();
            dgvPA.AutoResizeColumns();

        }

        private void SetDataGridReadOnly()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select ReffId From [PurchDtl] ";
            Query += "Where reffId = '" + txtPAID.Text + "' and (deletedby is null or deletedby='') ";
            Cmd = new SqlCommand(Query, Conn);
            string TmpPaID = (Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString());
            Conn.Close();
            if (Mode == "New")
            {
                dgvPA.ReadOnly = false;
                dgvPA.DefaultCellStyle.BackColor = Color.White;
                dgvPA.Columns["No"].ReadOnly = true;
                dgvPA.Columns["GroupId"].ReadOnly = true;
                dgvPA.Columns["SubGroup1ID"].ReadOnly = true;
                dgvPA.Columns["SubGroup2ID"].ReadOnly = true;
                dgvPA.Columns["ItemID"].ReadOnly = true;
                dgvPA.Columns["FullItemID"].ReadOnly = true;
                dgvPA.Columns["ItemName"].ReadOnly = true;
                dgvPA.Columns["BracketDesc"].ReadOnly = true;
                dgvPA.Columns["Base"].ReadOnly = true;
                if (txtPurchaseType.Text != "AMOUNT")
                {
                    dgvPA.Columns["Quantity"].ReadOnly = true;
                    dgvPA.Columns["RemainingQty"].ReadOnly = true;
                }
                if (txtPurchaseType.Text == "AMOUNT")
                {
                    dgvPA.Columns["Amount"].ReadOnly = true;
                    dgvPA.Columns["RemainingAmount"].ReadOnly = true;
                }
                dgvPA.Columns["Unit"].ReadOnly = true;
                dgvPA.Columns["Konv_Ratio"].ReadOnly = true;
                if (TmpPaID != "")
                    dgvPA.Columns["Price"].ReadOnly = false;
                else
                    dgvPA.Columns["Price"].ReadOnly = true;
                dgvPA.Columns["DeliveryMethod"].ReadOnly = true;
                dgvPA.Columns["AvailableDate"].ReadOnly = false;
                dgvPA.Columns["Total"].ReadOnly = true;
                dgvPA.Columns["Disc. Type"].ReadOnly = false;
                dgvPA.Columns["Disc. (%)"].ReadOnly = true;
                dgvPA.Columns["Disc. Amount"].ReadOnly = true;
                if (txtPurchaseType.Text == "AMOUNT")
                {
                    dgvPA.Columns["Total_PPN"].Visible = false;
                    dgvPA.Columns["Total_PPH"].Visible = false;
                }
                else
                {
                    dgvPA.Columns["Total_PPN"].ReadOnly = true;
                    dgvPA.Columns["Total_PPH"].ReadOnly = true;
                }
                dgvPA.Columns["CanvasId"].ReadOnly = true;
                dgvPA.Columns["CanvasSeqNo"].ReadOnly = true;
                dgvPA.Columns["BonusScheme"].ReadOnly = false;
                dgvPA.Columns["CashbackScheme"].ReadOnly = false;
                dgvPA.Columns["SeqNoGroup"].ReadOnly = true;
                dgvPA.Columns["Deskripsi"].ReadOnly = false;
            }
            else if (Mode == "Ammend")
            {
                dgvPA.ReadOnly = false;
                dgvPA.DefaultCellStyle.BackColor = Color.White;
                dgvPA.Columns["No"].ReadOnly = true;
                dgvPA.Columns["GroupId"].ReadOnly = true;
                dgvPA.Columns["SubGroup1ID"].ReadOnly = true;
                dgvPA.Columns["SubGroup2ID"].ReadOnly = true;
                dgvPA.Columns["ItemID"].ReadOnly = true;
                dgvPA.Columns["FullItemID"].ReadOnly = true;
                dgvPA.Columns["ItemName"].ReadOnly = true;
                dgvPA.Columns["BracketDesc"].ReadOnly = true;
                dgvPA.Columns["Base"].ReadOnly = true;
                if (txtPurchaseType.Text != "AMOUNT")
                {
                    dgvPA.Columns["Quantity"].ReadOnly = false;
                    dgvPA.Columns["RemainingQty"].ReadOnly = true;
                }
                if (txtPurchaseType.Text == "AMOUNT")
                {
                    dgvPA.Columns["Amount"].ReadOnly = false;
                    dgvPA.Columns["RemainingAmount"].ReadOnly = true;
                    dgvPA.Columns["Total_PPN"].Visible = false;
                    dgvPA.Columns["Total_PPH"].Visible = false;
                }
                dgvPA.Columns["Unit"].ReadOnly = true;
                dgvPA.Columns["Konv_Ratio"].ReadOnly = true;
                if (TmpPaID != "")
                    dgvPA.Columns["Price"].ReadOnly = false;
                else
                    dgvPA.Columns["Price"].ReadOnly = true;
                dgvPA.Columns["DeliveryMethod"].ReadOnly = true;
                dgvPA.Columns["AvailableDate"].ReadOnly = false;
                dgvPA.Columns["Total"].ReadOnly = true;
                dgvPA.Columns["Disc. Type"].ReadOnly = false;
                dgvPA.Columns["Disc. (%)"].ReadOnly = true;
                dgvPA.Columns["Disc. Amount"].ReadOnly = true;
                dgvPA.Columns["Total_PPN"].ReadOnly = true;
                dgvPA.Columns["Total_PPH"].ReadOnly = true;
                dgvPA.Columns["CanvasId"].ReadOnly = true;
                dgvPA.Columns["CanvasSeqNo"].ReadOnly = true;
                dgvPA.Columns["BonusScheme"].ReadOnly = false;
                dgvPA.Columns["CashbackScheme"].ReadOnly = false;
                dgvPA.Columns["SeqNoGroup"].ReadOnly = true;
                dgvPA.Columns["Deskripsi"].ReadOnly = false;
            }
            else if (Mode == "Edit")
            {
                dgvPA.ReadOnly = false;
                dgvPA.DefaultCellStyle.BackColor = Color.White;
                dgvPA.Columns["No"].ReadOnly = true;
                dgvPA.Columns["GroupId"].ReadOnly = true;
                dgvPA.Columns["SubGroup1ID"].ReadOnly = true;
                dgvPA.Columns["SubGroup2ID"].ReadOnly = true;
                dgvPA.Columns["ItemID"].ReadOnly = true;
                dgvPA.Columns["FullItemID"].ReadOnly = true;
                dgvPA.Columns["ItemName"].ReadOnly = true;
                dgvPA.Columns["BracketDesc"].ReadOnly = true;
                dgvPA.Columns["Base"].ReadOnly = true;
                if (txtPurchaseType.Text != "AMOUNT")
                {
                    dgvPA.Columns["Quantity"].ReadOnly = false;
                    dgvPA.Columns["RemainingQty"].ReadOnly = true;
                }
                if (txtPurchaseType.Text == "AMOUNT")
                {
                    dgvPA.Columns["Amount"].ReadOnly = false;
                    dgvPA.Columns["RemainingAmount"].ReadOnly = true;
                    dgvPA.Columns["Total_PPN"].Visible = false;
                    dgvPA.Columns["Total_PPH"].Visible = false;
                }
                dgvPA.Columns["Unit"].ReadOnly = true;
                dgvPA.Columns["Konv_Ratio"].ReadOnly = true;
                if (TmpPaID != "")
                    dgvPA.Columns["Price"].ReadOnly = false;
                else
                    dgvPA.Columns["Price"].ReadOnly = true;
                dgvPA.Columns["DeliveryMethod"].ReadOnly = true;
                dgvPA.Columns["AvailableDate"].ReadOnly = false;
                dgvPA.Columns["Total"].ReadOnly = true;
                dgvPA.Columns["Disc. Type"].ReadOnly = false;
                dgvPA.Columns["Disc. (%)"].ReadOnly = true;
                dgvPA.Columns["Disc. Amount"].ReadOnly = true;
                dgvPA.Columns["Total_PPN"].ReadOnly = true;
                dgvPA.Columns["Total_PPH"].ReadOnly = true;
                dgvPA.Columns["CanvasId"].ReadOnly = true;
                dgvPA.Columns["CanvasSeqNo"].ReadOnly = true;
                dgvPA.Columns["BonusScheme"].ReadOnly = false;
                dgvPA.Columns["CashbackScheme"].ReadOnly = false;
                dgvPA.Columns["SeqNoGroup"].ReadOnly = true;
                dgvPA.Columns["Deskripsi"].ReadOnly = false;
            }
        }

        private void RefreshNew()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select * From [CanvasSheetH] h JOIN [CanvasSheetD] d ON h.CanvasId = d.CanvasId ";
            Query += "JOIN [PurchQuotationH] qh ON d.PurchQuotId = qh.PurchQuotID ";
            Query += "JOIN [VendTable] v ON qh.VendId = v.VendId ";
            Query += "Where h.CanvasId = '" + CanvasId + "' And d.VendId = '" + VendId + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtOrderDate.Text = Dr["OrderDate"].ToString();
                txtPurchaseType.Text = Dr["TransType"].ToString();
                txtVendID.Text = Dr["VendID"].ToString();
                txtVendorName.Text = Dr["VendName"].ToString(); //hendry
                cmbCurrency.Text = Dr["CurrencyID"].ToString();
                txtCanvasID.Text = Dr["CanvasId"].ToString();
                txtQuotId.Text = Dr["PurchQuotId"].ToString();
                txtExchangeRate.Text = Convert.ToDecimal(Dr["ExchRate"]).ToString("N2");
                txtRefID.Text = "";// Dr["RefTransId"].ToString();
                //txtDiscount.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N4");
                //txtDiscount.Text = "0";
                cmbPaymentMode.SelectedItem = Dr["PaymentModeID"].ToString();
                cmbTermOfPayment.SelectedItem = Dr["TermofPayment"].ToString();
                //stv edit start
                //txtDPPercent.Text = Dr["DP"].ToString();
                //txtDPAmount.Text = Dr["DPAmount"].ToString();
                //stv edit end

                txtReferenceType.Text = "Canvas Sheet";
                txtRefID.Text = CanvasId;

                decimal pph = Convert.ToDecimal(Dr["PPH"]);
                decimal ppn = Convert.ToDecimal(Dr["PPN"]);

                cmbPPh.Text = pph.ToString("N2");
                cmbPPn.Text = ppn.ToString("N2");

                //BY: HC (S)
                if (Dr["DPType"].ToString() == "")
                    cmbDPRequired.SelectedItem = "NO";
                else
                    cmbDPRequired.SelectedItem = "YES";
                cmbDPType.Text = Dr["DPType"].ToString();
                tbxDPPercent.Text = Convert.ToDecimal(Dr["DP"]).ToString("N2");
                tbxDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                //BY: HC (E)

                txtExchangeRate.ReadOnly = false;
                cmbCurrency.Enabled = false;
                cmbPPh.Enabled = false;
                cmbPPn.Enabled = false;
            }
            Dr.Close();

            dgvPA.Rows.Clear();
            if (dgvPA.RowCount - 1 <= 0)
            {
                string[] DataGridViewHeader = new string[] { };
                GenerateDatagridViewHeader(ref DataGridViewHeader);
            }

            //MESTI CEK PQ ID YANG DITARIK
            Query = "Select qd.GroupID, qd.SubGroup1ID, qd.SubGroup2ID, qd.ItemID, qd.FullItemID, qd.ItemName, g.BracketDesc, qd.Base, d.Qty, d.CSAmount AS Amount, qd.Unit, qd.Ratio, qd.Price, qd.DeliveryMethod, qd.AvailableDate, qd.DiscType, qd.DiscPercent, qd.DiscAmount, d.CanvasId, d.CanvasSeqNo, qd.BonusScheme, qd.CashBackScheme, d.SeqNoGroup, qd.Deskripsi From [CanvasSheetH] h LEFT JOIN [CanvasSheetD] d ON h.CanvasId = d.CanvasId ";
            Query += "LEFT JOIN [PurchQuotationH] qh ON d.PurchQuotId = qh.PurchQuotID ";
            Query += "LEFT JOIN [PurchQuotation_Dtl] qd ON d.PurchQuotId = qd.PurchQuotID ";
            Query += "AND d.FullItemId=qd.FullItemID and d.DeliveryMethod = qd.DeliveryMethod AND qd.SeqNoGroup=d.SeqNoGroup ";
            Query += "LEFT JOIN [InventGelombangH] g ON qd.BracketId=g.BracketId AND qd.GelombangId = g.GelombangId ";
            Query += "Where h.CanvasId = '" + CanvasId + "' And d.VendId = '" + VendId + "' AND UPPER(d.StatusApproval) = 'YES' and d.PurchQuotId = '" + PurchQuotId + "'";

            //Query = "Select * From [CanvasSheetH] h JOIN [CanvasSheetD] d ON h.CanvasId = d.CanvasId ";
            //Query += "JOIN [PurchQuotationH] qh ON d.PurchQuotId = qh.PurchQuotID ";
            //Query += "LEFT JOIN [PurchQuotation_Dtl] qd ON qh.PurchQuotId = qd.PurchQuotID ";
            //Query += "JOIN [VendTable] v ON qh.VendId = v.VendId ";
            //Query += "Where h.CanvasId = '" + CanvasId + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int i = 0; int a = 0;
            while (Dr.Read())
            {
                //Done

                cellValue("Select [Deskripsi] from [dbo].[DiskonScheme]");
                cell.Value = "Select";

                if (txtPurchaseType.Text != "AMOUNT")
                {
                    //this.dgvPA.Rows.Add(i + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Qty"], Dr["Qty"], Dr["Unit"], Dr["Ratio"], Dr["Price"],/*DelivMethod*/"", Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy"), /*Total*/ "", /*DiscType*/"",/*Disc.%*/"",/*Disc.Amount*/"",/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);    
                    this.dgvPA.Rows.Add(i + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Qty"], Dr["Qty"], Dr["Unit"], Dr["Ratio"], Dr["Price"], Dr["DeliveryMethod"], Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), /*Total*/ "", cell.Value = Dr["DiscType"], Dr["DiscPercent"], Dr["DiscAmount"],/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                }
                else if (txtPurchaseType.Text == "AMOUNT")
                {
                    //this.dgvPA.Rows.Add(i + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Amount"], Dr["Amount"], Dr["Unit"], Dr["Ratio"], Dr["Price"],/*DelivMethod*/"", Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd-MM-yyyy"), /*Total*/ "", /*DiscType*/"",/*Disc.%*/"",/*Disc.Amount*/"",/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                    this.dgvPA.Rows.Add(i + 1, Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Amount"], Dr["Amount"], Dr["Unit"], Dr["Ratio"], Dr["Price"], Dr["DeliveryMethod"], Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), /*Total*/ "", cell.Value = Dr["DiscType"], Dr["DiscPercent"], Dr["DiscAmount"],/*TotalPPN*/"",/*TotalPPH*/"", Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                }

                dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["Disc. Type"] = cell;

                //REMARKED BY: HC (S)
                //DeliveryMethodValue("SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod]");
                //DeliveryMethod.Value = "Select";
                //dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["DeliveryMethod"] = DeliveryMethod;
                //REMARKED BY: HC (E)

                i++;
            }

            Dr.Close();
            SumFooter();

        }

        private void RefreshView()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT PA.DueDate,PA.OrderDate,PA.TransType,PA.VendId,VT.VendName,PA.CanvasID,PA.Deskripsi,";
            Query += "PA.CurrencyID,PA.ExchRate,PA.DPPercent, PA.DPAmount,PA.PurchQuotId,PA.RefTransId,PA.PaymentMode,PA.TermofPayment,";
            Query += "PA.PPH,PA.PPN,PA.Total,PA.Total_Disk,PA.Total_PPN,PA.Total_PPH, PA.GrandTotal, PA.ReferenceType, PA.DPType ";
            Query += "FROM PurchAgreementH PA LEFT JOIN VendTable VT ON VT.VendId=PA.VendID Where AgreementID = '" + AgreementId + "'"; //Hendry join vend  table
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPAID.Text = AgreementId;
                dtDueDate.Text = Dr["DueDate"].ToString();
                dtOrderDate.Text = Dr["OrderDate"].ToString();
                txtPurchaseType.Text = Dr["TransType"].ToString();
                txtVendID.Text = Dr["VendId"].ToString();
                txtVendorName.Text = Dr["VendName"].ToString(); //hendry
                //txtCanvasID.Text = Dr["CanvasID"].ToString();
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                cmbCurrency.Text = Dr["CurrencyID"].ToString();
                txtExchangeRate.Text = Convert.ToDecimal(Dr["ExchRate"]).ToString("N2");
                txtReferenceType.Text = Dr["ReferenceType"].ToString();
                if (Dr["ReferenceType"].ToString() == "Canvas Sheet")
                {
                    txtRefID.Text = Dr["CanvasId"].ToString();
                }
                else if (Dr["ReferenceType"].ToString() == "Purchase Agreement")
                {
                    txtRefID.Text = Dr["RefTransId"].ToString();
                }

                //BY: HC (S)
                if (Dr["DPType"].ToString() == "")
                    cmbDPRequired.SelectedItem = "NO";
                else
                    cmbDPRequired.SelectedItem = "YES";
                cmbDPType.Text = Dr["DPType"].ToString();
                tbxDPPercent.Text = Convert.ToDecimal(Dr["DPPercent"]).ToString("N2");
                tbxDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                //BY: HC (E)

                cmbPaymentMode.SelectedItem = Dr["PaymentMode"].ToString();
                cmbTermOfPayment.SelectedItem = Dr["TermofPayment"].ToString();

                cmbPPh.Text = Dr["PPH"].ToString();
                cmbPPn.Text = Dr["PPN"].ToString();
                cmbDPRequired.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");

                txtTotal.Text = Convert.ToDecimal(Dr["Total"]).ToString("N4");
                txtDiscount.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N2");
                txtTotalPPN.Text = Convert.ToDecimal(Dr["Total_PPN"]).ToString("N2");
                txtTotalPPH.Text = Convert.ToDecimal(Dr["Total_PPH"]).ToString("N2");
                txtGrandTotal.Text = Convert.ToDecimal(Dr["GrandTotal"]).ToString("N2");
                //txtDiscount.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N4");

                txtExchangeRate.ReadOnly = false;
                cmbCurrency.Enabled = false;
                cmbPPh.Enabled = false;
                cmbPPn.Enabled = false;
            }
            Dr.Close();
            Conn.Close();

            dgvPA.Rows.Clear();
            if (dgvPA.RowCount - 1 <= 0)
            {
                string[] DataGridViewHeader = new string[] { };
                GenerateDatagridViewHeader(ref DataGridViewHeader);
            }

            Conn = ConnectionString.GetConnection();
            if (txtPurchaseType.Text != "AMOUNT")
            {
                Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, BracketDesc, Base, [Qty], RemainingQty, [Unit], Konv_Ratio, [Price], Total, Total_PPN, Total_PPH,CanvasId, CanvasSeqNo, Deskripsi, AvailableDate, BonusScheme, CashBackScheme,SeqNoGroup,DeliveryMethod,DiscType,DiscPercentage,DiscAmount From [PurchAgreementDtl] Where AgreementID = '" + AgreementId + "' order by SeqNo asc";
            }
            else if (txtPurchaseType.Text == "AMOUNT")
            {
                Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, BracketDesc, Base, [Amount], RemainingAmount, [Unit], Konv_Ratio, [Price], Total, Total_PPN, Total_PPH,CanvasId, CanvasSeqNo, Deskripsi, AvailableDate, BonusScheme, CashBackScheme,SeqNoGroup,DeliveryMethod,DiscType,DiscPercentage,DiscAmount  From [PurchAgreementDtl] Where AgreementID = '" + AgreementId + "' order by SeqNo asc";

            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                //hasim update 28 Okt 2018
                if (txtPurchaseType.Text != "AMOUNT")
                {
                    this.dgvPA.Rows.Add(Dr["SeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["DeliveryMethod"], Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), Dr["Total"], Dr["DiscType"], Dr["DiscPercentage"], Dr["DiscAmount"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                }
                else if (txtPurchaseType.Text == "AMOUNT")
                {
                    this.dgvPA.Rows.Add(Dr["SeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Amount"], Dr["RemainingAmount"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["DeliveryMethod"], Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), Dr["Total"], Dr["DiscType"], Dr["DiscPercentage"], Dr["DiscAmount"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"]);
                }

                //REMARKED BY: HC (S)
                //cellValue("Select [Deskripsi] from [dbo].[DiskonScheme]");
                //cell.Value = Dr["DiscType"].ToString();
                //dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["Disc. Type"] = cell;

                //DeliveryMethodValue("SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod]");
                //DeliveryMethod.Value = Dr["DeliveryMethod"].ToString();
                //dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["DeliveryMethod"] = DeliveryMethod;
                //REMARKED BY: HC (E)

                i++;
            }
            Dr.Close();
        }

        private void RefreshAmend()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT PA.DueDate,PA.OrderDate,PA.TransType,PA.VendId,VT.VendName,PA.CanvasID,PA.Deskripsi,";
            Query += "PA.CurrencyID,PA.ExchRate,PA.DPPercent, PA.DPAmount,PA.PurchQuotId,PA.RefTransId,PA.PaymentMode,PA.TermofPayment,";
            Query += "PA.PPH,PA.PPN,PA.Total,PA.Total_Disk,PA.Total_PPN,PA.Total_PPH, PA.GrandTotal, PA.ReferenceType, PA.DPType ";
            Query += "FROM PurchAgreementH PA LEFT JOIN VendTable VT ON  VT.VendId=PA.VendID Where AgreementID = '" + AgreementId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtPAID.Text = AgreementId;
                dtDueDate.Text = Dr["DueDate"].ToString();
                dtOrderDate.Text = Dr["OrderDate"].ToString();
                txtPurchaseType.Text = Dr["TransType"].ToString();
                txtVendID.Text = Dr["VendId"].ToString();
                txtVendorName.Text = Dr["VendName"].ToString(); //Hendry
                //txtCanvasID.Text = Dr["CanvasID"].ToString();
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                cmbCurrency.Text = Dr["CurrencyID"].ToString();
                txtExchangeRate.Text = Convert.ToDecimal(Dr["ExchRate"]).ToString("N2");
                //txtQuotId.Text = Dr["PurchQuotId"].ToString();
                txtRefID.Text = Dr["RefTransId"].ToString();
                cmbPaymentMode.SelectedItem = Dr["PaymentMode"].ToString();
                cmbTermOfPayment.SelectedItem = Dr["TermofPayment"].ToString();
                txtReferenceType.Text = "Purchase Agreement";
                txtRefID.Text = AgreementId;

                cmbPPh.Text = Dr["PPH"].ToString();
                cmbPPn.Text = Dr["PPN"].ToString();
                //txtTotal.Text = Convert.ToDecimal(Dr["Total"]).ToString("N4");
                //txtTotalPPN.Text = Convert.ToDecimal(Dr["Total_PPN"]).ToString("N4");
                //txtTotalPPH.Text = Convert.ToDecimal(Dr["Total_PPH"]).ToString("N4");
                //txtDiscount.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N4");
                txtDiscount.Text = "0";

                //if (Convert.ToDecimal(txtDPAmount.Text) <= 0)
                //{
                //    cmbDPRequired.SelectedItem = "NO";
                //    txtDPPercent.ReadOnly = true;
                //    txtDPAmount.ReadOnly = true;
                //    txtDPPercent.Text = "0.00";
                //    txtDPAmount.Text = "0.00";
                //}
                //else
                //{
                //    cmbDPRequired.SelectedItem = "YES";
                //    txtDPPercent.ReadOnly = true;
                //    txtDPAmount.ReadOnly = true;
                //}

                //BY: HC (S)
                if (Dr["DPType"].ToString() == "")
                    cmbDPRequired.SelectedItem = "NO";
                else
                    cmbDPRequired.SelectedItem = "YES";
                cmbDPType.Text = Dr["DPType"].ToString();
                tbxDPPercent.Text = Convert.ToDecimal(Dr["DPPercent"]).ToString("N2");
                tbxDPAmount.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");
                txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                //BY: HC (E)

                cmbPaymentMode.SelectedItem = Dr["PaymentMode"].ToString();
                cmbTermOfPayment.SelectedItem = Dr["TermofPayment"].ToString();

                cmbPPh.Text = Dr["PPH"].ToString();
                cmbPPn.Text = Dr["PPN"].ToString();
                cmbDPRequired.Text = Convert.ToDecimal(Dr["DPAmount"]).ToString("N2");

                txtTotal.Text = Convert.ToDecimal(Dr["Total"]).ToString("N2");
                txtDiscount.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N2");
                txtTotalPPN.Text = Convert.ToDecimal(Dr["Total_PPN"]).ToString("N2");
                txtTotalPPH.Text = Convert.ToDecimal(Dr["Total_PPH"]).ToString("N2");
                txtGrandTotal.Text = Convert.ToDecimal(Dr["GrandTotal"]).ToString("N2");
                //txtDiscount.Text = Convert.ToDecimal(Dr["Total_Disk"]).ToString("N4");

                //txtDPPercent.Text = Dr["DPPercent"].ToString();
                ////txtDPAmount.Text = Dr["DPAmount"].ToString();
                //txtDPAmount.Text = (Convert.ToDecimal(txtGrandTotal.Text) * Convert.ToDecimal(txtDPPercent.Text) / 100).ToString("N2");

                txtExchangeRate.ReadOnly = true;
                cmbCurrency.Enabled = true;
                cmbPPh.Enabled = true;
                cmbPPn.Enabled = true;
            }
            Dr.Close();
            Conn.Close();

            //ga kepake soal nya bakal di clear juga terakhirnya.
            dgvPA.Rows.Clear();
            if (dgvPA.RowCount - 1 <= 0)
            {
                string[] DataGridViewHeader = new string[] { };
                GenerateDatagridViewHeader(ref DataGridViewHeader);
            }

            ////if (ControlMgr.GroupName == "SalesManager")
            ////{
            ////    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[Qty],[Unit],[VendID],[Deskripsi],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' order by SeqNo asc";
            ////}
            ////else if (ControlMgr.GroupName == "PurchaseManager")
            ////{
            ////    Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[ReffTransID],[ExpectedDateFrom],[ExpectedDateTo],[Qty],[Unit],[VendID],[Deskripsi],[TransStatus],[ApprovePerson],[ApprovalNotes],[TransStatusPurch],[ApprovePersonPurch],[ApprovalNotesPurch] From [PurchRequisition_Dtl] Where PurchReqID = '" + PurchReqID + "' And TransStatus = 'Yes' order by SeqNo asc";
            ////}
            Conn = ConnectionString.GetConnection();
            if (txtPurchaseType.Text != "AMOUNT")
            {
                Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, BracketDesc, Base, [Qty], RemainingQty, [Unit], Konv_Ratio, [Price], Total, Total_PPN, Total_PPH,CanvasId, CanvasSeqNo, Deskripsi, AvailableDate, BonusScheme, CashBackScheme,SeqNoGroup,DeliveryMethod,DiscType,DiscPercentage,DiscAmount ";
                Query += "From [PurchAgreementDtl] Where AgreementID = '" + AgreementId + "' order by SeqNo asc";
            }
            if (txtPurchaseType.Text == "AMOUNT")
            {
                Query = "Select SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, [FullItemID], ItemName, BracketDesc, Base, [Amount], RemainingAmount, [Unit], Konv_Ratio, [Price], Total, Total_PPN, Total_PPH,CanvasId, CanvasSeqNo, Deskripsi, AvailableDate, BonusScheme, CashBackScheme,SeqNoGroup,DeliveryMethod,DiscType,DiscPercentage,DiscAmount From [PurchAgreementDtl] Where AgreementID = '" + AgreementId + "' order by SeqNo asc";
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            while (Dr.Read())
            {
                if (txtPurchaseType.Text != "AMOUNT")
                {
                    this.dgvPA.Rows.Add(Dr["SeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Qty"], "0", Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["DeliveryMethod"], Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), Dr["Total"], Dr["DiscType"], Dr["DiscPercentage"], Dr["DiscAmount"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"], Dr["RemainingQty"]);
                }
                else if (txtPurchaseType.Text == "AMOUNT")
                {
                    this.dgvPA.Rows.Add(Dr["SeqNo"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["BracketDesc"], Dr["Base"], Dr["Amount"], "0", Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["DeliveryMethod"], Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(Dr["AvailableDate"]).ToString("dd/MM/yyyy"), Dr["Total"], Dr["DiscType"], Dr["DiscPercentage"], Dr["DiscAmount"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["CanvasId"], Dr["CanvasSeqNo"], Dr["BonusScheme"], Dr["CashBackScheme"], Dr["SeqNoGroup"], Dr["Deskripsi"], Dr["RemainingAmount"]);
                }

                //if (Dr["Base"].ToString() == "Y") //REMARKED BY: HC | GA TAU BASE N BOLEH EDIT APA KAGA
                //{
                cellValue("Select [Deskripsi] from [dbo].[DiskonScheme]");
                cell.Value = Dr["DiscType"].ToString();
                dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["Disc. Type"] = cell;

                DeliveryMethodValue("SELECT [DeliveryMethod] FROM [dbo].[DeliveryMethod]");
                DeliveryMethod.Value = Dr["DeliveryMethod"].ToString();
                dgvPA.Rows[(dgvPA.Rows.Count - 1)].Cells["DeliveryMethod"] = DeliveryMethod;
                //}
            }
            Dr.Close();
            amendSetting();
        }

        private void amendSetting()
        {
            Conn = ConnectionString.GetConnection();
            if (dgvPA.Rows.Count > 0)
            {
                for (int i = 0; i <= dgvPA.Rows.Count - 1; i++)
                {
                    String SeqNo = dgvPA.Rows[i].Cells["No"].Value == null ? "" : dgvPA.Rows[i].Cells["No"].Value.ToString();
                    String FullItemId = dgvPA.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPA.Rows[i].Cells["FullItemId"].Value.ToString();

                    Query = "SELECT [RemainingQty] FROM [ISBS-NEW4].[dbo].[PurchAgreementDtl] WHERE [AgreementID] = '" + AgreementId + "' AND [SeqNo] = '" + SeqNo + "' AND [FullItemId] = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Decimal RemainingQty = Convert.ToDecimal(Cmd.ExecuteScalar().ToString());

                    if (RemainingQty == 0)
                    {
                        if (txtPurchaseType.Text != "AMOUNT")
                            dgvPA.Rows[i].Cells["Quantity"].ReadOnly = true;
                        else if (txtPurchaseType.Text == "AMOUNT")
                            dgvPA.Rows[i].Cells["Amount"].ReadOnly = true;
                        dgvPA.Rows[i].Cells["Price"].ReadOnly = true;

                    }
                    else
                    {
                        if (txtPurchaseType.Text != "AMOUNT")
                            dgvPA.Rows[i].Cells["Quantity"].ReadOnly = false;
                        else if (txtPurchaseType.Text == "AMOUNT")
                            dgvPA.Rows[i].Cells["Amount"].ReadOnly = false;
                        dgvPA.Rows[i].Cells["Price"].ReadOnly = true;
                    }
                }
            }

            //REMARKED BY: HC (S) | GA TAU BASE N BOLEH EDIT APA KAGA
            //for (int i = 0; i < dgvPA.RowCount; i++)
            //{
            //    if (dgvPA.Rows[i].Cells["Base"].Value.ToString() == "N")
            //    {
            //        for(int j= 0; j<dgvPA.ColumnCount; j++)
            //        {
            //            dgvPA.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
            //            dgvPA.Rows[i].Cells[j].ReadOnly = true;
            //        }
            //    }
            //}
            //REMARKED BY: HC (E)
            Conn.Close();
        }

        private bool amendDeleteCheck()
        {
            Conn = ConnectionString.GetConnection();
            Boolean vBol = true;

            if (Mode == "Ammend")
            {
                if (dgvPA.RowCount > 0)
                {
                    Index = dgvPA.CurrentRow.Index;
                    String SeqNo = dgvPA.Rows[Index].Cells["No"].Value == null ? "" : dgvPA.Rows[Index].Cells["No"].Value.ToString();
                    String FullItemId = dgvPA.Rows[Index].Cells["FullItemId"].Value == null ? "" : dgvPA.Rows[Index].Cells["FullItemId"].Value.ToString();

                    Decimal RemainingQty = 0;
                    Decimal Qty = 0;

                    if (txtPurchaseType.Text == "QTY")
                    {
                        Query = "SELECT [RemainingQty],[Qty] FROM [ISBS-NEW4].[dbo].[PurchAgreementDtl] WHERE AgreementID = '" + AgreementId + "' AND [SeqNo] = '" + SeqNo + "' AND [FullItemId] = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            RemainingQty = Convert.ToDecimal(Dr["RemainingQty"].ToString());
                            Qty = Convert.ToDecimal(Dr["Qty"].ToString());
                        }
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Query = "SELECT [RemainingAmount],[Amount] FROM [ISBS-NEW4].[dbo].[PurchAgreementDtl] WHERE AgreementID = '" + AgreementId + "' AND [SeqNo] = '" + SeqNo + "' AND [FullItemId] = '" + FullItemId + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            RemainingQty = Convert.ToDecimal(Dr["RemainingAmount"].ToString());
                            Qty = Convert.ToDecimal(Dr["Amount"].ToString());
                        }
                    }

                    if (RemainingQty != Qty) //GA BISA DELETE JIKA SUDAH ADA DELIVERY
                    {
                        MessageBox.Show("Tidak bisa delte" + Environment.NewLine + "Item sudah ada delivery");
                        vBol = false;
                    }
                }
            }
            return vBol;
            Conn.Close();
        }

        private void RefreshData()
        {
            test.Clear();

            if (Mode == "New")
            {
                RefreshNew();
            }
            else if (Mode == "View")
            {
                RefreshView();
            }
            else if (Mode == "Ammend")
            {
                RefreshAmend();
            }

            RefreshAttachment();

            dgvPA.ReadOnly = true;
            dgvPA.DefaultCellStyle.BackColor = Color.LightGray;

            for (int z = 0; z < dgvPA.ColumnCount; z++)
            {
                dgvPA.Columns[z].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            SetMiddleRightDatagrid();
            SetDataGridReadOnly();
            SetVisibleDatagrid();
            HidePPnPPh();
            dgvPA.AutoResizeColumns();

            Conn.Close();

        }

        private void RefreshAttachment()
        {
            Conn = ConnectionString.GetConnection();
            dgvAttachment.Rows.Clear();
            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 5;
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "ContentType";
                dgvAttachment.Columns[2].Name = "File Size (kb)";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";

                dgvAttachment.Columns["Attachment"].Visible = false;
                dgvAttachment.Columns["Id"].Visible = false;
            }

            Query = "Select * From [tblAttachments] Where ReffTableName = 'PurchAgreementH' And ReffTransId = '" + AgreementId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                test.Add((byte[])Dr["Attachment"]);
            }

            dgvAttachment.AutoResizeColumns();
            Conn.Close();
        }

        private void saveNewPAHeader()
        {
            //begin============================================================================================
            //updated by : joshua
            //updated date : 14 Feb 2018
            //description : change generate sequence number, get from global function and update counter 
            string Jenis = "PA", Kode = "PA";
            decimal dp_persen = 0 , dp_amount = 0;
            AgreementId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

            Query = "Insert into PurchAgreementH (AgreementId,OrderDate,DueDate,TransType,CanvasId,PurchQuotID,CurrencyID,ExchRate,VendID,Total,Total_Disk,PPN,Total_PPN,PPH,Total_PPH,Deskripsi,CreatedDate,CreatedBy, TransStatus, ApprovedBy, DPPercent, DPAmount, ReferenceType, GrandTotal, TermOfPayment, PaymentMode, DPType) values ";
            Query += "(@agreementID, @OrderDate, @DueDate, @TransType, @CanvasId, @PurchQuotID, @CurrencyID, @ExchRate,";
            Query += "@VendID, @Total, @Total_Disk, @PPN, @Total_PPN, @PPH, @Total_PPH, @Notes, getdate(),'" + ControlMgr.UserId + "', '03', '" + ControlMgr.UserId + "', @DPPercent, @DPAmount,";

            //BY: HC (S)
            if (cmbDPRequired.Text == "YES")
            {
                if (cmbDPType.Text == "Percentage")
                {
                    dp_persen = Convert.ToDecimal(tbxDPPercent.Text) ;
                    dp_amount = 0;
                }
                else if (cmbDPType.Text == "Amount")
                {
                    dp_persen = 0;
                    dp_amount = Convert.ToDecimal(tbxDPAmount.Text);
                }
            }
            //BY: HC (E)

            //REMARKED BY: HC (S)
            //Query += "'" + (txtDPPercent.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDPPercent.Text).ToString()) + "',";
            //Query += "'" + (txtDPAmount.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDPAmount.Text).ToString()) + "',";
            //REMARKED BY: HC (E)

            Query += "@ReferenceType, @GrandTotal, @TermOfPayment, @PaymentMode, ";
            //BY: HC (S)
            if (cmbDPRequired.Text == "YES")
            {
                Query += "'" + cmbDPType.Text + "')";
            }
            else if (cmbDPRequired.Text == "NO")
            {
                Query += "NULL)";
            }
            //BY: HC (E)
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.AddWithValue("@agreementID", AgreementId);
            Cmd.Parameters.AddWithValue("@OrderDate", dtOrderDate.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@DueDate", dtDueDate.Value.ToString("yyyy-MM-dd"));
            Cmd.Parameters.AddWithValue("@TransType", txtPurchaseType.Text);
            Cmd.Parameters.AddWithValue("@CanvasId", CanvasId);
            Cmd.Parameters.AddWithValue("@PurchQuotID", txtQuotId.Text);
            Cmd.Parameters.AddWithValue("@CurrencyID", cmbCurrency.Text);
            Cmd.Parameters.AddWithValue("@ExchRate", (txtExchangeRate.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtExchangeRate.Text).ToString()));
            Cmd.Parameters.AddWithValue("@VendID", txtVendID.Text);
            Cmd.Parameters.AddWithValue("@Total", (txtTotal.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotal.Text).ToString()));
            Cmd.Parameters.AddWithValue("@Total_Disk", (txtDiscount.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDiscount.Text).ToString()));
            Cmd.Parameters.AddWithValue("@PPN", (cmbPPn.Text == "" ? "0.00" : cmbPPn.Text));
            Cmd.Parameters.AddWithValue("@Total_PPN", (txtTotalPPN.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotalPPN.Text).ToString()));
            Cmd.Parameters.AddWithValue("@PPH", (cmbPPh.Text == "" ? "0.00" : cmbPPh.Text));
            Cmd.Parameters.AddWithValue("@Total_PPH", (txtTotalPPH.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotalPPH.Text).ToString()));
            Cmd.Parameters.AddWithValue("@Notes", txtDeskripsi.Text.Trim());
            Cmd.Parameters.AddWithValue("@DPPercent", dp_persen);
            Cmd.Parameters.AddWithValue("@DPAmount", dp_amount);
            Cmd.Parameters.AddWithValue("@ReferenceType", txtReferenceType.Text.ToString());
            Cmd.Parameters.AddWithValue("@GrandTotal", (txtGrandTotal.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtGrandTotal.Text).ToString()));
            Cmd.Parameters.AddWithValue("@TermOfPayment", cmbTermOfPayment.Text);
            Cmd.Parameters.AddWithValue("@PaymentMode", cmbPaymentMode.Text);
            Cmd.ExecuteNonQuery();
        }

        private void saveNewPADetail()
        {
            Query = "";
            for (int i = 0; i <= dgvPA.RowCount - 1; i++)
            {
                if (dgvPA.Rows[i].Cells["AvailableDate"].Value == null || dgvPA.Rows[i].Cells["AvailableDate"].Value == "")
                    dgvPA.Rows[i].Cells["AvailableDate"].Value = "01/01/1900";

                if (txtPurchaseType.Text != "AMOUNT")
                    Query += "Insert PurchAgreementDtl (AgreementId,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,BracketDesc,Base,Qty,Qty_KG,RemainingQty,Unit,Konv_Ratio,Price,Price_KG,Total,DiscType,DiscPercentage,DiscAmount,Total_PPN,Total_PPH,CanvasId,CanvasSeqNo,AvailableDate,BonusScheme,CashBackScheme,Deskripsi,SeqNoGroup,CreatedDate,CreatedBy,DeliveryMethod) Values ";
                if (txtPurchaseType.Text == "AMOUNT")
                    Query += "Insert PurchAgreementDtl (AgreementId,OrderDate,SeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,BracketDesc,Base,Amount,Qty_KG,RemainingAmount,Unit,Konv_Ratio,Price,Price_KG,Total,DiscType,DiscPercentage,DiscAmount,Total_PPN,Total_PPH,CanvasId,CanvasSeqNo,AvailableDate,BonusScheme,CashBackScheme,Deskripsi,SeqNoGroup,CreatedDate,CreatedBy,DeliveryMethod) Values ";

                Query += "('" + AgreementId + "','";
                Query += (dtOrderDate.Value == null ? "" : dtOrderDate.Value.ToString("yyyy-MM-dd")) + "','";
                Query += (dgvPA.Rows[i].Cells["No"].Value == "" ? "" : dgvPA.Rows[i].Cells["No"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvPA.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["SubGroup1Id"].Value == "" ? "" : dgvPA.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvPA.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvPA.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvPA.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvPA.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["BracketDesc"].Value == "" ? "" : dgvPA.Rows[i].Cells["BracketDesc"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["Base"].Value == "" ? "" : dgvPA.Rows[i].Cells["Base"].Value.ToString()) + "','";


                Price = Convert.ToDecimal(dgvPA.Rows[i].Cells["Price"].Value);
                Konv_Ratio = Convert.ToDecimal(dgvPA.Rows[i].Cells["Konv_Ratio"].Value);

                if (txtPurchaseType.Text != "AMOUNT")
                {
                    Query += (dgvPA.Rows[i].Cells["Quantity"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Quantity"].Value.ToString()) + "','"; //Qty

                    Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Quantity"].Value);

                    if (dgvPA.Rows[i].Cells["Unit"].Value.ToString() == "BTG") //Qty_KG
                    {
                        Query += (Qty / Konv_Ratio) + "','";
                    }
                    else
                    {
                        Query += Qty + "','";
                    }
                    Query += (dgvPA.Rows[i].Cells["RemainingQty"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["RemainingQty"].Value.ToString()) + "','";
                }
                else if (txtPurchaseType.Text == "AMOUNT")
                {
                    Query += (dgvPA.Rows[i].Cells["Amount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Amount"].Value.ToString()) + "','";

                    Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Amount"].Value);

                    Query += Qty + "','";
                    Query += (dgvPA.Rows[i].Cells["RemainingAmount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["RemainingAmount"].Value.ToString()) + "','";
                }

                Query += (dgvPA.Rows[i].Cells["Unit"].Value == null ? "" : dgvPA.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["Konv_Ratio"].Value == "" ? "0.0000" : dgvPA.Rows[i].Cells["Konv_Ratio"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["Price"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Price"].Value.ToString()) + "','";

                if (dgvPA.Rows[i].Cells["Unit"].Value.ToString() == "BTG")
                {
                    Query += (Price / Konv_Ratio) + "','";
                }
                else
                {
                    Query += Price + "','";
                }
                Query += (dgvPA.Rows[i].Cells["Total"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total"].Value.ToString()) + "','";
                //BY: HC (S)
                if (dgvPA.Rows[i].Cells["Disc. Type"].Value == "" || dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() == "Select")
                    Query += "Amount', '";
                else
                    Query += dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() + "', '";
                //BY: HC (E)
                //Query += (dgvPA.Rows[i].Cells["Disc. Type"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString()) + "','"; //REMARKED BY: HC
                Query += (dgvPA.Rows[i].Cells["Disc. (%)"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. (%)"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["Disc. Amount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. Amount"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["Total_PPN"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total_PPN"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["Total_PPH"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total_PPH"].Value.ToString()) + "','";
                Query += CanvasId + "','";
                Query += (dgvPA.Rows[i].Cells["CanvasSeqNo"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["CanvasSeqNo"].Value.ToString()) + "',";
                Query += "@availdate,'";
                Query += (dgvPA.Rows[i].Cells["BonusScheme"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["BonusScheme"].Value.ToString()) + "','";
                Query += (dgvPA.Rows[i].Cells["CashBackScheme"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["CashBackScheme"].Value.ToString()) + "',";
                Query += "@desc,'";
                Query += (dgvPA.Rows[i].Cells["SeqNoGroup"].Value == "" ? "" : dgvPA.Rows[i].Cells["SeqNoGroup"].Value.ToString()) + "',";
                Query += "getdate(),'";
                Query += ControlMgr.UserId + "','";
                Query += dgvPA.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "');";


                //if (i % 5 == 0 && i > 0)
                //{
                adate = FormateDateddmmyyyy(dgvPA.Rows[i].Cells["AvailableDate"].Value.ToString());
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.AddWithValue("@availdate", adate);
                Cmd.Parameters.AddWithValue("@desc", (dgvPA.Rows[i].Cells["Deskripsi"].Value == "" ? "" : dgvPA.Rows[i].Cells["Deskripsi"].Value.ToString().Trim()));
                Cmd.ExecuteNonQuery();
                Query = "";
                //}

            }

            #region update stockview
            Query = "EXEC [dbo].[sv_pa_issued] @pa_id, @amount_or_qty; ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.AddWithValue("@pa_id", AgreementId);
            Cmd.Parameters.AddWithValue("@amount_or_qty", txtPurchaseType.Text);
            Cmd.ExecuteNonQuery();
            #endregion
        }

        private void saveNewAttachment()
        {
            for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
            {
                Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                Query += "( 'PurchAgreementH', '" + AgreementId + "', '";
                Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";

                Query += dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString();
                Query += "',@binaryValue";
                Query += ");";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                Cmd.ExecuteNonQuery();
            }
        }

        private void saveNew()
        {
            try
            {
                saveNewPAHeader();

                //created by : Thaddaeus Matthias, 14 March 2018
                //Insert into status log table supplier/vendor
                //========================================begin=============================================
                insertstatuslog(AgreementId);
                //=========================================end==============================================

                saveNewPADetail();

                saveNewAttachment();

                insertPA_LogTable(AgreementId, "", "Approved by Purchasing Manager - ALL", "N"); //BY: HC 

                Trans.Commit();
                MessageBox.Show("Data PA Number : " + AgreementId + " berhasil ditambahkan.");

                // this.Close();

                txtPAID.Text = AgreementId;

                ModeView();
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            finally
            {
                Conn.Close();
                Parent.ModeLoad();
            }
        }

        private void insertPA_LogTable(string PAID, string desc, string statusTransaksi, string action)
        {
            //BY: HC (S)
            Query = "INSERT INTO [dbo].[PurchAgreement_LogTable] ([TransaksiID],[Deskripsi],[StatusTransaksi],[Action],[UserID],[LogDatetime]) VALUES ('" + PAID + "', '', 'Approved by Purchasing Manager - ALL' , 'N', '" + ControlMgr.UserId + "', getdate()); ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
            //BY: HC (E)
        }

        private void saveAmmend()
        {
            try
            {
                //begin============================================================================================
                //updated by : joshua
                //updated date : 26 Feb 2018
                //description : change generate sequence number, get from global function and update counter 
                string Jenis = "PA", Kode = "PA";
                string AgreementId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "PurchAgreementH", txtRefID.Text, Conn, Trans, Cmd);
                Query = "Insert into PurchAgreementH (AgreementId,OrderDate,DueDate,RefTransID,TransType,CanvasId,PurchQuotID,CurrencyID,ExchRate,VendID,Total,Total_Disk,PPN,Total_PPN,PPH,Total_PPH,Deskripsi,CreatedDate,CreatedBy, TransStatus, DPPercent, DPAmount, ReferenceType, GrandTotal, TermOfPayment, PaymentMode, DPType) values ";
                Query += "('" + AgreementId + "',";
                Query += "'" + dtOrderDate.Value.ToString("yyyy-MM-dd") + "',";
                Query += "'" + dtDueDate.Value.ToString("yyyy-MM-dd") + "',";
                Query += "'" + oldPAID + "',";
                Query += "'" + txtPurchaseType.Text + "',";
                //BY: HC (S) | GET CANVAS ID & PQ ID
                Cmd = new SqlCommand("select CanvasId, PurchQuotID from PurchAgreementH where AgreementId = '" + txtRefID.Text.Substring(0, 13) + "'", Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    Query += "'" + Dr["CanvasId"] + "',";
                    Query += "'" + Dr["PurchQuotID"] + "',";
                }
                Dr.Close();
                //BY: HC (E)
                //REMARKED BY: HC (S)
                //Query += "'" + txtCanvasID.Text + "',";
                //Query += "'" + txtQuotId.Text + "',";
                //REMARKED BY: HC (E)
                Query += "'" + cmbCurrency.Text + "',";
                Query += "'" + (txtExchangeRate.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtExchangeRate.Text).ToString()) + "',";
                Query += "'" + txtVendID.Text + "',";
                Query += "'" + (txtTotal.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotal.Text).ToString()) + "',";
                Query += "'" + (txtDiscount.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDiscount.Text).ToString()) + "',";
                Query += "'" + (cmbPPn.Text == "" ? "0.00" : cmbPPn.Text) + "',";
                Query += "'" + (txtTotalPPN.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotalPPN.Text).ToString()) + "',";
                Query += "'" + (cmbPPh.Text == "" ? "0.00" : cmbPPh.Text) + "',";
                Query += "'" + (txtTotalPPH.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotalPPH.Text).ToString()) + "',";
                Query += "@Notes,";
                Query += "getdate(),'" + ControlMgr.UserId + "', '02',";
                //BY: HC (S)
                if (cmbDPRequired.Text == "YES")
                {
                    if (cmbDPType.Text == "Percentage")
                    {
                        Query += "'" + tbxDPPercent.Text + "', ";
                        Query += "'0', ";
                    }
                    else if (cmbDPType.Text == "Amount")
                    {
                        Query += "'0', ";
                        Query += "'" + tbxDPAmount.Text + "', ";
                    }
                }
                else if (cmbDPRequired.Text == "NO")
                {
                    Query += "'0', '0', ";
                }
                //BY: HC (E)

                //REMARKED BY: HC (S)
                //Query += "'" + (txtDPPercent.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDPPercent.Text).ToString()) + "',";
                //Query += "'" + (txtDPAmount.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDPAmount.Text).ToString()) + "',";
                //REMARKED BY: HC (E)
                Query += "'" + txtReferenceType.Text.ToString() + "',";
                Query += "'" + (txtGrandTotal.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtGrandTotal.Text).ToString()) + "',";
                Query += "'" + cmbTermOfPayment.Text + "',";
                Query += "'" + cmbPaymentMode.Text + "', ";
                //BY: HC (S)
                if (cmbDPRequired.Text == "YES")
                {
                    Query += "'" + cmbDPType.Text + "')";
                }
                else if (cmbDPRequired.Text == "NO")
                {
                    Query += "NULL)";
                }
                //BY: HC (E)
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.AddWithValue("@Notes", txtDeskripsi.Text.Trim());  //Hasim edit
                Cmd.ExecuteNonQuery();
                //end==================================================================================================

                Query = "Update PurchAgreementH Set StClose = 1, TransStatus = '06' Where AgreementId = '" + oldPAID + "' ";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                //BY: HC (S)
                insertPA_LogTable(AgreementId, "", "Amend - Waiting for Approval", "N");
                insertPA_LogTable(oldPAID, "", "Closed", "N");
                //BY: HC (E)

                //created by : Thaddaeus Matthias, 14 March 2018
                //Insert into status log table supplier/vendor
                //========================================begin=============================================
                insertstatuslogclose();
                insertstatuslog(AgreementId);
                //=========================================end==============================================

                Query = "";
                string tempQuery = "";

                for (int i = 0; i <= dgvPA.RowCount - 1; i++)
                {
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        if (dgvPA.Rows[i].Cells["Quantity"].Value == "" || dgvPA.Rows[i].Cells["Quantity"].Value == System.DBNull.Value)
                            dgvPA.Rows[i].Cells["Quantity"].Value = "0";
                        if (dgvPA.Rows[i].Cells["RemainingQty"].Value == "" || dgvPA.Rows[i].Cells["RemainingQty"].Value == System.DBNull.Value)
                            dgvPA.Rows[i].Cells["RemainingQty"].Value = "0";
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        if (dgvPA.Rows[i].Cells["Amount"].Value == null || dgvPA.Rows[i].Cells["Amount"].Value == System.DBNull.Value)
                            dgvPA.Rows[i].Cells["Amount"].Value = "0";
                        if (dgvPA.Rows[i].Cells["RemainingAmount"].Value == null || dgvPA.Rows[i].Cells["RemainingAmount"].Value == System.DBNull.Value)
                            dgvPA.Rows[i].Cells["RemainingAmount"].Value = "0";
                    }

                    if (dgvPA.Rows[i].Cells["AvailableDate"].Value == null || dgvPA.Rows[i].Cells["AvailableDate"].Value == "")
                        dgvPA.Rows[i].Cells["AvailableDate"].Value = "01/01/1900";

                    string seqno = "";

                    Price = Convert.ToDecimal(dgvPA.Rows[i].Cells["Price"].Value);
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        //Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Quantity"].Value);
                        Qty = dgvPA.Rows[i].Cells["Quantity"].Value == System.DBNull.Value ? 0 : Convert.ToDecimal(dgvPA.Rows[i].Cells["Quantity"].Value);
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        //Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Amount"].Value);
                        Qty = dgvPA.Rows[i].Cells["Amount"].Value == System.DBNull.Value ? 0 : Convert.ToDecimal(dgvPA.Rows[i].Cells["Amount"].Value);
                    }
                    Konv_Ratio = Convert.ToDecimal(dgvPA.Rows[i].Cells["Konv_Ratio"].Value);

                    tempQuery = "Select SeqNo From PurchAgreementDtl Where AgreementID = '" + oldPAID + "' and FullItemID = '" + dgvPA.Rows[i].Cells["FullItemID"].Value.ToString() + "' and SeqNoGroup = '" + dgvPA.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "'";
                    Cmd = new SqlCommand(tempQuery, Conn, Trans);
                    seqno = Cmd.ExecuteScalar().ToString();

                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        Query += "Insert PurchAgreementDtl (AgreementId,OrderDate,SeqNo,RefTransId,RefTransSeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,BracketDesc,Base,Qty,Qty_KG,RemainingQty,Unit,Konv_Ratio,Price,Price_KG,Total,DiscType,DiscPercentage,DiscAmount,Total_PPN,Total_PPH,CanvasId,CanvasSeqNo,AvailableDate,BonusScheme,CashBackScheme,Deskripsi,SeqNoGroup,CreatedDate,CreatedBy,DeliveryMethod) Values ";
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Query += "Insert PurchAgreementDtl (AgreementId,OrderDate,SeqNo,RefTransId,RefTransSeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,BracketDesc,Base,Amount,Qty_KG,RemainingAmount,Unit,Konv_Ratio,Price,Price_KG,Total,DiscType,DiscPercentage,DiscAmount,Total_PPN,Total_PPH,CanvasId,CanvasSeqNo,AvailableDate,BonusScheme,CashBackScheme,Deskripsi,SeqNoGroup,CreatedDate,CreatedBy,DeliveryMethod) Values ";
                    }
                    Query += "('" + AgreementId + "','";
                    Query += (dtOrderDate.Value == null ? "" : dtOrderDate.Value.ToString("yyyy-MM-dd")) + "','";

                    Query += (dgvPA.Rows[i].Cells["No"].Value == "" ? "" : dgvPA.Rows[i].Cells["No"].Value.ToString()) + "','";
                    Query += oldPAID + "','";
                    Query += seqno + "','";
                    Query += (dgvPA.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvPA.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["SubGroup1Id"].Value == "" ? "" : dgvPA.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvPA.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvPA.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvPA.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvPA.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["BracketDesc"].Value == "" ? "" : dgvPA.Rows[i].Cells["BracketDesc"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Base"].Value == "" ? "" : dgvPA.Rows[i].Cells["Base"].Value.ToString()) + "','";
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        Query += (dgvPA.Rows[i].Cells["Quantity"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Quantity"].Value.ToString()) + "','";
                        if (dgvPA.Rows[i].Cells["Unit"].Value.ToString() == "BTG")
                        {
                            Query += (Qty / Konv_Ratio) + "','";
                        }
                        else
                        {
                            Query += Qty + "','";
                        }
                        Query += (dgvPA.Rows[i].Cells["RemainingQty"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["RemainingQty"].Value.ToString()) + "','";
                    }
                    if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Query += (dgvPA.Rows[i].Cells["Amount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Amount"].Value.ToString()) + "','";
                        Query += Qty + "','";
                        Query += (dgvPA.Rows[i].Cells["RemainingAmount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["RemainingAmount"].Value.ToString()) + "','";
                    }

                    //Query += NewRemainingQty + "','";
                    Query += (dgvPA.Rows[i].Cells["Unit"].Value == null ? "" : dgvPA.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Konv_Ratio"].Value == "" ? "0.0000" : dgvPA.Rows[i].Cells["Konv_Ratio"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Price"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Price"].Value.ToString()) + "','";

                    if (dgvPA.Rows[i].Cells["Unit"].Value.ToString() == "BTG")
                    {
                        Query += (Price / Konv_Ratio) + "','";
                    }
                    else
                    {
                        Query += Price + "','";
                    }

                    Query += (dgvPA.Rows[i].Cells["Total"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total"].Value.ToString()) + "','";
                    //BY: HC (S)
                    if (dgvPA.Rows[i].Cells["Disc. Type"].Value == "" || dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() == "Select")
                        Query += "Amount', '";
                    else
                        Query += dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() + "', '";
                    //BY: HC (E)
                    //Query += (dgvPA.Rows[i].Cells["Disc. Type"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString()) + "','";
                    //Query += (dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() == "Select" ? "Amount" : dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Disc. (%)"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. (%)"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Disc. Amount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. Amount"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Total_PPN"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total_PPN"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Total_PPH"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total_PPH"].Value.ToString()) + "','";
                    Query += dgvPA.Rows[i].Cells["CanvasId"].Value.ToString() + "','";
                    Query += dgvPA.Rows[i].Cells["CanvasSeqNo"].Value.ToString() + "',";
                    //Query += (dgvPA.Rows[i].Cells["AvailableDate"].Value == null ? "1990/01/01" : FormateDateddmmyyyy(dgvPA.Rows[i].Cells["AvailableDate"].Value.ToString())) + "','"; //REMARKED BY: HC
                    //Hasim 29 Okt
                    Query += "@availdate,'";
                    Query += (dgvPA.Rows[i].Cells["BonusScheme"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["BonusScheme"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["CashBackScheme"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["CashBackScheme"].Value.ToString()) + "',";
                    Query += "@desc,'";
                    Query += (dgvPA.Rows[i].Cells["SeqNoGroup"].Value == "" ? "" : dgvPA.Rows[i].Cells["SeqNoGroup"].Value.ToString()) + "',";
                    Query += "getdate(),'";
                    Query += ControlMgr.UserId + "','";
                    Query += dgvPA.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "');";

                    adate = FormateDateddmmyyyy(dgvPA.Rows[i].Cells["AvailableDate"].Value.ToString());
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@availdate", adate);
                    Cmd.Parameters.AddWithValue("@desc", (dgvPA.Rows[i].Cells["Deskripsi"].Value == "" ? "" : dgvPA.Rows[i].Cells["Deskripsi"].Value.ToString().Trim()));
                    Cmd.ExecuteNonQuery();
                    Query = "";
                }

                if (Query != "")
                {
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                }

                for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                {
                    Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                    Query += "( 'PurchAgreementH', '" + AgreementId + "', '";
                    Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                    Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";

                    Query += dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString();
                    Query += "',@binaryValue";
                    Query += ");";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                    Cmd.ExecuteNonQuery();
                }

                Trans.Commit();
                MessageBox.Show("Data PA Number : " + AgreementId + " berhasil ditambahkan.");
                Parent.RefreshGrid();
                this.Close();
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void saveEdit()
        {
            try
            {
                Query = "UPDATE PurchAgreementH SET ";
                Query += "OrderDate = '" + dtOrderDate.Value.ToString("yyyy-MM-dd") + "', ";
                Query += "DueDate = '" + dtDueDate.Value.ToString("yyyy-MM-dd") + "', ";
                Query += "RefTransID = '" + txtRefID.Text + "', ";
                Query += "TransType = '" + txtPurchaseType.Text + "', ";
                Query += "CanvasId = '" + txtCanvasID.Text + "', ";
                Query += "PurchQuotID = '" + txtQuotId.Text + "', ";
                Query += "CurrencyId = '" + cmbCurrency.Text + "', ";
                Query += "VendID = '" + txtVendID.Text + "', ";
                Query += "Total_Disk = '" + txtDiscount.Text + "', ";
                Query += "ExchRate = '" + (txtExchangeRate.Text.ToString() == "" ? "0.00" : txtExchangeRate.Text) + "', ";
                Query += "PPN = '" + (cmbPPn.Text == "" ? "0.00" : cmbPPn.Text) + "', ";
                Query += "PPH = '" + (cmbPPh.Text == "" ? "0.00" : cmbPPh.Text) + "', ";
                //BY: HC (S)
                if (cmbDPRequired.Text == "YES")
                {
                    if (cmbDPType.Text == "Percentage")
                    {
                        Query += "DPPercent = '" + tbxDPPercent.Text + "', ";
                        Query += "DPAmount = '" + Convert.ToDecimal(tbxDPPercent.Text) * Convert.ToDecimal(txtGrandTotal.Text) / 100 + "', ";
                    }
                    else if (cmbDPType.Text == "Amount")
                    {
                        Query += "DPPercent = '" + Convert.ToDecimal(tbxDPAmount.Text) * 100 / Convert.ToDecimal(txtGrandTotal.Text) + "', ";
                        Query += "DPAmount = '" + tbxDPAmount.Text + "', ";
                    }
                }
                else if (cmbDPRequired.Text == "NO")
                {
                    Query += "DPPercent = '0', ";
                    Query += "DPAmount = '0', ";
                }
                //BY: HC (E)

                //REMARKED BY: HC (S)
                //Query += "DPPercent = '" + (txtDPPercent.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDPPercent.Text).ToString()) + "', ";
                //Query += "DPAmount = '" + (txtDPAmount.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtDPAmount.Text).ToString()) + "', ";
                //REMARKED BY: HC (E)
                Query += "Total = '" + (txtTotal.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotal.Text).ToString()) + "', ";
                Query += "Total_PPN = '" + (txtTotalPPN.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotalPPN.Text).ToString()) + "', ";
                Query += "Total_PPH = '" + (txtTotalPPH.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtTotalPPH.Text).ToString()) + "', ";
                Query += "Reference_Type = '" + txtReferenceType.Text.ToString() + "', ";
                Query += "GrandTotal = '" + (txtGrandTotal.Text.ToString() == "" ? "0.00" : Convert.ToDecimal(txtGrandTotal.Text).ToString()) + "', ";
                Query += "Deskripsi = '" + txtDeskripsi.Text.Trim() + "', ";
                Query += "TransStatus = '02', ";
                Query += "UpdatedBy = '" + ControlMgr.UserId + "', ";
                Query += "UpdatedDate = GETDATE(), ";
                Query += "TermOfPayment = '" + cmbTermOfPayment.Text + "', ";
                Query += "PaymentMode = '" + cmbPaymentMode.Text + "', ";
                //Query += "DPType = '" + cmbDPType.Text + "' "; //REMARKED BY: HC
                //BY: HC (S)
                if (cmbDPRequired.Text == "YES")
                {
                    Query += "DPType = '" + cmbDPType.Text + "')";
                }
                else if (cmbDPRequired.Text == "NO")
                {
                    Query += "DPType = NULL)";
                }
                //BY: HC (E)
                Query += "WHERE AgreementID = '" + AgreementId + "' ";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();


                insertPA_LogTable(AgreementId, "", "Amend - Waiting for Approval", "E"); //BY: HC

                //created by : Thaddaeus Matthias, 19 March 2018
                //Insert into status log table supplier/vendor
                //========================================begin=============================================
                insertstatuslog(AgreementId);
                //=========================================end==============================================

                string tempQuery = "";

                tempQuery = "DELETE FROM PurchAgreementDtl WHERE AgreementID = '" + AgreementId + "';";
                Query += "Delete from tblAttachments where ReffTableName='PurchAgreementH' And ReffTransId='" + AgreementId + "';";
                Cmd = new SqlCommand(tempQuery, Conn, Trans);
                Cmd.ExecuteNonQuery();

                for (int i = 0; i <= dgvPA.RowCount - 1; i++)
                {
                    if (dgvPA.Rows[i].Cells["AvailableDate"].Value == null || dgvPA.Rows[i].Cells["AvailableDate"].Value == "")
                        dgvPA.Rows[i].Cells["AvailableDate"].Value = "01/01/1900";

                    string seqno = "";

                    Price = Convert.ToDecimal(dgvPA.Rows[i].Cells["Price"].Value);
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Quantity"].Value);
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Amount"].Value);
                    }
                    Konv_Ratio = Convert.ToDecimal(dgvPA.Rows[i].Cells["Konv_Ratio"].Value);

                    tempQuery = "Select SeqNo From PurchAgreementDtl Where AgreementID = '" + txtPAID.Text + "' and FullItemID = '" + dgvPA.Rows[i].Cells["FullItemID"].Value.ToString() + "' and SeqNoGroup = '" + dgvPA.Rows[i].Cells["SeqNoGroup"].Value.ToString() + "'";
                    Cmd = new SqlCommand(tempQuery, Conn, Trans);
                    seqno = Convert.ToString(Cmd.ExecuteScalar());
                    if (seqno == "")
                    {
                        seqno = "NULL";
                    }

                    tempQuery = "SELECT Qty FROM PurchAgreementDtl WHERE AgreementID = '" + txtPAID.Text + "' AND SeqNo = '" + seqno + "'";
                    Cmd = new SqlCommand(tempQuery, Conn, Trans);
                    //decimal OldQty = Convert.ToDecimal(Cmd.ExecuteScalar());
                    //decimal NewQty = Convert.ToDecimal((dgvPA.Rows[i].Cells["Quantity"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Quantity"].Value.ToString()));
                    //decimal OldRemainingQty = Convert.ToDecimal((dgvPA.Rows[i].Cells["RemainingQty"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["RemainingQty"].Value.ToString()));
                    //decimal NewRemainingQty = (NewQty - OldQty) + OldRemainingQty;

                    //if (NewRemainingQty < 0)
                    //{
                    //    JmlhPO = NewQty + Math.Abs(NewRemainingQty);
                    //    MessageBox.Show("Qty harus lebih besar atau sama dengan " + Convert.ToInt32(JmlhPO));
                    //    return;
                    //}

                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        Query += "Insert PurchAgreementDtl (AgreementId,OrderDate,SeqNo,RefTransId,RefTransSeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,BracketDesc,Base,Qty,Qty_KG,RemainingQty,Unit,Konv_Ratio,Price,Price_KG,Total,DiscType,DiscPercentage,DiscAmount,Total_PPN,Total_PPH,CanvasId,CanvasSeqNo,AvailableDate,BonusScheme,CashBackScheme,Deskripsi,SeqNoGroup,CreatedDate,CreatedBy,DeliveryMethod) Values ";
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Query += "Insert PurchAgreementDtl (AgreementId,OrderDate,SeqNo,RefTransId,RefTransSeqNo,GroupId,SubGroup1Id,SubGroup2Id,ItemId,FullItemID,ItemName,BracketDesc,Base,Qty,Qty_KG,RemainingQty,Unit,Konv_Ratio,Price,Price_KG,Total,DiscType,DiscPercentage,DiscAmount,Total_PPN,Total_PPH,CanvasId,CanvasSeqNo,AvailableDate,BonusScheme,CashBackScheme,Deskripsi,SeqNoGroup,CreatedDate,CreatedBy,DeliveryMethod) Values ";
                    }
                    Query += "('" + AgreementId + "','";
                    Query += (dtOrderDate.Value == null ? "" : dtOrderDate.Value.ToString("yyyy-MM-dd")) + "','";

                    Query += (dgvPA.Rows[i].Cells["No"].Value == "" ? "" : dgvPA.Rows[i].Cells["No"].Value.ToString()) + "','";
                    Query += oldPAID + "','";
                    Query += seqno + "','";
                    Query += (dgvPA.Rows[i].Cells["GroupId"].Value == "" ? "" : dgvPA.Rows[i].Cells["GroupId"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["SubGroup1Id"].Value == "" ? "" : dgvPA.Rows[i].Cells["SubGroup1Id"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["SubGroup2Id"].Value == "" ? "" : dgvPA.Rows[i].Cells["SubGroup2Id"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["ItemId"].Value == "" ? "" : dgvPA.Rows[i].Cells["ItemId"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["FullItemID"].Value == "" ? "" : dgvPA.Rows[i].Cells["FullItemID"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["ItemName"].Value == "" ? "" : dgvPA.Rows[i].Cells["ItemName"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["BracketDesc"].Value == "" ? "" : dgvPA.Rows[i].Cells["BracketDesc"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Base"].Value == "" ? "" : dgvPA.Rows[i].Cells["Base"].Value.ToString()) + "','";
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        Query += (dgvPA.Rows[i].Cells["Quantity"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Quantity"].Value.ToString()) + "','";
                        if (dgvPA.Rows[i].Cells["Unit"].Value.ToString() == "BTG")
                        {
                            Query += (Qty / Konv_Ratio) + "','";
                        }
                        else
                        {
                            Query += Qty + "','";
                        }
                        Query += (dgvPA.Rows[i].Cells["Quantity"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Quantity"].Value.ToString()) + "','";
                    }
                    if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Query += (dgvPA.Rows[i].Cells["Amount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Amount"].Value.ToString()) + "','";
                        Query += Qty + "','";
                        Query += (dgvPA.Rows[i].Cells["Amount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Amount"].Value.ToString()) + "','";
                    }

                    //Query += NewRemainingQty + "','";
                    Query += (dgvPA.Rows[i].Cells["Unit"].Value == null ? "" : dgvPA.Rows[i].Cells["Unit"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Konv_Ratio"].Value == "" ? "0.0000" : dgvPA.Rows[i].Cells["Konv_Ratio"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Price"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Price"].Value.ToString()) + "','";

                    if (dgvPA.Rows[i].Cells["Unit"].Value.ToString() == "BTG")
                    {
                        Query += (Price / Konv_Ratio) + "','";
                    }
                    else
                    {
                        Query += Price + "','";
                    }

                    Query += (dgvPA.Rows[i].Cells["Total"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total"].Value.ToString()) + "','";
                    //BY: HC (S)
                    if (dgvPA.Rows[i].Cells["Disc. Type"].Value == "" || dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() == "Select")
                        Query += "Amount', '";
                    else
                        Query += dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() + "', '";
                    //BY: HC (E)
                    //Query += (dgvPA.Rows[i].Cells["Disc. Type"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString()) + "','"; //REMARKED BY: HC
                    Query += (dgvPA.Rows[i].Cells["Disc. (%)"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. (%)"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Disc. Amount"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Disc. Amount"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Total_PPN"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total_PPN"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["Total_PPH"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["Total_PPH"].Value.ToString()) + "','";
                    Query += dgvPA.Rows[i].Cells["CanvasId"].Value.ToString() + "','";
                    Query += dgvPA.Rows[i].Cells["CanvasSeqNo"].Value.ToString() + "',";
                    //hasim 29okt
                    Query += "@availdate,'";
                    Query += (dgvPA.Rows[i].Cells["BonusScheme"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["BonusScheme"].Value.ToString()) + "','";
                    Query += (dgvPA.Rows[i].Cells["CashBackScheme"].Value == "" ? "0.00" : dgvPA.Rows[i].Cells["CashBackScheme"].Value.ToString()) + "',";
                    Query += "@desc,'";
                    Query += (dgvPA.Rows[i].Cells["SeqNoGroup"].Value == "" ? "" : dgvPA.Rows[i].Cells["SeqNoGroup"].Value.ToString()) + "',";
                    Query += "getdate(),'";
                    Query += ControlMgr.UserId + "','";
                    Query += dgvPA.Rows[i].Cells["DeliveryMethod"].Value.ToString() + "');";

                    adate = FormateDateddmmyyyy(dgvPA.Rows[i].Cells["AvailableDate"].Value.ToString());
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@availdate", adate);
                    Cmd.Parameters.AddWithValue("@desc", (dgvPA.Rows[i].Cells["Deskripsi"].Value == "" ? "" : dgvPA.Rows[i].Cells["Deskripsi"].Value.ToString().Trim()));
                    Cmd.ExecuteNonQuery();
                }

                for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                {
                    Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment) Values";
                    Query += "( 'PurchAgreementH', '" + AgreementId + "', '";
                    Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                    Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";

                    Query += dgvAttachment.Rows[i].Cells["File Size (kb)"].Value.ToString();
                    Query += "',@binaryValue";
                    Query += ");";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, test[i].Length).Value = test[i];
                    Cmd.ExecuteNonQuery();
                }

                Trans.Commit();
                MessageBox.Show("Data PA Number : " + AgreementId + " berhasil diedit.");
                Parent.RefreshGrid();
                this.Close();
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private Boolean validasiSave()
        {
            bool vbol = true;

            if (dgvPA.Rows.Count < 1)
            {
                MessageBox.Show("Item harus diisi.");
                vbol = false;
            }
            else if (dgvAttachment.RowCount < 1)
            {
                MessageBox.Show("Attachment harus ada.");
                vbol = false;
            }
            else if (cmbTermOfPayment.Text == "")
            {
                MessageBox.Show("Term of Payment wajib dipilih.");
                vbol = false;
            }
            else if (cmbPaymentMode.Text == "")
            {
                MessageBox.Show("Payment Mode wajib dipilih.");
                vbol = false;
            }
            else if (cmbDPRequired.Text == "")
            {
                MessageBox.Show("DP Required wajib dipilih.");
                vbol = false;
            }
            else if (cmbDPRequired.Text == "YES")
            {
                //REMARKED BY: HC (S)
                //if (txtDPPercent.Text == "" && txtDPAmount.Text == "" || (Convert.ToDecimal(txtDPPercent.Text) <= 0 && Convert.ToDecimal(txtDPAmount.Text) <= 0))
                //{
                //    MessageBox.Show("DP Percent wajib diisi.");
                //    vbol = false;
                //}
                //REMARKED BY: HC (E)
                //BY: HC (S)
                if (tbxDPPercent.Text == "" && tbxDPAmount.Text == "" || (Convert.ToDecimal(tbxDPPercent.Text) <= 0 && Convert.ToDecimal(tbxDPAmount.Text) <= 0))
                {
                    MessageBox.Show("DP Percent wajib diisi.");
                    vbol = false;
                }
                //BY: HC (E)
            }

            for (int z = 0; z < dgvPA.RowCount; z++)
            {
                if (dgvPA.Rows[z].Cells["Base"].Value.ToString() == "Y")
                {
                    if (dgvPA.Rows[z].Cells["DeliveryMethod"].Value.ToString() == "Select" || dgvPA.Rows[z].Cells["DeliveryMethod"].Value.ToString() == "")
                    {
                        MessageBox.Show("DeliveryMethod baris=" + (z + 1) + " tidak boleh kosong");
                        vbol = false;
                    }
                    if (dgvPA.Rows[z].Cells["AvailableDate"].Value.ToString() == "" || dgvPA.Rows[z].Cells["AvailableDate"].Value.ToString() == "01/01/1900")
                    {
                        MessageBox.Show("AvailableDate baris=" + (z + 1) + " tidak boleh kosong atau 01/01/1900");
                        vbol = false;
                    }
                }
            }

            return vbol;
        }

        decimal Price = 0;
        decimal Qty = 0;
        decimal Konv_Ratio = 0;
        decimal JmlhPO = 0;

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                if (!validasiSave())
                    return;

                if (Mode == "New")
                {
                    saveNew();
                }
                else if (Mode == "Ammend")
                {
                    saveAmmend();
                }
                else if (Mode == "Edit")
                {
                    saveEdit();
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tabDgvControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            dgvReference.AutoResizeColumns();
            if (txtRefID.Text != "" && dgvReference.RowCount == 0)
            {
                Conn = ConnectionString.GetConnection();
                if (txtReferenceType.Text == "Purchase Agreement")
                {
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        Query = "Select ROW_NUMBER() OVER (ORDER BY AgreementID) No, AgreementID [PA No], FullItemId, ItemName, Base, Qty, Unit, RemainingQty, AvailableDate From [PurchAgreementDtl] a ";
                        Query += "Where AgreementID='" + txtRefID.Text + "' ;";
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Query = "Select ROW_NUMBER() OVER (ORDER BY AgreementID) No, AgreementID [PA No], FullItemId, ItemName, Base, Amount, RemainingAmount, AvailableDate From [PurchAgreementDtl] a ";
                        Query += "Where AgreementID='" + txtRefID.Text + "' ;";
                    }
                }
                else if (txtReferenceType.Text == "Canvas Sheet")
                {
                    if (txtPurchaseType.Text != "AMOUNT")
                    {
                        Query = "Select ROW_NUMBER() OVER (ORDER BY CanvasId) No, CanvasId [CS No], FullItemId, ItemName, Base, Qty, Unit, Ratio  From [CanvasSheetD] a ";
                        Query += "Where CanvasId='" + txtRefID.Text + "' and PurchQuotId = '" + txtQuotId.Text.Trim() + "' and StatusApproval = 'Yes' ;";
                    }
                    else if (txtPurchaseType.Text == "AMOUNT")
                    {
                        Query = "Select ROW_NUMBER() OVER (ORDER BY CanvasId) No, CanvasId [CS No], FullItemId, ItemName, Base,  CSAmount [Amount], Unit, Ratio  From [CanvasSheetD] a ";
                        Query += "Where CanvasId='" + txtRefID.Text + "' and PurchQuotId = '" + txtQuotId.Text.Trim() + "' and StatusApproval = 'Yes' ;";
                    }
                }
                Da = new SqlDataAdapter(Query, Conn);
                Dt = new DataTable();
                Da.Fill(Dt);

                dgvReference.AutoGenerateColumns = true;
                dgvReference.DataSource = Dt;
                dgvReference.Refresh();
                dgvReference.AutoResizeColumns();
                dgvReference.DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void cmbPPn_SelectedIndexChanged(object sender, EventArgs e)
        {
            SumFooter();
        }

        private void cmbPPh_SelectedIndexChanged(object sender, EventArgs e)
        {
            SumFooter();
        }

        private void HidePPnPPh()
        {
            if (txtPurchaseType.Text == "AMOUNT")
            {
                txtDiscount.Visible = false;
                txtTotalPPN.Visible = false;
                txtTotalPPH.Visible = false;
                txtGrandTotal.Visible = false;
                label12.Visible = false;
                label13.Visible = false;
                label28.Visible = false;
                label33.Visible = false;
                dgvPA.Columns["Total"].Visible = false;
                dgvPA.Columns["Total_PPN"].Visible = false;
                dgvPA.Columns["Total_PPH"].Visible = false;
            }
        }

        private void SumFooter()
        {
            txtTotal.Text = "0.00";
            txtDiscount.Text = "0.00";
            txtTotalPPN.Text = "0.00";
            txtTotalPPH.Text = "0.00";
            txtBonusScheme.Text = "0.00";
            txtCashBackScheme.Text = "0.00";
            for (int i = 0; i < dgvPA.RowCount; i++)
            {
                if (dgvPA.Rows[i].Cells["BonusScheme"].Value == null || dgvPA.Rows[i].Cells["BonusScheme"].Value.ToString() == "")
                    dgvPA.Rows[i].Cells["BonusScheme"].Value = "0";
                if (dgvPA.Rows[i].Cells["CashBackScheme"].Value == null || dgvPA.Rows[i].Cells["CashBackScheme"].Value.ToString() == "")
                    dgvPA.Rows[i].Cells["CashBackScheme"].Value = "0";
                if (dgvPA.Rows[i].Cells["Total"].Value == null || dgvPA.Rows[i].Cells["Total"].Value.ToString() == "")
                    dgvPA.Rows[i].Cells["Total"].Value = "0";
                if (dgvPA.Rows[i].Cells["Total_PPN"].Value == null || dgvPA.Rows[i].Cells["Total_PPN"].Value.ToString() == "")
                    dgvPA.Rows[i].Cells["Total_PPN"].Value = "0";
                if (dgvPA.Rows[i].Cells["Total_PPH"].Value == null || dgvPA.Rows[i].Cells["Total_PPH"].Value.ToString() == "")
                    dgvPA.Rows[i].Cells["Total_PPH"].Value = "0";
                if (dgvPA.Rows[i].Cells["Disc. (%)"].Value == null || dgvPA.Rows[i].Cells["Disc. (%)"].Value.ToString() == "")
                    dgvPA.Rows[i].Cells["Disc. (%)"].Value = "0";
                if (dgvPA.Rows[i].Cells["Disc. Amount"].Value == null || dgvPA.Rows[i].Cells["Disc. Amount"].Value.ToString() == "")
                    dgvPA.Rows[i].Cells["Disc. Amount"].Value = "0";
                if (txtTotal.Text == "")
                    txtTotal.Text = "0";
                if (txtTotalPPN.Text == "")
                    txtTotalPPN.Text = "0";
                if (txtTotalPPH.Text == "")
                    txtTotalPPH.Text = "0";

                decimal PPh = 0;
                decimal PPN = 0;
                decimal Qty = 0;
                decimal Price = 0;

                if (cmbPPh.SelectedIndex == 0 || cmbPPh.Text == "")
                {
                    PPh = 0;
                }
                else
                {
                    PPh = Convert.ToDecimal(cmbPPh.Text);
                }
                if (cmbPPn.SelectedIndex == 0 || cmbPPn.Text == "")
                {
                    PPN = 0;
                }
                else
                {
                    PPN = Convert.ToDecimal(cmbPPn.Text);
                }

                Price = Convert.ToDecimal(dgvPA.Rows[i].Cells["Price"].Value == "" ? "0" : dgvPA.Rows[i].Cells["Price"].Value.ToString());
                decimal Discount = Convert.ToDecimal(dgvPA.Rows[i].Cells["Disc. Amount"].Value == "" == null ? "0" : dgvPA.Rows[i].Cells["Disc. Amount"].Value.ToString());
                decimal Total = 0;
                if (txtPurchaseType.Text != "AMOUNT")
                {
                    Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Quantity"].Value.ToString() == "" ? "0" : dgvPA.Rows[i].Cells["Quantity"].Value.ToString());
                    dgvPA.Rows[i].Cells["Total"].Value = Qty * Price;
                    Total = Qty * Price;
                }
                else if (txtPurchaseType.Text == "AMOUNT")
                {
                    Qty = Convert.ToDecimal(dgvPA.Rows[i].Cells["Amount"].Value.ToString() == "" ? "0" : dgvPA.Rows[i].Cells["Amount"].Value.ToString());
                    dgvPA.Rows[i].Cells["Total"].Value = Qty;
                    //dgvPA.Rows[i].Cells["Total"].Value = Qty + (Qty * PPh / 100) + (Qty * PPN / 100);
                    Total = Qty;
                }


                dgvPA.Rows[i].Cells["Disc. Amount"].Value = Discount;

                dgvPA.Rows[i].Cells["Total_PPN"].Value = (Total - Discount) * PPN / 100;
                dgvPA.Rows[i].Cells["Total_PPH"].Value = (Total - Discount) * PPh / 100;
                
                txtTotal.Text = (Convert.ToDecimal(txtTotal.Text) + Convert.ToDecimal(dgvPA.Rows[i].Cells["Total"].Value.ToString())).ToString("N2");
                txtDiscount.Text = (Convert.ToDecimal(txtDiscount.Text) + Convert.ToDecimal(dgvPA.Rows[i].Cells["Disc. Amount"].Value.ToString())).ToString("N2");
                txtTotalPPN.Text = (Convert.ToDecimal(txtTotalPPN.Text) + Convert.ToDecimal(dgvPA.Rows[i].Cells["Total_PPN"].Value.ToString())).ToString("N2");
                txtTotalPPH.Text = (Convert.ToDecimal(txtTotalPPH.Text) + Convert.ToDecimal(dgvPA.Rows[i].Cells["Total_PPH"].Value.ToString())).ToString("N2");
                txtGrandTotal.Text = (Convert.ToDecimal(txtTotal.Text) - Convert.ToDecimal(txtDiscount.Text) + Convert.ToDecimal(txtTotalPPN.Text) + Convert.ToDecimal(txtTotalPPH.Text)).ToString("N2");
                txtCashBackScheme.Text = (Convert.ToDecimal(txtCashBackScheme.Text) + Convert.ToDecimal(dgvPA.Rows[i].Cells["CashBackScheme"].Value.ToString())).ToString("N2");
                txtBonusScheme.Text = (Convert.ToDecimal(txtBonusScheme.Text) + Convert.ToDecimal(dgvPA.Rows[i].Cells["BonusScheme"].Value.ToString())).ToString("N2");
                dgvPA.AutoResizeColumns();
            }
        }

        private void refreshGrey()
        {
            for (int i = 0; i < dgvPA.RowCount; i++)
            {
                if (dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() == "Select")
                {
                    dgvPA.Rows[i].Cells["Disc. (%)"].Style.BackColor = Color.LightGray;
                    dgvPA.Rows[i].Cells["Disc. Amount"].Style.BackColor = Color.LightGray;
                    dgvPA.Rows[i].Cells["Disc. (%)"].ReadOnly = true;
                    dgvPA.Rows[i].Cells["Disc. Amount"].ReadOnly = true;
                    dgvPA.Rows[i].Cells["Disc. (%)"].Value = 0;
                    dgvPA.Rows[i].Cells["Disc. Amount"].Value = 0;
                }
                if (dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() == "Percentage")
                {
                    dgvPA.Rows[i].Cells["Disc. (%)"].Style.BackColor = Color.White;
                    dgvPA.Rows[i].Cells["Disc. Amount"].Style.BackColor = Color.LightGray;
                    dgvPA.Rows[i].Cells["Disc. (%)"].ReadOnly = false;
                    dgvPA.Rows[i].Cells["Disc. Amount"].ReadOnly = true;
                }
                if (dgvPA.Rows[i].Cells["Disc. Type"].Value.ToString() == "Amount")
                {
                    dgvPA.Rows[i].Cells["Disc. (%)"].Style.BackColor = Color.LightGray;
                    dgvPA.Rows[i].Cells["Disc. Amount"].Style.BackColor = Color.White;
                    dgvPA.Rows[i].Cells["Disc. (%)"].ReadOnly = true;
                    dgvPA.Rows[i].Cells["Disc. Amount"].ReadOnly = false;
                }
                if (dgvPA.ReadOnly == true)
                {
                    dgvPA.Rows[i].Cells["Disc. (%)"].Style.BackColor = Color.LightGray;
                    dgvPA.Rows[i].Cells["Disc. Amount"].Style.BackColor = Color.LightGray;
                }
            }
        }

        private void dgvPA_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (txtPurchaseType.Text != "AMOUNT")
            {
                if (e.ColumnIndex == dgvPA.Columns["Quantity"].Index || e.ColumnIndex == dgvPA.Columns["RemainingQty"].Index)
                {
                    if (e.Value == null || e.Value.ToString() == "")
                    {
                        e.Value = "0.00";
                        return;
                    }
                    else
                    {
                        double d = double.Parse(e.Value.ToString());
                        e.Value = d.ToString("N2");
                    }
                }
            }
            if (txtPurchaseType.Text == "AMOUNT")
            {
                if (e.ColumnIndex == dgvPA.Columns["Amount"].Index || e.ColumnIndex == dgvPA.Columns["RemainingAmount"].Index)
                {
                    if (e.Value == null || e.Value.ToString() == "")
                    {
                        e.Value = "0.00";
                        return;
                    }
                    double d = double.Parse(e.Value.ToString());
                    e.Value = d.ToString("N2");
                }
            }
            if (/*e.ColumnIndex == dgvPA.Columns["Konv_Ratio"].Index || */
                e.ColumnIndex == dgvPA.Columns["Price"].Index ||
                e.ColumnIndex == dgvPA.Columns["Total"].Index ||
                e.ColumnIndex == dgvPA.Columns["Disc. (%)"].Index ||
                e.ColumnIndex == dgvPA.Columns["Disc. Amount"].Index ||
                e.ColumnIndex == dgvPA.Columns["Total_PPN"].Index ||
                e.ColumnIndex == dgvPA.Columns["Total_PPH"].Index ||
                e.ColumnIndex == dgvPA.Columns["CashBackScheme"].Index ||
                e.ColumnIndex == dgvPA.Columns["BonusScheme"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            if (/*e.ColumnIndex == dgvPA.Columns["Konv_Ratio"].Index ||*/
                e.ColumnIndex == dgvPA.Columns["Disc. (%)"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
            if (e.ColumnIndex == dgvPA.Columns["Konv_Ratio"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.0000";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }
            if (dgvPA.Columns[e.ColumnIndex].Name == "CreatedDate" || dgvPA.Columns[e.ColumnIndex].Name == "UpdatedDate" || dgvPA.Columns[e.ColumnIndex].Name.Contains("ValidTo"))
                dgvPA.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            else if (dgvPA.Columns[e.ColumnIndex].Name.Contains("Date"))
                dgvPA.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }


        public static string itemID;

        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void PAForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void btnAmmend_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                String PAId = txtPAID.Text;

                Conn = ConnectionString.GetConnection();
                Query = "Select StClose From PurchAgreementH Where AgreementId = '" + PAId + "'";
                Cmd = new SqlCommand(Query, Conn);

                if ((bool)Cmd.ExecuteScalar() == true)
                {
                    Conn.Close();
                    MessageBox.Show("Purchase Agreement telah selesai.");
                    return;
                }

                Mode = "Ammend";
                oldPAID = PAId;
                PAForm_Load(sender, e);
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseAgreement.AddItem F = new Purchase.PurchaseAgreement.AddItem();
            F.flag(AgreementId, dgvPA);
            F.setParent(this);
            F.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!amendDeleteCheck())
                return;

            Conn = ConnectionString.GetConnection();
            Query = "Select ReffId From [PurchDtl] ";
            Query += "Where reffId = '" + txtPAID.Text + "' and (deletedby is null or deletedby='') ";
            Cmd = new SqlCommand(Query, Conn);
            string TmpPaID = (Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString());
            Conn.Close();

            if (TmpPaID != "" || !(String.IsNullOrEmpty(TmpPaID)))
            {
                MessageBox.Show("PA sudah direlease PO, data tidak ada yang bisa didelete.");
                return;
            }

            if (dgvPA.RowCount > 0)
            {
                Index = dgvPA.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvPA.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvPA.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvPA.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //BY: HC (S)
                    if (dgvPA.Rows[dgvPA.CurrentRow.Index].Cells["Base"].Value.ToString() == "Y")
                    {
                        int rowCount = dgvPA.RowCount;
                        List<int> indexData = new List<int>();
                        indexData.Add(dgvPA.CurrentRow.Index);
                        for (int i = dgvPA.CurrentRow.Index + 1; i < rowCount; i++)
                        {
                            if (dgvPA.Rows[i].Cells["Base"].Value.ToString() == "N")
                                indexData.Add(i);
                            else
                                break;
                        }
                        for (int i = indexData.Count - 1; i >= 0; i--)
                        {
                            dgvPA.Rows.RemoveAt(indexData[i]);
                        }
                    }
                    else
                        dgvPA.Rows.RemoveAt(dgvPA.CurrentRow.Index);
                    //BY: HC (E)

                    //REMARKED BY: HC (S)
                    //dgvPA.Rows.RemoveAt(Index);

                    //for (int i = 0; i < dgvPA.RowCount; i++)
                    //{
                    //    dgvPA.Rows[i].Cells["No"].Value = i + 1;
                    //}
                    //REMARKED BY: HC (E)
                    SumFooter();

                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void dgvPA_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Index = dgvPA.CurrentRow.Index;
            if (dgvPA.Rows.Count > 0)
            {
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "Amount")
                {
                    if (dgvPA.Rows[Index].Cells["Amount"].Value.ToString() == "")
                    {
                        dgvPA.Rows[Index].Cells["Amount"].Value = "0";
                    }
                    if (Convert.ToDecimal(dgvPA.Rows[Index].Cells["RemainQty"].Value) >= Convert.ToDecimal(dgvPA.Rows[Index].Cells["Amount"].Value))
                    {
                        dgvPA.Rows[Index].Cells["RemainingAmount"].Value = Convert.ToDecimal(dgvPA.Rows[Index].Cells["RemainQty"].Value) - Convert.ToDecimal(dgvPA.Rows[Index].Cells["Amount"].Value);

                    }
                    else
                    {
                        MessageBox.Show("Amount(" + dgvPA.Rows[Index].Cells["Amount"].Value.ToString() + ") tidak boleh lebih besar dari Remaining Amount(" + dgvPA.Rows[Index].Cells["RemainQty"].Value.ToString() + ").");
                        dgvPA.Rows[Index].Cells["Amount"].Value = dgvPA.Rows[Index].Cells["RemainQty"].Value;
                        dgvPA.Rows[Index].Cells["RemainingAmount"].Value = "0";
                    }
                    dgvPA.Rows[Index].Cells["Total"].Value = Convert.ToDecimal(dgvPA.Rows[Index].Cells["Amount"].Value);
                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "Quantity")
                {

                    if (dgvPA.Rows[Index].Cells["Quantity"].Value.ToString() == "")
                    {
                        dgvPA.Rows[Index].Cells["Quantity"].Value = "0";
                    }
                    if (Convert.ToDecimal(dgvPA.Rows[Index].Cells["RemainQty"].Value) >= Convert.ToDecimal(dgvPA.Rows[Index].Cells["Quantity"].Value))
                    {
                        dgvPA.Rows[Index].Cells["RemainingQty"].Value = Convert.ToDecimal(dgvPA.Rows[Index].Cells["RemainQty"].Value) - Convert.ToDecimal(dgvPA.Rows[Index].Cells["Quantity"].Value);

                    }
                    else
                    {
                        MessageBox.Show("Quantity(" + dgvPA.Rows[Index].Cells["Quantity"].Value.ToString() + ") tidak boleh lebih besar dari Remaining Quantity(" + dgvPA.Rows[Index].Cells["RemainQty"].Value.ToString() + ").");
                        dgvPA.Rows[Index].Cells["Quantity"].Value = dgvPA.Rows[Index].Cells["RemainQty"].Value;
                        dgvPA.Rows[Index].Cells["RemainingQty"].Value = "0";
                    }

                    dgvPA.Rows[Index].Cells["Total"].Value = Convert.ToDecimal(dgvPA.Rows[Index].Cells["Quantity"].Value) * Convert.ToDecimal(dgvPA.Rows[Index].Cells["Price"].Value);
                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "Price")
                {
                    if (dgvPA.Rows[Index].Cells["Price"].Value.ToString() == "")
                    {
                        dgvPA.Rows[Index].Cells["Price"].Value = "0";
                    }
                    if (txtReferenceType.Text == "QTY")
                    {
                        dgvPA.Rows[Index].Cells["Total"].Value = Convert.ToDecimal(dgvPA.Rows[Index].Cells["Quantity"].Value) * Convert.ToDecimal(dgvPA.Rows[Index].Cells["Price"].Value);
                    }

                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "Disc. (%)")
                {
                    if (dgvPA.Rows[Index].Cells["Disc. (%)"].Value.ToString() == "")
                    {
                        dgvPA.Rows[Index].Cells["Disc. (%)"].Value = "0";
                    }
                    else
                    {
                        dgvPA.Rows[Index].Cells["Disc. Amount"].Value = Convert.ToDecimal(dgvPA.Rows[Index].Cells["Total"].Value) * Convert.ToDecimal(dgvPA.Rows[Index].Cells["Disc. (%)"].Value) / 100;
                    }
                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "Disc. Amount")
                {
                    if (dgvPA.Rows[Index].Cells["Disc. Amount"].Value.ToString() == "")
                    {
                        dgvPA.Rows[Index].Cells["Disc. Amount"].Value = "0";
                    }
                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "BonusScheme")
                {
                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "CashbackScheme")
                {
                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "Disc. Type")
                {
                    if (dgvPA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "Percentages")
                    {
                        if (dgvPA.Rows[Index].Cells["Disc. (%)"].Value.ToString() == "")
                        {
                            dgvPA.Rows[Index].Cells["Disc. (%)"].Value = "0";
                        }
                        dgvPA.Rows[Index].Cells["Disc. Amount"].Value = Convert.ToDecimal(dgvPA.Rows[Index].Cells["Total"].Value) * Convert.ToDecimal(dgvPA.Rows[Index].Cells["Disc. (%)"].Value) / 100;
                    }
                    else
                    {
                        dgvPA.Rows[Index].Cells["Disc. (%)"].Value = "0";
                    }
                    refreshGrey();
                    SumFooter();
                }
                if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
                {
                    if (dgvPA.CurrentCell.Value != "" && dgvPA.CurrentCell.Value != null)
                    {
                        //dgvPA.CurrentCell.Value = dtp.Value.Date.ToString("dd/MM/yyyy"); //REMARKED BY: HC
                        dgvPA.CurrentCell.Value = Convert.ToDateTime(dtp.Value); //BY: HC
                    }
                    else
                    {
                        dtp.Value = DateTime.Now;
                    }
                }

            }
            dtp.Visible = false;
            dgvPA.AutoResizeColumns();
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                Query = "Update [PurchAgreementH] set ";
                Query += "TransStatus = '03',";
                Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                Query += "UpdatedDate=getdate(),";
                Query += "UpdatedBy='" + ControlMgr.UserId + "' where AgreementID='" + txtPAID.Text.Trim() + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                insertPA_LogTable(txtPAID.Text.Trim(), "", "Approved by Purchasing Manager - ALL", "N"); //BY: HC

                //created by: Thaddaeus Matthias, 15 March 2018
                // inserting status log
                //=======================================begin=======================================
                insertstatuslogapprove();
                //========================================end========================================

                Trans.Commit();
                MessageBox.Show("Data PANumber : " + txtPAID.Text + " berhasil diupdate.");
                Parent.RefreshGrid();
                this.Close();
            }
            catch (Exception ex)
            {
                Trans.Rollback();
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "Agreement ID : " + AgreementId + "" + Environment.NewLine + "Akan di reject ? ", "Reject Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "Update [PurchAgreementH] set ";
                    Query += "TransStatus = '05',";
                    Query += "StClose = '0',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where AgreementID='" + txtPAID.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    insertPA_LogTable(txtPAID.Text, "", "Reject", "N"); //BY: HC

                    //created by: Thaddaeus Matthias, 15 March 2018
                    // inserting status log
                    //=======================================begin=======================================
                    insertstatuslogreject();
                    //========================================end========================================

                    Trans.Commit();
                    MessageBox.Show("Data PANumber : " + txtPAID.Text + " berhasil diupdate.");
                    Parent.RefreshGrid();
                    this.Close();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
        }

        private void btnRevision_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "Agreement ID : " + AgreementId + "" + Environment.NewLine + "Akan dilakukan permintaan perbaikan data ? ", "Revision Confirmation !", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "Update [PurchAgreementH] set ";
                    Query += "TransStatus = '04',";
                    Query += "Deskripsi = '" + txtDeskripsi.Text.Trim() + "',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where AgreementID='" + txtPAID.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    insertPA_LogTable(txtPAID.Text, "", "Approved by Purchasing Manager - Revision Needed", "N"); //BY: HC

                    //created by : Thaddaeus Matthias, 15 March 2018
                    //Insert into status log table supplier/vendor
                    //========================================begin=============================================
                    insertstatuslogrevision();
                    //=========================================end==============================================

                    Trans.Commit();
                    MessageBox.Show("Data PANumber : " + txtPAID.Text + " berhasil diupdate.");
                    Parent.RefreshGrid();
                    this.Close();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
        }

        private void dgvPA_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column1_KeyPress);
            if (txtPurchaseType.Text != "AMOUNT")
            {
                if (dgvPA.CurrentCell.ColumnIndex == dgvPA.Columns["Quantity"].Index) //Desired Column
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                    }
                }
                if (dgvPA.CurrentCell.ColumnIndex == dgvPA.Columns["RemainingQty"].Index) //Desired Column
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                    }
                }
            }
            if (txtPurchaseType.Text == "AMOUNT")
            {
                if (dgvPA.CurrentCell.ColumnIndex == dgvPA.Columns["Amount"].Index) //Desired Column
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                    }
                }
                if (dgvPA.CurrentCell.ColumnIndex == dgvPA.Columns["RemainingAmount"].Index) //Desired Column
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                    {
                        tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                    }
                }
            }
            if (dgvPA.CurrentCell.ColumnIndex == dgvPA.Columns["Price"].Index) //Desired Column
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(Column1_KeyPress);
                }
            }
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvPA_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvPA_KeyPress);
            }

        }

        private void Column1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";
                btnSave.Visible = true;
                btnEdit.Visible = false;
                btnCancel.Visible = true;
                btnNew.Visible = true;
                btnDelete.Visible = true;
                btnExit.Visible = false;
                txtExchangeRate.ReadOnly = true;
                dtDueDate.Enabled = true;
                cmbPPh.Enabled = true;
                cmbPPn.Enabled = true;
                txtDeskripsi.ReadOnly = true;

                SetDataGridReadOnly();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "View";
            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnNew.Visible = false;
            btnDelete.Visible = false;
            txtExchangeRate.ReadOnly = false;
            dtDueDate.Enabled = false;
            cmbPPh.Enabled = false;
            cmbPPn.Enabled = false;
            txtDeskripsi.ReadOnly = false;
            btnExit.Visible = true;
            RefreshData();
        }

        private void btnCancelApproved_Click(object sender, EventArgs e)
        {
            if (CheckStatusPO(AgreementId) == 0)
            {
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "Agreement ID : " + AgreementId + "" + Environment.NewLine + "Akan di Cancel Approve ? ", "Cancel Approve Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        Conn = ConnectionString.GetConnection();
                        Trans = Conn.BeginTransaction();

                        Query = "Update [PurchAgreementH] set ";
                        Query += "TransStatus = '02',";
                        Query += "Deskripsi = '" + txtDeskripsi.Text.Trim() + "',";
                        Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                        Query += "UpdatedDate=getdate(),";
                        Query += "UpdatedBy='" + ControlMgr.UserId + "' where AgreementID='" + txtPAID.Text.Trim() + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        insertPA_LogTable(txtPAID.Text.Trim(), "", "Amend - Waiting for Approval", "N"); //BY: HC

                        //created by : Thaddaeus Matthias, 15 March 2018
                        //Insert into status log table supplier/vendor
                        //========================================begin=============================================
                        insertstatuslogCancelApprove();
                        //=========================================end==============================================

                        Trans.Commit();
                        MessageBox.Show("Data PANumber : " + txtPAID.Text + " berhasil diupdate.");
                        Parent.RefreshGrid();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        Trans.Rollback();
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    finally
                    {
                        Conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Tidak dapat cancel approve karena data telah diproses.");
            }
        }

        private int CheckStatusPO(string prmAgreementID)
        {
            int result = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(AgreementID) AS CountData FROM PurchAgreementDtl WHERE AgreementID = '" + prmAgreementID + "' AND (Qty <> RemainingQty)";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToInt32(Dr["CountData"]);
            }
            Dr.Close();
            Conn.Close();

            return result;
        }

        private void textExchangeRate_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textExchangeRate_Leave(object sender, EventArgs e)
        {
            if (txtExchangeRate.Text != "")
            {
                double d = double.Parse(txtExchangeRate.Text);
                txtExchangeRate.Text = d.ToString("N4");
            }

        }

        private string GetRefTrans(string prmAgreementID)
        {
            string result = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT RefTransId FROM PurchAgreementH WHERE AgreementID = '" + prmAgreementID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                result = Convert.ToString(Dr["RefTransId"]);
            }
            Dr.Close();
            Conn.Close();

            return result;
        }

        //created by : Thaddaeus Matthias, 14 March 2018
        //Insert into status log table customer
        //========================================begin=============================================
        private void insertstatuslog(string id)
        {
            string Jenis = "PA", Kode = "PA";
            string AgreementId;
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, StatusLog_PK1, Vendor_Id, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, StatusLog_Status, StatusLog_UserID, StatusLog_Date, StatusLog_Description) VALUES ";
            Query += "('PAForm', ";
            if (Mode == "New")
            {
                AgreementId = id;
                Query += "'" + AgreementId + "', ";
                Query += "'" + txtVendID.Text + "', '', '" + "', '" + "', '03', '" + ControlMgr.UserId + "', getdate(), 'New Save')";
            }
            else if (Mode == "Ammend")
            {
                AgreementId = id;
                Query += "'" + AgreementId + "', ";
                Query += "'" + txtVendID.Text + "', '', '" + "', '" + "', '02', '" + ControlMgr.UserId + "', getdate(), 'New Amend')";
            }
            else if (Mode == "Edit")
            {
                Query += "'" + txtPAID.Text + "', ";
                Query += "'" + txtVendID.Text + "', '', '" + "', '" + "', '02', '" + ControlMgr.UserId + "', getdate(), 'New Edit')";
            }
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void insertstatuslogclose()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, StatusLog_PK1, Vendor_Id, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, StatusLog_Status, StatusLog_UserID, StatusLog_Date, StatusLog_Description) VALUES ";
            Query += "('PAForm', ";
            if (Mode == "Ammend")
            {
                Query += "'" + txtRefID.Text + "', ";
            }
            else
            {

            }
            Query += " '" + txtVendID.Text + "', '', '" + "', '" + "', '06', '" + ControlMgr.UserId + "', getdate(), 'Close because ammend')";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void insertstatuslogapprove()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, StatusLog_PK1, Vendor_Id, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, StatusLog_Status, StatusLog_UserID, StatusLog_Date, StatusLog_Description) VALUES ";
            Query += "('PAForm', '" + txtPAID.Text + "', ";
            Query += " '" + txtVendID.Text + "', '', '" + "', '" + "', '03', '" + ControlMgr.UserId + "', getdate(), 'Approved by " + ControlMgr.UserId + ". ')";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void insertstatuslogrevision()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, StatusLog_PK1, Vendor_Id, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, StatusLog_Status, StatusLog_UserID, StatusLog_Date, StatusLog_Description) VALUES ";
            Query += "('PAForm', '" + txtPAID.Text + "', ";
            Query += " '" + txtVendID.Text + "', '', '" + "', '" + "', '04', '" + ControlMgr.UserId + "', getdate(), 'revision')";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void insertstatuslogreject()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, StatusLog_PK1, Vendor_Id, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, StatusLog_Status, StatusLog_UserID, StatusLog_Date, StatusLog_Description) VALUES ";
            Query += "('PAForm', '" + txtPAID.Text + "', ";
            Query += " '" + txtVendID.Text + "', '', '" + "', '" + "', '05', '" + ControlMgr.UserId + "', getdate(), 'reject')";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void insertstatuslogCancelApprove()
        {
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] (StatusLog_FormName, StatusLog_PK1, Vendor_Id, StatusLog_PK2, StatusLog_PK3, StatusLog_PK4, StatusLog_Status, StatusLog_UserID, StatusLog_Date, StatusLog_Description) VALUES ";
            Query += "('PAForm', ";
            if (txtPAID.Text != null && txtPAID.Text != "")
            {
                Query += "'" + txtPAID.Text + "', ";
            }
            else
            {
                Query += "'" + txtRefID.Text + "', ";
            }
            Query += " '" + txtVendID.Text + "', '', '" + "', '" + "', '02', '" + ControlMgr.UserId + "', getdate(), 'approval cancel')";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void btnPaymentMode_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "PaymentMode";
            //string Where = "And (StClose = 'False')";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            cmbPaymentMode.SelectedItem = ConnectionString.Kode;
        }

        #region
        private void cmbPPh_Load()
        {
            if (cmbPPh.Items.Count == 0)
            {
                Query = "Select [TaxStatusCode], [TaxStatusName], [TaxPercent] From dbo.[TaxGroup] where TaxStatusCode like '%PPH%' or TaxStatusCode like ''";
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    cmbPPh.Items.Add(Dr["TaxPercent"].ToString());
                }
                Dr.Close();
            }

        }

        private void cmbPPn_Load()
        {
            if (cmbPPn.Items.Count == 0)
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select [TaxStatusCode], [TaxStatusName], [TaxPercent] From dbo.[TaxGroup] where TaxStatusCode like '%PPN%' or TaxStatusCode like ''";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    cmbPPn.Items.Add(Dr["TaxPercent"].ToString());
                }
                Dr.Close();
            }
        }

        private void AddCmbCurrency()
        {
            cmbCurrency.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select CurrencyID from [dbo].[CurrencyTable]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbCurrency.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();

            if (cmbCurrency.Items.Contains("IDR"))
            {
                cmbCurrency.SelectedItem = "IDR";
            }
        }

        private void AddCmbTermOfPayment()
        {
            cmbTermOfPayment.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [TermOfPayment] from TermOfPayment", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbTermOfPayment.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
        }

        private void AddCmbPaymentMode()
        {
            cmbPaymentMode.Items.Clear();
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [PaymentModeName] from [dbo].[PaymentMode]", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbPaymentMode.Items.Add(Dr[0]);
            }
            Dr.Close();
            Conn.Close();
        }

        private void GenerateAttachment()
        {
            dgvAttachment.Rows.Clear();
            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 5;
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "ContentType";
                dgvAttachment.Columns[2].Name = "File Size (kb)";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";

                dgvAttachment.Columns["Attachment"].Visible = false;
                dgvAttachment.Columns["Id"].Visible = false;
            }
        }

        private void GenerateDatagridViewHeader(ref string[] DataGridViewHeader)
        {
            if (txtPurchaseType.Text != "AMOUNT")
            {
                DataGridViewHeader = new string[] { "No", "GroupId", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", 
                "BracketDesc", "Base", "Quantity", "RemainingQty", "Unit", "Konv_Ratio", "Price", "DeliveryMethod", "AvailableDate", "Total", "Disc. Type", "Disc. (%)", "Disc. Amount", "Total_PPN",
                "Total_PPH", "CanvasID", "CanvasSeqNo", "BonusScheme", "CashbackScheme", "SeqNoGroup", "Deskripsi","RemainQty"};
            }
            else if (txtPurchaseType.Text == "AMOUNT")
            {
                DataGridViewHeader = new string[] { "No", "GroupId", "SubGroup1ID", "SubGroup2ID", "ItemID", "FullItemID", "ItemName", 
                "BracketDesc", "Base", "Amount", "RemainingAmount", "Unit", "Konv_Ratio", "Price", "DeliveryMethod", "AvailableDate", "Total", "Disc. Type", "Disc. (%)", "Disc. Amount", "Total_PPN",
                "Total_PPH", "CanvasID", "CanvasSeqNo", "BonusScheme", "CashbackScheme", "SeqNoGroup", "Deskripsi","RemainQty"};
            }

            dgvPA.ColumnCount = (DataGridViewHeader.Length);
            for (int z = 0; z < DataGridViewHeader.Length; z++)
            {
                dgvPA.Columns[z].Name = DataGridViewHeader[z].ToString();
            }
        }

        private void SetMiddleRightDatagrid()
        {
            if (txtPurchaseType.Text != "AMOUNT")
            {
                dgvPA.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPA.Columns["RemainingQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else if (txtPurchaseType.Text == "AMOUNT")
            {
                dgvPA.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPA.Columns["RemainingAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            dgvPA.Columns["RemainQty"].Visible = false;
            dgvPA.Columns["Konv_Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPA.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPA.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPA.Columns["Disc. (%)"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPA.Columns["Disc. Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPA.Columns["Total_PPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPA.Columns["Total_PPH"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void SetVisibleDatagrid()
        {

            dgvPA.Columns["GroupId"].Visible = false;
            dgvPA.Columns["SubGroup1ID"].Visible = false;
            dgvPA.Columns["SubGroup2ID"].Visible = false;
            dgvPA.Columns["ItemID"].Visible = false;
            dgvPA.Columns["FullItemID"].Visible = true;
            dgvPA.Columns["ItemName"].Visible = true;
            dgvPA.Columns["BracketDesc"].Visible = true;
            dgvPA.Columns["Base"].Visible = true;

            if (txtPurchaseType.Text != "AMOUNT")
            {
                dgvPA.Columns["Quantity"].Visible = true;
                dgvPA.Columns["RemainingQty"].Visible = false;
            }
            else if (txtPurchaseType.Text == "AMOUNT")
            {
                dgvPA.Columns["Amount"].Visible = true;
                dgvPA.Columns["RemainingAmount"].Visible = false;
            }

            dgvPA.Columns["Unit"].Visible = true;
            dgvPA.Columns["Konv_Ratio"].Visible = true;
            dgvPA.Columns["Price"].Visible = true;
            dgvPA.Columns["DeliveryMethod"].Visible = true;
            dgvPA.Columns["AvailableDate"].Visible = true;
            dgvPA.Columns["Total"].Visible = true;
            dgvPA.Columns["Disc. Type"].Visible = true;
            dgvPA.Columns["Disc. (%)"].Visible = true;
            dgvPA.Columns["Disc. Amount"].Visible = true;
            dgvPA.Columns["Total_PPN"].Visible = true;
            dgvPA.Columns["Total_PPH"].Visible = true;
            dgvPA.Columns["CanvasID"].Visible = false;
            dgvPA.Columns["CanvasSeqNo"].Visible = false;
            dgvPA.Columns["BonusScheme"].Visible = true;
            dgvPA.Columns["CashbackScheme"].Visible = true;
            dgvPA.Columns["SeqNoGroup"].Visible = false;
            dgvPA.Columns["Deskripsi"].Visible = true;
        }

        private DataGridViewComboBoxCell cellValue(string query)
        {
            cell = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            cell = new DataGridViewComboBoxCell();
            cell.Items.Add("Select");
            while (Dr2.Read())
                cell.Items.Add(Dr2[0].ToString());
            return cell;
        }

        private DataGridViewComboBoxCell DeliveryMethodValue(string query)
        {
            DeliveryMethod = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            DeliveryMethod = new DataGridViewComboBoxCell();
            DeliveryMethod.Items.Add("Select");
            while (Dr2.Read())
                DeliveryMethod.Items.Add(Dr2[0].ToString());
            return DeliveryMethod;
        }

        #endregion

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            choofdlog.FilterIndex = 3;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                FileName = new List<string>();
                Extension = new List<string>();
                sSelectedFile = new List<string>();


                int i = 0;

                foreach (string file in choofdlog.FileNames)
                {
                    FileStream objFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    int filelength = Convert.ToInt32(objFileStream.Length);
                    byte[] data = new byte[filelength];

                    objFileStream.Read(data, 0, filelength);
                    objFileStream.Close();

                    string tempFullName = Path.GetFileName(file);
                    string[] tempSplit = tempFullName.Split('.');

                    FileName.Add(tempSplit[0]);
                    Extension.Add(tempSplit[tempSplit.Count() - 1]);
                    int filesize = filelength / 1024;
                    this.dgvAttachment.Rows.Add(FileName[i], Extension[i], filesize.ToString(), System.Text.Encoding.UTF8.GetString(data));
                    test.Add(data);
                    i++;
                }

            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
                String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
                String ContentType = dgvAttachment.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["ContentType"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ContentType;
                sfd.FileName = fileName + "." + ContentType;
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.AddExtension = true;

                if (ContentType == "pdf")
                {
                    sfd.FilterIndex = 1;
                }
                else if (ContentType == "txt")
                {
                    sfd.FilterIndex = 2;
                }
                else
                {
                    sfd.FilterIndex = 3;
                }

                if (String.IsNullOrEmpty(fileid))
                {
                    MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
                    return;
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                Cmd = new SqlCommand(Query, Conn);

                byte[] data = (byte[])Cmd.ExecuteScalar();

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                    objFileStream.Write(data, 0, data.Length);
                    objFileStream.Close();
                    MessageBox.Show("Data tersimpan!");
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk didownload");
                return;
            }
        }

        private void btnDelAttachment_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                if (dgvAttachment.CurrentRow.Index > -1)
                {
                    test.RemoveAt(dgvAttachment.CurrentRow.Index);
                    dgvAttachment.Rows.RemoveAt(dgvAttachment.CurrentRow.Index);
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                dgvPA.AutoResizeColumns();
            }

        }

        private void dgvPA_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "AvailableDate")
            {
                dtp.Location = dgvPA.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                dtp.Visible = true;

                if (dgvPA.CurrentCell.Value != "" && dgvPA.CurrentCell.Value != null)
                {
                    DateTime dDate;
                    if (!DateTime.TryParse(dgvPA.CurrentCell.Value.ToString(), out dDate))
                    {
                        dtp.Value = Convert.ToDateTime(FormateDateyyyymmdd(dgvPA.CurrentCell.Value.ToString()));
                    }
                    else
                    {
                        dtp.Value = Convert.ToDateTime(dgvPA.CurrentCell.Value);
                    }
                }
                else
                {
                    dtp.Value = DateTime.Now;
                }
            }
            else
            {
                dtp.Visible = false;
            }
        }

        private string FormateDateyyyymmdd(string tmpDate)
        {
            //string reformat="";
            string[] data = tmpDate.Split('/');
            return data[2] + "/" + data[1] + "/" + data[0];
        }

        private void dgvPA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((dgvPA.Columns[dgvPA.CurrentCell.ColumnIndex].Name == "Quantity" && Convert.ToDecimal(dgvPA.Rows[dgvPA.CurrentRow.Index].Cells["RemainingQty"].Value) > 0) ||
                dgvPA.Columns[dgvPA.CurrentCell.ColumnIndex].Name == "Amount" ||
                dgvPA.Columns[dgvPA.CurrentCell.ColumnIndex].Name == "Price" ||
                dgvPA.Columns[dgvPA.CurrentCell.ColumnIndex].Name == "Disc. (%)" ||
                dgvPA.Columns[dgvPA.CurrentCell.ColumnIndex].Name == "Disc. Amount" ||
                dgvPA.Columns[dgvPA.CurrentCell.ColumnIndex].Name == "BonusScheme" ||
                dgvPA.Columns[dgvPA.CurrentCell.ColumnIndex].Name == "CashbackScheme"
                )
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

            }
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(Mode == "View" || Mode == "Ammend"))
            {
                if (cmbCurrency.Text == "IDR")
                {
                    txtExchangeRate.Text = "1";
                    txtExchangeRate.ReadOnly = true;
                }
                else
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [ExchRate] from [dbo].[ExchRate] where CurrencyId='" + cmbCurrency.Text + "' and convert(varchar(10), CreatedDate, 102)=convert(varchar(10), getdate(), 102)", Conn);
                    if (Cmd.ExecuteScalar() != null)
                    {
                        txtExchangeRate.Text = Cmd.ExecuteScalar().ToString();
                        txtExchangeRate.ReadOnly = true;
                    }
                    else
                    {
                        MessageBox.Show("ExchRate (" + cmbCurrency.Text + ") untuk hari ini belum di set, silahkan diisi sendiri.");
                        txtExchangeRate.ReadOnly = false;
                    }
                    Conn.Close();
                }
            }
        }

        private void cmbDPRequired_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbDPRequired.Text == "NO")
            //{
            //    txtDPPercent.ReadOnly = true;
            //    txtDPAmount.ReadOnly = true;
            //    txtDPPercent.Text = "0.00";
            //    txtDPAmount.Text = "0.00";
            //}
            //else
            //{
            //    txtDPPercent.ReadOnly = false;
            //    txtDPAmount.ReadOnly = false;
            //}

            //BY: HC (S)
            if (cmbDPRequired.Text == "NO")
            {
                label16.Visible = false;
                cmbDPType.Visible = false;
                cbxHitung.Visible = false;
                label24.Visible = false;
                tbxDPAmount.Visible = false;
                tbxDPPercent.Visible = false;
            }
            else if (cmbDPRequired.Text == "YES")
            {
                cmbDPType.Text = "Percentage";
                label16.Visible = true;
                cmbDPType.Visible = true;
                //cbxHitung.Visible = true;
                label24.Visible = true;
                tbxDPPercent.Visible = true;
            }
            //BY: HC (E)
        }

        //private void txtDPPercent_TextChanged(object sender, EventArgs e)
        //{
        //    if (txtDPPercent.Text == "")
        //        txtDPPercent.Text = "0";
        //    txtDPAmount.Text = (Convert.ToDecimal(txtGrandTotal.Text) * Convert.ToDecimal(txtDPPercent.Text) / 100).ToString("N4");

        //    //txtDPAmount.Text = (Convert.ToDecimal(txtTotalNett.Text) * Convert.ToDecimal(txtDPPercent.Text) / 100).ToString("N4");

        //}

        //private void txtDPAmount_TextChanged(object sender, EventArgs e)
        //{
        //    if (txtDPAmount.Text == "")
        //        txtDPAmount.Text = "0";

        //    //double d = double.Parse(txtDPAmount.Text.ToString());
        //    //txtDPAmount.Text = d.ToString("N2");
        //}
        //tia edit
        //klik kanan
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.FullItemId.FullItemId FID = null;
        Purchase.CanvasSheet.FormCanvasSheet2 CS = null;
        Purchase.PurchaseAgreement.PAForm Pa = null;

        Purchase.PurchaseOrderNew.POForm ParentToPO;

        public void ParentRefreshGrid(Purchase.PurchaseOrderNew.POForm Po)
        {
            ParentToPO = Po;
        }

        private void dgvPA_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            //{
            //    if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPA.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
            //    {
            //        PopUp.Stock.Stock f = new PopUp.Stock.Stock();
            //        itemID = dgvPA.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
            //        f.Show();
            //    }
            //}
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {

                if (FID == null || FID.Text == "")
                {
                    if (dgvPA.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPA.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        //PopUpItemName.Close();
                        // PopUpItemName = new PopUp.Stock.Stock();
                        //PopUpItemName = new PopUp.FullItemId.FullItemId();
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPA.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvPA.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvPA.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }

            }
        }

        private void txtVendID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    txtVendID.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendID.Text);

                    Vendor.Show();

                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
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

        private void txtVendorName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    txtVendorName.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtVendID.Text);
                    Vendor.Show();

                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }
        //PopUp.FullItemId.FullItemId FID = null;
        private void dgvReference_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {

                if (FID == null || FID.Text == "")
                {
                    if (dgvReference.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvReference.Columns[e.ColumnIndex].Name.ToString() == "FullItemID" || dgvReference.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {

                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvReference.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvReference.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvReference.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
                //untuk PA dan CS pada datagridview
                if (txtReferenceType.Text == "Canvas Sheet")
                {
                    if (CS == null || CS.Text == "")
                    {
                        if (dgvReference.Columns[e.ColumnIndex].Name.ToString() == "CS No")
                        {
                            CS = new Purchase.CanvasSheet.FormCanvasSheet2();
                            CS.ModePopUp(txtRefID.Text);
                            CS.ParentRefreshGrid(this);
                            CS.Show();
                        }
                    }
                    else if (CheckOpened(CS.Name))
                    {
                        CS.WindowState = FormWindowState.Normal;
                        CS.Show();
                        CS.Focus();
                    }
                }
                else if (txtReferenceType.Text == "Purchase Agreement")
                {

                    if (Pa == null || Pa.Text == "")
                    {
                        if (dgvReference.Columns[e.ColumnIndex].Name.ToString() == "PA No")
                        {
                            Pa = new Purchase.PurchaseAgreement.PAForm();
                            Pa.SetMode("View", "", txtRefID.Text);
                            // Pa.GetDataHeader();
                            Pa.Show();

                            //}
                        }
                    }
                    else if (CheckOpened(Pa.Name))
                    {
                        Pa.WindowState = FormWindowState.Normal;
                        Pa.Show();
                        Pa.Focus();
                    }
                }
            }
        }
        //masih salah
        public bool exit_CS = true;
        private void txtRefID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (txtReferenceType.Text == "Canvas Sheet")
                {
                    if (CS == null || CS.Text == "")
                    {
                        CS = new Purchase.CanvasSheet.FormCanvasSheet2();
                        CS.ModePopUp(txtRefID.Text);
                        CS.ParentRefreshGrid(this);
                        CS.Show();
                    }
                    else if (CheckOpened(CS.Name))
                    {
                        CS.WindowState = FormWindowState.Normal;
                        CS.Show();
                        CS.Focus();
                    }
                }
                else if (txtReferenceType.Text == "Purchase Agreement")
                {

                    if (Pa == null || Pa.Text == "")
                    {
                        Pa = new Purchase.PurchaseAgreement.PAForm();
                        Pa.SetMode("View", "", txtRefID.Text);
                        // Pa.GetDataHeader();
                        Pa.Show();

                        //}
                    }
                    else if (CheckOpened(Pa.Name))
                    {
                        Pa.WindowState = FormWindowState.Normal;
                        Pa.Show();
                        Pa.Focus();
                    }
                }
            }
        }

        private void dgvReference_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvReference.Columns[e.ColumnIndex].Name == "Ratio")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.0000";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }
            if (dgvReference.Columns[e.ColumnIndex].Name == "Amount" || dgvReference.Columns[e.ColumnIndex].Name == "RemainingAmount" || dgvReference.Columns[e.ColumnIndex].Name == "Qty" || dgvReference.Columns[e.ColumnIndex].Name == "RemainingQty")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
        }

        //private void cbxHitung_CheckedChanged(object sender, EventArgs e)
        //{
        //    //BY: HC (S)
        //    if (cbxHitung.Checked == true)
        //    {
        //        tbxDPAmount.Location = new Point(211, 149);
        //        tbxDPAmount.Visible = true;
        //        tbxDPAmount.Enabled = false;
        //        tbxDPAmount.Text = (Convert.ToDecimal(tbxDPPercent.Text) * Convert.ToDecimal(txtGrandTotal.Text) / 100).ToString("N2");
        //    }
        //    else if (cbxHitung.Checked == false)
        //    {
        //        tbxDPAmount.Location = new Point(211, 126);
        //        tbxDPAmount.Visible = false;
        //        tbxDPAmount.Enabled = true;
        //    }
        //    //BY: HC (E)
        //}

        private void cmbDPType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BY: HC (S)
            switch (cmbDPType.SelectedIndex)
            {
                case 0: //PERCENTAGE
                    cbxHitung.Checked = false;
                    tbxDPAmount.Visible = false;
                    tbxDPPercent.Visible = true;
                    label24.Visible = true;
                    //cbxHitung.Visible = true;
                    break;
                case 1: //AMOUNT
                    cbxHitung.Checked = false;
                    tbxDPAmount.Visible = true;
                    tbxDPPercent.Visible = false;
                    label24.Visible = false;
                    cbxHitung.Visible = false;
                    break;
            }
            //BY: HC (E)
        }

        private void tbxDPPercent_Leave(object sender, EventArgs e)
        {
            tbxDPPercent.Text = Convert.ToDecimal(tbxDPPercent.Text).ToString("N2");
        }

        private void tbxDPAmount_Leave(object sender, EventArgs e)
        {
            tbxDPAmount.Text = Convert.ToDecimal(tbxDPAmount.Text).ToString("N2");
        }

        private void txtExchangeRate_Leave_1(object sender, EventArgs e)
        {
            txtExchangeRate.Text = Convert.ToDecimal(txtExchangeRate.Text).ToString("N2");
        }

        private void dgvPA_Scroll(object sender, ScrollEventArgs e)
        {
            dtp.Visible = false;
        }
        //tia edit end



        //=========================================end==============================================

    }
}
