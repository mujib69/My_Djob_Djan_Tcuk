using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Transactions;
using System.Data.SqlClient;

namespace ISBS_New.Sales.CreditLimit
{
    public partial class CreditLimitHeader : MetroFramework.Forms.MetroForm
    {
        SqlCommand Cmd;
        TransactionScope scope;
        SqlDataReader Dr;
        SqlConnection Conn;

        //global mode
        string Mode = "";

        //set Parent From Global Inquiry
        GlobalInquiry Parent;
        public void SetParent(GlobalInquiry setParent) { Parent = setParent; }

        //set Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        //setMode
        public void SetMode(string passedMode, string passedID)
        {
            Mode = passedMode;
            txtTransactionNo.Text = passedID;
            if (Mode == "BeforeEdit")
            {
                ModeView();
                GetHeader();
            }
            else if (Mode == "New")
            {
                ModeNew();
            }
        }

        public CreditLimitHeader()
        {
            InitializeComponent();
        }

        private void CreditLimitHeader_Load(object sender, EventArgs e)
        {
            
        }

        private void ModeNew()
        {
            Mode = "New";
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnEdit.Enabled = false;
            btnExit.Enabled = true;
            btnApprove.Enabled = false;
            btnReject.Enabled = false;

            txtLimitTemporary.ReadOnly = false;
            txtPercent.ReadOnly = false;
            dtTransactionDate.Enabled = false;
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnEdit.Enabled = false;
            btnExit.Enabled = false;
            btnApprove.Enabled = false;
            btnReject.Enabled = false;

            txtLimitTemporary.ReadOnly = false;
            txtPercent.ReadOnly = false;
            dtTransactionDate.Enabled = false;
        }

        private void ModeView()
        {
            Mode = "View";
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;

            if (CekStatusCode() != "01")
            {
                btnEdit.Enabled = false;
                btnApprove.Enabled = false;
                btnReject.Enabled = false;
            }
            else
            {
                btnEdit.Enabled = true;
                btnApprove.Enabled = true;
                btnReject.Enabled = true;
            }
            txtLimitTemporary.ReadOnly = true;
            txtPercent.ReadOnly = true;
            dtTransactionDate.Enabled = true;
        }

