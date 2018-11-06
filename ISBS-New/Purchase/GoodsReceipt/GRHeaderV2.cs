using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Transactions;

//BY: HC
//MASUKIN Nota Transfer, Nota Retur Jual ke cbRef
namespace ISBS_New.Purchase.GoodsReceipt
{
    public partial class GRHeaderV2 : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        //private SqlConnection conMaster;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private string Query;
        /**********SQL*********/

        private static string vTransType;

        /**********STANDARD*********/

        InquiryGoodsReceipt Parent = new InquiryGoodsReceipt(vTransType);
        private string Mode;
        private string GRNumber = "";
        /**********STANDARD*********/

        /*********datagridview cols*********/
        string[] tableColsName1 = new string[] { "No", "GoodsReceivedSeqNo", "RefTransID", "RefTransSeqNo", "GroupId", "SubGroup1Id", "SubGroup2Id", "ItemId", "FullItemId", "ItemName", "Qty", "Qty_SJ", "Qty_Actual", "Remaining_Qty", "Unit", "Ratio", "Ratio_Actual", "TotalBerat", "TotalBerat_Actual", "InventSiteId", "InventSiteBlokID", "ActionCodeStatus", "Quality", "Notes" };
        string[] tableCols1Header1 = new string[] { "No", "GoodsReceivedSeqNo", "RefTransID", "RefTransSeqNo", "GroupId", "SubGroup1Id", "SubGroup2Id", "ItemId", "FullItemId", "ItemName", "Qty", "Qty_SJ", "Qty_Actual", "Remaining_Qty", "Unit", "Ratio", "Ratio_Actual", "TotalBerat", "TotalBerat_Actual", "InventSiteId", "InventSiteBlokID", "ActionCodeStatus", "Quality", "Notes" };

        string[] tableColsName2 = new string[] { "No", "RefTransID", "RefTransSeqNo", "GroupId", "SubGroup1Id", "SubGroup2Id", "ItemId", "FullItemId", "ItemName", "Qty", "Remaining_Qty", "Unit" };
        string[] tableCols1Header2 = new string[] { "No", "Ref Trans ID", "Ref Trans SeqNo", "GroupId", "SubGroup1Id", "SubGroup2Id", "ItemId", "FullItemId", "Item Name", "Ref Qty", "Ref Remaining Qty", "Unit" };
        /*********datagridview cols*********/

        /*********variable global form*********/
        string SchemaName, TableName, Where;
        string OldGrStat = "";
        /*********variable global form*********/

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        string msg;
        List<string> AllItemId = new List<string>();
        List<string> RefRemainingRO = new List<string>(); //urutan sesuai AllItemId
        /*********VALIDATION*********/

        /*********GV COMBO BOX*********/
        DataGridViewComboBoxCell cell;
        private SqlDataReader Dr2;
        /*********GV COMBO BOX*********/

        bool Journal = false;
        //tia edit
        ContextMenu vendid = new ContextMenu();
        //tiaeditend
        public void SetParent(InquiryGoodsReceipt F)
        {
            Parent = F;
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public void SetMode(string tmpMode, string tmpGRNumber)
        {
            Mode = tmpMode;
            GRNumber = tmpGRNumber;
            tbxGRNum.Text = tmpGRNumber;
        }

        public GRHeaderV2(string _vTransType)
        {
            InitializeComponent();
            vTransType = _vTransType;
        }

        private void ModeNew()
        {
            Mode = "New";
            if (tbxRefID.Text == String.Empty)
            {
                //hendry
                if (!cmbReferenceType.Items.Contains(vTransType))
                    cmbReferenceType.Items.Add(vTransType);
                cmbReferenceType.SelectedItem = vTransType;
                if (vTransType == "Receipt Order")
                {
                    if (!cmbReferenceType.Items.Contains("Nota Retur Beli"))
                        cmbReferenceType.Items.Add("Nota Retur Beli");
                    cmbReferenceType.Enabled = true;
                }

                dtGR.Value = DateTime.Now;
                dtDO.Value = DateTime.Now;
                dtWeight1.Value = DateTime.Now;
                btnCancel.Enabled = false;

                //hendry              
                btnSRefID.Enabled = true;
                tabPage2.Text = "Detail " + cmbReferenceType.Text;
                dataGridView1.Rows.Clear();
                dtRef.Value = Convert.ToDateTime("01/01/1753");
                tbxRefID.Text = String.Empty;
                tbxNameID.Text = String.Empty;
                tbxName.Text = String.Empty;
                dtExpectedDate.Value = Convert.ToDateTime("01/01/1753");
                tbxVOwnerID.Text = String.Empty;
                tbxVOwner.Text = String.Empty;
                tbxVType.Text = String.Empty;
                tbxVNumber.Text = String.Empty;
                tbxDriverName.Text = String.Empty;
                txtInventSiteID.Text = String.Empty;
                txtWarehouse.Text = String.Empty;
                txtSiteType.Text = String.Empty;
                tbxNotes.Text = String.Empty;
                cbVOwner.Checked = false;
                //hendry end
                btnApprove.Visible = false;
            }

            if (dataGridView1.ColumnCount != 0)
            {
                dataGridView1.Columns["GoodsReceivedSeqNo"].Visible = false;
                dataGridView1.Columns["RefTransID"].Visible = false;
                dataGridView1.Columns["RefTransSeqNo"].Visible = false;
                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.Columns["SubGroup1Id"].Visible = false;
                dataGridView1.Columns["SubGroup2Id"].Visible = false;
                dataGridView1.Columns["ItemId"].Visible = false;
                dataGridView1.Columns["Qty"].Visible = false;
                dataGridView1.Columns["Qty_Actual"].Visible = false;
                dataGridView1.Columns["Remaining_Qty"].Visible = false;
                dataGridView1.Columns["Ratio"].Visible = false;
                dataGridView1.Columns["Ratio_Actual"].Visible = false;
                dataGridView1.Columns["TotalBerat"].Visible = true;
                dataGridView1.Columns["TotalBerat_Actual"].Visible = false;
                dataGridView1.Columns["InventSiteId"].Visible = false;
                dataGridView1.Columns["Quality"].Visible = false;

                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    if (dataGridView1.Columns[i].Name == "Qty_SJ" || dataGridView1.Columns[i].Name == "InventSiteBlokID" || dataGridView1.Columns[i].Name == "Notes" || dataGridView1.Columns[i].Name == "ActionCodeStatus")
                    {
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView1.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        dataGridView1.Columns[i].ReadOnly = true;
                    }
                    if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                    {
                        if (dataGridView1.Columns[i].Name == "ActionCodeStatus")
                        {
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                            dataGridView1.Columns[i].ReadOnly = true;
                            //dataGridView1.Columns["Qty_Actual"].Visible = true;
                            dataGridView1.Columns["Ratio_Actual"].Visible = true;
                            dataGridView1.Columns["Qty_SJ"].HeaderText = "Qty Input";
                        }
                    }
                }
            }

            for (int i = 0; i < dataGridView3.ColumnCount; i++)
            {
                if (dataGridView3.Columns[i].Name == "Qty_SJ" || dataGridView3.Columns[i].Name == "InventSiteBlokID" || dataGridView3.Columns[i].Name == "Notes" || dataGridView3.Columns[i].Name == "ActionCodeStatus")
                {
                    dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    dataGridView3.Columns[i].ReadOnly = false;
                }
                else
                {
                    dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView3.Columns[i].ReadOnly = true;
                }
            }

            if (ControlMgr.GroupName == "Purchase Admin")
            {
                if (!cmbReferenceType.Items.Contains(vTransType))
                    cmbReferenceType.Items.Add(vTransType);
                cmbReferenceType.SelectedItem = vTransType;
                if (vTransType == "Receipt Order")
                {
                    if (!cmbReferenceType.Items.Contains("Nota Retur Beli"))
                        cmbReferenceType.Items.Add("Nota Retur Beli");
                    cmbReferenceType.Enabled = true;
                }
                gbWeight1.Enabled = false;
                gbWeight2.Enabled = false;
                gbWeight2.Visible = true;
                tbxVType.Enabled = false;
                tbxVNumber.Enabled = false;
                tbxDriverName.Enabled = false;
            }
        }

        private bool CheckVirtualSite()
        {
            bool status = false;
            if (txtWarehouse.Text != "" && txtWarehouse.Text != null)
            {
                Query = "SELECT [SiteType] FROM [dbo].[InventSite] WHERE [InventSiteID] = '" + txtWarehouse.Text + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            string text = Dr["SiteType"].ToString().ToUpper();
                            if (text == "VIRTUAL SITE")
                            {
                                status = true;
                            }
                        }
                    }
                    Dr.Close();
                }
            }
            return status;
        }

        private void ModeEdit()
        {
            Mode = "Edit";
            Conn = ConnectionString.GetConnection();
            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string GRStats = Cmd.ExecuteScalar().ToString();
            Conn.Close();

            btnEdit.Enabled = false;
            btnSave.Enabled = true;
            btnCancel.Enabled = true;
            btnExit.Enabled = false;

            btnPrint.Enabled = false;
            rbTicket.Enabled = false;

            if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "01")
            {
                dtDO.Enabled = true;
                tbxDelivNum.Enabled = true;
                btnSOwner.Enabled = true;
                cbVOwner.Enabled = true;
                tbxVType.Enabled = true;
                tbxVNumber.Enabled = true;
                tbxDriverName.Enabled = true;
                tbxNotes.Enabled = true;
                tbxWeight1.Enabled = true;
                if (cbWeight1.Checked == true)
                {
                    cbWeight1.Enabled = true;
                    tbxWeight1.Enabled = true;
                    btnWeight1.Enabled = false;
                }
                else
                {
                    cbWeight1.Enabled = false;
                    tbxWeight1.Enabled = false;
                    btnWeight1.Enabled = true;
                }
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;

                for (int i = 0; i < tableColsName1.Length; i++)
                {
                    if (dataGridView1.Columns[i].Name == "Qty_SJ" || dataGridView1.Columns[i].Name == "InventSiteBlokID" || dataGridView1.Columns[i].Name == "Notes" || dataGridView1.Columns[i].Name == "ActionCodeStatus")
                    {
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView1.Columns[i].ReadOnly = false;
                    }
                }

                for (int i = 0; i < tableColsName1.Length; i++)
                {
                    if (dataGridView3.Columns[i].Name == "Qty_SJ" || dataGridView3.Columns[i].Name == "InventSiteBlokID" || dataGridView3.Columns[i].Name == "Notes" || dataGridView3.Columns[i].Name == "ActionCodeStatus")
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView3.Columns[i].ReadOnly = false;
                    }
                }
            }
            else if (ControlMgr.GroupName == "KERANI" && (GRStats == "01" || GRStats == "02"))
            {
                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
                btnWeight2.Enabled = true;
                cbWeight2.Enabled = true;
                for (int i = 0; i < tableColsName1.Length; i++)
                {
                    if (dataGridView1.Columns[i].Name == "Qty_Actual" || dataGridView1.Columns[i].Name == "TotalBerat_Actual" || dataGridView1.Columns[i].Name == "Quality" || dataGridView1.Columns[i].Name == "Notes" || dataGridView1.Columns[i].Name == "ActionCodeStatus")
                    {
                        dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView1.Columns[i].ReadOnly = false;
                    }
                }
                for (int i = 0; i < tableColsName1.Length; i++)
                {
                    if (dataGridView3.Columns[i].Name == "Qty_SJ" || dataGridView3.Columns[i].Name == "Qty_Actual" || dataGridView3.Columns[i].Name == "TotalBerat_Actual" || dataGridView3.Columns[i].Name == "Quality" || dataGridView3.Columns[i].Name == "Notes" || dataGridView3.Columns[i].Name == "ActionCodeStatus")
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView3.Columns[i].ReadOnly = false;
                    }
                }

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        if (dataGridView1.Columns[j].Name == "GoodsReceivedSeqNo")
                        {
                            if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                            {
                                dataGridView1.Rows[i].Cells["InventSiteBlokID"].ReadOnly = false;
                                dataGridView1.Rows[i].Cells["InventSiteBlokID"].Style.BackColor = Color.White;
                            }
                        }
                    }
                }

                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView3.ColumnCount; j++)
                    {
                        if (dataGridView3.Columns[j].Name == "GoodsReceivedSeqNo")
                        {
                            if (dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                            {
                                dataGridView3.Rows[i].Cells["InventSiteBlokID"].ReadOnly = false;
                                dataGridView3.Rows[i].Cells["InventSiteBlokID"].Style.BackColor = Color.White;
                            }
                        }
                    }
                }
            }
            //else if (ControlMgr.GroupName == "KERANI" && GRStats == "02")
            //{
            //    btnNew.Enabled = true;
            //    btnDelete.Enabled = true;
            //    btnWeight2.Enabled = true;
            //    cbWeight2.Enabled = true;

            //    for (int i = 0; i < tableColsName1.Length; i++)
            //    {
            //        if (dataGridView1.Columns[i].Name == "Qty_SJ" || dataGridView1.Columns[i].Name == "Qty_Actual" || dataGridView1.Columns[i].Name == "TotalBerat_Actual" || dataGridView1.Columns[i].Name == "Quality" || dataGridView1.Columns[i].Name == "Notes" || dataGridView1.Columns[i].Name == "ActionCodeStatus")
            //        {
            //            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
            //            dataGridView1.Columns[i].ReadOnly = false;
            //        }
            //    }

            //    for (int i = 0; i < dataGridView1.RowCount; i++)
            //    {
            //        for (int j = 0; j < dataGridView1.ColumnCount; j++)
            //        {
            //            if (dataGridView1.Columns[j].Name == "GoodsReceivedSeqNo")
            //            {
            //                if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
            //                {
            //                    dataGridView1.Rows[i].Cells["InventSiteBlokID"].ReadOnly = false;
            //                    dataGridView1.Rows[i].Cells["InventSiteBlokID"].Style.BackColor = Color.White;
            //                }
            //            }
            //        }
            //    }
            //}
            else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "02")
            {
                cbWeight2.Enabled = true;
                btnWeight2.Enabled = true;
                cbWeight2_CheckedChanged(new object(), new EventArgs());
            }
            else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "03")
            {
                cbWeight2.Enabled = true;
                btnWeight2.Enabled = true;
                rbTT.Enabled = false;
            }
            else if (ControlMgr.GroupName == "KERANI" && GRStats == "03")
            {

            }
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            //cbRef.Enabled = false;
            btnSRefID.Enabled = false;
            dtDO.Enabled = false;
            tbxDelivNum.Enabled = false;
            btnSOwner.Enabled = false;
            cbVOwner.Enabled = false;
            tbxVType.Enabled = false;
            tbxVNumber.Enabled = false;
            tbxDriverName.Enabled = false;
            tbxNotes.Enabled = false;
            tbxWeight1.Enabled = false;
            cbWeight1.Enabled = false;
            btnWeight1.Enabled = false;
            tbxWeight2.Enabled = false;
            cbWeight2.Enabled = false;
            btnWeight2.Enabled = false;
            //tia edit
            tbxNameID.Enabled = true;
            tbxName.Enabled = true;
            tbxRefID.Enabled = true;

            tbxNameID.ReadOnly = true;
            tbxName.ReadOnly = true;
            tbxRefID.ReadOnly = true;

            tbxNameID.ContextMenu = vendid;
            tbxName.ContextMenu = vendid;
            tbxRefID.ContextMenu = vendid;

            //tia end
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;

            btnCancel.Enabled = false;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnExit.Enabled = true;

            //button approval configuration ===========================
            if (ControlMgr.GroupName.ToUpper() == "SITE MANAGER")
            {
                btnApprove.Visible = true;
            }
            string GRstatus = "";
            Query = "SELECT [GoodsReceivedStatus] FROM [dbo].[GoodsReceivedH] WHERE [GoodsReceivedId] = @GoodsReceivedId";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                GRstatus = Cmd.ExecuteScalar().ToString();
            }
            if (GRstatus != "05")
            {
                btnApprove.Enabled = false;
            }
            else
            {
                btnApprove.Enabled = true;
            }
            //end========================================================

            if (dataGridView1.ColumnCount != 0)
            {
                dataGridView1.Columns["GoodsReceivedSeqNo"].Visible = false;
                dataGridView1.Columns["RefTransID"].Visible = false;
                dataGridView1.Columns["RefTransSeqNo"].Visible = false;
                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.Columns["SubGroup1Id"].Visible = false;
                dataGridView1.Columns["SubGroup2Id"].Visible = false;
                dataGridView1.Columns["ItemId"].Visible = false;
                dataGridView1.Columns["Qty"].Visible = false;
                dataGridView1.Columns["Remaining_Qty"].Visible = false;
                dataGridView1.Columns["Ratio"].Visible = false;
                dataGridView1.Columns["Ratio_Actual"].Visible = false;
                dataGridView1.Columns["InventSiteId"].Visible = false;
            }
            Conn = ConnectionString.GetConnection();
            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string GRStats = Cmd.ExecuteScalar().ToString();

            if (dataGridView1.ColumnCount != 0)
            {
                if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "01")
                {
                    btnPrint.Enabled = true;
                    rbTicket.Enabled = true;
                    rbTicket.Checked = true;

                    dataGridView1.Columns["Qty_Actual"].Visible = false;
                    dataGridView1.Columns["TotalBerat"].Visible = true;
                    dataGridView1.Columns["TotalBerat_Actual"].Visible = false;
                    dataGridView1.Columns["Quality"].Visible = false;
                }
                else if (ControlMgr.GroupName == "KERANI" && GRStats == "01")
                {
                    //gbWeight2.Visible = true;
                    dataGridView1.Columns["Qty_SJ"].Visible = true;
                    dataGridView1.Columns["TotalBerat"].Visible = true;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "02")
                {
                    gbWeight2.Visible = true;
                }
                else if (ControlMgr.GroupName == "KERANI" && GRStats == "02")
                {
                    dataGridView1.Columns["Qty_SJ"].Visible = true;
                    dataGridView1.Columns["TotalBerat"].Visible = true;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "03")
                {
                    gbWeight2.Visible = true;
                    btnPrint.Enabled = true;
                    rbTT.Enabled = true;
                    rbTT.Checked = true;
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "KERANI" && GRStats == "03")
                {
                    btnEdit.Enabled = false;
                }
                else if ((ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "Purchase Manger") && GRStats == "05")
                {
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "SITE MANAGER" && GRStats == "05")
                {
                    btnApprove.Enabled = true;
                    gbWeight2.Visible = true;
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "SITE MANAGER" && GRStats == "06")
                {
                    btnEdit.Enabled = false;
                    btnApprove.Enabled = false;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "06")
                {
                    gbWeight2.Visible = true;
                    btnEdit.Enabled = false;
                    btnPrint.Enabled = true;
                    rbTT.Checked = true;
                    rbTT.Enabled = true;
                }
                if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "05")
                {
                    gbWeight2.Visible = true;
                }

                if (GRStats == "03")
                {
                    btnResize.Enabled = true;
                }

                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView1.Columns[i].ReadOnly = true;
                }

                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" || ControlMgr.GroupName == "Purchase Admin")
                {
                    btnEdit.Enabled = false;
                }
            }

            for (int i = 0; i < dataGridView3.ColumnCount; i++)
            {
                if (Mode == "New" || (Mode == "Edit" && GRStats == "01" && ControlMgr.GroupName == "WB OPERATOR"))
                {
                    if (dataGridView3.Columns[i].Name == "Qty_SJ" || dataGridView3.Columns[i].Name == "InventSiteBlokID" || dataGridView3.Columns[i].Name == "Notes" || dataGridView3.Columns[i].Name == "ActionCodeStatus")
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView3.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        dataGridView3.Columns[i].ReadOnly = true;
                    }
                }
                else if (Mode == "Edit" && ControlMgr.GroupName == "KERANI" && (GRStats == "01" || GRStats == "02"))
                {
                    if (dataGridView3.Columns[i].Name == "Qty_Actual" || dataGridView3.Columns[i].Name == "TotalBerat_Actual" || dataGridView3.Columns[i].Name == "ActionCodeStatus" || dataGridView3.Columns[i].Name == "Quality" || dataGridView3.Columns[i].Name == "Notes")
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView3.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        dataGridView3.Columns[i].ReadOnly = true;
                    }
                }
                else
                {
                    dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView3.Columns[i].ReadOnly = true;
                }
            }

            //set all control to disabled if login usergroup not registered
            if (!(ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "SITE MANAGER" || ControlMgr.GroupName == "Purchase Admin"))
            {
                ConnectionString.DisableOrEnableControls(this, false);
                ConnectionString.EnableControls(btnExit);
            }

            if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && ControlMgr.GroupName == "Purchase Admin")
            {
                btnPrint.Enabled = true;
            }
        }

        //tia edit
        private void ModePopUp()
        {
            Mode = "PopUp";

            btnSRefID.Enabled = false;
            dtDO.Enabled = false;
            tbxDelivNum.Enabled = false;
            btnSOwner.Enabled = false;
            cbVOwner.Enabled = false;
            tbxVType.Enabled = false;
            tbxVNumber.Enabled = false;
            tbxDriverName.Enabled = false;
            tbxNotes.Enabled = false;
            tbxWeight1.Enabled = false;
            cbWeight1.Enabled = false;
            btnWeight1.Enabled = false;
            tbxWeight2.Enabled = false;
            cbWeight2.Enabled = false;
            btnWeight2.Enabled = false;
            //tia edit
            tbxNameID.Enabled = true;
            tbxName.Enabled = true;
            tbxNameID.ReadOnly = true;
            tbxName.ReadOnly = true;
            tbxNameID.ContextMenu = vendid;
            tbxName.ContextMenu = vendid;
            //tia end
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;

            btnCancel.Visible = false;
            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnExit.Visible = true;
            btnApprove.Visible = true;


            if (dataGridView1.ColumnCount != 0)
            {
                dataGridView1.Columns["GoodsReceivedSeqNo"].Visible = false;
                dataGridView1.Columns["RefTransID"].Visible = false;
                dataGridView1.Columns["RefTransSeqNo"].Visible = false;
                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.Columns["SubGroup1Id"].Visible = false;
                dataGridView1.Columns["SubGroup2Id"].Visible = false;
                dataGridView1.Columns["ItemId"].Visible = false;
                dataGridView1.Columns["Qty"].Visible = false;
                dataGridView1.Columns["Remaining_Qty"].Visible = false;
                dataGridView1.Columns["Ratio"].Visible = false;
                dataGridView1.Columns["Ratio_Actual"].Visible = false;
                dataGridView1.Columns["InventSiteId"].Visible = false;
            }
            btnCancel.Visible = false;
            btnEdit.Visible = false;
            btnExit.Visible = true;
            btnApprove.Visible = false;

            Conn = ConnectionString.GetConnection();
            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            string GRStats = Cmd.ExecuteScalar().ToString();

            if (dataGridView1.ColumnCount != 0)
            {
                if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "01")
                {
                    btnPrint.Enabled = true;
                    rbTicket.Enabled = true;
                    rbTicket.Checked = true;

                    dataGridView1.Columns["Qty_Actual"].Visible = false;
                    dataGridView1.Columns["TotalBerat"].Visible = true;
                    dataGridView1.Columns["TotalBerat_Actual"].Visible = false;
                    dataGridView1.Columns["Quality"].Visible = false;
                }
                else if (ControlMgr.GroupName == "KERANI" && GRStats == "01")
                {
                    //gbWeight2.Visible = true;
                    dataGridView1.Columns["Qty_SJ"].Visible = true;
                    dataGridView1.Columns["TotalBerat"].Visible = true;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "02")
                {
                    gbWeight2.Visible = false;
                }
                else if (ControlMgr.GroupName == "KERANI" && GRStats == "02")
                {
                    dataGridView1.Columns["Qty_SJ"].Visible = true;
                    dataGridView1.Columns["TotalBerat"].Visible = true;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "03")
                {
                    gbWeight2.Visible = false;
                    btnPrint.Enabled = true;
                    rbTT.Enabled = true;
                    rbTT.Checked = true;
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "KERANI" && GRStats == "03")
                {

                }
                else if ((ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "Purchase Manger") && GRStats == "05")
                {
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "SITE MANAGER" && GRStats == "05")
                {
                    btnApprove.Enabled = true;
                    gbWeight2.Visible = false;
                    btnEdit.Enabled = false;
                }
                else if (ControlMgr.GroupName == "SITE MANAGER" && GRStats == "06")
                {
                    btnEdit.Enabled = false;
                    btnApprove.Enabled = false;
                }
                else if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "06")
                {
                    gbWeight2.Visible = false;
                    btnEdit.Enabled = false;
                    btnPrint.Enabled = true;
                    rbTT.Checked = true;
                    rbTT.Enabled = true;
                }
                if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "05")
                {
                    gbWeight2.Visible = false;
                }

                if (GRStats == "03")
                {
                    btnResize.Enabled = true;
                }

                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView1.Columns[i].ReadOnly = true;
                }

                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" || ControlMgr.GroupName == "Purchase Admin")
                {
                    btnEdit.Enabled = false;
                }
            }

            for (int i = 0; i < dataGridView3.ColumnCount; i++)
            {
                if (Mode == "New" || (Mode == "Edit" && GRStats == "01" && ControlMgr.GroupName == "WB OPERATOR"))
                {
                    if (dataGridView3.Columns[i].Name == "Qty_SJ" || dataGridView3.Columns[i].Name == "InventSiteBlokID" || dataGridView3.Columns[i].Name == "Notes" || dataGridView3.Columns[i].Name == "ActionCodeStatus")
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView3.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        dataGridView3.Columns[i].ReadOnly = true;
                    }
                }
                else if (Mode == "Edit" && ControlMgr.GroupName == "KERANI" && (GRStats == "01" || GRStats == "02"))
                {
                    if (dataGridView3.Columns[i].Name == "Qty_Actual" || dataGridView3.Columns[i].Name == "TotalBerat_Actual" || dataGridView3.Columns[i].Name == "ActionCodeStatus" || dataGridView3.Columns[i].Name == "Quality" || dataGridView3.Columns[i].Name == "Notes")
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        dataGridView3.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        dataGridView3.Columns[i].ReadOnly = true;
                    }
                }
                else
                {
                    dataGridView3.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView3.Columns[i].ReadOnly = true;
                }
            }

            //set all control to disabled if login usergroup not registered
            if (!(ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "SITE MANAGER" || ControlMgr.GroupName == "Purchase Admin"))
            {
                ConnectionString.DisableOrEnableControls(this, false);
                ConnectionString.EnableControls(btnExit);
            }

            if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && ControlMgr.GroupName == "Purchase Admin")
            {
                btnPrint.Enabled = true;
            }
        }
        //tia edit end

        private void GRHeaderV2_Load(object sender, EventArgs e)
        {
            //tia edit
            tbxNameID.Enabled = true;
            tbxName.Enabled = true;
            tbxVOwnerID.Enabled = true;
            tbxVOwner.Enabled = true;

            tbxNameID.ReadOnly = true;
            tbxName.ReadOnly = true;
            tbxVOwnerID.ReadOnly = true;
            tbxVOwner.ReadOnly = true;

            tbxNameID.ContextMenu = vendid;
            tbxName.ContextMenu = vendid;
            tbxVOwnerID.ContextMenu = vendid;
            tbxVOwner.ContextMenu = vendid;

            //tia end
            btnApprove.Visible = false;
            tabPage1.AutoScroll = true;
            tabPage2.AutoScroll = true;
            tabPage1.Text = "Detail GR";
            tabPage2.Text = "Detail Reference";

            if (!cmbReferenceType.Items.Contains(vTransType))
                cmbReferenceType.Items.Add(vTransType);
            cmbReferenceType.SelectedItem = vTransType;

            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                GetDataHeader();
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                GetDataHeader();
                ModeBeforeEdit();
            }
            else if (Mode == "PopUp")
            {
                GetDataHeader();
                ModePopUp();
            }
        }

        private DataGridViewComboBoxCell cellValue(string query)
        {
            cell = null;
            Cmd = new SqlCommand(query, Conn);
            Dr2 = Cmd.ExecuteReader();
            cell = new DataGridViewComboBoxCell();
            cell.Items.Add("Select");
            while (Dr2.Read())
                cell.Items.Add(Dr2[0].ToString());
            Dr2.Close();
            return cell;
        }

        //type value : #1 string #2 decimal #3 int
        private void createLabel(Control textbox, Control lblName, Control location, string type)
        {
            if (validate == false)
            {
                label[count] = new Label();
            }
            if (type == "string")
            {
                if (textbox.Text == String.Empty || textbox.Text == "Select")
                {
                    textbox.BackColor = Color.LightGoldenrodYellow;

                    label[count].Text = "*";
                    label[count].ForeColor = Color.Red;
                    label[count].Width = 10;
                    label[count].Location = new System.Drawing.Point(lblName.Location.X - 9, lblName.Location.Y);
                    label[count].BringToFront();

                    location.Controls.Add(label[count]);
                    label[count].Visible = true;
                    flag = 'X';
                }
                else
                {
                    label[count].Visible = false;
                    textbox.BackColor = Color.Empty;
                }
            }
            else if (type == "decimal" || type == "int")
            {
                if (Convert.ToDecimal(textbox.Text) == 0)
                {
                    textbox.BackColor = Color.LightGoldenrodYellow;

                    label[count].Text = "*";
                    label[count].ForeColor = Color.Red;
                    label[count].Width = 10;
                    label[count].Location = new System.Drawing.Point(lblName.Location.X - 9, lblName.Location.Y);
                    label[count].BringToFront();

                    location.Controls.Add(label[count]);
                    label[count].Visible = true;
                    flag = 'X';
                }
                else
                {
                    label[count].Visible = false;
                    textbox.BackColor = Color.Empty;
                }
            }
            count++;
        }

        private char Validation()
        {
            flag = '\0'; msg = "";
            if (validate == false)
                label = new Label[tableColsName1.Length];

            string GRStatus = "";
            if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Query = "select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                GRStatus = Cmd.ExecuteScalar().ToString();
                Conn.Close();
            }

            //createLabel(cbRef, lRefType, gbMain, "string");
            createLabel(tbxRefID, lRefID, gbMain, "string");
            createLabel(tbxDelivNum, lDelivNum, gbMain, "string");
            if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
            {
                createLabel(tbxVType, lVType, gbMain, "string");
                createLabel(tbxVNumber, lVNum, gbMain, "string");
                createLabel(tbxDriverName, lDriverName, gbMain, "string");
            }
            createLabel(tbxWeight1, lWeight1, gbWeight1, "decimal");
            createLabel(tbxVOwnerID, lVOwner, gbMain, "string");
            createLabel(tbxVOwner, lVOwner, gbMain, "string");

            if (Mode != "New")
            {
                if ((GRStatus == "02" || GRStatus == "03") && ControlMgr.GroupName == "WB OPERATOR")
                    createLabel(tbxWeight2, lWeight2, gbWeight2, "decimal");
            }

            if (flag == 'X')
                msg += "Field * harus diisi!\r\n";
            if (dataGridView1.RowCount <= 0)
            {
                msg += "-Item di grid tidak boleh kosong.\n";
            }
            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    if (dataGridView1.Columns[i].Name == "InventSiteBlokID" || dataGridView1.Columns[i].Name == "ActionCodeStatus")
                    {
                        if (dataGridView1.Rows[j].Cells[i].Value.ToString() == "Select")
                        {
                            msg += "Baris " + Convert.ToInt32(j + 1) + " Kolom GR " + dataGridView1.Columns[i].HeaderText + " tidak boleh kosong!\n";
                        }
                    }
                }
            }
            for (int j = 0; j < dataGridView3.RowCount; j++)
            {
                for (int i = 0; i < dataGridView3.ColumnCount; i++)
                {
                    if (dataGridView3.Columns[i].Name == "InventSiteBlokID" || dataGridView3.Columns[i].Name == "ActionCodeStatus")
                    {
                        if (dataGridView3.Rows[j].Cells[i].Value.ToString() == "Select")
                        {
                            msg += "Baris " + Convert.ToInt32(j + 1) + " Kolom Non-GR " + dataGridView3.Columns[i].HeaderText + " tidak boleh kosong!\n";
                        }
                    }
                }
            }

            //CHECK QUANTITY PER ROW
            //for (int i = 0; i < dataGridView1.RowCount; i++)
            //{
            //    if (!(dataGridView1.Rows[i].Cells["RefTransID"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransID"].Value == null))
            //    {
            //        if (dataGridView1.Rows[i].Cells["Qty_SJ"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty_SJ"].Value == null)
            //            dataGridView1.Rows[i].Cells["Qty_SJ"].Value = "0";
            //        if (dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty"].Value == null)
            //            dataGridView1.Rows[i].Cells["Qty"].Value = "0";
            //        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value) > Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value))
            //        {
            //            msg += "Row " + Convert.ToInt32(i + 1) + " " + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + " (" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + ")" + " cannot more than " + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) + "\r\n";
            //        }
            //    }
            //}

            //CHECK QTY WITH RO REMAINING QTY
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                decimal qty = 0;
                decimal qtyRefRemaining = 0;
                decimal qtyRef = 0;
                string CurrentRowStatus = dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString();
                string QtyTarget = "Qty_SJ";
                string StatusTarget = "Bongkar";
                string OldStatusTarget = "01";
                string QtyTargetText = "Qty SJ";
                string QtyTargetRef = "Qty_SJ";
                if (Mode.ToUpper() == "EDIT" && ControlMgr.GroupName.ToUpper() == "KERANI" && GRStatus == "02")
                {
                    QtyTarget = "Qty_Actual";
                    StatusTarget = "Received";
                    OldStatusTarget = "05";
                    QtyTargetText = "Qty Actual";
                    QtyTargetRef = "Qty_Actual";
                }
                else if (Mode.ToUpper() == "EDIT" && ControlMgr.GroupName.ToUpper() == "KERANI" && GRStatus == "01")
                {
                    QtyTarget = "Qty_Actual";
                    StatusTarget = "Received";
                    OldStatusTarget = "01";
                    QtyTargetText = "Qty Actual";
                    QtyTargetRef = "Qty_SJ";
                }

                if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value) <= 0 && CurrentRowStatus == "Bongkar")
                {
                    msg += "Baris " + Convert.ToInt32(i + 1) + " GR Qty SJ tidak boleh lebih kecil atau sama dengan 0!\r\n";
                }
                else if ((Convert.ToDecimal(dataGridView1.Rows[i].Cells[QtyTarget].Value) > Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value)) && CurrentRowStatus == StatusTarget && cmbReferenceType.Text != "Nota Retur Jual")
                {
                    msg += "Baris " + Convert.ToInt32(i + 1) + " GR " + QtyTargetText + " tidak boleh lebih dari " + dataGridView1.Rows[i].Cells["Qty"].Value.ToString() + "!\r\n";
                }
                else if (CurrentRowStatus == StatusTarget)
                {
                    if (cmbReferenceType.Text == "Receipt Order")
                    {
                        Query = "SELECT * FROM [ReceiptOrderD] WHERE [ReceiptOrderId] = @ReceiptOrderId AND [SeqNo]=@SeqNo";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@ReceiptOrderId", dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString());
                            Cmd.Parameters.AddWithValue("@SeqNo", Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value));
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                qtyRefRemaining = Convert.ToDecimal(Dr["RemainingQty"]);
                            }
                            Dr.Close();
                        }

                        for (int x = 0; x < dataGridView1.RowCount; x++)
                        {
                            if (!(dataGridView1.Rows[i].Cells["RefTransID"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransID"].Value == null))
                            {
                                if (dataGridView1.Rows[x].Cells["ActionCodeStatus"].Value.ToString() == StatusTarget && (dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString() == dataGridView1.Rows[x].Cells["RefTransID"].Value.ToString()) && (dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() == dataGridView1.Rows[x].Cells["FullItemId"].Value.ToString()))
                                {
                                    qty += Convert.ToDecimal(dataGridView1.Rows[x].Cells[QtyTarget].Value);
                                }
                            }
                        }
                        if (!(Mode.ToUpper() == "NEW" && ControlMgr.GroupName.ToUpper() == "WB OPERATOR"))
                        {
                            Query = "SELECT " + QtyTargetRef + " FROM [GoodsReceivedD] WHERE [GoodsReceivedId] = @GoodsReceivedId AND [FullItemId]=@FullItemId ";
                            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            {
                                Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                                //Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", Convert.ToInt32(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value));
                                Cmd.Parameters.AddWithValue("@FullItemId", dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString());
                                Dr = Cmd.ExecuteReader();
                                while (Dr.Read())
                                {
                                    if (CurrentRowStatus == "Bongkar" || CurrentRowStatus == "Received")
                                    {
                                        qtyRef += Dr[0] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[0]);
                                    }
                                }
                                Dr.Close();
                            }
                        }
                    }
                    if ((qty > (qtyRef + qtyRefRemaining)) && dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == StatusTarget)
                    {
                        msg += "Baris " + Convert.ToInt32(i + 1) + " gabungan GR " + QtyTargetText + " untuk item " + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + " tidak boleh lebih dari " + (qtyRef + qtyRefRemaining) + "!\r\n";
                    }
                }

            }

            //program dibwh gk kepake? krn datagridview 3 pasti gk ad reference?
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                decimal qty = 0;
                if (!(dataGridView3.Rows[i].Cells["RefTransID"].Value == String.Empty || dataGridView3.Rows[i].Cells["RefTransID"].Value == null))
                {
                    if (Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value) > Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty"].Value))
                    {
                        msg += "Baris " + Convert.ToInt32(i + 1) + " Non-GR Qty_SJ tidak boleh lebih dari " + dataGridView3.Rows[i].Cells["Qty"].Value.ToString() + "!\r\n";
                    }
                }
            }

            ////CHECK TOTAL QTY PER FULLITEMID WITH RO REMAINING QTY
            //Conn = ConnectionString.GetConnection();
            //Cmd = new SqlCommand("select [ItemName], [FullItemId], [RemainingQty] from [dbo].[ReceiptOrderD] where [ReceiptOrderId] = '" + tbxRefID.Text + "'", Conn);
            //Dr = Cmd.ExecuteReader();
            //while (Dr.Read())
            //{
            //    //check total qty of item (from RO) cannot more than RO remaining qty
            //    decimal qty = 0;
            //    for (int i = 0; i < dataGridView1.RowCount; i++)
            //    {
            //        if (Dr["FullItemId"].ToString() == dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString())
            //        {
            //            if (dataGridView1.Rows[i].Cells["Qty_SJ"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty"].Value == null)
            //                dataGridView1.Rows[i].Cells["Qty_SJ"].Value = "0";
            //            qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value) + qty;
            //        }
            //    }
            //    if (Convert.ToDecimal(Dr["RemainingQty"]) < qty)
            //        msg += "Total quantity of item " + Dr["FullItemId"].ToString() + " cannot more than " + Dr["RemainingQty"].ToString() + "!\r\n";
            //}
            //Dr.Close();
            //Conn.Close();

            if (Mode == "Edit" && ControlMgr.GroupName != "WB OPERATOR")
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    //if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty_Actual"].Value == null)
                    //    dataGridView1.Rows[i].Cells["Qty_Actual"].Value = 0;

                    //decimal qty = 0;
                    //if (!(dataGridView1.Rows[i].Cells["RefTransID"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransID"].Value == null))
                    //{
                    //    if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) > Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value))
                    //    {
                    //        msg += "Row " + Convert.ToInt32(i + 1) + " Qty_Actual cannot more than " + dataGridView1.Rows[i].Cells["Qty"].Value.ToString() + "!\r\n";
                    //    }
                    //}

                    if (dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value == String.Empty || dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value == null)
                        dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value = 0;

                    if (dataGridView1.Rows[i].Cells["Quality"].Value == String.Empty || dataGridView1.Rows[i].Cells["Quality"].Value == null)
                        dataGridView1.Rows[i].Cells["Quality"].Value = "Select";

                    if (dataGridView1.Rows[i].Cells["Quality"].Value.ToString() == "Select" && dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() != "Reject")
                        msg += "Baris " + Convert.ToInt32(i + 1) + " GR Pilih Quality!\r\n";
                }

                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    if (dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value == String.Empty || dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value == null)
                        dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value = 0;

                    if (dataGridView3.Rows[i].Cells["Quality"].Value == String.Empty || dataGridView3.Rows[i].Cells["Quality"].Value == null)
                        dataGridView3.Rows[i].Cells["Quality"].Value = "Select";

                    if (dataGridView3.Rows[i].Cells["Quality"].Value.ToString() == "Select" && dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString() != "Reject")
                        msg += "Baris " + Convert.ToInt32(i + 1) + " Non-GR Pilih Quality!\r\n";
                }
            }

            if (ControlMgr.GroupName == "KERANI" && (GRStatus == "01" || GRStatus == "02"))
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Bongkar")
                    {
                        msg += "Baris " + Convert.ToInt32(i + 1) + " GR ActionCodeStatus tidak boleh Bongkar!\r\n";
                    }
                }
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    if (dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Bongkar")
                    {
                        msg += "Baris " + Convert.ToInt32(i + 1) + " Non-GR ActionCodeStatus tidak boleh Bongkar!\r\n";
                    }
                }
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (!((dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == null || dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty) && (dataGridView1.Rows[i].Cells["RefTransID"].Value == null || dataGridView1.Rows[i].Cells["RefTransID"].Value == String.Empty)))
                    {
                        Conn = ConnectionString.GetConnection();
                        //GET RO CURRENT REMAINING QTY
                        if (cmbReferenceType.Text == "Nota Transfer")
                        {
                            Query = "Select a.RemainingQty from [NotaTransferD] a LEFT JOIN [GoodsIssuedD] b ON a.[TransferNo] = b.[RefTransID] where b.[GoodsIssuedId] = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and b.[GoodsIssuedSeqNo] = '" + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value + "'; ";
                        }
                        else
                        {
                            Query = "Select RemainingQty from ReceiptOrderD where [ReceiptOrderId] = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and [SeqNo] = '" + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value + "'; ";
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        decimal remainingQty = 0;// = (Decimal)Cmd.ExecuteScalar();
                        if (Cmd.ExecuteScalar() != null)
                            remainingQty = (Decimal)Cmd.ExecuteScalar();

                        //GET GR OLD QTY_SJ 
                        decimal GoodsReceivedSeqNo = 0;
                        if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                        { }
                        else if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() == String.Empty)
                        { }
                        else
                            GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                        Query = "select Qty_SJ from GoodsReceivedD where GoodsReceivedID = '" + tbxGRNum.Text + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal oldQty_SJ = 0;
                        if (Cmd.ExecuteScalar() != null)
                            oldQty_SJ = (Decimal)Cmd.ExecuteScalar();

                        //Created by Thaddaeus, 22JUNE2018,BEGIN================ Qty 0 cannot have action code other than reject
                        if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() != "Reject" && Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) == 0)
                        {
                            msg += "Baris " + Convert.ToInt32(i + 1) + " tidak bisa di save kalau Qty Actual = 0 dengan action code selain Reject!\r\n";
                        }
                        //END==================================================================

                        //Created by Thaddaeus, 24MAY2018,BEGIN================ totalCombineQty
                        if (GoodsReceivedSeqNo == 0)
                        {
                            string NewFullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                            decimal SumQtyWithTheSameId = 0;
                            if ((Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value) < Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value)) && (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Received"))
                            {
                                msg += "Baris " + Convert.ToInt32(i + 1) + " Qty Actual tidak boleh lebih dari " + (dataGridView1.Rows[i].Cells["Qty_SJ"].Value.ToString()) + "!\r\n";
                            }
                            //else
                            //{
                            //    for (int x = 0; x < dataGridView1.RowCount; x++)
                            //    {
                            //        if ((dataGridView1.Rows[x].Cells["FullItemId"].Value.ToString()) == NewFullItemId)
                            //        {
                            //            SumQtyWithTheSameId += Convert.ToDecimal(dataGridView1.Rows[x].Cells["Qty_Actual"].Value);
                            //        }
                            //    }
                            //    if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value) < SumQtyWithTheSameId)
                            //    {
                            //        msg += "Rows with FullItemId" + NewFullItemId + " Combined Qty Actual cannot more than " + (dataGridView1.Rows[i].Cells["Qty_SJ"].Value.ToString()) + "!\r\n";
                            //    }
                            //}
                        }
                        //END==========================================
                        else if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) > (remainingQty + oldQty_SJ))
                        {
                            //validasi sudah dilakukan di method sebelumnya
                            //msg += "Row " + Convert.ToInt32(i + 1) + " Qty Actual cannot more than " + (remainingQty + oldQty_SJ) + "!\r\n";
                        }

                        Conn.Close();
                    }
                }
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    if (!((dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value == null || dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty) && (dataGridView3.Rows[i].Cells["RefTransID"].Value == null || dataGridView3.Rows[i].Cells["RefTransID"].Value == String.Empty)))
                    {
                        Conn = ConnectionString.GetConnection();
                        //GET RO CURRENT REMAINING QTY
                        Query = "Select RemainingQty from ReceiptOrderD where [ReceiptOrderId] = '" + dataGridView3.Rows[i].Cells["RefTransID"].Value + "' and [SeqNo] = '" + dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value + "'; ";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal remainingQty = 0;// = (Decimal)Cmd.ExecuteScalar();
                        if (Cmd.ExecuteScalar() != null)
                            remainingQty = (Decimal)Cmd.ExecuteScalar();

                        //GET GR OLD QTY_SJ 
                        Query = "select Qty_SJ from GoodsReceivedD where GoodsReceivedID = '" + tbxGRNum.Text + "' and GoodsReceivedSeqNo = '" + dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        decimal oldQty_SJ = (Decimal)Cmd.ExecuteScalar();

                        if (Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value) > (remainingQty + oldQty_SJ))
                        {
                            msg += "Baris " + Convert.ToInt32(i + 1) + " Qty Actual tidak boleh lebih dari " + (remainingQty + oldQty_SJ) + "!\r\n";
                        }
                        Conn.Close();
                    }
                }
            }

            if (msg != String.Empty)
            {
                MetroFramework.MetroMessageBox.Show(this, msg, "Form harus diisi!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flag = 'X';
            }
            count = 0;
            validate = true;
            return flag;
        }

        private void GetDataHeader()
        {
            int addRow = 0;
            if (Mode == "New")
            {
                //GET RO DATE
                Conn = ConnectionString.GetConnection();
                //PASS DATA TO HEADER FIELD
                if (cmbReferenceType.Text == "Receipt Order")
                {
                    //Query = "select ReceiptOrderDate, DeliveryDate, [VendId], [VendorName], [VendId], [VendorName], [VehicleType], [VehicleNumber], [DriverName], [Notes], [InventSiteID] from [dbo].[ReceiptOrderH] where [ReceiptOrderId] = '" + tbxRefID.Text + "'";
                    Query = "select a.ReceiptOrderDate, a.DeliveryDate, a.[VendId], a.[VendorName], a.[VendorEkspedisi], b.VendName, a.[VehicleType], a.[VehicleNumber], a.[DriverName], a.[Notes], a.[InventSiteID] from [dbo].[ReceiptOrderH] a left join VendTable b on a.VendorEkspedisi = b.VendId where a.[ReceiptOrderId] = '" + tbxRefID.Text + "'";
                }
                else if (cmbReferenceType.Text == "Nota Transfer")
                {
                    //edited , by Thaddaeus 15May2018, begin
                    //Query = "select TransferDate, null, InventSiteFrom, InventSiteFromName, null, null, VehicleType, VehicleNo, DriverName, Notes, InventSiteFrom from [dbo].[NotaTransferH] where [TransferNo] = '" + tbxRefID.Text + "'";
                    Query = "select a.[RefTransDate], null, a.[InventSiteID], b.[InventSiteName], a.VehicleOwnerID, a.VehicleOwnerName, a.VehicleType, a.[VehicleNumber], a.DriverName, a.Notes, a.AccountNum from [dbo].[GoodsIssuedH] a LEFT JOIN [dbo].[InventSite] b ON a.[InventSiteID]=b.[InventSiteID] where a.[GoodsIssuedId] = '" + tbxRefId2.Text + "'";
                    //end
                }
                else if (cmbReferenceType.Text == "Nota Retur Jual")
                {
                    Query = "select NRJDate, null, CustId, CustName, null, null, null, null, null, Notes, [SiteId] as InventSiteID from [dbo].[NotaReturJualH] where [NRJId] = '" + tbxRefID.Text + "'";
                }
                else if (cmbReferenceType.Text == "Nota Retur Beli")
                {
                    Query = "select [NRBDate], null, [VendId], [VendName], null, null, null, null, null, Notes, [SiteId] as InventSiteID from [dbo].[NotaReturBeliH] where [NRBId] = '" + tbxRefID.Text + "'";
                }
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    dtRef.Value = Convert.ToDateTime(Dr[0]);
                    if (Dr[1].ToString() != String.Empty)
                        dtExpectedDate.Value = Convert.ToDateTime(Dr[1]);
                    tbxNameID.Text = Dr[2].ToString();
                    tbxName.Text = Dr[3].ToString();
                    tbxVOwnerID.Text = Dr[4].ToString();
                    tbxVOwner.Text = Dr[5].ToString();
                    tbxVType.Text = Dr[6].ToString();
                    tbxVNumber.Text = Dr[7].ToString();
                    tbxDriverName.Text = Dr[8].ToString();
                    tbxNotes.Text = Dr[9].ToString();
                    txtInventSiteID.Text = Dr[10].ToString();
                    Cmd = new SqlCommand("select InventSiteName, Lokasi, SiteType from InventSite where InventSiteId = '" + Dr[10] + "'", Conn);
                    SqlDataReader Dr2 = Cmd.ExecuteReader();
                    while (Dr2.Read())
                    {
                        txtWarehouse.Text = Dr2["InventSiteName"].ToString();
                        txtSiteType.Text = Dr2["SiteType"].ToString();
                    }
                    Dr2.Close();
                }
                Dr.Close();

                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ColumnCount = tableColsName1.Length;
                //GENERATE COLUMN HEADER NAME
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].Name = tableColsName1[i];
                    dataGridView1.Columns[i].HeaderText = tableCols1Header1[i];
                }

                switch (cmbReferenceType.Text)
                {
                    case "Receipt Order":
                        Query = "select [SeqNo], [ReceiptOrderId], [GroupId], [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [Qty], [RemainingQty], [Unit], [Ratio], [InventSiteID] from ReceiptOrderD where [ReceiptOrderId] = '" + tbxRefID.Text + "' and RemainingQty != 0";
                        break;
                    case "Nota Transfer":
                        //edited by Thaddaeus, 15May2018,begin
                        Query = "select [GoodsIssuedSeqNo] as 'SeqNo', [GoodsIssuedId] as 'ReceiptOrderId', [GroupId], [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [Qty], [Remaining_Qty] as 'RemainingQty', [Unit], [Ratio], [InventSiteID] from [GoodsIssuedD] where [GoodsIssuedId] = '" + tbxRefId2.Text + "'";
                        //end=================================
                        break;
                    case "Nota Retur Jual":
                        Query = "select [SeqNo], [NRJId] 'ReceiptOrderId', [GroupId], SubGroupId as [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [UoM_Qty] 'Qty', [RemainingQty], [UoM_Unit] 'Unit', [Ratio], [InventSiteId] 'InventSiteID' from NotaReturJual_Dtl where [NRJId] = '" + tbxRefID.Text + "'";
                        break;
                    case "Nota Retur Beli":
                        Query = "select [SeqNo], [NRBId] 'ReceiptOrderId', [GroupId], [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [UoM_Qty] 'Qty', [Remaining_Qty_RO] as 'RemainingQty', [UoM_Unit] 'Unit', [Ratio], [InventSiteId] 'InventSiteID' from [NotaReturBeli_Dtl] where [NRBId] = '" + tbxRefID.Text + "'";
                        break;
                }
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                addRow = 0;
                decimal TotalBerat = 0;
                while (Dr.Read())
                {
                    dataGridView1.Rows.Add(1);
                    for (int i = 0; i < tableColsName1.Length; i++)
                    {
                        //if (!(tableColsName1[i] == "GoodsReceivedSeqNo" || tableColsName1[i] == "Qty_Actual" || tableColsName1[i] == "Remaining_Qty" || tableColsName1[i] == "TotalBerat" || tableColsName1[i] == "TotalBerat_Actual" || tableColsName1[i] == "Notes"))

                        if (!(tableColsName1[i] == "GoodsReceivedSeqNo" || tableColsName1[i] == "Qty_Actual" || tableColsName1[i] == "Remaining_Qty" || tableColsName1[i] == "TotalBerat_Actual" || tableColsName1[i] == "Notes"))
                        {
                            if (tableColsName1[i] == "No")
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = dataGridView1.RowCount;
                            else if (tableColsName1[i] == "RefTransID")
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = Dr["ReceiptOrderId"];
                            else if (tableColsName1[i] == "RefTransSeqNo")
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = Dr["SeqNo"];
                            else if (tableColsName1[i] == "Qty" || tableColsName1[i] == "Qty_SJ")
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = Dr["RemainingQty"];
                            else if (tableColsName1[i] == "TotalBerat")
                            {
                                // dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = Dr["TotalBerat"];
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = Convert.ToDecimal(Dr["RemainingQty"].ToString()) * Convert.ToDecimal(Dr["Ratio"].ToString());
                            }
                            else if (tableColsName1[i] == "UnitAlt")
                            {
                                string UnitAlt = "";
                                string Query2 = " SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + Dr["FullItemId"] + "'";
                                SqlCommand Cmd2 = new SqlCommand(Query2, Conn);
                                UnitAlt = Convert.ToString(Cmd2.ExecuteScalar());
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = UnitAlt;
                            }
                            else if (tableColsName1[i] == "InventSiteBlokID")
                            {
                                cellValue("select InventSiteBlokID from InventSiteBlok where InventSiteID = '" + Dr["InventSiteID"] + "'");
                                //cellValue("select distinct c.InventSiteBlokID from [ReceiptOrderD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.ReceiptOrderId = '" + tbxRefID.Text + "'");
                                cell.Value = "Select";
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                            }
                            else if (tableColsName1[i] == "ActionCodeStatus")
                            {
                                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = "Received";
                                }
                                //edited by Thaddaeus 14 Sept 2018
                                else if (cmbReferenceType.Text == "Nota Retur Jual")
                                {
                                    cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01','02', '03')");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                }
                                //end==============================
                                else
                                {
                                    cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03')");
                                    cell.Value = "Select";
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                }
                            }
                            else if (tableColsName1[i] == "Quality")
                            {
                                cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                                cell.Value = "Select";
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                            }
                            else if (tableColsName1[i] == "Ratio_Actual" || tableColsName1[i] == "Ratio")
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = Dr["Ratio"];
                            else
                                dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = Dr[tableColsName1[i]];
                        }
                        else if (tableColsName1[i] == "Qty_Actual" && txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                        {
                            dataGridView1.Rows[addRow].Cells[tableColsName1[i]].Value = dataGridView1.Rows[addRow].Cells["Qty_SJ"].Value;
                        }
                    }
                    TotalBerat += Convert.ToDecimal(dataGridView1.Rows[addRow].Cells["TotalBerat"].Value);
                    addRow++;
                }
                Dr.Close();
                Conn.Close();
                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                {
                    tbxWeight1.Text = TotalBerat.ToString();
                    tbxWeight2.Text = TotalBerat.ToString();
                }
                ModeNew();
            }
            else if (Mode != "New")
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select * from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (Dr["ExpectedDate"].ToString() == String.Empty)
                        dtExpectedDate.Value = Convert.ToDateTime("01/01/1753");
                    else
                        dtExpectedDate.Value = Convert.ToDateTime(Dr["ExpectedDate"]);
                    dtGR.Value = Convert.ToDateTime(Dr["GoodsReceivedDate"]);
                    dtRef.Value = Convert.ToDateTime(Dr["RefTransDate"]);
                    cmbReferenceType.Text = Dr["RefTransType"].ToString();
                    //hendry 
                    cmbReferenceType.SelectedItem = Dr["RefTransType"].ToString();

                    dtDO.Value = Convert.ToDateTime(Dr["SJDate"]);
                    tbxRefID.Text = Dr["RefTransID"].ToString();
                    tbxNameID.Text = Dr["VendId"].ToString();
                    tbxName.Text = Dr["VendorName"].ToString();
                    tbxDelivNum.Text = Dr["SJNumber"].ToString();
                    tbxVNumber.Text = Dr["VehicleNumber"].ToString();
                    tbxVType.Text = Dr["VehicleType"].ToString();
                    tbxDriverName.Text = Dr["DriverName"].ToString();
                    dtWeight1.Value = Convert.ToDateTime(Dr["Timbang1Date"]);
                    tbxWeight1.Text = Convert.ToDecimal(Dr["Timbang1Weight"]).ToString("N2");
                    dtWeight2.Value = Convert.ToDateTime(Dr["Timbang2Date"]) == Convert.ToDateTime("1/1/1753") ? DateTime.Now : Convert.ToDateTime(Dr["Timbang2Date"]);
                    tbxWeight2.Text = Dr["Timbang2Weight"] == (object)DBNull.Value ? "0.00" : Convert.ToDecimal(Dr["Timbang2Weight"]).ToString("N2");
                    txtInventSiteID.Text = Dr["SiteID"].ToString();
                    txtWarehouse.Text = Dr["SiteName"].ToString();
                    txtSiteType.Text = Dr["SiteType"].ToString();
                    tbxNotes.Text = Dr["Notes"].ToString();
                    tbxVOwnerID.Text = Dr["VendId"].ToString();
                    tbxVOwner.Text = Dr["VendorName"].ToString();
                    if (Dr["StatusWeight1"].ToString() == "Manual")
                        cbWeight1.Checked = true;
                    else
                        cbWeight1.Checked = false;
                    if (Dr["StatusWeight2"].ToString() == "Manual")
                        cbWeight2.Checked = true;
                    else
                        cbWeight2.Checked = false;

                    //added by Thaddaeus, 15May2018, begin
                    if (cmbReferenceType.Text == "Nota Transfer")
                    {
                        tbxRefID.Text = Dr["RefTransID"].ToString();
                        tbxRefID.Size = new System.Drawing.Size(96, 20);
                        tbxRefID.Location = new Point(139, 122);
                        tbxRefId2.Text = Dr["RefTrans2Id"].ToString();
                        tbxRefId2.Location = new Point(239, 122);
                        tbxRefId2.Size = new System.Drawing.Size(100, 20);
                    }
                    //end
                }
                Dr.Close();

                dataGridView1.Rows.Clear();
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.ColumnCount = tableColsName1.Length;
                //GENERATE COLUMN HEADER NAME
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].Name = tableColsName1[i];
                    dataGridView1.Columns[i].HeaderText = tableCols1Header1[i];
                }

                Query = "select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                string GRStats = Cmd.ExecuteScalar().ToString();

                Query = "Select a.*, b.Deskripsi 'DeskripsiActionCodeStatus', c.Deskripsi 'DeskripsiQuality' from [GoodsReceivedD] as a left join [TransStatusTable] as b on a.ActionCodeStatus = b.StatusCode left join InventQuality as c on a.Quality = c.QualityID Where [GoodsReceivedId] = '" + tbxGRNum.Text + "' and b.TransCode = 'GRD' and (a.RefTransID is not null or a.RefTransId != '')";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                //int addRow = 0;
                while (Dr.Read())
                {
                    //if (!(ControlMgr.GroupName == "KERANI" && Dr["ActionCodeStatus"].ToString() == "03" && Mode == "Edit")) //HIDE REJECT STATUS WHEN KERANI EDIT
                    if (!(ControlMgr.GroupName == "KERANI" && Dr["ActionCodeStatus"].ToString() == "XX" && Mode == "Edit"))
                    {
                        dataGridView1.Rows.Add(1);
                        for (int i = 0; i < tableColsName1.Length; i++)
                        {
                            if (tableColsName1[i] == "No")
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = dataGridView1.RowCount;
                            else if (tableColsName1[i] == "ActionCodeStatus")
                            {
                                if (Mode == "Edit" && (!(ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))))
                                {
                                    //edited by Thaddaeus 14 Sept 2018
                                    if (cmbReferenceType.Text == "Nota Retur Jual" && ControlMgr.GroupName == "KERANI")
                                    {
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('03','02', '05')");
                                    }
                                    else if (cmbReferenceType.Text == "Nota Retur Jual" && ControlMgr.GroupName != "KERANI")
                                    {
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('03','02', '01')");
                                    }
                                    //end================================
                                    else if (ControlMgr.GroupName == "KERANI" && Dr["RefTransSeqNo"].ToString() == String.Empty)
                                    {
                                        //cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03', '09')");
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03', '05')");
                                    }
                                    else if (ControlMgr.GroupName != "KERANI" && Dr["RefTransSeqNo"].ToString() == String.Empty)
                                    {
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03')");
                                    }
                                    else if (ControlMgr.GroupName == "KERANI")
                                        //cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03', '05', '09')");
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03', '05')");
                                    else
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03')");

                                    if (Dr["DeskripsiActionCodeStatus"] != null && (Dr["DeskripsiActionCodeStatus"]).ToString().ToUpper() != "BONGKAR")
                                    {
                                        cell.Value = Dr["DeskripsiActionCodeStatus"].ToString();
                                    }
                                    else
                                    {
                                        cell.Value = "Select";
                                    }
                                    //edited by Thaddaeus, 24 Sept 2018
                                    //if (Convert.ToDecimal(dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["GoodsReceivedSeqNo"].Value) % 1 > 0)
                                    //{
                                    //    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = "Parked Tolerance - Need Action";
                                    //}
                                    //else
                                    //{
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                    //}
                                    //end===============================
                                }
                                else
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr["DeskripsiActionCodeStatus"].ToString();
                                }
                            }
                            else if (tableColsName1[i] == "Quality")
                            {
                                if (Mode == "Edit" && (!(ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))))
                                {
                                    cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                                    cell.Value = "Select";
                                    if (!(Dr[tableColsName1[i]] == null || Dr[tableColsName1[i]].ToString() == String.Empty))
                                    {
                                        Cmd = new SqlCommand("select Deskripsi from InventQuality where QualityID = '" + Dr[tableColsName1[i]].ToString() + "'", Conn);
                                        cell.Value = Cmd.ExecuteScalar().ToString();
                                    }
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                }
                                else
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr["DeskripsiQuality"].ToString();
                                }
                            }
                            else if (tableColsName1[i] == "InventSiteBlokID")
                            {
                                //Login kerani stats 01 && 02 
                                //Login WB stats 02
                                if ((Mode == "Edit" && (!(ControlMgr.GroupName == "KERANI" && (GRStats == "01" || GRStats == "02"))) && (Mode == "Edit" && (!(ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))))))
                                {
                                    cellValue("select InventSiteBlokID from InventSiteBlok where InventSiteID = '" + Dr["InventSiteID"] + "'");
                                    //cellValue("select distinct c.InventSiteBlokID from [ReceiptOrderD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.ReceiptOrderId = '" + tbxRefID.Text + "'");
                                    if (Dr[tableColsName1[i]] != null)
                                        cell.Value = Dr[tableColsName1[i]].ToString();
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                }
                                else
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr["InventSiteBlokID"].ToString();
                                }
                            }
                            else if (tableColsName1[i] == "TotalBerat")
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Convert.ToDecimal(Dr["Qty_SJ"].ToString()) * Convert.ToDecimal(Dr["Ratio"].ToString());
                            else if (tableColsName1[i] == "UnitAlt")
                            {
                                string UnitAlt = "";
                                string Query2 = " SELECT UoMAlt FROM InventTable WHERE FullItemID = '" + Dr["FullItemId"] + "'";
                                SqlCommand Cmd2 = new SqlCommand(Query2, Conn);
                                UnitAlt = Convert.ToString(Cmd2.ExecuteScalar());
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = UnitAlt;
                            }
                            //edited by Thaddaeus, 15MAY2018, Begin==============
                            else if (tableColsName1[i] == "RefTransID")
                            {
                                if (cmbReferenceType.Text == "Nota Transfer")
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["RefTransID"].Value = Dr["RefTrans2Id"].ToString();
                                }
                                else
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr[tableColsName1[i]].ToString();
                                }
                            }
                            else if (tableColsName1[i] == "RefTransSeqNo")
                            {
                                if (cmbReferenceType.Text == "Nota Transfer")
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["RefTransSeqNo"].Value = Dr["GoodsReceivedSeqNo"].ToString();
                                }
                                else
                                {
                                    dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr[tableColsName1[i]].ToString();
                                }
                            }
                            //end=====================================
                            else
                                dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr[tableColsName1[i]].ToString();
                        }
                    }
                }
                Dr.Close();
                if (dataGridView1.Rows.Count > 0 && tbxGRNum.Text != "")
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Reject")
                        {
                            Query = "SELECT b.[GroupName] FROM [dbo].[GoodsReceivedD] a LEFT JOIN [dbo].[sysUserGroup] b ON a.UpdatedBy = b.[UserID] WHERE a.[GoodsReceivedId] = '" + tbxGRNum.Text + "' AND a.[GoodsReceivedSeqNo] = " + dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value + "";
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                if (ControlMgr.GroupName == "KERANI" && Mode == "Edit" && Cmd.ExecuteScalar().ToString() != "KERANI")
                                {
                                    dataGridView1.Rows[i].ReadOnly = true;
                                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightSalmon;
                                }
                            }
                        }
                    }
                }

                //*********************************************************************************
                dataGridView3.Rows.Clear();
                dataGridView3.AllowUserToAddRows = false;
                dataGridView3.ColumnCount = tableColsName1.Length;
                //GENERATE COLUMN HEADER NAME
                for (int i = 0; i < dataGridView3.ColumnCount; i++)
                {
                    dataGridView3.Columns[i].Name = tableColsName1[i];
                    dataGridView3.Columns[i].HeaderText = tableCols1Header1[i];
                }

                Query = "Select a.*, b.Deskripsi 'DeskripsiActionCodeStatus', c.Deskripsi 'DeskripsiQuality' from [GoodsReceivedD] as a left join [TransStatusTable] as b on a.ActionCodeStatus = b.StatusCode left join InventQuality as c on a.Quality = c.QualityID Where [GoodsReceivedId] = '" + tbxGRNum.Text + "' and b.TransCode = 'GRD' and (a.RefTransID is null or a.RefTransId = '')";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                //int addRow = 0;
                while (Dr.Read())
                {
                    //if (!(ControlMgr.GroupName == "KERANI" && Dr["ActionCodeStatus"].ToString() == "03" && Mode == "Edit"))//HIDE REJECT STATUS WHEN KERANI EDIT
                    if (!(ControlMgr.GroupName == "KERANI" && Dr["ActionCodeStatus"].ToString() == "XX" && Mode == "Edit"))
                    {
                        dataGridView3.Rows.Add(1);
                        for (int i = 0; i < tableColsName1.Length; i++)
                        {
                            if (tableColsName1[i] == "No")
                                dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]].Value = dataGridView3.RowCount;
                            else if (tableColsName1[i] == "ActionCodeStatus")
                            {
                                if (Mode == "Edit" && (!(ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))))
                                {
                                    if (ControlMgr.GroupName == "KERANI" && Dr["RefTransSeqNo"].ToString() == String.Empty)
                                    {
                                        //cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03', '09')");
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03')"); //hendry
                                    }
                                    else if (ControlMgr.GroupName != "KERANI" && Dr["RefTransSeqNo"].ToString() == String.Empty)
                                    {
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03')");
                                    }
                                    else if (ControlMgr.GroupName == "KERANI")
                                        //cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03', '05', '09')");                                         
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03', '05')");
                                    else
                                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03')");

                                    if (Dr["DeskripsiActionCodeStatus"] != null)
                                        cell.Value = Dr["DeskripsiActionCodeStatus"].ToString();
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                }
                                else
                                {
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr["DeskripsiActionCodeStatus"].ToString();
                                }
                            }
                            else if (tableColsName1[i] == "Quality")
                            {
                                if (Mode == "Edit" && (!(ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))))
                                {
                                    cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                                    cell.Value = "Select";
                                    if (!(Dr[tableColsName1[i]] == null || Dr[tableColsName1[i]].ToString() == String.Empty))
                                    {
                                        Cmd = new SqlCommand("select Deskripsi from InventQuality where QualityID = '" + Dr[tableColsName1[i]].ToString() + "'", Conn);
                                        cell.Value = Cmd.ExecuteScalar().ToString();
                                    }
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                }
                                else
                                {
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr["DeskripsiQuality"].ToString();
                                }
                            }
                            else if (tableColsName1[i] == "InventSiteBlokID")
                            {
                                //Login kerani stats 01 && 02 
                                //Login WB stats 02
                                if ((Mode == "Edit" && (!(ControlMgr.GroupName == "KERANI" && (GRStats == "01" || GRStats == "02"))) && (Mode == "Edit" && (!(ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))))))
                                {
                                    cellValue("select InventSiteBlokID from InventSiteBlok where InventSiteID = '" + Dr["InventSiteID"] + "'");
                                    //cellValue("select distinct c.InventSiteBlokID from [ReceiptOrderD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.ReceiptOrderId = '" + tbxRefID.Text + "'");
                                    if (Dr[tableColsName1[i]] != null)
                                        cell.Value = Dr[tableColsName1[i]].ToString();
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]] = cell;
                                }
                                else
                                {
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr["InventSiteBlokID"].ToString();
                                }
                            }
                            else
                                dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells[tableColsName1[i]].Value = Dr[tableColsName1[i]].ToString();
                        }
                    }
                }
                Dr.Close();
                if (dataGridView3.Rows.Count > 0 && tbxGRNum.Text != "")
                {
                    for (int i = 0; i < dataGridView3.Rows.Count; i++)
                    {
                        if (dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Reject")
                        {
                            Query = "SELECT b.[GroupName] FROM [dbo].[GoodsReceivedD] a LEFT JOIN [dbo].[sysUserGroup] b ON a.UpdatedBy = b.[UserID] WHERE a.[GoodsReceivedId] = '" + tbxGRNum.Text + "' AND a.[GoodsReceivedSeqNo] = " + dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value + "";
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                if (ControlMgr.GroupName == "KERANI" && Mode == "Edit" && Cmd.ExecuteScalar().ToString() != "KERANI")
                                {
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].ReadOnly = true;
                                    dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].DefaultCellStyle.BackColor = Color.LightSalmon;
                                }
                            }
                        }
                    }
                }
                Conn.Close();
            }
            Conn = ConnectionString.GetConnection();
            dataGridView2.Rows.Clear();
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.ColumnCount = tableColsName2.Length;
            //GENERATE COLUMN HEADER NAME
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].Name = tableColsName2[i];
                dataGridView2.Columns[i].HeaderText = tableCols1Header2[i];
            }

            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":
                    Query = "select [SeqNo], [ReceiptOrderId], [GroupId], [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [Qty], [RemainingQty], [Unit], [Ratio], [InventSiteID] from ReceiptOrderD where [ReceiptOrderId] = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Transfer":
                    Query = "select [SeqNo], [TransferNo] 'ReceiptOrderId', [GroupId], [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [Qty], [RemainingQty], [UoM] 'Unit', [Ratio], [InventSite] 'InventSiteID' from NotaTransferD where [TransferNo] = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Retur Jual":
                    Query = "select [SeqNo], [NRJId] 'ReceiptOrderId', [GroupId], [SubGroupId] as 'SubGroup1Id', [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [UoM_Qty] 'Qty', [RemainingQty], [UoM_Unit] 'Unit', [Ratio], [InventSiteId] 'InventSiteID' from NotaReturJual_Dtl where [NRJId] = '" + tbxRefID.Text + "'";
                    break;
                case "Nota Retur Beli":
                    Query = "select [SeqNo], [NRBId] 'ReceiptOrderId', [GroupId], SubGroup1Id, [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [UoM_Qty] 'Qty', [Remaining_Qty_RO] as 'RemainingQty', [UoM_Unit] 'Unit', [Ratio], [InventSiteId] 'InventSiteID' from [NotaReturBeli_Dtl] where [NRBId] = '" + tbxRefID.Text + "'";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            addRow = 0;
            while (Dr.Read())
            {
                dataGridView2.Rows.Add(1);
                for (int i = 0; i < tableColsName2.Length; i++)
                {
                    if (tableColsName2[i] == "No")
                        dataGridView2.Rows[addRow].Cells[tableColsName2[i]].Value = dataGridView2.RowCount;
                    else if (tableColsName2[i] == "RefTransID")
                        dataGridView2.Rows[addRow].Cells[tableColsName2[i]].Value = Dr["ReceiptOrderId"];
                    else if (tableColsName2[i] == "RefTransSeqNo")
                        dataGridView2.Rows[addRow].Cells[tableColsName2[i]].Value = Dr["SeqNo"];
                    else if (tableColsName2[i] == "Remaining_Qty")
                        dataGridView2.Rows[addRow].Cells[tableColsName2[i]].Value = Dr["RemainingQty"];
                    else
                        dataGridView2.Rows[addRow].Cells[tableColsName2[i]].Value = Dr[tableColsName2[i]];
                }
                addRow++;
            }
            Dr.Close();
            Conn.Close();
        }

        private void btnSRefID_Click(object sender, EventArgs e)
        {
            Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();
            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":

                    //ControlMgr.TblName = "ReceiptOrderH";
                    //Methods.ControlMgr.tmpWhere = "WHERE ReceiptOrderStatus != '03'";
                    //ControlMgr.tmpSort = "ORDER BY ReceiptOrderDate DESC";

                    //FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    //FrmSearch.Text = "Search Transaction";
                    //FrmSearch.ShowDialog();

                    //if (ControlMgr.Kode != "")
                    //{
                    //    tbxRefID.Text = ControlMgr.Kode;
                    //    GetDataHeader();
                    //    ModeNew();
                    //}

                    //ControlMgr.TblName = "";
                    //ControlMgr.tmpSort = "";
                    //Methods.ControlMgr.tmpWhere = "";
                    //ControlMgr.Kode = "";

                    SearchQueryV1 tmpSearch = new SearchQueryV1();
                    tmpSearch.PrimaryKey = "RO_No";
                    tmpSearch.Order = "CreatedDate desc";
                    tmpSearch.Table = "[dbo].[ReceiptOrderH]";
                    tmpSearch.QuerySearch = "select a.ReceiptOrderId 'RO_No', convert(varchar, a.ReceiptOrderDate, 3) 'RO_Date', a.PurchaseOrderId 'PO_No', VendorName, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.ReceiptOrderStatus, b.RemainingQty from ReceiptOrderH a left join (Select ReceiptOrderId,sum(RemainingQty) as RemainingQty from [ReceiptOrderD] group by ReceiptOrderId) b on a.ReceiptOrderId = b.ReceiptOrderId   LEFT JOIN [ISBS-NEW4].[dbo].[InventSite] c ON a.InventSiteID = c.InventSiteID WHERE c.SiteType = 'Physical Site' ";
                    tmpSearch.FilterText = new string[] { "RO_No", "PO_No", "VendorName" };
                    tmpSearch.Mask = new string[] { "RO No", "RO Date", "PO No", "Vendor" };
                    //tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
                    tmpSearch.Select = new string[] { "RO_No" };
                    tmpSearch.Hide = new string[] { "ReceiptOrderStatus", "RemainingQty" };
                    tmpSearch.WherePlus = "and  ReceiptOrderStatus != '03' and RemainingQty>0 ";
                    //tmpSearch.SetSchemaTable(SchemaName, TableName);
                    if (ControlMgr.GroupName == "Purchase Admin")
                    {
                        tmpSearch.QuerySearch = "select a.ReceiptOrderId 'RO_No', a.ReceiptOrderDate 'RO_Date', a.PurchaseOrderId 'PO_No', VendorName, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a.ReceiptOrderStatus, b.RemainingQty from ReceiptOrderH a left join (Select ReceiptOrderId,sum(RemainingQty) as RemainingQty from [ReceiptOrderD] group by ReceiptOrderId) b on a.ReceiptOrderId = b.ReceiptOrderId   LEFT JOIN [ISBS-NEW4].[dbo].[InventSite] c ON a.InventSiteID = c.InventSiteID WHERE c.SiteType = 'Virtual Site' ";
                    }
                    tmpSearch.ShowDialog();
                    if (ConnectionString.Kodes != null)
                    {
                        tbxRefID.Text = ConnectionString.Kodes[0];
                        GetDataHeader();
                        ModeNew();
                        ConnectionString.Kodes = null;
                    }
                    break;
                case "Nota Transfer":
                    //edited by Thaddaeus, 15May2018, begin===================================================
                    //ControlMgr.TblName = "NotaTransferH";
                    //ControlMgr.tmpSort = "ORDER BY TransferDate DESC";

                    //FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    //FrmSearch.Text = "Search Transaction";
                    //FrmSearch.ShowDialog();

                    //if (ControlMgr.Kode != "")
                    //{
                    //    tbxRefID.Text = ControlMgr.Kode;
                    //    GetDataHeader();
                    //    ModeNew();
                    //}

                    //ControlMgr.TblName = "";
                    //ControlMgr.tmpSort = "";
                    //Methods.ControlMgr.tmpWhere = "";
                    //ControlMgr.Kode = "";

                    SearchQueryV1 tmpSearch1 = new SearchQueryV1();
                    tmpSearch1.PrimaryKey = "GoodsIssuedId";
                    tmpSearch1.Order = "CreatedDate asc";
                    tmpSearch1.Table = "[dbo].[GoodsIssuedH]";
                    tmpSearch1.QuerySearch = "select a.[RefTransID] as 'NT_No',a.GoodsIssuedId,  a.GoodsIssuedDate 'GI Date', a.InventSiteID, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, b.[TransStatus] from GoodsIssuedH a LEFT JOIN [dbo].[NotaTransferH] b ON a.[RefTransID]=b.[TransferNo] ";
                    tmpSearch1.FilterText = new string[] { "NT_No", "GoodsIssuedId", "InventSiteID" };
                    //tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
                    tmpSearch1.Select = new string[] { "NT_No", "GoodsIssuedId" };
                    tmpSearch1.Hide = new string[] { "TransStatus" };
                    tmpSearch1.WherePlus = "and  TransStatus IN ('08','09')  ";
                    //tmpSearch.SetSchemaTable(SchemaName, TableName);
                    tmpSearch1.ShowDialog();
                    if (ConnectionString.Kodes != null)
                    {
                        tbxRefID.Text = ConnectionString.Kodes[0];
                        tbxRefID.Size = new System.Drawing.Size(96, 20);
                        tbxRefID.Location = new Point(139, 122);
                        tbxRefId2.Text = ConnectionString.Kodes[1];
                        tbxRefId2.Location = new Point(239, 122);
                        tbxRefId2.Size = new System.Drawing.Size(100, 20);

                        GetDataHeader();
                        ModeNew();
                        ConnectionString.Kodes = null;
                    }
                    //end=====================================================================================================
                    break;
                case "Nota Retur Beli":
                    SearchQueryV1 tmpSearch2 = new SearchQueryV1();
                    tmpSearch2.PrimaryKey = "NRB_No";
                    tmpSearch2.Order = "CreatedDate desc";
                    tmpSearch2.Table = "[dbo].[NotaReturBeliH]";
                    tmpSearch2.QuerySearch = "select a.[NRBId] 'NRB_No', convert(varchar, a.[NRBDate], 3) 'NRB_Date', a.GoodsReceivedId 'Previous_GR',d.[GoodsIssuedId],a.[VendName] as 'VendorName', a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, b.RemainingQty from [NotaReturBeliH] a left join (Select [NRBId],sum([Remaining_Qty_RO]) as RemainingQty from [NotaReturBeli_Dtl] group by NRBId) b on a.NRBId = b.NRBId   LEFT JOIN [ISBS-NEW4].[dbo].[InventSite] c ON a.[SiteId] = c.InventSiteID LEFT JOIN [GoodsIssuedH] d ON a.NRBId = d.[RefTransID] WHERE c.SiteType = 'Physical Site' AND d.StatusCode IN ('03','06') ";
                    tmpSearch2.FilterText = new string[] { "NRB_No", "NRB_Date", "Previous_GR", "GoodsIssuedId", "VendorName" };
                    tmpSearch2.Mask = new string[] { "NRB No", "NRB Date", "Previous GR", "GoodsIssuedId", "Vendor" };
                    //tmpSearch.FilterDate = new string[] { "CreatedDate", "UpdatedDate" };
                    tmpSearch2.Select = new string[] { "NRB_No" };
                    tmpSearch2.Hide = new string[] { "RemainingQty" };
                    tmpSearch2.WherePlus = "and  RemainingQty > 0 ";
                    //tmpSearch.SetSchemaTable(SchemaName, TableName);
                    if (ControlMgr.GroupName == "Purchase Admin")
                    {
                        tmpSearch2.QuerySearch = "select a.[NRBId] 'NRB_No', convert(varchar, a.[NRBDate], 3) 'NRB_Date', a.GoodsReceivedId 'Previous_GR',a.[VendName] as 'VendorName', a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, b.RemainingQty from [NotaReturBeliH] a left join (Select [NRBId],sum([Remaining_Qty_RO]) as RemainingQty from [NotaReturBeli_Dtl] group by NRBId) b on a.NRBId = b.NRBId   LEFT JOIN [ISBS-NEW4].[dbo].[InventSite] c ON a.[SiteId] = c.InventSiteID WHERE c.SiteType = 'Virtual Site' ";
                    }
                    tmpSearch2.ShowDialog();
                    if (ConnectionString.Kodes != null)
                    {
                        tbxRefID.Text = ConnectionString.Kodes[0];
                        GetDataHeader();
                        ModeNew();
                        ConnectionString.Kodes = null;
                    }
                    break;
                case "Nota Retur Jual":
                    ControlMgr.TblName = "NotaReturJualH";
                    ControlMgr.tmpSort = "ORDER BY NRJDate DESC";

                    FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

                    FrmSearch.Text = "Search Transaction";
                    FrmSearch.ShowDialog();

                    if (ControlMgr.Kode != "")
                    {
                        tbxRefID.Text = ControlMgr.Kode;
                        GetDataHeader();
                        ModeNew();
                    }

                    ControlMgr.TblName = "";
                    ControlMgr.tmpSort = "";
                    ControlMgr.tmpWhere = "";
                    ControlMgr.Kode = "";
                    break;
            }

            //Surya Comment 2018-05-09
            //Form FrmSearch = new PopUp.FrmSearchMgr.FrmSearchMgr();

            //FrmSearch.Text = "Search Transaction";
            //FrmSearch.ShowDialog();

            //if (ControlMgr.Kode != "")
            //{
            //    tbxRefID.Text = ControlMgr.Kode;
            //    GetDataHeader();
            //    ModeNew();
            //}

            //ControlMgr.TblName = "";
            //ControlMgr.tmpSort = "";
            //Methods.ControlMgr.tmpWhere = "";
            //ControlMgr.Kode = "";
            //Surya Comment 2018-05-09

            /*Hendry comment ganti form search, 26 maret 2018
            SchemaName = "dbo"; TableName = ""; Where = "";
            switch (cbRef.Text)
            {
                case "Receipt Order":
                    TableName = "ReceiptOrderH";
                    Where = "and ReceiptOrderStatus != '03'";
                    break;
                case "Nota Transfer":
                    TableName = "NotaTransferH";
                    Where = "and TransStatus not in ('01', '02', '06')";
                    break;
                case "Nota Retur Jual":
                    TableName = "NotaReturJualH";
                    Where = "and TransStatus not in ('01')";
                    break;
            }

            SearchV2 f = new SearchV2();
            f.SetMode("No");
            f.SetSchemaTable(SchemaName, TableName, Where);
            f.ShowDialog();
            if (SearchV2.data.Count != 0)
            {
                tbxRefID.Text = SearchV2.data[0];
                GetDataHeader();
                ModeNew();
            }
            hendry comment end */
        }

        private void cbRef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                //if (cbRef.Text != "Select")
                //{
                btnSRefID.Enabled = true;
                tabPage2.Text = "Detail " + cmbReferenceType.Text;
                dataGridView1.Rows.Clear();
                dtRef.Value = Convert.ToDateTime("01/01/1753");
                tbxRefID.Text = String.Empty;
                tbxNameID.Text = String.Empty;
                tbxName.Text = String.Empty;
                dtExpectedDate.Value = Convert.ToDateTime("01/01/1753");
                tbxVOwnerID.Text = String.Empty;
                tbxVOwner.Text = String.Empty;
                tbxVType.Text = String.Empty;
                tbxVNumber.Text = String.Empty;
                tbxDriverName.Text = String.Empty;
                txtInventSiteID.Text = String.Empty;
                txtWarehouse.Text = String.Empty;
                txtSiteType.Text = String.Empty;
                tbxNotes.Text = String.Empty;
                cbVOwner.Checked = false;
                //}
                //else
                //{
                //    btnSRefID.Enabled = false;
                //    tabPage2.Text = "Detail Reference";
                //}
            }
        }

        private void tbxRefID_TextChanged(object sender, EventArgs e)
        {
            if (tbxRefID.Text.Trim() != String.Empty)
            {
                btnAdd.Enabled = true; btnDelete.Enabled = true;
            }
        }

        private void cbWeight1_CheckedChanged(object sender, EventArgs e)
        {

            if (cbWeight1.Checked == true)
            {
                tbxWeight1.Enabled = true;
                btnWeight1.Enabled = false;
            }
            else
            {
                tbxWeight1.Enabled = false;
                btnWeight1.Enabled = true;
                if (tbxGRNum.Text == String.Empty)
                {
                    tbxWeight1.Text = "0.0000";
                }
                else if (Mode == "Edit")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [Timbang1Weight] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    tbxWeight1.Text = Cmd.ExecuteScalar().ToString();
                }
            }
        }

        private void cbWeight2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWeight2.Checked == true)
            {
                tbxWeight2.Enabled = true;
                btnWeight2.Enabled = false;
            }
            else
            {
                tbxWeight2.Enabled = false;
                btnWeight2.Enabled = true;
                if (tbxGRNum.Text == String.Empty)
                {
                    tbxWeight2.Text = "0.0000";
                }
                else if (Mode == "Edit")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [Timbang2Weight] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    tbxWeight2.Text = Cmd.ExecuteScalar().ToString();
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].HeaderText.Contains("Qty"))
            {
                if (e.Value == null || e.Value == String.Empty || e.Value.ToString() == ".")
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N2");
            }
            if (dataGridView1.Columns[e.ColumnIndex].HeaderText.Contains("Ratio") || dataGridView1.Columns[e.ColumnIndex].HeaderText.Contains("TotalBerat"))
            {
                if (e.Value == null || e.Value == String.Empty || e.Value.ToString() == ".")
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N2");
            }
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["No"].Value = i + 1;
                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                {
                    dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value = i + 1;
                }
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //BY: HC (S)
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Ratio") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("TotalBerat"))
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    e.Handled = true;
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    e.Handled = true;
                if (e.KeyChar != '\b')
                {
                    if ((sender as TextBox) != null)
                    {
                        if ((sender as TextBox).Text.Length >= 15)
                            e.Handled = true;
                    }
                }
            }
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Notes"))
            {
                if (e.KeyChar != '\b')
                {
                    if ((sender as TextBox).Text.Length >= 255)
                        e.Handled = true;
                }
            }
            //BY: HC (E)
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(dataGridView1_KeyPress);
            if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "RatioActual")
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                    tb.KeyPress += new KeyPressEventHandler(dataGridView1_KeyPress);
            }
        }
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            decimal ratio = 0, qty = 0, totalBerat = 0;
            if ((tableColsName1[e.ColumnIndex] == "Qty_SJ" || tableColsName1[e.ColumnIndex] == "Ratio") && e.RowIndex >= 0)
            {
                if (!(dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value == null || dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value == String.Empty))
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells["Qty_SJ"].Value != null && dataGridView1.Rows[e.RowIndex].Cells["Qty_SJ"].Value != System.DBNull.Value)
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells["Qty_SJ"].Value.ToString() != "" && dataGridView1.Rows[e.RowIndex].Cells["Qty_SJ"].Value.ToString() != ".")
                        {
                            qty = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Qty_SJ"].Value);
                        }
                        else
                        {
                            qty = 0;
                        }
                    }
                    else
                    {
                        qty = 0;
                    }
                    ratio = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Ratio"].Value);
                    totalBerat = qty * ratio;
                    if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && tbxGRNum.Text == "")
                    {
                        tbxWeight1.Text = (Convert.ToDecimal(tbxWeight1.Text) - Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["TotalBerat"].Value) + totalBerat).ToString();
                        tbxWeight2.Text = (Convert.ToDecimal(tbxWeight2.Text) - Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["TotalBerat"].Value) + totalBerat).ToString();
                    }
                    dataGridView1.Rows[e.RowIndex].Cells["TotalBerat"].Value = totalBerat.ToString();
                }
            }
            else if (tableColsName1[e.ColumnIndex] == "Qty_Actual" || tableColsName1[e.ColumnIndex] == "Ratio_Actual")
            {
                if (!(dataGridView1.Rows[e.RowIndex].Cells["Ratio_Actual"].Value == null || dataGridView1.Rows[e.RowIndex].Cells["Ratio_Actual"].Value == String.Empty))
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells["Qty_Actual"].Value != null)
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells["Qty_Actual"].Value.ToString() == "" && dataGridView1.Rows[e.RowIndex].Cells["Qty_Actual"].Value.ToString() == ".")
                        {
                            qty = 0;
                        }
                        else
                        {
                            qty = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Qty_Actual"].Value);
                        }
                    }
                    else
                    {
                        qty = 0;
                    }
                    ratio = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells["Ratio_Actual"].Value);
                    totalBerat = qty * ratio;
                    dataGridView1.Rows[e.RowIndex].Cells["TotalBerat_Actual"].Value = totalBerat.ToString();
                }
            }
        }

        private void insertLockTable()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                decimal qtyToBeLocked = 0;
                Query = "SELECT * FROM [GoodsIssuedD] WHERE [GoodsIssuedId] = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString() + "' AND [GoodsIssuedSeqNo] = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value.ToString() + " ";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if ((Convert.ToDecimal(Dr["Reserved_Qty"]) != Convert.ToDecimal(0.00)) && (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) != 0))
                        {
                            if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Received")
                            {

                                if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) >= Convert.ToDecimal(Dr["Reserved_Qty"]))
                                {
                                    qtyToBeLocked = Convert.ToDecimal(Dr["Reserved_Qty"]);
                                }
                                else
                                {
                                    qtyToBeLocked = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                                }
                            }
                        }
                    }
                    Dr.Close();
                }
                if (qtyToBeLocked != 0)
                {
                    Query = "INSERT INTO [InventLockTable] VALUES ('GOODS RECEIVED','" + tbxGRNum.Text + "'," + Convert.ToInt32(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value) + ", ";
                    Query += " '" + tbxRefID.Text + "',0,'" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + txtInventSiteID.Text + "'," + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value) + ", ";
                    Query += " " + qtyToBeLocked + ", '" + dataGridView1.Rows[i].Cells["Unit"].Value.ToString() + "', " + (qtyToBeLocked * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value)) + ", 'KG','" + DateTime.Now + "','" + ControlMgr.UserId + "','" + DateTime.Now + "','' ) ";
                    using (Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //hendry perbaiki
            if (Validation() == '\0')
            {
                try
                {
                    using (TransactionScope Scope = new TransactionScope())
                    {
                        //created by Thaddaeus, 27 Sept 2018
                        if (tbxGRNum.Text != "")
                        {
                            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            {
                                OldGrStat = Cmd.ExecuteScalar().ToString();
                            }
                        }
                        //end==============================

                        Conn = ConnectionString.GetConnection();
                        if (Mode == "New")
                        {
                            //begin============================================================================================
                            //updated by : joshua
                            //updated date : 13 Feb 2018
                            //description : change generate sequence number, get from global function and update counter 
                            string Jenis = "", Kode = "", GRID = "";

                            if (cmbReferenceType.Text == "Receipt Order")
                            {
                                if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                                {
                                    Kode = "BBMV";
                                }
                                else
                                {
                                    Kode = "BBM";
                                }
                                Jenis = "GR";
                                GRID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                            }
                            else if (cmbReferenceType.Text == "Nota Transfer")
                            {
                                Jenis = "GR";
                                Kode = "BBMT";
                                GRID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                            }
                            else if (cmbReferenceType.Text == "Nota Retur Jual")
                            {
                                Jenis = "GR";
                                Kode = "BBMR";
                                GRID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                            }
                            else if (cmbReferenceType.Text == "Nota Retur Beli")
                            {
                                Jenis = "GR";
                                Kode = "BBMR";
                                GRID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                            }
                            tbxGRNum.Text = GRID;
                            //update counter
                            //string resultCounter = ConnectionString.UpdateCounter(Jenis, Kode, Conn, Trans, Cmd);
                            //end update counter
                            //end=============================================================================================
                            insertGRHeader();
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                decimal GoodsReceivedSeqNo;
                                if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                                    GoodsReceivedSeqNo = 0;
                                else
                                    GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                                decimal Qty_SJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                                string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                                decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
                                string ActionCode = dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString();
                                string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();

                                updateQtyToInventMovementQty(FullItemId, Qty_SJ, GoodsReceivedSeqNo, Ratio, ActionCode, Unit, i);
                            }
                            for (int i = 0; i < dataGridView3.RowCount; i++)
                            {
                                decimal GoodsReceivedSeqNo;
                                if (dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                                    GoodsReceivedSeqNo = 0;
                                else
                                    GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                                decimal Qty_SJ = dataGridView3.Rows[i].Cells["Qty_SJ"].Value == "" ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                                string FullItemId = dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString();
                                decimal Ratio = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value);
                                string ActionCode = dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString();
                                string Unit = dataGridView3.Rows[i].Cells["Unit"].Value.ToString();

                                updateQtyToInventMovementQty(FullItemId, Qty_SJ, GoodsReceivedSeqNo, Ratio, ActionCode, Unit, 0);
                            }
                            insertGRDetail();



                            //GET GR STATUS
                            string GRStats = "";
                            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            GRStats = Cmd.ExecuteScalar().ToString();

                            updateRefStatus(tbxRefID.Text, GRStats); //UPDATE RO HEADER STATUS

                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                decimal GoodsReceivedSeqNo;
                                if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                                    GoodsReceivedSeqNo = 0;
                                else
                                    GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                                int RefTransSeqNo;
                                if (dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == null)
                                    RefTransSeqNo = 0;
                                else
                                    RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                                decimal Qty_SJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);

                                string RefTransID = dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString();
                                string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                                decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
                                string ActionCode = dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString();
                                string InventSiteBlokID = dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString();
                                if (!((dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == null || dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty) && (dataGridView1.Rows[i].Cells["RefTransID"].Value == null || dataGridView1.Rows[i].Cells["RefTransID"].Value == String.Empty)))
                                {
                                    //UPDATE REF REMAINING QTY
                                    if (RefTransID != String.Empty)
                                        updateRefRemainingQty(Qty_SJ, GRID, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, i, ActionCode);

                                    switch (cmbReferenceType.Text)
                                    {
                                        case "Receipt Order":
                                            insertReceiptOrder_LogTable(Qty_SJ, Ratio, RefTransID, RefTransSeqNo, GRStats);
                                            break;
                                        case "Nota Transfer":
                                            insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                                            break;
                                        case "Nota Retur Jual":
                                            insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                                            break;
                                        case "Nota Retur Beli":
                                            insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                                            break;
                                    }
                                }
                                //insertGoodsReceived_LogTable(FullItemId, Convert.ToInt32(i + 1), Qty_SJ, Ratio, GRStats, RefTransID);
                            }
                            if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                            {
                                insertInventOnHandQty();
                                insertInventTrans();
                                updateRefStatus(tbxRefID.Text, GRStats);
                                insertNotaResizeHDtlLog();
                            }
                            insertGoodsReceived_LogTable(GRStats);
                        }
                        else
                        {
                            //updateGR >>> keseluruhan proses update GR
                            Journal = UpdateGr(Conn);
                            if (Journal == true)
                            {
                                Journal = false;
                                goto Outer;
                            }
                        }
                        Conn.Close();

                        //created by Thaddaeus, 27 Sept 2018,begin
                        string GrStats = "";
                        string LogDesc = "";
                        Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                        Cmd = new SqlCommand(Query, ConnectionString.GetConnection());
                        GrStats = Cmd.ExecuteScalar().ToString();
                        if (OldGrStat != "" && OldGrStat == GrStats)
                        {
                            LogDesc = "Edit";
                        }
                        if (Mode != "New" && GrStats == "01")
                        {
                            LogDesc = "Edit";
                        }
                        ListMethod.StatusLogVendor("GRHeaderV2", "GR", tbxNameID.Text, GrStats, LogDesc, tbxGRNum.Text, tbxRefID.Text, "", "");
                        OldGrStat = "";
                        //end=====================================

                        Scope.Complete();
                    }
                    if (Mode == "New")
                    {
                        MetroFramework.MetroMessageBox.Show(this, tbxGRNum.Text + " berhasil di save!", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Conn = ConnectionString.GetConnection();
                        Cmd = new SqlCommand("select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'", Conn);
                        string GRStats = Cmd.ExecuteScalar().ToString();
                        if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "05")
                        {
                            MetroFramework.MetroMessageBox.Show(this, tbxGRNum.Text + " berat 2 melebihi toleransi. Tolong request approval Site Manager!", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MetroFramework.MetroMessageBox.Show(this, tbxGRNum.Text + " berhasil di update!", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        Conn.Close();
                    }
                    Mode = "BeforeEdit";
                    GetDataHeader();
                    ModeBeforeEdit();

                Outer: ;
                }
                //catch (Exception ex)
                //{
                //    MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                finally
                {
                    Conn.Close();
                    Parent.RefreshGrid();
                }
            }
            //hendry end
        }

        private bool UpdateGr(SqlConnection Conn)
        {
            //Edited by Thaddaeus, 24 Sept 2018
            //for nota retur jual only, create new line if item qty within tolerance and make the action code for that line parked - tollerance GRD-12
            //CheckAndInsertParkTollerance(Conn);
            //end===============================

            //UPDATE REF REMAINING QTY & quantity movement for item with ref
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                #region variable
                decimal GoodsReceivedSeqNo;
                if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                    GoodsReceivedSeqNo = 0;
                else
                    GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);

                int RefTransSeqNo;
                if (dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == null)
                    RefTransSeqNo = 0;
                else
                    RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                if (dataGridView1.Rows[i].Cells["Qty_SJ"].Value == String.Empty || dataGridView1.Rows[i].Cells["Qty_SJ"].Value == null)
                    dataGridView1.Rows[i].Cells["Qty_SJ"].Value = 0;

                decimal Qty_SJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);

                string RefTransID = dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString();
                string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
                decimal Qty_Actual;
                if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
                    Qty_Actual = 0;
                else
                    Qty_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                //created by thaddaeus, 6 April 2018 begin
                string ActionCode = dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString();
                string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();
                //end==================================
                #endregion
                if (!((dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == null || dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty) && (dataGridView1.Rows[i].Cells["RefTransID"].Value == null || dataGridView1.Rows[i].Cells["RefTransID"].Value == String.Empty)))
                {
                    //edited by Thaddaeus 15MAY2018, begin, goal: cari apakah sedang WB1/WB2, klo WB1 tidak ad nilai qty actual jadi menggunakan nilai SJ/GI
                    if (ControlMgr.GroupName == "WB OPERATOR" && (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() != String.Empty || dataGridView1.Rows[i].Cells["Qty_Actual"].Value != null))
                    {
                        //end======================
                        if (RefTransID != String.Empty)
                        {
                            if (checkWeightTollerance(Conn) != "05")
                            {
                                updateRefRemainingQty(Qty_SJ, tbxGRNum.Text, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, i, ActionCode);
                                updateQtyToInventMovementQty(FullItemId, Qty_SJ, GoodsReceivedSeqNo, Ratio, ActionCode, Unit, i);
                            }
                        }
                    }
                    else
                    {
                        if (RefTransID != String.Empty)
                        {
                            if (checkWeightTollerance(Conn) != "05")
                            {
                                updateRefRemainingQty(Qty_Actual, tbxGRNum.Text, GoodsReceivedSeqNo, RefTransID, RefTransSeqNo, i, ActionCode);
                                updateQtyToInventMovementQty(FullItemId, Qty_Actual, GoodsReceivedSeqNo, Ratio, ActionCode, Unit, i);
                            }
                        }
                    }
                }
            }

            //update movement for item without ref
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                #region variable
                decimal GoodsReceivedSeqNo;
                if (dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                    GoodsReceivedSeqNo = 0;
                else
                    GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value);

                int RefTransSeqNo;
                if (dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value == null)
                    RefTransSeqNo = 0;
                else
                    RefTransSeqNo = Convert.ToInt32(dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value);

                if (dataGridView3.Rows[i].Cells["Qty_SJ"].Value == String.Empty || dataGridView3.Rows[i].Cells["Qty_SJ"].Value == null)
                    dataGridView3.Rows[i].Cells["Qty_SJ"].Value = 0;

                decimal Qty_SJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);

                string RefTransID = dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString();
                string FullItemId = dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString();
                decimal Ratio = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value);
                decimal Qty_Actual;
                if (dataGridView3.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
                    Qty_Actual = 0;
                else
                    Qty_Actual = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value);
                //created by thaddaeus, 6 April 2018 begin
                string ActionCode = dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString();
                string Unit = dataGridView3.Rows[i].Cells["Unit"].Value.ToString();
                //end==================================
                #endregion
                //edited by Thaddaeus 15MAY2018, begin, goal: cari apakah sedang WB1/WB2, klo WB1 tidak ad nilai qty actual jadi menggunakan nilai SJ/GI
                if (ControlMgr.GroupName == "WB OPERATOR" && (dataGridView3.Rows[i].Cells["Qty_Actual"].Value.ToString() != String.Empty || dataGridView3.Rows[i].Cells["Qty_Actual"].Value != null))
                {
                    //end======================
                    if (checkWeightTollerance(Conn) != "05")
                    {
                        updateQtyToInventMovementQty(FullItemId, Qty_SJ, GoodsReceivedSeqNo, Ratio, ActionCode, Unit, i);
                    }
                }
                else
                {
                    if (checkWeightTollerance(Conn) != "05")
                    {
                        updateQtyToInventMovementQty(FullItemId, Qty_Actual, GoodsReceivedSeqNo, Ratio, ActionCode, Unit, i);
                    }
                }
            }

            //GET GR STATUS
            string GRStats = "";
            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            GRStats = Cmd.ExecuteScalar().ToString();

            updateGRHeader(GRStats); //UPDATE GR HEADER

            updateGRDetail(); //UPDATE GR DETAIL

            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            GRStats = Cmd.ExecuteScalar().ToString();

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                #region variable
                decimal GoodsReceivedSeqNo;
                if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                    GoodsReceivedSeqNo = 0;
                else
                    GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);

                int RefTransSeqNo;
                if (dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value == null)
                    RefTransSeqNo = 0;
                else
                    RefTransSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                decimal Qty_Actual;
                if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
                    Qty_Actual = 0;
                else
                    Qty_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);

                decimal Qty_SJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();
                decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
                string RefTransID = dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString();
                string InventSiteBlokID = dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString();
                #endregion
                if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "01")
                {
                    //insertGoodsReceived_LogTable(FullItemId, GoodsReceivedSeqNo, Qty_SJ, Ratio, GRStats, RefTransID);
                    switch (cmbReferenceType.Text)
                    {
                        case "Receipt Order":
                            insertReceiptOrder_LogTable(Qty_SJ, Ratio, RefTransID, RefTransSeqNo, GRStats);
                            break;
                        case "Nota Transfer":
                            insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Jual":
                            insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Beli":
                            insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                    }
                }
                else
                {
                    //insertGoodsReceived_LogTable(FullItemId, GoodsReceivedSeqNo, Qty_Actual, Ratio, GRStats, RefTransID);
                    switch (cmbReferenceType.Text)
                    {
                        case "Receipt Order":
                            insertReceiptOrder_LogTable(Qty_Actual, Ratio, RefTransID, RefTransSeqNo, GRStats);
                            break;
                        case "Nota Transfer":
                            insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Jual":
                            insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Beli":
                            insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                    }
                }
            }

            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                #region variable
                decimal GoodsReceivedSeqNo;
                if (dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
                    GoodsReceivedSeqNo = 0;
                else
                    GoodsReceivedSeqNo = Convert.ToDecimal(dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value);

                int RefTransSeqNo;
                if (dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value == null)
                    RefTransSeqNo = 0;
                else
                    RefTransSeqNo = Convert.ToInt32(dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value);

                decimal Qty_Actual;
                if (dataGridView3.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
                    Qty_Actual = 0;
                else
                    Qty_Actual = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value);

                decimal Qty_SJ = 0;
                if (!(dataGridView3.Rows[i].Cells["Qty_SJ"].Value == String.Empty || dataGridView3.Rows[i].Cells["Qty_SJ"].Value == null))
                    Qty_SJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                string FullItemId = dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString();
                decimal Ratio = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value);
                string RefTransID = dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString();
                string InventSiteBlokID = dataGridView3.Rows[i].Cells["InventSiteBlokID"].Value.ToString();
                #endregion
                if (ControlMgr.GroupName == "WB OPERATOR" && GRStats == "01")
                {
                    //insertGoodsReceived_LogTable(FullItemId, GoodsReceivedSeqNo, Qty_SJ, Ratio, GRStats, RefTransID);
                    switch (cmbReferenceType.Text)
                    {
                        case "Receipt Order":
                            insertReceiptOrder_LogTable(Qty_SJ, Ratio, RefTransID, RefTransSeqNo, GRStats);
                            break;
                        case "Nota Transfer":
                            insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Jual":
                            insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Beli":
                            insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty_SJ, Qty_SJ * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                    }
                }
                else
                {
                    //insertGoodsReceived_LogTable(FullItemId, GoodsReceivedSeqNo, Qty_Actual, Ratio, GRStats, RefTransID);
                    switch (cmbReferenceType.Text)
                    {
                        case "Receipt Order":
                            insertReceiptOrder_LogTable(Qty_Actual, Ratio, RefTransID, RefTransSeqNo, GRStats);
                            break;
                        case "Nota Transfer":
                            insertNotaTransferLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Jual":
                            insertNotaReturJualLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                        case "Nota Retur Beli":
                            insertNotaReturBeliLogTable(RefTransID, RefTransSeqNo, Qty_Actual, Qty_Actual * Ratio, dtRef.Value, tbxNameID.Text, InventSiteBlokID, FullItemId, GRStats);
                            break;
                    }
                }
            }
            insertGoodsReceived_LogTable(GRStats);

            updateRefStatus(tbxRefID.Text, GRStats);

            Cmd = new SqlCommand("select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'", Conn);
            GRStats = Cmd.ExecuteScalar().ToString();

            if ((GRStats == "03" && ControlMgr.GroupName == "WB OPERATOR") || (GRStats == "06" && ControlMgr.GroupName.ToUpper() == "SITE MANAGER"))
            {
                //hendry stock 
                insertInventOnHandQty();//MANAGE PURCHASE ITEM TO INVENT STOCK //di insertInventOnHandQty ada updateROPurchaseQty(i);
                insertInventTrans();//INSERT TO INVENT TABLE TRANS
                //hendry end

                //Created by Thaddaeus, 24MAY2018, BEGIN
                //NOTE: dependent on the action code status (since any new line/item will be forced to not have the 'received' status), will be inserted to locktable if action code 'received' so if the same item 
                //with both recieved status, there will be an additional qty in locktable 
                insertLockTable();
                //end==============

                //Edited by Thaddaeus, 24 Sept 2018,begin
                if (cmbReferenceType.Text != "Nota Retur Jual")
                {
                    Query = "select count(*) from NotaPurchaseParkH where GoodsReceivedID = '" + tbxGRNum.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    if ((Int32)Cmd.ExecuteScalar() == 0)
                    {
                        //hendry
                        Cmd = new SqlCommand("usp_AutoInsertNotaPurchasePark", Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@GRId", tbxGRNum.Text.Trim());
                        Cmd.Parameters.AddWithValue("@UserId", ControlMgr.UserId);
                        Cmd.ExecuteNonQuery();
                        //hendry end
                    }
                }
                else
                {
                    InsertNRJParked();
                }
                //end=========================================

                insertNotaResizeHDtlLog();//INSERT TO NOTARESIZE WHERE ACTION CODE = RESIZE
                insertNotaPurchasedParkedHDtlLog();
                //manageQtyinInventMovementQty(); //deduct in progress qty when completed

                //Begin
                //Created By : Joshua
                //Created Date ; 06 Aug 2018
                //Desc : Create Journal
                CreateJournal();
                //edited By Thaddaeus, 04 Aug 2018,begin
                //if (Journal == true)
                //{
                //    Journal = false;
                //    goto Outer;
                //}
                //end=========================
                //End
            }
            return Journal;
        }

        private void insertNotaReturBeliLogTable(string NRBId, int SeqNo, decimal Qty_UoM, decimal Qty_Alt, DateTime NRBDate, string VendId, string SiteId, string FullItemId, string LogStatusCode)
        {

            decimal Amount = 0;
            DateTime GoodsReceivedDate = new DateTime(1753, 1, 1);
            string GoodsReceivedId = "";
            Query = "select a.NRBDate, a.GoodsReceivedId, a.GoodsReceivedDate, a.VendId, a.SiteId, b.FullItemId, ROD.Price from NotaReturBeliH a left join NotaReturBeli_Dtl b on a.NRBId = b.NRBId left join GoodsReceivedD c on c.GoodsReceivedId = b.GoodsReceivedId and b.GoodsReceived_SeqNo = c.GoodsReceivedSeqNo left join ReceiptOrderD ROD on c.RefTransID = ROD.ReceiptOrderId where b.NRBId = '" + NRBId + "' and b.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["Price"] != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Dr["Price"]);
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                NRBDate = Convert.ToDateTime(Dr["NRBDate"]);
                GoodsReceivedDate = Convert.ToDateTime(Dr["GoodsReceivedDate"]);
                GoodsReceivedId = Dr["GoodsReceivedId"].ToString();
                VendId = Dr["VendId"].ToString();
                SiteId = Dr["SiteId"].ToString();
                FullItemId = Dr["FullItemId"].ToString();
            }
            Dr.Close();

            Query = "select deskripsi from TransStatusTable where TransCode = 'GI' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[NotaReturBeli_LogTable] ([NRBDate],[NRBId],[GoodsReceivedDate],[GoodsReceivedId],[VendId],[SiteId],[FullItemId],[SeqNo],[Qty_UoM],[Qty_Alt],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES ('" + NRBDate + "', '" + NRBId + "', '" + GoodsReceivedDate + "', '" + GoodsReceivedId + "', '" + VendId + "', '" + SiteId + "', '" + FullItemId + "', '" + SeqNo + "', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertNotaReturJualLogTable(string NRJId, int SeqNo, decimal Qty_UoM, decimal Qty_Alt, DateTime NRJDate, string CustId, string SiteId, string FullItemId, string LogStatusCode)
        {

            decimal Amount = 0;
            DateTime GoodsReceivedDate = new DateTime(1753, 1, 1);
            string GoodsReceivedId = "";
            Query = "SELECT a.[NRJDate],a.[CustId],a.[SiteId],b.[FullItemId],e.[Price] FROM [NotaReturJualH] a LEFT JOIN [NotaReturJual_Dtl] b ON a.[NRJId]=b.[NRJId] LEFT JOIN [GoodsIssuedD] c ON b.[GoodsIssuedId] = c.[GoodsIssuedId] AND b.[GoodsIssued_SeqNo] = c.[GoodsIssuedSeqNo] LEFT JOIN [DeliveryOrderD] d ON d.[DeliveryOrderId] = c.[RefTransID] AND d.[SeqNo] = c.[RefTransSeqNo] LEFT JOIN [SalesOrderD] e ON e.[SalesOrderNo] =d.[SalesOrderId] AND e.[SeqNo] =d.[SalesOrderSeqNo]  WHERE a.NRJId = @NRJId AND b.SeqNo = @SeqNo ";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NRJId", NRJId);
            Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["Price"] != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Dr["Price"]);
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                NRJDate = Convert.ToDateTime(Dr["NRJDate"]);
                GoodsReceivedDate = dtGR.Value;
                GoodsReceivedId = tbxGRNum.Text;
                CustId = Dr["CustId"].ToString();
                SiteId = Dr["SiteId"].ToString();
                FullItemId = Dr["FullItemId"].ToString();
            }
            Dr.Close();

            Query = "select deskripsi from TransStatusTable where TransCode = 'GR' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[NotaReturJual_LogTable] ([NRJDate],[NRJId],[GoodsIssuedDate],[GoodsIssuedId],[CustId],[SiteId],[FullItemId],[SeqNo],[Qty_UoM],[Qty_Alt],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES ('" + NRJDate + "', '" + NRJId + "', '" + GoodsReceivedDate + "', '" + GoodsReceivedId + "', '" + CustId + "', '" + SiteId + "', '" + FullItemId + "', '" + SeqNo + "', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void insertNotaTransferLogTable(string NTId, int SeqNo, decimal Qty_UoM, decimal Qty_Alt, DateTime NTDate, string FromSiteId, string ToSiteId, string FullItemId, string LogStatusCode)
        {
            decimal Amount = 0;

            string RefTransId = "";
            DateTime RefTransDate = new DateTime(1753, 1, 1);
            Query = "select a.TransferDate, b.ReferenceId, c.OrderDate, a.InventSiteFrom, a.InventSiteTo, b.FullItemId, b.SeqNo, d.Price from NotaTransferH a left join  NotaTransferD b on a.TransferNo = b.TransferNo left join SalesOrderH c on b.ReferenceId = c.SalesOrderNo left join SalesOrderD d on c.SalesOrderNo = d.SalesOrderNo where b.TransferNo = '" + NTId + "' and b.SeqNo = '" + SeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["Price"] != (object)DBNull.Value)
                    Amount = Convert.ToDecimal(Dr["Price"]);
                else
                {
                    Query = "select UoM_AvgPrice from InventTable where FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Amount = Convert.ToDecimal(Cmd.ExecuteScalar());
                }
                if (!(Dr["ReferenceId"] == (object)DBNull.Value || Dr["ReferenceId"].ToString() == String.Empty))
                {
                    RefTransId = Dr["ReferenceId"].ToString();
                    RefTransDate = Convert.ToDateTime(Dr["TransferDate"]);
                }
            }

            Query = "select deskripsi from TransStatusTable where TransCode = 'GR' and StatusCode = '" + LogStatusCode + "'";
            Cmd = new SqlCommand(Query, Conn);
            string LogStatusDesc = Cmd.ExecuteScalar().ToString();

            Query = "INSERT INTO [dbo].[NotaTransfer_LogTable] ([NTDate],[NTId],[RefTransId],[RefTransDate],[FromSiteId],[ToSiteId],[FullItemId],[SeqNo],[Flag],[LockDocument],[Qty_UoM],[Qty_Alt],[Amount],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES (@NTDate, '" + NTId + "', @RefTransId, @RefTransDate, '" + FromSiteId + "', '" + ToSiteId + "', '" + FullItemId + "', '" + SeqNo + "', '0', 'BBM', '" + Qty_UoM + "', '" + Qty_Alt + "', '" + Amount + "', '" + LogStatusCode + "', '" + LogStatusDesc + "', @LogDescription, '" + ControlMgr.UserId + "', getdate())";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@NTDate", NTDate);
            Cmd.Parameters.AddWithValue("@RefTransId", RefTransId == "" ? (object)DBNull.Value : RefTransId);
            Cmd.Parameters.AddWithValue("@RefTransDate", RefTransId == "" ? (object)DBNull.Value : RefTransDate);
            Cmd.Parameters.AddWithValue("@LogDescription", "Status: " + LogStatusCode + ". " + LogStatusDesc);
            Cmd.ExecuteNonQuery();
        }

        private void manageQtyinInventMovementQty()
        {
            //for (int i = 0; i < dataGridView1.RowCount; i++)
            //{
            //    int GoodsReceivedSeqNo;
            //    if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
            //        GoodsReceivedSeqNo = 0;
            //    else
            //        GoodsReceivedSeqNo = Convert.ToInt32(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);

            //    decimal Qty_Actual;
            //    if (dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
            //        Qty_Actual = 0;
            //    else
            //        Qty_Actual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);

            //    decimal Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
            //    string FullItemId = dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString();

            //    Query = "select c.Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId left join InventTable d on d.FullItemId = b.FullItemId where b.GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "' and b.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    decimal UoM_AvgPrice = 0;
            //    decimal price = 0;
            //    while (Dr.Read())
            //    {
            //        if (Dr["Price"].ToString() != String.Empty)
            //            price = (Decimal)Dr["Price"];
            //        if (Dr["UoM_AvgPrice"].ToString() != String.Empty)
            //            UoM_AvgPrice = (Decimal)Dr["UoM_AvgPrice"];
            //    }
            //    Dr.Close();

            //    Query = "Select GR_In_Progress_UoM, GR_In_Progress_Alt, GR_In_Progress_Amount from Invent_Movement_Qty where FullItemId = '" + FullItemId + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    decimal GR_In_Progress_UoM = 0;
            //    decimal GR_In_Progress_Alt = 0;
            //    decimal GR_In_Progress_Amount = 0;
            //    while (Dr.Read())
            //    {
            //        GR_In_Progress_UoM = Convert.ToDecimal(Dr["GR_In_Progress_UoM"]);
            //        GR_In_Progress_Alt = Convert.ToDecimal(Dr["GR_In_Progress_Alt"]);
            //        GR_In_Progress_Amount = Convert.ToDecimal(Dr["GR_In_Progress_Amount"]);
            //    }
            //    Dr.Close();

            //    GR_In_Progress_UoM = GR_In_Progress_UoM - Qty_Actual;
            //    GR_In_Progress_Alt = GR_In_Progress_Alt - (Qty_Actual * Ratio);
            //    if (dataGridView1.Rows[i].Cells["RefTransId"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransId"].Value == null)
            //        GR_In_Progress_Amount = GR_In_Progress_Amount + (Qty_Actual * UoM_AvgPrice);
            //    else
            //        GR_In_Progress_Amount = GR_In_Progress_Amount + (Qty_Actual * price);
            //    Query = "Update Invent_Movement_Qty set GR_In_Progress_UoM = '" + GR_In_Progress_UoM + "', GR_In_Progress_Alt = '" + GR_In_Progress_Alt + "', GR_In_Progress_Amount = '" + GR_In_Progress_Amount + "' where FullItemId = '" + FullItemId + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Cmd.ExecuteNonQuery();
            //}

            //for (int i = 0; i < dataGridView3.RowCount; i++)
            //{
            //    int GoodsReceivedSeqNo;
            //    if (dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null)
            //        GoodsReceivedSeqNo = 0;
            //    else
            //        GoodsReceivedSeqNo = Convert.ToInt32(dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value);

            //    decimal Qty_Actual;
            //    if (dataGridView3.Rows[i].Cells["Qty_Actual"].Value.ToString() == String.Empty)
            //        Qty_Actual = 0;
            //    else
            //        Qty_Actual = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value);

            //    decimal Ratio = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value);
            //    string FullItemId = dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString();

            //    Query = "select c.Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId left join InventTable d on d.FullItemId = b.FullItemId where b.GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "' and b.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    decimal UoM_AvgPrice = 0;
            //    decimal price = 0;
            //    while (Dr.Read())
            //    {
            //        if (Dr["Price"].ToString() != String.Empty)
            //            price = (Decimal)Dr["Price"];
            //        if (Dr["UoM_AvgPrice"].ToString() != String.Empty)
            //            UoM_AvgPrice = (Decimal)Dr["UoM_AvgPrice"];
            //    }
            //    Dr.Close();

            //    Query = "Select GR_In_Progress_UoM, GR_In_Progress_Alt, GR_In_Progress_Amount from Invent_Movement_Qty where FullItemId = '" + FullItemId + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    decimal GR_In_Progress_UoM = 0;
            //    decimal GR_In_Progress_Alt = 0;
            //    decimal GR_In_Progress_Amount = 0;
            //    while (Dr.Read())
            //    {
            //        GR_In_Progress_UoM = Convert.ToDecimal(Dr["GR_In_Progress_UoM"]);
            //        GR_In_Progress_Alt = Convert.ToDecimal(Dr["GR_In_Progress_Alt"]);
            //        GR_In_Progress_Amount = Convert.ToDecimal(Dr["GR_In_Progress_Amount"]);
            //    }
            //    Dr.Close();

            //    GR_In_Progress_UoM = GR_In_Progress_UoM - Qty_Actual;
            //    GR_In_Progress_Alt = GR_In_Progress_Alt - (Qty_Actual * Ratio);
            //    if (dataGridView3.Rows[i].Cells["RefTransId"].Value == String.Empty || dataGridView3.Rows[i].Cells["RefTransId"].Value == null)
            //        GR_In_Progress_Amount = GR_In_Progress_Amount + (Qty_Actual * UoM_AvgPrice);
            //    else
            //        GR_In_Progress_Amount = GR_In_Progress_Amount + (Qty_Actual * price);
            //    Query = "Update Invent_Movement_Qty set GR_In_Progress_UoM = '" + GR_In_Progress_UoM + "', GR_In_Progress_Alt = '" + GR_In_Progress_Alt + "', GR_In_Progress_Amount = '" + GR_In_Progress_Amount + "' where FullItemId = '" + FullItemId + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Cmd.ExecuteNonQuery();
            //}
        }

        private void updateROPurchaseQty(int i)
        {
            decimal ROUoM = 0;
            decimal ROAlt = 0;
            decimal ROAmount = 0;
            decimal price = 0;
            string Unit = dataGridView1.Rows[i].Cells["Unit"].Value.ToString();
            if (cmbReferenceType.Text.ToUpper() == "RECEIPT ORDER")
            {
                if (Unit.ToUpper() == "KG")
                {
                    Query = "SELECT Price_KG FROM ReceiptOrderD WHERE ReceiptOrderId = '" + tbxRefID.Text + "' and FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
                }
                else
                {
                    //Price di RO merupakan Price PO, jadi walaupun RO mempunyai Unit BTG tetapi jika di PO unitnya adalah KG maka Pricenya adalah KG
                    //karena itu lebih aman menggunakan Price_KG dibanding Price yang unit bs berubah"
                    //Query = "SELECT Price FROM ReceiptOrderD WHERE ReceiptOrderId = '" + tbxRefID.Text + "' and FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
                    Query = "SELECT Price_KG FROM ReceiptOrderD WHERE ReceiptOrderId = '" + tbxRefID.Text + "' and FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
                }
            }
            else
            {
                return;
            }
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() == System.DBNull.Value)
            {
                price = 1;
            }
            else
            {
                price = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            ROAlt = Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value);
            if (dataGridView1.Rows[i].Cells["Unit"].Value.ToString().ToUpper() == "KG")
            {
                Query = "SELECT Ratio FROM InventConversion WHERE FullItemID = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' ";
                Cmd = new SqlCommand(Query, Conn);
                decimal Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
                ROUoM = ROAlt / Ratio;
            }
            else
            {
                ROUoM = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
            }
            ROAmount = ROAlt * price;
            Query = "UPDATE Invent_Purchase_Qty SET RO_Issued_UoM=RO_Issued_UoM-'" + ROUoM + "',RO_Issued_Alt=RO_Issued_Alt-'" + ROAlt + "',RO_Issued_Amount=RO_Issued_Amount-'" + ROAmount + "' WHERE FullItemID = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

        }

        private void insertGoodsReceived_LogTable(string GRStats) //string FullItemId, int GoodsReceivedSeqNo, decimal Qty_SJ, decimal Ratio, string GRStats, string RefTransId
        {
            decimal price = 0;
            decimal UoM_AvgPrice = 0;

            Query = "select b.GoodsReceivedId, b.GoodsReceivedSeqNo, a.GoodsReceivedDate, b.RefTransId, a.RefTransDate, a.VendId, b.InventSiteId, b.FullItemId, b.Qty_Actual, b.Qty_SJ, b.Ratio, a.[GoodsReceivedStatus] from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId where a.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                if (Dr["GoodsReceivedId"] != System.DBNull.Value && Dr["GoodsReceivedSeqNo"] != System.DBNull.Value)
                {
                    if (Dr["GoodsReceivedId"] != "" && Dr["GoodsReceivedSeqNo"] != "")
                    {
                        Query = "select c.Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId left join InventTable d on d.FullItemId = b.FullItemId where b.GoodsReceivedSeqNo = '" + Dr["GoodsReceivedSeqNo"] + "' and b.GoodsReceivedId = '" + Dr["GoodsReceivedId"] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        SqlDataReader Dr2 = Cmd.ExecuteReader();

                        while (Dr2.Read())
                        {
                            if (Dr2["Price"].ToString() != String.Empty)
                                price = (Decimal)Dr2["Price"];
                            if (Dr2["UoM_AvgPrice"].ToString() != String.Empty)
                                UoM_AvgPrice = (Decimal)Dr2["UoM_AvgPrice"];
                        }
                        Dr2.Close();
                    }
                    else
                    {
                        Query = "select c.Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId left join InventTable d on d.FullItemId = b.FullItemId where b.FullItemId = '" + Dr["FullItemId"] + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        SqlDataReader Dr2 = Cmd.ExecuteReader();

                        while (Dr2.Read())
                        {
                            if (Dr2["Price"].ToString() != String.Empty)
                                price = (Decimal)Dr2["Price"];
                            if (Dr2["UoM_AvgPrice"].ToString() != String.Empty)
                                UoM_AvgPrice = (Decimal)Dr2["UoM_AvgPrice"];
                        }
                        Dr2.Close();
                    }


                    Query = "INSERT INTO [dbo].[GoodsReceived_LogTable] ([GoodsReceivedDate] ,[GoodsReceivedId] ,[ReceiptOrderNo] ,[ReceiptOrderDate] ,[VendorID] ,[InventSiteID] ,[FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount] ,[LogStatusCode] ,[LogStatusDesc] ,[LogDescription] ,[UserID] ,[LogDate]) VALUES ('" + Dr["GoodsReceivedDate"] + "', '" + Dr["GoodsReceivedId"] + "', '" + Dr["RefTransId"] + "', '" + Dr["RefTransDate"] + "', '" + Dr["VendId"] + "', '" + Dr["InventSiteId"] + "', '" + Dr["FullItemId"] + "', '" + Dr["GoodsReceivedSeqNo"] + "'";
                    //edited by Thaddaeus 15May2018, Begin
                    if ((ControlMgr.GroupName == "KERANI" && Mode == "Edit") || (ControlMgr.GroupName == "WB OPERATOR" && Mode == "Edit" && Dr["GoodsReceivedStatus"].ToString() == "02"))
                        Query += ", '" + Dr["Qty_Actual"] + "', '" + Convert.ToDecimal(Dr["Ratio"]) * Convert.ToDecimal(Dr["Qty_Actual"]) + "'";
                    //end==================================
                    else
                        Query += ", '" + Dr["Qty_SJ"] + "', '" + Convert.ToDecimal(Dr["Ratio"]) * Convert.ToDecimal(Dr["Qty_SJ"]) + "'";

                    if (Dr["RefTransId"].ToString() == String.Empty)
                    {
                        //edited by Thaddaeus 15May2018, Begin
                        if ((ControlMgr.GroupName == "KERANI" && Mode == "Edit") || (ControlMgr.GroupName == "WB OPERATOR" && Mode == "Edit" && Dr["GoodsReceivedStatus"].ToString() == "02"))
                            Query += ", '" + Convert.ToDecimal(Dr["Qty_Actual"]) * UoM_AvgPrice + "'";
                        //END===========================================
                        else
                            Query += ", '" + Convert.ToDecimal(Dr["Qty_SJ"]) * UoM_AvgPrice + "'";
                    }
                    else
                    {
                        //edited by Thaddaeus 15May2018, Begin
                        if ((ControlMgr.GroupName == "KERANI" && Mode == "Edit") || (ControlMgr.GroupName == "WB OPERATOR" && Mode == "Edit" && Dr["GoodsReceivedStatus"].ToString() == "02"))
                            Query += ", '" + Convert.ToDecimal(Dr["Qty_Actual"]) * price + "'";
                        //END===================================
                        else
                            Query += ", '" + Convert.ToDecimal(Dr["Qty_SJ"]) * price + "'";
                    }
                    if (Mode == "New" || (GRStats == "01" && Mode == "Edit" && ControlMgr.GroupName == "WB OPERATOR"))
                        Query += ", '01', 'GR In Progress', 'GR In Progress'";
                    else if ((GRStats == "02" || GRStats == "03") && ControlMgr.GroupName == "WB OPERATOR") //marks || GRStats == "05"
                    {
                        decimal GRatioActual = 0;
                        decimal TotalBerat_Actual = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            TotalBerat_Actual = dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString());
                            GRatioActual += TotalBerat_Actual;
                        }
                        for (int i = 0; i < dataGridView3.RowCount; i++)
                        {
                            TotalBerat_Actual = dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString());
                            GRatioActual += TotalBerat_Actual;
                        }
                        decimal weightTolerance = (Convert.ToDecimal(tbxWeight2.Text) - (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual)) / (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual) * 100;
                        if (Math.Abs(weightTolerance) > 1)
                            Query += ", '05', 'Waiting for Approval', 'Waiting for Approval'";
                        else
                            Query += ", '03', 'GR Completed', 'GR Completed'";

                    }
                    else if ((GRStats == "01" || GRStats == "02") && ControlMgr.GroupName == "KERANI")
                        Query += ", '02', 'Unload and QC done', 'Unload and QC done'";
                    else if ((GRStats == "06") && ControlMgr.GroupName == "SITE MANAGER")
                        Query += ", '02', 'Unload and QC done', 'Unload and QC done'";
                    else if ((GRStats == "05") && ControlMgr.GroupName == "SITE MANAGER")
                        Query += ", '03', 'GR Completed', 'GR Completed'";
                    Query += ", '" + ControlMgr.UserId + "', getdate())";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                }
            }
            Dr.Close();

            //Query = "select c.Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId left join InventTable d on d.FullItemId = b.FullItemId where b.GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "' and b.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();
            //decimal price = 0;
            //decimal UoM_AvgPrice = 0;
            //while (Dr.Read())
            //{
            //    if (Dr["Price"].ToString() != String.Empty)
            //        price = (Decimal)Dr["Price"];
            //    if (Dr["UoM_AvgPrice"].ToString() != String.Empty)
            //        UoM_AvgPrice = (Decimal)Dr["UoM_AvgPrice"];
            //}
            //Dr.Close();

            //Query = "INSERT INTO [dbo].[GoodsReceived_LogTable] ([GoodsReceivedDate] ,[GoodsReceivedId] ,[ReceiptOrderNo] ,[ReceiptOrderDate] ,[VendorID] ,[InventSiteID] ,[FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount] ,[LogStatusCode] ,[LogStatusDesc] ,[LogDescription] ,[UserID] ,[LogDate]) VALUES ('" + dtGR.Value + "', '" + tbxGRNum.Text + "', '" + tbxRefID.Text + "', '" + dtRef.Value + "', '" + tbxVOwnerID.Text + "', '" + txtInventSiteID.Text + "', '" + FullItemId + "', '" + GoodsReceivedSeqNo + "', '" + Qty_SJ + "', '" + Ratio * Qty_SJ + "'";
            //if (RefTransId == String.Empty)
            //    Query += ", '" + Qty_SJ * UoM_AvgPrice + "'";
            //else
            //    Query += ", '" + Qty_SJ * price + "'";
            //if (Mode == "New" || (GRStats == "01" && Mode == "Edit" && ControlMgr.GroupName == "WB OPERATOR"))
            //    Query += ", '01', 'GR In Progress', 'GR In Progress'";
            //else if ((GRStats == "02" || GRStats == "03") && ControlMgr.GroupName == "WB OPERATOR")
            //{
            //    decimal GRatioActual = 0;
            //    for (int i = 0; i < dataGridView1.RowCount; i++)
            //    {
            //        GRatioActual += Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value);
            //    }
            //    for (int i = 0; i < dataGridView3.RowCount; i++)
            //    {
            //        GRatioActual += Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value);
            //    }
            //    decimal weightTolerance = (Convert.ToDecimal(tbxWeight2.Text) - (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual)) / (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual) * 100;
            //    if (Math.Abs(weightTolerance) > 1)
            //        Query += ", '05', 'Waiting for Approval', 'Waiting for Approval'";
            //    else
            //        Query += ", '03', 'GR Completed', 'GR Completed'";

            //}
            //else if ((GRStats == "01" || GRStats == "02") && ControlMgr.GroupName == "KERANI")
            //    Query += ", '02', 'Unload and QC done', 'Unload and QC done'";
            //Query += ", '" + ControlMgr.GroupName + "', getdate())";
            //Cmd = new SqlCommand(Query, Conn);
            //Cmd.ExecuteNonQuery();
        }

        private void insertReceiptOrder_LogTable(decimal Qty_SJ, decimal Ratio, string RefTransID, int RefTransSeqNo, string GRStats)
        {
            Query = "select a.[ReceiptOrderDate] ,a.[ReceiptOrderId] ,a.PurchaseOrderId ,a.[PurchaseOrderDate] ,a.VendId ,a.[InventSiteID] ,b.[FullItemId] ,b.SeqNo, b.RemainingQty from ReceiptOrderH a left join ReceiptOrderD b on a.ReceiptOrderId=b.ReceiptOrderId where a.ReceiptOrderId = '" + RefTransID + "' and SeqNo = '" + RefTransSeqNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                Query = "INSERT INTO [dbo].[ReceiptOrder_LogTable] ([ReceiptOrderDate] ,[ReceiptOrderNo] ,[PurchaseOrderNo] ,[PurchaseOrderDate] ,[VendorID] ,[InventSiteID], [FullItemId] ,[SeqNo] ,[Qty_UoM] ,[Qty_Alt] ,[Amount], GoodsReceivedID ,[LogStatusCode] ,[LogStatusDesc] ,[LogDescription] ,[UserID] ,[LogDate]) VALUES ('" + Dr["ReceiptOrderDate"] + "', '" + Dr["ReceiptOrderId"] + "',  '" + Dr["PurchaseOrderId"] + "', '" + Dr["PurchaseOrderDate"] + "', '" + Dr["VendId"] + "', '" + Dr["InventSiteID"] + "', '" + Dr["FullItemId"] + "', '" + Dr["SeqNo"] + "', '" + Qty_SJ + "', '" + Ratio * Qty_SJ + "', '0', '" + tbxGRNum.Text + "'";
                if (Mode == "New" || (Mode == "Edit" && (GRStats == "01" || GRStats == "02" || GRStats == "05" || GRStats == "06")))
                    Query += ", '02', 'GR in Progress', 'GR in Progress'";
                else if ((Mode == "Edit" && GRStats == "03") || (Mode == "BeforeEdit" && GRStats == "05"))
                {
                    if (Convert.ToDecimal(Dr["RemainingQty"]) == 0)
                        Query += ", '03', 'Goods Received', 'Goods Received'";
                    else
                        Query += ", '04', 'Goods Received Partial', 'Goods Received Partial'";
                }
                else
                {
                    if (Convert.ToDecimal(Dr["RemainingQty"]) == 0)
                        Query += ", '03', 'Goods Received', 'Goods Received'";
                    else
                        Query += ", '04', 'Goods Received Partial', 'Goods Received Partial'";
                }
                Query += ", '" + ControlMgr.UserId + "', getdate())";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
            Dr.Close();
        }

        private void insertGRHeader()
        {
            Query = "insert [dbo].[GoodsReceivedH] ( [GoodsReceivedId], [GoodsReceivedDate], GoodsReceivedStatus, RefTransType, [RefTransID], [RefTransDate], [SJDate],[SJNumber], ExpectedDate, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, Timbang1Weight, [Timbang2Date],[Timbang2Weight] , Notes, SiteID, SiteName, SiteType, CreatedDate, CreatedBy, [StatusWeight1], UpdatedDate,[RefTrans2Id],VehicleOwnerID) VALUES (@GoodsReceivedId, @GoodsReceivedDate, @GoodsReceivedStatus, @RefTransType, @RefTransID, @RefTransDate, @SJDate, @SJNumber, @ExpectedDate, @VendId, @VendorName, @VehicleType, @VehicleNumber, @DriverName, @Timbang1Date, @Timbang1Weight, @Timbang2Date, @Timbang2Weight, @Notes, @SiteID, @SiteName, @SiteType, getdate(), @CreatedBy, @StatusWeight1, '1753-01-01',@RefTransid2,@VehicleOwnerID)";
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                Cmd.Parameters.AddWithValue("@GoodsReceivedDate", dtGR.Value);
                Cmd.Parameters.AddWithValue("@GoodsReceivedStatus", txtSiteType.Text.ToUpper() == "VIRTUAL SITE" ? "03" : "01");
                Cmd.Parameters.AddWithValue("@RefTransType", cmbReferenceType.Text);
                Cmd.Parameters.AddWithValue("@RefTransID", tbxRefID.Text);
                Cmd.Parameters.AddWithValue("@RefTransDate", dtRef.Value);
                Cmd.Parameters.AddWithValue("@SJDate", dtDO.Value);
                Cmd.Parameters.AddWithValue("@SJNumber", tbxDelivNum.Text.Trim());
                Cmd.Parameters.AddWithValue("@ExpectedDate", dtExpectedDate.Value);
                Cmd.Parameters.AddWithValue("@VendId", tbxNameID.Text);
                Cmd.Parameters.AddWithValue("@VendorName", tbxName.Text);
                Cmd.Parameters.AddWithValue("@VehicleType", tbxVType.Text.Trim());
                Cmd.Parameters.AddWithValue("@VehicleNumber", tbxVNumber.Text.Trim());
                Cmd.Parameters.AddWithValue("@DriverName", tbxDriverName.Text.Trim());
                Cmd.Parameters.AddWithValue("@Timbang1Date", dtWeight1.Value);
                Cmd.Parameters.AddWithValue("@Timbang1Weight", Convert.ToDecimal(tbxWeight1.Text));
                Cmd.Parameters.AddWithValue("@Timbang2Date", dtWeight2.Value);
                Cmd.Parameters.AddWithValue("@Timbang2Weight", 0);
                Cmd.Parameters.AddWithValue("@Notes", tbxNotes.Text.Trim());
                Cmd.Parameters.AddWithValue("@SiteID", txtInventSiteID.Text);
                Cmd.Parameters.AddWithValue("@SiteName", txtWarehouse.Text);
                Cmd.Parameters.AddWithValue("@SiteType", txtSiteType.Text);
                Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@RefTransid2", tbxRefId2.Text);
                Cmd.Parameters.AddWithValue("@VehicleOwnerID", tbxVOwnerID.Text);
                string StatusWeight1;
                if (cbWeight1.Checked == true)
                    StatusWeight1 = "Manual";
                else
                    StatusWeight1 = "Mesin";
                Cmd.Parameters.AddWithValue("@StatusWeight1", StatusWeight1);
                Cmd.ExecuteNonQuery();
            }
        }

        private void updateQtyToInventMovementQty(string FullItemId, decimal Qty_Actual, decimal GoodsReceivedSeqNo, decimal Ratio, string ActionCode, string Unit, int x)
        {
            Cmd = new SqlCommand("select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'", Conn);
            string grstatus = Cmd.ExecuteScalar().ToString();

            if ((cmbReferenceType.Text == "Nota Retur Beli" || cmbReferenceType.Text == "Nota Retur Jual") && ControlMgr.GroupName == "WB OPERATOR")
            {
                return;
            }

            //CHECK IF THERES ANY RESIZE
            if ((ControlMgr.GroupName == "WB OPERATOR" && grstatus == "03") || (ControlMgr.GroupName == "Purchase Admin"))
            {
                Query = "select * from InventTable where FullItemID = '" + FullItemId + "' and Resize = '1'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows && ActionCode.ToUpper() == "RECEIVED")
                {
                    ActionCode = "RESIZE";
                }
                //VIRTUAL SITE DONT NEED ANY INVENT MOVEMENT except theres a resized item
                else if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                {
                    return;
                }
                Dr.Close();
            }
            //parked and gr in progress header name  
            string nameUoMQty = "";
            string nameAltQty = "";
            string nameAmount = "";
            decimal price = 0;

            //qty buat ambil referenece ke qty GI //Khusus Nota Transfer
            decimal QtyGI = 0;
            string GIId = "";

            if (ActionCode.ToUpper() == "BONGKAR")
            {
                nameUoMQty = "GR_In_Progress_UoM";
                nameAltQty = "GR_In_Progress_Alt";
                nameAmount = "GR_In_Progress_Amount";
            }
            else if (ActionCode.ToUpper() == "PARKED - NEED ACTION" || ActionCode.ToUpper() == "PARK - NEED RESIZE" || ActionCode.ToUpper() == "PARKED TOLERANCE - NEED ACTION")
            {
                nameUoMQty = "Parked_For_Action_Outstanding_UoM";
                nameAltQty = "Parked_For_Action_Outstanding_Alt";
                nameAmount = "Parked_For_Action_Outstanding_Amount";
            }
            else if (ActionCode.ToUpper() == "RESIZE")
            {
                nameUoMQty = "Resize_In_Progress_UoM";
                nameAltQty = "Resize_In_Progress_Alt";
                nameAmount = "Resize_In_Progress_Amount";
            }
            if (cmbReferenceType.Text == "Nota Transfer" && ActionCode.ToUpper() != "REJECT" && gbWeight2.Visible == false)
            {
                nameUoMQty = "Transfer_Masuk_In_Progress_UoM";
                nameAltQty = "Transfer_Masuk_In_Progress_Alt";
                nameAmount = "Transfer_Masuk_In_Progress_Amount";
            }

            if ((dataGridView1.Rows[x].Cells["Ratio_Actual"].Value == System.DBNull.Value || dataGridView1.Rows[x].Cells["Ratio_Actual"].Value.ToString() == "" || Convert.ToDecimal(dataGridView1.Rows[x].Cells["Ratio_Actual"].Value) == 0))
            {
                Query = "SELECT Ratio FROM InventConversion WHERE FullItemID ='" + FullItemId + "';";
                Cmd = new SqlCommand(Query, Conn);
                Ratio = Convert.ToDecimal(Cmd.ExecuteScalar());
            }
            else if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" || grstatus != "01")
            {
                Ratio = Convert.ToDecimal(dataGridView1.Rows[x].Cells["Ratio_Actual"].Value);
            }
            else if (grstatus == "01")
            {
                Ratio = Convert.ToDecimal(dataGridView1.Rows[x].Cells["Ratio"].Value);
            }

            if (Ratio == 0)
            {
                Ratio = 1;
            }

            if (cmbReferenceType.Text.ToUpper() == "RECEIPT ORDER")
            {
                if (Unit.ToUpper() == "KG")
                {
                    Query = "SELECT Price_KG FROM ReceiptOrderD WHERE ReceiptOrderId = '" + tbxRefID.Text + "' and FullItemId = '" + FullItemId + "';";
                }
                else
                {
                    Query = "SELECT Price FROM ReceiptOrderD WHERE ReceiptOrderId = '" + tbxRefID.Text + "' and FullItemId = '" + FullItemId + "';";
                }
            }
            else
            {
                if (Unit.ToUpper() == "KG")
                {
                    Query = "SELECT Alt_AvgPrice FROM InventTable WHERE FullItemID = '" + FullItemId + "';";
                }
                else
                {
                    Query = "SELECT UoM_AvgPrice FROM InventTable WHERE FullItemID = '" + FullItemId + "';";
                }
            }
            Cmd = new SqlCommand(Query, Conn);
            if (Cmd.ExecuteScalar() != System.DBNull.Value)
            {
                price = Convert.ToDecimal(Cmd.ExecuteScalar());
            }

            decimal amount = price * Qty_Actual;
            if (Unit.ToUpper() == "KG")
            {
                Qty_Actual = Qty_Actual / Ratio;
            }

            //getting GI value only for NT Reference
            //if (cmbReferenceType.Text == "Nota Transfer" && x>=0)
            //{
            //    //Query = "SELECT * FROM [GoodsIssuedD] WHERE [GoodsIssuedId]='" + dataGridView1.Rows[x].Cells["RefTransID"].Value.ToString() + "' AND [GoodsIssuedSeqNo]=" + Convert.ToInt32(dataGridView1.Rows[x].Cells["RefTransSeqNo"].Value) + " AND [FullItemId]='"+FullItemId+"'";
            //    using (Cmd = new SqlCommand(Query, Conn))
            //    {
            //        Dr = Cmd.ExecuteReader();
            //        while (Dr.Read())
            //        {
            //            QtyGI = Convert.ToDecimal(Dr["Qty"]);
            //        }
            //        Dr.Close();
            //    }
            //    GIId = dataGridView1.Rows[x].Cells["RefTransID"].Value.ToString();
            //}

            //get data from InventMovementQty
            //Query = "select " + nameUoMQty + ", " + nameAltQty + ", " + nameAmount + " from Invent_Movement_Qty where FullItemId = '" + FullItemId + "'";
            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();
            //decimal GR_In_Progress_UoM = 0;
            //decimal GR_In_Progress_Alt = 0;
            //decimal GR_In_Progress_Amount = 0;
            //while (Dr.Read())
            //{
            //    GR_In_Progress_UoM = Convert.ToDecimal(Dr[nameUoMQty]);
            //    GR_In_Progress_Alt = Convert.ToDecimal(Dr[nameAltQty]);
            //    GR_In_Progress_Amount = Convert.ToDecimal(Dr[nameAmount]);
            //}
            //Dr.Close();

            decimal oldGRQty_SJ = 0;
            if (Mode != "New")
            {
                Query = "select Qty_SJ from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                oldGRQty_SJ = (decimal)Cmd.ExecuteScalar();
            }
            if (GoodsReceivedSeqNo == 0)
            {

            }
            //WB OPERATOR, WHEN NEW (GR IN PROGRESS)
            if ((ControlMgr.GroupName == "WB OPERATOR" && grstatus == "01" && Mode == "New") || (GoodsReceivedSeqNo == 0) || (txtSiteType.Text == "Virtual Site" && Mode == "New"))
            {
                Query = "SELECT * FROM Invent_Movement_Qty WHERE FullItemID = '" + FullItemId + "';";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    if (nameUoMQty != "" || nameAltQty != "" || nameAmount != "")
                    {
                        Query = "UPDATE Invent_Movement_Qty SET " + nameUoMQty + "=" + nameUoMQty + " + " + Qty_Actual + ", " + nameAltQty + "=" + nameAltQty + "+ " + (Ratio * Qty_Actual) + ", " + nameAmount + "= " + nameAmount + "+ " + amount + " ";
                        if (cmbReferenceType.Text == "Nota Transfer" && (ControlMgr.GroupName == "WB OPERATOR" && grstatus == "01" && Mode == "New"))
                        {
                            Query += " ,Transfer_Keluar_In_Progress_UoM -= " + Qty_Actual + ",Transfer_Keluar_In_Progress_Alt -= " + (Ratio * Qty_Actual) + ",Transfer_Keluar_In_Progress_Amount -= " + amount + " ";
                        }
                        Query += " WHERE FullItemID = '" + FullItemId + "' ";
                    }
                    //else if (cmbReferenceType.Text == "Nota Transfer" && ActionCode.ToUpper() == "REJECT")
                    //{
                    //    Query = "UPDATE Invent_Movement_Qty SET Transfer_Keluar_In_Progress_UoM -= " + Qty_Actual + ",Transfer_Keluar_In_Progress_Alt -= " + (Qty_Actual * Ratio) + ",Transfer_Keluar_In_Progress_Amount -= " + (amount) + " ";
                    //    Query += " ,[Transfer_In_Progress_UoM]+= " + QtyGI + ", [Transfer_In_Progress_Alt]+= " + (QtyGI * Ratio) + ", [Transfer_In_Progress_Amount]+= " + (QtyGI * price) + " ";
                    //}
                }
                else
                {
                    //if theres item not listed in invent_movement_qty table, this function will insert the new item in the table
                    //not tested , begin=============
                    int i = dataGridView1.CurrentRow.Index;
                    Query = "INSERT INTO [dbo].[Invent_Movement_Qty] ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemID] ,[ItemName] ,[GR_In_Progress_UoM],[GR_In_Progress_Alt],[GR_In_Progress_Amount],[Resize_In_Progress_UoM],[Resize_In_Progress_Alt],[Resize_In_Progress_Amount],[Parked_For_Action_Outstanding_UoM],[Parked_For_Action_Outstanding_Alt],[Parked_For_Action_Outstanding_Amount],[Transfer_In_Progress_UoM],[Transfer_In_Progress_Alt],[Transfer_In_Progress_Amount],[Transfer_Masuk_In_Progress_UoM],[Transfer_Masuk_In_Progress_Alt],[Transfer_Masuk_In_Progress_Amount],[Transfer_Keluar_In_Progress_UoM],[Transfer_Keluar_In_Progress_Alt],[Transfer_Keluar_In_Progress_Amount],[Adjustment_In_Progress_UoM],[Adjustment_In_Progress_Alt],[Adjustment_In_Progress_Amount],[Disposed_In_Progress_UoM],[Disposed_In_Progress_Alt],[Disposed_In_Progress_Amount]) VALUES ( '" + dataGridView1.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + "', '" + Qty_Actual + "', '" + Qty_Actual * Ratio + "', '" + amount + "', 0, 0,0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0)";
                    //end============================
                }
                Dr.Close();
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                return;
            }
            //WB OPERATOR, WHEN EDIT (GR IN PROGRESS)
            else //if (ControlMgr.GroupName == "WB OPERATOR" && Cmd.ExecuteScalar().ToString() == "01" && Mode == "Edit")
            {
                Query = "SELECT ActionCodeStatus FROM GoodsReceivedD WHERE GoodsReceivedId = '" + tbxGRNum.Text + "' AND FullItemId = '" + FullItemId + "' AND GoodsReceivedSeqNo  = '" + GoodsReceivedSeqNo + "' ";
                Cmd = new SqlCommand(Query, Conn);
                string oldAction = "";
                oldAction = Cmd.ExecuteScalar().ToString();

                Query = "SELECT Deskripsi FROM TransStatusTable WHERE TransCode = 'GRD' AND StatusCode = '" + oldAction + "'";
                Cmd = new SqlCommand(Query, Conn);
                oldAction = Cmd.ExecuteScalar().ToString();

                decimal oldGRQty = 0;
                if (ControlMgr.GroupName == "WB OPERATOR" || (ControlMgr.GroupName == "KERANI" && grstatus == "01"))
                {
                    Query = "Select Qty_SJ from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
                }
                else if (ControlMgr.GroupName == "KERANI" && grstatus == "02")
                {
                    Query = "Select Qty_Actual from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
                }
                else
                {
                    Query = "Select Qty_Actual from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
                }
                Cmd = new SqlCommand(Query, Conn);
                oldGRQty = (Decimal)Cmd.ExecuteScalar();

                //if edit and change action, old action is different from the current action
                if (oldAction != ActionCode)
                {
                    decimal oldamount = oldGRQty * price;
                    if (Unit.ToUpper() == "KG")
                    {
                        oldGRQty = oldGRQty / Ratio;
                    }
                    if ((cmbReferenceType.Text == "Nota Retur Beli" || cmbReferenceType.Text == "Nota Retur Jual") && ControlMgr.GroupName == "KERANI" && grstatus == "01")
                    {
                        oldGRQty = 0;
                        oldGRQty_SJ = 0;
                        oldamount = 0;
                    }
                    if (oldAction.ToUpper() == "BONGKAR" && cmbReferenceType.Text != "Nota Transfer")
                    {
                        Query = "UPDATE Invent_Movement_Qty SET GR_In_Progress_UoM=GR_In_Progress_UoM- " + oldGRQty + ", GR_In_Progress_Alt=GR_In_Progress_Alt- " + oldGRQty * Ratio + ", GR_In_Progress_Amount= GR_In_Progress_Amount- " + oldamount + " ";
                        if (ActionCode.ToUpper() != "REJECT" && ActionCode.ToUpper() != "RECEIVED")
                        {
                            Query += " , " + nameUoMQty + "=" + nameUoMQty + " + " + Qty_Actual + ", " + nameAltQty + "=" + nameAltQty + " + " + Qty_Actual * Ratio + ", " + nameAmount + "=" + nameAmount + "+ " + amount + " ";
                        }
                    }
                    else if (oldAction.ToUpper() == "PARKED - NEED ACTION" && cmbReferenceType.Text != "Nota Transfer")
                    {
                        Query = "UPDATE Invent_Movement_Qty SET Parked_For_Action_Outstanding_UoM = Parked_For_Action_Outstanding_UoM- " + oldGRQty + ", Parked_For_Action_Outstanding_Alt=Parked_For_Action_Outstanding_Alt- " + oldGRQty * Ratio + ", Parked_For_Action_Outstanding_Amount= Parked_For_Action_Outstanding_Amount- " + oldamount + " ";
                        if (ActionCode.ToUpper() != "REJECT" && ActionCode.ToUpper() != "RECEIVED")
                        {
                            Query += " , " + nameUoMQty + "=" + nameUoMQty + " + " + Qty_Actual + ", " + nameAltQty + "=" + nameAltQty + " + " + Qty_Actual * Ratio + ", " + nameAmount + "=" + nameAmount + "+ " + amount + " ";
                        }
                    }
                    else if (oldAction.ToUpper() == "RESIZE" && cmbReferenceType.Text != "Nota Transfer")
                    {
                        Query = "UPDATE Invent_Movement_Qty SET Resize_In_Progress_UoM = Resize_In_Progress_UoM- " + oldGRQty + ", Resize_In_Progress_Alt=Resize_In_Progress_Alt- " + oldGRQty * Ratio + ", Resize_In_Progress_Amount= Resize_In_Progress_Amount- " + oldamount + " ";
                        if (ActionCode.ToUpper() != "REJECT" && ActionCode.ToUpper() != "RECEIVED")
                        {
                            Query += " , " + nameUoMQty + "=" + nameUoMQty + " + " + Qty_Actual + ", " + nameAltQty + "=" + nameAltQty + " + " + Qty_Actual * Ratio + ", " + nameAmount + "=" + nameAmount + "+ " + amount + " ";
                        }
                    }
                    else if (oldAction.ToUpper() == "REJECT")
                    {
                        if (ActionCode.ToUpper() == "RECEIVED")
                        {
                            if (cmbReferenceType.Text == "Nota Transfer")
                            {
                                Query = "UPDATE Invent_Movement_Qty SET [Transfer_Keluar_In_Progress_UoM]-= " + Qty_Actual + ", [Transfer_Keluar_In_Progress_Alt]-= " + (Qty_Actual * Ratio) + ", [Transfer_Keluar_In_Progress_Amount]-= " + (amount) + " ";
                                Query += " , [Transfer_Masuk_In_Progress_UoM]+= " + Qty_Actual + ", [Transfer_Masuk_In_Progress_Alt]+= " + (Qty_Actual * Ratio) + ", [Transfer_Masuk_In_Progress_Amount]+=" + (amount) + " ";
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            Query = "UPDATE Invent_Movement_Qty SET " + nameUoMQty + "=" + nameUoMQty + " + " + Qty_Actual + ", " + nameAltQty + "=" + nameAltQty + " + " + Qty_Actual * Ratio + ", " + nameAmount + "=" + nameAmount + "+ " + amount + " ";
                        }
                    }
                    else if (oldAction.ToUpper() == "RECEIVED")
                    {
                        if (ActionCode.ToUpper() == "REJECT")
                        {
                            if (cmbReferenceType.Text == "Nota Transfer")
                            {
                                Query = "UPDATE Invent_Movement_Qty SET  ";
                                Query += " [Transfer_Masuk_In_Progress_UoM]-= " + oldGRQty + ", [Transfer_Masuk_In_Progress_Alt]-= " + (oldGRQty * Ratio) + ", [Transfer_Masuk_In_Progress_Amount]-=" + (oldamount) + " ";
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (cmbReferenceType.Text == "Nota Transfer")
                            {
                                Query = "UPDATE Invent_Movement_Qty SET Transfer_Masuk_In_Progress_UoM += " + (Qty_Actual - oldGRQty) + ", Transfer_Masuk_In_Progress_Alt += " + (Ratio * (Qty_Actual - oldGRQty)) + ", Transfer_Masuk_In_Progress_Amount += " + (amount - oldamount) + " ";
                                Query += " ,Transfer_Keluar_In_Progress_UoM -= " + (Qty_Actual - oldGRQty) + ",Transfer_Keluar_In_Progress_Alt -= " + (Ratio * (Qty_Actual - oldGRQty)) + ",Transfer_Keluar_In_Progress_Amount -= " + (amount - oldamount) + " ";
                            }
                            else
                            {
                                Query = "UPDATE Invent_Movement_Qty SET " + nameUoMQty + "=" + nameUoMQty + " + " + Qty_Actual + ", " + nameAltQty + "=" + nameAltQty + " + " + Qty_Actual * Ratio + ", " + nameAmount + "=" + nameAmount + "+ " + amount + " ";
                            }
                        }
                    }
                    else if (oldAction.ToUpper() != "REJECT" && oldAction.ToUpper() != "RECEIVED" && cmbReferenceType.Text == "Nota Transfer")
                    {
                        if (ActionCode.ToUpper() == "REJECT")
                        {
                            Query = "UPDATE Invent_Movement_Qty SET  ";
                            Query += " [Transfer_Masuk_In_Progress_UoM]-= " + oldGRQty + ", [Transfer_Masuk_In_Progress_Alt]-=" + (oldGRQty * Ratio) + ", [Transfer_Masuk_In_Progress_Amount]-=" + (oldamount) + " ";
                        }
                        else
                        {
                            Query = "UPDATE Invent_Movement_Qty SET Transfer_Masuk_In_Progress_UoM += " + (Qty_Actual - oldGRQty) + ", Transfer_Masuk_In_Progress_Alt += " + (Ratio * (Qty_Actual - oldGRQty)) + ", Transfer_Masuk_In_Progress_Amount += " + (amount - oldamount) + " ";
                            Query += " ,Transfer_Keluar_In_Progress_UoM -= " + (Qty_Actual - oldGRQty) + ",Transfer_Keluar_In_Progress_Alt -= " + (Ratio * (Qty_Actual - oldGRQty)) + ",Transfer_Keluar_In_Progress_Amount -= " + (amount - oldamount) + " ";
                        }
                    }
                    Query += " WHERE FullItemId = '" + FullItemId + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    return;
                }
                else
                {
                    if (ActionCode.ToUpper() == "REJECT")
                    {
                        return;
                    }

                    if (ActionCode.ToUpper() == "RECEIVED")
                    {
                        if (cmbReferenceType.Text != "Nota Transfer")
                        {
                            return;
                        }
                    }


                    if (!((cmbReferenceType.Text == "Nota Retur Beli" || cmbReferenceType.Text == "Nota Retur Jual") && ControlMgr.GroupName == "KERANI" && grstatus == "01"))
                    {
                        amount = amount - (oldGRQty * price);
                    }
                    if (Unit.ToUpper() == "KG")
                    {
                        oldGRQty = oldGRQty / Ratio;
                        if ((cmbReferenceType.Text == "Nota Retur Beli" || cmbReferenceType.Text == "Nota Retur Jual") && ControlMgr.GroupName == "KERANI" && grstatus == "01")
                        {
                            oldGRQty = 0;
                        }
                    }
                    //WB2, untuk referensi NT
                    if (cmbReferenceType.Text == "Nota Transfer" && ControlMgr.GroupName == "WB OPERATOR" && Mode == "Edit" && gbWeight2.Visible == true && grstatus == "02")
                    {
                        //Query = "UPDATE Invent_Movement_Qty SET " + nameUoMQty + "=" + nameUoMQty + " + " + (Qty_Actual) + ", " + nameAltQty + "=" + nameAltQty + " + " + (Ratio * Qty_Actual) + ", " + nameAmount + "= " + nameAmount + "+ " + (amount + (oldGRQty * price)) + " ";
                        //Query += " ,[Transfer_Masuk_In_Progress_UoM] -= " + Qty_Actual + ", [Transfer_Masuk_In_Progress_Alt] -= " + (Ratio * Qty_Actual) + ", [Transfer_Masuk_In_Progress_Amount] -= " + (amount + (oldGRQty * price)) + " ";
                        Query = " UPDATE Invent_Movement_Qty SET [Transfer_Masuk_In_Progress_UoM] -= " + Qty_Actual + ", [Transfer_Masuk_In_Progress_Alt] -= " + (Ratio * Qty_Actual) + ", [Transfer_Masuk_In_Progress_Amount] -= " + (amount + (oldGRQty * price)) + " ";
                        if (nameUoMQty != "" || nameAltQty != "" || nameAmount != "")
                        {
                            Query += " ," + nameUoMQty + "=" + nameUoMQty + " + " + (Qty_Actual) + ", " + nameAltQty + "=" + nameAltQty + " + " + (Ratio * Qty_Actual) + ", " + nameAmount + "= " + nameAmount + "+ " + (amount + (oldGRQty * price)) + " ";
                        }
                    }
                    //WB1, Edit, action code tidak berubah tapi qty berubah, untuk referensi NT
                    else if (cmbReferenceType.Text == "Nota Transfer" && ControlMgr.GroupName == "WB OPERATOR" && Mode == "Edit" && gbWeight2.Visible == false && grstatus == "01")
                    {
                        Query = "UPDATE Invent_Movement_Qty SET Transfer_Masuk_In_Progress_UoM += " + (Qty_Actual - oldGRQty) + ", Transfer_Masuk_In_Progress_Alt += " + (Ratio * (Qty_Actual - oldGRQty)) + ", Transfer_Masuk_In_Progress_Amount += " + (amount) + " ";
                        Query += " ,Transfer_Keluar_In_Progress_UoM -= " + (Qty_Actual - oldGRQty) + ",Transfer_Keluar_In_Progress_Alt -= " + (Ratio * (Qty_Actual - oldGRQty)) + ",Transfer_Keluar_In_Progress_Amount -= " + (amount) + " ";
                    }
                    //QC, action code tidak berubah tapi qty berubah
                    else
                    {
                        Query = "UPDATE Invent_Movement_Qty SET " + nameUoMQty + "=" + nameUoMQty + " + " + (Qty_Actual - oldGRQty) + ", " + nameAltQty + "=" + nameAltQty + " + " + ((Ratio * Qty_Actual) - (oldGRQty * Ratio)) + ", " + nameAmount + "= " + nameAmount + "+ " + amount + " ";
                        if (cmbReferenceType.Text == "Nota Transfer")
                        {
                            Query += " ,Transfer_Keluar_In_Progress_UoM -= " + (Qty_Actual - oldGRQty) + ",Transfer_Keluar_In_Progress_Alt -= " + (Ratio * (Qty_Actual - oldGRQty)) + ",Transfer_Keluar_In_Progress_Amount -= " + (amount) + " ";
                        }
                    }
                    Query += " WHERE FullItemID = '" + FullItemId + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                    return;
                }
            }
            //KERANI, WHEN TO QUALITY CHECK (GR IN PROGRESS)
            //else if (ControlMgr.GroupName == "KERANI" && Cmd.ExecuteScalar().ToString() == "01")
            //{

            //    Query = "Select Qty_SJ from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    decimal oldGRQty = (Decimal)Cmd.ExecuteScalar();
            //    GR_In_Progress_Amount = GR_In_Progress_Amount - (oldGRQty * price) + amount;
            //    if (Unit.ToUpper() == "KG")
            //    {
            //        oldGRQty = oldGRQty / Ratio;
            //    }
            //    GR_In_Progress_UoM = GR_In_Progress_UoM - oldGRQty ;
            //    GR_In_Progress_Alt = GR_In_Progress_Alt - (oldGRQty * Ratio) ;
            //}
            //else if (ControlMgr.GroupName == "KERANI" && Cmd.ExecuteScalar().ToString() == "02")
            //{
            //    Query = "Select Qty_Actual from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    decimal oldGRQty_Actual = (Decimal)Cmd.ExecuteScalar();
            //    GR_In_Progress_Amount = GR_In_Progress_Amount - (oldGRQty_Actual * price) + amount;
            //    if (Unit.ToUpper() == "KG")
            //    {
            //        oldGRQty_Actual = oldGRQty_Actual / Ratio;
            //    }
            //    GR_In_Progress_UoM = GR_In_Progress_UoM - oldGRQty_Actual + Qty_Actual;
            //    GR_In_Progress_Alt = GR_In_Progress_Alt - (oldGRQty_Actual * Ratio) + (Qty_Actual * Ratio);
            //}            
            //Query = "update Invent_Movement_Qty set " + nameUoMQty + " = '" + GR_In_Progress_UoM + "', " + nameAltQty + " = '" + GR_In_Progress_Alt + "', "+nameAmount+" = '" + GR_In_Progress_Amount + "' where FullItemId = '" + FullItemId + "'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Cmd.ExecuteNonQuery();
        }

        private void insertGRDetail()
        {
            //Edited, by Thaddaeus 15MAY2018,Begin
            //Query = "select count(GoodsReceivedSeqNo) from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Query = "select MAX(GoodsReceivedSeqNo) from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            decimal GoodsReceivedSeqNo = 0;
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                if (cmd.ExecuteScalar() != System.DBNull.Value)
                {
                    GoodsReceivedSeqNo = (Decimal)cmd.ExecuteScalar() + 1;
                }
                else
                {
                    GoodsReceivedSeqNo += 1;
                }
            }
            //end======

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //GET STATUS CODE
                string statCode = "";
                Query = "Select StatusCode from [dbo].[TransStatusTable] where [Deskripsi] = '" + dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value + "' and [TransCode] = 'GRD'";
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            statCode = Convert.IsDBNull(Dr["StatusCode"]) ? "" : (string)Dr["StatusCode"];
                        }
                    }
                    Dr.Close();
                }
                //edited by Thaddaeus, 15 MAY 2018, begin
                string RefidTrans = "";
                int SeqNoTrans = 0;
                if (cmbReferenceType.Text == "Nota Transfer")
                {
                    Query = "SELECT [RefTransID], [RefTransSeqNo] FROM [GoodsIssuedD] WHERE [GoodsIssuedId]='" + dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString() + "' AND [GoodsIssuedSeqNo]=" + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value.ToString() + "";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            RefidTrans = Dr["RefTransID"].ToString();
                            SeqNoTrans = Convert.ToInt32(Dr["RefTransSeqNo"]);
                        }
                        Dr.Close();
                    }
                }
                //end

                decimal price = 0;
                decimal total = 0;
                decimal totalDisc = 0;
                decimal ppn = 0;
                decimal totalppn = 0;
                decimal pph = 0;
                decimal totalpph = 0;
                switch (cmbReferenceType.Text)
                {
                    case "Receipt Order":
                        string queryprice = "SELECT [UoM_AvgPrice] FROM [ISBS-NEW4].[dbo].[InventTable] WHERE [FullItemID] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "'";
                        using (Cmd = new SqlCommand(queryprice, Conn))
                        {
                            price = Convert.ToDecimal(Cmd.ExecuteScalar());
                        }
                        Query = "select ISNULL(b.Price,0) as 'Price', ISNULL(b.Diskon,0) as 'Diskon', ISNULL(c.PPN,0) as 'PPN', ISNULL(c.PPH,0) as 'PPH' from ReceiptOrderD a left join PurchDtl b on a.PurchaseOrderId = b.PurchID and a.PurchaseOrderSeqNo = b.SeqNo left join PurchH c on b.PurchID = c.PurchID where a.ReceiptOrderId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            if (Convert.ToDecimal(Dr["Price"]) != 0)
                            {
                                price = Convert.ToDecimal(Dr["Price"]);
                            }
                            totalDisc = Convert.ToDecimal(Dr["Diskon"]) * price / 100;
                            ppn = Convert.ToDecimal(Dr["PPN"]);
                            totalppn = (price - totalDisc) * ppn / 100;
                            pph = Convert.ToDecimal(Dr["PPH"]);
                            totalpph = (price - totalDisc) * pph / 100;
                        }
                        Dr.Close();
                        break;
                    case "Nota Transfer":
                        Query = "select COGS from NotaTransferD where TransferNo = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        Cmd = new SqlCommand(Query, Conn);
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                        totalDisc = 0;
                        ppn = 0;
                        totalppn = 0;
                        pph = 0;
                        totalpph = 0;
                        break;
                    case "Nota Retur Jual":
                        Query = "select d.Price, d.DiscPercent, e.PPN, e.PPH from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            if (Dr["Price"] == System.DBNull.Value)
                            {
                                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                                Cmd = new SqlCommand(Query, Conn);
                                {
                                    Cmd.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                                }
                            }
                            else
                            {
                                price = Convert.ToDecimal(Dr["Price"]);
                                totalDisc = price * Convert.ToDecimal(Dr["DiscPercent"]) / 100;
                                ppn = Convert.ToDecimal(Dr["PPN"]);
                                totalppn = (price - totalDisc) * ppn / 100;
                                pph = Convert.ToDecimal(Dr["PPH"]);
                                totalpph = (price - totalDisc) * pph / 100;
                            }
                        }
                        Dr.Close();
                        break;
                    case "Nota Retur Beli":
                        Query = "select d.Price, d.[Total_Disk], e.PPN, e.PPH from [NotaReturBeli_Dtl] a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.[NRBId] = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and  a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value + " ";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            if (Dr["Price"] == System.DBNull.Value)
                            {
                                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                                Cmd = new SqlCommand(Query, Conn);
                                {
                                    Cmd.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                                }
                            }
                            else
                            {
                                price = Convert.ToDecimal(Dr["Price"]);
                                totalDisc = Convert.ToDecimal(Dr["Total_Disk"]);
                                ppn = Convert.ToDecimal(Dr["PPN"]);
                                totalppn = (price - totalDisc) * ppn / 100;
                                pph = Convert.ToDecimal(Dr["PPH"]);
                                totalpph = (price - totalDisc) * pph / 100;

                            }
                        }
                        Dr.Close();
                        break;
                }

                Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                SqlCommand Cmd3 = new SqlCommand(Query, Conn);
                string GRStats = Cmd3.ExecuteScalar().ToString();

                if (ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))
                    total = price * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                else
                    total = price * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);

                Query = "insert into [dbo].[GoodsReceivedD] ([GoodsReceivedDate],[GoodsReceivedId],[GoodsReceivedSeqNo],[RefTransID],[RefTransSeqNo],[RefTrans2Id],[RefTrans2SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[Qty],[Qty_SJ],[Qty_Actual],[Unit],[Ratio],TotalBerat,[Ratio_Actual],[InventSiteId],[InventSiteBlokID],[Quality],[Notes],[ActionCodeStatus],[CreatedDate],[CreatedBy],[DeliveryMethod], updatedDate, Price, Total, Total_Discount, PPN, Total_PPN, PPH, Total_PPH) values (@GoodsReceivedDate, @GoodsReceivedId, @GoodsReceivedSeqNo, @RefTransID, @RefTransSeqNo,@RefTrans2Id,@RefTrans2SeqNo, @GroupId, @SubGroup1Id, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @Qty, @Qty_SJ, @Qty_Actual, @Unit, @Ratio, @TotalBerat, @Ratio_Actual, @InventSiteId, @InventSiteBlokID, @Quality, @Notes, @ActionCodeStatus, getdate(), @CreatedBy, @DeliveryMethod, '1753-01-01', @Price, @Total, @Total_Discount, @PPN, @Total_PPN, @PPH, @Total_PPH)";
                using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@GoodsReceivedDate", dtGR.Value);
                    Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                    Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", GoodsReceivedSeqNo);//dataGridView1.Rows[i].Cells["No"].Value.ToString());
                    //edited by Thaddaeus, 15 MAY 2018, begin
                    string refid = "";
                    int seqnoid = 0;
                    string refid2 = "";
                    int seqnoid2 = 0;
                    if (cmbReferenceType.Text == "Nota Transfer")
                    {
                        refid = RefidTrans;
                        seqnoid = SeqNoTrans;
                        refid2 = dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString();
                        seqnoid2 = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);
                    }
                    else
                    {
                        refid = dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString();
                        seqnoid = Convert.ToInt32(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);
                    }
                    Cmd.Parameters.AddWithValue("@RefTransID", refid);
                    Cmd.Parameters.AddWithValue("@RefTransSeqNo", seqnoid);
                    Cmd.Parameters.AddWithValue("@RefTrans2Id", refid2);
                    Cmd.Parameters.AddWithValue("@RefTrans2SeqNo", seqnoid2);
                    //end=================================================
                    Cmd.Parameters.AddWithValue("@GroupId", dataGridView1.Rows[i].Cells["GroupId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@SubGroup1Id", dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@SubGroup2Id", dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@ItemId", dataGridView1.Rows[i].Cells["ItemId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@FullItemId", dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@ItemName", dataGridView1.Rows[i].Cells["ItemName"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Qty", dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value));
                    Cmd.Parameters.AddWithValue("@Qty_SJ", dataGridView1.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value));
                    if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                    {
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView1.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value));
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView1.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                    }
                    Cmd.Parameters.AddWithValue("@Unit", dataGridView1.Rows[i].Cells["Unit"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Ratio", Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value));
                    if (dataGridView1.Rows[i].Cells["TotalBerat"].Value == "")
                        dataGridView1.Rows[i].Cells["TotalBerat"].Value = "0";
                    Cmd.Parameters.AddWithValue("@TotalBerat", Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat"].Value));
                    Cmd.Parameters.AddWithValue("@Ratio_Actual", Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value));
                    Cmd.Parameters.AddWithValue("@InventSiteId", dataGridView1.Rows[i].Cells["InventSiteId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@InventSiteBlokID", dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Quality", (object)DBNull.Value);
                    if (dataGridView1.Rows[i].Cells["Notes"].Value == null)
                    {
                        Cmd.Parameters.AddWithValue("@Notes", "");
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Notes", dataGridView1.Rows[i].Cells["Notes"].Value.ToString().Trim());
                    }
                    Cmd.Parameters.AddWithValue("@ActionCodeStatus", statCode);
                    Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                    Cmd.Parameters.AddWithValue("@DeliveryMethod", (object)DBNull.Value);
                    Cmd.Parameters.AddWithValue("@Price", price);
                    Cmd.Parameters.AddWithValue("@Total", total);
                    Cmd.Parameters.AddWithValue("@Total_Discount", totalDisc);
                    Cmd.Parameters.AddWithValue("@PPN", ppn);
                    Cmd.Parameters.AddWithValue("@Total_PPN", totalppn);
                    Cmd.Parameters.AddWithValue("@PPH", pph);
                    Cmd.Parameters.AddWithValue("@Total_PPH", totalpph);
                    Cmd.ExecuteNonQuery();
                }
                GoodsReceivedSeqNo++;
            }
            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                string statCode = "";
                //GET STATUS CODE
                Query = "Select StatusCode from [dbo].[TransStatusTable] where [Deskripsi] = '" + dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value + "' and [TransCode] = 'GRD'";
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            statCode = Convert.IsDBNull(Dr["StatusCode"]) ? "" : (string)Dr["StatusCode"];
                        }
                    }
                    Dr.Close();
                }

                Query = "insert into [dbo].[GoodsReceivedD] ([GoodsReceivedDate],[GoodsReceivedId],[GoodsReceivedSeqNo],[RefTransID],[RefTransSeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[Qty],[Qty_SJ],[Qty_Actual],[Unit],[Ratio],TotalBerat,[Ratio_Actual],[InventSiteId],[InventSiteBlokID],[Quality],[Notes],[ActionCodeStatus],[CreatedDate],[CreatedBy],[DeliveryMethod], updatedDate) values (@GoodsReceivedDate, @GoodsReceivedId, @GoodsReceivedSeqNo, @RefTransID, @RefTransSeqNo, @GroupId, @SubGroup1Id, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @Qty, @Qty_SJ, @Qty_Actual, @Unit, @Ratio, @TotalBerat, @Ratio_Actual, @InventSiteId, @InventSiteBlokID, @Quality, @Notes, @ActionCodeStatus, getdate(), @CreatedBy, @DeliveryMethod, '1753-01-01')";
                using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                {
                    Cmd.Parameters.AddWithValue("@GoodsReceivedDate", dtGR.Value);
                    Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                    Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", GoodsReceivedSeqNo);//dataGridView3.Rows[i].Cells["No"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@RefTransID", dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@RefTransSeqNo", dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@GroupId", dataGridView3.Rows[i].Cells["GroupId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@SubGroup1Id", dataGridView3.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@SubGroup2Id", dataGridView3.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@ItemId", dataGridView3.Rows[i].Cells["ItemId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@FullItemId", dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@ItemName", dataGridView3.Rows[i].Cells["ItemName"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Qty", dataGridView3.Rows[i].Cells["Qty"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty"].Value));
                    Cmd.Parameters.AddWithValue("@Qty_SJ", dataGridView3.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value));
                    if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                    {
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView3.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value));
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView3.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value));
                    }
                    Cmd.Parameters.AddWithValue("@Unit", dataGridView3.Rows[i].Cells["Unit"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Ratio", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value));
                    Cmd.Parameters.AddWithValue("@TotalBerat", dataGridView3.Rows[i].Cells["TotalBerat"].Value == "" ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat"].Value));
                    Cmd.Parameters.AddWithValue("@Ratio_Actual", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value));
                    Cmd.Parameters.AddWithValue("@InventSiteId", dataGridView3.Rows[i].Cells["InventSiteId"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@InventSiteBlokID", dataGridView3.Rows[i].Cells["InventSiteBlokID"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Quality", (object)DBNull.Value);
                    if (dataGridView3.Rows[i].Cells["Notes"].Value == null)
                    {
                        Cmd.Parameters.AddWithValue("@Notes", "");
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Notes", dataGridView3.Rows[i].Cells["Notes"].Value.ToString());
                    }
                    Cmd.Parameters.AddWithValue("@ActionCodeStatus", statCode);
                    Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                    Cmd.Parameters.AddWithValue("@DeliveryMethod", (object)DBNull.Value);
                    Cmd.ExecuteNonQuery();
                }
                GoodsReceivedSeqNo++;
            }
        }

        private void updateGRHeader(string GRStats)
        {
            Query = "update [dbo].[GoodsReceivedH] set [SJDate] = @SJDate, [ExpectedDate] = @ExpectedDate, [VendId] = @VendId, [VendorName] = @VendorName, [VehicleType] = @VehicleType, [VehicleNumber] = @VehicleNumber, [DriverName] = @DriverName, [Timbang1Date] = @Timbang1Date, [Timbang1Weight] = @Timbang1Weight, [Timbang2Date] = @Timbang2Date, [SiteID] = @SiteID, [SiteName] = @SiteName, [SiteType] = @SiteType, [SJNumber] = @SJNumber ";
            if (tbxWeight2.Text != String.Empty)
                Query += ", [Timbang2Weight] = @Timbang2Weight ";
            Query += ", [Notes] = @Notes, [UpdatedDate] = getdate(), [UpdatedBy] = @UpdatedBy ";

            if ((GRStats == "02" || GRStats == "03") && ControlMgr.GroupName == "WB OPERATOR")
            {
                decimal GRatioActual = 0;
                decimal TotalBerat_Actual = 0;
                decimal RejectedWeight = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    TotalBerat_Actual = dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString());
                    GRatioActual += TotalBerat_Actual;
                    //edited by Thaddaeus, 3JULY2018, rejected item gonna be in the weigh2
                    if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "REJECT")
                    {
                        RejectedWeight += TotalBerat_Actual;
                    }
                }
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    TotalBerat_Actual = dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString());
                    GRatioActual += TotalBerat_Actual;
                    if (dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "REJECT")
                    {
                        RejectedWeight += TotalBerat_Actual;
                    }
                }
                decimal weightTolerance = ((Convert.ToDecimal(tbxWeight1.Text) - GRatioActual) - (Convert.ToDecimal(tbxWeight2.Text) - RejectedWeight)) / (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual) * 100;
                if (Math.Abs(weightTolerance) > 1)
                {
                    Query += ", [GoodsReceivedStatus] = '05' ";
                }
                else
                {
                    Query += ", [GoodsReceivedStatus] = '03' ";
                }

                if (cbWeight2.Checked == true)
                    Query += ", [StatusWeight2] = 'Manual' ";
                else
                    Query += ", [StatusWeight2] = 'Mesin' ";

            }
            else if ((GRStats == "01" || GRStats == "02") && ControlMgr.GroupName == "KERANI")
            {
                Query += ", [GoodsReceivedStatus] = '02' ";
            }
            Query += "where [GoodsReceivedId] = @GoodsReceivedId; ";
            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@SJDate", dtDO.Value);
                Cmd.Parameters.AddWithValue("@ExpectedDate", dtExpectedDate.Value);
                Cmd.Parameters.AddWithValue("@VendId", tbxVOwnerID.Text);
                Cmd.Parameters.AddWithValue("@VendorName", tbxVOwner.Text);
                Cmd.Parameters.AddWithValue("@VehicleType", tbxVType.Text.Trim());
                Cmd.Parameters.AddWithValue("@VehicleNumber", tbxVNumber.Text.Trim());
                Cmd.Parameters.AddWithValue("@DriverName", tbxDriverName.Text.Trim());
                Cmd.Parameters.AddWithValue("@Timbang1Date", dtWeight1.Value);
                Cmd.Parameters.AddWithValue("@Timbang1Weight", Convert.ToDecimal(tbxWeight1.Text));
                Cmd.Parameters.AddWithValue("@Timbang2Date", dtWeight2.Value);
                Cmd.Parameters.AddWithValue("@SiteID", txtInventSiteID.Text.Trim());
                Cmd.Parameters.AddWithValue("@SiteName", txtWarehouse.Text.Trim());
                Cmd.Parameters.AddWithValue("@SiteType", txtSiteType.Text.Trim());
                Cmd.Parameters.AddWithValue("@SJNumber", tbxDelivNum.Text.Trim());
                if (tbxWeight2.Text != String.Empty)
                    Cmd.Parameters.AddWithValue("@Timbang2Weight", Convert.ToDecimal(tbxWeight2.Text));
                Cmd.Parameters.AddWithValue("@Notes", tbxNotes.Text.Trim());
                Cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                Cmd.ExecuteNonQuery();
            }
        }

        private void updateGRDetail()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //Createdby Thadaeus, 3JULY2018
                decimal tempSeqNo = dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Reject")
                {
                    Query = "SELECT b.[GroupName] FROM [dbo].[GoodsReceivedD] a LEFT JOIN [dbo].[sysUserGroup] b ON a.UpdatedBy = b.[UserID] WHERE a.[GoodsReceivedId] = '" + tbxGRNum.Text + "' AND a.[GoodsReceivedSeqNo] = " + tempSeqNo + "";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {

                    }
                    else
                    {
                        while (Dr.Read())
                        {
                            if (ControlMgr.GroupName == "KERANI" && Mode == "Edit" && Dr["GroupName"].ToString() != "KERANI")
                            {
                                continue;
                            }
                        }
                    }
                    Dr.Close();
                }
                //END=============================

                //GET STATUS CODE
                string statCode = "";
                Query = "Select StatusCode from [dbo].[TransStatusTable] where [Deskripsi] = '" + dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value + "' and [TransCode] = 'GRD'";
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            statCode = Convert.IsDBNull(Dr["StatusCode"]) ? "" : (string)Dr["StatusCode"];
                        }
                    }
                    Dr.Close();
                }

                decimal price = 0;
                decimal total = 0;
                decimal totalDisc = 0;
                decimal ppn = 0;
                decimal totalppn = 0;
                decimal pph = 0;
                decimal totalpph = 0;
                switch (cmbReferenceType.Text)
                {
                    case "Receipt Order":

                        Query = "select b.Price, b.Diskon, c.PPN, c.PPH from ReceiptOrderD a left join PurchDtl b on a.PurchaseOrderId = b.PurchID and a.PurchaseOrderSeqNo = b.SeqNo left join PurchH c on b.PurchID = c.PurchID where a.ReceiptOrderId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            price = Convert.ToDecimal(Dr["Price"]);
                            totalDisc = Convert.ToDecimal(Dr["Diskon"]) * price / 100;
                            ppn = Convert.ToDecimal(Dr["PPN"]);
                            totalppn = (price - totalDisc) * ppn / 100;
                            pph = Convert.ToDecimal(Dr["PPH"]);
                            totalpph = (price - totalDisc) * pph / 100;
                        }
                        Dr.Close();
                        break;
                    case "Nota Transfer":
                        Query = "select COGS from NotaTransferD where TransferNo = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        Cmd = new SqlCommand(Query, Conn);
                        price = Convert.ToDecimal(Cmd.ExecuteScalar());
                        totalDisc = 0;
                        ppn = 0;
                        totalppn = 0;
                        pph = 0;
                        totalpph = 0;
                        break;
                    case "Nota Retur Jual":
                        Query = "select b.RefTransID from NotaReturJual_Dtl a left join [GoodsIssuedD] b on a.[GoodsIssuedId] = b.[GoodsIssuedId] and a.[GoodsIssued_SeqNo] = b.[GoodsIssuedSeqNo] where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            if (Dr["RefTransID"] == System.DBNull.Value)
                            {
                                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                {
                                    Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                    price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                                }
                            }
                            else if (Dr["RefTransID"].ToString().Split('/')[0] == "DO" || Dr["RefTransID"].ToString().Split('/')[0] == "DOA")
                            {
                                Query = "select d.Price, d.DiscPercent, e.PPN, e.PPH from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                SqlDataReader Dr2 = Cmd2.ExecuteReader();
                                while (Dr2.Read())
                                {
                                    price = Convert.ToDecimal(Dr2["Price"]);
                                    totalDisc = price * Convert.ToDecimal(Dr2["DiscPercent"]) / 100;
                                    ppn = Convert.ToDecimal(Dr2["PPN"]);
                                    totalppn = (price - totalDisc) * ppn / 100;
                                    pph = Convert.ToDecimal(Dr2["PPH"]);
                                    totalpph = (price - totalDisc) * pph / 100;
                                }
                                Dr2.Close();
                            }
                            else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                            {
                                Query = "select COGS from NotaTransferD where TransferNo = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                            else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRB")
                            {
                                Query = "select d.Price, d.Diskon, e.PPN, e.PPH from NotaReturBeli_Dtl a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.NRBId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                SqlDataReader Dr2 = Cmd2.ExecuteReader();
                                while (Dr2.Read())
                                {
                                    price = Convert.ToDecimal(Dr2["Price"]);
                                    totalDisc = price * Convert.ToDecimal(Dr2["Diskon"]) / 100;
                                    ppn = Convert.ToDecimal(Dr2["PPN"]);
                                    totalppn = (price - totalDisc) * ppn / 100;
                                    pph = Convert.ToDecimal(Dr2["PPH"]);
                                    totalpph = (price - totalDisc) * pph / 100;
                                }
                                Dr2.Close();
                            }
                            else
                            {
                                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                {
                                    Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                    price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                                }
                            }
                        }
                        break;
                    case "Nota Retur Beli":
                        Query = "select b.RefTransID from NotaReturBeli_Dtl a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId and a.GoodsReceived_SeqNo = b.GoodsReceivedSeqNo where a.NRBId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            if (Dr["RefTransID"] == System.DBNull.Value)
                            {
                                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                {
                                    Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                    price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                                }
                            }
                            else if (Dr["RefTransID"].ToString().Split('/')[0] == "RO" || Dr["RefTransID"].ToString().Split('/')[0] == "ROA")
                            {
                                Query = "select d.Price, d.[Total_Disk], e.PPN, e.PPH from [NotaReturBeli_Dtl] a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.[NRBId] = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and  a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value + "";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                SqlDataReader Dr2 = Cmd2.ExecuteReader();
                                while (Dr2.Read())
                                {
                                    price = Convert.ToDecimal(Dr2["Price"]);
                                    totalDisc = Convert.ToDecimal(Dr2["Total_Disk"]);
                                    ppn = Convert.ToDecimal(Dr2["PPN"]);
                                    totalppn = (price - totalDisc) * ppn / 100;
                                    pph = Convert.ToDecimal(Dr2["PPH"]);
                                    totalpph = (price - totalDisc) * pph / 100;
                                }
                                Dr2.Close();
                            }
                            else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                            {
                                Query = "select COGS from NotaTransferD where TransferNo = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                            else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRJ")
                            {
                                Query = "select d.Price, d.DiscPercent, e.PPN, e.PPH from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                SqlDataReader Dr2 = Cmd2.ExecuteReader();
                                while (Dr2.Read())
                                {
                                    price = Convert.ToDecimal(Dr2["Price"]);
                                    totalDisc = price * Convert.ToDecimal(Dr2["DiscPercent"]) / 100;
                                    ppn = Convert.ToDecimal(Dr2["PPN"]);
                                    totalppn = (price - totalDisc) * ppn / 100;
                                    pph = Convert.ToDecimal(Dr2["PPH"]);
                                    totalpph = (price - totalDisc) * pph / 100;
                                }
                                Dr2.Close();
                            }
                            else
                            {
                                Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                                SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                                {
                                    Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                    price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                                }
                            }
                        }
                        break;
                }

                Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                SqlCommand Cmd3 = new SqlCommand(Query, Conn);
                string GRStats = Cmd3.ExecuteScalar().ToString();

                if (ControlMgr.GroupName == "WB OPERATOR" && (GRStats == "02" || GRStats == "03"))
                    total = price * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                else
                    total = price * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);

                if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null || Convert.ToDecimal(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value) == 0)
                {
                    decimal GoodsReceivedSeqNo = 0;
                    Query = "Select MAX(GoodsReceivedSeqNo) from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        GoodsReceivedSeqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;
                    }
                    decimal GoodsReceivedSeqNoDecimal = 0;
                    if (statCode == "12")
                    {
                        int k = ChildParkedRow.IndexOf(i, 0);
                        GoodsReceivedSeqNoDecimal = Convert.ToDecimal(dataGridView1.Rows[ParentParkedRow[k]].Cells["GoodsReceivedSeqNo"].Value) + Convert.ToDecimal(0.01);
                    }
                    //INSERT QUERY GoodsReceivedD
                    Query = "insert into [dbo].[GoodsReceivedD] ([GoodsReceivedDate],[GoodsReceivedId],[GoodsReceivedSeqNo],[RefTransID],[RefTransSeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[Qty],[Qty_SJ],[Qty_Actual],[Unit],[Ratio],TotalBerat,TotalBerat_Actual,[Ratio_Actual],[InventSiteId],[InventSiteBlokID],[Quality],[Notes],[ActionCodeStatus],[CreatedDate],[CreatedBy],[DeliveryMethod], Remaining_Qty, Price, Total, Total_Discount, PPN, Total_PPN, PPH, Total_PPH) values (@GoodsReceivedDate, @GoodsReceivedId, @GoodsReceivedSeqNo, @RefTransID, @RefTransSeqNo, @GroupId, @SubGroup1Id, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @Qty, @Qty_SJ, @Qty_Actual, @Unit, @Ratio, @TotalBerat, @TotalBerat_Actual, @Ratio_Actual, @InventSiteId, @InventSiteBlokID, @Quality, @Notes, @ActionCodeStatus, getdate(), @CreatedBy, @DeliveryMethod, @Remaining_Qty, @Price, @Total, @Total_Discount, @PPN, @Total_PPN, @PPH, @Total_PPH)";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@GoodsReceivedDate", dtGR.Value);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                        if (statCode == "12")
                        {
                            Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", GoodsReceivedSeqNoDecimal);
                        }
                        else
                        {
                            Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", GoodsReceivedSeqNo);//dataGridView1.Rows[i].Cells["No"].Value.ToString());
                        }
                        Cmd.Parameters.AddWithValue("@RefTransID", dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@RefTransSeqNo", dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@GroupId", dataGridView1.Rows[i].Cells["GroupId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@SubGroup1Id", dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@SubGroup2Id", dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@ItemId", dataGridView1.Rows[i].Cells["ItemId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@FullItemId", dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@ItemName", dataGridView1.Rows[i].Cells["ItemName"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@Qty", dataGridView1.Rows[i].Cells["Qty"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value));
                        Cmd.Parameters.AddWithValue("@Qty_SJ", dataGridView1.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value));
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView1.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@Unit", dataGridView1.Rows[i].Cells["Unit"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@Ratio", dataGridView1.Rows[i].Cells["Ratio"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@TotalBerat", dataGridView1.Rows[i].Cells["TotalBerat"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat"].Value));
                        Cmd.Parameters.AddWithValue("@TotalBerat_Actual", dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@Ratio_Actual", dataGridView1.Rows[i].Cells["Ratio_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@InventSiteId", dataGridView1.Rows[i].Cells["InventSiteId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@InventSiteBlokID", dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value.ToString());
                        string QualityID = String.Empty;
                        if (dataGridView1.Rows[i].Cells["Quality"].Value != "Select")
                        {
                            SqlCommand Cmd2 = new SqlCommand("Select [QualityID] From [dbo].[InventQuality] where Deskripsi = '" + dataGridView1.Rows[i].Cells["Quality"].Value + "'", Conn);
                            QualityID = Cmd2.ExecuteScalar().ToString();
                        }
                        Cmd.Parameters.AddWithValue("@Quality", dataGridView1.Rows[i].Cells["Quality"].Value == "Select" ? (object)DBNull.Value : QualityID);
                        Cmd.Parameters.AddWithValue("@Notes", dataGridView1.Rows[i].Cells["Notes"].Value.ToString().Trim());
                        Cmd.Parameters.AddWithValue("@ActionCodeStatus", statCode);
                        Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@DeliveryMethod", (object)DBNull.Value);
                        Cmd.Parameters.AddWithValue("@Remaining_Qty", dataGridView1.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value) : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@Price", price);
                        Cmd.Parameters.AddWithValue("@Total", total);
                        Cmd.Parameters.AddWithValue("@Total_Discount", totalDisc);
                        Cmd.Parameters.AddWithValue("@PPN", ppn);
                        Cmd.Parameters.AddWithValue("@Total_PPN", totalppn);
                        Cmd.Parameters.AddWithValue("@PPH", pph);
                        Cmd.Parameters.AddWithValue("@Total_PPH", totalpph);
                        Cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // UPDATE QUERY GoodsReceivedD
                    Query = "update [dbo].[GoodsReceivedD] set [Qty_SJ] = @Qty_SJ, InventSiteBlokID = @InventSiteBlokID, ActionCodeStatus = @ActionCodeStatus, Notes = @Notes, Qty_Actual = @Qty_Actual, Remaining_Qty = @Remaining_Qty, TotalBerat_Actual = @TotalBerat_Actual, Quality = @Quality, UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "', Price = @Price, Total = @Total, Total_Discount = @Total_Discount, PPN = @PPN, Total_PPN = @Total_PPN, PPH = @PPH, Total_PPH = @Total_PPH where [GoodsReceivedId] = @GoodsReceivedId and [FullItemId] = @FullItemId and [GoodsReceivedSeqNo] = @GoodsReceivedSeqNo; ";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@Qty_SJ", dataGridView1.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value));
                        Cmd.Parameters.AddWithValue("@InventSiteBlokID", dataGridView1.Rows[i].Cells["InventSiteBlokID"].Value);
                        Cmd.Parameters.AddWithValue("@ActionCodeStatus", statCode);
                        Cmd.Parameters.AddWithValue("@Notes", dataGridView1.Rows[i].Cells["Notes"].Value.ToString().Trim());
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView1.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@Remaining_Qty", dataGridView1.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@TotalBerat_Actual", dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value));

                        string QualityID = String.Empty;
                        if (!(dataGridView1.Rows[i].Cells["Quality"].Value == "Select" || dataGridView1.Rows[i].Cells["Quality"].Value == String.Empty))
                        {
                            SqlCommand Cmd2 = new SqlCommand("Select [QualityID] From [dbo].[InventQuality] where Deskripsi = '" + dataGridView1.Rows[i].Cells["Quality"].Value + "'", Conn);
                            QualityID = Cmd2.ExecuteScalar().ToString();
                            Cmd.Parameters.AddWithValue("@Quality", QualityID);
                        }
                        else
                            Cmd.Parameters.AddWithValue("@Quality", (object)DBNull.Value);
                        //Cmd.Parameters.AddWithValue("@Quality", dataGridView1.Rows[i].Cells["Quality"].Value == "Select" ? (object)DBNull.Value : QualityID);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                        Cmd.Parameters.AddWithValue("@FullItemId", dataGridView1.Rows[i].Cells["FullItemId"].Value);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                        Cmd.Parameters.AddWithValue("@Price", price);
                        Cmd.Parameters.AddWithValue("@Total", total);
                        Cmd.Parameters.AddWithValue("@Total_Discount", totalDisc);
                        Cmd.Parameters.AddWithValue("@PPN", ppn);
                        Cmd.Parameters.AddWithValue("@Total_PPN", totalppn);
                        Cmd.Parameters.AddWithValue("@PPH", pph);
                        Cmd.Parameters.AddWithValue("@Total_PPH", totalpph);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }

            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                //Createdby Thadaeus, 3JULY2018
                decimal tempSeqNo = dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                if (dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Reject")
                {
                    Query = "SELECT UpdatedBy FROM [dbo].[GoodsReceivedD] WHERE [GoodsReceivedId] = '" + tbxGRNum.Text + "' AND [GoodsReceivedSeqNo] = " + tempSeqNo + "";
                    Cmd = new SqlCommand(Query, Conn);
                    if (Cmd.ExecuteScalar() != System.DBNull.Value && Cmd.ExecuteScalar().ToString() != "")
                    {
                        Query = "SELECT b.[GroupName] FROM [dbo].[GoodsReceivedD] a LEFT JOIN [dbo].[sysUserGroup] b ON a.UpdatedBy = b.[UserID] WHERE a.[GoodsReceivedId] = '" + tbxGRNum.Text + "' AND a.[GoodsReceivedSeqNo] = " + dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value + "";
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            Dr = Cmd.ExecuteReader();
                            if (Dr.HasRows)
                            {

                            }
                            else
                            {
                                while (Dr.Read())
                                {
                                    if (ControlMgr.GroupName == "KERANI" && Mode == "Edit" && Dr["GroupName"].ToString() != "KERANI")
                                    {
                                        continue;
                                    }
                                }
                            }
                            Dr.Close();
                        }
                    }
                }
                //END=============================

                //GET STATUS CODE
                string statCode = "";
                Query = "Select StatusCode from [dbo].[TransStatusTable] where [Deskripsi] = '" + dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value + "' and [TransCode] = 'GRD'";
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            statCode = Convert.IsDBNull(Dr["StatusCode"]) ? "" : (string)Dr["StatusCode"];
                        }
                    }
                    Dr.Close();
                }

                if (dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value == null || Convert.ToDecimal(dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value) == 0)
                {
                    decimal GoodsReceivedSeqNo = 0;
                    Query = "Select MAX(GoodsReceivedSeqNo) from GoodsReceivedD where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        GoodsReceivedSeqNo = Convert.ToInt32(Cmd.ExecuteScalar()) + 1;
                    }
                    //INSERT QUERY GoodsReceivedD
                    Query = "insert into [dbo].[GoodsReceivedD] ([GoodsReceivedDate],[GoodsReceivedId],[GoodsReceivedSeqNo],[RefTransID],[RefTransSeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[Qty],[Qty_SJ],[Qty_Actual],[Unit],[Ratio],TotalBerat,TotalBerat_Actual,[Ratio_Actual],[InventSiteId],[InventSiteBlokID],[Quality],[Notes],[ActionCodeStatus],[CreatedDate],[CreatedBy],[DeliveryMethod], Remaining_Qty) values (@GoodsReceivedDate, @GoodsReceivedId, @GoodsReceivedSeqNo, @RefTransID, @RefTransSeqNo, @GroupId, @SubGroup1Id, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @Qty, @Qty_SJ, @Qty_Actual, @Unit, @Ratio, @TotalBerat, @TotalBerat_Actual, @Ratio_Actual, @InventSiteId, @InventSiteBlokID, @Quality, @Notes, @ActionCodeStatus, getdate(), @CreatedBy, @DeliveryMethod, @Remaining_Qty)";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@GoodsReceivedDate", dtGR.Value);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", GoodsReceivedSeqNo);//dataGridView3.Rows[i].Cells["No"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@RefTransID", dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@RefTransSeqNo", dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@GroupId", dataGridView3.Rows[i].Cells["GroupId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@SubGroup1Id", dataGridView3.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@SubGroup2Id", dataGridView3.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@ItemId", dataGridView3.Rows[i].Cells["ItemId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@FullItemId", dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@ItemName", dataGridView3.Rows[i].Cells["ItemName"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@Qty", dataGridView3.Rows[i].Cells["Qty"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty"].Value));
                        Cmd.Parameters.AddWithValue("@Qty_SJ", dataGridView3.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value));
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView3.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@Unit", dataGridView3.Rows[i].Cells["Unit"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@Ratio", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value));
                        Cmd.Parameters.AddWithValue("@TotalBerat", dataGridView3.Rows[i].Cells["TotalBerat"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat"].Value));
                        Cmd.Parameters.AddWithValue("@TotalBerat_Actual", dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@Ratio_Actual", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@InventSiteId", dataGridView3.Rows[i].Cells["InventSiteId"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@InventSiteBlokID", dataGridView3.Rows[i].Cells["InventSiteBlokID"].Value.ToString());
                        string QualityID = String.Empty;
                        if (dataGridView3.Rows[i].Cells["Quality"].Value != "Select")
                        {
                            SqlCommand Cmd2 = new SqlCommand("Select [QualityID] From [dbo].[InventQuality] where Deskripsi = '" + dataGridView3.Rows[i].Cells["Quality"].Value + "'", Conn);
                            QualityID = Cmd2.ExecuteScalar().ToString();
                        }
                        Cmd.Parameters.AddWithValue("@Quality", dataGridView3.Rows[i].Cells["Quality"].Value == "Select" ? (object)DBNull.Value : QualityID);
                        Cmd.Parameters.AddWithValue("@Notes", dataGridView3.Rows[i].Cells["Notes"].Value.ToString());
                        Cmd.Parameters.AddWithValue("@ActionCodeStatus", statCode);
                        Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                        Cmd.Parameters.AddWithValue("@DeliveryMethod", (object)DBNull.Value);
                        Cmd.Parameters.AddWithValue("@Remaining_Qty", dataGridView3.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value) : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value));
                        Cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // UPDATE QUERY GoodsReceivedD
                    Query = "update [dbo].[GoodsReceivedD] set [Qty_SJ] = @Qty_SJ, InventSiteBlokID = @InventSiteBlokID, ActionCodeStatus = @ActionCodeStatus, Notes = @Notes, Qty_Actual = @Qty_Actual, Remaining_Qty = @Qty_Actual, TotalBerat_Actual = @TotalBerat_Actual, Quality = @Quality, UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where [GoodsReceivedId] = @GoodsReceivedId and [FullItemId] = @FullItemId and [GoodsReceivedSeqNo] = @GoodsReceivedSeqNo; ";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@Qty_SJ", dataGridView3.Rows[i].Cells["Qty_SJ"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value));
                        Cmd.Parameters.AddWithValue("@InventSiteBlokID", dataGridView3.Rows[i].Cells["InventSiteBlokID"].Value);
                        Cmd.Parameters.AddWithValue("@ActionCodeStatus", statCode);
                        Cmd.Parameters.AddWithValue("@Notes", dataGridView3.Rows[i].Cells["Notes"].Value);
                        Cmd.Parameters.AddWithValue("@Qty_Actual", dataGridView3.Rows[i].Cells["Qty_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value));
                        Cmd.Parameters.AddWithValue("@TotalBerat_Actual", dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value == String.Empty ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value));

                        string QualityID = String.Empty;
                        if (!(dataGridView3.Rows[i].Cells["Quality"].Value == "Select" || dataGridView3.Rows[i].Cells["Quality"].Value == null || dataGridView3.Rows[i].Cells["Quality"].Value == String.Empty))
                        {
                            SqlCommand Cmd2 = new SqlCommand("Select [QualityID] From [dbo].[InventQuality] where Deskripsi = '" + dataGridView3.Rows[i].Cells["Quality"].Value + "'", Conn);
                            QualityID = Cmd2.ExecuteScalar().ToString();
                            Cmd.Parameters.AddWithValue("@Quality", QualityID);
                        }
                        else
                            Cmd.Parameters.AddWithValue("@Quality", (object)DBNull.Value);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                        Cmd.Parameters.AddWithValue("@FullItemId", dataGridView3.Rows[i].Cells["FullItemId"].Value);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void UpdateResizeInventMovement(int i)
        {
            decimal qty = 0;
            decimal Ratio = 0;
            decimal Price = 0;
            if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
            {
                qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                Ratio = Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value) == 0 ? 1 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value);
            }
            else
            {
                qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                Ratio = Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value) == 0 ? 1 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
            }
            Query = "SELECT [UoM_AvgPrice] FROM [InventTable] WHERE [FullItemID] = @FullItemID";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString());
                Price = Convert.ToDecimal(Cmd.ExecuteScalar());
            }
            Query = "UPDATE [dbo].[Invent_Movement_Qty] SET [Resize_In_Progress_UoM] += " + qty + ",[Resize_In_Progress_Alt] += " + (qty * Ratio) + ",[Resize_In_Progress_Amount] += " + (qty * Price) + " WHERE [FullItemID] = @FullItemID";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString());
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertInventOnHandQty()
        {
            //GR only insert on hand status 1 time only
            string strSql = "SELECT * FROM InventTrans WHERE TransId='" + tbxGRNum.Text + "'";
            using (Cmd = new SqlCommand(strSql, Conn))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    Dr.Close();
                    return;
                }
                Dr.Close();
            }


            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //CHECK IF THERES ANY RESIZE
                Query = "select * from InventTable where FullItemID = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and Resize = '1'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows && dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "RECEIVED")
                {
                    Dr.Close();
                    UpdateResizeInventMovement(i);
                    continue;
                }
                Dr.Close();


                if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "RECEIVED")
                {
                    if (cmbReferenceType.Text.ToUpper() == "RECEIPT ORDER")
                    {
                        //update invent purchase table
                        updateROPurchaseQty(i);
                    } 
                    decimal qty = 0;
                    decimal Ratio = 0;
                    if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                    {
                        qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                        Ratio = Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value) == 0 ? 1 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value);
                    }
                    else
                    {
                        qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                        Ratio = Ratio = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value) == 0 ? 1 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio"].Value);
                    }
                    decimal Available_For_Sale_UoM = 0;
                    decimal Available_For_Sale_Alt = 0;
                    decimal Available_UoM = 0;
                    decimal Available_Alt = 0;
                    decimal Available_For_Sale_Reserved_UoM = 0;
                    decimal Available_For_Sale_Reserved_Alt = 0;

                    if (cmbReferenceType.Text.ToUpper() == "RECEIPT ORDER")
                    {
                        Query = "SELECT Price_KG FROM ReceiptOrderD WHERE ReceiptOrderId = '" + tbxRefID.Text + "' and FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
                    }
                    else if (cmbReferenceType.Text == "Nota Retur Jual")
                    {
                        Query = "select d.[Price_Alt] from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                    }
                    else if (cmbReferenceType.Text == "Nota Retur Beli")
                    {
                        Query = "select d.Price_KG from [NotaReturBeli_Dtl] a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.[NRBId] = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and  a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value + "";
                    }
                    else
                    {
                        Query = "SELECT Alt_AvgPrice FROM InventTable WHERE FullItemID = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    decimal price = 0;
                    if (Cmd.ExecuteScalar() != System.DBNull.Value)
                    {
                        price = Convert.ToDecimal(Cmd.ExecuteScalar()); //in kg
                    }

                    Query = "select * from Invent_OnHand_Qty where FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and InventSiteId = '" + txtInventSiteID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    if (Dr.HasRows)
                    {
                        //while (Dr.Read())
                        //{
                        //    Available_UoM = Convert.ToDecimal(Dr["Available_UoM"]);
                        //    Available_For_Sale_UoM = Convert.ToDecimal(Dr["Available_For_Sale_UoM"]);
                        //    Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr["Available_For_Sale_Reserved_UoM"]);
                        //}
                        Dr.Close();

                        if (dataGridView1.Rows[i].Cells["Unit"].Value.ToString().ToUpper() == "KG")
                        {
                            Available_UoM = (qty / Ratio);
                            Available_Alt = Available_UoM * Ratio;
                            Available_For_Sale_UoM = (qty / Ratio);
                            Available_For_Sale_Alt = Ratio * Available_For_Sale_UoM;
                        }
                        else
                        {
                            Available_UoM = (qty);
                            Available_Alt = Available_UoM * Ratio;

                            if (cmbReferenceType.Text == "Nota Transfer")
                            {
                                Query = "SELECT * FROM [GoodsIssuedD] WHERE [GoodsIssuedId] = '" + tbxGRNum.Text + "' AND [GoodsIssuedSeqNo] = " + dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value + " ";
                                using (Cmd = new SqlCommand(Query, Conn))
                                {
                                    Dr = Cmd.ExecuteReader();
                                    while (Dr.Read())
                                    {
                                        Available_For_Sale_UoM = Convert.ToDecimal(Dr["Available_Qty"]);
                                        Available_For_Sale_Alt = Ratio * Available_For_Sale_UoM;
                                        Available_For_Sale_Reserved_UoM = Convert.ToDecimal(Dr["Reserved_Qty"]);
                                        Available_For_Sale_Reserved_Alt = Ratio * Available_For_Sale_Reserved_UoM;
                                    }
                                    Dr.Close();
                                }
                            }
                            else
                            {
                                //Available_UoM += (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                                //Available_Alt = Available_UoM * Ratio;
                                Available_For_Sale_UoM = (qty);
                                Available_For_Sale_Alt = Ratio * Available_For_Sale_UoM;
                            }
                        }
                        string queryyy = "";
                        if (cmbReferenceType.Text == "Nota Transfer")
                        {
                            queryyy = "update Invent_OnHand_Qty set Available_UoM += '" + Available_UoM + "', Available_Alt += '" + Available_Alt + "',Available_Amount += " + Available_Alt * price + ", Available_For_Sale_UoM += '" + Available_For_Sale_UoM + "', Available_For_Sale_Alt += '" + Available_For_Sale_Alt + "', Available_For_Sale_Amount += " + Available_For_Sale_Alt * price + ", Available_For_Sale_Reserved_UoM += " + Available_For_Sale_Reserved_UoM + ", Available_For_Sale_Reserved_Alt += " + Available_For_Sale_Reserved_Alt + ", Available_For_Sale_Reserved_Amount += " + (Available_For_Sale_Reserved_Alt * price) + " where FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and InventSiteId = '" + txtInventSiteID.Text + "'";
                        }
                        else
                        {
                            queryyy = "update Invent_OnHand_Qty set Available_UoM += '" + Available_UoM + "', Available_Alt += '" + Available_Alt + "',Available_Amount += " + Available_Alt * price + ", Available_For_Sale_UoM += '" + Available_For_Sale_UoM + "', Available_For_Sale_Alt += '" + Available_For_Sale_Alt + "', Available_For_Sale_Amount += " + Available_For_Sale_Alt * price + " where FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and InventSiteId = '" + txtInventSiteID.Text + "'";
                        }
                        Cmd = new SqlCommand(queryyy, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        if (price == null || price == 0)
                        {
                            price = 1;
                        }

                        if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
                        {
                            Query = "INSERT INTO [dbo].[Invent_OnHand_Qty] ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName] ,[InventSiteId], [Available_UoM] ,[Available_Alt] ,[Available_Amount], [Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Available_For_Sale_Reserved_UoM] ,[Available_For_Sale_Reserved_Alt] ,[Available_For_Sale_Reserved_Amount]) VALUES ( '" + dataGridView1.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + "', '" + txtInventSiteID.Text + "', '" + dataGridView1.Rows[i].Cells["Qty_SJ"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["TotalBerat"].Value.ToString() + "', 0, '" + dataGridView1.Rows[i].Cells["Qty_SJ"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["TotalBerat"].Value.ToString() + "', '" + (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value)) * price + "', 0, 0, 0)";
                        }
                        else
                        {
                            Query = "INSERT INTO [dbo].[Invent_OnHand_Qty] ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName] ,[InventSiteId], [Available_UoM] ,[Available_Alt] ,[Available_Amount], [Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Available_For_Sale_Reserved_UoM] ,[Available_For_Sale_Reserved_Alt] ,[Available_For_Sale_Reserved_Amount]) VALUES ( '" + dataGridView1.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + "', '" + txtInventSiteID.Text + "', '" + dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() + "', 0, '" + dataGridView1.Rows[i].Cells["Qty_Actual"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() + "', '" + (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value)) * price + "', 0, 0, 0)";
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }

            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                //CHECK IF THERES ANY RESIZE
                Query = "select * from InventTable where FullItemID = '" + dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString() + "' and Resize = '1'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows && dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "RECEIVED")
                {
                    Dr.Close();
                    continue;
                }
                Dr.Close();

                if (dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "RECEIVED")
                {
                    decimal qty = 0;
                    decimal Ratio = 0;
                    if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                    {
                        qty = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value);
                        Ratio = Ratio = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value) == 0 ? 1 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value);
                    }
                    else
                    {
                        qty = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                        Ratio = Ratio = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value) == 0 ? 1 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio"].Value);
                    }

                    decimal Available_For_Sale_UoM = 0;
                    decimal Available_For_Sale_Alt = 0;
                    decimal Available_UoM = 0;
                    decimal Available_Alt = 0;

                    if (cmbReferenceType.Text.ToUpper() == "RECEIPT ORDER")
                    {
                        Query = "SELECT Price_KG FROM ReceiptOrderD WHERE ReceiptOrderId = '" + tbxRefID.Text + "' and FullItemId = '" + dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
                    }
                    else
                    {
                        Query = "SELECT Alt_AvgPrice FROM InventTable WHERE FullItemID = '" + dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString() + "';";
                    }
                    Cmd = new SqlCommand(Query, Conn);
                    decimal price = Convert.ToDecimal(Cmd.ExecuteScalar());

                    Query = "select * from Invent_OnHand_Qty where FullItemId = '" + dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString() + "' and InventSiteId = '" + txtInventSiteID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        //while (Dr.Read())
                        //{
                        //    Available_UoM = Convert.ToDecimal(Dr["Available_UoM"]);
                        //    Available_For_Sale_UoM = Convert.ToDecimal(Dr["Available_For_Sale_UoM"]);
                        //}
                        Dr.Close();

                        if (dataGridView3.Rows[i].Cells["Unit"].Value.ToString().ToUpper() == "KG")
                        {
                            Available_UoM = (qty / Ratio);
                            Available_Alt = Available_UoM * Ratio;
                            Available_For_Sale_UoM = (qty / Ratio);
                            Available_For_Sale_Alt = Ratio * Available_For_Sale_UoM;
                        }
                        else
                        {
                            Available_UoM = qty;
                            Available_Alt = Available_UoM * Ratio;
                            Available_For_Sale_UoM = qty;
                            Available_For_Sale_Alt = Ratio * Available_For_Sale_UoM;
                        }

                        Cmd = new SqlCommand("update Invent_OnHand_Qty set Available_UoM += '" + Available_UoM + "', Available_Alt += '" + Available_Alt + "',Available_Amount += " + Available_Alt * price + ", Available_For_Sale_UoM += '" + Available_For_Sale_UoM + "', Available_For_Sale_Alt += '" + Available_For_Sale_Alt + "', Available_For_Sale_Amount += " + Available_For_Sale_Alt * price + " where FullItemId = '" + dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString() + "' and InventSiteId = '" + txtInventSiteID.Text + "'", Conn);
                        Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Cmd = new SqlCommand("INSERT INTO [dbo].[Invent_OnHand_Qty] {[GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId], [FullItemId] ,[ItemName] ,[InventSiteId], [Available_UoM] ,[Available_Alt] ,[Available_Amount], [Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Available_For_Sale_Reserved_UoM] ,[Available_For_Sale_Reserved_Alt] ,[Available_For_Sale_Reserved_Amount]) VALUES ( '" + dataGridView3.Rows[i].Cells["GroupId"].Value.ToString() + "', '" + dataGridView3.Rows[i].Cells["SubGroup1Id"].Value.ToString() + "', '" + dataGridView3.Rows[i].Cells["SubGroup2Id"].Value.ToString() + "', '" + dataGridView3.Rows[i].Cells["ItemId"].Value.ToString() + "', '" + dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dataGridView3.Rows[i].Cells["ItemName"].Value.ToString() + "', '" + txtInventSiteID.Text + "', '" + dataGridView3.Rows[i].Cells["Qty_Actual"].Value.ToString() + "', '" + dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() + "', 0, '" + dataGridView3.Rows[i].Cells["Qty_Actual"].Value.ToString() + "', '" + dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() + "', 0, 0, 0, 0)", Conn);
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void insertInventTrans()
        {
            //GR only insert on hand status 1 time only
            string strSql = "SELECT * FROM InventTrans WHERE TransId='" + tbxGRNum.Text + "'";
            using (Cmd = new SqlCommand(strSql, Conn))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    Dr.Close();
                    return;
                }
                Dr.Close();
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                //check if item is received or not, if not then skip the trans
                if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() != "RECEIVED")
                {
                    continue;
                }

                //CHECK IF THERES ANY RESIZE
                Query = "select * from InventTable where FullItemID = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and Resize = '1'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows && dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "RECEIVED")
                {
                    Dr.Close();
                    continue;
                }
                Dr.Close();

                decimal itemPrice = 0;
                if (!(dataGridView1.Rows[i].Cells["RefTransID"].Value == String.Empty || dataGridView1.Rows[i].Cells["RefTransID"].Value == null))
                {
                    Query = "select b.Price_KG, a.ReceiptOrderId, a.SeqNo ROSeqNo, b.PurchID, b.SeqNo 'POSeqNo', a.FullItemId from ReceiptOrderD as a left join PurchDtl as b on a.PurchaseOrderId = b.PurchID where a.ReceiptOrderId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = '" + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["Price_KG"] == System.DBNull.Value)
                        {
                            itemPrice = 1;
                        }
                        else
                        {
                            itemPrice = Convert.ToDecimal(Dr["Price_KG"]);
                        }
                    }
                    Dr.Close();
                }

                Query = "INSERT INTO [dbo].[InventTrans] ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId] ,[FullItemId] ,[ItemName] ,[InventSiteId] ,[TransId], SeqNo ,[TransDate] ,[Ref_TransId] ,[Ref_TransDate] ,[Ref_Trans_SeqNo] ,[AccountId],[AccountName] ,[Available_UoM] ,[Available_Alt] ,[Available_Amount] ,[Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Notes]) VALUES (@GroupId, @SubGroupId, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @InventSiteId, @TransId, @SeqNo, @TransDate, @Ref_TransId, @Ref_TransDate, @Ref_Trans_SeqNo, @AccountId, @AccountName, @Available_UoM, @Available_Alt, @Available_Amount, @Available_For_Sale_UoM, @Available_For_Sale_Alt, @Available_For_Sale_Amount, @Notes)";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.Parameters.AddWithValue("@GroupId", dataGridView1.Rows[i].Cells["GroupId"].Value.ToString());
                Cmd.Parameters.AddWithValue("@SubGroupId", dataGridView1.Rows[i].Cells["SubGroup1Id"].Value.ToString());
                Cmd.Parameters.AddWithValue("@SubGroup2Id", dataGridView1.Rows[i].Cells["SubGroup2Id"].Value.ToString());
                Cmd.Parameters.AddWithValue("@ItemId", dataGridView1.Rows[i].Cells["ItemId"].Value.ToString());
                Cmd.Parameters.AddWithValue("@FullItemId", dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString());
                Cmd.Parameters.AddWithValue("@ItemName", dataGridView1.Rows[i].Cells["ItemName"].Value.ToString());
                Cmd.Parameters.AddWithValue("@InventSiteId", txtInventSiteID.Text);
                Cmd.Parameters.AddWithValue("@TransId", tbxGRNum.Text);
                Cmd.Parameters.AddWithValue("@SeqNo", dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString());
                Cmd.Parameters.AddWithValue("@TransDate", dtGR.Value);
                Cmd.Parameters.AddWithValue("@Ref_TransId", dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString());
                Cmd.Parameters.AddWithValue("@Ref_TransDate", dtRef.Value);
                Cmd.Parameters.AddWithValue("@Ref_Trans_SeqNo", dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value.ToString());
                Cmd.Parameters.AddWithValue("@AccountId", tbxNameID.Text);
                Cmd.Parameters.AddWithValue("@AccountName", tbxName.Text);
                Cmd.Parameters.AddWithValue("@Available_UoM", Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                Cmd.Parameters.AddWithValue("@Available_Alt", Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value));
                Cmd.Parameters.AddWithValue("@Available_Amount", dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? 0 : itemPrice * (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value)));
                Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value));
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value));
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", dataGridView1.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? 0 : itemPrice * (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value)));
                Cmd.Parameters.AddWithValue("@Notes", dataGridView1.Rows[i].Cells["Notes"].Value == null ? "" : dataGridView1.Rows[i].Cells["Notes"].Value.ToString().Trim());
                Cmd.ExecuteNonQuery();
            }
            //datagridview 3 tidak ada actioncode received jadi tidak bisa masuk available on hand, jdi gk ush ada trans nya
            //for (int i = 0; i < dataGridView3.RowCount; i++)
            //{
            //    //CHECK IF THERES ANY RESIZE
            //    Query = "select * from InventTable where FullItemID = '" + dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString() + "' and Resize = '1'";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Dr = Cmd.ExecuteReader();
            //    if (Dr.HasRows && dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "RECEIVED")
            //    {
            //        Dr.Close();
            //        continue;
            //    }
            //    Dr.Close();

            //    decimal itemPrice = 0;
            //    if (!(dataGridView3.Rows[i].Cells["RefTransID"].Value == String.Empty || dataGridView3.Rows[i].Cells["RefTransID"].Value == null))
            //    {
            //        Query = "select b.Price_KG, a.ReceiptOrderId, a.SeqNo ROSeqNo, b.PurchID, b.SeqNo 'POSeqNo', a.FullItemId from ReceiptOrderD as a left join PurchDtl as b on a.PurchaseOrderId = b.PurchID where a.ReceiptOrderId = '" + dataGridView3.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = '" + dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value + "'";
            //        Cmd = new SqlCommand(Query, Conn);
            //        Dr = Cmd.ExecuteReader();
            //        while (Dr.Read())
            //        {
            //            itemPrice = Convert.ToDecimal(Dr["Price_KG"]);
            //        }
            //        Dr.Close();
            //    }

            //    Query = "INSERT INTO [dbo].[InventTrans] ([GroupId] ,[SubGroupId] ,[SubGroup2Id] ,[ItemId] ,[FullItemId] ,[ItemName] ,[InventSiteId] ,[TransId], SeqNo ,[TransDate] ,[Ref_TransId] ,[Ref_TransDate] ,[Ref_Trans_SeqNo] ,[AccountId],[AccountName] ,[Available_UoM] ,[Available_Alt] ,[Available_Amount] ,[Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Notes]) VALUES (@GroupId, @SubGroupId, @SubGroup2Id, @ItemId, @FullItemId, @ItemName, @InventSiteId, @TransId, @SeqNo, @TransDate, @Ref_TransId, @Ref_TransDate, @Ref_Trans_SeqNo, @AccountId, @AccountName, @Available_UoM, @Available_Alt, @Available_Amount, @Available_For_Sale_UoM, @Available_For_Sale_Alt, @Available_For_Sale_Amount, @Notes)";
            //    Cmd = new SqlCommand(Query, Conn);
            //    Cmd.Parameters.AddWithValue("@GroupId", dataGridView3.Rows[i].Cells["GroupId"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@SubGroupId", dataGridView3.Rows[i].Cells["SubGroup1Id"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@SubGroup2Id", dataGridView3.Rows[i].Cells["SubGroup2Id"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@ItemId", dataGridView3.Rows[i].Cells["ItemId"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@FullItemId", dataGridView3.Rows[i].Cells["FullItemId"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@ItemName", dataGridView3.Rows[i].Cells["ItemName"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@InventSiteId", txtInventSiteID.Text);
            //    Cmd.Parameters.AddWithValue("@TransId", tbxGRNum.Text);
            //    Cmd.Parameters.AddWithValue("@SeqNo", dataGridView3.Rows[i].Cells["GoodsReceivedSeqNo"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@TransDate", dtGR.Value);
            //    Cmd.Parameters.AddWithValue("@Ref_TransId", dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@Ref_TransDate", dtRef.Value);
            //    Cmd.Parameters.AddWithValue("@Ref_Trans_SeqNo", dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value.ToString() == String.Empty ? (object)DBNull.Value : dataGridView3.Rows[i].Cells["RefTransSeqNo"].Value.ToString());
            //    Cmd.Parameters.AddWithValue("@AccountId", tbxNameID.Text);
            //    Cmd.Parameters.AddWithValue("@AccountName", tbxName.Text);
            //    Cmd.Parameters.AddWithValue("@Available_UoM", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value));
            //    Cmd.Parameters.AddWithValue("@Available_Alt", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value));
            //    Cmd.Parameters.AddWithValue("@Available_Amount", dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? 0 : itemPrice * (Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value)));
            //    Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value));
            //    Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value));
            //    Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", dataGridView3.Rows[i].Cells["RefTransID"].Value.ToString() == String.Empty ? 0 : itemPrice * (Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView3.Rows[i].Cells["Ratio_Actual"].Value)));
            //    Cmd.Parameters.AddWithValue("@Notes", dataGridView3.Rows[i].Cells["Notes"].Value.ToString());
            //    Cmd.ExecuteNonQuery();
            //}
        }

        private void insertNotaResizeHDtlLog()
        {
            if (cmbReferenceType.Text == "") return; //hendry

            //CHECK IF GRH STATUS 'COMPLETED' AND GRDTL ACTION CODE STATS 'RESIZE'
            if (cmbReferenceType.Text == "Nota Transfer")
            {
                Query = "select b.GoodsReceivedId, b.GoodsReceivedSeqNo, b.RefTransID, b.FullItemId, b.ItemName, b.Qty_Actual, b.Unit, b.Ratio, d.UoM_AvgPrice as Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join [GoodsIssuedD] c on b.RefTrans2Id = c.[GoodsIssuedId] and b.RefTrans2SeqNo = c.[GoodsIssuedSeqNo] left join InventTable d on d.FullItemId = b.FullItemId where a.GoodsReceivedStatus in ('03','06') and b.ActionCodeStatus in ('05', '09') and a.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            }
            else if (cmbReferenceType.Text == "Receipt Order") //ACTION CODE STATUS 05 Received & 06 Resize
            {
                Query = "select b.GoodsReceivedId, b.GoodsReceivedSeqNo, b.RefTransID, b.FullItemId, b.ItemName, b.Qty_Actual, b.Unit, b.Ratio, c.Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId and b.RefTransSeqNo = c.SeqNo left join InventTable d on d.FullItemId = b.FullItemId where a.GoodsReceivedStatus in ( '03','06') and b.ActionCodeStatus in ('05', '09') and a.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            }
            else if (cmbReferenceType.Text == "Nota Retur Jual")
            {
                Query = "select b.GoodsReceivedId, b.GoodsReceivedSeqNo, b.RefTransID, b.FullItemId, b.ItemName, b.Qty_Actual, b.Unit, b.Ratio, d.UoM_AvgPrice as Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join [NotaReturJual_Dtl] c on b.RefTransID = c.[NRJId] and b.RefTransSeqNo = c.[SeqNo] left join InventTable d on d.FullItemId = b.FullItemId where a.GoodsReceivedStatus in ('03','06') and b.ActionCodeStatus in ('05', '09') and a.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            }
            else if (cmbReferenceType.Text == "Nota Retur Beli")
            {
                Query = "select b.GoodsReceivedId, b.GoodsReceivedSeqNo, b.RefTransID, b.FullItemId, b.ItemName, b.Qty_Actual, b.Unit, b.Ratio, d.UoM_AvgPrice as Price, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join [NotaReturBeli_Dtl] c on b.RefTransID = c.[NRBId] and b.RefTransSeqNo = c.[SeqNo] left join InventTable d on d.FullItemId = b.FullItemId where a.GoodsReceivedStatus in ('03','06') and b.ActionCodeStatus in ('05', '09') and a.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int NRZSeqNo = 1;
            string NRZID = "";
            decimal amount = 0;
            bool hasresize = false;
            if (Dr.HasRows)
            {
                while (Dr.Read())
                {
                    //CHECK IN INVENT TABLE IF RESIZE = 1 (NEED RESIZE)
                    Query = "select TOP 1 * from InventTable where FullItemID = '" + Dr["FullItemId"] + "' and Resize = '1'";
                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader Dr2 = Cmd.ExecuteReader();
                    if (Dr2.HasRows)
                    {
                        hasresize = true;
                        //edited by Thaddaeus, 12/10/2018
                        if (NRZID == "")
                        {
                            string Jenis = "NRZ";
                            string Kode = "NRZ";
                            NRZID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Cmd);
                        }

                        Query = "INSERT INTO [dbo].[NotaResize_Dtl]([NRZId],[SeqNo],[FromFullItemId],[FromItemName],[ToFullItemId],[ToItemName],[Qty],[Price],[Unit],[LineAmount],[GoodsReceivedId],[GoodsReceiveSeqNo],[Notes1],[Notes2],[CreatedDate],[CreatedBy])VALUES ('" + NRZID + "', '" + NRZSeqNo + "', '" + Dr["FullItemId"] + "', '" + Dr["ItemName"] + "'";
                        Cmd = new SqlCommand("Select TOP 1 * from InventResize where From_FullItemId = '" + Dr["FullItemId"] + "'", Conn);
                        SqlDataReader Dr3 = Cmd.ExecuteReader();
                        if (Dr3.HasRows)
                        {
                            while (Dr3.Read())
                            {
                                Query += ", '" + Dr3["To_FullItemId"] + "', '" + Dr3["To_ItemName"] + "', '" + Dr["Qty_Actual"] + "'";
                            }
                        }
                        else
                        {
                            Query += ", '', '', '" + Dr["Qty_Actual"] + "'";
                        }
                        Dr3.Close();
                        if (Dr["RefTransID"] == null || Dr["RefTransID"].ToString() == String.Empty)
                        {
                            Query += ", '" + Dr["UoM_AvgPrice"] + "', '" + Dr["Unit"] + "', '" + Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["UoM_AvgPrice"]) + "'";
                        }
                        else
                        {
                            Query += ", '" + Dr["Price"] + "', '" + Dr["Unit"] + "', '" + Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["Price"]) + "'";
                        }
                        Query += ", '" + Dr["GoodsReceivedId"] + "', '" + Dr["GoodsReceivedSeqNo"] + "', '', '', getdate(), '" + ControlMgr.UserId + "')";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        if (Dr["RefTransID"] == null || Dr["RefTransID"].ToString() == String.Empty)
                        {
                            amount = amount + (Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["UoM_AvgPrice"]));
                        }
                        else
                        {
                            amount = amount + (Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["Price"]));
                        }
                        NRZSeqNo++;
                    }
                    Dr2.Close();
                }
                if (hasresize == true)
                {
                    //INSERT INTO NOTA RESIZE LOG TABLE
                    Query = "INSERT INTO [dbo].[NotaResize_LogTable]([NRZId],[NRZDate],[GoodsReceivedDate],[GoodsReceivedId],[VendID],[InventSiteID],[LogStatusCode],[LogStatusDesc],[Action],[UserID],[LogDate])VALUES ('" + NRZID + "', CAST(getdate() AS DATE), '" + dtGR.Value + "', '" + tbxGRNum.Text + "', '" + tbxVOwnerID.Text + "', '" + txtInventSiteID.Text + "' ";
                    Query += ", '01', 'Created', 'N', '" + ControlMgr.UserId + "', getdate())";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();

                    Query = "INSERT INTO [dbo].[NotaResizeH]([NRZId],[NRZDate],[GoodsReceivedDate],[GoodsReceivedId],[SiteID],[VendID],[Amount],[Posted],[CreatedDate],[CreatedBy])VALUES('" + NRZID + "', CAST(getdate() AS DATE), '" + dtGR.Value + "', '" + tbxGRNum.Text + "', '" + txtInventSiteID.Text + "', '" + tbxVOwnerID.Text + "', '" + amount + "', '0', getdate(), '" + ControlMgr.UserId + "')";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.ExecuteNonQuery();
                }
                //end================================
            }
            else
            {
                MessageBox.Show("Data tidak masuk kedalam Nota Resize Log karena ada data yang 'null' atau Query untuk tipe form referensi belum dibuat.");
            }
            Dr.Close();

            /*//CHECK IF ITEM NEED RESIZE
            Query = "select * from GoodsReceivedH as GRH left join GoodsReceivedD as GRD on GRH.GoodsReceivedId = GRD.GoodsReceivedId where GRH.GoodsReceivedId = '" + tbxGRNum.Text + "' and GRH.GoodsReceivedStatus = '03' and GRD.FullItemId in (select distinct(RD.FullItemId) from ResizeTableH as RH left join ResizeTableD as RD on RH.TransId = RD.TransId)";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            if (Dr.HasRows)
            {
                HeaderResizeGR f = new HeaderResizeGR();
                string resizeID = f.GenerateID();
                Query = "insert into [dbo].[ResizeH] ([TransDate] ,[TransId] ,[RefTransId] ,[Posted] ,[ResizeType] ,[CreatedDate] ,[CreatedBy])values ( getdate(), '" + resizeID + "', '" + tbxGRNum.Text + "', 1, 'Auto', getdate(), '" + ControlMgr.GroupName + "'); ";
                int seqNoA = 1;
                int seqNoB = 1;
                while (Dr.Read())
                {
                    Query += "insert into [dbo].[Resize_Dtl] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";
                    Query += "values ('" + resizeID + "', '" + seqNoA + "', '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"].ToString() + "', '" + Dr["ItemId"].ToString() + "', '" + Dr["FullItemId"].ToString() + "', '" + Dr["ItemName"].ToString() + "', '" + Dr["InventSiteId"] + "', NULL, '" + Convert.ToInt32(Convert.ToInt32(Dr["Qty_Actual"]) * -1) + "', '" + Dr["Unit"] + "', '" + tbxGRNum.Text + "', '" + Dr["GoodsReceivedSeqNo"] + "', NULL, NULL, NULL, NULL, NULL, NULL, NULL, getdate(), '" + ControlMgr.GroupName + "'); ";


                    seqNoA += 1;
                    seqNoB += 1;

                    Query += "insert into [dbo].[Resize_Dtl] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";

                    Cmd = new SqlCommand("select count(distinct(RD.ToFullItemId)) from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn, Trans);
                    int countRszItem = (Int32)Cmd.ExecuteScalar();

                    if (countRszItem == 1)
                    {
                        Cmd = new SqlCommand("select rd.ToFullItemId from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn, Trans);
                        string fullItemID = Cmd.ExecuteScalar().ToString();

                        Cmd = new SqlCommand("select rd.ToItemName from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn, Trans);
                        string itemName = Cmd.ExecuteScalar().ToString();

                        Query += "values ('" + resizeID + "', '" + seqNoB + "', '" + fullItemID.Split('.')[0] + "', '" + fullItemID.Split('.')[1] + "', '" + fullItemID.Split('.')[2] + "', '" + fullItemID.Split('.')[3] + "', '" + fullItemID + "', '" + itemName + "', NULL, NULL, '" + Dr["Qty_Actual"] + "', '" + Dr["Unit"] + "', '" + tbxGRNum.Text + "', NULL, '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"] + "', '" + Dr["ItemId"] + "', '" + Dr["FullItemId"] + "', '" + Dr["ItemName"] + "', '" + Dr["GoodsReceivedSeqNo"] + "', getdate(), '" + ControlMgr.GroupName + "'); ";
                    }
                    else
                    {
                        Query += "values ('" + resizeID + "', '" + seqNoB + "', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '" + Dr["Qty_Actual"] + "', '" + Dr["Unit"] + "', '" + tbxGRNum.Text + "', NULL, '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"] + "', '" + Dr["ItemId"] + "', '" + Dr["FullItemId"] + "', '" + Dr["ItemName"] + "', '" + Dr["GoodsReceivedSeqNo"] + "', getdate(), '" + ControlMgr.GroupName + "'); ";
                    }
                    seqNoA += 1;
                    seqNoB += 1;
                }
                Dr.Close();
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }*/
        }

        private void insertNotaPurchasedParkedHDtlLog()
        {
            //CHECK IF GRH STATUS 'COMPLETED' AND GRDTL ACTION CODE STATS 'PARKED - NEED ACTION'
            Query = "select b.GoodsReceivedId, b.GoodsReceivedSeqNo, b.RefTransID, b.FullItemId,a.GoodsReceivedDate, b.ItemName, b.Qty_Actual, b.Unit, b.Ratio, c.Price, b.InventSiteId, d.UoM_AvgPrice from GoodsReceivedH a left join GoodsReceivedD b on a.GoodsReceivedId = b.GoodsReceivedId left join ReceiptOrderD c on b.RefTransID = c.ReceiptOrderId and b.RefTransSeqNo = c.SeqNo left join InventTable d on d.FullItemId = b.FullItemId where a.GoodsReceivedStatus IN ('03','05') and b.ActionCodeStatus in ('02','04') and a.GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int NPPSeqNo = 1;
            string NPPID = "";
            decimal amount = 0;
            while (Dr.Read())
            {
                //Theres already stored procedure command to insert npp detail and header in the program before
                //string Jenis = "NPP";
                //string Kode = "NPP";
                //NPPID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                //Query = "INSERT INTO [dbo].[NotaPurchaseParkD]([NPPID],[SeqNo],[FullItemID],[ItemName],[Qty],[Unit],[Price],[GoodsReceivedID],[GoodsReceived_SeqNo],[Notes],[ActionCode],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])VALUES ('" + NPPID + "', '" + NPPSeqNo + "', '" + Dr["FullItemId"] + "', '" + Dr["ItemName"] + "'";
                //Query += ", '" + Dr["Qty_Actual"] + "'";


                //if (Dr["RefTransID"] == null || Dr["RefTransID"].ToString() == String.Empty)
                //    Query += ", '" + Dr["Unit"] + "', '" + Dr["UoM_AvgPrice"] + "'";
                //else
                //    Query += ", '" + Dr["Unit"] + "', '" + Dr["Price"] + "'";

                //Query += ", '" + Dr["GoodsReceivedId"] + "', '" + Dr["GoodsReceivedSeqNo"] + "', '','01', getdate(), 'SYSTEM', '1753-01-01', NULL)";
                //Cmd = new SqlCommand(Query, Conn);
                //Cmd.ExecuteNonQuery();

                //if (Dr["RefTransID"] == null || Dr["RefTransID"].ToString() == String.Empty)
                //    amount = amount + (Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["UoM_AvgPrice"]));
                //else
                //    amount = amount + (Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["Price"]));

                Query = "SELECT NPPID FROM NotaPurchaseParkH WHERE [GoodsReceivedID] = '" + tbxGRNum.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                NPPID = Cmd.ExecuteScalar().ToString();
                //INSERT INTO NOTA Purchase Parked LOG TABLE
                //REMARKED BY: HC | KOLOMNYA SEMUA DIUBAH
                /*Query = "INSERT INTO [dbo].[NotaPurchasePark_LogTable]([NPPId],[NPPDate],[RefTransDate],[RefTransId],[AccountId],[AccountName],[RefTrans2Id], [RefTrans2Date],[InventSiteID],[Qty_UoM],[Qty_Alt] ,[Amount],[Notes], [LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate]) VALUES ('" + NPPID + "', getdate(),'" + dtGR.Value + "','" + tbxGRNum.Text + "', '" + tbxVOwnerID.Text + "','', '','','" + Dr["InventSiteId"] + "' ,'" + Dr["Qty_Actual"] + "', '" + Convert.ToDecimal(Dr["Ratio"]) * Convert.ToDecimal(Dr["Qty_Actual"]) + "'";

                if (Dr["RefTransID"] == System.DBNull.Value || Dr["RefTransID"].ToString() == String.Empty || Dr["Price"] == System.DBNull.Value)
                    Query += ", '" + Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["UoM_AvgPrice"]) + "'";
                else
                    Query += ", '" + Convert.ToDecimal(Dr["Qty_Actual"]) * Convert.ToDecimal(Dr["Price"]) + "'";

                Query += ", '','01', 'Created', 'Created', '" + ControlMgr.UserId + "', getdate())";*/
                Query = "INSERT INTO NotaPurchasePark_LogTable (TransaksiID, Deskripsi, StatusTransaksi, Action, UserID, LogDatetime) VALUES ('" + NPPID + "', '', 'Created', 'N', '" + ControlMgr.UserId + "', getdate())";
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
                NPPSeqNo++;
                break;
            }

            if (Dr.HasRows)
            {
                //Query = "INSERT INTO [dbo].[NotaPurchaseParkH]([NPPID],[NPPDate],[GoodsReceivedID],[VendID],[TransCode],[ApprovalNotes],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])VALUES('" + NPPID + "', getdate(), '" + dtGR.Value + "', '" + tbxVOwnerID.Text + "', '', '', getdate(), 'SYSTEM', '1753-01-01', NULL)";
                //Cmd = new SqlCommand(Query, Conn);
                //Cmd.ExecuteNonQuery();
            }
            Dr.Close();
        }

        private void updateRefRemainingQty(decimal Qty_Actual, string GRID, decimal GoodsReceivedSeqNo, string RefTransID, int RefTransSeqNo, int i, string CurrentRowStatus)
        {
            decimal remainingQty = 0;
            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":
                    Query = "Select RemainingQty from ReceiptOrderD where [ReceiptOrderId] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    break;
                case "Nota Transfer":
                    //edited by Thaddaeus, 15May2018, begin
                    //Query = "Select RemainingQty from [NotaTransferD] where [TrasnferNo] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    Query = "Select a.RemainingQty from [NotaTransferD] a LEFT JOIN [GoodsIssuedD] b ON b.[RefTransID]=a.[TransferNo] AND a.[SeqNo]=b.[RefTransSeqNo] where b.[GoodsIssuedId] = '" + RefTransID + "' and b.[GoodsIssuedSeqNo] = '" + RefTransSeqNo + "'; ";
                    //end
                    break;
                case "Nota Retur Jual":
                    if (ControlMgr.GroupName != "KERANI")
                    {
                        if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                        {
                            return;
                        }
                    }
                    Query = "Select RemainingQty from NotaReturJual_Dtl where [NRJID] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    break;
                case "Nota Retur Beli":
                    if (ControlMgr.GroupName != "KERANI")
                    {
                        if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                        {
                            return;
                        }
                    }
                    Query = "Select [Remaining_Qty_RO] as 'RemainingQty' from NotaReturBeli_Dtl where [NRBId] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    break;
                default: Query = ""; break;
            }
            //hendry
            if (Query != "")
            {
                Cmd = new SqlCommand(Query, Conn);
                remainingQty = (Decimal)Cmd.ExecuteScalar();
            }

            //if same action code status after and before edit (action code status = reject && parked)
            Query = "SELECT [ActionCodeStatus],[Qty_SJ],[Qty_Actual] FROM [dbo].[GoodsReceivedD] WHERE [GoodsReceivedId] = '" + GRID + "' AND [GoodsReceivedSeqNo] = " + GoodsReceivedSeqNo + "";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (((Dr["ActionCodeStatus"].ToString() == "03") && (CurrentRowStatus == "Reject")) || (Dr["ActionCodeStatus"].ToString() == "02") && (CurrentRowStatus == "Parked - Need Action"))
                    {
                        Dr.Close();
                        return;
                    }
                }
                Dr.Close();
            }

            //CALCULATE NEW REF REMAINING QTY
            Cmd = new SqlCommand("select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + GRID + "'", Conn);
            if ((ControlMgr.GroupName == "WB OPERATOR" && Cmd.ExecuteScalar().ToString() == "01" && Mode == "New") || (GoodsReceivedSeqNo == 0) || (txtSiteType.Text.ToUpper() == "VIRTUAL SITE" && ControlMgr.GroupName == "Purchase Admin"))
            {
                if (CurrentRowStatus == "Bongkar" || CurrentRowStatus == "Received")
                {
                    remainingQty = remainingQty - Qty_Actual;
                }
            }
            else if (ControlMgr.GroupName == "WB OPERATOR" && Cmd.ExecuteScalar().ToString() == "01" && Mode == "Edit")
            {
                Query = "Select Qty_SJ,ActionCodeStatus from GoodsReceivedD where GoodsReceivedId = '" + GRID + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                decimal oldGIQty = 0;
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    //action code status 03 = reject, 02 = parked
                    if ((Dr["ActionCodeStatus"].ToString() == "03" || Dr["ActionCodeStatus"].ToString() == "02") && (CurrentRowStatus == "Bongkar" || CurrentRowStatus == "Received"))
                    {
                        oldGIQty = 0;
                    }
                    else if ((CurrentRowStatus != "Bongkar" && CurrentRowStatus != "Received") && (Dr["ActionCodeStatus"].ToString() == "01" || Dr["ActionCodeStatus"].ToString() == "05"))
                    {
                        oldGIQty = Convert.ToDecimal(Dr["Qty_SJ"]) + Qty_Actual;
                    }
                    else
                    {
                        oldGIQty = Convert.ToDecimal(Dr["Qty_SJ"]);
                    }
                }
                remainingQty = remainingQty + (oldGIQty - Qty_Actual);
                Dr.Close();
            }
            else if (ControlMgr.GroupName == "KERANI" && Cmd.ExecuteScalar().ToString() == "01")
            {
                Query = "Select Qty_SJ,ActionCodeStatus from GoodsReceivedD where GoodsReceivedId = '" + GRID + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                decimal oldGIQty = 0;
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    //action code status 03 = reject, 02 = parked
                    if (((Dr["ActionCodeStatus"].ToString() == "03" || Dr["ActionCodeStatus"].ToString() == "02") && (CurrentRowStatus == "Bongkar" || CurrentRowStatus == "Received")) || (cmbReferenceType.Text == "Nota Retur Beli"))
                    {
                        oldGIQty = 0;
                    }
                    //action code status 01 = Bongkar, 05 = Received
                    else if ((CurrentRowStatus != "Bongkar" && CurrentRowStatus != "Received") && (Dr["ActionCodeStatus"].ToString() == "01" || Dr["ActionCodeStatus"].ToString() == "05"))
                    {
                        oldGIQty = Convert.ToDecimal(Dr["Qty_SJ"]) + Qty_Actual;
                    }
                    else
                    {
                        oldGIQty = Convert.ToDecimal(Dr["Qty_SJ"]);
                    }
                }
                remainingQty = remainingQty + oldGIQty - Qty_Actual;
                Dr.Close();
            }
            else if (ControlMgr.GroupName == "KERANI" && Cmd.ExecuteScalar().ToString() == "02")
            {
                Query = "Select Qty_Actual,ActionCodeStatus from GoodsReceivedD where GoodsReceivedId = '" + GRID + "' and GoodsReceivedSeqNo = '" + GoodsReceivedSeqNo + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                decimal oldGIQty_Actual = 0;
                while (Dr.Read())
                {
                    //action code status 03 = reject, 02 = parked
                    if ((Dr["ActionCodeStatus"].ToString() == "03" || Dr["ActionCodeStatus"].ToString() == "02") && (CurrentRowStatus == "Bongkar" || CurrentRowStatus == "Received"))
                    {
                        oldGIQty_Actual = 0;
                    }
                    else if ((CurrentRowStatus != "Bongkar" && CurrentRowStatus != "Received") && (Dr["ActionCodeStatus"].ToString() == "01" || Dr["ActionCodeStatus"].ToString() == "05"))
                    {
                        oldGIQty_Actual = Convert.ToDecimal(Dr["Qty_Actual"]) + Qty_Actual;
                    }
                    else
                    {
                        oldGIQty_Actual = Convert.ToDecimal(Dr["Qty_Actual"]);
                    }
                }
                if (oldGIQty_Actual > Qty_Actual)
                    remainingQty = remainingQty + (oldGIQty_Actual - Qty_Actual);
                else if (oldGIQty_Actual < Qty_Actual)
                    remainingQty = remainingQty - (Qty_Actual - oldGIQty_Actual);

                Dr.Close();
            }

            //UPDATE RO REMAINING QTY 
            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":
                    Query = "update [dbo].[ReceiptOrderD] set [RemainingQty] = '" + remainingQty + "' where [ReceiptOrderId] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    break;
                case "Nota Transfer":
                    ////edited by Thaddaeus, 15May2018,23MAY2018 begin
                    //remaining sudah dikurang pas GI
                    ////Query = "update [dbo].[NotaTransferD] set [RemainingQty] = '" + remainingQty + "' where [TransferNo] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    //Query = "update a set a.[RemainingQty] = '" + remainingQty + "' FROM [dbo].[NotaTransferD] as a LEFT JOIN [dbo].[GoodsIssuedD] as b ON b.[RefTransID]=a.[TransferNo] AND a.[SeqNo]=b.[RefTransSeqNo] where b.[GoodsIssuedId] = '" + RefTransID + "' and b.[GoodsIssuedSeqNo] = '" + RefTransSeqNo + "'; ";
                    ////end===================================
                    break;
                case "Nota Retur Jual":
                    decimal oldGRQty_Actual = 0;
                    if (Cmd.ExecuteScalar().ToString() == "02")
                    {
                        Query = "Select [Qty_Actual] from [GoodsReceivedD] where [GoodsReceivedId] = '" + GRID + "' and [GoodsReceivedSeqNo] = '" + GoodsReceivedSeqNo + "'";
                        using (Cmd = new SqlCommand(Query, Conn))
                        {
                            oldGRQty_Actual = Convert.ToDecimal(Cmd.ExecuteScalar());
                        }
                    }
                    Query = "update [dbo].[NotaReturJual_Dtl] set [Remaining_Qty_DO] = 0 where [NRJID] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "' AND Remaining_Qty_DO IS NULL; ";
                    Query += "update [dbo].[NotaReturJual_Dtl] set [RemainingQty] -= '" + Qty_Actual + "',[Remaining_Qty_DO] +='" + (Qty_Actual - oldGRQty_Actual) + "' where [NRJID] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    break;
                case "Nota Retur Beli":
                    Query = "update [dbo].[NotaReturBeli_Dtl] set [Remaining_Qty_RO] -= '" + Qty_Actual + "' where [NRBId] = '" + RefTransID + "' and [SeqNo] = '" + RefTransSeqNo + "'; ";
                    break;
                default: Query = ""; break;
            }
            //hendry
            if (Query != "")
            {
                Cmd = new SqlCommand(Query, Conn);
                Cmd.ExecuteNonQuery();
            }
        }

        private void updateRefStatus(string RefTransID, string GRStat)
        {
            string status = "";

            //hendry
            if (cmbReferenceType.Text == "") return;
            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":
                    Query = "select count(*) from ReceiptOrderD where ReceiptOrderId = '" + RefTransID + "'";
                    break;
                case "Nota Transfer":
                    //edited by Thaddaeus, 15May2018, begin
                    Query = "Select count(*) From NotaTransferD where TransferNo = '" + tbxRefID.Text + "'";
                    //end==================================
                    break;
                case "Nota Retur Jual":
                    if (ControlMgr.GroupName != "KERANI")
                    {
                        if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                        {
                            return;
                        }
                    }
                    Query = "select count(*) from NotaReturJual_Dtl where NRJID = '" + RefTransID + "'";
                    break;
                case "Nota Retur Beli":
                    if (ControlMgr.GroupName != "KERANI")
                    {
                        if (txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
                        {
                            return;
                        }
                    }
                    Query = "select count(*) from [NotaReturBeli_Dtl] where [NRBId] = '" + RefTransID + "'";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            int countItem = (Int32)Cmd.ExecuteScalar();

            string oldRefStatus = "";
            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":
                    Query = "select Qty, RemainingQty from ReceiptOrderD where ReceiptOrderId = '" + RefTransID + "'";
                    oldRefStatus = "SELECT [ReceiptOrderStatus] FROM [dbo].[ReceiptOrderH] WHERE [ReceiptOrderId]=@ReceiptOrderId;";
                    using (Cmd = new SqlCommand(oldRefStatus, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@ReceiptOrderId", RefTransID);
                        oldRefStatus = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                    }
                    break;
                case "Nota Transfer":
                    //edited by Thaddaeus, 15May2018, begin
                    Query = "Select a.Qty, a.RemainingQty From NotaTransferD a where TransferNo = '" + tbxRefID.Text + "'";
                    //end===================================
                    break;
                //EDITED BY Thaddaeus, 1 SEPT 2018, START
                case "Nota Retur Jual":
                    Query = "select SUM(UoM_Qty),SUM(RemainingQty), SUM([Remaining_Qty_DO]) from NotaReturJual_Dtl where NRJId = '" + RefTransID + "'";
                    oldRefStatus = "SELECT [TransStatusId] FROM [dbo].[NotaReturJualH] WHERE [NRJId]=@NRJId;";
                    using (Cmd = new SqlCommand(oldRefStatus, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@NRJId", RefTransID);
                        oldRefStatus = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                    }
                    break;
                case "Nota Retur Beli":
                    Query = "select SUM(UoM_Qty), SUM(RemainingQty),SUM(Remaining_Qty_RO) from NotaReturBeli_Dtl where NRBId = '" + RefTransID + "'";
                    oldRefStatus = "SELECT [TransStatusId] FROM [dbo].[NotaReturBeliH] WHERE [NRBId]=@NRBId;";
                    using (Cmd = new SqlCommand(oldRefStatus, Conn))
                    {
                        Cmd.Parameters.AddWithValue("@NRBId", RefTransID);
                        oldRefStatus = Cmd.ExecuteScalar() == System.DBNull.Value ? "" : Cmd.ExecuteScalar().ToString();
                    }
                    break;
                //END================================
            }
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int checkZero = 0;
            while (Dr.Read())
            {
                //EDITED BY Thaddaeus, 1 SEPT 2018, START
                if (cmbReferenceType.Text == "Nota Retur Beli" || cmbReferenceType.Text == "Nota Retur Jual")
                {
                    decimal UoM_Qty = Dr[0] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[0]);
                    decimal RemainingQty = Dr[1] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[1]);
                    decimal Remaining_Qty_Ref = Dr[2] == System.DBNull.Value ? 0 : Convert.ToDecimal(Dr[2]);
                    if (cmbReferenceType.Text == "Nota Retur Beli" || cmbReferenceType.Text == "Nota Retur Jual")
                    {
                        if (RemainingQty == 0)
                        {
                            if (Remaining_Qty_Ref == 0)
                            {
                                if (cmbReferenceType.Text == "Nota Retur Beli")
                                {
                                    status = "11";
                                }
                                else if (cmbReferenceType.Text == "Nota Retur Jual")
                                {
                                    status = "08";
                                }
                            }
                            else if (Remaining_Qty_Ref != 0)
                            {
                                if (UoM_Qty == Remaining_Qty_Ref)
                                {
                                    if (cmbReferenceType.Text == "Nota Retur Beli")
                                    {
                                        status = "08";
                                    }
                                    else if (cmbReferenceType.Text == "Nota Retur Jual")
                                    {
                                        status = "11";
                                    }
                                }
                                else if (UoM_Qty != Remaining_Qty_Ref)
                                {
                                    if (cmbReferenceType.Text == "Nota Retur Beli")
                                    {
                                        status = "09";
                                    }
                                    else if (cmbReferenceType.Text == "Nota Retur Jual")
                                    {
                                        status = "12";
                                    }
                                }
                            }
                        }
                        else if (RemainingQty != 0)
                        {
                            if (Remaining_Qty_Ref == 0)
                            {
                                if (UoM_Qty == RemainingQty)
                                {
                                    status = "";
                                }
                                else if (UoM_Qty != RemainingQty)
                                {
                                    if (cmbReferenceType.Text == "Nota Retur Beli")
                                    {
                                        status = "09";
                                    }
                                    else if (cmbReferenceType.Text == "Nota Retur Jual")
                                    {
                                        status = "10";
                                    }
                                }
                            }
                            else if (Remaining_Qty_Ref != 0)
                            {
                                if (cmbReferenceType.Text == "Nota Retur Beli")
                                {
                                    status = "09";
                                }
                                else if (cmbReferenceType.Text == "Nota Retur Jual")
                                {
                                    status = "10";
                                }
                            }
                        }
                    }
                }
                //END================================
                else if (Convert.ToDecimal(Dr["RemainingQty"]) == 0)
                {
                    checkZero++;
                }
            }
            Dr.Close();

            //UPDATE REF STATUS
            if (Mode == "New" && txtSiteType.Text.ToUpper() != "VIRTUAL SITE")
            {
                switch (cmbReferenceType.Text)
                {
                    case "Receipt Order":
                        status = "02";
                        break;
                    case "Nota Transfer":
                        status = "03";
                        break;
                }
            }
            else if (checkZero == countItem)
            {
                switch (cmbReferenceType.Text)
                {
                    case "Receipt Order":
                        if (ControlMgr.GroupName.ToString() == "WB OPERATOR" && GRStat == "03")
                        {
                            status = "03";
                        }
                        else
                        {
                            status = "02";
                        }
                        break;
                    case "Nota Transfer":
                        status = "04";
                        break;
                }
            }
            else
            {
                switch (cmbReferenceType.Text)
                {
                    case "Receipt Order":
                        if (ControlMgr.GroupName.ToString() == "WB OPERATOR" && GRStat == "03")
                        {
                            status = "04";
                        }
                        else
                        {
                            status = "02";
                        }
                        break;
                    case "Nota Transfer":
                        status = "05";
                        break;
                }
            }

            if (status == "")
            {
                return;
            }

            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":
                    Query = "update ReceiptOrderH set ReceiptOrderStatus = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where ReceiptOrderId = '" + RefTransID + "'; ";
                    break;
                case "Nota Transfer":
                    //edited by Thaddaeus, 15May2018, begin
                    //Query = "update NotaTransferH set TransStatus = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.GroupName + "' where TransferNo = '" + RefTransID + "'; ";
                    Query = "update NotaTransferH set TransStatus = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where TransferNo = '" + tbxRefID.Text + "'; ";
                    //end===================================
                    break;
                case "Nota Retur Jual":
                    Query = "update NotaReturJualH set TransStatusId = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where NRJID= '" + RefTransID + "'; ";
                    break;
                case "Nota Retur Beli":
                    Query = "update [NotaReturBeliH] set TransStatusId = '" + status + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.UserId + "' where [NRBId]= '" + RefTransID + "'; ";
                    break;
            }
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            //insert master status log reference
            string LogDesc = "";
            if (oldRefStatus != "" && oldRefStatus == status)
            {
                LogDesc = "Edit";
            }
            switch (cmbReferenceType.Text)
            {
                case "Receipt Order":
                    //edited by Thaddaeus, 27 Sept2018
                    ListMethod.StatusLogVendor("GRHeaderV2", "RO", tbxNameID.Text, status, LogDesc, tbxRefID.Text, tbxGRNum.Text, "", "");
                    break;
                case "Nota Retur Beli":
                    ListMethod.StatusLogVendor("GRHeaderV2", "NotaReturBeli", tbxNameID.Text, status, LogDesc, tbxRefID.Text, tbxGRNum.Text, "", "");
                    break;

                case "Nota Retur Jual":
                    ListMethod.StatusLogCustomer("GRHeaderV2", "NotaReturJual", tbxNameID.Text, status, LogDesc, tbxRefID.Text, tbxGRNum.Text, "", "");
                    break;
            }
            //end==========================
        }

        private void tbxWeight1_Leave(object sender, EventArgs e)
        {
            if (tbxWeight1.Text == String.Empty || tbxWeight1.Text == ".")
                tbxWeight1.Text = "0";
            tbxWeight1.Text = string.Format("{0:#,0.00}", double.Parse(tbxWeight1.Text));
        }

        private void tbxWeight2_Leave(object sender, EventArgs e)
        {
            if (tbxWeight2.Text == String.Empty || tbxWeight2.Text == ".")
                tbxWeight2.Text = "0";
            tbxWeight2.Text = string.Format("{0:#,0.00}", double.Parse(tbxWeight2.Text));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            GetDataHeader();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                SearchV2 f = new SearchV2();
                f.SetMode("Check");
                if (tabControl1.SelectedIndex == 1) //GR NON REF
                {
                    dataGridView3.ColumnCount = tableColsName1.Length;
                    //GENERATE COLUMN HEADER NAME
                    for (int i = 0; i < dataGridView3.ColumnCount; i++)
                    {
                        dataGridView3.Columns[i].Name = tableColsName1[i];
                        dataGridView3.Columns[i].HeaderText = tableCols1Header1[i];
                    }
                    string stringFullitemID = "";
                    SchemaName = "dbo"; TableName = "InventTable"; Where = "";
                    switch (cmbReferenceType.Text)
                    {
                        case "Receipt Order":
                            Where = "AND a.FullItemID NOT IN (select FullItemId from ReceiptOrderD where ReceiptOrderId = '" + tbxRefID.Text + "')";
                            break;
                        case "Nota Transfer":
                            //edited by Thaddaeus, 15May2018, begin
                            //Where = "AND a.FullItemID NOT IN (select FullItemId from NotaTransferD where TransferNo = '" + tbxRefID.Text + "')";
                            Where = "AND a.FullItemID NOT IN (select [FullItemId] from [GoodsIssuedD] where [GoodsIssuedId] = '" + tbxRefId2.Text + "')";
                            //end
                            break;
                        case "Nota Retur Jual":
                            Where = "AND a.FullItemID NOT IN (select FullItemId from NotaReturJual_Dtl where NRJID = '" + tbxRefID.Text + "')";
                            break;
                        case "Nota Retur Beli":
                            Where = "AND a.FullItemID NOT IN (select [FullItemId] from [NotaReturBeli_Dtl] where [NRBId] = '" + tbxRefID.Text + "')";
                            break;
                    }
                    f.SetSchemaTable(SchemaName, TableName, Where, "a.*", TableName + " a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0)
                    {
                        for (int i = 0; i < SearchV2.data.Count; i++)
                        {
                            if (i >= 1)
                                stringFullitemID += ", ";
                            stringFullitemID += "'" + SearchV2.data[i] + "'";
                        }
                        Conn = ConnectionString.GetConnection();
                        Query = "select distinct a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.ItemID, a.FullItemID, a.ItemDeskripsi, a.[UoM], b.Ratio from [dbo].[InventTable] as a left join InventConversion as b on a.FullItemID = b.FullItemID  where a.[FullItemID] in (" + stringFullitemID + ")";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            this.dataGridView3.Rows.Add(Convert.ToInt32(dataGridView3.RowCount + 1), "", "", "", Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemID"], Dr["ItemDeskripsi"], "", "", "", "", Dr["UoM"], Dr["Ratio"], Dr["Ratio"], "", "", txtInventSiteID.Text, "", "", "", "");

                            //cellValue("select distinct c.InventSiteBlokID from [ReceiptOrderD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.ReceiptOrderId = '" + tbxRefID.Text + "'");
                            cellValue("select b.InventSiteBlokID from InventSite a left join InventSiteBlok b on a.InventSiteID = b.InventSiteID where a.InventSiteID = '" + txtInventSiteID.Text + "'");
                            cell.Value = "Select";
                            dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells["InventSiteBlokID"] = cell;

                            if (ControlMgr.GroupName == "KERANI")
                                //cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03', '09')"); //StatusCode in ('02', '03', '05')
                                cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03')"); //hendry 
                            else
                                cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03')"); //StatusCode in ('01', '02', '03')
                            cell.Value = "Select";
                            dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells["ActionCodeStatus"] = cell;

                            cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                            cell.Value = "Select";
                            dataGridView3.Rows[(dataGridView3.Rows.Count - 1)].Cells["Quality"] = cell;
                        }
                        Dr.Close();
                        Conn.Close();
                        if (Mode != "New")
                            ModeEdit();
                        else
                            ModeNew();
                    }
                }
                else if (tabControl1.SelectedIndex == 0)
                {
                    List<string[]> criteria = new List<string[]> { };
                    string refID = "";
                    string seqNo = "";
                    switch (cmbReferenceType.Text)
                    {
                        case "Receipt Order":
                            SchemaName = "dbo"; TableName = "ReceiptOrderD"; Where = "and a.ReceiptOrderId = '" + tbxRefID.Text + "'";// and a.RemainingQty > 0";
                            break;
                        case "Nota Transfer":
                            //edited by Thaddaeus, 15May2018, begin
                            //SchemaName = "dbo"; TableName = "NotaTransferD"; Where = "and a.TransferNo = '" + tbxRefID.Text + "'";
                            SchemaName = "dbo"; TableName = "GoodsIssuedD"; Where = "and a.GoodsIssuedId = '" + tbxRefId2.Text + "'";
                            //end
                            break;
                        case "Nota Retur Jual":
                            SchemaName = "dbo"; TableName = "NotaReturJual_Dtl"; Where = "and a.NRJID = '" + tbxRefID.Text + "'";
                            break;
                        case "Nota Retur Beli":
                            SchemaName = "dbo"; TableName = "[NotaReturBeli_Dtl]"; Where = "and a.[NRBId] = '" + tbxRefID.Text + "'";
                            break;
                    }
                    f.SetSchemaTable(SchemaName, TableName, Where, "a.*", TableName + " a");
                    f.ShowDialog();
                    if (SearchV2.data.Count != 0 && SearchV2.data2.Count != 0)
                    {
                        for (int i = 0; i < SearchV2.data.Count; i++)
                        {
                            if (i >= 1)
                                refID += ", ";
                            refID += "'" + SearchV2.data[i] + "'";
                        }
                        for (int i = 0; i < SearchV2.data2.Count; i++)
                        {
                            if (i >= 1)
                                seqNo += ", ";
                            seqNo += "'" + SearchV2.data2[i] + "'";
                        }
                        Conn = ConnectionString.GetConnection();
                        switch (cmbReferenceType.Text)
                        {
                            case "Receipt Order":
                                Query = "Select ReceiptOrderId 'RefID', SeqNo, GroupID, SubGroup1ID, SubGroup2ID, ItemID, FullItemID, ItemName, RemainingQty,Qty, Unit, Ratio, InventSiteID from ReceiptOrderD where ReceiptOrderID in (" + refID + ") and SeqNo in (" + seqNo + ")";
                                break;
                            case "Nota Transfer":
                                //edited by Thaddaeus, 15May2018, begin
                                Query = "select GoodsIssuedId 'RefID', GoodsIssuedSeqNo 'SeqNo', GroupId 'GroupID', SubGroup1Id 'SubGroup1ID',SubGroup1Id 'SubGroup2ID',[ItemId] 'ItemID',FullItemId 'FullItemID', ItemName,Qty,Remaining_Qty 'RemainingQty', Unit, Ratio, InventSiteId 'InventSiteID' from [GoodsIssuedD] where [GoodsIssuedId] in (" + refID + ") and [GoodsIssuedSeqNo] in (" + seqNo + ")";
                                //end
                                break;
                            case "Nota Retur Jual":
                                Query = "select NRJID 'RefID', SeqNo, GroupID, SubGroupID 'SubGroup1ID', SubGroup2ID, ItemID, FullItemID, ItemName,UoM_Qty as 'Qty', RemainingQty, Uom_Unit 'Unit', Ratio, InventSiteID from NotaReturJual_Dtl where NRJID in (" + refID + ") and SeqNo in (" + seqNo + ")";
                                break;
                            case "Nota Retur Beli":
                                Query = "select [NRBId] 'RefID', SeqNo, GroupID, SubGroup1Id 'SubGroup1ID', SubGroup2Id, ItemId, FullItemId, ItemName,UoM_Qty as 'Qty',[Remaining_Qty_RO] as 'RemainingQty', Uom_Unit 'Unit', Ratio, InventSiteId from [NotaReturBeli_Dtl] where [NRBId] in (" + refID + ") and SeqNo in (" + seqNo + ")";
                                break;
                        }
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            //created by Thaddaeus, 24MAY2018, BEGIN================
                            decimal QtySJ = 0;
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                if (dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value != null)
                                {
                                    if (dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() == Dr["FullItemID"].ToString())//&& Convert.ToInt32(dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value) != 0)
                                    {
                                        QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                                    }
                                }
                            }
                            if (QtySJ == 0)
                            {
                                QtySJ = Convert.ToDecimal(Dr["RemainingQty"]);
                            }
                            //END==============================================
                            this.dataGridView1.Rows.Add(Convert.ToInt32(dataGridView1.RowCount + 1), "", Dr["RefID"], Dr["SeqNo"], Dr["GroupID"], Dr["SubGroup1ID"], Dr["SubGroup2ID"], Dr["ItemID"], Dr["FullItemID"], Dr["ItemName"], Dr["Qty"], QtySJ, "", "", Dr["Unit"], Dr["Ratio"], Dr["Ratio"], QtySJ * Convert.ToDecimal(Dr["Ratio"]), QtySJ * Convert.ToDecimal(Dr["Ratio"]), Dr["InventSiteID"], "", "", "", "");

                            //cellValue("select distinct c.InventSiteBlokID from [ReceiptOrderD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.ReceiptOrderId = '" + tbxRefID.Text + "'");
                            cellValue("select b.InventSiteBlokID from InventSite a left join InventSiteBlok b on a.InventSiteID = b.InventSiteID where a.InventSiteID = '" + txtInventSiteID.Text + "'");
                            cell.Value = "Select";
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["InventSiteBlokID"] = cell;

                            if (ControlMgr.GroupName == "KERANI")
                                //cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03', '09')");                                  
                                cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('02', '03')"); //hendry
                            else
                                cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03')");
                            cell.Value = "Select";
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["ActionCodeStatus"] = cell;

                            cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                            cell.Value = "Select";
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Quality"] = cell;
                        }
                        Dr.Close();
                        Conn.Close();
                        if (Mode != "New")
                            ModeEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (dataGridView1.RowCount != 0)
                {
                    if (!(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["GoodsReceivedSeqNo"].Value == null))
                    {
                        MetroFramework.MetroMessageBox.Show(this, "Tidak bisa hapus item!\nItem ini di generate dari " + tbxGRNum.Text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    //else if (!(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["RefTransID"].Value == String.Empty || dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["RefTransID"].Value == null))
                    //{
                    //    MetroFramework.MetroMessageBox.Show(this, "Cannot delete this item!\nThis item is generated from " + tbxRefID.Text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                    else
                    {
                        dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                    }
                }
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                if (dataGridView3.RowCount != 0)
                {
                    if (!(dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells["GoodsReceivedSeqNo"].Value == String.Empty || dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells["GoodsReceivedSeqNo"].Value == null))
                    {
                        MetroFramework.MetroMessageBox.Show(this, "Tidak bisa delete item!\nItem ini di generate dari " + tbxGRNum.Text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    //else if (!(dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells["RefTransID"].Value == String.Empty || dataGridView3.Rows[dataGridView3.CurrentRow.Index].Cells["RefTransID"].Value == null))
                    //{
                    //    MetroFramework.MetroMessageBox.Show(this, "Cannot delete this item!\nThis item is generated from " + tbxRefID.Text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                    else
                    {
                        dataGridView3.Rows.RemoveAt(dataGridView3.CurrentRow.Index);
                    }
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
            GetDataHeader();

            //edited by Thaddaeus, 24 Sept 2018
            //for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //{
            //    if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Parked Tolerance - Need Action")
            //    {
            //        dataGridView1.Rows[i].Cells["Qty_Actual"].ReadOnly = true;
            //        dataGridView1.Rows[i].Cells["Qty_Actual"].Style.BackColor = Color.LightGray;
            //        dataGridView1.Rows[i].Cells["TotalBerat_Actual"].ReadOnly = true;
            //        dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Style.BackColor = Color.LightGray;
            //        dataGridView1.Rows[i].Cells["Quality"].ReadOnly = true;
            //        dataGridView1.Rows[i].Cells["Quality"].Style.BackColor = Color.LightGray;
            //        dataGridView1.Rows[i].Cells["ActionCodeStatus"].ReadOnly = true;
            //        dataGridView1.Rows[i].Cells["ActionCodeStatus"].Style.BackColor = Color.LightGray;
            //    }
            //}
            //end==============================
        }

        private void btnSOwner_Click(object sender, EventArgs e)
        {
            try
            {
                SchemaName = "dbo";
                TableName = "VendTable";
                Where = "";

                SearchV2 f = new SearchV2();
                f.SetMode("No");
                f.SetSchemaTable(SchemaName, TableName, Where, "a.*", "VendTable a");
                f.ShowDialog();
                if (SearchV2.data.Count != 0)
                {
                    tbxVOwnerID.Text = SearchV2.data[0];
                    tbxVOwner.Text = SearchV2.data[1];
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbVOwner_CheckedChanged(object sender, EventArgs e)
        {
            //if (cbVOwner.Checked == false)
            if (cbVOwner.Checked == true)
            {
                tbxVOwnerID.Text = tbxNameID.Text;
                tbxVOwner.Text = tbxName.Text;
                btnSOwner.Enabled = true;
            }
            else
            {
                //if (tbxRefID.Text != String.Empty && cbRef.Text != "Select")
                //{
                //    //using (Conn = ConnectionString.GetConnection())
                //    //{
                //    //    TableName = String.Empty;
                //    //    string id = String.Empty;
                //    //    if (cbRef.Text == "Receipt Order")
                //    //    {
                //    //        TableName = "ReceiptOrderH";
                //    //        id = "ReceiptOrderId";
                //    //    }
                //    //    else if (cbRef.Text == "Nota Transfer")
                //    //    {
                //    //        TableName = "NotaTransferH";
                //    //        id = "TransferNo";
                //    //    }
                //    //    else if (cbRef.Text == "Nota Retur Jual")
                //    //    {
                //    //        TableName = "NotaReturJualH";
                //    //        id = "NRJID";
                //    //    }
                //    //    Query = "select VendId, VendorName from " + TableName + " where " + id + " = '" + tbxRefID.Text + "'";
                //    //    using (Cmd = new SqlCommand(Query, Conn))
                //    //    {
                //    //        Dr = Cmd.ExecuteReader();
                //    //        while (Dr.Read())
                //    //        {
                //    //            tbxVOwnerID.Text = Dr["VendId"].ToString();
                //    //            tbxVOwner.Text = Dr["VendorName"].ToString();
                //    //        }
                //    //        Dr.Close();
                //    //    }
                //    //}
                //    tbxVOwnerID.Text = tbxNameID.Text;
                //    tbxVOwner.Text = tbxName.Text;
                //    btnSOwner.Enabled = false;
                //}
            }
        }

        private void tbxWeight1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void tbxWeight2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                e.Handled = true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 2)
            {
                btnAdd.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                if (tbxRefID.Text != String.Empty && (Mode == "Edit" || Mode == "New"))
                {
                    btnAdd.Enabled = true;
                    btnDelete.Enabled = true;
                }
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.PermissionAccess(ControlMgr.Approve) > 0)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        using (Conn = ConnectionString.GetConnection())
                        {
                            Query = "UPDATE [GoodsReceivedH] SET GoodsReceivedStatus = '06', [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.UserId + "' WHERE [GoodsReceivedId] = @GoodsReceivedId ";
                            using (Cmd = new SqlCommand(Query, Conn))
                            {
                                Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                                Cmd.ExecuteNonQuery();
                            }

                            Journal = UpdateGr(Conn);
                            if (Journal == true)
                            {
                                Journal = false;
                                goto Outer;
                            }
                            insertGoodsReceived_LogTable("06");

                            //created by Thaddaeus, 27 Sept 2018,begin
                            ListMethod.StatusLogVendor("GRHeaderV2", "GR", tbxNameID.Text, "06", "", tbxGRNum.Text, tbxRefID.Text, "", "");
                            //end=====================================

                            scope.Complete();
                        }
                    }
                    MetroFramework.MetroMessageBox.Show(this, "Approve sukses! Tolong kasi tau WB untuk aksi selanjutnya!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Parent.RefreshGrid();
                    Mode = "BeforeEdit";
                    GetDataHeader();
                    ModeBeforeEdit();

                Outer: ;
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { }
        }

        private bool CekNRJKembalikanBarang(string NRJID, int SeqNo)
        {
            bool stat = false;
            Query = "SELECT [ActionCode] FROM [NotaReturJual_Dtl] WHERE [NRJId] = @NRJId AND [SeqNo] = @SeqNo";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@NRJId", NRJID);
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                    stat = true;
                }
            }
            return stat;
        }

        private bool CekNRBKembalikanBarang(string NRBID, int SeqNo)
        {
            bool stat = false;
            Query = "SELECT [ActionCode] FROM [NotaReturBeli_Dtl] WHERE [NRBId] = @NRBId AND [SeqNo] = @SeqNo";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@NRBId", NRBID);
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                    stat = true;
                }
            }
            return stat;
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 06 Aug 2018
            //Desc : Create Journal

            string RefNo = tbxRefID.Text;
            string GRType = cmbReferenceType.Text;
            string SiteType = txtSiteType.Text;

            if (GRType.ToUpper() == "RECEIPT ORDER")
            {
                int CountAvailabe = 0;
                int CountResize = 0;
                int CountParked = 0;

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                    string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                    Boolean ResizeType = GetResizeType(FullItemID);

                    if (ActionCode.ToUpper().Contains("PARKED"))
                    {
                        CountParked = CountParked + 1;
                    }
                    else if (ActionCode.ToUpper().Contains("RECEIVED"))
                    {
                        if (ResizeType)
                        {
                            CountResize = CountResize + 1;
                        }
                        else
                        {
                            CountAvailabe = CountAvailabe + 1;
                        }
                    }
                }

                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                    string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                    Boolean ResizeType = GetResizeType(FullItemID);

                    if (ActionCode.ToUpper().Contains("PARKED"))
                    {
                        CountParked = CountParked + 1;
                    }
                    else if (ActionCode.ToUpper().Contains("RECEIVED"))
                    {
                        if (ResizeType)
                        {
                            CountResize = CountResize + 1;
                        }
                        else
                        {
                            CountAvailabe = CountAvailabe + 1;
                        }
                    }
                }

                decimal Available = 0, Resize = 0, Parked = 0;
                //decimal Tax = 0;
                //decimal POPPN = 0, POPPH = 0;
                string JournalHID = "";

                //POPPN = GetPOTax(RefNo, "PPN");
                //POPPH = GetPOTax(RefNo, "PPH");

                //GR Ref
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                    decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                    string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                    string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                    //GET PricePO
                    string Price = GetPOPrice(RefNo, RefSeqNo);

                    decimal PODPPPrice;
                    if (Price == "")
                    {
                        PODPPPrice = 0;
                    }
                    else
                    {
                        PODPPPrice = Convert.ToDecimal(Price);
                    }


                    Boolean ResizeType = GetResizeType(FullItemID);

                    if (ActionCode.ToUpper().Contains("PARKED"))
                    {
                        if (PODPPPrice == 0)
                        {
                            PODPPPrice = 1;
                            Parked = Parked + (PODPPPrice * QtySJ);
                        }
                        else
                        {
                            Parked = Parked + (PODPPPrice * QtySJ);
                        }

                        //if (POPPN != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPN) / 100);
                        //}

                        //if (POPPH != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPH) / 100);
                        //}

                    }
                    else if (ActionCode.ToUpper().Contains("RECEIVED"))
                    {
                        if (ResizeType)
                        {
                            Resize = Resize + (PODPPPrice * QtySJ);

                            //if (POPPN != 0)
                            //{
                            //    Tax = Tax + ((Resize * POPPN) / 100);
                            //}

                            //if (POPPH != 0)
                            //{
                            //    Tax = Tax + ((Resize * POPPH) / 100);
                            //}

                        }
                        else
                        {
                            Available = Available + (PODPPPrice * QtySJ);

                            //if (POPPN != 0)
                            //{
                            //    Tax = Tax + ((Available * POPPN) / 100);
                            //}

                            //if (POPPH != 0)
                            //{
                            //    Tax = Tax + ((Available * POPPH) / 100);
                            //}
                        }
                    }
                }

                //GR Non Ref
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                    decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value);
                    string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                    decimal PODPPPrice = 1;

                    if (ActionCode.ToUpper().Contains("PARKED"))
                    {
                        Parked = Parked + (PODPPPrice * QtySJ);

                        //if (POPPN != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPN) / 100);
                        //}

                        //if (POPPH != 0)
                        //{
                        //    Tax = Tax + ((Parked * POPPH) / 100);
                        //}
                    }
                }


                if (SiteType.ToUpper() == "PHYSICAL SITE")
                {
                    JournalHID = "IN01";
                }
                else
                {
                    JournalHID = "IN05";
                }

                string Notes = "";

                Notes = GetNotes();

                if (Available != 0 || Resize != 0 || Parked != 0)
                {
                    //Get GLJournalHID
                    Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + tbxGRNum.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

                    if (GLJournalHID != "")
                    {
                        Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                        Cmd = new SqlCommand(Query, Conn);
                        int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountData == 1)
                        {
                            //Delete Journal Detail
                            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                    }
                    else
                    {

                        //Insert Header GLJournal
                        string Jenis = "JN", Kode = "JN";
                        GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Cmd);

                        Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxGRNum.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        //Select Config Journal
                        int SeqNo = 1;
                        int JournalIDSeqNo = 0;
                        string Type = "";
                        string FQA_ID = "";
                        string FQA_Desc = "";
                        decimal AmountValue = 0;

                        Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                            Type = Convert.ToString(Dr["Type"]);
                            FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                            FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                            AmountValue = 0;

                            if (JournalHID == "IN01")
                            {
                                if (JournalIDSeqNo == 1)
                                {
                                    AmountValue = Resize;
                                }
                                else if (JournalIDSeqNo == 2)
                                {
                                    AmountValue = Available;
                                }
                                else if (JournalIDSeqNo == 3)
                                {
                                    AmountValue = Parked;
                                }
                                else if (JournalIDSeqNo == 4)
                                {
                                    AmountValue = Resize + Available + Parked;
                                }
                            }
                            else if (JournalHID == "IN05")
                            {
                                if (JournalIDSeqNo == 1)
                                {
                                    AmountValue = Resize;
                                }
                                else if (JournalIDSeqNo == 2)
                                {
                                    AmountValue = Available;
                                }
                                else if (JournalIDSeqNo == 3)
                                {
                                    AmountValue = Resize + Available;
                                }
                            }

                            if (AmountValue == 0)
                            {
                                continue;
                            }

                            //Insert Detail GLJournal
                            Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                            Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                            Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                            Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                        Dr.Close();
                    }
                }


                //K = KResize + KParked + KAvailable;

                //if (DParked != 0)
                //{
                //    JournalHID = "IN02";
                //}
                //else if (SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN01";
                //}
                //else
                //{
                //    JournalHID = "IN05";
                //}

                //if (DResize != 0 || DAvailable != 0 || DParked != 0)
                //{
                //    //Insert Header GLJournal
                //    string Jenis = "JN", Kode = "JN";
                //    string GLJournalHID = ConnectionString.GenerateSeqID(9, Jenis, Kode, Conn, Cmd);

                //    Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy]) ";
                //    Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxGRNum.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "')";
                //    Cmd = new SqlCommand(Query, Conn);
                //    Cmd.ExecuteNonQuery();

                //    //Select Config Journal
                //    int SeqNo = 1;
                //    int JournalIDSeqNo = 0;
                //    string Type = "";
                //    string FQA_ID = "";
                //    string FQA_Desc = "";
                //    decimal AmountValue = 0;

                //    Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                //    Cmd = new SqlCommand(Query, Conn);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                //        Type = Convert.ToString(Dr["Type"]);
                //        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                //        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);


                //        if (JournalHID == "IN01")
                //        {
                //            if (JournalIDSeqNo == 1)
                //            {
                //                AmountValue = DResize;
                //            }
                //            else if (JournalIDSeqNo == 2)
                //            {
                //                AmountValue = DAvailable;
                //            }
                //            else if (JournalIDSeqNo == 3)
                //            {
                //                AmountValue = K;
                //            }
                //        }
                //        if (JournalHID == "IN02")
                //        {
                //            if (JournalIDSeqNo == 1)
                //            {
                //                AmountValue = DResize;
                //            }
                //            else if (JournalIDSeqNo == 2)
                //            {
                //                AmountValue = DAvailable;
                //            }
                //            else if (JournalIDSeqNo == 2)
                //            {
                //                AmountValue = DParked;
                //            }
                //            else if (JournalIDSeqNo == 3)
                //            {
                //                AmountValue = K;
                //            }
                //        }
                //        if (JournalHID == "IN05")
                //        {
                //            if (JournalIDSeqNo == 1)
                //            {
                //                AmountValue = DResize;
                //            }
                //            else if (JournalIDSeqNo == 2)
                //            {
                //                AmountValue = DAvailable;
                //            }
                //            else if (JournalIDSeqNo == 3)
                //            {
                //                AmountValue = K;
                //            }
                //        }

                //        if (AmountValue == 0)
                //        {
                //            continue;
                //        }

                //        //Insert Detail GLJournal
                //        Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                //        Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                //        Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                //        Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                //        Cmd = new SqlCommand(Query, Conn);
                //        Cmd.ExecuteNonQuery();
                //        SeqNo++;
                //    }
                //    Dr.Close();
                //}               


                //if (CountAvailabe != 0 && CountResize != 0 && CountParked != 0 && SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN07";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (PODPPPrice == 0)
                //            {
                //                PODPPPrice = 1;
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;

                //            }
                //            else
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;
                //            }
                //        }
                //        else if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Resize)
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DResize = DResize + Amount;
                //                KResize = KResize + Amount;
                //            }
                //            else
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DAvailable = DAvailable + Amount;
                //                KAvailable = KAvailable + Amount;
                //            }
                //        }
                //    }

                //    //GR Non Ref
                //    for (int i = 0; i < dataGridView3.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //        decimal PODPPPrice = 1;

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            Amount = PODPPPrice + QtySJ;
                //            DParked = DParked + Amount;
                //            KParked = KParked + Amount;
                //        }
                //    }

                //    K = (KAvailable + KResize + KParked);
                //}
                //else if (CountAvailabe != 0 && CountResize == 0 && CountParked != 0 && SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN06";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (PODPPPrice == 0)
                //            {
                //                PODPPPrice = 1;
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;

                //            }
                //            else
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;
                //            }
                //        }
                //        else if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            Amount = PODPPPrice + QtySJ;
                //            DAvailable = DAvailable + Amount;
                //            KAvailable = KAvailable + Amount;                            
                //        }
                //    }

                //    //GR Non Ref
                //    for (int i = 0; i < dataGridView3.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //        decimal PODPPPrice = 1;

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            Amount = PODPPPrice + QtySJ;
                //            DParked = DParked + Amount;
                //            KParked = KParked + Amount;
                //        }
                //    }

                //    K = (KAvailable + KParked);
                //}
                //else if (CountAvailabe == 0 && CountResize != 0 && CountParked != 0 && SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN05";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (PODPPPrice == 0)
                //            {
                //                PODPPPrice = 1;
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;

                //            }
                //            else
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;
                //            }
                //        }
                //        else if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Resize)
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DResize = DResize + Amount;
                //                KResize = KResize + Amount;
                //            }
                //        }
                //    }

                //    //GR Non Ref
                //    for (int i = 0; i < dataGridView3.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //        decimal PODPPPrice = 1;

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            Amount = PODPPPrice + QtySJ;
                //            DParked = DParked + Amount;
                //            KParked = KParked + Amount;
                //        }
                //    }

                //    K = (KResize + KParked);
                //}
                //else if (CountAvailabe == 0 && CountResize == 0 && CountParked != 0 && SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN04";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (PODPPPrice == 0)
                //            {
                //                PODPPPrice = 1;
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;

                //            }
                //            else
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DParked = DParked + Amount;
                //                KParked = KParked + Amount;
                //            }
                //        }
                //    }

                //    //GR Non Ref
                //    for (int i = 0; i < dataGridView3.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //        decimal PODPPPrice = 1;

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            Amount = PODPPPrice + QtySJ;
                //            DParked = DParked + Amount;
                //            KParked = KParked + Amount;
                //        }
                //    }

                //    K = KParked;
                //}
                //else if (CountAvailabe != 0 && CountResize != 0 && CountParked == 0 && SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN03";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Resize)
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DResize = DResize + Amount;
                //                KResize = KResize + Amount;
                //            }
                //            else
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DAvailable = DAvailable + Amount;
                //                KAvailable = KAvailable + Amount;
                //            }
                //        }
                //    }

                //    K = (KAvailable + KResize);
                //}
                //else if (CountAvailabe != 0 && CountResize == 0 && CountParked == 0 && SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN02";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (!Resize)
                //            {                              
                //                Amount = PODPPPrice + QtySJ;
                //                DAvailable = DAvailable + Amount;
                //                KAvailable = KAvailable + Amount;
                //            }
                //        }
                //    }

                //    K = KAvailable;
                //}
                //else if (CountAvailabe == 0 && CountResize != 0 && CountParked == 0 && SiteType.ToUpper() == "PHYSICAL SITE")
                //{
                //    JournalHID = "IN01";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Resize)
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DResize = DResize + Amount;
                //                KResize = KResize + Amount;
                //            }
                //        }
                //    }

                //    K = KResize;
                //}
                //else if (CountAvailabe == 0 && CountResize != 0 && CountParked == 0 && SiteType.ToUpper() != "PHYSICAL SITE")
                //{
                //    JournalHID = "IN01";


                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Resize)
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DResize = DResize + Amount;
                //                KResize = KResize + Amount;
                //            }
                //        }
                //    }

                //    K = KResize;
                //}
                //else if (CountAvailabe != 0 && CountResize == 0 && CountParked == 0 && SiteType.ToUpper() != "PHYSICAL SITE")
                //{
                //    JournalHID = "IN02";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (!Resize)
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DAvailable = DAvailable + Amount;
                //                KAvailable = KAvailable + Amount;
                //            }
                //        }
                //    }

                //    K = KAvailable;
                //}
                //else if (CountAvailabe != 0 && CountResize != 0 && CountParked == 0 && SiteType.ToUpper() != "PHYSICAL SITE")
                //{
                //    JournalHID = "IN03";

                //    //GR Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                //        //GET PricePO
                //        string Price = GetPOPrice(RefNo, RefSeqNo);

                //        decimal PODPPPrice;
                //        if (Price == "")
                //        {
                //            PODPPPrice = 0;
                //        }
                //        else
                //        {
                //            PODPPPrice = Convert.ToDecimal(Price);
                //        }


                //        Boolean Resize = GetResizeType(FullItemID);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Resize)
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DResize = DResize + Amount;
                //                KResize = KResize + Amount;
                //            }
                //            else
                //            {
                //                Amount = PODPPPrice + QtySJ;
                //                DAvailable = DAvailable + Amount;
                //                KAvailable = KAvailable + Amount;
                //            }
                //        }
                //    }

                //    K = (KAvailable + KResize);
                //}

                ////Insert Header GLJournal
                //string Jenis = "JN", Kode = "JN";
                //string GLJournalHID = ConnectionString.GenerateSeqID(9, Jenis, Kode, Conn, Cmd);

                //Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy]) ";
                //Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxGRNum.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "')";
                //Cmd = new SqlCommand(Query, Conn);
                //Cmd.ExecuteNonQuery();

                ////Select Config Journal
                //int SeqNo = 1;
                //int JournalIDSeqNo = 0;
                //string Type = "";
                //string FQA_ID = "";
                //string FQA_Desc = "";
                //decimal AmountValue = 0;

                //Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                //Cmd = new SqlCommand(Query, Conn);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                //    Type = Convert.ToString(Dr["Type"]);
                //    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                //    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);


                //    if (JournalHID == "IN07")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DResize;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = DAvailable;
                //        }
                //        else if (JournalIDSeqNo == 4)
                //        {
                //            AmountValue = K;
                //        }
                //    }
                //    else if (JournalHID == "IN06")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DAvailable;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = K;
                //        }
                //    }
                //    else if (JournalHID == "IN05")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DResize;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = K;
                //        }
                //    }
                //    else if (JournalHID == "IN04")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = K;
                //        }
                //    }
                //    else if (JournalHID == "IN03")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DResize;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = K;
                //        }
                //    }
                //    else if (JournalHID == "IN02")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = K;
                //        }
                //    }
                //    else if (JournalHID == "IN01")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DResize;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = K;
                //        }
                //    }

                //    //Insert Detail GLJournal
                //    Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                //    Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                //    Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                //    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                //    Cmd = new SqlCommand(Query, Conn);
                //    Cmd.ExecuteNonQuery();
                //    SeqNo++;
                //}
                //Dr.Close();
            }
            else if (GRType.ToUpper() == "NOTA RETUR BELI")
            {
                int CountAvailabe = 0, CountParked = 0, CountRTB = 0, CountRDN = 0;

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                    string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                    if (ActionCode.ToUpper().Contains("PARKED"))
                    {
                        CountParked = CountParked + 1;
                    }
                    else if (ActionCode.ToUpper().Contains("RECEIVED"))
                    {
                        CountAvailabe = CountAvailabe + 1;
                    }

                    //Type RTB(Retur Tukar Barang)                        
                    CountRTB = CountRTB + GetCountRTB();

                    //Type Retur Debet Nota                        
                    CountRDN = CountRDN + GetCountRDN();
                }

                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                    string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                    if (ActionCode.ToUpper().Contains("PARKED"))
                    {
                        CountParked = CountParked + 1;
                    }

                    //Type RTB(Retur Tukar Barang)                        
                    CountRTB = CountRTB + GetCountRTB();

                    //Type Retur Debet Nota                        
                    CountRDN = CountRDN + GetCountRDN();
                }

                //decimal DRTBAvailable = 0, KRTBAvailable = 0, DRTBAvailableGainLoss = 0, KRTBAvailableGainLoss = 0;
                //decimal DRDNAvailable = 0, KRDNAvailable = 0, DRDNAvailableGainLoss = 0, KRDNAvailableGainLoss = 0;
                decimal DParked = 0, KParked = 0, DAvailable = 0, KAvailable = 0;
                //decimal DSum = 0, KSum = 0, DSumGainLoss = 0, KSumGainLoss = 0;
                string JournalHID = "";


                ////Ref
                //for (int i = 0; i < dataGridView1.RowCount; i++)
                //{
                //    string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //    string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);
                //    decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //    string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                //    if (ActionCode.ToUpper().Contains("BONGKAR"))
                //    {
                //        if (Unit.ToUpper() == "KG")
                //        {
                //            DAvailable = DAvailable + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //            KAvailable = KAvailable + GetPriceFromSO(FullItemID);
                //        }
                //        else
                //        {
                //            DAvailable = DAvailable + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //            KAvailable = KAvailable + GetPriceFromSO(FullItemID);
                //        }
                //    }
                //    else if (ActionCode.ToUpper().Contains("PARKED"))
                //    {
                //        if (Unit.ToUpper() == "KG")
                //        {
                //            DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //            KParked = DParked;
                //        }
                //        else
                //        {
                //            DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //            KParked = DParked;
                //        }
                //    }
                //}

                ////Non Ref
                //for (int i = 0; i < dataGridView3.RowCount; i++)
                //{
                //    string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //    string Unit = Convert.ToString(dataGridView3.Rows[i].Cells["Unit"].Value);
                //    decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //    string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //    if (ActionCode.ToUpper().Contains("PARKED"))
                //    {
                //        if (Unit.ToUpper() == "KG")
                //        {
                //            DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //            KParked = DParked;
                //        }
                //        else
                //        {
                //            DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //            KParked = DParked;
                //        }
                //    }
                //}

                //if (DParked != 0)
                //{
                //    JournalHID = "IN63";                    
                //}
                //else if (CountRDN != 0)
                //{
                //    JournalHID = "IN64";
                //}
                //else if (CountRTB == 0)
                //{
                //    JournalHID = "IN62";
                //}

                //if (DParked != 0 || CountRDN != 0 || CountRTB == 0)
                //{
                //    //Insert Header GLJournal
                //    string Jenis = "JN", Kode = "JN";
                //    string GLJournalHID = ConnectionString.GenerateSeqID(9, Jenis, Kode, Conn, Cmd);

                //    Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy]) ";
                //    Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxGRNum.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "')";
                //    Cmd = new SqlCommand(Query, Conn);
                //    Cmd.ExecuteNonQuery();

                //    //Select Config Journal
                //    int SeqNo = 1;
                //    int JournalIDSeqNo = 0;
                //    string Type = "";
                //    string FQA_ID = "";
                //    string FQA_Desc = "";
                //    decimal AmountValue = 0;

                //    Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                //    Cmd = new SqlCommand(Query, Conn);
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                //        Type = Convert.ToString(Dr["Type"]);
                //        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                //        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);


                //        if (JournalHID == "IN61" || JournalHID == "IN62")
                //        {
                //            if (JournalIDSeqNo == 1)
                //            {
                //                AmountValue = DAvailable;
                //            }
                //            else if (JournalIDSeqNo == 2)
                //            {
                //                AmountValue = KAvailable;
                //            }
                //            else if (JournalIDSeqNo == 3)
                //            {
                //                if (DAvailable > KAvailable)
                //                {
                //                    AmountValue = 0;
                //                }
                //                else
                //                {
                //                    AmountValue = KAvailable - DAvailable;
                //                }
                //            }
                //            else if (JournalIDSeqNo == 4)
                //            {
                //                if (DAvailable > KAvailable)
                //                {
                //                    AmountValue = DAvailable - KAvailable;
                //                }
                //                else
                //                {
                //                    AmountValue = 0;
                //                }
                //            }
                //        }
                //        else if (JournalHID == "IN64")
                //        {
                //            if (JournalIDSeqNo == 1)
                //            {
                //                AmountValue = DAvailable;
                //            }
                //            else if (JournalIDSeqNo == 2)
                //            {
                //                AmountValue = DParked;
                //            }
                //            else if (JournalIDSeqNo == 3)
                //            {
                //                AmountValue = KAvailable;
                //            }
                //            else if (JournalIDSeqNo == 4)
                //            {
                //                AmountValue = KAvailable + KParked;
                //            }
                //            else if (JournalIDSeqNo == 5)
                //            {
                //                if (DAvailable > KAvailable)
                //                {
                //                    AmountValue = 0;
                //                }
                //                else
                //                {
                //                    AmountValue = KAvailable - DAvailable;
                //                }
                //            }
                //            else if (JournalIDSeqNo == 6)
                //            {
                //                if (DAvailable > KAvailable)
                //                {
                //                    AmountValue = DAvailable - KAvailable;
                //                }
                //                else
                //                {
                //                    AmountValue = 0;
                //                }
                //            }
                //        }

                //        if (AmountValue == 0)
                //        {
                //            continue;
                //        }

                //        //Insert Detail GLJournal
                //        Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                //        Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                //        Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                //        Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                //        Cmd = new SqlCommand(Query, Conn);
                //        Cmd.ExecuteNonQuery();
                //        SeqNo++;
                //    }
                //    Dr.Close();
                //}




                //if (CountParked == 0 && CountAvailabe != 0 && CountRTB == 0 & CountRDN == 0)
                //{
                //    JournalHID = "IN61";

                //    //Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DRTBAvailable = DRTBAvailable + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KRTBAvailable = KRTBAvailable + GetPriceFromSO(FullItemID);
                //            }
                //            else
                //            {
                //                DRTBAvailable = DRTBAvailable + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KRTBAvailable = KRTBAvailable + GetPriceFromSO(FullItemID);
                //            }
                //        }                       
                //    }

                //    if(DRTBAvailable - KRTBAvailable != 0)
                //    {
                //        JournalHID = "IN62";

                //        if(DRTBAvailable > KRTBAvailable)
                //        {
                //            KRTBAvailableGainLoss = DRTBAvailable - KRTBAvailable;
                //            DRTBAvailableGainLoss = 0;
                //        }
                //        else
                //        {
                //            DRTBAvailableGainLoss = KRTBAvailable - DRTBAvailable;
                //            KRTBAvailableGainLoss = 0;
                //        }
                //    }
                //}
                //else if (CountParked == 0 && CountAvailabe != 0 && CountRDN != 0)
                //{
                //    JournalHID = "IN63";

                //    //Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DRDNAvailable = DRDNAvailable + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KRDNAvailable = KRDNAvailable + GetPriceFromSO(FullItemID);
                //            }
                //            else
                //            {
                //                DRDNAvailable = DRDNAvailable + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KRDNAvailable = KRDNAvailable + GetPriceFromSO(FullItemID);
                //            }
                //        }                      
                //    }

                //    if (DRDNAvailable - KRDNAvailable != 0)
                //    {
                //        JournalHID = "IN64";

                //        if (DRDNAvailable > KRDNAvailable)
                //        {
                //            KRDNAvailableGainLoss = DRDNAvailable - KRDNAvailable;
                //            DRDNAvailableGainLoss = 0;
                //        }
                //        else
                //        {
                //            DRDNAvailableGainLoss = KRDNAvailable - DRDNAvailable;
                //            KRDNAvailableGainLoss = 0;
                //        }
                //    }
                //}
                //else if (CountParked != 0 && CountAvailabe == 0)
                //{
                //    JournalHID = "IN65";

                //    //Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //            else
                //            {
                //                DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //        }                       
                //    }

                //    //Non Ref
                //    for (int i = 0; i < dataGridView3.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView3.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //            else
                //            {
                //                DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //        }
                //    }
                //}
                //else if (CountParked != 0 && CountAvailabe != 0 && CountRTB == 0 && CountRDN == 0)
                //{
                //    JournalHID = "IN66";

                //    //Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DRTBAvailable = DRTBAvailable + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KRTBAvailable = KRTBAvailable + GetPriceFromSO(FullItemID);
                //            }
                //            else
                //            {
                //                DRTBAvailable = DRTBAvailable + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KRTBAvailable = KRTBAvailable + GetPriceFromSO(FullItemID);
                //            }
                //        }
                //        else if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //            else
                //            {
                //                DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //        }
                //    }

                //    //Non Ref
                //    for (int i = 0; i < dataGridView3.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView3.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //            else
                //            {
                //                DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //        }
                //    }

                //    DSum = DRTBAvailable + DParked;
                //    KSum = KRTBAvailable + KParked;

                //    if (DSum - KSum != 0)
                //    {
                //        JournalHID = "IN67";

                //        if (DSum > KSum)
                //        {
                //            KSumGainLoss = DSum - KSum;
                //            DSumGainLoss = 0;
                //        }
                //        else
                //        {
                //            DSumGainLoss = KSum - DSum;
                //            KSumGainLoss = 0;
                //        }
                //    }
                //}
                //else if (CountParked != 0 && CountAvailabe != 0 && CountRDN != 0)
                //{
                //    JournalHID = "IN68";

                //    //Ref
                //    for (int i = 0; i < dataGridView1.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("BONGKAR"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DRDNAvailable = DRDNAvailable + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KRDNAvailable = KRDNAvailable + GetPriceFromSO(FullItemID);
                //            }
                //            else
                //            {
                //                DRDNAvailable = DRDNAvailable + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KRDNAvailable = KRDNAvailable + GetPriceFromSO(FullItemID);
                //            }
                //        }
                //        else if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //            else
                //            {
                //                DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //        }
                //    }

                //    //Non Ref
                //    for (int i = 0; i < dataGridView3.RowCount; i++)
                //    {
                //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
                //        string Unit = Convert.ToString(dataGridView3.Rows[i].Cells["Unit"].Value);
                //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
                //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                //        if (ActionCode.ToUpper().Contains("PARKED"))
                //        {
                //            if (Unit.ToUpper() == "KG")
                //            {
                //                DParked = DParked + GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //            else
                //            {
                //                DParked = DParked + GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                //                KParked = DParked;
                //            }
                //        }
                //    }

                //    DSum = DRDNAvailable + DParked;
                //    KSum = KRDNAvailable + KParked;

                //    if (DSum - KSum != 0)
                //    {
                //        JournalHID = "IN69";

                //        if (DSum > KSum)
                //        {
                //            KSumGainLoss = DSum - KSum;
                //            DSumGainLoss = 0;
                //        }
                //        else
                //        {
                //            DSumGainLoss = KSum - DSum;
                //            KSumGainLoss = 0;
                //        }
                //    }
                //}

                // //Insert Header GLJournal
                //string Jenis = "JN", Kode = "JN";
                //string GLJournalHID = ConnectionString.GenerateSeqID(9, Jenis, Kode, Conn, Cmd);

                //Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy]) ";
                //Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxGRNum.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "')";
                //Cmd = new SqlCommand(Query, Conn);
                //Cmd.ExecuteNonQuery();

                // //Select Config Journal
                //int SeqNo = 1;
                //int JournalIDSeqNo = 0;
                //string Type = "";
                //string FQA_ID = "";
                //string FQA_Desc = "";
                //decimal AmountValue = 0;

                //Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                //Cmd = new SqlCommand(Query, Conn);
                //Dr = Cmd.ExecuteReader();
                //while (Dr.Read())
                //{
                //    JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                //    Type = Convert.ToString(Dr["Type"]);
                //    FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                //    FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);


                //    if (JournalHID == "IN61")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRTBAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = KRTBAvailable;
                //        }
                //    }
                //    if (JournalHID == "IN62")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRTBAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DRTBAvailableGainLoss;
                //        }
                //        if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = KRTBAvailable;
                //        }
                //        else if (JournalIDSeqNo == 4)
                //        {
                //            AmountValue = KRTBAvailableGainLoss;
                //        }
                //    }
                //    if (JournalHID == "IN63")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRDNAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = KRDNAvailable;
                //        }
                //    }
                //    if (JournalHID == "IN64")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRDNAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DRDNAvailableGainLoss;
                //        }
                //        if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = KRDNAvailableGainLoss;
                //        }
                //        else if (JournalIDSeqNo == 4)
                //        {
                //            AmountValue = KRDNAvailable;
                //        }
                //    }
                //    if (JournalHID == "IN65")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = KParked;
                //        }
                //    }
                //    if (JournalHID == "IN66")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRTBAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = KSum;
                //        }
                //    }
                //    if (JournalHID == "IN67")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRTBAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DSumGainLoss;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 4)
                //        {
                //            AmountValue = KSum;
                //        }
                //        else if (JournalIDSeqNo == 5)
                //        {
                //            AmountValue = KSumGainLoss;
                //        }
                //    }
                //    if (JournalHID == "IN68")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRDNAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = KParked;
                //        }
                //        else if (JournalIDSeqNo == 4)
                //        {
                //            AmountValue = KRDNAvailable;
                //        }
                //    }
                //    if (JournalHID == "IN69")
                //    {
                //        if (JournalIDSeqNo == 1)
                //        {
                //            AmountValue = DRDNAvailable;
                //        }
                //        else if (JournalIDSeqNo == 2)
                //        {
                //            AmountValue = DSumGainLoss;
                //        }
                //        else if (JournalIDSeqNo == 3)
                //        {
                //            AmountValue = DParked;
                //        }
                //        else if (JournalIDSeqNo == 4)
                //        {
                //            AmountValue = KParked;
                //        }
                //        else if (JournalIDSeqNo == 5)
                //        {
                //            AmountValue = KSumGainLoss;
                //        }
                //        else if (JournalIDSeqNo == 6)
                //        {
                //            AmountValue = KRDNAvailable;
                //        }
                //    }

                //    //Insert Detail GLJournal
                //    Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                //    Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                //    Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                //    Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                //    Cmd = new SqlCommand(Query, Conn);
                //    Cmd.ExecuteNonQuery();
                //    SeqNo++;
                //}
                //Dr.Close();
            }
            else if (GRType.ToUpper() == "NOTA RETUR JUAL")
            {
                string ReturJualType = GetReturJualType();
                decimal TukarBarang = 0, CreditNote = 0, AvgPrice = 0, DVarian = 0, KVarian = 0;
                string JournalHID = "";


                //Tukar Barang
                if (ReturJualType == "01")
                {
                    JournalHID = "IN61";

                    //GR Ref
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        decimal QtyActual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

                        //GET PriceBBK
                        string Price = GetBBKPrice(RefSeqNo);

                        decimal BKKPrice;
                        if (Price == "")
                        {
                            BKKPrice = 1;
                        }
                        else
                        {
                            BKKPrice = Convert.ToDecimal(Price);
                        }

                        if (ActionCode.ToUpper().Contains("PARKED") || ActionCode.ToUpper().Contains("RECEIVED"))
                        {
                            TukarBarang = TukarBarang + (BKKPrice * QtyActual);
                        }
                    }

                    //GR Non Ref
                    for (int i = 0; i < dataGridView3.RowCount; i++)
                    {
                        decimal QtyActual = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value);
                        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

                        decimal BKKPrice = 1;

                        if (ActionCode.ToUpper().Contains("PARKED"))
                        {
                            TukarBarang = TukarBarang + (BKKPrice * QtyActual);
                        }
                    }
                }
                //Credit Note
                else if (ReturJualType == "02")
                {
                    JournalHID = "IN62";

                    //GR Ref
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        decimal QtyActual = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
                        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);
                        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                        string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);

                        //GET PriceBBK
                        string Price = GetBBKPrice(RefSeqNo);

                        decimal BKKPrice;
                        if (Price == "")
                        {
                            BKKPrice = 1;
                        }
                        else
                        {
                            BKKPrice = Convert.ToDecimal(Price);
                        }

                        if (ActionCode.ToUpper().Contains("PARKED") || ActionCode.ToUpper().Contains("RECEIVED"))
                        {
                            CreditNote = CreditNote + (BKKPrice * QtyActual);
                            decimal InventPrice = 0;
                            if (Unit.ToUpper() == "KG")
                            {
                                InventPrice = GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                                AvgPrice = AvgPrice + (QtyActual * InventPrice);
                            }
                            else
                            {
                                InventPrice = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                                AvgPrice = AvgPrice + (QtyActual * InventPrice);
                            }
                        }
                    }

                    //GR Non Ref
                    for (int i = 0; i < dataGridView3.RowCount; i++)
                    {
                        decimal QtyActual = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_Actual"].Value);
                        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);
                        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
                        string Unit = Convert.ToString(dataGridView1.Rows[i].Cells["Unit"].Value);

                        decimal BKKPrice = 1;

                        if (ActionCode.ToUpper().Contains("PARKED"))
                        {
                            CreditNote = CreditNote + (BKKPrice * QtyActual);

                            decimal InventPrice = 0;
                            if (Unit.ToUpper() == "KG")
                            {
                                InventPrice = GetPriceFromInventTable("Alt_AvgPrice", FullItemID);
                                AvgPrice = AvgPrice + (QtyActual * InventPrice);
                            }
                            else
                            {
                                InventPrice = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);
                                AvgPrice = AvgPrice + (QtyActual * InventPrice);
                            }
                        }
                    }

                    if (CreditNote - AvgPrice >= 0)
                    {
                        DVarian = CreditNote - AvgPrice;
                    }
                    else
                    {
                        KVarian = AvgPrice - CreditNote;
                    }
                }

                string Notes = "";

                Notes = GetNotes();

                if (TukarBarang != 0 || CreditNote != 0 || AvgPrice != 0)
                {
                    //Get GLJournalHID
                    Query = "SELECT GLJournalHID FROM GLJournalH WHERE Referensi = '" + tbxGRNum.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn);
                    string GLJournalHID = Convert.ToString(Cmd.ExecuteScalar());

                    if (GLJournalHID != "")
                    {
                        Query = "SELECT COUNT(GLJournalHID) FROM GLJournalH WHERE UPPER(Status) = 'GUNAKAN' AND Posting = 0 AND GLJournalHID = '" + GLJournalHID + "' ";
                        Cmd = new SqlCommand(Query, Conn);
                        int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                        if (CountData == 1)
                        {
                            //Delete Journal Detail
                            Query = "DELETE FROM GLJournalDtl WHERE GLJournalHID = '" + GLJournalHID + "'";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MetroFramework.MetroMessageBox.Show(this, "Tidak dapat diedit karena Jurnal sudah di posting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else
                    {

                        //Insert Header GLJournal
                        string Jenis = "JN", Kode = "JN";
                        GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, Conn, Cmd);

                        Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                        Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + tbxGRNum.Text + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                        Cmd = new SqlCommand(Query, Conn);
                        Cmd.ExecuteNonQuery();

                        //Select Config Journal
                        int SeqNo = 1;
                        int JournalIDSeqNo = 0;
                        string Type = "";
                        string FQA_ID = "";
                        string FQA_Desc = "";
                        decimal AmountValue = 0;

                        Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                            Type = Convert.ToString(Dr["Type"]);
                            FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                            FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                            AmountValue = 0;

                            if (JournalHID == "IN61")
                            {
                                if (JournalIDSeqNo == 1)
                                {
                                    AmountValue = TukarBarang;
                                }
                                else if (JournalIDSeqNo == 2)
                                {
                                    AmountValue = TukarBarang;
                                }
                            }
                            else if (JournalHID == "IN62")
                            {
                                if (JournalIDSeqNo == 1)
                                {
                                    AmountValue = AvgPrice;
                                }
                                else if (JournalIDSeqNo == 2)
                                {
                                    AmountValue = CreditNote;
                                }
                                else if (JournalIDSeqNo == 3)
                                {
                                    if (DVarian > 0)
                                    {
                                        AmountValue = DVarian;
                                    }
                                }
                                else if (JournalIDSeqNo == 4)
                                {
                                    if (KVarian > 0)
                                    {
                                        AmountValue = KVarian;
                                    }
                                }
                                else if (JournalIDSeqNo == 5)
                                {
                                    AmountValue = CreditNote;
                                }
                                else if (JournalIDSeqNo == 6)
                                {
                                    AmountValue = CreditNote;
                                }
                            }

                            if (AmountValue == 0)
                            {
                                continue;
                            }

                            //Insert Detail GLJournal
                            Query = "INSERT INTO [GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID] ";
                            Query += ",[FQADesc],[JournalDType],[Auto],[Amount],[CreatedDate],[CreatedBy]) ";
                            Query += "VALUES('" + GLJournalHID + "', '" + SeqNo + "', '" + JournalHID + "', '" + JournalIDSeqNo + "', '" + FQA_ID + "' ";
                            Query += ", '" + FQA_Desc + "', '" + Type + "', 'Auto', " + AmountValue + ", GETDATE(), '" + ControlMgr.UserId + "')";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();
                            SeqNo++;
                        }
                        Dr.Close();
                    }
                }
            }

            //End

            //string RefNo = tbxRefID.Text;
            //string GRType = cmbReferenceType.Text;
            //if (GRType.ToUpper() == "RECEIPT ORDER")
            //{
            //    string GLJournalID = ConnectionString.GenerateSequenceNo(10, "GLJournal", "GLJournalID", Conn, Cmd);
            //    int SeqNo = 1;

            //    //Detail GR
            //    for (int i = 0; i < dataGridView1.RowCount; i++)
            //    {
            //        string FullItemID = Convert.ToString(dataGridView1.Rows[i].Cells["FullItemId"].Value);
            //        decimal QtySJ = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value);
            //        string ActionCode = Convert.ToString(dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value);
            //        string RefSeqNo = Convert.ToString(dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);

            //        //GET PricePO
            //        Query = "SELECT PO.Price FROM ReceiptOrderD RO INNER JOIN PurchDtl PO ";
            //        Query += "ON PO.PurchID = RO.PurchaseOrderId ";
            //        Query += "WHERE RO.ReceiptOrderId = '" + RefNo + "' ";
            //        Query += "AND RO.SeqNo = '" + RefSeqNo + "' ";
            //        Query += "AND PO.SeqNo = RO.PurchaseOrderSeqNo";
            //        Cmd = new SqlCommand(Query, Conn);
            //        string Price = Convert.ToString(Cmd.ExecuteScalar());
            //        Decimal PODPPPrice;
            //        if (Price == "")
            //        {
            //            PODPPPrice = 0;
            //        }
            //        else
            //        {
            //            PODPPPrice = Convert.ToDecimal(Price);
            //        }

            //        Query = "SELECT Resize FROM InventTable WHERE FullItemID = '" + FullItemID + "' ";
            //        Cmd = new SqlCommand(Query, Conn);
            //        Boolean Resize = Convert.ToBoolean(Cmd.ExecuteScalar());

            //        string JournalHID = "";
            //        decimal Amount = 0;

            //        if(txtSiteType.Text.ToUpper() == "PHYSICAL SITE")
            //        {
            //            if (ActionCode.ToUpper() == "BONGKAR")
            //            {
            //                if (Resize)
            //                {
            //                    JournalHID = "IN01";
            //                }
            //                else
            //                {
            //                    JournalHID = "IN11";
            //                }

            //                Amount = PODPPPrice * QtySJ;
            //            }
            //            else if (ActionCode.ToUpper().Contains("PARKED"))
            //            {
            //                JournalHID = "IN02";

            //                if (PODPPPrice <= 0)
            //                {
            //                    Amount = 1;
            //                }
            //                else
            //                {
            //                    Amount = PODPPPrice * QtySJ;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (ActionCode.ToUpper() == "BONGKAR")
            //            {
            //                if (Resize)
            //                {
            //                    JournalHID = "IN05";
            //                }
            //                else
            //                {
            //                    JournalHID = "IN06";
            //                }

            //                Amount = PODPPPrice * QtySJ;
            //            }
            //        }

            //        if ((txtSiteType.Text.ToUpper() == "PHYSICAL SITE" && (ActionCode.ToUpper() == "BONGKAR" || ActionCode.ToUpper().Contains("PARKED"))) || (txtSiteType.Text.ToUpper() != "PHYSICAL SITE" && ActionCode.ToUpper() == "BONGKAR"))
            //        {  
            //            //SELECT Config Journal
            //            Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            //            Cmd = new SqlCommand(Query, Conn);
            //            Dr = Cmd.ExecuteReader();
            //            while (Dr.Read())
            //            {
            //                if (Convert.ToString(Dr["Type"]).ToUpper() == "K")
            //                {
            //                    Amount = Amount * -1;
            //                }

            //                //INSERT INTO GLJournal
            //                Query = "INSERT INTO [GLJournal]([GLJournalID],[SeqNo],[Referensi], [FullItemID],[JournalHID],[JournalHSeqNo],[Type],[FQA_ID] ";
            //                Query += ", [FQA_Desc],[Amount],[Posting] ,[Auto],[Status],[CreatedDate],[CreatedBy]) ";
            //                Query += "VALUES('" + GLJournalID + "', " + SeqNo + ", '" + tbxGRNum.Text + "', '" + FullItemID + "', '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "'  ";
            //                Query += ", '" + Convert.ToString(Dr["Type"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', '" + Convert.ToString(Dr["FQA_Desc"]) + "' ";
            //                Query += "," + Amount + ", 0, 1, 'Gunakan', GETDATE(), '" + ControlMgr.UserId + "'  ) ";
            //                Cmd = new SqlCommand(Query, Conn);
            //                Cmd.ExecuteNonQuery();
            //                SeqNo++;
            //            }
            //            Dr.Close();
            //        }                  
            //    }

            //    //Detail GR Non Ref
            //    //Detail GR
            //    for (int i = 0; i < dataGridView3.RowCount; i++)
            //    {
            //        string FullItemID = Convert.ToString(dataGridView3.Rows[i].Cells["FullItemId"].Value);
            //        decimal QtySJ = Convert.ToDecimal(dataGridView3.Rows[i].Cells["Qty_SJ"].Value);
            //        string ActionCode = Convert.ToString(dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value);

            //        Decimal PODPPPrice = 1;                   

            //        string JournalHID = "";
            //        decimal Amount = 0;

            //        if (txtSiteType.Text.ToUpper() == "PHYSICAL SITE")
            //        {
            //            if (ActionCode.ToUpper().Contains("PARKED"))
            //            {
            //               JournalHID = "IN02";
            //               Amount = PODPPPrice * QtySJ;                            
            //            }
            //        }

            //        if (txtSiteType.Text.ToUpper() == "PHYSICAL SITE" && ActionCode.ToUpper().Contains("PARKED"))
            //        {
            //            //SELECT Config Journal
            //            Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
            //            Cmd = new SqlCommand(Query, Conn);
            //            Dr = Cmd.ExecuteReader();
            //            while (Dr.Read())
            //            {
            //                if (Convert.ToString(Dr["Type"]).ToUpper() == "K")
            //                {
            //                    Amount = Amount * -1;
            //                }

            //                //INSERT INTO GLJournal
            //                Query = "INSERT INTO [GLJournal]([GLJournalID],[SeqNo],[Referensi], [FullItemID],[JournalHID],[JournalHSeqNo],[Type],[FQA_ID] ";
            //                Query += ", [FQA_Desc],[Amount],[Posting] ,[Auto],[Status],[CreatedDate],[CreatedBy]) ";
            //                Query += "VALUES('" + GLJournalID + "', " + SeqNo + ", '" + tbxGRNum.Text + "', '" + FullItemID + "', '" + JournalHID + "', '" + Convert.ToString(Dr["SeqNo"]) + "'  ";
            //                Query += ", '" + Convert.ToString(Dr["Type"]) + "', '" + Convert.ToString(Dr["FQA_ID"]) + "', '" + Convert.ToString(Dr["FQA_Desc"]) + "' ";
            //                Query += "," + Amount + ", 0, 1, 'Gunakan', GETDATE(), '" + ControlMgr.UserId + "'  ) ";
            //                Cmd = new SqlCommand(Query, Conn);
            //                Cmd.ExecuteNonQuery();
            //                SeqNo++;
            //            }
            //            Dr.Close();
            //        }
            //    }
            //}
            //End
        }

        private string GetReturJualType()
        {
            Query = "SELECT ActionCode FROM NotaReturJualH WHERE NRJId = '" + tbxRefID.Text + "'";

            Cmd = new SqlCommand(Query, Conn);
            string ActionCode = Convert.ToString(Cmd.ExecuteScalar());

            return ActionCode;
        }

        private string GetBBKPrice(string RefSeqNo)
        {
            //GET BBK Price
            Query = "SELECT GI.Price FROM GoodsIssuedD GI INNER JOIN NotaReturJual_Dtl NR ";
            Query += "ON NR.GoodsIssuedId = GI.GoodsIssuedId ";
            Query += "AND NR.GoodsIssued_SeqNo = GI.GoodsIssuedSeqNo ";
            Query += "AND NR.NRJId = '" + tbxRefID.Text + "' ";
            Query += "AND NR.SeqNo = " + RefSeqNo + " ";

            Cmd = new SqlCommand(Query, Conn);
            string Price = Convert.ToString(Cmd.ExecuteScalar());

            return Price;
        }


        private string GetPOPrice(string RefNo, string RefSeqNo)
        {
            //GET PricePO
            Query = "SELECT PO.Price FROM ReceiptOrderD RO INNER JOIN PurchDtl PO ";
            Query += "ON PO.PurchID = RO.PurchaseOrderId ";
            Query += "WHERE RO.ReceiptOrderId = '" + RefNo + "' ";
            Query += "AND RO.SeqNo = '" + RefSeqNo + "' ";
            Query += "AND PO.SeqNo = RO.PurchaseOrderSeqNo";

            Cmd = new SqlCommand(Query, Conn);
            string Price = Convert.ToString(Cmd.ExecuteScalar());

            return Price;
        }

        private decimal GetPOTax(string RefNo, string FieldName)
        {
            //GET Tax
            Query = "SELECT PO." + FieldName + " FROM ReceiptOrderH RO INNER JOIN PurchH PO ";
            Query += "ON PO.PurchID = RO.PurchaseOrderId ";
            Query += "WHERE RO.ReceiptOrderId = '" + RefNo + "' ";

            Cmd = new SqlCommand(Query, Conn);
            string result = Convert.ToString(Cmd.ExecuteScalar());
            decimal Tax = 0;
            if (result == "")
            {
                Tax = 0;
            }
            else
            {
                Tax = Convert.ToDecimal(result);
            }

            return Tax;
        }

        private bool GetResizeType(string FullItemID)
        {
            Query = "SELECT Resize FROM InventTable WHERE FullItemID = '" + FullItemID + "' ";

            Cmd = new SqlCommand(Query, Conn);
            Boolean Resize = Convert.ToBoolean(Cmd.ExecuteScalar());

            return Resize;
        }

        private int GetCountRTB()
        {
            //Type RTB(Retur Tukar Barang)
            Query = "SELECT COUNT(GR.GoodsReceivedId) FROM GoodsReceivedH GR ";
            Query += "INNER JOIN NotaReturJualH NR ON NR.NRJId = RefTransID ";
            Query += "INNER JOIN GoodsIssuedH GI ON GI.GoodsIssuedId = NR.GoodsIssuedId ";
            Query += "INNER JOIN CustInvoice_Dtl_SO_Dtl CI ON CI.GI_No = GI.GoodsIssuedId ";
            Query += "WHERE GR.GoodsReceivedId = '" + tbxGRNum.Text + "' ";
            Query += "AND UPPER(GR.RefTransType) = 'NOTA RETUR JUAL' ";
            Query += "AND NR.ActionCode = '01' ";

            Cmd = new SqlCommand(Query, Conn);
            int CountRTB = Convert.ToInt32(Cmd.ExecuteScalar());
            return CountRTB;
        }

        private int GetCountRDN()
        {
            //Type Retur Debet Nota
            Query = "SELECT COUNT(GR.GoodsReceivedId) FROM GoodsReceivedH GR ";
            Query += "INNER JOIN NotaReturJualH NR ON NR.NRJId = RefTransID ";
            Query += "INNER JOIN GoodsIssuedH GI ON GI.GoodsIssuedId = NR.GoodsIssuedId ";
            Query += "WHERE GR.GoodsReceivedId = '" + tbxGRNum.Text + "' ";
            Query += "AND UPPER(GR.RefTransType) = 'NOTA RETUR JUAL' ";
            Query += "AND NR.ActionCode = '02' ";

            Cmd = new SqlCommand(Query, Conn);
            int CountDN = Convert.ToInt32(Cmd.ExecuteScalar());
            return CountDN;
        }

        private decimal GetPriceFromInventTable(string FieldName, string FullItemID)
        {
            Query = "SELECT " + FieldName + " FROM InventTable WHERE FullItemID = '" + FullItemID + "'";

            Cmd = new SqlCommand(Query, Conn);
            string result = Convert.ToString(Cmd.ExecuteScalar());
            decimal Price;
            if (result == "")
            {
                Price = 1;
            }
            else if (Convert.ToDecimal(result) == 0)
            {
                Price = 1;
            }
            else
            {
                Price = Convert.ToDecimal(result);
            }
            return Price;
        }

        private decimal GetPriceFromSO(string FullItemID)
        {
            Query = "SELECT SO.Price FROM GoodsReceivedH GRH ";
            Query += "INNER JOIN GoodsReceivedD GRD ON GRH.GoodsReceivedId = GRD.GoodsReceivedId ";
            Query += "INNER JOIN NotaReturJual_Dtl NR ON NR.NRJId = GRD.RefTransID ";
            Query += "INNER JOIN GoodsIssuedD GI ON GI.GoodsIssuedId = NR.GoodsIssuedId ";
            Query += "INNER JOIN DeliveryOrderD DO ON GI.RefTransID = DO.DeliveryOrderId ";
            Query += "INNER JOIN SalesOrderD SO ON SO.SalesOrderNo = DO.SalesOrderId ";
            Query += "WHERE GRH.GoodsReceivedId = '" + tbxGRNum.Text + "' ";
            Query += "AND UPPER(GRH.RefTransType) = 'NOTA RETUR JUAL' ";
            Query += "AND GRD.RefTransSeqNo = NR.SeqNo ";
            Query += "AND NR.GoodsIssued_SeqNo = GI.GoodsIssuedSeqNo ";
            Query += "AND GI.RefTransSeqNo = DO.SeqNo ";
            Query += "AND SO.SeqNo = DO.SalesOrderSeqNo ";
            Query += "AND SO.FullItemID = '" + FullItemID + "' ";

            Cmd = new SqlCommand(Query, Conn);
            string result = Convert.ToString(Cmd.ExecuteScalar());
            decimal Price;
            if (result == "")
            {
                Price = 1;
            }
            else if (Convert.ToDecimal(result) == 0)
            {
                Price = 1;
            }
            else
            {
                Price = Convert.ToDecimal(result);
            }
            return Price;
        }

        private string GetNotes()
        {
            //Get Vendor
            string result = "";
            Query = "SELECT TOP 1 VendId, VendorName FROM GoodsReceivedH WHERE GoodsReceivedId = '" + tbxGRNum.Text + "' ";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                string VendId = Convert.ToString(Dr["VendId"]);
                string VendorName = Convert.ToString(Dr["VendorName"]);

                result = VendId + " - " + VendorName;
            }
            Dr.Close();

            return result;
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            HeaderResizeGR f = new HeaderResizeGR();
            f.GRID = tbxGRNum.Text;
            f.SetMode("New", "");
            f.Show();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ModeBeforeEdit();
                GetDataHeader();
                Parent.RefreshGrid();
                MetroFramework.MetroMessageBox.Show(this, "Printing...!\r\n*Note: Cuman pop up, belum print!\r\n", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            string GRStats = "";
            if (Mode != "New")
            {
                using (SqlConnection Conn2 = ConnectionString.GetConnection())
                {
                    Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
                    using (SqlCommand Cmd = new SqlCommand(Query, Conn2))
                        GRStats = Cmd.ExecuteScalar().ToString();
                }
            }
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView3.Columns["GoodsReceivedSeqNo"].Visible = false;
            dataGridView3.Columns["RefTransID"].Visible = false;
            dataGridView3.Columns["RefTransSeqNo"].Visible = false;
            dataGridView3.Columns["GroupId"].Visible = false;
            dataGridView3.Columns["SubGroup1Id"].Visible = false;
            dataGridView3.Columns["SubGroup2Id"].Visible = false;
            dataGridView3.Columns["ItemId"].Visible = false;
            dataGridView3.Columns["Qty"].Visible = false;
            dataGridView3.Columns["Remaining_Qty"].Visible = false;
            dataGridView3.Columns["Ratio"].Visible = false;
            dataGridView3.Columns["Ratio_Actual"].Visible = false;
            dataGridView3.Columns["InventSiteId"].Visible = false;

            if (Mode == "New" || (GRStats == "01" && ControlMgr.GroupName == "WB OPERATOR"))
            {
                dataGridView3.Columns["Qty_Actual"].Visible = false;
                dataGridView3.Columns["TotalBerat"].Visible = false;
                dataGridView3.Columns["TotalBerat_Actual"].Visible = false;
                dataGridView3.Columns["Quality"].Visible = false;
            }
            else if (ControlMgr.GroupName == "KERANI" && (GRStats == "01" || GRStats == "02"))
            {
                dataGridView3.Columns["Qty_SJ"].Visible = false;
                dataGridView3.Columns["TotalBerat"].Visible = false;
            }

            if (dataGridView3.Columns[e.ColumnIndex].HeaderText.Contains("Qty"))
            {
                if (e.Value == null || e.Value == String.Empty || e.Value.ToString() == ".")
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                dataGridView3.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N2");
            }

            if (dataGridView3.Columns[e.ColumnIndex].HeaderText.Contains("Ratio") || dataGridView3.Columns[e.ColumnIndex].HeaderText.Contains("TotalBerat"))
            {
                if (e.Value == null || e.Value == String.Empty)
                    e.Value = "0";
                double d = double.Parse(e.Value.ToString());
                dataGridView3.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                e.Value = d.ToString("N4");
            }
            for (int i = 0; i < dataGridView3.Rows.Count; i++)
            {
                dataGridView3.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView2.Columns["RefTransID"].Visible = false;
            dataGridView2.Columns["RefTransSeqNo"].Visible = false;
            dataGridView2.Columns["GroupId"].Visible = false;
            dataGridView2.Columns["SubGroup1Id"].Visible = false;
            dataGridView2.Columns["SubGroup2Id"].Visible = false;
            dataGridView2.Columns["ItemId"].Visible = false;

            dataGridView2.Columns[e.ColumnIndex].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView2.Columns[e.ColumnIndex].ReadOnly = true;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            decimal ratio = 0, qty = 0, totalBerat = 0;
            if (tableColsName1[e.ColumnIndex] == "Qty_SJ" || tableColsName1[e.ColumnIndex] == "Ratio")
            {
                if (!(dataGridView3.Rows[e.RowIndex].Cells["Ratio"].Value == null || dataGridView3.Rows[e.RowIndex].Cells["Ratio"].Value == String.Empty))
                {
                    if (dataGridView3.Rows[e.RowIndex].Cells["Qty_SJ"].Value != null)
                    {
                        if (dataGridView3.Rows[e.RowIndex].Cells["Qty_SJ"].Value.ToString() != "." && dataGridView3.Rows[e.RowIndex].Cells["Qty_SJ"].Value.ToString() != "")
                        {
                            qty = Convert.ToDecimal(dataGridView3.Rows[e.RowIndex].Cells["Qty_SJ"].Value);
                        }
                    }
                    ratio = Convert.ToDecimal(dataGridView3.Rows[e.RowIndex].Cells["Ratio"].Value);
                    totalBerat = qty * ratio;
                    dataGridView3.Rows[e.RowIndex].Cells["TotalBerat"].Value = totalBerat.ToString();
                }
            }
            else if (tableColsName1[e.ColumnIndex] == "Qty_Actual" || tableColsName1[e.ColumnIndex] == "Ratio_Actual")
            {
                if (!(dataGridView3.Rows[e.RowIndex].Cells["Ratio_Actual"].Value == null || dataGridView3.Rows[e.RowIndex].Cells["Ratio_Actual"].Value == String.Empty))
                {
                    if (dataGridView3.Rows[e.RowIndex].Cells["Qty_Actual"].Value != null)
                    {
                        if (dataGridView3.Rows[e.RowIndex].Cells["Qty_Actual"].Value.ToString() != "." && dataGridView3.Rows[e.RowIndex].Cells["Qty_Actual"].Value.ToString() != "")
                        {
                            qty = Convert.ToDecimal(dataGridView3.Rows[e.RowIndex].Cells["Qty_Actual"].Value);
                        }
                    }

                    ratio = Convert.ToDecimal(dataGridView3.Rows[e.RowIndex].Cells["Ratio_Actual"].Value);
                    totalBerat = qty * ratio;
                    dataGridView3.Rows[e.RowIndex].Cells["TotalBerat_Actual"].Value = totalBerat.ToString();
                }
            }
        }

        private void dataGridView3_KeyPress(object sender, KeyPressEventArgs e)
        {
            //BY: HC (S)
            if (dataGridView3.Columns[dataGridView3.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView3.Columns[dataGridView3.CurrentCell.ColumnIndex].Name.Contains("Ratio") || dataGridView3.Columns[dataGridView3.CurrentCell.ColumnIndex].Name.Contains("TotalBerat"))
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    e.Handled = true;
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    e.Handled = true;
                if (e.KeyChar != '\b')
                {
                    if ((sender as TextBox).Text.Length >= 15)
                        e.Handled = true;
                }
            }
            if (dataGridView3.Columns[dataGridView3.CurrentCell.ColumnIndex].Name == "Notes")
            {
                if (e.KeyChar != '\b')
                {
                    if ((sender as TextBox).Text.Length >= 255)
                        e.Handled = true;
                }
            }
            //BY: HC (E)
        }

        private void dataGridView3_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(dataGridView3_KeyPress);
            if (dataGridView3.Columns[dataGridView3.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView3.Columns[dataGridView3.CurrentCell.ColumnIndex].Name == "RatioActual")
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                    tb.KeyPress += new KeyPressEventHandler(dataGridView3_KeyPress);
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (txtSiteType.Text.ToUpper() == "VIRTUAL SITE")
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells["Qty_Actual"].Value = dataGridView1.Rows[i].Cells["Qty_SJ"].Value;
                }
            }
        }
        //tia edit
        //kanan
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.FullItemId.FullItemId FID = null;
        Purchase.NotaReturBeli.ReturBeliHeader NRBNumb = null;
        Purchase.ReceiptOrder.HeaderReceiptOrder RONumb = null;
        Sales.NotaReturJual.NRJHeader NRJNumb = null;

        Purchase.NotaPurchaseParked.HeaderNotaPurchaseParked ParentToNPP;
        Purchase.NotaReturBeli.ReturBeliHeader ParentToNRB;
        Purchase.NotaReturBeli.NRBApproval ParentToNRBA;
        TaskList.Purchase.GoodsReceipt.TaskListGR ParentToTaskListGR;
        AccountPayable.HeaderAccountsPayable ParentToAP;

        AccountAssignment.GLJournal.FormGLJournalHeader PArentToGL;

        public void ParentRefreshGrid(Purchase.NotaPurchaseParked.HeaderNotaPurchaseParked npp)
        {
            ParentToNPP = npp;
        }

        public void ParentRefreshGrid2(Purchase.NotaReturBeli.ReturBeliHeader nrb)
        {
            ParentToNRB = nrb;
        }
        public void ParentRefreshGrid3(Purchase.NotaReturBeli.NRBApproval nrba)
        {
            ParentToNRBA = nrba;
        }

        public void ParentRefreshGrid4(TaskList.Purchase.GoodsReceipt.TaskListGR TlGR)
        {
            ParentToTaskListGR = TlGR;
        }
        public void ParentRefreshGrid5(AccountPayable.HeaderAccountsPayable ap)
        {
            ParentToAP = ap;
        }
        public void ParentRefreshGrid6(AccountAssignment.GLJournal.FormGLJournalHeader GL)
        {
            PArentToGL = GL;
        }


        private void tbxNameID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    tbxNameID.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(tbxNameID.Text);
                    Vendor.Show();
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

        private void tbxName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    tbxName.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(tbxNameID.Text);
                    Vendor.Show();
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void tbxRefID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //msh error
                if (tbxRefID.Text.Contains("RO"))
                {
                    if (RONumb == null || RONumb.Text == "")
                    {
                        RONumb = new ISBS_New.Purchase.ReceiptOrder.HeaderReceiptOrder();
                        RONumb.SetMode("BeforeEdit", tbxRefID.Text);
                        //RONumb.ParentRefreshGrid(this);
                        RONumb.Show();
                    }
                    else if (CheckOpened(RONumb.Name))
                    {
                        RONumb.WindowState = FormWindowState.Normal;
                        RONumb.Show();
                        RONumb.Focus();
                    }
                }
                else if (tbxRefID.Text.Contains("NRJ"))
                {
                    if (NRJNumb == null || NRJNumb.Text == "")
                    {
                        NRJNumb = new ISBS_New.Sales.NotaReturJual.NRJHeader();
                        NRJNumb.SetMode("BeforeEdit", tbxRefID.Text);
                        //NRJNumb.ParentRefreshGrid(this);
                        NRJNumb.Show();
                    }
                    else if (CheckOpened(NRJNumb.Name))
                    {
                        NRJNumb.WindowState = FormWindowState.Normal;
                        NRJNumb.Show();
                        NRJNumb.Focus();
                    }
                }
                else if (tbxRefID.Text.Contains("NRB"))
                {
                    if (NRBNumb == null || NRBNumb.Text == "")
                    {
                        NRBNumb = new ISBS_New.Purchase.NotaReturBeli.ReturBeliHeader();
                        NRBNumb.SetMode("BeforeEdit", tbxRefID.Text);
                        //NRJNumb.ParentRefreshGrid(this);
                        NRBNumb.Show();
                    }
                    else if (CheckOpened(NRBNumb.Name))
                    {
                        NRBNumb.WindowState = FormWindowState.Normal;
                        NRBNumb.Show();
                        NRBNumb.Focus();
                    }
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        //itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }

        private void dataGridView3_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        //itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemId" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemId")
                    {
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        //itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }

        private void tbxVOwnerID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    tbxVOwnerID.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(tbxVOwnerID.Text);
                    Vendor.Show();
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }

        private void tbxVOwner_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vendor == null || Vendor.Text == "")
                {
                    tbxVOwner.Enabled = true;
                    Vendor = new PopUp.Vendor.Vendor();
                    Vendor.GetData(tbxVOwnerID.Text);
                    Vendor.Show();
                }
                else if (CheckOpened(Vendor.Name))
                {
                    Vendor.WindowState = FormWindowState.Normal;
                    Vendor.Show();
                    Vendor.Focus();
                }
            }
        }
        //end

        private void cmbReferenceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbxRefID.Text = "";
            dataGridView1.Columns.Clear();
            dataGridView2.Columns.Clear();
            dataGridView3.Columns.Clear();
            txtSiteType.Text = "";
            txtWarehouse.Text = "";
            txtInventSiteID.Text = "";
            tbxVNumber.Text = "";
            tbxVOwner.Text = "";
            tbxVType.Text = "";
            tbxName.Text = "";
            tbxNameID.Text = "";
            tbxDelivNum.Text = "";
            tbxDriverName.Text = "";
        }

        private string checkWeightTollerance(SqlConnection Conn)
        {
            string GRStats = "";
            Query = "select GoodsReceivedStatus from GoodsReceivedH where GoodsReceivedId = '" + tbxGRNum.Text + "'";
            Cmd = new SqlCommand(Query, Conn);
            GRStats = Cmd.ExecuteScalar().ToString();

            string weightStat = "";
            if ((GRStats == "02" || GRStats == "03") && ControlMgr.GroupName == "WB OPERATOR")
            {
                decimal GRatioActual = 0;
                decimal TotalBerat_Actual = 0;
                decimal RejectedWeight = 0;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    TotalBerat_Actual = dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value.ToString());
                    GRatioActual += TotalBerat_Actual;
                    //edited by Thaddaeus, 3JULY2018, rejected item gonna be in the weigh2
                    if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "REJECT")
                    {
                        RejectedWeight += TotalBerat_Actual;
                    }
                }
                for (int i = 0; i < dataGridView3.RowCount; i++)
                {
                    TotalBerat_Actual = dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString() == "" ? 0 : Convert.ToDecimal(dataGridView3.Rows[i].Cells["TotalBerat_Actual"].Value.ToString());
                    GRatioActual += TotalBerat_Actual;
                    if (dataGridView3.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "REJECT")
                    {
                        RejectedWeight += TotalBerat_Actual;
                    }
                }
                decimal weightTolerance = ((Convert.ToDecimal(tbxWeight1.Text) - GRatioActual) - (Convert.ToDecimal(tbxWeight2.Text) - RejectedWeight)) / (Convert.ToDecimal(tbxWeight1.Text) - GRatioActual) * 100;
                if (Math.Abs(weightTolerance) > 1)
                {
                    weightStat = "05";
                }
                else
                {
                    weightStat = "03";
                }
            }
            else if ((GRStats == "01" || GRStats == "02") && ControlMgr.GroupName == "KERANI")
            {
                weightStat = "02";
            }
            else if ((GRStats == "05") && (ControlMgr.GroupName == "SITE MANAGER"))
            {
                weightStat = "06";
            }
            return weightStat;
        }

        List<int> ParentParkedRow = new List<int>();
        List<int> ChildParkedRow = new List<int>();
        private void CheckAndInsertParkTollerance(SqlConnection Conn)
        {
            if (!(cmbReferenceType.Text == "Nota Retur Jual" && ControlMgr.GroupName == "KERANI"))
            {
                return;
            }
            ParentParkedRow.Clear();
            ChildParkedRow.Clear();
            //Delete previous item with parked tolerance action status
            List<int> DeletedRow = new List<int>();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() == "Parked Tolerance - Need Action")
                {
                    //get price
                    decimal price = 0;
                    #region getprice
                    Query = "select b.RefTransID from NotaReturJual_Dtl a left join [GoodsIssuedD] b on a.[GoodsIssuedId] = b.[GoodsIssuedId] and a.[GoodsIssued_SeqNo] = b.[GoodsIssuedSeqNo] where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if (Dr["RefTransID"] == System.DBNull.Value)
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            {
                                Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "DO" || Dr["RefTransID"].ToString().Split('/')[0] == "DOA")
                        {
                            Query = "select d.Price, d.DiscPercent, e.PPN, e.PPH from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                            }
                            Dr2.Close();
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                        {
                            Query = "select COGS from NotaTransferD where TransferNo = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                        else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRB")
                        {
                            Query = "select d.Price, d.Diskon, e.PPN, e.PPH from NotaReturBeli_Dtl a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.NRBId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            SqlDataReader Dr2 = Cmd2.ExecuteReader();
                            while (Dr2.Read())
                            {
                                price = Convert.ToDecimal(Dr2["Price"]);
                            }
                            Dr2.Close();
                        }
                        else
                        {
                            Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                            SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                            {
                                Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                                price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                            }
                        }
                    }
                    #endregion

                    //delete qty in invent movement
                    Query = "UPDATE [Invent_Movement_Qty] SET [Parked_For_Action_Outstanding_UoM] -= @Parked_For_Action_Outstanding_UoM, ";
                    Query += " [Parked_For_Action_Outstanding_Alt] -= @Parked_For_Action_Outstanding_Alt, ";
                    Query += " [Parked_For_Action_Outstanding_Amount] -= @Parked_For_Action_Outstanding_Amount ";
                    Query += " WHERE [FullItemID] = @FullItemID;";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        decimal QtyAlt = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value);
                        decimal QtyAmount = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * price;
                        Cmd.Parameters.AddWithValue("@Parked_For_Action_Outstanding_UoM", dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                        Cmd.Parameters.AddWithValue("@Parked_For_Action_Outstanding_Alt", QtyAlt);
                        Cmd.Parameters.AddWithValue("@Parked_For_Action_Outstanding_Amount", QtyAmount);
                        Cmd.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                        Cmd.ExecuteNonQuery();
                    }
                    //delete GR Detail parked tolerance
                    Query = "DELETE FROM [dbo].[GoodsReceivedD] WHERE [GoodsReceivedId]=@GoodsReceivedId AND [GoodsReceivedSeqNo] = @GoodsReceivedSeqNo;";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.Parameters.AddWithValue("@GoodsReceivedId", tbxGRNum.Text);
                        Cmd.Parameters.AddWithValue("@GoodsReceivedSeqNo", dataGridView1.Rows[i].Cells["GoodsReceivedSeqNo"].Value);
                        Cmd.ExecuteNonQuery();
                    }
                    //delete from grid
                    DeletedRow.Add(i);
                }
            }
            for (int i = 0; i < DeletedRow.Count; i++)
            {
                dataGridView1.Rows.RemoveAt(DeletedRow[i]);
            }

            //add item to grid
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString().ToUpper() == "RECEIVED")
                {
                    if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value) != Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value))
                    {
                        ParentParkedRow.Add(i);
                        ChildParkedRow.Add((dataGridView1.Rows.Count));
                        //header name => "No", "GoodsReceivedSeqNo", "RefTransID", "RefTransSeqNo", "GroupId", "SubGroup1Id", "SubGroup2Id", "ItemId", "FullItemId", "ItemName", "Qty", "Qty_SJ", "Qty_Actual", "Remaining_Qty", "Unit", "Ratio", "Ratio_Actual", "TotalBerat", "TotalBerat_Actual", "InventSiteId", "InventSiteBlokID", "ActionCodeStatus", "Quality", "Notes" };
                        string[] rowValue = new string[tableColsName1.Length];
                        for (int j = 0; j < tableColsName1.Length; j++)
                        {
                            if (tableColsName1[j] == "No")
                            {
                                rowValue[j] = dataGridView1.Rows.Count.ToString();
                            }
                            else if (tableColsName1[j] == "Qty_SJ" || tableColsName1[j] == "TotalBerat" || tableColsName1[j] == "GoodsReceivedSeqNo")
                            {
                                rowValue[j] = "0";
                            }
                            else if (tableColsName1[j] == "Qty_Actual")
                            {
                                rowValue[j] = (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value)).ToString();
                            }
                            else if (tableColsName1[j] == "Remaining_Qty")
                            {
                                rowValue[j] = (Math.Abs(Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value))).ToString();
                            }
                            else if (tableColsName1[j] == "TotalBerat_Actual")
                            {
                                rowValue[j] = (Math.Abs(Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) - Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value)) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value)).ToString();
                            }
                            else if (tableColsName1[j] == "ActionCodeStatus")
                            {
                                rowValue[j] = "Parked Tolerance - Need Action";
                            }
                            else
                            {
                                rowValue[j] = dataGridView1.Rows[i].Cells[tableColsName1[j]].Value.ToString();
                            }
                        }
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) > Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_SJ"].Value))
                        {
                            dataGridView1.Rows[i].Cells["Qty_Actual"].Value = dataGridView1.Rows[i].Cells["Qty_SJ"].Value;
                        }
                        dataGridView1.Rows[i].Cells["TotalBerat_Actual"].Value = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["Ratio_Actual"].Value);

                        dataGridView1.Rows.Add(rowValue);
                    }
                }
            }
        }

        private void InsertNRJParked()
        {
            bool NRJPExist = false;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                //if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() != "Parked Tolerance - Need Action")
                if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() != "Parked - Need Action")
                {
                    NRJPExist = true;
                    break;
                }
            }
            if (NRJPExist == false)
            {
                return;
            }
            //generate ID
            string Jenis = "NRJP";
            string Kode = "NRJP";
            string NRJP = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

            //Insert NRJP Header
            Query = "INSERT INTO [NotaReturJualParkedH] ([NRJPDate],[NRJPID],[NRJID],[CustId],[StatusCode],[CreatedBy],[CreatedDate]) ";
            Query += " VALUES (@NRJPDate,@NRJPID,@NRJID,@CustId,@StatusCode,@CreatedBy,getdate())";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@NRJPDate", dtRef.Value);
                Cmd.Parameters.AddWithValue("@NRJPID", NRJP);
                Cmd.Parameters.AddWithValue("@NRJID", tbxRefID.Text);
                Cmd.Parameters.AddWithValue("@CustId", tbxNameID.Text);
                Cmd.Parameters.AddWithValue("@StatusCode", "00");
                Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }

            //Insert NRJP Detail
            int x = 1;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                //if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() != "Parked Tolerance - Need Action")
                if (dataGridView1.Rows[i].Cells["ActionCodeStatus"].Value.ToString() != "Parked - Need Action")
                {
                    continue;
                }

                //getprice
                decimal price = 0;
                #region getprice
                Query = "select b.RefTransID from NotaReturJual_Dtl a left join [GoodsIssuedD] b on a.[GoodsIssuedId] = b.[GoodsIssuedId] and a.[GoodsIssued_SeqNo] = b.[GoodsIssuedSeqNo] where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    if (Dr["RefTransID"] == System.DBNull.Value)
                    {
                        Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                        SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                        {
                            Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                            price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                    }
                    else if (Dr["RefTransID"].ToString().Split('/')[0] == "DO" || Dr["RefTransID"].ToString().Split('/')[0] == "DOA")
                    {
                        Query = "select d.Price, d.DiscPercent, e.PPN, e.PPH from NotaReturJual_Dtl a left join GoodsIssuedD b on a.GoodsIssuedId = b.GoodsIssuedId and a.GoodsIssued_SeqNo = b.GoodsIssuedSeqNo left join DeliveryOrderD c on b.RefTransID = c.DeliveryOrderId and b.RefTransSeqNo = c.SeqNo left join SalesOrderD d on c.SalesOrderId = d.SalesOrderNo and c.SalesOrderSeqNo = d.SeqNo left join SalesOrderH e on d.SalesOrderNo = e.SalesOrderNo where a.NRJId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                        SqlDataReader Dr2 = Cmd2.ExecuteReader();
                        while (Dr2.Read())
                        {
                            price = Convert.ToDecimal(Dr2["Price"]);
                        }
                        Dr2.Close();
                    }
                    else if (Dr["RefTransID"].ToString().Split('/')[0] == "NT")
                    {
                        Query = "select COGS from NotaTransferD where TransferNo = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                        price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                    }
                    else if (Dr["RefTransID"].ToString().Split('/')[0] == "NRB")
                    {
                        Query = "select d.Price, d.Diskon, e.PPN, e.PPH from NotaReturBeli_Dtl a left join [GoodsReceivedD] b on a.[GoodsReceivedId] = b.[GoodsReceivedId] and a.[GoodsReceived_SeqNo] = b.[GoodsReceivedSeqNo] left join [ReceiptOrderD] c on b.[RefTransID] = c.[ReceiptOrderId] and b.[RefTransSeqNo] = c.SeqNo left join [PurchDtl] d on c.[PurchaseOrderId] = d.[PurchID] and c.[PurchaseOrderSeqNo] = d.SeqNo left join [PurchH] e on d.[PurchID] = e.[PurchID] where a.NRBId = '" + dataGridView1.Rows[i].Cells["RefTransID"].Value + "' and a.SeqNo = " + dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value;
                        SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                        SqlDataReader Dr2 = Cmd2.ExecuteReader();
                        while (Dr2.Read())
                        {
                            price = Convert.ToDecimal(Dr2["Price"]);
                        }
                        Dr2.Close();
                    }
                    else
                    {
                        Query = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = @FullItemID";
                        SqlCommand Cmd2 = new SqlCommand(Query, Conn);
                        {
                            Cmd2.Parameters.AddWithValue("@FullItemID", dataGridView1.Rows[i].Cells["FullItemID"].Value);
                            price = Convert.ToDecimal(Cmd2.ExecuteScalar());
                        }
                    }
                }
                #endregion

                Query = "INSERT INTO [NotaReturJualParkedD] ([NRJPDate],[NRJPID],[SeqNo],[FullItemId],[ItemName],[Qty],[Unit],[Price],[Amount],[NRJID],[NRJ_SeqNo],[ActionCode],[CreatedBy],[CreatedDate]) ";
                Query += " VALUES (@NRJPDate,@NRJPID,@SeqNo,@FullItemId,@ItemName,@Qty,@Unit,@Price,@Amount,@NRJID,@NRJ_SeqNo,@ActionCode,@CreatedBy,getdate())";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@NRJPDate", dtRef.Value);
                    Cmd.Parameters.AddWithValue("@NRJPID", NRJP);
                    Cmd.Parameters.AddWithValue("@SeqNo", x);
                    Cmd.Parameters.AddWithValue("@FullItemId", dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@ItemName", dataGridView1.Rows[i].Cells["ItemName"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Qty", dataGridView1.Rows[i].Cells["Qty_Actual"].Value);
                    Cmd.Parameters.AddWithValue("@Unit", dataGridView1.Rows[i].Cells["Unit"].Value.ToString());
                    Cmd.Parameters.AddWithValue("@Price", price);
                    Cmd.Parameters.AddWithValue("@Amount", (Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty_Actual"].Value) * price));
                    Cmd.Parameters.AddWithValue("@NRJID", dataGridView1.Rows[i].Cells["RefTransID"].Value);
                    Cmd.Parameters.AddWithValue("@NRJ_SeqNo", dataGridView1.Rows[i].Cells["RefTransSeqNo"].Value);
                    Cmd.Parameters.AddWithValue("@ActionCode", "00");
                    Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                    Cmd.ExecuteNonQuery();
                }
                x++;
            }
        }



    }
}
