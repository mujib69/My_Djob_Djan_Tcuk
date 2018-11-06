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

namespace ISBS_New.Sales.NotaCredit
{
    public partial class FrmT_NotaCredit : MetroFramework.Forms.MetroForm
    {
        string Query;
        string Mode;
        string[] HideField;
        SqlConnection con;
        SqlCommand cmd;
        SqlTransaction trans;
        SqlDataAdapter adapter;
        SqlDataReader Dr;
        private InquiryV1 Parent = new InquiryV1();
        //tia edit
        ContextMenu vendid = new ContextMenu();
        //tia edit end

        bool Journal = false;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FrmT_NotaCredit()
        {
            InitializeComponent();
            DueDate.Value = DateTime.Now;
            DateTax.Value = DateTime.Now;
            CNDate.Value = DateTime.Now;
        }

        private void FrmT_NotaCredit_Load(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 0;
            if (validate() == true)
            {
                txtCNNo.Text = Variable.Kode[0];
                CNDate.Text = Variable.Kode[1];
                populate();
                refreshGrid();
            }
            mode(Mode);
        }

        public void SetParent(InquiryV1 F)
        {
            Parent = F;
        }
        private void mode(string mode)
        {
            Mode = mode;
            if (mode.ToUpper() == "EDIT")
            {
                //BUTTON
                btnSearchAcc.Enabled = false;
                btnSearchKurs.Enabled = true;
                btnNew.Enabled = true;
                btnDelete.Enabled = true;
                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                btnExit.Enabled = false;
                btnApproval.Enabled = false;
                btnUnapprove.Enabled = false;
                //TEXT
                txttblDOPabrikH_Ket1.Enabled = true;
                DateTax.Enabled = true;
                txtTaxNumber.Enabled = true;
                txtTaxName.Enabled = true;
                txtTaxAddress.Enabled = true;
                txtPPH.Enabled = true;
                txtPPN.Enabled = true;
                DueDate.Enabled = true;
                //make grid not sortable
                foreach (DataGridViewColumn column in dgvDetail.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            else if (mode.ToUpper() == "VIEW")
            {
                //BUTTON
                btnSearchAcc.Enabled = false;
                btnSearchKurs.Enabled = false;
                btnNew.Enabled = false;
                btnDelete.Enabled = false;
                btnEdit.Enabled = true;
                btnSave.Enabled = false;
                btnCancel.Enabled = false;
                btnExit.Enabled = true;

                Query = "SELECT TransStatus FROM NotaCreditH WHERE CN_No = '"+txtCNNo.Text+"'";            
                using(cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
                {
                    if (cmd.ExecuteScalar().ToString() == "01")
                    {
                        btnApproval.Enabled = true;
                        btnUnapprove.Enabled = false;
                    }
                    else if (cmd.ExecuteScalar().ToString() == "03")
                    {
                        btnUnapprove.Enabled = true;
                        btnApproval.Enabled = false;
                    }
                    else
                    {
                        btnApproval.Enabled = false;
                        btnUnapprove.Enabled = false;
                    }
                }
                    
                //text
                txttblDOPabrikH_Ket1.Enabled = false;
                DateTax.Enabled = false;
                txtTaxNumber.Enabled = false;
                txtTaxName.Enabled = false;
                txtTaxAddress.Enabled = false;
                txtPPH.Enabled = false;
                txtPPN.Enabled = false;
                DueDate.Enabled = false;
                txtKurs.Enabled = false;
                //tia edit
                txtAccName.Enabled = true;
                txtAccId.Enabled = true;
                txtAccId.ReadOnly = true;
                txtAccName.ReadOnly = true;
                txtAccId.ContextMenu = vendid;
                txtAccName.ContextMenu = vendid;
                //tia edit end
            }
            else if (mode.ToUpper() == "NEW")
            {
                //BUTTON
                btnSearchAcc.Enabled = true;
                btnSearchKurs.Enabled = true;
                btnNew.Enabled = true;
                btnDelete.Enabled = true;
                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                btnCancel.Enabled = false;
                btnExit.Enabled = true;
                btnApproval.Enabled = false;
                btnUnapprove.Enabled = false;
                //text
                txttblDOPabrikH_Ket1.Enabled = true;
                DateTax.Enabled = true;
                txtTaxNumber.Enabled = true;
                txtTaxName.Enabled = true;
                txtTaxAddress.Enabled = true;
                txtPPH.Enabled = true;
                txtPPN.Enabled = true;
                DueDate.Enabled = true;
            }
            else if (mode.ToUpper()=="POPUP")
            {
                this.StartPosition = FormStartPosition.Manual;
                foreach (var scrn in Screen.AllScreens)
                {
                    if (scrn.Bounds.Contains(this.Location))
                        this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
                }

                //BUTTON
                btnSearchAcc.Visible = false;
                btnSearchKurs.Visible = false;
                btnNew.Visible = false;
                btnDelete.Visible = false;
                btnEdit.Visible = false;
                btnSave.Visible = false;
                btnCancel.Visible = false;
                btnExit.Visible = true;
                //text
                txttblDOPabrikH_Ket1.Enabled = false;
                DateTax.Enabled = false;
                txtTaxNumber.Enabled = false;
                txtTaxName.Enabled = false;
                txtTaxAddress.Enabled = false;
                txtPPH.Enabled = false;
                txtPPN.Enabled = false;
                DueDate.Enabled = false;
                //tia edit
                txtAccName.Enabled = true;
                txtAccId.Enabled = true;
                txtAccId.ReadOnly = true;
                txtAccName.ReadOnly = true;
                txtAccId.ContextMenu = vendid;
                txtAccName.ContextMenu = vendid;
                //tia edit end
            }

        }

        private string GetSOfromNRJId(string NRJID)
        {
            string SOID = "";
            Query = "SELECT c.[SalesOrderId] FROM [dbo].[NotaReturJualH] a LEFT JOIN [dbo].[GoodsIssuedH] b ON a.[GoodsIssuedId]=b.[GoodsIssuedId] LEFT JOIN [dbo].[DeliveryOrderH] c ON b.[RefTransID]=c.[DeliveryOrderId] WHERE [NRJId] = @NRJId";
            using (cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@NRJId", NRJID);
                SqlDataReader Dr2 = cmd.ExecuteReader();
                if (Dr2.HasRows)
                {
                    while (Dr2.Read())
                    {
                        SOID = Dr2["SalesOrderId"].ToString();
                    }
                }
                Dr2.Close();
            }
            return SOID;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mode("View");
            populate();
            refreshGrid();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            mode("Edit");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (dgvDetail.Rows.Count > 0)
                    {
                        if (validate2() == true)
                        {
                            if (Mode.ToUpper() == "NEW")
                            {
                                insertTableH();
                                insertTableDtl();
                                MessageBox.Show("Data berhasil di input");
                            }
                            else if (Mode.ToUpper() == "EDIT")
                            {
                                updateTable();
                                MessageBox.Show("Data berhasil di update");
                            }
                            populate();
                            refreshGrid();
                            mode("View");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tidak ada Item di DataGrid");
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally { }
        }

        private void refreshGrid()
        {
            if (txtCNNo.Text != null)
            {
                using (Method C = new Method())
                {
                    dgvDetail.Columns.Clear();
                    C.DgvCreate(dgvDetail, new string[] { "CN_Date", "CN_No", "SeqNo", "NRJId", "NRJ_SeqNo","SO_Reff", "CurrencyId", "ExchRate", "TaxStatusCode", "TaxPercent", "PPHTaxStatusCode", "PPHTaxPercent", "LineAmount", "TaxBaseAmount", "TaxAmount", "PPHTaxAmount", "Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" });
                    Query = "SELECT * from NotaCredit_Dtl WHERE CN_No = '" + txtCNNo.Text + "'";
                    C.Dr = C.ReturnDr(Query);
                    while (C.Dr.Read())
                    {
                        string CNdate = Convert.ToDateTime(C.Dr[0]).ToString("dd/MM/yyyy");
                        string CreatedDate = Convert.ToDateTime(C.Dr[16]).ToString("dd/MM/yyyy HH:mm:ss");
                        string UpdatedDate = Convert.ToDateTime(C.Dr[18]).ToString("dd/MM/yyyy HH:mm:ss");

                        dgvDetail.Rows.Add(CNdate, C.Dr[1], C.Dr[2], C.Dr[3], C.Dr[4], GetSOfromNRJId(C.Dr[3].ToString()), C.Dr[5], String.Format("{0:#,##0.###0}", C.Dr[6]), C.Dr[7], C.Dr[8], C.Dr[9], C.Dr[10], String.Format("{0:#,##0.###0}", C.Dr[11]), String.Format("{0:#,##0.###0}", C.Dr[12]), String.Format("{0:#,##0.###0}", C.Dr[13]), String.Format("{0:#,##0.###0}", C.Dr[14]), C.Dr[15], CreatedDate, C.Dr[17], UpdatedDate, C.Dr[19]);
                    }
                    HideField = new string[] { "CN_Date", "CN_No", "SeqNo", "NRJ_SeqNo", "CurrencyId", "ExchRate", "TaxStatusCode", "PPHTaxStatusCode" };
                    C.DgvVisible(dgvDetail, HideField);
                    dgvDetail.AutoResizeColumns();
                }
            }
        }
        private void populate()
        {
            Query = "SELECT * FROM NotaCreditH WHERE CN_No = '" + txtCNNo.Text + "' ;";
            using (con = ConnectionString.GetConnection())
            using (cmd = new SqlCommand(Query, con))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtAccId.Text = Dr["AccountNum"].ToString();
                    txtAccName.Text = Dr["AccountName"].ToString();
                    txtPPH.Text = Dr["PPHTaxPercent"].ToString();
                    txtPPHbwh.Text = String.Format("{0:#,##0.###0}", Dr["PPHTaxAmount"]);
                    txtPPN.Text = Dr["TaxPercent"].ToString();
                    txtPPNbwh.Text = String.Format("{0:#,##0.###0}", Dr["TaxAmount"]);
                    txtMataUang.Text = Dr["CurrencyId"].ToString();
                    txtKurs.Text = String.Format("{0:#,##0.###0}", Dr["ExchRate"]);
                    txtTotalNett.Text = String.Format("{0:#,##0.###0}", Dr["TotalAmount"]);

                    String total = String.Format("{0:#,##0.###0}", (Convert.ToInt32(Dr["TotalAmount"]) + Convert.ToInt32(Dr["TaxAmount"]) + Convert.ToInt32(Dr["PPHTaxAmount"])));
                    txtTotal.Text = String.Format("{0:#,##0.###0}", total);
                    txtNPWP.Text = Dr["NPWP"].ToString();
                    txttblDOPabrikH_Ket1.Text = Dr["Notes"].ToString();
                    DateTax.Text = Dr["TaxDate"].ToString();
                    txtTaxNumber.Text = Dr["TaxNum"].ToString();
                    txtTaxName.Text = Dr["TaxName"].ToString();
                    txtTaxAddress.Text = Dr["TaxAddress"].ToString();
                    DueDate.Text = Dr["DueDate"].ToString();
                }
                Dr.Close();
            }
        }
        private void updateTable()
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Query = "UPDATE NotaCreditH SET TaxNum='" + txtTaxNumber.Text + "', TaxAddress='" + txtTaxAddress.Text + "', TaxName = '" + txtTaxName.Text + "', TaxDate = '" + DateTax.Value.Date + "', Notes = '" + txttblDOPabrikH_Ket1.Text + "', UpdatedBy = '" + ControlMgr.UserId + "', UpdatedDate = '" + DateTime.Now + "' WHERE CN_No = '"+txtCNNo.Text+"';";
                    using (con = ConnectionString.GetConnection())
                    using (cmd = new SqlCommand(Query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    Query = "UPDATE NotaCredit_Dtl SET Notes = '" + txttblDOPabrikH_Ket1.Text + "', UpdatedBy = '" + ControlMgr.UserId + "', UpdatedDate = '" + DateTime.Now + "'WHERE CN_No = '" + txtCNNo.Text + "';";
                    using (con = ConnectionString.GetConnection())
                    using (cmd = new SqlCommand(Query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    scope.Complete();
                    MessageBox.Show("Data Berhasil di Update");
                }

                ListMethod.StatusLogCustomer("FrmT_NotaCredit", "NotaCredit", txtAccId.Text, "01", "Edit", txtCNNo.Text, "", "", "");

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        private bool validate()
        {
            //passing data from parent isnt null (which means mode view,edit), if null mode new)
            if (Variable.Kode != null)
            {
                Mode = "View";
                return true;
            }
            else
            {
                Mode = "New";
                return false;
            }
        }

        private bool validate2() //validate no empty textbox when save or any mandatory paramter (when mode new)
        {
            bool status = false;
            if (txtAccId.Text == null || txtAccId.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Customer belom terpilih!");
            }
            else if (dgvDetail.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada NRJ yang dipilih!");
            }
            else if (txtTaxName.Text == null || txtTaxName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Tax Name kosong!");
            }
            else if (txtTaxAddress.Text == null || txtTaxAddress.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Tax Address kosong!");
            }
            else if (txtTaxNumber.Text == null || txtTaxNumber.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Tax Number kosong!");
            }
            else if (Convert.ToDecimal(txtKurs.Text) == 0)
            {
                MessageBox.Show("Nilai kurs tidak boleh 0.");
            }
            else
            {
                if (Mode.ToUpper() == "NEW")
                {
                    for (int i = 0; i < dgvDetail.Rows.Count; i++)
                    {
                        Query = "SELECT a.* FROM NotaCredit_Dtl a INNER JOIN NotaReturJualH b ON a.NRJId=b.NRJId WHERE a.NRJId = '" + dgvDetail.Rows[i].Cells["NRJId"].Value.ToString() + "'";
                        using (cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Dr = cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                MessageBox.Show("NRJId sudah terinput dalam Nota Debet lain.");
                                Dr.Close();
                                return false;
                            }
                            else
                            {
                                status = true;
                            }
                            Dr.Close();
                        }
                    }
                }
            }
            return status;
        }

        private void insertTableH()
        {
            string CNID = "";
            decimal taxbaseamount = (Convert.ToDecimal(txtPPNbwh.Text) + Convert.ToDecimal(txtPPHbwh.Text));

            Query = "INSERT INTO NotaCreditH VALUES (@CN_Date,@CN_No,@CNMode,@NRJId,@DueDate,@AccountNum,@AccountName,@CurrencyId ";
            Query += ","+Convert.ToDecimal(txtKurs.Text.ToString())+",@TaxStatusCode," + Convert.ToDecimal(txtPPN.Text.ToString()) + ",@PPHTaxStatusCode," + Convert.ToDecimal(txtPPH.Text.ToString()) + "," + Convert.ToDecimal(txtTotalNett.Text.ToString()) + "," + taxbaseamount + "," + Convert.ToDecimal(txtPPNbwh.Text.ToString()) + " ";
            Query += "," + Convert.ToDecimal(txtPPHbwh.Text.ToString()) + ",@NPWP,@TaxNum,@TaxAddress,@TaxName,@TaxDate,@TransStatus,@Notes,@CreatedDate,@CreatedBy,'','',0)";
            using (con = ConnectionString.GetConnection())
            using (cmd = new SqlCommand(Query, con))
            {
                SqlCommand cmd2 = new SqlCommand();
                string Jenis = "CN";
                string Kode = "CN";
                CNID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", con, cmd2);
                
                cmd.Parameters.AddWithValue("@CN_Date", CNDate.Value);
                cmd.Parameters.AddWithValue("@CN_No", CNID);
                cmd.Parameters.AddWithValue("@CNMode", "");
                cmd.Parameters.AddWithValue("@NRJId", "");
                cmd.Parameters.AddWithValue("@DueDate", DueDate.Value);
                cmd.Parameters.AddWithValue("@AccountNum", txtAccId.Text);
                cmd.Parameters.AddWithValue("@AccountName", txtAccName.Text);
                cmd.Parameters.AddWithValue("@CurrencyId", txtMataUang.Text);
                //cmd.Parameters.AddWithValue("@ExchRate", txtKurs.Text);
                cmd.Parameters.AddWithValue("@TaxStatusCode", "");
                //cmd.Parameters.AddWithValue("@TaxPercent", ); //Note decimal require more param configuration
                cmd.Parameters.AddWithValue("@PPHTaxStatusCode", "");
                //cmd.Parameters.AddWithValue("@PPHTaxPercent", );
                //cmd.Parameters.AddWithValue("@TotalAmount",);
                //cmd.Parameters.AddWithValue("@TaxBaseAmount", taxbaseamount);
                //cmd.Parameters.AddWithValue("@TaxAmount", );
                //cmd.Parameters.AddWithValue("@PPHTaxAmount", );
                cmd.Parameters.AddWithValue("@NPWP", txtNPWP.Text);
                cmd.Parameters.AddWithValue("@TaxNum", txtTaxNumber.Text);
                cmd.Parameters.AddWithValue("@TaxAddress", txtTaxAddress.Text);
                cmd.Parameters.AddWithValue("@TaxName", txtTaxName.Text);
                cmd.Parameters.AddWithValue("@TaxDate", DateTax.Value);
                cmd.Parameters.AddWithValue("@TransStatus", "01");
                cmd.Parameters.AddWithValue("@Notes", txttblDOPabrikH_Ket1.Text);
                cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                //cmd.Parameters.AddWithValue("@UpdatedDate", "");
                //cmd.Parameters.AddWithValue("@UpdatedBy", "");
                cmd.ExecuteNonQuery();
            }
            txtCNNo.Text = CNID;

            ListMethod.StatusLogCustomer("FrmT_NotaCredit", "NotaCredit", txtAccId.Text, "01", "", txtCNNo.Text, "", "", "");
        }

        private void insertTableDtl()
        {
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                Query = "INSERT INTO NotaCredit_Dtl VALUES (@CN_Date,@CN_No,@SeqNo,@NRJId,@NRJ_SeqNo,@CurrencyId," + Convert.ToDecimal(txtKurs.Text.ToString()) + ",@TaxStatusCode ";
                Query += "," + Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxPercent"].Value) + ",@PPHTaxStatusCode," + Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxPercent"].Value) + "," + Convert.ToDecimal(dgvDetail.Rows[i].Cells["LineAmount"].Value) + "," + Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxBaseAmount"].Value) + "," + Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxAmount"].Value) + "," + Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxAmount"].Value) + ",@Notes,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)";

                using (con = ConnectionString.GetConnection())
                using (cmd = new SqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@CN_Date", CNDate.Value);
                    cmd.Parameters.AddWithValue("@CN_No", txtCNNo.Text);
                    cmd.Parameters.AddWithValue("@SeqNo", (i + 1));
                    cmd.Parameters.AddWithValue("@NRJId", dgvDetail.Rows[i].Cells["NRJId"].Value.ToString());
                    cmd.Parameters.AddWithValue("@NRJ_SeqNo", (i + 1));
                    cmd.Parameters.AddWithValue("@CurrencyId", txtMataUang.Text);
                    //cmd.Parameters.AddWithValue("@ExchRate", Convert.ToDecimal(txtKurs.Text.ToString()));
                    cmd.Parameters.AddWithValue("@TaxStatusCode", "");
                    //cmd.Parameters.AddWithValue("@TaxPercent", Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxPercent"].Value));
                    cmd.Parameters.AddWithValue("@PPHTaxStatusCode", "");
                    //cmd.Parameters.AddWithValue("@PPHTaxPercent", Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxPercent"].Value));
                    //cmd.Parameters.AddWithValue("@LineAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["LineAmount"].Value));
                    //cmd.Parameters.AddWithValue("@TaxBaseAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxBaseAmount"].Value));
                    //cmd.Parameters.AddWithValue("@TaxAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxAmount"].Value));
                    //cmd.Parameters.AddWithValue("@PPHTaxAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxAmount"].Value));
                    cmd.Parameters.AddWithValue("@Notes", txttblDOPabrikH_Ket1.Text);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UpdatedBy", "");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void callSearchQueryV2Form(string Table, string QuerySearch, string[] FilterText, string[] Select, string PrimaryKey, string[] HideField)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();

            populateAfterSearch(Table);
        }

        private void populateAfterSearch(string Table)
        {
            if (Table == "[dbo].[CustTable]" && Variable.Kode2 != null)
            {
                if (Variable.Kode2.GetUpperBound(1) == 7)
                {
                    txtAccId.Text = Variable.Kode2[0, 0];
                    txtAccName.Text = Variable.Kode2[0, 1];
                    txtNPWP.Text = Variable.Kode2[0, 2];
                    txtPPN.Text = Variable.Kode2[0, 3];
                    txtPPH.Text = Variable.Kode2[0, 4];
                    txtTaxName.Text = Variable.Kode2[0, 5];
                    txtTaxAddress.Text = Variable.Kode2[0, 6];
                    txtMataUang.Text = Variable.Kode2[0, 7];
                    populate4(Variable.Kode2[0, 7]);
                }
                Variable.Kode2 = null;
            }
            if (Table == "[dbo].[CurrencyTable]" && Variable.Kode2 != null)
            {
                if (Variable.Kode2.GetUpperBound(1) == 1)
                {
                    txtMataUang.Text = Variable.Kode2[0, 0];
                    populate4(Variable.Kode2[0, 0]);
                }
                Variable.Kode2 = null;
            }
            if (Table == "[dbo].[NotaReturJualH]" && Variable.Kode2 != null)
            {
                if (Variable.Kode2.GetUpperBound(1) == 3)
                {
                    using (Method C = new Method())
                    {
                        if (dgvDetail.Rows.Count <= 0)
                        {
                            dgvDetail.Columns.Clear();
                            C.DgvCreate(dgvDetail, new string[] { "CN_Date", "CN_No", "SeqNo", "NRJId", "NRJ_SeqNo","SO_Reff", "CurrencyId", "ExchRate", "TaxStatusCode", "TaxPercent", "PPHTaxStatusCode", "PPHTaxPercent", "LineAmount", "TaxBaseAmount", "TaxAmount", "PPHTaxAmount", "Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" });
                        }
                        for (int i = 0; i <= Variable.Kode2.GetUpperBound(0); i++)
                        {
                            Query = "SELECT COUNT(NRJId) FROM [dbo].[NotaReturJual_Dtl] WHERE NRJId = '" + Variable.Kode2[i, 0] + "'";
                            using (con = ConnectionString.GetConnection())
                            using (cmd = new SqlCommand(Query, con))
                            {
                                decimal LineAmount = 0;
                                for (int x = 0; x < Convert.ToInt32(cmd.ExecuteScalar()); x++)
                                {
                                    string FullItemId = "";
                                    decimal UoM_Qty = 0;
                                    Query = "SELECT [FullItemId], [UoM_Qty] FROM [dbo].[NotaReturJual_Dtl] WHERE [NRJId] ='" + Variable.Kode2[i, 0] + "' and [SeqNo]=" + (x + 1) + "";
                                    using (SqlCommand cmd2 = new SqlCommand(Query, con))
                                    {
                                        Dr = cmd2.ExecuteReader();
                                        while (Dr.Read())
                                        {
                                            FullItemId = Dr["FullItemId"].ToString();
                                            UoM_Qty = Convert.ToDecimal(Dr["UoM_Qty"]);
                                        }
                                        Dr.Close();
                                    }
                                    Query = "SELECT [Price] FROM [dbo].[SalesOrderD] WHERE [SalesOrderNo]='" + Variable.Kode2[i, 2] + "' AND [FullItemID]='" + FullItemId + "'";
                                    using (SqlCommand cmd2 = new SqlCommand(Query, con))
                                    {
                                        Dr = cmd2.ExecuteReader();
                                        if (Dr.HasRows)
                                        {
                                            while (Dr.Read())
                                            {
                                                LineAmount += (Convert.ToDecimal(Dr["Price"]) * UoM_Qty);
                                            }
                                        }
                                        else
                                        {
                                            Query = "SELECT [UoM_AvgPrice] FROM InventTable WHERE [FullItemID] ='" + FullItemId + "'";
                                            using (SqlCommand cmd3 = new SqlCommand(Query, con))
                                            {
                                                LineAmount += (Convert.ToDecimal(cmd3.ExecuteScalar()) * UoM_Qty);
                                            }
                                        }
                                        Dr.Close();
                                    }
                                }

                                string CNdate = CNDate.Value.Date.ToString("dd/MM/yyyy");
                                string CreatedDate = null;
                                string UpdatedDate = null;

                                dgvDetail.Rows.Add(CNdate, "", (i + 1), Variable.Kode2[i, 0], (i + 1), GetSOfromNRJId(Variable.Kode2[i, 0]), "", String.Format("{0:#,##0.###0}", 0), "", txtPPN.Text, "", txtPPH.Text, String.Format("{0:#,##0.###0}", LineAmount), String.Format("{0:#,##0.###0}", 0), String.Format("{0:#,##0.###0}", (Convert.ToDecimal(txtPPN.Text) * LineAmount) / Convert.ToDecimal(100)), String.Format("{0:#,##0.###0}", (Convert.ToDecimal(txtPPH.Text) * LineAmount) / Convert.ToDecimal(100)), "", CreatedDate, "", UpdatedDate, "");

                            }
                        }
                        HideField = new string[] { "CN_Date", "CN_No", "SeqNo", "NRJ_SeqNo", "CurrencyId", "ExchRate", "TaxStatusCode", "PPHTaxStatusCode" };
                        C.DgvVisible(dgvDetail, HideField);
                        dgvDetail.AutoResizeColumns();
                    }

                }
                Variable.Kode2 = null;
            }
        }

        private void populate4(string currency) //untuk kurs
        {
            if (txtMataUang.Text != null && txtMataUang.Text.Trim() != "")
            {
                if (currency.ToUpper() == "IDR")
                {
                    txtKurs.Text = "1";
                }
                else if (currency != "" && currency != null)
                {
                    Query = "SELECT TOP 1 [ExchRate] FROM [dbo].[ExchRate] WHERE [CurrencyId]='" + currency + "' AND CreatedDate > '" + DateTime.Now.Date + "' ORDER BY UpdatedDate DESC";
                    using (con = ConnectionString.GetConnection())
                    using (cmd = new SqlCommand(Query, con))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                txtKurs.Text = Dr["ExchRate"].ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Nilai kurs hari ini belom ada!");
                            txtKurs.Enabled = true;
                        }
                        Dr.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Default Currency Customer belom terdaftar.");
                }
            }
        }

