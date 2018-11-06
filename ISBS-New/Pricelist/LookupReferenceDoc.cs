using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Pricelist
{
    public partial class LookupReferenceDoc : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Query = null;
        String PricelistType = "";

        Pricelist.PricelistHeader Parent;

        public LookupReferenceDoc()
        {
            InitializeComponent();           
        }

        public void ParentRefreshGrid(Pricelist.PricelistHeader F)
        {
            Parent = F;
        }

        private void LookupReferenceDoc_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //cmbCriteria.SelectedIndex = 0;
            addCmbCrit();
            cmbCriteria.SelectedItem = "All";
            RefreshDataGrid();
        }

        private void LookupReferenceDoc_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        public void ParamHeader(string prmPricelistType)
        {
            PricelistType = prmPricelistType;
        }

        private void addCmbCrit()
        {
            try
            {
                cmbCriteria.Items.Add("All");
                Conn = ConnectionString.GetConnection();
                Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'ReferenceDoc' order by OrderNo";

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

        private void RefreshDataGrid()
        {
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
            //Thaddaeus, edited
            Query = "SELECT No, PricelistNo, PricelistDate, ValidTo, DeliveryMethod FROM (SELECT ROW_NUMBER() OVER (ORDER BY PricelistNo ASC) No, PricelistNo, PricelistDate, ValidTo, DeliveryMethod FROM PricelistH WHERE Active = 1 AND TransStatus = '03' AND UPPER(Type) = '" + Type + "') a WHERE ";
           
            //if (cmbCriteria.SelectedIndex == 0)
            if (cmbCriteria.Text == "All")
            {
                Query += "(PricelistNo like @search or PricelistDate like @search or DeliveryMethod like @search or ValidTo like @search) ";
            }
            //else if (cmbCriteria.SelectedIndex == 1)
            //{
            //    Query += "(PricelistNo like@search) ";
            //}
            else
            {
                string QueryTemp = "SELECT FieldName FROM [User].[Table] WHERE DisplayName = '" + cmbCriteria.Text + "'";
                Cmd = new SqlCommand(QueryTemp, Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query += "(" + crit + " like @search) ";
                //Query += "(DeliveryMethod like @search) ";
            }

            //Conn = ConnectionString.GetConnection();


            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            //end=======
            Dt = new DataTable();
            if (dgvReferenceDoc.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvReferenceDoc.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);

            dgvReferenceDoc.AutoGenerateColumns = true;
            dgvReferenceDoc.DataSource = Dt;
            dgvReferenceDoc.Refresh();
            dgvSetting();
            
            dgvReferenceDoc.AutoResizeColumns();

            Conn.Close();
        }

        private void dgvSetting()
        {
            dgvReferenceDoc.ReadOnly = false;
            dgvReferenceDoc.Columns["No"].ReadOnly = true;
            dgvReferenceDoc.Columns["PricelistNo"].ReadOnly = true;
            dgvReferenceDoc.Columns["PricelistDate"].ReadOnly = true;
            dgvReferenceDoc.Columns["ValidTo"].ReadOnly = true;
            dgvReferenceDoc.Columns["DeliveryMethod"].ReadOnly = true;

            dgvReferenceDoc.Columns["PricelistNo"].HeaderText = "Pricelist No";
            dgvReferenceDoc.Columns["PricelistDate"].HeaderText = "Pricelist Date";
            dgvReferenceDoc.Columns["ValidTo"].HeaderText = "Valid To";
            dgvReferenceDoc.Columns["DeliveryMethod"].HeaderText = "Delivery Method";

            dgvReferenceDoc.Columns["PricelistDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvReferenceDoc.Columns["ValidTo"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            MethodSelectData();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDataGrid();

            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvReferenceDoc.RowCount - 1; i++)
            {
                dgvReferenceDoc.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void MethodSelectData()
        {
            int DataChecked = 0;
            for (int i = 0; i <= dgvReferenceDoc.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvReferenceDoc.Rows[i].Cells["chk"].Value);               
                if (Check == true)
                {

                    if (DataChecked > 1)
                    {
                        MessageBox.Show("Maksimal Reference Doc adalah 2 data");
                        break;
                    }

                    if (DataChecked == 0)
                    {
                        Parent.lblReferenceDoc1.Text = "1. " + dgvReferenceDoc.Rows[i].Cells["PricelistNo"].Value.ToString();
                        Parent.lblReferenceDoc2.Text = "2.";                    
                    }
                    else
                    {
                         Parent.lblReferenceDoc2.Text = "2. " + dgvReferenceDoc.Rows[i].Cells["PricelistNo"].Value.ToString();
                    }

                    DataChecked++;
                 }                
            }

            if (DataChecked == 0)
            {
                //MessageBox.Show("Silahkan checklist data");
               // return;
                Parent.lblReferenceDoc1.Text = "1.";
                Parent.lblReferenceDoc2.Text = "2.";
            }

            this.Close();
        }

        private void dgvReferenceDoc_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                string PricelistNo = dgvReferenceDoc.Rows[e.RowIndex].Cells["PricelistNo"].Value.ToString();
                Pricelist.PricelistHeader PH = new Pricelist.PricelistHeader();
                PH.SetMode("View", PricelistNo, PricelistType);
                PH.SetParentView(this);
                PH.Show();
                
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
