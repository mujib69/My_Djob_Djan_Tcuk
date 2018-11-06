using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.VendorReference
{
    public partial class VendorReference : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        public string VendId;

        string Query = "";
        DataGridView dgvVendorReferenceHeader;

        Purchase.PriceListBeli.HeaderPLB Parent;

        public VendorReference()
        {
            InitializeComponent();
        }

        private void VendorReference_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select ROW_NUMBER() OVER (ORDER BY VendID asc) No,* From (Select a.VendID, a.VendName From dbo.[VendTable] a) b ";
          
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvVendorReference.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvVendorReference.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);
            dgvVendorReference.AutoGenerateColumns = true;
            dgvVendorReference.ReadOnly = false;
            dgvVendorReference.DataSource = Dt;
            dgvVendorReference.Refresh();

            dgvVendorReference.Columns["No"].ReadOnly = true;
            dgvVendorReference.Columns["VendID"].ReadOnly = true;
            dgvVendorReference.Columns["VendName"].ReadOnly = true;

            string VendIDH = "";
            string VendIDD = "";
            List<string> RemoveVendID = new List<string>();

            for (int i = 0; i < dgvVendorReference.RowCount; i++)
            {
                VendIDD = dgvVendorReference.Rows[i].Cells["VendID"].Value.ToString();

                for (int j = 0; j < dgvVendorReferenceHeader.RowCount; j++)
                {
                    VendIDH = dgvVendorReferenceHeader.Rows[j].Cells["VendID"].Value.ToString();

                    if (VendIDH == VendIDD)
                    {
                        RemoveVendID.Add(VendIDD);
                    }
                }
            }

            for (int i = 0; i < RemoveVendID.Count; i++)
            {
                for (int j = 0; j < dgvVendorReference.RowCount; j++)
                {
                    VendIDD = dgvVendorReference.Rows[j].Cells["VendID"].Value.ToString();
                    if (VendIDD == RemoveVendID[i])
                    {
                        dgvVendorReference.Rows.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < dgvVendorReference.RowCount; i++)
            {
                dgvVendorReference.Rows[i].Cells["No"].Value = i + 1;
            }

            

            Conn.Close();
           
        }

        public void ParamHeader(DataGridView prmDgvVendorFromHeader)
        {
            dgvVendorReferenceHeader = prmDgvVendorFromHeader;
        }

        private void VendorReference_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> VendID = new List<string>();
            List<string> VendName = new List<string>();

            int CountChk = 0;
            for (int i = 0; i <= dgvVendorReference.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvVendorReference.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    VendID.Add(dgvVendorReference.Rows[i].Cells["VendID"].Value.ToString());
                    VendName.Add(dgvVendorReference.Rows[i].Cells["VendName"].Value.ToString());
                }
            }

            Parent.AddDataGridVendor(VendID, VendName);
            this.Close();
        }

        public void ParentRefreshGrid(Purchase.PriceListBeli.HeaderPLB F)
        {
            Parent = F;
        }

    }
}