        private void btnSearchAcc_Click(object sender, EventArgs e)
        {
            string Table = "[dbo].[CustTable]";
            string QuerySearch = "SELECT [CustId], [CustName], [NPWP], [PPN], [PPH], [TaxName], [TaxAddress],[CurrencyId] FROM [dbo].[CustTable] ";
            string[] FilterText = { "CustId", "CustName" };
            string[] Select = { "CustId", "CustName", "NPWP", "PPN", "PPH", "TaxName", "TaxAddress", "CurrencyId" };
            string PrimaryKey = "CustId";
            string[] HideField = { "Check", "NPWP", "PPN", "PPH", "TaxName", "TaxAddress", "CurrencyId" };
            callSearchQueryV2Form(Table, QuerySearch, FilterText, Select, PrimaryKey, HideField);
            dgvDetail.Columns.Clear();
        }

        private void btnSearchKurs_Click(object sender, EventArgs e)
        {
            string Table = "[dbo].[CurrencyTable]";
            string QuerySearch = "SELECT [CurrencyID], [CurrencyName] FROM [dbo].[CurrencyTable]  ";
            string[] FilterText = { "CurrencyID", "CurrencyName" };
            string[] Select = { "CurrencyID", "CurrencyName" };
            string PrimaryKey = "CurrencyID";
            string[] HideField = { "Check" };
            callSearchQueryV2Form(Table, QuerySearch, FilterText, Select, PrimaryKey, HideField);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (txtAccId.Text != "" && txtAccId.Text != null)
            {
                string Table = "[dbo].[NotaReturJualH]";
                string QuerySearch = "SELECT a.[NRJId], a.[NRJDate], a.[GoodsIssuedId],a.[SalesId],a.[SiteId]  FROM [dbo].[NotaReturJualH] a LEFT JOIN [dbo].[NotaCredit_Dtl] b ON a.NRJId=b.NRJId WHERE b.NRJId IS NULL AND a.CustId ='" + txtAccId.Text + "' AND (a.TransStatusId = '04' OR a.TransStatusId = '03') ";
                for (int i = 0; i < dgvDetail.Rows.Count; i++)
                {
                    QuerySearch += " AND NOT a.NRJId = '" + dgvDetail.Rows[i].Cells["NRJId"].Value.ToString() + "'";
                }
                string[] FilterText = { "NRJId", "GoodsIssuedId", "SalesId", "SiteId" };
                string[] Select = { "NRJId", "GoodsIssuedId", "SalesId", "SiteId" };
                string PrimaryKey = "NRJId";
                string[] HideField = { };
                callSearchQueryV2Form(Table, QuerySearch, FilterText, Select, PrimaryKey, HideField);
            }
            else
            {
                MessageBox.Show("Account Id belom terpilih!");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDetail.Rows.Count > 0)
            {
                dgvDetail.Rows.RemoveAt(dgvDetail.CurrentRow.Index);
            }
        }

