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

namespace ISBS_New.Sales.DeliveryOrder
{
    public partial class RequestCancelDO : MetroFramework.Forms.MetroForm
    {
        string Query;
        SqlCommand Cmd;
        SqlDataReader Dr;
        private TransactionScope scope;

        public RequestCancelDO()
        {
            InitializeComponent();
            ModeLoad();
        }

        private void ModeLoad()
        {
            dtpCancelDate.Enabled = false;
        }

        private Boolean Validasi(string DONo)
        {
            bool vBol = true;           

            Query = "SELECT SUM(Qty) AS TotalQty, SUM(RemainingQty) AS TotalRemaining FROM DeliveryOrderD WHERE DeliveryOrderId = '" + DONo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    decimal TotalQty = Convert.ToDecimal(Dr["TotalQty"].ToString());
                    decimal TotalRemaining = Convert.ToDecimal(Dr["TotalRemaining"].ToString());

                    if (TotalQty != TotalRemaining)
                    {
                        MessageBox.Show("Tidak bisa cancel" + Environment.NewLine + "Qty tidak sama dengan RemainingQty!");
                        vBol = false;
                    }
                }         
            }
            return vBol;
        }

        private void btnAddDO_Click(object sender, EventArgs e)
        {          
            string Table = "[dbo].[DeliveryOrderH]";

            Query = "SELECT [DeliveryOrderId], [DeliveryOrderDate], [CustName] FROM [DeliveryOrderH] WHERE ";

            //include DO yang belom tarik GI (Qty sama dengan RemainingQty)
            Query += "[DeliveryOrderId] IN (SELECT a.DeliveryOrderId FROM DeliveryOrderH a ";
            Query += "LEFT JOIN DeliveryOrderD b ON a.DeliveryOrderId = b.DeliveryOrderId ";
            Query += "GROUP BY a.DeliveryOrderId ";
            Query += "HAVING SUM(b.Qty) = SUM(b.RemainingQty)) ";

            //exclude DO deleted + cancelled
            Query += "AND [DeliveryOrderId] NOT IN (SELECT DeliveryOrderId FROM DeliveryOrderH ";
            Query += "WHERE [DeliveryOrderStatus] IN ('07','11','12','13')) ";
            
            //exclude DO yang sudah di grid
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                Query += "AND DeliveryOrderId != '" + dataGridView1.Rows[i].Cells["DeliveryOrderId"].Value.ToString() + "' ";
            }

            string[] FilterText = { "DeliveryOrderId", "DeliveryOrderDate", "CustName" };
            string[] Mask = { "DeliveryOrderId", "DeliveryOrderDate", "CustName" };
            string[] Select = { "DeliveryOrderId" };
            string PrimaryKey = "DeliveryOrderId";
            string[] HideField = { };
            callSearchQueryV2Form(Table, Query, FilterText, Mask, Select, PrimaryKey, HideField);
        }

        private void callSearchQueryV2Form(string Table, string QuerySearch, string[] FilterText, string[] Mask, string[] Select, string PrimaryKey, string[] HideField)
        {
            ISBS_New.SearchQueryV2 F = new SearchQueryV2();
            F.Table = Table;
            F.QuerySearch = QuerySearch;
            F.FilterText = FilterText;
            F.Mask = Mask;
            F.Select = Select;
            F.PrimaryKey = PrimaryKey;
            F.HideField = HideField;
            F.Parent = this;
            F.ShowDialog();

            populateAfterSearch(Table);
        }

        private void populateAfterSearch(string Table)
        {
            if (Variable.Kode2 == null)
            {
                return;
            }
            if (Table == "[dbo].[DeliveryOrderH]")
            {
                using (Method C = new Method())
                {
                    if (dataGridView1.Rows.Count <= 0)
                    {
                        dataGridView1.ColumnCount = 1;
                        dataGridView1.Columns[0].Name = "DeliveryOrderId";
                    }

                    for (int i = 0; i <= ((Variable.Kode2.GetUpperBound(0))); i++)
                    {
                        dataGridView1.Rows.Add(Variable.Kode2[i, 0]);
                    }

                    dataGridView1.ReadOnly = false;
                    string[] read = new string[] { "DeliveryOrderId" };
                    for (int i = 0; i < read.Length; i++)
                    {
                        dataGridView1.Columns[read[i]].ReadOnly = true;
                    }

                    dataGridView1.AutoResizeColumns();
                    Variable.Kode2 = null;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {           
            try
            {
                using (scope = new TransactionScope())
                {
                    if (dataGridView1.Rows.Count < 1)
                    {
                        MessageBox.Show("Pilih DO terlebih dahulu!");
                        return;
                    }

                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        string DONo = dataGridView1.Rows[i].Cells["DeliveryOrderId"].Value.ToString();

                        if (!Validasi(DONo))
                            return;

                        Query = "UPDATE [DeliveryOrderH] SET [DeliveryOrderStatus] = '11',";
                        Query += "[RequestCancelDate] = getdate(),";
                        Query += "[RequestCancelBy] = '" + ControlMgr.UserId + "' ";
                        Query += "WHERE DeliveryOrderId = '" + DONo + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            Cmd.ExecuteNonQuery();
                    }
                    //MessageBox.Show("Request Cancel DO Initiated - Waiting For Approval SM");
                    //btnSave.Enabled = false;

                    DialogResult dialogResult = MessageBox.Show("Request Cancel DO Initiated - Waiting For Approval SM" + Environment.NewLine + "Do you want to cancel another DO?", "Success!" , MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        dataGridView1.Rows.Clear();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        this.Close();
                    }
                    scope.Complete();
                }
            }
            catch (Exception Ex)
            {
                MetroFramework.MetroMessageBox.Show(this, Ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally { }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count < 1)
                return;

            int Index = dataGridView1.CurrentRow.Index; 
            DialogResult dr = MessageBox.Show("Apakah Anda Ingin Menghapus " + dataGridView1.Rows[Index].Cells["DeliveryOrderId"].Value.ToString() + "?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                if (dataGridView1.RowCount > 0)                
                    dataGridView1.Rows.RemoveAt(Index);                
            }
        }
    }
}
