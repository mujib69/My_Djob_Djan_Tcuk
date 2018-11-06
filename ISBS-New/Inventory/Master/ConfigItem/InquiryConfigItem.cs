using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.ConfigItem
{
    public partial class InquiryConfigItem : MetroFramework.Forms.MetroForm
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

        List<FormConfigItem> ListFormConfigItem = new List<FormConfigItem>();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public InquiryConfigItem()
        {
            InitializeComponent();
        }

        public void RefreshGrid()
        {
            string Select = ", GroupId,GroupDesc, SubGroup1ID,SubGroup1Desc, SubGroup2ID,SubGroup2Desc, Ukuran1, Ukuran2, Ukuran3, Ukuran4, Ukuran5, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
            string SelectLike = " (GroupId like @search or SubGroup1ID like @search or SubGroup2ID like @search or Ukuran1 like @search or Ukuran2 like @search or Ukuran3 like @search or Ukuran4 like @search or Ukuran5 like @search) ";
            
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (cmbCriteria.Text == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No " + Select + " From [dbo].[InventConfig] ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text =="All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No " + Select + " From [dbo].[InventConfig] Where " + SelectLike + "  ) a " ;
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text.Contains("Date"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No " + Select + " From [dbo].[InventConfig] Where ";

                if (cmbCriteria.Text == "Created Date")
                {
                    Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                }
                if (cmbCriteria.Text == "Updated Date")
                {
                    Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                }

                Query += SelectLike + " ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE TableName='InventConfig' AND DisplayName = '" + cmbCriteria.Text + "'", Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY GroupId) No " + Select + " From [dbo].[InventConfig] Where ";
                Query += crit + " Like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@search",
                Value = "%" + txtSearch.Text + "%",
                SqlDbType = SqlDbType.NVarChar,
                Size = 2000  
            });
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvConfigItem.AutoGenerateColumns = true;
            dgvConfigItem.DataSource = Dt;
            dgvConfigItem.Refresh();
            dgvConfigItem.AutoResizeColumns();
            dgvSetting();
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(GroupId) GroupId From [dbo].[InventConfig] ;";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(GroupId) GroupId From [dbo].[InventConfig] Where " + SelectLike + ";";
            }
            else if (cmbCriteria.Text.Contains("Date"))
            {
                Query = "Select Count(GroupId) From [dbo].[InventConfig] Where ";
                if (cmbCriteria.Text == "Created Date")
                {
                    Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                }
                if (cmbCriteria.Text == "Updated Date")
                {
                    Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And ";
                }
                Query += SelectLike;
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE TableName='InventConfig' AND DisplayName = '" + cmbCriteria.Text + "'", Conn);
                crit = Cmd.ExecuteScalar().ToString();

                Query = "Select Count(GroupId) GroupId From [dbo].[InventConfig] Where ";
                Query += crit + " Like @search";
            }
            
            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void dgvSetting()
        {
            string[] Header;
            Header = new string[] { "Group","Group Name", "Sub Group","Sub Group Name", "Sub Group2","Sub Group2 Name", "Ukuran 1", "Ukuran 2", "Ukuran 3", "Ukuran 4", "Ukuran 5", "Created Date", "Created By", "Updated Date", "Updated By" };
            for (int i = 1; i < Header.Count() + 1; i++)
            {
                dgvConfigItem.Columns[i].HeaderText = Header[i - 1];
            }

            dgvConfigItem.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            dgvConfigItem.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'InventConfig'";

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
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            RefreshGrid();
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                FormConfigItem ConfigItem = new FormConfigItem();

                ListFormConfigItem.Add(ConfigItem);
                ConfigItem.ModeNew();
                ConfigItem.SetParent(this);
                ConfigItem.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end       
        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                FormConfigItem ConfigItem = new FormConfigItem();
                ConfigItem.SetParent(this);
                string ID = dgvConfigItem.CurrentRow.Cells["GroupId"].Value.ToString() + dgvConfigItem.CurrentRow.Cells["SubGroup1Id"].Value.ToString() + dgvConfigItem.CurrentRow.Cells["SubGroup2Id"].Value.ToString();
                ConfigItem.Show();
                ConfigItem.GetDataHeader(ID);
                ConfigItem.ModeBeforeEdit();
                RefreshGrid();
            }
        }

        private bool cekValidasi(string SubGroup2Id)
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventConfig] Where SubGroup2ID='" + SubGroup2Id + "'";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Item Config tidak ditemukan..";
                            vBol = false;
                            RefreshGrid();
                        }
                        Dr.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
                finally
                {
                    Conn.Close();
                }
            }            

            if (vBol == false) { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvConfigItem.RowCount > 0)
                    {
                        Index = dgvConfigItem.CurrentRow.Index;
                        string GroupId = dgvConfigItem.Rows[Index].Cells["GroupId"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["GroupId"].Value.ToString();
                        string SubGroup1Id = dgvConfigItem.Rows[Index].Cells["SubGroup1ID"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["SubGroup1ID"].Value.ToString();
                        string SubGroup2Id = dgvConfigItem.Rows[Index].Cells["SubGroup2ID"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["SubGroup2ID"].Value.ToString();
                        string ID = GroupId + SubGroup1Id + SubGroup2Id;
                        string No = dgvConfigItem.Rows[Index].Cells["No"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["No"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        if (cekValidasi(SubGroup2Id) == false)
                        {
                            return;
                        }

                        DialogResult dr = MessageBox.Show("No = " + No + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            //delete header
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[InventConfig] where GroupId+SubGroup1ID+SubGroup2ID='" + ID + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("No = " + No + "\n" + "Data berhasil dihapus.");

                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
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
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access  
            FormConfigItem ConfigItem = new FormConfigItem();                 
            if (ConfigItem.PermissionAccess(ControlMgr.View) > 0)
            {
                //Simpen HeaderId
                if (dgvConfigItem.RowCount > 0)
                {
                     ConfigItem.SetParent(this);
                    string ID = dgvConfigItem.CurrentRow.Cells["GroupId"].Value.ToString() + dgvConfigItem.CurrentRow.Cells["SubGroup1Id"].Value.ToString() + dgvConfigItem.CurrentRow.Cells["SubGroup2Id"].Value.ToString();
                    ConfigItem.Show();
                    ConfigItem.GetDataHeader(ID);
                    ConfigItem.ModeBeforeEdit();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            cmbCriteria.SelectedItem = "All";
            ModeLoad();
        }

        //private void DashPR_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    timerRefresh = null;
        //    for (int i = 0; i < ListFormConfigItem.Count(); i++)
        //    {
        //        ListFormConfigItem[i].Close();
        //    }
        //}

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }

        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshGrid();
            }
        }

        private void InquiryConfigItem_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
            //setTimer();
        }

    }
}
