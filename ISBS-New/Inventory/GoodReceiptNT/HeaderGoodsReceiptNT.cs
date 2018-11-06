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
    public partial class HeaderGoodsReceiptNT : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query, crit = null;
        public string GRNumber = "";
        private string id;
        private int count;
        private char flag;
        Label[] label;
        bool check;
        DataGridViewComboBoxCell cell;
        SqlDataReader Dr2;

        InquiryGoodsReceiptNT Parent = new InquiryGoodsReceiptNT();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end


        public void SetParent(InquiryGoodsReceiptNT F)
        {
            Parent = F;
        }

        public HeaderGoodsReceiptNT()
        {
            InitializeComponent();
        }

        #region SetMode
        public void SetMode(string tmpMode, string tmpGRNumber)
        {
            Mode = tmpMode;
            GRNumber = tmpGRNumber;
            tbxGRNum.Text = tmpGRNumber;
        }
        #endregion


        #region GenerateID
        private string GenerateID()
        {
            id = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select count(*) from [dbo].[GoodsReceivedH]", Conn);
            count = (Int32)Cmd.ExecuteScalar();
            if (count == 0)
                count++;
            else
            {
                Cmd = new SqlCommand("SELECT TOP 1 [GoodsReceivedId] FROM [dbo].[GoodsReceivedH] order by [CreatedDate] desc", Conn);
                string[] lastID = Cmd.ExecuteScalar().ToString().Split('-');
                if (lastID[1] != DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM"))
                    count = 1;
                else
                    count = Convert.ToInt32(lastID[2]) + 1;
            }
            if (count.ToString().Length == 1)
                id += "0000" + count;
            else if (count.ToString().Length == 2)
                id += "000" + count;
            else if (count.ToString().Length == 3)
                id += "00" + count;
            else if (count.ToString().Length == 4)
                id += "0" + count;
            else if (count.ToString().Length == 5)
                id += "" + count;
            Conn.Close();
            count = 0;
            return "GR-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-" + id;
        }
        #endregion

        private void HeaderGoodsReceiptNT_Load(object sender, EventArgs e)
        {
            try
            {
                if (Mode != "New")
                    GetDataHeader();

                if (Mode == "New")
                {
                    ModeNew();
                    tbxGRNum.Text = GenerateID();
                    btnNew.Enabled = false;
                    btnDelete.Enabled = false;
                }
                else if (Mode == "Edit")
                    ModeEdit();
                else if (Mode == "BeforeEdit")
                    ModeBeforeEdit();
                else if (Mode == "ModeView")
                    ModeView();
                rbTicket.Checked = true;
                //Purchase.GoodsReceipt.InquiryGoodsReceiptNT f = new Purchase.GoodsReceipt.InquiryGoodsReceiptNT();
                //f.RefreshGrid();
                Parent.RefreshGrid();

                if (Mode != "New")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    if (Cmd.ExecuteScalar().ToString() == "01")
                        gbWeight2.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "System Failure: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #region ModeNew
        public void ModeNew()
        {
            GRNumber = "";
            dtGR.Value = DateTime.Now;
            dtWeight2.Value = DateTime.Now;
            btnEdit.Enabled = false; btnCancel.Enabled = false; btnPrint.Enabled = false;
            rbTicket.Enabled = false; rbTT.Enabled = false;
            tbxRONum.Enabled = false;
            gbWeight2.Visible = false;
            dtWeight1.Value = DateTime.Now;
            dtDO.Value = DateTime.Now;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        #endregion

        #region ModeEdit
        public void ModeEdit()
        {
            Mode = "Edit";

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "01")
            {
                if (Login.UserGroup == "SalesManager") //WB
                {
                    //MAIN
                    dtDO.Enabled = true; tbxDelivNum.Enabled = true;
                    btnSRO.Enabled = false; btnSOwner.Enabled = true;
                    dtExpectedDate.Enabled = true;
                    tbxVType.Enabled = true;
                    tbxVNumber.Enabled = true; tbxDriverName.Enabled = true; tbxNotes.ReadOnly = false;
                    tbxNotes.ScrollBars = ScrollBars.Vertical;

                    //BODY
                    btnNew.Enabled = true; btnDelete.Enabled = true;

                    dataGridView1.ReadOnly = false;
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if (dataGridView1.Columns[i].Name == "Qty" || dataGridView1.Columns[i].Name == "Blok" || dataGridView1.Columns[i].Name == "Deskripsi" || dataGridView1.Columns[i].Name == "Notes")
                        {
                            dataGridView1.Columns[i].ReadOnly = false;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        }
                        else
                        {
                            dataGridView1.Columns[i].ReadOnly = true;
                        }

                        //HIDE GV
                        if (dataGridView1.Columns[i].Name == "QtyActual" || dataGridView1.Columns[i].Name == "RatioActual" || dataGridView1.Columns[i].Name == "Quality")
                        {
                            dataGridView1.Columns[i].Visible = false;
                        }
                    }

                    //FOOTER
                    cbWeight1.Enabled = true; btnWeight1.Enabled = true;
                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbTT.Enabled = false;
                    btnEdit.Enabled = false; btnSave.Enabled = true; btnCancel.Enabled = true;
                }
                else if (Login.UserGroup == "PurchaseManager") //KERANI
                {
                    //MAIN
                    tbxVNumber.Enabled = false; tbxVType.Enabled = false; tbxDriverName.Enabled = false;
                    dtDO.Enabled = false; tbxDelivNum.Enabled = false; btnSOwner.Enabled = false;
                    tbxNotes.ReadOnly = true; tbxNotes.ScrollBars = ScrollBars.Vertical;
                    dtExpectedDate.Enabled = true;
                    dataGridView1.ReadOnly = false;
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if (dataGridView1.Columns[i].Name == "QtyActual" || dataGridView1.Columns[i].Name == "RatioActual" || dataGridView1.Columns[i].Name == "Quality")
                        {
                            dataGridView1.Columns[i].ReadOnly = false;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        }
                        else
                        {
                            dataGridView1.Columns[i].ReadOnly = true;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }

                    //GROUP GV
                    btnNew.Enabled = true; btnDelete.Enabled = true;

                    //FOOTER
                    btnWeight1.Enabled = false; cbWeight1.Enabled = false;

                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbTT.Enabled = false;
                    btnSave.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = true;
                }
            }
            else if (Cmd.ExecuteScalar().ToString() == "02")
            {
                if (Login.UserGroup == "SalesManager") //WB
                {
                    cbWeight2.Enabled = true; btnWeight2.Enabled = true;
                    //FOOTER
                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbTT.Enabled = false;
                    btnSave.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = true;
                }
                else if (Login.UserGroup == "PurchaseManager") //kerani
                {
                    //MAIN
                    tbxVNumber.Enabled = false; tbxVType.Enabled = false; tbxDriverName.Enabled = false;
                    dtDO.Enabled = false; tbxDelivNum.Enabled = false; btnSOwner.Enabled = false;
                    tbxNotes.ReadOnly = true; tbxNotes.ScrollBars = ScrollBars.Vertical;

                    dataGridView1.ReadOnly = false;
                    //int index = 0;
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if (dataGridView1.Columns[i].Name == "QtyActual" || dataGridView1.Columns[i].Name == "RatioActual" || dataGridView1.Columns[i].Name == "Quality")
                        {
                            dataGridView1.Columns[i].ReadOnly = false;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.White;
                            //if (dataGridView1.Columns[i].Name == "Quality")
                            //{
                            //    cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                            //    if (dataGridView1.Rows[index].Cells["Quality"].Value.ToString() != null)
                            //        cell.Value = dataGridView1.Rows[index].Cells["Quality"].Value;
                            //    dataGridView1.Rows[index].Cells["Quality"] = cell;
                            //    index++;
                            //}
                        }
                        else
                        {
                            dataGridView1.Columns[i].ReadOnly = true;
                            dataGridView1.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }

                    //GROUP GV
                    btnNew.Enabled = true; btnDelete.Enabled = true;

                    //FOOTER
                    btnWeight1.Enabled = false; cbWeight1.Enabled = false;

                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbTT.Enabled = false;
                    btnSave.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = true;
                }
            }
            else if (Cmd.ExecuteScalar().ToString() == "03")
            {

            }
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            ////MAIN GROUP
            //btnWeight1.Enabled = true;
            //tbxVNumber.Enabled = false; tbxVType.Enabled = false; tbxDriverName.Enabled = false;
            //dtDO.Enabled = false; tbxDelivNum.Enabled = false; btnSOwner.Enabled = false;
            //tbxNotes.ReadOnly = true; tbxNotes.ScrollBars = ScrollBars.Vertical;
            //cbWeight1.Enabled = true;
            //btnSave.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = true;
            //btnPrint.Enabled = false;
            //rbTicket.Enabled = false; rbTT.Enabled = false;

            ////GROUP GV
            //btnNew.Enabled = true; btnDelete.Enabled = true;

            //dataGridView1.ReadOnly = false;
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //dataGridView1.Columns["No"].ReadOnly = true;
            //dataGridView1.Columns["FullItemID"].ReadOnly = true;
            //dataGridView1.Columns["ItemName"].ReadOnly = true;
            //dataGridView1.Columns["Unit"].ReadOnly = true;
            ////if (Login.UserGroup == "PurchaseManager") //ORANG WB
            //    dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.White;
            ////if (Login.UserGroup == "PurchaseManager")
            ////{
            //    dataGridView1.Columns["QtyActual"].DefaultCellStyle.BackColor = Color.White;
            //    dataGridView1.Columns["RatioActual"].DefaultCellStyle.BackColor = Color.White;
            //    dataGridView1.Columns["Quality"].DefaultCellStyle.BackColor = Color.White;
            ////}
            ////else if (Login.UserGroup == "SalesManager")
            ////{
            ////}

            //Conn = ConnectionString.GetConnection();
            //Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            //if (Cmd.ExecuteScalar().ToString() != "01")
            //{
            //    cbWeight2.Enabled = true;
            //    btnWeight2.Enabled = true;
            //}
            //if (Cmd.ExecuteScalar().ToString() == "03")
            //{
            //    tbxVNumber.Enabled = true; tbxVType.Enabled = true; tbxDriverName.Enabled = true;
            //    dtDO.Enabled = true; tbxDelivNum.Enabled = true; tbxNotes.ReadOnly = false;
            //}

            //if (Cmd.ExecuteScalar().ToString() == "01" || Cmd.ExecuteScalar().ToString() == "02")
            //{
            //    cbWeight1.Enabled = false;
            //    btnWeight1.Enabled = false;
            //    dataGridView1.Columns["Qty"].ReadOnly = true;
            //    dataGridView1.Columns["Blok"].ReadOnly = true;
            //    dataGridView1.Columns["Notes"].ReadOnly = true;
            //    dataGridView1.Columns["Deskripsi"].ReadOnly = true;
            //    dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.LightGray;
            //    dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.LightGray;
            //    dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;
            //    dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.LightGray;
            //}
            //if (Cmd.ExecuteScalar().ToString() == "02")
            //{
            //    btnNew.Enabled = false; btnDelete.Enabled = false;
            //    dataGridView1.Columns["QtyActual"].ReadOnly = true;
            //    dataGridView1.Columns["RatioActual"].ReadOnly = true;
            //    dataGridView1.Columns["Quality"].ReadOnly = true;
            //    dataGridView1.Columns["QtyActual"].DefaultCellStyle.BackColor = Color.LightGray;
            //    dataGridView1.Columns["RatioActual"].DefaultCellStyle.BackColor = Color.LightGray;
            //    dataGridView1.Columns["Quality"].DefaultCellStyle.BackColor = Color.LightGray;
            //}
        }
        #endregion

        public void ModeEditNewlySaved()
        {
            Mode = "Edit";
            btnWeight1.Enabled = true;
            tbxVNumber.Enabled = false; tbxVType.Enabled = false; tbxDriverName.Enabled = false;
            dtDO.Enabled = false; tbxDelivNum.Enabled = false; btnSOwner.Enabled = false;
            tbxNotes.ReadOnly = true; tbxNotes.ScrollBars = ScrollBars.Vertical;
            cbWeight1.Enabled = true;
            dtExpectedDate.Enabled = false;
            btnSave.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = true;
            btnPrint.Enabled = false;
            rbTicket.Enabled = false; rbTT.Enabled = false;
            btnNew.Enabled = true; btnDelete.Enabled = true;

            dataGridView1.ReadOnly = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.Columns["No"].ReadOnly = true;
            dataGridView1.Columns["FullItemID"].ReadOnly = true;
            dataGridView1.Columns["ItemName"].ReadOnly = true;
            dataGridView1.Columns["Unit"].ReadOnly = true;
            //dataGridView1.Columns["RemainingQty"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.White;

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() != "01")
            {
                cbWeight2.Enabled = true;
                btnWeight2.Enabled = true;
            }
            if (Cmd.ExecuteScalar().ToString() == "03")
            {
                tbxVNumber.Enabled = true; tbxVType.Enabled = true; tbxDriverName.Enabled = true;
                dtDO.Enabled = true; tbxDelivNum.Enabled = true; tbxNotes.ReadOnly = false;
            }

            if (Cmd.ExecuteScalar().ToString() == "01" || Cmd.ExecuteScalar().ToString() == "02")
            {
                cbWeight1.Enabled = false;
                btnWeight1.Enabled = false;
                dataGridView1.Columns["RemainingQty"].ReadOnly = true;
                dataGridView1.Columns["Blok"].ReadOnly = true;
                dataGridView1.Columns["Notes"].ReadOnly = true;
                dataGridView1.Columns["Deskripsi"].ReadOnly = true;
                //dataGridView1.Columns["RemainingQty"].DefaultCellStyle.BackColor = Color.LightGray;
                //dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.LightGray;
                //dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;
                //dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.LightGray;
            }
            if (Cmd.ExecuteScalar().ToString() == "02")
            {
                btnNew.Enabled = false; btnDelete.Enabled = false;
            }
        }

        #region ModeBeforeEdit
        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";
            btnSave.Enabled = false; btnCancel.Enabled = false;
            btnEdit.Enabled = true; btnPrint.Enabled = true;
            rbTicket.Enabled = true; rbTT.Enabled = true;
            btnNew.Enabled = false; btnDelete.Enabled = false;
            dtExpectedDate.Enabled = false;
            btnResize.Enabled = false;

            tbxVNumber.Enabled = false; tbxVOwner.Enabled = false; tbxVType.Enabled = false;
            tbxDriverName.Enabled = false; tbxDelivNum.Enabled = false;
            tbxNotes.ReadOnly = true; tbxNotes.ScrollBars = ScrollBars.Vertical;
            tbxRONum.Enabled = false; dtDO.Enabled = false;
            btnSRO.Enabled = false; btnSOwner.Enabled = false;
            //WEIGHT 1
            btnWeight1.Enabled = false; dtWeight1.Enabled = false; tbxWeight1.Enabled = false;
            cbWeight1.Checked = false; tbxTicketNum1.Enabled = false; cbWeight1.Enabled = false;
            //WEIGHT 2
            btnWeight2.Enabled = false; dtWeight2.Enabled = false; tbxWeight2.Enabled = false;
            cbWeight2.Checked = false; tbxTicketNum2.Enabled = false; cbWeight2.Enabled = false;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["QtyActual"].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns["RatioActual"].DefaultCellStyle.BackColor = Color.LightGray; // RATIO ACTUAL
            dataGridView1.Columns["Quality"].DefaultCellStyle.BackColor = Color.LightGray;

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "01")
            {
                if (Login.UserGroup == "SalesManager") //WB
                {
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        //HIDE GV
                        if (dataGridView1.Columns[i].Name == "QtyActual" || dataGridView1.Columns[i].Name == "RatioActual" || dataGridView1.Columns[i].Name == "Quality")
                        {
                            dataGridView1.Columns[i].Visible = false;
                        }
                    }
                }
                else if (Login.UserGroup == "PurchaseManager") //Kerani
                {
                }
            }
            else if (Cmd.ExecuteScalar().ToString() == "02")
            {
                if (Login.UserGroup == "SalesManager") //WB
                {
                    gbWeight2.Visible = true;
                }
                else if (Login.UserGroup == "PurchaseManager") //Kerani
                {
                    gbWeight2.Visible = false;
                }
            }
            else if (Cmd.ExecuteScalar().ToString() == "03")
            {
                btnEdit.Enabled = false;
                btnResize.Enabled = true;
            }
        }
        #endregion

        #region ModeView
        public void ModeView()
        {
            Mode = "ModeView";

            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;
        }
        #endregion

        #region cellValue
        private DataGridViewComboBoxCell cellValue(string query)
        {
            cell = null;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(query, Conn);
            Dr2 = Cmd.ExecuteReader();
            cell = new DataGridViewComboBoxCell();
            while (Dr2.Read())
                cell.Items.Add(Dr2[0].ToString());
            return cell;
        }
        #endregion

        #region AddDataRowToGV
        private void AddDataRowToGV(string stats)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select * from [GoodsReceivedD] as a left join [TransStatusTable] as b on a.ActionCodeStatus = b.StatusCode Where [GoodsReceivedId] = '" + tbxGRNum.Text + "' and b.TransCode = 'GRD'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemName"], Dr["Qty_SJ"], Dr["Unit"], Dr["Qty_Actual"], Dr["Ratio_Actual"], Dr["Quality"], Dr["InventSiteBlokID"], Dr["Deskripsi"], Dr["Notes"]);
                if (stats == "01")
                {
                    if (Login.UserGroup == "PurchaseManager")
                    {
                        cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                        if (Dr["Quality"] != null)
                            cell.Value = Dr["Quality"].ToString();
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Quality"] = cell;
                    }
                    else if (Login.UserGroup == "SalesManager")
                    {
                        cellValue("select distinct c.InventSiteBlokID from [GoodsReceivedD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.GoodsReceivedId = '" + tbxGRNum.Text + "'; ");
                        if (Dr["InventSiteBlokID"] != null)
                            cell.Value = Dr["InventSiteBlokID"].ToString();
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"] = cell;

                        cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD'");
                        if (Dr["Deskripsi"] != null)
                            cell.Value = Dr["Deskripsi"].ToString();
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;
                    }
                }
                else if (stats == "02")
                {
                    if (Login.UserGroup == "PurchaseManager")
                    {
                        cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                        if (Dr["Quality"] != null)
                            cell.Value = Dr["Quality"].ToString();
                        dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Quality"] = cell;
                    }
                }
            }
            Dr.Close();
        }
        #endregion

        #region GetDataHeader
        public void GetDataHeader()
        {
            if (GRNumber == "")
                GRNumber = tbxGRNum.Text.Trim();
            Conn = ConnectionString.GetConnection();

            Query = "Select * From [dbo].[GoodsReceivedH] a ";
            Query += "Where [GoodsReceivedId] = '" + GRNumber + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtGR.Value = Convert.ToDateTime(Dr["GoodsReceivedDate"]);
                dtRO.Value = Convert.ToDateTime(Dr["ReceiptOrderDate"]);
                dtDO.Value = Convert.ToDateTime(Dr["SJDate"]);
                tbxRONum.Text = Dr["ReceiptOrderId"].ToString();
                tbxDelivNum.Text = Dr["SJNumber"].ToString();
                tbxVNumber.Text = Dr["VehicleNumber"].ToString();
                tbxVType.Text = Dr["VehicleType"].ToString();
                tbxDriverName.Text = Dr["DriverName"].ToString();
                dtWeight1.Value = Convert.ToDateTime(Dr["Timbang1Date"]);
                tbxTicketNum1.Text = Dr["Timbang1Id"].ToString();
                tbxWeight1.Text = Dr["Timbang1Weight"].ToString();
                if (Dr["Timbang2Date"].ToString() != "")
                    dtWeight2.Value = Convert.ToDateTime(Dr["Timbang2Date"]);
                tbxTicketNum2.Text = Dr["Timbang2Id"].ToString();
                tbxWeight2.Text = Dr["Timbang2Weight"].ToString();
                txtInventSiteID.Text = Dr["SiteID"].ToString();
                txtWarehouse.Text = Dr["SiteName"].ToString();
                txtLocation.Text = Dr["SiteLocation"].ToString();
                tbxNotes.Text = Dr["Notes"].ToString();
                tbxVOwnerID.Text = Dr["VendId"].ToString();
                tbxVOwner.Text = Dr["VendorName"].ToString();
            }
            Dr.Close();

            dataGridView1.Rows.Clear();
            dataGridView1.AllowUserToAddRows = false;
            if (dataGridView1.RowCount - 1 <= 0)
            {
                dataGridView1.ColumnCount = 11;
                dataGridView1.Columns[0].Name = "No";
                dataGridView1.Columns[1].Name = "FullItemID";
                dataGridView1.Columns[2].Name = "ItemName";
                dataGridView1.Columns[3].Name = "Qty"; dataGridView1.Columns[3].HeaderText = "Delivery Qty";
                dataGridView1.Columns[4].Name = "Unit";
                dataGridView1.Columns[5].Name = "QtyActual";
                dataGridView1.Columns[6].Name = "RatioActual";
                dataGridView1.Columns[7].Name = "Quality";
                dataGridView1.Columns[8].Name = "Blok";
                dataGridView1.Columns[9].Name = "Deskripsi"; dataGridView1.Columns[9].HeaderText = "Action Plan";
                dataGridView1.Columns[10].Name = "Notes";
            }

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "01")
            {
                AddDataRowToGV(Cmd.ExecuteScalar().ToString());
            }
            else if (Cmd.ExecuteScalar().ToString() == "02")
            {
                AddDataRowToGV(Cmd.ExecuteScalar().ToString());
            }
            else if (Cmd.ExecuteScalar().ToString() == "03")
            {
                AddDataRowToGV(Cmd.ExecuteScalar().ToString());
            }
        }
        #endregion

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.Edit) > 0)
            {
                if (Mode == "New")
                {
                    ModeEditNewlySaved();
                }
                else
                {
                    Mode = "Edit";
                    ModeEdit();
                    GetDataHeader();
                }
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end             
        }

        private void cbWeight1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWeight1.Checked == true)
            {
                tbxWeight1.Enabled = true;
                tbxTicketNum1.Enabled = true;
                btnWeight1.Enabled = false;
            }
            else
            {
                tbxWeight1.Enabled = false;
                tbxTicketNum1.Enabled = false;
                btnWeight1.Enabled = true;
                tbxWeight1.Clear();
                tbxTicketNum1.Clear();
                if (Mode != "New")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [Timbang1Id],[Timbang1Weight] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        tbxWeight1.Text = Dr["Timbang1Weight"].ToString();
                        tbxTicketNum1.Text = Dr["Timbang1Id"].ToString();
                    }
                }
            }
        }

        private void cbWeight2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWeight2.Checked == true)
            {
                tbxWeight2.Enabled = true;
                tbxTicketNum2.Enabled = true;
                btnWeight2.Enabled = false;
            }
            else
            {
                tbxWeight2.Enabled = false;
                tbxTicketNum2.Enabled = false;
                btnWeight2.Enabled = true;
                tbxWeight2.Clear();
                tbxTicketNum2.Clear();
                if (Mode != "New")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [Timbang2Id],[Timbang2Weight] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        tbxWeight2.Text = Dr["Timbang2Weight"].ToString();
                        tbxTicketNum2.Text = Dr["Timbang2Id"].ToString();
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            //HeaderGoodsReceiptNT f = new HeaderGoodsReceiptNT();
            //f.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Mode = "BeforeEdit";
                ModeBeforeEdit();
                GetDataHeader();
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.RowCount >= 1)
                {
                    flag = '\0';

                    //Conn = ConnectionString.GetConnection();
                    //Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    if (Mode == "New")
                    {
                        //Query = "select [FullItemId] from [dbo].[ReceiptOrderD] where [FullItemId] = '" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FullItemId"].Value + "' and [TransferNo] = '" + tbxRONum.Text + "'";
                        //Cmd = new SqlCommand(Query, Conn);
                        //Dr = Cmd.ExecuteReader();
                        //while (Dr.Read())
                        //{
                        //    if (Dr["FullItemId"].ToString() == dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FullItemId"].Value.ToString())
                        //    {
                        //        flag = 'X';
                        //        MetroFramework.MetroMessageBox.Show(this, "Cannot delete this item!\nThis item is generated from " + tbxRONum.Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    }
                        //}
                        Query = "select count(*) from [dbo].[ReceiptOrderD] where [TransferNo] = '" + tbxRONum.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        int count = (Int32)Cmd.ExecuteScalar();
                        if (Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["No"].Value.ToString()) <= count)
                        {
                            flag = 'X';
                            MetroFramework.MetroMessageBox.Show(this, "Cannot delete this item!\nThis item is generated from " + tbxRONum.Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        //Query = "select [FullItemId] from [dbo].[GoodsReceivedD] where [FullItemId] = '" + dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FullItemId"].Value + "' and [GoodsReceivedId] = '" + tbxGRNum.Text + "'";
                        //Cmd = new SqlCommand(Query, Conn);
                        //Dr = Cmd.ExecuteReader();
                        //while (Dr.Read())
                        //{
                        //    if (Dr["FullItemId"].ToString() == dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["FullItemId"].Value.ToString())
                        //    {
                        //        flag = 'X';
                        //        MetroFramework.MetroMessageBox.Show(this, "Cannot delete this item!\nThis item is generated from " + tbxGRNum.Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    }
                        //}
                        Query = "select count(*) from [dbo].[GoodsReceivedD] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        int count = (Int32)Cmd.ExecuteScalar();
                        if (Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["No"].Value.ToString()) <= count)
                        {
                            flag = 'X';
                            MetroFramework.MetroMessageBox.Show(this, "Cannot delete this item!\nThis item is generated from " + tbxGRNum.Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //CHECKING DONE
                    if (flag != 'X')
                        dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSRO_Click(object sender, EventArgs e)
        {

            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "TransferNo";
            tmpSearch.Table = "[dbo].[NotaTransferH]";
            tmpSearch.QuerySearch = "SELECT [TransferNo],[TransferDate],[InventSiteFrom],[InventSiteFromName],[InventSiteTo],[InventSiteToName],[Referensi],[VehicleType],[VehicleNo],[DriverName],[TransStatus],[Notes] FROM [ISBSN].[dbo].[NotaTransferH]";
            tmpSearch.FilterText = new string[] { "TransferNo" };
            tmpSearch.Select = new string[] { "TransferNo", "TransferDate", "InventSiteFrom", "InventSiteFromName", "VehicleType", "VehicleNo", "DriverName","Notes" };
            //tmpSearch.WherePlus = "";
            //tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();

            if (ConnectionString.Kodes != null)
            {
                tbxRONum.Text = ConnectionString.Kodes[0];
                dtRO.Value = Convert.ToDateTime(ConnectionString.Kodes[1]);
                txtWarehouse.Text = ConnectionString.Kodes[2];
                txtLocation.Text = ConnectionString.Kodes[3];
                tbxVType.Text = ConnectionString.Kodes[4];
                tbxVNumber.Text = ConnectionString.Kodes[5];
                tbxDriverName.Text = ConnectionString.Kodes[6];
                tbxNotes.Text = ConnectionString.Kodes[7];

                ConnectionString.Kodes = null;
            }

            if (tbxRONum.Text != "")
            {
                if (dataGridView1.Rows.Count >= 1)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();
                }
                if (dataGridView1.RowCount - 1 <= 0)
                {
                    dataGridView1.ColumnCount = 14;
                    dataGridView1.Columns[0].Name = "No";
                    dataGridView1.Columns[1].Name = "FullItemID";
                    dataGridView1.Columns[2].Name = "ItemName";
                    dataGridView1.Columns[3].Name = "QtyUoM";
                    dataGridView1.Columns[4].Name = "UoM";
                    dataGridView1.Columns[5].Name = "QtyAlt";
                    dataGridView1.Columns[6].Name = "UoMAlt";
                    dataGridView1.Columns[7].Name = "Ratio";
                    dataGridView1.Columns[8].Name = "Blok";
                    dataGridView1.Columns[9].Name = "Notes";
                    dataGridView1.Columns[10].Name = "GroupId";
                    dataGridView1.Columns[11].Name = "SubGroup1Id";
                    dataGridView1.Columns[12].Name = "SubGroup2Id";
                    dataGridView1.Columns[13].Name = "ItemId";
                }

                Conn = ConnectionString.GetConnection();
                Query = "select [SeqNo],[FullItemId],[ItemName],[Qty],[Uom],[Qty_Alt],[Uom_Alt],[Ratio],[InventSite],[Notes],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId] from [dbo].[NotaTransferD] where [TransferNo] = '" + tbxRONum.Text + "'; ";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader(); 

                while (Dr.Read())
                {
                    this.dataGridView1.Rows.Add(Dr[0].ToString(), Dr[1].ToString(), Dr[2].ToString(), Dr[3].ToString(), Dr[4].ToString(), Dr[5].ToString(), Dr[6].ToString(), Dr[7].ToString(), Dr[8].ToString(), Dr[9].ToString(), Dr[10].ToString(), Dr[11].ToString(), Dr[12].ToString(), Dr[13].ToString());
                }

                dataGridView1.ReadOnly = false;
                dataGridView1.Columns["No"].ReadOnly = true;
                dataGridView1.Columns["FullItemID"].ReadOnly = true;
                dataGridView1.Columns["ItemName"].ReadOnly = true;
                dataGridView1.Columns["QtyUoM"].ReadOnly = false;
                dataGridView1.Columns["UoM"].ReadOnly = true;
                dataGridView1.Columns["QtyAlt"].ReadOnly = false;
                dataGridView1.Columns["UoMAlt"].ReadOnly = true;
                dataGridView1.Columns["Ratio"].ReadOnly = true;
                dataGridView1.Columns["Blok"].ReadOnly = false;
                dataGridView1.Columns["Notes"].ReadOnly = false;

                dataGridView1.Columns["GroupId"].Visible = false;
                dataGridView1.Columns["SubGroup1Id"].Visible = false;
                dataGridView1.Columns["SubGroup2Id"].Visible = false;
                dataGridView1.Columns["ItemId"].Visible = false;

                dataGridView1.Columns["No"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["FullItemID"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["ItemName"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["QtyUoM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["UoM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["QtyAlt"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["UoMAlt"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Ratio"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Blok"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Notes"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridView1.AutoResizeColumns();
                dataGridView1.Refresh();
                
            }
        }

        #region createLabel
        private void createLabel(int x, int y, string group, string status)
        {
            if (validate == false)
            {
                label[count] = new Label();
            }
            if (status == "OK")
                label[count].Visible = false;
            else
            {
                label[count].Text = "*";
                label[count].ForeColor = Color.Red;
                label[count].Width = 10;
                label[count].Location = new System.Drawing.Point(x, y);
                label[count].BringToFront();
                if (group == "Weight1")
                    this.gbWeight1.Controls.Add(label[count]);
                else if (group == "Weight2")
                    this.gbWeight2.Controls.Add(label[count]);
                else
                    this.gbMain.Controls.Add(label[count]);

                check = false;
            }

            count++;
        }
        #endregion

        bool validate;
        #region Validation
        private char Validation()
        {
            flag = '\0';
            string msg = "";
            //check = true;
            if (validate == false)
            {
                label = new Label[20];
            }
            //count = 0;

            //MAIN
            if (tbxRONum.Text == String.Empty) { createLabel(9, 104, "Main", ""); flag = 'X'; }
            else { createLabel(9, 104, "Main", "OK"); }
            if (tbxDelivNum.Text == String.Empty) { createLabel(9, 156, "Main", ""); flag = 'X'; }
            else { createLabel(9, 156, "Main", "OK"); }
            if (tbxVOwner.Text == String.Empty || tbxVOwnerID.Text == String.Empty) { createLabel(424, 25, "Main", ""); flag = 'X'; }
            else { createLabel(424, 25, "Main", "OK"); }
            if (tbxVType.Text == String.Empty) { createLabel(424, 52, "Main", ""); flag = 'X'; }
            else { createLabel(424, 52, "Main", "OK"); }
            if (tbxVNumber.Text == String.Empty) { createLabel(424, 78, "Main", ""); flag = 'X'; }
            else { createLabel(424, 78, "Main", "OK"); }
            if (tbxDriverName.Text == String.Empty) { createLabel(424, 104, "Main", ""); flag = 'X'; }
            else { createLabel(424, 104, "Main", "OK"); }

            if (tbxWeight1.Text == String.Empty) { createLabel(4, 78, "Weight1", ""); flag = 'X'; }
            else { createLabel(4, 78, "Weight1", "OK"); }
            if (tbxTicketNum1.Text == String.Empty) { createLabel(4, 52, "Weight1", ""); flag = 'X'; }
            else { createLabel(4, 52, "Weight1", "OK"); }

            if (Mode != "New")
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                if (Cmd.ExecuteScalar().ToString() == "02" && Login.UserGroup == "SalesManager")
                {
                    if (tbxWeight2.Text == String.Empty) { createLabel(4, 78, "Weight2", ""); flag = 'X'; }
                    else { createLabel(4, 78, "Weight2", "OK"); }
                    if (tbxTicketNum2.Text == String.Empty) { createLabel(4, 52, "Weight2", ""); flag = 'X'; }
                    else { createLabel(4, 52, "Weight2", "OK"); }
                }
            }

            //if (flag != 'X')
            //{
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            if (Mode != "New" && flag != 'X')
            {
                if (Cmd.ExecuteScalar().ToString() == "01")
                {
                    if (Login.UserGroup == "SalesManager") //WB
                    {
                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            for (int j = 0; j < dataGridView1.RowCount; j++)
                            {
                                if (dataGridView1.Columns[i].Name == "Qty" || dataGridView1.Columns[i].Name == "Blok" || dataGridView1.Columns[i].Name == "Deskripsi")
                                {
                                    if (dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value != null)
                                    {
                                        if (dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value.ToString() == String.Empty)
                                        //if (string.IsNullOrEmpty(dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value as string))
                                        {
                                            if (dataGridView1.Columns[i].Name == "Deskripsi")
                                                msg += "Row " + Convert.ToInt32(j + 1) + " Column Action Plan cannot blank!\n";
                                            else
                                                msg += "Row " + Convert.ToInt32(j + 1) + " Column " + dataGridView1.Columns[i].Name + " cannot blank!\n";
                                            flag = 'X';
                                        }
                                    }
                                }
                            }
                            if (flag == 'X')
                                break;
                        }

                        //HARUS NGECEK QTY
                        Cmd = new SqlCommand("select [ItemName], [FullItemId], [Qty], [Qty_SJ], [Qty_Actual] from [dbo].[GoodsReceivedD] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                        Dr = Cmd.ExecuteReader();
                        int index = 0;
                        while (Dr.Read())
                        {
                            if (Convert.ToInt32(Dr["Qty"]) != 0)
                            {
                                if (Convert.ToDecimal(Dr["Qty"]) < Convert.ToDecimal(dataGridView1.Rows[index].Cells["Qty"].Value.ToString()))
                                {
                                    msg += "Row " + Convert.ToInt32(index + 1) + " " + Dr["ItemName"] + " (" + Dr["FullItemId"] + ")" + " cannot more than " + Dr["Qty"] + "\n";
                                    flag = 'X';
                                }
                            }
                            index++;
                        }
                        Dr.Close();

                    }
                    else if (Login.UserGroup == "PurchaseManager")
                    {
                        for (int j = 0; j < dataGridView1.RowCount; j++)
                        {
                            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                            {
                                if (dataGridView1.Columns[i].Name == "QtyActual" || dataGridView1.Columns[i].Name == "RatioActual" || dataGridView1.Columns[i].Name == "Quality")
                                {
                                    if (string.IsNullOrEmpty(dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value as string) || dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value.ToString() == "0.00" || dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value.ToString() == "0.0000")
                                    {
                                        msg += "Column " + dataGridView1.Columns[i].Name + " cannot blank!\n";
                                        flag = 'X';
                                        //check = false;
                                    }
                                }
                            }
                            if (flag == 'X')
                                break;
                        }

                        //HARUS NGECEK QTY
                        if (flag != 'X')
                        {
                            Cmd = new SqlCommand("select [ItemName], [FullItemId], [Qty] ,[Qty_SJ] ,[Qty_Actual] from [dbo].[GoodsReceivedD] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                            Dr = Cmd.ExecuteReader();
                            int index = 0;
                            while (Dr.Read())
                            {
                                if (Dr["Qty"].ToString() != "0.00")
                                {
                                    if (Convert.ToDecimal(Dr["Qty"]) < Convert.ToInt32(dataGridView1.Rows[index].Cells["QtyActual"].Value.ToString()))
                                    {
                                        msg += Dr["ItemName"] + "(" + Dr["FullItemId"] + ")" + " cannot more than " + Dr["Qty"];
                                        //msg += "Qty Actual cannot more than qty RO!\n";
                                        flag = 'X';
                                        break;
                                    }
                                }
                                index++;
                            }
                            Dr.Close();
                        }
                    }
                }
                if (Cmd.ExecuteScalar().ToString() == "02")
                {
                    if (Login.UserGroup == "SalesManager") //WB
                    {

                    }
                    else if (Login.UserGroup == "PurchaseManager")
                    {
                        if (tbxWeight2.Text == String.Empty) { createLabel(4, 78, "Weight2", ""); }
                        else { createLabel(4, 78, "Weight2", "OK"); }
                        if (tbxTicketNum2.Text == String.Empty) { createLabel(4, 52, "Weight2", ""); }
                        else { createLabel(4, 52, "Weight2", "OK"); }
                    }
                }
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                }
            }
            //}
            else //MODE NEW
            {
                //for(int i= 0; i < dataGridView1.RowCount;i++)
                //{

                //if(dataGridView1.Rows[i].Cells["Blok"].Value == String.Empty)
                //{
                //}
                //}

                //for (int i = 0; i < dataGridView1.ColumnCount; i++)
                //{
                //    for (int j = 0; j < dataGridView1.RowCount; j++)
                //    {
                //        if (dataGridView1.Columns[i].Name == "Qty" || dataGridView1.Columns[i].Name == "Blok" || dataGridView1.Columns[i].Name == "Deskripsi")
                //        {
                //            if (dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value != null)
                //            {
                //                if (dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value.ToString() == String.Empty)
                //                //if (string.IsNullOrEmpty(dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value as string))
                //                {
                //                    if (dataGridView1.Columns[i].Name == "Deskripsi")
                //                        msg += "Row " + Convert.ToInt32(j + 1) + " Column Action Plan cannot blank!\n";
                //                    else
                //                        msg += "Row " + Convert.ToInt32(j + 1) + " Column " + dataGridView1.Columns[i].Name + " cannot blank!\n";
                //                    flag = 'X';
                //                }
                //            }
                //        }
                //    }
                //    if (flag == 'X')
                //        break;
                //}

                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if (dataGridView1.Columns[i].Name == "Blok" || dataGridView1.Columns[i].Name == "Deskripsi")
                        {
                            if (string.IsNullOrEmpty(dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value as string))
                            {
                                if (dataGridView1.Columns[i].Name == "Deskripsi")
                                    msg += "Row " + Convert.ToInt32(j + 1) + " Column Action Plan cannot blank!\n";
                                else
                                    msg += "Row " + Convert.ToInt32(j + 1) + " Column " + dataGridView1.Columns[i].Name + " cannot blank!\n";
                                flag = 'X';
                            }
                        }
                    }
                    if (flag == 'X')
                        break;
                }

                //HARUS NGECEK QTY
                Cmd = new SqlCommand("select [ItemName], [FullItemId], [RemainingQty] from [dbo].[ReceiptOrderD] where [ReceiptOrderId] = '" + tbxRONum.Text + "'", Conn);
                Dr = Cmd.ExecuteReader();
                int index = 0;
                while (Dr.Read())
                {
                    if (Convert.ToDecimal(Dr["RemainingQty"]) < Convert.ToDecimal(dataGridView1.Rows[index].Cells["Qty"].Value.ToString()))
                    {
                        msg += "Row " + Convert.ToInt32(index + 1) + " " + Dr["ItemName"] + " (" + Dr["FullItemId"] + ")" + " cannot more than " + Dr["RemainingQty"];
                        flag = 'X';
                    }
                    index++;
                }
                Dr.Close();
            }

            if (msg != String.Empty)
                MetroFramework.MetroMessageBox.Show(this, msg, "Fill the form!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (flag == 'X')
                MetroFramework.MetroMessageBox.Show(this, "Field with * is required!", "Fill the form!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            count = 0;
            validate = true;
            return flag;
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            ListMethod CheckValidasi = new ListMethod();

            CheckValidasi.Validasi(tbxRONum);
            //CheckValidasi.Validasi(tbxVOwnerID);
            //CheckValidasi.Validasi(tbxVOwner);
            //CheckValidasi.Validasi(tbxTicketNum1);

            if (CheckValidasi.VarValidasi == false)
            {
                MessageBox.Show("Text yang berwarna merah harus diisi.");
            }

            try
            {
                if (CheckValidasi.VarValidasi != false)
                {
                    Conn = ConnectionString.GetConnection();
                    //SAVE MODE EDIT
                    if (Mode == "Edit")
                    {
                        Query = "update [dbo].[GoodsReceivedNTH] set [SJDate] = '" + dtDO.Value.ToString("yyyy-MM-dd") + "' ";
                        Query += ", [SJNumber] = '" + tbxDelivNum.Text + "' ";
                        Query += ", [ExpectedDate] = '" + dtExpectedDate.Text + "' ";
                        Query += ", [VendId] = '" + tbxVOwnerID.Text + "' ";
                        Query += ", [VendorName] = '" + tbxVOwner.Text + "' ";
                        Query += ", [VehicleType] = '" + tbxVType.Text + "' ";
                        Query += ", [VehicleNumber] = '" + tbxVNumber.Text + "' ";
                        Query += ", [DriverName] = '" + tbxDriverName.Text + "' ";
                        Query += ", [Timbang1Date] = '" + dtWeight1.Value.ToString("yyyy-MM-dd") + "' ";
                        Query += ", [Timbang1Id] = '" + tbxTicketNum1.Text + "' ";
                        Query += ", [Timbang1Weight] = '" + Convert.ToDecimal(tbxWeight1.Text) + "' ";
                        Query += ", [Timbang2Date] = '" + dtWeight2.Value.ToString("yyyy-MM-dd") + "' ";
                        Query += ", [SiteID] = '" + txtInventSiteID.Text.Trim() + "' ";
                        Query += ", [SiteName] = '" + txtWarehouse.Text.Trim() + "' ";
                        Query += ", [SiteLocation] = '" + txtLocation.Text.Trim() + "' ";
                        Query += ", [Timbang2Id] = '" + tbxTicketNum2.Text + "' ";
                        if (tbxWeight2.Text != String.Empty)
                            Query += ", [Timbang2Weight] = '" + Convert.ToDecimal(tbxWeight2.Text) + "' ";
                        Query += ", [Notes] = '" + tbxNotes.Text + "' ";
                        Query += ", [UpdatedDate] = '" + DateTime.Now + "' ";
                        Query += ", [UpdatedBy] = '" + Login.UserGroup + "' ";


                        Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                        if (Cmd.ExecuteScalar().ToString() == "02" && Login.UserGroup == "SalesManager")
                        {
                            Query += ", [GoodsReceivedStatus] = '03' ";
                            if (cbWeight2.Checked == true)
                                Query += ", [StatusWeight2] = 'Manual' ";
                            else
                                Query += ", [StatusWeight2] = 'Mesin' ";
                        }
                        else if (Cmd.ExecuteScalar().ToString() == "01" && Login.UserGroup == "PurchaseManager")
                        {
                            Query += ", [GoodsReceivedStatus] = '02' ";
                        }
                        Query += "where [GoodsReceivedId] = '" + tbxGRNum.Text + "'; ";

                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            Cmd = new SqlCommand("Select [StatusCode] from [dbo].[TransStatusTable] where [Deskripsi] = '" + dataGridView1.Rows[i].Cells["Deskripsi"].Value + "' and [TransCode] = 'GRD'", Conn);
                            string statCode = Cmd.ExecuteScalar().ToString();

                            Cmd = new SqlCommand("select count(*) from [dbo].[GoodsReceivedD] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                            if (i < Convert.ToInt32(Cmd.ExecuteScalar()))
                            {
                                Query += "update [dbo].[GoodsReceivedD] set ";
                                if (dataGridView1.Rows[i].Cells["Qty"].Value.ToString() == String.Empty)
                                    Query += "[Qty_SJ] = '0', ";
                                else
                                    Query += "[Qty_SJ] = '" + dataGridView1.Rows[i].Cells["Qty"].Value + "', ";
                                Query += "[InventSiteBlokID] = '" + dataGridView1.Rows[i].Cells["Blok"].Value + "', ";
                                Query += "[ActionCodeStatus] = '" + statCode + "', ";
                                Query += "[Notes] = '" + dataGridView1.Rows[i].Cells["Notes"].Value + "', ";
                                if (dataGridView1.Rows[i].Cells["QtyActual"].Value.ToString() == String.Empty)
                                    Query += "[Qty_Actual] = '0', ";
                                else
                                    Query += "[Qty_Actual] = '" + dataGridView1.Rows[i].Cells["QtyActual"].Value + "', ";
                                if (dataGridView1.Rows[i].Cells["RatioActual"].Value == null || dataGridView1.Rows[i].Cells["RatioActual"].Value.ToString() == String.Empty)
                                    Query += "[Ratio_Actual] = '0', ";
                                else
                                    Query += "[Ratio_Actual] = '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["RatioActual"].Value) + "', ";
                                if (dataGridView1.Rows[i].Cells["Quality"].Value == null || dataGridView1.Rows[i].Cells["Quality"].Value.ToString() == String.Empty)
                                    Query += "[Quality] = NULL, ";
                                else
                                    Query += "[Quality] = '" + dataGridView1.Rows[i].Cells["Quality"].Value + "', ";
                                Query += "[UpdatedDate] = getdate(), ";
                                Query += "[UpdatedBy] = '" + Login.UserGroup + "' ";
                                Query += "where [GoodsReceivedId] = '" + tbxGRNum.Text + "' ";
                                Query += "and [FullItemId] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "' ";
                                Query += "and [GoodsReceivedSeqNo] = '" + Convert.ToInt32(i + 1) + "'; ";
                            }
                            else
                            {
                                Cmd = new SqlCommand("select distinct [InventSiteID] from [dbo].[InventSiteBlok] where [InventSiteBlokID] = '" + dataGridView1.Rows[i].Cells["Blok"].Value.ToString() + "'; ", Conn);
                                string inventID = Cmd.ExecuteScalar().ToString();

                                Query += "insert into [dbo].[GoodsReceivedD] ( [GoodsReceivedDate], [GoodsReceivedId], [GoodsReceivedSeqNo], [FullItemId], [ItemName], [Qty], [Qty_SJ], [Unit], [InventSiteBlokID], [ActionCodeStatus], [Notes], [Qty_Actual], [Ratio_Actual], [InventSiteId], [Quality], [CreatedDate], [CreatedBy] ) values ( getdate(), '" + tbxGRNum.Text + "', '" + Convert.ToInt32(i + 1) + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value + "', '0', '";
                                if (dataGridView1.Rows[i].Cells["Qty"].Value.ToString() == String.Empty)
                                    Query += "0',";
                                else
                                    Query += dataGridView1.Rows[i].Cells["Qty"].Value + "', ";
                                Query += "'" + dataGridView1.Rows[i].Cells["Unit"].Value + "', '" + dataGridView1.Rows[i].Cells["Blok"].Value + "', '" + statCode + "', '" + dataGridView1.Rows[i].Cells["Notes"].Value + "', '0', '0', '" + inventID + "', '" + dataGridView1.Rows[i].Cells["Quality"].Value + "', getdate(), '" + Login.UserGroup + "' ); ";
                            }
                        }

                        Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                        if (Cmd.ExecuteScalar().ToString() == "02" && Login.UserGroup == "SalesManager")
                        {
                            //UPDATE REMAINING QTY 
                            int remainingQty = 0;
                            Cmd = new SqlCommand("select a.TransferNo, a.ReceiptOrderSeqNo, a.Qty_Actual, b.RemainingQty from [dbo].[GoodsReceivedD] as a left join ReceiptOrderD as b on a.TransferNo = b.TransferNo where a.TransferNo = '" + tbxRONum.Text + "' and a.GoodsReceivedId = '" + tbxGRNum.Text + "'", Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                remainingQty = Convert.ToInt32(Dr["RemainingQty"]) - Convert.ToInt32(Dr["Qty_Actual"]);
                                Query += "update [dbo].[ReceiptOrderD] set [RemainingQty] = '" + remainingQty + "' where [TransferNo] = '" + Dr["TransferNo"] + "' and [SeqNo] = '" + Dr["ReceiptOrderSeqNo"] + "'";
                            }
                            Dr.Close();
                            //CHANGE RO HEADER STATS
                            Cmd = new SqlCommand("select [Qty] ,[Qty_SJ] ,[Qty_Actual] from [dbo].[GoodsReceivedD] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                if (Convert.ToDecimal(Dr["Qty"]) == Convert.ToDecimal(Dr["Qty_Actual"]))
                                    Query += "update [dbo].[NotaTransferH] set [UpdatedDate] = getdate(), [UpdatedBy] = '" + Login.UserGroup + "', [ReceiptOrderStatus] ='03' where [TransferNo] = '" + tbxRONum.Text + "'; ";
                                else if (Convert.ToDecimal(Dr["Qty"]) > Convert.ToDecimal(Dr["Qty_Actual"]))
                                    Query += "update [dbo].[NotaTransferH] set [UpdatedDate] = getdate(), [UpdatedBy] = '" + Login.UserGroup + "', [ReceiptOrderStatus] ='02' where [TransferNo] = '" + tbxRONum.Text + "'; ";
                            }
                            Dr.Close();
                        }
                        //Cmd = new SqlCommand(Query, Conn);
                        //int k = Cmd.ExecuteNonQuery();
                        //if (k == 0)
                        //{
                        //    MessageBox.Show("Not able to save detail data!");
                        //}

                        //AUTO RESIZE
                        //CHECK IF ITEM NEED RESIZE
                        Cmd = new SqlCommand("select * from GoodsReceivedNTH as GRH left join GoodsReceivedD as GRD on GRH.GoodsReceivedId = GRD.GoodsReceivedId where GRH.GoodsReceivedId = '" + tbxGRNum.Text + "' and GRH.GoodsReceivedStatus = '03' and GRD.FullItemId in (select distinct(RD.FullItemId) from ResizeTableH as RH left join ResizeTableD as RD on RH.TransId = RD.TransId)", Conn);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            HeaderResizeGRNT f = new HeaderResizeGRNT();
                            //string resizeID = f.GenerateID();
//                            Query += "insert into [dbo].[InventResizeH] ([TransDate] ,[TransId] ,[CreatedDate] ,[CreatedBy],[Posted])values ( getdate(), '" + resizeID + "', getdate(), '" + Login.UserGroup + "', 1); ";

                            int seqNoA = 1;
                            int seqNoB = 1;
                            while (Dr.Read())
                            {
                                Query += "insert into [dbo].[InventResizeD] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";
                                //Query += "values ('" + resizeID + "', '" + seqNoA + "', '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"].ToString() + "', '" + Dr["ItemId"].ToString() + "', '" + Dr["FullItemId"].ToString() + "', '" + Dr["ItemName"].ToString() + "', '" + Dr["InventSiteId"] + "', NULL, '" + Convert.ToInt32(Convert.ToInt32(Dr["Qty_Actual"]) * -1) + "', '" + Dr["Unit"] + "', '" + tbxGRNum.Text + "', '" + Dr["GoodsReceivedSeqNo"] + "', NULL, NULL, NULL, NULL, NULL, NULL, NULL, getdate(), '" + Login.UserGroup + "'); ";


                                Query += "insert into [dbo].[InventResizeD] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";


                                Cmd = new SqlCommand("select count(distinct(RD.ToFullItemId)) from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn);
                                int countRszItem = (Int32)Cmd.ExecuteScalar();

                                if (countRszItem == 1)
                                {
                                    Cmd = new SqlCommand("select rd.ToFullItemId from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn);
                                    string fullItemID = Cmd.ExecuteScalar().ToString();

                                    Cmd = new SqlCommand("select rd.ToItemName from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn);
                                    string itemName = Cmd.ExecuteScalar().ToString();

                                   // Query += "values ('" + resizeID + "', '" + seqNoB + "', '" + fullItemID.Split('.')[0] + "', '" + fullItemID.Split('.')[1] + "', '" + fullItemID.Split('.')[2] + "', '" + fullItemID.Split('.')[3] + "', '" + fullItemID + "', '" + itemName + "', NULL, NULL, '" + Dr["Qty_Actual"] + "', '" + Dr["Unit"] + "', '" + tbxGRNum.Text + "', NULL, '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"] + "', '" + Dr["ItemId"] + "', '" + Dr["FullItemId"] + "', '" + Dr["ItemName"] + "', '" + Dr["GoodsReceivedSeqNo"] + "', getdate(), '" + Login.UserGroup + "'); ";
                                }
                                else
                                {
                                    //Query += "values ('" + resizeID + "', '" + seqNoB + "', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '" + Dr["Qty_Actual"] + "', '" + Dr["Unit"] + "', '" + tbxGRNum.Text + "', NULL, '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"] + "', '" + Dr["ItemId"] + "', '" + Dr["FullItemId"] + "', '" + Dr["ItemName"] + "', '" + Dr["GoodsReceivedSeqNo"] + "', getdate(), '" + Login.UserGroup + "'); ";
                                }
                                seqNoA += 1;
                                seqNoB += 1;
                            }
                        }
                        Dr.Close();
                        Cmd = new SqlCommand(Query, Conn);
                        int k = Cmd.ExecuteNonQuery();
                        if (k == 0)
                        {
                            MessageBox.Show("Not able to save detail data!");
                        }
                        MetroFramework.MetroMessageBox.Show(this, tbxGRNum.Text + " has been successfully saved!", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ModeBeforeEdit();
                        //Parent.RefreshGrid();
                    }
                    //SAVE MODE NEW
                    else if (Mode == "New")
                    {
                        Query = "insert [dbo].[GoodsReceivedNTH] ( [GoodsReceivedId], [GoodsReceivedDate], GoodsReceivedStatus, [TransferNo], [TransferDate], [SJDate], SJNumber, ExpectedDate, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, Timbang1Id, Timbang1Weight, [Timbang2Date], Notes, SiteID, SiteName, SiteLocation, CreatedDate, CreatedBy, [StatusWeight1] )";
                        Query += "values ( (Select 'NTGR-'+FORMAT(getdate(), 'yyMM')+'-'+Right('00000' + CONVERT(NVARCHAR, case when Max(GoodsReceivedId) is null then '1' else substring(Max(GoodsReceivedId),11,4)+1 end), 5) ";
                        Query += "from [GoodsReceivedNTH] where Left(convert(varchar, createddate, 112),6) = Left(convert(varchar, getdate(), 112),6))";
                        Query += ", '" + dtGR.Value.ToString("yyyy-MM-dd") + "'";
                        Query += ", '01', '" + tbxRONum.Text + "'";
                        Query += ", '" + dtRO.Value.ToString("yyyy-MM-dd") + "'";
                        Query += ", '" + dtDO.Value.ToString("yyyy-MM-dd") + "'";
                        Query += ", '" + tbxDelivNum.Text + "'";
                        Query += ", '" + dtExpectedDate.Value.ToString("yyyy-MM-dd") + "'";
                        Query += ", '" + tbxVOwnerID.Text + "'";
                        Query += ", '" + tbxVOwner.Text + "'";
                        Query += ", '" + tbxVType.Text + "'";
                        Query += ", '" + tbxVNumber.Text + "'";
                        Query += ", '" + tbxDriverName.Text + "'";
                        Query += ", '" + dtWeight1.Value.ToString("yyyy-MM-dd") + "'";
                        Query += ", '" + tbxTicketNum1.Text + "'";
                        Query += ", '" + Convert.ToDecimal(tbxWeight1.Text == "" ? "0" : tbxWeight1.Text) + "'";
                        Query += ", '" + dtWeight2.Value.ToString("yyyy-MM-dd") + "'";
                        Query += ", '" + tbxNotes.Text + "'";
                        Query += ", '" + txtInventSiteID.Text + "'";
                        Query += ", '" + txtWarehouse.Text + "'";
                        Query += ", '" + txtLocation.Text + "'";
                        Query += ", '" + DateTime.Now + "'";
                        Query += ", '" + Login.UserGroup + "'";
                        if (cbWeight1.Checked == true)
                            Query += ", 'Manual' ); ";
                        else
                            Query += ", 'Mesin' ); ";
                        Cmd = new SqlCommand(Query, Conn);
                        int k = Cmd.ExecuteNonQuery();
                        if (k > 0)
                        {
                            //INSERT DETAIL ITEM 
                            decimal seq = 1;
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                Query = "Select [StatusCode] from [dbo].[TransStatusTable] where [Deskripsi] = '" + dataGridView1.Rows[i].Cells["Deskripsi"].Value + "' and [TransCode] = 'GRD'";
                                Cmd = new SqlCommand(Query, Conn);
                                string statCode = Cmd.ExecuteScalar().ToString();

                                decimal ROQty = 0;
                                decimal RORemainingQty = 0;
                                string ROID = tbxRONum.Text;
                                int seqRO = Int32.Parse(dataGridView1.Rows[i].Cells["No"].Value.ToString());
                                try
                                {
                                    Query = "Select [Qty] from [dbo].[ReceiptOrderD] where [TransferNo] = '" + tbxRONum.Text + "' and [FullItemId] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "' and [SeqNo] = '" + dataGridView1.Rows[i].Cells["No"].Value + "'; ";
                                    Cmd = new SqlCommand(Query, Conn);
                                    ROQty = (Decimal)Cmd.ExecuteScalar();
                                }
                                catch
                                {
                                    ROQty = 0;
                                    ROID = "";
                                    seqRO = 0;
                                }
                                //flag = "";
                                //if (ROQty < Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value))
                                //{
                                //    MetroFramework.MetroMessageBox.Show(this, "Quantity Surat Jalan tidak boleh melebihi Quantity Order", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                //    flag = 'X';
                                //}
                                try
                                {
                                    Query = "Select [RemainingQty] from [dbo].[ReceiptOrderD] where [TransferNo] = '" + tbxRONum.Text + "' and [FullItemId] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "' and [SeqNo] = '" + dataGridView1.Rows[i].Cells["No"].Value + "'; ";
                                    Cmd = new SqlCommand(Query, Conn);
                                    RORemainingQty = (Decimal)Cmd.ExecuteScalar();
                                }
                                catch
                                {
                                    RORemainingQty = 0;
                                }

                                if (RORemainingQty == 0)
                                    ROQty = RORemainingQty;
                                else if (RORemainingQty == ROQty)
                                { }
                                else if (RORemainingQty < ROQty)
                                    ROQty = RORemainingQty;

                                //GET ITEM DETAILS
                                Query = "select [GroupID], [SubGroup1ID], [SubGroup2ID], [ItemID] from [dbo].[InventTable] where [FullItemID] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "'; ";
                                Cmd = new SqlCommand(Query, Conn);
                                Dr = Cmd.ExecuteReader();
                                string groupID = "", subGroup1ID = "", subGroup2ID = "", itemID = "";
                                while (Dr.Read())
                                {
                                    groupID = Dr["GroupID"].ToString();
                                    subGroup1ID = Dr["SubGroup1ID"].ToString();
                                    subGroup2ID = Dr["SubGroup2ID"].ToString();
                                    itemID = Dr["itemID"].ToString();
                                }

                                //GET INVENT SITE
                                Query = "select [InventSiteID] from [dbo].[InventSiteBlok] where [InventSiteBlokID] = '" + dataGridView1.Rows[i].Cells["Blok"].Value + "'; ";
                                Cmd = new SqlCommand(Query, Conn);
                                string inventSite = Cmd.ExecuteScalar().ToString();

                                Query = "insert into [dbo].[GoodsReceivedD] values ('" + dtGR.Value.ToString("yyyy-MM-dd") + "'";
                                Query += ", '" + tbxGRNum.Text + "'";
                                Query += ", '" + seq + "'";
                                if (ROID == String.Empty)
                                    Query += ", NULL";
                                else
                                    Query += ", '" + ROID + "'";
                                if (seqRO == 0)
                                    Query += ", NULL";
                                else
                                    Query += ", '" + seqRO + "'";
                                Query += ", '" + groupID/*dataGridView1.Rows[i].Cells["GroupId"].Value*/ + "'";
                                Query += ", '" + subGroup1ID/*dataGridView1.Rows[i].Cells["SubGroup1Id"].Value*/ + "'";
                                Query += ", '" + subGroup2ID/*dataGridView1.Rows[i].Cells["SubGroup2Id"].Value*/ + "'";
                                Query += ", '" + itemID + "'";
                                Query += ", '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "'";
                                Query += ", '" + dataGridView1.Rows[i].Cells["ItemName"].Value + "'";
                                Query += ", '" + ROQty + "'";
                                Query += ", '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) + "'";
                                Query += ", '0.00'";
                                Query += ", '" + dataGridView1.Rows[i].Cells["Unit"].Value + "'";
                                Query += ", '0'"; //Ratio
                                Query += ", '0', '" + inventSite + "'";
                                Query += ", '" + dataGridView1.Rows[i].Cells["Blok"].Value + "'";
                                Query += ", NULL" /*+ dataGridView1.Rows[i].Cells["Quality"].Value +*/  + "";
                                Query += ", '" + dataGridView1.Rows[i].Cells["Notes"].Value + "'";
                                Query += ", '" + statCode + "'";
                                Query += ", getdate(), '" + Login.UserGroup + "'";
                                Query += ", '1753/01/01', ''); ";
                                Cmd = new SqlCommand(Query, Conn);
                                k = Cmd.ExecuteNonQuery();
                                if (k == 0)
                                    MessageBox.Show("Not able to save detail data!");
                                seq++;
                            }
                            //UPDATE RO STATUS
                            Query = "update [dbo].[NotaTransferH] set [ReceiptOrderStatus] = '04', [UpdatedDate] = getdate(), [UpdatedBy] = '" + Login.UserGroup + "' where [TransferNo] = '" + tbxRONum.Text + "'; ";
                            Cmd = new SqlCommand(Query, Conn);
                            k = Cmd.ExecuteNonQuery();
                            if (k == 0)
                                MessageBox.Show("Not able to save detail data!");

                            MetroFramework.MetroMessageBox.Show(this, tbxGRNum.Text + " has been successfully saved!", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dataGridView1.ReadOnly = true;
                            //dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.LightGray;
                            //dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;

                            btnSave.Enabled = false; btnCancel.Enabled = false;
                            btnEdit.Enabled = true; btnPrint.Enabled = true;
                            rbTicket.Enabled = true; rbTT.Enabled = true;
                            btnNew.Enabled = false; btnDelete.Enabled = false;

                            tbxVNumber.Enabled = false; tbxVOwner.Enabled = false; tbxVType.Enabled = false;
                            tbxDriverName.Enabled = false; tbxDelivNum.Enabled = false;
                            tbxNotes.ReadOnly = true;
                            tbxRONum.Enabled = false; dtDO.Enabled = false;
                            btnSRO.Enabled = false; btnSOwner.Enabled = false;
                            //WEIGHT 1
                            btnWeight1.Enabled = false; dtWeight1.Enabled = false; tbxWeight1.Enabled = false;
                            tbxTicketNum1.Enabled = false; cbWeight1.Enabled = false;
                            //WEIGHT 2
                            btnWeight2.Enabled = false; dtWeight2.Enabled = false; tbxWeight2.Enabled = false;
                            tbxTicketNum2.Enabled = false; cbWeight2.Enabled = false;
                            //InquiryGoodsReceiptNT F = new InquiryGoodsReceiptNT();
                            //F.RefreshGrid();
                        }
                        else
                            MessageBox.Show("Not able to save data!");
                    }

                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                Parent.RefreshGrid();
            }
        }

        private void btnSOwner_Click(object sender, EventArgs e)
        {
            try
            {
                string SchemaName = "dbo";
                string TableName = "VendTable";
                //string Where = " AND TransStatus='13'";

                Search tmpSearch = new Search();
                tmpSearch.SetSchemaTable(SchemaName, TableName);//, Where);
                tmpSearch.ShowDialog();
                if (ConnectionString.Kode != String.Empty && ConnectionString.Kode2 != String.Empty)
                {
                    tbxVOwnerID.Text = ConnectionString.Kode;
                    tbxVOwner.Text = ConnectionString.Kode2;
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dataGridView1.Columns[e.ColumnIndex].HeaderText.Contains("Qty") || dataGridView1.Columns[e.ColumnIndex].HeaderText.Contains("QtyActual"))
                {
                    if (e.Value == null)
                        e.Value = "0";
                    if (e.Value != null)
                    {
                        if (e.Value.ToString() != "")
                        {
                            double d = double.Parse(e.Value.ToString());
                            dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            e.Value = d.ToString("N2");
                        }
                    }
                }
                if (dataGridView1.Columns[e.ColumnIndex].HeaderText.Contains("Ratio") || dataGridView1.Columns[e.ColumnIndex].HeaderText.Contains("Price"))
                {
                    if (e.Value != null) //e.Value != String.Empty || 
                    {
                        if (e.Value.ToString() != "")
                        {
                            double d = double.Parse(e.Value.ToString());
                            dataGridView1.Columns[e.ColumnIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            e.Value = d.ToString("N4");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //if (dataGridView1.Columns[3].Name == "Qty" || dataGridView1.Columns["Ratio_Actual"].Name == "Ratio Actual")
                //{
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    e.Handled = true;
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                e.Control.KeyPress -= new KeyPressEventHandler(dataGridView1_KeyPress);
                if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "Qty" || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "RatioActual" || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "QtyActual")
                {
                    TextBox tb = e.Control as TextBox;
                    if (tb != null)
                        tb.KeyPress += new KeyPressEventHandler(dataGridView1_KeyPress);
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tbxWeight1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    e.Handled = true;
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tbxWeight2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tbxWeight1_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tbxWeight1.Text != String.Empty)
                {
                    double d = double.Parse(tbxWeight1.Text);
                    tbxWeight1.Text = d.ToString("N4");
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tbxWeight2_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tbxWeight2.Text != String.Empty)
                {
                    double d = double.Parse(tbxWeight2.Text);
                    tbxWeight2.Text = d.ToString("N4");
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //try
            //{
            if (tbxRONum.Text == "" || tbxRONum.Text == null)
            {
                MessageBox.Show("Pilih RO terlebih dahulu.");
                return;
            }
            PopUpRONT f = new PopUpRONT();
            f.RONumber = tbxRONum.Text;
            f.GRNumber = tbxGRNum.Text;
            f.Mode = Mode;
            f.ShowDialog();

            dataGridView1.AllowUserToAddRows = false;
            if (PopUpRONT.FullItemID.Count >= 1)
            {
                //flag = '\0';
                //for (int i = 0; i < dataGridView1.RowCount; i++)
                //{
                //    for (int j = 0; j < PopUpRO.FullItemID.Count; j++)
                //    {
                //        if (dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString() == PopUpRO.FullItemID[j])
                //        {
                //            flag = 'X';
                //            break;
                //        }
                //    }
                //    if (flag == 'X')
                //    {
                //        MetroFramework.MetroMessageBox.Show(this, "Cannot select items that are already in the gridview", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        break;
                //    }
                //}
                //if (/*flag != 'X' || */dataGridView1.RowCount == 0)
                //{
                for (int i = 0; i < PopUpRONT.FullItemID.Count; i++)
                {
                    Conn = ConnectionString.GetConnection();
                    //Query = "select [SeqNo] 'No', [TransferNo] 'RO ID', [GroupId], [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [Qty], [Unit], [Ratio], [InventSiteID] from ReceiptOrderD where [TransferNo] = '" + tbxRONum.Text + "' and [FullItemId] = '" + PopUpRO.FullItemID[i] + "'";
                    Query = "select [FullItemID], [ItemDeskripsi], [UoMAlt] from [dbo].[InventTable] where [FullItemID] = '" + PopUpRONT.FullItemID[i] + "'; ";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        if (dataGridView1.ColumnCount == 11)
                            this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"], "", Dr["UoMAlt"], "", "", "", "", "", "");
                        else //(dataGridView1.ColumnCount == 15)
                        {
                            //DataTable dataTable = (DataTable)dataGridView1.DataSource;
                            //DataRow drToAdd = dataTable.NewRow();
                            //drToAdd[0] = (dataGridView1.Rows.Count + 1).ToString();
                            //drToAdd["FullItemId"] = Dr["FullItemID"];
                            //drToAdd[2] = Dr["ItemDeskripsi"];//drToAdd[2] = Dr["ItemName"];
                            //drToAdd[3] = Decimal.Parse("0");  //drToAdd[3] = Dr["Qty"];
                            //drToAdd[4] = Dr["UoMAlt"]; //drToAdd[4] = Dr["Unit"];
                            //drToAdd[5] = "";
                            //drToAdd[6] = "";
                            //drToAdd[7] = "";
                            ////drToAdd[8] = Decimal.Parse("0");
                            ////drToAdd[9] = "";
                            ////drToAdd[10] = Decimal.Parse("0");
                            ////drToAdd[11] = "";

                            //dataTable.Rows.Add(drToAdd);
                            //dataTable.AcceptChanges();

                            this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"], "", Dr["UoMAlt"], "", "", "", "");
                        }

                        Conn = ConnectionString.GetConnection();
                        Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedNTH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                        if (Mode == "New")
                        {
                            cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD'");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;

                            cellValue("select distinct InventSiteBlokID from [InventSiteBlok]; ");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"] = cell;
                        }
                        else if (Cmd.ExecuteScalar().ToString() == "01" || Cmd.ExecuteScalar().ToString() == "02")
                        {
                            cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Quality"] = cell;

                            cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD'");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;

                            cellValue("select distinct InventSiteBlokID from [InventSiteBlok]; ");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"] = cell;

                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"].ReadOnly = false;
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"].ReadOnly = false;
                        }
                    }
                    Dr.Close();
                }
            }
            //}
            //}
            //catch (Exception ex)
            //{
            //    MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string GRId = tbxGRNum.Text;

                if (rbTicket.Checked == true)
                {
                    PreviewGRNT f = new PreviewGRNT(GRId, "BM");
                    f.Show();
                }
                else if (rbTT.Checked == true)
                {
                    PreviewGRNT f = new PreviewGRNT(GRId, "TT");
                    f.Show();
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && e.RowIndex > -1)
                {
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "VendId")
                    {
                        String TotalVendor = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                        String[] VendorSatuan = TotalVendor.Split(';');

                        PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();

                        //if (!CheckForm(PopUpVendor))//jika tidak ada
                        //{
                        for (int i = 0; i < VendorSatuan.Count(); i++)
                        {
                            PopUp.Vendor.Vendor PopUpVendor1 = new PopUp.Vendor.Vendor();

                            PopUpVendor1.GetData(VendorSatuan[i].ToString());
                            PopUpVendor1.Y += 100 * i;
                            PopUpVendor1.Show();
                        }
                        //}

                    }
                    if (dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dataGridView1.Columns[e.ColumnIndex].Name.ToString() == "FullItemID")
                    {
                        PopUp.Stock.Stock PopUpStock = new PopUp.Stock.Stock();
                        //if (!CheckForm(PopUpStock))
                        //{
                        PopUpStock.GetData(dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                        PopUpStock.Show();

                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, "Error 404: " + ex.Message, "System Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            HeaderResizeGRNT f = new HeaderResizeGRNT();
            f.GRID = tbxGRNum.Text;
            f.SetMode("New", "");
            f.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSite";
            //string Where = "And (TransStatus = '21' or TransStatus = '13' or TransStatus = '14')";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);//, Where);
            tmpSearch.ShowDialog();
            txtInventSiteID.Text = ConnectionString.Kode;
            txtWarehouse.Text = ConnectionString.Kode2;
            txtLocation.Text = ConnectionString.Kode3;
        }
    }
}
