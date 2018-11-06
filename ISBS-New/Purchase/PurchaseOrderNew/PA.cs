using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseOrderNew
{
    public partial class PA : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit, amount_qty = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        int flagRefresh = 0;
        String PAID = null;
        String POID = null;

        Purchase.PurchaseOrderNew.POForm Parent;

        public void setParent(Purchase.PurchaseOrderNew.POForm F)
        {
            Parent = F;
        }

        public void flag(String tmpPAID,String tmpPOID)
        {
            PAID = tmpPAID;
            POID = tmpPOID;
        }

        public PA()
        {
            InitializeComponent();
        }

        private void RefreshGrid()
        {
            dgvPA.AutoGenerateColumns = true;
            Conn = ConnectionString.GetConnection();
            
            if (PAID == "" && POID != "")
            {
                if (crit == null)
                {
                    Query = "Select FullItemID, ItemName, RemainingQty 'Qty', RemainingQty, Unit, KOnv_Ratio, Price, Total, Diskon, Total_Disk, Total_PPN, Total_PPH, PurchID, SeqNo, ReffSeqNo From [dbo].[PurchDtl] Where PurchID = '" + POID + "' And ";
                    Query += Parent.getPOID() + ";";
                }
                else if (crit.Equals("All"))
                {
                    Query = "Select FullItemID, ItemName, a.RemainingQty 'Qty', RemainingQty, Unit, KOnv_Ratio, Price, Total, Diskon, Total_Disk, Total_PPN, Total_PPH, PurchID, SeqNo, ReffSeqNo From [dbo].[PurchDtl] ";
                    Query += "Where (FullItemID Like '%" + txtSearch.Text + "%' OR ItemName Like '%" + txtSearch.Text + "%') And PurchID = '" + POID + "' And RemainingQty >0 and ";
                    Query += Parent.getPOID() + ";";
                }
                else
                {
                    Query = "Select FullItemID, ItemName, a.RemainingQty 'Qty', RemainingQty, Unit, KOnv_Ratio, Price, Total, Diskon, Total_Disk, Total_PPN, Total_PPH, PurchID, SeqNo, ReffSeqNo From [dbo].[PurchDtl] ";
                    Query += "Where (" + crit + " Like '%" + txtSearch.Text + "%') And  PurchID = '" + POID + "' And RemainingQty >0 and ";
                    Query += Parent.getPOID() + ";";
                }

                Da = new SqlDataAdapter(Query, Conn);
                Dt = new DataTable();


                dgvPA.DataSource = Dt;
                dgvPA.Refresh();
                if (dgvPA.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    dgvPA.Columns.Add(chk);
                    chk.HeaderText = "";
                    chk.Name = "chk";
                }
                Da.Fill(Dt);

            }
            else
            {
                Cmd = new SqlCommand("Select TransType From PurchAgreementH WHERE AgreementID = '" + PAID + "'", Conn);
                amount_qty = Cmd.ExecuteScalar().ToString();

                if (amount_qty == "AMOUNT")
                {
                    Query = "Select a.FullItemID, a.ItemName, a.RemainingAmount, a.Base, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, a.Total, a.DiscPercentage, a.DiscAmount, a.Total_PPN, a.Total_PPH, a.AgreementID, a.SeqNo, a.SeqNoGroup,c.TransType ";
                }
                else
                {
                    Query = "Select a.FullItemID, a.ItemName, a.RemainingQty, a.Base, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, a.Total, a.DiscPercentage, a.DiscAmount, a.Total_PPN, a.Total_PPH, a.AgreementID, a.SeqNo, a.SeqNoGroup,c.TransType ";
                }
                Query += "From [dbo].[PurchAgreementDtl] a ";
                Query += "Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
                Query += "Left JOIN dbo.PurchAgreementH c ON c.AgreementID = a.AgreementID ";
                    
                if (crit == null)
                {
                    Query += "Where a.AgreementID = '" + PAID + "' And ";
                }
                else if (crit.Equals("All"))
                {
                    Query += "Where (a.FullItemID Like @search OR a.ItemName Like @search) And  a.AgreementID = '" + PAID + "' And ";
                }
                else
                {
                    Query += "Where (a." + crit + " Like @search) And   a.AgreementID = '" + PAID + "' And ";
                }
                Query += Parent.getPAID() + ";";

                Da = new SqlDataAdapter(Query, Conn);
                Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                Dt = new DataTable();


                dgvPA.DataSource = Dt;
                dgvPA.Refresh();
                if (dgvPA.Columns.Contains("chk") == false)
                {
                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    dgvPA.Columns.Add(chk);
                    chk.HeaderText = "";
                    chk.Name = "chk";
                }
                Da.Fill(Dt);

                for (int i = 0; i < 17; i++)
                {
                    if (i == 0)
                    {
                        dgvPA.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvPA.Columns[i].ReadOnly = true;
                    }
                }
                dgvPA.Columns["SeqNoGroup"].Visible = false;
                dgvPA.Columns["SeqNo"].Visible = false;
                dgvPA.Columns["AgreementId"].Visible = false;
                dgvPA.Columns["TransType"].Visible = false;
                dgvPA.AutoResizeColumns();
                Conn.Close();

                lblTotal.Text = "Total Rows : " + Total.ToString();
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
                lblPage.Text = "/ " + Page2;
            }

            
        }

        private void ModeLoad()
        {
            addCmbCrit();
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text == "" ? "0" : cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
        }

        private void AddVendor_Load(object sender, EventArgs e)
        {
            ModeLoad();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            flagRefresh++;
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
            flagRefresh++;
            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            flagRefresh++;
            RefreshGrid();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                flagRefresh++;
                RefreshGrid();
            }
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                flagRefresh++;
                RefreshGrid();
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
            flagRefresh++;
        }

        private void addCmbCrit()
        {
            //cmbCriteria.Items.Add("All");
            //Conn = ConnectionString.GetConnection();
            //Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'VendTable'";

            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();

            //while (Dr.Read())
            //{
            //    cmbCriteria.Items.Add(Dr[0]);
            //}
            //cmbCriteria.SelectedIndex = 0;
            //Conn.Close();
            //cmbCriteria.Items.Add("VendID");
            //cmbCriteria.Items.Add("VendName");
            //cmbCriteria.Items.Add("TaxName");
            //cmbCriteria.Items.Add("NPWP");
            //cmbCriteria.Items.Add("PKP");
            //cmbCriteria.Items.Add("Type");
            //cmbCriteria.Items.Add("TempoBayar");
            //cmbCriteria.Items.Add("PaymentModeID");
            //cmbCriteria.Items.Add("TaxGroup");
            //cmbCriteria.Items.Add("ReffFullItemID");
            cmbCriteria.Items.Clear();
            cmbCriteria.Items.Add("All");
            cmbCriteria.Items.Add("FullItemID");
            cmbCriteria.Items.Add("ItemName");
            cmbCriteria.SelectedIndex = 0;
        }

        private void cmbShowLoad()
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select CmbValue From [Setting].[CmbBox] ";

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    SqlDataReader Dr = Command.ExecuteReader();
                    cmbShow.Items.Clear();
                    while (Dr.Read())
                        cmbShow.Items.Add(Dr["CmbValue"].ToString());
                    Dr.Close();
                }

                using (SqlCommand Command = new SqlCommand(Query, Conn))
                {
                    Total = Convert.ToInt32(Command.ExecuteScalar());
                }
                cmbShow.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                Conn.Close();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == null || txtSearch.Text.Equals(""))
            {
                MessageBox.Show("Masukkan Kata Kunci");
            }
            else if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }

            RefreshGrid();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (PAID == "" && POID != "")
            {
                List<string> PurchID = new List<string>();
                List<string> SeqNo = new List<string>();
                List<string> ReffSeqNo = new List<string>();

                for (int i = 0; i <= dgvPA.RowCount - 1; i++)
                {
                    Boolean Check = Convert.ToBoolean(dgvPA.Rows[i].Cells["chk"].Value);
                    if (Check == true)
                    {
                        PurchID.Add(dgvPA.Rows[i].Cells["PurchID"].Value == null ? "" : dgvPA.Rows[i].Cells["PurchID"].Value.ToString());
                        SeqNo.Add(dgvPA.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvPA.Rows[i].Cells["SeqNo"].Value.ToString());
                        ReffSeqNo.Add(dgvPA.Rows[i].Cells["ReffSeqNo"].Value == null ? "" : dgvPA.Rows[i].Cells["ReffSeqNo"].Value.ToString());
                    }
                }
                Parent.AddDataGridFromDetailAmend(PurchID, SeqNo, ReffSeqNo);

                this.Close();
            }
            else
            {
                List<string> PurchAID = new List<string>();
                List<string> SeqNo = new List<string>();
                List<string> SeqNoGroup = new List<string>();

                for (int i = 0; i <= dgvPA.RowCount - 1; i++)
                {
                    Boolean Check = Convert.ToBoolean(dgvPA.Rows[i].Cells["chk"].Value);
                    if (Check == true)
                    {
                        PurchAID.Add(dgvPA.Rows[i].Cells["AgreementID"].Value == null ? "" : dgvPA.Rows[i].Cells["AgreementID"].Value.ToString());
                        SeqNo.Add(dgvPA.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvPA.Rows[i].Cells["SeqNo"].Value.ToString());
                        SeqNoGroup.Add(dgvPA.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : dgvPA.Rows[i].Cells["SeqNoGroup"].Value.ToString());
                    }
                }
                Parent.AddDataGridFromDetail(PurchAID, SeqNo, SeqNoGroup);

                this.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvPA.RowCount - 1; i++)
            {
                dgvPA.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void dgvPA_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPA.Columns[e.ColumnIndex].Name == "Total" ||
                dgvPA.Columns[e.ColumnIndex].Name == "Total_Disk" ||
                dgvPA.Columns[e.ColumnIndex].Name == "Total_PPN" ||
                dgvPA.Columns[e.ColumnIndex].Name == "Total_PPH" ||
                dgvPA.Columns[e.ColumnIndex].Name == "Price" ||
                dgvPA.Columns[e.ColumnIndex].Name == "RemainingAmount" ||
                dgvPA.Columns[e.ColumnIndex].Name == "DiscAmount")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.00";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                dgvPA.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N2");
            }
            if(dgvPA.Columns[e.ColumnIndex].Name == "KOnv_Ratio")
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.0000";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N4");
            }
        }
    }
}
