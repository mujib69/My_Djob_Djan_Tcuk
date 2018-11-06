using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace ISBS_New.Sales.MoUCustomer
{
    public partial class HeaderMoUCustomer : MetroFramework.Forms.MetroForm
    {

        public string MoUNumber = "";

        List<DetailMoUCustomer> ListDetailMoUCustomer = new List<DetailMoUCustomer>();

        Sales.MoUCustomer.InqueryMoUCustomer Parent;

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, crit = null;

        int Index;
        
        decimal MoU_Before = 0;
        
        ContextMenu vendid = new ContextMenu();

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public HeaderMoUCustomer()
        {
            InitializeComponent();           
        }

        public void SetParent(Sales.MoUCustomer.InqueryMoUCustomer F)
        {
            Parent = F;
        }

        public void SetMode(string tmpMode, string tmpMoUNumber)
        {
            Mode = tmpMode;
            MoUNumber = tmpMoUNumber;
            txtMoUNumber.Text = tmpMoUNumber;
        }
       
        private void HeaderMoUCustomer2_Load(object sender, EventArgs e)
        {
            txtLCNo.Enabled = false;
            txtLCType.Enabled = false;
            dtLC.Enabled = false;
            dtJatuhTempo.Enabled = false;
            btnLookUpBank.Enabled = false;

            dtLC.CustomFormat = " ";
            dtLC.Format = DateTimePickerFormat.Custom;
            dtJatuhTempo.CustomFormat = " ";
            dtJatuhTempo.Format = DateTimePickerFormat.Custom;
            SetCmbBankGuarantee();
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
            else if (Mode=="PopUp")//tia edit
            {
                ModePopUp();
            }
            //tia edit end

            Sales.MoUCustomer.InqueryMoUCustomer f = new Sales.MoUCustomer.InqueryMoUCustomer();
           // f.RefreshGrid();
        }

        private void SetCmbBankGuarantee()
        {
            cmbBankGuarantee.DataSource = null;
            cmbBankGuarantee.Items.Add("-select-");
            cmbBankGuarantee.Items.Add("Yes");
            cmbBankGuarantee.Items.Add("No");
            cmbBankGuarantee.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBankGuarantee.SelectedIndex = 0;
        }

        private void HeaderMoUCustomer2_Shown(object sender, EventArgs e)
        {
            //this.Location = new Point(170, 63);
        }

        private void HeaderMoUCustomer2_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < ListDetailMoUCustomer.Count(); i++)
            {
                ListDetailMoUCustomer[i].Close();
            }
         //   Sales.MoUCustomer.InqueryMoUCustomer f = new Sales.MoUCustomer.InqueryMoUCustomer();
          //  f.RefreshGrid();
        }

        public void ModeNew()
        {
            MoUNumber = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
        }

        private bool Used()
        {
            Boolean vBol = false;
            try
            {
                Query = "SELECT SalesMouNo FROM SalesQuotationH WHERE SalesMouNo='" + txtMoUNumber.Text + "' ";
                Query += "UNION ALL SELECT SalesMouNo FROM SalesOrderH WHERE SalesMouNo='" + txtMoUNumber.Text + "' ";
                Query += "UNION ALL SELECT SalesMouNo FROM SalesAgreementH WHERE SalesMouNo='" + txtMoUNumber.Text + "' ";
                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();
                    if (Dr.Read())
                    {
                        vBol = true;
                    }
                    else
                    {
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

            return vBol;
        }

        public void ModeEdit()
        {
            if (Used()) { MessageBox.Show("Tidak boleh Edit, MoU sudah pernah digunakan.."); return; }
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;

            btnNew.Enabled = true;
            btnDelete.Enabled = true;
            dtValidTo.Enabled = true;
            textLimitCredit.Enabled = true;
            cmbBankGuarantee.Enabled = true;

            if (cmbBankGuarantee.SelectedIndex == 1)
            {
                txtBankID.Enabled = true;
                txtBankName.Enabled = true;
                dtLC.Enabled = true;
                dtJatuhTempo.Enabled = true;
                txtLCNo.Enabled = true;
                txtLCType.Enabled = true;
                btnLookUpBank.Enabled = true;
            }
            else
            {
                txtBankID.Enabled = false;
                txtBankName.Enabled = false;
                dtLC.Enabled = false;
                dtJatuhTempo.Enabled = false;
                txtLCNo.Enabled = false;
                txtLCType.Enabled = false;
                btnLookUpBank.Enabled = false;
            }       
           

            dgvMoUCustomerDetails.ReadOnly = false;
            dgvMoUCustomerDetails.Columns["No"].ReadOnly = true;
            dgvMoUCustomerDetails.Columns["CustID"].ReadOnly = true;
            dgvMoUCustomerDetails.Columns["CustName"].ReadOnly = true;

            dgvMoUCustomerDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvMoUCustomerDetails.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvMoUCustomerDetails.Columns["CreditAmount"].SortMode = DataGridViewColumnSortMode.NotSortable;


            dgvMoUCustomerDetails.AutoResizeColumns();
            EditColor();
            dgvMoUCustomerDetails.DefaultCellStyle.BackColor = Color.White;
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
            textLimitCredit.Enabled = false;
            dtValidTo.Enabled = false;
            btnLookUpCustGroup.Enabled = false;

            cmbBankGuarantee.Enabled = false;
            txtBankID.Enabled = false;
            txtBankName.Enabled = false;
            dtLC.Enabled = false;
            dtJatuhTempo.Enabled = false;
            txtLCNo.Enabled = false;
            txtLCType.Enabled = false;
            btnLookUpBank.Enabled = false;
            //tia edit
            textCustGroupID.Enabled = true;
            textCustGroupName.Enabled = true;
            textCustGroupID.ReadOnly = true;
            textCustGroupName.ReadOnly = true;
            textCustGroupID.ContextMenu = vendid;
            textCustGroupName.ContextMenu = vendid;
            //tia end
            dgvMoUCustomerDetails.ReadOnly = true;
            BeforeEditColor();
            dgvMoUCustomerDetails.DefaultCellStyle.BackColor = Color.LightGray;
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

            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            textLimitCredit.Enabled = false;
            dtValidTo.Enabled = false;
            btnLookUpCustGroup.Enabled = false;

            cmbBankGuarantee.Enabled = false;
            txtBankID.Enabled = false;
            txtBankName.Enabled = false;
            dtLC.Enabled = false;
            dtJatuhTempo.Enabled = false;
            txtLCNo.Enabled = false;
            txtLCType.Enabled = false;
            btnLookUpBank.Enabled = false;
            //tia edit
            textCustGroupID.Enabled = true;
            textCustGroupName.Enabled = true;
            textCustGroupID.ReadOnly = true;
            textCustGroupName.ReadOnly = true;
            textCustGroupID.ContextMenu = vendid;
            textCustGroupName.ContextMenu = vendid;
            //tia end
            dgvMoUCustomerDetails.ReadOnly = true;
            BeforeEditColor();
            dgvMoUCustomerDetails.DefaultCellStyle.BackColor = Color.LightGray;
        }
        //tia edit end

        private void EditColor()
        {
            for (int i = 0; i < dgvMoUCustomerDetails.RowCount; i++)
            {
                dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Style.BackColor = Color.White;
            }
        }

        private void BeforeEditColor()
        {
            for (int i = 0; i < dgvMoUCustomerDetails.RowCount; i++)
            {
                dgvMoUCustomerDetails.Rows[i].Cells["CustID"].Style.BackColor = Color.LightGray;
                dgvMoUCustomerDetails.Rows[i].Cells["CustName"].Style.BackColor = Color.LightGray;
                dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Style.BackColor = Color.LightGray;
            }
        }

        public void AddDataGridDetail(List<string> CustID, List<string> CustName, List<string> MoUBalance, List<string> Sisa_Limit_Mou)
        {
            if (dgvMoUCustomerDetails.RowCount - 1 <= 0)
            {
                dgvMoUCustomerDetails.ColumnCount = 6;
                dgvMoUCustomerDetails.Columns[0].Name = "No";
                dgvMoUCustomerDetails.Columns[1].Name = "CustID";
                dgvMoUCustomerDetails.Columns[2].Name = "CustName";
                dgvMoUCustomerDetails.Columns[3].Name = "CreditAmount";
                dgvMoUCustomerDetails.Columns[4].Name = "MoUBalance";
                dgvMoUCustomerDetails.Columns[5].Name = "SisaLimitMoU";
            }


            string CreditAmount = "0.0000";

            for (int i = 0; i < CustID.Count; i++)
            {
                this.dgvMoUCustomerDetails.Rows.Add((dgvMoUCustomerDetails.RowCount + 1).ToString(), CustID[i], CustName[i], CreditAmount, MoUBalance[i], Sisa_Limit_Mou[i]);
            }

            dgvMoUCustomerDetails.ReadOnly = false;
            dgvMoUCustomerDetails.Columns["No"].ReadOnly = true;
            dgvMoUCustomerDetails.Columns["CustID"].ReadOnly = true;
            dgvMoUCustomerDetails.Columns["CustName"].ReadOnly = true;

            dgvMoUCustomerDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvMoUCustomerDetails.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvMoUCustomerDetails.Columns["CreditAmount"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvMoUCustomerDetails.AutoResizeColumns();
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvMoUCustomerDetails.RowCount; i++)
            {
                dgvMoUCustomerDetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        public void GetDataHeader()
        {
            try
            {
                if (MoUNumber == "")
                {
                    MoUNumber = txtMoUNumber.Text.Trim();
                }
                Conn = ConnectionString.GetConnection();

                Query = "Select MoUNo, MoUDate, ValidTo, CustID, CustName, TotalAmount, BankGuarantee, LC_No, LC_Type, LC_Date, LC_DueDate, BankID From [CustMouH]  ";
                Query += "Where MoUNo = '" + MoUNumber + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                string BankGuarantee = "";
                while (Dr.Read())
                {
                    txtMoUNumber.Text = Dr["MoUNo"].ToString();
                    dtMoUDate.Text = Dr["MoUDate"].ToString();
                    textCustGroupID.Text = Dr["CustID"].ToString();
                    textCustGroupName.Text = Dr["CustName"].ToString();
                    textLimitCredit.Text = Dr["TotalAmount"].ToString();
                    dtValidTo.Text = Dr["ValidTo"].ToString();
                    txtLCNo.Text = Dr["LC_No"].ToString();
                    txtLCType.Text = Dr["LC_Type"].ToString();
                    txtBankID.Text = Dr["BankID"].ToString();
                    BankGuarantee = Dr["BankGuarantee"].ToString();
                    dtLC.Text = Dr["LC_Date"].ToString();
                    dtJatuhTempo.Text = Dr["LC_DueDate"].ToString();                 
                }
                Dr.Close();

                if (BankGuarantee == "Yes")
                {
                    cmbBankGuarantee.SelectedIndex = 1;
                }
                else if (BankGuarantee == "No")
                {
                    cmbBankGuarantee.SelectedIndex = 2;
                }
                else
                {
                    cmbBankGuarantee.SelectedIndex = 0;
                }

                Conn = ConnectionString.GetConnection();

                Query = "Select tblBank_Group_Nama From tblBank_Group Where tblBank_Group_Group = '" + txtBankID.Text + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtBankName.Text = Dr["tblBank_Group_Nama"].ToString();
                }
                Dr.Close();



                dgvMoUCustomerDetails.Rows.Clear();
                if (dgvMoUCustomerDetails.RowCount - 1 <= 0)
                {
                    dgvMoUCustomerDetails.ColumnCount = 6;
                    dgvMoUCustomerDetails.Columns[0].Name = "No";
                    dgvMoUCustomerDetails.Columns[1].Name = "CustID";
                    dgvMoUCustomerDetails.Columns[2].Name = "CustName";
                    dgvMoUCustomerDetails.Columns[3].Name = "CreditAmount";
                    dgvMoUCustomerDetails.Columns[4].Name = "MoUBalance";
                    dgvMoUCustomerDetails.Columns[5].Name = "SisaLimitMoU";
                }

                Query = "Select SeqNo AS No, a.CustID, a.CustName, a.Amount AS CreditAmount, b.Mou_Balance, b.Sisa_Limit_Mou From [CustMou_Dtl] a left join CustTable b on a.CustID=b.CustId Where MoUNo = '" + MoUNumber + "' order by SeqNo asc";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int i = 0;
                while (Dr.Read())
                {

                    this.dgvMoUCustomerDetails.Rows.Add(i + 1, Dr[1], Dr[2], Dr[3], Dr[4], Dr[5]);
                    i++;
                }
                Dr.Close();

                dgvMoUCustomerDetails.ReadOnly = false;
                dgvMoUCustomerDetails.Columns["No"].ReadOnly = true;
                dgvMoUCustomerDetails.Columns["CustID"].ReadOnly = true;
                dgvMoUCustomerDetails.Columns["CustName"].ReadOnly = true;
                dgvMoUCustomerDetails.Columns["MoUBalance"].ReadOnly = true;
                dgvMoUCustomerDetails.Columns["SisaLimitMoU"].ReadOnly = true;

                dgvMoUCustomerDetails.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvMoUCustomerDetails.Columns["CustID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvMoUCustomerDetails.Columns["CreditAmount"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvMoUCustomerDetails.Columns["MoUBalance"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvMoUCustomerDetails.Columns["SisaLimitMoU"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvMoUCustomerDetails.AutoResizeColumns();

                Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            DetailMoUCustomer DetailMoUCustomer = new DetailMoUCustomer();

            List<DetailMoUCustomer> ListDetailMoUCustomer = new List<DetailMoUCustomer>();
            DetailMoUCustomer.ParentRefreshGrid(this);
            DetailMoUCustomer.ParamHeader(dgvMoUCustomerDetails);
            DetailMoUCustomer.ShowDialog();
            EditColor();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMoUCustomerDetails.RowCount > 0)
                if (dgvMoUCustomerDetails.RowCount > 0)
                {
                    Index = dgvMoUCustomerDetails.CurrentRow.Index;
                    DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + "No = " + dgvMoUCustomerDetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "CustID = " + dgvMoUCustomerDetails.Rows[Index].Cells["CustID"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        dgvMoUCustomerDetails.Rows.RemoveAt(Index);
                        SortNoDataGrid();
                    }
                }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (dgvMoUCustomerDetails.RowCount == 0)
            {
                MessageBox.Show("Jumlah item tidak boleh kosong.");
                return;
            }
            else
            {
                Decimal TotalCreditAmount = 0;

                for (int i = 0; i <= dgvMoUCustomerDetails.RowCount - 1; i++)
                {
                    if ((dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == null ? "" : dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString()) == "")
                    {

                        MessageBox.Show("Item No = " + dgvMoUCustomerDetails.Rows[i].Cells["No"].Value + ", Credit Amount tidak boleh kosong.");
                        return;
                    }
                    else if (Convert.ToDecimal((dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == "" ? "0.0000" : dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString())) <= 0)
                    {

                        MessageBox.Show("Item No = " + dgvMoUCustomerDetails.Rows[i].Cells["No"].Value + ", Credit Amount tidak boleh lebih kecil atau sama dengan 0");
                        return;
                    }

                    TotalCreditAmount = TotalCreditAmount + (dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == "" ? 0 : Convert.ToDecimal(dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString()));
                }

                DateTime ValidDate = DateTime.ParseExact(dtValidTo.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                DateTime CurrentDate = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);

                if (ValidDate <= CurrentDate)
                {
                    MessageBox.Show("Valid Date harus lebih besar dari hari ini");
                    return;
                }
                else if (textCustGroupID.Text == "")
                {
                    MessageBox.Show("Customer ID harus diisi");
                    return;
                }
                else if (textCustGroupName.Text == "")
                {
                    MessageBox.Show("Customer Name harus diisi");
                    return;
                }
                else if (Convert.ToDecimal((textLimitCredit.Text == "" ? "0.000" : textLimitCredit.Text)) <= 0)
                {
                    MessageBox.Show("Limit Credit tidak boleh lebih kecil atau sama dengan 0");
                    return;
                }
                else if (TotalCreditAmount > Convert.ToDecimal(textLimitCredit.Text))
                {
                    MessageBox.Show("Total Credit Amount harus lebih kecil atau sama dengan Limit Credit");
                    return;
                }

                if (Convert.ToString(cmbBankGuarantee.Text) == "-select-")
                {
                    MessageBox.Show("Bank Guarantee harus diisi");
                    return;
                }

                if (Convert.ToString(cmbBankGuarantee.Text) == "Yes")
                {
                    DateTime LCDate = DateTime.ParseExact(dtLC.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                    DateTime JatuhTempo = DateTime.ParseExact(dtJatuhTempo.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", null);
                
                    //if (CurrentDate > LCDate)
                    //{
                    //    MessageBox.Show("LC Date harus lebih besar atau sama dengan dari hari ini");
                    //    return;
                    //}
                    if (JatuhTempo <= LCDate)
                    {
                        MessageBox.Show("Jatuh Tempo harus lebih besar dari LC Date");
                        return;
                    }
                    else if (txtLCNo.Text == "")
                    {
                        MessageBox.Show("LC No harus diisi");
                        return;
                    }
                    else if (txtLCType.Text == "")
                    {
                        MessageBox.Show("LC Type harus diisi");
                        return;
                    }
                    else if (txtBankID.Text == "")
                    {
                        MessageBox.Show("Bank ID harus diisi");
                        return;
                    }
                    else if (txtBankName.Text == "")
                    {
                        MessageBox.Show("Bank Name harus diisi");
                        return;
                    }
                }
            }

            try
            {
                string dateLC = dtLC.Text == "" ? DBNull.Value.ToString() : dtLC.Value.ToString("yyyy-MM-dd");
                string dateJatuhTempo = dtJatuhTempo.Text == "" ? DBNull.Value.ToString() : dtJatuhTempo.Value.ToString("yyyy-MM-dd");

                using (TransactionScope scope = new TransactionScope())
                {
                    Conn = ConnectionString.GetConnection();

                    if (Mode == "New" || txtMoUNumber.Text == "")
                    {
                        //Old Code=======================================================================================                  

                        //Query = "Insert into CustMouH (MoUNo, MoUDate, CustID, CustName, ValidTo, TotalAmount,CreatedDate, CreatedBy, BankGuarantee, LC_No, LC_Type, LC_Date, LC_DueDate, BankID) OUTPUT INSERTED.MoUNo values ";
                        //Query += "((Select 'MOU/'+FORMAT(getdate(), 'yyMM')+'/'+Right('00000' + CONVERT(NVARCHAR, case when Max(MoUNo) is null then '1' else substring(Max(MoUNo),11,5)+1 end), 5) ";
                        //Query += "from [CustMouH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6)),";
                        //Query += "'" + dtMoUDate.Value.ToString("yyyy-MM-dd") + "', '" + textCustGroupID.Text + "', '" + textCustGroupName.Text + "','" + dtValidTo.Value.ToString("yyyy-MM-dd") + "',";
                        //Query += "'" + textLimitCredit.Text + "',getdate(),'" + ControlMgr.UserId + "', '" + Convert.ToString(cmbBankGuarantee.Text) + "', '" + txtLCNo.Text + "', '" + txtLCType.Text + "', '" + dateLC + "', '" + dateJatuhTempo + "', '" + txtBankID.Text + "');";
                        //Cmd = new SqlCommand(Query, Conn, Trans);

                        //string MoUNumber = Cmd.ExecuteScalar().ToString();
                        //End Old Code=====================================================================================

                        //begin============================================================================================
                        //updated by : joshua
                        //updated date : 14 Feb 2018
                        //description : change generate sequence number, get from global function and update counter 
                        string Jenis = "MOU", Kode = "MOU";
                        MoUNumber = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);

                        //Query = "Insert into CustMouH (MoUNo, MoUDate, CustID, CustName, ValidTo, TotalAmount,CreatedDate, CreatedBy, BankGuarantee, LC_No, LC_Type, LC_Date, LC_DueDate, BankID) values ";
                        //Query += "('" + MoUNumber + "', '";
                        //Query += dtMoUDate.Value.ToString("yyyy-MM-dd") + "', '";
                        //Query += textCustGroupID.Text + "', '";
                        //Query += textCustGroupName.Text + "','";
                        //Query += dtValidTo.Value.ToString("yyyy-MM-dd") + "',";
                        //Query += "'" + Convert.ToDecimal(textLimitCredit.Text) + "', ";
                        //Query += "getdate(),'";
                        //Query += ControlMgr.UserId + "', '";
                        //Query += Convert.ToString(cmbBankGuarantee.Text) + "', '";
                        //Query += txtLCNo.Text + "', '";
                        //Query += txtLCType.Text + "', '";
                        //Query += dateLC + "', '";
                        //Query += dateJatuhTempo + "', '";
                        //Query += txtBankID.Text + "');";
                        //Cmd = new SqlCommand(Query, Conn, Trans);

                        Query = "Insert into CustMouH (MoUNo, MoUDate, CustID, CustName, ValidTo, TotalAmount,CreatedDate, CreatedBy, BankGuarantee, LC_No, LC_Type, LC_Date, LC_DueDate, BankID) values ";
                        Query += "(@MoUNumber,";
                        Query += "@MoUDate,";
                        Query += "@CustGroupID,";
                        Query += "@CustGroupName,";
                        Query += "@ValidTo,";
                        Query += "@Limit,";
                        Query += "getdate(),";
                        Query += "@Login,";
                        Query += "@BankGuarantee,";
                        Query += "@LCNo,";
                        Query += "@LCType,";
                        Query += "@dateLC,";
                        Query += "@dateJatuhTempo,";
                        Query += "@BankID);";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.Add("@MoUNumber", MoUNumber);
                        Cmd.Parameters.Add("@MoUDate", dtMoUDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@CustGroupID", textCustGroupID.Text);
                        Cmd.Parameters.Add("@CustGroupName", textCustGroupName.Text);
                        Cmd.Parameters.Add("@ValidTo", dtValidTo.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@Limit", Convert.ToDecimal(textLimitCredit.Text));
                        Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                        Cmd.Parameters.Add("@BankGuarantee", Convert.ToString(cmbBankGuarantee.Text));
                        Cmd.Parameters.Add("@LCNo", txtLCNo.Text);
                        Cmd.Parameters.Add("@LCType", txtLCType.Text);
                        Cmd.Parameters.Add("@dateLC", dateLC == null ? DBNull.Value.ToString() : dateLC);
                        Cmd.Parameters.Add("@dateJatuhTempo", dateJatuhTempo == null ? DBNull.Value.ToString() : dateJatuhTempo);
                        Cmd.Parameters.Add("@BankID", txtBankID.Text);
                        Cmd.ExecuteNonQuery();

                        //update counter
                        //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                        //end update counter
                        //end=============================================================================================

                        Query = "";
                        for (int i = 0; i <= dgvMoUCustomerDetails.RowCount - 1; i++)
                        {
                            //Query += "Insert CustMou_Dtl (MoUNo, CustID, CustName, Amount, CreatedDate, CreatedBy) Values ";
                            //Query += "('" + MoUNumber + "','";
                            //Query += (dgvMoUCustomerDetails.Rows[i].Cells["CustID"].Value == null ? "" : dgvMoUCustomerDetails.Rows[i].Cells["CustID"].Value.ToString()) + "','";
                            //Query += (dgvMoUCustomerDetails.Rows[i].Cells["CustName"].Value == null ? "" : dgvMoUCustomerDetails.Rows[i].Cells["CustName"].Value.ToString()) + "','";
                            //Query += (dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == null ? 0 : Convert.ToDecimal(dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString())) + "',";
                            //Query += "getdate(),";
                            //Query += "'" + ControlMgr.UserId + "');";
                            Query = "Insert CustMou_Dtl (MoUNo, CustID, CustName, Amount, Remaining_Amount, CreatedDate, CreatedBy) Values ";
                            Query += "(@MoUNumber,";
                            Query += "@CustID,";
                            Query += "@CustName,";
                            Query += "@CreditAmount,";
                            Query += "@Remaining_Amount,";
                            Query += "getdate(),";
                            Query += "@Login);";
                            Query += "Update [dbo].[CustTable] Set Sisa_Limit_MoU=Sisa_Limit_MoU+@CreditAmount, MoU_Balance=MoU_Balance+@CreditAmount where CustID=@CustID;";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@MoUNumber", MoUNumber);
                            Cmd.Parameters.Add("@CustID", (dgvMoUCustomerDetails.Rows[i].Cells["CustID"].Value == null ? "" : dgvMoUCustomerDetails.Rows[i].Cells["CustID"].Value.ToString()));
                            Cmd.Parameters.Add("@CustName", (dgvMoUCustomerDetails.Rows[i].Cells["CustName"].Value == null ? "" : dgvMoUCustomerDetails.Rows[i].Cells["CustName"].Value.ToString()));
                            Cmd.Parameters.Add("@CreditAmount", (dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == null ? 0 : Convert.ToDecimal(dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString())));
                            Cmd.Parameters.Add("@Remaining_Amount", (dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == null ? 0 : Convert.ToDecimal(dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString())));
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.ExecuteNonQuery();
                        }

                        //Save CustMou_Logs
                        //Query = "Select Deskripsi from TransStatusTable where TransCode='PaymentVoucher' and StatusCode='01'";
                        //Cmd = new SqlCommand(Query, Conn);
                        //string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                        SaveCustMouLogs(MoUNumber, "-", "Created", "N");

                        MessageBox.Show("Data MoU Number : " + MoUNumber + " berhasil ditambahkan.");
                        txtMoUNumber.Text = MoUNumber;
                        MainMenu f = new MainMenu();
                    }
                    else
                    {
                        Query = "Update CustMouH set ";
                        Query += "MoUDate=@MoUDate,";
                        Query += "ValidTo=@ValidTo,";
                        Query += "CustID=@CustID,";
                        Query += "CustName=@CustName,";
                        Query += "BankGuarantee=@BankGuarantee,";
                        Query += "LC_No=@LC_No,";
                        Query += "LC_Type=@LC_Type,";
                        Query += "LC_Date=@LC_Date,";
                        Query += "LC_DueDate=@LC_DueDate,";
                        Query += "BankID=@BankID,";
                        Query += "TotalAmount=@TotalAmount,";
                        Query += "UpdatedDate=getdate(),";
                        Query += "UpdatedBy=@Login where MoUNo=@MoUNo";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.Parameters.Add("@MoUDate", dtMoUDate.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@ValidTo", dtValidTo.Value.ToString("yyyy-MM-dd"));
                        Cmd.Parameters.Add("@CustID", textCustGroupID.Text);
                        Cmd.Parameters.Add("@CustName", textCustGroupName.Text);
                        Cmd.Parameters.Add("@BankGuarantee", Convert.ToString(cmbBankGuarantee.Text));
                        Cmd.Parameters.Add("@LC_No", txtLCNo.Text);
                        Cmd.Parameters.Add("@LC_Type", txtLCType.Text);
                        Cmd.Parameters.Add("@LC_Date", dateLC);
                        Cmd.Parameters.Add("@LC_DueDate", dateJatuhTempo);
                        Cmd.Parameters.Add("@BankID", txtBankID.Text);
                        Cmd.Parameters.Add("@TotalAmount", Convert.ToDecimal(textLimitCredit.Text));
                        Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                        Cmd.Parameters.Add("@MoUNo", txtMoUNumber.Text.Trim());
                        Cmd.ExecuteNonQuery();

                        
                        Query = "Select CustID,Amount from [dbo].[CustMou_Dtl] where MouNo='" + txtMoUNumber.Text + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            Query = "Update [dbo].[CustTable] Set Sisa_Limit_MoU=Sisa_Limit_MoU-" + Convert.ToDecimal(Dr["Amount"]) + ", MoU_Balance=MoU_Balance-" + Convert.ToDecimal(Dr["Amount"]) + " where CustID='" + Dr["CustId"].ToString() + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        Dr.Close();

                        Query = "Delete from CustMou_Dtl where MoUNo='" + txtMoUNumber.Text.Trim() + "';";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                        for (int i = 0; i <= dgvMoUCustomerDetails.RowCount - 1; i++)
                        {
                            Query += "Insert CustMou_Dtl (MoUNo, CustID, CustName, Amount, Remaining_Amount, CreatedDate, CreatedBy) Values ";
                            Query += "(@MoUNumber,";
                            Query += "@CustID,";
                            Query += "@CustName,";
                            Query += "@CreditAmount,";
                            Query += "@Remaining_Amount,";
                            Query += "getdate(),";
                            Query += "@Login);";
                            Query += "Update [dbo].[CustTable] Set Sisa_Limit_MoU=Sisa_Limit_MoU+@CreditAmount, MoU_Balance=MoU_Balance+@CreditAmount where CustID=@CustID;";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.Add("@MoUNumber", txtMoUNumber.Text.Trim());
                            Cmd.Parameters.Add("@CustID", (dgvMoUCustomerDetails.Rows[i].Cells["CustID"].Value == null ? "" : dgvMoUCustomerDetails.Rows[i].Cells["CustID"].Value.ToString()));
                            Cmd.Parameters.Add("@CustName", (dgvMoUCustomerDetails.Rows[i].Cells["CustName"].Value == null ? "" : dgvMoUCustomerDetails.Rows[i].Cells["CustName"].Value.ToString()));
                            Cmd.Parameters.Add("@CreditAmount", (dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == null ? 0 : Convert.ToDecimal(dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString())));
                            Cmd.Parameters.Add("@Remaining_Amount", (dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value == null ? 0 : Convert.ToDecimal(dgvMoUCustomerDetails.Rows[i].Cells["CreditAmount"].Value.ToString())));
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);

                            Cmd.ExecuteNonQuery();
                            Query = "";
                        }

                        //Save CustMou_Logs
                        //Query = "Select Deskripsi from TransStatusTable where TransCode='PaymentVoucher' and StatusCode='01'";
                        //Cmd = new SqlCommand(Query, Conn);
                        //string LogsStatusDesc = Cmd.ExecuteScalar().ToString();
                        SaveCustMouLogs(MoUNumber, "-", "Created", "E");

                        MessageBox.Show("Data MoU Number : " + txtMoUNumber.Text + " berhasil diupdate.");
                    }
                    scope.Complete();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
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


        private void SaveCustMouLogs(string InvoiceID, string Deskripsi, string Status, string Action)
        {
            string Query = "INSERT INTO CustMou_Logs VALUES ";
            Query += "(@InvoiceID,@Deskripsi,@Status,@Action,@Login,getdate())";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.Add("@InvoiceID", InvoiceID);
                Cmd.Parameters.Add("@Deskripsi", Deskripsi);
                Cmd.Parameters.Add("@Status", Status);
                Cmd.Parameters.Add("@Action", Action);
                Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            ModeBeforeEdit();
            GetDataHeader();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";
                ModeEdit();
                btnSave.Visible = true;
                btnExit.Visible = false;
                btnEdit.Visible = false;
                btnCancel.Visible = true;
                btnLookUpCustGroup.Enabled = true;                
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnLookUpCustGroup_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "CustID";
            tmpSearch.Order = "CustId Asc";
            tmpSearch.Table = "[dbo].[CustTable]";
            tmpSearch.QuerySearch = "SELECT a.CustID, a.CustName FROM [dbo].[CustTable] a";
            tmpSearch.FilterText = new string[] { "CustID", "CustName" };
            tmpSearch.Select = new string[] { "CustID", "CustName" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                textCustGroupID.Text = ConnectionString.Kodes[0];
                textCustGroupName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }

        }

        private void dgvMoUCustomerDetails_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvMoUCustomerDetails.Columns["CreditAmount"].Index && e.Value != null)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }
        }

        private void dgvMoUCustomerDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvMoUCustomerDetails.Columns[dgvMoUCustomerDetails.CurrentCell.ColumnIndex].Name == "CreditAmount")
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

        private void dgvMoUCustomerDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() == "Text")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvMoUCustomerDetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvMoUCustomerDetails_KeyPress);
            }
        }

        private void textLimitCredit_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textLimitCredit_Leave(object sender, EventArgs e)
        {
            if (textLimitCredit.Text != "")
            {
                double d = double.Parse(textLimitCredit.Text);
                textLimitCredit.Text = d.ToString("N4");
            }
         
        }

        private void btnLookUpBank_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "tblBank_Group_Group";
            tmpSearch.Order = "tblBank_Group_Group Asc";
            tmpSearch.Table = "tblBank_Group";
            tmpSearch.QuerySearch = "SELECT a.tblBank_Group_Group, a.tblBank_Group_Nama FROM tblBank_Group a";
            tmpSearch.FilterText = new string[] { "tblBank_Group_Group", "tblBank_Group_Nama" };
            tmpSearch.Select = new string[] { "tblBank_Group_Group", "tblBank_Group_Nama" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtBankID.Text = ConnectionString.Kodes[0];
                txtBankName.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void BankGuarantee_SelectedIndexChange(object sender, EventArgs e)
        {
            if (Convert.ToString(cmbBankGuarantee.Text) == "No" || Convert.ToString(cmbBankGuarantee.Text) == "-select-")
            {
                txtLCNo.Text = "";
                txtLCType.Text = "";
                txtBankID.Text = "";
                txtBankName.Text = "";
                dtLC.CustomFormat = " ";
                dtLC.Format = DateTimePickerFormat.Custom;
                dtJatuhTempo.CustomFormat = " ";
                dtJatuhTempo.Format = DateTimePickerFormat.Custom;
                dtLC.Text = "";
                dtJatuhTempo.Text = "";

                txtLCNo.Enabled = false;
                txtLCType.Enabled = false;
                dtLC.Enabled = false;
                dtJatuhTempo.Enabled = false;
                btnLookUpBank.Enabled = false;
            }
            else
            {
                txtLCNo.Enabled = true;
                txtLCType.Enabled = true;
                dtLC.Enabled = true;
                dtJatuhTempo.Enabled = true;
                dtLC.CustomFormat = "dd/MM/yyyy";
                dtLC.Format = DateTimePickerFormat.Custom;               
                dtJatuhTempo.CustomFormat = "dd/MM/yyyy";
                dtJatuhTempo.Format = DateTimePickerFormat.Custom;                
                btnLookUpBank.Enabled = true;        
            }
        }
        //tia edit
        ////klik kanan
        PopUp.CustomerID.Customer Cust = null;

        Sales.SalesAgreement.SAHeader ParentToSA;
        Sales.SalesOrder.SOHeader ParentToSO;
        Sales.SalesQuotation.SQHeader2 ParentTOSQ;

        public void ParentRefreshGrid(Sales.SalesAgreement.SAHeader sa)
        {
            ParentToSA = sa;
        }

        public void ParentRefreshGrid2(Sales.SalesOrder.SOHeader so)
        {
            ParentToSO = so;
        }

        public void ParentRefreshGrid3(Sales.SalesQuotation.SQHeader2 sq)
        {
            ParentTOSQ = sq;
        }
        private void dgvMoUCustomerDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (Cust == null || Cust.Text == "")
                {
                    if (dgvMoUCustomerDetails.Columns[e.ColumnIndex].Name.ToString() == "CustID" || dgvMoUCustomerDetails.Columns[e.ColumnIndex].Name.ToString() == "CustName")
                    {
                        Cust = new PopUp.CustomerID.Customer();
                        Cust.GetData(dgvMoUCustomerDetails.Rows[e.RowIndex].Cells["CustID"].Value.ToString());
                        Cust.Show();
                    }
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.GetData(dgvMoUCustomerDetails.Rows[e.RowIndex].Cells["CustID"].Value.ToString());
                    Cust.Show();
                    Cust.Focus();
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

        private void textCustGroupID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    textCustGroupID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(textCustGroupID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        //private void textCustGroupName_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        if (Cust == null || Cust.Text == "")
        //        {
        //            textCustGroupName.Enabled = true;
        //            Cust = new PopUp.CustomerID.Customer();
        //            Cust.GetData(textCustGroupID.Text);
        //            Cust.Show();
        //        }
        //        else if (CheckOpened(Cust.Name))
        //        {
        //            Cust.WindowState = FormWindowState.Normal;
        //            Cust.Show();
        //            Cust.Focus();
        //        }
        //    }
        //}
        //end       

    }
}
