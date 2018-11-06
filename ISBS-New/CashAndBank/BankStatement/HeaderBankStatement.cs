using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;

namespace ISBS_New.CashAndBank.BankStatement
{
    public partial class HeaderBankStatement : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private System.Data.DataTable Dt;
        private DataSet Ds;

        string Mode, Query, crit = null;

        int Index;
        private string BS_Id = "";
        private string BankGroup = "";
        private string ErrorMessage = "";
        DateTime CheckDate;

        CashAndBank.BankStatement.InquiryBankStatement Parent;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public void SetParent(CashAndBank.BankStatement.InquiryBankStatement F)
        {
            Parent = F;
        }

        public HeaderBankStatement()
        {
            InitializeComponent();
        }

        private void HeaderBankStatement_Shown(object sender, EventArgs e)
        {
            this.Location = new System.Drawing.Point(170, 63);
        }

        private void HeaderBankStatement_Load(object sender, EventArgs e)
        {
            setHeaderDgvBankStatement();

            if (Mode == "New")
            {
                ModeNew();
            }
            //else if (Mode == "Edit")
            //{
            //    ModeEdit();
            //}
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ModeNew()
        {
            BS_Id = "";
        }

        public void ModeBeforeEdit()
        {
            GetDataHeader();
            btnAttachment.Enabled = false;
            btnLookUpBank.Enabled = false;
            btnImport.Enabled = false;
        }

        public void SetMode(string tmpMode, string tmpBS_Id, string tmpBankCode)
        {
            Mode = tmpMode;
            BS_Id = tmpBS_Id;
            txtBankCode.Text = tmpBankCode;
        }

        private void GetDataHeader()
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Query = "SELECT [Account_No],[Account_Name],[Bank_Code] FROM [dbo].[ImportBankStatementH] WHERE BS_Id = '" + BS_Id + "' ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtAccountNo.Text = Convert.ToString(Dr["Account_No"]);
                    txtAccountName.Text = Convert.ToString(Dr["Account_Name"]);
                    txtBankCode.Text = Convert.ToString(Dr["Bank_Code"]);
                }
                Dr.Close();

                dgvBankStatement.Rows.Clear();

