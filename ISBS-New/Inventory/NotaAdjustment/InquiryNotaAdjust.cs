using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Transactions;

namespace ISBS_New.Inventory.NotaAdjustment
{
    public partial class InquiryNotaAdjust : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private TransactionScope Scope;

        private string TransStatus = String.Empty;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        public static int dataShow;
        private static int countNA;

        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public InquiryNotaAdjust()
        {
            InitializeComponent();
        }

        private void InquiryNotaAdjust_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01','04'";
                }
                Query = "Select [No], [NA ID], [NA Date], ActionCode, [Site ID], TransStatus, TransStatusName, ApprovedBy, ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY AdjustId desc) [No], AdjustId 'NA ID',ApprovedBy, AdjustDate 'NA Date', ActionCode , InventID 'Site ID', TransStatus, b.Deskripsi 'TransStatusName', ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [ISBS-NEW4].[dbo].[NotaAdjustmentH] a  LEFT JOIN [ISBS-NEW4].[dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.TransCode = 'NotaAdjustment'  ";
                Query += "Where TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                string subquery = "";
                Query = "SELECT [FieldName] FROM [User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'NotaAdjustmentH';";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (subquery == "")
                        {
                            subquery += Dr["FieldName"].ToString() + " like @search ";
                        }
                        else
                        {
                            subquery += "or " + Dr["FieldName"].ToString() + " like @search ";
                        }
                    }
                    Dr.Close();
                }
                Query = "Select [No], [NA ID], [NA Date], ActionCode, [Site ID], TransStatus, TransStatusName, ApprovedBy, ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY AdjustId desc) [No], AdjustId 'NA ID',ApprovedBy, AdjustDate 'NA Date', ActionCode , InventID 'Site ID', TransStatus, b.Deskripsi 'TransStatusName', ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [ISBS-NEW4].[dbo].[NotaAdjustmentH] a  LEFT JOIN [ISBS-NEW4].[dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.TransCode = 'NotaAdjustment'  ";
                Query += " WHERE ("+subquery+") and TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "Select [No], [NA ID], [NA Date], ActionCode, [Site ID], TransStatus, TransStatusName, ApprovedBy, ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY AdjustId desc) [No], AdjustId 'NA ID',ApprovedBy, AdjustDate 'NA Date', ActionCode , InventID 'Site ID', TransStatus, b.Deskripsi 'TransStatusName', ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [ISBS-NEW4].[dbo].[NotaAdjustmentH] a  LEFT JOIN [ISBS-NEW4].[dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.TransCode = 'NotaAdjustment'  ";
                Query += " WHERE (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (AdjustId like @search or ActionCode like @search  or InventID like @search or TransStatus like @search or ApprovedBy like @search or ApprovedNotes like @search or CreatedBy like @search) and TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("UpdatedDate"))
            {
                Query = "Select [No], [NA ID], [NA Date], ActionCode, [Site ID], TransStatus, TransStatusName, ApprovedBy, ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                Query += "From (Select ROW_NUMBER() OVER (ORDER BY AdjustId desc) [No], AdjustId 'NA ID',ApprovedBy, AdjustDate 'NA Date', ActionCode , InventID 'Site ID', TransStatus, b.Deskripsi 'TransStatusName', ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [ISBS-NEW4].[dbo].[NotaAdjustmentH] a  LEFT JOIN [ISBS-NEW4].[dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.TransCode = 'NotaAdjustment'  ";
                Query += " WHERE (CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (AdjustId like @search or ActionCode like @search  or InventID like @search or TransStatus like @search or ApprovedBy like @search or ApprovedNotes like @search or CreatedBy like @search) and TransStatus in (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                try
                {
                    string truecrit = "";
                    Query = "SELECT [FieldName] FROM [User].[Table] WHERE [DisplayName] = @DisplayName AND [SchemaName] = 'dbo' AND [TableName] = 'NotaAdjustmentH';";
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@DisplayName", crit);
                        truecrit = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                    }
                    Query = "Select [No], [NA ID], [NA Date], ActionCode, [Site ID], TransStatus, TransStatusName, ApprovedBy, ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                    Query += "From (Select ROW_NUMBER() OVER (ORDER BY AdjustId desc) [No], AdjustId 'NA ID',ApprovedBy, AdjustDate 'NA Date', ActionCode , InventID 'Site ID', TransStatus, b.Deskripsi 'TransStatusName', ApprovedNotes, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [ISBS-NEW4].[dbo].[NotaAdjustmentH] a  LEFT JOIN [ISBS-NEW4].[dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.TransCode = 'NotaAdjustment'  ";
                    Query += " WHERE " + truecrit + " Like @search and TransStatus in (" + TransStatus + ")) a ";
                    Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
                }
                catch (Exception EE)
                {
                    MessageBox.Show(EE.ToString());
                    return;
                }
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvNA.AutoGenerateColumns = true;
            dgvNA.DataSource = Dt;
            dgvNA.Refresh();
            
            dgvNA.Columns["NA Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNA.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvNA.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(AdjustId) From [dbo].[NotaAdjustmentH] ";
                Query += " WHERE TransStatus in ("+TransStatus+");";
            }
            else if (crit.Equals("All"))
            {
                string subquery = "";
                Query = "SELECT [FieldName] FROM [User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'NotaAdjustmentH';";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (subquery == "")
                        {
                            subquery += Dr["FieldName"].ToString() + " like @search ";
                        }
                        else
                        {
                            subquery += "or "+Dr["FieldName"].ToString() + " like @search ";
                        }
                    }
                    Dr.Close();
                }
                Query = "Select Count(AdjustId) From [dbo].[NotaAdjustmentH] Where ";
                Query += "("+subquery+") ";
                Query += " AND TransStatus in (" + TransStatus + ");";
            }
            else if (crit.Equals("NA Date"))
            {
                Query = "Select Count(AdjustId) From [dbo].[NotaAdjustmentH] Where ";
                Query += "(CONVERT(VARCHAR(10),AdjustDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),AdjustDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                Query += " AND TransStatus in (" + TransStatus + ");";
            }
            else if (crit.Equals("UpdatedDate"))
            {
                Query = "Select Count(AdjustId) From [dbo].[NotaAdjustmentH] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                Query += " AND TransStatus in (" + TransStatus + ");";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query = "Select Count (AdjustId) From [dbo].[NotaAdjustmentH] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                Query += " AND TransStatus in (" + TransStatus + ");";
            }
            else
            {
                try
                {
                    string truecrit = "";
                    Query = "SELECT [FieldName] FROM [User].[Table] WHERE [DisplayName] = @DisplayName AND [SchemaName] = 'dbo' AND [TableName] = 'NotaAdjustmentH';";
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@DisplayName", crit);
                        truecrit = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                    }
                    Query = "Select Count(AdjustId) From [dbo].[NotaAdjustmentH] Where ";
                    Query += truecrit + " Like @search ";
                    Query += " AND TransStatus in (" + TransStatus + ");";
                }
                catch(Exception EE)
                {
                    MessageBox.Show(EE.ToString());
                    return;
                }
            }

            //Cmd = new SqlCommand(Query, Conn);
            //Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            //Conn.Close();

            //lblTotal.Text = "Total Rows : " + Total.ToString();
            //if (dataShow != 0)
            //    Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            //else
            //    Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            //lblPage.Text = "/ " + Page2;

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            dgvNA.AutoResizeColumns();
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            //cmbCriteria.Items.Add("NotaAdjustmentDate");
            //cmbCriteria.Items.Add("CreatedDate");
            
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'NotaAdjustmentH'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    cmbCriteria.Items.Add(Dr[0]);
                }
                Dr.Close();
                cmbCriteria.SelectedIndex = 0;
            }
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            dataShow = Int32.Parse(cmbShow.Text);
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            RefreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }

            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Page2 != null)
            {
                if (Convert.ToInt32(txtPage.Text) > Page2)
                {
                    txtPage.Text = Page2.ToString();
                }
            }
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
            else if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                RefreshGrid();

            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShow.Items.Clear();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr.GetInt32(0));
            }
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }
            txtPage.Text = "1";
            Limit1 = 1;
            Limit2 = 10;
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        //btnBatalkan
        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                //decimal QtyBefore = 0;
                //decimal PurchOrderSeqNo = 0;
                //String POID = null;
                if (dgvNA.Rows[dgvNA.CurrentCell.RowIndex].Cells["TransStatusName"].Value.ToString() == "Transaction Complete")
                {
                    MessageBox.Show("Transaksi sudah selesai, tidak dapat di delete.");
                    return;
                }
                if (dgvNA.Rows[dgvNA.CurrentCell.RowIndex].Cells["TransStatusName"].Value.ToString() == "Revision")
                {
                    MessageBox.Show("Transaksi diminta untuk direvisi, tidak dapat di delete.");
                    return;
                }
                if (dgvNA.Rows[dgvNA.CurrentCell.RowIndex].Cells["TransStatusName"].Value.ToString() == "Reject")
                {
                    MessageBox.Show("Transaksi sudah di-reject, tidak dapat di delete.");
                    return;
                }

                try
                {
                    if (dgvNA.RowCount > 0)
                    {
                        Index = dgvNA.CurrentRow.Index;
                        string AdjustID = dgvNA.Rows[Index].Cells["NotaAdjustmentNumber"].Value == null ? "" : dgvNA.Rows[Index].Cells["NotaAdjustmentNumber"].Value.ToString();

                        DialogResult dr = MessageBox.Show("NotaAdjustmentNumber = " + AdjustID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            Trans = Conn.BeginTransaction();

                            //Update movement
                            UpdateMovement(Conn, Trans,"-");

                            //update Header
                            Query = "UPDATE [dbo].[NotaAdjustmentH] SET TransStatus = '02' where AdjustID ='" + AdjustID + "';";

                            //Begin
                            //Created By : Joshua
                            //Created Date ; 24 Aug 2018
                            //Desc : Delete Journal
                            //DeleteJournal(AdjustID);
                            //End

                            MessageBox.Show("NotaAdjustNumber = " + AdjustID.ToUpper() + "\n" + "Data berhasil dibatalkan.");

                            Index = 0;
                            Trans.Commit();
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void DeleteJournal(string AdjustID)
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 24 Aug 2018
            //Desc : Delete Journal
            //Delete Journal Detail

            //Get GLJournalHID
            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + AdjustID + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();

            Query = "DELETE FROM GLJournalH WHERE GLJournalHID = '" + GLJournalHID + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
            //End
        }

        //private void DeleteUpdateInventOnhand(SqlConnection Con, SqlTransaction Transs)
        private void UpdateMovement(SqlConnection Con, SqlTransaction Transs, string opera)
        {
            SqlDataReader Dr2;
            //getting  values
            List<string> OldItemId = new List<string>();
            //List<string> OldToItemId = new List<string>();
            List<decimal> OldQtyUoM = new List<decimal>();
            List<decimal> OldQtyAlt = new List<decimal>();
            List<decimal> OldPrice = new List<decimal>();
            List<int> OldSeqNo = new List<int>();
            string InventId = "";
            Query = "SELECT a.[SeqNo],a.[FullItemID],a.[ToFullItemID],a.[Qty_UoM],a.[Qty_Alt],b.[UoM_AvgPrice],c.[InventID] FROM [NotaAdjustment_Dtl] a LEFT JOIN [dbo].[InventTable] b ON a.[FullItemID] = b.[FullItemID] AND a.[Qty_Unit]=b.[UoM] AND a.[Alt_Unit]=b.[UoMAlt] LEFT JOIN [dbo].[NotaAdjustmentH] c ON a.AdjustID=c.[AdjustID] WHERE a.[AdjustID] = '" + dgvNA.CurrentRow.Cells["NotaAdjustmentNumber"].Value.ToString() +"'";
            using (Cmd = new SqlCommand(Query, Con, Transs))
            {
                Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    //OldToItemId.Add(Dr["ToFullItemID"].ToString());
                    OldSeqNo.Add(Convert.ToInt32(Dr2["SeqNo"]));
                    OldItemId.Add(Dr2["FullItemID"].ToString());
                    OldQtyUoM.Add(Convert.ToDecimal(Dr2["Qty_UoM"]));
                    OldQtyAlt.Add(Convert.ToDecimal(Dr2["Qty_Alt"]));
                    OldPrice.Add(Convert.ToDecimal(Dr2["UoM_AvgPrice"]));
                    InventId = Dr2["InventID"].ToString();
                }
                Dr2.Close();
            }
            //deleting  amount
            for (int i = 0; i < OldItemId.Count(); i++)
            {
                Query = "UPDATE [dbo].[Invent_Movement_Qty] SET [Adjustment_In_Progress_UoM]" + opera + "=" + OldQtyUoM[i] + ",[Adjustment_In_Progress_Alt]" + opera + "=" + OldQtyAlt[i] + ",[Adjustment_In_Progress_Amount]" + opera + "=" + (OldQtyUoM[i] * OldPrice[i]) + " WHERE [FullItemID] = '" + OldItemId[i] + "'";
                using (Cmd = new SqlCommand(Query, Con, Transs))
                {
                    Cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateInventTrans(SqlConnection Con, SqlTransaction Transs, string FullItemID, int SeqNo, decimal Qty, decimal Qtyalt, decimal Qtyamount)
        {
            SqlDataReader Dr2;
            string GroupId = "";
            string SubGroupId1 = "";
            string SubGroupId2 = "";
            string ItemID = "";
            string ItemName = "";
            string FullItemId = FullItemID;
            decimal QtyUoM = Qty;
            decimal QtyAlt = Qtyalt;
            decimal QtyAmount = Qtyamount;
            string
            Query = "SELECT * FROM [dbo].[InventTable] WHERE [FullItemId] = '" + FullItemId + "'";
            using (Cmd = new SqlCommand(Query, Con, Transs))
            {
                Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    ItemName = Dr2["ItemDeskripsi"].ToString();
                    GroupId = Dr2["GroupID"].ToString();
                    SubGroupId1 = Dr2["SubGroup1ID"].ToString();
                    SubGroupId2 = Dr2["SubGroup2ID"].ToString();
                    ItemID = Dr2["ItemID"].ToString();
                }
                Dr2.Close();
            }
            Query = "INSERT INTO [dbo].[InventTrans]([GroupId],[SubGroupId],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteId],[TransId],[SeqNo],[TransDate],[Available_UoM],[Available_Alt],[Available_Amount],[Available_For_Sale_UoM],[Available_For_Sale_Alt],[Available_For_Sale_Amount],[Available_For_Sale_Reserved_UoM],[Available_For_Sale_Reserved_Alt],[Available_For_Sale_Reserved_Amount],[Notes]) ";
            Query += " VALUES(@GroupId,@SubGroupId,@SubGroup2Id,@ItemId,@FullItemId,@ItemName,@InventSiteId,@TransId,@SeqNo,@TransDate,@Available_UoM,@Available_Alt,@Available_Amount,@Available_For_Sale_UoM,@Available_For_Sale_Alt,@Available_For_Sale_Amount,@Available_For_Sale_Reserved_UoM,@Available_For_Sale_Reserved_Alt,@Available_For_Sale_Reserved_Amount,@Notes)";
            using (Cmd = new SqlCommand(Query, Con, Transs))
            {
                Cmd.Parameters.AddWithValue("@GroupId", GroupId);
                Cmd.Parameters.AddWithValue("@SubGroupId", SubGroupId1);
                Cmd.Parameters.AddWithValue("@SubGroup2Id", SubGroupId2);
                Cmd.Parameters.AddWithValue("@ItemId", ItemID);
                Cmd.Parameters.AddWithValue("@FullItemId", FullItemId);
                Cmd.Parameters.AddWithValue("@ItemName", ItemName);
                Cmd.Parameters.AddWithValue("@InventSiteId", dgvNA.Rows[dgvNA.CurrentCell.RowIndex].Cells["InventID"].Value.ToString());
                Cmd.Parameters.AddWithValue("@TransId", dgvNA.Rows[dgvNA.CurrentCell.RowIndex].Cells["NotaAdjustmentNumber"].Value.ToString());
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                Cmd.Parameters.AddWithValue("@TransDate", dgvNA.Rows[dgvNA.CurrentCell.RowIndex].Cells["NotaAdjustmentDate"].Value);
                Cmd.Parameters.AddWithValue("@Available_UoM", QtyUoM);
                Cmd.Parameters.AddWithValue("@Available_Alt", QtyAlt);
                Cmd.Parameters.AddWithValue("@Available_Amount", QtyAmount);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", QtyUoM);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", QtyAlt);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", QtyAmount);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM", 0);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt", 0);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount", 0);
                Cmd.Parameters.AddWithValue("@Notes", "");
                Cmd.ExecuteNonQuery();
            }
        }

        public static int CountNA { get { return countNA; } set { countNA = value; } }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                HeaderNotaAdjust HeaderNotaAdjust = new HeaderNotaAdjust();
                HeaderNotaAdjust.SetMode("New", "");
                HeaderNotaAdjust.Show();
                HeaderNotaAdjust.SetParent(this);
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            HeaderNotaAdjust header = new HeaderNotaAdjust();                   
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvNA.RowCount > 0)
                {
                    header.SetMode("BeforeEdit", dgvNA.CurrentRow.Cells["NA ID"].Value.ToString());
                    header.Show();
                    header.SetParent(this);
                    RefreshGrid();
                }
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
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void dgvNA_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNA.RowCount > 0)
            {
                HeaderNotaAdjust header = new HeaderNotaAdjust();
                header.SetMode("BeforeEdit", dgvNA.CurrentRow.Cells["NA ID"].Value.ToString());
                header.Show();
                header.SetParent(this);
                RefreshGrid();
            }
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            TransStatus = "'01','04'";
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
            RefreshGrid();
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            TransStatus = "'02','03'";
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
            RefreshGrid();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                try
                {
                    if (dgvNA.RowCount > 0)
                    {
                        HeaderNotaAdjust HeaderNotaAdjust = new HeaderNotaAdjust();
                        //header.SetParent(this);
                        HeaderNotaAdjust.Show();
                        HeaderNotaAdjust.ApproveStatus = true;
                        HeaderNotaAdjust.txtNotaNumber.Text = dgvNA.CurrentRow.Cells["NA ID"].Value.ToString();
                        HeaderNotaAdjust.GetDataHeader();
                        HeaderNotaAdjust.ModeApprove();

                        if (ConnectionString.GetConnection().State == ConnectionState.Closed)
                        {
                            Conn.Open();
                        }
                        else
                            Conn.Close();
                        Query = "select count(*) from [dbo].[NotaAdjustmentH] where [TransStatus] = '01'";
                        Cmd = new SqlCommand(Query, Conn);
                        Conn.Open();
                        CountNA = Int32.Parse(Cmd.ExecuteScalar().ToString());
                        lblTotal.Text = "Total Rows : " + CountNA.ToString();

                        //Begin
                        //Created By : Joshua
                        //Created Date ; 24 Aug 2018
                        //Desc : Create Journal
                        //Create
                        //End

                        RefreshGrid();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnGunakan_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                if (dgvNA.Rows[dgvNA.CurrentCell.RowIndex].Cells["TransStatusName"].Value.ToString() != "Reject")
                {
                    MessageBox.Show("Transaksi sudah aktif.");
                    return;
                }

                try
                {
                    if (dgvNA.RowCount > 0)
                    {
                        Index = dgvNA.CurrentRow.Index;
                        string AdjustID = dgvNA.Rows[Index].Cells["NotaAdjustmentNumber"].Value == null ? "" : dgvNA.Rows[Index].Cells["NotaAdjustmentNumber"].Value.ToString();

                        DialogResult dr = MessageBox.Show("NotaAdjustmentNumber = " + AdjustID + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            Trans = Conn.BeginTransaction();

                            //Update movement
                            UpdateMovement(Conn, Trans,"+");

                            //update Header
                            Query = "UPDATE [dbo].[NotaAdjustmentH] SET TransStatus = '01' where AdjustID ='" + AdjustID + "';";

                            MessageBox.Show("NotaAdjustNumber = " + AdjustID.ToUpper() + "\n" + "Data berhasil dibatalkan.");

                            Index = 0;
                            Trans.Commit();
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        

        

    }
}
