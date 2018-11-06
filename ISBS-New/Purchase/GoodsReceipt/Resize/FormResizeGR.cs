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

        //MODE DIGUNAKAN UNTUK NEW, EDIT, DELETE
        string Mode = "";

        //DIGUNAKAN UNTUK SET CHILD
        private List<Form> MDI = new List<Form>();

        //VARIABEL BACKGROUND UNTUK SAVE DATA
        private string ResizeType;
        //InquiryV1 Parent = new InquiryV1();

        private InquiryV1 Parent = new InquiryV1();

        string siteid = "";
        int posted;

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
        }

        private void ModeEdit()
        {
            Mode = "EDIT";
            this.Text = "Form GR Resize (" + Mode + ")";
            this.Refresh();
            NonActivationControl(true);
            btnEdit.Enabled = false;
            btnExit.Enabled = false;
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
            if (Variable.Kode != null)
            {
                txtTransNo.Text = Variable.Kode[0];
                siteid = Variable.Kode[2];
                ViewData();
                ModeView();
            }
            else
            {
                ModeNew();
            }
            btnNew.Visible = false;
            btnDelete.Visible = false;

            dgvPrDetails.AutoResizeColumns();
            SqlConnection con = ConnectionString.GetConnection();
            lblposted.Text = validate3(con).ToString() == "1" ? "Current Status : Posted" : "Current Status : Unposted";
            con.Close();
        }

        private void ViewData()
        {
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
                C.DgvCreate(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Qty", "Unit", "To_FullItemId", "To_ItemName", "Notes", "Notes2", "Price", "SeqNo"});
                    
                //CREATE DETAIL
                //Query = "Select [SeqNo] [No],[FullItemID], ItemName [ItemDesc],[Qty],[Unit],[Base],[VendID] Vendor,[DeliveryMethod],[ReffTransID] SalesSO,[ExpectedDateFrom],[ExpectedDateTo],[Deskripsi],GroupId,SubGroup1Id,SubGroup2Id,ItemId,GelombangId,BracketId,Price,SeqNoGroup,BracketDesc From [PurchRequisition_Dtl] Where PurchReqID = '" + txtTransNo.Text.Trim() + "' order by SeqNo asc";
                Query = "Select SeqNo No, FromFullItemId From_FullItemId, FromItemName From_ItemName, Qty, Unit, ToFullItemId To_FullItemId, ToItemName To_ItemName, Notes1 Notes, Notes2 Notes2, Price, SeqNo from NotaResize_Dtl where NRZId ='" + txtTransNo.Text.Trim() + "'";
                C.Dr = C.ReturnDr(Query);
                int i = 0;
                while (C.Dr.Read())
                {
                    //string ExpectedDateFrom = Convert.ToDateTime(C.Dr[9]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(C.Dr[9]).ToString("dd-MM-yyyy");
                    //string ExpectedDateTo = Convert.ToDateTime(C.Dr[10]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(C.Dr[10]).ToString("dd-MM-yyyy");
                    dgvPrDetails.Rows.Add(C.Dr[0], C.Dr[1], C.Dr[2], C.Dr[3], C.Dr[4], C.Dr[5], C.Dr[6], C.Dr[7], C.Dr[8], C.Dr[9], C.Dr[10]);//, "", C.Dr[7], C.Dr[8], C.Dr[9], C.Dr[10], C.Dr[11], C.Dr[12], C.Dr[13], C.Dr[14]);
                }

                dgvPrDetails.ReadOnly = false;
                C.DgvReadOnly(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Unit", "Qty", "To_ItemName", "Notes" });
                C.DgvColor(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Unit", "To_ItemName", "Qty" , "Notes"}, Color.LightGray);
                //C.DgvColor(dgvPrDetails, new string[] { "Qty" }, Color.LightYellow);
                C.DgvNotSort(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Qty", "Unit", "To_FullItemId", "To_ItemName", "To_ItemName", "Notes", "Notes2" });
                //C.DgvVisible(dgvPrDetails, new string[] { "From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId" });
                C.DgvVisible(dgvPrDetails, new string[] { "Price" , "SeqNo"});
                C.DgvAlignRight(dgvPrDetails, new string[] { "To_FullItemId" });

                dgvPrDetails.Refresh();
                dgvPrDetails.AutoResizeColumns();
            }
        }

        /*private void btnNew_Click(object sender, EventArgs e)
        {
            SearchQueryV2 tmpSearch = new SearchQueryV2();

            tmpSearch.Text = "Search Item Resize";
            tmpSearch.PrimaryKey = "From_FullItemId";
            tmpSearch.QuerySearch = "Select a.From_FullItemId, a.From_ItemName, b.GroupId From_GroupId, b.SubGroup1Id From_SubGroup1Id, b.SubGroup2Id From_SubGroup2Id, b.ItemId From_ItemId, b.Uom Unit, a.To_FullItemId, a.To_ItemName,c.GroupId To_GroupId, c.SubGroup1Id To_SubGroup1Id, c.SubGroup2Id To_SubGroup2Id, c.ItemId To_ItemId From dbo.[InventResize] a left join InventTable b on a.From_FullItemId=b.FullItemId left join InventTable c on a.To_FullItemId=c.FullItemId";
            tmpSearch.FilterText = new string[] { "From_FullItemId", "From_ItemName", "Unit", "To_FullItemId", "To_ItemName" };
            tmpSearch.Select = new string[] { "From_FullItemId", "From_ItemName", "Unit", "To_FullItemId", "To_ItemName", "From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId" };
            tmpSearch.HideField = new string[] {"From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId"};
            tmpSearch.Parent = this;
            tmpSearch.Notes = "";

            tmpSearch.ShowDialog();

            if (Variable.Kode2 != null)
            {
                using (Method C = new Method())
                {

                    C.DgvCreate(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Qty", "Unit", "To_FullItemId", "To_ItemName", "Notes", "From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId" });
                    for (int i = 0; i < Variable.Kode2.GetLength(0); i++)
                    {
                        dgvPrDetails.Rows.Add(dgvPrDetails.RowCount + 1, Variable.Kode2[i, 0], Variable.Kode2[i, 1], "0.00", Variable.Kode2[i, 2], Variable.Kode2[i, 3], Variable.Kode2[i, 4], "", Variable.Kode2[i, 5], Variable.Kode2[i, 6], Variable.Kode2[i, 7], Variable.Kode2[i, 8], Variable.Kode2[i, 9], Variable.Kode2[i, 10], Variable.Kode2[i, 11], Variable.Kode2[i, 12]);
                    }
                    dgvPrDetails.ReadOnly = false;
                    C.DgvReadOnly(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Unit", "To_FullItemId", "To_ItemName", "From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId" });
                    C.DgvColor(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Unit", "To_FullItemId", "To_ItemName" }, Color.LightGray);
                    C.DgvColor(dgvPrDetails, new string[] { "Qty" }, Color.LightYellow);
                    C.DgvNotSort(dgvPrDetails, new string[] { "No", "From_FullItemId", "From_ItemName", "Qty", "Unit", "To_FullItemId", "To_ItemName", "To_ItemName", "Notes" });
                    C.DgvVisible(dgvPrDetails, new string[] { "From_GroupId", "From_SubGroup1Id", "From_SubGroup2Id", "From_ItemId", "To_GroupId", "To_SubGroup1Id", "To_SubGroup2Id", "To_ItemId" });
                    C.DgvAlignRight(dgvPrDetails, new string[] { "Qty" });
                    dgvPrDetails.AutoResizeColumns();
                }
            }
        }*/

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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPrDetails.RowCount > 0)
            {
                int Index = dgvPrDetails.CurrentRow.Index;
                using (Method C = new Method ())
                {
                    C.DeleteDgv1(dgvPrDetails, Index, new string[] { "No", "From_FullItemId", "From_ItemName" });
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
            dtTransDate.Enabled = false;
        }


        //edited, Thaddaeus Matthias, 28 March 2018 BEGIN===================
        private void btnSave_Click(object sender, EventArgs e)
        {
            bool status3 = false;
            status3 = validate1();

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    SqlConnection con = ConnectionString.GetConnection();
                    bool status = false;
                    bool status2 = true; //cek qty invent saat mw unpost
                    int posted = validate3(con); //cek status posted
                    decimal ratio = 0;

                    if (posted == 0 && chkPosted.Checked)
                    {
                        if (status3 == false)
                        {
                            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
                            {
                                string passedid = dgvPrDetails.Rows[i].Cells["From_FullItemId"].Value.ToString();
                                string passedid2 = dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString();
                                decimal passedqty = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value);
                                decimal passedprice = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Price"].Value);
                                int passedseq = Convert.ToInt32(dgvPrDetails.Rows[i].Cells["SeqNo"].Value);
                                
                                decimal qtyaltTo = qtyalt(con, i, passedqty, passedid2);
                                decimal qtyaltFrom = qtyalt(con, i, passedqty, passedid);

                                //update tabel invent on hand
                                UpdateInventOnHand(con, passedqty, qtyaltTo, passedid2, passedprice, "+");

                                //update table invent movement
                                UpdateInventMovement(con, passedqty, qtyaltFrom, passedid, passedprice, "-");

                                insertstatuslog(con, "02", passedid, passedqty, passedprice, passedseq, passedid2, qtyaltFrom);
                            }
                            status = true;
                            MessageBox.Show("Data Invent berhasil diupdate.");
                        }
                        else {MessageBox.Show("Tujuan Resize belom terpilih"); }
                    }
                    else if (posted == 1 && !chkPosted.Checked)
                    {
                        //cek qty di invent on hand
                        status2 = validate2(con);
                        if (status2 == true)
                        {
                            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
                            {
                                string iditem = dgvPrDetails.Rows[i].Cells["From_FullItemId"].Value.ToString();

                                string passedid = dgvPrDetails.Rows[i].Cells["From_FullItemId"].Value.ToString();
                                decimal passedqty = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Qty"].Value);
                                decimal passedprice = Convert.ToDecimal(dgvPrDetails.Rows[i].Cells["Price"].Value);
                                int passedseq = Convert.ToInt32(dgvPrDetails.Rows[i].Cells["SeqNo"].Value);
                                string passedid2 = dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString();

                                decimal qtyaltTo = qtyalt(con, i, passedqty, passedid2);
                                decimal qtyaltFrom = qtyalt(con, i, passedqty, passedid);

                                UpdateInventOnHand(con, passedqty, qtyaltTo, passedid2, passedprice, "-");
                                
                                UpdateInventMovement(con, passedqty, qtyaltFrom, passedid, passedprice, "+");

                                insertstatuslog(con, "03", passedid, passedqty, passedprice, passedseq, passedid2, qtyaltTo);
                                status = true;
                            }
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
                        for (int i = dgvPrDetails.Rows.Count - 1; i >= 0; i--)
                        {
                            Query = "UPDATE NotaResize_Dtl SET ToFullItemId = @ToFullItemId, ToItemName = @ToItemName, Notes2 = @Notes2, UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy ";
                            Query += "  WHERE [NRZId] = '" + txtTransNo.Text + "' AND [SeqNo] = '" + (i + 1) + "'";
                            using (SqlCommand cmd = new SqlCommand(Query, con))
                            {
                                cmd.Parameters.AddWithValue("@ToFullItemId", dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString());
                                cmd.Parameters.AddWithValue("@ToItemName", dgvPrDetails.Rows[i].Cells["To_ItemName"].Value.ToString());
                                cmd.Parameters.AddWithValue("@Notes2", dgvPrDetails.Rows[i].Cells["Notes2"].Value.ToString());
                                cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now.ToString());
                                cmd.Parameters.AddWithValue("@UpdatedBy", Login.Username);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        Query = "UPDATE NotaResizeH SET Posted = @posted, [UpdatedDate]= @updateddate, [UpdatedBy]=@updatedby WHERE [NRZId] = '" + txtTransNo.Text + "'";
                        using (SqlCommand cmd2 = new SqlCommand(Query, con))
                        {
                            cmd2.Parameters.AddWithValue("@posted", chkPosted.Checked == true ? "1" : "0");
                            cmd2.Parameters.AddWithValue("@updateddate", DateTime.Now);
                            cmd2.Parameters.AddWithValue("@updatedby", Login.Username);
                            cmd2.ExecuteNonQuery();
                        }
                        lblposted.Text = validate3(con).ToString() == "1" ? "Current Status : Posted" : "Current Status : Unposted";
                    }
                    else if (status == false)
                    {
                        chkPosted.Checked = Convert.ToBoolean(posted);
                        ViewData();
                    }
                    con.Close();
                    scope.Complete();
                }
                //Parent.RefreshDataGrid();
            }
            //catch (Exception Ex)
            //{
            //    MessageBox.Show(Ex.Message);
            //    if (Mode == "NEW")
            //    {
            //        txtTransNo.Text = "";
            //    }            
            //}
            finally 
            {
                ModeView();
            }
        }

        private void insertstatuslog(SqlConnection con, string post, string id, decimal qty, decimal price, int seq, string id2, decimal qtyalt)
        {
            DateTime date = new DateTime();
            String vendid = "";
            decimal amount = qty * price;
            Query = "SELECT [GoodsReceivedDate],[VendID] FROM [dbo].[NotaResizeH] WHERE [NRZId] = '" + txtTransNo.Text + "'";
            SqlCommand cmd = new SqlCommand(Query, con);
            SqlDataReader Dr = cmd.ExecuteReader();
            while (Dr.Read())
            {
                date = (DateTime)Dr[0];
                vendid = Dr[1].ToString();
            }

            Query = "INSERT INTO [dbo].[NotaResize_LogTable] VALUES (@NRZId, @NRZDate, @GoodsReceivedDate, @GoodsReceivedId, @VendID, @InventSiteID, @FullItemId, @ToFullItemId, @SeqNo, @Qty_UoM,@Qty_Alt, @Amount,@LogStatusCode,@LogStatusDesc,@LogDescription,@UserID,@LogDate) ";
            using (SqlCommand cmd11 = new SqlCommand(Query, con))
            {
                cmd11.Parameters.AddWithValue("@NRZId", txtTransNo.Text);
                cmd11.Parameters.AddWithValue("@NRZDate", dtTransDate.Value.Date);
                cmd11.Parameters.AddWithValue("@GoodsReceivedDate", date);
                cmd11.Parameters.AddWithValue("@GoodsReceivedId", txtReffTransID.Text);
                cmd11.Parameters.AddWithValue("@VendID", vendid);
                cmd11.Parameters.AddWithValue("@InventSiteID", siteid);
                cmd11.Parameters.AddWithValue("@FullItemId", post == "02" ? id : id2);
                cmd11.Parameters.AddWithValue("@ToFullItemId", post == "02" ? id2 : id);
                cmd11.Parameters.AddWithValue("@Qty_UoM", qty);
                cmd11.Parameters.AddWithValue("@Qty_Alt", qtyalt);
                cmd11.Parameters.AddWithValue("@SeqNo", seq);
                cmd11.Parameters.AddWithValue("@Amount", amount);
                cmd11.Parameters.AddWithValue("@LogStatusCode", post);
                cmd11.Parameters.AddWithValue("@LogStatusDesc", post == "02" ? "Posted" : "Unposted");
                cmd11.Parameters.AddWithValue("@LogDescription", post == "02" ? "Posted" : "Unposted");
                cmd11.Parameters.AddWithValue("@UserID", Login.Username);
                cmd11.Parameters.AddWithValue("@LogDate", DateTime.Now);
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

        private void UpdateInventOnHand(SqlConnection con, decimal passedqty, decimal qtyaltTo, string passedid2, decimal passedprice, string opera)
        {
            //update tabel invent on hand
            Query = "UPDATE [dbo].[Invent_OnHand_Qty] SET ";
            Query += " [Available_UoM] = [Available_UoM] "+opera+" @Qty ,";
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

        private bool validate1()
        {
            for (int i = 0; i < dgvPrDetails.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dgvPrDetails.Rows[i].Cells["To_FullItemId"].Value.ToString()) == true)
                {
                    return true;
                }
            }
            return false;
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

        private int validate3(SqlConnection con)
        {
            Query = "SELECT Posted FROM [dbo].[NotaResizeH] WHERE [NRZId] = '" + txtTransNo.Text + "'";
            SqlCommand cmd5 = new SqlCommand(Query, con);
            int posted = Convert.ToInt32(cmd5.ExecuteScalar());
            return posted;
        }
        //END=================================================================

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
            //if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "Qty")
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            //    {
            //        e.Handled = true;
            //    }
            //    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            //    {
            //        e.Handled = true;
            //    }
            //}
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
            dgvPrDetails.AutoResizeColumns();
        }

        public void SetParent(InquiryV1 F)
        {
            Parent = F;
        }

        private void dgvPrDetails_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            posted = validate3(ConnectionString.GetConnection());
            ConnectionString.GetConnection().Close();
            if (dgvPrDetails.Columns[dgvPrDetails.CurrentCell.ColumnIndex].Name == "To_FullItemId")
            {
                if (posted == 0)
                {
                    //string passedid = dgvPrDetails.CurrentRow.Cells["From_FullItemId"].EditedFormattedValue.ToString();
                    //Purchase.GoodsReceipt.Resize.InventResizeSearch mform = new InventResizeSearch(this, passedid);
                    //mform.ShowDialog();
                    dgvPrDetails.AutoResizeColumns();
                }
                else
                {
                    MessageBox.Show("Tidak bisa diedit karena belom di unpost.");
                }
            }
        }


    }
}
