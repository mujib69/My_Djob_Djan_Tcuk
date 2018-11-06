using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Data.SqlClient;
using System.Collections.Generic;
using CrystalDecisions.ReportSource;
using System.IO;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Transactions;


namespace ISBS_New.Inventory.NotaTransfer
{
    public partial class NT_Inquiry : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query, crit = null;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        List<NTForm> ListNTForm = new List<NTForm>();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public NT_Inquiry()
        {
            InitializeComponent();
        }

        private void NT_Inquiry_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            this.Location = new Point(148, 47);
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'NotaTransferH'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            cmbCriteria.SelectedIndex = 0;
            Conn.Close();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            string searchfield = "";
            if (crit != null && crit != "All")
            {
                Query = "SELECT FieldName FROM [User].[Table] WHERE TableName = 'NotaTransferH' AND DisplayName ='" + crit + "' ";
                using (Cmd = new SqlCommand(Query, Conn))
                {
                    searchfield = Cmd.ExecuteScalar().ToString();
                }
            }
            if (crit == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY TransferNo desc) No, TransferNo, TransferDate, ReferenceType,a.InventSiteFrom,a.InventSiteFromName,a.InventSiteTo,a.InventSiteToName, TransStatus, b.Deskripsi,a.CreatedBy,a.CreatedDate,a.UpdatedBy,a.UpdatedDate From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' WHERE ReferenceType = 'SALES ORDER') a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY TransferNo desc) No, TransferNo, TransferDate, ReferenceType,a.InventSiteFrom,a.InventSiteFromName,a.InventSiteTo,a.InventSiteToName, TransStatus, b.Deskripsi,a.CreatedBy,a.CreatedDate,a.UpdatedBy,a.UpdatedDate From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' Where ReferenceType = 'SALES ORDER' AND ";
                Query += "TransferNo like @txtsearch or TransStatus like @txtsearch or ReferenceType like @txtsearch or a.InventSiteFromName like @txtsearch or a.InventSiteToName like @txtsearch or a.CreatedBy like @txtsearch or a.UpdatedBy like @txtsearch) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ";
            }
            else if (crit.Contains("Date"))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY TransferNo desc) No, TransferNo, TransferDate, ReferenceType,a.InventSiteFrom,a.InventSiteFromName,a.InventSiteTo,a.InventSiteToName, TransStatus, b.Deskripsi,a.CreatedBy,a.CreatedDate,a.UpdatedBy,a.UpdatedDate From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' where ReferenceType = 'SALES ORDER' AND ";
                Query += "(CONVERT(VARCHAR(10)," + searchfield + ",120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10)," + searchfield + ",120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (TransferNo like @txtsearch)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ";
            }
            else if (crit.Equals(crit))
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY TransferNo desc) No, TransferNo, TransferDate, ReferenceType,a.InventSiteFrom,a.InventSiteFromName,a.InventSiteTo,a.InventSiteToName, TransStatus, b.Deskripsi,a.CreatedBy,a.CreatedDate,a.UpdatedBy,a.UpdatedDate From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' Where ReferenceType = 'SALES ORDER' AND ";
                Query += " " + searchfield + " like @txtsearch) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ";
            }
            else
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY TransferNo desc) No, TransferNo, TransferDate, ReferenceType,a.InventSiteFrom,a.InventSiteFromName,a.InventSiteTo,a.InventSiteToName, TransStatus, b.Deskripsi,a.CreatedBy,a.CreatedDate,a.UpdatedBy,a.UpdatedDate From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' WHERE ReferenceType = 'SALES ORDER') a Where " + crit + " Like @txtsearch) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ";
            }
            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@txtsearch",
                Value = "%"+txtSearch.Text+"%",
                SqlDbType = SqlDbType.NVarChar,
                Size = 50
            });
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvNT.AutoGenerateColumns = true;
            dgvNT.DataSource = Dt;
            dgvNT.Refresh();
            dgvNT.AutoResizeColumns();
            dgvNT.Columns["TransferDate"].DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvNT.Columns[0].HeaderText = "No";
            dgvNT.Columns[1].HeaderText = "NT No";
            dgvNT.Columns[2].HeaderText = "NT Date";
            dgvNT.Columns[3].HeaderText = "Reference";
            dgvNT.Columns[4].HeaderText = "FromId";
            dgvNT.Columns[5].HeaderText = "From";
            dgvNT.Columns[6].HeaderText = "ToId";
            dgvNT.Columns[7].HeaderText = "To";
            dgvNT.Columns[8].HeaderText = "Status";
            dgvNT.Columns[9].HeaderText = "Deskripsi";
            dgvNT.Columns[10].HeaderText = "Created By";
            dgvNT.Columns[11].HeaderText = "Created Date";
            dgvNT.Columns[12].HeaderText = "Updated By";
            dgvNT.Columns[13].HeaderText = "Updated Date";
            dgvNT.Columns[4].Visible = false;
            dgvNT.Columns[6].Visible = false;
           
            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (crit == null)
            {
                Query = "Select Count(TransferNo) From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' WHERE ReferenceType = 'SALES ORDER';";
            }
            else if (crit.Equals("All"))
            {
                Query = "Select Count(TransferNo) From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' Where ReferenceType = 'SALES ORDER' AND ";
                Query += "TransferNo like @txtsearch or TransStatus like @txtsearch or ReferenceType like @txtsearch or a.InventSiteFromName like @txtsearch or a.InventSiteToName like @txtsearch or a.CreatedBy like @txtsearch or a.UpdatedBy like @txtsearch ";
            }
            else if (crit.Contains("Date"))
            {
                Query = "Select Count(TransferNo) From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' Where ReferenceType = 'SALES ORDER' AND ";
                Query += "(CONVERT(VARCHAR(10)," + searchfield + ",120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10)," + searchfield + ",120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (TransferNo like @txtsearch)";
            }
            else if (crit.Equals(crit))
            {
                Query = "Select Count(TransferNo) From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' Where ReferenceType = 'SALES ORDER' AND ";
                Query += " " + searchfield + " like @txtsearch";
            }
            else
            {
                Query = "Select Count(TransferNo) From [dbo].[NotaTransferH] a LEFT JOIN [dbo].[TransStatusTable] b ON a.TransStatus = b.StatusCode and b.Transcode ='TransferNote' Where ReferenceType = 'SALES ORDER' AND ";
                Query += crit + " Like @txtsearch";
            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@txtsearch", "%"+txtSearch.Text+"%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;

            dgvNT.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvNT.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
        }

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            RefreshGrid();
        }

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(txtPage.Text) > Page2)
            {
                txtPage.Text = Page2.ToString();
            }
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
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

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            //tak dipakai
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                RefreshGrid();
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShow.Items.Clear();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr.GetInt32(0));
            }
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.New) > 0)
            {
                NTForm NTForm = new NTForm();
                ListNTForm.Add(NTForm);
                //NTForm.SetMode("New", "", "");
                NTForm.SetParent(this);
                NTForm.Show();
                NTForm.ModeNew();
                RefreshGrid();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void dgvPO_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            /* belum
            if (e.RowIndex > -1)
            {
                POForm header = new POForm();
                header.SetMode("BeforeEdit", dgvPO.CurrentRow.Cells["PurchID"].Value.ToString(),"");
                header.Show();
                header.SetParent(this);
                RefreshGrid();
            }
             */
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvNT.Rows[dgvNT.CurrentRow.Index].Cells["TransStatus"].Value.ToString() != "01")
            {
                MessageBox.Show("Data tidak bisa didelete karena status bukan 01.");
                return;
            }
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvNT.RowCount > 0)
                    {
                        Index = dgvNT.CurrentRow.Index;
                        string TransferNo = dgvNT.Rows[Index].Cells["TransferNo"].Value == null ? "" : dgvNT.Rows[Index].Cells["TransferNo"].Value.ToString();

                        DialogResult dr = MessageBox.Show("TransferNo = " + TransferNo + "\n" + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            string Check = "";
                            
                            //insert status delete to notalog, invent trans, invent movemen, inhand
                            insertnotalog(Trans,"Delete");
                            
                            //delete header
                            Query += "Delete from [dbo].[NotaTransferH] where TransferNo ='" + TransferNo + "';";


                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //delete item
                            Query = "Delete from [dbo].[NotaTransferD] where TransferNo='" + TransferNo + "'; ";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            Query = "DELETE FROM [dbo].[NotaTransfer_SO_List] WHERE [TransferNo] = '"+TransferNo+"'; ";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("TransferNo = " + TransferNo.ToUpper() + "\n" + "Data berhasil dihapus.");

                            Index = 0;
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end              
        }

        private void insertnotalog(SqlTransaction Trans, string status)
        {

            string Query2 = "";
            Index = dgvNT.CurrentRow.Index;
            string TransferNo = dgvNT.Rows[Index].Cells["TransferNo"].Value == null ? "" : dgvNT.Rows[Index].Cells["TransferNo"].Value.ToString();
            int row = getRow("[NotaTransferD]","TransferNo",TransferNo,"");
            var seqno = new List<object>();
            Query2 = "SELECT SeqNo FROM NotaTransferD WHERE TransferNo = '"+TransferNo+"'";
            using (Cmd = new SqlCommand(Query2, Conn, Trans))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    seqno.Add(Dr["SeqNo"]);
                }
                Dr.Close();
            }


            for (int i = 0; i < row; i++)
            {
                int tes = Convert.ToInt32(seqno[i]);
                string fullitemid = "";
                decimal qty = 0;
                decimal qtyres = 0;
                decimal ratio = 0;
                string InventSite = "";
                int No = 0;
                string RefTransType = "";
                string refid = "";
                string time="";
                string InventTo = "";
                int transseq = 0;
                string transdate = "";
                Query2 = "SELECT a.[FullItemId],a.[NT_Available_Qty],a.[NT_Reserved_Qty],a.[Ratio],a.[SeqNo],a.[InventSite],a.[LockDocument],a.[ReferenceId],b.[TransferDate],b.[InventSiteTo],a.[SeqNo],b.[TransferDate] FROM [dbo].[NotaTransferD] a LEFT JOIN [dbo].[NotaTransferH] b ON a.[TransferNo]=b.[TransferNo] WHERE a.[TransferNo]='" + TransferNo + "' AND a.[SeqNo]='" + seqno[i] + "'";
                using (Cmd = new SqlCommand(Query2, Conn, Trans))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        fullitemid = Dr[0].ToString();
                        qty = Convert.ToDecimal(Dr[1]);
                        qtyres = Convert.ToDecimal(Dr[2]);
                        ratio = Convert.ToDecimal(Dr[3]);
                        No = Convert.ToInt32(Dr[4]);
                        InventSite = Dr[5].ToString();
                        RefTransType = Dr[6].ToString();
                        refid = Dr[7].ToString();
                        time = Dr[8].ToString();
                        InventTo = Dr[9].ToString();
                        transseq = Convert.ToInt32(Dr[10]);
                        transdate = Dr[11].ToString();
                    }
                    Dr.Close();
                }

                decimal price = 0;
                Query2 = "SELECT [UoM_AvgPrice] FROM [dbo].[InventTable] WHERE [FullItemID] = '" + fullitemid + "'";
                using (Cmd = new SqlCommand(Query2, Conn, Trans))
                {
                    price = Convert.ToDecimal(Cmd.ExecuteScalar());
                }

                decimal amount = qty * price;
                decimal UoMAlt = qty * ratio;

                insertInventTrans(fullitemid, Trans, qty, qtyres, InventSite, TransferNo, transseq, transdate,status);
                insertInventOnHand(row, refid, fullitemid, qtyres, Trans, qty, ratio, price,InventSite,status);
                insertMovementTable(fullitemid, qtyres, Trans, qty, ratio, price, InventSite,status);

                if (status.ToUpper() == "DELETE")
                {
                    Query2 = "INSERT INTO [dbo].[NotaTransfer_LogTable] VALUES ('" + time + "','" + TransferNo + "','','" + refid + "','','" + InventSite + "','" + InventTo + "','" + fullitemid + "','" + No + "','','" + RefTransType + "'," + (qty + qtyres) + "," + UoMAlt + "," + amount + ",'XX','Deleted','Deleted','" + ControlMgr.UserId + "','" + DateTime.Now + "')";
                    using (Cmd = new SqlCommand(Query2, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
                else if (status.ToUpper() == "CREATE")
                {
                    Query2 = "INSERT INTO [dbo].[NotaTransfer_LogTable] VALUES ('" + time + "','" + TransferNo + "','','" + refid + "','','" + InventSite + "','" + InventTo + "','" + fullitemid + "','" + No + "','','" + RefTransType + "'," + (qty + qtyres) + "," + UoMAlt + "," + amount + ",'01','Created','Renewed','" + ControlMgr.UserId + "','" + DateTime.Now + "')";
                    using (Cmd = new SqlCommand(Query2, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                }
            }
            insertLockTable(Trans, status);
        }

        private void insertInventTrans(string FullItemId, SqlTransaction Trans,decimal qty, decimal qtyres, string InventSite,string TransferNo,int transseq,string transdate, string status)
        {
            string Query2 = "";
            string opera = "+";
            string GroupId = "";
            string SubGroupId = "";
            string SubGroup2Id = "";
            string ItemId = "";
            string ItemName = "";
            decimal price = 0;
            decimal ratio = 0;
            Query2 = "SELECT a.*, b.* FROM InventTable a INNER JOIN InventConversion b ON a.FullItemID=b.FullItemID WHERE a.FullItemId = '" + FullItemId + "' AND b.FromUnit='BTG' AND b.ToUnit='KG'";
            using (Cmd = new SqlCommand(Query2, Conn, Trans))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    GroupId = Dr["GroupId"].ToString();
                    SubGroup2Id = Dr["SubGroup2Id"].ToString();
                    SubGroupId = Dr["SubGroup1Id"].ToString();
                    ItemId = Dr["ItemId"].ToString();
                    ItemName = Dr["ItemDeskripsi"].ToString();
                    price = Convert.ToDecimal(Dr["UoM_AvgPrice"]);
                    ratio = Convert.ToDecimal(Dr["Ratio"]);
                }
                Dr.Close();
            }
            if (status.ToUpper() == "CREATE")
            {
                Query2 = "INSERT INTO InventTrans VALUES ( ";
                Query2 += "'" + GroupId + "','" + SubGroupId + "','" + SubGroup2Id + "','" + ItemId + "','" + FullItemId + "','" + ItemName + "','" + InventSite + "','" + TransferNo + "', ";
                Query2 += " " + transseq + ",'" + transdate + "','','',0,'','', ";
                Query2 += " " + (qty + qtyres) + "," + ((qty + qtyres) * ratio) + "," + ((qty + qtyres) * price) + "," + (qty) + "," + (qty * ratio) + ", ";
                Query2 += " " + (qty + price) + "," + (qtyres) + "," + (qtyres * ratio) + "," + (qtyres * price) + ",'')";
            }
            else if(status.ToUpper() == "DELETE")
            {
                Query = "DELETE FROM InventTrans WHERE [TransId] = '" + TransferNo + "' ";
            }
            //if (status.ToUpper() == "CREATE")
            //{
            //    opera = "-";
            //}
            //Query2 = "INSERT INTO InventTrans VALUES ( ";
            //Query2 += "'" + GroupId + "','" + SubGroupId + "','" + SubGroup2Id + "','" + ItemId + "','" + FullItemId + "','" + ItemName + "','" + InventSite + "','" + TransferNo + "', ";
            //Query2 += " " + transseq + ",'" + transdate + "','','',0,'','', ";
            //Query2 += " " + opera + (qty + qtyres) + "," + opera + ((qty + qtyres) * ratio) + "," + opera + ((qty + qtyres) * price) + "," + opera + (qty) + "," + opera + (qty * ratio) + ", ";
            //Query2 += " " + opera + (qty + price) + "," + opera + (qtyres) + "," + opera + (qtyres * ratio) + "," + opera + (qtyres * price) + ",'')";
            using (Cmd = new SqlCommand(Query2, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertLockTable(SqlTransaction Trans, string status)
        {
            Index = dgvNT.CurrentRow.Index;
            string TransferNo = dgvNT.Rows[Index].Cells["TransferNo"].Value == null ? "" : dgvNT.Rows[Index].Cells["TransferNo"].Value.ToString();
            string Query2 = "";
            if (status.ToUpper() == "CREATE")
            {
                var seqno = new List<object>();
                Query2 = "SELECT SeqNo FROM NotaTransfer_SO_List WHERE [TransferNo] = '" + TransferNo + "'";
                using (Cmd = new SqlCommand(Query2, Conn, Trans))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        seqno.Add(Dr["SeqNo"]);
                    }
                    Dr.Close();
                }

                for (int i = 0; i < seqno.Count; i++)
                {
                    decimal lockQty = 0;
                    string soid = "";
                    int seqsoid = 0;
                    int seqrefid2 = 0;
                    Query2 = "SELECT * FROM [NotaTransfer_SO_List] WHERE [TransferNo] = '"+TransferNo+"' AND SeqNo = "+Convert.ToInt32(seqno[i])+"";
                    using(Cmd = new SqlCommand(Query2,Conn,Trans))
                    {
                        Dr=Cmd.ExecuteReader();
                        while(Dr.Read())
                        {
                            soid = Dr["SOId"].ToString();
                            seqsoid = Convert.ToInt32(Dr["SO_SeqNo"]);
                            seqrefid2 = Convert.ToInt32(Dr["Transfer_SeqNo"]);
                            lockQty = Convert.ToDecimal(Dr["NT_Qty"]);
                        }
                        Dr.Close();
                    }
                    int recid = 0;
                    decimal ratio = 0;
                    Query2 = "SELECT TOP 1 [RecId],[Ratio] FROM [dbo].[InventLockTable] WHERE [RefTransType]='SALES ORDER' AND [RefTransId]='" + soid + "' AND [RefTrans_SeqNo]=" + seqsoid + " AND [RefTrans2Id] = '" + TransferNo + "' AND [RefTrans2_SeqNo]=" + seqrefid2 + " AND [SiteId]='" + dgvNT.Rows[Index].Cells["InventSiteFrom"].Value.ToString() + "' AND Lock_Qty = 0 ORDER BY RecId DESC";
                    using (Cmd = new SqlCommand(Query2, Conn, Trans))
                    {
                        Dr = Cmd.ExecuteReader();
                        while (Dr.Read())
                        {
                            recid = Convert.ToInt32(Dr["RecId"]);
                            ratio = Convert.ToDecimal(Dr["Ratio"]);
                        }
                        Dr.Close();
                    }
                    decimal qtyalt = lockQty * ratio;
                    Query2 = "UPDATE [dbo].[InventLockTable] SET [Lock_Qty] = " + (-lockQty) + ",[Lock_Qty_Alt]=" + (-qtyalt) + ",[UpdatedDate]='" + DateTime.Now + "',[UpdatedBy]='"+ControlMgr.UserId+"' WHERE RecId = " + recid + "";
                    using (Cmd = new SqlCommand(Query2, Conn, Trans))
                    {
                        Cmd.ExecuteNonQuery();
                    }
                    Query2 = "";
                }
            }
            else if (status.ToUpper() == "DELETE")
            {
                Query2 = "UPDATE [dbo].[InventLockTable] SET Lock_Qty=0,Lock_Qty_Alt=0 WHERE [RefTrans2Id]='" + dgvNT.Rows[dgvNT.CurrentRow.Index].Cells["TransferNo"].Value.ToString() + "' ";
                using (Cmd = new SqlCommand(Query2, Conn, Trans))
                {
                    Cmd.ExecuteNonQuery();
                }
                Query2 = "";
            }
        }

        private void insertInventOnHand(int row, string RefId, string fullitemid, decimal TransQtyRes, SqlTransaction Trans,decimal TransQty, decimal Ratio, decimal price,string inventsite, string status)
        {
            string opera = "";
            if (status.ToUpper() == "CREATE")
            {
                opera = "-";
            }
            else if (status.ToUpper() == "DELETE")
            {
                opera = "+";
            }
            string Query2 = "";
            Query2 = "UPDATE [dbo].[Invent_OnHand_Qty] SET ";
            Query2 += " [Available_For_Sale_UoM] = [Available_For_Sale_UoM] "+opera+" " + TransQty + ", ";
            Query2 += " [Available_For_Sale_Alt] = [Available_For_Sale_Alt] "+opera+" " + (TransQty * Ratio) + ", ";
            Query2 += " [Available_For_Sale_Amount] = [Available_For_Sale_Amount] "+opera+" " + (TransQty * price) + ", ";
            Query2 += " [Available_For_Sale_Reserved_UoM] = [Available_For_Sale_Reserved_UoM] "+opera+" " + TransQtyRes + ", ";
            Query2 += " [Available_For_Sale_Reserved_Alt] = [Available_For_Sale_Reserved_Alt] "+opera+" " + (TransQtyRes * Ratio) + ", ";
            Query2 += " [Available_For_Sale_Reserved_Amount] = [Available_For_Sale_Reserved_Amount] "+opera+" " + (TransQtyRes * price) + ", ";
            Query2 += " [Available_UoM] = [Available_UoM] "+opera+" " + (TransQty + TransQtyRes) + ", ";
            Query2 += " [Available_Alt] = [Available_Alt] "+opera+" " + ((TransQty + TransQtyRes) * Ratio) + ", ";
            Query2 += " [Available_Amount] = [Available_Amount] "+opera+" " + ((TransQty + TransQtyRes) * price) + " ";
            Query2 += " WHERE [FullItemId] = '" + fullitemid + "' AND [InventSiteId]='" + inventsite + "' ";
            using (Cmd = new SqlCommand(Query2, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void insertMovementTable( string fullitemid, decimal TransQtyRes, SqlTransaction Trans, decimal TransQty, decimal Ratio, decimal price, string inventsite, string status)
        {
            string opera = "";
            if (status.ToUpper() == "CREATE")
            {
                opera = "+";
            }
            else if (status.ToUpper() == "DELETE")
            {
                opera = "-";
            }
            string Query2 = "";
            Query2 = "UPDATE [dbo].[Invent_Movement_Qty] SET ";
            Query2 += " [Transfer_In_Progress_UoM] = [Transfer_In_Progress_UoM] "+opera+" " + (TransQty + TransQtyRes) + ", ";
            Query2 += " [Transfer_In_Progress_Alt] = [Transfer_In_Progress_Alt] " + opera + " " + ((TransQty + TransQtyRes) * Ratio) + ", ";
            Query2 += " [Transfer_In_Progress_Amount] = [Transfer_In_Progress_Amount] " + opera + " " + ((TransQty + TransQtyRes) * price) + " ";
            Query2 += " WHERE [FullItemId] = '" + fullitemid + "'  ";
            using (Cmd = new SqlCommand(Query2, Conn, Trans))
            {
                Cmd.ExecuteNonQuery();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            NTForm header = new NTForm();                  
            if (header.PermissionAccess(ControlMgr.View) > 0)
            {
                if (dgvNT.RowCount > 0)
                {
                    header.SetParent(this);
                    header.Show();
                    header.TransferNo = dgvNT.CurrentRow.Cells["TransferNo"].Value.ToString();
                    header.ModeView();
                    RefreshGrid();
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end              
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            crit = cmbCriteria.Text;
            btnMPrev_Click(sender, e);
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            crit = null;
            txtSearch.Text = "";
            ModeLoad();
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

        private void POInquiry_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenu f = new MainMenu();
            f.refreshTaskList();
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (cmbCriteria.SelectedIndex == -1)
                {
                    crit = "All";
                }
                else
                {
                    crit = cmbCriteria.SelectedItem.ToString();
                }
                RefreshGrid();
            }
        }

        private void dgvNT_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNT.RowCount > 0)
            {
                NTForm header = new NTForm();
                header.SetParent(this);
                header.Show();
                header.TransferNo = dgvNT.CurrentRow.Cells["TransferNo"].Value.ToString();
                header.ModeView();
                RefreshGrid();
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Stock Manager")
            {
                MessageBox.Show("Tidak dapat melakukann approve, karena user group bukan stock manager.");
                goto Outer;
            }
            else if (dgvNT.Rows[dgvNT.CurrentRow.Index].Cells["TransStatus"].Value.ToString() != "01")
            {
                MessageBox.Show("Sudah tidak dapat melakukann approve.");
                goto Outer;
            }
            else
            {
                if (dgvNT.RowCount > 0)
                {
                    NTForm header = new NTForm();
                    header.SetParent(this);
                    header.Show();
                    header.TransferNo = dgvNT.CurrentRow.Cells["TransferNo"].Value.ToString();
                    header.ModeApprove();
                    RefreshGrid();
                }
            }
        Outer: ;
        }

        private void btnMPrev_Click(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }

            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private int getRow(string Table, string field, string value, string query3)
        {
            int row = 0;
            Query = "SELECT COUNT(*) FROM "+Table+" WHERE "+field+" ='" + value + "' ";
            Query += " "+query3+"";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                row = Convert.ToInt32(Cmd.ExecuteScalar());
            }
            return row;
        }

        private bool validate()
        {
            Index = dgvNT.CurrentRow.Index;
            string TransferNo = dgvNT.Rows[Index].Cells["TransferNo"].Value == null ? "" : dgvNT.Rows[Index].Cells["TransferNo"].Value.ToString();
            int row = getRow("[NotaTransferD]","TransferNo",TransferNo,"");

            var qty = new List<object>();
            var qtyres = new List<object>();
            var fullitemid = new List<object>();
            Query = "SELECT [FullItemId],[NT_Available_Qty],[NT_Reserved_Qty] FROM [dbo].[NotaTransferD] WHERE TransferNo ='" + TransferNo + "' ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    qty.Add(Dr["NT_Available_Qty"]);
                    qtyres.Add(Dr["NT_Reserved_Qty"]);
                    fullitemid.Add(Dr["FullItemId"]);
                }
                Dr.Close();
            }
            for (int i = 0; i < row; i++)
            {
                Query = "SELECT [Available_For_Sale_UoM], [Available_For_Sale_Reserved_UoM] FROM [dbo].[Invent_OnHand_Qty] WHERE [FullItemId] = '"+fullitemid[i].ToString()+"'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    if (Convert.ToDecimal(qty[i]) > Convert.ToDecimal(Cmd.ExecuteScalar()))
                    {
                        MessageBox.Show("Tidak bisa digunakan kembali karena Qty pada onHand tidak cukup");
                        return false;
                    }
                    else if (Convert.ToDecimal(qtyres[i]) > Convert.ToDecimal(Cmd.ExecuteScalar()))
                    {
                        MessageBox.Show("Tidak bisa digunakan kembali karena Qty Reserved onHand tidak cukup");
                        return false;
                    }
                }                
            }

            var seqno = new List<object>();
            Query = "SELECT [SeqNo] FROM [NotaTransfer_SO_List] WHERE [TransferNo]='" + TransferNo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    seqno.Add(Dr["SeqNo"]) ;
                }
            }
            for (int x = 0; x < getRow("NotaTransfer_SO_List", "TransferNo", TransferNo, ""); x++)
            {
                string soid = "";
                int soseqno = 0;
                decimal NTQtyRes=0;
                Query = "SELECT * FROM [NotaTransfer_SO_List] WHERE [TransferNo] = '" + TransferNo + "' AND SeqNO = " + Convert.ToInt32(seqno[x]) + " ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        soid = Dr["SOId"].ToString();
                        soseqno = Convert.ToInt32(Dr["SO_SeqNo"]);
                        NTQtyRes = Convert.ToDecimal(Dr["NT_Qty"]);
                    }
                }
                Query = " SELECT SUM(Lock_Qty) FROM [InventLockTable] WHERE [RefTransType]='SALES ORDER' AND [RefTransId]='" + soid + "' AND [RefTrans_SeqNo]=" + soseqno + " AND [SiteId]='" + dgvNT.Rows[Index].Cells["InventSiteFrom"].Value.ToString() + "' GROUP BY RefTransType,RefTransId,RefTrans_SeqNo,SiteId ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    if (NTQtyRes > Convert.ToDecimal(Cmd.ExecuteScalar()))
                    {
                        MessageBox.Show("Qty Lock Reserve untuk SO "+soid[x].ToString()+" sudah tidak mencukupi.");
                        return false;
                    }
                }
            }

            return true;
        } 

        private void btnBatal_Click(object sender, EventArgs e)
        {
            if (dgvNT.Rows[dgvNT.CurrentRow.Index].Cells["TransStatus"].Value.ToString() != "01")
            {
                MessageBox.Show("Tidak bisa dibatalkan");
                return;
            }

            if (ControlMgr.GroupName != "Administrator" && ControlMgr.GroupName != "Stock Admin" && ControlMgr.GroupName != "Stock Manager")
            {
                MessageBox.Show("Hanya Stock Admin dan Stock Manager yang dapat membatalkan transaksi.");
                return;
            }

                try
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        if (dgvNT.RowCount > 0)
                        {
                            Index = dgvNT.CurrentRow.Index;
                            string TransferNo = dgvNT.Rows[Index].Cells["TransferNo"].Value == null ? "" : dgvNT.Rows[Index].Cells["TransferNo"].Value.ToString();

                            DialogResult dr = MessageBox.Show("TransferNo = " + TransferNo + "\n" + "Apakah data diatas akan dibatalkan ?", "Konfirmasi", MessageBoxButtons.YesNo);
                            if (dr == DialogResult.Yes)
                            {
                                Conn = ConnectionString.GetConnection();
                                string Check = "";

                                //insert status delete to notalog, invent trans, invent movemen, inhand
                                insertnotalog(Trans, "Delete");

                                //update header
                                Query += "UPDATE [dbo].[NotaTransferH] SET TransStatus='XX' where TransferNo ='" + TransferNo + "';";

                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                //delete item
                                Query = "UPDATE [dbo].[NotaTransferD] SET [Notes]='Status Deleted' where TransferNo='" + TransferNo + "'; ";

                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                MessageBox.Show("TransferNo = " + TransferNo.ToUpper() + "\n" + "Data berhasil dibatalkan.");

                                Index = 0;
                                Conn.Close();
                                RefreshGrid();
                            }
                        }
                        scope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        private void btnGunakan_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName != "Administrator" && ControlMgr.GroupName != "Stock Admin" && ControlMgr.GroupName != "Stock Manager")
            {
                MessageBox.Show("Hanya Stock Admin dan Stock Manager yang dapat menggunakan kembali transaksi.");
                return;
            }
            if (dgvNT.Rows[dgvNT.CurrentRow.Index].Cells["TransStatus"].Value.ToString() != "XX")
            {
                return;
            }
            if (validate() == false)
            {
                return;
            }
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (dgvNT.RowCount > 0)
                    {
                        Index = dgvNT.CurrentRow.Index;
                        string TransferNo = dgvNT.Rows[Index].Cells["TransferNo"].Value == null ? "" : dgvNT.Rows[Index].Cells["TransferNo"].Value.ToString();

                        DialogResult dr = MessageBox.Show("TransferNo = " + TransferNo + "\n" + "Apakah data diatas akan digunakan kembali ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            Conn = ConnectionString.GetConnection();
                            string Check = "";

                            //insert status delete to notalog, invent trans, invent movemen, inhand
                            insertnotalog(Trans, "Create");

                            //update header
                            Query += "UPDATE [dbo].[NotaTransferH] SET TransStatus='01' where TransferNo ='" + TransferNo + "';";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            //delete item
                            Query = "UPDATE [dbo].[NotaTransferD] SET [Notes]='Renewed' where TransferNo='" + TransferNo + "'; ";

                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("TransferNo = " + TransferNo.ToUpper() + "\n" + "Data berhasil dikembalikan.");

                            Index = 0;
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }
}
