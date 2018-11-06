using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Pricelist
{
    public partial class LookupCustomer : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        public string CustId;

        string Query = "";
        DataGridView dgvCustomerHeader;

        Pricelist.PricelistHeader Parent;

        public LookupCustomer()
        {
            InitializeComponent();
        }

        private void LookupCustomer_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select ROW_NUMBER() OVER (ORDER BY CustID asc) No,* From (Select a.CustID, a.CustName From dbo.[CustTable] a) b ";
          
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvCustomer.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvCustomer.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);

            dgvCustomer.AutoGenerateColumns = true;
            dgvCustomer.ReadOnly = false;
            dgvCustomer.DataSource = Dt;
            dgvCustomer.Refresh();

            dgvCustomer.Columns["No"].ReadOnly = true;
            dgvCustomer.Columns["CustID"].ReadOnly = true;
            dgvCustomer.Columns["CustName"].ReadOnly = true;


            string CustIDH = "";
            string CustIDD = "";
            List<string> RemoveCustID = new List<string>();

            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {
                CustIDD = dgvCustomer.Rows[i].Cells["CustID"].Value.ToString();

                for (int j = 0; j < dgvCustomerHeader.RowCount; j++)
                {
                    CustIDH = dgvCustomerHeader.Rows[j].Cells["CustID"].Value.ToString();

                    if (CustIDH == CustIDD)
                    {
                        RemoveCustID.Add(CustIDD);
                    }
                }
            }

            for (int i = 0; i < RemoveCustID.Count; i++)
            {
                for (int j = 0; j < dgvCustomer.RowCount; j++)
                {
                    CustIDD = dgvCustomer.Rows[j].Cells["CustID"].Value.ToString();
                    if (CustIDD == RemoveCustID[i])
                    {
                        dgvCustomer.Rows.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < dgvCustomer.RowCount; i++)
            {
                dgvCustomer.Rows[i].Cells["No"].Value = i + 1;
            }           

            Conn.Close();
           
        }

        public void ParamHeader(DataGridView prmDgvCustomerFromHeader)
        {
            dgvCustomerHeader = prmDgvCustomerFromHeader;
        }

        private void LookupCustomer_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> CustID = new List<string>();
            List<string> CustName = new List<string>();

            int CountChk = 0;          
       
            for (int i = 0; i <= dgvCustomer.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvCustomer.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    CustID.Add(dgvCustomer.Rows[i].Cells["CustID"].Value.ToString());
                    CustName.Add(dgvCustomer.Rows[i].Cells["CustName"].Value.ToString());
                }
            }

            if (CountChk == 0)
            {
                MessageBox.Show("Silahkan checklist data");
                return;
            }

            Parent.AddDataGridCustomer(CustID, CustName);
            this.Close();
        }

        public void ParentRefreshGrid(Pricelist.PricelistHeader F)
        {
            Parent = F;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
