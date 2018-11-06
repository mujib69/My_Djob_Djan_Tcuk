using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.CanvasSheet
{
    public partial class SearchQuotation : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;

        Purchase.CanvasSheet.FormCanvasSheet2 Parent;

        List<PopUp.Vendor.Vendor> ListVendor = new List<PopUp.Vendor.Vendor>();

        public SearchQuotation()
        {
            InitializeComponent();
        }

        private void SearchQuotation_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();

            Query = "SELECT distinct a.PurchQuotID, a.OrderDate, b.VendID, b.VendName, b.PPH, b.PPN FROM [dbo].[PurchQuotation_Dtl] a Left Join PurchQuotationH b on a.PurchQuotID=b.[PurchQuotID] Where a.ReffTransID ='" + Parent.txtPrNumber.Text + "' and b.TransStatus<>'01'";
            for (int i=0; i<Parent.NotInPurchQuotIdString.Count; i++)
            {
                if (i == 0)
                {
                    Query += " and a.PurchQuotID not in (" + Parent.NotInPurchQuotIdString[i].ToString() + "";
                }
                else
                {
                    Query += "," + Parent.NotInPurchQuotIdString[i].ToString() + "";
                }
                if (i == Parent.NotInPurchQuotIdString.Count-1)
                {
                    Query += ") order by a.PurchQuotID";
                }
            }
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            if (dgvDetailPQ.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvDetailPQ.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }
            dgvDetailPQ.AutoGenerateColumns = true;
            dgvDetailPQ.DataSource = Dt;
            dgvDetailPQ.Refresh();

            dgvDetailPQ.ReadOnly = false;
            dgvDetailPQ.Columns["PurchQuotID"].ReadOnly = true;
            dgvDetailPQ.Columns["OrderDate"].ReadOnly = true;
            dgvDetailPQ.Columns["VendID"].ReadOnly = true;
            dgvDetailPQ.Columns["VendName"].ReadOnly = true;
            dgvDetailPQ.AutoResizeColumns();

            Conn.Close();
        }

        public void SetParent(Purchase.CanvasSheet.FormCanvasSheet2 F)
        {
            Parent = F;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string msg = "";
            List<string> PQnotvalid = new List<string>();
            Conn = ConnectionString.GetConnection();
            //if (Parent.NotInPurchQuotIdString.Count < 3)
            //{
                List<string> PQId = new List<string>();
                int CountChk = 0;
                //Parent.PurchQuotId.Clear();
                Parent.PurchQuotIdString = "";
                //for (int i = 0; i <= dgvDetailPQ.RowCount - 1; i++)
                //{
                //    if (Check == true)
                //    {
                //        CountChk++;
                //    }
                //}

                //if ((Parent.dgvPqDetails.ColumnCount + (8 * CountChk) < (8 + (8 * 3))))
                //{
                    for (int i = 0; i <= dgvDetailPQ.RowCount - 1; i++)
                    {
                        Boolean Check = Convert.ToBoolean(dgvDetailPQ.Rows[i].Cells["chk"].Value);
                        if (Check == true)
                        {
                            Query = "SELECT CASE WHEN ISNULL(ValidTo,'1900-01-01') >= (cast(GETDATE()-6 as date)) THEN 'V' ELSE 'N' END FROM PurchQuotationH WHERE PurchQuotID = @PQId";
                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.Parameters.AddWithValue("@PQId", dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value);
                            String ValidPQ = Cmd.ExecuteScalar().ToString();
                            if (ValidPQ == "N")
                            {
                                PQnotvalid.Add(dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value == null ? "" : dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value.ToString());
                            }
                            else
                            {
                                if (Parent.PurchQuotIdString == "")
                                {
                                    Parent.PurchQuotIdString = dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value == null ? "" : "'" + dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value.ToString() + "'";
                                }
                                else
                                {
                                    Parent.PurchQuotIdString += dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value == null ? "" : (",'" + dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value.ToString() + "'");
                                }
                                Parent.NotInPurchQuotIdString.Add(dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value == null ? "" : "'" + dgvDetailPQ.Rows[i].Cells["PurchQuotID"].Value.ToString() + "'");
                            }
                        }
                    }
                //}
                //else
                //{
                //    Parent.PurchQuotIdString = "";
                //    MessageBox.Show("Data Vendor tidak boleh lebih dari 3.");
                //}
                //Parent.AddDataGridDetail(FullItemID,PRId);
                //this.Close();
            //}
            //else
            //{
            //    MessageBox.Show("Data Vendor tidak boleh lebih dari 3.");
            //}
                    if (PQnotvalid.Count() > 0)
                    {
                        msg = "Maaf ada tanggal PQ yang sudah tidak Valid :";
                        for (int i = 0; i < PQnotvalid.Count(); i++)
                        {
                            msg += "\n  " + (i + 1) + ". " + PQnotvalid[i];
                        }
                    }
                    if (msg != "")
                    {
                        MessageBox.Show(msg);
                    }
                    Conn.Close();
                    this.Close();
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            for (int i = 0; i <= dgvDetailPQ.RowCount - 1; i++)
            {
                dgvDetailPQ.Rows[i].Cells["chk"].Value = Check;
            }
        }

        private void SearchQuotation_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void dgvDetailPQ_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {
                if (dgvDetailPQ.Columns[e.ColumnIndex].Name.ToString() == "VendID")
                {
                    string TmpListVendor = dgvDetailPQ.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    string[] SplitVendor = TmpListVendor.Split(';');

                    for (int i = 0; i < SplitVendor.Count(); i++)
                    {
                        PopUp.Vendor.Vendor PopUpVendor = new PopUp.Vendor.Vendor();
                        ListVendor.Add(PopUpVendor);
                        PopUpVendor.GetData(SplitVendor[i].ToString());
                        PopUpVendor.Y += 100 * i;
                        PopUpVendor.Show();
                    }
                }
            }
        }
    }
}
