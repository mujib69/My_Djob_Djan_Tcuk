using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.CashAndBank.ReceiptVoucher
{
    public partial class InquiryReceiptVoucher : MetroFramework.Forms.MetroForm
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
        private string StatusCode = String.Empty;

        public InquiryReceiptVoucher()
        {
            InitializeComponent();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void InquiryReceiptVouhcer_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            cmbCriteria.SelectedIndex = 0;

            RefreshGrid();
        }

        private void addCmbCrit()
        {
            try
            {
                cmbCriteria.Items.Add("All");
                Conn = ConnectionString.GetConnection();
                Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'ReceiptVoucher_H'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    cmbCriteria.Items.Add(Dr[0]);
                }
                Conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
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

        private void InquiryReceiptVouhcer_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
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

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
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
                RefreshGrid();

            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            if (StatusCode == String.Empty)
            {
                StatusCode = "'01', '03'"; Limit1 = 1; Limit2 = dataShow;
            }

            int mflag;

            Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY R.CreatedDate DESC) No, R.RV_Date RVDate, R.RV_No AS RVNo, R.Cust_Id AS Customer, R.Giro_No AS NoGiro, ";
            Query += "R.Receipt_No AS KetTransferAtauTunai, R.Bank_Id AS Bank, R.Payment_DueDate AS JthTempo, R.Tgl_Cair AS TglCair, R.Total_Payment AS Nominal, T.Deskripsi AS StatusName, ";
            Query += "R.CreatedDate, R.CreatedBy, R.UpdatedDate, R.UpdatedBy ";
            Query += "FROM ReceiptVoucher_H R INNER JOIN TransStatusTable T on R.StatusCode = T.StatusCode ";
            Query += "AND T.TransCode='ReceiptVoucher' AND R.StatusCode IN (" + StatusCode + ") ";
            mflag = 1;

            if (crit == null)
            {
                Query += ") a ";
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += "AND (R.RV_No LIKE @search OR R.Cust_Id LIKE @search OR R.Giro_No LIKE @search ";
                Query += " OR R.Receipt_No LIKE @search OR R.Bank_Id LIKE @search OR R.Total_Payment LIKE @search OR R.CreatedBy LIKE @search OR R.UpdatedBy LIKE @search)) a ";
           }
            else if (crit.Equals("RVNo"))
            {
                Query += "AND R.RV_No LIKE @search) a ";
            }
            else if (crit.Equals("Customer"))
            {
                Query += "AND R.Cust_Id LIKE @search) a ";
            }
            else if (crit.Equals("NoGiro"))
            {
                Query += "AND R.Giro_No LIKE @search) a ";
            }
            else if (crit.Equals("KetTransferAtauTunai"))
            {
                Query += "AND R.Receipt_No LIKE @search) a ";
            }
            else if (crit.Equals("Bank"))
            {
                Query += "AND R.Bank_Id LIKE @search) a ";
            }
            else if (crit.Equals("Nominal"))
            {
                Query += "AND R.Total_Payment LIKE @search) a ";
            }
            else if (crit.Equals("StatusName"))
            {
                Query += "AND T.Deskripsi LIKE @search) a ";
           }
            else if (crit.Equals("CreatedBy"))
            {
                Query += "AND R.CreatedBy LIKE @search) a ";
            }
            else if (crit.Equals("UpdatedBy"))
            {
                Query += "AND R.UpdatedBy LIKE @search) a ";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),R.CreatedDate,120) >= @from AND CONVERT(VARCHAR(10), R.CreatedDate,120) <= @to)) a ";
                mflag = 2;
            }
            else if (crit.Equals("UpdatedDate"))
            {
                 Query += "AND (CONVERT(VARCHAR(10),R.UpdatedDate,120) >= @from AND CONVERT(VARCHAR(10), R.UpdatedDate,120) <= @to)) a ";
                 mflag = 2;
            }
            else if (crit.Equals("RVDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),R.RV_Date,120) >= @from AND CONVERT(VARCHAR(10), R.RV_Date,120) <= @to)) a ";
                mflag = 2;
            }
            else if (crit.Equals("JthTempo"))
            {
                Query += "AND (CONVERT(VARCHAR(10),R.Payment_DueDate,120) >= @from AND CONVERT(VARCHAR(10), R.Payment_DueDate,120) <= @to)) a ";
                mflag = 2;
            }
            else if (crit.Equals("TglCair"))
            {
                Query += "AND (CONVERT(VARCHAR(10),R.Tgl_Cair,120) >= @from AND CONVERT(VARCHAR(10), R.Tgl_Cair,120) <= @to)) a ";
                mflag = 2;
            }

            Query += "WHERE No Between @limit1 AND @limit2 ";

            Query += " ORDER BY a.RVNo DESC";

            Da = new SqlDataAdapter(Query, Conn);

            Da.SelectCommand.Parameters.AddWithValue("@limit1", Limit1);
            Da.SelectCommand.Parameters.AddWithValue("@limit2", Limit2);
            switch (mflag)
            {
                case 1:
                    Da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    break;
                case 2:
                    Da.SelectCommand.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd"));
                    Da.SelectCommand.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd"));
                    break;
            }
            
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvReceiptVoucher.AutoGenerateColumns = true;
            dgvReceiptVoucher.DataSource = Dt;
            dgvReceiptVoucher.Refresh();
            dgvReceiptVoucher.AutoResizeColumns();
            dgvReceiptVoucher.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvReceiptVoucher.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvReceiptVoucher.Columns["TglCair"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvReceiptVoucher.Columns["JthTempo"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvReceiptVoucher.Columns["RVDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvReceiptVoucher.Columns["Nominal"].DefaultCellStyle.Format = "N2";

            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            Query = "SELECT COUNT(RV_No) FROM ReceiptVoucher_H WHERE StatusCode IN (" + StatusCode + ")";
            mflag = 1;

            if (crit == null)
            {
                Query += "";
                mflag = 0;
            }
            else if (crit.Equals("All"))
            {
                Query += "AND (RV_No LIKE @search OR Cust_Id LIKE @search OR Giro_No LIKE @search ";
                Query += " OR Receipt_No LIKE @search OR Bank_Id LIKE @search OR Total_Payment LIKE @search OR CreatedBy LIKE @search OR UpdatedBy LIKE @search) ";
            }
            else if (crit.Equals("RVNo"))
            {
                Query += "AND RV_No LIKE @search ";
            }
            else if (crit.Equals("Customer"))
            {
                Query += "AND Cust_Id LIKE @search ";
            }
            else if (crit.Equals("NoGiro"))
            {
                Query += "AND Giro_No LIKE @search ";
            }
            else if (crit.Equals("KetTransferAtauTunai"))
            {
                Query += "AND Receipt_No LIKE @search ";
            }
            else if (crit.Equals("Bank"))
            {
                Query += "AND Bank_Id LIKE @search ";
            }
            else if (crit.Equals("Nominal"))
            {
                Query += "AND Total_Payment LIKE @search ";
            }
            else if (crit.Equals("Nominal"))
            {
                Query += "AND T.Deskripsi LIKE @search ";
            }
            else if (crit.Equals("CreatedBy"))
            {
                Query += "AND CreatedBy LIKE @search ";
            }
            else if (crit.Equals("UpdatedBy"))
            {
                Query += "AND UpdatedBy LIKE @search ";
            }
            else if (crit.Equals("CreatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),CreatedDate,120) >= @from AND CONVERT(VARCHAR(10), CreatedDate,120) <= @to) ";
                mflag = 2;
            }
            else if (crit.Equals("UpdatedDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),UpdatedDate,120) >= @from AND CONVERT(VARCHAR(10), UpdatedDate,120) <= @to) ";
                mflag = 2;
            }
            else if (crit.Equals("RVDate"))
            {
                Query += "AND (CONVERT(VARCHAR(10),RV_Date,120) >= @from AND CONVERT(VARCHAR(10), RV_Date,120) <= @to) ";
                mflag = 2;
            }
            else if (crit.Equals("JthTempo"))
            {
               Query += "AND (CONVERT(VARCHAR(10),Payment_DueDate,120) >= @from AND CONVERT(VARCHAR(10), Payment_DueDate,120) <= @to) ";
               mflag = 2;
            }
            else if (crit.Equals("TglCair"))
            {
                Query += "AND (CONVERT(VARCHAR(10),Tgl_Cair,120) >= @from AND CONVERT(VARCHAR(10), Tgl_Cair,120) <= @to) ";
                mflag = 2;
            }

            Cmd = new SqlCommand(Query, Conn);
            switch (mflag)
            {
                case 1:
                    Cmd.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                    break;
                case 2:
                    Cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date.ToString("yyyy-MM-dd"));
                    Cmd.Parameters.AddWithValue("@to", dtTo.Value.Date.ToString("yyyy-MM-dd"));
                    break;
            }
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void btnOnProgress_Click(object sender, EventArgs e)
        {
            StatusCode = "'01', '03'";
            RefreshGrid();
            btnOnProgress.BackColor = Color.DeepSkyBlue;
            btnOnProgress.ForeColor = Color.White;
            btnCompleted.BackColor = Color.LightGray;
            btnCompleted.ForeColor = Color.Black;
        }

        private void btnCompleted_Click(object sender, EventArgs e)
        {
            StatusCode = "'02'";
            RefreshGrid();
            btnOnProgress.BackColor = Color.LightGray;
            btnOnProgress.ForeColor = Color.Black;
            btnCompleted.BackColor = Color.DeepSkyBlue;
            btnCompleted.ForeColor = Color.White;
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("Tgl") || cmbCriteria.Text.Contains("JthTempo"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }


            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("Tgl") || cmbCriteria.Text.Contains("JthTempo"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
            {
                crit = "All";
            }
            else
            {
                crit = cmbCriteria.SelectedItem.ToString();
            }

            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            RefreshGrid();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                HeaderReceiptVoucher H = new HeaderReceiptVoucher();
                H.SetMode("New", "");
                H.SetParent(this);
                H.Show();  
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectData();
        }

        private void dgvReceiptVoucher_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectData();
        }

        private void SelectData()
        {
            HeaderReceiptVoucher header = new HeaderReceiptVoucher();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                header.SetParent(this);
                header.SetMode("BeforeEdit", dgvReceiptVoucher.CurrentRow.Cells["RVNo"].Value.ToString());
                header.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 10 jul 2018
            //description : check permission access
            //if (this.PermissionAccess(ControlMgr.Delete) > 0)
            //{
            //    try
            //    {
            //        if (dgvReceiptVoucher.RowCount > 0)
            //        {
            //            if (TransStatus == "'02'")
            //            {
            //                MessageBox.Show("Data tidak dapat dihapus karena sudah diapprove.");
            //                return;
            //            }
            //            else
            //            {
            //                Index = dgvPLB.CurrentRow.Index;
            //                string PriceListNo = dgvPLB.Rows[Index].Cells["PriceListNo"].Value == null ? "" : dgvPLB.Rows[Index].Cells["PriceListNo"].Value.ToString();

            //                DialogResult dr = MessageBox.Show("PriceListNo = " + PriceListNo + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
            //                if (dr == DialogResult.Yes)
            //                {
            //                    string Check = "";
            //                    Conn = ConnectionString.GetConnection();

            //                    Query = "Select StatusCode from [dbo].[PurchPriceListH] where [PriceListNo]='" + dgvPLB.CurrentRow.Cells["PriceListNo"].Value.ToString() + "';";
            //                    Cmd = new SqlCommand(Query, Conn);
            //                    Check = Cmd.ExecuteScalar().ToString();
            //                    if (Check != "01")
            //                    {
            //                        MessageBox.Show("PriceListNo = " + dgvPLB.CurrentRow.Cells["PriceListNo"].Value.ToString().ToUpper() + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
            //                        Conn.Close();
            //                        return;
            //                    }

            //                    Conn = ConnectionString.GetConnection();

            //                    //delete detail
            //                    Query = "Delete from [dbo].[PurchPriceListDtl] where PriceListNo ='" + PriceListNo + "';";


            //                    //delete header
            //                    Query += "Delete from [dbo].[PurchPriceListH] where PriceListNo='" + PriceListNo + "';";


            //                    Cmd = new SqlCommand(Query, Conn, Trans);
            //                    Cmd.ExecuteNonQuery();

            //                    MessageBox.Show("PriceListNo = " + PriceListNo.ToUpper() + "\n" + "Data berhasil dihapus.");

            //                    Index = 0;
            //                    Conn.Close();
            //                    RefreshGrid();

            //                }
            //            }
            //        }
            //        else
            //        {
            //            MessageBox.Show("Silahkan pilih data untuk dihapus");
            //            return;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            //end
        }
    }
}
