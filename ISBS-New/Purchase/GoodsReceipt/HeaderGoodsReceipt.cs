using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

//BY: HC
namespace ISBS_New.Purchase.GoodsReceipt
{
    public partial class HeaderGoodsReceipt : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
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

        /*********VALIDATION*********/
        bool validate;
        Label[] label;
        char flag;
        int count; //label
        /*********VALIDATION*********/

        /*********GV COMBO BOX*********/
        DataGridViewComboBoxCell cell;
        private SqlDataReader Dr2;
        /*********GV COMBO BOX*********/

        private string id;

        //begin
        //created by : joshua
        //created date : 22 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void SetParent(InquiryGoodsReceipt F)
        {
            Parent = F;
        }

        public HeaderGoodsReceipt()
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

        private void HeaderGoodsReceipt_Load(object sender, EventArgs e)
        {
            try
            {
                tabPage1.AutoScroll = true;
                tabPage2.AutoScroll = true;
                tabPage1.Text = "Detail GR";
                tabPage2.Text = "Detail Reference";
                if (Mode != "New")
                    GetDataHeader();

                if (Mode == "New")
                {
                    ModeNew();
                    cbRef.SelectedIndex = 0;
                    btnNew.Enabled = false;
                    btnDelete.Enabled = false;
                }
                else if (Mode == "Edit")
                    ModeEdit();
                else if (Mode == "BeforeEdit")
                    ModeBeforeEdit();
                rbTicket.Checked = true;
                Parent.RefreshGrid();

                if (Mode != "New")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
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
            //btnSRefID.Enabled = false;
            dtGR.Value = DateTime.Now;
            dtWeight2.Value = DateTime.Now;
            btnEdit.Enabled = false; btnCancel.Enabled = false; btnPrint.Enabled = false;
            rbTicket.Enabled = false; rbTT.Enabled = false;
            tbxRefID.Enabled = false;
            gbWeight2.Visible = false;
            dtWeight1.Value = DateTime.Now;
            dtDO.Value = DateTime.Now;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.DefaultCellStyle.BackColor = Color.LightGray;
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns[i].ReadOnly = true;
            }
            if (dataGridView2.ColumnCount != 0)
            {
                dataGridView2.Columns["Qty"].HeaderText = "Ref Qty";
                dataGridView2.Columns["RemainingQty"].HeaderText = "Ref Remaining Qty";
                dataGridView2.Columns["Ratio"].HeaderText = "Ref Ratio";
            }
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
                if (ControlMgr.GroupName == "Sales Manager") //WB
                {
                    //MAIN
                    dtDO.Enabled = true; tbxDelivNum.Enabled = true;
                    btnSRefID.Enabled = false; btnSOwner.Enabled = true;
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
                    if (cbWeight1.Checked == true)
                    {
                        tbxWeight1.Enabled = true;
                        btnWeight1.Enabled = false;
                    }
                    else
                    {
                        tbxWeight1.Enabled = true;
                        btnWeight1.Enabled = false;
                    }
                    cbWeight1.Enabled = true; //btnWeight1.Enabled = true;
                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbTT.Enabled = false;
                    btnEdit.Enabled = false; btnSave.Enabled = true; btnCancel.Enabled = true;
                }
                else if (ControlMgr.GroupName == "Purchase Manager") //KERANI
                {
                    //MAIN
                    tbxVNumber.Enabled = false; tbxVType.Enabled = false; tbxDriverName.Enabled = false;
                    dtDO.Enabled = false; tbxDelivNum.Enabled = false; btnSOwner.Enabled = false;
                    tbxNotes.ReadOnly = true; tbxNotes.ScrollBars = ScrollBars.Vertical;
                    //dtExpectedDate.Enabled = true;
                    dataGridView1.ReadOnly = false;
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        if (dataGridView1.Columns[i].Name == "QtyActual" || dataGridView1.Columns[i].Name == "RatioActual" || dataGridView1.Columns[i].Name == "Quality" || dataGridView1.Columns[i].Name == "Deskripsi")
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
                if (ControlMgr.GroupName == "Sales Manager") //WB
                {
                    cbWeight2.Enabled = true; btnWeight2.Enabled = true;
                    //FOOTER
                    btnPrint.Enabled = false; rbTicket.Enabled = false; rbTT.Enabled = false;
                    btnSave.Enabled = true; btnEdit.Enabled = false; btnCancel.Enabled = true;
                }
                else if (ControlMgr.GroupName == "Purchase Manager") //kerani
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
            ////if (ControlMgr.GroupName == "PurchaseManager") //ORANG WB
            //    dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.White;
            //dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.White;
            ////if (ControlMgr.GroupName == "PurchaseManager")
            ////{
            //    dataGridView1.Columns["QtyActual"].DefaultCellStyle.BackColor = Color.White;
            //    dataGridView1.Columns["RatioActual"].DefaultCellStyle.BackColor = Color.White;
            //    dataGridView1.Columns["Quality"].DefaultCellStyle.BackColor = Color.White;
            ////}
            ////else if (ControlMgr.GroupName == "SalesManager")
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
            dataGridView1.Columns["RemainingQty"].DefaultCellStyle.BackColor = Color.White;
            dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.White;
            dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.White;
            dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.White;

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
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
                dataGridView1.Columns["RemainingQty"].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.LightGray;
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

            cbVOwner.Enabled = false;
            //tbxDelivNum.Visible = false; lDelivNum.Visible = false;
            cbRef.Enabled = false;
            tbxVNumber.Enabled = false; tbxVOwner.Enabled = false; tbxVType.Enabled = false;
            tbxDriverName.Enabled = false; tbxDelivNum.Enabled = false;
            tbxNotes.ReadOnly = true; tbxNotes.ScrollBars = ScrollBars.Vertical;
            tbxRefID.Enabled = false; dtDO.Enabled = false;
            btnSRefID.Enabled = false; btnSOwner.Enabled = false;
            //WEIGHT 1
            btnWeight1.Enabled = false; dtWeight1.Enabled = false; tbxWeight1.Enabled = false;
            cbWeight1.Enabled = false;//cbWeight1.Checked = false; 
            //WEIGHT 2
            btnWeight2.Enabled = false; dtWeight2.Enabled = false; tbxWeight2.Enabled = false;
            cbWeight2.Enabled = false;//cbWeight2.Checked = false; 

            dataGridView1.Columns[3].Visible = false; //QtyRO
            dataGridView1.Columns[4].Visible = false; //RemainingQty


            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                if (dataGridView1.Columns[i].Name.Contains("Qty"))
                    dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.LightGray;
                if (dataGridView1.Columns[i].Name.Contains("Blok"))
                    dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.LightGray;
                if (dataGridView1.Columns[i].Name.Contains("Notes"))
                    dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;
                if (dataGridView1.Columns[i].Name.Contains("Deskripsi"))
                    dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.LightGray;
                if (dataGridView1.Columns[i].Name.Contains("QtyActual"))
                    dataGridView1.Columns["QtyActual"].DefaultCellStyle.BackColor = Color.LightGray;
                if (dataGridView1.Columns[i].Name.Contains("RatioActual"))
                    dataGridView1.Columns["RatioActual"].DefaultCellStyle.BackColor = Color.LightGray;
                if (dataGridView1.Columns[i].Name.Contains("Quality"))
                    dataGridView1.Columns["Quality"].DefaultCellStyle.BackColor = Color.LightGray;
            }
            //dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.LightGray;
            //dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.LightGray;
            //dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;
            //dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.LightGray;
            //dataGridView1.Columns["QtyActual"].DefaultCellStyle.BackColor = Color.LightGray;
            //dataGridView1.Columns["RatioActual"].DefaultCellStyle.BackColor = Color.LightGray; // RATIO ACTUAL
            //dataGridView1.Columns["Quality"].DefaultCellStyle.BackColor = Color.LightGray;

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            if (Cmd.ExecuteScalar().ToString() == "01")
            {
                if (ControlMgr.GroupName == "Sales Manager") //WB
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
                else if (ControlMgr.GroupName == "Purchase Manager") //Kerani
                {
                }
            }
            else if (Cmd.ExecuteScalar().ToString() == "02")
            {
                if (ControlMgr.GroupName == "Sales Manager") //WB
                {
                    gbWeight2.Visible = true;
                }
                else if (ControlMgr.GroupName == "Purchase Manager") //Kerani
                {
                    gbWeight2.Visible = false;
                }
            }
            else if (Cmd.ExecuteScalar().ToString() == "03")
            {
                btnEdit.Enabled = false;
                btnResize.Enabled = true;
            }

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.DefaultCellStyle.BackColor = Color.LightGray;
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView2.Columns[i].ReadOnly = true;
            }
            dataGridView2.Columns["Qty"].HeaderText = "Ref Qty";
            dataGridView2.Columns["RemainingQty"].HeaderText = "Ref Remaining Qty";
            dataGridView2.Columns["Ratio"].HeaderText = "Ref Ratio";
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
                    if (ControlMgr.GroupName == "Purchase Manager")
                    {
                        if (Mode == "Edit")
                        {
                            cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                            if (Dr["Quality"] != null)
                                cell.Value = Dr["Quality"].ToString();
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Quality"] = cell;

                            cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('04', '05', '06')");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;
                        }
                    }
                    else if (ControlMgr.GroupName == "Sales Manager")
                    {
                        if (Mode == "Edit")
                        {
                            cellValue("select distinct c.InventSiteBlokID from [GoodsReceivedD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.GoodsReceivedId = '" + tbxGRNum.Text + "'; ");
                            if (Dr["InventSiteBlokID"] != null)
                                cell.Value = Dr["InventSiteBlokID"].ToString();
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"] = cell;

                            cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03')");
                            if (Dr["Deskripsi"] != null)
                                cell.Value = Dr["Deskripsi"].ToString();
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;
                        }
                    }
                }
                else if (stats == "02")
                {
                    if (ControlMgr.GroupName == "Purchase Manager")
                    {
                        if (Mode == "Edit")
                        {
                            cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                            if (Dr["Quality"] != null)
                                cell.Value = Dr["Quality"].ToString();
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Quality"] = cell;

                            cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('04', '05', '06')");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;
                        }
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
                dtExpectedDate.Value = Convert.ToDateTime(Dr["ExpectedDate"]);
                dtGR.Value = Convert.ToDateTime(Dr["GoodsReceivedDate"]);
                dtRef.Value = Convert.ToDateTime(Dr["RefTransDate"]);
                cbRef.Text = Dr["RefTransType"].ToString();
                dtDO.Value = Convert.ToDateTime(Dr["SJDate"]);
                tbxRefID.Text = Dr["RefTransID"].ToString();
                tbxNameID.Text = Dr["VendId"].ToString();
                tbxName.Text = Dr["VendorName"].ToString();
                tbxDelivNum.Text = Dr["SJNumber"].ToString();
                tbxVNumber.Text = Dr["VehicleNumber"].ToString();
                tbxVType.Text = Dr["VehicleType"].ToString();
                tbxDriverName.Text = Dr["DriverName"].ToString();
                dtWeight1.Value = Convert.ToDateTime(Dr["Timbang1Date"]);
                tbxWeight1.Text = Dr["Timbang1Weight"].ToString();
                if (Dr["Timbang2Date"].ToString() != "")
                    dtWeight2.Value = Convert.ToDateTime(Dr["Timbang2Date"]);
                tbxWeight2.Text = Dr["Timbang2Weight"].ToString();
                txtInventSiteID.Text = Dr["SiteID"].ToString();
                txtWarehouse.Text = Dr["SiteName"].ToString();
                txtLocation.Text = Dr["SiteLocation"].ToString();
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
                dataGridView1.Columns[6].Name = "RatioActual"; dataGridView1.Columns[6].HeaderText = "Berat Actual";
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
            Conn.Close();

            /*********************GV2*******************/
            string tableName = String.Empty;
            string tableId = String.Empty;

            if (cbRef.Text == "Receipt Order")
            {
                tableName = "ReceiptOrderD";
                tableId = "ReceiptOrderId";
            }
            else if (cbRef.Text == "Nota Transfer")
            {
                tableName = "NotaTransferD";
                tableId = "TransferNo";
            }
            else if (cbRef.Text == "Nota Retur Jual")
            {
                tableName = "NotaReturJual_Dtl";
                tableId = "NRJID";
            }
            dataGridView2.ColumnCount = 7;
            dataGridView2.Columns[0].Name = "No";
            dataGridView2.Columns[1].Name = "FullItemId";
            dataGridView2.Columns[2].Name = "ItemName";
            dataGridView2.Columns[3].Name = "Qty";
            dataGridView2.Columns[4].Name = "RemainingQty";
            dataGridView2.Columns[5].Name = "Unit";
            dataGridView2.Columns[6].Name = "Ratio";
            dataGridView2.Rows.Clear();
            dataGridView2.AllowUserToAddRows = false;
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select * from " + tableName + " where " + tableId + " = '" + tbxRefID.Text + "'", Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                this.dataGridView2.Rows.Add(Convert.ToInt32(dataGridView2.RowCount + 1), Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Ratio"]);
            }
            Dr.Close();
            Conn.Close();
            /*********************GV2*******************/
            //ModeBeforeEdit();
        }
        #endregion

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
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
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end          
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
                tbxWeight1.Clear();
                if (Mode != "New")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [Timbang1Weight] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    tbxWeight1.Text = Cmd.ExecuteScalar().ToString();
                    //Dr = Cmd.ExecuteReader();
                    //while (Dr.Read())
                    //{
                    //    tbxWeight1.Text = Dr["Timbang1Weight"].ToString();
                    //}
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
                tbxWeight2.Clear();
                if (Mode != "New")
                {
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [Timbang2Id],[Timbang2Weight] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        tbxWeight2.Text = Dr["Timbang2Weight"].ToString();
                    }
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            //HeaderGoodsReceipt f = new HeaderGoodsReceipt();
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

                    Conn = ConnectionString.GetConnection();
                    if (Mode == "New")
                    {
                        Query = "select count(*) from [dbo].[ReceiptOrderD] where [ReceiptOrderId] = '" + tbxRefID.Text + "'";
                        Cmd = new SqlCommand(Query, Conn);
                        int count = (Int32)Cmd.ExecuteScalar();
                        if (Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["No"].Value.ToString()) <= count)
                        {
                            flag = 'X';
                            MetroFramework.MetroMessageBox.Show(this, "Cannot delete this item!\nThis item is generated from " + tbxRefID.Text, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
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
            string SchemaName = "dbo";
            string TableName = String.Empty;
            string Where = String.Empty;
            if (cbRef.Text == "Receipt Order")
            {
                TableName = "ReceiptOrderH";
                Where = "and ReceiptOrderStatus != '03'";
            }
            else if (cbRef.Text == "Nota Transfer")
            {
                TableName = "NotaTransferH";
            }
            else if (cbRef.Text == "Nota Retur Jual")
            {
                TableName = "NotaReturJualH";
            }

            //if (cbRef.Text == "Receipt Order")
            //{
            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode != String.Empty)
            {
                tbxRefID.Text = ConnectionString.Kode;
                if (cbRef.Text == "Receipt Order")
                {
                    btnNew.Enabled = true;
                    btnDelete.Enabled = true;
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select [ReceiptOrderDate] from [dbo].[ReceiptOrderH] where [ReceiptOrderId] = '" + tbxRefID.Text + "'", Conn);
                    dtRef.Value = Convert.ToDateTime(Cmd.ExecuteScalar());

                    Cmd = new SqlCommand("select [VendId], [VendorName], [VehicleType], [VehicleNumber], [DriverName], [Notes], [InventSiteID] ,[InventSiteName],[InventSiteLocation], DeliveryDate from [dbo].[ReceiptOrderH] where [ReceiptOrderId] = '" + tbxRefID.Text + "'", Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        dtExpectedDate.Value = Convert.ToDateTime(Dr["DeliveryDate"]);
                        tbxNameID.Text = Dr["VendId"].ToString();
                        tbxName.Text = Dr["VendorName"].ToString();
                        //tbxVOwnerID.Text = Dr["VendId"].ToString();
                        tbxVOwnerID.Text = Dr["VehicleOwnerID"].ToString();
                        tbxVOwner.Text = Dr["VendorName"].ToString();
                        tbxVType.Text = Dr["VehicleType"].ToString();
                        tbxVNumber.Text = Dr["VehicleNumber"].ToString();
                        tbxDriverName.Text = Dr["DriverName"].ToString();
                        txtInventSiteID.Text = Dr["InventSiteID"].ToString();
                        txtWarehouse.Text = Dr["InventSiteName"].ToString();
                        txtLocation.Text = Dr["InventSiteLocation"].ToString();
                        tbxNotes.Text = Dr["Notes"].ToString();
                    }
                    Dr.Close();

                    dataGridView1.Rows.Clear();
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    if (dataGridView1.RowCount - 1 <= 0)
                    {
                        dataGridView1.ColumnCount = 10;
                        dataGridView1.Columns[0].Name = "No";
                        dataGridView1.Columns[1].Name = "FullItemID";
                        dataGridView1.Columns[2].Name = "ItemName";
                        dataGridView1.Columns[3].Name = "QtyRO"; dataGridView1.Columns[3].HeaderText = "Qty RO";
                        dataGridView1.Columns[4].Name = "RemainingQty"; dataGridView1.Columns[4].HeaderText = "Outstanding Qty";
                        dataGridView1.Columns[5].Name = "Qty"; dataGridView1.Columns[5].HeaderText = "Delivery Qty";
                        dataGridView1.Columns[6].Name = "Unit"; ;
                        dataGridView1.Columns[7].Name = "Blok";
                        dataGridView1.Columns[8].Name = "Deskripsi"; dataGridView1.Columns[8].HeaderText = "Action Plan";
                        dataGridView1.Columns[9].Name = "Notes";
                    }

                    Conn = ConnectionString.GetConnection();
                    Query = "select [SeqNo] 'No', [ReceiptOrderId] 'RO ID', [GroupId], [SubGroup1Id], [SubGroup2Id] ,[ItemId] ,[FullItemId], [ItemName], [Qty], [RemainingQty], [Unit], [Ratio], [InventSiteID] from ReceiptOrderD where [ReceiptOrderId] = '" + tbxRefID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["RemainingQty"], Dr["Unit"], "", "", "");
                    }
                    Dr.Close();

                    if (Mode == "Edit" || Mode == "New")
                    {
                        DataGridViewComboBoxCell combo1;
                        SqlDataReader Dr1;
                        List<string> status = new List<string>();
                        Query = "Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03')";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr1 = Cmd.ExecuteReader();
                        while (Dr1.Read())
                            status.Add(Dr1[0].ToString());

                        DataGridViewComboBoxCell blok;
                        SqlDataReader Dr3;
                        List<string> blok2 = new List<string>();
                        Query = "select distinct c.InventSiteBlokID from [ReceiptOrderD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.ReceiptOrderId = '" + tbxRefID.Text + "'; ";
                        Cmd = new SqlCommand(Query, Conn);
                        Dr3 = Cmd.ExecuteReader();
                        while (Dr3.Read())
                        {
                            blok2.Add(Dr3[0].ToString());
                        }
                        Dr.Close();
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            combo1 = new DataGridViewComboBoxCell();
                            for (int j = 0; j < status.Count; j++)
                                combo1.Items.Add(status[j]);
                            //if (dataGridView1.Rows[i].Cells["Deskripsi"].Value != null)
                            //    combo1.Value = dataGridView1.Rows[i].Cells["Deskripsi"].ToString();
                            dataGridView1.Rows[i].Cells["Deskripsi"] = combo1;

                            blok = new DataGridViewComboBoxCell();
                            for (int j = 0; j < blok2.Count; j++)
                                blok.Items.Add(blok2[j]);
                            dataGridView1.Rows[i].Cells["Blok"] = blok;
                        }
                    }
                    dataGridView1.Columns["No"].ReadOnly = true;
                    dataGridView1.Columns["FullItemId"].ReadOnly = true;
                    dataGridView1.Columns["ItemName"].ReadOnly = true;
                    dataGridView1.Columns["Unit"].ReadOnly = true;
                    dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.White;
                    dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.White;
                    dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.White;
                    dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.White;

                    dataGridView1.Columns[3].Visible = false; //QtyRO
                    dataGridView1.Columns[4].Visible = false; //RemainingQty
                    Conn.Close();

                    /*********************GV2*******************/
                    string tableName = String.Empty;
                    string tableId = String.Empty;

                    if (cbRef.Text == "Receipt Order")
                    {
                        tableName = "ReceiptOrderD";
                        tableId = "ReceiptOrderId";
                    }
                    else if (cbRef.Text == "Nota Transfer")
                    {
                        tableName = "NotaTransferD";
                        tableId = "TransferNo";
                    }
                    else if (cbRef.Text == "Nota Retur Jual")
                    {
                        tableName = "NotaReturJual_Dtl";
                        tableId = "NRJID";
                    }
                    dataGridView2.ColumnCount = 7;
                    dataGridView2.Columns[0].Name = "No";
                    dataGridView2.Columns[1].Name = "FullItemId";
                    dataGridView2.Columns[2].Name = "ItemName";
                    dataGridView2.Columns[3].Name = "Qty";
                    dataGridView2.Columns[4].Name = "RemainingQty";
                    dataGridView2.Columns[5].Name = "Unit";
                    dataGridView2.Columns[6].Name = "Ratio";

                    dataGridView2.AllowUserToAddRows = false;
                    Conn = ConnectionString.GetConnection();
                    Cmd = new SqlCommand("select * from " + tableName + " where " + tableId + " = '" + tbxRefID.Text + "'", Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        this.dataGridView2.Rows.Add(Convert.ToInt32(dataGridView2.RowCount + 1), Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Ratio"]);
                    }
                    Dr.Close();
                    Conn.Close();
                    /*********************GV2*******************/
                }
                else if (cbRef.Text == "Nota Transfer")
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select a.TransferDate, a.InventSiteFrom, a.InventSiteFromName, a.VehicleType, a.VehicleNo, a.DriverName, a.InventSiteTo, a.InventSiteToName, a.Notes, b.Lokasi from NotaTransferH as a left join InventSite as b on a.InventSiteTo = b.InventSiteID where a.TransferNo = '" + tbxRefID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        dtExpectedDate.Value = Convert.ToDateTime(Dr["TransferDate"]);
                        tbxNameID.Text = Dr["InventSiteFrom"].ToString();
                        tbxName.Text = Dr["InventSiteFromName"].ToString();
                        //tbxVOwnerID.Text = Dr["VendId"].ToString();
                        //tbxVOwner.Text = Dr["VendorName"].ToString();
                        tbxVType.Text = Dr["VehicleType"].ToString();
                        tbxVNumber.Text = Dr["VehicleNo"].ToString();
                        tbxDriverName.Text = Dr["DriverName"].ToString();
                        txtInventSiteID.Text = Dr["InventSiteTo"].ToString();
                        txtWarehouse.Text = Dr["InventSiteToName"].ToString();
                        txtLocation.Text = Dr["Lokasi"].ToString();
                        tbxNotes.Text = Dr["Notes"].ToString();
                    }
                    Dr.Close();
                    Conn.Close();

                    string[] tableCols = new string[] { "No", "FullItemID", "ItemName", "QtyRO", "RemainingQty", "Qty", "Unit", "Blok", "Deskripsi", "Notes" };

                    dataGridView1.Rows.Clear();
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    if (dataGridView1.RowCount - 1 <= 0)
                    {
                        dataGridView1.ColumnCount = tableCols.Length;
                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            dataGridView1.Columns[i].Name = tableCols[i];
                        }
                    }

                    //Conn = ConnectionString.GetConnection();
                    //Query = "select [SeqNo] 'No',[FullItemId], [ItemName], [Qty], [RemainingQty], [Unit], [Ratio], [InventSiteID] from ReceiptOrderD where [ReceiptOrderId] = '" + tbxRefID.Text + "'";
                    //Cmd = new SqlCommand(Query, Conn);
                    //Dr = Cmd.ExecuteReader();
                    //while (Dr.Read())
                    //{
                    //    this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemId"], Dr["ItemName"], Dr["Qty"], Dr["RemainingQty"], Dr["RemainingQty"], Dr["Unit"], "", "", "");
                    //}
                    //Dr.Close();

                    //if (Mode == "Edit" || Mode == "New")
                    //{
                    //    DataGridViewComboBoxCell combo1;
                    //    SqlDataReader Dr1;
                    //    List<string> status = new List<string>();
                    //    Query = "Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD' and StatusCode in ('01', '02', '03')";
                    //    Cmd = new SqlCommand(Query, Conn);
                    //    Dr1 = Cmd.ExecuteReader();
                    //    while (Dr1.Read())
                    //        status.Add(Dr1[0].ToString());

                    //    DataGridViewComboBoxCell blok;
                    //    SqlDataReader Dr3;
                    //    List<string> blok2 = new List<string>();
                    //    Query = "select distinct c.InventSiteBlokID from [ReceiptOrderD] as a left join [InventSite] as b on a.InventSiteId = b.InventSiteID left join [InventSiteBlok] as c on b.InventSiteID = c.InventSiteID where a.ReceiptOrderId = '" + tbxRefID.Text + "'; ";
                    //    Cmd = new SqlCommand(Query, Conn);
                    //    Dr3 = Cmd.ExecuteReader();
                    //    while (Dr3.Read())
                    //    {
                    //        blok2.Add(Dr3[0].ToString());
                    //    }
                    //    Dr.Close();
                    //    for (int i = 0; i < dataGridView1.RowCount; i++)
                    //    {
                    //        combo1 = new DataGridViewComboBoxCell();
                    //        for (int j = 0; j < status.Count; j++)
                    //            combo1.Items.Add(status[j]);
                    //        //if (dataGridView1.Rows[i].Cells["Deskripsi"].Value != null)
                    //        //    combo1.Value = dataGridView1.Rows[i].Cells["Deskripsi"].ToString();
                    //        dataGridView1.Rows[i].Cells["Deskripsi"] = combo1;

                    //        blok = new DataGridViewComboBoxCell();
                    //        for (int j = 0; j < blok2.Count; j++)
                    //            blok.Items.Add(blok2[j]);
                    //        dataGridView1.Rows[i].Cells["Blok"] = blok;
                    //    }
                    //}
                    //dataGridView1.Columns["No"].ReadOnly = true;
                    //dataGridView1.Columns["FullItemId"].ReadOnly = true;
                    //dataGridView1.Columns["ItemName"].ReadOnly = true;
                    //dataGridView1.Columns["Unit"].ReadOnly = true;
                    //dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.White;
                    //dataGridView1.Columns["Blok"].DefaultCellStyle.BackColor = Color.White;
                    //dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.White;
                    //dataGridView1.Columns["Deskripsi"].DefaultCellStyle.BackColor = Color.White;

                    //dataGridView1.Columns[3].Visible = false; //QtyRO
                    //dataGridView1.Columns[4].Visible = false; //RemainingQty
                    Conn.Close();
                }
                else if (cbRef.Text == "Nota Retur Jual")
                {

                }
                ModeNew();
            }
            //}
            //else
            //    MessageBox.Show(cbRef.Text + " feature coming soon!", "Information");
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
                label[count].Location = new System.Drawing.Point(x - 9, y);
                label[count].BringToFront();
                if (group == "Weight1")
                    this.gbWeight1.Controls.Add(label[count]);
                else if (group == "Weight2")
                    this.gbWeight2.Controls.Add(label[count]);
                else
                    this.gbMain.Controls.Add(label[count]);
                label[count].Visible = true;
                flag = 'X';
            }
            count++;
        }
        #endregion

        #region Validation
        private char Validation()
        {
            flag = '\0';
            string msg = "";
            if (validate == false)
            {
                label = new Label[20];
            }
            //count = 0;

            //MAIN
            if (cbRef.Text == "Select") { createLabel(lRefType.Location.X, lRefType.Location.Y, "Main", ""); flag = 'X'; }
            else { createLabel(lRefType.Location.X, lRefType.Location.Y, "Main", "OK"); }

            if (tbxRefID.Text == String.Empty) { createLabel(lRefID.Location.X, lRefID.Location.Y, "Main", ""); flag = 'X'; }
            else { createLabel(lRefID.Location.X, lRefID.Location.Y, "Main", "OK"); }

            if (tbxDelivNum.Text == String.Empty) { createLabel(lDelivNum.Location.X, lDelivNum.Location.Y, "Main", ""); flag = 'X'; }
            else { createLabel(lDelivNum.Location.X, lDelivNum.Location.Y, "Main", "OK"); }

            if (tbxVOwner.Text == String.Empty || tbxVOwnerID.Text == String.Empty) { createLabel(lVOwner.Location.X, lVOwner.Location.Y, "Main", ""); flag = 'X'; }
            else { createLabel(lVOwner.Location.X, lVOwner.Location.Y, "Main", "OK"); }

            if (tbxVType.Text == String.Empty) { createLabel(lVType.Location.X, lVType.Location.Y, "Main", ""); flag = 'X'; }
            else { createLabel(lVType.Location.X, lVType.Location.Y, "Main", "OK"); }

            if (tbxVNumber.Text == String.Empty) { createLabel(lVNum.Location.X, lVNum.Location.Y, "Main", ""); flag = 'X'; }
            else { createLabel(lVNum.Location.X, lVNum.Location.Y, "Main", "OK"); }

            if (tbxDriverName.Text == String.Empty) { createLabel(lDriverName.Location.X, lDriverName.Location.Y, "Main", ""); flag = 'X'; }
            else { createLabel(lDriverName.Location.X, lDriverName.Location.Y, "Main", "OK"); }

            if (tbxWeight1.Text == String.Empty) { createLabel(lWeight1.Location.X, lWeight1.Location.Y, "Weight1", ""); flag = 'X'; }
            else { createLabel(lWeight1.Location.X, lWeight1.Location.Y, "Weight1", "OK"); }

            if (Mode != "New")
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                if (Cmd.ExecuteScalar().ToString() == "02" && ControlMgr.GroupName == "Sales Manager")
                {
                    if (tbxWeight2.Text == String.Empty)
                    {
                        createLabel(lWeight2.Location.X, lWeight2.Location.Y, "Weight2", "");
                        flag = 'X';
                    }
                    else
                    {
                        createLabel(lWeight2.Location.X, lWeight2.Location.Y, "Weight2", "OK");
                        decimal GRatioActual = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            if (dataGridView1.Columns[i].Name == "RatioActual")
                            {
                                GRatioActual += Convert.ToDecimal(dataGridView1.Rows[i].Cells["RatioActual"].Value);
                            }
                        }
                        decimal nett = Decimal.Parse(tbxWeight1.Text) - Decimal.Parse(tbxWeight2.Text);
                        if (/*(nett * (1 + 0.01))*/ (nett + (nett * 1 / 100)) < GRatioActual && /*(nett * (1 - 0.01))*/ (nett - (nett * 1 / 100)) > GRatioActual)
                        { }
                        else
                        {
                            msg += "Weight 2 cannot exceed ±1% of total item weight!";
                        }
                    }
                }
            }

            if (flag == 'X')
                msg += "Field with * is required!\r\n";

            //if (flag != 'X')
            //{
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
            if (Mode != "New" && flag != 'X')
            {
                if (Cmd.ExecuteScalar().ToString() == "01")
                {
                    if (ControlMgr.GroupName == "Sales Manager") //WB
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
                    else if (ControlMgr.GroupName == "Purchase Manager")
                    {
                        for (int j = 0; j < dataGridView1.RowCount; j++)
                        {
                            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                            {
                                if (dataGridView1.Columns[i].Name == "QtyActual" || dataGridView1.Columns[i].Name == "RatioActual" || dataGridView1.Columns[i].Name == "Quality" || dataGridView1.Columns[i].Name == "Deskripsi")
                                {
                                    if (string.IsNullOrEmpty(dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value as string) || dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value.ToString() == "0.00" || dataGridView1.Rows[j].Cells[dataGridView1.Columns[i].Index].Value.ToString() == "0.0000")
                                    {
                                        msg += "Column " + dataGridView1.Columns[i].HeaderText + " cannot blank!\n";
                                        flag = 'X';
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
                    if (ControlMgr.GroupName == "Sales Manager") //WB
                    {

                    }
                    else if (ControlMgr.GroupName == "Purchase Manager")
                    {
                        //BY: HC SUDAH ADA PENGECEKAN DIATAS
                        //if (tbxWeight2.Text == String.Empty) { createLabel(lWeight2.Location.X, lWeight2.Location.Y, "Weight2", ""); }
                        //else { createLabel(lWeight2.Location.X, lWeight2.Location.Y, "Weight2", "OK"); }

                    }
                }
                if (Cmd.ExecuteScalar().ToString() == "03")
                {
                }
            }
            //}
            else //MODE NEW
            {
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
                Cmd = new SqlCommand("select [ItemName], [FullItemId], [RemainingQty] from [dbo].[ReceiptOrderD] where [ReceiptOrderId] = '" + tbxRefID.Text + "'", Conn);
                Dr = Cmd.ExecuteReader();
                int index = 0;
                while (Dr.Read())
                {
                    //check total qty of item (from RO) cannot more than RO remaining qty
                    decimal qty = 0;
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (Dr["FullItemId"].ToString() == dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString())
                        {
                            qty = Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) + qty;
                        }
                    }
                    if (Convert.ToDecimal(Dr["RemainingQty"]) < qty)
                        msg += "Total quantity of item " + Dr["FullItemId"].ToString() + " cannot more than " + Dr["RemainingQty"].ToString() + "!\r\n";

                    if (Convert.ToDecimal(Dr["RemainingQty"]) < Convert.ToDecimal(dataGridView1.Rows[index].Cells["Qty"].Value.ToString()))
                    {
                        msg += "Row " + Convert.ToInt32(index + 1) + " " + Dr["ItemName"] + " (" + Dr["FullItemId"] + ")" + " cannot more than " + Dr["RemainingQty"];
                        //flag = 'X';
                    }
                    index++;
                }
                Dr.Close();
            }
            if (msg != String.Empty)
            {
                MetroFramework.MetroMessageBox.Show(this, msg, "Fill the form!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                flag = 'X';
            }
            //else if (flag == 'X')
            //    MetroFramework.MetroMessageBox.Show(this, "Field with * is required!", "Fill the form!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            count = 0;
            validate = true;
            return flag;
        }
        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Validation() != 'X')
                {
                    Conn = ConnectionString.GetConnection();
                    //SAVE MODE EDIT
                    if (Mode == "Edit")
                    {
                        DateTime SJDate = dtDO.Value;//.ToString("yyyy-MM-dd");
                        DateTime ExpectedDate = dtExpectedDate.Value;
                        string VendId = tbxVOwnerID.Text;
                        string VendorName = tbxVOwner.Text;
                        string VehicleType = tbxVType.Text;
                        string VehicleNumber = tbxVNumber.Text;
                        string DriverName = tbxDriverName.Text;
                        DateTime Timbang1Date = dtWeight1.Value;
                        decimal Timbang1Weight = Convert.ToDecimal(tbxWeight1.Text);
                        DateTime Timbang2Date = dtWeight2.Value;
                        string SiteID = txtInventSiteID.Text.Trim();
                        string SiteName = txtWarehouse.Text.Trim();
                        string SiteLocation = txtLocation.Text.Trim();
                        string Notes = tbxNotes.Text;
                        string UpdatedDate = "getdate()";
                        string UpdatedBy = ControlMgr.GroupName;

                        Query = "update [dbo].[GoodsReceivedH] set [SJDate] = '" + SJDate + "', [ExpectedDate] = '" + ExpectedDate + "', [VendId] = '" + VendId + "', [VendorName] = '" + VendorName + "', [VehicleType] = '" + VehicleType + "', [VehicleNumber] = '" + VehicleNumber + "', [DriverName] = '" + DriverName + "', [Timbang1Date] = '" + Timbang1Date + "', [Timbang1Weight] = '" + Timbang1Weight + "', [Timbang2Date] = '" + Timbang2Date + "', [SiteID] = '" + SiteID + "', [SiteName] = '" + SiteName + "', [SiteLocation] = '" + SiteLocation + "' ";
                        if (tbxWeight2.Text != String.Empty)
                            Query += ", [Timbang2Weight] = '" + Convert.ToDecimal(tbxWeight2.Text) + "' ";
                        Query += ", [Notes] = '" + Notes + "', [UpdatedDate] = " + UpdatedDate + ", [UpdatedBy] = '" + UpdatedBy + "' ";

                        Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                        if (Cmd.ExecuteScalar().ToString() == "02" && ControlMgr.GroupName == "Sales Manager")
                        {
                            Query += ", [GoodsReceivedStatus] = '03' ";
                            if (cbWeight2.Checked == true)
                                Query += ", [StatusWeight2] = 'Manual' ";
                            else
                                Query += ", [StatusWeight2] = 'Mesin' ";
                        }
                        else if (Cmd.ExecuteScalar().ToString() == "01" && ControlMgr.GroupName == "Purchase Manager")
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
                                Query += "[UpdatedBy] = '" + ControlMgr.GroupName + "' ";
                                Query += "where [GoodsReceivedId] = '" + tbxGRNum.Text + "' ";
                                Query += "and [FullItemId] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "' ";
                                Query += "and [GoodsReceivedSeqNo] = '" + Convert.ToInt32(i + 1) + "'; ";
                            }
                            else
                            {
                                Cmd = new SqlCommand("select distinct [InventSiteID] from [dbo].[InventSiteBlok] where [InventSiteBlokID] = '" + dataGridView1.Rows[i].Cells["Blok"].Value.ToString() + "'; ", Conn);
                                string inventID = Cmd.ExecuteScalar().ToString();

                                Query += "insert into [dbo].[GoodsReceivedD] ( [GoodsReceivedDate], [GoodsReceivedId], [GoodsReceivedSeqNo],[GroupId] ,[SubGroup1Id] ,[SubGroup2Id] ,[ItemId], [FullItemId], [ItemName], [Qty], [Qty_SJ], [Unit],[Ratio],[TotalBerat], [InventSiteBlokID], [ActionCodeStatus], [Notes], [Qty_Actual], [Ratio_Actual], [InventSiteId], [Quality], [CreatedDate], [CreatedBy] ) values ( getdate(), '" + tbxGRNum.Text + "', '" + Convert.ToInt32(i + 1) + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString().Split('.')[0] + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString().Split('.')[1] + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString().Split('.')[2] + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString().Split('.')[3] + "', '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value + "', '0', '";
                                if (dataGridView1.Rows[i].Cells["Qty"].Value.ToString() == String.Empty)
                                    Query += "0',";
                                else
                                    Query += dataGridView1.Rows[i].Cells["Qty"].Value + "', ";
                                Query += "'" + dataGridView1.Rows[i].Cells["Unit"].Value + "', '" + dataGridView1.Rows[i].Cells["Blok"].Value + "', '" + statCode + "', '" + dataGridView1.Rows[i].Cells["Notes"].Value + "', '0', '0', '" + inventID + "', '" + dataGridView1.Rows[i].Cells["Quality"].Value + "', getdate(), '" + ControlMgr.GroupName + "' ); ";
                            }
                        }

                        Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                        if (Cmd.ExecuteScalar().ToString() == "02" && ControlMgr.GroupName == "Sales Manager")
                        {
                            //UPDATE REMAINING QTY 
                            int remainingQty = 0;
                            Cmd = new SqlCommand("select a.RefTransID, a.RefTransSeqNo, a.Qty_Actual, b.RemainingQty from [dbo].[GoodsReceivedD] as a left join ReceiptOrderD as b on a.RefTransID = b.ReceiptOrderId where a.RefTransID = '" + tbxRefID.Text + "' and a.GoodsReceivedId = '" + tbxGRNum.Text + "'", Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                remainingQty = Convert.ToInt32(Dr["RemainingQty"]) - Convert.ToInt32(Dr["Qty_Actual"]);
                                Query += "update [dbo].[ReceiptOrderD] set [RemainingQty] = '" + remainingQty + "' where [ReceiptOrderId] = '" + Dr["RefTransID"] + "' and [SeqNo] = '" + Dr["RefTransSeqNo"] + "'";
                            }
                            Dr.Close();
                            //CHANGE RO HEADER STATS
                            Cmd = new SqlCommand("select [Qty] ,[Qty_SJ] ,[Qty_Actual] from [dbo].[GoodsReceivedD] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                if (Convert.ToDecimal(Dr["Qty"]) == Convert.ToDecimal(Dr["Qty_Actual"]))
                                    Query += "update [dbo].[ReceiptOrderH] set [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.GroupName + "', [ReceiptOrderStatus] ='03' where [ReceiptOrderId] = '" + tbxRefID.Text + "'; ";
                                else if (Convert.ToDecimal(Dr["Qty"]) > Convert.ToDecimal(Dr["Qty_Actual"]))
                                    Query += "update [dbo].[ReceiptOrderH] set [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.GroupName + "', [ReceiptOrderStatus] ='02' where [ReceiptOrderId] = '" + tbxRefID.Text + "'; ";
                            }
                            Dr.Close();

                            //MANAGE PURCHASE ITEM TO INVENT STOCK
                            for (int i = 0; i < dataGridView1.RowCount; i++)
                            {
                                decimal AvailableforSale_Qty_UoM = 0;
                                decimal AvailableforSale_Qty_Alt = 0;
                                decimal Ratio = 0;
                                Cmd = new SqlCommand("select * from Invent_OnHand_Qty where FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and InventSiteId = '" + txtInventSiteID.Text + "'", Conn);
                                Dr = Cmd.ExecuteReader();
                                if (Dr.HasRows)
                                {
                                    while (Dr.Read())
                                    {
                                        Ratio = Convert.ToDecimal(Dr["Ratio"]);
                                        AvailableforSale_Qty_UoM = Convert.ToDecimal(Dr["AvailableforSale_Qty_UoM"]);
                                    }
                                    AvailableforSale_Qty_UoM += Convert.ToDecimal(dataGridView1.Rows[i].Cells["QtyActual"].Value);
                                    AvailableforSale_Qty_Alt = Ratio * AvailableforSale_Qty_UoM;

                                    Cmd = new SqlCommand("update Invent_OnHand_Qty set Available_For_Sale_UoM = '" + AvailableforSale_Qty_UoM + "', Available_For_Sale_Alt = '" + AvailableforSale_Qty_Alt + "', UpdatedDate = getdate(), UpdatedBy = '" + ControlMgr.GroupName + "' where FullItemId = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "' and InventSiteId = '" + txtInventSiteID.Text + "'", Conn);
                                    Cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    Cmd = new SqlCommand("INSERT INTO Invent_OnHand_Qty ([FullItemId] ,[ItemName] ,[Ratio] ,[InventSiteId] ,[Quality] ,[Available_For_Sale_UoM] ,[Available_For_Sale_Alt] ,[Available_For_Sale_Amount] ,[Available_For_Sale_Reserved_UoM] ,[Available_For_Sale_Reserved_Alt] ,[Available_For_Sale_Reserved_Amount]) VALUES ( '" + dataGridView1.Rows[i].Cells["FullItemId"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["RatioActual"].Value.ToString() + "', '" + txtInventSiteID.Text + "', '" + dataGridView1.Rows[i].Cells["Quality"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["QtyActual"].Value.ToString() + "', '" + Convert.ToString(Convert.ToDecimal(dataGridView1.Rows[i].Cells["RatioActual"].Value) * Convert.ToDecimal(dataGridView1.Rows[i].Cells["QtyActual"].Value)) + "', 0, 0, 0, 0)", Conn);
                                    Cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        //Cmd = new SqlCommand(Query, Conn);
                        //int k = Cmd.ExecuteNonQuery();
                        //if (k == 0)
                        //{
                        //    MessageBox.Show("Not able to save detail data!");
                        //}

                        //AUTO RESIZE
                        //CHECK IF ITEM NEED RESIZE
                        Cmd = new SqlCommand("select * from GoodsReceivedH as GRH left join GoodsReceivedD as GRD on GRH.GoodsReceivedId = GRD.GoodsReceivedId where GRH.GoodsReceivedId = '" + tbxGRNum.Text + "' and GRH.GoodsReceivedStatus = '03' and GRD.FullItemId in (select distinct(RD.FullItemId) from ResizeTableH as RH left join ResizeTableD as RD on RH.TransId = RD.TransId)", Conn);
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            HeaderResizeGR f = new HeaderResizeGR();
                            string resizeID = f.GenerateID();
                            Query += "insert into [dbo].[InventResizeH] ([TransDate] ,[TransId] ,[CreatedDate] ,[CreatedBy],[Posted])values ( getdate(), '" + resizeID + "', getdate(), '" + ControlMgr.GroupName + "', 1); ";

                            int seqNoA = 1;
                            int seqNoB = 1;
                            while (Dr.Read())
                            {
                                Query += "insert into [dbo].[InventResizeD] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";
                                Query += "values ('" + resizeID + "', '" + seqNoA + "', '" + Dr["GroupId"] + "', '" + Dr["SubGroup1Id"] + "', '" + Dr["SubGroup2Id"].ToString() + "', '" + Dr["ItemId"].ToString() + "', '" + Dr["FullItemId"].ToString() + "', '" + Dr["ItemName"].ToString() + "', '" + Dr["InventSiteId"] + "', NULL, '" + Convert.ToInt32(Convert.ToInt32(Dr["Qty_Actual"]) * -1) + "', '" + Dr["Unit"] + "', '" + tbxGRNum.Text + "', '" + Dr["GoodsReceivedSeqNo"] + "', NULL, NULL, NULL, NULL, NULL, NULL, NULL, getdate(), '" + ControlMgr.GroupName + "'); ";


                                Query += "insert into [dbo].[InventResizeD] ( [TransId],[SeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteIdIssue],[InventSiteIdReceive],[Qty],[Unit],[RefTransId],[RefSeqNo],[OriginalGroupId] ,[OriginalSubGroup1Id] ,[OriginalSubGroup2Id] ,[OriginalItemId] ,[OriginalFullItemId], [OriginalItemName], [ParentSeqNo] ,[CreatedDate] ,[CreatedBy] ) ";


                                Cmd = new SqlCommand("select count(distinct(RD.ToFullItemId)) from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn);
                                int countRszItem = (Int32)Cmd.ExecuteScalar();

                                if (countRszItem == 1)
                                {
                                    Cmd = new SqlCommand("select rd.ToFullItemId from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn);
                                    string fullItemID = Cmd.ExecuteScalar().ToString();

                                    Cmd = new SqlCommand("select rd.ToItemName from ResizeTableD as RD where Rd.FullItemId = '" + Dr["FullItemId"] + "'", Conn);
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
                        }
                        Dr.Close();
                        Cmd = new SqlCommand(Query, Conn);
                        int k = Cmd.ExecuteNonQuery();
                        if (k == 0)
                        {
                            MessageBox.Show("Not able to save detail data!");
                        }
                        //hendry
                        Cmd = new SqlCommand("usp_AutoInsertNotaPurchasePark", Conn, Trans);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@GRId", tbxGRNum.Text.Trim());
                        Cmd.ExecuteNonQuery();
                        //hendry end
                        MetroFramework.MetroMessageBox.Show(this, tbxGRNum.Text + " has been successfully saved!", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ModeBeforeEdit();
                        GetDataHeader();
                        //Parent.RefreshGrid();
                    }
                    //SAVE MODE NEW
                    else if (Mode == "New")
                    {
                        //Old Code=======================================================================================
                        //string GRID = tbxGRNum.Text;
                        //End Old Code=====================================================================================

                        //begin============================================================================================
                        //updated by : joshua
                        //updated date : 13 Feb 2018
                        //description : change generate sequence number, get from global function and update counter 
                        string Jenis = "", Kode = "", GRID = "";

                        if (cbRef.Text == "Receipt Order")
                        {
                            Jenis = "GR";
                            Kode = "BBM";
                            GRID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                        }
                        else if (cbRef.Text == "Nota Transfer")
                        {
                            Jenis = "GR";
                            Kode = "BBMT";
                            GRID = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);
                        }
                        else if (cbRef.Text == "Nota Retur Jual")
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

                        DateTime GoodsReceivedDate = dtGR.Value;
                        string GoodsReceivedStatus = "01";
                        string RefTransType = cbRef.Text;
                        string RefTransID = tbxRefID.Text;
                        DateTime RefTransDate = dtRef.Value;
                        DateTime SJDate = dtDO.Value;
                        string SJNumber = tbxDelivNum.Text;
                        DateTime ExpectedDate = dtExpectedDate.Value;
                        string VendId = tbxVOwnerID.Text;
                        string VendorName = tbxVOwner.Text;
                        string VehicleType = tbxVType.Text;
                        string VehicleNumber = tbxVNumber.Text;
                        string DriverName = tbxDriverName.Text;
                        DateTime Timbang1Date = dtWeight1.Value;
                        decimal Timbang1Weight = Convert.ToDecimal(tbxWeight1.Text);
                        DateTime Timbang2Date = dtWeight2.Value;
                        string Notes = tbxNotes.Text;
                        string SiteID = txtInventSiteID.Text;
                        string SiteName = txtWarehouse.Text;
                        string SiteLocation = txtLocation.Text;
                        string CreatedDate = "getdate()";
                        string CreatedBy = ControlMgr.GroupName;
                        string StatusWeight1;
                        if (cbWeight1.Checked == true)
                            StatusWeight1 = "Manual";
                        else
                            StatusWeight1 = "Mesin";
                        Query = "insert [dbo].[GoodsReceivedH] ( [GoodsReceivedId], [GoodsReceivedDate], GoodsReceivedStatus, RefTransType, [RefTransID], [RefTransDate], [SJDate],[SJNumber], ExpectedDate, VendId, VendorName, VehicleType, VehicleNumber, DriverName, Timbang1Date, Timbang1Weight, [Timbang2Date], Notes, SiteID, SiteName, SiteLocation, CreatedDate, CreatedBy, [StatusWeight1] ) ";
                        Query += "VALUES ( '" + GRID + "', '" + GoodsReceivedDate + "', '" + GoodsReceivedStatus + "', '" + RefTransType + "', '" + RefTransID + "', '" + RefTransDate + "', '" + SJDate + "', '" + SJNumber + "', '" + ExpectedDate + "', '" + VendId + "', '" + VendorName + "', '" + VehicleType + "', '" + VehicleNumber + "', '" + DriverName + "', '" + Timbang1Date + "', '" + Timbang1Weight + "', '" + Timbang2Date + "', '" + Notes + "', '" + SiteID + "', '" + SiteName + "', '" + SiteLocation + "', " + CreatedDate + ", '" + CreatedBy + "', '" + StatusWeight1 + "' ); ";
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
                                string ROID = tbxRefID.Text;
                                int seqRO = Int32.Parse(dataGridView1.Rows[i].Cells["No"].Value.ToString());
                                try
                                {
                                    Query = "Select [Qty] from [dbo].[ReceiptOrderD] where [ReceiptOrderId] = '" + tbxRefID.Text + "' and [FullItemId] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "' and [SeqNo] = '" + dataGridView1.Rows[i].Cells["No"].Value + "'; ";
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
                                    Query = "Select [RemainingQty] from [dbo].[ReceiptOrderD] where [ReceiptOrderId] = '" + tbxRefID.Text + "' and [FullItemId] = '" + dataGridView1.Rows[i].Cells["FullItemId"].Value + "' and [SeqNo] = '" + dataGridView1.Rows[i].Cells["No"].Value + "'; ";
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

                                Query = "insert into [dbo].[GoodsReceivedD] ";
                                Query += "([GoodsReceivedDate],[GoodsReceivedId],[GoodsReceivedSeqNo],[RefTransID],[RefTransSeqNo],[GroupId],[SubGroup1Id],[SubGroup2Id],";
                                Query += "[ItemId],[FullItemId],[ItemName],[Qty],[Qty_SJ],[Qty_Actual],[Unit],[Ratio],TotalBerat,[Ratio_Actual],[InventSiteId],[InventSiteBlokID],[Quality],";
                                Query += "[Notes],[ActionCodeStatus],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[DeliveryMethod]) ";
                                Query += "values ('" + dtGR.Value.ToString("yyyy-MM-dd") + "'";
                                Query += ", '" + GRID + "'";
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
                                Query += ", '" + Convert.ToDecimal(dataGridView1.Rows[i].Cells["Qty"].Value) + "'";
                                Query += ", '0', '" + inventSite + "'";
                                Query += ", '" + dataGridView1.Rows[i].Cells["Blok"].Value + "'";
                                Query += ", NULL" /*+ dataGridView1.Rows[i].Cells["Quality"].Value +*/  + "";
                                Query += ", '" + dataGridView1.Rows[i].Cells["Notes"].Value + "'";
                                Query += ", '" + statCode + "'";
                                Query += ", getdate(), '" + ControlMgr.GroupName + "'";
                                Query += ", '1753/01/01', ''";
                                Cmd = new SqlCommand("select [DeliveryMethod] from ReceiptOrderD where ReceiptOrderId = '" + ROID + "' and SeqNo = '" + seqRO + "'", Conn);
                                if (Cmd.ExecuteScalar() != null)
                                    Query += ", '" + Cmd.ExecuteScalar().ToString() + "'); ";
                                else
                                    Query += ", NULL ); ";
                                Cmd = new SqlCommand(Query, Conn);
                                k = Cmd.ExecuteNonQuery();
                                if (k == 0)
                                    MessageBox.Show("Not able to save detail data!");
                                seq++;
                            }
                            //UPDATE RO STATUS
                            Query = "update [dbo].[ReceiptOrderH] set [ReceiptOrderStatus] = '04', [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.GroupName + "' where [ReceiptOrderId] = '" + tbxRefID.Text + "'; ";
                            Cmd = new SqlCommand(Query, Conn);
                            k = Cmd.ExecuteNonQuery();
                            if (k == 0)
                                MessageBox.Show("Not able to save detail data!");

                            MetroFramework.MetroMessageBox.Show(this, GRID + " has been successfully saved!", "Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dataGridView1.ReadOnly = true;
                            dataGridView1.Columns["Qty"].DefaultCellStyle.BackColor = Color.LightGray;
                            dataGridView1.Columns["Notes"].DefaultCellStyle.BackColor = Color.LightGray;

                            btnSave.Enabled = false; btnCancel.Enabled = false;
                            btnEdit.Enabled = true; btnPrint.Enabled = true;
                            rbTicket.Enabled = true; rbTT.Enabled = true;
                            btnNew.Enabled = false; btnDelete.Enabled = false;

                            tbxVNumber.Enabled = false; tbxVOwner.Enabled = false; tbxVType.Enabled = false;
                            tbxDriverName.Enabled = false; tbxDelivNum.Enabled = false;
                            tbxNotes.ReadOnly = true;
                            tbxRefID.Enabled = false; dtDO.Enabled = false; dtExpectedDate.Enabled = false;
                            btnSRefID.Enabled = false; btnSOwner.Enabled = false;
                            //WEIGHT 1
                            btnWeight1.Enabled = false; dtWeight1.Enabled = false; tbxWeight1.Enabled = false;
                            cbWeight1.Enabled = false;
                            //WEIGHT 2
                            btnWeight2.Enabled = false; dtWeight2.Enabled = false; tbxWeight2.Enabled = false;
                            cbWeight2.Enabled = false;
                            //InquiryGoodsReceipt F = new InquiryGoodsReceipt();
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

        //private void SavePurchAmountTable(string TmpPurchId)
        //{
        //    Query = "SELECT [PurchId] FROM [dbo].[PurchAmountTable] where PurchID = '" + TmpPurchId + "'";
        //    Cmd = new SqlCommand(Query, Conn);
        //    if (Cmd.ExecuteScalar() == null)
        //    {
        //        Query = "INSERT INTO [dbo].[PurchAmountTable] ([PurchId],[PurchDate],[TermOfPayment],[CurrencyCode],[ExchRate],[PurchAmount]";
        //        Query += ",[CreatedDate],[CreatedBy])";
        //        Query += "Values (";
        //        Query += "'" + TmpPurchId + "',";
        //        Query += "'" + dtPODate.Value.Date + "',";
        //        Query += "'" + txtToP.Text + "',";
        //        Query += "'" + txtCurrencyID.Text + "','";
        //        Query += txtExchRate.Text == "" ? "0.00','" : Decimal.Parse(txtExchRate.Text.ToString()) + "','";
        //        Query += txtTotalNett.Text == "" ? "0.00'," : Decimal.Parse(txtTotalNett.Text.ToString()) + "',";
        //        Query += "getdate(),";
        //        Query += "'" + ControlMgr.UserId + "')";
        //    }
        //    else
        //    {
        //        Query = "Update [dbo].[PurchAmountTable] Set";
        //        Query += "[PurchDate]='" + dtPODate.Value.Date + "',";
        //        Query += "[TermOfPayment]='" + txtToP.Text + "',";
        //        Query += "[CurrencyCode]='" + txtCurrencyID.Text + "',";
        //        Query += "[ExchRate]='" + txtExchRate.Text == "" ? "0.00','" : Decimal.Parse(txtExchRate.Text.ToString()) + "','";
        //        Query += "[PurchAmount]='" + txtTotalNett.Text == "" ? "0.00'," : Decimal.Parse(txtTotalNett.Text.ToString()) + "',";
        //        Query += "[CreatedDate]=getdate(),";
        //        Query += "[CreatedBy]='" + ControlMgr.UserId + "' where [PurchId]='" + TmpPurchId + "'";
        //    }
        //    Cmd = new SqlCommand(Query, Conn);
        //    Cmd.ExecuteNonQuery();
        //}

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
                if (dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name.Contains("Qty") || dataGridView1.Columns[dataGridView1.CurrentCell.ColumnIndex].Name == "RatioActual")
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
            if (tbxRefID.Text == "" || tbxRefID.Text == null)
            {
                MessageBox.Show("Pilih RO terlebih dahulu.");
                return;
            }
            PopUpRO f = new PopUpRO();
            f.RONumber = tbxRefID.Text;
            f.GRNumber = tbxGRNum.Text;
            f.Mode = Mode;
            f.ShowDialog();

            dataGridView1.AllowUserToAddRows = false;
            if (PopUpRO.FullItemID.Count >= 1)
            {
                for (int i = 0; i < PopUpRO.FullItemID.Count; i++)
                {
                    Conn = ConnectionString.GetConnection();
                    Query = "select [FullItemID], [ItemDeskripsi], [UoM] from [dbo].[InventTable] where [FullItemID] = '" + PopUpRO.FullItemID[i] + "'; ";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        if (dataGridView1.ColumnCount == 11)
                            this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"], "", Dr["UoM"], "", "", "", "", "", "");
                        else
                        {
                            this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"], "", "", "", Dr["UoM"], "", "", "", "");
                        }

                        Conn = ConnectionString.GetConnection();
                        Cmd = new SqlCommand("select [GoodsReceivedStatus] from [dbo].[GoodsReceivedH] where [GoodsReceivedId] = '" + tbxGRNum.Text + "'", Conn);
                        if (Mode == "New")
                        {
                            cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD'");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;

                            cellValue("select distinct InventSiteBlokID from [InventSiteBlok] where InventSiteID = '" + txtInventSiteID.Text + "'; ");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"] = cell;
                        }
                        else if (Cmd.ExecuteScalar().ToString() == "01" || Cmd.ExecuteScalar().ToString() == "02")
                        {
                            cellValue("Select [Deskripsi] From [dbo].[InventQuality]");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Quality"] = cell;

                            cellValue("Select Deskripsi From dbo.[TransStatusTable] where TransCode = 'GRD'");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"] = cell;

                            cellValue("select distinct InventSiteBlokID from [InventSiteBlok] where InventSiteID = '" + txtInventSiteID.Text + "'; ");
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"] = cell;

                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Deskripsi"].ReadOnly = false;
                            dataGridView1.Rows[(dataGridView1.Rows.Count - 1)].Cells["Blok"].ReadOnly = false;
                        }
                    }
                    Dr.Close();
                }
                PopUpRO.FullItemID.Clear();
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
                    //PreviewGR f = new PreviewGR(GRId, "BM");
                    //f.Show();

                    GlobalPreview f = new GlobalPreview("Goods Receipt", GRId);
                    f.SetMode("BM"); //Inquiry buka tanda terima
                    f.Show();
                }
                else if (rbTT.Checked == true)
                {
                    //PreviewGR f = new PreviewGR(GRId, "TT");
                    //f.Show();

                    GlobalPreview f = new GlobalPreview("Goods Receipt", GRId);
                    f.SetMode("TT"); //Inquiry buka tanda terima
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
            HeaderResizeGR f = new HeaderResizeGR();
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

        private void cbRef_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                if (cbRef.Text != "Select")
                    btnSRefID.Enabled = true;
                else
                    btnSRefID.Enabled = false;
            }
        }

        private void cbVOwner_CheckedChanged(object sender, EventArgs e)
        {
            if (cbVOwner.Checked == false)
            {
                tbxVOwnerID.Text = tbxNameID.Text;
                tbxVOwner.Text = tbxName.Text;
            }
            else
            {
                if (tbxRefID.Text != String.Empty && cbRef.Text != "Select")
                {
                    Conn = ConnectionString.GetConnection();
                    string tableName = String.Empty;
                    string id = String.Empty;
                    if (cbRef.Text == "Receipt Order")
                    {
                        tableName = "ReceiptOrderH";
                        id = "ReceiptOrderId";
                    }
                    else if (cbRef.Text == "Nota Transfer")
                    {
                        tableName = "NotaTransferH";
                        id = "TransferNo";
                    }
                    else if (cbRef.Text == "Nota Retur Jual")
                    {
                        tableName = "NotaReturJualH";
                        id = "NRJID";
                    }
                    Query = "select VendId, VendorName from " + tableName + " where " + id + " = '" + tbxRefID.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        tbxVOwnerID.Text = Dr["VendId"].ToString();
                        tbxVOwner.Text = Dr["VendorName"].ToString();
                    }
                    Dr.Close();
                    Conn.Close();
                }
            }
        }
    }
}
