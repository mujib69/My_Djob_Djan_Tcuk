using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseAgreement
{
    public partial class SelectPQ : MetroFramework.Forms.MetroForm
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
        Purchase.PurchaseAgreement.PAForm Parent;

        public static string PQId = "";
        public static string VendId = "";

        public SelectPQ()
        {
            InitializeComponent();
        }

        public void GetCSId(string tmpCSId)
        {
            CSId = tmpCSId;
        }

        public void SetParentForm(Purchase.PurchaseAgreement.PAForm F)
        {
            Parent = F;
        }

        private void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            string QuotID = "";

            Query = "Select PurchQuotId From PurchAgreementH Where CanvasID = '" + CSId + "'";
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

            //Query = "Select DISTINCT PurchQuotId,VendId from [dbo].[CanvasSheetD] Where CanvasId = '" + CSId + "' and StatusApproval = 'YES' and PurchQuotId NOT IN(" + QuotID + ")";

            //STV Edit
            Query = "SELECT DISTINCT CSd.PurchQuotId 'PQ No', PQh.OrderDate 'PQ Date',CSd.VendID, VT.VendName 'Vendor', CSd.PurchReqId 'PR No', PRh.OrderDate 'PR Date', CSd.CreatedBy 'Created By', CSd.CreatedDate 'Created Date',CSd.UpdatedBy 'Updated By',CSd.UpdatedDate 'Updated Date'";
            Query += "FROM [dbo].[CanvasSheetD] CSd ";
            Query += "LEFT JOIN PurchRequisitionH PRh ON CSd.PurchReqId = PRh.PurchReqId ";
            Query += "LEFT JOIN VendTable VT ON CSd.VendID = VT.VendId ";
            Query += "LEFT JOIN PurchQuotationH PQh ON CSd.PurchQuotId = PQh.PurchQuotID ";
            Query += "WHERE CSd.PurchQuotId Not in (SELECT ReffId2 from PurchH WHERE ReffTableName='CanvasSheet') ";
            Query += "AND CanvasId = '" + CSId + "' ";
            Query += "AND StatusApproval = 'YES' AND CSd.PurchQuotId NOT IN(" + QuotID + ") ";
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
            

            //dgvPQ.Columns["PurchQuotId"].ReadOnly = true;
            //dgvPQ.Columns["VendID"].Visible = false;
            dgvPQ.ReadOnly = false;
            dgvPQ.Columns["VendID"].Visible = false;
            dgvPQ.Columns["PQ No"].ReadOnly = true;
            dgvPQ.Columns["PQ Date"].ReadOnly = true;
            dgvPQ.Columns["Vendor"].ReadOnly = true;
            dgvPQ.Columns["PR No"].ReadOnly = true;
            dgvPQ.Columns["PR Date"].ReadOnly = true;
            dgvPQ.Columns["Created By"].ReadOnly = true;
            dgvPQ.Columns["Created Date"].ReadOnly = true;
            dgvPQ.Columns["Updated By"].ReadOnly = true;
            dgvPQ.Columns["Updated Date"].ReadOnly = true;

            dgvPQ.Columns["Created Date"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPQ.Columns["Updated Date"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvPQ.AutoResizeColumns();
            Conn.Close();
        }

        private void SelectPQ_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string msg = "";
            string ValidPQ;
            Conn = ConnectionString.GetConnection();
            for (int i = 0; i <= dgvPQ.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvPQ.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    //Query = "SELECT ISNULL(ValidTo,'1900-01-01') FROM PurchQuotationH WHERE PurchQuotID = @PQId";
                    Query = "SELECT CASE WHEN ISNULL(ValidTo,'1900-01-01') >= (cast(GETDATE()-6 as date)) THEN 'V' ELSE 'N' END FROM PurchQuotationH WHERE PurchQuotID = @PQId";
                    Cmd = new SqlCommand(Query, Conn);
                    Cmd.Parameters.AddWithValue("@PQId", dgvPQ.Rows[i].Cells["PQ No"].Value);
                    ValidPQ = Cmd.ExecuteScalar().ToString();
                    //DateTime ValidPQ = Convert.ToDateTime(Cmd.ExecuteScalar());
                    if (ValidPQ == "N")
                    {
                        msg = "Maaf tanggal PQ dengan PQID: " + dgvPQ.Rows[i].Cells["PQ No"].Value + ". Sudah tidak valid";
                    }
                    else
                    {
                        PQId = (dgvPQ.Rows[i].Cells["PQ No"].Value == null ? "" : dgvPQ.Rows[i].Cells["PQ No"].Value.ToString());
                        VendId = (dgvPQ.Rows[i].Cells["VendId"].Value == null ? "" : dgvPQ.Rows[i].Cells["VendId"].Value.ToString());
                    }
                }
            }
            Conn.Close();
            if (msg != "")
                MessageBox.Show(msg);
            else
            {
                Parent.AddQuotation(PQId, VendId);
                this.Close();
            }
        }

        private void dgvPQ_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //update by Hasim 24 Okt 2018 agar tidak muncul error
            if (e.ColumnIndex > -1)
            {
                if (dgvPQ.Columns[e.ColumnIndex].Name == "chk")
                {
                    for (int i = 0; i < dgvPQ.Rows.Count; i++)
                    {
                        Boolean Check = Convert.ToBoolean(dgvPQ.Rows[i].Cells["chk"].Value);

                        if (Check == true)
                        {
                            dgvPQ.Rows[i].Cells["chk"].Value = false;
                        }
                    }
                }
            }
        }


    }
}
