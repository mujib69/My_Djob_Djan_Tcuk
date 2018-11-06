using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Transactions;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.GoodsReceipt.Resize
{
    public partial class FormResizeGR : MetroFramework.Forms.MetroForm
    {
        string Query = "";
        const int x = 0;

        //MODE DIGUNAKAN UNTUK NEW, EDIT, DELETE
        string Mode = "";

        //DIGUNAKAN UNTUK SET CHILD
        private List<Form> MDI = new List<Form>();

        //VARIABEL BACKGROUND UNTUK SAVE DATA
        private string ResizeType;
        //InquiryV1 Parent = new InquiryV1();

        string siteid = "";
        int posted;
        //tia edit
        ContextMenu CM = new ContextMenu();
        PopUp.FullItemId.FullItemId FullItemId = new PopUp.FullItemId.FullItemId();
        //tia adit end
        GlobalInquiry Parent = new GlobalInquiry();
        public void SetParent(GlobalInquiry F) { Parent = F; }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public void SetMode(string tmpMode, string tmpNumber, string tmpRefNumber, DateTime tmpDate)
        {
            Mode = tmpMode;
            txtTransNo.Text = tmpNumber;
            txtReffTransID.Text = tmpRefNumber;
            dtTransDate.Value = tmpDate.Date;
            //tia edit
            txtReffTransID.Enabled = true;
            txtReffTransID.ReadOnly = true;
            txtReffTransID.ContextMenu = CM;
            //tia edit end
            ViewData();
            ModeView();
        }

        private void ModeNew()
        {
            Mode = "NEW";
            this.Text = "Form GR Resize (" + Mode + ")";
            this.Refresh();
            NonActivationControl(true);
            btnEdit.Enabled = false;
            btnCancel.Enabled = false;
        }

        private void ModeView()
        {
            Mode = "VIEW";
            this.Text = "Form GR Resize (" + Mode + ")";
            this.Refresh();
            NonActivationControl(false);
            btnEdit.Enabled = true;
            btnExit.Enabled = true;
            dgvPrDetails.Columns["Action"].Visible = false;
        }

        private void ModeEdit()
        {
            Mode = "EDIT";
            this.Text = "Form GR Resize (" + Mode + ")";
            this.Refresh();
            NonActivationControl(true);
            btnEdit.Enabled = false;
            btnExit.Enabled = false;
            dgvPrDetails.Columns["Action"].Visible = true;
        }

        private void NonActivationControl(bool TmpBool)
        { 
            using(Method C = new Method ())
            {
                C.ControlNonActivation(this, TmpBool);
            }
        }

        public FormResizeGR()
        {
            InitializeComponent();
        }

        private void FormResizeGR_Load(object sender, EventArgs e)
        {
            Location = new Point(100, 48);
            //if (Variable.Kode != null)
            //{
            //    txtTransNo.Text = Variable.Kode[0];
            //    siteid = Variable.Kode[2];
            //    ViewData();
            //    ModeView();
            //}
            //else
            //{
            //    ModeNew();
            //}
            Query = "SELECT [SiteID] FROM [dbo].[NotaResizeH] WHERE [NRZId]=@NRZId ;";
            using (SqlCommand cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@NRZId",txtTransNo.Text);
                siteid = cmd.ExecuteScalar() == System.DBNull.Value ? "" : cmd.ExecuteScalar().ToString();
            }
            dgvPrDetails.AutoResizeColumns();
            SqlConnection con = ConnectionString.GetConnection();
            lblposted.Text = checkPosted(con).ToString() == "1" ? "Current Status : Posted" : "Current Status : Unposted";
            con.Close();
        }

        private void getSubSeqNo()
        {
            Query = "SELECT (RIGHT(SeqNo, 2)) FROM NotaResize_Dtl WHERE NRZId = '"+txtTransNo.Text+"'";
        }
 
        private void ViewData()
        {
            if (dgvPrDetails.Rows.Count > 0)
            {
                dgvPrDetails.Columns.Clear();
            }
            using (Method C = new Method ())
            {
                //CREATE HEADER
                Query = "SELECT [NRZId],[NRZDate],[GoodsReceivedId],[Posted],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy] FROM [NotaResizeH] Where [NRZId] = '" + txtTransNo.Text.Trim() + "'";

                C.Dr = C.ReturnDr(Query);
                while (C.Dr.Read())
                {
                    txtTransNo.Text = C.Dr["NRZId"].ToString();
                    dtTransDate.Text = C.Dr["NRZDate"].ToString();
                    txtReffTransID.Text = C.Dr["GoodsReceivedId"].ToString().ToUpper();
                    //ResizeType = C.Dr["ResizeType"].ToString();
                    chkPosted.Checked = Boolean.Parse(C.Dr["Posted"].ToString());
                }
                C.Dr.Close();
                dgvPrDetails.Rows.Clear();

                DataGridViewButtonCell button = new DataGridViewButtonCell();
                button.Value = "New Button Value";

                C.DgvCreate(dgvPrDetails, new string[] { "Action", "No", "From_FullItemId", "From_ItemName", "Qty", "Unit", "To_FullItemId", "To_ItemName", "Notes", "Notes2", "Price", "SeqNo", "CreatedDate", "CreatedBy", "LineAmount", "GoodsReceiveSeqNo" });


                //CREATE DETAIL
                //Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[Qty],[Unit],[Base],[VendID] Vendor,[DeliveryMethod],[ReffTransID] SalesSO,[ExpectedDateFrom],[ExpectedDateTo],[Deskripsi],GroupId,SubGroup1Id,SubGroup2Id,ItemId,GelombangId,BracketId,Price,SeqNoGroup,BracketDesc From [PurchRequisition_Dtl] Where PurchReqID = '" + txtTransNo.Text.Trim() + "' order by SeqNo asc";
                Query = "Select SeqNo No, FromFullItemId From_FullItemId, FromItemName From_ItemName, Qty, Unit, ToFullItemId To_FullItemId, ToItemName To_ItemName, Notes1 Notes, Notes2 Notes2, Price, SeqNo, CreatedDate, CreatedBy, LineAmount, GoodsReceiveSeqNo from NotaResize_Dtl where NRZId ='" + txtTransNo.Text.Trim() + "'";
                C.Dr = C.ReturnDr(Query);
                
                while (C.Dr.Read())
                {
                    //string ExpectedDateFrom = Convert.ToDateTime(C.Dr[9]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(C.Dr[9]).ToString("dd-MM-yyyy");
                    //string ExpectedDateTo = Convert.ToDateTime(C.Dr[10]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(C.Dr[10]).ToString("dd-MM-yyyy");
                    dgvPrDetails.Rows.Add("button",C.Dr[0], C.Dr[1], C.Dr[2], C.Dr[3], C.Dr[4], C.Dr[5], C.Dr[6], C.Dr[7], C.Dr[8], C.Dr[9], C.Dr[10], C.Dr[11],C.Dr[12],C.Dr[13],C.Dr[14]);//, "", C.Dr[7], C.Dr[8], C.Dr[9], C.Dr[10], C.Dr[11], C.Dr[12], C.Dr[13], C.Dr[14]);
                }

                dgvPrDetails.ReadOnly = false;
                C.DgvReadOnly(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Unit", "To_ItemName", "Notes", "CreatedDate", "CreatedBy", "LineAmount", "GoodsReceiveSeqNo" });
                C.DgvColor(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Unit", "To_ItemName", "Notes", "CreatedDate", "CreatedBy", "LineAmount", "GoodsReceiveSeqNo" }, Color.LightGray);

                for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
                {
                    if ((string)dgvPrDetails.Rows[i].Cells[0].Value == "button")
                    {
                        dgvPrDetails.Rows[i].Cells[0] = new DataGridViewButtonCell();
                        double seqno = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["No"].Value);
                        if (seqno % 1 != 0)
                        {
                            dgvPrDetails.Rows[i].Cells[0].Value = "Delete";
                        }
                        else
                        {
                            dgvPrDetails.Rows[i].Cells[0].Value = "Add New";
                            dgvPrDetails.Rows[i].DefaultCellStyle.BackColor = Color.LightBlue;
                            dgvPrDetails.Rows[i].Cells["To_FullItemId"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["Notes2"].Style.BackColor = Color.White;
                            dgvPrDetails.Rows[i].Cells["Qty"].ReadOnly = true;
                        }
                    }
                }
                
                //C.DgvColor(dgvPrDetails, new string[] { "Qty" }, Color.LightYellow);
                C.DgvNotSort(dgvPrDetails, new string[] { "Action","No", "From_FullItemId", "From_ItemName", "Qty", "Unit", "To_FullItemId", "To_ItemName", "To_ItemName", "Notes", "Notes2","CreatedDate","CreatedBy" });
                //C.DgvVisible(dgvPrDetails, new string[] { "From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId" });
                C.DgvVisible(dgvPrDetails, new string[] { "Action","Price", "SeqNo", "CreatedDate", "CreatedBy", "LineAmount", "GoodsReceiveSeqNo" });
                C.DgvAlignRight(dgvPrDetails, new string[] { "To_FullItemId" });

                dgvPrDetails.Refresh();
                dgvPrDetails.AutoResizeColumns();
            }
            ModeView();
        }


        public void passedToId(string id, string name)
        {
            dgvPrDetails.CurrentRow.Cells["To_FullItemId"].ReadOnly = true;
            passedIdPrivate(id, name);
        }

        private void passedIdPrivate(string id, string name)
        {
            dgvPrDetails.CurrentRow.Cells["To_FullItemId"].Value = id;
            dgvPrDetails.CurrentRow.Cells["To_ItemName"].Value = name;
            dgvPrDetails.CurrentRow.Cells["To_FullItemId"].ReadOnly = false;
        }

        //private void btnDelete_Click(object sender, EventArgs e)
        //{
        //    if (dgvPrDetails.RowCount > 0)
        //    {
        //        int Index = dgvPrDetails.CurrentRow.Index;
        //        using (Method C = new Method ())
        //        {
        //            C.DeleteDgv1(dgvPrDetails, Index, new string[] { "No", "From_FullItemId", "From_ItemName" });
        //        }
        //    }
        //}

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
            dtTransDate.Enabled = false;
        }


        //edited, Thaddaeus Matthias, 28 March 2018 BEGIN===================
        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvPrDetails.Rows.Count;i++ )
            {
                if (dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString().Trim() == "")
                {
                    MessageBox.Show("Pilih item tujuan pada row "+(i+1)+".");
                    return;
                }
                if (Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value) <= 0)
                {
                    MessageBox.Show("Item resize tidak boleh bernilai 0 pada row " + (i + 1) + ".");
                    return;
                }
            }

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    SqlConnection con = ConnectionString.GetConnection();
                    bool status = false;
                    bool status2 = true; //cek qty invent saat mw unpost
                    int posted = checkPosted(con); //cek status posted
                    decimal ratio = 0;

                    if (posted == 0 && chkPosted.Checked) //posting
                    {
                        for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
                        {
                            string passedid = dgvPrDetails.Rows[i].Cells["From_FullItemId"].Value.ToString();
                            string passedid2 = dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString();
                            decimal passedqty = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value);
                            decimal passedprice = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Price"].Value);
                            double passedseq = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["No"].Value);

                            decimal qtyaltTo = qtyalt(con, i, passedqty, passedid2);
                            decimal qtyaltFrom = qtyalt(con, i, passedqty, passedid);

                            //update tabel invent on hand
                            UpdateInventOnHand(con, passedqty, qtyaltTo, passedid2, passedprice, "+", i);

                            //update table invent movement
                            UpdateInventMovement(con, passedqty, qtyaltFrom, passedid, passedprice, "-");

                            insertstatuslog(con, "02", "E");
                        }
                        //Begin
                        //Created By : Joshua
                        //Created Date : 23 Aug 2018
                        //Desc : Create Journal
                        CreateJournal();
                        //End

                        status = true;
                        MessageBox.Show("Data Invent berhasil diupdate.");
                    }
                    else if (posted == 1 && !chkPosted.Checked) //unposting
                    {
                        //cek qty di invent on hand apakah cukup untuk diambil dari invent
                        status2 = validate2(con);
                        if (status2 == true)
                        {
                            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
                            {
                                string passedid = dgvPrDetails.Rows[i].Cells["From_FullItemId"].Value.ToString();
                                decimal passedqty = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value);
                                decimal passedprice = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Price"].Value);
                                double passedseq = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["No"].Value);
                                string passedid2 = dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString();

                                decimal qtyaltTo = qtyalt(con, i, passedqty, passedid2);
                                decimal qtyaltFrom = qtyalt(con, i, passedqty, passedid);

                                UpdateInventOnHand(con, passedqty, qtyaltTo, passedid2, passedprice, "-",i);
                                
                                UpdateInventMovement(con, passedqty, qtyaltFrom, passedid, passedprice, "+");

                                insertstatuslog(con, "03", "E");
                                status = true;
                            }
                            //Begin
                            //Created By : Joshua
                            //Created Date : 23 Aug 2018
                            //Desc : Delete Journal
                            DeleteJournal();
                            //End

                            MessageBox.Show("Data Invent berhasil di update.");
                        }
                        else
                        {
                            status = false;
                        }
                    }
                    else if ((posted == 1 && chkPosted.Checked) || (posted == 0 && !chkPosted.Checked))
                    {
                        status = true;
                    }
                    if (status == true)
                    {
                        Query = "DELETE FROM NotaResize_Dtl WHERE NRZId = '" + txtTransNo.Text + "'";
                        using (SqlCommand cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        for (int i = dgvPrDetails.Rows.Count - 1; i >= 0; i--)
                        {
                            double seqno = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["No"].Value);
                            Query = "INSERT INTO NotaResize_Dtl([NRZId],[SeqNo],[FromFullItemId],[FromItemName],[ToFullItemId],[ToItemName],[Qty],[Price],[Unit],[LineAmount],[GoodsReceivedId],[GoodsReceiveSeqNo],[Notes1],[Notes2],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) VALUES ('" + txtTransNo.Text + "'," + seqno + ",'" + dgvPrDetails.Rows[i].Cells["From_FullItemId"].Value.ToString() + "','" + dgvPrDetails.Rows[i].Cells["From_ItemName"].Value.ToString() + "',@ToFullItemId, @ToItemName,@Qty,@Price,'" + dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString() + "',@LineAmount,'" + txtReffTransID.Text + "',@GoodReceivedSeqNo,'', @Notes2,@CreatedDate,@CreatedBy, @UpdatedDate, @UpdatedBy) ";
                                using (SqlCommand cmd = new SqlCommand(Query, con))
                                {
                                    cmd.Parameters.AddWithValue("@ToFullItemId", dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString());
                                    cmd.Parameters.AddWithValue("@ToItemName", dgvPrDetails.Rows[i].Cells["To_ItemName"].Value.ToString());
                                    cmd.Parameters.AddWithValue("@Notes2", dgvPrDetails.Rows[i].Cells["Notes2"].Value.ToString().Trim());
                                    cmd.Parameters.AddWithValue("@CreatedDate", dgvPrDetails.Rows[i].Cells["CreatedDate"].Value == null ? DateTime.Now.ToString() : dgvPrDetails.Rows[i].Cells["CreatedDate"].Value.ToString());
                                    if (seqno % 1 == 0)
                                    {
                                        cmd.Parameters.AddWithValue("@GoodReceivedSeqNo", dgvPrDetails.Rows[i].Cells["GoodsReceiveSeqNo"].Value.ToString());
                                        cmd.Parameters.AddWithValue("@LineAmount", dgvPrDetails.Rows[i].Cells["LineAmount"].Value.ToString());
                                    }
                                    else
                                    {
                                        cmd.Parameters.AddWithValue("@GoodReceivedSeqNo", 0);
                                        cmd.Parameters.AddWithValue("@LineAmount", 0.00);
                                    }
                                    cmd.Parameters.AddWithValue("@Qty",Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value));
                                    cmd.Parameters.AddWithValue("@Price",Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value));
                                    cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now.ToString());
                                    cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                                    cmd.ExecuteNonQuery();
                                }
                        }
                        Query = "UPDATE NotaResizeH SET Posted = @posted, [UpdatedDate]= @updateddate, [UpdatedBy]=@updatedby WHERE [NRZId] = '" + txtTransNo.Text + "'";
                        using (SqlCommand cmd2 = new SqlCommand(Query, con))
                        {
                            cmd2.Parameters.AddWithValue("@posted", chkPosted.Checked == true ? "1" : "0");
                            cmd2.Parameters.AddWithValue("@updateddate", DateTime.Now);
                            cmd2.Parameters.AddWithValue("@updatedby", ControlMgr.UserId);
                            cmd2.ExecuteNonQuery();
                        }
                        lblposted.Text = checkPosted(con).ToString() == "1" ? "Current Status : Posted" : "Current Status : Unposted";
                        ModeView();
                    }
                    //else if (status == false)
                    //{
                    //    //chkPosted.Checked = Convert.ToBoolean(posted);
                    //    //ViewData();
                    //}
                    con.Close();
                    scope.Complete();
                }
                Parent.RefreshGrid();
                ViewData();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                if (Mode == "NEW")
                {
                    txtTransNo.Text = "";
                }
            }
            finally 
            {
                
            }
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date : 23 Aug 2018
            //Desc : Create Journal
            SqlConnection con = ConnectionString.GetConnection();
            SqlCommand cmd;
            SqlDataReader Dr;

            decimal resize = 0;
            decimal Price = 0;
            for (int i = 0; i < dgvPrDetails.RowCount; i++)
            {
                //string FromFullItemId = Convert.ToString(dgvPrDetails.Rows[i].Cells["From_ItemName"].Value);
                //string Unit = Convert.ToString(dgvPrDetails.Rows[i].Cells["Unit"].Value);
                int GoodsReceivedSeqNo = Convert.ToInt32(dgvPrDetails.Rows[i].Cells["GoodsReceiveSeqNo"].Value);

                Price = GetPriceGoodsReceived(GoodsReceivedSeqNo);

                //if (Unit.ToUpper() == "KG")
                //{
                //    resize = resize + GetPriceFromInventTable("Alt_AvgPrice", ToFullItemId);
                //}
                //else
                //{
                //    resize = resize + GetPriceFromInventTable("UoM_AvgPrice", ToFullItemId);
                //}
            }

            if (Price != 0)
            {
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = "";
                string JournalHID = "IN21";
                GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, con, cmd = new SqlCommand());
                string Notes = txtReffTransID.Text;

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtTransNo.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                cmd = new SqlCommand(Query, con);
                cmd.ExecuteNonQuery();

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                cmd = new SqlCommand(Query, con);
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                    Type = Convert.ToString(Dr["Type"]);
                    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                    AmountValue = 0;

                    if (JournalIDSeqNo == 1)
                    {
                        AmountValue = resize;
                    }
                    else if (JournalIDSeqNo == 2)
                    {
                        AmountValue = resize;
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
                    cmd = new SqlCommand(Query, con);
                    cmd.ExecuteNonQuery();
                    SeqNo++;
                }
                Dr.Close();
            }            
            //End
        }

        private decimal GetPriceGoodsReceived(int GoodsReceivedSeqNo)
        {
            SqlConnection con = ConnectionString.GetConnection();
            SqlCommand cmd;

            Query = "SELECT Price FROM GoodsReceivedD WHERE GoodsReceivedId = '" + txtReffTransID.Text + "' AND GoodsReceivedSeqNo = " + GoodsReceivedSeqNo + " ";
            cmd = new SqlCommand(Query, con);
            string Result = Convert.ToString(cmd.ExecuteScalar());
            decimal Price = 0;
            if (Result != "")
            {
                Price = Convert.ToDecimal(Result);
            }

            return Price;
        }

        private void DeleteJournal()
        {
            SqlConnection con = ConnectionString.GetConnection();
            SqlCommand cmd;

            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + txtTransNo.Text + "' ";
            cmd = new SqlCommand(Query, con);
            string GLJournalHID = Convert.ToString(cmd.ExecuteScalar());

            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "' ";
            cmd = new SqlCommand(Query, con);
            cmd.ExecuteNonQuery();

            Query = "DELETE FROM GLJournalH WHERE GLJournalHID = '" + GLJournalHID + "' ";
            cmd = new SqlCommand(Query, con);
            cmd.ExecuteNonQuery();
        }

        private decimal GetPriceFromInventTable(string FieldName, string FullItemID)
        {
            SqlConnection con = ConnectionString.GetConnection();
            SqlCommand cmd;

            Query = "SELECT " + FieldName + " FROM InventTable WHERE FullItemID = '" + FullItemID + "'";

            cmd = new SqlCommand(Query, con);
            string result = Convert.ToString(cmd.ExecuteScalar());
            decimal Price;
            if (result == "")
            {
                Price = 1;
            }
            else if (Convert.ToDecimal(result) == 0)
            {
                Price = 1;
            }
            else
            {
                Price = Convert.ToDecimal(result);
            }
            return Price;
        }

        private void insertstatuslog(SqlConnection con, string post, string action)
        {
            DateTime date = new DateTime();
            String vendid = "";
            Query = "SELECT [GoodsReceivedDate],[VendID] FROM [dbo].[NotaResizeH] WHERE [NRZId] = '" + txtTransNo.Text + "'";
            SqlCommand cmd = new SqlCommand(Query, con);
            SqlDataReader Dr = cmd.ExecuteReader();
            while (Dr.Read())
            {
                date = (DateTime)Dr[0];
                vendid = Dr[1].ToString();
            }

            Query = "INSERT INTO [dbo].[NotaResize_LogTable]([NRZId],[NRZDate],[Deskripsi],[GoodsReceivedDate],[GoodsReceivedId],[VendID],[InventSiteID],[LogStatusCode],[LogStatusDesc],[Action],[UserID],[LogDate]) VALUES (@NRZId, @NRZDate,@Deskripsi, @GoodsReceivedDate, @GoodsReceivedId, @VendID, @InventSiteID, @LogStatusCode,@LogStatusDesc,@Action,@UserID,@LogDate) ";
            using (SqlCommand cmd11 = new SqlCommand(Query, con))
            {
                cmd11.Parameters.AddWithValue("@NRZId", txtTransNo.Text);
                cmd11.Parameters.AddWithValue("@NRZDate", dtTransDate.Value.Date);
                cmd11.Parameters.AddWithValue("@GoodsReceivedDate", date);
                cmd11.Parameters.AddWithValue("@GoodsReceivedId", txtReffTransID.Text);
                cmd11.Parameters.AddWithValue("@VendID", vendid);
                cmd11.Parameters.AddWithValue("@InventSiteID", siteid);
                cmd11.Parameters.AddWithValue("@LogStatusCode", post);
                cmd11.Parameters.AddWithValue("@LogStatusDesc", post == "02" ? "Posted" : "Unposted");
                cmd11.Parameters.AddWithValue("@Action", action);
                cmd11.Parameters.AddWithValue("@UserID", ControlMgr.UserId);
                cmd11.Parameters.AddWithValue("@LogDate", DateTime.Now);
                cmd11.Parameters.AddWithValue("@Deskripsi","");
                cmd11.ExecuteNonQuery();
            }
        }

        private decimal qtyalt(SqlConnection con, int i, decimal passedqty, string passedid)
        {
            //geting the ratio from database invent conversion From
            Query = "SELECT [Ratio] FROM [dbo].[InventConversion] WHERE [FullItemID] = @FullItemId AND [FromUnit] = @FromUnit";
            SqlCommand cmd = new SqlCommand(Query, con);
            cmd.Parameters.AddWithValue("@FullItemId", passedid);
            cmd.Parameters.AddWithValue("@FromUnit", dgvPrDetails.Rows[i].Cells["Unit"].Value.ToString());
            decimal ratio = Convert.ToDecimal(cmd.ExecuteScalar());
            decimal qtyalt = passedqty * ratio;
            return qtyalt;
        }

        private void UpdateInventOnHand(SqlConnection con, decimal passedqty, decimal qtyaltTo, string passedid2, decimal passedprice, string opera, int i)
        {
            //update tabel invent on hand
            Query = "UPDATE [dbo].[Invent_OnHand_Qty] SET ";
            Query += " [Available_UoM] = [Available_UoM] " + opera + " @Qty ,";
            Query += " [Available_Alt] = [Available_Alt] " + opera + " @QtyAlt ,";
            Query += " [Available_Amount] = [Available_Amount] " + opera + " @Qty*@Price ,";
            Query += " [Available_For_Sale_UoM] = [Available_For_Sale_UoM] " + opera + " @Qty ,";
            Query += " [Available_For_Sale_Alt] = [Available_For_Sale_Alt] " + opera + " @QtyAlt ,";
            Query += " [Available_For_Sale_Amount] = [Available_For_Sale_Amount] " + opera + " @Qty*@Price ";
            Query += " WHERE [FullItemId] = @itemid ";
            Query += " AND [InventSiteId] = @siteid ";
            using (SqlCommand cmd3 = new SqlCommand(Query, con))
            {
                cmd3.Parameters.AddWithValue("@Qty", passedqty);
                cmd3.Parameters.AddWithValue("@QtyAlt", qtyaltTo);
                cmd3.Parameters.AddWithValue("@itemid", passedid2);
                cmd3.Parameters.AddWithValue("@siteid", siteid);
                cmd3.Parameters.AddWithValue("@Price", passedprice);
                cmd3.ExecuteNonQuery();
            }

            string GroupId = "";
            string SubGroup1Id = "";
            string SubGroup2Id = "";
            string ItemId = "";
            string ItemName = "";
            DateTime RefTransDate = DateTime.Now;
            string VendId = "";
            string VendorName = "";

            Query = " SELECT a.GroupId, a.SubGroup1Id, a.SubGroup2Id, a.ItemId, a.ItemName, b.RefTransDate, b.VendId, b.VendorName FROM [ISBS-NEW4].[dbo].[GoodsReceivedD] AS a INNER JOIN [ISBS-NEW4].[dbo].[GoodsReceivedH] AS b ON a.[GoodsReceivedId] = b.[GoodsReceivedId] WHERE a.GoodsReceivedId = '" + txtReffTransID.Text + "' AND a.GoodsReceivedSeqNo =" + Convert.ToInt32(dgvPrDetails.Rows[i].Cells["No"].Value) + " ";
            using (SqlCommand cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                SqlDataReader Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    GroupId = Dr[0].ToString();
                    SubGroup1Id = Dr[1].ToString();
                    SubGroup2Id = Dr[2].ToString();
                    ItemId = Dr[3].ToString();
                    ItemName = Dr[4].ToString();
                    RefTransDate = Convert.ToDateTime(Dr[5]);
                    VendId = Dr[6].ToString();
                    VendorName = Dr[7].ToString();
                }
            }

            Query = "INSERT INTO InventTrans VALUES (@GroupId, @SubGroupId, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @InventSiteId, @TransId, @SeqNo, @TransDate, @Ref_TransId,@Ref_TransDate, @Ref_Trans_SeqNo, @AccountId, @AccountName, @Available_UoM, @Available_Alt, @Available_Amount, @Available_For_Sale_UoM, @Available_For_Sale_Alt, @Available_For_Sale_Amount, @Available_For_Sale_Reserved_UoM, @Available_For_Sale_Reserved_Alt, @Available_For_Sale_Reserved_Amount, @Notes) ";
            using (SqlCommand cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@GroupId", GroupId);
                cmd.Parameters.AddWithValue("@SubGroupId", SubGroup1Id);
                cmd.Parameters.AddWithValue("@SubGroup2Id", SubGroup2Id);
                cmd.Parameters.AddWithValue("@ItemId", ItemId);
                cmd.Parameters.AddWithValue("@FullItemId", dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString());
                cmd.Parameters.AddWithValue("@ItemName", ItemName);
                cmd.Parameters.AddWithValue("@InventSiteId", siteid);
                cmd.Parameters.AddWithValue("@TransId", txtTransNo.Text);
                cmd.Parameters.AddWithValue("@SeqNo", dgvPrDetails.Rows[i].Cells["No"].Value);
                cmd.Parameters.AddWithValue("@TransDate", dtTransDate.Value);
                cmd.Parameters.AddWithValue("@Ref_TransId", txtReffTransID.Text);
                cmd.Parameters.AddWithValue("@Ref_TransDate", RefTransDate);
                cmd.Parameters.AddWithValue("@Ref_Trans_SeqNo", Convert.ToInt32(dgvPrDetails.Rows[i].Cells["No"].Value));
                cmd.Parameters.AddWithValue("@AccountId", VendId);
                cmd.Parameters.AddWithValue("@AccountName", VendorName);
                cmd.Parameters.AddWithValue("@Available_UoM", opera == "-" ? -passedqty : passedqty);
                cmd.Parameters.AddWithValue("@Available_Alt", opera == "-" ? -qtyaltTo : qtyaltTo);
                cmd.Parameters.AddWithValue("@Available_Amount", opera == "-" ? -passedprice : passedprice);
                cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", opera == "-" ? -passedqty : passedqty);
                cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", opera == "-" ? -qtyaltTo : qtyaltTo);
                cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", opera == "-" ? -passedprice : passedprice);
                cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM", 0);
                cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt", 0);
                cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount", 0);
                cmd.Parameters.AddWithValue("@Notes", dgvPrDetails.Rows[i].Cells["Notes2"].Value.ToString());
                cmd.ExecuteNonQuery();
            }
        }


        private void UpdateInventMovement(SqlConnection con, decimal passedqty, decimal qtyaltFrom, string passedid, decimal passedprice, string opera)
        {
            //update table invent movement
            Query = "UPDATE [dbo].[Invent_Movement_Qty] SET ";
            Query += "[Resize_In_Progress_UoM] = [Resize_In_Progress_UoM] " + opera + " @Qty, ";
            Query += "[Resize_In_Progress_Alt] = [Resize_In_Progress_Alt] " + opera + " @QtyAlt, ";
            Query += "[Resize_In_Progress_Amount] = [Resize_In_Progress_Amount] " + opera + " @Qty*@Price ";
            Query += " WHERE [FullItemId] = @itemid ";
            using (SqlCommand cmd4 = new SqlCommand(Query, con))
            {
                cmd4.Parameters.AddWithValue("@Qty", passedqty);
                cmd4.Parameters.AddWithValue("@QtyAlt", qtyaltFrom);
                cmd4.Parameters.AddWithValue("@itemid", passedid);
                cmd4.Parameters.AddWithValue("@Price", passedprice);
                cmd4.ExecuteNonQuery();
            }
        }

        private bool validate2(SqlConnection con)
        {
            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
            {
                string iditem = dgvPrDetails.Rows[i].Cells["From_FullItemId"].Value.ToString();
                Query = "SELECT [Available_UoM] FROM [dbo].[Invent_OnHand_Qty] WHERE [FullItemId] = '" + iditem + "' AND [InventSiteId] = '" + siteid + "' ";
                SqlCommand cmd6 = new SqlCommand(Query, con);
                decimal qty = Convert.ToDecimal(cmd6.ExecuteScalar());
                if (qty < Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value))
                {
                    MessageBox.Show("Stock Inventory On Hand itemid:" + iditem.ToString() + " tidak memungkinkan untuk edit resize.");
                    return false;
                }
            }
            return true;
        }

        private int checkPosted(SqlConnection con)
        {
            Query = "SELECT Posted FROM [dbo].[NotaResizeH] WHERE [NRZId] = '" + txtTransNo.Text + "'";
            SqlCommand cmd5 = new SqlCommand(Query, con);
            int posted = Convert.ToInt32(cmd5.ExecuteScalar());
            return posted;
        }        

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeView();
            ViewData();
        }

        private void dgvPrDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "Qty")
            {
                posted = checkPosted(ConnectionString.GetConnection());
                if (posted == 0)
                {
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    {
                        e.Handled = true;
                    }
                    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }

            }
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "To_FullItemId")
            {
                 e.Handled = true;
            }
        }

        private void dgvPrDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress += new KeyPressEventHandler(dgvPrDetails_KeyPress);
        }

        private void dgvPrDetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            updateQtyGrid();
        }

        private void updateQtyGrid()
        {
            dgvPrDetails.AutoResizeColumns();
            double totalQty = 0.00;
            double MainQty = 0.00;
            int MainIndex = 0;

            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
            {
                double qtyformat = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["Qty"].Value);
                dgvPrDetails.Rows[i].Cells["Qty"].Value = String.Format("{0:#,##0.#0}", qtyformat);
                double seqno = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["No"].Value);
                if (seqno % 1 != 0)
                {
                    double tempQty = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["Qty"].Value);
                    totalQty += tempQty;
                    if (totalQty > MainQty)
                    {
                        totalQty -= Convert.ToDouble(dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["Qty"].Value);
                        MessageBox.Show("Quantity melebihi Quantity Item Utama!");
                        dgvPrDetails.CurrentRow.Cells["Qty"].Value = String.Format("{0:#,##0.#0}", 0.00);
                    }
                    else if(MainQty == totalQty)
                    {
                        totalQty -= Convert.ToDouble(dgvPrDetails.Rows[dgvPrDetails.CurrentRow.Index].Cells["Qty"].Value);
                        MessageBox.Show("Quantity Item Utama Tidak Boleh 0!");
                        dgvPrDetails.CurrentRow.Cells["Qty"].Value = String.Format("{0:#,##0.#0}", 0.00);
                    }
                    dgvPrDetails.Rows[MainIndex].Cells["Qty"].Value = String.Format("{0:#,##0.#0}", (MainQty - totalQty));
                }
                else
                {
                    MainIndex = i;
                    Query = "SELECT SUM(Qty) FROM [dbo].[NotaResize_Dtl] WHERE [NRZId] = '" + txtTransNo.Text + "' AND SeqNo >= " + dgvPrDetails.Rows[i].Cells["No"].Value + " AND SeqNo< (" + dgvPrDetails.Rows[i].Cells["No"].Value + "+1.00);";
                    using (SqlCommand cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        MainQty = Convert.ToDouble(cmd.ExecuteScalar());
                    }
                    totalQty = 0.00;
                    dgvPrDetails.Rows[MainIndex].Cells["Qty"].Value = String.Format("{0:#,##0.#0}", MainQty);
                }
            }
            dgvPrDetails.AutoResizeColumns();
        }

        private void dgvPrDetails_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            posted = checkPosted(ConnectionString.GetConnection());
            ConnectionString.GetConnection().Close();
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "Qty" && e.RowIndex >= 0)
            {
                if (posted != 0)
                {
                    MessageBox.Show("Tidak bisa diedit karena belom di unpost.");
                }
            }
        }

        private void dgvPrDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            posted = checkPosted(ConnectionString.GetConnection());
            ConnectionString.GetConnection().Close();
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "Action" && e.RowIndex >= 0)
            {
                if (posted == 0)
                {
                    string ButtonStatus = dgvPrDetails.Rows[e.RowIndex].Cells["Action"].Value.ToString();
                    if (ButtonStatus.ToUpper() == "ADD NEW")
                    {
                        InsertNewSubItem();
                    }
                    else
                    {
                        DeleteCurrentSubItem();
                    }
                }
                else
                {
                    MessageBox.Show("Tidak bisa diedit karena belom di unpost.");
                }
            }
        }

        private void refreshButtonGrid()
        {
            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
            {
                if ((string)dgvPrDetails.Rows[i].Cells[0].Value == "button")
                {
                    dgvPrDetails.Rows[i].Cells[0] = new DataGridViewButtonCell();
                    double seqno = Convert.ToDouble(dgvPrDetails.Rows[i].Cells["No"].Value);
                    if (seqno % 1 != 0)
                    {
                        dgvPrDetails.Rows[i].Cells[0].Value = "Delete";
                    }
                    else
                    {
                        dgvPrDetails.Rows[i].Cells[0].Value = "Add New";
                        dgvPrDetails.Rows[i].Cells["Qty"].Style.BackColor = Color.LightGray;
                        dgvPrDetails.Rows[i].Cells["Qty"].ReadOnly = true;
                    }
                }
            }
        }

        private void InsertNewSubItem()
        {
            int a=0;
            int b = 0;
            int index = dgvPrDetails.CurrentRow.Index;
            double seqno = Convert.ToDouble(dgvPrDetails.Rows[index].Cells["No"].Value);
            int count = Convert.ToInt32(dgvPrDetails.Rows[index].Cells["No"].Value);
            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
            {
                if(Convert.ToInt32(dgvPrDetails.Rows[i].Cells["No"].Value)==count)
                {
                    a++;
                }
                else if (a == 0)
                {
                    b++;
                }
            }
            dgvPrDetails.Rows.Insert(a + b, "button", Convert.ToDouble(seqno + (a * 0.01)), dgvPrDetails.Rows[index].Cells["From_FullItemId"].Value.ToString(), dgvPrDetails.Rows[index].Cells["From_ItemName"].Value.ToString(), "0.00", dgvPrDetails.Rows[index].Cells["Unit"].Value.ToString(), "", "", "", "", dgvPrDetails.Rows[index].Cells["Price"].Value, Convert.ToDouble(seqno + (a * 0.01)), "", "", 0.00, dgvPrDetails.Rows[index].Cells["GoodsReceiveSeqNo"].Value);
            refreshButtonGrid();
        }

        private void DeleteCurrentSubItem()
        {
            double currentNo = Convert.ToDouble(dgvPrDetails.CurrentRow.Cells["No"].Value);
            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
            {
                if(Convert.ToInt32(dgvPrDetails.Rows[i].Cells["No"].Value) == Convert.ToInt32(currentNo))
                {
                    if (Convert.ToDouble(dgvPrDetails.Rows[i].Cells["No"].Value) > currentNo)
                    {
                        dgvPrDetails.Rows[i].Cells["No"].Value = currentNo;
                        currentNo += 0.01;
                    }
                }
            }
            dgvPrDetails.Rows.RemoveAt(dgvPrDetails.CurrentRow.Index);
            updateQtyGrid();
        }

        private void dgvPrDetails_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            posted = checkPosted(ConnectionString.GetConnection());
            ConnectionString.GetConnection().Close();
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "To_FullItemId" && e.RowIndex >= 0)
            {
                if (posted == 0)
                {
                    string passedid = dgvPrDetails.Rows[e.RowIndex].Cells["From_FullItemId"].EditedFormattedValue.ToString();
                    Purchase.GoodsReceipt.Resize.InventResizeSearch mform = new InventResizeSearch(this, passedid);
                    mform.ShowDialog();
                    dgvPrDetails.AutoResizeColumns();
                }
                else
                {
                    MessageBox.Show("Tidak bisa diedit karena belom di unpost.");
                }
            }
        }
        //kanan
        public static string itemID;

        public string ItemID { get { return itemID; } set { itemID = value; } }

        PopUp.FullItemId.FullItemId FID = null;
        Purchase.GoodsReceipt.GRHeaderV2 GRNumb = null;

        private void dgvPrDetails_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                //from
                if (FID == null || FID.Text == "")
                {
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "From_FullItemId" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "From_ItemName")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["From_FullItemId"].Value.ToString());
                        itemID = dgvPrDetails.Rows[e.RowIndex].Cells["From_FullItemId"].Value.ToString();
                        FID.Show();
                    }
                    else if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "To_FullItemId" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "To_ItemName")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["To_FullItemId"].Value.ToString());
                        itemID = dgvPrDetails.Rows[e.RowIndex].Cells["To_FullItemId"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "From_FullItemId" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "From_ItemName")
                    {
                        FID.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["From_FullItemId"].Value.ToString());
                    }
                    else if (dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "To_FullItemId" || dgvPrDetails.Columns[e.ColumnIndex].Name.ToString() == "To_ItemName")
                    {
                        FID.GetData(dgvPrDetails.Rows[e.RowIndex].Cells["To_FullItemId"].Value.ToString());
                    }
                    FID.Show();
                    FID.Focus();
                }
                //to
              
            }
        }

        private void txtReffTransID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (GRNumb == null || GRNumb.Text == "")
                {
                    txtReffTransID.Enabled = true;
                    GRNumb = new Purchase.GoodsReceipt.GRHeaderV2("Receipt Order");
                    GRNumb.SetMode("PopUp",txtReffTransID.Text);
                    GRNumb.Show();
                }
                else if (CheckOpened(GRNumb.Name))
                {
                    GRNumb.WindowState = FormWindowState.Normal;
                    GRNumb.Show();
                    GRNumb.Focus();
                }
            }
        }

        private bool CheckOpened(string name)
        {
            // FormCollection FC = Application.OpenForms;
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
        
       
        //tia END
    }
}
