using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.TaskList.Purchase.GoodsReceipt
{
    public partial class TaskListGR : MetroFramework.Forms.MetroForm
    {
        private string vTransType;
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

        List<ISBS_New.Purchase.GoodsReceipt.GRHeaderV2> ListHeaderGoodsReceipt = new List<ISBS_New.Purchase.GoodsReceipt.GRHeaderV2>();

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public TaskListGR(string _vTransType)
        {
            InitializeComponent();
            vTransType = _vTransType;
        }

        private void TaskListGR_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select FieldName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'GoodsReceivedH'";

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

            //Hendry Revisi : 11 April jika panggil dari Purchase, Sales, dan Inventory 
            //Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY [CreatedDate] desc) [No], [GoodsReceivedId] 'GR ID',  [GoodsReceivedDate] 'GR Date', b.Deskripsi 'Status', [SJNumber] 'Delivery Number', a.RefTransType as [Ref Type],a.RefTransDate as [Ref Trans Date],a.RefTransID as [Ref Trans], [VendId], [VendorName],  [Timbang1Weight] 'Weight 1', [Timbang2Weight] 'Weight 2', [CreatedDate], [CreatedBy] From [dbo].[GoodsReceivedH] as a left join TransStatusTable as b on b.StatusCode = a.GoodsReceivedStatus where b.TransCode = 'GR' and a.[GoodsReceivedStatus] != '04'";
            Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CreatedDate desc) AS 'No',GoodsReceivedId AS 'GR No',GoodsReceivedDate AS 'GR Date',";
            Query += "b.Deskripsi AS 'Status',SJNumber AS 'Delivery Number',a.RefTransID AS 'Reference',a.RefTransDate AS 'Reference Date',";
            Query += "VendId AS AccountNum,VendorName AS 'Account Name',Timbang1Weight AS 'Weight 1',Timbang2Weight AS 'Weight 2',";
            Query += "CreatedBy AS 'Created By',CreatedDate AS 'Created Date',UpdatedBy AS 'Updated By',UpdatedDate AS 'Updated Date' ";
            Query += "FROM GoodsReceivedH AS a LEFT JOIN TransStatusTable AS b ON b.StatusCode = a.GoodsReceivedStatus ";
            Query += "WHERE b.TransCode = 'GR' AND a.[GoodsReceivedStatus] = '05' ";
            Query += "AND a.RefTransType='" + vTransType + "' ";

            if (crit == null)
                Query += ") a ";
            else if (crit.Equals("All"))
            {
                Query += "and ([GoodsReceivedId] like @txtSearch or [GoodsReceivedStatus] like @txtSearch or [SJNumber] like @txtSearch or [VendId] like @txtSearch or [VendorName] like @txtSearch) ) a ";
            }
            else if (crit.Contains("Date"))
            {
                Query += "and (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ([GoodsReceivedStatus] = '05')) a ";
            }
            else if (crit.Equals("GoodsReceivedStatus"))
            {
                Query += " AND b.Deskripsi like @txtSearch ) a ";
            }
            else
            {
                Query += "and " + crit + " like @txtSearch ) a ";
            }
            Query += "Where a.No Between " + Limit1 + " and " + Limit2 + " ;";

            Da = new SqlDataAdapter(Query, Conn);
            //created by Thaddaeus,6JUNE2018,begin
            Da.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@txtSearch",
                Value = "%" + txtSearch.Text + "%",
                SqlDbType = SqlDbType.NVarChar,
                Size = 50
            });
            //end=================
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
            Query = "Select Count([GoodsReceivedId]) From ( Select [GoodsReceivedId] From [dbo].[GoodsReceivedH] a LEFT JOIN TransStatusTable b ON a.[GoodsReceivedStatus]=b.[StatusCode]  ";
            Query += "WHERE RefTransType='" + vTransType + "' AND b.TransCode = 'GR' ";
            if (crit == null)
                Query += "AND [GoodsReceivedStatus] = '05') a;";
            else if (crit.Equals("All"))
            {
                Query += "AND ([GoodsReceivedId] like @txtSearch or [GoodsReceivedStatus] like @txtSearch or [SJNumber] like @txtSearch) and [GoodsReceivedStatus] = '05') a ";
            }
            else if (crit.Contains("Date"))
            {
                Query += "and (CONVERT(VARCHAR(10)," + crit + ",120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10)," + crit + ",120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ([GoodsReceivedStatus] = '05')) a ";
            }
            else if (crit.Equals("GoodsReceivedStatus"))
            {
                Query += " AND b.[Deskripsi] like @txtSearch and [GoodsReceivedStatus] = '05') a ";
            }
            else
            {
                Query += "and (" + crit + " like @txtSearch ) and [GoodsReceivedStatus] = '05') a ";
            }

            Cmd = new SqlCommand(Query, Conn);
            //created by Thaddaeus, 6JUNE2018, begin
            Cmd.Parameters.AddWithValue("@txtSearch", "%" + txtSearch.Text + "%");
            //end=====================
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            if (dataShow != 0)
            {
                Page2 = (int)Math.Ceiling((decimal)Total / dataShow);
                dataShow = 0;
            }
            else
                Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            if (Convert.ToInt32(txtPage.Text) > Page2)
            {
                txtPage.Text = Page2.ToString();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmbCriteria.SelectedIndex == -1)
                crit = "All";
            else
                crit = cmbCriteria.SelectedItem.ToString();
            btnMPrev_Click(sender, e);
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            cmbCriteria.SelectedIndex = 0;
            ModeLoad();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                
                ISBS_New.Purchase.GoodsReceipt.GRHeaderV2 f = new ISBS_New.Purchase.GoodsReceipt.GRHeaderV2(vTransType);
             
                f.SetMode("New", "");
                f.ParentRefreshGrid4(this);
                f.Show();
                RefreshGrid();
              
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
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

        private void SelectPR()
        {

            ISBS_New.Purchase.GoodsReceipt.GRHeaderV2 f = new ISBS_New.Purchase.GoodsReceipt.GRHeaderV2(vTransType);
            if (f.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPR.RowCount > 0)
                {
                    f.SetMode("BeforeEdit", dgvPR.CurrentRow.Cells["GR No"].Value.ToString());
                    f.ParentRefreshGrid4(this);
                    f.Show();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
           
        }

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
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                Conn = ConnectionString.GetConnection();
                Query = "select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + dgvPR.CurrentRow.Cells["GR No"].Value.ToString() + "'";
                Cmd = new SqlCommand(Query, Conn);
                string stats = Cmd.ExecuteScalar().ToString();

                if (stats == "03")
                {
                    MetroFramework.MetroMessageBox.Show(this, dgvPR.CurrentRow.Cells["GR No"].Value.ToString() + " is completed. Cannot delete data!", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    string msg = "";
                    int count = 0;
                    foreach (DataGridViewRow r in dgvPR.SelectedRows)
                    {
                        if (count >= 1)
                            msg += ", ";
                        msg += dgvPR.Rows[r.Index].Cells["GR No"].Value.ToString();
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
                            string GRID = "";
                            foreach (DataGridViewRow r in dgvPR.SelectedRows)
                            {
                                Query = "update [dbo].[GoodsReceivedH] set [GoodsReceivedStatus] = '04' where [GoodsReceivedId] = '" + dgvPR.Rows[r.Index].Cells["GR No"].Value.ToString() + "'";
                                Cmd = new SqlCommand(Query, Conn);
                                Dr = Cmd.ExecuteReader();
                                //created by Thaddaeus Matthias , 15 March 2018
                                //insert status log
                                //====================================begin====================================
                                insertstatuslogDelete(r);
                                //=====================================end=====================================
                                GRID = dgvPR.Rows[r.Index].Cells["GR No"].Value.ToString();
                            }
                            //invent movement when deleted
                            InsertInventMovement(GRID); //still check, hasnt set to be executed yet
                            //end==============================================================================
                        }
                        Conn.Close();
                        RefreshGrid();
                    }
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void dgvPR_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvPR.Rows[dgvPR.CurrentRow.Index].Selected = true;

            if (e.ColumnIndex != -1 && e.RowIndex > -1)
            {
                string GRId = dgvPR.Rows[e.RowIndex].Cells["GR No"].Value == null ? "" : dgvPR.Rows[e.RowIndex].Cells["GR No"].Value.ToString();

                Cmd = new SqlCommand("SELECT [VendId] FROM [GoodsReceivedH] WHERE [GoodsReceivedId] = '" + GRId + "'", Conn);
                string VendId = Cmd.ExecuteScalar().ToString();

                if (dgvPR.Columns[e.ColumnIndex].Name == "Preview")
                {
                    //Mode = "TT";
                    //ISBS_New.Purchase.GoodsReceipt.PreviewGR f = new ISBS_New.Purchase.GoodsReceipt.PreviewGR(GRId, Mode);
                    //f.Show();

                    GlobalPreview f = new GlobalPreview("Goods Receipt", GRId);
                    f.SetMode("TT"); //Inquiry buka tanda terima
                    f.Show();
                }
                else if (dgvPR.Columns[e.ColumnIndex].Name == "Send Email")
                {
                    //string ROId = dgvPR.Rows[e.RowIndex].Cells["GR No"].Value == null ? "" : dgvPR.Rows[e.RowIndex].Cells["GR No"].Value.ToString();

                    //ISBS_New.Purchase.GoodsReceipt.SendEmail s = new ISBS_New.Purchase.GoodsReceipt.SendEmail();
                    //s.flag(ROId); //,TransType);
                    //s.Show();

                    GlobalSendEmail f = new GlobalSendEmail("Goods Receipt", GRId, VendId);
                    f.Show();
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

        private void insertstatuslogDelete(DataGridViewRow r)
        {
            string PK1 = dgvPR.Rows[r.Index].Cells["GR No"].Value.ToString();
            string PK2 = dgvPR.Rows[r.Index].Cells["AccountNum"].Value.ToString();
            string PK3 = dgvPR.Rows[r.Index].Cells["Reference"].Value.ToString();
            Query = "INSERT INTO [dbo].[StatusLog_Vendor] VALUES ";
            Query += "('GRInquiry', '" + PK1 + "', '" + PK2 + "', '" + PK3 + "', '', '04', 'GR Deleted', '" + ControlMgr.UserId + "', getdate())";
            SqlCommand cmd2 = new SqlCommand(Query, Conn, Trans);
            cmd2.ExecuteNonQuery();
        }

        private void InsertInventMovement(string GRID)
        {
            Query = "SELECT MAX(GoodsReceivedSeqNo) FROM [GoodsReceivedD] WHERE [GoodsReceivedId] = '" + GRID + "' ";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                int row = Convert.ToInt32(Cmd.ExecuteScalar());
                string[] TargetIncrease = new string[row];
                string[] TargetIncreaseAlt = new string[row];
                string[] TargetIncreaseAmount = new string[row];
                string[] TargetDecrease = new string[row];
                string[] TargetDecreaseAlt = new string[row];
                string[] TargetDecreaseAmount = new string[row];
                string[] FullItemId = new string[row];
                decimal[] QtyIncrease = new decimal[row];
                decimal[] QtyDecrease = new decimal[row];
                decimal[] Ratio = new decimal[row];
                decimal[] Price = new decimal[row];
                for (int i = 0; i < row; i++)
                {
                    Query = "SELECT c.UoM_AvgPrice,a.*,b.* FROM [GoodsReceivedD] a LEFT JOIN [GoodsReceivedH] b ON a.[GoodsReceivedId] = b.[GoodsReceivedId] LEFT JOIN [InventTable] c ON c.FullItemID = a.FullItemId WHERE a.[GoodsReceivedId] = '" + GRID + "' AND a.[GoodsReceivedSeqNo] = " + (i + 1) + "";
                    using (SqlCommand Cmd2 = new SqlCommand(Query, Conn))
                    {
                        Dr = Cmd2.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            while (Dr.Read())
                            {
                                FullItemId[i] = Dr["FullItemId"].ToString();
                                Ratio[i] = Convert.ToDecimal(Dr["Ratio"]);
                                Price[i] = Convert.ToDecimal(Dr["UoM_AvgPrice"]);
                                if (Dr["Qty_Actual"] != null && Convert.ToDecimal(Dr["Qty_Actual"]) != 0)
                                {
                                    QtyIncrease[i] = Convert.ToDecimal(Dr["Qty_Actual"]);
                                    QtyDecrease[i] = Convert.ToDecimal(Dr["Qty_Actual"]);
                                }
                                else if (Dr["Qty_SJ"] != null && Convert.ToDecimal(Dr["Qty_SJ"]) != 0)
                                {
                                    QtyIncrease[i] = Convert.ToDecimal(Dr["Qty_SJ"]);
                                    QtyDecrease[i] = Convert.ToDecimal(Dr["Qty_SJ"]);
                                }
                                else
                                {
                                    QtyIncrease[i] = 0;
                                    QtyDecrease[i] = 0;
                                }
                                switch (Dr["ActionCodeStatus"].ToString())
                                {
                                    case "01": //bongkar
                                        if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "NT")
                                        {
                                            TargetIncrease[i] = "Transfer_Keluar_In_Progress_";
                                            TargetDecrease[i] = "Transfer_Masuk_In_Progress_";
                                        }
                                        else if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "RO")
                                        {
                                            TargetIncrease[i] = "";
                                            TargetDecrease[i] = "GR_In_Progress_";
                                        }
                                        break;
                                    case "02": //parked for action
                                        if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "NT")
                                        {
                                            TargetIncrease[i] = "Transfer_Keluar_In_Progress_";
                                            TargetDecrease[i] = "Transfer_Masuk_In_Progress_";
                                        }
                                        else if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "RO")
                                        {
                                            TargetIncrease[i] = "";
                                            TargetDecrease[i] = "Parked_For_Action_Outstanding_";
                                        }
                                        break;
                                    case "03": //reject
                                        break;
                                    case "04": //park - resize
                                        if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "NT")
                                        {
                                            TargetIncrease[i] = "Transfer_Keluar_In_Progress_";
                                            TargetDecrease[i] = "Transfer_Masuk_In_Progress_";
                                        }
                                        else if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "RO")
                                        {
                                            TargetIncrease[i] = "";
                                            TargetDecrease[i] = "Parked_For_Action_Outstanding_";
                                        }
                                        break;
                                    case "05": //Received
                                        if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "NT")
                                        {
                                            TargetIncrease[i] = "Transfer_Keluar_In_Progress_";
                                            TargetDecrease[i] = "Transfer_Masuk_In_Progress_";
                                        }
                                        else if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "RO")
                                        {
                                            TargetIncrease[i] = "";
                                            TargetDecrease[i] = "";
                                        }
                                        break;
                                    case "06": //new purchase
                                        break;
                                    case "07": //Retur Debit Note
                                        break;
                                    case "08": //Retur Tukar Barang
                                        break;
                                    case "09": //Resize
                                        if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "NT")
                                        {
                                            TargetIncrease[i] = "Transfer_Keluar_In_Progress_";
                                            TargetDecrease[i] = "Transfer_Masuk_In_Progress_";
                                        }
                                        else if (Dr["RefTransID"] != null && Dr["RefTransID"].ToString().Substring(0, 2).ToUpper() == "RO")
                                        {
                                            TargetIncrease[i] = "";
                                            TargetDecrease[i] = "Resize_In_Progress_";
                                        }
                                        break;
                                }
                                string QueryMovement = "UPDATE [Invent_Movement_Qty] SET ";
                                if (TargetIncrease[i] != "" && TargetIncrease[i] != null)
                                {
                                    TargetIncreaseAlt[i] = TargetIncrease[i] + "Alt";
                                    TargetIncreaseAmount[i] = TargetIncrease[i] + "Amount";
                                    TargetIncrease[i] = TargetIncrease[i] + "UoM";

                                    QueryMovement += " " + TargetIncrease[i] + " += " + QtyIncrease[i] + ", ";
                                    QueryMovement += " " + TargetIncreaseAlt[i] + " += " + (QtyIncrease[i] * Ratio[i]) + ", ";
                                    QueryMovement += " " + TargetIncreaseAmount[i] + " += " + (QtyIncrease[i] * Price[i]) + ", ";
                                }
                                if (TargetDecrease[i] != "" && TargetDecrease[i] != null)
                                {
                                    TargetDecreaseAlt[i] = TargetDecrease[i] + "Alt";
                                    TargetDecreaseAmount[i] = TargetDecrease[i] + "Amount";
                                    TargetDecrease[i] = TargetDecrease[i] + "UoM";

                                    QueryMovement += " " + TargetDecrease[i] + " -= " + QtyDecrease[i] + ", ";
                                    QueryMovement += " " + TargetDecreaseAlt[i] + " -= " + (QtyDecrease[i] * Ratio[i]) + ", ";
                                    QueryMovement += " " + TargetDecreaseAmount[i] + " -= " + (QtyDecrease[i] * Price[i]) + ", ";
                                }
                                QueryMovement += " [ItemName]=[ItemName] WHERE FullItemId = '" + FullItemId[i] + "' ";
                                //need recheck targetincrease n targetdecrease, benar atau tidak
                                //using (Cmd = new SqlCommand(QueryMovement, Conn))
                                //{
                                //    Cmd.ExecuteNonQuery();
                                //}
                            }
                            Dr.Close();
                        }
                    }
                }

            }
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Page2 != null)
            {
                if (Convert.ToInt32(txtPage.Text) > Page2)
                {
                    txtPage.Text = Page2.ToString();
                }
            }
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
            else if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria != null)
            {
                if (cmbCriteria.Text.Contains("Date"))
                {
                    dtFrom.Enabled = true;
                    dtTo.Enabled = true;
                    txtSearch.Text = "";
                    txtSearch.Enabled = false;
                }
                else
                {
                    dtFrom.Enabled = false;
                    dtTo.Enabled = false;
                    txtSearch.Enabled = true;
                }
            }
        }
















    }
}
