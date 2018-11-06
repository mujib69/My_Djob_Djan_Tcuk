using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.GoodReceiptNT
{
    public partial class InquiryGoodsReceiptNT : MetroFramework.Forms.MetroForm
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
        public static int dataShow;

        List<HeaderGoodsReceiptNT> ListHeaderGoodsReceipt = new List<HeaderGoodsReceiptNT>();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public InquiryGoodsReceiptNT()
        {
            InitializeComponent();
        }

        private void InquiryGoodsReceiptNT_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            //lblForm.Location = new Point(16, 11);
            //setTimer();
            //gvheader();
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'GoodsReceiptNTH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
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

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY [GoodsReceivedId] desc) [No], [GoodsReceivedId] 'GR ID',  [GoodsReceivedDate] 'GR Date', b.Deskripsi 'Status', [TransferNo] 'RO ID', [SJNumber] 'Delivery Number', [VendId], [VendorName], [Timbang1Id] 'Weight 1 ID',  [Timbang1Weight] 'Weight 1', [Timbang2Id] 'Weight 2 ID', [Timbang2Weight] 'Weight 2', [CreatedDate], [CreatedBy] From [dbo].[GoodsReceivedNTH] as a left join TransStatusTable as b on b.StatusCode = a.GoodsReceivedStatus where b.TransCode = 'GR' and a.[GoodsReceivedStatus] != '04'";

            if (crit == null)
                Query += ") a ";
            else if (crit.Equals("All"))
            {
                Query += "and [GoodsReceivedId] like '%" + txtSearch.Text + "%' or [GoodsReceivedStatus] like '%" + txtSearch.Text + "%' or [TransferNo] like '%" + txtSearch.Text + "%' or [SJNumber] like '%" + txtSearch.Text + "%' or [Timbang1Id] like '%" + txtSearch.Text + "%' or [Timbang2Id] like '%" + txtSearch.Text + "%' or [VendId] like '%" + txtSearch.Text + "%' or [VendorName] like '%" + txtSearch.Text + "%') a ";
            }
            else if (crit.Equals("TransferNo") || crit.Equals("CreatedDate") || crit.Equals("ReceiptOrderStatus"))
            {
                //Query += "where (CONVERT(VARCHAR(10),TransferDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),TransferDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (" + crit + " like '%" + txtSearch.Text + "%')) a ";
            }
            Query += "Where a.No Between " + Limit1 + " and " + Limit2 + " ;";

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            DataGridViewButtonColumn buttonpreview = new DataGridViewButtonColumn();
            buttonpreview.Name = "Preview";
            buttonpreview.HeaderText = "Preview";
            buttonpreview.Text = "Preview";
            buttonpreview.UseColumnTextForButtonValue = true;
            //Dt.Columns.Add(new DataColumn("colStatus", typeof(System.Windows.Forms.Button)));

            DataGridViewButtonColumn buttonSend = new DataGridViewButtonColumn();
            buttonSend.Name = "Send Email";
            buttonSend.HeaderText = "Send Email";
            buttonSend.Text = "Send Email";
            buttonSend.UseColumnTextForButtonValue = true;

            dgvPR.AutoGenerateColumns = true;
            dgvPR.DataSource = Dt;
            dgvPR.Refresh();
            dgvPR.AutoResizeColumns();
            dgvPR.Columns["GR Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            if (!dgvPR.Columns.Contains("Preview"))
                dgvPR.Columns.Add(buttonpreview);
            if (!dgvPR.Columns.Contains("Send Email"))
                dgvPR.Columns.Add(buttonSend);

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            Query = "Select Count([GoodsReceivedId]) From ( Select [GoodsReceivedId] From [dbo].[GoodsReceivedNTH] ";
            if (crit == null)
                Query += "Where [GoodsReceivedStatus] != '04') a;";
            else if (crit.Equals("All"))
            {
                Query += "where [GoodsReceivedId] like '%" + txtSearch.Text + "%' or [GoodsReceivedStatus] like '%" + txtSearch.Text + "%' or [TransferNo] like '%" + txtSearch.Text + "%' or [SJNumber] like '%" + txtSearch.Text + "%' or [Timbang1Id] like '%" + txtSearch.Text + "%' or [Timbang2Id] like '%" + txtSearch.Text + "%' and [GoodsReceivedStatus] != '04') a ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (dataShow != 0)
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
                crit = "All";
            else
                crit = cmbCriteria.SelectedItem.ToString();

            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.New) > 0)
            {
                Inventory.GoodReceiptNT.HeaderGoodsReceiptNT f = new Inventory.GoodReceiptNT.HeaderGoodsReceiptNT();
                f.SetMode("New", "");
                f.SetParent(this);
                f.Show();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end              

            //if (Login.UserGroup != "PurchaseManager")
            //{
            //    Inventory.GoodReceiptNT.HeaderGoodsReceiptNT f = new Inventory.GoodReceiptNT.HeaderGoodsReceiptNT();
            //    f.SetMode("New", "");
            //    f.SetParent(this);
            //    f.Show();
            //    RefreshGrid();
            //}
            //else
            //{
            //    MetroFramework.MetroMessageBox.Show(this,"User Group : PurchaseManager \nDo not have permission to Create GR!.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectPR();
        }

        #region SelectPR
        private void SelectPR()
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            HeaderGoodsReceiptNT f = new HeaderGoodsReceiptNT();                   
            if (f.PermissionAccess(Login.View) > 0)
            {
                if (dgvPR.RowCount > 0)
                {
                    f.SetMode("BeforeEdit", dgvPR.CurrentRow.Cells["GR ID"].Value.ToString());
                    f.SetParent(this);
                    f.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end             
        }
        #endregion

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
                SelectPR();
        }

        private void dgvPR_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvPR.Columns["Weight 1"].Index ||
                e.ColumnIndex == dgvPR.Columns["Weight 2"].Index)
            {
                if (e.Value == null || e.Value.ToString() == "")
                {
                    e.Value = "0.0000";
                    return;
                }
                double d = double.Parse(e.Value.ToString());
                dgvPR.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N4");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.Delete) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Query = "select [GoodsReceivedStatus] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + dgvPR.CurrentRow.Cells["GR ID"].Value.ToString() + "'";
                Cmd = new SqlCommand(Query, Conn);
                string stats = Cmd.ExecuteScalar().ToString();

                if (stats == "03")
                {
                    MetroFramework.MetroMessageBox.Show(this, dgvPR.CurrentRow.Cells["GR ID"].Value.ToString() + " is completed. Cannot delete data!", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    string msg = "";
                    int count = 0;
                    foreach (DataGridViewRow r in dgvPR.SelectedRows)
                    {
                        if (count >= 1)
                            msg += ", ";
                        msg += dgvPR.Rows[r.Index].Cells["GR ID"].Value.ToString();
                        count++;
                    }
                    if (msg == String.Empty)
                    {
                        MetroFramework.MetroMessageBox.Show(this, "Select Row(s)!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        DialogResult result = MetroFramework.MetroMessageBox.Show(this, "Are you sure to delete " + msg + "?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            foreach (DataGridViewRow r in dgvPR.SelectedRows)
                            {
                                Query = "update [dbo].[GoodsReceivedNTH] set [GoodsReceivedStatus] = '04' where [GoodsReceivedId] = '" + dgvPR.Rows[r.Index].Cells["GR ID"].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Dr = Cmd.ExecuteReader();
                            }
                        }
                        Conn.Close();
                        RefreshGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end             
        }

        private void dgvPR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvPR.Rows[dgvPR.CurrentRow.Index].Selected = true;
        }

        private void dgvPR_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {
                if (dgvPR.Columns[e.ColumnIndex].Name.ToString() == "VendId" || dgvPR.Columns[e.ColumnIndex].Name.ToString() == "VendorName")
                {
                    String TotalVendor = dgvPR.Rows[e.RowIndex].Cells["VendId"].Value.ToString();
                    String[] VendorSatuan = TotalVendor.Split(';');

                    PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();
                    for (int i = 0; i < VendorSatuan.Count(); i++)
                    {
                        PopUp.Vendor.Vendor PopUpVendor1 = new PopUp.Vendor.Vendor();

                        PopUpVendor1.GetData(VendorSatuan[i].ToString());
                        PopUpVendor1.Y += 100 * i;
                        PopUpVendor1.Show();
                    }

                }
                if (dgvPR.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPR.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                {
                    PopUp.Stock.Stock PopUpStock = new PopUp.Stock.Stock();
                    PopUpStock.GetData(dgvPR.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    PopUpStock.Show();

                }
            }
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

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }
    }
}
