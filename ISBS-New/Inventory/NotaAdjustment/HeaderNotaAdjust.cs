using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;

namespace ISBS_New.Inventory.NotaAdjustment
{
    public partial class HeaderNotaAdjust : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        //private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private TransactionScope scope;
        public Boolean ApproveStatus = false;

        string Mode, Query, Query1, crit = null;
        public string AdjustId = "";
        int Index;
        ComboBox ToSpec;
        ComboBox Quality;

        Inventory.NotaAdjustment.InquiryNotaAdjust Parent;

        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void SetMode(string tmpMode, string tmpAdjustId)
        {
            Mode = tmpMode;
            AdjustId = tmpAdjustId;
        }

        public HeaderNotaAdjust()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Inventory.NotaAdjustment.SearchItem F = new Inventory.NotaAdjustment.SearchItem();
            //F.flag(txtPONumber.Text);
            F.setParent(this);
            F.ShowDialog();
        }

        public void SetParent(Inventory.NotaAdjustment.InquiryNotaAdjust F)
        {
            Parent = F;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HeaderNotaAdjust_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
            cmbAddAction();
            GetDataHeader();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
            }
            else if (Mode == "ModeView")
            {
                ModeView();
            }

            ToSpec = new ComboBox();
            ToSpec.DropDownStyle = ComboBoxStyle.DropDownList;
            ToSpec.Visible = false;
            dgvNADetails.Controls.Add(ToSpec);
            ToSpec.DropDownClosed += this.ToSpec_DropDownClosed;
            ToSpec.SelectionChangeCommitted += this.ToSpec_SelectionChangeCommitted;

            Quality = new ComboBox();
            Quality.DropDownStyle = ComboBoxStyle.DropDownList;
            Quality.Visible = false;
            dgvNADetails.Controls.Add(Quality);
            Quality.DropDownClosed += this.Quality_DropDownClosed;
            Quality.SelectionChangeCommitted += this.Quality_SelectionChangeCommitted;
        }

        private void cmbAddAction()
        {
            cmbActionCode.Items.Add("Adjust Spec");
            cmbActionCode.Items.Add("Adjust Quality");
            cmbActionCode.Items.Add("Adjust Quantity");
            cmbActionCode.SelectedIndex = 0;
        }

        public void AddDataGridFromDetail(List<string> FullItemID , List<string> ItemName)
        {
            if (cmbActionCode.SelectedIndex == 0)
            {
                dgvNADetails.Rows.Clear();
                dgvNADetails.Columns.Clear();

                if (dgvNADetails.RowCount - 1 <= 0)
                {
                    dgvNADetails.ColumnCount = 13;
                    dgvNADetails.Columns[0].Name = "No";
                    dgvNADetails.Columns[1].Name = "FullItemID";
                    dgvNADetails.Columns[2].Name = "ItemName";
                    dgvNADetails.Columns[3].Name = "Spec";
                    dgvNADetails.Columns[4].Name = "QtyUoM"; dgvNADetails.Columns[4].HeaderText = "Qty UoM";
                    dgvNADetails.Columns[5].Name = "UoMUnit"; dgvNADetails.Columns[5].HeaderText = "UoM Unit";
                    dgvNADetails.Columns[6].Name = "QtyAlt"; dgvNADetails.Columns[6].HeaderText = "Qty Alt";
                    dgvNADetails.Columns[7].Name = "AltUnit"; dgvNADetails.Columns[7].HeaderText = "Alt Unit";
                    dgvNADetails.Columns[8].Name = "Ratio";
                    dgvNADetails.Columns[9].Name = "ToFullItemId"; dgvNADetails.Columns[9].HeaderText = "To FullItemID";
                    dgvNADetails.Columns[10].Name = "ToItemName"; dgvNADetails.Columns[10].HeaderText = "To Item Name";
                    dgvNADetails.Columns[11].Name = "ToSpec"; dgvNADetails.Columns[11].HeaderText = "To Spec";
                    dgvNADetails.Columns[12].Name = "Notes";
                }

                for (int i = 0; i < FullItemID.Count; i++)
                {
                    Query = "Select a.FullItemId, a.ItemDeskripsi, a.SpecID, a.UoM, a.UoMAlt, b.Ratio From [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID where a.FullItemId = '" + FullItemID[i] + "' and a.ItemDeskripsi = '"+ItemName[i]+"'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            this.dgvNADetails.Rows.Add((dgvNADetails.RowCount + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["SpecID"], "", Dr["UoM"], "", Dr["UoMAlt"], Dr["Ratio"], "", "", "", "");
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                }

                for (int i = 0; i < dgvNADetails.Columns.Count; i++)
                {
                    if (dgvNADetails.Columns[i].Name == "QtyUoM" || dgvNADetails.Columns[i].Name == "ToFullItemId" || dgvNADetails.Columns[i].Name == "Notes")
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvNADetails.Columns[i].ReadOnly = true;
                    }
                }

                dgvNADetails.AutoResizeColumns();
            }
            else if (cmbActionCode.SelectedIndex == 1)
            {
                dgvNADetails.Rows.Clear();
                dgvNADetails.Columns.Clear();

                if (dgvNADetails.RowCount - 1 <= 0)
                {
                    dgvNADetails.ColumnCount = 11;
                    dgvNADetails.Columns[0].Name = "No";
                    dgvNADetails.Columns[1].Name = "FullItemID";
                    dgvNADetails.Columns[2].Name = "ItemName";
                    dgvNADetails.Columns[3].Name = "Spec";
                    dgvNADetails.Columns[4].Name = "QtyUoM"; dgvNADetails.Columns[4].HeaderText = "Qty UoM";
                    dgvNADetails.Columns[5].Name = "UoMUnit"; dgvNADetails.Columns[5].HeaderText = "UoM Unit";
                    dgvNADetails.Columns[6].Name = "QtyAlt"; dgvNADetails.Columns[6].HeaderText = "Qty Alt";
                    dgvNADetails.Columns[7].Name = "AltUnit"; dgvNADetails.Columns[7].HeaderText = "Alt Unit";
                    dgvNADetails.Columns[8].Name = "Ratio";
                    dgvNADetails.Columns[9].Name = "Quality";
                    dgvNADetails.Columns[10].Name = "Notes";
                }

                for (int i = 0; i < FullItemID.Count; i++)
                {
                    Query = "Select a.FullItemId, a.ItemDeskripsi, a.SpecID, a.UoM, a.UoMAlt, b.Ratio From [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID where a.FullItemId = '" + FullItemID[i] + "' and a.ItemDeskripsi = '" + ItemName[i] + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            this.dgvNADetails.Rows.Add((dgvNADetails.RowCount + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["SpecID"], "", Dr["UoM"], "", Dr["UoMAlt"], Dr["Ratio"], "", "");
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                }

                for (int i = 0; i < dgvNADetails.Columns.Count;i++ )
                {
                    if (dgvNADetails.Columns[i].Name == "QtyUoM" || dgvNADetails.Columns[i].Name == "Quality" || dgvNADetails.Columns[i].Name == "Notes")
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvNADetails.Columns[i].ReadOnly = true;
                    }
                }

                dgvNADetails.AutoResizeColumns();

            }
            else if (cmbActionCode.SelectedIndex == 2)
            {
                dgvNADetails.Rows.Clear();
                dgvNADetails.Columns.Clear();

                if (dgvNADetails.RowCount - 1 <= 0)
                {
                    dgvNADetails.ColumnCount = 10;
                    dgvNADetails.Columns[0].Name = "No";
                    dgvNADetails.Columns[1].Name = "FullItemID";
                    dgvNADetails.Columns[2].Name = "ItemName";
                    dgvNADetails.Columns[3].Name = "Spec";
                    dgvNADetails.Columns[4].Name = "QtyUoM"; dgvNADetails.Columns[4].HeaderText = "Qty UoM";
                    dgvNADetails.Columns[5].Name = "UoMUnit"; dgvNADetails.Columns[5].HeaderText = "UoM Unit";
                    dgvNADetails.Columns[6].Name = "QtyAlt"; dgvNADetails.Columns[6].HeaderText = "Qty Alt";
                    dgvNADetails.Columns[7].Name = "AltUnit"; dgvNADetails.Columns[7].HeaderText = "Alt Unit";
                    dgvNADetails.Columns[8].Name = "Ratio";
                    dgvNADetails.Columns[9].Name = "Notes";
                }

                for (int i = 0; i < FullItemID.Count; i++)
                {
                    Query = "Select a.FullItemId, a.ItemDeskripsi, a.SpecID, a.UoM, a.UoMAlt, b.Ratio From [dbo].[InventTable] a LEFT JOIN [dbo].[InventConversion] b ON a.FullItemID = b.FullItemID where a.FullItemId = '" + FullItemID[i] + "' and a.ItemDeskripsi = '" + ItemName[i] + "'";

                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            this.dgvNADetails.Rows.Add((dgvNADetails.RowCount + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"], Dr["SpecID"], "", Dr["UoM"], "", Dr["UoMAlt"], Dr["Ratio"], "");
                        }
                        Dr.Close();
                    }
                    Conn.Close();
                }

                for (int i = 0; i < dgvNADetails.Columns.Count; i++)
                {
                    if (dgvNADetails.Columns[i].Name == "QtyUoM" || dgvNADetails.Columns[i].Name == "Notes")
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                    }
                    else
                    {
                        dgvNADetails.Columns[i].ReadOnly = true;
                    }
                }

                dgvNADetails.AutoResizeColumns();
            }
            
        }

        public void ModeNew()
        {
            AdjustId = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnApprove.Enabled = false;
            btnUnapprove.Enabled = false;
            btnRevision.Enabled = false;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            cmbActionCode.Enabled = true;
            btnSearchW.Enabled = true;
            btnApprove.Enabled = false;
            btnUnapprove.Enabled = false;
            btnRevision.Enabled = false;
            btnNew.Visible = true;
            btnDelete.Visible = true;
            //dgvPrDetails.ReadOnly = true;
            
            dgvNADetails.Columns["QtyUoM"].ReadOnly = false;
            if (cmbActionCode.SelectedIndex == 1)
            {dgvNADetails.Columns["Quality"].ReadOnly = false;}
            dgvNADetails.Columns["Notes"].ReadOnly = false;
            if (cmbActionCode.SelectedIndex == 0)
            { dgvNADetails.Columns["ToSpec"].ReadOnly = false; }


            dgvColumnConfig();

            //make grid not sorable
            foreach (DataGridViewColumn column in dgvNADetails.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        public void ModeApprove()
        {
            Mode = "Approve";

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            cmbActionCode.Enabled = false;
            btnSearchW.Enabled = false;

            string transStatus = "";
            Query = "SELECT [TransStatus] FROM [ISBS-NEW4].[dbo].[NotaAdjustmentH] WHERE [AdjustID] = @AdjustID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@AdjustID", txtNotaNumber.Text);
                transStatus = Cmd.ExecuteScalar().ToString();
            }
            if (transStatus == "01")
            {
                btnApprove.Enabled = true;
                btnUnapprove.Enabled = false ;
                btnRevision.Enabled = true;
            }
            else if (transStatus == "03")
            {
                btnApprove.Enabled = false;
                btnUnapprove.Enabled = true;
                btnRevision.Enabled = false;
            }
            else
            {
                btnApprove.Enabled = false;
                btnUnapprove.Enabled = false;
                btnRevision.Enabled = false;
            }

            dgvColumnConfig();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            cmbActionCode.Enabled = false;
            btnSearchW.Enabled = false;
            btnNew.Visible = false;
            btnDelete.Visible = false;

            string transStatus = "";
            Query = "SELECT [TransStatus] FROM [ISBS-NEW4].[dbo].[NotaAdjustmentH] WHERE [AdjustID] = @AdjustID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@AdjustID", txtNotaNumber.Text);
                transStatus = Cmd.ExecuteScalar().ToString();
            }
            if (transStatus == "01")
            {
                btnApprove.Enabled = true;
                btnUnapprove.Enabled = false;
                btnRevision.Enabled = true;
            }
            else if (transStatus == "03")
            {
                btnApprove.Enabled = false;
                btnUnapprove.Enabled = true;
                btnRevision.Enabled = false;
            }
            else
            {
                btnApprove.Enabled = false;
                btnUnapprove.Enabled = false;
                btnRevision.Enabled = false;
            }

            dgvColumnConfig();
        }

        public void ModeView()
        {
            Mode = "ModeView";

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnNew.Visible = false;
            btnDelete.Visible = false;

            string transStatus = "";
            Query = "SELECT [TransStatus] FROM [ISBS-NEW4].[dbo].[NotaAdjustmentH] WHERE [AdjustID] = @AdjustID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@AdjustID", txtNotaNumber.Text);
                transStatus = Cmd.ExecuteScalar().ToString();
            }
            if (transStatus == "01")
            {
                btnApprove.Enabled = true;
                btnUnapprove.Enabled = false;
                btnRevision.Enabled = true;
            }
            else if (transStatus == "03")
            {
                btnApprove.Enabled = false;
                btnUnapprove.Enabled = true;
                btnRevision.Enabled = false;
            }
            else
            {
                btnApprove.Enabled = false;
                btnUnapprove.Enabled = false;
                btnRevision.Enabled = false;
            }

            btnNew.Enabled = false;
            btnDelete.Enabled = false;

            dgvColumnConfig();
        }

        private void dgvColumnConfig()
        {
            for (int i = 0; i < dgvNADetails.Columns.Count; i++)
            {
                if (cmbActionCode.Text == "Adjust Quantity")
                {
                    if (Mode == "Edit" && (dgvNADetails.Columns[i].Name == "QtyUoM" || dgvNADetails.Columns[i].Name == "Notes"))
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else if(dgvNADetails.Columns[i].Name == "ApprovalNotes")
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvNADetails.Columns[i].ReadOnly = true;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
                else if (cmbActionCode.Text == "Adjust Spec")
                {
                    if (Mode == "Edit" && (dgvNADetails.Columns[i].Name == "QtyUoM" || dgvNADetails.Columns[i].Name == "Notes" || dgvNADetails.Columns[i].Name == "ToFullItemId"))
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else if (dgvNADetails.Columns[i].Name == "ApprovalNotes")
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvNADetails.Columns[i].ReadOnly = true;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
                else if (cmbActionCode.Text == "Adjust Quality")
                {
                    if (Mode == "Edit" && (dgvNADetails.Columns[i].Name == "QtyUoM" || dgvNADetails.Columns[i].Name == "Notes" || dgvNADetails.Columns[i].Name == "Quality"))
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else if (dgvNADetails.Columns[i].Name == "ApprovalNotes")
                    {
                        dgvNADetails.Columns[i].ReadOnly = false;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.White;
                    }
                    else
                    {
                        dgvNADetails.Columns[i].ReadOnly = true;
                        dgvNADetails.Columns[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                }
                
            }
        }

        public void GetDataHeader()
        {
            if (AdjustId == "")
            {
                AdjustId = txtNotaNumber.Text.Trim();
            }
            Conn = ConnectionString.GetConnection();

            Query = "Select [AdjustDate],[AdjustId],[ActionCode],[InventID],TransStatus,ApprovedBy,ApprovedNotes,InventSiteName From [NotaAdjustmentH] a LEFT JOIN [InventSite] b ON a.InventID = b.InventSiteID ";
            Query += "Where AdjustId = '" + AdjustId + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtNotaDate.Text = Dr["AdjustDate"].ToString();
                txtNotaNumber.Text = Dr["AdjustId"].ToString();
                txtInventSiteName.Text = Dr["InventSiteName"].ToString();
                String ActionCode = Dr["ActionCode"].ToString();
                if (ActionCode == "01")
                { 
                    cmbActionCode.Text = "Adjust Spec";
                }
                else if (ActionCode == "02")
                {
                    cmbActionCode.Text = "Adjust Quality";
                }
                else
                {
                    cmbActionCode.Text = "Adjust Quantity";
                }
                txtInventSiteID.Text = Dr["InventID"].ToString();
            }
            Dr.Close();

            if (cmbActionCode.SelectedIndex == 0)
            {
                dgvNADetails.Rows.Clear();
                dgvNADetails.Columns.Clear();
                if (dgvNADetails.RowCount - 1 <= 0)
                {
                    dgvNADetails.ColumnCount = 14;
                    dgvNADetails.Columns[0].Name = "No";
                    dgvNADetails.Columns[1].Name = "FullItemID";
                    dgvNADetails.Columns[2].Name = "ItemName";
                    dgvNADetails.Columns[3].Name = "Spec";
                    dgvNADetails.Columns[4].Name = "QtyUoM"; dgvNADetails.Columns[4].HeaderText = "Qty UoM";
                    dgvNADetails.Columns[5].Name = "UoMUnit"; dgvNADetails.Columns[5].HeaderText = "UoM Unit";
                    dgvNADetails.Columns[6].Name = "QtyAlt"; dgvNADetails.Columns[6].HeaderText = "Qty Alt";
                    dgvNADetails.Columns[7].Name = "AltUnit"; dgvNADetails.Columns[7].HeaderText = "Alt Unit";
                    dgvNADetails.Columns[8].Name = "Ratio";
                    dgvNADetails.Columns[9].Name = "ToFullItemId"; dgvNADetails.Columns[9].HeaderText = "To FullItemID";
                    dgvNADetails.Columns[10].Name = "ToItemName"; dgvNADetails.Columns[10].HeaderText = "To Item Name";
                    dgvNADetails.Columns[11].Name = "ToSpec"; dgvNADetails.Columns[11].HeaderText = "To Spec";
                    dgvNADetails.Columns[12].Name = "Notes";
                    dgvNADetails.Columns[13].Name = "ApprovalNotes";

                    if (ControlMgr.GroupName.ToUpper() != "STOCK MANAGER")
                    {
                        dgvNADetails.Columns["ApprovalNotes"].Visible = false;
                    }
                    
                }

                Query = "Select a.FullItemId, a.ItemName, Spec, Qty_UoM, Qty_Unit, Qty_Alt, ALt_Unit, b.Ratio, ToFullItemId, ToItemName, ToSpec, Notes, TransStatus, ApprovalNotes from [dbo].[NotaAdjustment_Dtl] a LEFT JOIN [dbo].[InventConversion] b ON (a.FullItemId = b.FullItemId and a.ItemName = b.ItemDeskripsi) Where AdjustId = '" + txtNotaNumber.Text + "' ";
                Conn = ConnectionString.GetConnection();

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    this.dgvNADetails.Rows.Add((dgvNADetails.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemName"], Dr["Spec"], Dr["Qty_UoM"], Dr["Qty_Unit"], Dr["Qty_Alt"], Dr["Alt_Unit"], Dr["Ratio"], Dr["ToFullItemId"], Dr["ToItemName"], Dr["ToSpec"], Dr["Notes"], Dr["ApprovalNotes"]);

                    j++;
                }
                Dr.Close();

                for (int i = 0; i < 13; i++)
                {
                    dgvNADetails.Columns[i].ReadOnly = true;
                }

                dgvNADetails.Columns["ToSpec"].Visible = false;
                dgvNADetails.AutoResizeColumns();
            }
            else if (cmbActionCode.SelectedIndex == 1)
            {
                dgvNADetails.Rows.Clear();
                dgvNADetails.Columns.Clear();
                if (dgvNADetails.RowCount - 1 <= 0)
                {
                    dgvNADetails.ColumnCount = 12;
                    dgvNADetails.Columns[0].Name = "No";
                    dgvNADetails.Columns[1].Name = "FullItemID";
                    dgvNADetails.Columns[2].Name = "ItemName";
                    dgvNADetails.Columns[3].Name = "Spec";
                    dgvNADetails.Columns[4].Name = "QtyUoM"; dgvNADetails.Columns[4].HeaderText = "Qty UoM";
                    dgvNADetails.Columns[5].Name = "UoMUnit"; dgvNADetails.Columns[5].HeaderText = "UoM Unit";
                    dgvNADetails.Columns[6].Name = "QtyAlt"; dgvNADetails.Columns[6].HeaderText = "Qty Alt";
                    dgvNADetails.Columns[7].Name = "AltUnit"; dgvNADetails.Columns[7].HeaderText = "Alt Unit";
                    dgvNADetails.Columns[8].Name = "Ratio";
                    dgvNADetails.Columns[9].Name = "Quality";
                    dgvNADetails.Columns[10].Name = "Notes";
                    dgvNADetails.Columns[11].Name = "ApprovalNotes";

                    if (ControlMgr.GroupName.ToUpper() != "STOCK MANAGER")
                    {
                        dgvNADetails.Columns["ApprovalNotes"].Visible = false;
                    }
                }

                Query = "Select a.FullItemId, ItemName, Spec, Qty_UoM, Qty_Unit, Qty_Alt, ALt_Unit, b.Ratio, Quality, Notes, TransStatus, ApprovalNotes from [dbo].[NotaAdjustment_Dtl] a LEFT JOIN [dbo].[InventConversion] b ON (a.FullItemId = b.FullItemId and a.ItemName = b.ItemDeskripsi) Where AdjustId = '" + txtNotaNumber.Text + "' ";
                Conn = ConnectionString.GetConnection();

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    this.dgvNADetails.Rows.Add((dgvNADetails.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemName"], Dr["Spec"], Dr["Qty_UoM"], Dr["Qty_Unit"], Dr["Qty_Alt"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Quality"], Dr["Notes"], Dr["ApprovalNotes"]);

                    j++;
                }
                Dr.Close();

                for (int i = 0; i < 11; i++)
                {
                    dgvNADetails.Columns[i].ReadOnly = true;
                }

                dgvNADetails.AutoResizeColumns();
            }
            else if (cmbActionCode.SelectedIndex == 2)
            {
                dgvNADetails.Rows.Clear();
                dgvNADetails.Columns.Clear();
                if (dgvNADetails.RowCount - 1 <= 0)
                {
                    dgvNADetails.ColumnCount = 11;
                    dgvNADetails.Columns[0].Name = "No";
                    dgvNADetails.Columns[1].Name = "FullItemID";
                    dgvNADetails.Columns[2].Name = "ItemName";
                    dgvNADetails.Columns[3].Name = "Spec";
                    dgvNADetails.Columns[4].Name = "QtyUoM"; dgvNADetails.Columns[4].HeaderText = "Qty UoM";
                    dgvNADetails.Columns[5].Name = "UoMUnit"; dgvNADetails.Columns[5].HeaderText = "UoM Unit";
                    dgvNADetails.Columns[6].Name = "QtyAlt"; dgvNADetails.Columns[6].HeaderText = "Qty Alt";
                    dgvNADetails.Columns[7].Name = "AltUnit"; dgvNADetails.Columns[7].HeaderText = "Alt Unit";
                    dgvNADetails.Columns[8].Name = "Ratio";
                    dgvNADetails.Columns[9].Name = "Notes";
                    dgvNADetails.Columns[10].Name = "ApprovalNotes";

                    if (ControlMgr.GroupName.ToUpper() != "STOCK MANAGER")
                    {
                        dgvNADetails.Columns["ApprovalNotes"].Visible = false;
                    }
                }

                Query = "Select a.FullItemId, ItemName, Spec, Qty_UoM, Qty_Unit, Qty_Alt, ALt_Unit, b.Ratio, Notes, TransStatus, ApprovalNotes from [dbo].[NotaAdjustment_Dtl] a LEFT JOIN [dbo].[InventConversion] b ON (a.FullItemId = b.FullItemId and a.ItemName = b.ItemDeskripsi) Where AdjustId = '" + txtNotaNumber.Text + "' ";
                Conn = ConnectionString.GetConnection();

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                int j = 0;
                while (Dr.Read())
                {
                    this.dgvNADetails.Rows.Add((dgvNADetails.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemName"], Dr["Spec"], Dr["Qty_UoM"], Dr["Qty_Unit"], Dr["Qty_Alt"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Notes"], Dr["ApprovalNotes"]);

                    j++;
                }
                Dr.Close();

                for (int i = 0; i < 10; i++)
                {
                    dgvNADetails.Columns[i].ReadOnly = true;
                }

                dgvNADetails.AutoResizeColumns();
            }
        }       

        private void btnSearchW_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSite";
            //string Where = "And (TransStatus = '21' or TransStatus = '13' or TransStatus = '14')";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);//, Where);
            tmpSearch.ShowDialog();
            txtInventSiteID.Text = ConnectionString.Kode;
            txtInventSiteName.Text = ConnectionString.Kode2;
        }

        public string getFullItemID()
        {
            string FullItemId = "";

            if (dgvNADetails.RowCount > 0)
            {
                for (int i = 0; i <= dgvNADetails.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        FullItemId += "(a.FullItemId <> '";
                        FullItemId += dgvNADetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvNADetails.Rows[i].Cells["FullItemId"].Value.ToString();
                        FullItemId += "' or a.ItemDeskripsi <> '";
                        FullItemId += dgvNADetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvNADetails.Rows[i].Cells["ItemName"].Value.ToString();
                        FullItemId += "')";
                    }
                    else
                    {
                        FullItemId += " and (a.FullItemId <> '";
                        FullItemId += dgvNADetails.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvNADetails.Rows[i].Cells["FullItemId"].Value.ToString();
                        FullItemId += "' or a.ItemDeskripsi <> '";
                        FullItemId += dgvNADetails.Rows[i].Cells["ItemName"].Value == null ? "" : dgvNADetails.Rows[i].Cells["ItemName"].Value.ToString();
                        FullItemId += "')";
                    }
                }

                return FullItemId;
            }
            else
            {
                FullItemId = "(a.FullItemId <> '' or a.ItemDeskripsi <> '')";
                return FullItemId;
            }
        }

        private void cmbActionCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                dgvNADetails.Rows.Clear();
                dgvNADetails.Columns.Clear();
                GetDataHeader();
            }
        }

        private string validation()
        {
            string msg = "";

            if (dgvNADetails.RowCount == 0)
            {
                msg += "-Jumlah item tidak boleh kosong.\n\r";
            }
            if (txtInventSiteID.Text == "")
            {
                msg += "-Warehouse tidak boleh kosong.\n\r";
            }

            SqlDataReader Dr2;        
            for (int i = 0; i < dgvNADetails.Rows.Count; i++)
            {
                string ItemId = "";
                if (dgvNADetails.Rows[i].Cells["FullItemID"].Value == null || dgvNADetails.Rows[i].Cells["FullItemID"].Value.ToString() == "")
                {
                    msg += "-Pilih ID item pada row " + (i + 1) + ".\n\r";
                }
                else
                {
                    ItemId = dgvNADetails.Rows[i].Cells["FullItemID"].Value.ToString();
                }
                string ToItemId = "";
                if (cmbActionCode.Text == "Adjust Spec")
                {
                    if (dgvNADetails.Rows[i].Cells["ToFullItemId"].Value == null || dgvNADetails.Rows[i].Cells["ToFullItemId"].Value.ToString() == "")
                    {
                        msg += "-Pilih ID item tujuan pada row " + (i + 1) + ".\n\r";
                    }
                    else
                    {
                        ToItemId = dgvNADetails.Rows[i].Cells["ToFullItemId"].Value.ToString();
                    }
                }
                decimal QtyUoM = 0;
                if (dgvNADetails.Rows[i].Cells["QtyUoM"].Value == null || dgvNADetails.Rows[i].Cells["QtyUoM"].Value.ToString() == "" || Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyUoM"].Value) == 0)
                {
                    msg += "-Masukkan nilai QtyUoM pada row " + (i + 1) + ".\n\r";
                }
                else
                {
                    QtyUoM = Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyUoM"].Value);
                }
                decimal QtyAlt = 0;
                if (dgvNADetails.Rows[i].Cells["QtyAlt"].Value == null || dgvNADetails.Rows[i].Cells["QtyAlt"].Value.ToString() == "" || Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyAlt"].Value) == 0)
                {
                    //msg += "-Nilai QtyAlt pada row " + (i + 1) + " tidak valid.\n\r";
                }
                else
                {
                    QtyAlt = Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyAlt"].Value);
                }
                string UoMUnit = "";
                if (dgvNADetails.Rows[i].Cells["UoMUnit"].Value == null || dgvNADetails.Rows[i].Cells["UoMUnit"].Value.ToString() == "" )
                {
                    msg += "-UoMUnit pada row " + (i + 1) + " kosong.\n\r";
                }
                else
                {
                    UoMUnit = dgvNADetails.Rows[i].Cells["UoMUnit"].Value.ToString();
                }
                string UoMAlt = "";
                if (dgvNADetails.Rows[i].Cells["AltUnit"].Value == null || dgvNADetails.Rows[i].Cells["AltUnit"].Value.ToString() == "")
                {
                    //msg += "-AltUnit pada row " + (i + 1) + " kosong.\n\r";
                }
                else
                {
                    UoMAlt = dgvNADetails.Rows[i].Cells["AltUnit"].Value.ToString();
                }

                if (ItemId != "" && QtyUoM != 0 && (cmbActionCode.Text == "Adjust Quantity" || cmbActionCode.Text == "Adjust Spec"))
                {
                    decimal Price = 0;
                    Query = "SELECT * FROM [dbo].[Invent_OnHand_Qty] WHERE [FullItemId] = '" + ItemId + "' AND [InventSiteId] = '" + txtInventSiteID.Text + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Dr2 = Cmd.ExecuteReader();
                        while (Dr2.Read())
                        {
                            if (Convert.ToDecimal(Dr2["Available_For_Sale_UoM"]) + (QtyUoM) < 0)
                            {
                                msg += "-Quantity Available For Sale Item " + ItemId + " pada InventSite " + txtInventSiteID.Text + " tidak mencukupi.\n\r";
                            }
                        }
                        Dr2.Close();
                    }
                }
                else if (cmbActionCode.Text == "Adjust Quality")
                {
                    if (dgvNADetails.Rows[i].Cells["Quality"].Value == null || dgvNADetails.Rows[i].Cells["Quality"].Value.ToString() == "")
                    {
                        msg += "-Quality pada row " + (i + 1) + " kosong.\n\r";
                    }
                }
            }
            return msg;
        }

        private void UpdateInventTrans( string FullItemID,int SeqNo, decimal Qty, decimal Qtyalt, decimal Qtyamount)
        {
            SqlDataReader Dr2;
            string GroupId = "";
            string SubGroupId1 = "";
            string SubGroupId2 = "";
            string ItemID = "";
            string ItemName = "";
            string FullItemId = FullItemID;
            decimal QtyUoM = Qty;
            decimal QtyAlt = Qtyalt;
            decimal QtyAmount = Qtyamount;
            string
            Query = "SELECT * FROM [dbo].[InventTable] WHERE [FullItemId] = '" + FullItemId + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr2 = Cmd.ExecuteReader();
                while (Dr2.Read())
                {
                    ItemName = Dr2["ItemDeskripsi"].ToString();
                    GroupId = Dr2["GroupID"].ToString();
                    SubGroupId1 = Dr2["SubGroup1ID"].ToString();
                    SubGroupId2 = Dr2["SubGroup2ID"].ToString();
                    ItemID = Dr2["ItemID"].ToString();
                }
                Dr2.Close();
            }
            Query = "INSERT INTO [dbo].[InventTrans]([GroupId],[SubGroupId],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteId],[TransId],[SeqNo],[TransDate],[Available_UoM],[Available_Alt],[Available_Amount],[Available_For_Sale_UoM],[Available_For_Sale_Alt],[Available_For_Sale_Amount],[Available_For_Sale_Reserved_UoM],[Available_For_Sale_Reserved_Alt],[Available_For_Sale_Reserved_Amount],[Notes]) ";
            Query += " VALUES(@GroupId,@SubGroupId,@SubGroup2Id,@ItemId,@FullItemId,@ItemName,@InventSiteId,@TransId,@SeqNo,@TransDate,@Available_UoM,@Available_Alt,@Available_Amount,@Available_For_Sale_UoM,@Available_For_Sale_Alt,@Available_For_Sale_Amount,@Available_For_Sale_Reserved_UoM,@Available_For_Sale_Reserved_Alt,@Available_For_Sale_Reserved_Amount,@Notes)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GroupId", GroupId);
                Cmd.Parameters.AddWithValue("@SubGroupId", SubGroupId1);
                Cmd.Parameters.AddWithValue("@SubGroup2Id", SubGroupId2);
                Cmd.Parameters.AddWithValue("@ItemId", ItemID);
                Cmd.Parameters.AddWithValue("@FullItemId", FullItemId);
                Cmd.Parameters.AddWithValue("@ItemName", ItemName);
                Cmd.Parameters.AddWithValue("@InventSiteId", txtInventSiteID.Text);
                Cmd.Parameters.AddWithValue("@TransId", txtNotaNumber.Text);
                Cmd.Parameters.AddWithValue("@SeqNo", SeqNo);
                Cmd.Parameters.AddWithValue("@TransDate", dtNotaDate.Value);
                Cmd.Parameters.AddWithValue("@Available_UoM", QtyUoM);
                Cmd.Parameters.AddWithValue("@Available_Alt", QtyAlt);
                Cmd.Parameters.AddWithValue("@Available_Amount", QtyAmount);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM", QtyUoM);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt", QtyAlt);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount", QtyAmount);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM", 0);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt", 0);
                Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount", 0);
                Cmd.Parameters.AddWithValue("@Notes", "");
                Cmd.ExecuteNonQuery();
            }
        }

        private void UpdateInventOnHandAndMovement(string button)
        {
            string Status = button;
            SqlDataReader Dr2;
            if (txtNotaNumber.Text != "")
            {
                //getting previous values
                List<string> OldItemId = new List<string>();
                List<decimal> OldQtyUoM = new List<decimal>();
                List<decimal> OldQtyAlt = new List<decimal>();
                List<decimal> OldPrice = new List<decimal>();
                List<int> OldSeqNo = new List<int>();
                string InventId = "";
                Query = "SELECT a.[SeqNo],a.[FullItemID],a.[ToFullItemID],a.[Qty_UoM],a.[Qty_Alt],b.[UoM_AvgPrice],c.[InventID] FROM [NotaAdjustment_Dtl] a LEFT JOIN [dbo].[InventTable] b ON a.[FullItemID] = b.[FullItemID] AND a.[Qty_Unit]=b.[UoM] AND a.[Alt_Unit]=b.[UoMAlt] LEFT JOIN [dbo].[NotaAdjustmentH] c ON a.AdjustID=c.[AdjustID] WHERE a.[AdjustID] = '" + txtNotaNumber.Text + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr2 = Cmd.ExecuteReader();
                    while (Dr2.Read())
                    {
                        OldSeqNo.Add(Convert.ToInt32(Dr2["SeqNo"]));
                        if (cmbActionCode.Text == "Adjust Spec" && Status == "Unapprove")
                        {
                            OldItemId.Add(Dr2["ToFullItemID"].ToString());
                            decimal RatioToItem = 0;
                            Query = "SELECT Ratio FROM [dbo].[InventTable] WHERE [FullItemID] = '" + Dr2["ToFullItemID"] + "' ";
                            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            {
                                RatioToItem = Cmd.ExecuteScalar() == System.DBNull.Value ? 1 : Convert.ToDecimal(Cmd.ExecuteScalar());
                            }
                            OldQtyAlt.Add((Convert.ToDecimal(Dr2["Qty_UoM"])*RatioToItem));
                        }
                        else
                        {
                            OldItemId.Add(Dr2["FullItemID"].ToString());
                            OldQtyAlt.Add(Convert.ToDecimal(Dr2["Qty_Alt"]));
                        }
                        OldQtyUoM.Add(Convert.ToDecimal(Dr2["Qty_UoM"]));
                        OldPrice.Add(Convert.ToDecimal(Dr2["UoM_AvgPrice"]));
                        InventId = Dr2["InventID"].ToString();
                    }
                    Dr2.Close();
                }
                //deleting previous amount
                for (int i = 0; i < OldItemId.Count(); i++)
                {
                    if ((Status == "Edit" ) || (Status == "Approve") )
                    {
                        Query = "UPDATE [dbo].[Invent_Movement_Qty] SET [Adjustment_In_Progress_UoM]-=" + OldQtyUoM[i] + ",[Adjustment_In_Progress_Alt]-=" + OldQtyAlt[i] + ",[Adjustment_In_Progress_Amount]-=" + (OldQtyUoM[i] * OldPrice[i]) + " WHERE [FullItemID] = '" + OldItemId[i] + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    
                    if ( Status == "Unapprove" && (cmbActionCode.Text != "Adjust Quality"))
                    {
                        Query = "UPDATE [dbo].[Invent_OnHand_Qty] SET ";
                        Query += " [Available_For_Sale_UoM]-=" + OldQtyUoM[i] + ",[Available_For_Sale_Alt]-=" + OldQtyAlt[i] + ",[Available_For_Sale_Amount]-=" + (OldQtyUoM[i] * OldPrice[i]) + ", ";
                        Query += " [Available_UoM]-=" + OldQtyUoM[i] + ",[Available_Alt]-=" + OldQtyAlt[i] + ",[Available_Amount]-=" + (OldQtyUoM[i] * OldPrice[i]) + " ";
                        Query += " WHERE [FullItemId] = '" + OldItemId[i] + "' AND [InventSiteId] = '" + InventId + "' ";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        UpdateInventTrans( OldItemId[i], OldSeqNo[i], -OldQtyUoM[i], -OldQtyAlt[i], -(OldQtyUoM[i] * OldPrice[i]));
                    }
                }
            }
            //adding new amount
            for (int i = 0; i < dgvNADetails.Rows.Count; i++)
            {
                string ItemId = dgvNADetails.Rows[i].Cells["FullItemID"].Value.ToString();
                decimal QtyUoM = Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyUoM"].Value);
                decimal QtyAlt = Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyAlt"].Value);
                decimal Price = 0;

                if (cmbActionCode.Text == "Adjust Spec" && Status == "Approve" )//&& transstatus == "Yes")
                {
                    ItemId = dgvNADetails.Rows[i].Cells["ToFullItemId"].Value.ToString();
                    decimal RatioToItem = 0;
                    Query = "SELECT Ratio FROM [ISBS-NEW4].[dbo].[InventTable] WHERE [FullItemID] = '" + ItemId + "' ";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        RatioToItem = Cmd.ExecuteScalar() == System.DBNull.Value ? 1 : Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                    QtyAlt = QtyUoM * RatioToItem;
                }

                Query = "SELECT UoM_AvgPrice FROM [ISBS-NEW4].[dbo].[InventTable] WHERE [FullItemID] = '" + ItemId + "' ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    if (Cmd.ExecuteScalar() == System.DBNull.Value)
                    {
                        Price = 1;
                    }
                    else
                    {
                        Price = Convert.ToDecimal(Cmd.ExecuteScalar());
                    }
                }
                if (Status == "New" || (Status == "Edit" ) || (Status == "Unapprove" ))
                {
                    Query = "UPDATE [dbo].[Invent_Movement_Qty] SET [Adjustment_In_Progress_UoM]+=" + QtyUoM + ",[Adjustment_In_Progress_Alt]+=" + QtyAlt + ",[Adjustment_In_Progress_Amount]+=" + (QtyUoM * Price) + " WHERE [FullItemID] = '" + ItemId + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
                if ((Status == "Approve") && (cmbActionCode.Text != "Adjust Quality"))
                {
                    bool hasrow = false;
                    using (Cmd = new SqlCommand("SELECT * FROM [dbo].[Invent_OnHand_Qty] WHERE [FullItemId] = '" + ItemId + "' AND [InventSiteId] = '" + txtInventSiteID.Text + "';", ConnectionString.GetConnection()))
                    {
                        Dr = Cmd.ExecuteReader();
                        if (Dr.HasRows)
                        {
                            hasrow = true;
                        }
                        Dr.Close();
                    }
                    if (hasrow == true)
                    {
                        Query = "UPDATE [dbo].[Invent_OnHand_Qty] SET ";
                        Query += " [Available_For_Sale_UoM]+=" + QtyUoM + ",[Available_For_Sale_Alt]+=" + QtyAlt + ",[Available_For_Sale_Amount]+=" + (QtyUoM * Price) + ", ";
                        Query += " [Available_UoM]+=" + QtyUoM + ",[Available_Alt]+=" + QtyAlt + ",[Available_Amount]+=" + (QtyUoM * Price) + " ";
                        Query += " WHERE [FullItemId] = '" + ItemId + "' AND [InventSiteId] = '" + txtInventSiteID.Text + "' ";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    else if (hasrow == false)
                    {
                        string Groupid = "";
                        string subgroupid = "";
                        string subgroupid2 ="";
                        string itemidd = "";
                        string itemname = "";
                        Query = "SELECT * FROM [dbo].[InventTable] WHERE [FullItemID]=@FullItemID;";
                        using(Cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@FullItemID",ItemId);
                            Dr = Cmd.ExecuteReader();
                            while(Dr.Read())
                            {
                                Groupid = Dr["GroupID"] == System.DBNull.Value?"":Dr["GroupID"].ToString();
                                subgroupid = Dr["SubGroup1ID"] == System.DBNull.Value ? "" : Dr["SubGroup1ID"].ToString();
                                subgroupid2 = Dr["SubGroup2ID"] == System.DBNull.Value ? "" : Dr["SubGroup2ID"].ToString();
                                itemidd = Dr["ItemID"] == System.DBNull.Value ? "" : Dr["ItemID"].ToString();
                                itemname = Dr["ItemDeskripsi"] == System.DBNull.Value ? "" : Dr["ItemDeskripsi"].ToString(); 
                            }
                            Dr.Close();
                        }

                        Query = "INSERT INTO [dbo].[Invent_OnHand_Qty]([GroupId],[SubGroupId],[SubGroup2Id],[ItemId],[FullItemId],[ItemName],[InventSiteId],[Available_UoM],[Available_Alt],[Available_Amount],[Available_For_Sale_UoM],[Available_For_Sale_Alt],[Available_For_Sale_Amount],[Available_For_Sale_Reserved_UoM],[Available_For_Sale_Reserved_Alt],[Available_For_Sale_Reserved_Amount])";
                        Query += " VALUES (@GroupId,@SubGroupId,@SubGroup2Id,@ItemId,@FullItemId,@ItemName,@InventSiteId,@Available_UoM,@Available_Alt,@Available_Amount,@Available_For_Sale_UoM,@Available_For_Sale_Alt,@Available_For_Sale_Amount,@Available_For_Sale_Reserved_UoM,@Available_For_Sale_Reserved_Alt,@Available_For_Sale_Reserved_Amount)";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@GroupId",Groupid);
                            Cmd.Parameters.AddWithValue("@SubGroupId",subgroupid);
                            Cmd.Parameters.AddWithValue("@SubGroup2Id",subgroupid2);
                            Cmd.Parameters.AddWithValue("@ItemId",itemidd);
                            Cmd.Parameters.AddWithValue("@FullItemId",ItemId);
                            Cmd.Parameters.AddWithValue("@ItemName",itemname);
                            Cmd.Parameters.AddWithValue("@InventSiteId",txtInventSiteID.Text);
                            Cmd.Parameters.AddWithValue("@Available_UoM",QtyUoM);
                            Cmd.Parameters.AddWithValue("@Available_Alt",QtyAlt);
                            Cmd.Parameters.AddWithValue("@Available_Amount",(QtyUoM * Price));
                            Cmd.Parameters.AddWithValue("@Available_For_Sale_UoM",QtyUoM);
                            Cmd.Parameters.AddWithValue("@Available_For_Sale_Alt",QtyAlt);
                            Cmd.Parameters.AddWithValue("@Available_For_Sale_Amount",(QtyUoM * Price));
                            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_UoM",0);
                            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Alt",0);
                            Cmd.Parameters.AddWithValue("@Available_For_Sale_Reserved_Amount",0);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    UpdateInventTrans(ItemId, (i + 1), QtyUoM, QtyAlt, (QtyUoM * Price));
                }
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;

            try
            {
                using (scope = new TransactionScope())
                {
                    string msg = validation();
                    if (msg != "")
                    {
                        MessageBox.Show(msg);
                        return;
                    }

                    if (Mode == "New")
                    {
                        Query = "Declare @tmp table (Id varchar (50)) ";
                        Query += "Insert into [dbo].[NotaAdjustmentH] (AdjustId,";
                        Query += "AdjustDate,";
                        Query += "ActionCode,";
                        Query += "InventID,";
                        Query += "TransStatus,";
                        Query += "CreatedDate,";
                        Query += "CreatedBy) Output (Inserted.AdjustId) into @tmp values (";
                        string NAId = "";
                        {
                            string Jenis = "NA", Kode = "NA";
                            NAId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", ConnectionString.GetConnection(), Cmd);
                        }
                        //if (cmbActionCode.SelectedIndex == 0)
                        //{
                        //    string Jenis = "NA", Kode = "NA1";
                        //    NAId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", ConnectionString.GetConnection(), Cmd);
                        //    //Query += "(select  'NA1-'+ RIGHT(cast(year(getdate()) as varchar(4)),2) + RIGHT('00' + cast(month(getdate()) as varchar(2)),2) + '-' + RIGHT('00000' + CONVERT(VARCHAR(5), case when (max(right(AdjustId,5)) is null) then 0 else max(right(AdjustId,5)) end +1), 5) FROM [NotaAdjustmentH] where AdjustID like ('%NA1-'+ RIGHT(cast(year(getdate()) as varchar(4)),2) + RIGHT('00' + cast(month(getdate()) as varchar(2)),2)+ '-%')),'";
                        //}
                        //else if (cmbActionCode.SelectedIndex == 1)
                        //{
                        //    string Jenis = "NA", Kode = "NA2";
                        //    NAId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", ConnectionString.GetConnection(), Cmd);
                        //    //Query += "(select  'NA2-'+ RIGHT(cast(year(getdate()) as varchar(4)),2) + RIGHT('00' + cast(month(getdate()) as varchar(2)),2) + '-' + RIGHT('00000' + CONVERT(VARCHAR(5), case when (max(right(AdjustId,5)) is null) then 0 else max(right(AdjustId,5)) end +1), 5) FROM [NotaAdjustmentH] where AdjustID like ('%NA2-'+ RIGHT(cast(year(getdate()) as varchar(4)),2) + RIGHT('00' + cast(month(getdate()) as varchar(2)),2)+ '-%')),'";
                        //}
                        //else if (cmbActionCode.SelectedIndex == 2)
                        //{
                        //    string Jenis = "NA", Kode = "NA3";
                        //    NAId = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", ConnectionString.GetConnection(), Cmd);
                        //    //Query += "(select  'NA3-'+ RIGHT(cast(year(getdate()) as varchar(4)),2) + RIGHT('00' + cast(month(getdate()) as varchar(2)),2) + '-' + RIGHT('00000' + CONVERT(VARCHAR(5), case when (max(right(AdjustId,5)) is null) then 0 else max(right(AdjustId,5)) end +1), 5) FROM [NotaAdjustmentH] where AdjustID like ('%NA3-'+ RIGHT(cast(year(getdate()) as varchar(4)),2) + RIGHT('00' + cast(month(getdate()) as varchar(2)),2)+ '-%')),'";
                        //}
                        Query += " '" + NAId + "',";
                        Query += " '" + dtNotaDate.Value.Date + "',";
                        if (cmbActionCode.SelectedIndex == 0)
                        {
                            Query += "'01','";
                        }
                        else if (cmbActionCode.SelectedIndex == 1)
                        {
                            Query += "'02','";
                        }
                        else if (cmbActionCode.SelectedIndex == 2)
                        {
                            Query += "'03','";
                        }
                        Query += txtInventSiteID.Text + "',";
                        Query += "'01',";
                        Query += "getdate(),";
                        Query += "@CreatedBy) select * from @tmp;";

                        using (SqlCommand Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                            AdjustId = Cmd.ExecuteScalar().ToString();
                            txtNotaNumber.Text = AdjustId;
                        }

                        UpdateInventOnHandAndMovement("New");

                        if (dgvNADetails.Rows.Count > 0)
                        {
                            for (int j = 0; j <= dgvNADetails.Rows.Count - 1; j++)
                            {
                                String FullItemId = dgvNADetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvNADetails.Rows[j].Cells["FullItemId"].Value.ToString();
                                decimal SeqNo = dgvNADetails.Rows[j].Cells["No"].Value == null ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["No"].Value.ToString());
                                String ItemName = dgvNADetails.Rows[j].Cells["ItemName"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ItemName"].Value.ToString();
                                String Spec = dgvNADetails.Rows[j].Cells["Spec"].Value == null ? "" : dgvNADetails.Rows[j].Cells["Spec"].Value.ToString();
                                decimal QtyUoM = dgvNADetails.Rows[j].Cells["QtyUoM"].Value == "" ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["QtyUoM"].Value.ToString());
                                String UoMUnit = dgvNADetails.Rows[j].Cells["UoMUnit"].Value == null ? "" : dgvNADetails.Rows[j].Cells["UoMUnit"].Value.ToString();
                                decimal QtyAlt = dgvNADetails.Rows[j].Cells["QtyAlt"].Value == "" ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["QtyAlt"].Value.ToString());
                                String AltUnit = dgvNADetails.Rows[j].Cells["AltUnit"].Value == null ? "" : dgvNADetails.Rows[j].Cells["AltUnit"].Value.ToString();
                                decimal Ratio = dgvNADetails.Rows[j].Cells["Ratio"].Value == null ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["Ratio"].Value.ToString());
                                String Notes = dgvNADetails.Rows[j].Cells["Notes"].Value == null ? "" : dgvNADetails.Rows[j].Cells["Notes"].Value.ToString().Trim();


                                if (cmbActionCode.SelectedIndex == 0)
                                {
                                    String ToFullItemId = dgvNADetails.Rows[j].Cells["ToFullItemId"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ToFullItemId"].Value.ToString();
                                    String ToItemName = dgvNADetails.Rows[j].Cells["ToItemName"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ToItemName"].Value.ToString();
                                    String ToSpec = dgvNADetails.Rows[j].Cells["ToSpec"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ToSpec"].Value.ToString();

                                    Query = "Insert into [dbo].[NotaAdjustment_Dtl] (AdjustID, SeqNo, FullItemID, ItemName, Spec, Quality, Qty_UoM, Qty_Unit, Qty_Alt, Alt_Unit, ToFullItemID, ToItemName, ToSpec, Notes, CreatedDate, CreatedBy) ";
                                    Query += "values ('" + AdjustId + "',  '" + SeqNo + "', '" + FullItemId + "', @ItemName,'" + Spec + "', '', '" + QtyUoM + "', '" + UoMUnit + "', '" + QtyAlt + "','" + AltUnit + "','" + ToFullItemId + "','" + ToItemName + "','" + ToSpec + "',@Notes ,getdate(), @CreatedBy);";
                                }
                                else if (cmbActionCode.SelectedIndex == 1)
                                {
                                    String Quality = dgvNADetails.Rows[j].Cells["Quality"].Value == null ? "" : dgvNADetails.Rows[j].Cells["Quality"].Value.ToString();

                                    Query = "Insert into [dbo].[NotaAdjustment_Dtl] (AdjustID, SeqNo, FullItemID, ItemName, Spec, Quality, Qty_UoM, Qty_Unit, Qty_Alt, Alt_Unit, ToFullItemID, ToItemName, ToSpec, Notes, CreatedDate, CreatedBy) ";
                                    Query += "values ('" + AdjustId + "',  '" + SeqNo + "', '" + FullItemId + "', @ItemName,'" + Spec + "', '" + Quality + "', '" + QtyUoM + "', '" + UoMUnit + "', '" + QtyAlt + "','" + AltUnit + "','','','',@Notes ,getdate(), @CreatedBy);";
                                }
                                else if (cmbActionCode.SelectedIndex == 2)
                                {
                                    Query = "Insert into [dbo].[NotaAdjustment_Dtl] (AdjustID, SeqNo, FullItemID, ItemName, Spec, Quality, Qty_UoM, Qty_Unit, Qty_Alt, Alt_Unit, ToFullItemID, ToItemName, ToSpec, Notes, CreatedDate, CreatedBy) ";
                                    Query += "values ('" + AdjustId + "',  '" + SeqNo + "', '" + FullItemId + "', @ItemName,'" + Spec + "', '', '" + QtyUoM + "', '" + UoMUnit + "', '" + QtyAlt + "','" + AltUnit + "','','','',@Notes,getdate(), @CreatedBy);";
                                }
                                Cmd = new SqlCommand(Query, ConnectionString.GetConnection());
                                Cmd.Parameters.AddWithValue("@ItemName", ItemName);
                                Cmd.Parameters.AddWithValue("@Notes", Notes);
                                Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                                Cmd.ExecuteNonQuery();
                            }

                            //Begin
                            //Created By : Joshua
                            //Created Date ; 24 Aug 2018
                            //Desc : Create Journal
                            CreateJournal();
                            //End

                            InsertNALog("01", "N");
                        }
                        txtNotaNumber.Text = AdjustId;
                        MessageBox.Show("Data :" + AdjustId + " Berhasil ditambahkan.");
                    }
                    else if (Mode == "Edit")
                    {
                        String TransStatus = "";

                        UpdateInventOnHandAndMovement("Edit");
                        Query = "Update [dbo].[NotaAdjustmentH] set [TransStatus] = '01', InventID = '" + txtInventSiteID.Text + "' , ";
                        if (cmbActionCode.SelectedIndex == 0)
                        {
                            Query += "ActionCode = '01', ";
                        }
                        else if (cmbActionCode.SelectedIndex == 1)
                        {
                            Query += "ActionCode = '02', ";
                        }
                        else if (cmbActionCode.SelectedIndex == 2)
                        {
                            Query += "ActionCode = '03', ";
                        }
                        Query += "UpdatedDate = getDate(), UpdatedBy = @UpdatedBy OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy where AdjustId = '" + txtNotaNumber.Text + "' ";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                            Dr = Cmd.ExecuteReader();
                            while (Dr.Read())
                            {
                                CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                                CreatedBy = Dr["CreatedBy"].ToString();
                            }
                            Dr.Close();
                        }
                        if (dgvNADetails.Rows.Count > 0)
                        {
                            Query1 = "Delete from [dbo].[NotaAdjustment_Dtl] where AdjustID = '" + txtNotaNumber.Text.Trim() + "' ";
                            using (Cmd = new SqlCommand(Query1, ConnectionString.GetConnection()))
                            {
                                Cmd.ExecuteNonQuery();
                            }
                            for (int j = 0; j <= dgvNADetails.RowCount - 1; j++)
                            {
                                String FullItemId = dgvNADetails.Rows[j].Cells["FullItemId"].Value == null ? "" : dgvNADetails.Rows[j].Cells["FullItemId"].Value.ToString();
                                decimal SeqNo = dgvNADetails.Rows[j].Cells["No"].Value == null ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["No"].Value.ToString());
                                String ItemName = dgvNADetails.Rows[j].Cells["ItemName"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ItemName"].Value.ToString();
                                String Spec = dgvNADetails.Rows[j].Cells["Spec"].Value == null ? "" : dgvNADetails.Rows[j].Cells["Spec"].Value.ToString();
                                decimal QtyUoM = dgvNADetails.Rows[j].Cells["QtyUoM"].Value == null ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["QtyUoM"].Value.ToString());
                                String UoMUnit = dgvNADetails.Rows[j].Cells["UoMUnit"].Value == null ? "" : dgvNADetails.Rows[j].Cells["UoMUnit"].Value.ToString();
                                decimal QtyAlt = dgvNADetails.Rows[j].Cells["QtyAlt"].Value == null ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["QtyAlt"].Value.ToString());
                                String AltUnit = dgvNADetails.Rows[j].Cells["AltUnit"].Value == null ? "" : dgvNADetails.Rows[j].Cells["AltUnit"].Value.ToString();
                                //decimal Ratio = dgvNADetails.Rows[j].Cells["Ratio"].Value == null ? 0 : decimal.Parse(dgvNADetails.Rows[j].Cells["Ratio"].Value.ToString());
                                string strRasio = dgvNADetails.Rows[j].Cells["Ratio"].Value.ToString();
                                decimal Ratio = 0;
                                if (strRasio != "")
                                {
                                    Ratio = decimal.Parse(dgvNADetails.Rows[j].Cells["Ratio"].Value.ToString());
                                }

                                String Notes = dgvNADetails.Rows[j].Cells["Notes"].Value == null ? "" : dgvNADetails.Rows[j].Cells["Notes"].Value.ToString().Trim();

                                if (cmbActionCode.SelectedIndex == 0)
                                {
                                    String ToFullItemId = dgvNADetails.Rows[j].Cells["ToFullItemId"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ToFullItemId"].Value.ToString();
                                    String ToItemName = dgvNADetails.Rows[j].Cells["ToItemName"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ToItemName"].Value.ToString();
                                    String ToSpec = dgvNADetails.Rows[j].Cells["ToSpec"].Value == null ? "" : dgvNADetails.Rows[j].Cells["ToSpec"].Value.ToString();

                                    Query = "Insert into [dbo].[NotaAdjustment_Dtl] (AdjustID, SeqNo, FullItemID, ItemName, Spec, Quality, Qty_UoM, Qty_Unit, Qty_Alt, Alt_Unit, ToFullItemID, ToItemName, ToSpec, Notes, CreatedDate, CreatedBy) ";
                                    Query += "values ('" + AdjustId + "',  '" + SeqNo + "', '" + FullItemId + "', @ItemName,'" + Spec + "', '', '" + QtyUoM + "', '" + UoMUnit + "', '" + QtyAlt + "','" + AltUnit + "','" + ToFullItemId + "','" + ToItemName + "','" + ToSpec + "',@Notes ,getdate(), @UpdatedBy);";
                                }
                                else if (cmbActionCode.SelectedIndex == 1)
                                {
                                    String Quality = dgvNADetails.Rows[j].Cells["Quality"].Value == null ? "" : dgvNADetails.Rows[j].Cells["Quality"].Value.ToString();

                                    Query = "Insert into [dbo].[NotaAdjustment_Dtl] (AdjustID, SeqNo, FullItemID, ItemName, Spec, Quality, Qty_UoM, Qty_Unit, Qty_Alt, Alt_Unit, ToFullItemID, ToItemName, ToSpec, Notes, CreatedDate, CreatedBy) ";
                                    Query += "values ('" + AdjustId + "',  '" + SeqNo + "', '" + FullItemId + "', @ItemName,'" + Spec + "', '" + Quality + "', '" + QtyUoM + "', '" + UoMUnit + "', '" + QtyAlt + "','" + AltUnit + "','','','',@Notes ,getdate(), @UpdatedBy);";
                                }
                                else if (cmbActionCode.SelectedIndex == 2)
                                {
                                    Query = "Insert into [dbo].[NotaAdjustment_Dtl] (AdjustID, SeqNo, FullItemID, ItemName, Spec, Quality, Qty_UoM, Qty_Unit, Qty_Alt, Alt_Unit, ToFullItemID, ToItemName, ToSpec, Notes, CreatedDate, CreatedBy) ";
                                    Query += "values ('" + AdjustId + "',  '" + SeqNo + "', '" + FullItemId + "', @ItemName,'" + Spec + "', '', '" + QtyUoM + "', '" + UoMUnit + "', '" + QtyAlt + "','" + AltUnit + "','','','',@Notes ,getdate(), @UpdatedBy);";
                                }
                                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                                {
                                    Cmd.Parameters.AddWithValue("@ItemName", ItemName);
                                    Cmd.Parameters.AddWithValue("@Notes", Notes);
                                    Cmd.Parameters.AddWithValue("@UpdatedBy", ControlMgr.UserId);
                                    Cmd.ExecuteNonQuery();
                                }
                            }

                            ReversalJournal("IN22");
                            CreateJournal();
                            InsertNALog("01", "E");
                        }
                        
                        MessageBox.Show("Data :" + AdjustId + " Berhasil diupdate.");
                    }
                    scope.Complete();
                }
                GetDataHeader();
                ModeBeforeEdit();
                Parent.RefreshGrid();
            }
            catch(Exception E)
            {
                MessageBox.Show(E.ToString());
            }
        }

        private void InsertNALog(string statuscode, string action)
        {
            string statusdesc = "";
            Query = "SELECT [Deskripsi] FROM [dbo].[TransStatusTable] WHERE [TransCode] = 'NotaAdjustment' AND [StatusCode] = @StatusCode;";
            using(Cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@StatusCode",statuscode);
                statusdesc = Cmd.ExecuteScalar() == System.DBNull.Value ? "": Cmd.ExecuteScalar().ToString();
            }

            if (txtNotaNumber.Text == "")
            {
                action = "N";
            }

            if (action == "")
            {
                Query = "SELECT TOP 1 Action FROM [dbo].[NotaAdjustment_LogTable] WHERE [NAId] = @NAId ORDER BY [LogDate] DESC";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@NAId", txtNotaNumber.Text);
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        while (Dr.Read())
                        {
                            action = Dr["Action"] == System.DBNull.Value ? "" : Dr["Action"].ToString();
                        }
                    }
                    Dr.Close();
                }
            }

            Query = "INSERT INTO [ISBS-NEW4].[dbo].[NotaAdjustment_LogTable] ([NADate],[NAId],[Type],[SiteId],[LogStatusCode],[LogStatusDesc],[Action],[UserID],[LogDate]) ";
            Query += "VALUES (@NADate,@NAId,@Type,@SiteId,@LogStatusCode,@LogStatusDesc,@Action,@UserID,getdate())";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@NADate", dtNotaDate.Value);
                Cmd.Parameters.AddWithValue("@NAId", txtNotaNumber.Text);
                Cmd.Parameters.AddWithValue("@Type", cmbActionCode.Text);
                Cmd.Parameters.AddWithValue("@SiteId", txtInventSiteID.Text);
                Cmd.Parameters.AddWithValue("@LogStatusCode", statuscode);
                Cmd.Parameters.AddWithValue("@LogStatusDesc", statusdesc);
                Cmd.Parameters.AddWithValue("@Action", action);
                Cmd.Parameters.AddWithValue("@UserID", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
            
        }

        private void CreateJournal()
        {
            //Begin
            //Created By : Joshua
            //Created Date ; 24 Aug 2018
            //Desc : Create Journal
            string ActionCode = cmbActionCode.SelectedItem.ToString();

            decimal AdjustInProgress = 0, Available = 0, GainLossSuspense = 0, AdjustSuspense = 0;
            decimal Price = 0;
            for (int i = 0; i < dgvNADetails.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNADetails.Rows[i].Cells["FullItemID"].Value);
                string QtyUoM = Convert.ToString(dgvNADetails.Rows[i].Cells["QtyUoM"].Value) == "" ? "0" : Convert.ToString(dgvNADetails.Rows[i].Cells["QtyUoM"].Value);

                Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);

                if (ActionCode != "Adjust Quantity")
                {
                    AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                }
                else if (ActionCode == "Adjust Quantity")
                {
                    if (QtyUoM.Contains("-"))
                    {
                        GainLossSuspense = GainLossSuspense + (Price * Convert.ToDecimal(QtyUoM.Substring(1)));
                        Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    }
                    else
                    {
                        AdjustSuspense = AdjustSuspense + (Price * Convert.ToDecimal(QtyUoM));
                        AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    }
                }
            }

            if (AdjustInProgress != 0 || Available != 0 || GainLossSuspense != 0 || AdjustSuspense != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "IN22";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, ConnectionString.GetConnection(), Cmd);
                string Notes = "";

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + AdjustId + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.ExecuteNonQuery();
                }
                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                        Type = Convert.ToString(Dr["Type"]);
                        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                        AmountValue = 0;

                        if (JournalHID == "IN22")
                        {
                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = AdjustInProgress;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = GainLossSuspense;
                            }
                            else if (JournalIDSeqNo == 3)
                            {
                                AmountValue = Available;
                            }
                            else if (JournalIDSeqNo == 4)
                            {
                                AmountValue = AdjustSuspense;
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
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        SeqNo++;
                    }
                    Dr.Close();
                }
            }
            //End
        }

        private void ReversalJournal(string JournalHID) //Reversal IN22
        {
            string Jenis = "JN", Kode = "JN";
            string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, ConnectionString.GetConnection(), Cmd);
            string Notes = "";

            int countseqno = 0;

            Conn = ConnectionString.GetConnection();

            Query = "SELECT * FROM [ISBS-NEW4].[dbo].[GLJournalDtl] WHERE [GLJournalHID] = (SELECT TOP 1 GLJournalHID FROM [ISBS-NEW4].[dbo].[GLJournalH] WHERE Referensi = @Referensi AND JournalHID = @JournalHID ORDER BY CreatedDate Desc) ";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@Referensi", txtNotaNumber.Text);
                Cmd.Parameters.AddWithValue("@JournalHID", JournalHID);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        Query = "INSERT INTO [ISBS-NEW4].[dbo].[GLJournalDtl]([GLJournalHID],[SeqNo],[JournalHID],[JournalIDSeqNo],[FQAID],[FQADesc],[JournalDType],[Auto],[Amount],CreatedDate,CreatedBy) ";
                        Query += " VALUES (@GLJournalHID,@SeqNo,@JournalHID,@JournalIDSeqNo,@FQAID,@FQADesc,@JournalDType,@Auto,@Amount,getdate(),@CreatedBy) ";
                        using (SqlCommand Cmd2 = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd2.Parameters.AddWithValue("@GLJournalHID", GLJournalHID);
                            Cmd2.Parameters.AddWithValue("@SeqNo", Dr["SeqNo"]);
                            Cmd2.Parameters.AddWithValue("@JournalHID", Dr["JournalHID"]);
                            Cmd2.Parameters.AddWithValue("@JournalIDSeqNo", Dr["JournalIDSeqNo"]);
                            Cmd2.Parameters.AddWithValue("@FQAID", Dr["FQAID"]);
                            Cmd2.Parameters.AddWithValue("@FQADesc", Dr["FQADesc"]);
                            Cmd2.Parameters.AddWithValue("@JournalDType", Dr["JournalDType"].ToString() == "D" ? "K" : "D");
                            Cmd2.Parameters.AddWithValue("@Auto", Dr["Auto"]);
                            Cmd2.Parameters.AddWithValue("@Amount", Dr["Amount"]);
                            Cmd2.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                            Cmd2.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("fail");
                }
                Dr.Close();
            }

            Query = " INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[PostingDate],[Referensi],[Notes],[Status],[Posting],CreatedDate,CreatedBy) ";
            Query += " SELECT TOP 1 @GLJournalHID,[JournalHID],[PostingDate],[Referensi],[Notes],[Status],[Posting],getdate(),@CreatedBy FROM [GLJournalH] ";
            Query += " WHERE Referensi = @Referensi AND JournalHID = @JournalHID ORDER BY CreatedDate Desc;";
            using (Cmd = new SqlCommand(Query, Conn))
            {
                Cmd.Parameters.AddWithValue("@CreatedBy", ControlMgr.UserId);
                Cmd.Parameters.AddWithValue("@Referensi", txtNotaNumber.Text);
                Cmd.Parameters.AddWithValue("@JournalHID", JournalHID);
                Cmd.Parameters.AddWithValue("@GLJournalHID", GLJournalHID);
                Cmd.ExecuteNonQuery();
            }
            Conn.Close();
        }

        private void CreateJournalApproval()
        {
            string ActionCode = cmbActionCode.SelectedItem.ToString();

            decimal AdjustInProgress = 0, Available = 0, GainLossSuspense = 0, AdjustSuspense = 0,Gain =0,Loss = 0;
            decimal Price = 0;
            for (int i = 0; i < dgvNADetails.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNADetails.Rows[i].Cells["FullItemID"].Value);
                string QtyUoM = Convert.ToString(dgvNADetails.Rows[i].Cells["QtyUoM"].Value) == "" ? "0" : Convert.ToString(dgvNADetails.Rows[i].Cells["QtyUoM"].Value);

                Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);

                if (ActionCode != "Adjust Quantity")
                {
                    AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                }
                else if (ActionCode == "Adjust Quantity")
                {
                    if (QtyUoM.Contains("-"))
                    {
                        GainLossSuspense = GainLossSuspense + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                        //Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                        Loss = Loss + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    }
                    else
                    {
                        AdjustSuspense = AdjustSuspense + (Price * Convert.ToDecimal(QtyUoM));
                        AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                        Gain = Gain + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                        Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    }
                }
            }

            if (AdjustInProgress != 0 || Available != 0 || GainLossSuspense != 0 || AdjustSuspense != 0 || Gain != 0 || Loss != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "IN23";
                string Jenis = "JN", Kode = "JN";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, ConnectionString.GetConnection(),Cmd);
                string Notes = "";

                Query = "INSERT INTO [GLJournalH]([GLJournalHID],[JournalHID],[Referensi],[Status],[Posting],[CreatedDate],[CreatedBy], [PostingDate], [Notes]) ";
                Query += "VALUES('" + GLJournalHID + "', '" + JournalHID + "', '" + AdjustId + "', 'Gunakan', 0, GETDATE(), '" + ControlMgr.UserId + "', '1900/01/01', '" + Notes + "')";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.ExecuteNonQuery();
                }
                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                        Type = Convert.ToString(Dr["Type"]);
                        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                        AmountValue = 0;

                        if (JournalHID == "IN23")
                        {
                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = Available;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = Loss;
                            }
                            else if (JournalIDSeqNo == 3)
                            {
                                AmountValue = AdjustSuspense;
                            }
                            else if (JournalIDSeqNo == 4)
                            {
                                AmountValue = AdjustInProgress;
                            }
                            else if (JournalIDSeqNo == 5)
                            {
                                AmountValue = GainLossSuspense;
                            }
                            else if (JournalIDSeqNo == 6)
                            {
                                AmountValue = Gain;
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
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        SeqNo++;
                    }
                    Dr.Close();
                }
            }
            //End
        }

        private void CreateJournalUnapprove()
        {
            string ActionCode = cmbActionCode.SelectedItem.ToString();

            decimal AdjustInProgress = 0, Available = 0, AdjustSuspense = 0, Gain =0;
            decimal Price = 0;
            for (int i = 0; i < dgvNADetails.RowCount; i++)
            {
                string FullItemID = Convert.ToString(dgvNADetails.Rows[i].Cells["FullItemID"].Value);
                string QtyUoM = Convert.ToString(dgvNADetails.Rows[i].Cells["QtyUoM"].Value) == "" ? "0" : Convert.ToString(dgvNADetails.Rows[i].Cells["QtyUoM"].Value);

                Price = GetPriceFromInventTable("UoM_AvgPrice", FullItemID);

                if (ActionCode != "Adjust Quantity")
                {
                    AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                }
                else if (ActionCode == "Adjust Quantity")
                {
                    if (QtyUoM.Contains("-"))
                    {
                        //GainLossSuspense = GainLossSuspense + (Price * Convert.ToDecimal(QtyUoM.Substring(1)));
                        Available = Available + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                        Gain = Gain + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    }
                    else
                    {
                        AdjustSuspense = AdjustSuspense + (Price * Convert.ToDecimal(QtyUoM));
                        AdjustInProgress = AdjustInProgress + (Price * Convert.ToDecimal(QtyUoM.Contains("-") == true ? QtyUoM.Substring(1) : QtyUoM));
                    }
                }
            }

            if (AdjustInProgress != 0 || Available != 0 || Gain != 0 || AdjustSuspense != 0)
            {
                //Insert Header GLJournal
                string JournalHID = "IN29";
                string Jenis = "JN", Kode = "JN";
                string Notes = "";
                string GLJournalHID = ConnectionString.GenerateSeqID(7, Jenis, Kode, ConnectionString.GetConnection(), Cmd);

                //Select Config Journal
                int SeqNo = 1;
                int JournalIDSeqNo = 0;
                string Type = "";
                string FQA_ID = "";
                string FQA_Desc = "";
                decimal AmountValue = 0;

                Query = "SELECT SeqNo, [Type], FQA_ID, FQA_Desc FROM M_JournalD WHERE JournalHID ='" + JournalHID + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        JournalIDSeqNo = Convert.ToInt32(Dr["SeqNo"]);
                        Type = Convert.ToString(Dr["Type"]);
                        FQA_ID = Convert.ToString(Dr["FQA_ID"]);
                        FQA_Desc = Convert.ToString(Dr["FQA_Desc"]);
                        AmountValue = 0;

                        if (JournalHID == "IN29")
                        {
                            if (JournalIDSeqNo == 1)
                            {
                                AmountValue = Available;
                            }
                            else if (JournalIDSeqNo == 2)
                            {
                                AmountValue = AdjustSuspense;
                            }
                            else if (JournalIDSeqNo == 3)
                            {
                                AmountValue = AdjustInProgress;
                            }
                            else if (JournalIDSeqNo == 4)
                            {
                                AmountValue = Gain;
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
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                        SeqNo++;
                    }
                    Dr.Close();
                }
            }
        }


        private decimal GetPriceFromInventTable(string FieldName, string FullItemID)
        {
            Query = "SELECT " + FieldName + " FROM InventTable WHERE FullItemID = '" + FullItemID + "'";
            string result = "";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                result = Convert.ToString(Cmd.ExecuteScalar());
            }
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

        private void dgvNADetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dgvNADetails.Rows.Count; i++)
            {
                decimal QtyUoM = 0;
                decimal Ratio = 0;
                if (dgvNADetails.Rows[i].Cells["QtyUoM"].Value == null)
                {
                    dgvNADetails.Rows[i].Cells["QtyUoM"].Value = "0.0000";
                }
                else if (dgvNADetails.Rows[i].Cells["QtyUoM"].Value.ToString() == "-")
                {
                    dgvNADetails.Rows[i].Cells["QtyUoM"].Value = "-1";
                }
                QtyUoM = Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyUoM"].Value.ToString() == "" || dgvNADetails.Rows[i].Cells["QtyUoM"].Value == null ? "0" : dgvNADetails.Rows[i].Cells["QtyUoM"].Value.ToString());
                Ratio = Convert.ToDecimal(dgvNADetails.Rows[i].Cells["Ratio"].Value.ToString() == "" || dgvNADetails.Rows[i].Cells["Ratio"].Value == null ? "0" : dgvNADetails.Rows[i].Cells["Ratio"].Value.ToString());

                dgvNADetails.Rows[i].Cells["QtyAlt"].Value = QtyUoM * Ratio;
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                string transStatus = "";
                Query = "SELECT [TransStatus] FROM [ISBS-NEW4].[dbo].[NotaAdjustmentH] WHERE [AdjustID] = @AdjustID";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@AdjustID", txtNotaNumber.Text);
                    transStatus = Cmd.ExecuteScalar().ToString();
                }
                if (transStatus == "03" || transStatus == "02")
                {
                    MessageBox.Show("Transaksi sudah tidak bisa diedit.");
                    return;
                }
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GetDataHeader();
            ModeBeforeEdit();
        }

        private void ToSpec_DropDownClosed(object sender, EventArgs e)
        {
            ToSpec.Visible = false;
        }

        private void Quality_DropDownClosed(object sender, EventArgs e)
        {
            Quality.Visible = false;
        }

        private void ToSpec_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvNADetails.CurrentCell.Value = ToSpec.Text.ToString();
            for (int j = 0; j < dgvNADetails.RowCount; j++)
            {
                if (dgvNADetails.Rows[j].Cells["No"].Value == dgvNADetails.Rows[dgvNADetails.CurrentCell.RowIndex].Cells["No"].Value.ToString())
                {
                    dgvNADetails.Rows[j].Cells["ToSpec"].Value = dgvNADetails.Rows[dgvNADetails.CurrentCell.RowIndex].Cells["ToSpec"].Value.ToString();
                }
            }
        }

        private void Quality_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dgvNADetails.CurrentCell.Value = Quality.Text.ToString();
            for (int j = 0; j < dgvNADetails.RowCount; j++)
            {
                if (dgvNADetails.Rows[j].Cells["No"].Value == dgvNADetails.Rows[dgvNADetails.CurrentCell.RowIndex].Cells["No"].Value.ToString())
                {
                    dgvNADetails.Rows[j].Cells["Quality"].Value = dgvNADetails.Rows[dgvNADetails.CurrentCell.RowIndex].Cells["Quality"].Value.ToString();
                }
            }
        }

        private void dgvNADetails_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                if (dgvNADetails.Columns[e.ColumnIndex].Name.ToString() == "ToSpec")
                {
                    ToSpec.Location = dgvNADetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                    ToSpec.Visible = true;
                    string tmpFullItemId = dgvNADetails.Rows[dgvNADetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                    string tmpToSpec = "";
                    Conn = ConnectionString.GetConnection();
                    for (int i = 0; i < dgvNADetails.RowCount; i++)
                    {
                        if (dgvNADetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                        {
                            if (dgvNADetails.Rows[i].Cells["ToSpec"].Value != null)
                            {
                                if (tmpToSpec == "")
                                {
                                    tmpToSpec = "'" + dgvNADetails.Rows[i].Cells["ToSpec"].Value.ToString() + "'";
                                }
                                else
                                {
                                    tmpToSpec += ",'" + dgvNADetails.Rows[i].Cells["ToSpec"].Value.ToString() + "'";
                                }
                            }
                        }
                    }

                    Query = "SELECT [SpecID] FROM [dbo].[InventSpec] ";

                    if (tmpToSpec != "")
                        Query += "Where SpecID not in (" + tmpToSpec + ");";

                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader DrCmb;
                    DrCmb = Cmd.ExecuteReader();

                    ToSpec.Items.Clear();
                    ToSpec.Items.Add("");
                    while (DrCmb.Read())
                    {
                        ToSpec.Items.Add(DrCmb[0].ToString());
                    }
                    ToSpec.SelectedIndex = 0;
                    DrCmb.Close();

                    Conn.Close();
                }
                else if (dgvNADetails.Columns[e.ColumnIndex].Name.ToString() == "Quality")
                {
                    Quality.Location = dgvNADetails.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false).Location;
                    Quality.Visible = true;
                    string tmpFullItemId = dgvNADetails.Rows[dgvNADetails.CurrentRow.Index].Cells["FullItemId"].Value.ToString();
                    string tmpQuality = "";
                    Conn = ConnectionString.GetConnection();
                    for (int i = 0; i < dgvNADetails.RowCount; i++)
                    {
                        if (dgvNADetails.Rows[i].Cells["FullItemId"].Value.ToString() == tmpFullItemId)
                        {
                            if (dgvNADetails.Rows[i].Cells["Quality"].Value != null)
                            {
                                if (tmpQuality == "")
                                {
                                    tmpQuality = "'" + dgvNADetails.Rows[i].Cells["Quality"].Value.ToString() + "'";
                                }
                                else
                                {
                                    tmpQuality += ",'" + dgvNADetails.Rows[i].Cells["Quality"].Value.ToString() + "'";
                                }
                            }
                        }
                    }

                    Query = "SELECT [QualityID] FROM [dbo].[InventQuality] ";

                    if (tmpQuality != "")
                        Query += "Where QualityID not in (" + tmpQuality + ");";

                    Cmd = new SqlCommand(Query, Conn);
                    SqlDataReader DrCmb;
                    DrCmb = Cmd.ExecuteReader();

                    Quality.Items.Clear();
                    Quality.Items.Add("");
                    while (DrCmb.Read())
                    {
                        Quality.Items.Add(DrCmb[0].ToString());
                    }
                    Quality.SelectedIndex = 0;
                    DrCmb.Close();

                    Conn.Close();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        private void dgvNADetails_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNADetails.Columns[e.ColumnIndex].Name.ToString() == "ToSpec")
            {
                ToSpec.Visible = false;
            }
            else if (dgvNADetails.Columns[e.ColumnIndex].Name.ToString() == "Quality")
            {
                Quality.Visible = false;
            }
            dgvNADetails.AutoResizeColumns();
        }

        private void dgvNADetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvNADetails.Columns[dgvNADetails.CurrentCell.ColumnIndex].Name == "QtyUoM")
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-')
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                //hendry end

                if (e.KeyChar == '-' && (sender as TextBox).Text.IndexOf('-') > -1)
                {
                    e.Handled = true;
                }
            }
        }

        private void dgvNADetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvNADetails_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvNADetails_KeyPress);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvNADetails.RowCount > 0)
            {
                Index = dgvNADetails.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvNADetails.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " FullItemID = " + dgvNADetails.Rows[Index].Cells["FullItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvNADetails.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //string NumberGroupSeq = dgvRODetails.Rows[dgvRODetails.CurrentCell.RowIndex].Cells["PurchaseOrderSeqNo"].Value.ToString();

                    dgvNADetails.Rows.RemoveAt(Index);

                    SortNoDataGrid();
                }
                //GetGelombang();
            }
        }

        private void SortNoDataGrid()
        {
            for (int i = 0; i < dgvNADetails.RowCount; i++)
            {
                dgvNADetails.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void dgvNADetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                if (dgvNADetails.Columns[e.ColumnIndex].Name.ToString() == "ToFullItemId" )
                {
                    //string SchemaName = "dbo";
                    //string TableName = "InventTable";

                    //Search tmpSearch = new Search();
                    //tmpSearch.SetSchemaTable(SchemaName, TableName);
                    //tmpSearch.ShowDialog();
                    //dgvNADetails.CurrentCell.Value = ConnectionString.Kode;
                    //dgvNADetails.Rows[e.RowIndex].Cells["ToItemName"].Value = ConnectionString.Kode3;
                    //dgvNADetails.Rows[e.RowIndex].Cells["ToSpec"].Value = ConnectionString.Kode2;

                    SearchQueryV1 tmpSearch = new SearchQueryV1();
                    tmpSearch.PrimaryKey = "FullItemId";
                    tmpSearch.Order = "FullItemId Asc";
                    tmpSearch.QuerySearch = "SELECT Distinct [FullItemId],[ItemDeskripsi],[SpecID] FROM [InventTable] ";
                    tmpSearch.FilterText = new string[] { "FullItemId", "ItemDeskripsi", "SpecID" };
                    tmpSearch.Select = new string[] { "FullItemId","ItemDeskripsi","SpecID" };
                    tmpSearch.ShowDialog();

                    if (ConnectionString.Kodes != null)
                    {
                        dgvNADetails.CurrentCell.Value = ConnectionString.Kodes[0];
                        dgvNADetails.Rows[e.RowIndex].Cells["ToItemName"].Value = ConnectionString.Kodes[1];
                        dgvNADetails.Rows[e.RowIndex].Cells["ToSpec"].Value = ConnectionString.Kodes[2];
                        ConnectionString.Kodes = null;
                        dgvNADetails.AutoResizeColumns();
                    }
                }
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) <= 0)
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
                return;
            }

            string transStatus = "";
            Query = "SELECT [TransStatus] FROM [ISBS-NEW4].[dbo].[NotaAdjustmentH] WHERE [AdjustID] = @AdjustID";
            using(Cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@AdjustID", txtNotaNumber.Text);
                transStatus = Cmd.ExecuteScalar().ToString();
            }
            if(transStatus != "01")
            {
                MessageBox.Show("Status bukan waiting for approval, tidak bisa di approve.");
                return;
            }

            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;

            Mode = "Approve";

            String TransStatus = "03";

            try
            {
                using (scope = new TransactionScope())
                {
                    UpdateInventOnHandAndMovement("Approve");

                    Query = "Update [dbo].[NotaAdjustmentH] set TransStatus = '" + TransStatus + "' , ApprovedBy = '" + ControlMgr.UserId + "' Where AdjustID = '" + txtNotaNumber.Text + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                    for (int i = 0; i <= dgvNADetails.RowCount - 1; i++)
                    {
                        String ApprovalNotes = "";
                        if (dgvNADetails.Columns.Contains("ApprovalNotes"))
                        {
                            ApprovalNotes = dgvNADetails.Rows[i].Cells["ApprovalNotes"].Value == null ? "" : dgvNADetails.Rows[i].Cells["ApprovalNotes"].Value.ToString();
                        }
                        Query = "Update NotaAdjustment_Dtl Set ";
                        Query += "[ApprovalNotes]='" + ApprovalNotes + "' ";
                        Query += " Where AdjustId = '" + AdjustId + "' And SeqNo = '" + dgvNADetails.Rows[i].Cells["No"].Value.ToString() + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }

                    CreateJournalApproval();

                    InsertNALog("03", "");
                    scope.Complete();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Data :" + AdjustId + " Berhasil diApprove.");
            ModeBeforeEdit();
            Parent.RefreshGrid();
        }

        private void btnUnapprove_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) <= 0)
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
                return;
            }

            for (int i = 0; i < dgvNADetails.Rows.Count; i++)
            {
                Query = "SELECT * FROM [ISBS-NEW4].[dbo].[Invent_OnHand_Qty] WHERE [FullItemId]=@FullItemId AND [InventSiteId]=@InventSiteId";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@FullItemId",dgvNADetails.Rows[i].Cells["FullItemId"].Value);
                    Cmd.Parameters.AddWithValue("@InventSiteId",txtInventSiteID.Text);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        if ((Convert.ToDecimal(Dr["Available_For_Sale_UoM"]) - Convert.ToDecimal(dgvNADetails.Rows[i].Cells["QtyUoM"].Value)) <= 0)
                        {
                            MessageBox.Show("Qty untuk item " + dgvNADetails.Rows[i].Cells["FullItemId"].Value + " sudah tidak mencukupi.");
                            Dr.Close();
                            return;
                        }
                    }
                    Dr.Close();
                }
            }

            string transStatus = "";
            Query = "SELECT [TransStatus] FROM [ISBS-NEW4].[dbo].[NotaAdjustmentH] WHERE [AdjustID] = @AdjustID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@AdjustID",txtNotaNumber.Text);
                transStatus = Cmd.ExecuteScalar().ToString();
            }
            if (transStatus != "03")
            {
                MessageBox.Show("Transaksi tidak bisa di Un-approve, status ("+transStatus+").");
                return;
            }

            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;

            Mode = "Approve";

            String TransStatus = "01";

            try
            {
                using (scope = new TransactionScope())
                {
                    UpdateInventOnHandAndMovement("Unapprove");
                    Query = "Update [dbo].[NotaAdjustmentH] set TransStatus = '" + TransStatus + "' , ApprovedBy = '" + ControlMgr.UserId + "' Where AdjustID = '" + txtNotaNumber.Text + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                    for (int i = 0; i <= dgvNADetails.RowCount - 1; i++)
                    {
                        String ApprovalNotes = "";
                        if (dgvNADetails.Columns.Contains("ApprovalNotes"))
                        {
                            ApprovalNotes = dgvNADetails.Rows[i].Cells["ApprovalNotes"].Value == null ? "" : dgvNADetails.Rows[i].Cells["ApprovalNotes"].Value.ToString();
                        }

                        Query = "Update NotaAdjustment_Dtl Set  ";
                        Query += "[ApprovalNotes]='" + ApprovalNotes + "' ";
                        Query += " Where AdjustId = '" + AdjustId + "' And SeqNo = '" + dgvNADetails.Rows[i].Cells["No"].Value.ToString() + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    CreateJournalUnapprove();
                    InsertNALog("01", "");
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Data :" + AdjustId + " Berhasil di-Unapprove.");
            ModeBeforeEdit();
            Parent.RefreshGrid();
        }

        private void btnRevision_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.Approve) <= 0)
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
                return;
            }

            string transStatus = "";
            Query = "SELECT [TransStatus] FROM [ISBS-NEW4].[dbo].[NotaAdjustmentH] WHERE [AdjustID] = @AdjustID";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@AdjustID", txtNotaNumber.Text);
                transStatus = Cmd.ExecuteScalar().ToString();
            }
            if (transStatus != "01")
            {
                MessageBox.Show("Status bukan waiting for approval, tidak bisa di revisi.");
                return;
            }

            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;
            String TransStatus = "04";

            try
            {
                using (scope = new TransactionScope())
                {
                    Query = "Update [dbo].[NotaAdjustmentH] set TransStatus = '" + TransStatus + "' , ApprovedBy = '" + ControlMgr.UserId + "' Where AdjustID = '" + txtNotaNumber.Text + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                    InsertNALog("04", "");
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("Data :" + AdjustId + " request untuk revisi berhasil.");
            ModeBeforeEdit();
            Parent.RefreshGrid();
        }

    }
}
