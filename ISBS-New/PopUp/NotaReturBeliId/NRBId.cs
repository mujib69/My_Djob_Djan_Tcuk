using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.NotaReturBeliId
{
    public partial class NRBId : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;

        private string Query;
        private string NRBID;
        private string FullItemID;
        PopUp.FullItemId.FullItemId FID = null;
        PopUp.Vendor.Vendor Vend = null;

        public NRBId()
        {
            InitializeComponent();
        }

        private void NRBId_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
            }
            dtRBDate.Enabled = false;
            rtNotes.Enabled = false;
        }

        public void GetData(string NRBID)
        {
            Query = "SELECT TOP 1 NRBH.NRBId, NRBH.NRBDate, NRBH.GoodsReceivedID, NRBH.PurchId, NRBH.VendID, NRBH.VendName, NRBH.SiteId, INS.InventSiteName, INS.Lokasi, NRBH.Notes, NRBH.TransStatusId, TST.Deskripsi, NRBH.ApprovedBy, NRBH.ActionCode ";
            Query += "FROM NotaReturBeliH NRBH LEFT JOIN InventSite INS ON INS.InventSiteID = NRBH.SiteId LEFT JOIN TransStatusTable TST ON NRBH.TransStatusId = TST.StatusCode And TST.TransCode = 'NotaReturBeli' ";
            Query += "WHERE NRBId = '" + NRBID + "'";

            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtRBDate.Text=Dr["NRBDate"].ToString();
                lblRBNumber.Text=Dr["NRBId"].ToString();
                lblGoodReceiptNo.Text = Dr["GoodsReceivedID"].ToString();
                lblPONumber.Text=Dr["PurchId"].ToString();
                lblRBStatus.Text = Dr["TransStatusId"].ToString();
                lblRBApproved.Text = Dr["Deskripsi"].ToString();
                lblVendorID.Text=Dr["VendID"].ToString();
                lblVendorName.Text = Dr["VendName"].ToString();
                lblWarehouseID.Text = Dr["SiteId"].ToString();
                lblWarehouseName.Text = Dr["InventSiteName"].ToString();
                lblLocation.Text = Dr["Lokasi"].ToString();
                rtNotes.Text = Dr["Notes"].ToString();
                if (Dr["ActionCode"].ToString()=="01")
                {
                    lblJenisRetur.Text = "Retur Tukar Barang";
                }
                else if (Dr["ActionCode"].ToString()=="02")
                {
                    lblJenisRetur.Text = "Retur Debet Note";
                }
            }
            Dr.Close();

            dgvNRB.DataSource = null;
            if (dgvNRB.RowCount==0)
            {
                dgvNRB.Rows.Clear();
                dgvNRB.ColumnCount = 18;
                dgvNRB.Columns[0].Name = "No";
                dgvNRB.Columns[1].Name = "ItemID";
                dgvNRB.Columns[2].Name = "FullItemID"; 
                dgvNRB.Columns[3].Name = "ItemName"; dgvNRB.Columns["ItemName"].HeaderText = "Name";
                dgvNRB.Columns[4].Name = "GroupId";
                dgvNRB.Columns[5].Name = "SubGroup1ID";
                dgvNRB.Columns[6].Name = "SubGroup2ID";
                dgvNRB.Columns[7].Name = "Qty_GR";
                dgvNRB.Columns[8].Name = "UoM_Qty";
                dgvNRB.Columns[9].Name = "UoM_Unit";
                dgvNRB.Columns[10].Name = "Alt_Qty";
                dgvNRB.Columns[11].Name = "Alt_Unit";
                dgvNRB.Columns[12].Name = "Ratio";
                dgvNRB.Columns[13].Name = "Ratio_Actual";
                dgvNRB.Columns[14].Name = "GoodsReceivedSeqNo";
                dgvNRB.Columns[15].Name = "InventSiteId";
                dgvNRB.Columns[16].Name = "Quality";
                dgvNRB.Columns[17].Name = "Notes";
            }
            Query = "SELECT NRBId, SeqNo, GroupId, SubGroup1Id, SubGroup2Id, ItemId, FullItemId, ItemName, RemainingQty, GoodsReceivedId, GoodsReceived_SeqNo, UoM_Qty, Alt_Qty, UoM_Unit, Alt_Unit, Ratio, NRBD.InventSiteId, ISB.InventSiteBlokID, ActionCode, Notes, Ratio_Actual, Quality ";
            Query += "FROM NotaReturBeli_Dtl NRBD LEFT JOIN InventSiteBlok ISB ON ISB.InventSiteID = NRBD.InventSiteId WHERE NRBId = '" + NRBID + "' ORDER BY SeqNo ASC";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Dr = Cmd.ExecuteReader();

            int j = 1;
            while (Dr.Read())
            {
                 Query = "SELECT Remaining_Qty FROM GoodsReceivedD WHERE GoodsReceivedId = '" +Dr["GoodsReceivedID"] + "' AND FullItemID = '" + Dr["FullItemID"] + "'";
                 Cmd = new SqlCommand(Query, Conn, Trans);
                 this.dgvNRB.Rows.Add(Dr["SeqNo"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], decimal.Parse(Cmd.ExecuteScalar().ToString()) + decimal.Parse(Dr["UoM_Qty"].ToString()), Dr["UoM_Qty"], Dr["UoM_Unit"], Dr["Alt_Qty"], Dr["Alt_Unit"], Dr["Ratio"], Dr["Ratio_Actual"], Dr["GoodsReceived_SeqNo"], Dr["InventSiteId"], Dr["Quality"], Dr["Notes"]);
                 j++;
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvNRB_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "FullItemID" || dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "ItemID" || dgvNRB.Columns[e.ColumnIndex].Name.ToString() == "ItemName")
                    {

                    FID = new PopUp.FullItemId.FullItemId();
                    FID.GetData(dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvNRB.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }
            }
        }
        private bool CheckOpened(string name)
        {
            FormCollection FC = Application.OpenForms;
            foreach (Form frm in FC)
            {
                if (frm.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void lblVendorID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    lblVendorID.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(lblVendorID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }
        }

        private void lblVendorName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Vend == null || Vend.Text == "")
                {
                    lblVendorName.Enabled = true;
                    Vend = new PopUp.Vendor.Vendor();
                    Vend.GetData(lblVendorID.Text);
                    Vend.Show();
                }
                else if (CheckOpened(Vend.Name))
                {
                    Vend.WindowState = FormWindowState.Normal;
                    Vend.Show();
                    Vend.Focus();
                }
            }
        }

      
    }
}