        private void GetHeader()
        {
            if (txtTransactionNo.Text != "")
            {
                string Query = "SELECT a.*,b.[CustName],b.[Limit_Total] FROM [dbo].[CreditLimit] a LEFT JOIN [dbo].[CustTable] b ON a.[Customer_Id] = b.[CustId] WHERE [Trans_No] = @Trans_No;";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@Trans_No", txtTransactionNo.Text);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        dtTransactionDate.Value = Convert.ToDateTime(Dr["Trans_Date"]);
                        txtCustID.Text = Dr["Customer_Id"].ToString();
                        txtCustName.Text = Dr["CustName"].ToString();
                        txtLimitTemporary.Text = String.Format("{0:#,##0.###0}",Convert.ToDecimal(Dr["Limit_Temp"])).ToString();
                        txtTotalLimit.Text = String.Format("{0:#,##0.###0}",Convert.ToDecimal(Dr["Limit_Total"])).ToString();
                        txtPercent.Text = String.Format("{0:#,##0.###0}",((Convert.ToDecimal(Dr["Limit_Temp"]) / Convert.ToDecimal(Dr["Limit_Total"])) * 100)).ToString();
                    }
                    Dr.Close();
                }
            }
        }

        private string validation()
        {
            string msg = "";

            decimal sumTempLimit = 0;
            DateTime TGL = DateTime.Now;
            string Query = "SELECT SUM([Limit_Temp]) FROM [dbo].[CreditLimit] WHERE [Customer_Id]=@Customer_Id AND [StatusCode] in ('01','02') AND [Trans_No]!=@Trans_No ";
            Query += "  AND (CASE WHEN ApprovedBy = '' THEN CreatedDate ELSE ApprovedDate END) > ('"+TGL.Year+"-"+TGL.Month+"-"+TGL.Day+" 00:00:01')";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@Trans_No", txtTransactionNo.Text);
                Cmd.Parameters.AddWithValue("@Customer_Id",txtCustID.Text);
                if (Cmd.ExecuteScalar() == System.DBNull.Value)
                {
                    sumTempLimit = 0;
                }
                else
                {
                    sumTempLimit = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
            }
            if ((Convert.ToDecimal(txtLimitTemporary.Text) / Convert.ToDecimal(txtTotalLimit.Text) * 100) > 20)
            {
                msg += "-Limit temporary tidak boleh diatas 20%.\n\r";
            }
            else if (((sumTempLimit + (Convert.ToDecimal(txtLimitTemporary.Text))) / Convert.ToDecimal(txtTotalLimit.Text) * 100) > 20)
            {
                msg += "-Limit temporary total tidak boleh diatas 20%.(coba cek transakasi credit limit yang sudah dan belum di-approve)\n\r";
            }
            if (Convert.ToDecimal(txtLimitTemporary.Text) == 0)
            {
                msg += "-Limit temporary tidak boleh 0.\n\r";
            }
            if (txtCustID.Text == "")
            {
                msg += "-Pilih Customer ID.\n\r";
            }
            return msg;
        }

        private void InsertCreditLimit()
        {
            string CLID = ConnectionString.GenerateSequenceNo("CLT","CLT","","",ConnectionString.GetConnection(),Cmd);
            txtTransactionNo.Text = CLID;
            string Query = "INSERT INTO [CreditLimit] ([Trans_Date],[Trans_No],[Customer_Id],[Limit_Temp],[StatusCode],[CreatedBy],[CreatedDate]) VALUES (@Trans_Date,@Trans_No,@Customer_Id,@Limit_Temp,@StatusCode,@CreatedBy,getdate())";
            using(Cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@Trans_Date",dtTransactionDate.Value);
                Cmd.Parameters.AddWithValue("@Trans_No",txtTransactionNo.Text);
                Cmd.Parameters.AddWithValue("@Customer_Id",txtCustID.Text);
                Cmd.Parameters.AddWithValue("@Limit_Temp",Convert.ToDecimal(txtLimitTemporary.Text));
                Cmd.Parameters.AddWithValue("@StatusCode","01");
                Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }

            ListMethod.StatusLogCustomer("CreditLimitHeader", "CreditLimit", txtCustID.Text, "01", "", txtTransactionNo.Text, "", "", "");
        }

        private void UpdateCreditLimit()
        {
            string Query = "UPDATE [CreditLimit] SET [Trans_Date]=@Trans_Date,[Customer_Id]=@Customer_Id,[Limit_Temp]=@Limit_Temp,UpdatedBy = @UpdatedBy, UpdatedDate = getdate() WHERE Trans_No=@Trans_No";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@Trans_Date", dtTransactionDate.Value);
                Cmd.Parameters.AddWithValue("@Trans_No", txtTransactionNo.Text);
                Cmd.Parameters.AddWithValue("@Customer_Id", txtCustID.Text);
                Cmd.Parameters.AddWithValue("@Limit_Temp", Convert.ToDecimal(txtLimitTemporary.Text));
                Cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }

            ListMethod.StatusLogCustomer("CreditLimitHeader", "CreditLimit", txtCustID.Text, "01", "Edit", txtTransactionNo.Text, "", "", "");
        }

        private void UpdateApprove()
        {
            string Query = "UPDATE [CreditLimit] SET [StatusCode]= @StatusCode,[ApprovedBy]= @ApprovedBy,[ApprovedDate]=getdate() WHERE Trans_No=@Trans_No";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@StatusCode","02");
                Cmd.Parameters.AddWithValue("@Trans_No", txtTransactionNo.Text);
                Cmd.Parameters.AddWithValue("@ApprovedBy", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }

            ListMethod.StatusLogCustomer("CreditLimitHeader", "CreditLimit", txtCustID.Text, "02", "", txtTransactionNo.Text, "", "", "");


            Query = "UPDATE [CustTable] SET [Limit_Temp]+=@Limit_Temp WHERE [CustId]=@CustId";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CustId",txtCustID.Text);
                Cmd.Parameters.AddWithValue("@Limit_Temp", Convert.ToDecimal(txtLimitTemporary.Text));
                Cmd.ExecuteNonQuery();
            }
        }

        private void UpdateReject()
        {
            string Query = "UPDATE [CreditLimit] SET [StatusCode]=@StatusCode,[ApprovedBy]=@ApprovedBy,[ApprovedDate]=getdate() WHERE Trans_No=@Trans_No";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@StatusCode", "XX");
                Cmd.Parameters.AddWithValue("@Trans_No", txtTransactionNo.Text);
                Cmd.Parameters.AddWithValue("@ApprovedBy", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }

            ListMethod.StatusLogCustomer("CreditLimitHeader", "CreditLimit", txtCustID.Text, "XX", "", txtTransactionNo.Text, "", "", "");

        }

        private void btnSearchCust_Click(object sender, EventArgs e)
        {
            try
            {
                SearchV2 f = new SearchV2();
                f.SetMode("No");
                f.SetSchemaTable("dbo", "CustTable", "", "", "CustTable a");
                f.ShowDialog();
                if (SearchV2.data.Count != 0)
                {
                    txtCustID.Text = SearchV2.data[0];
                    txtCustName.Text = SearchV2.data[1];
                }
                f.Dispose();
                string Query = "SELECT [Limit_Total] FROM [dbo].[CustTable] WHERE [CustId] = @CustId";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@CustId", txtCustID.Text);
                    if (Cmd.ExecuteScalar() != System.DBNull.Value || Cmd.ExecuteScalar() != "")
                    {
                        txtTotalLimit.Text = String.Format("{0:#,##0.###0}", (Convert.ToDecimal(Cmd.ExecuteScalar()))).ToString();
                    }
                    else
                    {
                        txtTotalLimit.Text = "0.0000";
                    }
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeView();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (PermissionAccess("Edit") > 0)
            {
                if (CekStatusCode() == "01")
                {
                    ModeEdit();
                }
                else
                {
                    MessageBox.Show("Transaksi sudah tidak dapat diedit.");
                }
            }
            else
            {
                MessageBox.Show("Permission Denied.");
            }
        }

        private void txtLimitTemporary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtCustID.Text == null || txtCustID.Text == "")
            {
                e.Handled = true;
                MessageBox.Show("Pilih Customer ID terlebih dahulu.");
                return;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            { 
                e.Handled = true; 
            }
        }

        private void txtPercent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtCustID.Text == null || txtCustID.Text == "")
            {
                e.Handled = true;
                MessageBox.Show("Pilih Customer ID terlebih dahulu.");
                return;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtLimitTemporary_Leave(object sender, EventArgs e)
        {
            if (txtLimitTemporary.Text != "")
            {
                if (Convert.ToDecimal(txtTotalLimit.Text) == 0)
                {
                    txtLimitTemporary.Text = "0.0000";
                    txtPercent.Text = "0.00";
                }
                else
                {
                    decimal PercentLimit = Convert.ToDecimal(txtLimitTemporary.Text) / Convert.ToDecimal(txtTotalLimit.Text) * 100;
                    if (txtPercent.Text == "" || (PercentLimit != Convert.ToDecimal(txtPercent.Text)))
                    {
                        txtPercent.Text = String.Format("{0:#,##0.#0}", PercentLimit).ToString();
                        txtLimitTemporary.Text = String.Format("{0:#,##0.###0}", Convert.ToDecimal(txtLimitTemporary.Text)).ToString();
                    }
                }
            }
            else
            {
                txtLimitTemporary.Text = "0.0000";
                txtPercent.Text = "0.00";
            }
        }

        private void txtPercent_Leave(object sender, EventArgs e)
        {
            if (txtPercent.Text != "")
            {
                decimal LimitTemporary = Convert.ToDecimal(txtPercent.Text) * Convert.ToDecimal(txtTotalLimit.Text) / 100;
                if (txtLimitTemporary.Text == "" || (LimitTemporary != Convert.ToDecimal(txtLimitTemporary.Text)))
                {
                    txtLimitTemporary.Text = String.Format("{0:#,##0.###0}", LimitTemporary).ToString();
                    txtPercent.Text = String.Format("{0:#,##0.#0}", Convert.ToDecimal(txtPercent.Text)).ToString();
                }
            }
            else
            {
                txtLimitTemporary.Text = "0.0000";
                txtPercent.Text = "0.00";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string message = validation();
            if (message != "")
            {
                MessageBox.Show(message);
                return;
            }
            try
            {
                if (Mode == "New")
                {
                    using (scope = new TransactionScope())
                    {
                        InsertCreditLimit();
                        scope.Complete();
                        MessageBox.Show("Credit limit berhasil di save.");
                    }
                    GetHeader();
                    ModeView();
                    Parent.RefreshGrid();
                }
                else if (Mode == "Edit")
                {
                    using (scope = new TransactionScope())
                    {
                        UpdateCreditLimit();
                        scope.Complete();
                        MessageBox.Show("Credit limit berhasil di update.");
                    }
                    GetHeader();
                    ModeView();
                    Parent.RefreshGrid();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
            finally
            {
            }

        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (PermissionAccess("Approve") > 0)
            {
                if (CekStatusCode() == "01")
                {
                    UpdateApprove();
                    MessageBox.Show("Transaksi berhasil di approve.");
                    ModeView();
                    Parent.RefreshGrid();
                }
                else
                {
                    MessageBox.Show("Transaksi sudah tidak dapat diapprove.");
                }
            }
            else
            {
                MessageBox.Show("Permission Denied.");
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (PermissionAccess("Reject") > 0)
            {
                if (CekStatusCode() == "01")
                {
                    UpdateReject();
                    MessageBox.Show("Transaksi berhasil di reject.");
                    ModeView();
                    Parent.RefreshGrid();
                }
                else
                {
                    MessageBox.Show("Transaksi sudah tidak dapat direject.");
                }
            }
            else
            {
                MessageBox.Show("Permission Denied.");
            }
        }

        private string CekStatusCode()
        {
            string status = "";
            string Query = "SELECT StatusCode FROM CreditLimit WHERE Trans_No = @Trans_No";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@Trans_No", txtTransactionNo.Text);
                status = Cmd.ExecuteScalar().ToString();
            }
            return status;
        }
    }
}
