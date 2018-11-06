using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;

namespace ISBS_New.AccountAssignment
{
    public partial class FormCOAMaster : MetroFramework.Forms.MetroForm
    {
        SqlConnection Conn;
        SqlCommand Cmd;
        SqlDataReader Dr;
        private SqlTransaction Trans;
        TransactionScope Scope;
        string Query;

        string Mode;
        string SelectedId;
        string RefId;

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FormCOAMaster()
        {
            InitializeComponent();
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            SelectedId = id;
        }

        private void FormCOA_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "View" || Mode == "BeforeEdit")
            {
                ModeView();
            }
        }

        private void ModeLoad()
        {
            cmbCOAMasterType.Items.Clear();
            cmbCOAMasterType.Items.Add("D");
            cmbCOAMasterType.Items.Add("K");

            txtCOAMasterDesk.MaxLength = 50;
            txtStartCOA.MaxLength = 4;
            txtEndCOA.MaxLength = 4;

            chkInActive.Enabled = false;
        }

        private void ModeNew()
        {
            Mode = "New";
            txtCOAMasterDesk.Enabled = true;
            cmbCOAMasterType.Enabled = true;
            txtStartCOA.Enabled = true;
            txtEndCOA.Enabled = true;

            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnInActive.Enabled = false;
        }

        private void ModeView()
        {
            Mode = "View";
            txtCOAMasterDesk.Enabled = false;
            cmbCOAMasterType.Enabled = false;
            txtStartCOA.Enabled = false;
            txtEndCOA.Enabled = false;

            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnInActive.Enabled = true;

            GetHeader();
        }

        string Deskripsi;
        private void ModeEdit()
        {
            Mode = "Edit";
            txtCOAMasterDesk.Enabled = true;
            cmbCOAMasterType.Enabled = true;
            txtStartCOA.Enabled = false;
            txtEndCOA.Enabled = false;

            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnExit.Enabled = true;
            btnInActive.Enabled = false;            
        }

        private Boolean Validasi()
        {          
            if (txtStartCOA.Text.Trim() == "")
            {
                MessageBox.Show("Start COA harus diisi");
                return false;
            }
            else if (txtEndCOA.Text.Trim() == "")
            {
                MessageBox.Show("End COA harus diisi");
                return false;
            }
            else if (Convert.ToDecimal(txtStartCOA.Text) >= Convert.ToDecimal(txtEndCOA.Text))
            {
                MessageBox.Show("End COA harus lebih besar dari Start COA");
                return false;
            }
            else if (txtCOAMasterDesk.Text.Trim() == "")
            {
                MessageBox.Show("Master COA Name harus diisi");
                return false;
            }
            else if (!CheckAvailability())
            {
                return false;
            }
            else if (cmbCOAMasterType.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih COA Type");
                return false;
            }
            else
                return true;
        }

        private Boolean CheckRange()
        {
            Decimal V1 = Convert.ToDecimal(txtStartCOA.Text);
            Decimal V2 = Convert.ToDecimal(txtEndCOA.Text);
            Boolean vBol = false;
            Query = "SELECT * FROM [dbo].[M_COAMaster] WHERE ";
            Query += "(" + V1 + " BETWEEN StartRangeCOA AND EndRangeCOA) OR ";
            Query += "(" + V2 + " BETWEEN StartRangeCOA AND EndRangeCOA) OR ";
            Query += "(StartRangeCOA BETWEEN " + V1 + " AND " + V2 + ") OR ";
            Query += "(EndRangeCOA BETWEEN " + V1 + " AND " + V2 + ")";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    MessageBox.Show("Range sudah dipakai");
                    vBol = false;
                }
                else
                    vBol = true;
                Dr.Close();
            }

            return vBol;
        }

        private Boolean CheckAvailability()
        {
            Boolean vBol = false;

            if (Mode == "New")
            {
                Query = "SELECT * FROM [dbo].[M_COAMaster] WHERE [M_COADesc] = @deskripsi ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@deskripsi", txtCOAMasterDesk.Text.Trim());
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        MessageBox.Show("Deskripsi sudah dipakai");
                        vBol = false;
                    }
                    else
                    {
                        vBol = true;
                    }
                    Dr.Close();
                }
            }
            else if (Mode == "Edit")
            {
                if (txtCOAMasterDesk.Text.Trim() == Deskripsi)
                    vBol = true;
                else
                {
                    Query = "SELECT * FROM [dbo].[M_COAMaster] WHERE [M_COADesc] = @deskripsi ";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@deskripsi", txtCOAMasterDesk.Text.Trim());
                        SqlDataReader Dr2 = Cmd.ExecuteReader();
                        if (Dr2.HasRows)
                        {
                            MessageBox.Show("Deskripsi sudah dipakai");
                            vBol = false;
                        }
                        else
                        {
                            vBol = true;
                        }
                        Dr2.Close();
                    }
                }
            }
            return vBol;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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
                    if (!CheckRange())
                        return;

                    Query = "INSERT INTO [dbo].[M_COAMaster] ([M_COADesc],[M_COAType],[StartRangeCOA],[EndRangeCOA],[Status],[CreatedDate],[CreatedBy]) values ";
                    Query += "(@mcoaname, @mcoatype, @startcoa, @endcoa, @status, @date, @UserId)";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@mcoaname", txtCOAMasterDesk.Text.Trim());
                        Cmd.Parameters.AddWithValue("@mcoatype", cmbCOAMasterType.Text);
                        Cmd.Parameters.AddWithValue("@startcoa", txtStartCOA.Text.Trim());
                        Cmd.Parameters.AddWithValue("@endcoa", txtEndCOA.Text.Trim());
                        Cmd.Parameters.AddWithValue("@status", "Gunakan");
                        Cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();                    
                    MessageBox.Show(txtCOAMasterDesk.Text + " berhasil dibuat");
                }
                if (Mode == "Edit")
                {
                    Query = "UPDATE [dbo].[M_COAMaster] SET [M_COADesc]=@mcoaname,[M_COAType]=@mcoatype,[StartRangeCOA]=@startcoa,[EndRangeCOA]=@endcoa,[UpdatedDate]=@date,[UpdatedBy]=@UserId WHERE [M_COAID]=@mcoaid";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@mcoaname", txtCOAMasterDesk.Text.Trim());
                        Cmd.Parameters.AddWithValue("@mcoatype", cmbCOAMasterType.Text);
                        Cmd.Parameters.AddWithValue("@startcoa", txtStartCOA.Text.Trim());
                        Cmd.Parameters.AddWithValue("@endcoa", txtEndCOA.Text.Trim());
                        Cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@mcoaid", SelectedId);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show(txtCOAMasterDesk.Text + " berhasil diupdate");
                }
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
                return;
            }
            finally { }
            ModeView();
            GetHeader();
            getNewId();
            Form parentform = Application.OpenForms["GlobalInquiry"];
            if (parentform != null)
                Parent.RefreshGrid();
        }

        private void GetHeader()
        {
            Query = "SELECT * FROM [dbo].[M_COAMaster] WHERE [M_COAID] = @mcoaid ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@mcoaid", SelectedId);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtCOAMasterDesk.Text = Dr["M_COADesc"].ToString();
                    cmbCOAMasterType.SelectedItem = Dr["M_COAType"].ToString();
                    txtStartCOA.Text = Dr["StartRangeCOA"].ToString();
                    txtEndCOA.Text = Dr["EndRangeCOA"].ToString();
                    chkInActive.Checked = Dr["Status"].ToString() == "Gunakan" ? true : false;
                }
                Dr.Close();
            }
            if (chkInActive.Checked == true)
            {
                btnInActive.Text = "Batal";
            }
            else if (chkInActive.Checked == false)
            {
                btnInActive.Text = "Gunakan";
            }
        }

        private void btnInActive_Click(object sender, EventArgs e)
        {
            if (chkInActive.Checked == true)
            {
                Query = "UPDATE [dbo].[M_COAMaster] SET Status=@Status WHERE [M_COAID] = @mcoaid";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@mcoaid", SelectedId);
                    Cmd.Parameters.AddWithValue("@Status", "Batal");
                    Cmd.ExecuteNonQuery();
                }
            }
            else
            {
                Query = "UPDATE [dbo].[M_COAMaster] SET Status=@Status WHERE [M_COAID] = @mcoaid";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@mcoaid", SelectedId);
                    Cmd.Parameters.AddWithValue("@Status", "Gunakan");
                    Cmd.ExecuteNonQuery();
                }
            }
            ModeView();
            GetHeader();
            Form parentform = Application.OpenForms["GlobalInquiry"];
            if (parentform != null)
                Parent.RefreshGrid();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeView();
            GetHeader();            
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!CheckUsage())
                return;

            Deskripsi = txtCOAMasterDesk.Text.Trim();            
            ModeEdit();            
        }

        private void getNewId()
        {
            Query = "SELECT [M_COAID] FROM [dbo].[M_COAMaster] WHERE [M_COADesc] = @mcoadesc";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@mcoadesc", txtCOAMasterDesk.Text);
                SelectedId = Cmd.ExecuteScalar().ToString();
            }
        }

        private Boolean CheckUsage()
        {
            Boolean vBol = false;
            Query = "SELECT * FROM [dbo].[M_COA] WHERE [M_COAID] = @mcoaid";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@mcoaid", SelectedId);

                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    MessageBox.Show("Tidak bisa di-edit karena Master COA ID sudah dipakai");
                    vBol = false;
                }
                else
                    vBol = true;
                Dr.Close();
            }

            return vBol;
        }

        private void txtStartCOA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }    
        }

        private void txtEndCOA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }    
        }
    }
}
