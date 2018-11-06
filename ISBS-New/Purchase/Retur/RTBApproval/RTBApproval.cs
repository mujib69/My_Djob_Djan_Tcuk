using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.Retur.RTBApproval
{
    public partial class RTBApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Status, Query, crit, RTBNumber, GRID = null;
        Purchase.Retur.RTBApproval.InqRTBApproval Parent;

        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public RTBApproval()
        {
            InitializeComponent();
        }

        private void RTBApproval_Load(object sender, EventArgs e)
        {
            ModeApprove();
            GetDataHeader();
        }

        public void setParent(Purchase.Retur.RTBApproval.InqRTBApproval f)
        {
            Parent = f;
        }

        public void SetMode(string tmpMode, string tmpRTBNumber)
        {
            Mode = tmpMode;
            RTBNumber = tmpRTBNumber;
        }

        public void CreateGrid()
        {
            dgvRTB.Rows.Clear();
            if (dgvRTB.RowCount - 1 <= 0)
            {
                dgvRTB.ColumnCount = 18;
                dgvRTB.Columns[0].Name = "No";
                dgvRTB.Columns[1].Name = "ItemID";
                dgvRTB.Columns[2].Name = "FullItemID"; dgvRTB.Columns["FullItemID"].HeaderText = "Item ID";
                dgvRTB.Columns[3].Name = "ItemName"; dgvRTB.Columns["ItemName"].HeaderText = "Name";
                dgvRTB.Columns[4].Name = "GroupId";
                dgvRTB.Columns[5].Name = "SubGroup1ID";
                dgvRTB.Columns[6].Name = "SubGroup2ID";
                dgvRTB.Columns[7].Name = "Qty_GR";
                dgvRTB.Columns[8].Name = "UoM_Qty";
                dgvRTB.Columns[9].Name = "UoM_Unit";
                dgvRTB.Columns[10].Name = "Alt_Qty";
                dgvRTB.Columns[11].Name = "Alt_Unit";
                dgvRTB.Columns[12].Name = "Ratio";
                dgvRTB.Columns[13].Name = "Ratio_Actual";
                dgvRTB.Columns[14].Name = "GoodsReceivedSeqNo";
                dgvRTB.Columns[15].Name = "InventSiteBlokID";
                dgvRTB.Columns[16].Name = "Quality";
                dgvRTB.Columns[17].Name = "Notes";
            }
        }

        public void ModeApprove()
        {
            Mode = "Approve";

            txtNotes.Enabled = false;
            dgvRTB.ReadOnly = true;

            gbApprove.Visible = true;
        }

        public void GetDataHeader()
        {
            if (RTBNumber != "")
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select Top 1 RTBDate, GoodsReceivedID, VendID, VendName, SiteID, SiteName, SiteLocation, Notes, TransStatus, b.Deskripsi, ApprovedBy From [ReturTukarBarangH] a Left JOIN [TransStatusTable] b ON a.TransStatus = b.StatusCode And b.TransCode = 'ReturTukarBarang' Where RTBId = '" + RTBNumber + "'";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtRTB.Text = Dr["RTBDate"].ToString();
                    txtRTBNum.Text = RTBNumber;
                    txtGRNum.Text = Dr["GoodsReceivedID"].ToString();
                    txtVendID.Text = Dr["VendID"].ToString();
                    txtVendName.Text = Dr["VendName"].ToString();
                    txtSiteID.Text = Dr["SiteID"].ToString();
                    txtSiteName.Text = Dr["SiteName"].ToString();
                    txtSiteLocation.Text = Dr["SiteLocation"].ToString();
                    txtNotes.Text = Dr["Notes"].ToString();
                    txtStatusName.Text = Dr["Deskripsi"].ToString();
                    txtApproved.Text = Dr["ApprovedBy"].ToString();

                    if (Dr["TransStatus"].ToString() == "03" || Dr["TransStatus"].ToString() == "04" || Dr["TransStatus"].ToString() == "05")
                    {
                        gbApprove.Visible = false;
                    }
                }
                Dr.Close();

                CreateGrid();

                Query = "Select SeqNo, ItemId, [FullItemID], ItemName, GroupId, SubGroup1Id, SubGroup2Id, Qty_GR, [UoM_Qty], [UoM_Unit], [Alt_Qty], [Alt_Unit], Ratio, Ratio_Actual, [GoodsReceivedID], [GoodsReceivedSeqNo], [InventSiteId], [InventSiteBlokID], [Quality], Notes From [ReturTukarBarangD] Where RTBId = '" + RTBNumber + "' order by SeqNo asc";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    this.dgvRTB.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemID"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["Qty_GR"], Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceivedSeqNo"], Dr["InventSiteBlokID"], Dr["Quality"], Dr["Notes"]);
                    j++;
                }
                Dr.Close();

                dgvRTB.ReadOnly = true;
                dgvRTB.Columns["No"].ReadOnly = true;
                dgvRTB.Columns["FullItemID"].ReadOnly = true;
                dgvRTB.Columns["ItemName"].ReadOnly = true;
                dgvRTB.Columns["Qty_GR"].ReadOnly = true;
                dgvRTB.Columns["UoM_Qty"].ReadOnly = false;
                dgvRTB.Columns["UoM_Unit"].ReadOnly = true;
                dgvRTB.Columns["Alt_Qty"].ReadOnly = true;
                dgvRTB.Columns["Alt_Unit"].ReadOnly = true;
                dgvRTB.Columns["Ratio"].ReadOnly = true;
                dgvRTB.Columns["Ratio_Actual"].ReadOnly = true;
                dgvRTB.Columns["InventSiteBlokID"].ReadOnly = true;
                dgvRTB.Columns["Quality"].ReadOnly = true;
                dgvRTB.Columns["Notes"].ReadOnly = true;

                dgvRTB.Columns["ItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["UoM_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["UoM_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Alt_Qty"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Alt_Unit"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Ratio_Actual"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["InventSiteBlokID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Quality"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvRTB.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dgvRTB.Columns["Qty_GR"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["UoM_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["Ratio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["Ratio_Actual"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvRTB.Columns["Alt_Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                dgvRTB.Columns["ItemID"].Visible = false;
                dgvRTB.Columns["GroupId"].Visible = false;
                dgvRTB.Columns["SubGroup1ID"].Visible = false;
                dgvRTB.Columns["SubGroup2ID"].Visible = false;
                dgvRTB.Columns["Ratio"].Visible = false;
                dgvRTB.Columns["GoodsReceivedSeqNo"].Visible = false;

                dgvRTB.AutoResizeColumns();

                Conn.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "Update [ReturTukarBarangH] set ";
                    Query += "TransStatus = '03',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where RTBId='" + txtRTBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data RTBNumber : " + txtRTBNum.Text + " berhasil diupdate.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
            
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "Update [ReturTukarBarangH] set ";
                    Query += "TransStatus = '05',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where RTBId='" + txtRTBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //Update Qty
                    List<string> FullItemID = new List<string>();
                    List<string> GoodsReceivedID = new List<string>();
                    List<decimal> Qty = new List<decimal>();
                    decimal RemainingQty, QtyNew = 0;
                    Query = "Select FullItemID,UoM_Qty,[GoodsReceivedID] From [ReturTukarBarangD] Where RTBId='" + txtRTBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        FullItemID.Add(Dr["FullItemID"].ToString());
                        GoodsReceivedID.Add(Dr["GoodsReceivedID"].ToString());
                        Qty.Add(decimal.Parse(Dr["UoM_Qty"].ToString()));
                    }
                    Dr.Close();

                    for (int i = 0; i < FullItemID.Count; i++)
                    {
                        Query = "Select Remaining_Qty From [GoodsReceivedD] Where [GoodsReceivedId] = '" + GoodsReceivedID[i] + "' AND FullItemId = '" + FullItemID[i] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        RemainingQty = decimal.Parse(Cmd.ExecuteScalar().ToString());

                        QtyNew = RemainingQty + Qty[i];

                        Query = "Update [GoodsReceivedD] set ";
                        Query += "Remaining_Qty='" + QtyNew + "' ";
                        Query += "where GoodsReceivedId='" + GoodsReceivedID[i] + "' And FullItemID='" + FullItemID[i] + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }


                    Trans.Commit();
                    MessageBox.Show("Data RTBNumber : " + txtRTBNum.Text + " berhasil diupdate.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void btnRevision_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Approve) > 0)
            {
                try
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "Update [ReturTukarBarangH] set ";
                    Query += "TransStatus = '02',";
                    Query += "ApprovedBy = '" + ControlMgr.UserId + "',";
                    Query += "UpdatedDate=getdate(),";
                    Query += "UpdatedBy='" + ControlMgr.UserId + "' where RTBId='" + txtRTBNum.Text.Trim() + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    Trans.Commit();
                    MessageBox.Show("Data RTBNumber : " + txtRTBNum.Text + " berhasil diupdate.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                finally
                {
                    Conn.Close();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }
    }
}
