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

namespace ISBS_New.AccountAssignment.GLJournal
{
    public partial class FormGLJournalHeader : MetroFramework.Forms.MetroForm
    {
        //basic object
        SqlConnection Con;
        SqlDataReader Dr;
        TransactionScope Scope;
        SqlCommand Cmd;

        //control the grid// function in setColumnConfig() 
        string[] TableColHeaderText = { "No", "GLJournalHID", "SeqNo","JournalHID","JournalIDSeqNo", "FQAID", "FQADesc","JournalDType","Auto", "Amount","Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
        string[] TableColName = { "No", "GLJournalHID", "SeqNo","JournalHID","JournalIDSeqNo", "FQAID", "FQADesc","JournalDType","Auto", "Amount","Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
        string[] TableColReadOnlyFalse = { "Amount" ,"JournalDType","Notes"};
        string[] TableColVisibleTrue = { "No", "JournalDType", "FQAID", "FQADesc", "Amount", "Notes", "Auto" };
        //Uncomment if want to show all//string[] TableColVisibleTrue = { "No", "GLJournalHID", "SeqNo", "JournalHID", "JournalIDSeqNo", "FQAID", "FQADesc", "JournalDType", "Auto", "Amount", "Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };

        //global variable
        string Query;
        string Mode;
        string ID;

        //tia edit
        ContextMenu Reff = new ContextMenu();
        //tia edit end

        //setting global inquiry as parent form > mainly to refresh parent form
        GlobalInquiry Parent;
        public void SetParent(GlobalInquiry F)
        {
            Parent = F;
        }

        //access setting can be added into sql database
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FormGLJournalHeader()
        {
            InitializeComponent();
        }

        //passed variable from global inquiry, to set reference id and in which mode
        public void SetMode(string PassedMode, string PassedId)
        {
            Mode = PassedMode;
            ID = PassedId;
        }

        //control config when in new mode
        private void ModeNew()
        {
            Mode = "New";
            btnCancel.Enabled = false;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnExit.Enabled = true;
            btnSearchJournalType.Enabled = true;
            txtNotes.ReadOnly = false;

            btnSearchReference.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;
            btnInActive.Enabled = false;
            btnPosting.Enabled = false;

            dgvJournalHeader.ReadOnly = false;
        }

        //control config when in edit mode
        private void ModeEdit()
        {
            Mode = "Edit";
            btnCancel.Enabled = true;
            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnExit.Enabled = false;

            txtNotes.ReadOnly = false;
            btnSearchReference.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;
            btnInActive.Enabled = false;
            btnPosting.Enabled = false;
            for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
            {
                if (dgvJournalHeader.Rows[i].Cells["Auto"].Value.ToString() == "Auto")
                {
                    btnSearchJournalType.Enabled = false;
                    break;
                }
                btnSearchJournalType.Enabled = true;
            }
            dgvJournalHeader.ReadOnly = false;

            SetColumnConfig();
        }

        //control config when in edit mode
        private void ModeView()
        {
            Mode = "View";
            btnCancel.Enabled = false;
            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnExit.Enabled = true;
            btnSearchJournalType.Enabled = false;
            txtNotes.ReadOnly = true;
            btnSearchReference.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;
            btnInActive.Enabled = true;
            btnPosting.Enabled = true;

            dgvJournalHeader.ReadOnly = true;


            SetColumnConfig();
        }

        //get header part of the form (used when opening an already made journal or after saving the new journal)
        private void GetHeader()
        {
            Query = "SELECT * FROM GLJournalH WHERE GLJournalHID = @GLJournalHID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID", ID);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtGLJournalCode.Text = Dr["GLJournalHID"].ToString();
                    chkpost.Checked = Convert.ToBoolean(Dr["Posting"]);
                    btnInActive.Text = Dr["Status"].ToString() == "Gunakan" ? "Inactive" : "Active";
                    txtJournalType.Text = Dr["JournalHID"].ToString();
                    txtReference.Text = Dr["Referensi"].ToString();
                    dtJournalDate.Value = Convert.ToDateTime(Dr["CreatedDate"]);
                }
                Dr.Close();
            }
        }

