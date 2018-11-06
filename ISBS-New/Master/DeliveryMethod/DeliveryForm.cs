using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.DeliveryMethod
{
    public partial class DeliveryForm : MetroFramework.Forms.MetroForm
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
        String DeliveryMethod = null;
        //Master.DeliveryMethod.DeliveryInquiry Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //public void flag(String deliverymethod, String mode)
        //{
        //    DeliveryMethod = deliverymethod;
        //    Mode = mode;
        //}

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            DeliveryMethod = id;
        }

        public DeliveryForm()
        {
            InitializeComponent();
        }

        private void DeliveryForm_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "BeforeEdit")
            {
                modeAfterSave();
                RefreshGrid();
            }
            //else
            //{
            //    RefreshGrid();
            //}
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[DeliveryMethod] Where DeliveryMethod = @deliverymethod";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@deliverymethod", DeliveryMethod);

                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtDeliveryMethod.Text = DeliveryMethod.ToString();
                    txtDeskripsi.Text = Dr["Deskripsi"].ToString();

                    if (Dr["Deskripsi"].ToString() == "FRANCO")
                    {
                        rdBtnFranco.Checked = true;
                    }
                    else
                    { rdBtnLoco.Checked = true;  }
                }
            }
            Conn.Close();
        }

        private void ModeNew()
        {
            txtDeliveryMethod.Enabled = true;
            txtDeskripsi.Enabled = true;
            rdBtnFranco.Enabled = true;
            rdBtnLoco.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtDeliveryMethod.Enabled = false;
            rdBtnLoco.Enabled = false;
            rdBtnFranco.Enabled = false;
            txtDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnCancel.Visible = true;
        }

        //public void ParentRefreshGrid(Master.DeliveryMethod.DeliveryInquiry f)
        //{
        //    Parent = f;
        //}

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
            txtDeliveryMethod.Enabled = false;
            txtDeskripsi.Enabled = false;

            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
        }

        private bool cekValidasi(String DeliveryMethod)
        {
            Query = "Select * From [dbo].[DeliveryMethod] Where DeliveryMethod = @deliverymethod ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@deliverymethod", DeliveryMethod);

                Dr = Cmd.ExecuteReader();

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

        private Boolean Validasi()
        {
            if (txtDeliveryMethod.Text.Trim() == "")
            {
                MessageBox.Show("Data DeliveryMethod harus diisi");
                return false;
            }
            else if (rdBtnFranco.Checked == false && rdBtnLoco.Checked == false)
            {
                MessageBox.Show("Pilih Type Antar..");
                return false;            
            }
            else if (Mode == "New" && cekValidasi(txtDeliveryMethod.Text) == false)
            {
                MessageBox.Show("Data sudah ada di database.");
                return false;
            }
            else
                return true;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validasi() == false)
                return;

            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            try
            {
                if (Mode == "New")
                {
                    Query = "insert into [dbo].[DeliveryMethod] (DeliveryMethod,Franco_Loco, Deskripsi, CreatedDate, CreatedBy) ";
                    Query += "values (@deliverymethod,@Franco_Loco, @deskripsi , getDate(), @UserId);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@deliverymethod", txtDeliveryMethod.Text);

                        if (rdBtnFranco.Checked)
                        {
                            Cmd.Parameters.AddWithValue("@Franco_Loco", "FRANCO");
                        }
                        else
                        {
                            Cmd.Parameters.AddWithValue("@Franco_Loco", "LOCO");
                        }

                        Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();

                    }
                    Trans.Commit();
                    MessageBox.Show("Data " + txtDeliveryMethod.Text + ", berhasil ditambahkan.");
                }
                if (Mode == "Edit")
                {
                    Query = "update [dbo].[DeliveryMethod] set Deskripsi = @deskripsi, UpdatedDate = getDate(), UpdatedBy = @UserId where DeliveryMethod = @deliverymethod ;";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@deliverymethod", txtDeliveryMethod.Text);
                    Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text);
                    Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                    Cmd.ExecuteScalar();

                    Trans.Commit();
                    MessageBox.Show("Data " + txtDeliveryMethod.Text + ", berhasil diupdate.");
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
                if (f.Text == "Delivery Method")
                    Parent.RefreshGrid();
        }

        //created by Thaddaeus Matthias, 20 March 2018, BEGIN
        private void modeAfterSave()
        {
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
            txtDeliveryMethod.Enabled = false;
            txtDeskripsi.Enabled = false;
            rdBtnFranco.Enabled = false;
            rdBtnLoco.Enabled = false;
            Mode = "Edit";
        }
        //END ===============================================

    }
}
