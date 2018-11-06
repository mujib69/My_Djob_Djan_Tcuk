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
    public partial class FormFQA : MetroFramework.Forms.MetroForm
    {
        GlobalInquiry Parent;
        SqlConnection Con;
        SqlDataReader Dr;
        TransactionScope Scope;
        SqlCommand Cmd;

        string Mode = "";
        string SelectedId = "";
        string Query = "";

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FormFQA()
        {
            InitializeComponent();
        }

        public void SetParent(GlobalInquiry Form)
        {
            Parent = Form;
        }

        public void SetMode(string PassedMode, string PassedId)
        {
            Mode = PassedMode;
            SelectedId = PassedId;
        }

        private void ModeNew()
        {
            Mode = "New";
            btnInActive.Enabled = false;
            btnExit.Enabled = true;
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
            btnSave.Enabled = true;
            btnSearchCOA.Enabled = true;

            txtFQADesc.ReadOnly = false;
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            btnInActive.Enabled = false;
            btnExit.Enabled = false;
            btnEdit.Enabled = false;
            btnCancel.Enabled = true;
            btnSave.Enabled = true;
            btnSearchCOA.Enabled = false;

            txtFQADesc.ReadOnly = false;
        }

        private void ModeView()
        {
            Mode = "View";
            btnInActive.Enabled = true;
            btnExit.Enabled = true;
            btnEdit.Enabled = true;
            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnSearchCOA.Enabled = false;

            txtFQADesc.ReadOnly = true;

            GetHeader();
        }

        private void FormFQA_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "View" || Mode == "BeforeEdit")
            {
                ModeView();
            }
        }

        private string Validation()
        {
            string msg = "";
            Query = "SELECT * FROM [M_FQA1] WHERE [FQA1_Desc] = @FQA1_Desc ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@FQA1_Desc", txtFQADesc.Text);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    msg += "-FQA Desc tersebut sudah ada.\n\r";
                }
                Dr.Close();
            }
            if (txtCOAID.Text == "" || txtCOAID.Text == null)
            {
                msg += "-Pilih COA-Id terlebih dahulu.\n\r";
            }
            if (txtFQADesc.Text =="" || txtFQADesc.Text == null)
            {
                msg += "-Isi Deskripsi FQA1 terlebih dahulu.\n\r";
            }
            if (GenerateFQAID(ConnectionString.GetConnection()) == "")
            {
                msg += "-Untuk COA-Id "+txtCOAID.Text+" sequence sudah melebihi nilai 999.\n\r";
            }
            return msg;
        }

        private void GetHeader()
        {
            if (SelectedId != null && SelectedId != "")
            {
                txtFQAID.Text = SelectedId;
            }
            Query = "SELECT * FROM [dbo].[M_FQA1] WHERE [FQA1_ID] = @FQA1_ID ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@FQA1_ID",txtFQAID.Text);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtCOAID.Text = Dr["COA_ID"].ToString();
                    txtFQAID.Text = Dr["FQA1_ID"].ToString(); ;
                    txtFQADesc.Text = Dr["FQA1_Desc"].ToString();
                    chkInActive.Checked = Dr["Status"].ToString()=="Gunakan" ? true : false;
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

        private void InsertFQA(SqlConnection con)
        {
            string FQASeqNo = GenerateFQAID(con);
            string FQAID = txtCOAID.Text+"."+FQASeqNo;
            Query = "INSERT INTO [M_FQA1] ([FQA1_ID],[FQA1_SeqNo],[FQA1_Desc],[Status],[COA_ID],[CreatedDate],[CreatedBy]) VALUES (@FQA1_ID,@FQA1_SeqNo,@FQA1_Desc,@Status,@COA_ID,@CreatedDate,@CreatedBy)";
            using(Cmd = new SqlCommand(Query,con))
            {
                Cmd.Parameters.AddWithValue("@FQA1_ID", FQAID);
                Cmd.Parameters.AddWithValue("@FQA1_SeqNo", FQASeqNo);
                Cmd.Parameters.AddWithValue("@FQA1_Desc", txtFQADesc.Text);
                Cmd.Parameters.AddWithValue("@Status","Gunakan");
                Cmd.Parameters.AddWithValue("@COA_ID",txtCOAID.Text);
                Cmd.Parameters.AddWithValue("@CreatedDate",DateTime.Now);
                Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
            txtFQAID.Text = FQAID;
    
        }

        private void UpdateFQA(SqlConnection con)
        {
            Query = "UPDATE [M_FQA1] SET [FQA1_Desc]=@FQA1_Desc,UpdatedDate=@UpdatedDate, UpdatedBy = @UpdatedBy WHERE [FQA1_ID] = @FQA1_ID";
            using (Cmd = new SqlCommand(Query, con))
            {
                Cmd.Parameters.AddWithValue("@FQA1_ID", txtFQAID.Text);
                Cmd.Parameters.AddWithValue("@FQA1_Desc", txtFQADesc.Text);
                Cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                Cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }

        }

        private string GenerateFQAID(SqlConnection con)
        {
            string FQASeq = "001";
            Query = "SELECT MAX(CAST(FQA1_SeqNo AS int)) as fqaid FROM [M_FQA1] WHERE [COA_ID] = @COA_ID ";
            using (Cmd = new SqlCommand(Query, con))
            {
                Cmd.Parameters.AddWithValue("@COA_ID", txtCOAID.Text);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        if (Dr["fqaid"] != System.DBNull.Value)
                        {
                            if (Convert.ToInt32(Dr["fqaid"]) + 1 > 999)
                            {
                                FQASeq = "";
                            }
                            else
                            {
                                FQASeq = (Convert.ToInt32(Dr["fqaid"]) + 1).ToString("D3");
                            }
                        }
                    }
                    Dr.Close();
                }
            }
            
            return FQASeq;
        }

        private void btnInActive_Click(object sender, EventArgs e)
        {
            if (chkInActive.Checked == true)
            {
                Query = "UPDATE [M_FQA1] SET Status=@Status WHERE [FQA1_ID] = @FQA1_ID";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@FQA1_ID", txtFQAID.Text);
                    Cmd.Parameters.AddWithValue("@Status", "Batal");
                    Cmd.ExecuteNonQuery();
                }
            }
            else
            {
                Query = "UPDATE [M_FQA1] SET Status=@Status WHERE [FQA1_ID] = @FQA1_ID";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@FQA1_ID", txtFQAID.Text);
                    Cmd.Parameters.AddWithValue("@Status", "Gunakan");
                    Cmd.ExecuteNonQuery();
                }
            }
            ModeView();
            if (Parent.Visible == true)
            {
                Parent.RefreshGrid();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            bool Status = true;
            Query = "SELECT * FROM [dbo].[M_FQA2] WHERE [FQA1_ID] = @FQA1_ID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@FQA1_ID", txtFQAID.Text);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    Status = false;
                }
                Dr.Close();
            }
            if (Status == true)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show("FQA 1 Sudah tidak dapat di-edit karena sudah dibuat FQA 2 nya.");
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            string msg = Validation();
            if (msg != "")
            {
                MetroFramework.MetroMessageBox.Show(this,msg);
                return;
            }
            try
            {
                Con = ConnectionString.GetConnection();
                using (Scope = new TransactionScope())
                {
                    if (Mode == "New")
                    {
                        InsertFQA(Con);
                        MessageBox.Show("FQA2 Berhasil di simpan.");
                    }
                    else if (Mode == "Edit")
                    {
                        UpdateFQA(Con);
                        MessageBox.Show("FQA2 Berhasil di update.");
                    }
                    Scope.Complete();
                }
                Con.Close();
                ModeView();
                if (Parent.Visible == true)
                {
                    Parent.RefreshGrid();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
            finally { }
        }

        private void btnSearchCOA_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "M_COA", "and a.Status='Gunakan'", "a.*", "M_COA a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                txtCOAID.Text = SearchV2.data[0];
            }
        }
    }
}
