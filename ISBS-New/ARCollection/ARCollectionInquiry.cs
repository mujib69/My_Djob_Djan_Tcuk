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

namespace ISBS_New.ARCollection
{
    public partial class ARCollectionInquiry : MetroFramework.Forms.MetroForm
    {
        SqlConnection Conn;
        SqlCommand Cmd;
        SqlDataReader Dr;
        TransactionScope scope;

        string Query;
        string Mode; //ad 3 mode, "TabApproved","TabManual","TabEmail"
        string SubMode = "Belom Ditagih"; // ada 4 mode, "BelomDitagih","ProsesTagih","Tertagih","TertagihPartial"
        int limit1;
        int limit2;

        public ARCollectionInquiry()
        {
            InitializeComponent();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void ARCollectionInquiry_Load(object sender, EventArgs e)
        {
            txtPage.Text = "1";  
            addCrit();
            addCmbShow();
            addRowsCount();
            btnTabApproved_Click(sender, e);
        }

        private void addCmbShow()
        {
            Query = "SELECT CmbValue FROM [ISBS-NEW4].[Setting].[CmbBox]";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    cmbShow.Items.Add(Dr["CmbValue"]);
                }
                Dr.Close();
            }
            cmbShow.SelectedIndex = 0;
        }

        private void addCrit()
        {
            cmbCriteria.Items.Add("All");
            cmbCriteria.SelectedIndex = 0;
            Query = "SELECT DisplayName FROM [ISBS-NEW4].[User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'CustInvoiceH' AND [OrderNo] IN (1,3,5,6)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    cmbCriteria.Items.Add(Dr["DisplayName"]);
                }
                Dr.Close();
            }
        }

        private int addRowsCount()
        {
            //getting the number of total rows
            int TotalRow = 0;
            Query = "SELECT COUNT(*) FROM [CustInvoice_Dtl] a left join [CustInvoice_H] b ON a.[InvoiceId]=b.[Invoice_Id] WHERE ";
            if (cmbCriteria.Text.Contains("Date"))
            {
                Query += " " + getFieldName() + " BETWEEN " + dtFrom + " AND " + dtTo + " AND ";
            }
            else if (cmbCriteria.Text == "All")
            {
                //string QueryTemp = "SELECT [FieldName] FROM [User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'CustInvoiceH' ";
                //using (Cmd = new SqlCommand(QueryTemp, ConnectionString.GetConnection()))
                //{
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        Query += " "+Dr["FieldName"]+" LIKE @txtSearch AND ";
                //    }
                //    Dr.Close();
                //}
            }
            else if (txtSearch != null && txtSearch.Text.Trim() != "")
            {
                Query += " " + getFieldName() + " LIKE @txtSearch AND ";
            }
            Query += " [TransStatus] = '03'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@txtSearch", "%" + txtSearch.Text + "%");
                TotalRow = Convert.ToInt32(Cmd.ExecuteScalar());
            }
            lblTotal.Text = "Total Rows :" + TotalRow.ToString();
            double cmbshow = Convert.ToDouble(cmbShow.Text.ToString());
            lblPage.Text = "/ " + Convert.ToInt32(Math.Ceiling(Convert.ToDouble(TotalRow) / cmbshow)).ToString();

            return TotalRow;
        }

        public void refresh2()
        {
            refreshGrid();
        }

        private void refreshGrid()
        {
            if (Mode == "TabApproved")
            {
                refreshGridTabInvoice();
            }
            else
            {
                switch (SubMode)
                {
                    case "BelomDitagih":
                        refreshGridBelomDitagih();
                        break;
                    case "ProsesTagih":
                        refreshGridProsesTagih();
                        break;
                    case "Tertagih":
                        refreshGridTertagih();
                        break;
                    case "TertagihPartial":
                        refreshGridTertagihPartial();
                        break;
                }
            }
        }

        private void refreshGridBelomDitagih()
        {
            
        }
        private void refreshGridProsesTagih()
        {

        }
        private void refreshGridTertagih()
        {

        }
        private void refreshGridTertagihPartial()
        {

        }


        private string getFieldName()
        {
            //getting the fieldname from displayname(cmb cirteria)
            string FieldName = "";
            if (cmbCriteria != null && cmbCriteria.Text.Trim() != "" && cmbCriteria.Text != "All")
            {
                Query = "SELECT [FieldName] FROM [User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'CustInvoiceH' AND [DisplayName] = '" + cmbCriteria.Text + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    FieldName = Cmd.ExecuteScalar().ToString();
                }
            }
            return FieldName;
        }

        private void refreshGridTabInvoice()
        {
            limit1 = (Convert.ToInt32(txtPage.Text) * Convert.ToInt32(cmbShow.Text)) - (Convert.ToInt32(cmbShow.Text) - 1);
            limit2 = limit1 + Convert.ToInt32(cmbShow.Text);

            dgvApprovedInvoice.Rows.Clear();
            if (dgvApprovedInvoice.RowCount - 1 <= 0)
            {
                dgvApprovedInvoice.ColumnCount = 15;
                dgvApprovedInvoice.Columns[0].Name = "No";
                dgvApprovedInvoice.Columns[1].Name = "Customer";
                dgvApprovedInvoice.Columns[2].Name = "Invoice Id";
                dgvApprovedInvoice.Columns[3].Name = "Invoice Amount";
                dgvApprovedInvoice.Columns[4].Name = "Due Date";
                dgvApprovedInvoice.Columns[5].Name = "Payment Method";
                dgvApprovedInvoice.Columns[6].Name = "SO Id";
                dgvApprovedInvoice.Columns[7].Name = "SO Amount";
                dgvApprovedInvoice.Columns[8].Name = "CN Id";
                dgvApprovedInvoice.Columns[9].Name = "CN Amount";
                dgvApprovedInvoice.Columns[10].Name = "Approval Notes";
                dgvApprovedInvoice.Columns[11].Name = "Created By";
                dgvApprovedInvoice.Columns[12].Name = "Created Date";
                dgvApprovedInvoice.Columns[13].Name = "Updated By";
                dgvApprovedInvoice.Columns[14].Name = "Updated Date";
            }

            Query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (order by CASE WHEN a.CreatedDate >= a.UpdatedDate THEN a.CreatedDate ELSE a.UpdatedDate END DESC) No, ";
            Query += " a.*,";
            Query += " b.Cust_Name,b.Invoice_Date,b.Notes,b.Payment_Method ";
            Query += " FROM [CustInvoice_Dtl] a left join [CustInvoice_H] b ON a.[InvoiceId]=b.[Invoice_Id] WHERE b.TransStatus = '03' ";
            if (cmbCriteria.Text.Contains("Date"))
            {
                Query += " AND " + getFieldName() + " BETWEEN " + dtFrom + " AND " + dtTo + " ";
            }
            else if (cmbCriteria.Text == "All")
            {
                //string QueryTemp = "SELECT [FieldName] FROM [User].[Table] WHERE [SchemaName] = 'dbo' AND [TableName] = 'CustInvoiceH' ";
                //using (Cmd = new SqlCommand(QueryTemp, ConnectionString.GetConnection()))
                //{
                //    Dr = Cmd.ExecuteReader();
                //    while (Dr.Read())
                //    {
                //        Query += " " + Dr["FieldName"] + " LIKE @txtSearch AND ";
                //    }
                //    Dr.Close();
                //}
            }
            else if (txtSearch != null && txtSearch.Text.Trim() != "")
            {
                Query += " AND " + getFieldName() + " LIKE @txtSearch ";
            }
            Query += ") a ";
            Query += " WHERE a.No BETWEEN " + limit1 + " AND " + limit2 + " ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@txtSearch", "%"+txtSearch.Text+"%");
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    for (int i = 0; i < Convert.ToInt32(cmbShow.Text); i++)
                    {
                        dgvApprovedInvoice.Rows.Add(Dr["No"], Dr["Cust_Name"], Dr["InvoiceId"], Dr["Invoice_Amount"], Dr["Invoice_Date"], Dr["Payment_Method"], Dr["SO_Id"], Dr["SO_Amount"], "", "", Dr["Notes"], Dr["CreatedBy"], Dr["CreatedDate"], Dr["UpdatedBy"], Dr["UpdatedDate"]);
                    }
                }
                Dr.Close();
            }
        }

        private void btnTabApproved_Click(object sender, EventArgs e)
        {
            metroTabControl1.Hide();
            btnTabApproved.BackColor = Color.Black;
            btnTabApproved.ForeColor = Color.White;
            btnTabManual.BackColor = Color.White;
            btnTabManual.ForeColor = Color.Black;
            btnTabEmail.BackColor = Color.White;
            btnTabEmail.ForeColor = Color.Black;
            Mode = "TabApproved";
            refreshGrid();
        }

        private void btnTabManual_Click(object sender, EventArgs e)
        {
            metroTabControl1.Show();
            metroTabControl1.SelectedTab = metroTabPage1;
            btnTabApproved.BackColor = Color.White;
            btnTabApproved.ForeColor = Color.Black;
            btnTabManual.BackColor = Color.Black;
            btnTabManual.ForeColor = Color.White;
            btnTabEmail.BackColor = Color.White;
            btnTabEmail.ForeColor = Color.Black;
            Mode = "TabManual";
            refreshGrid();
        }

        private void btnTabEmail_Click(object sender, EventArgs e)
        {
            metroTabControl1.Show();
            metroTabControl1.SelectedTab = metroTabPage1;
            btnTabApproved.BackColor = Color.White;
            btnTabApproved.ForeColor = Color.Black;
            btnTabManual.BackColor = Color.White;
            btnTabManual.ForeColor = Color.Black;
            btnTabEmail.BackColor = Color.Black;
            btnTabEmail.ForeColor = Color.White;
            Mode = "TabEmail";
            refreshGrid();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (Mode == "TabApproved")
            {
                if (dgvApprovedInvoice.Rows.Count > 0)
                {
                    PassData();
                }
            }
            else
            {
                switch (SubMode)
                {
                    case "BelomDitagih":
                        if (dgvBelomTagih.Rows.Count > 0)
                        {
                            PassData();
                        }
                        break;
                    case "ProsesTagih":
                        if (dgvProsesTagih.Rows.Count > 0)
                        {
                            PassData();
                        }
                        break;
                    case "Tertagih":
                        if (dgvTertagih.Rows.Count > 0)
                        {
                            PassData();
                        }
                        break;
                    case "TertagihPartial":
                        if (dgvTertagihPartial.Rows.Count > 0)
                        {
                            PassData();
                        }
                        break;
                }
            }
        }

        private void PassData()
        {
            ARCollection.ARCollectionForm F = new ARCollectionForm("View",passingData());
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Parent(this);
                F.Show();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ARCollection.ARCollectionForm F = new ARCollectionForm("New");
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Parent(this);
                F.ShowDialog();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private List<string> passingData()
        {
            int index = 0;
            string Customer = "";
            string InvoiceId = "";
            string InvoiceAmount = "";
            string DueDate = "";
            string PaymentMethod = "";
            string ApprovalNotes = "";
            if (Mode == "TabApproved")
            {
                 index = dgvApprovedInvoice.CurrentRow.Index;
                 Customer = dgvApprovedInvoice.Rows[index].Cells["Customer"].Value.ToString();
                 InvoiceId = dgvApprovedInvoice.Rows[index].Cells["Invoice Id"].Value.ToString();
                 InvoiceAmount = dgvApprovedInvoice.Rows[index].Cells["Invoice Amount"].Value.ToString();
                 DueDate = dgvApprovedInvoice.Rows[index].Cells["Due Date"].Value.ToString();
                 PaymentMethod = dgvApprovedInvoice.Rows[index].Cells["Payment Method"].Value.ToString();
                 ApprovalNotes = dgvApprovedInvoice.Rows[index].Cells["Approval Notes"].Value.ToString();
            }
            else
            {
                switch (SubMode)
                {
                    case "BelomDitagih":
                        {
                             index = dgvBelomTagih.CurrentRow.Index;
                             Customer = dgvBelomTagih.Rows[index].Cells["Customer"].Value.ToString();
                             InvoiceId = dgvBelomTagih.Rows[index].Cells["Invoice Id"].Value.ToString();
                             InvoiceAmount = dgvBelomTagih.Rows[index].Cells["Invoice Amount"].Value.ToString();
                             DueDate = dgvBelomTagih.Rows[index].Cells["Due Date"].Value.ToString();
                             PaymentMethod = dgvBelomTagih.Rows[index].Cells["Payment Method"].Value.ToString();
                             ApprovalNotes = dgvBelomTagih.Rows[index].Cells["Approval Notes"].Value.ToString();
                        }
                        break;
                    case "ProsesTagih":
                        {
                             index = dgvProsesTagih.CurrentRow.Index;
                             Customer = dgvProsesTagih.Rows[index].Cells["Customer"].Value.ToString();
                             InvoiceId = dgvProsesTagih.Rows[index].Cells["Invoice Id"].Value.ToString();
                             InvoiceAmount = dgvProsesTagih.Rows[index].Cells["Invoice Amount"].Value.ToString();
                             DueDate = dgvProsesTagih.Rows[index].Cells["Due Date"].Value.ToString();
                             PaymentMethod = dgvProsesTagih.Rows[index].Cells["Payment Method"].Value.ToString();
                             ApprovalNotes = dgvProsesTagih.Rows[index].Cells["Approval Notes"].Value.ToString();
                        }
                        break;
                    case "Tertagih":
                        {
                             index = dgvTertagih.CurrentRow.Index;
                             Customer = dgvTertagih.Rows[index].Cells["Customer"].Value.ToString();
                             InvoiceId = dgvTertagih.Rows[index].Cells["Invoice Id"].Value.ToString();
                             InvoiceAmount = dgvTertagih.Rows[index].Cells["Invoice Amount"].Value.ToString();
                             DueDate = dgvTertagih.Rows[index].Cells["Due Date"].Value.ToString();
                             PaymentMethod = dgvTertagih.Rows[index].Cells["Payment Method"].Value.ToString();
                             ApprovalNotes = dgvTertagih.Rows[index].Cells["Approval Notes"].Value.ToString();
                        }
                        break;
                    case "TertagihPartial":
                        {
                             index = dgvTertagihPartial.CurrentRow.Index;
                             Customer = dgvTertagihPartial.Rows[index].Cells["Customer"].Value.ToString();
                             InvoiceId = dgvTertagihPartial.Rows[index].Cells["Invoice Id"].Value.ToString();
                             InvoiceAmount = dgvTertagihPartial.Rows[index].Cells["Invoice Amount"].Value.ToString();
                             DueDate = dgvTertagihPartial.Rows[index].Cells["Due Date"].Value.ToString();
                             PaymentMethod = dgvTertagihPartial.Rows[index].Cells["Payment Method"].Value.ToString();
                             ApprovalNotes = dgvTertagihPartial.Rows[index].Cells["Approval Notes"].Value.ToString();
                        }
                        break;
                }
            }
            List<string> passingdata = new List<string> {Customer,InvoiceId,InvoiceAmount,DueDate,PaymentMethod,ApprovalNotes };
            return passingdata;
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (metroTabControl1.SelectedIndex)
            {
                case 0 :
                    SubMode = "BelomDitagih";
                    break;
                case 1 :
                    SubMode = "ProsesTagih";
                    break;
                case 2 :
                    SubMode = "Tertagih";
                    break;
                case 3 :
                    SubMode = "TertagihPartial";
                    break;
            }
            refreshGrid();
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            double cmbshow = Convert.ToDouble(cmbShow.Text);
            int lastpage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(addRowsCount()) / cmbshow));
            if (Convert.ToInt32(txtPage.Text) < lastpage)
            {
                int NewPage = Convert.ToInt32(txtPage.Text) + 1;
                txtPage.Text = NewPage.ToString();
                refreshGrid();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > 1)
            {
                int NewPage = Convert.ToInt32(txtPage.Text) - 1;
                txtPage.Text = NewPage.ToString();
                refreshGrid();
            }
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            double cmbshow = Convert.ToDouble(cmbShow.Text);
            int lastpage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(addRowsCount()) / cmbshow));
            txtPage.Text = lastpage.ToString();
            refreshGrid();
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            refreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            cmbCriteria.SelectedIndex = 0;
            refreshGrid();
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshGrid();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            int total = addRowsCount();
            if (Convert.ToInt32(txtPage.Text) > total)
            {
                txtPage.Text = total.ToString();
            }
            if (e.KeyChar == (char)13)
            {
                limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                refreshGrid();
            }
            else if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
