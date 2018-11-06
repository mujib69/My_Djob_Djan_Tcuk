using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUpSelect
{
    public partial class Vendor : MetroFramework.Forms.MetroForm
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

        Purchase.PurchaseRequisition.HeaderPR Parent;

        public Vendor()
        {
            InitializeComponent();
        }

        private void Vendor_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            //Query = "Select [GelombangId], [BracketId], ItemId, ItemName, [Base], [Price] From dbo.[InventGelombangD] where GelombangId+BracketId in (Select GelombangId+BracketId From dbo.[InventGelombangD] ";
            //Query = "Select b.GroupId, b.SubGroup1Id, b.SubGroup2Id, a.[GelombangId], a.[BracketId], b.ItemId, b.FullItemId, a.ItemName, a.[Base], case when a.Price is null then '0.00' end Price, c.VendId ";
            //Query += "From dbo.[InventGelombangD] a Left Join dbo.[InventTable] b on a.[ItemId]=b.FullItemId left join dbo.InventGelombangVendor c on b.GelombangId=c.GelombangId and b.BracketId=c.BracketId ";
            //Query += "Where GelombangId='" + Gelombang1 + "' and BracketId='" + Bracket + "' order by GelombangId asc, BracketId asc, Base desc ";
            Query = "Select VendId,VendName,TaxName,NPWP,PKP,SIUP,TermOfPayment,PaymentModeId,CurrencyId,Sisa_Limit_Total,Deposito From VendTable ";
            
            //if (VendId.Count >= 0)
            //{
            //    Query += "Where VendId not in {";
            //    for (int i = 0; i < VendId.Count(); i++ )
            //    {
            //        if(i==0)
            //            Query += "'" + VendId[i].ToString() + "'";
            //        else
            //            Query += ",'" + VendId[i].ToString() + "'";
            //    }
            //    Query += "}";
            //}

            if (dgvVendor.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                chk.HeaderText = "Check";
                chk.Name = "chk";
                dgvVendor.Columns.Add(chk);
            }

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvVendor.AutoGenerateColumns = true;
            dgvVendor.DataSource = Dt;
            dgvVendor.Refresh();
            
            Conn.Close();

            GetVendorList();
            dgvVendor.ReadOnly = false;
            dgvVendor.Columns["VendName"].ReadOnly = false;
            dgvVendor.AutoResizeColumns();
           
        }

        private void Vendor_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            VendId = "";

            for (int i = 0; i < dgvVendor.RowCount; i++)
            {
                if (dgvVendor.Rows[i].Cells["chk"].Value != null)
                {
                    if ((bool)dgvVendor.Rows[i].Cells["chk"].Value == true)
                    {
                        if (VendId == "")
                            VendId += dgvVendor.Rows[i].Cells["VendId"].Value.ToString();
                        else
                            VendId += ";" + dgvVendor.Rows[i].Cells["VendId"].Value.ToString();
                    }
                }
            }
            this.Close();
        }

        private void GetVendorList()
        {
            string s = VendId;
            string[] words = s.Split(';');

            for(int i=0; i < words.Count(); i++)
            {
                for(int j=0; j<dgvVendor.RowCount; j++)
                {
                    if (dgvVendor.Rows[j].Cells["VendId"].Value.ToString() == words[i])
                    {
                        dgvVendor.Rows[j].Cells["chk"].Value = true ;
                    }
                }
            }
            //VendId = TmpVendId;
            //foreach (string word in words)
            //{
            //}
        }

    }
}
