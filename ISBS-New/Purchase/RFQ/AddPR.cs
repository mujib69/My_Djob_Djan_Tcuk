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
    public partial class AddPR : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String TransType = null;
        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        int flagRefresh = 0;
        String PurchReqId = null;

        Purchase.RFQ.RFQForm Parent;

        public AddPR()
        {
            InitializeComponent();
        }

        public void flag(String transtype)
        {
            TransType = transtype;
        }

        private void CreateTable()
        {
            try
            {
                //if (TransType == "Fix")
                //{
                    dgvPR.ColumnCount = 9;
                    dgvPR.Columns[0].Name = "PurchReqID";
                    dgvPR.Columns[1].Name = "SeqNo";
                    dgvPR.Columns[2].Name = "Type";
                    dgvPR.Columns[3].Name = "FullItemID";
                    dgvPR.Columns[4].Name = "ItemDeskripsi";
                    dgvPR.Columns[5].Name = "DeliveryMethod";
                    dgvPR.Columns[6].Name = "Qty";
                    dgvPR.Columns[7].Name = "Unit";
                    dgvPR.Columns[8].Name = "Deskripsi";

                    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                    dgvPR.Columns.Add(chk);
                    chk.HeaderText = "";
                    chk.Name = "chk";
                    dgvPR.Columns[8].Width = 40;
                //}
                //else
                //{
                //    dgvPR.ColumnCount = 11;
                //    dgvPR.Columns[0].Name = "PurchReqID";
                //    dgvPR.Columns[1].Name = "OrderDate";
                //    dgvPR.Columns[2].Name = "SeqNo";
                //    dgvPR.Columns[3].Name = "SeqNoDtl";
                //    dgvPR.Columns[4].Name = "FullItemID";
                //    dgvPR.Columns[5].Name = "ItemName";
                //    dgvPR.Columns[6].Name = "Base";
                //    dgvPR.Columns[7].Name = "Price";
                //    dgvPR.Columns[8].Name = "GelombangID";
                //    dgvPR.Columns[9].Name = "BracketID";

                //    DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                //    dgvPR.Columns.Add(chk);
                //    chk.HeaderText = "";
                //    chk.Name = "chk";
                //    dgvPR.Columns[10].Width = 40;
                //}
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
            dgvPR.AutoGenerateColumns = true;
            Conn = ConnectionString.GetConnection();

            //if (Parent.dgvDetails.Rows.Count > 0)
            //{
            if (crit == null)
            {
                Query = "Select a.PurchReqID, SeqNo, b.TransType, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi From [dbo].[PurchRequisition_Dtl] a LEFT JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqId] = b.[PurchReqId] Where TransStatusPurch = 'Yes' And b.TransType = '" + TransType + "' And ";
                Query += Parent.getPurchReqId() + ";";
                //Query += Parent.getSeqNo() + ");";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select a.PurchReqID, SeqNo, b.TransType, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi From [dbo].[PurchRequisition_Dtl] a LEFT JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqId] = b.[PurchReqId] ";
                Query += "Where (a.PurchReqID Like '%" + txtSearch.Text + "%' or RefSeqNo Like '%" + txtSearch.Text + "%' or FullItemID Like '%" + txtSearch.Text + "%' or ItemName Like '%" + txtSearch.Text + "%' or DeliveryMethod Like '%" + txtSearch.Text + "%' or Qty Like '%" + txtSearch.Text + "%' or Unit Like '%" + txtSearch.Text + "%' or Deskripsi Like '%" + txtSearch.Text + "%') And TransStatusPurch = 'Yes' And b.TransType = '" + TransType + "' And ";
                Query += Parent.getPurchReqId() + ";";
                //Query += Parent.getSeqNo() + ");";
            }
            else
            {
                Query = "Select a.PurchReqID, SeqNo, b.TransType, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi From [dbo].[PurchRequisition_Dtl] a LEFT JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqId] = b.[PurchReqId] ";
                Query += "Where (" + crit + " Like '%" + txtSearch.Text + "%') And TransStatusPurch = 'Yes' And b.TransType = '" + TransType + "' And ";
                Query += Parent.getPurchReqId() + ";";
                //Query += Parent.getSeqNo() + ");";
            }
            //}
            //else
            //{
                //if (crit == null)
                //{
                //    Query = "Select a.PurchReqID, SeqNo, b.TransType, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi From [dbo].[PurchRequisition_Dtl] a JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqId] = b.[PurchReqId] Where TransStatusPurch = 'Yes' And b.TransType = '" + TransType + "' And ";
                //    Query += Parent.getPurchReqId() + ";";
                //}
                //else if (crit.Equals("All"))
                //{
                //    Query = "Select a.PurchReqID, SeqNo, b.TransType, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi From [dbo].[PurchRequisition_Dtl] a JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqId] = b.[PurchReqId] ";
                //    Query += "Where (a.PurchReqID Like '%" + txtSearch.Text + "%' or RefSeqNo Like '%" + txtSearch.Text + "%' or FullItemID Like '%" + txtSearch.Text + "%' or ItemName Like '%" + txtSearch.Text + "%' or DeliveryMethod Like '%" + txtSearch.Text + "%' or Qty Like '%" + txtSearch.Text + "%' or Unit Like '%" + txtSearch.Text + "%' or Deskripsi Like '%" + txtSearch.Text + "%') And TransStatusPurch = 'Yes' And b.TransType = '" + TransType + "' And ";
                //    Query += Parent.getPurchReqId() + ";";
                //}
                //else
                //{
                //    Query = "Select a.PurchReqID, SeqNo, b.TransType, FullItemID, ItemName, DeliveryMethod, Qty, Unit, Deskripsi From [dbo].[PurchRequisition_Dtl] a JOIN [dbo].[PurchRequisitionH] b ON a.[PurchReqId] = b.[PurchReqId] ";
                //    Query += "Where (" + crit + " Like '%" + txtSearch.Text + "%') And TransStatusPurch = 'Yes' And b.TransType = '" + TransType + "' And ";
                //    Query += Parent.getPurchReqId() + ";";
                //}
            //}

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            
            Da.Fill(Dt);

            if (dgvPR.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvPR.Columns.Add(chk);
                chk.HeaderText = "";
                chk.Name = "chk";
            }

            dgvPR.AutoGenerateColumns = true;
            dgvPR.DataSource = Dt;
            dgvPR.Refresh();

            dgvPR.ReadOnly = false;
            dgvPR.Columns["PurchReqId"].ReadOnly = true;
            dgvPR.Columns["SeqNo"].ReadOnly = true;
            dgvPR.Columns["TransType"].ReadOnly = true;
            dgvPR.Columns["FullItemID"].ReadOnly = true;
            dgvPR.Columns["ItemName"].ReadOnly = true;
            dgvPR.Columns["DeliveryMethod"].ReadOnly = true;
            dgvPR.Columns["Qty"].ReadOnly = true;
            dgvPR.Columns["Unit"].ReadOnly = true;
            dgvPR.Columns["Deskripsi"].ReadOnly = true;

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

        private void AddPR_Load(object sender, EventArgs e)
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
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PurchRequisition_Dtl'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
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
            //List<string> PurchReqId = new List<string>();
            //List<string> PurchReqSeqNo = new List<string>();

            //for (int i = 0; i <= dgvPR.RowCount - 1; i++)
            //{
            //    Boolean Check = Convert.ToBoolean(dgvPR.Rows[i].Cells["chk"].Value);
            //    if (Check == true)
            //    {
            //        PurchReqId.Add(dgvPR.Rows[i].Cells["PurchReqId"].Value == null ? "" : dgvPR.Rows[i].Cells["PurchReqID"].Value.ToString());
            //        PurchReqSeqNo.Add(dgvPR.Rows[i].Cells["SeqNo"].Value == null ? "" : dgvPR.Rows[i].Cells["SeqNo"].Value.ToString());
            //    }
            //}
            //Parent.AddDataGridFromDetail(PurchReqId,PurchReqSeqNo);

            //this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvPR.RowCount - 1; i++)
            {
                dgvPR.Rows[i].Cells["chk"].Value = Check;
            }
        }
    }
}
