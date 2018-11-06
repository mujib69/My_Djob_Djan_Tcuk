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

namespace ISBS_New.JournalType.FormJournal
{
    public partial class FormJournal : MetroFramework.Forms.MetroForm
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

        public FormJournal()
        {
            InitializeComponent();
        }

        public void SetMode(string passedMode, string id)
        {
            Mode = passedMode;
            SelectedId = id;
        }

        private void FormJournal_Load(object sender, EventArgs e)
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
            chkInActive.Enabled = false;
        }

        private void ModeNew()
        {
            Mode = "New";

            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnInActive.Enabled = false;

            txtJournalID.Enabled = true;
            txtDeskripsi.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;
        }

        private void ModeView()
        {
            Mode = "View";

            btnEdit.Enabled = true;
            btnSave.Enabled = false;
            btnCancel.Enabled = false;
            btnExit.Enabled = true;
            btnInActive.Enabled = true;

            txtJournalID.Enabled = false;
            txtDeskripsi.Enabled = false;
            btnNew.Enabled = false;
            btnDelete.Enabled = false;           

            GetHeader();
            RefreshGrid();
            dgvDetail.ReadOnly = true;
        }

        private void ModeEdit()
        {
            Mode = "Edit";

            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnExit.Enabled = true;
            btnInActive.Enabled = false;

            txtJournalID.Enabled = false;
            txtDeskripsi.Enabled = true;
            btnNew.Enabled = true;
            btnDelete.Enabled = true;

            dgvDetail.ReadOnly = false;
            GetHeader();
            RefreshGrid();

            Deskripsi = txtDeskripsi.Text.Trim();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean Validasi()
        {
            if (txtJournalID.Text.Trim() == "")
            {
                MessageBox.Show("Journal ID harus diisi.");
                return false;
            }
            else if (txtDeskripsi.Text.Trim() == "")
            {
                MessageBox.Show("Deskripsi harus diisi.");
                return false;
            }
            else if (!CheckAvailability())
            {
                return false;
            }
            else if (dgvDetail.RowCount < 1)
            {
                MessageBox.Show("Data grid tidak boleh kosong.");
                return false;
            }
            else if (!CheckType())
            {                
                return false;
            }
            else if (Mode == "Edit" && !CheckUsage())
            {
                return false;
            }
            return true;
        }

        private Boolean CheckType()
        {
            Boolean vBol = true;
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                if (dgvDetail.Rows[i].Cells["Type"].Value == null || dgvDetail.Rows[i].Cells["Type"].Value.ToString() == String.Empty)
                {
                    MessageBox.Show("Masukan Type untuk row nomor " + (i + 1));
                    vBol = false;
                    return vBol;
                }
            }
            return vBol;
        }

        private Boolean CheckAvailability()
        {
            Boolean vBol = false;

            if (Mode == "New")
            {
                Query = "SELECT * FROM [dbo].[M_JournalH] WHERE [JournalHID] = @jhid ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@jhid", txtJournalID.Text.Trim());
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        MessageBox.Show("Journal ID sudah dipakai");
                        vBol = false;
                    }
                    else
                    {
                        Query = "SELECT * FROM [dbo].[M_JournalH] WHERE [JournalHDesc] = @deskripsi ";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text.Trim());
                            SqlDataReader Dr2 = Cmd.ExecuteReader();
                            if (Dr2.HasRows)
                            {
                                MessageBox.Show("Deskripsi sudah dipakai");
                                vBol = false;
                            }
                            else
                                vBol = true;
                            Dr.Close();
                        }
                    }
                    Dr.Close();
                }
            }
            else if (Mode == "Edit")
            {
                if (txtDeskripsi.Text.Trim() == Deskripsi)
                    vBol = true;
                else
                {
                    Query = "SELECT * FROM [dbo].[M_JournalH] WHERE [JournalHDesc] = @deskripsi ";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text.Trim());
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
                    }
                }
            }
            return vBol;
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
                    //insert journalH
                    Query = "INSERT INTO [dbo].[M_JournalH] ([JournalHID],[JournalHDesc],[Status],[CreatedDate],[CreatedBy]) values ";
                    Query += "(@jhid, @deskripsi, @status, @date, @UserId)";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@jhid", txtJournalID.Text.Trim());
                        Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text.Trim());
                        Cmd.Parameters.AddWithValue("@status", "Gunakan");
                        Cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                    }

                    //insert journalD
                    for (int i = 0; i < dgvDetail.Rows.Count; i++)
                    {
                        Query = "INSERT INTO [dbo].[M_JournalD] ([JournalHID],[SeqNo],[Type],[FQA_ID],[FQA_Desc],[Status],[CreatedDate],[CreatedBy]) values ";
                        Query += "(@jhid, @seqno, @type, @fqaid, @fqadesc, @status, @date, @UserId)";

                        using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.Parameters.AddWithValue("@jhid", txtJournalID.Text.Trim());
                            Cmd.Parameters.AddWithValue("@seqno", dgvDetail.Rows[i].Cells["No"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@type", dgvDetail.Rows[i].Cells["Type"].Value.ToString().ToUpper());
                            Cmd.Parameters.AddWithValue("@fqaid", dgvDetail.Rows[i].Cells["FQA_ID"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@fqadesc", dgvDetail.Rows[i].Cells["FQA_Desc"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@status", true);
                            Cmd.Parameters.AddWithValue("@date", DateTime.Now);
                            Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                            Cmd.ExecuteNonQuery();
                        }
                    }

                    Trans.Commit();
                    MessageBox.Show(txtJournalID.Text + " berhasil dibuat");
                }
                if (Mode == "Edit")
                {
                    //Update journalH
                    Query = "UPDATE [dbo].[M_JournalH] SET [JournalHDesc] = @deskripsi, [UpdatedDate] = @date,[UpdatedBy] = @UserId WHERE [JournalHID] = @jhid ";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@deskripsi", txtDeskripsi.Text.Trim());
                        Cmd.Parameters.AddWithValue("@date", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@jhid", txtJournalID.Text.Trim());
                        Cmd.ExecuteNonQuery();
                    }

                    //Clear journalD
                    Query = "DELETE FROM [dbo].[M_JournalD] WHERE [JournalHID] = @jhid ";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        Cmd.Parameters.AddWithValue("@jhid", txtJournalID.Text.Trim());
                        Cmd.ExecuteNonQuery();
                    }

                    //insert journalD
                    for (int i = 0; i < dgvDetail.Rows.Count; i++)
                    {
                        Query = "INSERT INTO [dbo].[M_JournalD] ([JournalHID],[SeqNo],[Type],[FQA_ID],[FQA_Desc],[Status],[CreatedDate],[CreatedBy]) values ";
                        Query += "(@jhid, @seqno, @type, @fqaid, @fqadesc, @status, @date, @UserId)";

                        using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.Parameters.AddWithValue("@jhid", txtJournalID.Text.Trim());
                            Cmd.Parameters.AddWithValue("@seqno", dgvDetail.Rows[i].Cells["No"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@type", dgvDetail.Rows[i].Cells["Type"].Value.ToString().ToUpper());
                            Cmd.Parameters.AddWithValue("@fqaid", dgvDetail.Rows[i].Cells["FQA_ID"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@fqadesc", dgvDetail.Rows[i].Cells["FQA_Desc"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@status", true);
                            Cmd.Parameters.AddWithValue("@date", DateTime.Now);
                            Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                            Cmd.ExecuteNonQuery();
                        }
                    }

                    Trans.Commit();
                    MessageBox.Show(txtJournalID.Text + " berhasil diupdate");
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
            RefreshGrid();
            Form parentform = Application.OpenForms["GlobalInquiry"];
            if (parentform != null)
                Parent.RefreshGrid();
        }

        private void GetHeader()
        {
            if (SelectedId != null && SelectedId != "")
            {
                txtJournalID.Text = SelectedId;
            }
            Query = "SELECT * FROM [dbo].[M_JournalH] WHERE [JournalHID] = @jhid ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@jhid", SelectedId);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtDeskripsi.Text = Dr["JournalHDesc"].ToString();
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
                Query = "UPDATE [dbo].[M_JournalH] SET Status=@Status WHERE [JournalHID] = @jhid";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@jhid", SelectedId);
                    Cmd.Parameters.AddWithValue("@Status", "Batal");
                    Cmd.ExecuteNonQuery();
                }
            }
            else
            {
                Query = "UPDATE [dbo].[M_JournalH] SET Status=@Status WHERE [JournalHID] = @jhid";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@jhid", SelectedId);
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

        private string Deskripsi;
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!CheckUsage())
                return;

            ModeEdit();            
        }

        private Boolean CheckUsage()
        {
            Boolean vBol = false;
            Query = "SELECT * FROM [dbo].[GLJournalH] WHERE [JournalHID] = @jhid";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@jhid", SelectedId);

                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    MessageBox.Show("Tidak bisa di-edit karena sudah dipakai di GL Journal");
                    vBol = false;
                }
                else
                    vBol = true;
                Dr.Close();
            }

            return vBol;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //string Table = "[dbo].[M_FQA1]";
            //string QuerySearch = "SELECT [FQA1_ID] AS 'FQAID',[FQA1_Desc] AS 'FQADesc', M_COAType FROM [dbo].[M_FQA1] f1 ";
            //QuerySearch += "LEFT JOIN [M_COA] c ON f1.COA_ID = c.COA_ID ";
            //QuerySearch += "UNION SELECT [FQA2_ID] AS 'FQAID',[FQA2_Desc] AS 'FQADesc', M_COAType FROM [dbo].[M_FQA2] f2 ";
            //QuerySearch += "LEFT JOIN [M_FQA1] f1 ON f1.FQA1_ID = f2.FQA1_ID ";
            //QuerySearch += "LEFT JOIN [M_COA] c ON f1.COA_ID = c.COA_ID ";


            //string[] FilterText = { "FQAID", "FQADesc", "M_COAType" };
            //string[] Mask = { "FQA ID", "Deskripsi", "Type" };
            //string[] Select = { "FQAID", "FQADesc", "M_COAType" };
            //string PrimaryKey = "FQAID";
            //string[] HideField = { };

            string Table = "[dbo].[M_FQA1]";
            string QuerySearch = "SELECT [FQA1_ID] AS 'FQAID',[FQA1_Desc] AS 'FQADesc' FROM [dbo].[M_FQA1] f1 ";
            QuerySearch += "UNION SELECT [FQA2_ID] AS 'FQAID',[FQA2_Desc] AS 'FQADesc' FROM [dbo].[M_FQA2] f2 ";
            QuerySearch += "LEFT JOIN [M_FQA1] f1 ON f1.FQA1_ID = f2.FQA1_ID ";

            string[] FilterText = { "FQAID", "FQADesc" };
            string[] Mask = { "FQA ID", "Deskripsi" };
            string[] Select = { "FQAID", "FQADesc" };
            string PrimaryKey = "FQAID";
            string[] HideField = { };

            callSearchQueryV2Form(Table, QuerySearch, FilterText, Mask, Select, PrimaryKey, HideField);
        }

        private void callSearchQueryV2Form(string Table, string QuerySearch, string[] FilterText, string[] Mask, string[] Select, string PrimaryKey, string[] HideField)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Mask = Mask;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();

            populateAfterSearch(Table);
        }

        private void populateAfterSearch(string Table)
        {
            if (Variable.Kode2 == null)
            {
                return;
            }
            if (Table == "[dbo].[M_FQA1]")
            {
                using (Method C = new Method())
                {
                    if (dgvDetail.Columns.Count <= 0)
                    {
                        dgvDetail.ColumnCount = 4;
                        dgvDetail.Columns[0].Name = "No";
                        dgvDetail.Columns[1].Name = "FQA_ID";
                        dgvDetail.Columns[2].Name = "FQA_Desc";
                        dgvDetail.Columns[3].Name = "Type";
                    }

                    for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                    {
                        dgvDetail.Rows.Add("", Variable.Kode2[i, 0], Variable.Kode2[i, 1],"cmbtype");
                    }

                    dgvDetail.ReadOnly = false;
                    string[] read = new string[] { "No", "FQA_ID", "FQA_Desc" };
                    for (int i = 0; i < read.Length; i++)
                    {
                        dgvDetail.Columns[read[i]].ReadOnly = true;
                    }

                    dgvDetail.AutoResizeColumns();
                    dgvDetail.AllowUserToAddRows = false;
                    Variable.Kode2 = null;
                }
            }
            
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                DataGridViewComboBoxCell cmbType = new DataGridViewComboBoxCell();
                cmbType.Items.Add("D");
                cmbType.Items.Add("K");
                if (dgvDetail.Rows[i].Cells["Type"].Value != null)
                {
                    if (dgvDetail.Rows[i].Cells["Type"].Value.ToString() == "D")
                    {
                        cmbType.Value = cmbType.Items[0];
                    }
                    else if (dgvDetail.Rows[i].Cells["Type"].Value.ToString() == "K")
                    {
                        cmbType.Value = cmbType.Items[1];
                    }
                }
                dgvDetail.Rows[i].Cells["Type"] = cmbType;
            }
                ReorderGridNo();
        }

        private void ReorderGridNo()
        {
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                dgvDetail.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            dgvDetail.Rows.Clear();
            if (dgvDetail.Rows.Count <= 0)
            {
                dgvDetail.ColumnCount = 4;
                dgvDetail.Columns[0].Name = "No";
                dgvDetail.Columns[1].Name = "FQA_ID";
                dgvDetail.Columns[2].Name = "FQA_Desc";
                dgvDetail.Columns[3].Name = "Type";
            }

            //Query = "SELECT a.FQA_ID, ";
            //Query += "COALESCE(c.FQA2_Desc, b.FQA1_Desc) AS FQA_Desc, Type ";
            //Query += "FROM M_JournalD a ";
            //Query += "LEFT JOIN [M_FQA1] b ON a.[FQA_ID] = b.FQA1_ID ";
            //Query += "LEFT JOIN [M_FQA2] c ON a.[FQA_ID] = c.FQA2_ID ";
            //Query += "LEFT JOIN [M_COA] d ON d.COA_ID = b.COA_ID ";
            //Query += "WHERE JournalHID = @jhid ";

            Query = "SELECT a.FQA_ID, ";
            Query += "COALESCE(c.FQA2_Desc, b.FQA1_Desc) AS FQA_Desc, Type ";
            Query += "FROM M_JournalD a ";
            Query += "LEFT JOIN [M_FQA1] b ON a.[FQA_ID] = b.FQA1_ID ";
            Query += "LEFT JOIN [M_FQA2] c ON a.[FQA_ID] = c.FQA2_ID ";
            Query += "LEFT JOIN [M_COA] d ON d.COA_ID = b.COA_ID ";
            Query += "WHERE JournalHID = @jhid ";

            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@jhid", txtJournalID.Text.Trim());
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    this.dgvDetail.Rows.Add("", Dr["FQA_ID"], Dr["FQA_Desc"],Dr["Type"]);
                }
                Dr.Close();
            }
            dgvDetail.AllowUserToAddRows = false;
            string[] read = new string[] { "No", "FQA_ID", "FQA_Desc" };
            for (int i = 0; i < read.Length; i++)
            {
                dgvDetail.Columns[read[i]].ReadOnly = true;
            }

            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                DataGridViewComboBoxCell cmbType = new DataGridViewComboBoxCell();
                cmbType.Items.Add("D");
                cmbType.Items.Add("K");
                if (dgvDetail.Rows[i].Cells["Type"].Value != null)
                {
                    if (dgvDetail.Rows[i].Cells["Type"].Value.ToString() == "D")
                    {
                        cmbType.Value = cmbType.Items[0];
                    }
                    else if (dgvDetail.Rows[i].Cells["Type"].Value.ToString() == "K")
                    {
                        cmbType.Value = cmbType.Items[1];
                    }
                }
                dgvDetail.Rows[i].Cells["Type"] = cmbType;
            }

            ReorderGridNo();
            Conn.Close();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDetail.RowCount > 0)
            {
                int x = dgvDetail.CurrentRow.Index;
                DialogResult dr = MessageBox.Show("Apakah Anda Ingin Menghapus FQA " + dgvDetail.Rows[x].Cells["FQA_ID"].Value.ToString() + " ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    if (dgvDetail.RowCount > 0)
                    {
                        dgvDetail.Rows.RemoveAt(x);
                        for (int i = 0; i < dgvDetail.RowCount; i++)
                        {
                            dgvDetail.Rows[i].Cells["No"].Value = i + 1;
                        }
                    }
                }
                ReorderGridNo();
            }
        }


    }
}
