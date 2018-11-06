using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.CompanyInfo
{
    public partial class FormCompanyInfo : MetroFramework.Forms.MetroForm
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

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public FormCompanyInfo()
        {
            InitializeComponent();
        }

        //Mode
        string Mode = "";

        private void FormCompanyInfo_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }

        public void ModeNew()
        {
            Mode = "New";

            txtCompanyId.Enabled = true;
            txtCompanyName.Enabled = true;
            txtCompanyAddress.Enabled = true;
            txtCompanyPhone.Enabled = true;
            txtCompanyFax.Enabled = true;
            txtNpwp.Enabled = true;
            txtTaxName.Enabled = true;
            txtTaxAddress.Enabled = true;
            txtSignatoryName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtCompanyId.Enabled = false;
            txtCompanyName.Enabled = true;
            txtCompanyAddress.Enabled = true;
            txtCompanyPhone.Enabled = true;
            txtCompanyFax.Enabled = true;
            txtNpwp.Enabled = true;
            txtTaxName.Enabled = true;
            txtTaxAddress.Enabled = true;
            txtSignatoryName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtCompanyId.Enabled = false;
            txtCompanyName.Enabled = false;
            txtCompanyAddress.Enabled = false;
            txtCompanyPhone.Enabled = false;
            txtCompanyFax.Enabled = false;
            txtNpwp.Enabled = false;
            txtTaxName.Enabled = false;
            txtTaxAddress.Enabled = false;
            txtSignatoryName.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
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
                if (txtCompanyId.Text == "")
                {
                    MessageBox.Show("Data Company Id tidak boleh kosong.");
                    return;
                }
                else if (txtCompanyName.Text == "")
                {
                    MessageBox.Show("Data Company Name tidak boleh kosong.");
                    return;
                }

                Query = "Select CompanyId from CompanyInfo where CompanyId=@coid";

                using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                {
                    Cmd.Parameters.AddWithValue("@coid", txtCompanyId.Text.Trim().ToUpper());

                    CekData = Cmd.ExecuteScalar() == null ? "" : Cmd.ExecuteScalar().ToString();
                }

                if (CekData != "" && Mode != "Edit")
                {
                    MessageBox.Show("CompanyId " + txtCompanyId.Text.Trim().ToUpper() + " sudah digunakan, silahkan diganti dengan yang lain.");
                    Conn.Close();
                    return;
                }

                //Jika New
                if (Mode == "New")
                {
                    Query = "Insert into dbo.CompanyInfo ";
                    Query += "(CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName) values ";
                    Query += "(@coid, @coname, @coaddress, @cophone, @cofax, @npwp, @taxname, @taxaddress, @signname)";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@coid", txtCompanyId.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@coname", txtCompanyName.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@coaddress", txtCompanyAddress.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@cophone", txtCompanyPhone.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@cofax", txtCompanyFax.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@npwp", txtNpwp.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@taxname", txtTaxName.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@taxaddress", txtTaxAddress.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@signname", txtSignatoryName.Text.Trim().ToUpper());
                        Cmd.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("CompanyId = " + txtCompanyId.Text.Trim().ToUpper() + Environment.NewLine + "CompanyName = " + txtCompanyName.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode == "Edit")
                {
                    Query = "Update dbo.CompanyInfo set CompanyName=@coname, CompanyAddress=@coaddress, CompanyPhone=@cophone, CompanyFax=@cofax, NPWP=@npwp, TaxName=@taxname, TaxAddress=@taxaddress, SignatoryName=@signname where CompanyId=@coid";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@coid", txtCompanyId.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@coname", txtCompanyName.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@coaddress", txtCompanyAddress.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@cophone", txtCompanyPhone.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@cofax", txtCompanyFax.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@npwp", txtNpwp.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@taxname", txtTaxName.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@taxaddress", txtTaxAddress.Text.Trim().ToUpper());
                        Cmd.Parameters.AddWithValue("@signname", txtSignatoryName.Text.Trim().ToUpper());
                        Cmd.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("CompanyId = " + txtCompanyId.Text.Trim().ToUpper() + Environment.NewLine + "CompanyName = " + txtCompanyName.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diedit.");
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

        public void GetDataHeader(string CompanyId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CompanyId, CompanyName, CompanyAddress, CompanyPhone, CompanyFax, NPWP, TaxName, TaxAddress, SignatoryName From [dbo].[CompanyInfo] where CompanyId=@coid";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@coid", CompanyId);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtCompanyId.Text = Dr["CompanyId"].ToString();
                txtCompanyName.Text = Dr["CompanyName"].ToString();
                txtCompanyAddress.Text = Dr["CompanyAddress"].ToString();
                txtCompanyPhone.Text = Dr["CompanyPhone"].ToString();
                txtCompanyFax.Text = Dr["CompanyFax"].ToString();
                txtNpwp.Text = Dr["NPWP"].ToString();
                txtTaxName.Text = Dr["TaxName"].ToString();
                txtTaxAddress.Text = Dr["TaxAddress"].ToString();
                txtSignatoryName.Text = Dr["SignatoryName"].ToString();
            }
            Dr.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCompanyPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCompanyFax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtNpwp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
        }
     
                        
    }
}
