using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseQuotation
{
    public partial class AddItem : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        int flagRefresh = 0;
        String RfqID = null;
        String TransType = "";

        Purchase.PurchaseQuotation.FormPQ Parent;

        public void setParent(Purchase.PurchaseQuotation.FormPQ F)
        {
            Parent = F;
        }

        public void flag(String RfqNumber, String transtype)
        {
            RfqID = RfqNumber;
            TransType = transtype;
        }

        public AddItem()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddItem_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();

            if (dgvItem.RowCount - 1 <= 0)
            {
                dgvItem.ColumnCount = 12;
                dgvItem.Columns[0].Name = "Check";
                dgvItem.Columns[1].Name = "FullItemId";
                dgvItem.Columns[2].Name = "Item Name";
                dgvItem.Columns[3].Name = "Qty";
                dgvItem.Columns[4].Name = "Unit";
                dgvItem.Columns[5].Name = "Deskripsi";
                dgvItem.Columns[6].Name = "Purchase Requistition ID";
                dgvItem.Columns[7].Name = "PR Sequence Number";
                dgvItem.Columns[8].Name = "GelombangId";
                dgvItem.Columns[9].Name = "BracketId";
                dgvItem.Columns[10].Name = "Base";
                dgvItem.Columns[11].Name = "SeqNoGroup";
            }

            
            //Query = "Select a.[FullItemID], ItemDeskripsi, [Qty], [Unit], Deskripsi,PurchReqID,PurchReqSeqNo,GelombangID From [RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId inner JOIN [RequestForQuotation_DtlDtl] dd on a.RfqId=dd.RfqId Where a.RfqId = '" + RfqID + "' order by RfqSeqNo asc";

            if (TransType == "FIX")
            {
                Query = "Select Distinct a.[FullItemID], ItemDeskripsi, [Qty], [Unit], Deskripsi,b.PurchReqID,PurchReqSeqNo, SeqNoGroup From ([RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId) Where a.RfqId = '" + RfqID + "' Order By PurchReqSeqNo Asc";
                dgvItem.Columns["GelombangID"].Visible = false;
                dgvItem.Columns["BracketID"].Visible = false;
                dgvItem.Columns["Base"].Visible = false;
            }
            else
            {
                Query = "Select a.[FullItemID], ItemDeskripsi, [Qty], [Unit], Deskripsi,b.PurchReqID,PurchReqSeqNo,GelombangID,BracketID,Base,SeqNoGroup From ([RequestForQuotationD] a inner join RequestForQuotationH b on a.RfqId=b.RfqId) Where a.RfqId = '" + RfqID + "' Order By PurchReqSeqNo Asc";
                
            }

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;

            while (Dr.Read())
            {
                DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                

                if (TransType == "FIX")
                {
                    this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemDeskripsi"], Dr["Qty"], Dr["Unit"], Dr["Deskripsi"], Dr["PurchReqId"], Dr["PurchReqSeqNo"],"","","", Dr["SeqNoGroup"]);
                    dgvItem.Rows[i].Cells["Check"] = chk;
                    dgvItem.Rows[i].Cells["Check"].Value = false;
                }
                else
                {
                    if (Dr["Base"].ToString() != "N")
                    {
                        this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemDeskripsi"], Dr["Qty"], Dr["Unit"], Dr["Deskripsi"], Dr["PurchReqId"], Dr["PurchReqSeqNo"], Dr["GelombangId"], Dr["BracketId"], Dr["Base"], Dr["SeqNoGroup"]);
                        dgvItem.Rows[i].Cells["Check"] = chk;
                        dgvItem.Rows[i].Cells["Check"].Value = false;
                    }
                    else
                    {
                        this.dgvItem.Rows.Add("", Dr["FullItemId"], Dr["ItemDeskripsi"], Dr["Qty"], Dr["Unit"], Dr["Deskripsi"], Dr["PurchReqId"], Dr["PurchReqSeqNo"], Dr["GelombangId"], Dr["BracketId"], Dr["Base"], Dr["SeqNoGroup"]);
                        dgvItem.Rows[i].ReadOnly = true;
                        dgvItem.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                    }
                    
                    
                }
                
                i++;
            }
            
            
            dgvItem.ReadOnly = false;
            dgvItem.Columns["Check"].ReadOnly = false;
            dgvItem.Columns["FullItemID"].ReadOnly = true;
            dgvItem.Columns["Item Name"].ReadOnly = true;
            dgvItem.Columns["Qty"].ReadOnly = true;
            dgvItem.Columns["Unit"].ReadOnly = true;
            dgvItem.Columns["Deskripsi"].ReadOnly = true;
            dgvItem.Columns["Purchase Requistition ID"].ReadOnly = true;
            dgvItem.Columns["GelombangID"].ReadOnly = true;
            dgvItem.Columns["BracketID"].ReadOnly = true;
            dgvItem.Columns["Base"].ReadOnly = true;

            dgvItem.Columns["PR Sequence Number"].Visible = false;
            dgvItem.Columns["SeqNoGroup"].Visible = false;
            dgvItem.AutoResizeColumns();

            Conn.Close();

        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            Boolean Check = chkAll.Checked;

            if (TransType == "FIX")
            {
                for (int i = 0; i <= dgvItem.RowCount - 1; i++)
                {
                    dgvItem.Rows[i].Cells["Check"].Value = Check;
                }
            }
            else
            {
                for (int i = 0; i <= dgvItem.RowCount - 1; i++)
                {

                    if (dgvItem.Rows[i].Cells["Base"].Value.ToString() != "N")
                    {
                        dgvItem.Rows[i].Cells["Check"].Value = Check;
                    }
                }
            }
            
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> FullItemID = new List<string>();
            List<string> PRId = new List<string>();
            List<string> PurchReqSeqNo = new List<string>();
            List<string> SeqNoGroup = new List<string>();
            int CountChk = 0;
            for (int i = 0; i <= dgvItem.RowCount - 1; i++)
            {
                Boolean Check = false;
                if (dgvItem.Rows[i].Cells["Check"].Value == null || dgvItem.Rows[i].Cells["Check"].Value.ToString() == "")
                {
                    
                }
                else
                {
                    Check = Convert.ToBoolean(dgvItem.Rows[i].Cells["Check"].Value);
                }
                
                if (Check == true)
                {
                    CountChk++;
                    FullItemID.Add(dgvItem.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvItem.Rows[i].Cells["FullItemId"].Value.ToString());
                    PRId.Add(dgvItem.Rows[i].Cells["Purchase Requistition ID"].Value == null ? "" : dgvItem.Rows[i].Cells["Purchase Requistition ID"].Value.ToString());
                    PurchReqSeqNo.Add(dgvItem.Rows[i].Cells["PR Sequence Number"].Value == null ? "" : dgvItem.Rows[i].Cells["PR Sequence Number"].Value.ToString());
                    SeqNoGroup.Add(dgvItem.Rows[i].Cells["SeqNoGroup"].Value == null ? "" : dgvItem.Rows[i].Cells["SeqNoGroup"].Value.ToString());
                }
            }
            Parent.AddDataGridDetail(PRId, SeqNoGroup);
                
            this.Close();
            
        }

        private void dgvItem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //dgvItem.Columns["Check"].ReadOnly = true;

            //if (dgvItem.Rows[e.RowIndex].Cells["Base"].Value.ToString() == "N")
            //{
            //    //harus checklist yg Y klo N ditekan
            //    String group = "";

            //    group = dgvItem.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString();

            //    if((bool)dgvItem.Rows[e.RowIndex].Cells["Check"].Value == true)
            //        dgvItem.Rows[e.RowIndex].Cells["Check"].Value = false;
            //    else
            //        dgvItem.Rows[e.RowIndex].Cells["Check"].Value = true;

            //    for (int i = 0; i < dgvItem.RowCount; i++)
            //    {
            //        if (dgvItem.Rows[i].Cells["SeqNoGroup"].Value.ToString() == group)
            //        {
            //            if (dgvItem.Rows[i].Cells["Base"].Value.ToString() == "Y" && (bool)dgvItem.Rows[i].Cells["Check"].Value == false)
            //            {
            //                dgvItem.Rows[i].Cells["Check"].Value = true;
            //                //dgvItem.Rows[i].Cells["Check"].ReadOnly = true;
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    String group = "";

            //    group = dgvItem.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString();

            //    int countN = 0;

            //    for (int i = 0; i < dgvItem.RowCount; i++)
            //    {
                    
            //        if (dgvItem.Rows[i].Cells["SeqNoGroup"].Value.ToString() == group)
            //        {
            //            if (dgvItem.Rows[i].Cells["Base"].Value.ToString() == "N" && (bool)dgvItem.Rows[i].Cells["Check"].Value == true)
            //            {
            //                countN++;
            //                //MessageBox.Show("Base harus diinclude.");
            //                //dgvItem.Rows[i].Cells["Check"].Value = true;
            //                //dgvItem.Rows[i].Cells["Check"].ReadOnly = true;
            //            }
            //        }
            //    }
            //    if (countN==0)
            //    {
            //        if ((bool)dgvItem.Rows[e.RowIndex].Cells["Check"].Value == true)
            //            dgvItem.Rows[e.RowIndex].Cells["Check"].Value = false;
            //        else
            //            dgvItem.Rows[e.RowIndex].Cells["Check"].Value = true;
                    
            //    }
            //    else
            //        MessageBox.Show("Base harus diinclude.");
            //}


            //if (e.ColumnIndex == dgvItem.Columns["Check"].Index)
            //{
            //    if (dgvItem.Rows[e.RowIndex].Cells["Base"].Value.ToString() == "N")
            //    {
            //        //harus checklist yg Y klo N ditekan
            //        String group = "";
            //        group = dgvItem.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString();

            //        for (int i = 0; i < dgvItem.RowCount; i++)
            //        {
            //            if (dgvItem.Rows[i].Cells["SeqNoGroup"].Value.ToString() == group)
            //            {
            //                if (dgvItem.Rows[i].Cells["Base"].Value.ToString() == "Y" && (bool)dgvItem.Rows[i].Cells["Check"].Value == false)
            //                {
            //                    dgvItem.Rows[i].Cells["Check"].Value = true;
            //                    //dgvItem.Rows[i].Cells["Check"].ReadOnly = true;
            //                }
            //            }
            //        }

            //    }
            //    //else
            //    //{
            //    //    //ga boleh uncheck Y

            //    //    String group = "";
            //    //    group = dgvItem.Rows[e.RowIndex].Cells["SeqNoGroup"].Value.ToString();
            //    //    int v = 0;

            //    //    for (int i = 0; i < dgvItem.RowCount; i++)
            //    //    {
            //    //        if (dgvItem.Rows[i].Cells["SeqNoGroup"].Value.ToString() == group)
            //    //        {
            //    //            if (dgvItem.Rows[i].Cells["Base"].Value.ToString() == "N")
            //    //            {
            //    //                if ((bool)dgvItem.Rows[i].Cells["Check"].Value == true)
            //    //                {
            //    //                    v++;
            //    //                }
            //    //            }
            //    //        }
            //    //        if (v > 0)
            //    //        {
            //    //            MessageBox.Show("Base harus diinclude.");
            //    //            //dgvItem.Rows[e.RowIndex].Cells["Check"].Value = true;
            //    //            return;
            //    //        }
            //    //    }
            //    //}
            //}
        }


       
    }
}
