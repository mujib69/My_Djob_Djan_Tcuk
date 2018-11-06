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

namespace ISBS_New.Purchase.NotaDebet
{
    public partial class FrmT_NotaDebet : MetroFramework.Forms.MetroForm
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
        ContextMenu vendid = new ContextMenu();


        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public FrmT_NotaDebet()
        {
            InitializeComponent();
            DueDate.Value = DateTime.Now;
            dateTax.Value = DateTime.Now;
            DNDate.Value = DateTime.Now;
        }

        private void FrmT_NotaDebet_Load(object sender, EventArgs e)
        {  
            if (validate() == true)
            {
                txtDNNo.Text = Variable.Kode[0];
                DNDate.Text = Variable.Kode[1];
                populate();
                refreshGrid();
            }
            mode(Mode);
        }

        private void BtnExitHeader_NotaDebet_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnEditHeader_NotaDebet_Click(object sender, EventArgs e)
        {
            mode("Edit");
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
                BtnEditHeader_NotaDebet.Enabled = false;
                BtnSaveHeader_NotaDebet.Enabled = true;
                BtnCancelHeader_NotaDebet.Enabled = true;
                BtnExitHeader_NotaDebet.Enabled = false;
                //TEXT
                txttblDOPabrikH_Ket1.Enabled = true;
                dateTax.Enabled = true;
                txtTaxNumber.Enabled = true;
                txtTaxName.Enabled = true;
                txtTaxAddess.Enabled = true;
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
                BtnEditHeader_NotaDebet.Enabled = true;
                BtnSaveHeader_NotaDebet.Enabled = false;
                BtnCancelHeader_NotaDebet.Enabled = false;
                BtnExitHeader_NotaDebet.Enabled = true;
                //text
                txttblDOPabrikH_Ket1.Enabled = false;
                dateTax.Enabled = false;
                txtTaxNumber.Enabled = false;
                txtTaxName.Enabled = false;
                txtTaxAddess.Enabled = false;
                txtPPH.Enabled = false;
                txtPPN.Enabled = false;
                DueDate.Enabled = false;
                //tia edit
                txtAccId.Enabled = true;
                txtAccId.ReadOnly = true;
                txtAccName.Enabled = true;
                txtAccName.ReadOnly = true;
                txtAccId.ContextMenu = vendid;
                txtAccName.ContextMenu = vendid;

                //tia end
            }
            else if (mode.ToUpper() == "NEW")
            {
                //BUTTON
                btnSearchAcc.Enabled = true;
                btnSearchKurs.Enabled = true;
                btnNew.Enabled = true;
                btnDelete.Enabled = true;
                BtnEditHeader_NotaDebet.Enabled = false;
                BtnSaveHeader_NotaDebet.Enabled = true;
                BtnCancelHeader_NotaDebet.Enabled = false;
                BtnExitHeader_NotaDebet.Enabled = true;
                //text
                txttblDOPabrikH_Ket1.Enabled = true;
                dateTax.Enabled = true;
                txtTaxNumber.Enabled = true;
                txtTaxName.Enabled = true;
                txtTaxAddess.Enabled = true;
                txtPPH.Enabled = true;
                txtPPN.Enabled = true;
                DueDate.Enabled = true;
            }
            
        }

        private void BtnSaveHeader_NotaDebet_Click(object sender, EventArgs e)
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
                                ListMethod.StatusLogVendor("NotaDebet", "NotaDebet", txtAccId.Text, "01", "", txtDNNo.Text, "", "", "");
                                MessageBox.Show("Data berhasil di input");
                            }
                            else if (Mode.ToUpper() == "EDIT")
                            {
                                updateTable();
                                ListMethod.StatusLogVendor("NotaDebet", "NotaDebet", txtAccId.Text, "01", "Edit", txtDNNo.Text, "", "", "");
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
                MessageBox.Show("Vendor belom terpilih!");
            }
            else if (dgvDetail.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada NRB yang dipilih!");
            }
            else if (txtTaxName.Text == null || txtTaxName.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Tax Name kosong!");
            }
            else if (txtTaxAddess.Text == null || txtTaxAddess.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Tax Address kosong!");
            }
            else if (txtTaxNumber.Text == null || txtTaxNumber.Text.ToString().Trim() == "")
            {
                MessageBox.Show("Tax Number kosong!");
            }
            else if(Convert.ToDecimal(txtKurs.Text) == 0)
            {
                MessageBox.Show("Nilai kurs tidak boleh 0.");
            }
            else
            {
                if (Mode.ToUpper() == "NEW")
                {
                    for (int i = 0; i < dgvDetail.Rows.Count; i++)
                    {
                        Query = "SELECT a.* FROM NotaDebet_Dtl a INNER JOIN NotaReturBeliH b ON a.NRBId=b.NRBId WHERE a.NRBId = '" + dgvDetail.Rows[i].Cells["NRBId"].Value.ToString() + "'";
                        using (cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Dr = cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {
                                MessageBox.Show("NRBId sudah terinput dalam Nota Debet lain.");
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
                else
                {
                    status = true;
                }
            }
            return status;
        }

        private void insertTableH()
        {
            string DNID ="";

            Query = "INSERT INTO NotaDebetH VALUES (@DN_Date,@DN_No,@DNMode,@NRBId,@DueDate,@AccountNum,@AccountName,@CurrencyId ";
            Query += ",@ExchRate,@TaxStatusCode,@TaxPercent,@PPHTaxStatusCode,@PPHTaxPercent,@TotalAmount,@TaxBaseAmount,@TaxAmount ";
            Query += ",@PPHTaxAmount,@NPWP,@TaxNum,@TaxAddress,@TaxName,@TaxDate,@TransStatus,@Notes,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)";
            using(con = ConnectionString.GetConnection())
            using(cmd = new SqlCommand(Query,con))
            {
                SqlCommand cmd2 = new SqlCommand();
                string Jenis = "DN";
                string Kode = "DN";
                DNID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", con, cmd2);

                decimal taxbaseamount = (Convert.ToDecimal(txtPPNbwh.Text) + Convert.ToDecimal(txtPPHbwh.Text));
                cmd.Parameters.AddWithValue("@DN_Date",DNDate.Value);
                cmd.Parameters.AddWithValue("@DN_No",DNID);
                cmd.Parameters.AddWithValue("@DNMode","");
                cmd.Parameters.AddWithValue("@NRBId","");
                cmd.Parameters.AddWithValue("@DueDate",DueDate.Value);
                cmd.Parameters.AddWithValue("@AccountNum",txtAccId.Text);
                cmd.Parameters.AddWithValue("@AccountName",txtAccName.Text);
                cmd.Parameters.AddWithValue("@CurrencyId",txtMataUang.Text);
                cmd.Parameters.AddWithValue("@ExchRate",(txtKurs.Text));
                cmd.Parameters.AddWithValue("@TaxStatusCode","");
                cmd.Parameters.AddWithValue("@TaxPercent",Convert.ToDecimal(txtPPN.Text.ToString()));
                cmd.Parameters.AddWithValue("@PPHTaxStatusCode","");
                cmd.Parameters.AddWithValue("@PPHTaxPercent", Convert.ToDecimal(txtPPH.Text.ToString()));
                cmd.Parameters.AddWithValue("@TotalAmount", Convert.ToDecimal(txtTotalNett.Text.ToString()));
                cmd.Parameters.AddWithValue("@TaxBaseAmount",taxbaseamount);
                cmd.Parameters.AddWithValue("@TaxAmount", Convert.ToDecimal(txtPPNbwh.Text.ToString()));
                cmd.Parameters.AddWithValue("@PPHTaxAmount", Convert.ToDecimal(txtPPHbwh.Text.ToString()));
                cmd.Parameters.AddWithValue("@NPWP",txtNPWP.Text);
                cmd.Parameters.AddWithValue("@TaxNum",txtTaxNumber.Text);
                cmd.Parameters.AddWithValue("@TaxAddress",txtTaxAddess.Text);
                cmd.Parameters.AddWithValue("@TaxName",txtTaxName.Text);
                cmd.Parameters.AddWithValue("@TaxDate",dateTax.Value);
                cmd.Parameters.AddWithValue("@TransStatus","01");
                cmd.Parameters.AddWithValue("@Notes",txttblDOPabrikH_Ket1.Text);
                cmd.Parameters.AddWithValue("@CreatedDate",DateTime.Now);
                cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                cmd.Parameters.AddWithValue("@UpdatedDate","");
                cmd.Parameters.AddWithValue("@UpdatedBy","");
                cmd.ExecuteNonQuery();
            }
            txtDNNo.Text = DNID;
        }

        private void insertTableDtl()
        {
            for (int i = 0; i < dgvDetail.Rows.Count; i++)
            {
                Query = "INSERT INTO NotaDebet_Dtl VALUES (@DN_Date,@DN_No,@SeqNo,@NRBId,@NRB_SeqNo,@CurrencyId,@ExchRate,@TaxStatusCode ";
                Query += ",@TaxPercent,@PPHTaxStatusCode,@PPHTaxPercent,@LineAmount,@TaxBaseAmount,@TaxAmount,@PPHTaxAmount,@Notes,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)";

                using (con = ConnectionString.GetConnection())
                using (cmd = new SqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@DN_Date", DNDate.Value);
                    cmd.Parameters.AddWithValue("@DN_No", txtDNNo.Text);
                    cmd.Parameters.AddWithValue("@SeqNo", (i + 1));
                    cmd.Parameters.AddWithValue("@NRBId", dgvDetail.Rows[i].Cells["NRBId"].Value.ToString());
                    cmd.Parameters.AddWithValue("@NRB_SeqNo", (i + 1));
                    cmd.Parameters.AddWithValue("@CurrencyId", txtMataUang.Text);
                    cmd.Parameters.AddWithValue("@ExchRate", Convert.ToDecimal(txtKurs.Text.ToString()));
                    cmd.Parameters.AddWithValue("@TaxStatusCode", "");
                    cmd.Parameters.AddWithValue("@TaxPercent", Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxPercent"].Value));
                    cmd.Parameters.AddWithValue("@PPHTaxStatusCode", "");
                    cmd.Parameters.AddWithValue("@PPHTaxPercent", Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxPercent"].Value));
                    cmd.Parameters.AddWithValue("@LineAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["LineAmount"].Value));
                    cmd.Parameters.AddWithValue("@TaxBaseAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxBaseAmount"].Value));
                    cmd.Parameters.AddWithValue("@TaxAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["TaxAmount"].Value));
                    cmd.Parameters.AddWithValue("@PPHTaxAmount", Convert.ToDecimal(dgvDetail.Rows[i].Cells["PPHTaxAmount"].Value));
                    cmd.Parameters.AddWithValue("@Notes", txttblDOPabrikH_Ket1.Text);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UpdatedBy", "");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void updateTable()
        {
            using (con = ConnectionString.GetConnection())
            {
                //update table NotaDebetH
                Query = "UPDATE NotaDebetH SET TaxPercent = "+txtPPN.Text+",PPHTaxPercent = "+txtPPH.Text+",TaxNum='" + txtTaxNumber.Text + "', TaxAddress='" + txtTaxAddess.Text + "', TaxName = '" + txtTaxName.Text + "', TaxDate = '" + dateTax.Value.Date + "', Notes = '" + txttblDOPabrikH_Ket1.Text + "',CurrencyId='" + txtMataUang.Text + "',ExchRate='" + txtKurs.Text + "',DueDate='" + DueDate.Value.Date + "', UpdatedBy = '" + ControlMgr.UserId + "', UpdatedDate = '" + DateTime.Now + "' WHERE DN_No = '" + txtDNNo.Text + "';";
                using (cmd = new SqlCommand(Query, con))
                {
                    cmd.ExecuteNonQuery();
                }

                //update table NotaDebet_Dtl
                Query = "DELETE FROM NotaDebet_Dtl WHERE DN_No = '"+txtDNNo.Text+"'";
                using(cmd = new SqlCommand(Query,con))
                {
                    cmd.ExecuteNonQuery();
                }
                insertTableDtl();
                //int RemovedLine = 0;
                //Query = "SELECT COUNT (DN_No) FROM NotaDebet_Dtl WHERE DN_No = '"+txtDNNo.Text+"'";
                //using (cmd = new SqlCommand(Query, con))
                //{
                //    int count = Convert.ToInt32(cmd.ExecuteScalar());
                //    if ( count != dgvDetail.Rows.Count)
                //    {
                //        RemovedLine = count = dgvDetail.Rows.Count;
                //    }
                //}
                //int newseq = 0;
                //Query = "SELECT SeqNo FROM NotaDebet_Dtl WHERE DN_No = '"+txtDNNo.Text+"'";
                //using (cmd = new SqlCommand(Query, con))
                //{
                //    Dr = cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        List<int> seq = new List<int>();
                //        seq.Add(Dr["SeqNo"]);
                //        for (int x = 0; x < seq.Count; x++)
                //        {
                //            for (int i = 0; i < dgvDetail.Rows.Count; i++)
                //            {
                //                if (Convert.ToInt32(dgvDetail.Rows[i].Cells["SeqNo"].Value) == seq[x])
                //                {
                //                    break;
                //                }
                //                else if (Convert.ToInt32(dgvDetail.Rows[i].Cells["SeqNo"].Value != seq[x]) && (i + 1) == dgvDetail.Rows.Count)
                //                {
                //                    string QueryDelete = "DELETE FROM NotaDebet_Dtl WHERE SeqNo = "+seq[x]+" AND DN_No = '"+txtDNNo.Text+"'";
                //                    using (SqlCommand cmd2 = new SqlCommand(QueryDelete, con))
                //                    {
                //                        cmd2.ExecuteNonQuery();
                //                    }
                //                    newseq++;
                //                }
                //            }
                //        }
                //    }
                //}
                //for (int i = 0; i < newseq; i++)
                //{
                //    Query = "UPDATE NotaDebet_Dtl SET SeqNo = " + (i + 1) + ", UpdatedDate = " + DateTime.Now + ", UpdatedBy='" + ControlMgr.GroupName + "' ";
                //}

            }
        }

        private void populate()
        {
            Query = "SELECT * FROM NotaDebetH WHERE DN_No = '"+txtDNNo.Text+"' ;";
            using(con = ConnectionString.GetConnection())
            using(cmd= new SqlCommand(Query,con))
            {
                Dr = cmd.ExecuteReader();
                while (Dr.Read())
                {
                    txtAccId.Text = Dr["AccountNum"].ToString();
                    txtAccName.Text = Dr["AccountName"].ToString();
                    txtPPH.Text = Dr["PPHTaxPercent"].ToString();
                    txtPPHbwh.Text = String.Format("{0:#,##0.###0}",Dr["PPHTaxAmount"]);
                    txtPPN.Text = Dr["TaxPercent"].ToString();
                    txtPPNbwh.Text = String.Format("{0:#,##0.###0}",Dr["TaxAmount"]);
                    txtMataUang.Text = Dr["CurrencyId"].ToString();
                    txtKurs.Text = String.Format("{0:#,##0.###0}",Dr["ExchRate"]);
                    txtTotalNett.Text = String.Format("{0:#,##0.###0}",Dr["TotalAmount"]);
                    
                    String total = String.Format("{0:#,##0.###0}",(Convert.ToInt32(Dr["TotalAmount"]) + Convert.ToInt32(Dr["TaxAmount"]) + Convert.ToInt32(Dr["PPHTaxAmount"])));
                    txtTotal.Text = String.Format("{0:#,##0.###0}",total);
                    txtNPWP.Text = Dr["NPWP"].ToString();
                    txttblDOPabrikH_Ket1.Text = Dr["Notes"].ToString();
                    dateTax.Text = Dr["TaxDate"].ToString();
                    txtTaxNumber.Text = Dr["TaxNum"].ToString();
                    txtTaxName.Text = Dr["TaxName"].ToString();
                    txtTaxAddess.Text = Dr["TaxAddress"].ToString();
                    DueDate.Text = Dr["DueDate"].ToString();
                }
                Dr.Close();
            }
        }

        private void refreshGrid()
        {
            if (txtDNNo.Text != null)
            {
                using (Method C = new Method())
                {
                    dgvDetail.Columns.Clear();
                    C.DgvCreate(dgvDetail, new string[] { "DN_Date", "DN_No", "SeqNo", "NRBId", "NRB_SeqNo", "CurrencyId", "ExchRate", "TaxStatusCode", "TaxPercent", "PPHTaxStatusCode", "PPHTaxPercent", "LineAmount", "TaxBaseAmount", "TaxAmount", "PPHTaxAmount", "Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" });
                    Query = "SELECT * from NotaDebet_Dtl WHERE DN_No = '" + txtDNNo.Text + "'";
                    C.Dr = C.ReturnDr(Query);
                    while (C.Dr.Read())
                    {
                        string DNdate = Convert.ToDateTime(C.Dr[0]).ToString("dd/MM/yyyy");
                        string CreatedDate = Convert.ToDateTime(C.Dr[16]).ToString("dd/MM/yyyy HH:mm:ss");
                        string UpdatedDate = Convert.ToDateTime(C.Dr[18]).ToString("dd/MM/yyyy HH:mm:ss");

                        dgvDetail.Rows.Add(DNdate, C.Dr[1], C.Dr[2], C.Dr[3], C.Dr[4], C.Dr[5], String.Format("{0:#,##0.###0}",C.Dr[6]), C.Dr[7], C.Dr[8], C.Dr[9], C.Dr[10], String.Format("{0:#,##0.###0}", C.Dr[11]), String.Format("{0:#,##0.###0}", C.Dr[12]), String.Format("{0:#,##0.###0}", C.Dr[13]), String.Format("{0:#,##0.###0}", C.Dr[14]), C.Dr[15], CreatedDate, C.Dr[17], UpdatedDate, C.Dr[19]);
                    }
                    HideField = new string[] { "DN_Date", "DN_No", "SeqNo", "NRB_SeqNo", "CurrencyId", "ExchRate", "TaxStatusCode", "PPHTaxStatusCode" };
                    C.DgvVisible(dgvDetail, HideField);
                    dgvDetail.AutoResizeColumns();
                }
            }
        }

        private void BtnCancelHeader_NotaDebet_Click(object sender, EventArgs e)
        {
            mode("View");
            populate();
            refreshGrid();
        }

        public void SetParent(InquiryV1 F)
        {
            Parent = F;
        }

        private void callSearchQueryV2Form(string Table, string QuerySearch, string[] FilterText, string[] Select, string PrimaryKey, string [] HideField)
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
            if (Table == "[dbo].[VendTable]" && Variable.Kode2 != null)
            {
                if (Variable.Kode2.GetUpperBound(1) == 7)
                {
                    txtAccId.Text = Variable.Kode2[0, 0];
                    txtAccName.Text = Variable.Kode2[0, 1];
                    txtNPWP.Text = Variable.Kode2[0, 2];
                    txtPPN.Text = Variable.Kode2[0, 3];
                    txtPPH.Text = Variable.Kode2[0, 4];
                    txtTaxName.Text = Variable.Kode2[0, 5];
                    txtTaxAddess.Text = Variable.Kode2[0, 6];
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
            if (Table == "[dbo].[NotaReturBeliH]" && Variable.Kode2 != null)
            {
                if (Variable.Kode2.GetUpperBound(1) == 3)
                {
                    using (Method C = new Method())
                    {
                        if (dgvDetail.Rows.Count <= 0)
                        {
                            dgvDetail.Columns.Clear();
                            C.DgvCreate(dgvDetail, new string[] { "DN_Date", "DN_No", "SeqNo", "NRBId", "NRB_SeqNo", "CurrencyId", "ExchRate", "TaxStatusCode", "TaxPercent", "PPHTaxStatusCode", "PPHTaxPercent", "LineAmount", "TaxBaseAmount", "TaxAmount", "PPHTaxAmount", "Notes", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" });
                        }
                        for (int i = 0; i <= Variable.Kode2.GetUpperBound(0); i++)
                        {
                            Query = "SELECT COUNT(NRBId) FROM [dbo].[NotaReturBeli_Dtl] WHERE NRBId = '" + Variable.Kode2[i, 0] + "'";
                            using (con = ConnectionString.GetConnection())
                            using (cmd = new SqlCommand(Query, con))
                            {
                                decimal LineAmount = 0;
                                for (int x = 0; x < Convert.ToInt32(cmd.ExecuteScalar()); x++)
                                {
                                    string FullItemId = "";
                                    decimal UoM_Qty = 0;
                                    Query = "SELECT [FullItemId], [UoM_Qty] FROM [dbo].[NotaReturBeli_Dtl] WHERE [NRBId] ='" + Variable.Kode2[i, 0] + "' and [SeqNo]=" + (x + 1) + "";
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
                                    Query = "SELECT [Price] FROM [dbo].[PurchDtl] WHERE [PurchID]='" + Variable.Kode2[i, 2] + "' AND [FullItemID]='" + FullItemId + "'";
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
                                    //ASDF
                                    //Query = "INSERT INTO NotaDebet_Dtl ";
                                }

                                string DNdate = DNDate.Value.Date.ToString("dd/MM/yyyy");
                                string CreatedDate = null;
                                string UpdatedDate = null;

                                dgvDetail.Rows.Add(DNdate, "", (i + 1), Variable.Kode2[i, 0], (i + 1), "", String.Format("{0:#,##0.###0}", 0), "", txtPPN.Text, "", txtPPH.Text, String.Format("{0:#,##0.###0}", LineAmount), String.Format("{0:#,##0.###0}", 0), String.Format("{0:#,##0.###0}", (Convert.ToDecimal(txtPPN.Text) * LineAmount)/Convert.ToDecimal(100)), String.Format("{0:#,##0.###0}", (Convert.ToDecimal(txtPPH.Text) * LineAmount)/Convert.ToDecimal(100)), "", CreatedDate, "", UpdatedDate, "");
                                
                            }
                        }
                        HideField = new string[] { "DN_Date", "DN_No", "SeqNo", "NRB_SeqNo", "CurrencyId", "ExchRate", "TaxStatusCode", "PPHTaxStatusCode" };
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
                    MessageBox.Show("Default Currency Vendor belom terdaftar.");
                }
            }
        }

        private void btnSearchAcc_Click(object sender, EventArgs e)
        {
            string Table = "[dbo].[VendTable]";
            string QuerySearch = "SELECT [VendId], [VendName], [NPWP], [PPN], [PPH], [TaxName], [TaxAddress],[CurrencyId] FROM [dbo].[VendTable] ";
            string[] FilterText = { "VendId", "VendName" };
            string[] Select = { "VendId", "VendName", "NPWP", "PPN", "PPH", "TaxName", "TaxAddress","CurrencyId" };
            string PrimaryKey = "VendId";
            string[] HideField = { "Check", "NPWP", "PPN", "PPH", "TaxName", "TaxAddress","CurrencyId" };
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
                string Table = "[dbo].[NotaReturBeliH]";
                string QuerySearch = "SELECT a.[NRBId], a.[NRBDate], a.[GoodsReceivedId],a.[PurchId],a.[SiteId]  FROM [dbo].[NotaReturBeliH] a LEFT JOIN [dbo].[NotaDebet_Dtl] b ON a.NRBId=b.NRBId WHERE b.NRBId IS NULL AND a.VendId ='" + txtAccId.Text + "' AND (a.TransStatusId = '04' OR a.TransStatusId = '03') ";
                for (int i = 0; i < dgvDetail.Rows.Count; i++)
                {
                    QuerySearch += " AND NOT a.NRBId = '" + dgvDetail.Rows[i].Cells["NRBId"].Value.ToString() + "'";
                }    
                string[] FilterText = { "NRBId", "GoodsReceivedId", "PurchId", "SiteId" };
                string[] Select = { "NRBId", "GoodsReceivedId", "PurchId", "SiteId" };
                string PrimaryKey = "NRBId";
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
            txtTotal.Text = String.Format("{0:#,##0.###0}",(totalline + ppn + pph));
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

        private void FrmT_NotaDebet_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Parent.Text == "Inquiry Nota Debet")
            {
                Parent.RefreshDataGrid();
            }
        }
        //tia edit
        //klik kanan
        PopUp.Vendor.Vendor Vendor = null;
        Purchase.NotaReturBeli.ReturBeliHeader RrbId = null;
        
        private void txtAccId_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    txtAccId.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtAccId.Text);

                    Vendor.Show();
                    //}
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
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

        private void txtAccName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    txtAccId.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(txtAccId.Text);

                    Vendor.Show();
                    //}
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void dgvDetail_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (RrbId == null || RrbId.Text == "")
                {
                    if (dgvDetail.Columns[e.ColumnIndex].Name.ToString() == "NRBId")
                    {
                        RrbId = new Purchase.NotaReturBeli.ReturBeliHeader();
                        RrbId.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["NRBId"].Value.ToString());
                        RrbId.ParentRefreshGrid(this);
                        RrbId.Show();
                    }
                }
                else if (CheckOpened(RrbId.Name))
                {
                    RrbId.WindowState = FormWindowState.Normal;
                    RrbId.SetMode("PopUp", dgvDetail.Rows[e.RowIndex].Cells["NRBId"].Value.ToString());
                    RrbId.ParentRefreshGrid(this);
                    RrbId.Show();
                    RrbId.Focus();
                }
            }
        }
        //end


    }
}
