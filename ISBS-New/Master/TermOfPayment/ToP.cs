using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.TermOfPayment
{
    public partial class ToP : MetroFramework.Forms.MetroForm
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
        String TermOfPayment = null;
        string vOldDeskripsi = "";

        

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public ToP()
        {
            InitializeComponent();
        }

        public void flag(String termofpayment, String mode)
        {
            TermOfPayment = termofpayment;
            Mode = mode;
        }

        //Master.TermOfPayment.InqToP Parent;
        //public void setParent(Master.TermOfPayment.InqToP F)
        //{
        //    Parent = F;
        //}

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            TermOfPayment = id;
        }

        private void ToP_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                modeAfterSave();
            }
        }

        private bool cekValidasi(String TermOfPayment)
        {
            Query = "Select * From [dbo].[TermOfPayment] Where TermOfPayment = '" + TermOfPayment + "'";

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
            Query = "Select * From [dbo].[TermOfPayment] Where TermOfPayment = '" + TermOfPayment + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtTermOfPayment.Text = Dr["TermOfPayment"].ToString(); ;
                    numDueDate.Value = decimal.Parse(Dr["DueDate"].ToString());
                    numGracePeriod.Value = decimal.Parse(Dr["GracePeriod"].ToString());
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();
                    cmbMethod.SelectedItem = Dr["Method"].ToString();
                    vOldDeskripsi = txtDeskripsi.Text;
                }
            }
            Conn.Close();
        }

        private void addCmbCrit()
        {
            cmbMethod.Items.Add("Order");
            cmbMethod.Items.Add("Delivery");
            cmbMethod.Items.Add("Receipt");
            cmbMethod.Items.Add("Invoice");
        }

        private void ModeLoad()
        {
            addCmbCrit();
            RefreshGrid();
            cmbMethod.Enabled = false;
        }

        private void resetText()
        {
            txtTermOfPayment.Text = "";
            numDueDate.Value = 0;
            numGracePeriod.Value = 0;
            txtDeskripsi.Text = "";

            cmbMethod.Enabled = true;
        }

        private void ModeNew()
        {
            resetText();

            txtTermOfPayment.Enabled = true;
            numDueDate.Enabled = true;
            numGracePeriod.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;

            cmbMethod.Enabled = true;
        }

        private void ModeEdit()
        {
            txtTermOfPayment.Enabled = false;
            numDueDate.Enabled = true;
            numGracePeriod.Enabled = true;
            txtDeskripsi.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;

            cmbMethod.Enabled = true;
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
            Parent.RefreshGrid();
            this.Close();
        }

        private Boolean Validasi()
        {
            if (String.IsNullOrEmpty(txtTermOfPayment.Text) || numDueDate.Value == 0)
            {
                MessageBox.Show("Data harus diisi");
                return false;
            }
            else if (String.IsNullOrEmpty(cmbMethod.Text))
            {
                MessageBox.Show("Method harus diisi");
                return false;
            }
            else if (Mode == "New" && cekValidasi(txtTermOfPayment.Text) == false)
            {
                MessageBox.Show("Data sudah ada di database.");
                return false;
            }
            else
                return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                if (Mode == "New")
                {
                    Query = "insert into [dbo].[TermOfPayment] (TermOfPayment, Method, DueDate, GracePeriod, Deskripsi, CreatedDate, CreatedBy) ";
                    Query += "values (@top, @method, '" + numDueDate.Value + "', '" + numGracePeriod.Value + "', @deskripsi, '" + DateTime.Now.ToString() + "', @UserId);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@top", txtTermOfPayment.Text.ToUpper());
                        Cmd.Parameters.AddWithValue("@method", cmbMethod.Text);
                        Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);

                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    txtTermOfPayment.Text = txtTermOfPayment.Text.ToUpper();
                    MessageBox.Show("Data " + txtTermOfPayment.Text + ", berhasil ditambahkan.");
                }
                else if (Mode == "Edit")
                {
                    Query = "update [dbo].[TermOfPayment] set Method = '" + cmbMethod.Text + "', DueDate='" + numDueDate.Value + "', GracePeriod='" + numGracePeriod.Value + "', Deskripsi='" + txtDeskripsi.Text + "', UpdatedDate = '" + DateTime.Now.ToString() + "', UpdatedBy = '" + ControlMgr.UserId + "' where TermOfPayment='" + txtTermOfPayment.Text + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
                    Trans.Commit();
                    MessageBox.Show("Data " + txtTermOfPayment.Text + ", berhasil diupdate.");
                }
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            Conn.Close();
            modeAfterSave();

            Form f = Application.OpenForms["GlobalInquiry"];
            if (f != null)
                if (f.Text == "Term of Payment")
                    Parent.RefreshGrid();

        }

        //created by Thaddaeus Matthias, 20 March 2018, BEGIN
        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = false;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            txtDeskripsi.Text = vOldDeskripsi;
            txtDeskripsi.Enabled = false;

            cmbMethod.Enabled = false;
        }

        private void modeAfterSave()
        {
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
            txtTermOfPayment.Enabled = false;
            txtDeskripsi.Enabled = false;
            numDueDate.Enabled = false;
            numGracePeriod.Enabled = false;
            vOldDeskripsi = txtDeskripsi.Text;
            Mode = "Edit";
            cmbMethod.Enabled = false;
        }
        //END=================================================
    }
}
