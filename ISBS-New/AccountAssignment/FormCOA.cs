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
    public partial class FormCOA : MetroFramework.Forms.MetroForm
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
        int MCOAID;
        string MCOAType;

        string vOldCoaID, vOldCoaDesc;

        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FormCOA()
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
            txtCOAID.MaxLength = 4;
            txtStartCoa.MaxLength = 3;
            txtEndCoa.MaxLength = 3;

            chkInActive.Enabled = false;
        }

        private void ModeNew()
        {
            Mode = "New";
            txtMCOADeskripsi.Enabled = true;
            txtCOAID.Enabled = true;
            txtStartCoa.Enabled = true;
            txtEndCoa.Enabled = true;
            txtCOADeskripsi.Enabled = true;

            btnSearchMCOA.Enabled = true;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnInActive.Enabled = false;
        }

        private void ModeView()
        {
            Mode = "View";
            txtMCOADeskripsi.Enabled = false;
            txtCOAID.Enabled = false;
            txtStartCoa.Enabled = false;
            txtEndCoa.Enabled = false;
            txtCOADeskripsi.Enabled = false;

            btnSearchMCOA.Enabled = false;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnInActive.Enabled = true;

            GetHeader();
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            txtMCOADeskripsi.Enabled = false;
            txtCOAID.Enabled = true;
            txtStartCoa.Enabled = true;
            txtEndCoa.Enabled = true;
            txtCOADeskripsi.Enabled = true;

            btnSearchMCOA.Enabled = false;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnExit.Enabled = true;
            btnInActive.Enabled = false;
        }

        private void txtStartFQA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }    
        }

        private void txtEndFQA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            } 
        }

        private Boolean CheckAvailability()
        {
            Boolean vBol = true;

            if (Mode == "New")
            {
                Query = "SELECT * FROM [M_COA] WHERE [COA_ID] = @COA_ID";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@COA_ID", Convert.ToInt32(txtCOAID.Text).ToString("D4"));
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        MessageBox.Show("COA ID tersebut sudah ada.");
                        vBol = false;
                        return vBol;
                    }                    
                    Dr.Close();
                }

                Query = "SELECT * FROM [M_COA] WHERE [COA_Desc] = @COA_Desc ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@COA_Desc", txtCOADeskripsi.Text);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        MessageBox.Show("COA Desc tersebut sudah ada.");
                        vBol = false;
                        return vBol;
                    }               
                    Dr.Close();
                }
            }

            if (Mode == "Edit")
            {
                if (txtCOAID.Text == vOldCoaID && txtCOADeskripsi.Text == vOldCoaDesc)
                {
                    vBol = true;
                    return vBol;
                }

                else
                {
                    if (txtCOAID.Text == vOldCoaID)
                    {
                        Query = "SELECT * FROM [M_COA] WHERE [COA_Desc] = @COA_Desc ";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@COA_Desc", txtCOADeskripsi.Text);
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                MessageBox.Show("COA Desc tersebut sudah ada.");
                                vBol = false;
                                return vBol;
                            }
                            Dr.Close();
                        }
                    }

                    if (txtCOADeskripsi.Text == vOldCoaDesc)
                    {
                        Query = "SELECT * FROM [M_COA] WHERE [COA_ID] = @COA_ID";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@COA_ID", Convert.ToInt32(txtCOAID.Text).ToString("D4"));
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                MessageBox.Show("COA ID tersebut sudah ada.");
                                vBol = false;
                                return vBol;
                            }
                            Dr.Close();
                        }
                    }
                }
            }
            return vBol;
        }

        private Boolean Validasi()
        {
            int StartLimit = 0;
            int EndLimit = 1;
            int COAId = Convert.ToInt32(txtCOAID.Text);
            Query = "SELECT * FROM [M_COAMaster] WHERE [M_COAID] = @M_COAID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@M_COAID",MCOAID);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    StartLimit = Convert.ToInt32(Dr["StartRangeCOA"]);
                    EndLimit = Convert.ToInt32(Dr["EndRangeCOA"]);
                }
                Dr.Close();
            }            

            if (txtCOADeskripsi.Text == "")
            {
                MessageBox.Show("COA Deskripsi harus diisi");
                return false;
            }
            else if (txtCOAID.Text.Trim() == "")
            {
                MessageBox.Show("COA Id harus diisi");
                return false;
            }
            else if(COAId > EndLimit)
            {
                MessageBox.Show("COA Id tidak boleh melebihi End Range.");
                return false;
            }
            else if (COAId < StartLimit)
            {
                MessageBox.Show("COA Id tidak boleh kurang dari Start Range.");
                return false;
            }
            else if (!CheckAvailability())
            {
                return false;
            }
            else
                return true;
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
                    Query = "INSERT INTO [dbo].[M_COA] ([COA_ID],[COA_Desc],[M_COAID],[M_COAType],[M_COADesc],[Status],[CreatedDate],[CreatedBy]) values ";
                    Query += "(@COA_ID, @COA_Desc, @M_COAID, @M_COAType, @M_COADesc, @Status, @CreatedDate, @CreatedBy)";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@COA_ID", Convert.ToInt32(txtCOAID.Text).ToString("D4"));
                        Cmd.Parameters.AddWithValue("@COA_Desc", txtCOADeskripsi.Text);
                        Cmd.Parameters.AddWithValue("@M_COAID", MCOAID);
                        Cmd.Parameters.AddWithValue("@M_COAType", MCOAType);
                        Cmd.Parameters.AddWithValue("@M_COADesc", txtMCOADeskripsi.Text);
                        Cmd.Parameters.AddWithValue("@Status", "Gunakan");
                        Cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("COA ID " + txtCOAID.Text + " berhasil dibuat");
                }
                if (Mode == "Edit")
                {
                    Query = "UPDATE [dbo].[M_COA] SET [COA_ID]=@newCoaId,[COA_Desc]=@COA_Desc,[UpdatedDate]=@UpdatedDate,[UpdatedBy]=@UpdatedBy WHERE [COA_ID]=@oldCoaId";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@newCoaId", txtCOAID.Text);
                        Cmd.Parameters.AddWithValue("@COA_Desc", txtCOADeskripsi.Text);
                        Cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@oldCoaId", vOldCoaID);

                        Cmd.ExecuteNonQuery();
                    }
                    Trans.Commit();
                    MessageBox.Show("COA ID " + txtCOAID.Text + " berhasil diupdate");
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
            SelectedId = txtCOAID.Text;
            Form parentform = Application.OpenForms["GlobalInquiry"];
            if (parentform != null)
                Parent.RefreshGrid();
        }

        private void GetHeader()
        {
            if (SelectedId != null && SelectedId != "")
            {
                txtCOAID.Text = SelectedId;
            }
            
            Query = "SELECT * FROM [dbo].[M_COA] WHERE [COA_ID] = @COA_ID ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@COA_ID", txtCOAID.Text);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtCOADeskripsi.Text = Dr["COA_Desc"].ToString();
                    chkInActive.Checked = Dr["Status"].ToString()=="Gunakan" ? true : false;
                    MCOAID = Convert.ToInt32(Dr["M_COAID"]);
                }
                Dr.Close();
            }
            Query = "SELECT * FROM [dbo].[M_COAMaster] WHERE [M_COAID] = @M_COAID ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@M_COAID", MCOAID);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtMCOADeskripsi.Text = Dr["M_COADesc"].ToString();
                    txtStartCoa.Text = Dr["StartRangeCOA"].ToString();
                    txtEndCoa.Text = Dr["EndRangeCOA"].ToString();
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
                Query = "UPDATE [M_COA] SET Status=@Status WHERE [COA_ID] = @COA_ID";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@COA_ID", txtCOAID.Text);
                    Cmd.Parameters.AddWithValue("@Status", "Batal");
                    Cmd.ExecuteNonQuery();
                }
            }
            else
            {
                Query = "UPDATE [M_COA] SET Status=@Status WHERE [COA_ID] = @COA_ID";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@COA_ID", txtCOAID.Text);
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

        private Boolean CheckUsage()
        {
            bool vBol = true;

            Query = "SELECT * FROM [dbo].[M_FQA1] WHERE [COA_ID] = @COA_ID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@COA_ID", txtCOAID.Text);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    vBol = false;
                    MessageBox.Show("COA Sudah tidak dapat di-edit karena sudah dibuat FQA1 nya.");
                }
                Dr.Close();
            }

            return vBol;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!CheckUsage())
                return;

            vOldCoaID = txtCOAID.Text;
            vOldCoaDesc = txtCOADeskripsi.Text;
            ModeEdit();
        }

        private void txtCOAID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSearchMCOA_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "M_COAMaster", "and a.Status='Gunakan' ", "a.*", "M_COAMaster a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                MCOAID = Convert.ToInt32(SearchV2.data[9]);
                MCOAType = SearchV2.data[1].ToString();
                txtMCOADeskripsi.Text = SearchV2.data[0];
                txtStartCoa.Text = SearchV2.data[2];
                txtEndCoa.Text = SearchV2.data[3];
            }
        }

        private void txtCOAID_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }
    }
}
