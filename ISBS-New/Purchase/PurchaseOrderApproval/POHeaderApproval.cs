using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.PurchaseOrderApproval
{
    public partial class POHeaderApproval : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlConnection Conn2;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String PurchOrderID = null;
        PopUp.Stock.Stock PopUpItemName = new PopUp.Stock.Stock();
        Purchase.PurchaseOrderApproval.POInquiryApproval Parent;

        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public POHeaderApproval()
        {
            InitializeComponent();
        }

        public void setParent(Purchase.PurchaseOrderApproval.POInquiryApproval F)
        {
            Parent = F;
        }

        public void flag(String purchorderid)
        {
            PurchOrderID = purchorderid;
        }

        public void GetData()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select OrderDate, PurchID, TransType, TransStatus from [dbo].[PurchH] where PurchId = '"+PurchOrderID+"'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtPODate.Text = Dr["OrderDate"].ToString();
                txtPONumber.Text = Dr["PurchID"].ToString();
                cmbPOType.SelectedItem = Dr["TransType"].ToString();
                txtPOStatus.Text = Dr["TransStatus"].ToString();
            }
            Dr.Close();

            dgvPODetails1.Rows.Clear();
            if (dgvPODetails1.Rows.Count - 1 <= 0)
            {
                dgvPODetails1.ColumnCount = 27;
                dgvPODetails1.Columns[0].Name = "SeqNo"; dgvPODetails1.Columns[0].HeaderText = "No"; 
                dgvPODetails1.Columns[1].Name = "OrderDate";
                dgvPODetails1.Columns[2].Name = "GroupId";
                dgvPODetails1.Columns[3].Name = "SubGroup1Id";
                dgvPODetails1.Columns[4].Name = "SubGroup2Id";
                dgvPODetails1.Columns[5].Name = "ItemId";
                dgvPODetails1.Columns[6].Name = "FullItemId";
                dgvPODetails1.Columns[7].Name = "ItemName";
                dgvPODetails1.Columns[8].Name = "InventSiteId";
                dgvPODetails1.Columns[9].Name = "Qty";
                dgvPODetails1.Columns[10].Name = "RemainingQty";
                dgvPODetails1.Columns[11].Name = "Unit";
                dgvPODetails1.Columns[12].Name = "Ratio";
                dgvPODetails1.Columns[13].Name = "Price";
                dgvPODetails1.Columns[14].Name = "Total";
                dgvPODetails1.Columns[15].Name = "Diskon(%)";
                dgvPODetails1.Columns[16].Name = "TotalDisk";
                dgvPODetails1.Columns[17].Name = "TotalPPN";
                dgvPODetails1.Columns[18].Name = "TotalPPh";
                dgvPODetails1.Columns[19].Name = "TotalNett";
                dgvPODetails1.Columns[20].Name = "ReffId";
                dgvPODetails1.Columns[21].Name = "ReffSeqNo";
                dgvPODetails1.Columns[22].Name = "Deskripsi";
                dgvPODetails1.Columns[23].Name = "AvailableDate";
                dgvPODetails1.Columns[24].Name = "DiscScheme";
                dgvPODetails1.Columns[25].Name = "BonusScheme";
                dgvPODetails1.Columns[26].Name = "CashBackScheme";
            }

            Query = "Select * from [dbo].[PurchDtl] Where PurchId = '" + PurchOrderID + "' ";
            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                //string OrderDate = Convert.ToDateTime(Dr["OrderDate"]).ToString("dd-MM-yyyy");

                while (Dr.Read())
                {
                    this.dgvPODetails1.Rows.Add((dgvPODetails1.Rows.Count + 1).ToString(), Convert.ToDateTime(Dr["OrderDate"]), Dr["GroupId"], Dr["SubGroup1Id"], Dr["SubGroup2Id"], Dr["ItemId"], Dr["FullItemId"], Dr["ItemName"], Dr["InventSiteId"], Dr["Qty"], Dr["RemainingQty"], Dr["Unit"], Dr["Konv_Ratio"], Dr["Price"], Dr["Total"], Dr["Diskon"], Dr["Total_Disk"], Dr["Total_PPN"], Dr["Total_PPH"], Dr["Total_Nett"], Dr["ReffId"], Dr["ReffSeqNo"], Dr["Deskripsi"], Convert.ToDateTime(Dr["AvailableDate"]), Dr["DiscScheme"], Dr["BonusScheme"], Dr["CashBackScheme"]);
                }
            }

            Conn.Close();

            dgvPODetails1.Columns["GroupId"].Visible = false;
            dgvPODetails1.Columns["SubGroup1Id"].Visible = false;
            dgvPODetails1.Columns["SubGroup2Id"].Visible = false;
            dgvPODetails1.Columns["ItemId"].Visible = false;
            dgvPODetails1.Columns["InventSiteId"].Visible = false;
            dgvPODetails1.Columns["OrderDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["AvailableDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["RemainingQty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Total"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["Ratio"].DefaultCellStyle.Format = "N4";
            dgvPODetails1.Columns["Qty"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalPPH"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.Columns["TotalNett"].DefaultCellStyle.Format = "N2";
            dgvPODetails1.AutoResizeColumns();
        }

        private void POHeaderApproval_Load(object sender, EventArgs e)
        {
            GetData();
            for (int i = 0; i < dgvPODetails1.ColumnCount; i++)
                dgvPODetails1.Columns[i].ReadOnly = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            Decimal QtyUoM;
            Decimal QtyAlt;

            if (ControlMgr.GroupName == "Purchase Manager" || ControlMgr.GroupName == "Management")
            {
                if (this.PermissionAccess(ControlMgr.Approve) > 0)
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    try
                    {
                        DialogResult dialogResult = MessageBox.Show("Apakah PO : " + PurchOrderID + " akan di approve ", "Update Status Confirmation !", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Query = "Update [dbo].[PurchH] set TransStatus = '05' where PurchId = '" + PurchOrderID + "'";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();

                            if (dgvPODetails1.Rows.Count > 0)
                            {
                                Query = "";
                                for (int i = 0; i <= dgvPODetails1.Rows.Count - 1; i++)
                                {
                                    String Unit = dgvPODetails1.Rows[i].Cells["Unit"].Value == "" ? "" : dgvPODetails1.Rows[i].Cells["Unit"].Value.ToString();
                                    decimal Total = dgvPODetails1.Rows[i].Cells["Total"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Total"].Value.ToString()); //hendry tambah 
                                    decimal Qty = dgvPODetails1.Rows[i].Cells["Qty"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Qty"].Value.ToString());
                                    decimal Ratio = dgvPODetails1.Rows[i].Cells["Ratio"].Value == "" ? 0 : decimal.Parse(dgvPODetails1.Rows[i].Cells["Ratio"].Value.ToString());
                                    String FullItemId = dgvPODetails1.Rows[i].Cells["FullItemId"].Value == null ? "" : dgvPODetails1.Rows[i].Cells["FullItemId"].Value.ToString();

                                    if (cmbPOType.Text != "AMOUNT")
                                    {
                                        if (Unit == "KG")
                                        {
                                            QtyUoM = Qty / Ratio;

                                            if (ControlMgr.GroupName == "Purchase Manager" && txtPOStatus.Text == "08")
                                            {
                                                Query += "Update Invent_Purchase_Qty set PO_From_PA_Issued_UoM = (PO_From_PA_Issued_UoM - " + QtyUoM + "),PO_From_PA_Issued_Alt = (PO_From_PA_Issued_Alt - " + Qty + "), PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM + " + QtyUoM + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt + " + Qty + ") where FullItemID = '" + FullItemId + "'; ";
                                            }
                                            else if (ControlMgr.GroupName == "Management")
                                            {
                                                Query += "Update Invent_Purchase_Qty set PO_From_PA_Approved_UoM = (PO_From_PA_Approved_UoM - " + QtyUoM + "),PO_From_PA_Approved_Alt = (PO_From_PA_Approved_Alt - " + Qty + "), PO_From_PA_Approved2_UoM = (PO_From_PA_Approved2_UoM + " + QtyUoM + "),PO_From_PA_Approved2_Alt = (PO_From_PA_Approved2_Alt + " + Qty + ") where FullItemID = '" + FullItemId + "'; ";
                                            }

                                        }
                                        else
                                        {
                                            QtyAlt = Qty * Ratio;

                                            if (ControlMgr.GroupName == "Purchase Manager" && txtPOStatus.Text == "08")
                                            {
                                                Query += "Update Invent_Purchase_Qty set PO_From_PA_Issued_UoM = (PO_From_PA_Issued_UoM - " + Qty + "),PO_From_PA_Issued_Alt = (PO_From_PA_Issued_Alt - " + QtyAlt + "), PO_Issued_Outstanding_UoM = (PO_Issued_Outstanding_UoM + " + Qty + "),PO_Issued_Outstanding_Alt = (PO_Issued_Outstanding_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "'; ";
                                            }
                                            else if (ControlMgr.GroupName == "Management")
                                            {
                                                Query += "Update Invent_Purchase_Qty set PO_From_PA_Approved_UoM = (PO_From_PA_Approved_UoM - " + Qty + "),PO_From_PA_Approved_Alt = (PO_From_PA_Approved_Alt - " + QtyAlt + "), PO_From_PA_Approved2_UoM = (PO_From_PA_Approved2_UoM+ " + Qty + "),PO_From_PA_Approved2_Alt = (PO_From_PA_Approved2_Alt + " + QtyAlt + ") where FullItemID = '" + FullItemId + "'; ";
                                            }

                                        }
                                    }
                                    else
                                    {
                                        #region PA_FROM_PO_APPROVE
                                        if (Unit == "KG")
                                        {
                                            QtyUoM = Qty / Ratio;
                                            QtyAlt = Qty;
                                        }
                                        else
                                        {
                                            QtyUoM = Qty;
                                            QtyAlt = Qty * Ratio;
                                        }

                                        if (ControlMgr.GroupName == "Purchase Manager" && txtPOStatus.Text == "08")
                                        {
                                            Query += "Update Invent_Purchase_Qty set PO_From_PA_Issued_UoM = PO_From_PA_Issued_UoM - " + QtyUoM + ", ";
                                            Query += "PO_From_PA_Issued_Alt = PO_From_PA_Issued_Alt - " + QtyAlt + ", ";
                                            Query += "PO_From_PA_Issued_Amount = (PO_From_PA_Issued_Amount - " + Total + "), ";
                                            Query += "PO_Issued_Outstanding_UoM = PO_Issued_Outstanding_UoM + " + QtyUoM + ", ";
                                            Query += "PO_Issued_Outstanding_Alt = PO_Issued_Outstanding_Alt + " + QtyAlt + ", ";
                                            Query += "PO_Issued_Outstanding_Amount = (PO_Issued_Outstanding_Amount + " + Total + ") ";
                                            Query += "WHERE FullItemID = '" + FullItemId + "'; ";
                                        }
                                        else if (ControlMgr.GroupName == "Management")
                                        {
                                            Query += "Update Invent_Purchase_Qty set ";
                                            Query += "PO_From_PA_Approved_UoM = PO_From_PA_Approved_UoM - " + QtyUoM + ", ";
                                            Query += "PO_From_PA_Approved_Alt = PO_From_PA_Approved_Alt - " + QtyAlt + ", ";
                                            Query += "PO_From_PA_Approved_Amount = (PO_From_PA_Approved_Amount - " + Total + "), ";
                                            Query += "PO_From_PA_Approved2_UoM = PO_From_PA_Approved2_UoM + " + QtyUoM + ", ";
                                            Query += "PO_From_PA_Approved2_Alt = PO_From_PA_Approved2_Alt + " + QtyAlt + ", ";
                                            Query += "PO_From_PA_Approved2_Amount = (PO_From_PA_Approved2_Amount+ " + Total + ") ";
                                            Query += "where FullItemID = '" + FullItemId + "'; ";
                                        }
                                        #endregion
                                    }
                                }

                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();

                                //BY: HC (S)
                                Query = "INSERT INTO [dbo].[PO_Issued_LogTable] ([PODate],[POId],[VendId],[Qty_UoM],[Qty_Alt],[Amount],[PAId],[PADate],[LogStatusCode],[LogStatusDesc],[LogDescription],[UserID],[LogDate],[POSeqNo])VALUES (@PODate, '" + txtPONumber.Text + "', '', 0, 0, 0, '', '', '05' , 'Approved by Purchasing Manager - ALL', 'Approved by Purchasing Manager - ALL', '" + ControlMgr.UserId + "', getdate(), 0); ";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.Parameters.AddWithValue("@PODate", dtPODate.Value);
                                Cmd.ExecuteNonQuery();

                                Query = "INSERT INTO [dbo].[StatusLog_Vendor] ([StatusLog_FormName],[Vendor_Id],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]) VALUES ('POForm', '', '" + txtPONumber.Text + "', '', '', '', '05', 'Approved by Purchasing Manager - ALL', '" + ControlMgr.UserId + "', getdate()); ";
                                Cmd = new SqlCommand(Query, Conn, Trans);
                                Cmd.ExecuteNonQuery();
                                //BY: HC (E)
                                Trans.Commit();

                                MessageBox.Show("Data :" + PurchOrderID + " Berhasil Approve.");
                                Parent.RefreshGrid();
                                this.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trans.Rollback();
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    finally
                    {
                        Conn.Close();
                    }
                }
                else
                {
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else
            {
                MessageBox.Show("Approve can only be done by user Purchase Manager or Management");
                return;
            }
            //end            
        }
        //tia edit
        //klik kanan
        public static string itemID;
        //string ValuePA="PA";
        PopUp.FullItemId.FullItemId FID = null;
        Purchase.PurchaseAgreement.PAForm Pa = null;
        Purchase.PurchaseOrderNew.POForm Po = null;

        private void dgvPODetails1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (FID == null || FID.Text == "")
                {
                    if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "ItemName" || dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "FullItemId")
                    {
                        PopUpItemName.Close();
                        FID = new PopUp.FullItemId.FullItemId();
                        FID.GetData(dgvPODetails1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                        itemID = dgvPODetails1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString();
                        FID.Show();
                    }
                }
                else if (CheckOpened(FID.Name))
                {
                    FID.WindowState = FormWindowState.Normal;
                    FID.GetData(dgvPODetails1.Rows[e.RowIndex].Cells["FullItemId"].Value.ToString());
                    FID.Show();
                    FID.Focus();
                }

                //reffid
                if (Pa==null||Pa.Text=="")
                {
                    if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "ReffId")
                    {
                        if (dgvPODetails1.Rows[e.RowIndex].Cells["ReffId"].Value.ToString().Contains("PA"))
                        {
                             Pa = new Purchase.PurchaseAgreement.PAForm();
                             Pa.SetMode("View", "", dgvPODetails1.Rows[e.RowIndex].Cells["ReffId"].Value.ToString());
                             Pa.Show();
                        }
                  
                    }
                }
                else if (CheckOpened(Pa.Name))
                {
                    Pa.WindowState = FormWindowState.Normal;
                    Pa.SetMode("View", "", dgvPODetails1.Rows[e.RowIndex].Cells["ReffId"].Value.ToString());
                    Pa.Show();
                    Pa.Focus();
                }
                //po
                if (Po == null || Po.Text == "")
                {
                    if (dgvPODetails1.Columns[e.ColumnIndex].Name.ToString() == "ReffId")
                    {
                        if (dgvPODetails1.Rows[e.RowIndex].Cells["ReffId"].Value.ToString().Contains("PO"))
                        {
                            Po = new Purchase.PurchaseOrderNew.POForm();
                            // Po.SetMode("View", dgvPODetails1.Rows[e.RowIndex].Cells["ReffId"].Value.ToString(),"");
                            Po.SetMode("PopUp", txtPONumber.Text, "");
                            Po.ParentRefreshGrid8(this);
                            Po.Show();
                        }

                    }
                }
                else if (CheckOpened(Po.Name))
                {
                    Po.WindowState = FormWindowState.Normal;
                    //Po.SetMode("PopUp", "", dgvPODetails1.Rows[e.RowIndex].Cells["ReffId"].Value.ToString());
                    Po.SetMode("PopUp", txtPONumber.Text, "");
                    Po.ParentRefreshGrid8(this);
                    Po.Show();
                    Po.Focus();
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

        public void HidePrice()
        {
            if (ControlMgr.GroupName == "WB OPERATOR" || ControlMgr.GroupName == "KERANI" || ControlMgr.GroupName == "SITE MANAGER")
            {
                dgvPODetails1.Columns["Price"].Visible=false;
                dgvPODetails1.Columns["Total"].Visible=false;
                dgvPODetails1.Columns["Diskon(%)"].Visible=false;
                dgvPODetails1.Columns["TotalDisk"].Visible=false;
                dgvPODetails1.Columns["TotalPPN"].Visible=false;
                dgvPODetails1.Columns["TotalPPh"].Visible=false;
                dgvPODetails1.Columns["TotalNett"].Visible=false;
                dgvPODetails1.Columns["DiscScheme"].Visible=false;
                dgvPODetails1.Columns["BonusScheme"].Visible=false;
                dgvPODetails1.Columns["CashBackScheme"].Visible=false;
            }
        }

        private void dgvPODetails1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //BY: HC (S)
            dgvPODetails1.Columns["Price"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalDisk"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalPPN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalPPh"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["TotalNett"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["BonusScheme"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvPODetails1.Columns["CashBackScheme"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            //BY: HC (E)
        }

        //tia edit end
        
    }
}
