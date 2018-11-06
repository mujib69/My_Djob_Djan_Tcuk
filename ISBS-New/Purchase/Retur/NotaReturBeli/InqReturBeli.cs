using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.NotaReturBeli
{
    public partial class InqReturBeli : MetroFramework.Forms.MetroForm
    {
        public InqReturBeli()
        {
            InitializeComponent();
        }

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query, crit = null;
        int Total, Limit1, Limit2, Page1, Page2, Index;
        public static int dataShow;

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10 * 1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (timerRefresh == null)
            {

            }
            else
            {
                RefreshGrid();
            }
        }

        private void InquiryRP_Load(object sender, EventArgs e)
        {
            addCmbCriteria();
            ModeLoad();
        }

        private void addCmbCriteria()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName, DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'NotaReturBeliH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[1]);
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
            Query = "Select CmbValue From [Setting].[CmbBox]";

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
            Conn = ConnectionString.GetConnection();
            Query = "SELECT * FROM (SELECT ROW_NUMBER()OVER(ORDER BY NRB.NRBId DESC) AS [No], NRB.NRBId AS [Nota Retur Beli Id], NRB.NRBDate AS [Tgl Nota], NRB.NRBMode AS [Mode Nota], NRB.VendId AS [Vendor Id], VT.VendName AS [Nama Vendor], NRB.GoodsReceivedId AS [Goods Received Id], NRB.SiteId AS [Site Id], NRB.SiteName AS [Site Name], TST.Deskripsi AS [Deskripsi], NRB.CreatedDate AS [Created Date], NRB.CreatedBy AS [Created By] FROM NotaReturBeliH NRB LEFT JOIN TransStatusTable TST ON NRB.TransStatusId = TST.StatusCode LEFT JOIN VendTable VT ON VT.VendId = NRB.VendId WHERE TST.TransCode = 'NotaReturBeli' ";

            if (crit == null)
            {
                Query += ") A ";
            }
            else if (crit.Equals("All"))
            {
                Query += "AND NRB.NRBId LIKE '%" + txtSearch.Text + "%' OR NRB.NRBMode LIKE '%" + txtSearch.Text + "%' OR NRB.VendName LIKE '%" + txtSearch.Text + "%' OR NRB.GoodsReceivedId LIKE '%" + txtSearch.Text + "%' OR NRB.CreatedBy LIKE '%" + txtSearch.Text + "%') A ";
            }  
            else if (crit.Equals("Nota Retur Beli ID"))
            {
                Query += "AND NRB.NRBId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Nota Retur Beli Mode"))
            {
                Query += "AND NRB.NRBMode LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Nama Vendor"))
            {
                Query += "AND NRB.VendName LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Goods Received ID"))
            {
                Query += "AND NRB.GoodsReceivedId LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Created By"))
            {
                Query += "AND NRB.CreatedBy LIKE '%" + txtSearch.Text + "%') A ";
            }
            else if (crit.Equals("Tgl Nota"))
            {
                Query += "AND (CONVERT(VARCHAR(10),NRB.NRBDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRB.NRBDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            else if (crit.Equals("Tgl Buat"))
            {
                Query += "AND (CONVERT(VARCHAR(10),NRB.CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRB.CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) A ";
            }
            Query += "WHERE A.[No] BETWEEN " + Limit1 + " AND " + Limit2;

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

            dgvNRB.AutoGenerateColumns = true;
            dgvNRB.DataSource = Dt;
            dgvNRB.Refresh();
            dgvNRB.AutoResizeColumns();
            dgvNRB.Columns["Tgl Nota"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvNRB.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            Conn.Close();

            if (!dgvNRB.Columns.Contains("Preview"))
                dgvNRB.Columns.Add(buttonpreview);
            if (!dgvNRB.Columns.Contains("Send Email"))
                dgvNRB.Columns.Add(buttonSend);

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            
            if (crit == null)
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "NRBId LIKE '%" + txtSearch.Text + "%' OR NRBMode LIKE '%" + txtSearch.Text + "%' OR VendName LIKE '%" + txtSearch.Text + "%' OR GoodsReceivedId LIKE '%" + txtSearch.Text + "%' OR CreatedBy LIKE '%" + txtSearch.Text + "%' ";
            }
            else if (crit.Equals("Tgl Nota"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(CONVERT(VARCHAR(10),NRBDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),NRBDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ;";
            }
            else if (crit.Equals("Tgl Buat"))
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ;";
            }
            else
            {
                Query = "SELECT COUNT(NRBId) FROM NotaReturBeliH WHERE ";
                if (crit.Equals("Nota Retur Beli ID"))
                    Query += "NRBId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Nota Retur Beli Mode"))
                    Query += "NRBMode Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Nama Vendor"))
                    Query += "VendName Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Goods Received ID"))
                    Query += "GoodsReceivedId Like '%" + txtSearch.Text + "%' ";
                else if (crit.Equals("Created By"))
                    Query += "CreatedBy Like '%" + txtSearch.Text + "%' ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            //if (dataShow != 0)
            //    Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
            //else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
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
            RefreshGrid();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(Login.Delete) > 0)
            {
                try
                {
                    if (dgvNRB.RowCount > 0)
                    {
                        Index = dgvNRB.CurrentRow.Index;
                        string NRBId = dgvNRB.Rows[Index].Cells["Nota Retur Beli Id"].Value == null ? "" : dgvNRB.Rows[Index].Cells["Nota Retur Beli Id"].Value.ToString();

                        DialogResult dr = MessageBox.Show("Nota Retur Beli Id = " + NRBId + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            string Check = "";

                            Query = "SELECT TransStatusId FROM NotaReturBeliH WHERE NRBId ='" + NRBId + "';";
                            Cmd = new SqlCommand(Query, Conn);
                            Check = Cmd.ExecuteScalar().ToString();
                            if (Check != "01")
                            {
                                MessageBox.Show("NRB ID = " + NRBId + ".\n" + "Tidak bisa dihapus karena sudah diposting.");
                                Conn.Close();
                                return;
                            }

                            //Update Qty
                            List<string> GoodsReceivedSeqNo = new List<string>();
                            List<string> GoodsReceivedID = new List<string>();
                            List<decimal> Qty = new List<decimal>();
                            decimal RemainingQty, QtyNew = 0;
                            Query = "SELECT GoodsReceived_SeqNo, UoM_Qty, GoodsReceivedId FROM NotaReturBeli_Dtl WHERE NRBId='" + NRBId + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                GoodsReceivedSeqNo.Add(Dr["GoodsReceived_SeqNo"].ToString());
                                GoodsReceivedID.Add(Dr["GoodsReceivedId"].ToString());
                                Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                            }
                            Dr.Close();

                            for (int i = 0; i < GoodsReceivedSeqNo.Count; i++)
                            {
                                Query = "SELECT Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" + GoodsReceivedID[i] + "' AND GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo[i] + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                                QtyNew = RemainingQty + Qty[i];

                                Query = "Update GoodsReceivedD set ";
                                Query += "Remaining_Qty='" + QtyNew + "' ";
                                Query += "where GoodsReceivedId='" + GoodsReceivedID[i] + "' And GoodsReceivedSeqNo='" + GoodsReceivedSeqNo[i] + "'";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                                Query = "";
                            }

                            //delete header
                            Query = "Delete from NotaReturBeliH where NRBId='" + NRBId + "';";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //delete item
                            Query = "Delete from NotaReturBeli_Dtl where NRBId ='" + NRBId + "'; ";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("NRB ID = " + NRBId.ToUpper() + "\n" + "Data berhasil dihapus.");

                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & Login.Username & "');";
                            Index = 0;
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(Login.New) > 0)
            {
                ReturBeliHeader header = new ReturBeliHeader();
                header.SetMode("New", "");
                header.setParent(this);
                header.Show();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectNRB();
        }

        private void SelectNRB()
        {
            ReturBeliHeader f = new ReturBeliHeader();

            if (f.PermissionAccess(Login.View) > 0)
            {
                string NRBID = dgvNRB.CurrentRow.Cells["Nota Retur Beli Id"].Value.ToString();

                f.SetMode("BeforeEdit", NRBID);
                f.setParent(this);
                f.Show();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end
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

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }

        private void dgvNRB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                SelectNRB();
            }
        }
    }
}
