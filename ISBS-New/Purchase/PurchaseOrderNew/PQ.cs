using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseOrderNew
{
    public partial class PQ : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Query = null;

        string CSId = "";
        Purchase.PurchaseOrderNew.POForm Parent;

        public PQ()
        {
            InitializeComponent();
        }

        public void GetCSId(string tmpCSId)
        {
            CSId = tmpCSId;
        }

        public void SetParentForm(Purchase.PurchaseOrderNew.POForm F)
        {
            Parent = F;
        }

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            string QuotID = "";

            Query = "Select ReffId2 From PurchH Where ReffId = '" + CSId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                if (QuotID == "")
                {
                    QuotID = "'" + Dr[0].ToString() + "'";
                }
                else
                {
                    QuotID += ",";
                    QuotID += "'" + Dr[0].ToString() + "'";
                }

            }
            Dr.Close();
            if (QuotID == "")
            {
                QuotID = "''";
            }

            #region Refresh dgvPQ
            //Query = "Select DISTINCT PurchQuotId from [dbo].[CanvasSheetD] Where PurchQuotId Not in (select ReffId2 from PurchH where ReffTableName='CanvasSheet') and CanvasId = '" + CSId + "' and StatusApproval = 'YES' and Qty > '0' ";

            //STV Edit
            Query = "SELECT DISTINCT CSd.PurchQuotId, PQh.OrderDate, VT.VendName, CSd.PurchReqId, PRh.OrderDate, CSd.CreatedBy, CSd.CreatedDate, CSd.UpdatedBy, CSd.UpdatedDate ";
            Query += "FROM [dbo].[CanvasSheetD] CSd ";
            Query += "LEFT JOIN PurchRequisitionH PRh ON CSd.PurchReqId = PRh.PurchReqId ";
            Query += "LEFT JOIN VendTable VT ON CSd.VendID = VT.VendId ";
            Query += "LEFT JOIN PurchQuotationH PQh ON CSd.PurchQuotId = PQh.PurchQuotID ";
            Query += "WHERE CSd.PurchQuotId Not in (SELECT ReffId2 from PurchH WHERE ReffTableName='CanvasSheet') ";
            Query += "AND CanvasId = '" + CSId + "' AND CSd.PurchQuotId NOT IN(" + QuotID + ") ";
            Query += "AND StatusApproval = 'YES' AND Qty > '0' ";
            //End

            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvPQ.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvPQ.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);
            dgvPQ.AutoGenerateColumns = true;
            dgvPQ.DataSource = Dt;
            dgvPQ.Refresh();
            dgvPQ.ReadOnly = false;

            dgvPQ.Columns["PurchQuotId"].HeaderText = "PQ Id";
            dgvPQ.Columns["OrderDate"].HeaderText = "PQ Date";
            dgvPQ.Columns["VendName"].HeaderText = "Vendor";
            dgvPQ.Columns["PurchReqId"].HeaderText = "PR No";
            dgvPQ.Columns["OrderDate1"].HeaderText = "PR Date";
            dgvPQ.Columns["CreatedBy"].HeaderText = "Created By";
            dgvPQ.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvPQ.Columns["UpdatedBy"].HeaderText = "Updated By";
            dgvPQ.Columns["UpdatedDate"].HeaderText = "Updated Date";
            

            dgvPQ.Columns["PurchQuotId"].ReadOnly = true;
            dgvPQ.Columns["OrderDate"].ReadOnly = true;
            dgvPQ.Columns["VendName"].ReadOnly = true;
            dgvPQ.Columns["PurchReqId"].ReadOnly = true;
            dgvPQ.Columns["OrderDate1"].ReadOnly = true;
            dgvPQ.Columns["CreatedBy"].ReadOnly = true;
            dgvPQ.Columns["CreatedDate"].ReadOnly = true;
            dgvPQ.Columns["UpdatedBy"].ReadOnly = true;
            dgvPQ.Columns["UpdatedDate"].ReadOnly = true;
            

            dgvPQ.AutoResizeColumns();
            #endregion

            Conn.Close();
        }

        private void PQ_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        //public static List<string> PQID1 = new List<string>();
        bool Close = true;
        private void btnSelect_Click(object sender, EventArgs e)
        {
            string msg = "";
            Conn = ConnectionString.GetConnection();
            string ValidPQ;
            List<string> PQID1 = new List<string>();
            
            for (int i = 0; i <= dgvPQ.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvPQ.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    //BY: HC (S)
                    Query = "select CASE WHEN ISNULL(ValidTo,'1900-01-01') >= (cast(GETDATE()-6 as date)) THEN 'V' ELSE 'N' END from PurchQuotationH where PurchQuotID = '" + dgvPQ.Rows[i].Cells["PurchQuotId"].Value.ToString() + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    ValidPQ = Cmd.ExecuteScalar().ToString();
                    if (ValidPQ == "N")
                    {
                        msg = "Maaf tanggal PQ dengan PQID: " + dgvPQ.Rows[i].Cells["PurchQuotId"].Value + ". Sudah tidak valid";
                    }
                    else
                        PQID1.Add(dgvPQ.Rows[i].Cells["PurchQuotId"].Value == null ? "" : dgvPQ.Rows[i].Cells["PurchQuotId"].Value.ToString());
                    //BY: HC (E)

                    //REMARKED BY: HC (S)
                    //Query = "SELECT ISNULL(ValidTo,'1900-01-01') FROM PurchQuotationH WHERE PurchQuotID = @PQId";
                    //Cmd = new SqlCommand(Query, Conn);
                    //Cmd.Parameters.AddWithValue("@PQId", dgvPQ.Rows[i].Cells["PurchQuotId"].Value);
                    //DateTime ValidPQ = Convert.ToDateTime(Cmd.ExecuteScalar());
                    //if (ValidPQ.Date.DayOfYear < DateTime.Now.Date.DayOfYear)
                    //    msg = "Maaf tanggal PQ dengan PQID: " + dgvPQ.Rows[i].Cells["PurchQuotId"].Value + ". Sudah tidak valid";
                    //else
                    //    PQID1.Add(dgvPQ.Rows[i].Cells["PurchQuotId"].Value == null ? "" : dgvPQ.Rows[i].Cells["PurchQuotId"].Value.ToString());
                    //REMARKED BY: HC (E)
                }
            }

            if (msg != "")
                MessageBox.Show(msg);
            else
            {
                if (PQID1.Count > 0)
                {
                    Close = false;
                }
                Parent.AddDataGridDetail(PQID1);
                this.Close();
            }
        }

        private void dgvPQ_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (dgvPQ.Columns[e.ColumnIndex].Name == "chk")
            //{
            //    for (int i = 0; i < dgvPQ.Rows.Count; i++)
            //    {
            //        Boolean Check = Convert.ToBoolean(dgvPQ.Rows[i].Cells["chk"].Value);
            //        //int a = dgvPQ.CurrentCell.RowIndex;

            //        if (Check == true)
            //        {
            //            dgvPQ.Rows[i].Cells["chk"].Value = false;
            //        }
            //    }
            //}
        }

        private void dgvPQ_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPQ.Columns[e.ColumnIndex].Name == "chk")
            {
                for (int i = 0; i < dgvPQ.Rows.Count; i++)
                {
                    Boolean Check = Convert.ToBoolean(dgvPQ.Rows[i].Cells["chk"].Value);
                    //int a = dgvPQ.CurrentCell.RowIndex;

                    if (Check == true)
                    {
                        dgvPQ.Rows[i].Cells["chk"].Value = false;
                    }
                }
            }
        }

        private void PQ_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (Close == true)
                {
                    Parent.CalltoClose();
                }
            }
        }

        private void dgvPQ_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPQ.Columns[e.ColumnIndex].Name == "CreatedDate" || dgvPQ.Columns[e.ColumnIndex].Name == "UpdatedDate" || dgvPQ.Columns[e.ColumnIndex].Name.Contains("ValidTo"))
                dgvPQ.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy H:mm:ss";
            else if (dgvPQ.Columns[e.ColumnIndex].Name.Contains("Date"))
                dgvPQ.Columns[e.ColumnIndex].DefaultCellStyle.Format = "dd/MM/yyyy";
        }




    }
}