                Query = "SELECT [BS_Id],[SeqNo],[Account_No],[Account_Name],[Bank_Code], CONVERT (varchar(10), TransDate, 103) AS TransDate ,[Description],[Amount],[Type] FROM [dbo].[ImportBankStatementDtl] WHERE BS_Id = '" + BS_Id + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dgvBankStatement.Rows.Add(true, Dr["SeqNo"], Dr["TransDate"], Dr["Type"], Dr["Description"], Convert.ToString(Convert.ToDouble(Dr["Amount"]).ToString("N4")), Dr["SeqNo"], "Y");
                }
                Dr.Close();

                dgvBankStatement.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conn.Close();
            }
        }

        private void btnLookUpBank_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "BankCode";
            tmpSearch.Order = "BankCode Asc";
            tmpSearch.Table = "[dbo].[BankTable]";
            tmpSearch.QuerySearch = "SELECT a.BankGroupID, a.BankId AS BankCode, a.AccountNo, a.AccountName FROM [dbo].[BankTable] a";
            tmpSearch.FilterText = new string[] { "BankGroupID", "BankCode", "AccountNo", "AccountName" };
            tmpSearch.Select = new string[] { "BankGroupID", "BankCode", "AccountNo", "AccountName" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtBankCode.Text = ConnectionString.Kodes[1];
                txtAccountNo.Text = ConnectionString.Kodes[2];
                txtAccountName.Text = ConnectionString.Kodes[3];
                BankGroup = ConnectionString.Kodes[0];
                ConnectionString.Kodes = null;

                btnAttachment.Enabled = true;

                dgvBankStatement.Rows.Clear();
                txtAttachment.Text = "";
            }
        }

        private void btnAttachment_Click(object sender, EventArgs e)         
        {
            ErrorMessage = "";

            OpenFileDialog choofdlog = new OpenFileDialog();

            if (BankGroup.ToUpper() == "BNI" || BankGroup.ToUpper() == "MANDIRI")
            {
                choofdlog.Filter = "Excel Files|*.xls";
            }
            else
            {
                choofdlog.Filter = "CSV Files|*.csv";
            }     
            
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                dgvBankStatement.Rows.Clear();

                txtAttachment.Text = choofdlog.FileName;

                if (BankGroup.ToUpper() == "BCA")
                {
                    AttachmentBankBCA();
                }
                else if (BankGroup.ToUpper() == "NISP")
                {
                    AttachmentBankNISP();
                }
                else if (BankGroup.ToUpper() == "MANDIRI")
                {
                    AttachmentBankMandiri();
                }
                else if (BankGroup.ToUpper() == "BNI")
                {
                    AttachmentBankBNI();
                }
                else
                {
                    MessageBox.Show("Format templete bank belum tersedia, silahkan kontak team IT");
                    return;
                }

                if (ErrorMessage == "")
                {
                    if (dgvBankStatement.RowCount == 0)
                    {
                        MessageBox.Show("Data tidak ditemukan, silahkan cek kembali dokumen Anda");
                        return;
                    }
                    else
                    {
                        dgvBankStatement.Sort(dgvBankStatement.Columns["Existing"], ListSortDirection.Descending);
                        SortNoDataGridBankStatement();
                    }
                }   
            }
        }

        private void AttachmentBankBCA()
        {
            try
            {
                int i = 1;
                int RowNo = 0;

                var parser = new TextFieldParser(new StringReader(File.ReadAllText(txtAttachment.Text)))
                {
                    HasFieldsEnclosedInQuotes = true,
                    Delimiters = new string[] { ",", ";" },
                    TrimWhiteSpace = true
                };

                var csvSplitList = new List<string>();

                while (!parser.EndOfData)
                {
                    csvSplitList.Add(String.Join(";", parser.ReadFields()).Replace("\"", ""));
                    if (i > 7)
                    {
                        var Values = csvSplitList[i - 1].Split(';');

                        string Existing = "";

                        //Date Valdiation
                        string TransDate = Values[0].Replace("'", "").Trim();

                        if (TransDate == "")
                        {
                            ErrorMessage = "TransDate harus diisi pada baris ke ";
                            MessageBox.Show(ErrorMessage + i);
                            dgvBankStatement.Rows.Clear();
                            return;
                        }

                        if (TransDate.Length == 5 && TransDate.Contains("/"))
                        {
                            try
                            {
                                int Month = Convert.ToInt32(TransDate.Split('/')[1]);
                                int Year = DateTime.Now.Year;
                                if (DateTime.Now.Month < Month)
                                {
                                    Year = DateTime.Now.Year - 1;
                                }

                                TransDate = Convert.ToString(TransDate.Split('/')[0]) + "/" + Convert.ToString(TransDate.Split('/')[1]) + "/" + Convert.ToString(Year);

                                if (DateTime.TryParseExact(TransDate, "dd/MM/yyyy", new System.Globalization.CultureInfo("id-ID"), DateTimeStyles.None, out CheckDate) == false)
                                {
                                    ErrorMessage = "Format Date tidak valid pada baris ke ";
                                    MessageBox.Show(ErrorMessage + i);
                                    dgvBankStatement.Rows.Clear();
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }
                        else
                        {
                            if (TransDate.ToUpper().Trim() == "PEND")
                            {
                                continue;
                            }
                            else
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }

                        string Description = Values[1];

                        //Amount Validation

                        var AmountType = Values[3].Split(' ');

                        string Amount = AmountType[0];

                        if (Amount != "" && Convert.ToDecimal(Amount) != 0)
                        {
                            double Num;
                            bool isNum = double.TryParse(Amount, out Num);

                            if (!isNum)
                            {
                                ErrorMessage = "Format Amount tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                            else
                            {
                                Amount = Convert.ToDouble(Amount).ToString("N4");
                            }
                        }
                        else
                        {
                            ErrorMessage = "Amount harus lebih besar dari 0 pada baris ke ";
                            MessageBox.Show(ErrorMessage + i);
                            dgvBankStatement.Rows.Clear();
                            return;
                        }


                        //Type Validation
                        string Type = AmountType[1];

                        if (Type == "")
                        {
                            ErrorMessage = "Type harus diisi pada baris ke ";
                            MessageBox.Show(ErrorMessage + i);
                            dgvBankStatement.Rows.Clear();
                            return;
                        }

                        if (Type.ToUpper().Trim() != "CR")
                        {
                            if (Type.ToUpper().Trim() != "DB")
                            {
                                ErrorMessage = "Data Type tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }

                        //Check Data Existing
                        Conn = ConnectionString.GetConnection();
                        Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementDtl ";
                        Query += "WHERE Account_No = '" + txtAccountNo.Text + "' AND Account_Name = '" + txtAccountName.Text + "' ";
                        Query += "AND Bank_Code = '" + txtBankCode.Text + "' AND CONVERT (varchar(10), TransDate, 103) = CONVERT (varchar(10), '" + TransDate + "', 103) ";
                        Query += "AND CONVERT(VARCHAR(1000),Description) = '" + Description + "' AND CONVERT(DECIMAL(28,4), Amount) = CONVERT(DECIMAL(28,4), " + Convert.ToDecimal(Amount) + ") AND UPPER(Type) = UPPER('" + Type + "') ";
                        Cmd = new SqlCommand(Query, Conn);
                        int result = Convert.ToInt32(Cmd.ExecuteScalar());
                        Conn.Close();

                        bool Check = true;
                        if (result > 0)
                        {
                            Existing = "Y";
                            Check = false;
                        }
                        else
                        {
                            Existing = "N";
                            Check = true;
                        }

                        dgvBankStatement.Rows.Add(Check, dgvBankStatement.RowCount + 1, TransDate, Type, Description, Amount, "", Existing);

                        if (Check == true)
                        {
                            dgvBankStatement.Rows[RowNo].Cells[0].ReadOnly = true;
                        }
                        else
                        {
                            dgvBankStatement.Rows[RowNo].DefaultCellStyle.BackColor = Color.Red;
                        }

                        RowNo++;
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Object reference not set to an instance of an object"))
                {
                    ErrorMessage = "Templete tidak sesuai";
                    MessageBox.Show(ErrorMessage);
                }
                else if (e.Message.Contains("is being used by another process"))
                {
                    ErrorMessage = "File sedang dibuka, silahkan tutup file terlebih dahulu";
                    MessageBox.Show(ErrorMessage);
                }

                return;
            }
        }

        private void AttachmentBankNISP()
        {
            try
            {
                int i = 1;
                int RowNo = 0;

                var parser = new TextFieldParser(new StringReader(File.ReadAllText(txtAttachment.Text)))
                {
                    HasFieldsEnclosedInQuotes = true,
                    Delimiters = new string[] { ",", ";" },
                    TrimWhiteSpace = true
                };

                var csvSplitList = new List<string>();

                while (!parser.EndOfData)
                {
                    csvSplitList.Add(String.Join(";", parser.ReadFields()));
                    if (i > 10)
                    {
                        var Values = csvSplitList[i - 1].Split(';');

                        string Existing = "";

                        //Date Valdiation
                        string TransDate = Values[0].Replace("'", "").Trim();

                        if (TransDate == "")
                        {
                            ErrorMessage = "TransDate harus diisi pada baris ke ";
                            MessageBox.Show(ErrorMessage + i);
                            dgvBankStatement.Rows.Clear();
                            return;
                        }

                        if (TransDate.Length == 8 && TransDate.Contains("/"))
                        {
                            try
                            {
                                DateTime temp = DateTime.ParseExact(TransDate, "dd/MM/yy", CultureInfo.InvariantCulture);
                                TransDate = DateTime.ParseExact(temp.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

                                if (DateTime.TryParseExact(TransDate, "dd/MM/yyyy", new System.Globalization.CultureInfo("id-ID"), DateTimeStyles.None, out CheckDate) == false)
                                {
                                    ErrorMessage = "Format Date tidak valid pada baris ke ";
                                    MessageBox.Show(ErrorMessage + i);
                                    dgvBankStatement.Rows.Clear();
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }
                        else
                        {
                            if (TransDate.ToUpper().Trim() == "PEND")
                            {
                                continue;
                            }
                            else
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }

                        string Description = Values[4];

                        //Amount & Type Validation                    
                        string Amount = Values[5];
                        string Type = "";

                        if (Amount != "" && Convert.ToDecimal(Amount) != 0)
                        {
                            double Num;
                            bool isNum = double.TryParse(Amount, out Num);

                            if (!isNum)
                            {
                                ErrorMessage = "Format Amount tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                            else
                            {
                                Amount = Convert.ToDouble(Amount).ToString("N4");
                                Type = "DB";
                            }
                        }
                        else
                        {
                            Amount = Values[6];

                            if (Amount != "" && Convert.ToDecimal(Amount) != 0)
                            {
                                double Num;
                                bool isNum = double.TryParse(Amount, out Num);

                                if (!isNum)
                                {
                                    ErrorMessage = "Format Amount tidak valid pada baris ke ";
                                    MessageBox.Show(ErrorMessage + i);
                                    dgvBankStatement.Rows.Clear();
                                    return;
                                }
                                else
                                {
                                    Amount = Convert.ToDouble(Amount).ToString("N4");
                                    Type = "CR";
                                }
                            }
                            else
                            {
                                ErrorMessage = "Amount harus lebih besar dari 0 pada baris ke ";
                                MessageBox.Show(ErrorMessage + i);
                                dgvBankStatement.Rows.Clear();
                                return;
                            }

                        }

                        //Check Data Existing
                        Conn = ConnectionString.GetConnection();
                        Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementDtl ";
                        Query += "WHERE Account_No = '" + txtAccountNo.Text + "' AND Account_Name = '" + txtAccountName.Text + "' ";
                        Query += "AND Bank_Code = '" + txtBankCode.Text + "' AND CONVERT (varchar(10), TransDate, 103) = CONVERT (varchar(10), '" + TransDate + "', 103) ";
                        Query += "AND CONVERT(VARCHAR(1000),Description) = '" + Description + "' AND CONVERT(DECIMAL(28,4), Amount) = CONVERT(DECIMAL(28,4), " + Convert.ToDecimal(Amount) + ") AND UPPER(Type) = UPPER('" + Type + "') ";
                        Cmd = new SqlCommand(Query, Conn);
                        int result = Convert.ToInt32(Cmd.ExecuteScalar());
                        Conn.Close();

                        bool Check = true;
                        if (result > 0)
                        {
                            Existing = "Y";
                            Check = false;
                        }
                        else
                        {
                            Existing = "N";
                            Check = true;
                        }

                        dgvBankStatement.Rows.Add(Check, dgvBankStatement.RowCount + 1, TransDate, Type, Description, Amount, "", Existing);

                        if (Check == true)
                        {
                            dgvBankStatement.Rows[RowNo].Cells[0].ReadOnly = true;
                        }
                        else
                        {
                            dgvBankStatement.Rows[RowNo].DefaultCellStyle.BackColor = Color.Red;
                        }

                        RowNo++;
                    }
                    i++;
                }
            }
            catch (Exception e)
            {


                if (e.Message.Contains("Object reference not set to an instance of an object"))
                {
                    ErrorMessage = "Templete tidak sesuai";
                    MessageBox.Show(ErrorMessage);
                }
                else if (e.Message.Contains("is being used by another process"))
                {
                    ErrorMessage = "File sedang dibuka, silahkan tutup file terlebih dahulu";
                    MessageBox.Show(ErrorMessage);
                }

                return;
            }
        }

        private void AttachmentBankMandiri()
        {
            try
            {

                int RowNo = 0;

                HSSFWorkbook hssfwb;
                using (FileStream file = new FileStream(txtAttachment.Text, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new HSSFWorkbook(file);
                }

                HSSFSheet sheet = (HSSFSheet)hssfwb.GetSheetAt(0);
                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null)
                    {
                        string Existing = "";

                        //TransDate
                        string TransDate = sheet.GetRow(row).GetCell(1).StringCellValue;

                        if (TransDate == "")
                        {
                            ErrorMessage = "TransDate harus diisi pada baris ke ";
                            MessageBox.Show(ErrorMessage + (row + 1));
                            dgvBankStatement.Rows.Clear();
                            return;
                        }

                        if (TransDate.Length == 10 && TransDate.Contains("/"))
                        {
                            if (DateTime.TryParseExact(TransDate, "dd/MM/yyyy", new System.Globalization.CultureInfo("id-ID"), DateTimeStyles.None, out CheckDate) == false)
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }
                        else
                        {
                            if (TransDate.ToUpper().Trim() == "PEND")
                            {
                                continue;
                            }
                            else
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }

                        //Description
                        string Description = sheet.GetRow(row).GetCell(2).StringCellValue;

                        //Amount & Type Validation                    
                        string Amount = sheet.GetRow(row).GetCell(4).StringCellValue;
                        string Type = "";

                        if (Amount != "" && Convert.ToDecimal(Amount) != 0)
                        {
                            double Num;
                            bool isNum = double.TryParse(Amount, out Num);

                            if (!isNum)
                            {
                                ErrorMessage = "Format Amount tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                            else
                            {
                                Amount = Convert.ToDouble(Amount).ToString("N4");
                                Type = "DB";
                            }
                        }
                        else
                        {
                            Amount = sheet.GetRow(row).GetCell(5).StringCellValue;

                            if (Amount != "" && Convert.ToDecimal(Amount) != 0)
                            {
                                double Num;
                                bool isNum = double.TryParse(Amount, out Num);

                                if (!isNum)
                                {
                                    ErrorMessage = "Format Amount tidak valid pada baris ke ";
                                    MessageBox.Show(ErrorMessage + (row + 1));
                                    dgvBankStatement.Rows.Clear();
                                    return;
                                }
                                else
                                {
                                    Amount = Convert.ToDouble(Amount).ToString("N4");
                                    Type = "CR";
                                }
                            }
                            else
                            {
                                ErrorMessage = "Amount harus lebih besar dari 0 pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }

                        }

                        //Check Data Existing
                        Conn = ConnectionString.GetConnection();
                        Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementDtl ";
                        Query += "WHERE Account_No = '" + txtAccountNo.Text + "' AND Account_Name = '" + txtAccountName.Text + "' ";
                        Query += "AND Bank_Code = '" + txtBankCode.Text + "' AND CONVERT (varchar(10), TransDate, 103) = CONVERT (varchar(10), '" + TransDate + "', 103) ";
                        Query += "AND CONVERT(VARCHAR(1000),Description) = '" + Description + "' AND CONVERT(DECIMAL(28,4), Amount) = CONVERT(DECIMAL(28,4), " + Convert.ToDecimal(Amount) + ") AND UPPER(Type) = UPPER('" + Type + "') ";
                        Cmd = new SqlCommand(Query, Conn);
                        int result = Convert.ToInt32(Cmd.ExecuteScalar());
                        Conn.Close();

                        bool Check = true;
                        if (result > 0)
                        {
                            Existing = "Y";
                            Check = false;
                        }
                        else
                        {
                            Existing = "N";
                            Check = true;
                        }

                        dgvBankStatement.Rows.Add(Check, dgvBankStatement.RowCount + 1, TransDate, Type, Description, Amount, "", Existing);

                        if (Check == true)
                        {
                            dgvBankStatement.Rows[RowNo].Cells[0].ReadOnly = true;
                        }
                        else
                        {
                            dgvBankStatement.Rows[RowNo].DefaultCellStyle.BackColor = Color.Red;
                        }

                        RowNo++;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Object reference not set to an instance of an object"))
                {
                    ErrorMessage = "Templete tidak sesuai";
                    MessageBox.Show(ErrorMessage);
                }
                else if (e.Message.Contains("is being used by another process"))
                {
                    ErrorMessage = "File sedang dibuka, silahkan tutup file terlebih dahulu";
                    MessageBox.Show(ErrorMessage);
                }

                return;
            }
        }

        private void AttachmentBankBNI()
        {
            try
            {
                int RowNo = 0;

                HSSFWorkbook hssfwb;
                using (FileStream file = new FileStream(txtAttachment.Text, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new HSSFWorkbook(file);
                }

                HSSFSheet sheet = (HSSFSheet)hssfwb.GetSheetAt(0);
                for (int row = 15; row <= sheet.LastRowNum; row = row + 3)
                {
                    if (sheet.GetRow(row) != null)
                    {
                        string Existing = "";

                        //TransDate
                        string TransDate = (sheet.GetRow(row).GetCell(1).StringCellValue);

                        if (TransDate == "")
                        {
                            string TotalDebet = (sheet.GetRow(row).GetCell(3).StringCellValue);
                            if ((TotalDebet.ToUpper()).Contains("TOTAL"))
                            {
                                break;
                            }
                            else
                            {
                                ErrorMessage = "Date harus diisi pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }

                        if (TransDate.Length >= 10)
                        {
                            TransDate = (sheet.GetRow(row).GetCell(1).StringCellValue).Substring(0, 10);
                        }

                        if (TransDate.Length == 10 && TransDate.Contains("/"))
                        {
                            if (DateTime.TryParseExact(TransDate, "dd/MM/yyyy", new System.Globalization.CultureInfo("id-ID"), DateTimeStyles.None, out CheckDate) == false)
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }
                        else
                        {
                            if (TransDate.ToUpper().Trim() == "PEND")
                            {
                                continue;
                            }
                            else
                            {
                                ErrorMessage = "Format Date tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }

                        //Description
                        string Description = sheet.GetRow(row).GetCell(11).StringCellValue;

                        //Amount & Type Validation                    
                        string Amount = sheet.GetRow(row).GetCell(17).StringCellValue;

                        if (Amount != "" && Convert.ToDecimal(Amount) != 0)
                        {
                            double Num;
                            bool isNum = double.TryParse(Amount, out Num);

                            if (!isNum)
                            {
                                ErrorMessage = "Format Amount tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                            else
                            {
                                Amount = Convert.ToDouble(Amount).ToString("N4");
                            }
                        }
                        else
                        {
                            ErrorMessage = "Amount harus lebih besar dari 0 pada baris ke ";
                            MessageBox.Show(ErrorMessage + (row + 1));
                            dgvBankStatement.Rows.Clear();
                            return;
                        }

                        //Type
                        string Type = sheet.GetRow(row).GetCell(20).StringCellValue;
                        if (Type == "")
                        {
                            ErrorMessage = "Type harus diisi pada baris ke ";
                            MessageBox.Show(ErrorMessage + (row + 1));
                            dgvBankStatement.Rows.Clear();
                            return;
                        }

                        if (Type != "K")
                        {
                            if (Type != "D")
                            {
                                ErrorMessage = "Type tidak valid pada baris ke ";
                                MessageBox.Show(ErrorMessage + (row + 1));
                                dgvBankStatement.Rows.Clear();
                                return;
                            }
                        }

                        if (Type == "K")
                        {
                            Type = "CR";
                        }
                        else if (Type == "D")
                        {
                            Type = "DB";
                        }
                        else
                        {
                            ErrorMessage = "Type tidak valid pada baris ke ";
                            MessageBox.Show(ErrorMessage + (row + 1));
                            dgvBankStatement.Rows.Clear();
                            return;
                        }


                        //Check Data Existing
                        Conn = ConnectionString.GetConnection();
                        Query = "SELECT COUNT(BS_Id) FROM ImportBankStatementDtl ";
                        Query += "WHERE Account_No = '" + txtAccountNo.Text + "' AND Account_Name = '" + txtAccountName.Text + "' ";
                        Query += "AND Bank_Code = '" + txtBankCode.Text + "' AND CONVERT (varchar(10), TransDate, 103) = CONVERT (varchar(10), '" + TransDate + "', 103) ";
                        Query += "AND CONVERT(VARCHAR(1000),Description) = '" + Description + "' AND CONVERT(DECIMAL(28,4), Amount) = CONVERT(DECIMAL(28,4), " + Convert.ToDecimal(Amount) + ") AND UPPER(Type) = UPPER('" + Type + "') ";
                        Cmd = new SqlCommand(Query, Conn);
                        int result = Convert.ToInt32(Cmd.ExecuteScalar());
                        Conn.Close();

                        bool Check = true;
                        if (result > 0)
                        {
                            Existing = "Y";
                            Check = false;
                        }
                        else
                        {
                            Existing = "N";
                            Check = true;
                        }

                        dgvBankStatement.Rows.Add(Check, dgvBankStatement.RowCount + 1, TransDate, Type, Description, Amount, "", Existing);

                        if (Check == true)
                        {
                            dgvBankStatement.Rows[RowNo].Cells[0].ReadOnly = true;
                        }
                        else
                        {
                            dgvBankStatement.Rows[RowNo].DefaultCellStyle.BackColor = Color.Red;
                        }

                        RowNo++;
                    }
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Object reference not set to an instance of an object"))
                {
                    ErrorMessage = "Templete tidak sesuai";
                    MessageBox.Show(ErrorMessage);
                }
                else if (e.Message.Contains("is being used by another process"))
                {
                    ErrorMessage = "File sedang dibuka, silahkan tutup file terlebih dahulu";
                    MessageBox.Show(ErrorMessage);
                }               
               
                return;
            }
        }

        private void setHeaderDgvBankStatement()
        {
            if (dgvBankStatement.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = chk = new DataGridViewCheckBoxColumn();
                dgvBankStatement.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            dgvBankStatement.ColumnCount = 8;
            dgvBankStatement.Columns[1].Name = "No";
            dgvBankStatement.Columns[2].Name = "TransDate";
            dgvBankStatement.Columns[3].Name = "Type";
            dgvBankStatement.Columns[4].Name = "Description";
            dgvBankStatement.Columns[5].Name = "Amount";
            dgvBankStatement.Columns[6].Name = "SeqNo";
            dgvBankStatement.Columns[7].Name = "Existing";

            dgvBankStatement.Columns["TransDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvBankStatement.Columns[6].Visible = false;
            dgvBankStatement.Columns[7].Visible = false;

            dgvBankStatement.ReadOnly = false;

            for (int i = 0; i <= 7; i++)
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
        }

        private void SortNoDataGridBankStatement()
        {
            for (int i = 0; i < dgvBankStatement.RowCount; i++)
            {
                dgvBankStatement.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                int xCount = dgvBankStatement.Rows
                .Cast<DataGridViewRow>()
                .Select(row => row.Cells["Existing"].Value.ToString())
                .Count(s => s == "Y");

                if (xCount > 0)
                {
                    DialogResult dr = MessageBox.Show("Apakah Anda yakin akan melakukan import ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        SaveConfirmation();
                    }
                }
                else
                {
                    SaveConfirmation();
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

        private void SaveConfirmation()
        {      
            //INSERT HEADER
            Query = "INSERT INTO ImportBankStatementH([Account_No],[Account_Name],[Bank_Code] ,[Status_Code],[CreatedDate],[CreatedBy]) OUTPUT INSERTED.BS_Id ";
            Query += "VALUES (@AccountNo, @AccountName, @BankCode, '01', GETDATE(), @Login)";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.Add("@AccountNo", txtAccountNo.Text);
            Cmd.Parameters.Add("@AccountName", txtAccountName.Text);
            Cmd.Parameters.Add("@BankCode", txtBankCode.Text);
            Cmd.Parameters.Add("@Login", ControlMgr.UserId);
            BS_Id = Cmd.ExecuteScalar().ToString();

            //INSERT DETAIL
            int SeqNo = 1;
            for (int i = 0; i < dgvBankStatement.RowCount; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvBankStatement.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    //Query = "INSERT INTO ImportBankStatementDtl([BS_Id], [SeqNo],[Account_No],[Account_Name],[Bank_Code],[TransDate],[Description],[Amount],[Type],[CreatedDate],[CreatedBy]) ";
                    //Query += " VALUES('" + BS_Id + "', " + SeqNo + ", '" + txtAccountNo.Text + "', '" + txtAccountName.Text + "', '" + txtBankCode.Text + "', ";
                    //Query += "CONVERT(DATE,'" + dgvBankStatement.Rows[i].Cells["TransDate"].Value.ToString() + "', 103), '" + dgvBankStatement.Rows[i].Cells["Description"].Value.ToString() + "', " + Convert.ToDecimal(dgvBankStatement.Rows[i].Cells["Amount"].Value.ToString()) + ", ";
                    //Query += "'" + dgvBankStatement.Rows[i].Cells["Type"].Value.ToString() + "', GETDATE(), '" + ControlMgr.UserId + "')";
                    Query = "INSERT INTO ImportBankStatementDtl([BS_Id], [SeqNo],[Account_No],[Account_Name],[Bank_Code],[TransDate],[Description],[Amount],[Type],[CreatedDate],[CreatedBy]) ";
                    Query += " VALUES(@BS_Id, @SeqNo, @txtAccountNo, @txtAccountName, @txtBankCode, ";
                    Query += "CONVERT(DATE, @TransDate, 103), @Description, @Amount , ";
                    Query += "'" + dgvBankStatement.Rows[i].Cells["Type"].Value.ToString() + "', GETDATE(), '" + ControlMgr.UserId + "')";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.Add("@BS_Id", BS_Id);
                    Cmd.Parameters.Add("@SeqNo", SeqNo);
                    Cmd.Parameters.Add("@txtAccountNo", txtAccountNo.Text);
                    Cmd.Parameters.Add("@txtAccountName", txtAccountName.Text);
                    Cmd.Parameters.Add("@txtBankCode", txtBankCode.Text);
                    Cmd.Parameters.Add("@TransDate", dgvBankStatement.Rows[i].Cells["TransDate"].Value.ToString());
                    Cmd.Parameters.Add("@Description", dgvBankStatement.Rows[i].Cells["Description"].Value.ToString());
                    Cmd.Parameters.Add("@Amount", Convert.ToDecimal(dgvBankStatement.Rows[i].Cells["Amount"].Value.ToString()));
                    Cmd.ExecuteNonQuery();
                    SeqNo++;
                }
            }

            if (SeqNo == 1)
            {
                MessageBox.Show("Silahkan checklist data yang akan diimport");
                return;
            }

            Trans.Commit();
            MessageBox.Show("Data berhasil diimport.");
            Parent.RefreshGrid();
            ModeBeforeEdit();
        }
    }
}
