using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Currency
{
    public partial class Currency : MetroFramework.Forms.MetroForm
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
        String CurrencyId = null;

        //Master.Currency.InqCurrency Parent;

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
            CurrencyId = id;
        }

        public Currency()
        {
            InitializeComponent();
        }

        //public void flag(String currencyid, String mode)
        //{
        //    CurrencyId = currencyid;
        //    Mode = mode;
        //}

        //public void setParent(Master.Currency.InqCurrency F)
        //{
        //    Parent = F;
        //}

        private void Currency_Load(object sender, EventArgs e)
        {
            //ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                RefreshGrid();
                ModeBeforeEdit();
            }
        }

        private bool cekValidasi(String CurrencyId)
        {
            Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + CurrencyId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                if (Dr.Read())//sama dengan di database
                {
                    Conn.Close();
                    return false;
                }
                else
                {
                    Conn.Close();
                    return true;
                }
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[CurrencyTable] Where CurrencyId = '" + CurrencyId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtCurrencyId.Text = Dr["CurrencyId"].ToString(); ;
                    txtCurrencyName.Text = Dr["CurrencyName"].ToString();
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
            txtCurrencyName.Text = "";
        }

        private void ModeNew()
        {
            Mode = "New";
            resetText();

            txtCurrencyId.Enabled = true;
            txtCurrencyName.Enabled = true;

            btnEdit.Enabled = false;
            btnSave.Enabled = true;
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            txtCurrencyId.Enabled = false;
            txtCurrencyName.Enabled = true;

            btnSave.Enabled = true;
            btnEdit.Enabled = false;
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            txtCurrencyId.Enabled = false;
            txtCurrencyName.Enabled = false;

            btnSave.Enabled = false;
            btnEdit.Enabled = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean Validasi()
        {
            if (txtCurrencyId.Text.Trim() == "")
            {
                MessageBox.Show("Currency Id harus diisi");
                return false;
            }
            else if (txtCurrencyName.Text.Trim() == "")
            {
                MessageBox.Show("Deskripsi harus diisi");
                return false;
            }
            else if (Mode == "New" && !cekValidasi(txtCurrencyId.Text))
            {
                MessageBox.Show("Data sudah ada di database.");
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
                    Query = "insert into [dbo].[CurrencyTable] (CurrencyId, CurrencyName) ";
                    Query += "values (@currid, @currname);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@currid", txtCurrencyId.Text);
                        Cmd.Parameters.AddWithValue("@currname", txtCurrencyName.Text);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("Data " + txtCurrencyId.Text + ", berhasil ditambahkan.");
                }
                else if (Mode == "Edit")
                {
                    Query = "update [dbo].[CurrencyTable] set CurrencyName = @currname where CurrencyId = @currid";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@currid", txtCurrencyId.Text);
                        Cmd.Parameters.AddWithValue("@currname", txtCurrencyName.Text);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("Data " + txtCurrencyId.Text + ", berhasil diupdate.");
                }
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            finally { }
            Conn.Close();
            ModeBeforeEdit();

            Form f = Application.OpenForms["GlobalInquiry"];
            if (f != null)
                if (f.Text == "Currency")
                    Parent.RefreshGrid();
        }
    }
}
