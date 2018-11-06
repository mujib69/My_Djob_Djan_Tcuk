using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Payment_Mode
{
    public partial class PaymentMode : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query , Message;
        string RefId;

        int Limit1, Limit2, Total, Page1, Page2, Index;
        String PaymentModeId = null;
        string vOldName;

        

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        //Master.Payment_Mode.InqPaymentMode Parent;
        //public void setParent(Master.Payment_Mode.InqPaymentMode F)
        //{
        //    Parent = F;
        //}

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public PaymentMode()
        {
            InitializeComponent();
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            PaymentModeId = id;
        }

        public void flag(String paymentmodeid, String mode)
        {
            PaymentModeId = paymentmodeid;
            Mode = mode;
        }

        private void PaymentMode_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String PaymentModeId)
        {
            Query = "Select * From [dbo].[PaymentMode] Where PaymentModeId = @paymentid";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@paymentid", PaymentModeId);
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

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[PaymentMode] Where PaymentModeId = @paymentid";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@paymentid", PaymentModeId);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtPaymentModeId.Text = Dr["PaymentModeId"].ToString(); ;
                    txtPaymentModeName.Text = Dr["PaymentModeName"].ToString();
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
            txtPaymentModeId.Text = "";
            txtPaymentModeName.Text = "";
        }

        private void ModeNew()
        {
            Mode = "New";
            resetText();

            txtPaymentModeId.Enabled = true;
            txtPaymentModeName.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            txtPaymentModeId.Enabled = false;
            txtPaymentModeName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
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
            if (txtPaymentModeId.Text.Trim() == "" || txtPaymentModeName.Text.Trim() == "")
            {
                MessageBox.Show("Data harus diisi");
                return false;
            }
            else if (Mode == "New" && cekValidasi(txtPaymentModeId.Text) == false)
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
                    Query = "insert into [dbo].[PaymentMode] (PaymentModeId, PaymentModeName, CreatedBy, CreatedDate) ";
                    Query += "values ( @paymentid , @paymentname , @UserId, @date);";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@paymentid", txtPaymentModeId.Text);
                        Cmd.Parameters.AddWithValue("@paymentname", txtPaymentModeName.Text);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString());
                        Cmd.ExecuteNonQuery();
                        Message = "Data " + txtPaymentModeId.Text + ", berhasil ditambahkan.";
                    }
                }
                if (Mode == "Edit")
                {
                    Query = "update [dbo].[PaymentMode] set PaymentModeName = @paymentname, UpdatedBy = @UserId, UpdatedDate = @date where PaymentModeId = @paymentid";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.Parameters.AddWithValue("@paymentid", txtPaymentModeId.Text);
                    Cmd.Parameters.AddWithValue("@paymentname", txtPaymentModeName.Text);
                    Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                    Cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString());
                    Cmd.ExecuteScalar();
                    Message = "Data " + txtPaymentModeId.Text + ", berhasil diupdate.";
                }
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            finally
            {
                Trans.Commit();                
                Conn.Close();
                MessageBox.Show(Message);
                modeAfterSave();


                Form f = Application.OpenForms["GlobalInquiry"];
                if (f != null)
                    if (f.Text == "Payment Mode")
                        Parent.RefreshGrid();
            }           
        }
        

        //created by Thaddaeus Matthias, 19 March 2018, BEGIN
        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = false;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            RefreshGrid();
            txtPaymentModeName.Enabled = false;
        }

        private void modeAfterSave()
        {
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;
            txtPaymentModeId.Enabled = false;
            txtPaymentModeName.Enabled = false;
            RefreshGrid();
            Mode = "Edit";
        }
        //END================================================
    }
}
