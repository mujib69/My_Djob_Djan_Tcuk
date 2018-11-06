using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.ReceiptOrder
{
    public partial class PO : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        int flagRefresh = 0;
        String POID = null;

        Purchase.ReceiptOrder.HeaderReceiptOrder Parent;

        public PO()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.ReceiptOrder.HeaderReceiptOrder F)
        {
            Parent = F;
        }

        public void flag(String tmpPOID)
        {
            POID = tmpPOID;
        }

        //private void CreateTable()
        //{
        //    try
        //    {
        //        dgvPO.ColumnCount = 22;
        //        dgvPO.Columns[0].Name = "No";
        //        dgvPO.Columns[1].Name = "FullItemID";
        //        dgvPO.Columns[2].Name = "GroupId";
        //        dgvPO.Columns[3].Name = "SubGroup1Id";
        //        dgvPO.Columns[4].Name = "SubGroup2Id";
        //        dgvPO.Columns[5].Name = "ItemId";
        //        dgvPO.Columns[6].Name = "ItemName";
        //        dgvPO.Columns[7].Name = "POQty"; dgvPO.Columns[7].HeaderText = "PO Qty";
        //        dgvPO.Columns[8].Name = "PORemaining"; dgvPO.Columns[8].HeaderText = "PO Remaining";
        //        dgvPO.Columns[9].Name = "POUnit"; dgvPO.Columns[9].HeaderText = "PO Unit";
        //        dgvPO.Columns[10].Name = "Ratio";
        //        dgvPO.Columns[11].Name = "Price";
        //        dgvPO.Columns[12].Name = "Price_KG";
        //        dgvPO.Columns[13].Name = "Total";
        //        dgvPO.Columns[14].Name = "Diskon";
        //        dgvPO.Columns[15].Name = "Total_Disk"; dgvPO.Columns[19].HeaderText = "Total Diskon";
        //        dgvPO.Columns[16].Name = "Total_PPN"; dgvPO.Columns[20].HeaderText = "Total PPN";
        //        dgvPO.Columns[17].Name = "Total_PPH"; dgvPO.Columns[21].HeaderText = "Total PPH";
        //        dgvPO.Columns[18].Name = "PurchaseOrderID";
        //        dgvPO.Columns[19].Name = "PurchaseOrderSeqNo";
        //        dgvPO.Columns[20].Name = "DeliveryMethod";

        //        DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
        //        dgvPO.Columns.Add(chk);
        //        chk.HeaderText = "";
        //        chk.Name = "chk";
                
        //    }
        //    catch (Exception x)
        //    {
        //        MessageBox.Show(x.Message);
        //    }
        //}

        private void RefreshGrid()
        {
            dgvPO.AutoGenerateColumns = true;
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select a.FullItemID, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.ItemName, a.Qty, a.RemainingQty, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, a.Price_KG, a.Total, a.Diskon, a.Total_Disk, a.Total_PPN, a.Total_PPH, a.PurchID, a.SeqNo, a.DeliveryMethod From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID Where a.PurchID = '" + POID + "' And ";
                Query += Parent.getPOID() + ";";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select a.FullItemID, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.ItemName, a.Qty, a.RemainingQty, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, a.Price_KG, a.Total, a.Diskon, a.Total_Disk, a.Total_PPN, a.Total_PPH, a.PurchID, a.SeqNo, a.DeliveryMethod From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
                Query += "Where (a.PurchID Like '%" + txtSearch.Text + "%') And Where a.PurchID = '" + POID + "' And ";
                Query += Parent.getPOID() + ";";
            }
            else
            {
                Query = "Select a.FullItemID, a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.ItemName, a.Qty, a.RemainingQty, a.Unit, b.UoM, a.KOnv_Ratio, a.Price, a.Price_KG, a.Total, a.Diskon, a.Total_Disk, a.Total_PPN, a.Total_PPH, a.PurchID, a.SeqNo, a.DeliveryMethod From [dbo].[PurchDtl] a Left JOIN [dbo].[InventTable] b ON a.FullItemID = b.FullItemID ";
                Query += "Where (" + crit + " Like '%" + txtSearch.Text + "%') And Where a.PurchID = '" + POID + "' And ";
                Query += Parent.getPOID() + ";";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();


            dgvPO.AutoGenerateColumns = true;
            dgvPO.DataSource = Dt;
            dgvPO.Refresh();
            if (dgvPO.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvPO.Columns.Add(chk);
                chk.HeaderText = "";
                chk.Name = "chk";
            }
            Da.Fill(Dt);

            for (int i = 0; i < 22; i++)
            {
                if (i == 0)
                {
                    dgvPO.Columns[i].ReadOnly = false;
                }
                else
                {
                    dgvPO.Columns[i].ReadOnly = true;
                }
            }

            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
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
            List<string> POID = new List<string>();
            List<string> SeqNo = new List<string>();

            for (int i = 0; i <= dgvPO.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvPO.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    POID.Add(dgvPO.Rows[i].Cells["PurchId"].Value == null ? "" : dgvPO.Rows[i].Cells["PurchId"].Value.ToString());
                    SeqNo.Add(dgvPO.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvPO.Rows[i].Cells["SeqNo"].Value.ToString());
                }
            }
            Parent.AddDataGridFromDetail(POID,SeqNo);

            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            //for (int i = 0; i <= dgvVendor.RowCount - 1; i++)
            //{
            //    dgvVendor.Rows[i].Cells["chk"].Value = Check;
            //}
        }

    }
}
