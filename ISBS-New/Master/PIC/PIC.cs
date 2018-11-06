using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.PIC
{
    public partial class PIC : Form
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        static int billno = 0;
        string code;
        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String PICId = null;
        Master.PIC.DashPIC Parent;

        public PIC()
        {
            InitializeComponent();
            addCmbUserId();
        }

        public void flag(String picId, String mode)
        {
            PICId = picId;
            Mode = mode;
        }


        private void PIC_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
                billno = billno + 1;
                txtUserId.Text = billno.ToString();
            }
            else
            {
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [Master].[PIC] Where PICId = '" + PICId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtPICId.Text = PICId.ToString();

                    txtPICName.Text = Dr["PICName"].ToString();
                    txtUserId.Text = Dr["UserId"].ToString();
                    cmbUserId.Text = Dr["UserId"].ToString();
                }
            }
            Conn.Close();



            //lblTotal.Text = "Total Rows : " + Total.ToString();
            //Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            //lblPage.Text = "/ " + Page2;
        }

        private void ModeLoad()
        {
            Mode = "Edit";

            btnSave.Visible = false;
            btnCancel.Visible = false;

            RefreshGrid();
        }

        private void ModeNew()
        {
            txtPICId.Enabled = true;
            txtPICName.Enabled = true;
            txtUserId.Enabled = true;
            cmbUserId.Enabled = true;


            btnEdit.Visible = false;


            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtPICId.Enabled = false;
            txtPICName.Enabled = true;
            txtUserId.Enabled = true;
            cmbUserId.Enabled = true;


            btnSave.Visible = true;
            btnCancel.Visible = true;
            btnEdit.Visible = false;
            btnExit.Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPICId.Enabled = false;
            txtPICName.Enabled = false;
            txtUserId.Enabled = false;
            cmbUserId.Enabled = false;


            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPICId.Text.Equals(""))
                {
                    MessageBox.Show("PIC Id masih kosong, silahkan diisi terlebih dahulu.");
                }

                if (Mode.Equals("New"))
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    try
                    {
                        Query = "insert into [Master].[PIC] (PICId, PICName, UserId, UserId) ";
                        Query += "values ('" + txtPICId.Text + "', '" + txtPICName.Text + "', '" + txtUserId.Text + "','" + cmbUserId.Text + "');";
                        //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.Vendor','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                        using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        Trans.Rollback();
                        return;
                    }
                    Trans.Commit();
                    Conn.Close();
                    MessageBox.Show("Data PIC Name = " + txtPICName.Text + ", berhasil ditambahkan.");
                }
                if (Mode.Equals("Edit"))
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    try
                    {
                        Query = "update [Master].[PIC] set PICName='" + txtPICName.Text + "', UserId='" + txtUserId.Text + "', UserId='" + cmbUserId.Text + "' where PICId='" + txtPICId.Text + "';";
                        //Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.Vendor','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";

                        using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception)
                    {
                        Trans.Rollback();
                        return;
                    }
                    Trans.Commit();
                    Conn.Close();
                    MessageBox.Show("Data PIC Id = " + txtPICId.Text + ", berhasil diupdate.");
                }

                this.Close();
                Parent.RefreshGrid();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void ParentRefreshGrid(Master.PIC.DashPIC pic)
        {
            Parent = pic;
        }

        private void addCmbUserId()
        {

            Conn = ConnectionString.GetConnection();
            Query = "Select UserId From [User].[User] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbUserId.Items.Add(Dr[0]);
            }
            Conn.Close();
        }

        private void cmbUserId_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        

    }
}
