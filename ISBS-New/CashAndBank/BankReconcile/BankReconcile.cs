using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.CashAndBank.BankReconcile
{
    partial class BankReconcile : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private System.Data.DataTable Dt;
        private DataSet Ds;

        string Query, crit = null;
        int CountBankSystem = 0;
        int CountBankStatement = 0;
        string ErrorMessage = "";

        List<bool> ListCheckedBankSystem = new List<bool>();
        List<bool> ListCheckedBankStatement = new List<bool>();
        List<string> ListNoCheckBankSystem = new List<string>();
        List<string> ListNoCheckBankStatement = new List<string>();

        public BankReconcile()
        {
            InitializeComponent();           
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void BankReconcile_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            addCmbBank();
            addCmbType();
            GetHeaderBankSystem();
            GetHeaderBankStatement();
            GetData();
        }

        private void BankReconcile_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void GetHeaderBankSystem()
        {
            if (dgvBankSystem.RowCount - 1 <= 0)
            {
                if (dgvBankSystem.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();                   
                    chk.HeaderText = "Check";
                    chk.Name = "chk";
                    chk.TrueValue = true;
                    chk.FalseValue = false;
                    dgvBankSystem.Columns.Add(chk);
                }

                dgvBankSystem.ColumnCount = 10;
                dgvBankSystem.Columns[1].Name = "No";
                dgvBankSystem.Columns[2].Name = "BankSystemNo";
                dgvBankSystem.Columns[3].Name = "TransDate";
                dgvBankSystem.Columns[4].Name = "BankCode";
                dgvBankSystem.Columns[5].Name = "AccountNo";
                dgvBankSystem.Columns[6].Name = "AccountName";
                dgvBankSystem.Columns[7].Name = "AccountType";
                dgvBankSystem.Columns[8].Name = "AccountAmount";
                dgvBankSystem.Columns[9].Name = "NoCheck";

                dgvBankSystem.ReadOnly = false;

                for (int i = 0; i < 9; i++)
                {
                    if (i == 0)
                    {
                        dgvBankSystem.Columns[i].ReadOnly = false;    
                    }
                    else
                    {
                      dgvBankSystem.Columns[i].ReadOnly = true;
                    }                                   
                }

                dgvBankSystem.AutoResizeColumns();
            }
        }

        private void GetHeaderBankStatement()
        {
            if (dgvBankStatement.RowCount - 1 <= 0)
            {
                if (dgvBankStatement.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = chk = new DataGridViewCheckBoxColumn();
                    dgvBankStatement.Columns.Add(chk);
                    chk.HeaderText = "Check";
                    chk.Name = "chk";
                }

                dgvBankStatement.ColumnCount = 11;
                dgvBankStatement.Columns[1].Name = "No";
                dgvBankStatement.Columns[2].Name = "TransDate";
                dgvBankStatement.Columns[3].Name = "BankCode";
                dgvBankStatement.Columns[4].Name = "AccountNo";
                dgvBankStatement.Columns[5].Name = "AccountName";
                dgvBankStatement.Columns[6].Name = "AccountType";
                dgvBankStatement.Columns[7].Name = "AccountAmount";
                dgvBankStatement.Columns[8].Name = "Desc";
                dgvBankStatement.Columns[9].Name = "NoCheck";
                dgvBankStatement.Columns[10].Name = "Id";

                dgvBankStatement.ReadOnly = false;

                dgvBankStatement.Columns["Id"].Visible = false;

                for (int i = 0; i < 10; i++)
                {
                    if (i == 0)
                    {
                        dgvBankStatement.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvBankStatement.Columns[i].ReadOnly = true;
                    }
                }

                dgvBankStatement.AutoResizeColumns();
            }
        }
       
        private void GetData()
        {
            Conn = ConnectionString.GetConnection();

            //Bank System
            if(crit == null)
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB WHERE 1=1 ";           
            }
            else if (crit == "All")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE (TB.BankSystemNo LIKE '%" + txtSearch.Text + "%' OR TB.BankCode LIKE '%" + txtSearch.Text + "%' OR TB.AccountNo LIKE '%" + txtSearch.Text + "%' OR TB.AccountName LIKE '%" + txtSearch.Text + "%' OR TB.AccountAmount LIKE '%" + txtSearch.Text + "%')";
            }
            else if (crit == "BankSystemNo")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE TB.BankSystemNo LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "BankCode")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE TB.BankCode LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "AccountNo")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE TB.AccountNo LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "AccountName")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.AccountNo = BS.tblBank_No_Rek) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE TB.AccountName LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "AccountAmount")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE TB.AccountAmount LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "Desc")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE 1=1 ";
            }
            else if (crit == "TransDate")
            {
                Query = "SELECT * FROM (SELECT BS.VoucherNo AS BankSystemNo, CONVERT(VARCHAR(10), BS.VoucherDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.AccountNo) AS BankCode, ";
                Query += "BS.AccountNo, BS.AccountName, BS.Type, BS.AccountAmount FROM BankSystem BS WHERE BS.ImportBankStatementDtl_Id IS NULL) AS TB ";
                Query += "WHERE (CONVERT(VARCHAR(10),TB.TransDate,103) >= '" + dtFrom.Value.Date.ToString("dd/MM/yyyy") + "' AND CONVERT(VARCHAR(10), TB.TransDate,103) <= '" + dtTo.Value.Date.ToString("dd/MM/yyyy") + "')";
            }

            if (cmbBankGroup.SelectedIndex > 0)
            {
                Query += " AND TB.BankCode LIKE '%" + cmbBankGroup.SelectedItem.ToString() + "%'"; 
            }
           
            if (cmbType.SelectedIndex > 0)
            {
                Query += " AND TB.Type LIKE '%" + cmbType.SelectedItem.ToString() + "%'";
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 1;           
            dgvBankSystem.Rows.Clear();
            while (Dr.Read())
            {
                dgvBankSystem.Rows.Add(false, i++, Dr["BankSystemNo"], Dr["TransDate"], Dr["BankCode"], Dr["AccountNo"], Dr["AccountName"], Dr["Type"], Convert.ToString(Convert.ToDouble(Dr["AccountAmount"]).ToString("N4")));
           }
            Dr.Close();

            dgvBankSystem.AutoResizeColumns();


            //Bank Statement
            if (crit == null)
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB WHERE 1=1 ";
            }
            else if (crit == "All")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB ";
                Query += "WHERE (TB.BankCode LIKE '%" + txtSearch.Text + "%' OR TB.AccountNo LIKE '%" + txtSearch.Text + "%' OR TB.AccountName LIKE '%" + txtSearch.Text + "%' OR TB.AccountAmount LIKE '%" + txtSearch.Text + "%')";
            }
            else if (crit == "BankSystemNo")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB WHERE 1=1 ";
            }
            else if (crit == "BankCode")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB ";
                Query += "WHERE TB.BankCode LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "AccountNo")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB ";
                Query += "WHERE TB.AccountNo LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "AccountName")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB ";
                Query += "WHERE TB.AccountName LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "AccountAmount")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB ";
                Query += "WHERE TB.AccountAmount LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "Desc")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB ";
                Query += "WHERE TB.[Desc] LIKE '%" + txtSearch.Text + "%'";
            }
            else if (crit == "TransDate")
            {
                Query = "SELECT * FROM (SELECT CONVERT(VARCHAR(10), BS.TransDate, 103) AS TransDate, (SELECT BT.tblBank_Group FROM tblBank BT WHERE BT.tblBank_No_Rek = BS.Account_No) AS BankCode, ";
                Query += "BS.Account_No AS AccountNo, BS.Account_Name AS AccountName, BS.Type, BS.Amount AS AccountAmount, BS.Description AS [Desc], BS.Id FROM ImportBankStatementDtl BS WHERE VoucherNo IS NULL) AS TB ";
                Query += "WHERE (CONVERT(VARCHAR(10),TB.TransDate,103) >= '" + dtFrom.Value.Date.ToString("dd/MM/yyyy") + "' AND CONVERT(VARCHAR(10), TB.TransDate,103) <= '" + dtTo.Value.Date.ToString("dd/MM/yyyy") + "')";
            }

            if (cmbBankGroup.SelectedIndex > 0)
            {
                Query += " AND TB.BankCode LIKE '%" + cmbBankGroup.SelectedItem.ToString() + "%'";
            }

            if (cmbType.SelectedIndex > 0)
            {
                Query += " AND TB.Type LIKE '%" + cmbType.SelectedItem.ToString() + "%'";
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int j = 1;
            dgvBankStatement.Rows.Clear();
            while (Dr.Read())
            {
                dgvBankStatement.Rows.Add(false, j++, Dr["TransDate"], Dr["BankCode"], Dr["AccountNo"], Dr["AccountName"], Dr["Type"], Convert.ToString(Convert.ToDouble(Dr["AccountAmount"]).ToString("N4")), Dr["Desc"], "", Dr["Id"]);
            }
            Dr.Close();

            dgvBankStatement.AutoResizeColumns();

            Conn.Close();        
        }

        private void addCmbCrit()
        {
            try
            {
                cmbCriteria.Items.Add("All");
                Conn = ConnectionString.GetConnection();
                Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'BankReconcile'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    cmbCriteria.Items.Add(Dr[0]);
                }
                Conn.Close();

                cmbCriteria.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void addCmbBank()
        {
            try
            {
                cmbBankGroup.Items.Add("All");
                Conn = ConnectionString.GetConnection();
                Query = "SELECT tblBank_Group_Group FROM tblBank_Group";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    cmbBankGroup.Items.Add(Dr[0]);
                }               
                Conn.Close();

                cmbBankGroup.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void addCmbType()
        {
            try
            {
                cmbType.Items.Add("All");
                cmbType.Items.Add("CR");
                cmbType.Items.Add("DB");
                //Conn = ConnectionString.GetConnection();
                //Query = "SELECT Distinct Type FROM BankSystem";

                //Cmd = new SqlCommand(Query, Conn);
                //Dr = Cmd.ExecuteReader();

                //while (Dr.Read())
                //{
                //    cmbType.Items.Add(Dr[0]);
                //}
                //Conn.Close();

                cmbType.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            GetData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }

            GetData();
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }


            if (cmbCriteria.Text.Contains("Date"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void dgvBankSystem_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            if (dgvBankSystem.Columns[e.ColumnIndex].Name.ToString() == "chk")
            {
                Boolean Check = Convert.ToBoolean(dgvBankSystem.Rows[e.RowIndex].Cells["chk"].EditedFormattedValue);
                if (Check == true)
                {
                    GridValidation("BankSystem");

                    if (ErrorMessage == "")
                    {
                        dgvBankSystem.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;

                        for (int i = 0; i < dgvBankStatement.RowCount; i++)
                        {
                            Boolean CheckList = Convert.ToBoolean(dgvBankStatement.Rows[i].Cells["chk"].Value);
                            string NoCheck = Convert.ToString(dgvBankStatement.Rows[i].Cells["NoCheck"].Value);
                            string NoBankStatement = Convert.ToString(dgvBankStatement.Rows[i].Cells["No"].Value);
                            string BankCodeStatement = Convert.ToString(dgvBankStatement.Rows[i].Cells["BankCode"].Value);
                            string AccountAmountStatement = Convert.ToString(dgvBankStatement.Rows[i].Cells["AccountAmount"].Value);
                            string TransDateStatement = Convert.ToString(dgvBankStatement.Rows[i].Cells["TransDate"].Value);
                            string TypeStatement = Convert.ToString(dgvBankStatement.Rows[i].Cells["AccountType"].Value);

                            string NoBankSystem = Convert.ToString(dgvBankSystem.Rows[e.RowIndex].Cells["No"].Value);
                            string BankCodeSystem = Convert.ToString(dgvBankSystem.Rows[e.RowIndex].Cells["BankCode"].Value);
                            string AccountAmountSystem = Convert.ToString(dgvBankSystem.Rows[e.RowIndex].Cells["AccountAmount"].Value);
                            string TransDateSystem = Convert.ToString(dgvBankSystem.Rows[e.RowIndex].Cells["TransDate"].Value);
                            string TypeSystem = Convert.ToString(dgvBankSystem.Rows[e.RowIndex].Cells["AccountType"].Value);

                            if (CheckList == true && NoCheck == "")
                            {
                                //Validasi Data
                                if (BankCodeSystem == BankCodeStatement && AccountAmountSystem == AccountAmountStatement && TransDateSystem == TransDateStatement && TypeSystem == TypeStatement)
                                {
                                    dgvBankSystem.Rows[e.RowIndex].Cells["NoCheck"].Value = NoBankStatement;
                                    dgvBankStatement.Rows[i].Cells["NoCheck"].Value = NoBankSystem;

                                    dgvBankStatement.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                                    dgvBankSystem.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                                }
                                else
                                {
                                    dgvBankSystem.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                                    dgvBankSystem.CancelEdit();

                                    ErrorMessage = "Data antara Bank System dengan Bank Statement tidak sesuai";
                                    MessageBox.Show(ErrorMessage);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        dgvBankSystem.CancelEdit();

                        dgvBankSystem.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;                    
                    }
                }
                else
                {
                    dgvBankSystem.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;

                    string NoCheckSystem = Convert.ToString(dgvBankSystem.Rows[e.RowIndex].Cells["NoCheck"].Value);
                    string NoSystem = Convert.ToString(dgvBankSystem.Rows[e.RowIndex].Cells["No"].Value);

                    //Reset NoCheck If NoCheck is not null
                    if (NoCheckSystem != "")
                    {
                        for (int i = 0; i < dgvBankStatement.RowCount; i++)
                        {
                            if (Convert.ToString(dgvBankStatement.Rows[i].Cells["No"].Value) == NoCheckSystem)
                            {
                                dgvBankStatement.Rows[i].Cells["NoCheck"].Value = "";
                                dgvBankSystem.Rows[e.RowIndex].Cells["NoCheck"].Value = "";

                                dgvBankStatement.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                dgvBankSystem.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                                break;
                            }
                        }
                    }
                    else
                    {
                        dgvBankSystem.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
        }

        private void GridValidation(string GridName)
        {
            ErrorMessage = "";
            if (cmbBankGroup.SelectedIndex == 0)
            {
                ErrorMessage = "Silahkan pilih Bank Code terlebih dahulu";
                MessageBox.Show(ErrorMessage);
                return;
            }

            CountBankSystem = 0;
            for (int i = 0; i < dgvBankSystem.RowCount; i++)
            {
                Boolean CheckList = Convert.ToBoolean(dgvBankSystem.Rows[i].Cells["chk"].EditedFormattedValue);
                if (CheckList == true)
                {
                    CountBankSystem = CountBankSystem + 1;
                }
            }

            CountBankStatement = 0;
            for (int i = 0; i < dgvBankStatement.RowCount; i++)
            {
                Boolean CheckList = Convert.ToBoolean(dgvBankStatement.Rows[i].Cells["chk"].EditedFormattedValue);

                if (CheckList == true)
                {
                    CountBankStatement = CountBankStatement + 1;
                }
            }

            if (GridName == "BankSystem")
            {
                if (CountBankSystem - CountBankStatement >= 2)
                {
                    ErrorMessage = "Silahkan pilih Bank Statement terlebih dahulu";
                    MessageBox.Show(ErrorMessage);
                    return;
                }
            }
            else
            {
                if (CountBankStatement - CountBankSystem >= 2)
                {
                    ErrorMessage = "Silahkan pilih Bank System terlebih dahulu";
                    MessageBox.Show(ErrorMessage);
                    return;
                }
            }          
        }

        private void cmbBankGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetHeaderBankSystem();
            GetHeaderBankStatement();
            GetData();
        }

        private void dgvBankStatement_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvBankStatement.Columns[e.ColumnIndex].Name.ToString() == "chk")
            {
                Boolean Check = Convert.ToBoolean(dgvBankStatement.Rows[e.RowIndex].Cells["chk"].EditedFormattedValue);
                if (Check == true)
                {
                    GridValidation("BankStatement");

                    if (ErrorMessage == "")
                    {
                        dgvBankStatement.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;

                        for (int i = 0; i < dgvBankSystem.RowCount; i++)
                        {
                            Boolean CheckList = Convert.ToBoolean(dgvBankSystem.Rows[i].Cells["chk"].Value);
                            string NoCheck = Convert.ToString(dgvBankSystem.Rows[i].Cells["NoCheck"].Value);
                            string NoBankSystem = Convert.ToString(dgvBankSystem.Rows[i].Cells["No"].Value);
                            string BankCodeSystem = Convert.ToString(dgvBankSystem.Rows[i].Cells["BankCode"].Value);
                            string AccountAmountSystem = Convert.ToString(dgvBankSystem.Rows[i].Cells["AccountAmount"].Value);
                            string TransDateSystem = Convert.ToString(dgvBankSystem.Rows[i].Cells["TransDate"].Value);
                            string TypeSystem = Convert.ToString(dgvBankSystem.Rows[i].Cells["AccountType"].Value);

                            string NoBankStatement = Convert.ToString(dgvBankStatement.Rows[e.RowIndex].Cells["No"].Value);
                            string BankCodeStatement = Convert.ToString(dgvBankStatement.Rows[e.RowIndex].Cells["BankCode"].Value);
                            string AccountAmountStatement = Convert.ToString(dgvBankStatement.Rows[e.RowIndex].Cells["AccountAmount"].Value);
                            string TransDateStatement = Convert.ToString(dgvBankStatement.Rows[e.RowIndex].Cells["TransDate"].Value);
                            string TypeStatement = Convert.ToString(dgvBankStatement.Rows[i].Cells["AccountType"].Value);

                            if (CheckList == true && NoCheck == "")
                            {
                                //Validasi Data
                                if (BankCodeSystem == BankCodeStatement && AccountAmountSystem == AccountAmountStatement && TransDateSystem == TransDateStatement && TypeSystem == TypeStatement)
                                {
                                    dgvBankStatement.Rows[e.RowIndex].Cells["NoCheck"].Value = NoBankSystem;
                                    dgvBankSystem.Rows[i].Cells["NoCheck"].Value = NoBankStatement;


                                    dgvBankSystem.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                                    dgvBankStatement.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Green;
                                }
                                else
                                {
                                    dgvBankStatement.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                                    dgvBankStatement.CancelEdit();

                                    ErrorMessage = "Data antara Bank System dengan Bank Statement tidak sesuai";
                                    MessageBox.Show(ErrorMessage);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        dgvBankStatement.CancelEdit();
                        dgvBankStatement.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }   
                }
                else
                {
                    dgvBankStatement.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;

                    string NoCheckStatement = Convert.ToString(dgvBankStatement.Rows[e.RowIndex].Cells["NoCheck"].Value);
                    string NoStatement = Convert.ToString(dgvBankStatement.Rows[e.RowIndex].Cells["No"].Value);


                    //Reset NoCheck If NoCheck is not null
                    if (NoCheckStatement != "")
                    {
                        for (int i = 0; i < dgvBankSystem.RowCount; i++)
                        {
                            if (Convert.ToString(dgvBankSystem.Rows[i].Cells["No"].Value) == NoCheckStatement)
                            {
                                dgvBankSystem.Rows[i].Cells["NoCheck"].Value = "";
                                dgvBankStatement.Rows[e.RowIndex].Cells["NoCheck"].Value = "";

                                dgvBankSystem.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                dgvBankStatement.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;

                                break;
                            }
                        }
                    }
                    else
                    {
                        dgvBankStatement.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();


                //Update Bank System
                int a = 0;
                for (int i = 0; i < dgvBankSystem.RowCount; i++)
                {
                    Boolean Check = Convert.ToBoolean(dgvBankSystem.Rows[i].Cells["chk"].Value);
                    string NoCheck = Convert.ToString(dgvBankSystem.Rows[i].Cells["NoCheck"].Value);
                    string No = Convert.ToString(dgvBankSystem.Rows[i].Cells["No"].Value);
                    string VoucherNo = Convert.ToString(dgvBankSystem.Rows[i].Cells["BankSystemNo"].Value);
                    if (Check == true)
                    {
                        if (NoCheck != "")
                        {
                            string IdBankStatement = "";
                            for (int j = 0; j < dgvBankStatement.RowCount; j++)
                            {
                                if (Convert.ToString(dgvBankStatement.Rows[j].Cells["No"].Value) == NoCheck)
                                {
                                    IdBankStatement = Convert.ToString(dgvBankStatement.Rows[j].Cells["Id"].Value);
                                    break;
                                }
                            }

                            Query = "UPDATE BankSystem SET ImportBankStatementDtl_Id = @IdBankStatement, UpdatedBy = @Login, UpdatedDate = GETDATE() WHERE VoucherNo = @VoucherNo";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.Parameters.Add("@IdBankStatement", IdBankStatement);
                            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
                            Cmd.Parameters.Add("@VoucherNo", VoucherNo);
                            Cmd.ExecuteNonQuery();
                            a++;
                        }
                        else
                        {
                            MessageBox.Show("Data Bank System pada No " + No + " belum ter-reconcile");
                            return;
                        }
                    }
                }

                //Update Bank Statement
                int b = 0;
                for (int i = 0; i < dgvBankStatement.RowCount; i++)
                {
                    Boolean Check = Convert.ToBoolean(dgvBankStatement.Rows[i].Cells["chk"].Value);
                    string NoCheck = Convert.ToString(dgvBankStatement.Rows[i].Cells["NoCheck"].Value);
                    string No = Convert.ToString(dgvBankStatement.Rows[i].Cells["No"].Value);
                    string Id = Convert.ToString(dgvBankStatement.Rows[i].Cells["Id"].Value);
                    if (Check == true)
                    {
                        if (NoCheck != "")
                        {
                            string VoucherNo = "";
                            for (int j = 0; j < dgvBankSystem.RowCount; j++)
                            {                               
                                if (Convert.ToString(dgvBankSystem.Rows[j].Cells["No"].Value) == NoCheck)
                                {
                                    VoucherNo = Convert.ToString(dgvBankSystem.Rows[j].Cells["BankSystemNo"].Value);
                                    break;
                                }
                            }

                            Query = "UPDATE ImportBankStatementDtl SET VoucherNo = @VoucherNo WHERE Id = @Id";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.Parameters.Add("@Id", Id);
                            Cmd.Parameters.Add("@VoucherNo", VoucherNo);
                            Cmd.ExecuteNonQuery();
                            
                            b++;
                        }
                        else
                        {
                            MessageBox.Show("Data Bank Statement pada No " + No + " belum ter-reconcile");
                            return;
                        }
                    }
                }

                if (a == 0 && b == 0)
                {
                    MessageBox.Show("Silahkan pilih data yang akan di reconcile");
                    return;
                }
                else
                {
                    Trans.Commit();
                    MessageBox.Show("Data berhasil disimpan");
                    GetData();
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
            }
        }

     }
}