        //refresh grid just like getheader but for grid
        private void RefreshGrid()
        {
            dgvJournalHeader.Columns.Clear();
            establishedColumnConfig();
            SetColumnConfig();
            int x = 0;
            Query = "SELECT * FROM [dbo].[GLJournalDtl] WHERE [GLJournalHID] = @GLJournalHID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID", txtGLJournalCode.Text);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dgvJournalHeader.Rows.Add(1);
                    for (int i = 0; i < TableColName.Count(); i++)
                    {
                        string tes = TableColName[i].ToString();
                        if (TableColName[i] == "Amount")
                        {
                            dgvJournalHeader.Rows[x].Cells[TableColName[i]].Value = String.Format("{0:#,##0.###0}", (Convert.ToDecimal(Dr[TableColName[i]])));
                        }
                        else if (TableColName[i] == "No")
                        {
                            dgvJournalHeader.Rows[x].Cells[TableColName[i]].Value = dgvJournalHeader.Rows.Count;
                        }
                        else
                        {
                            dgvJournalHeader.Rows[x].Cells[TableColName[i]].Value = Dr[TableColName[i]];
                        }
                    }
                    x++;
                }
                Dr.Close();
            }
            FillCreditAndDebet();
        }

        //validation when save, if every requirement were met
        private string Validation()
        {
            string msg = "";
            if (Convert.ToDateTime(dtRefDate.Value) < Convert.ToDateTime(dtJournalDate.Value))
            {
                msg += "-Tanggal journal tidak boleh sebelum tanggal transaksi reference "+txtReference.Text+".\n\r";
            }
            if (Convert.ToDecimal(txtTotalDebet.Text) != Convert.ToDecimal(txtTotalKredit.Text))
            {
                msg += "-Total Debet dan Kredit harus sama.\n\r";
            }
            if (dgvJournalHeader.Rows.Count <= 0)
            {
                msg += "-Datagrid tidak boleh kosong.\n\r";
            }
            for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
            {
                if (dgvJournalHeader.Rows[i].Cells["JournalDType"].Value == null || dgvJournalHeader.Rows[i].Cells["JournalDType"].Value == System.DBNull.Value)
                {
                    msg += "-Pilih tipe debet atau kredit pada row " + (i + 1) + ".\n\r";
                }
                else if (dgvJournalHeader.Rows[i].Cells["JournalDType"].Value.ToString() == "")
                {
                    msg += "-Pilih tipe debet atau kredit pada row "+(i+1)+".\n\r";
                }
            }
            return msg;
        }

        //insert into database
        private void InsertJournalDtl(SqlConnection con)
        {
            for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
            {
                if (dgvJournalHeader.Rows[i].Cells["Auto"].Value.ToString() == "Manual")
                {
                    Query = "INSERT INTO [dbo].[GLJournalDtl] ([GLJournalHID],[SeqNo],[JournalHID],[JournalDType],[FQAID],[FQADesc],[Amount],[Notes],[JournalIDSeqNo],[Auto],[CreatedDate],[CreatedBy]) VALUES(@GLJournalHID,@SeqNo,@JournalHID,@JournalDType,@FQAID,@FQADesc,@Amount,@Notes,@JournalIDSeqNo,@Auto,@CreatedDate,@CreatedBy)";
                    using (Cmd = new SqlCommand(Query, con))
                    {
                        Cmd.Parameters.AddWithValue("@GLJournalHID", ID);
                        Cmd.Parameters.AddWithValue("@SeqNo", Convert.ToInt32(dgvJournalHeader.Rows[i].Cells["SeqNo"].Value));
                        Cmd.Parameters.AddWithValue("@JournalHID", dgvJournalHeader.Rows[i].Cells["JournalHID"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@JournalDType", dgvJournalHeader.Rows[i].Cells["JournalDType"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@FQAID", dgvJournalHeader.Rows[i].Cells["FQAID"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@FQADesc", dgvJournalHeader.Rows[i].Cells["FQADesc"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(dgvJournalHeader.Rows[i].Cells["Amount"].Value));
                        Cmd.Parameters.AddWithValue("@Notes", dgvJournalHeader.Rows[i].Cells["Notes"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@Auto", dgvJournalHeader.Rows[i].Cells["Auto"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@JournalIDSeqNo", dgvJournalHeader.Rows[i].Cells["JournalIDSeqNo"].Value);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void InsertJournalH(SqlConnection con)
        {
            Query = "INSERT INTO [dbo].[GLJournalH] ([GLJournalHID],[JournalHID],[Notes],[Status],[Posting],[CreatedDate],[CreatedBy],[Referensi],[PostingDate]) VALUES(@GLJournalHID,@JournalHID,@Notes,@Status,@Posting,@CreatedDate,@CreatedBy,@Referensi,@PostingDate)";
            using (Cmd = new SqlCommand(Query, con))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID", ID);
                Cmd.Parameters.AddWithValue("@JournalHID", txtJournalType.Text);
                Cmd.Parameters.AddWithValue("@Referensi", txtReference.Text);
                Cmd.Parameters.AddWithValue("@Status", "Gunakan");
                Cmd.Parameters.AddWithValue("@Posting", 0);
                Cmd.Parameters.AddWithValue("@Notes", txtNotes.Text);
                Cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@PostingDate", DateTime.Now);
                Cmd.ExecuteNonQuery();
            }
        }

        //Delete from database
        private void DeleteJournalDtl(SqlConnection con)
        {
            Query = "DELETE FROM [dbo].[GLJournalDtl] WHERE [Auto] = @Auto AND GLJournalHID = @GLJournalHID";
            using (Cmd = new SqlCommand(Query, con))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID", txtGLJournalCode.Text);
                Cmd.Parameters.AddWithValue("@Auto","Manual");
                Cmd.ExecuteNonQuery();
            }
        }

        private void FormGLJournalHeader_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
                dtJournalDate.Value = DateTime.Now;
            }
            else if (Mode == "View" || Mode == "BeforeEdit")
            {
                GetHeader();
                RefreshGrid();
                ModeView();
            }
        }

        //Established the column header
        private void establishedColumnConfig()
        {
            if (dgvJournalHeader.Rows.Count < 1)
            {
                //Established column header
                dgvJournalHeader.ColumnCount = TableColName.Count();
                for (int i = 0; i < TableColName.Count(); i++)
                {
                    dgvJournalHeader.Columns[i].Name = TableColName[i];
                    dgvJournalHeader.Columns[i].HeaderText = TableColHeaderText[i];
                }
            }
        }

        //setting the configuration of columns
        private void SetColumnConfig()
        {
            if (dgvJournalHeader.Rows.Count > 0)
            {
                for (int i = 0; i < TableColName.Count(); i++)
                {
                    for (int j = 0; j < TableColVisibleTrue.Count(); j++)
                    {
                        if (dgvJournalHeader.Columns[i].HeaderText == TableColVisibleTrue[j])
                        {
                            dgvJournalHeader.Columns[i].Visible = true;
                            break;
                        }
                        else
                        {
                            dgvJournalHeader.Columns[i].Visible = false;
                        }
                    }
                    for (int j = 0; j < TableColReadOnlyFalse.Count(); j++)
                    {
                        if (Mode == "View" || Mode == "BeforeEdit")
                        {
                            dgvJournalHeader.Columns[i].ReadOnly = true;
                            dgvJournalHeader.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        else
                        {
                            if (dgvJournalHeader.Columns[i].HeaderText == TableColReadOnlyFalse[j])
                            {
                                dgvJournalHeader.Columns[i].ReadOnly = false;
                                dgvJournalHeader.Columns[i].DefaultCellStyle.BackColor = Color.White;
                                break;
                            }
                            else
                            {
                                dgvJournalHeader.Columns[i].ReadOnly = true;
                                dgvJournalHeader.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                            }
                        }
                    }
                }
                for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
                {
                    if (dgvJournalHeader.Rows[i].Cells["Auto"].Value != null)
                    {
                        if (dgvJournalHeader.Rows[i].Cells["Auto"].Value.ToString() == "Manual")
                        {
                            string tipe = dgvJournalHeader.Rows[i].Cells["JournalDType"].Value == null? "":dgvJournalHeader.Rows[i].Cells["JournalDType"].Value.ToString();
                            DataGridViewComboBoxCell cmbType = new DataGridViewComboBoxCell();
                            cmbType.Items.Add("D");
                            cmbType.Items.Add("K");
                            if (tipe == "D")
                            {
                                cmbType.Value = cmbType.Items[0];
                            }
                            else if(tipe == "K")
                            {
                                cmbType.Value = cmbType.Items[1];
                            }
                            dgvJournalHeader.Rows[i].Cells["JournalDType"] = cmbType;
                            if (Mode == "Edit" || Mode == "New")
                            {
                                dgvJournalHeader.Rows[i].Cells["JournalDType"].ReadOnly = false;
                            }
                            else
                            {
                                dgvJournalHeader.Rows[i].Cells["JournalDType"].ReadOnly = true;
                            }
                        }
                        else
                        {
                            dgvJournalHeader.Rows[i].Cells["Amount"].ReadOnly = true;
                            dgvJournalHeader.Rows[i].Cells["Amount"].Style.BackColor = Color.LightGray;
                            dgvJournalHeader.Rows[i].Cells["JournalDType"].ReadOnly = true;
                            dgvJournalHeader.Rows[i].Cells["JournalDType"].Style.BackColor = Color.LightGray;
                        }
                    }
                }
            }
        }

        private void UpdateJournalH(SqlConnection con)
        {
            Query = "UPDATE [dbo].[GLJournalH] SET [PostingDate] = getdate() where [GLJournalHID] = @GLJournalHID";
            using (Cmd = new SqlCommand(Query, con))
            {
                //Cmd.Parameters.AddWithValue("@PostingDate",Convert.ToDateTime(dtPostingDate.Value));
                Cmd.Parameters.AddWithValue("@GLJournalHID",txtGLJournalCode.Text);
                Cmd.ExecuteNonQuery();
            }
        }

        //input value into textbox/datagridview after search
        private void populateGrid()
        {
            int seqno = 1;
            if (txtGLJournalCode.Text != "")
            {
                Query = "SELECT MAX(SeqNo) FROM [dbo].[GLJournalDtl] a WHERE a.[GLJournalHID] = @GLJournalHID ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@GLJournalHID", txtGLJournalCode.Text);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            seqno = Convert.ToInt32(Dr[0]) + 1;
                        }
                    }
                    Dr.Close();
                }
            }

            if (SearchV2.data.Count != 0)
            {
                Query = "SELECT * FROM [M_JournalD] WHERE [JournalHID] = @JournalHID";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@JournalHID", SearchV2.data[0]);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        for (int x = 0; x < dgvJournalHeader.Rows.Count; x++)
                        {
                            if (Convert.ToInt32(dgvJournalHeader.Rows[x].Cells["SeqNo"].Value) >= seqno)
                            {
                                seqno = Convert.ToInt32(dgvJournalHeader.Rows[x].Cells["SeqNo"].Value) + 1;
                            }
                        }
                        dgvJournalHeader.Rows.Add(dgvJournalHeader.Rows.Count + 1, txtGLJournalCode.Text, seqno, SearchV2.data[0], Dr["SeqNo"], Dr["FQA_ID"], Dr["FQA_Desc"], Dr["Type"], "Manual", 0.0000, "", DateTime.Now.ToString("dd/MM/yyyy"), ControlMgr.UserId, "", "");
                    }
                    Dr.Close();
                }
                SearchV2.data.Clear();
            }

            if (Variable.Kode2 != null)
            {
                
                //populate datagridview
                for (int i = 0; i <= Variable.Kode2.GetUpperBound(0); i++)
                {
                    for (int x = 0; x < dgvJournalHeader.Rows.Count; x++)
                    {
                        if (Convert.ToInt32(dgvJournalHeader.Rows[x].Cells["SeqNo"].Value) >= seqno)
                        {
                            seqno = Convert.ToInt32(dgvJournalHeader.Rows[x].Cells["SeqNo"].Value) + 1;
                        }
                    }
                    DataGridViewComboBoxCell cmbType = new DataGridViewComboBoxCell();
                    cmbType.Items.Add("D");
                    cmbType.Items.Add("K");
                    dgvJournalHeader.Rows.Add(dgvJournalHeader.Rows.Count + 1, txtGLJournalCode.Text, seqno, "", "", Variable.Kode2[i, 0], Variable.Kode2[i, 1], cmbType, "Manual", 0.0000, "", DateTime.Now.ToString("dd/MM/yyyy"), ControlMgr.UserId, "", "");
                }
                Variable.Kode2 = null;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Variable.Kode2 = null;
            //search invoice
            string Table = "[dbo].[M_FQA1]";
            string QuerySearch = "SELECT a.* FROM [dbo].[M_FQA1] a WHERE a.[Status] ='Gunakan' ";
            //for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
            //{
            //    QuerySearch += " AND NOT a.[FQA1_ID] = '" + dgvJournalHeader.Rows[i].Cells["FQAID"].Value.ToString() + "' ";
            //}
            QuerySearch += " UNION SELECT b.* FROM [dbo].[M_FQA2] b WHERE b.[Status] ='Gunakan' ";
            //for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
            //{
            //    QuerySearch += " AND NOT b.[FQA2_ID] = '" + dgvJournalHeader.Rows[i].Cells["FQAID"].Value.ToString() + "' ";
            //}
            string[] FilterText = { "FQA1_ID", "FQA1_Desc"};
            string[] Select = { "FQA1_ID", "FQA1_Desc" };
            string PrimaryKey = "FQA1_ID";
            string[] HideField = {  };

            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();

            establishedColumnConfig();
            populateGrid();
            SetColumnConfig();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!(dgvJournalHeader.Rows.Count > 0))
            {
                return;
            }
            string Auto = "Manual";
            Query = "SELECT Auto FROM [dbo].[GLJournalDtl] WHERE [GLJournalHID] = @GLJournalHID AND [SeqNo]=@SeqNo ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID",txtGLJournalCode.Text);
                Cmd.Parameters.AddWithValue("@SeqNo",Convert.ToInt32(dgvJournalHeader.Rows[dgvJournalHeader.CurrentRow.Index].Cells["SeqNo"].Value));
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        Auto = Dr["Auto"].ToString();
                    }
                    Dr.Close();
                }
            }
            if (Auto == "Manual")
            {
                if (dgvJournalHeader.Rows[dgvJournalHeader.CurrentRow.Index].Cells["JournalHID"].Value != null)
                {
                    if (dgvJournalHeader.Rows[dgvJournalHeader.CurrentRow.Index].Cells["JournalHID"].Value.ToString() != "")
                    {
                        RemoveLineWithJournalHID();
                        txtJournalType.Text = "";
                    }
                    else
                    {
                        dgvJournalHeader.Rows.RemoveAt(dgvJournalHeader.CurrentRow.Index);
                    }
                }
                else
                {
                    dgvJournalHeader.Rows.RemoveAt(dgvJournalHeader.CurrentRow.Index);
                }
            }
            else
            {
                MessageBox.Show("Data tidak bisa didelete.");
            }
        }

        private void btnInActive_Click(object sender, EventArgs e)
        {
            string Status = "";
            Query = "SELECT [Status] FROM [GLJournal] WHERE [GLJournalID] = @GLJournalID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalID", txtGLJournalCode.Text);
                Status = Convert.ToString(Cmd.ExecuteScalar());
            }
            switch (Status)
            {
                case "Gunakan":
                    Status = "Batal";
                    break;
                case "Batal":
                    Status = "Gunakan";
                    break;
            }
            Query = "UPDATE [GLJournal] SET Status='" + Status + "' WHERE [GLJournalID] = @GLJournalID ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalID", txtGLJournalCode.Text);
                Cmd.ExecuteNonQuery();
            }
            btnInActive.Text = Status == "Gunakan" ? "Inactive" : "Active";
            RefreshGrid();
            SetColumnConfig();
        }

        private void btnPosting_Click(object sender, EventArgs e)
        {
            string tex = chkpost.Checked == true ? "diunpost" : "dipost";
            DialogResult dialogResult = MessageBox.Show("Journal akan "+tex+"?", "Some Title", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }
            int Post1 = 0;
            bool Post = false;
            Query = "SELECT [Posting] FROM [GLJournal] WHERE [GLJournalID] = @GLJournalID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalID", txtGLJournalCode.Text);
                Post = Convert.ToBoolean(Cmd.ExecuteScalar());
            }
            switch (Post)
            {
                case true:
                    Post = false;
                    Post1 = 0;
                    break;
                case false:
                    Post = true;
                    Post1 = 1;
                    break;
            }
            Query = "UPDATE [GLJournal] SET Posting=" + Post1 + " WHERE [GLJournalID] = @GLJournalID ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalID", txtGLJournalCode.Text);
                Cmd.ExecuteNonQuery();
            }
            chkpost.Checked = Post;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GetHeader();
            RefreshGrid();
            ModeView();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string msg = Validation();
            if (msg != "")
            {
                MetroFramework.MetroMessageBox.Show(Owner, msg);
                return;
            }
            try
            {
                using (Scope = new TransactionScope())
                {
                    Con = ConnectionString.GetConnection();

                    if (Mode.ToUpper() == "NEW")
                    {
                        ID = ID = ConnectionString.GenerateSeqID(7, "JN", "JN", Con, Cmd);
                        InsertJournalH(Con);
                        InsertJournalDtl(Con);
                        Con.Close();
                        Scope.Complete();
                        MessageBox.Show("Journal berhasil disave.");
                    }
                    else if (Mode.ToUpper() == "EDIT")
                    {
                        UpdateJournalH(Con);
                        DeleteJournalDtl(Con);
                        InsertJournalDtl(Con);
                        Con.Close();
                        Scope.Complete();
                        MessageBox.Show("Journal berhasil diupdate.");
                    }
                }
                ModeView();
                GetHeader();
                RefreshGrid();
                SetColumnConfig();
                Parent.RefreshGrid();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
            finally { }
        }

        //sorting the column No
        private void SortNo()
        {
            for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
            {
                dgvJournalHeader.Rows[i].Cells["No"].Value = (i + 1);
            }
        }

        //calculate the debet and credit and input it into txtbox
        private void FillCreditAndDebet()
        {
            decimal Credit = 0;
            decimal Debet = 0;
            for (int i = 0; i < dgvJournalHeader.Rows.Count; i++)
            {
                if (dgvJournalHeader.Rows[i].Cells["JournalDType"].Value != null)
                {
                    if (dgvJournalHeader.Rows[i].Cells["JournalDType"].Value.ToString() == "D")
                    {
                        Debet += Convert.ToDecimal(dgvJournalHeader.Rows[i].Cells["Amount"].Value);
                    }
                    else if (dgvJournalHeader.Rows[i].Cells["JournalDType"].Value.ToString() == "K")
                    {
                        Credit += Convert.ToDecimal(dgvJournalHeader.Rows[i].Cells["Amount"].Value);
                    }
                }
            }
            txtTotalDebet.Text = String.Format("{0:#,##0.###0}", Debet);
            txtTotalKredit.Text = String.Format("{0:#,##0.###0}", Credit);
        }

        private void dgvJournalHeader_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            SortNo();
        }

        private void dgvJournalHeader_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            FillCreditAndDebet();
            SortNo();
        }

        private void dgvJournalHeader_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int i = dgvJournalHeader.CurrentCell.RowIndex;
            if ((dgvJournalHeader.Rows[i].Cells["Amount"].Value).ToString() == ".")
            {
                dgvJournalHeader.Rows[i].Cells["Amount"].Value = 0.0000;
            }
            else
            {
                string tipe = dgvJournalHeader.Rows[i].Cells["JournalDType"].Value == null ? "" : dgvJournalHeader.Rows[i].Cells["JournalDType"].Value.ToString();
                if (tipe == "K")
                {
                    dgvJournalHeader.Rows[i].Cells["Amount"].Value = String.Format("{0:#,##0.###0}", Convert.ToDecimal(dgvJournalHeader.Rows[i].Cells["Amount"].Value.ToString()));
                }
                else
                {
                    dgvJournalHeader.Rows[i].Cells["Amount"].Value = String.Format("{0:#,##0.###0}", Convert.ToDecimal(dgvJournalHeader.Rows[i].Cells["Amount"].Value.ToString()));
                }
            }
            FillCreditAndDebet();
        }

        private void txtReference_TextChanged(object sender, EventArgs e)
        {
            dgvJournalHeader.Rows.Clear();
        }

        private void dgvJournalHeader_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvJournalHeader.Columns[dgvJournalHeader.CurrentCell.ColumnIndex].Name == "Amount")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvJournalHeader_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvJournalHeader_KeyPress);
            }
        }

        private void dgvJournalHeader_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvJournalHeader.Rows.Count > 0)
            {
                if (dgvJournalHeader.Columns[dgvJournalHeader.CurrentCell.ColumnIndex].Name == "Amount")
                {
                    if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.'))
                    {
                        e.Handled = true;
                    }
                    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    {
                        e.Handled = true;
                    }

                }
            }
        }

        private void chkpost_CheckedChanged(object sender, EventArgs e)
        {
            if (chkpost.Checked == true)
            {
                chkpost.Text = "Posted";
            }
            else
            {
                chkpost.Text = "Unposted";
            }
        }

        private void btnSearchJournalType_Click(object sender, EventArgs e)
        {
            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable("dbo", "M_JournalH", "and a.Status='Gunakan'", "a.*", "M_JournalH a");
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                RemoveLineWithJournalHID();
                txtJournalType.Text = SearchV2.data[0];
            }
            establishedColumnConfig();
            populateGrid();
            SetColumnConfig();
        }

        private void RemoveLineWithJournalHID()
        {
            if (txtJournalType.Text != "")
            {
                for (int i = (dgvJournalHeader.Rows.Count - 1); i >= 0; i--)
                {
                    if (dgvJournalHeader.Rows[i].Cells["JournalHID"].Value.ToString() == txtJournalType.Text)
                    {
                        dgvJournalHeader.Rows.RemoveAt(i);
                    }
                }
            }
        }

        private void btnSearchReference_Click(object sender, EventArgs e)
        {
            Variable.Kode2 = null;
            //search invoice
            string Table = "[dbo].[GoodsReceivedH]";
            string QuerySearch = "SELECT O.ID,O.Date, O.CreatedDate,O.CreatedBy FROM(";
            QuerySearch += " SELECT a.[GoodsReceivedId] AS 'ID',a.[GoodsReceivedDate] as 'Date',a.CreatedDate,a.CreatedBy FROM [dbo].[GoodsReceivedH] a ";//WHERE a.[GoodsReceivedStatus] ='03' ";
            QuerySearch += " UNION SELECT b.[NRBId] AS 'ID',[NRBDate] as 'Date',b.CreatedDate,b.CreatedBy FROM [dbo].[NotaReturBeliH] b ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT c.[NPPID] AS 'ID',[NPPDate] as 'Date',c.CreatedDate,c.CreatedBy FROM [dbo].[NotaPurchaseParkH] c ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT d.[NRJId] AS 'ID',[NRJDate] as 'Date',d.CreatedDate,d.CreatedBy FROM [dbo].[NotaReturJualH] d ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT e.[NRZId] AS 'ID',[NRZDate] as 'Date',e.CreatedDate,e.CreatedBy FROM [dbo].[NotaResizeH] e ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT f.[AdjustID] AS 'ID',[AdjustDate] as 'Date',f.CreatedDate,f.CreatedBy FROM [dbo].[NotaAdjustmentH] f ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT g.[TransferNo] AS 'ID',[TransferDate] as 'Date',g.CreatedDate,g.CreatedBy FROM [dbo].[NotaTransferH] g ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT h.[DeliveryOrderId] AS 'ID',[DeliveryOrderDate] as 'Date',h.CreatedDate,h.CreatedBy FROM [dbo].[DeliveryOrderH] h ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT i.[InvoiceId] AS 'ID',[InvoiceDate] as 'Date',i.CreatedDate,i.CreatedBy FROM [dbo].[VendInvoiceH] i ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT j.[PV_No] AS 'ID',[PV_Date] as 'Date',j.CreatedDate,j.CreatedBy FROM [dbo].[PaymentVoucher_H] j ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT k.[Invoice_Id] AS 'ID',[Invoice_Date] as 'Date',k.CreatedDate,k.CreatedBy FROM [dbo].[CustInvoice_H] k ";//WHERE b.[TransStatusId] ='Gunakan' ";
            QuerySearch += " UNION SELECT l.[RV_No] AS 'ID',[RV_Date] as 'Date',l.CreatedDate,l.CreatedBy FROM [dbo].[ReceiptVoucher_H] l ) O ";//WHERE b.[TransStatusId] ='Gunakan' ";
            string[] FilterText = { "ID" };
            string[] Select = { "ID","Date" };
            string PrimaryKey = "ID";
            string[] HideField = { "Check"};

            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();
            if (Variable.Kode2 != null)
            {
                txtReference.Text = Variable.Kode2[0, 0];
                dtRefDate.Value = Convert.ToDateTime(Variable.Kode2[0, 1]);
                Variable.Kode2 = null;
            }
        }
        //tia edit
        //klik kanan
        Sales.BBK.BBKHeader BBKId = null;
        Purchase.NotaReturBeli.ReturBeliHeader NrbId = null;
        Sales.DeliveryOrder.DOHeader DeliveryOrder = null;
        ISBS_New.Purchase.GoodsReceipt.GRHeaderV2 Gr = null;
        TaskList.GlobalTasklist Parent2 = new TaskList.GlobalTasklist();

        public void SetParent2(TaskList.GlobalTasklist Tl)
        {
            Parent2 = Tl;
        }
       
       private bool CheckOpened(string name)
       {
           FormCollection FC = Application.OpenForms;
           foreach (Form frm in FC)
           {
               if (frm.Name == name)
               {
                   return true;
               }
           }
           return false;
       }
        //bbm, bbma, nrba, NA, DO
        private void txtReference_MouseDown(object sender, MouseEventArgs e)
       {
           txtReference.ContextMenu = Reff;
           if (e.Button == MouseButtons.Right)
           {
               //NRB
               if (NrbId == null || NrbId.Text == "")
               {
                   if (txtReference.Text.Contains("NRB"))
                       {
                           NrbId = new Purchase.NotaReturBeli.ReturBeliHeader();
                           NrbId.SetMode("PopUp", txtReference.Text);
                           NrbId.Show();
                       }
               }
               else if (CheckOpened(NrbId.Name))
               {
                   NrbId.WindowState = FormWindowState.Normal;
                   NrbId.SetMode("PopUp", txtReference.Text);
                   NrbId.Show();
                   NrbId.Focus();
               }
               //DO
               if (DeliveryOrder == null || DeliveryOrder.Text == "")
               {
                   if (txtReference.Text.Contains("DO"))
                   {
                       DeliveryOrder = new Sales.DeliveryOrder.DOHeader();
                       DeliveryOrder.SetMode("PopUp", txtReference.Text);
                       DeliveryOrder.ParentRefreshGrid2(this);
                       DeliveryOrder.Show();
                   }
               }
               else if (CheckOpened(DeliveryOrder.Name))
               {
                   DeliveryOrder.WindowState = FormWindowState.Normal;
                   DeliveryOrder.SetMode("PopUp", txtReference.Text);
                   DeliveryOrder.ParentRefreshGrid2(this);
                   DeliveryOrder.Show();
                   DeliveryOrder.Focus();
               }
               //BBM
               if (Gr == null || Gr.Text == "")
               {
                   if (txtReference.Text.Contains("BBM"))
                   {
                       Gr = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                       Gr.SetMode("PopUp", txtReference.Text);
                       Gr.ParentRefreshGrid6(this);
                       Gr.Show();
                   }
               }
               else if (CheckOpened(Gr.Name))
               {
                   Gr.WindowState = FormWindowState.Normal;
                   Gr.Show();
                   Gr.Focus();
               }
               //BBK
               if (BBKId == null || BBKId.Text == "")
               {
                 if (txtReference.Text.Contains("BBK"))
                 {
                     txtReference.Enabled = true;
                     BBKId = new Sales.BBK.BBKHeader();
                     BBKId.SetMode("PopUp", txtReference.Text);
                     BBKId.ParentRefreshGrid4(this);
                     BBKId.Show();
                 }
               }
               else if (CheckOpened(BBKId.Name))
               {
                  BBKId.WindowState = FormWindowState.Normal;
                  BBKId.Show();
                  BBKId.Focus();
               }
               //NA?
               
           }
        }
        //tia edit end
    }
}
