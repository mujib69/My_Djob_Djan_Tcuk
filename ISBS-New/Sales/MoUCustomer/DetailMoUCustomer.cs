using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

namespace ISBS_New.Sales.MoUCustomer
{
     public partial class DetailMoUCustomer : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;

        Sales.MoUCustomer.HeaderMoUCustomer Parent;
        DataGridView dgvDetailFromHeader;

        public DetailMoUCustomer()
        {
            InitializeComponent();            
        }

        public void ParentRefreshGrid(Sales.MoUCustomer.HeaderMoUCustomer F)
        {
            Parent = F;
        }

        private void DetailMoUCustomer2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            lblForm.Location = new Point(16, 11);

            cmbCriteria.SelectedIndex = 0;

        }

        private void DetailMoUCustomer2_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            Query = "Select ROW_NUMBER() OVER (ORDER BY CustID asc, CustName asc) No,* From (Select a.CustID, a.CustName, a.MoU_Balance, a.Sisa_Limit_MoU FROM CustTable a ";

            if (cmbCriteria.SelectedIndex == 0)
            {
                Query += "Where CustID like'%" + txtSearch.Text + "%' or CustName like '%" + txtSearch.Text + "%') a;";
            }
            else if (cmbCriteria.SelectedIndex == 1)
            {
               Query += "Where CustID like'%" + txtSearch.Text + "%') a;";
            }
            else
            {
                Query += "Where CustName like '%" + txtSearch.Text + "%') a;";
            }


            Conn = ConnectionString.GetConnection();

          
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvMoUDetailDetails.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvMoUDetailDetails.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);

            dgvMoUDetailDetails.AutoGenerateColumns = true;
            dgvMoUDetailDetails.DataSource = Dt;
            dgvMoUDetailDetails.Refresh();

            dgvMoUDetailDetails.ReadOnly = false;
            dgvMoUDetailDetails.Columns["No"].ReadOnly = true;
            dgvMoUDetailDetails.Columns["CustID"].ReadOnly = true;
            dgvMoUDetailDetails.Columns["CustName"].ReadOnly = true;
            dgvMoUDetailDetails.AutoResizeColumns();

            string CustIDH = "";
            string CustIDD = "";
            List<string> RemoveCustID = new List<string>();

            for (int i = 0; i < dgvMoUDetailDetails.RowCount; i++)
            {
                CustIDD = dgvMoUDetailDetails.Rows[i].Cells["CustID"].Value.ToString();

                for (int j = 0; j < dgvDetailFromHeader.RowCount; j++)
                {
                    CustIDH = dgvDetailFromHeader.Rows[j].Cells["CustID"].Value.ToString();

                    if (CustIDH == CustIDD)
                    {
                        RemoveCustID.Add(CustIDD);
                    }
                }
            }

            for (int i = 0; i < RemoveCustID.Count; i++)
            {
                for (int j = 0; j < dgvMoUDetailDetails.RowCount; j++)
                {
                    CustIDD = dgvMoUDetailDetails.Rows[j].Cells["CustID"].Value.ToString();
                    if (CustIDD == RemoveCustID[i])
                    {
                        dgvMoUDetailDetails.Rows.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < dgvMoUDetailDetails.RowCount; i++)
            {
                dgvMoUDetailDetails.Rows[i].Cells["No"].Value = i + 1;
            }

            Conn.Close();

        }

        public void ParamHeader(DataGridView prmDgvDetailFromHeader)
        {
            dgvDetailFromHeader = prmDgvDetailFromHeader;
        }


        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            RefreshDataGrid();

            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvMoUDetailDetails.RowCount - 1; i++)
            {
                dgvMoUDetailDetails.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            MethodSelectData();
        }

        private void MethodSelectData()
        {
            List<string> CustID = new List<string>();
            List<string> CustName = new List<string>();
            List<string> MoUBalance = new List<string>();
            List<string> SisaLimitMou = new List<string>();

            int CountChk = 0;
            for (int i = 0; i <= dgvMoUDetailDetails.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvMoUDetailDetails.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    CustID.Add(dgvMoUDetailDetails.Rows[i].Cells["CustID"].Value.ToString());
                    CustName.Add(dgvMoUDetailDetails.Rows[i].Cells["CustName"].Value.ToString());
                    MoUBalance.Add(dgvMoUDetailDetails.Rows[i].Cells["MoU_Balance"].Value.ToString());
                    SisaLimitMou.Add(dgvMoUDetailDetails.Rows[i].Cells["Sisa_Limit_Mou"].Value.ToString());
                }
            }

            Parent.AddDataGridDetail(CustID, CustName, MoUBalance, SisaLimitMou);



            this.Close();
        }


    }
}
