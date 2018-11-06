using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.NotaReturBeli
{
    public partial class NRBApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        bool Journal = false;

        string Mode, Status, Query, crit, NRBNumber, GRID = null;
        Purchase.NotaReturBeli.InqNRBApproval Parent;
        ContextMenu vendid = new ContextMenu();

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        
        public NRBApproval()
        {
            InitializeComponent();
        }

        private void NRBApproval_Load(object sender, EventArgs e)
        {
            btnRevision.Visible = false;
            AddJenisRetur();
            ModeApprove();
            GetDataHeader();
        }

        public void setParent(Purchase.NotaReturBeli.InqNRBApproval f)
        {
            Parent = f;
        }

        public void SetMode(string tmpMode, string tmpNRBNumber)
        {
            Mode = tmpMode;
            NRBNumber = tmpNRBNumber;
        }

        public void CreateGrid()
        {
            dgvNRB.Rows.Clear();
            if (dgvNRB.RowCount - 1 <= 0)
            {
                dgvNRB.ColumnCount = 18;
                dgvNRB.Columns[0].Name = "No";
                dgvNRB.Columns[1].Name = "ItemID";
                dgvNRB.Columns[2].Name = "FullItemID"; dgvNRB.Columns["FullItemID"].HeaderText = "Item ID";
                dgvNRB.Columns[3].Name = "ItemName"; dgvNRB.Columns["ItemName"].HeaderText = "Name";
                dgvNRB.Columns[4].Name = "GroupId";
                dgvNRB.Columns[5].Name = "SubGroup1ID";
                dgvNRB.Columns[6].Name = "SubGroup2ID";
                dgvNRB.Columns[7].Name = "Qty_GR";
                dgvNRB.Columns[8].Name = "UoM_Qty";
                dgvNRB.Columns[9].Name = "UoM_Unit";
                dgvNRB.Columns[10].Name = "Alt_Qty";
                dgvNRB.Columns[11].Name = "Alt_Unit";
                dgvNRB.Columns[12].Name = "Ratio";
                dgvNRB.Columns[13].Name = "Ratio_Actual";
                dgvNRB.Columns[14].Name = "GoodsReceivedSeqNo";
                dgvNRB.Columns[15].Name = "InventSiteId";
                dgvNRB.Columns[16].Name = "Quality";
                dgvNRB.Columns[17].Name = "Notes";

                DataGridViewComboBoxColumn JenisRetur = new DataGridViewComboBoxColumn();
                JenisRetur.Name = "JenisRetur";
                JenisRetur.HeaderText = "      Jenis Retur       ";
                JenisRetur.Items.Add("Retur Tukar Barang");// code: 01
                JenisRetur.Items.Add("Retur Debet Note");// code: 02
                this.dgvNRB.Columns.Add(JenisRetur);
            }
        }

        private void AddJenisRetur()
        {
            cmbJenisRetur.Items.Add("---- Jenis Retur ----");
            cmbJenisRetur.Items.Add("Retur Tukar Barang");
            cmbJenisRetur.Items.Add("Retur Debet Note");
            cmbJenisRetur.SelectedIndex = 0;
        }

        public void ModeApprove()
        {
            Mode = "Approve";
            txtNotes.Enabled = false;
            cmbJenisRetur.Enabled = false;
            dgvNRB.ReadOnly = true;
            //tia edit
            txtVendID.Enabled = true;
            txtVendName.Enabled = true;
            txtPONum.Enabled = true;
            txtGRNum.Enabled = true;

            txtVendID.ReadOnly = true;
            txtVendName.ReadOnly = true;
            txtPONum.ReadOnly = true;
            txtGRNum.ReadOnly = true;

            txtVendID.ContextMenu = vendid;
            txtVendName.ContextMenu = vendid;
            txtPONum.ContextMenu = vendid;
            txtGRNum.ContextMenu = vendid;
            //tia end
            dgvNRB.DefaultCellStyle.BackColor = Color.LightGray;
            //gbApprove.Visible = true;
        }

        public void ModeAfterApproveOrReject()
        {
            btnApprove.Visible = false;
            btnReject.Visible = false;
            btnRevision.Visible = false;
        }

        public void GetDataHeader()
        {
            if (NRBNumber != "")
            {
                dgvNRB.Rows.Clear();
                Conn = ConnectionString.GetConnection();
                Query = "SELECT TOP 1 NRBH.NRBId, NRBH.NRBDate, NRBH.GoodsReceivedID, NRBH.PurchId, NRBH.VendID, NRBH.VendName, NRBH.SiteId, INS.InventSiteName, INS.Lokasi, NRBH.Notes, NRBH.TransStatusId, TST.Deskripsi, NRBH.ApprovedBy, ActionCode ";
                Query += "FROM NotaReturBeliH NRBH LEFT JOIN InventSite INS ON INS.InventSiteID = NRBH.SiteId LEFT JOIN TransStatusTable TST ON NRBH.TransStatusId = TST.StatusCode And TST.TransCode = 'NotaReturBeli' ";
                Query += "WHERE NRBId = '" + NRBNumber + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    //txtGRNum.Text = Dr["NRBId"].ToString();
                    dtNRB.Text = Dr["NRBDate"].ToString();
                    txtNRBNum.Text = NRBNumber;
                    txtGRNum.Text = Dr["GoodsReceivedID"].ToString();
                    txtVendID.Text = Dr["VendID"].ToString();
                    txtVendName.Text = Dr["VendName"].ToString();
                    txtSiteID.Text = Dr["SiteId"].ToString();
                    txtSiteName.Text = Dr["InventSiteName"].ToString();
                    txtSiteLocation.Text = Dr["Lokasi"].ToString();
                    txtNotes.Text = Dr["Notes"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString();
                    txtApproved.Text = Dr["ApprovedBy"].ToString();
                    txtPONum.Text = Dr["PurchId"].ToString();
                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        cmbJenisRetur.SelectedIndex = 1;
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        cmbJenisRetur.SelectedIndex = 2;
                    }

                    //if (Dr["TransStatusId"].ToString() == "03" || Dr["TransStatusId"].ToString() == "04" || Dr["TransStatusId"].ToString() == "05")
                    //{
                    //    gbApprove.Visible = false;
                    //}
                }
                Dr.Close();

                CreateGrid();

                Query = "SELECT NRBId, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, RemainingQty, GoodsReceivedId, GoodsReceived_SeqNo, UoM_Qty, Alt_Qty, UoM_Unit, Alt_Unit, Ratio, NRBD.InventSiteId, ISB.InventSiteBlokID, ActionCode, Notes, Ratio_Actual, Quality ";
                Query += "FROM NotaReturBeli_Dtl NRBD LEFT JOIN InventSiteBlok ISB ON ISB.InventSiteID = NRBD.InventSiteId WHERE NRBId = '" + NRBNumber + "' ORDER BY SeqNo ASC";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    Query = "SELECT Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + txtGRNum.Text + "' AND FullItemID = '" + Dr["FullItemID"] + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    this.dgvNRB.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], decimal.Parse(Cmd.ExecuteScalar().ToString()) + decimal.Parse(Dr["UoM_Qty"].ToString()), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceived_SeqNo"], Dr["InventSiteId"], Dr["Quality"], Dr["Notes"]);
                    if (Dr["ActionCode"].ToString() == "01")
                    {
                        dgvNRB.Rows[j].Cells["JenisRetur"].Value = "Retur Tukar Barang";
                    }
                    else if (Dr["ActionCode"].ToString() == "02")
                    {
                        dgvNRB.Rows[j].Cells["JenisRetur"].Value = "Retur Debet Note";
                    }
                    j++;
                }
                Dr.Close();

                dgvNRB.ReadOnly = true;
                dgvNRB.Columns["No"].ReadOnly = true;
                dgvNRB.Columns["FullItemID"].ReadOnly = true;
                dgvNRB.Columns["ItemName"].ReadOnly = true;
                dgvNRB.Columns["Qty_GR"].ReadOnly = true;
                dgvNRB.Columns["UoM_Qty"].ReadOnly = false;
                dgvNRB.Columns["UoM_Unit"].ReadOnly = true;
                dgvNRB.Columns["Alt_Qty"].ReadOnly = true;
                dgvNRB.Columns["Alt_Unit"].ReadOnly = true;
                dgvNRB.Columns["Ratio"].ReadOnly = true;
                dgvNRB.Columns["Ratio_Actual"].ReadOnly = true;
                dgvNRB.Columns["InventSiteId"].ReadOnly = true;
                dgvNRB.Columns["Quality"].ReadOnly = true;
                dgvNRB.Columns["Notes"].ReadOnly = true;

                dgvNRB.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["InventSiteId"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Quality"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvNRB.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvNRB.Columns["Qty_GR"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvNRB.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvNRB.Columns["ItemID"].Visible = false;
                dgvNRB.Columns["GroupId"].Visible = false;
                dgvNRB.Columns["SubGroup1ID"].Visible = false;
                dgvNRB.Columns["SubGroup2ID"].Visible = false;
                dgvNRB.Columns["Ratio"].Visible = false;
                dgvNRB.Columns["GoodsReceivedSeqNo"].Visible = false;
                dgvNRB.Columns["JenisRetur"].Visible = false;

                dgvNRB.AutoResizeColumns();

                Conn.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                try
                {
                    DialogResult dr = MessageBox.Show("NRB No = " + NRBNumber + "\n" + "Apakah data diatas akan diapprove ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Query = "UPDATE NotaReturBeliH SET ";
                        Query += "TransStatusId = '04', ";
                        Query += "ApprovedBy = '" + ControlMgr.UserId + "', ";
                        Query += "UpdatedDate = GETDATE(), ";
                        Query += "UpdatedBy = '" + ControlMgr.UserId + "' ";
                        Query += "WHERE NRBId = '" + txtNRBNum.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        List<decimal> NRBSeqNo = new List<decimal>();
                        List<string> FullItemID = new List<string>();
                        List<decimal> UoM_Qty = new List<decimal>();
                        List<decimal> Alt_Qty = new List<decimal>();
                        Query = "SELECT SeqNo, GoodsReceived_SeqNo, GoodsReceivedId, FullItemId, UoM_Qty, Alt_Qty FROM NotaReturBeli_Dtl WHERE NRBId='" + txtNRBNum.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            NRBSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                            FullItemID.Add(Dr["FullItemId"].ToString());
                            UoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                            Alt_Qty.Add(decimal.Parse(Dr["Alt_Qty"].ToString()));
                        }
                        Dr.Close();

                        for (int i = 0; i < NRBSeqNo.Count; i++)
                        {                               
                            Query = "";
                            //Get Price With GRid and FullItemId
                            Query = "SELECT POD.Price ";
                            Query += "FROM GoodsReceivedD GRD ";
                            Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                            Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                            Query += "WHERE GRD.GoodsReceivedId = '" + txtGRNum.Text + "' AND POD.FullItemId = '" + FullItemID[i] + "' ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string getUoM_Qty = UoM_Qty[i].ToString();
                            string getAlt_Qty = Alt_Qty[i].ToString();
                            string Price = Cmd.ExecuteScalar().ToString();
                            decimal amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                            decimal amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                            Query = "INSERT INTO NotaReturBeli_LogTable ";
                            Query += "(NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, ";
                            Query += "VendId, SiteId, FullItemId, SeqNo, ";
                            Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                            Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                            Query += "('" + dtNRB.Value.ToString("yyyy-MM-dd") + "', '" + NRBNumber + "', '" + dtGR.Value.ToString("yyyy-MM-dd") + "', '" + txtGRNum.Text + "', ";
                            Query += "'" + txtVendID.Text + "', '" + txtSiteID.Text + "', '" + FullItemID[i] + "', '" + dgvNRB.Rows[i].Cells["No"].Value.ToString() + "', ";
                            Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amountPU + "', '04', ";
                            Query += "'Approved by Purchasing Manager', 'Status: 04. Approved by Purchasing Manager', '" + ControlMgr.UserId + "', GETDATE()) ";
                            
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }                   

                    Trans.Commit();
                    MessageBox.Show("Data NRBNumber : " + txtNRBNum.Text + " berhasil diapprove.");
                    //this.Close();
                    Parent.RefreshGrid();
                    ModeAfterApproveOrReject();
                }
                catch (Exception Ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(Ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date : 06 Sept 2018
            //Desc : Create Journal
            //string NRBNumber = txtNRBNum.Text;
            //string PONumber = txtPONum.Text;
            decimal Retur = 0;
            //decimal POPPN = 0, POPPH = 0, Tax = 0;
            //string GRNumber = txtGRNum.Text;

            //POPPN = GetPOTax(PONumber, "PPN");
            //POPPH = GetPOTax(PONumber, "PPH");

            for (int i = 0; i < dgvNRB.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNRB.Rows[i].Cells["FullItemID"].Value);
                decimal Qty = Convert.ToDecimal(dgvNRB.Rows[i].Cells["UoM_Qty"].Value);
                int GoodsReceivedSeqNo = Convert.ToInt32(dgvNRB.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                decimal Price = GetNRBPrice(FullItemID, GoodsReceivedSeqNo);
                Retur = Retur + (Qty * Price);

                //string POPrice = GetPOPrice(GRNumber, JournalGRSeqNo);
                //if (POPrice == "")
                //{
                //    POPrice = "1";
                //}

                //Retur = Retur + (Convert.ToDecimal(POPrice) * JournalQty);


                //if (POPPN != 0)
                //{
                //    Tax = Tax + ((Retur * POPPN) / 100);
                //}

                //if (POPPH != 0)
                //{
                //    Tax = Tax + ((Retur * POPPH) / 100);
                //}

                //if (Unit.ToUpper() == "KG")
                //{
                //    DAvailable = DAvailable + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //    KAvailable = KAvailable + GetPriceFromSO(FullItemID);
                //}
                //else
                //{
                //    DAvailable = DAvailable + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //    KAvailable = KAvailable + GetPriceFromSO(FullItemID);
                //}
            }

            string JournalHID = "IN49";
            string Notes = txtGRNum.Text + " - " + txtVendName.Text;
            string Jenis = "JN", Kode = "JN";
            string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);

            Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
            Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtNRBNum.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();

            //Select Config Journal
            int SeqNo = 1;
            int JournalIDSeqNo = 0;
            string Type = "";
            string FQA_ID = "";
            string FQA_Desc = "";
            decimal AmountValue;

            Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                Type = Convert.ToString(Dr["Type"]);
                FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                AmountValue = 0;

                if (JournalHID == "IN49")
                {
                    if (JournalIDSeqNo == 1)
                    {
                        AmountValue = Retur;
                    }
                    else if (JournalIDSeqNo == 2)
                    {
                        AmountValue = Retur;
                    }
                }

                if (AmountValue == 0)
                {
                    continue;
                }

                //Insert Detail GLJournal
                Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
                SeqNo++;
            }
            Dr.Close();


            //string JournalHID = "IN42";
            //string Notes = "";
            //string Jenis = "JN", Kode = "JN";
            //string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Trans, Cmd);

            //Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
            //Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + NRBNumber + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
            //Cmd = new SqlCommand(Query, Conn, Trans);
            //Cmd.ExecuteNonQuery();

            ////Select Config Journal
            //int SeqNo = 1;
            //int JournalIDSeqNo = 0;
            //string Type = "";
            //string FQA_ID = "";
            //string FQA_Desc = "";
            //decimal AmountValue;

            //Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            //Cmd = new SqlCommand(Query, Conn, Trans);
            //Dr = Cmd.ExecuteReader();
            //while (Dr.Read())
            //{
            //    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
            //    Type = Convert.ToString(Dr["Type"]);
            //    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
            //    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
            //    AmountValue = 0;

            //    if (JournalIDSeqNo == 1)
            //    {
            //        if (cmbJenisRetur.SelectedItem.ToString() == "Retur Debet Note")
            //        {
            //            AmountValue = Retur;
            //        }
            //    }
            //    else if (JournalIDSeqNo == 2)
            //    {
            //        if (cmbJenisRetur.SelectedItem.ToString() == "Retur Tukar Barang")
            //        {
            //            AmountValue = Retur;
            //        }
            //    }
            //    else if (JournalIDSeqNo == 3)
            //    {
            //        AmountValue = Retur;
            //    }
            //    else if (JournalIDSeqNo == 4)
            //    {
            //        //AmountValue = Tax;
            //    }
            //    else if (JournalIDSeqNo == 5)
            //    {
            //        if (!NRBNumber.ToUpper().Contains("NRBA"))
            //        {

            //        }

            //        //AmountValue = Parked;
            //    }
            //    else if (JournalIDSeqNo == 6)
            //    {
            //        if (!NRBNumber.ToUpper().Contains("NRBA"))
            //        {

            //        }
            //    }

            //    if (AmountValue == 0)
            //    {
            //        continue;
            //    }

            //    //Insert Detail GLJournal
            //    Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
            //    Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
            //    Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
            //    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
            //    Cmd = new SqlCommand(Query, Conn, Trans);
            //    Cmd.ExecuteNonQuery();
            //    SeqNo++;
            //}
            //Dr.Close();
            //End
        }

        private decimal GetNRBPrice(string FullItemId, int GoodsReceivedSeqNo)
        {
            //GET Price
            Query = "SELECT Price FROM NotaReturBeli_Dtl WHERE NRBId = '" + txtNRBNum.Text + "' AND FullItemId = '" + FullItemId + "' AND GoodsReceivedId = '" + txtGRNum.Text + "' AND GoodsReceived_SeqNo = " + GoodsReceivedSeqNo + "";

            Cmd = new SqlCommand(Query, Conn, Trans);
            string result = Convert.ToString(Cmd.ExecuteScalar());
            decimal Price = 0;
            if (result == "")
            {
                Price = 0;
            }
            else
            {
                Price = Convert.ToDecimal(result);
            }

            return Price;
        }

        //private decimal GetPOTax(string PONumber, string FieldName)
        //{
        //    //GET Tax
        //    Query = "SELECT " + FieldName + " FROM PurchH WHERE PurchID = '" + PONumber + "'";

        //    Cmd = new SqlCommand(Query, Conn, Trans);
        //    string result = Convert.ToString(Cmd.ExecuteScalar());
        //    decimal Tax = 0;
        //    if (result == "")
        //    {
        //        Tax = 0;
        //    }
        //    else
        //    {
        //        Tax = Convert.ToDecimal(result);
        //    }

        //    return Tax;
        //}

        //private string GetPOPrice(string GRNumber, string GRSeqNo)
        //{
        //    //GET PricePO
        //    Query = " SELECT PO.Price FROM GoodsReceivedD GR ";
        //    Query += "INNER JOIN ReceiptOrderD RO ON GR.RefTransID = RO.ReceiptOrderId ";
        //    Query += "INNER JOIN PurchDtl PO ON RO.PurchaseOrderId = PO.PurchID ";
        //    Query += "WHERE GR.GoodsReceivedId = '" + GRNumber + "' ";
        //    Query += "AND GR.GoodsReceivedSeqNo = '" + GRSeqNo + "' ";
        //    Query += "AND GR.RefTransSeqNo = RO.SeqNo ";
        //    Query += "AND RO.PurchaseOrderSeqNo = PO.SeqNo ";

        //    Cmd = new SqlCommand(Query, Conn, Trans);
        //    string Price = Convert.ToString(Cmd.ExecuteScalar());

        //    return Price;
        //}

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                try
                {
                    DialogResult dr = MessageBox.Show("NRB No = " + NRBNumber + "\n" + "Apakah data diatas akan direject ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        Query = "UPDATE NotaReturBeliH SET ";
                        Query += "TransStatusId = '05', ";
                        Query += "ApprovedBy = '" + ControlMgr.UserId + "', ";
                        Query += "UpdatedBy = '" + ControlMgr.UserId + "', ";
                        Query += "UpdatedDate = GETDATE() ";
                        Query += "WHERE NRBId = '" + txtNRBNum.Text.Trim() + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        //Update Qty
                        List<decimal> NRBSeqNo = new List<decimal>();
                        List<string> GoodsReceivedSeqNo = new List<string>();
                        List<string> GoodsReceivedID = new List<string>();
                        List<string> FullItemID = new List<string>();
                        List<decimal> UoM_Qty = new List<decimal>();
                        List<decimal> Alt_Qty = new List<decimal>();
                        decimal RemainingQty, QtyNew = 0;
                        Query = "SELECT SeqNo, GoodsReceived_SeqNo, GoodsReceivedId, FullItemId, UoM_Qty, Alt_Qty FROM NotaReturBeli_Dtl WHERE NRBId='" + txtNRBNum.Text + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            NRBSeqNo.Add(decimal.Parse(Dr["SeqNo"].ToString()));
                            GoodsReceivedSeqNo.Add(Dr["GoodsReceived_SeqNo"].ToString());
                            GoodsReceivedID.Add(Dr["GoodsReceivedId"].ToString());
                            FullItemID.Add(Dr["FullItemId"].ToString());
                            UoM_Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                            Alt_Qty.Add(decimal.Parse(Dr["Alt_Qty"].ToString()));
                        }
                        Dr.Close();

                        for (int i = 0; i < GoodsReceivedSeqNo.Count; i++)
                        {
                            Query = "SELECT ISNULL(Remaining_Qty, 0)AS Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                            QtyNew = RemainingQty + UoM_Qty[i];
                            //Update Remaining Qty diGR
                            Query = "Update GoodsReceivedD set ";
                            Query += "Remaining_Qty='" + QtyNew + "' ";
                            Query += "where GoodsReceivedId='" + GoodsReceivedID[i] + "' And GoodsReceivedSeqNo='" + GoodsReceivedSeqNo[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            Query = "";
                            //Get Price With GRid and FullItemId
                            Query = "SELECT POD.Price ";
                            Query += "FROM GoodsReceivedD GRD ";
                            Query += "LEFT JOIN ReceiptOrderD ROD ON ROD.ReceiptOrderId=GRD.RefTransID AND ROD.SeqNo=GRD.RefTransSeqNo ";
                            Query += "LEFT JOIN PurchDtl POD ON POD.PurchID=ROD.PurchaseOrderId AND ROD.PurchaseOrderSeqNo=POD.SeqNo ";
                            Query += "WHERE GRD.GoodsReceivedId = '" + txtGRNum.Text + "' AND POD.FullItemId = '" + FullItemID[i] + "' ";
                            Cmd = new SqlCommand(Query, Conn, Trans);

                            ////Cek Price Ada dalam list atau tidak
                            //if (Cmd.ExecuteScalar() == null)
                            //{
                            //    MessageBox.Show("Item No. " + dgvNRB.Rows[i].Cells["No"].Value + ", item tidak terdapat dalam list Pembelian.");
                            //    return;
                            //}

                            string getUoM_Qty = UoM_Qty[i].ToString();
                            string getAlt_Qty = Alt_Qty[i].ToString();
                            string POPrice = Convert.ToString(Cmd.ExecuteScalar());
                            string Price = "";
                            if(POPrice == "")
                            {
                                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + FullItemID[i] + "'";
                                using (Cmd = new SqlCommand(Query, Conn, Trans))
                                {
                                    Price = Cmd.ExecuteScalar().ToString();
                                }
                            }
                            else
                            {
                                Price = POPrice;
                            }
                           

                            decimal amountPU = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());
                            decimal amountOH = decimal.Parse(Price) * decimal.Parse(getUoM_Qty.ToString());

                            Query = "";
                            //Get Retur_Beli_In_Progress_UoM, Retur_Beli_In_Progress_Alt, Retur_Beli_In_Progress_Amount (Old)
                            Query = "SELECT Retur_Beli_In_Progress_UoM FROM Invent_Purchase_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIP_UoM_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Retur_Beli_In_Progress_Alt FROM Invent_Purchase_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIP_Alt_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Retur_Beli_In_Progress_Amount FROM Invent_Purchase_Qty WHERE FullItemID = '" + FullItemID[i] + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIP_Amount_Old = Cmd.ExecuteScalar().ToString();

                            decimal RBIP_UoM_New = decimal.Parse(RBIP_UoM_Old.ToString()) - decimal.Parse(getUoM_Qty.ToString());
                            decimal RBIP_Alt_New = decimal.Parse(RBIP_Alt_Old.ToString()) - decimal.Parse(getAlt_Qty.ToString());
                            decimal RBIP_Amount_New = decimal.Parse(RBIP_Amount_Old.ToString()) - decimal.Parse(amountPU.ToString());
                            Query = "";
                            //Update Invent_Purchase_Qty
                            Query = "UPDATE Invent_Purchase_Qty SET ";
                            Query += "Retur_Beli_In_Progress_UoM = '" + RBIP_UoM_New + "', ";
                            Query += "Retur_Beli_In_Progress_Alt = '" + RBIP_Alt_New + "', ";
                            Query += "Retur_Beli_In_Progress_Amount = '" + RBIP_Amount_New + "' ";
                            Query += "WHERE FullItemId = '" + FullItemID[i] + "' ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            Query = "";
                            //Get Available_For_Sale_UoM,Available_For_Sale_Alt,Available_For_Sale_Amount (Old)
                            Query = "SELECT Available_For_Sale_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIO_UoM_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Available_For_Sale_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIO_Alt_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Available_For_Sale_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIO_Amount_Old = Cmd.ExecuteScalar().ToString();

                            //Get Available_UoM, Available_Alt, Available_Amount (Old)
                            Query = "SELECT Available_UoM FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIO_Av_UoM_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Available_Alt FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIO_Av_Alt_Old = Cmd.ExecuteScalar().ToString();
                            Query = "SELECT Available_Amount FROM Invent_OnHand_Qty WHERE FullItemID = '" + FullItemID[i] + "' AND InventSiteId = '" + txtSiteID.Text + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            string RBIO_Av_Amount_Old = Cmd.ExecuteScalar().ToString();

                            decimal RBIO_UoM_New = decimal.Parse(RBIO_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                            decimal RBIO_Alt_New = decimal.Parse(RBIO_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                            decimal RBIO_Amount_New = decimal.Parse(RBIO_Amount_Old.ToString()) + decimal.Parse(amountOH.ToString());

                            decimal RBIO_Av_UoM_New = decimal.Parse(RBIO_Av_UoM_Old.ToString()) + decimal.Parse(getUoM_Qty.ToString());
                            decimal RBIO_Av_Alt_New = decimal.Parse(RBIO_Av_Alt_Old.ToString()) + decimal.Parse(getAlt_Qty.ToString());
                            decimal RBIO_Av_Amount_New = decimal.Parse(RBIO_Av_Amount_Old.ToString()) + decimal.Parse(amountOH.ToString());
                            
                            Query = "";
                            //Update Invent_OnHand_Qty
                            Query = "UPDATE Invent_OnHand_Qty SET ";
                            Query += "Available_For_Sale_UoM = '" + RBIO_UoM_New + "', ";
                            Query += "Available_For_Sale_Alt = '" + RBIO_Alt_New + "', ";
                            Query += "Available_For_Sale_Amount = '" + RBIO_Amount_New + "', ";
                            Query += "Available_UoM = '" + RBIO_Av_UoM_New + "', ";
                            Query += "Available_Alt = '" + RBIO_Av_Alt_New + "', ";
                            Query += "Available_Amount = '" + RBIO_Av_Amount_New + "' ";
                            Query += "WHERE FullItemId = '" + FullItemID[i] + "' AND InventSiteId = '" + txtSiteID.Text + "' ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                            
                            Query = "INSERT INTO NotaReturBeli_LogTable ";
                            Query += "(NRBDate, NRBId, GoodsReceivedDate, GoodsReceivedId, ";
                            Query += "VendId, SiteId, FullItemId, SeqNo, ";
                            Query += "Qty_UoM, Qty_Alt, Amount, LogStatusCode, ";
                            Query += "LogStatusDesc, LogDescription, UserID, LogDate) VALUES ";
                            Query += "('" + dtNRB.Value.ToString("yyyy-MM-dd") + "', '" + NRBNumber + "', '" + dtGR.Value.ToString("yyyy-MM-dd") + "', '" + txtGRNum.Text + "', ";
                            Query += "'" + txtVendID.Text + "', '" + txtSiteID.Text + "', '" + FullItemID[i] + "', '" + dgvNRB.Rows[i].Cells["No"].Value.ToString() + "', ";
                            Query += "'" + getUoM_Qty + "', '" + getAlt_Qty + "', '" + amountPU + "', '05', ";
                            Query += "'Rejected', 'Status: 05. Rejected', '" + ControlMgr.UserId + "', GETDATE()) ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }

                        //Begin
                        //Created By : Joshua
                        //Created Date : 16 Aug 2018
                        //Desc : Create Journal
                        CreateJournal();
                        //if (Journal == true)
                        //{
                        //    Journal = false;
                        //    goto Outer;
                        //}
                        //End
                    }

                    Trans.Commit();
                    MessageBox.Show("Data NRBNumber : " + txtNRBNum.Text + " berhasil direject.");
                    //this.Close();

                     //Outer: ;

                    Parent.RefreshGrid();
                    ModeAfterApproveOrReject();
                }
                catch (Exception Ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(Ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnRevision_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "UPDATE NotaReturBeliH SET ";
                    Query += "TransStatusId = '02', ";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "', ";
                    Query += "UpdatedBy = '" + ControlMgr.UserId + "', ";
                    Query += "UpdatedDate = GETDATE() ";
                    Query += "WHERE NRBId = '" + txtNRBNum.Text.Trim() + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data NRBNumber : " + txtNRBNum.Text + " berhasil diupdate.");
                    this.Close();
                }
                catch (Exception Ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(Ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }
        //Tia 26062018
        //klik kanan
        PopUp.Vendor.Vendor Vend = null;
        PopUp.InventSite Inventsite = null;
        Purchase.PurchaseOrderNew.POForm PONumber = null;
        PopUp.FullItemId.FullItemId FID = null;
        ISBS_New.Purchase.GoodsReceipt.GRHeaderV2 Gr = null;

        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvNRB_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        itemID = dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
                if (Inventsite == null || Inventsite.Text == "")
                {
                    if (dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "InventSiteId")
                    {
                        Inventsite = new PopUp.InventSite();
                        Inventsite.GetData(dgvNRB.Rows[e.RowIndex].Cells["InventSiteId"].Value.ToString(), dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        Inventsite.Show();
                    }
                }
                else if (CheckOpened(Inventsite.Name))
                {
                    Inventsite.WindowState = FormWindowState.Normal;
                    Inventsite.GetData(dgvNRB.Rows[e.RowIndex].Cells["InventSiteId"].Value.ToString(), dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    Inventsite.Show();
                    Inventsite.Focus();
                }
            }
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

        private void txtVendID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtVendID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtVendID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }
        }

        private void txtVendName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtVendID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtVendID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }
        }

        private void txtPONum_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (PONumber == null || PONumber.Text == "")
                {
                    txtPONum.Enabled = true;
                    PONumber = new Purchase.PurchaseOrderNew.POForm();
                    PONumber.SetMode("PopUp", txtPONum.Text, "");
                    PONumber.ParentRefreshGrid3(this);
                    PONumber.Show();
                }
                else if (CheckOpened(PONumber.Name))
                {
                    PONumber.WindowState = FormWindowState.Normal;
                    PONumber.Show();
                    PONumber.Focus();
                }
            }
        }

        private void txtGRNum_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Gr == null || Gr.Text == "")
                {
                    Gr = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                    Gr.SetMode("PopUp", txtGRNum.Text);
                    Gr.ParentRefreshGrid3(this);
                    Gr.Show();
                    //}
                }
                else if (CheckOpened(Gr.Name))
                {
                    Gr.WindowState = FormWindowState.Normal;
                    Gr.Show();
                    Gr.Focus();
                }
            }
        }
        //end
    }
}
