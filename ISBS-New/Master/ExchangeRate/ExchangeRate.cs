using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.ExchangeRate
{
    public partial class ExchangeRate : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String RecId = null;

        //Master.ExchangeRate.ExchangeRateInq Parent;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            RecId = id;
        }

        public ExchangeRate()
        {
            InitializeComponent();
        }

        //public void flag(String recid, String mode)
        //{
        //    RecId = recid;
        //    Mode = mode;
        //}

        //public void setParent(Master.ExchangeRate.ExchangeRateInq F)
        //{
        //    Parent = F;
        //}

        private void ExchangeRate_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
            if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
            if (Mode == "Edit")
            {
                ModeEdit();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select [CurrencyID],[RecID],[ExchRate] From [dbo].[ExchRate] Where RecId = '" + RecId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtRecId.Text = Dr["RecID"].ToString();
                    txtCurrencyId.Text = Dr["CurrencyId"].ToString();
                    txtExchRate.Text = Dr["ExchRate"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeLoad()
        {
            RefreshGrid();
        }

        private void resetText()
        {
            txtCurrencyId.Text = "";
            txtExchRate.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtCurrencyId.Enabled = false;
            txtExchRate.Enabled = true;
            txtRecId.Enabled = false;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnSCust.Enabled = true;
        }

        private void ModeEdit()
        {
            txtCurrencyId.Enabled = false;
            txtExchRate.Enabled = true;
            txtRecId.Enabled = false;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnSCust.Enabled = true;
        }

        private void ModeBeforeEdit()
        {
            txtCurrencyId.Enabled = false;
            txtExchRate.Enabled = false;
            txtRecId.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnSCust.Enabled = false;
        }

        private Boolean Validasi()
        {
            if (txtCurrencyId.Text.Trim() == "")
            {
                MessageBox.Show("Currency Id harus diisi");
                return false;
            }
            else if (txtExchRate.Text.Trim() == "")
            {
                MessageBox.Show("Exchange Rate harus diisi");
                return false;
            }
            else if (Convert.ToInt32(Math.Ceiling(Convert.ToDouble(txtExchRate.Text))) == 0)
            {
                MessageBox.Show("Exhange Rate harus lebih dari 0");
                return false;
            }
            else
                return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!Validasi())
                return;

            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                if (Mode == "New")
                {
                    Query = "insert into [dbo].[ExchRate] (CurrencyId, ExchRate, CreatedDate, CreatedBy) ";
                    Query += "values (@currid,@exchrate,getdate(),@UserId);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@currid", txtCurrencyId.Text);
                        Cmd.Parameters.AddWithValue("@exchrate", txtExchRate.Text);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("Currency = " + txtCurrencyId.Text.Trim().ToUpper() + Environment.NewLine + "Exchange Rate = " + txtExchRate.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }
                else if (Mode == "Edit")
                {
                    Query = "update [dbo].[ExchRate] set ExchRate=@exchrate , UpdatedDate = getdate(),UpdatedBy = @UserId where RecId=@recid";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@recid", txtRecId.Text);
                        Cmd.Parameters.AddWithValue("@exchrate", txtExchRate.Text);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("RecID " + txtRecId.Text + ", berhasil diupdate.");
                }
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }            
            Conn.Close();
            

            Form f = Application.OpenForms["GlobalInquiry"];
            if (f != null)
                if (f.Text == "Exchange Rate")
                    Parent.RefreshGrid();

            ModeBeforeEdit();
        }
    

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";
                ModeEdit();                
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnSCust_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "CurrencyTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.Text = "Search Currency";
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode != "")
            {
                txtCurrencyId.Text = ConnectionString.Kode;
            }
            ConnectionString.Kode = "";
        }

        private void txtExchRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }



    }
}
