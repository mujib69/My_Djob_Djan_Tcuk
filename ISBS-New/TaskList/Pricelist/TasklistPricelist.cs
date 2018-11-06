using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.TaskList.Pricelist
{
    public partial class TasklistPricelist : MetroFramework.Forms.MetroForm
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
        public static int dataShow;
        private string TransStatus = String.Empty;
        private string PricelistType = "";

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        public TasklistPricelist()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            //MainMenu f = new MainMenu();
            //f.refreshTaskList();
        }
        public void PricelistTypes(string prmPricelistType)
        {
            PricelistType = prmPricelistType;
            lblForm.Text = lblForm.Text + " " + PricelistType + " Inquiry";
        }

        private void TasklistPricelist_Load(object sender, EventArgs e)
        {
            addCmbStatusCode();
            addCmbCrit();
            addCmbPricelistStatus();
            ModeLoad();
            lblForm.Location = new Point(16, 11);
        }

        private void TasklistPricelist_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void TasklistPricelist_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
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

            cmbCriteria.SelectedIndex = 0;
            cmbStatusCode.SelectedIndex = 0;


            cmbStatusCode.Enabled = false;

            RefreshGrid();
        }

        private void addCmbStatusCode()
        {
            cmbStatusCode.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select StatusCode, Deskripsi FROM TransStatusTable WHERE TransCode ='Pricelist'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbStatusCode.DisplayMember = "Text";
            cmbStatusCode.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbStatusCode.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[1] + "" });
            }
            cmbStatusCode.SelectedIndex = 0;
            Conn.Close();
        }

        private void addCmbPricelistStatus()
        {
            cmbPricelistStatus.Items.Add("All");


            cmbPricelistStatus.DisplayMember = "Text";
            cmbPricelistStatus.ValueMember = "Value";


            cmbPricelistStatus.Items.Add(new { Value = "1", Text = "Active" });
            cmbPricelistStatus.Items.Add(new { Value = "2", Text = "Inactive" });

            cmbPricelistStatus.SelectedIndex = 0;
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

        private void addCmbCrit()
        {
            try
            {
                cmbCriteria.Items.Add("All");
                Conn = ConnectionString.GetConnection();
                Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'PricelistH' order by OrderNo";

                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    cmbCriteria.Items.Add(Dr[0]);
                }
                // cmbCriteria.SelectedIndex = 0;
                Conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            //kalau mau pakai datetimepicker
            //else if (cmbCriteria.Text.Contains("Valid"))
            //{
            //    dtFrom.Enabled = true;
            //    dtTo.Enabled = true;
            //}
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }

            if (cmbCriteria.Text.Contains("StatusName"))
            {
                cmbStatusCode.Enabled = true;
            }
            else
            {
                cmbStatusCode.Enabled = false;
                cmbStatusCode.SelectedIndex = 0;
            }

            if (cmbCriteria.Text.Contains("PricelistStatus"))
            {
                cmbPricelistStatus.Enabled = true;
            }
            else
            {
                cmbPricelistStatus.Enabled = false;
                cmbPricelistStatus.SelectedIndex = 0;
            }

            if (cmbCriteria.Text.Contains("Date") || cmbCriteria.Text.Contains("StatusName") || cmbCriteria.Text.Contains("PricelistStatus"))
            {
                txtSearch.Enabled = false;
                txtSearch.Text = "";
            }
            else
            {
                txtSearch.Enabled = true;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                ISBS_New.Pricelist.PricelistHeader PH = new ISBS_New.Pricelist.PricelistHeader();
                PH.SetMode("New", "", PricelistType);
                PH.SetParent2(this);
                PH.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }
       
        public void RefreshGrid()
        {

            string cmbSelected = Convert.ToString(cmbStatusCode.SelectedIndex);
            if (cmbSelected == "1")
            {
                cmbSelected = "01";
            }
            else if (cmbSelected == "2")
            {
                cmbSelected = "02";
            }
            else if (cmbSelected == "3")
            {
                cmbSelected = "03";
            }
            else if (cmbSelected == "4")
            {
                cmbSelected = "04";
            }
            else
            {
                cmbSelected = "";
            }

            string cmbPricelist = Convert.ToString(cmbPricelistStatus.SelectedIndex);
            if (cmbPricelist == "1")
            {
                cmbPricelist = "1";
            }
            else if (cmbPricelist == "2")
            {
                cmbPricelist = "0";
            }
            else
            {
                cmbPricelist = "";
            }

            //Menampilkan data
            Conn = ConnectionString.GetConnection();

            string Type = "";
            if (PricelistType.ToUpper() == "JUAL")
            {
                Type = "SALES";
            }
            else
            {
                Type = "PURCHASE";
            }

            if (crit == null)
            {
                if (TransStatus == String.Empty)
                {
                    TransStatus = "'01'"; Limit1 = 1; Limit2 = dataShow;
                }

                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ")) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("All"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND (p.PricelistNo LIKE '%" + txtSearch.Text + "%' OR p.DeliveryMethod LIKE '%" + txtSearch.Text + "%')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (crit.Equals("PricelistNo"))
            else if (crit.Equals("Pricelist No"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND p.PricelistNo LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (crit.Equals("DeliveryMethod"))
            else if (crit.Equals("Delivery Method"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND p.DeliveryMethod LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (crit.Equals("PricelistStatus"))
            else if (crit.Equals("Active"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND p.Active LIKE '%" + cmbPricelist + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (crit.Equals("TransStatusName"))
            else if (crit.Equals("Status"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND t.StatusCode LIKE '%" + cmbSelected + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (crit.Equals("PricelistDate"))
            else if (crit.Equals("Date"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND (CONVERT(VARCHAR(10),p.PricelistDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), p.PricelistDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (crit.Equals("ValidDate"))
            else if (crit.Equals("Valid From"))
            {
                //KALAU MAU PAKAI DATETIMEPICKER
                //Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                //Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                //Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                //Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                //Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                //Query += "AND ((CONVERT(VARCHAR(10),p.ValidFrom,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), p.ValidFrom,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                //Query += "OR (CONVERT(VARCHAR(10),p.ValidTo,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), p.ValidFrom,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "'))) a ";
                //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND p.ValidFrom LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            //else if (crit.Equals("ValidDate"))
            else if (crit.Equals("Valid To"))
            {
                //KALAU MAU PAKAI DATETIMEPICKER
                //Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                //Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                //Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                //Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                //Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                //Query += "AND ((CONVERT(VARCHAR(10),p.ValidFrom,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), p.ValidFrom,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                //Query += "OR (CONVERT(VARCHAR(10),p.ValidTo,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), p.ValidFrom,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "'))) a ";
                //Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";

                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND p.ValidTo LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND (CONVERT(VARCHAR(10),p.CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), p.CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (crit.Equals("Updated Date"))
            {
                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND (CONVERT(VARCHAR(10),p.UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10), p.UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                if (crit.Equals("Created By"))
                    crit = "CreatedBy";
                else if (crit.Equals("Updated By"))
                    crit = "UpdatedBy";

                Query = "SELECT No, PricelistNo, PricelistDate, ValidFrom, ValidTo, DeliveryMethod, PricelistStatus, StatusName, CreatedDate, CreatedBy, ";
                Query += "UpdatedDate, UpdatedBy FROM (SELECT ROW_NUMBER() OVER (ORDER BY p.CreatedDate desc) No, p.PriceListNo, p.PricelistDate, p.ValidFrom, p.ValidTo, ";
                Query += "p.DeliveryMethod, CASE WHEN p.Active = 1 THEN 'Active' ELSE 'Inactive' END AS PricelistStatus, ";
                Query += "UPPER(t.Deskripsi) StatusName, p.CreatedDate, p.CreatedBy, p.UpdatedDate, p.UpdatedBy FROM PricelistH p ";
                Query += "INNER JOIN TransStatusTable t on p.TransStatus = t.StatusCode and t.TransCode='Pricelist' WHERE UPPER(p.Type) = '" + Type + "' AND p.TransStatus IN (" + TransStatus + ") ";
                Query += "AND " + crit + " LIKE '%" + txtSearch.Text + "%') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvPricelist.AutoGenerateColumns = true;
            dgvPricelist.DataSource = Dt;
            dgvPricelist.Refresh();
            dgvPricelist.AutoResizeColumns();

            Conn.Close();
            dgvSetting();

            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(PriceListNo) From PricelistH Where UPPER(Type) = '" + Type + "' AND TransStatus IN (" + TransStatus + ")";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "WHERE UPPER(Type) = '" + Type + "' AND TransStatus IN (" + TransStatus + ") AND (PricelistNo LIKE '%" + txtSearch.Text + "%' OR DeliveryMethod LIKE '%" + txtSearch.Text + "%')";
            }
            //else if (crit.Equals("PricelistNo"))
            else if (crit.Equals("Pricelist No"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "WHERE UPPER(Type) = '" + Type + "' AND TransStatus IN (" + TransStatus + ") AND PriceListNo LIKE '%" + txtSearch.Text + "%'";
            }
            //else if (crit.Equals("DeliveryMethod"))
            else if (crit.Equals("Delivery Method"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "WHERE UPPER(Type) = '" + Type + "' AND TransStatus IN (" + TransStatus + ") AND DeliveryMethod LIKE '%" + txtSearch.Text + "%'";
            }
            //else if (crit.Equals("PricelistStatus"))
            else if (crit.Equals("Active"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "WHERE UPPER(Type) = '" + Type + "' AND TransStatus IN (" + TransStatus + ") AND Active LIKE '%" + cmbPricelist + "%'";
            }
            //else if (crit.Equals("TransStatusName"))
            else if (crit.Equals("Status"))
            {
                Query = "Select Count(PriceListNo) From PricelistH a ";
                Query += "INNER JOIN TransStatusTable b on b.StatusCode = a.TransStatus and TransCode='Pricelist' ";
                Query += "WHERE UPPER(a.Type) = '" + Type + "' AND a.TransStatus IN (" + TransStatus + ") AND b.StatusCode like '%" + cmbSelected + "%' ;";
            }
            //else if (crit.Equals("PricelistDate"))
            else if (crit.Equals("Date"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "Where UPPER(Type) = '" + Type + "' AND (CONVERT(VARCHAR(10),PricelistDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),PricelistDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatus IN (" + TransStatus + ") ";
            }
            //else if (crit.Equals("ValidDate"))
            else if (crit.Equals("Valid From"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "Where UPPER(Type) = '" + Type + "' AND ((CONVERT(VARCHAR(10),ValidFrom,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),ValidFrom,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                Query += "OR (CONVERT(VARCHAR(10),ValidTo,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),ValidTo,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) AND TransStatus IN (" + TransStatus + ") ";
            }
            else if (crit.Equals("Valid To"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "Where UPPER(Type) = '" + Type + "' AND ((CONVERT(VARCHAR(10),ValidFrom,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),ValidFrom,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') ";
                Query += "OR (CONVERT(VARCHAR(10),ValidTo,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),ValidTo,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "')) AND TransStatus IN (" + TransStatus + ") ";
            }
            //else if (crit.Equals("CreatedDate"))
            else if (crit.Equals("Created Date"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "Where UPPER(Type) = '" + Type + "' AND (CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatus IN (" + TransStatus + ") ";
            }
            else if (crit.Equals("Created Date"))
            {
                Query = "Select Count(PriceListNo) From PricelistH ";
                Query += "Where UPPER(Type) = '" + Type + "' AND (CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') AND TransStatus IN (" + TransStatus + ") ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

        }

        private void dgvSetting()
        {
            dgvPricelist.Columns["PricelistNo"].HeaderText = "Pricelist No";
            dgvPricelist.Columns["PricelistDate"].HeaderText = "Date";
            dgvPricelist.Columns["ValidFrom"].HeaderText = "Valid From";
            dgvPricelist.Columns["ValidTo"].HeaderText = "Valid To";
            dgvPricelist.Columns["DeliveryMethod"].HeaderText = "Delivery Method";
            dgvPricelist.Columns["PricelistStatus"].HeaderText = "Active";
            dgvPricelist.Columns["StatusName"].HeaderText = "Status";
            dgvPricelist.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvPricelist.Columns["CreatedBy"].HeaderText = "Created By";
            dgvPricelist.Columns["UpdatedDate"].HeaderText = "Updated Date";
            dgvPricelist.Columns["UpdatedBy"].HeaderText = "Updated By";

            dgvPricelist.Columns["ValidFrom"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPricelist.Columns["ValidTo"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPricelist.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPricelist.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvPricelist.Columns["PricelistDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 08 mei 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvPricelist.RowCount > 0)
                    {
                        Index = dgvPricelist.CurrentRow.Index;
                        string PriceListNo = dgvPricelist.Rows[Index].Cells["PriceListNo"].Value == null ? "" : dgvPricelist.Rows[Index].Cells["PriceListNo"].Value.ToString();

                        Conn = ConnectionString.GetConnection();
                        Trans = Conn.BeginTransaction();

                        Query = "SELECT TransStatus FROM PricelistH WHERE PricelistNo='" + PriceListNo + "'";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        TransStatus = Cmd.ExecuteScalar().ToString();

                        if (TransStatus == "02" || TransStatus == "03" || TransStatus == "04")
                        {
                            MessageBox.Show("Data tidak dapat dihapus karena sudah diproses.");
                            return;
                        }
                        else
                        {
                            DialogResult dr = MessageBox.Show("PriceListNo = " + PriceListNo + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {

                                //delete accountlist
                                Query = "DELETE FROM PriceList_AccountList WHERE PriceListNo ='" + PriceListNo + "';";

                                //delete pricelist detail
                                Query += "DELETE FROM Pricelist_Dtl WHERE PriceListNo ='" + PriceListNo + "';";

                                //delete pricelist header
                                Query += "DELETE FROM PricelistH  WHERE PriceListNo='" + PriceListNo + "';";


                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                Trans.Commit();

                                MessageBox.Show("PriceListNo = " + PriceListNo.ToUpper() + "\n" + "Data berhasil dihapus.");

                                Index = 0;


                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Silahkan pilih data untuk dihapus");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Trans.Rollback();
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Conn.Close();
                    RefreshGrid();
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
            SelectPricelist();
        }

        private void SelectPricelist()
        {
            //begin
            //updated by : joshua
            //updated date : 04 Mei 2018
            //description : check permission access
            ISBS_New.Pricelist.PricelistHeader header = new ISBS_New.Pricelist.PricelistHeader();
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvPricelist.RowCount > 0)
                {
                    header.SetMode("BeforeEdit", dgvPricelist.CurrentRow.Cells["PriceListNo"].Value.ToString(), PricelistType);
                    header.SetParent2(this);
                    header.Show();
                }
                else
                {
                    MessageBox.Show("Silahkan pilih data");
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void dgvPricelist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectPricelist();
        }
    }
}