        private void populate3()
        {
            decimal totalline = 0;
            decimal pph = 0;
            decimal ppn = 0;
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                totalline += Convert.ToDecimal(dgvDetail.Rows[i].Cells["LineAmount"].Value);
                pph += Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxAmount"].Value);
                ppn += Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxAmount"].Value);
            }

            txtPPHbwh.Text = String.Format("{0:#,##0.###0}", pph);
            txtPPNbwh.Text = String.Format("{0:#,##0.###0}", ppn);
            txtTotalNett.Text = String.Format("{0:#,##0.###0}", totalline);
            txtTotal.Text = String.Format("{0:#,##0.###0}", (totalline + ppn + pph));
        }

        private void dgvDetail_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            populate3();
        }

        private void dgvDetail_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            populate3();
        }

        private void txtPPN_TextChanged(object sender, EventArgs e)
        {
            if (txtPPN.Text == null || txtPPN.Text.ToString().Trim() == "")
            {
                txtPPN.Text = "0.00";
            }
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                if (txtPPN.Text != null && txtPPN.Text.Trim() != "")
                {
                    dgvDetail.Rows[i].Cells["TaxPercent"].Value = txtPPN.Text;
                    dgvDetail.Rows[i].Cells["TaxAmount"].Value = Convert.ToDecimal(dgvDetail.Rows[i].Cells["LineAmount"].Value) * Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxPercent"].Value) / (Decimal)100;
                }
            }
            populate3();
        }

        private void txtPPH_TextChanged(object sender, EventArgs e)
        {
            if (txtPPH.Text == null || txtPPH.Text.ToString().Trim() == "")
            {
                txtPPH.Text = "0.00";
            }
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                if (txtPPH.Text != null && txtPPH.Text.Trim() != "")
                {
                    dgvDetail.Rows[i].Cells["PPHTaxPercent"].Value = txtPPH.Text;
                    dgvDetail.Rows[i].Cells["PPHTaxAmount"].Value = Convert.ToDecimal(dgvDetail.Rows[i].Cells["LineAmount"].Value) * Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxPercent"].Value) / (Decimal)100;
                }
            }
            populate3();
        }

        private void txtMataUang_TextChanged(object sender, EventArgs e)
        {
            if (txtMataUang.Text.ToUpper() == "IDR")
            {
                txtKurs.Enabled = false;
            }
        }

        private void txtPPN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void txtPPH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void txtKurs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void txtKurs_TextChanged(object sender, EventArgs e)
        {
            if (txtKurs.Text == null || txtKurs.Text.ToString().Trim() == "")
            {
                txtKurs.Text = "0.00";
            }
        }

        private void FrmT_NotaCredit_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshDataGrid();
        }
        //tia edit
        //klik kanan
        PopUp.Vendor.Vendor Vend = null;
        Sales.NotaReturJual.NRJHeader NrjId = null;

        AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice ParentToCI;

        public void ParentRefreshGrid(AccountsReceivable.CustomerInvoice.HeaderCustomerInvoice ci)
        {
            ParentToCI = ci;
        }

        private void txtAccId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtAccId.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtAccId.Text);
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

        private void txtAccName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    txtAccName.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(txtAccId.Text);
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

        private void dgvDetail_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (NrjId == null || NrjId.Text == "")
                {
                    if (dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "NRJId")
                    {
                        NrjId = new Sales.NotaReturJual.NRJHeader();
                        NrjId.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["NRJId"].Value.ToString());
                        NrjId.ParentRefreshGrid(this);
                        NrjId.Show();
                    }
                }
                else if (CheckOpened(NrjId.Name))
                {
                    NrjId.WindowState = FormWindowState.Normal;
                    NrjId.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["NRJId"].Value.ToString());
                    NrjId.ParentRefreshGrid(this);
                    NrjId.Show();
                    NrjId.Focus();
                }
            }
        }
        //tia edit end

        private void btnApproval_Click(object sender, EventArgs e)
        {
            if (txtCNNo.Text == "")
            {
                return;
            }
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    SqlConnection Conn = ConnectionString.GetConnection();
                    if (this.PermissionAccess(ControlMgr.Approve) > 0)
                    {
                        string Status = "";
                        Query = "SELECT [TransStatus] FROM NotaCreditH WHERE [CN_No] = @CN_No";
                        using (cmd = new SqlCommand(Query, Conn))
                        {
                            cmd.Parameters.AddWithValue("@CN_No", txtCNNo.Text);
                            Dr = cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    if (Dr["TransStatus"] != System.DBNull.Value)
                                    {
                                        Status = Dr["TransStatus"].ToString();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Nota Credit "+txtCNNo.Text+" tidak mempunyai status, hubungi pihak IT.");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Tidak ada transaksi nota tersebut pada database.");
                            }
                        }
                        if (Status == "01")
                        {
                            Query = "UPDATE NotaCreditH SET [TransStatus]='03',UpdatedDate = getdate(), UpdatedBy = '"+ControlMgr.UserId+"'  WHERE [CN_No] = @CN_No";
                            using (cmd = new SqlCommand(Query, Conn))
                            {
                                cmd.Parameters.AddWithValue("@CN_No", txtCNNo.Text);
                                cmd.ExecuteNonQuery();                                
                            }
                            ListMethod.StatusLogCustomer("FrmT_NotaCredit", "NotaCredit", txtAccId.Text, "03", "", txtCNNo.Text, "", "", "");

                            //Begin
                            //Created By : Joshua
                            //Created Date : 08 Sept 2018
                            //Desc : Create Journal
                            CreateJournal(Conn);
                            //End

                            MessageBox.Show("Nota Credit berhasil di approve.");
                        }
                        else
                        {
                            MessageBox.Show("Nota Credit tersebut tidak dapat di approve.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Access Denied.");
                    }
                    Conn.Close();
                    scope.Complete();
                }
                btnApproval.Enabled = false;
                Parent.RefreshDataGrid();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }

        private void CreateJournal(SqlConnection Conn)
        {
            //Begin
            //Created By : Joshua
            //Created Date : 08 Sept 2018
            //Desc : Create Journal

            SqlCommand Cmd;

            //Insert Header GLJournal
            string JournalHID = "AR15";
            string Jenis = "JN", Kode = "JN";
            string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Cmd = new SqlCommand());
            string Notes = txtAccId.Text + " - " + txtAccName.Text;


            Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
            Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + txtCNNo.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            //Select Config Journal
            int SeqNo = 1;
            int JournalIDSeqNo = 0;
            string Type = "";
            string FQA_ID = "";
            string FQA_Desc = "";
            decimal AmountValue = 0;

            Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                Type = Convert.ToString(Dr["Type"]);
                FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                AmountValue = 0;

                if (JournalHID == "AR15")
                {
                    if (JournalIDSeqNo == 1)
                    {
                        AmountValue = Convert.ToDecimal(txtTotal.Text);
                    }
                    else if (JournalIDSeqNo == 2)
                    {
                        AmountValue = Convert.ToDecimal(txtPPNbwh.Text) + Convert.ToDecimal(txtPPHbwh.Text);
                    }
                    else if (JournalIDSeqNo == 3)
                    {
                        AmountValue = Convert.ToDecimal(txtTotal.Text) + Convert.ToDecimal(txtPPNbwh.Text) + Convert.ToDecimal(txtPPHbwh.Text);
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
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                SeqNo++;
            }
            Dr.Close();


            //End
        }

        private void btnUnapprove_Click(object sender, EventArgs e)
        {
            if (txtCNNo.Text == "")
            {
                return;
            }
            try
            {
                
                using (TransactionScope scope = new TransactionScope())
                {
                    SqlConnection Conn = ConnectionString.GetConnection();
                    if (this.PermissionAccess(ControlMgr.Approve) > 0)
                    {
                        string Status = "";
                        Query = "SELECT [TransStatus] FROM NotaCreditH WHERE [CN_No] = @CN_No";
                        using (cmd = new SqlCommand(Query, Conn))
                        {
                            cmd.Parameters.AddWithValue("@CN_No", txtCNNo.Text);
                            Dr = cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                while (Dr.Read())
                                {
                                    if (Dr["TransStatus"] != System.DBNull.Value)
                                    {
                                        Status = Dr["TransStatus"].ToString();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Nota Credit " + txtCNNo.Text + " tidak mempunyai status, hubungi pihak IT.");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Tidak ada transaksi nota tersebut pada database.");
                            }
                        }
                        if (Status == "03")
                        {
                            Query = "UPDATE NotaCreditH SET [TransStatus]='01',UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "'  WHERE [CN_No] = @CN_No";
                            using (cmd = new SqlCommand(Query, Conn))
                            {
                                cmd.Parameters.AddWithValue("@CN_No", txtCNNo.Text);
                                cmd.ExecuteNonQuery();
                            }

                            ListMethod.StatusLogCustomer("FrmT_NotaCredit", "NotaCredit", txtAccId.Text, "01", "Unapproved", txtCNNo.Text, "", "", "");


                            //Begin
                            //Created By : Joshua
                            //Created Date : 06 Sep 2018
                            //Desc : Closed Journal
                            UnapproveJournal(Conn);
                            if (Journal == true)
                            {
                                Journal = false;
                                goto Outer;
                            }
                            //End

                            MessageBox.Show("Nota Credit berhasil di Unapprove.");
                        }
                        else
                        {
                            MessageBox.Show("Nota Credit tersebut tidak dapat di Unapprove.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Access Denied.");
                    }
                    Conn.Close();
                    scope.Complete();
                }

                Outer: ;

                
                btnApproval.Enabled = false;
                Parent.RefreshDataGrid();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }

        private void UnapproveJournal(SqlConnection Conn)
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 08 Sept 2018
            //Desc : Batal Journal

            SqlCommand Cmd;

            //Get GLJournalHID
            Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + txtCNNo.Text + "' ";
            Cmd = new SqlCommand(Query, Conn);
            string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

            Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
            Cmd = new SqlCommand(Query, Conn);
            int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

            if (CountData == 1)
            {
                //Delete Journal Detail
                Query = "UPDATE GLJournalH SET Status = 'Batal' WHERE GLJournalHID = '" + GLJournalHID + "'";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
            else
            {
                MetroFramework.MetroMessageBox.Show(this, "Tidak dapat Unapprove karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Journal = true;
                return;
            }

            //End
        }
        
    }
}
