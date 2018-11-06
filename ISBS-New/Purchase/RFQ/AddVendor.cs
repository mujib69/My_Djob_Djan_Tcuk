using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.RFQ
{
    public partial class AddVendor : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit, qplus = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        int flagRefresh = 0;
        String VendId = null;

        Purchase.RFQ.RFQForm Parent;

        public AddVendor()
        {
            InitializeComponent();
        }

        private void CreateTable()
        {
            try
            {
                dgvVendor.ColumnCount = 3;
                dgvVendor.Columns[0].Name = "No";
                dgvVendor.Columns[1].Name = "VendId";
                dgvVendor.Columns[2].Name = "VendName";

                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvVendor.Columns.Add(chk);
                chk.HeaderText = "";
                chk.Name = "chk";
                dgvVendor.Columns[3].Width = 40;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        public void ParentRefreshGrid(Purchase.RFQ.RFQForm F)
        {
            Parent = F;
        }

        private void RefreshGrid()
        {
            dgvVendor.AutoGenerateColumns = true;
             Conn = ConnectionString.GetConnection();
            //hasim mau edit skrg
             String prid = Parent.txtPurchReqID.Text;
             
            qplus = " VendID NOT IN (Select VendID FROM RequestForQuotationH WHERE PurchReqId=@prid AND TransStatus <> '05')";
            if (crit == null)
            {
                    
                Query = "Select VendId, VendName From [dbo].[VendTable] Where 1=1 And ";
                Query += qplus + " AND ";
                Query += Parent.getVendId() + ";";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select VendId,VendName From [dbo].[VendTable] ";
                Query += "Where (VendId Like @search or VendName Like @search) And 1=1 And ";
                Query += qplus + " AND ";
                Query += Parent.getVendId() + ";";
            }
            else if (crit.Equals("ReffFullItemID"))
            {
                Query = "Select VendId,VendName From [dbo].[VendTable] ";
                Query += "Where (ReffFullItemID1+' '+ReffFullItemID2+' '+ReffFullItemID3+' '+ReffFullItemID4+' '+ReffFullItemID5  Like @search) And 1=1 And ";
                Query += qplus + " AND ";
                Query += Parent.getVendId() + ";";
            }
            else
            {
                Query = "Select VendId,VendName From [dbo].[VendTable] ";
                Query += "Where (" + crit + " Like @search) And 1=1 And ";
                Query += qplus + " AND ";
                Query += Parent.getVendId() + ";";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@prid", prid);
            Da.SelectCommand.Parameters.AddWithValue("@search", txtSearch.Text);
            Dt = new DataTable();
            

            dgvVendor.AutoGenerateColumns = true;
            dgvVendor.DataSource = Dt;
            dgvVendor.Refresh();
            if (dgvVendor.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvVendor.Columns.Add(chk);
                chk.HeaderText = "";
                chk.Name = "chk";
            }
            Da.Fill(Dt);

            dgvVendor.ReadOnly = false;
            dgvVendor.Columns["VendId"].ReadOnly = true;
            dgvVendor.Columns["VendName"].ReadOnly = true;

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
            flagRefresh++;
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
            cmbCriteria.Items.Add("VendID");
            cmbCriteria.Items.Add("VendName");
            cmbCriteria.Items.Add("TaxName");
            cmbCriteria.Items.Add("NPWP");
            cmbCriteria.Items.Add("PKP");
            cmbCriteria.Items.Add("Type");
            cmbCriteria.Items.Add("TempoBayar");
            cmbCriteria.Items.Add("PaymentModeID");
            cmbCriteria.Items.Add("TaxGroup");
            cmbCriteria.Items.Add("ReffFullItemID");

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
            List<string> VendId = new List<string>();

            for (int i = 0; i <= dgvVendor.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvVendor.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    VendId.Add(dgvVendor.Rows[i].Cells["VendId"].Value == null ? "" : dgvVendor.Rows[i].Cells["VendID"].Value.ToString());
                }
            }
            Parent.AddDataGridFromDetail2(VendId);

            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvVendor.RowCount - 1; i++)
            {
                dgvVendor.Rows[i].Cells["chk"].Value = Check;
            }
        }



    }
}
