using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ISBS_New.Master.Invent.Measurement
{
    public partial class InquiryMeasurement : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;

        List<FormMeasurement> ListFormMeasurement = new List<FormMeasurement>();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //timer autorefresh
        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10*1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        public InquiryMeasurement()
        {
            InitializeComponent();
        }

        private void InquiryMeasurement_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
            //setTimer();
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (cmbCriteria.Text == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY MeasurementId) No, MeasurementId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventMeasurement] ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text =="All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY MeasurementId) No, MeasurementId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventMeasurement] Where ";
                Query += "MeasurementId like @search or Deskripsi like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY MeasurementId) No, MeasurementId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventMeasurement] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (MeasurementId like @search or Deskripsi like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY MeasurementId) No, MeasurementId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventMeasurement] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (MeasurementId like @search or Deskripsi like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "' AND TableName = 'InventMeasurement'", Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY MeasurementId) No, MeasurementId, Deskripsi, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy From [dbo].[InventMeasurement] Where ";
                Query += cmbCriteria.Text + " Like @search) a ";
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
            dgvSetting();
            dgvConfigItem.AutoResizeColumns();
            //dgvCompanyInfo.Columns["GroupId"].Visible = false;
            //dgvCompanyInfo.Columns["SubGroup1Id"].Visible = false;
            //dgvCompanyInfo.Columns["SubGroup2Id"].Visible = false;
            //dgvCompanyInfo.Columns["CompanyInfoId"].Visible = false;
            Conn.Close();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(MeasurementId) From [dbo].[InventMeasurement];";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(MeasurementId) From [dbo].[InventMeasurement] Where ";
                Query += "MeasurementId like @search or Deskripsi like @search";
                
            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select Count(MeasurementId) From [dbo].[InventMeasurement] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (MeasurementId like @search or Deskripsi like @search)";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select Count(MeasurementId) From [dbo].[InventMeasurement] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (MeasurementId like @search or Deskripsi like @search)";
            }
            else
            {
                Cmd = new SqlCommand("SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "' AND TableName = 'InventMeasurement'", Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select Count(MeasurementId) From [dbo].[InventMeasurement] Where ";
                Query += cmbCriteria.Text + " Like @search";
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
            Header = new string[] { "Measurement ", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
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
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'InventMeasurement' Order By OrderNo";

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
                FormMeasurement FormMeasurement = new FormMeasurement();

                ListFormMeasurement.Add(FormMeasurement);
                FormMeasurement.SetParent(this);
                FormMeasurement.ModeNew();
                FormMeasurement.Show();
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
                FormMeasurement FormMeasurement = new FormMeasurement();
                FormMeasurement.SetParent(this);
                FormMeasurement.GetDataHeader(dgvConfigItem.CurrentRow.Cells["MeasurementID"].Value.ToString());
                FormMeasurement.ModeBeforeEdit();
                FormMeasurement.Show();
                RefreshGrid();
            }
        }

        private bool cekValidasi(string MeasurementId)
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventMeasurement] Where MeasurementId='" + MeasurementId + "'";
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
                            ErrMsg = "Measurement tidak ditemukan..";
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

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventTable] Where Ukuran1MeasurementID='" + MeasurementId + "' OR Ukuran2MeasurementID='" + MeasurementId + "' OR Ukuran3MeasurementID='" + MeasurementId + "' OR Ukuran4MeasurementID = '" + MeasurementId + "' OR Ukuran5MeasurementID='" + MeasurementId + "'";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Measurement sudah pernah digunakan..";
                            vBol = false;
                        }
                        else
                        {
                            vBol = true;
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
                        string MeasurementID = dgvConfigItem.Rows[Index].Cells["MeasurementID"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["MeasurementID"].Value.ToString();
                        string No = dgvConfigItem.Rows[Index].Cells["No"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["No"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        if (cekValidasi(MeasurementID) == false)
                        {
                            return;
                        }

                        DialogResult dr = MessageBox.Show("MeasurementID = " + MeasurementID + "\nApakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            //delete header
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[InventMeasurement] where MeasurementID=@MeasurementID ";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.AddWithValue("@MeasurementID", MeasurementID);
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
            FormMeasurement FormMeasurement = new FormMeasurement();
            if (FormMeasurement.PermissionAccess(ControlMgr.View) > 0)
            {
                //Simpen HeaderId
                if (dgvConfigItem.RowCount > 0)
                {
                    FormMeasurement.SetParent(this);
                    FormMeasurement.GetDataHeader(dgvConfigItem.CurrentRow.Cells["MeasurementID"].Value.ToString());
                    FormMeasurement.ModeBeforeEdit();
                    FormMeasurement.Show();
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
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
            txtSearch.Text = "";
            cmbCriteria.SelectedItem = "All";
            ModeLoad();
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

        //private void InquirySubGroup1_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    timerRefresh = null;
        //    for (int i = 0; i < ListFormMeasurement.Count(); i++)
        //    {
        //        ListFormMeasurement[i].Close();
        //    }
        //}

    }
}
