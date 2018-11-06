using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.Gelombang
{
    public partial class GelombangForm : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        /// <summary>
   
        /// </summary>
        String Mode, Query, Query1,vJenis;
        String GelombangId = null;
        String BracketId = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        Master.Gelombang.GelombangInquiry Parent;

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public GelombangForm(string _vJenis)
        {
            InitializeComponent();
            vJenis = _vJenis;
        }

        public void flag(String gelombangid, String bracketid, String mode)
        {
            GelombangId = gelombangid;
            BracketId = bracketid;
            Mode = mode;
        }

        private void GelombangForm_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "NewGel")
            {
                ModeNewGel();
            }
            else if (Mode == "NewBrk")
            {
                ModeNewBrk();
                Bracket();
                
            }
            else if(Mode == "Edit")
            {
                RefreshGrid();
                GetDataHeader();
                LoadVendor();
            }
        }

        public string getItemId()
        {
            string ItemId = "";

            if (dgvGelombangD.RowCount > 0)
            {
                for (int i = 0; i <= dgvGelombangD.RowCount - 1; i++)
                {
                    if (i == 0)
                    {
                        ItemId += "FullItemId not in ('";
                        ItemId += dgvGelombangD.Rows[i].Cells["ItemId"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemId"].Value.ToString();
                        ItemId += "'";
                    }
                    else
                    {
                        ItemId += ",'";
                        ItemId += dgvGelombangD.Rows[i].Cells["ItemId"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemId"].Value.ToString();
                        ItemId += "'";
                    }
                }
                ItemId += ")";
                return ItemId;
            }
            else
            {
                ItemId = "FullItemId not in('')";
                return ItemId;
            }
        }

        public void GetDataHeader()
        {
            if (GelombangId != "" && BracketId != "")
            {
                Conn = ConnectionString.GetConnection();

                if (dgvGelombangD.RowCount - 1 <= 0)
                {
                    dgvGelombangD.ColumnCount = 6;
                    dgvGelombangD.Columns[0].Name = "No";
                    dgvGelombangD.Columns[1].Name = "ItemID";
                    dgvGelombangD.Columns[2].Name = "ItemName";
                    dgvGelombangD.Columns[3].Name = "Base";
                    dgvGelombangD.Columns[4].Name = "Price";
                    dgvGelombangD.Columns[5].Name = "SeqNo";
                    dgvGelombangD.Columns[0].Width = 40;
                    dgvGelombangD.Columns[1].Width = 150;
                    dgvGelombangD.Columns[2].Width = 150;
                    dgvGelombangD.Columns[3].Width = 40;
                }

                Query = "Select * From(Select ROW_NUMBER() OVER (ORDER BY GelombangId) No, ItemId, ItemName, Base, Price, SeqNo From [dbo].[InventGelombangD] Where GelombangId = '" + GelombangId + "' and BracketId = '"+BracketId+"' and Type='" + vJenis + "') a";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    // this.dgvGelombangD.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Convert.ToDecimal(Dr[4]).ToString("N2"), Dr[5]);
                     this.dgvGelombangD.Rows.Add(Dr[0], Dr[1], Dr[2], Dr[3], Dr[4], Dr[5]);
                }

                dgvGelombangD.AutoResizeColumns();
                Conn.Close();   
            }
        }

        //hendry tambah no urut : 9 april 2018
        private int CariNoUrutTerakhir()
        {
            int vKe;
            int vtxtNo_Urut=0;
            int xtxtNo_Urut=0;
            Boolean vAda = false;

            for (vKe = 0; vKe < dgvGelombangD.RowCount;vKe++)
            {
                vtxtNo_Urut = Convert.ToInt32(dgvGelombangD.Rows[vKe].Cells["SeqNo"].Value);
                if(vtxtNo_Urut>xtxtNo_Urut)
                {
                    xtxtNo_Urut = vtxtNo_Urut;
                }
            }

            if (xtxtNo_Urut == 0)
            {
                return 1;
            }
            else
            {
                return xtxtNo_Urut + 1;
            }
        }
        //hendry end

        public void AddDataGridFromDetail( List<string> ItemId )
        {
            if (dgvGelombangD.RowCount - 1 <= 0)
            {
                dgvGelombangD.ColumnCount = 6;
                dgvGelombangD.Columns[0].Name = "No";
                dgvGelombangD.Columns[1].Name = "ItemID";
                dgvGelombangD.Columns[2].Name = "ItemName";
                dgvGelombangD.Columns[3].Name = "Base";
                dgvGelombangD.Columns[4].Name = "Price";
                dgvGelombangD.Columns[5].Name = "SeqNo";
                dgvGelombangD.Columns[0].Width = 40;
                dgvGelombangD.Columns[1].Width = 150;
                dgvGelombangD.Columns[2].Width = 150;
                dgvGelombangD.Columns[3].Width = 40;
            }

            for (int i = 0; i < ItemId.Count; i++)
            {
                Query = "Select FullItemId, ItemDeskripsi From [dbo].[InventTable] Where FullItemId = '" + ItemId[i] + "' ";

                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();

                    while (Dr.Read())
                    {
                        String ItemId1 = Dr["FullItemId"] == null ? "" : Dr["FullItemId"].ToString();
                        String ItemName = Dr["ItemDeskripsi"] == null ? "" : Dr["ItemDeskripsi"].ToString();

                        this.dgvGelombangD.Rows.Add((dgvGelombangD.RowCount + 1).ToString(), ItemId1, ItemName, "N", 0, CariNoUrutTerakhir());
                    }
                }
                Conn.Close();
            }

            dgvGelombangD.ReadOnly = false;
            dgvGelombangD.Columns["No"].ReadOnly = true;
            dgvGelombangD.Columns["ItemID"].ReadOnly = true;
            dgvGelombangD.Columns["ItemName"].ReadOnly = true;
            dgvGelombangD.Columns["SeqNo"].ReadOnly = true;
            dgvGelombangD.AutoResizeColumns();

            for (int j = 0; j < dgvGelombangD.RowCount; j++)
            {
                if (dgvGelombangD.Rows[j].Cells["Base"].Value.ToString() != "Y" )
                {
                    dgvGelombangD.Rows[j].Cells["Base"].Value = "N";                    
                }
            }
            
        }

        private void Bracket()
        {
            Query = "Select GelombangId From [dbo].[InventGelombangH] Where GelombangId = '" + GelombangId + "' and BracketId = '" + BracketId + "' and Type='" + vJenis+ "' ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtGelombangId.Text = GelombangId.ToString();
                }
            }
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            Query = "Select Count(*) From [dbo].[InventGelombangD] Where GelombangId = '" + GelombangId + "' and BracketId = '" + BracketId + "' and Type='" + vJenis + "' ";

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[InventGelombangH] Where GelombangId = '" + GelombangId + "' and BracketId = '"+ BracketId +"' and Type='" + vJenis + "' ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtGelombangId.Text = GelombangId.ToString();
                    txtBracketId.Text = Dr["BracketId"].ToString();
                    dtDate.Value = (DateTime)Dr["Date"];
                    txtBracketDesc.Text = Dr["BracketDesc"].ToString();
                    rtxtDesc.Text = Dr["Deskripsi"].ToString();
                }
            }
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();
            Query = "Select Count(*) From [dbo].[InventGelombangD] Where GelombangId = '" + GelombangId + "' and BracketId = '" + BracketId + "' and Type='" + vJenis + "' ";

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void LoadVendor()
        {
            Query = "Select VendId From [dbo].[InventGelombangH] Where GelombangId = '" + GelombangId + "' and BracketId = '"+ BracketId +"' and Type='" + vJenis + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    string VendIdtest = Dr["VendID"].ToString();
                    List<string> Vid = VendIdtest.Split(';').ToList();
                    for (int k = 0; k < Vid.Count; k++)
                    {
                        Vid[k] = Vid[k].Trim();
                        this.lBoxVendor.Items.Add(Vid[k]);
                    }


                }
            }
            Conn.Close();

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

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text == "" ? "0" : cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
            //RefreshGrid();
        }

        private void ModeNewGel()
        {
            txtGelombangId.Enabled = false;
            dtDate.Enabled = true;
            txtBracketId.Enabled = false;

            txtBracketDesc.Enabled = true;
            rtxtDesc.Enabled = true;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            dgvGelombangD.Enabled = true;

            btnAddVendor.Enabled = true;
            btnRemove.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;

            dtDate.Enabled = false;
        }

        private void ModeNewBrk()
        {
            txtGelombangId.Enabled = false;
            dtDate.Enabled = true;
            txtBracketId.Enabled = false;
            txtBracketId.Text = "";
            txtBracketDesc.Text = "";
            rtxtDesc.Text = "";

            txtBracketDesc.Enabled = true;
            rtxtDesc.Enabled = true;
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;
            dgvGelombangD.Enabled = true;

            btnAddVendor.Enabled = true;
            btnRemove.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;

            dtDate.Enabled = false;

        }

        private void ModeEdit()
        {
            txtGelombangId.Enabled = false;
            dtDate.Enabled = false;
            txtBracketId.Enabled = false;
            
            btnAdd.Enabled = true;
            btnDelete.Enabled = true;

            txtBracketDesc.Enabled = true;
            rtxtDesc.Enabled = true;
            btnAddVendor.Enabled = true;
            btnRemove.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnExit.Visible = false;
            btnCancel.Visible = true;

            dgvGelombangD.ReadOnly = false;
            dgvGelombangD.Columns["No"].ReadOnly = true;
            dgvGelombangD.Columns["ItemID"].ReadOnly = true;
            dgvGelombangD.Columns["ItemName"].ReadOnly = true;
            dgvGelombangD.Columns["SeqNo"].ReadOnly = true;

            dtDate.Enabled = false;

            //make grid not sortable
            foreach (DataGridViewColumn column in dgvGelombangD.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }

        public void ParentRefreshGrid(Master.Gelombang.GelombangInquiry f)
        {
            Parent = f;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void ModeCancel()
        {
            txtGelombangId.Enabled = false;
            dtDate.Enabled = false;
            txtBracketId.Enabled = false;
            btnAdd.Enabled = false;
            btnDelete.Enabled = false;

            txtBracketDesc.Enabled = false;
            rtxtDesc.Enabled = false;
            btnAddVendor.Enabled = false;
            btnRemove.Enabled = false;
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;

            dgvGelombangD.ReadOnly = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeCancel();
            dgvGelombangD.Rows.Clear();
            GetDataHeader();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string CreatedBy = "";
            DateTime CreatedDate = DateTime.Now;
            String GelombangId = txtGelombangId.Text;
            if (txtBracketDesc.Text.Trim() == "")
            {
                MessageBox.Show("Bracket Description harus diisi");
            }
            else if (dgvGelombangD.Rows.Count == 0)
            {
                MessageBox.Show("Isi Detail dahulu");
            }
            else
            if (Mode == "NewGel")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                
                try
                {
                    String Bracket = "BRK00001";
                    string VendId = "";
                    for (int i = 0; i < lBoxVendor.Items.Count; i++)
                    {
                        if (VendId == "")
                            VendId = lBoxVendor.Items[i].ToString();
                        else
                            VendId += ";" + lBoxVendor.Items[i].ToString();
                    }

                    Query = "Declare @tmp table (Id varchar (50)) ";
                    Query += "Insert into [dbo].[InventGelombangH] (GelombangId,";
                    Query += "BracketId,Type,";
                    Query += "VendId,";
                    Query += "Date,";
                    Query += "BracketDesc,";
                    Query += "Deskripsi,";
                    Query += "CreatedDate,";
                    Query += "CreatedBy) Output (Inserted.GelombangId) into @tmp values (";
                    Query += "(select 'GLB'+ RIGHT('00000' + CONVERT(VARCHAR(5), case when (max(right(GelombangId,5)) is null) then 0 else max(right(GelombangId,5)) end +1), 5) FROM [dbo].[InventGelombangH] where Type='" + vJenis + "' AND GelombangId like ('%GLB%'))" + ",'";
                    Query += Bracket + "','";
                    Query += vJenis + "','";
                    Query += VendId + "','";
                    Query += dtDate.Value.Date + "','";
                    Query += txtBracketDesc.Text.Trim() + "','";
                    Query += rtxtDesc.Text.Trim() + "',";
                    Query += "getdate(),'";
                    Query += ControlMgr.UserId + "') select * from @tmp;";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        GelombangId = Cmd.ExecuteScalar().ToString();
                        //Cmd.ExecuteNonQuery();
                    }

                    if (dgvGelombangD.Rows.Count > 0)
                    {
                        int flag = 0;

                        for (int i = 0; i < dgvGelombangD.Rows.Count; i++)
                        {
                            String ItemId = dgvGelombangD.Rows[i].Cells["ItemId"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemId"].Value.ToString();
                            String ItemName = dgvGelombangD.Rows[i].Cells["ItemName"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemName"].Value.ToString();
                            String Base = dgvGelombangD.Rows[i].Cells["Base"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["Base"].Value.ToString();
                            decimal Price = dgvGelombangD.Rows[i].Cells["Price"].Value == null ? 0 : decimal.Parse(dgvGelombangD.Rows[i].Cells["Price"].Value.ToString());
                            decimal SeqNo = dgvGelombangD.Rows[i].Cells["SeqNo"].Value == null ? 0 : decimal.Parse(dgvGelombangD.Rows[i].Cells["SeqNo"].Value.ToString());

                            if (Base == "Y" || Base == "y")
                            {
                                flag += 1;
                            }
                            
                            if (flag <= 1)
                            {
                                Query = "Insert into [dbo].[InventGelombangD] (GelombangId, BracketId,Type, ItemId, ItemName, Base, Price, SeqNo, CreatedDate, CreatedBy) ";
                                Query += "values ('" + GelombangId + "',  '" + Bracket + "','" + vJenis + "', '" + ItemId + "', '" + ItemName + "', '" + Base + "', '" + Price + "','" + SeqNo + "',getdate(), '" + ControlMgr.UserId + "');";
                            }
                            else
                            {
                                Trans.Rollback();
                                MessageBox.Show("Y tidak boleh lebih dari 2");
                                return;
                            }

                            if (i == dgvGelombangD.Rows.Count - 1)
                            {
                                if (flag == 0)
                                {
                                    Trans.Rollback();
                                    MessageBox.Show("Minimal ada 1 Y");
                                    return;
                                }
                            }

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }

                    //if (lBoxVendor.Items.Count > 0)
                    //{
                    //    for (int i = 0; i < lBoxVendor.Items.Count; i++)
                    //    {
                    //        Query = "Insert into [dbo].[InventGelombangVendor] (GelombangId, BracketId, VendId, CreatedDate, CreatedBy) ";
                    //        Query += "values ('" + GelombangId + "',  '" + Bracket + "',  '" + lBoxVendor.Items[i] + "' ,getdate(), '" + ControlMgr.UserId + "');";

                    //        Cmd = new SqlCommand(Query, Conn, Trans);
                    //        Cmd.ExecuteNonQuery();
                    //    }
                    //}
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data :" + GelombangId + ", berhasil ditambahkan.");
                Parent.RefreshGrid();
                this.Close();
            }
            else if (Mode == "NewBrk")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                String BracketId = txtBracketId.Text;

                try
                {
                    string VendId = "";
                    for (int i = 0; i < lBoxVendor.Items.Count; i++)
                    {
                        if (VendId == "")
                            VendId = lBoxVendor.Items[i].ToString();
                        else
                            VendId += ";" + lBoxVendor.Items[i].ToString();
                    }


                    Query = "Declare @tmp table (Id varchar (50)) ";
                    Query += "Insert into [dbo].[InventGelombangH] (BracketId,";
                    Query += "GelombangId,";
                    Query += "VendId,Type,";
                    Query += "Date,";
                    Query += "BracketDesc,";
                    Query += "Deskripsi,";
                    Query += "CreatedDate,";
                    Query += "CreatedBy) Output (Inserted.BracketId) into @tmp values (";
                    Query += "(select 'BRK'+ RIGHT('00000' + CONVERT(VARCHAR(5), case when (max(right(BracketId,5)) is null) then 0 else max(right(BracketId,5)) end +1), 5) FROM [dbo].[InventGelombangH] where Type ='" + vJenis + "' AND BracketId like ('%BRK%') and GelombangId = '" + txtGelombangId.Text + "' )" + ",'";
                    Query += txtGelombangId.Text + "','";
                    Query += VendId + "','";
                    Query += vJenis + "','";
                    Query += dtDate.Value.Date + "','";
                    Query += txtBracketDesc.Text.Trim() + "','";
                    Query += rtxtDesc.Text.Trim() + "',";
                    Query += "getdate(),'";
                    Query += ControlMgr.UserId + "') select * from @tmp;";

                    using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                    {
                        BracketId = Cmd.ExecuteScalar().ToString();
                        //Cmd.ExecuteNonQuery();
                    }

                    if (dgvGelombangD.Rows.Count > 0)
                    {
                        int flag = 0;

                        for (int i = 0; i < dgvGelombangD.Rows.Count; i++)
                        {
                            String ItemId = dgvGelombangD.Rows[i].Cells["ItemId"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemId"].Value.ToString();
                            String ItemName = dgvGelombangD.Rows[i].Cells["ItemName"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemName"].Value.ToString();
                            String Base = dgvGelombangD.Rows[i].Cells["Base"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["Base"].Value.ToString();
                            decimal Price = dgvGelombangD.Rows[i].Cells["Price"].Value == null ? 0 : decimal.Parse(dgvGelombangD.Rows[i].Cells["Price"].Value.ToString());
                            decimal SeqNo = dgvGelombangD.Rows[i].Cells["SeqNo"].Value == null ? 0 : decimal.Parse(dgvGelombangD.Rows[i].Cells["SeqNo"].Value.ToString());

                            if (Base == "Y" || Base == "y")
                            {
                                flag += 1;
                            }

                            if (flag <= 1)
                            {
                                Query = "Insert into [dbo].[InventGelombangD] (GelombangId, BracketId,Type, ItemId, ItemName, Base, Price, SeqNo, CreatedDate, CreatedBy) ";
                                Query += "values ('" + txtGelombangId.Text + "',  '" + BracketId + "','" + vJenis + "', '" + ItemId + "', '" + ItemName + "', '" + Base + "', '" + Price + "','" + SeqNo + "', getdate(), '" + ControlMgr.UserId + "');";
                            }
                            else
                            {
                                Trans.Rollback();
                                MessageBox.Show("Y tidak boleh lebih dari 2");
                                return;
                            }

                            if (i == dgvGelombangD.Rows.Count - 1)
                            {
                                if (flag == 0)
                                {
                                    Trans.Rollback();
                                    MessageBox.Show("Minimal ada 1 Y");
                                    return;
                                }
                            }

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }

                    //if (lBoxVendor.Items.Count > 0)
                    //{
                    //    for (int i = 0; i < lBoxVendor.Items.Count; i++)
                    //    {
                    //        Query = "Insert into [dbo].[InventGelombangVendor] (GelombangId, BracketId, VendId, CreatedDate, CreatedBy) ";
                    //        Query += "values ('" + txtGelombangId.Text + "',  '" + BracketId + "',  '" + lBoxVendor.Items[i] + "' ,getdate(), '" + ControlMgr.UserId + "');";

                    //        Cmd = new SqlCommand(Query, Conn, Trans);
                    //        Cmd.ExecuteNonQuery();
                    //    }
                    //}
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data :" + GelombangId + ", berhasil ditambahkan.");
                Parent.RefreshGrid();
                this.Close();
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
             
                try
                {
                    if (dgvGelombangD.Rows.Count > 0)
                    {
                        int flag = 0;
                        string VendId = "";
                        for (int i = 0; i < lBoxVendor.Items.Count; i++)
                        {
                            if (VendId == "")
                                VendId = lBoxVendor.Items[i].ToString();
                            else
                                VendId += ";" + lBoxVendor.Items[i].ToString();
                        }

                        Query = "update [dbo].[InventGelombangH] set BracketDesc = '"+ txtBracketDesc.Text.Trim() +"', Deskripsi = '"+ rtxtDesc.Text.Trim() +"', VendID ='" + VendId + "',UpdatedDate = getDate() ,UpdatedBy= '"+ControlMgr.UserId+"' OUTPUT INSERTED.CreatedDate, INSERTED.CreatedBy where GelombangId ='" + txtGelombangId.Text + "' and BracketId = '"+ txtBracketId.Text +"' ;";

                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Dr = Cmd.ExecuteReader();

                        while (Dr.Read())
                        {
                            CreatedDate = Convert.ToDateTime(Dr["CreatedDate"]);
                            CreatedBy = Dr["CreatedBy"].ToString();
                        }

                        Dr.Close();



                        Query1 = "Delete from [dbo].[InventGelombangD] where GelombangId='" + txtGelombangId.Text.Trim() + "' and BracketId = '" + txtBracketId.Text.Trim() + "' and Type='" + vJenis + "';";
                        Cmd = new SqlCommand(Query1, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        for (int i = 0; i <= dgvGelombangD.RowCount - 1; i++)
                        {
                            String ItemId = dgvGelombangD.Rows[i].Cells["ItemId"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemId"].Value.ToString();
                            String ItemName = dgvGelombangD.Rows[i].Cells["ItemName"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["ItemName"].Value.ToString();
                            String Base = dgvGelombangD.Rows[i].Cells["Base"].Value == null ? "" : dgvGelombangD.Rows[i].Cells["Base"].Value.ToString();
                            decimal Price = dgvGelombangD.Rows[i].Cells["Price"].Value == null ? 0 : decimal.Parse(dgvGelombangD.Rows[i].Cells["Price"].Value.ToString());
                            decimal SeqNo = dgvGelombangD.Rows[i].Cells["SeqNo"].Value == null ? 0 : decimal.Parse(dgvGelombangD.Rows[i].Cells["SeqNo"].Value.ToString());

                            if (Base == "Y" || Base == "y")
                            {
                                flag += 1;
                            }

                            if (flag <= 1)
                            {
                                Query = "Insert into [dbo].[InventGelombangD] (GelombangId, BracketId,Type, ItemId, ItemName, Base, Price, SeqNo, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) ";
                                Query += "values ('" + txtGelombangId.Text + "',  '" + txtBracketId.Text + "','" + vJenis + "', '" + ItemId + "', '" + ItemName + "', '" + Base + "', '" + Price + "','" + SeqNo + "', '" + CreatedDate + "','" + CreatedBy + "',getdate(),'" + ControlMgr.UserId + "' );";
                            }
                            else
                            {
                                Trans.Rollback();
                                MessageBox.Show("Y tidak boleh lebih dari 2");
                                return;
                            }

                            if (i == dgvGelombangD.Rows.Count - 1)
                            {
                                if (flag == 0)
                                {
                                    Trans.Rollback();
                                    MessageBox.Show("Minimal ada 1 Y");
                                    return;
                                }
                            }

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }                   
                }
                catch(Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data GelombangId = " + txtGelombangId.Text + " , berhasil di update.");
                Parent.RefreshGrid();
                ModeCancel();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Master.Gelombang.AddItem F = new Master.Gelombang.AddItem();
            F.ParentRefreshGrid(this);
            F.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {            
            if (dgvGelombangD.RowCount > 0)
            {
                Index = dgvGelombangD.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data : " + Environment.NewLine + " No = " + dgvGelombangD.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + " ItemID = " + dgvGelombangD.Rows[Index].Cells["ItemID"].Value.ToString() + Environment.NewLine + " ItemName = " + dgvGelombangD.Rows[Index].Cells["ItemName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    dgvGelombangD.Rows.RemoveAt(Index);
                    //tia edit
                    for (int i = 0; i < dgvGelombangD.RowCount; i++)
                    {
                        dgvGelombangD.Rows[i].Cells["No"].Value = i + 1;
                    }
                    //tia edit end
                } 
            }
        }


        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewTextBoxCell cellPrice = dgvGelombangD[4, e.RowIndex] as DataGridViewTextBoxCell;
            DataGridViewTextBoxCell cellBase = dgvGelombangD[3, e.RowIndex] as DataGridViewTextBoxCell;

            if (cellPrice != null)
            {
                if (e.ColumnIndex == 4)
                {                   
                    Regex strPattern = new Regex("^[0-9.-]*$");
                    if (strPattern.IsMatch(e.FormattedValue.ToString()))
                    {

                    }
                    else
                    {
                        MessageBox.Show("Masukan Angka saja di kolom price");
                        e.Cancel = true;
                    }
                }
            }

            if (cellBase != null)
            {
                if (e.ColumnIndex == 3)
                {
                    Regex strPattern = new Regex("^[YNyn_]*$");
                    if (strPattern.IsMatch(e.FormattedValue.ToString()) && e.FormattedValue.ToString() != "")
                    {
                        
                    }
                    else
                    {
                        MessageBox.Show("Masukan Y/N saja");
                        e.Cancel = true;
                    }
                }
            }
        }

        private void btnAddVendor_Click(object sender, EventArgs e)
        {
            if (vJenis == "Purchase")
            {
                SearchQueryV1 tmpSearch = new SearchQueryV1();
                tmpSearch.PrimaryKey = "VendId";
                tmpSearch.Order = "VendId Asc";
                tmpSearch.QuerySearch = "SELECT Distinct [VendId],[VendName] FROM [VendTable]";
                tmpSearch.FilterText = new string[] { "VendId" , "VendName"};
                tmpSearch.Mask = new string[] { "Vendor Id", "Vendor Name" };
                tmpSearch.Select = new string[] { "VendId" };
                tmpSearch.ShowDialog();
            }
            else
            {
                SearchQueryV1 tmpSearch = new SearchQueryV1();
                tmpSearch.PrimaryKey = "CustId";
                tmpSearch.Order = "CustId Asc";
                tmpSearch.QuerySearch = "SELECT Distinct [CustId],[CustName] FROM [CustTable]";
                tmpSearch.FilterText = new string[] { "CustId" ,"CustName"};
                tmpSearch.Mask = new string[] { "Customer Id", "Customer Name" };
                tmpSearch.Select = new string[] { "CustId" };
                tmpSearch.ShowDialog();
            }
            
            if (ConnectionString.Kodes != null)            
            {
                if (lBoxVendor.Items.Contains(ConnectionString.Kodes[0]))             
                {
                    if (vJenis == "Purchase")
                    {
                        MessageBox.Show("Vendor sudah ada");
                    }
                    else
                    {
                        MessageBox.Show("Customer sudah ada");
                    }
                }                 
                else                 
                {                    
                    lBoxVendor.Items.Add(ConnectionString.Kodes[0]);                 
                }                 
                ConnectionString.Kodes = null;               
            }            
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection selectedItems = new ListBox.SelectedObjectCollection(lBoxVendor);
            selectedItems = lBoxVendor.SelectedItems;

            if (lBoxVendor.SelectedIndex != -1)
            {
                for (int i = selectedItems.Count - 1; i >= 0; i--)
                    lBoxVendor.Items.Remove(selectedItems[i]);
            }
            else
            {
                MessageBox.Show("!!!");
            }
        }

        private void dgvGelombangD_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {                        
            for (int j = 0; j < dgvGelombangD.RowCount; j++)
            {
                if (dgvGelombangD.Rows[j].Cells["Base"].Value.ToString() == "y")
                {
                    dgvGelombangD.Rows[j].Cells["Base"].Value = "Y";
                }

                if (dgvGelombangD.Rows[j].Cells["Base"].Value.ToString() == "n")
                {
                    dgvGelombangD.Rows[j].Cells["Base"].Value = "N";
                }

                if (dgvGelombangD.Rows[j].Cells["Base"].Value.ToString() == "Y")
                {
                    dgvGelombangD.Rows[j].Cells["Price"].Value = "0";
                }
            }

            if (dgvGelombangD.Columns[e.ColumnIndex].Name == "Base" && dgvGelombangD.CurrentRow.Cells["Base"].Value == "Y")
            {
                int a = dgvGelombangD.CurrentCell.RowIndex;

                for (int i = 0; i < dgvGelombangD.Rows.Count; i++)
                {
                    if (a != i)
                    {
                        dgvGelombangD.Rows[i].Cells["Base"].Value = "N";
                        dgvGelombangD.Rows[i].Cells["Price"].ReadOnly = false;
                        dgvGelombangD.Rows[i].Cells["Price"].Style.BackColor = Color.White;
                    }
                }
            }
        }
        //tia edit
        //klik kanan
        PopUp.Vendor.Vendor Vend = null;
        PopUp.FullItemId.FullItemId FID = null;
        public static string itemID;
        public string ItemID { get { return itemID; } set { itemID = value; } }

        private void dgvGelombangD_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvGelombangD.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvGelombangD.Columns[e.ColumnIndex].Name.ToString() == "ItemID")
                    {

                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvGelombangD.Rows[e.RowIndex].Cells["ItemID"].Value.ToString());
                        itemID = dgvGelombangD.Rows[e.RowIndex].Cells["ItemID"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvGelombangD.Rows[e.RowIndex].Cells["ItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }
        private bool CheckOpened(string name)
        {
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

        private void lBoxVendor_MouseDown(object sender, MouseEventArgs e)
        {
            //old code
            //lBoxVendor.SelectedIndex = lBoxVendor.IndexFromPoint(e.X, e.Y);
            //string VendID = lBoxVendor.SelectedItem.ToString();

            //STV EDIT start
            ListBox lb = ((ListBox)sender);
            Point pt = new Point(e.X, e.Y);
            int index = lb.IndexFromPoint(pt);
            lBoxVendor.SelectedIndex = lBoxVendor.IndexFromPoint(e.X, e.Y);
            string VendID = "";
            if (index >= 0)
            {
                VendID = lBoxVendor.SelectedItem.ToString();
            }
            //STV EDIT End

            if (e.Button == MouseButtons.Right && VendID != "")
            {
                if (Vend == null || Vend.Text == "")
                {
                    lBoxVendor.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(VendID);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.GetData(VendID);
                    Vend.Show();
                    Vend.Focus();
                }
            }

        }
        //end
    }
}
