using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New
{
    public partial class MainMenu : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        public static string TransStatus;

        string Mode, Query, crit = null;
        private List<Form> MDI = new List<Form>();

        private static int countPQ, countPO, countPA, countCS;

        public static int Timer = 18000;
        public MainMenu()
        {
            InitializeComponent();
            refreshTaskList();
        }

        private void SetCmbRoles()
        {
            cmbRoles.Items.Clear();

            Conn = ConnectionString.GetConnection();
            string strSql = "SELECT GroupName FROM sysGroupMr";
            using (SqlCommand Cmd = new SqlCommand(strSql, Conn))
            {
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        cmbRoles.Items.Add((string)Dr["GroupName"]);
                    }
                }
                cmbRoles.SelectedItem = ControlMgr.GroupName;
            }
            Conn.Close();
        }

        private void userLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.User.UserId.UserIdDashboard F = new Master.User.UserId.UserIdDashboard();
            F.Show();
        }

        public void FormNew(InquiryV1 TmpGlobalForm, string TmpInquiry)
        {
            //if (TmpInquiry == "FormResizeGR")
            //{
            //    ISBS_New.Purchase.GoodsReceipt.Resize.FormResizeGR TmpFormResizeGR = new ISBS_New.Purchase.GoodsReceipt.Resize.FormResizeGR();
            //    TmpFormResizeGR.SetParent(TmpGlobalForm);
            //    TmpGlobalForm.OpenForm = TmpFormResizeGR;
            //}
            if (TmpInquiry == "Inquiry_Nota_Debet")
            {
                ISBS_New.Purchase.NotaDebet.FrmT_NotaDebet TmpFormNotaDebet = new ISBS_New.Purchase.NotaDebet.FrmT_NotaDebet();
                TmpFormNotaDebet.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormNotaDebet;
            }
            if (TmpInquiry == "Inquiry_Nota_Credit")
            {
                ISBS_New.Sales.NotaCredit.FrmT_NotaCredit TmpFormNotaCredit = new ISBS_New.Sales.NotaCredit.FrmT_NotaCredit();
                TmpFormNotaCredit.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormNotaCredit;
            }
            if (TmpInquiry == "Inquiry_Payment_Voucher")
            {
                ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucher TmpFormPaymentVoucher = new ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucher();
                TmpFormPaymentVoucher.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormPaymentVoucher;
            }
            if (TmpInquiry == "Inquiry_Payment_Voucher_Giro")
            {
                ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucherGiro TmpFormPaymentVoucher = new ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucherGiro();
                TmpFormPaymentVoucher.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormPaymentVoucher;
            }
            
            //if (TmpInquiry == "PURCH.GR.RESIZE")
            //{
            //    //TmpGlobalForm.OpenForm = new Purchase.PurchaseRequisition.FormPurchaseRequisition();
            //}
        }

        public void FormNew(InquiryV2 TmpGlobalForm, string TmpInquiry)
        {
            
            if (TmpInquiry == "Inquiry_Payment_Voucher")
            {
                ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucher TmpFormPaymentVoucher = new ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucher();
                TmpFormPaymentVoucher.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormPaymentVoucher;
            }
            if (TmpInquiry == "Inquiry_Payment_Voucher_Giro")
            {
                ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucherGiro TmpFormPaymentVoucher = new ISBS_New.AccountPayable.Payment_Voucher.PaymentVoucherGiro();
                TmpFormPaymentVoucher.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormPaymentVoucher;
            }
            if (TmpInquiry == "Inquiry Purchase Invoice")
            {
                ISBS_New.AccountPayable.InvoicePayment.InvoicePayment TmpFormPaymentVoucher = new ISBS_New.AccountPayable.InvoicePayment.InvoicePayment();
                TmpFormPaymentVoucher.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormPaymentVoucher;
            }
        }

        public void PreviewNew(InquiryV2 TmpGlobalForm, string TmpInquiry)
        {
            if (TmpInquiry == "Inquiry_Payment_Voucher")
            {
                string PVId = TmpGlobalForm.dgvInquiry.Rows[TmpGlobalForm.dgvInquiry.CurrentCell.RowIndex].Cells["PV_No"].Value == null ? "" : TmpGlobalForm.dgvInquiry.Rows[TmpGlobalForm.dgvInquiry.CurrentCell.RowIndex].Cells["PV_No"].Value.ToString();
                string Type = TmpGlobalForm.dgvInquiry.Rows[TmpGlobalForm.dgvInquiry.CurrentCell.RowIndex].Cells["PaymentMethod"].Value == null ? "" : TmpGlobalForm.dgvInquiry.Rows[TmpGlobalForm.dgvInquiry.CurrentCell.RowIndex].Cells["PaymentMethod"].Value.ToString();

                GlobalPreview TmpFormPaymentVoucher = new GlobalPreview("Inquiry_Payment_Voucher", PVId, Type);
                TmpFormPaymentVoucher.SetParent(TmpGlobalForm);
                TmpGlobalForm.OpenForm = TmpFormPaymentVoucher;
            }
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            lblForm.Location = new Point(16, 11);

            SetCmbRoles();
            RefreshDocumentApproval();
            lblIp.Text = ControlMgr.IpAddress;
            lblDesktop.Text = ControlMgr.DesktopName;

            // ControlMgr.GroupName = cmbRoles.Text.ToString();

            //BY: HC (S)
            if (ControlMgr.UserId.ToUpper() == "HC")
                panel1.Visible = true;
            //BY: HC (E)
        }

        private void groupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.User.Group.GroupDashboard F = new Master.User.Group.GroupDashboard();
            F.Show();
        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.User.Menu.MenuDashboard F = new Master.User.Menu.MenuDashboard();
            F.Show();
        }

        private void userGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.User.UserGroup.UserGroupDashboard F = new Master.User.UserGroup.UserGroupDashboard();
            F.Show();
        }

        private void KeluarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void groupMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.User.GroupMenu.GroupMenuDashboard F = new Master.User.GroupMenu.GroupMenuDashboard();
            F.Show();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = Timer.ToString();
            Timer -= 1;
        }

        private void ReceiptListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Pembelian.Receipt_List.DashRL F = new Pembelian.Receipt_List.DashRL();
            //F.Show();   
        }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void pRDashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseRequisition.InquiryPR F = new Purchase.PurchaseRequisition.InquiryPR();
            F.Show();
        }

        private void counterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.Counter.InqCounter F = new Master.Counter.InqCounter();
            //F.Show();
        }

        private void manufacturerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Manufacturer.InqManufacturer F = new Master.Manufacturer.InqManufacturer();
            F.Show();
        }

        private void golonganToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Master.Golongan.GolonganInquiry F = new Master.Golongan.GolonganInquiry();
            F.Show();
        }

        private void dimensionToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Master.Invent.Dimension.InquiryDimension F = new Master.Invent.Dimension.InquiryDimension();
            F.Show();
        }

        private void unitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Master.Invent.Unit.InquiryUnit F = new Master.Invent.Unit.InquiryUnit();
            F.Show();
        }

        private void typeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Invent.Type.InquiryType F = new Master.Invent.Type.InquiryType();
            F.Show();
        }

        private void subGroup1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Invent.SubGroup1.InquirySubGroup1 F = new Master.Invent.SubGroup1.InquirySubGroup1();
            F.Show();
        }

        private void subGroup2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Invent.SubGroup2.InquirySubGroup2 F = new Master.Invent.SubGroup2.InquirySubGroup2();
            F.Show();
        }

        private void inventTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inventory.Master.InvantTable.InquiryInventTable F = new Inventory.Master.InvantTable.InquiryInventTable();
            F.Show();
        }

        private void bankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.Bank.BankInquiry F = new Master.Bank.BankInquiry();
            //F.Show();
        }

        private void currencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.Currency.InqCurrency F = new Master.Currency.InqCurrency();
            //F.Show();
        }

        private void merekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Merek.Merek F = new Master.Merek.Merek();
            F.Show();
        }

        private void paymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.Payment_Mode.InqPaymentMode F = new Master.Payment_Mode.InqPaymentMode();
            //F.Show();
        }

        private void qualityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Quality.InqQuality F = new Master.Quality.InqQuality();
            F.Show();
        }

        private void specToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Spec.InqSpec F = new Master.Spec.InqSpec();
            F.Show();
        }

        private void termOfPaymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.TermOfPayment.InqToP F = new Master.TermOfPayment.InqToP();
            //F.Show();
        }

        private void groupToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Master.Group.GroupInquiry F = new Master.Group.GroupInquiry();
            F.Show();
        }

        private void companyInfoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Master.Invent.Type.InquiryType F = new Master.Invent.Type.InquiryType();
            F.Show();
        }

        private void SalesMasterCurrencyMenu_Click(object sender, EventArgs e)
        {
            //Master.Currency.InqCurrency F = new Master.Currency.InqCurrency();
            ////begin
            ////updated by : joshua
            ////updated date : 24 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end
        }

        private void paymentToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Master.Payment_Mode.InqPaymentMode F = new Master.Payment_Mode.InqPaymentMode();
            ////begin
            ////updated by : joshua
            ////updated date : 24 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end
        }

        private void PurchaseMasterTermOfPaymentMenu_Click(object sender, EventArgs e)
        {

        }

        private void SalesMasterTermOfPaymentMenu_Click(object sender, EventArgs e)
        {
            //Master.TermOfPayment.InqToP F = new Master.TermOfPayment.InqToP();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end
        }


        private void InventoryMasterItemMenu_Click(object sender, EventArgs e)
        {
            Inventory.Master.InvantTable.InquiryInventTable F = new Inventory.Master.InvantTable.InquiryInventTable();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterGroupMenu_Click(object sender, EventArgs e)
        {
            Master.Group.GroupInquiry F = new Master.Group.GroupInquiry();
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterSubGroup1Menu_Click(object sender, EventArgs e)
        {
            Master.Invent.SubGroup1.InquirySubGroup1 F = new Master.Invent.SubGroup1.InquirySubGroup1();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterSubGroup2Menu_Click(object sender, EventArgs e)
        {
            Master.Invent.SubGroup2.InquirySubGroup2 F = new Master.Invent.SubGroup2.InquirySubGroup2();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterMerekMenu_Click(object sender, EventArgs e)
        {
            Master.Merek.InqMerek F = new Master.Merek.InqMerek();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterUnitMenu_Click(object sender, EventArgs e)
        {
            Master.Invent.Unit.InquiryUnit F = new Master.Invent.Unit.InquiryUnit();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterDimensionMenu_Click(object sender, EventArgs e)
        {
            Master.Invent.Dimension.InquiryDimension F = new Master.Invent.Dimension.InquiryDimension();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterSiteMenu_Click(object sender, EventArgs e)
        {

        }

        private void InventoryMasterTypeMenu_Click(object sender, EventArgs e)
        {
            Master.Invent.Type.InquiryType F = new Master.Invent.Type.InquiryType();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterConfigItemMenu_Click(object sender, EventArgs e)
        {
            Master.ConfigItem.InquiryConfigItem F = new Master.ConfigItem.InquiryConfigItem();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterGelombangMenu_Click(object sender, EventArgs e)
        {

        }

        private void InventoryMasterMeasurementMenu_Click(object sender, EventArgs e)
        {
            Master.Invent.Measurement.InquiryMeasurement F = new Master.Invent.Measurement.InquiryMeasurement();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterManufacturerMenu_Click(object sender, EventArgs e)
        {
            Master.Manufacturer.InqManufacturer F = new Master.Manufacturer.InqManufacturer();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterGolonganMenu_Click(object sender, EventArgs e)
        {
            Master.Golongan.GolonganInquiry F = new Master.Golongan.GolonganInquiry();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterSpecMenu_Click(object sender, EventArgs e)
        {
            Master.Spec.InqSpec F = new Master.Spec.InqSpec();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void InventoryMasterQualityMenu_Click(object sender, EventArgs e)
        {
            Master.Quality.InqQuality F = new Master.Quality.InqQuality();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void PurchaseTransactionPRCreateMenu_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseRequisition.InquiryPR F = new Purchase.PurchaseRequisition.InquiryPR();
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void PurchaseTransactionPRApprove_Click(object sender, EventArgs e)
        {
            //Purchase.PurchaseRequisitionApproval.InquiryPRApproval F = new Purchase.PurchaseRequisitionApproval.InquiryPRApproval();
            //F.Show();
            //refreshTaskList();
            Purchase.PurchaseRequisitionApproval.InquiryPRApproval F = new Purchase.PurchaseRequisitionApproval.InquiryPRApproval();

            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.SetParent(this);
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void PurchaseTransactionPQCreateMenu_Click(object sender, EventArgs e)
        {

        }

        private void createToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void requestForQuotationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.RFQ.RFQInquiry F = new Purchase.RFQ.RFQInquiry();

            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 

        }

        private void cmbRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MOVED TO SELECTEION CHANGE COMMITED
        }

        private void cmbRoles_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ControlMgr.GroupName = cmbRoles.Text.ToString();
            RefreshDocumentApproval();
        }

        public void RefreshDocumentApproval()
        {
            TransStatus = "";
            //ControlMgr.GroupName = cmbRoles.Text;
            if (ControlMgr.GroupName == "Sales Manager")
            {
                TransStatus = "'01','02'";//,'05'";
                panelSalesM.Visible = true;
                panelPurchM.Visible = false;
            }
            else if (ControlMgr.GroupName == "Purchase Manager")
            {
                TransStatus = "'03','04','12','14', '21'";//"'03','04','12','13','14','15','21'";
                panelPurchM.Visible = true;
                panelSalesM.Visible = false;
            }

            if (TransStatus != "")
            {
                Query = "Select Count([PurchReqID]) from [dbo].[PurchRequisitionH] where TransStatus in (" + TransStatus + ");";
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand(Query, Conn);
                lblPurchRequisition.Text = Cmd.ExecuteScalar().ToString();
                Conn.Close();

                //Query = "select count(*) from PurchQuotationH where TransStatus in (" + TransStatus + ");";
                //Conn = ConnectionString.GetConnection();
                //Cmd = new SqlCommand(Query, Conn);
                //lblPurchQuotation.Text = Cmd.ExecuteScalar().ToString();
                //Conn.Close();
            }
        }

        private void btnShowPR_Click(object sender, EventArgs e)
        {
            //PopUp.DashboardApprovalPR.DashboardApprovalPR F = new PopUp.DashboardApprovalPR.DashboardApprovalPR();
            //Purchase.PurchaseRequisitionApproval.InquiryPRApproval F = new Purchase.PurchaseRequisitionApproval.InquiryPRApproval();
            //F.SetParent(this);
            //F.Show();
            Purchase.PurchaseRequisitionApproval.InquiryPRApproval F = new Purchase.PurchaseRequisitionApproval.InquiryPRApproval();
            F.SetParent(this);
            F.Show();
            refreshTaskList();
        }

        private void createToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseOrderNew.POInquiry F = new Purchase.PurchaseOrderNew.POInquiry();


            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.PurchaseRequisition.InquiryPR f = new Purchase.PurchaseRequisition.InquiryPR();
            f.Show();
        }

        private void btnShowPQ_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseQuotation.InquiryPQ F = new Purchase.PurchaseQuotation.InquiryPQ();
            F.Show();
            lblPurchQuotation.Text = CountPQ.ToString();
        }

        private void btnShowCS_Click(object sender, EventArgs e)
        {
            Purchase.CanvasSheet.InquiryCanvasSheet F = new Purchase.CanvasSheet.InquiryCanvasSheet();
            //F.SetParent(this);
            F.Show();
            refreshTaskList();
        }

        public void refreshTaskList()
        {
            RefreshDocumentApproval();


            Conn = ConnectionString.GetConnection();
            Query = "select count(*) from [dbo].[PurchQuotationH]";
            Cmd = new SqlCommand(Query, Conn);
            CountPQ = Int32.Parse(Cmd.ExecuteScalar().ToString());
            lblPurchQuotation.Text = CountPQ.ToString();

            Query = "select count(*) from [dbo].[CanvasSheetH] where [TransStatus] = '01'";
            Cmd = new SqlCommand(Query, Conn);
            CountCS = Int32.Parse(Cmd.ExecuteScalar().ToString());
            lblCanvasSheet.Text = CountCS.ToString();

            Query = "select count(*) from [dbo].[PurchAgreementH]";
            Cmd = new SqlCommand(Query, Conn);
            CountPA = Int32.Parse(Cmd.ExecuteScalar().ToString());
            lblPurchAgreement.Text = CountPA.ToString();

            Query = "select count(*) from [dbo].[PurchH]";
            Cmd = new SqlCommand(Query, Conn);
            countPO = Int32.Parse(Cmd.ExecuteScalar().ToString());
            lblPurchOrder.Text = countPO.ToString();
        }

        //-----------------------------------------------------------------------------------------------
        //METHOD
        public static int CountPQ { get { return countPQ; } set { countPQ = value; } }

        public static int CountCS { get { return countCS; } set { countCS = value; } }

        public static int CountPO { get { return countPO; } set { countPO = value; } }

        public static int CountPA { get { return countPA; } set { countPA = value; } }

        //-----------------------------------------------------------------------------------------------
        private void btnPO_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseOrderNew.POInquiry F = new Purchase.PurchaseOrderNew.POInquiry();
            F.Show();
            lblPurchOrder.Text = CountPO.ToString();
            refreshTaskList();
        }

        private void btnPA_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseAgreement.PAInq F = new Purchase.PurchaseAgreement.PAInq();
            F.Show();
            lblPurchAgreement.Text = CountPA.ToString();
        }
        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Bookmark f = new Bookmark();
            f.Show();
        }

        private void lblCrPRSales_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.PurchaseRequisition.InquiryPR f = new Purchase.PurchaseRequisition.InquiryPR();
            f.Show();
            refreshTaskList();
        }

        private void createToolStripMenuItem2_Click(object sender, EventArgs e)
        {

        }

        private void llCreateQuotation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.PurchaseQuotation.InquiryPQ f = new Purchase.PurchaseQuotation.InquiryPQ();
            f.Show();
        }

        private void llCreateCS_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.CanvasSheet.InquiryCanvasSheet f = new Purchase.CanvasSheet.InquiryCanvasSheet();
            f.Show();
        }

        private void llCreatePA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.PurchaseAgreement.PAInq f = new Purchase.PurchaseAgreement.PAInq();
            f.Show();
        }

        private void llCreatePO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.PurchaseOrderNew.POInquiry F = new Purchase.PurchaseOrderNew.POInquiry();
            F.Show();
        }

        private void llGenerateRFQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.PurchaseRequisition.InquiryPR f = new Purchase.PurchaseRequisition.InquiryPR();
            f.Show();
        }

        private void llCreateRO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.ReceiptOrder.InquiryReceiptOrder F = new Purchase.ReceiptOrder.InquiryReceiptOrder();
            F.Show();
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Purchase.GoodsReceipt.InquiryResizeGR F = new Purchase.GoodsReceipt.InquiryResizeGR();
            F.Show();
        }

        private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void createToolStripMenuItem3_Click(object sender, EventArgs e)
        {

        }

        private void createToolStripMenuItem4_Click(object sender, EventArgs e)
        {


        }

        private void resizeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Inventory.Master.InvantTable.InquiryResize F = new Inventory.Master.InvantTable.InquiryResize();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Inventory.Master.InvantTable.InquiryResize F = new Inventory.Master.InvantTable.InquiryResize();
            F.Show();
        }

        private void approveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseOrderApproval.POInquiryApproval F = new Purchase.PurchaseOrderApproval.POInquiryApproval();

            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void headerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales.SalesQuotation.SQHeader2 F = new Sales.SalesQuotation.SQHeader2();
            F.Show();
        }

        private void createToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Inventory.NotaTransfer.NT_Inquiry F = new Inventory.NotaTransfer.NT_Inquiry();

            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }       

        private void salesQuotationToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Sales.SalesQuotation.SQInq F = new Sales.SalesQuotation.SQInq();
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }
        private void createToolStripMenuItem7_Click(object sender, EventArgs e)
        {

        }

        private void llSalesQuotation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            salesQuotationToolStripMenuItem_Click_1(new object(), new EventArgs());
            //Sales.SalesQuotation.SQInq F = new Sales.SalesQuotation.SQInq();
            //F.Show();
        }

        private void notaPurchaseParkedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked F = new Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked();
            F.Show();
        }

        private void llSalesOrder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            salesOrderToolStripMenuItem_Click(new object(), new EventArgs());
            //Sales.SalesOrder.SOInq F = new Sales.SalesOrder.SOInq();
            //F.Show();
        }

        private void salesOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "a.OrderDate, a.SalesOrderNo, a.TransStatus, b.Deskripsi, a.CustName, a.CurrencyID, a.Total_Nett, a.TermofPayment, a.PaymentModeID, a.DPType, a.DPAmount, a.DPDueDate, a.ValidTo, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 and b.TransCode = 'SalesOrder'";
            List<string> headerText = new List<string> { "No", "SO Date", "SO No", "Status ID", "Status", "Customer Name", "Currency", "Total Nett", "Term of Payment", "Payment Mode", "DP Type", "DP Amount", "DP Due Date", "Valid To", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            F.SetMode("SalesOrderNo", "dbo", "SalesOrderH", "SalesOrderH a left join TransStatusTable b on a.TransStatus = b.StatusCode", fieldName, where, "Sales Order", "SOInq", headerText);
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            ////Sales.SalesOrder.SOInq F = new Sales.SalesOrder.SOInq();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end 
        }
               
        private void stockViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Inventory.StockView.StockView F = new Inventory.StockView.StockView();
            //F.Show();
        }      

        private void llDO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            deliveryOrderToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void llBBK_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            goodsIssuedToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void deliveryOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            //string fieldName = "a.DeliveryOrderId 'DO No', a.DeliveryOrderDate 'DO Date', a.DeliveryDate 'Delivery Date', a.CustID 'Customer', a.InventSiteID 'Warehouse',a.DeliveryOrderStatus, b.Deskripsi, a.SalesOrderId 'SO No', a.SalesOrderDate 'SO Date', a.CreatedBy, a.CreatedDate,a.UpdatedBy,a.UpdatedDate ";
            string fieldName = "a.DeliveryOrderId, a.DeliveryOrderDate, a.DeliveryDate, a.CustID, a.InventSiteID,a.DeliveryOrderStatus, b.Deskripsi, a.SalesOrderId, a.SalesOrderDate, a.CreatedBy, a.CreatedDate,a.UpdatedBy, a.UpdatedDate ";
            List<string> headerText = new List<string> { "No", "DO No", "DO Date", "Delivery Date", "Customer", "Warehouse", "Status ID", "Status", "SO No", "SO Date", "Created By", "Created Date", "Updated By", "Updated Date", "Preview", "Send Email" };
            F.SetMode("DeliveryOrderId", "dbo", "DeliveryOrderH", "[dbo].[DeliveryOrderH] a left join TransStatusTable b on a.DeliveryOrderStatus = b.StatusCode", fieldName, "1=1 and a.DeliveryOrderStatus != '07' and b.TransCode = 'DO'", "Delivery Order", "DOInq", headerText);
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void SalesNotaReturJualCreate_Click(object sender, EventArgs e)
        {
            Sales.NotaReturJual.InqNRJCreate F = new Sales.NotaReturJual.InqNRJCreate();
            //begin
            //updated by : Mujib
            //updated date : 18 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void actionParkedItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Purchase.ActionParkedItem.InquiryActionParkedItem F = new Purchase.ActionParkedItem.InquiryActionParkedItem();

            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            ////    F.Show();
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end 
        }

        private void parkedItemToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked F = new Purchase.NotaPurchaseParked.InquiryNotaPurchaseParked();
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end

        }

        private void goodsIssuedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "a.GoodsIssuedDate, a.GoodsIssuedId, a.StatusCode, b.Deskripsi, a.AccountNum, a.AccountName, a.RefTransID, a.RefTransDate, a.Notes, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 and b.TransCode = 'GI' AND a.RefTransID NOT LIKE '%NT%' ";
            List<String> headerText = new List<string> { "No", "GI Date", "GI ID", "Status Code", "Status", "Account ID", "Account Name", "Reference ID", "Reference Date", "Notes", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            F.SetMode("GoodsIssuedId", "dbo", "GoodsIssuedH", "[dbo].[GoodsIssuedH] a left join TransStatusTable b on a.StatusCode = b.StatusCode", fieldName, where, "Bukti Barang Keluar", "BBKInq", headerText);
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void accessRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ControlMgr.GroupName == "Administrator")
            {
                Form Frm_UDataPemakai = new Utility.Frm_UDataPemakai();
                Frm_UDataPemakai.Show();
            }
        }

        private void notaAjustmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inventory.NotaAdjustment.InquiryNotaAdjust F = new Inventory.NotaAdjustment.InquiryNotaAdjust();
            // begin
            // updated by : joshua
            // updated date : 26 feb 2018
            //  description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void salesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.Sales.InqSales F = new Master.Sales.InqSales();
            //// begin
            //// updated by : joshua
            //// updated date : 27 feb 2018
            ////  description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end

            ////F.Show();

            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.Kode_Sls, a.Nama_Sls, a.Alamat, a.Telepon, a.HP, a.Persen, a.Group_Brg, a.Toko_Proyek, a.Kategori, a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "Sales ID", "Sales Name", "Alamat", "Telepon", "HP", "Persen", "Group Barang", "Toko Proyek", "Kategori", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("Kode_Sls", "dbo", "Sales", "Sales a ", fieldName, where, "Sales", "Sales", headerText, "Sales", HideField);
            F.btnOnProgress.Visible = false;
            F.btnCompleted.Visible = false;
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void customerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Customer.FrmL_Customer F = new Master.Customer.FrmL_Customer();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end        
        }

        private void supplierFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Vendor.FrmL_Vendor F = new Master.Vendor.FrmL_Vendor();
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void counterToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Master.Counter.InqCounter F = new Master.Counter.InqCounter();
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end

            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[Jenis],a.[Kode],a.[Deskripsi],a.[Counter], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "Jenis", "Kode", "Deskripsi", "Counter", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("Jenis", "dbo", "Counter", "Counter a ", fieldName, where, "Counter", "Counter", headerText, "Counter", HideField);
            F.btnOnProgress.Visible = false;
            F.btnCompleted.Visible = false;
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void cityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form FrmL_Kota = new Master.Kota.FrmL_Kota();
            FrmL_Kota.Show();            
        }

        private void bankGroupToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form FrmL_BankGroup = new Master.BankGroup.FrmL_BankGroup();
            FrmL_BankGroup.Show();      
        }

        private void bankToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Master.Bank.BankInquiry F = new Master.Bank.BankInquiry();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end
        }

        private void companyInfoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Master.CompanyInfo.FormCompanyInfo F = new Master.CompanyInfo.FormCompanyInfo();
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void currencyToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Master.Currency.InqCurrency F = new Master.Currency.InqCurrency();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end

            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[CurrencyId], [CurrencyName], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Preview", "Send Email", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            List<String> headerText = new List<string> { "No", "Currency Id", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("CurrencyId", "dbo", "CurrencyTable", "CurrencyTable a ", fieldName, where, "Currency", "Currency", headerText, "Currency", HideField);
            F.btnOnProgress.Visible = false;
            F.btnCompleted.Visible = false;
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void paymentMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.Payment_Mode.InqPaymentMode F = new Master.Payment_Mode.InqPaymentMode();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end

            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[PaymentModeID], a.[PaymentModeName], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "Payment Mode Id", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("PaymentModeID", "dbo", "PaymentMode", "PaymentMode a ", fieldName, where, "Payment Mode", "Payment Mode", headerText, "Payment Mode", HideField);
            F.btnOnProgress.Visible = false;
            F.btnCompleted.Visible = false;
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void termOfPaymentToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Master.TermOfPayment.InqToP F = new Master.TermOfPayment.InqToP();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end

            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[TermOfPayment], a.[DueDate], a.[GracePeriod],a.[Deskripsi], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "TermOfPayment", "DueDate", "GracePeriod", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("TermOfPayment", "dbo", "TermOfPayment", "TermOfPayment a ", fieldName, where, "Term of Payment", "Term of Payment", headerText, "Term Of Payment", HideField);
            F.btnOnProgress.Visible = false;
            F.btnCompleted.Visible = false;
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void deliveryMethodToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Master.DeliveryMethod.DeliveryInquiry F = new Master.DeliveryMethod.DeliveryInquiry();
            ////begin
            ////updated by : joshua
            ////updated date : 21 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //    refreshTaskList();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end

            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[DeliveryMethod], a.[Deskripsi], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "Delivery Method", "Deskripsi", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("DeliveryMethod", "dbo", "DeliveryMethod", "DeliveryMethod a ", fieldName, where, "Delivery Method", "Delivery Method", headerText, "Delivery Method", HideField);
            F.btnOnProgress.Visible = false;
            F.btnCompleted.Visible = false;
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void companyGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form FrmL_GolPrsh = new Master.Golongan.FrmL_GolPrsh();
            FrmL_GolPrsh.Show();
        }

        private void addressTypeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Master.AddressType.FrmL_AddressType F = new Master.AddressType.FrmL_AddressType();
            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }        

        private void siteToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void gelombangToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem.ToString().Contains("Sales"))
            {
                MessageBox.Show("You Don't Have Permission to Access");
            }
            else
            {
                Master.Gelombang.GelombangInquiry F = new Master.Gelombang.GelombangInquiry("Purchase");
                //begin
                //updated by : joshua
                //updated date : 24 feb 2018
                //description : check permission access
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.Show();
                }
                else
                {
                    F.Close();
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
                //end
            }
        }
        private void siteBlokToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exchangeRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Master.ExchangeRate.ExchangeRateInq F = new Master.ExchangeRate.ExchangeRateInq();

            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}

            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[CurrencyId], a.[ExchRate] , a.[RecId], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "Currency Id", "Exchange Rate", "Rec Id", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("RecId", "dbo", "ExchRate", "ExchRate a ", fieldName, where, "Exchange Rate", "Exchange Rate", headerText, "Exchange Rate", HideField);
            F.btnOnProgress.Visible = false;
            F.btnCompleted.Visible = false;
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }

        }

        private void createToolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Purchase.ParkedItem.InquiryParkedItem F = new Purchase.ParkedItem.InquiryParkedItem();
            F.Show();
        }

        private void approveToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //Purchase.Retur.RTBApproval.InqRTBApproval F = new Purchase.Retur.RTBApproval.InqRTBApproval();
            //F.Show();
        }

        private void toolStripMenuItem_KodeBerat_Click(object sender, EventArgs e)
        {
            Inventory.Master.KodeBerat.InquiryKodeBerat F = new Inventory.Master.KodeBerat.InquiryKodeBerat();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void WarehouseSiteMenu_Click(object sender, EventArgs e)
        {
            Master.Site.InquirySite F = new Master.Site.InquirySite();
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void WarehouseBlokMenu_Click(object sender, EventArgs e)
        {
            //BEGIN STEVEN EDIT
            Master.Site.SiteBlokInq F = new Master.Site.SiteBlokInq();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }

            //END STEVEN EDIT
        }

        private void PurchaseTransactionPurchaseQuotationMenu_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseQuotation.InquiryPQ F = new Purchase.PurchaseQuotation.InquiryPQ();

            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void canvasSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.CanvasSheet.InquiryCanvasSheet F = new Purchase.CanvasSheet.InquiryCanvasSheet();

            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void purchaseAgreementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseAgreement.PAInq F = new Purchase.PurchaseAgreement.PAInq();

            //begin
            //updated by : joshua
            //updated date : 21 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void receiptOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.ReceiptOrder.InquiryReceiptOrder F = new Purchase.ReceiptOrder.InquiryReceiptOrder();

            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void goodsReceiptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> GetType = new List<string>();
            GetType.Add("Receipt Order");
            GetType.Add("Nota Retur Beli");
            Purchase.GoodsReceipt.InquiryGoodsReceipt F = new Purchase.GoodsReceipt.InquiryGoodsReceipt(GetType);

            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void notaResizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldname = " a.[NRZId] AS 'NRZ No',CONVERT(nvarchar(30),CAST(a.NRZDate AS DATE),103) as 'NRZ Date',a.[GoodsReceivedId] as 'GR No',CONVERT(nvarchar(30),CAST(a.GoodsReceivedDate AS DATE),103) as 'GR Date',a.[SiteID] AS 'Warehouse',CASE WHEN Posted=1 THEN 'POSTED' ELSE '' END AS 'Status',a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] ";
            string where = " 1=1 ";
            //List<string> headerText = new List<string> { "No", "NRZ No", "NRZ Date", "GR No", "GR Date", "Warehouse", "Status", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode2("NRZId", "dbo", "NotaResizeH", "[dbo].[NotaResizeH] a", fieldname, where, "Inquiry Nota Resize", "InquiryNotaResize");
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void moUCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales.MoUCustomer.InqueryMoUCustomer F = new Sales.MoUCustomer.InqueryMoUCustomer();

            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void PurchaseNotaReturBeliMenu_Click(object sender, EventArgs e)
        {
            //Purchase.Retur.NotaReturBeli.InqReturBeli F = new Purchase.Retur.NotaReturBeli.InqReturBeli();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end 
        }

        private void SalesGelombangMenu_Click(object sender, EventArgs e)
        {
            if (cmbRoles.SelectedItem.ToString().Contains("Purchase"))
            {
                MessageBox.Show("You Don't Have Permission to Access");
            }
            else
            {
                Master.Gelombang.GelombangInquiry F = new Master.Gelombang.GelombangInquiry("Sales");
                if (F.PermissionAccess(ControlMgr.View) > 0)
                {
                    F.Show();
                }
                else
                {
                    F.Close();
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
        }

        private void PurchaseNotaDebetMenu_Click(object sender, EventArgs e)
        {
            Variable.Kode = null;
            InquiryV1 tmpSearch = new InquiryV1();
            tmpSearch.InquiryName = "InquiryNotaDebet";
            if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            {
                MDI.Add(tmpSearch);
                //Digunakan untuk menampilkan data grid
                tmpSearch.Text = "Inquiry Nota Debet";
                tmpSearch.PrimaryKey = "DN_No";
                //tmpSearch.Table = "[dbo].[NotaDebetH]";
                tmpSearch.QuerySearch = "SELECT a.[DN_Date],a.[DN_No], a.[DNMode], a.[NRBId], a.[AccountNum], a.[AccountName],b.[Deskripsi],a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM NotaDebetH a LEFT JOIN [TransStatusTable] b ON a.[TransStatus]=b.[StatusCode] WHERE [TransCode]='NotaDebet'";
                tmpSearch.Order = "CreatedDate Desc";
                tmpSearch.FilterText = new string[] { "DN_No", "DN_Date", "AccountNum", "AccountName","Deskripsi", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
                tmpSearch.Select = new string[] { "DN_No", "DN_Date" };
                tmpSearch.Parent = this;
                tmpSearch.Table = "NotaDebetH";
                tmpSearch.HideField = new string[] { "DNMode", "NRBId", "AccountNum" };
                //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
                tmpSearch.FormName = "Inquiry_Nota_Debet";
                tmpSearch.visibleBtn();
                //Digunakan untsuk menentukan form yang akan dideletes
                tmpSearch.Delete = new string[] { "DN_No" };
                tmpSearch.CreateJournal(true);

                //tmpSearch.WherePlus = "";
                //tmpSearch.SetSchemaTable(SchemaName, TableName);
                tmpSearch.Show();
            }
            else
            {
                tmpSearch.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void SalesNotaCreditMenu_Click(object sender, EventArgs e)
        {
            Variable.Kode = null;
            InquiryV1 tmpSearch = new InquiryV1();
            tmpSearch.InquiryName = "InquiryNotaCredit";
            if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            {
                MDI.Add(tmpSearch);
                //Digunakan untuk menampilkan data grid
                tmpSearch.Text = "Inquiry Nota Credit";
                tmpSearch.PrimaryKey = "CN_No";
                //tmpSearch.Table = "[dbo].[NotaDebetH]";
                tmpSearch.QuerySearch = "SELECT a.[CN_Date],a.[CN_No], a.[CNMode], a.[NRJId], a.[AccountNum], a.[AccountName],b.[Deskripsi],a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM NotaCreditH a LEFT JOIN [TransStatusTable] b ON a.[TransStatus]=b.[StatusCode] WHERE [TransCode]='NotaCredit' ";
                tmpSearch.Order = "CreatedDate Desc";
                tmpSearch.FilterText = new string[] { "CN_No", "CN_Date", "AccountNum", "AccountName","Deskripsi", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
                tmpSearch.Select = new string[] { "CN_No", "CN_Date" };
                tmpSearch.Parent = this;
                tmpSearch.Table = "NotaCreditH";
                tmpSearch.HideField = new string[] { "CNMode", "NRJId", "AccountNum" };
                //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
                tmpSearch.FormName = "Inquiry_Nota_Credit";
                tmpSearch.visibleBtn();
                //Digunakan untsuk menentukan form yang akan dideletes
                tmpSearch.Delete = new string[] { "CN_No" };
                tmpSearch.CreateJournal(true);
                tmpSearch.btnApproval.Visible = false;
                //tmpSearch.WherePlus = "";
                //tmpSearch.SetSchemaTable(SchemaName, TableName);
                tmpSearch.Show();
            }
            else
            {
                tmpSearch.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void userInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccessRight.UserProfile F = new AccessRight.UserProfile();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void SalesSalesAgreementMenu_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "a.OrderDate, a.SalesAgreementNo, a.TransStatus, b.Deskripsi, a.TransType, a.CustName, a.CurrencyID, a.Total_Nett, a.TermofPayment, a.PaymentModeID, a.DPType, a.DPAmount, a.DPDueDate, a.ValidTo, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 and b.TransCode = 'SA'";
            List<string> headerText = new List<string> { "No", "SA Date", "SA No", "Status ID", "Status", "Type", "Customer Name", "Currency", "Total Nett", "Term of Payment", "Payment Mode", "DP Type", "DP Amount", "DP Due Date", "Valid To", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            F.SetMode("SalesAgreementNo", "dbo", "SalesAgreementH", "[dbo].[SalesAgreementH] a left join TransStatusTable b on a.TransStatus = b.StatusCode", fieldName, where, "Sales Agreement", "SAInq", headerText);
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void llSA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SalesSalesAgreementMenu_Click(new object(), new EventArgs());
        }

        private void InventoryGoodsReceivedMenu_Click(object sender, EventArgs e)
        {
            Purchase.GoodsReceipt.InquiryGoodsReceipt F = new Purchase.GoodsReceipt.InquiryGoodsReceipt("Nota Transfer");

            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void SalesGoodsReceivedMenu_Click(object sender, EventArgs e)
        {
            Purchase.GoodsReceipt.InquiryGoodsReceipt F = new Purchase.GoodsReceipt.InquiryGoodsReceipt("Nota Retur Jual");

            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void PurchaseNotaReturBeliCreateMenu_Click(object sender, EventArgs e)
        {
            Purchase.NotaReturBeli.InqReturBeli F = new Purchase.NotaReturBeli.InqReturBeli();
            //begin
            //updated by : Mujib
            //updated date : 12 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void PurchaseNotaReturBeliApproveMenu_Click(object sender, EventArgs e)
        {
            Purchase.NotaReturBeli.InqNRBApproval F = new Purchase.NotaReturBeli.InqNRBApproval();
            //begin
            //updated by : Mujib
            //updated date : 12 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName == "Purchase Manager")
                {
                    F.Show();
                }
                else
                {
                    MessageBox.Show("Maaf approval hanya bisa dilakukan oleh:\nPurchase Manager", "Warning!!!", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void SalesNotaReturJualApprove_Click(object sender, EventArgs e)
        {
            Sales.NotaReturJual.InqNRJApproval F = new Sales.NotaReturJual.InqNRJApproval();
            //begin
            //updated by : Mujib
            //updated date : 18 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Stock Manager")
                {
                    F.Show();
                }
                else
                {
                    MessageBox.Show("Maaf approval hanya bisa dilakukan oleh:\nSales Manager atau Stock Manager", "Warning!!!", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void puchaseInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //end 
        }

        private void InventoryStockViewMenu_Click(object sender, EventArgs e)
        {
            Inventory.StockView.StockViewHeader F = new Inventory.StockView.StockViewHeader();
            F.Show();
        }

        private void pricelistConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.PricelistConfig.PricelistConfigInquiry F = new Master.PricelistConfig.PricelistConfigInquiry();
            //begin
            //updated by : Joshua
            //updated date : 25 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void createToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            AccountPayable.InquiryAccountsPayable F = new AccountPayable.InquiryAccountsPayable();
            //begin
            //updated by : Mujib
            //updated date : 18 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }


        }

        private void approveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AccountPayable.InquiryAccountPayableApproval F = new AccountPayable.InquiryAccountPayableApproval();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void pricelistJualNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pricelist.PricelistInquiry F = new Pricelist.PricelistInquiry();
            //begin
            //updated by : Joshua
            //updated date : 25 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName.ToUpper() == "ADMINISTRATOR" || ControlMgr.GroupName.ToUpper() == "SALES MANAGER" || ControlMgr.GroupName.ToUpper() == "SALES ADMIN")
                {
                    F.PricelistTypes("Jual");
                    F.Text = "Pricelist Jual Inquiry";
                    F.Show();
                }
                else
                {
                    F.Close();
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }  
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void pricelistBeliNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pricelist.PricelistInquiry F = new Pricelist.PricelistInquiry();
            //begin
            //updated by : Joshua
            //updated date : 25 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName.ToUpper() == "ADMINISTRATOR" || ControlMgr.GroupName.ToUpper() == "PURCHASE MANAGER" || ControlMgr.GroupName.ToUpper() == "PURCHASE ADMIN")
                {
                    F.PricelistTypes("Beli");
                    F.Text = "Pricelist Beli Inquiry";
                    F.Show();
                }
                else
                {
                    F.Close();
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }                
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void pricelistConfigToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Master.PricelistConfig.PricelistConfigInquiry F = new Master.PricelistConfig.PricelistConfigInquiry();
            //begin
            //updated by : Joshua
            //updated date : 25 Apr 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            
        }

        private void goodsIssuedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = " CONVERT(nvarchar(30),CAST(a.GoodsIssuedDate AS DATE),103) as 'GI Date', a.GoodsIssuedId, a.StatusCode, b.Deskripsi, a.AccountNum, a.AccountName, a.RefTransID, a.RefTransDate, a.Notes, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 and b.TransCode = 'GI' and a.RefTransID like '%NT%'";
            List<String> headerText = new List<string> { "No", "GI Date", "GI ID", "Status Code", "Status", "Account ID", "Account Name", "Reference ID", "Reference Date", "Notes", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            F.SetMode3("GoodsIssuedId", "dbo", "GoodsIssuedH", "[dbo].[GoodsIssuedH] a left join TransStatusTable b on a.StatusCode = b.StatusCode", fieldName, where, "Bukti Barang Keluar", "BBKInq", headerText, "NOTA TRANSFER");
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void aRCollectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ARCollection.ARCollectionInquiry F = new ISBS_New.ARCollection.ARCollectionInquiry();
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
        }

        private void InventoryNotaTransferMenu_Click(object sender, EventArgs e)
        {
            Inventory.NotaTransfer.NT_Inquiry F = new Inventory.NotaTransfer.NT_Inquiry();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void tandaTerimaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ARCollection.TandaTerimaInquiry F = new ARCollection.TandaTerimaInquiry();
            F.Show();
        }

        private void createToolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
            ARCollection.Collection.CollectionTaskList F = new ISBS_New.ARCollection.Collection.CollectionTaskList();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void resultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ARCollection.CollectionResult.CLR_Inquery F = new ARCollection.CollectionResult.CLR_Inquery();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void invoiceCustomerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountsReceivable.CustomerInvoice.InquiryCustomerInvoice F = new AccountsReceivable.CustomerInvoice.InquiryCustomerInvoice();
            //begin
            //updated by : Joshua
            //updated date : 23 Mei 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName.ToUpper() == "AR ADMIN" || ControlMgr.GroupName.ToUpper() == "AR MANAGER" || ControlMgr.GroupName.ToUpper() == "TAX ADMIN" || ControlMgr.GroupName.ToUpper() == "TAX MANAGER" || ControlMgr.GroupName.ToUpper() == "ADMINISTRATOR")
                {
                    F.Show();
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void PaymentVoucher_Click(object sender, EventArgs e)
        {
            
        }

        private void resultToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //AccountsReceivable.CollectionResult.InquiryCollectionResult F = new AccountsReceivable.CollectionResult.InquiryCollectionResult();
            ////begin
            ////updated by : Joshua
            ////updated date : 06 Juli 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
        }

        private void receiptVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = " CONVERT(nvarchar(30),CAST(a.RV_Date AS DATE),103) as 'RV Date', a.RV_No,a.Cust_Id,a.[Cust_Name],a.[Payment_Method],a.[Bank_Id],a.[Bank_Name],a.[Giro_No],a.[Receipt_No],a.[Rek_Cust],a.[Payment_DueDate],a.[Tgl_Cair],a.[Tgl_Tolak],a.[Tgl_Pending],a.[Total_Payment],a.[Signed_Amount],a.[StatusCode],a.[Notes],a.[Notes2],a.[Notes3],a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy]";
            string where = "1=1 and b.TransCode = 'ReceiptVoucher' and a.Payment_Method != 'TRANSFER' ";
            List<String> HideField = new List<string> { "Cust_ID", "Bank_Id", "Bank_Name", "Rek_Cust", "Payment_DueDate", "Tgl_Cair", "Tgl_Tolak", "Tgl_Pending", "Total_Payment", "Signed_Amount", "Notes", "Notes2", "Notes3"};
            List<String> headerText = new List<string> { "No", "RV Date", "RV No", "Cust ID", "Customer", "Payment Method", "Bank ID", "Bank Name", "Giro No", "Receipt No", "Rek Cust", "Payment DueDate", "Tanggal Cair", "Tanggal Tolak", "Tanggal Pending", "Total Payment", "Signed Amount", "Status Code", "Notes", "Notes2", "Notes3",  "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("RV_No", "dbo", "ReceiptVoucher_H", "[dbo].[ReceiptVoucher_H] a left join TransStatusTable b on a.StatusCode = b.StatusCode", fieldName, where, "Receipt Voucher", "RVInq", headerText, "ACCOUNT RECEIVABLE",HideField);
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void tagSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Master.Invent.TagSize.InquiryTagSize F = new Master.Invent.TagSize.InquiryTagSize();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void toolStripMenuItem7_Click_1(object sender, EventArgs e)
        {
            CashAndBank.ReceiptVoucher.InquiryReceiptVoucher F = new CashAndBank.ReceiptVoucher.InquiryReceiptVoucher();
            //begin
            //updated by : Joshua
            //updated date : 09 Juli 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void CreatePaymentVoucher_Click(object sender, EventArgs e)
        {
            //Variable.Kode = null;
            //InquiryV1 tmpSearch = new InquiryV1();
            //tmpSearch.InquiryName = "InquiryNotaDebet";
            //if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            //{

            //    MDI.Add(tmpSearch);
            //    //Digunakan untuk menampilkan data grid
            //    tmpSearch.InquiryName = "PaymentVoucher";
            //    tmpSearch.Text = "Inquiry Purchase Voucher";
            //    tmpSearch.PrimaryKey = "PV_No";
            //    //tmpSearch.Table = "[dbo].[NotaDebetH]";
            //    tmpSearch.QuerySearch = "SELECT [PV_No], [PV_Date], [PaymentMethod], [Total_Payment],[Payment_DueDate],CreatedDate, CreatedBy, UpdatedDate, UpdatedBy FROM [dbo].[PaymentVoucher_H]";
            //    tmpSearch.Order = "CreatedDate Desc";
            //    tmpSearch.FilterText = new string[] { "PV_No", "PaymentMethod", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            //    tmpSearch.FilterDate = new string[] { "PV_Date", "CreatedDate", "UpdatedDate" };
            //    tmpSearch.Select = new string[] { "PV_No" };
            //    tmpSearch.Parent = this;
            //    tmpSearch.Table = "[dbo].[PaymentVoucher_H]";
            //    //tmpSearch.HideField = new string[] { "DNMode", "NRBId", "AccountNum" };
            //    //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
            //    tmpSearch.FormName = "Inquiry_Payment_Voucher";
            //    tmpSearch.visibleBtn();
            //    //Digunakan untsuk menentukan form yang akan dideletes
            //    tmpSearch.Delete = new string[] { "PV_No" };
            //    tmpSearch.btnBatal.Visible = false;
            //    tmpSearch.btnDelete.Visible = false;
            //    tmpSearch.btnGunakan.Visible = false;
            //    tmpSearch.btnApproval.Visible = false;

            //    //tmpSearch.WherePlus = "";
            //    //tmpSearch.SetSchemaTable(SchemaName, TableName);
            //    tmpSearch.Show();
            //}
            //else
            //{
            //    tmpSearch.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}

            Variable.Kode = null;
            InquiryV2 tmpSearch = new InquiryV2();
            tmpSearch.InquiryName = "InquiryNotaDebet";
            if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            {
                MDI.Add(tmpSearch);
                //Digunakan untuk menampilkan data grid
                tmpSearch.Text = "Inquiry Purchase Voucher";
                tmpSearch.PrimaryKey = "PV_No";
                tmpSearch.Table = "[dbo].[PaymentVoucher_H] a ";
                tmpSearch.Fields = "[PV_No], Convert(varchar,[PV_Date],103) as PV_Date, [PaymentMethod],(SELECT [Deskripsi] FROM [ISBS-NEW4].[dbo].[TransStatusTable] b WHERE [TransCode] = 'PaymentVoucher' AND b.[StatusCode] = a.[StatusCode]) as Deskripsi, FORMAT(Total_Payment, 'N2') Total_Payment, Convert(varchar,[Payment_DueDate],103) as Payment_DueDate,CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                //tmpSearch.QuerySearch += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
                tmpSearch.FilterText = new string[] { "PV_No", "PV_Date", "PaymentMethod", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
                tmpSearch.FilterDate = new string[] { "PV_Date", "CreatedDate", "UpdatedDate" };
                tmpSearch.Data2 = new string[] { "Deskripsi" };
                tmpSearch.Select = new string[] { "PV_No" };
                tmpSearch.Parent = this;
                //tmpSearch.Notes = "NT adalah Nota Transfer. \n Step approval : \n 1. Sales Manager. \n 2. Purchase Manager.";
                tmpSearch.TabPageName = new string[] { "OnProgress", "Completed" };
                tmpSearch.WhereTabPage = new string[] { "1=1", "1=0" };
                tmpSearch.btnApproval.Visible = false;
                tmpSearch.btnDelete.Visible = false;
                tmpSearch.btnPreview.Visible = true;

                //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
                tmpSearch.FormName = "Inquiry_Payment_Voucher";

                //Digunakan untuk menentukan form yang akan dideletes
                tmpSearch.Delete = new string[] { "PV_No" };

                //tmpSearch.WherePlus = "";
                //tmpSearch.SetSchemaTable(SchemaName, TableName);
                tmpSearch.Show();
                tmpSearch.Location = new Point(187, 62);
            }
            else
            {
                tmpSearch.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void ApprovalPaymentVoucher_Click(object sender, EventArgs e)
        {
            //Variable.Kode = null;
            //InquiryV1 tmpSearch = new InquiryV1();
            //tmpSearch.InquiryName = "InquiryNotaDebet";
            //if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            //{

            //    MDI.Add(tmpSearch);
            //    //Digunakan untuk menampilkan data grid
            //    tmpSearch.InquiryName = "ApprovalPaymentVoucher";
            //    tmpSearch.Text = "Inquiry Purchase Voucher";
            //    tmpSearch.PrimaryKey = "PV_No";
            //    //tmpSearch.Table = "[dbo].[NotaDebetH]";
            //    tmpSearch.QuerySearch = "SELECT [PV_No], [PV_Date], [PaymentMethod], [Total_Payment],[Payment_DueDate],CreatedDate, CreatedBy, UpdatedDate, UpdatedBy FROM [dbo].[PaymentVoucher_H]";
            //    tmpSearch.Order = "CreatedDate Desc";
            //    tmpSearch.FilterText = new string[] { "PV_No", "PaymentMethod", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            //    tmpSearch.FilterDate = new string[] { "PV_Date", "CreatedDate", "UpdatedDate" };
            //    tmpSearch.Select = new string[] { "PV_No" };
            //    tmpSearch.Parent = this;
            //    tmpSearch.Table = "[dbo].[PaymentVoucher_H]";
            //    //tmpSearch.HideField = new string[] { "DNMode", "NRBId", "AccountNum" };
            //    //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
            //    tmpSearch.FormName = "Inquiry_Payment_Voucher";
            //    tmpSearch.visibleBtn();
            //    //Digunakan untsuk menentukan form yang akan dideletes
            //    tmpSearch.Delete = new string[] { "PV_No" };
            //    tmpSearch.btnNew.Visible = false;
            //    tmpSearch.btnGunakan.Visible = false;
            //    tmpSearch.btnBatal.Visible = false;
            //    tmpSearch.btnSelect.Visible = false;

            //    //tmpSearch.WherePlus = "";
            //    //tmpSearch.SetSchemaTable(SchemaName, TableName);
            //    tmpSearch.Show();
            //}
            //else
            //{
            //    tmpSearch.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}

            Variable.Kode = null;
            InquiryV2 tmpSearch = new InquiryV2();
            tmpSearch.InquiryName = "InquiryNotaDebet";
            if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            {
                MDI.Add(tmpSearch);
                //Digunakan untuk menampilkan data grid
                tmpSearch.Text = "Inquiry Approval Purchase Voucher";
                tmpSearch.PrimaryKey = "PV_No";
                tmpSearch.Table = "[dbo].[PaymentVoucher_H] a ";
                tmpSearch.Fields = "[PV_No], [PV_Date], [PaymentMethod], [Total_Payment],[Payment_DueDate],(SELECT [Deskripsi] FROM [ISBS-NEW4].[dbo].[TransStatusTable] b WHERE [TransCode] = 'PaymentVoucher' AND b.[StatusCode] = a.[StatusCode]) as Deskripsi,CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                //tmpSearch.QuerySearch += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
                tmpSearch.FilterText = new string[] { "PV_No", "PV_Date", "PaymentMethod", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
                tmpSearch.FilterDate = new string[] { "PV_Date", "CreatedDate", "UpdatedDate" };
                tmpSearch.Data2 = new string[] { "Deskripsi" };
                tmpSearch.Select = new string[] { "PV_No" };
                tmpSearch.Parent = this;
                //tmpSearch.Notes = "NT adalah Nota Transfer. \n Step approval : \n 1. Sales Manager. \n 2. Purchase Manager.";
                tmpSearch.TabPageName = new string[] { "OnProgress", "Completed" };
                tmpSearch.WhereTabPage = new string[] { "StatusCode in ('01')", "StatusCode in ('02','03','04')" };
                tmpSearch.btnApproval.Visible = true;
                tmpSearch.btnNew.Visible = false;
                tmpSearch.btnSelect.Visible = false;
                tmpSearch.btnDelete.Visible = false;

                //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
                tmpSearch.FormName = "Inquiry_Payment_Voucher";


                //Digunakan untuk menentukan form yang akan dideletes
                tmpSearch.Delete = new string[] { "PV_No" };

                //tmpSearch.WherePlus = "";
                //tmpSearch.SetSchemaTable(SchemaName, TableName);
                tmpSearch.Show();
                tmpSearch.Location = new Point(187, 62);
            }
            else
            {
                tmpSearch.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void bankStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CashAndBank.BankStatement.InquiryBankStatement F = new CashAndBank.BankStatement.InquiryBankStatement();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void bankReconcileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CashAndBank.BankReconcile.BankReconcile F = new CashAndBank.BankReconcile.BankReconcile();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }        

        private void fQAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.FQA2_ID,a.FQA2_SeqNo,a.FQA2_Desc,a.[Status],a.[FQA1_ID],a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Status", "Preview", "FQA2_SeqNo", "Send Email" };
            List<String> headerText = new List<string> { "No", "FQA2_ID", "FQA2_SeqNo", "FQA2_Desc", "Status", "FQA1 ID", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("FQA2_ID", "dbo", "M_FQA2", "[dbo].[M_FQA2] a ", fieldName, where, "FQA2", "FQA2", headerText, "ACCOUNT ASSIGNMENT", HideField);
            F.btnOnProgress.Text = "Active";
            F.btnCompleted.Text = "InActive";
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void cOAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.COA_ID, a.COA_Desc, a.M_COAType, a.[M_COADesc], a.[M_COAID], a.Status, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Status", "Preview", "Send Email", "M_COAID" };
            List<String> headerText = new List<string> { "No", "COA Id", "Deskripsi", "Type", "MCOA Deskripsi", "M_COAID", "Status", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("COA_ID", "dbo", "M_COA", "M_COA a ", fieldName, where, "COA", "COA", headerText, "ACCOUNT ASSIGNMENT", HideField);
            F.btnOnProgress.Text = "Active";
            F.btnCompleted.Text = "InActive";
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void subCOAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[FQA1_ID],a.FQA1_SeqNo,a.FQA1_Desc,a.[Status],a.[COA_ID],a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Status", "Preview", "Send Email", "FQA1_SeqNo" };
            List<String> headerText = new List<string> { "No", "FQA1 ID", "FQA1_SeqNo", "FQA1_Desc", "Status", "COA ID", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("FQA1_ID", "dbo", "M_FQA1", "[dbo].[M_FQA1] a ", fieldName, where, "FQA", "FQA", headerText, "ACCOUNT ASSIGNMENT FQA", HideField);
            F.btnOnProgress.Text = "Active";
            F.btnCompleted.Text = "InActive";
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void masterCOAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[M_COAID], a.[M_COADesc], a.[M_COAType], a.[StartRangeCOA], a.[EndRangeCOA], a.[Status], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Status", "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "COA Master Id", "Deskripsi", "Type", "Start Range COA", "End Range FQA", "Status", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("M_COAID", "dbo", "M_COAMaster", "M_COAMaster a ", fieldName, where, "MASTERCOA", "MASTERCOA", headerText, "ACCOUNT ASSIGNMENT", HideField);
            F.btnOnProgress.Text = "Active";
            F.btnCompleted.Text = "InActive";
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void journalTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[JournalHID], a.[JournalHDesc], a.[Status], a.[CreatedDate], a.[CreatedBy], a.[UpdatedDate], a.[UpdatedBy]";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Status", "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "Journal Header ID", "Deskripsi", "Status", "Created Date", "Created By", "Updated Date", "Updated By" };
            F.SetMode4("JournalHID", "dbo", "M_JournalH", "M_JournalH a ", fieldName, where, "JOURNAL", "JOURNAL", headerText, "ACCOUNT ASSIGNMENT", HideField);
            F.btnOnProgress.Text = "Active";
            F.btnCompleted.Text = "InActive";
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void gLJournalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            //string fieldName = "  a.[GLJournalHID],a.JournalHID,a.Referensi,a.[Notes],a.[Posting],a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy]";
            //string where = "1=1 ";
            //List<String> HideField = new List<string> { "Posting", "Preview", "Send Email" };
            //List<String> headerText = new List<string> { "No", "GL Journal ID", "Journal HID", "Referensi", "Notes", "Posting", "Created Date", "Created By", "Updated Date", "Updated By" };
            string fieldName = "  a.[GLJournalHID],a.JournalHID,a.[Referensi],a.Notes,a.Status,a.[Posting],a.CreatedDate,a.CreatedBy,a.UpdatedDate,a.UpdatedBy ";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Posting", "Preview", "Send Email" };
            List<String> headerText = new List<string> { "No", "GLJournalHID", "JournalHID","Referensi","Notes","Status", "Posting","CreatedDate","CreatedBy","UpdatedDate","UpdatedBy" };
            F.SetMode4("GLJournalHID", "dbo", "GLJournalH", "[dbo].[GLJournalH] a ", fieldName, where, "GLJournal", "GLJournal", headerText, "ACCOUNT ASSIGNMENT GLJournal", HideField);
            F.btnOnProgress.Text = "Unposted";
            F.btnCompleted.Text = "Posted";
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

       
        //private void bankToolStripMenuItem_Click_2(object sender, EventArgs e)
        //{
            //Master.Bank.BankInquiry F = new Master.Bank.BankInquiry();
            ////begin
            ////updated by : joshua
            ////updated date : 23 feb 2018
            ////description : check permission access
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end

            //GlobalInquiry F = new GlobalInquiry();
            //string fieldName = "a.GoodsIssuedDate, a.GoodsIssuedId, a.StatusCode, b.Deskripsi, a.AccountNum, a.AccountName, a.RefTransID, a.RefTransDate, a.Notes, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            //string where = "1=1 and b.TransCode = 'GI' AND a.RefTransID NOT LIKE '%NT%' ";
            //List<String> headerText = new List<string> { "No", "GI Date", "GI ID", "Status Code", "Status", "Account ID", "Account Name", "Reference ID", "Reference Date", "Notes", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            //F.SetMode("GoodsIssuedId", "dbo", "GoodsIssuedH", "[dbo].[GoodsIssuedH] a left join TransStatusTable b on a.StatusCode = b.StatusCode", fieldName, where, "Bukti Barang Keluar", "BBKInq", headerText);
           
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
        //}

        private void canvaSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Purchase.CanvasSheet.InquiryCanvasSheet F = new Purchase.CanvasSheet.InquiryCanvasSheet();
            ISBS_New.TaskList.Purchase.TaskListCanvasSheet F = new ISBS_New.TaskList.Purchase.TaskListCanvasSheet();
           
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
          
        }

        private void purchaseRequisitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ISBS_New.TaskList.Purchase.PurchaseRequisition.TaskListPR F = new ISBS_New.TaskList.Purchase.PurchaseRequisition.TaskListPR();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.SetParent(this);
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void receiptOrderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Purchase.ReceiptOrder.InquiryReceiptOrder F = new Purchase.ReceiptOrder.InquiryReceiptOrder();
            TaskList.Purchase.ReceiptOrder.TaskListRO F = new ISBS_New.TaskList.Purchase.ReceiptOrder.TaskListRO();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void goodsReceivedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskList.Purchase.GoodsReceipt.TaskListGR F = new TaskList.Purchase.GoodsReceipt.TaskListGR("Receipt Order");

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void notToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // Sales.NotaReturJual.InqNRJApproval F = new Sales.NotaReturJual.InqNRJApproval();
            TaskList.Sales.NotaReturJual.TaskListNRJ F = new ISBS_New.TaskList.Sales.NotaReturJual.TaskListNRJ();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Stock Manager")
                {
                    F.Show();
                }
                else
                {
                    MessageBox.Show("Maaf approval hanya bisa dilakukan oleh:\nSales Manager atau Stock Manager", "Warning!!!", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }
        //tia edit
        private void purchaseRequisitionToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ISBS_New.TaskList.Purchase.PurchaseRequisition.TaskListPR F = new ISBS_New.TaskList.Purchase.PurchaseRequisition.TaskListPR();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.SetParent(this);
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }

        }

        private void canvasSheetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Purchase.CanvasSheet.InquiryCanvasSheet F = new Purchase.CanvasSheet.InquiryCanvasSheet();
            ISBS_New.TaskList.Purchase.TaskListCanvasSheet F = new ISBS_New.TaskList.Purchase.TaskListCanvasSheet();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }

        }

        private void receivedOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Purchase.ReceiptOrder.InquiryReceiptOrder F = new Purchase.ReceiptOrder.InquiryReceiptOrder();
            TaskList.Purchase.ReceiptOrder.TaskListRO F = new ISBS_New.TaskList.Purchase.ReceiptOrder.TaskListRO();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }

        }

        private void goodReceiptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskList.Purchase.GoodsReceipt.TaskListGR F = new TaskList.Purchase.GoodsReceipt.TaskListGR("Receipt Order");

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }

        }

        private void notaReturJualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Sales.NotaReturJual.InqNRJApproval F = new Sales.NotaReturJual.InqNRJApproval();
            TaskList.Sales.NotaReturJual.TaskListNRJ F = new ISBS_New.TaskList.Sales.NotaReturJual.TaskListNRJ();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName == "Sales Manager" || ControlMgr.GroupName == "Stock Manager")
                {
                    F.Show();
                }
                else
                {
                    MessageBox.Show("Maaf approval hanya bisa dilakukan oleh:\nSales Manager atau Stock Manager", "Warning!!!", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void goodsIssuedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TaskList.GlobalTasklist F = new ISBS_New.TaskList.GlobalTasklist();
            string fieldName = "a.GoodsIssuedDate, a.GoodsIssuedId, a.StatusCode, b.Deskripsi, a.AccountNum, a.AccountName, a.RefTransID, a.RefTransDate, a.Notes, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 and b.TransCode = 'GI' AND a.RefTransID NOT LIKE '%NT%' ";
            List<String> headerText = new List<string> { "No", "GI Date", "GI ID", "Status Code", "Status", "Account ID", "Account Name", "Reference ID", "Reference Date", "Notes", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            F.SetMode("GoodsIssuedId", "dbo", "GoodsIssuedH", "[dbo].[GoodsIssuedH] a left join TransStatusTable b on a.StatusCode = b.StatusCode", fieldName, where, "Bukti Barang Keluar", "BBKInq", headerText);
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end

        }

        private void purchaseAgreementToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Purchase.PurchaseAgreement.PAInq F = new Purchase.PurchaseAgreement.PAInq();
            TaskList.Purchase.PurchaseAgreement.TaskListPA F = new ISBS_New.TaskList.Purchase.PurchaseAgreement.TaskListPA();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
                refreshTaskList();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void salesAgreementToolStripMenuItem_Click(object sender, EventArgs e)
        {
           TaskList.GlobalTasklist F = new ISBS_New.TaskList.GlobalTasklist();
            string fieldName = "a.OrderDate, a.SalesAgreementNo, a.TransStatus, b.Deskripsi, a.TransType, a.CustName, a.CurrencyID, a.Total_Nett, a.TermofPayment, a.PaymentModeID, a.DPType, a.DPAmount, a.DPDueDate, a.ValidTo, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 and b.TransCode = 'SA'";
            List<string> headerText = new List<string> { "No", "SA Date", "SA No", "Status ID", "Status", "Type", "Customer Name", "Currency", "Total Nett", "Term of Payment", "Payment Mode", "DP Type", "DP Amount", "DP Due Date", "Valid To", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            F.SetMode("SalesAgreementNo", "dbo", "SalesAgreementH", "[dbo].[SalesAgreementH] a left join TransStatusTable b on a.TransStatus = b.StatusCode", fieldName, where, "Sales Agreement", "SAInq", headerText);
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void salesOrderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
           TaskList.GlobalTasklist F = new ISBS_New.TaskList.GlobalTasklist();
            string fieldName = "a.OrderDate, a.SalesOrderNo, a.TransStatus, b.Deskripsi, a.CustName, a.CurrencyID, a.Total_Nett, a.TermofPayment, a.PaymentModeID, a.DPType, a.DPAmount, a.DPDueDate, a.ValidTo, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy";
            string where = "1=1 and b.TransCode = 'SalesOrder'";
            List<string> headerText = new List<string> { "No", "SO Date", "SO No", "Status ID", "Status", "Customer Name", "Currency", "Total Nett", "Term of Payment", "Payment Mode", "DP Type", "DP Amount", "DP Due Date", "Valid To", "Created Date", "Created By", "Updated Date", "Updated By", "Preview", "Send Email" };
            F.SetMode("SalesOrderNo", "dbo", "SalesOrderH", "SalesOrderH a left join TransStatusTable b on a.TransStatus = b.StatusCode", fieldName, where, "Sales Order", "SOInq", headerText);
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void notaCreditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Variable.Kode = null;
            //InquiryV1 tmpSearch = new InquiryV1();
            //tmpSearch.InquiryName = "InquiryNotaCredit";
            //if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    MDI.Add(tmpSearch);
            //    //Digunakan untuk menampilkan data grid
            //    tmpSearch.Text = "Inquiry Nota Credit";
            //    tmpSearch.PrimaryKey = "CN_No";
            //    //tmpSearch.Table = "[dbo].[NotaDebetH]";
            //    tmpSearch.QuerySearch = "SELECT a.[CN_Date],a.[CN_No], a.[CNMode], a.[NRJId], a.[AccountNum], a.[AccountName],b.[Deskripsi],a.[CreatedDate],a.[CreatedBy],a.[UpdatedDate],a.[UpdatedBy] FROM NotaCreditH a LEFT JOIN [TransStatusTable] b ON a.[TransStatus]=b.[StatusCode] WHERE [TransCode]='NotaCredit' ";
            //    tmpSearch.Order = "CreatedDate Desc";
            //    tmpSearch.FilterText = new string[] { "CN_No", "CN_Date", "AccountNum", "AccountName", "Deskripsi", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            //    tmpSearch.Select = new string[] { "CN_No", "CN_Date" };
            //    tmpSearch.Parent = this;
            //    tmpSearch.Table = "NotaCreditH";
            //    tmpSearch.HideField = new string[] { "CNMode", "NRJId", "AccountNum" };
            //    //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
            //    tmpSearch.FormName = "Inquiry_Nota_Credit";
            //    tmpSearch.visibleBtn();
            //    //Digunakan untsuk menentukan form yang akan dideletes
            //    tmpSearch.Delete = new string[] { "CN_No" };

            //    //tmpSearch.WherePlus = "";
            //    //tmpSearch.SetSchemaTable(SchemaName, TableName);
            //    tmpSearch.Show();
            //}
            //else
            //{
            //    tmpSearch.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
        }

        private void notaPurchaseParkedToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            TaskList.Purchase.NotaPurchaseParked.TaskListNotaPurchaseParked F = new TaskList.Purchase.NotaPurchaseParked.TaskListNotaPurchaseParked();
           
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void pricelistBeliToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           TaskList.Pricelist.TasklistPricelist F = new ISBS_New.TaskList.Pricelist.TasklistPricelist();
          
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName.ToUpper() == "ADMINISTRATOR" || ControlMgr.GroupName.ToUpper() == "PURCHASE MANAGER" || ControlMgr.GroupName.ToUpper() == "PURCHASE ADMIN")
                {
                    F.PricelistTypes("Beli");
                    F.Text = "Pricelist Beli Inquiry";
                    F.Show();
                }
                else
                {
                    F.Close();
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            
        }

        private void pricelistJualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskList.Pricelist.TasklistPricelist F = new ISBS_New.TaskList.Pricelist.TasklistPricelist();
          
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName.ToUpper() == "ADMINISTRATOR" || ControlMgr.GroupName.ToUpper() == "SALES MANAGER" || ControlMgr.GroupName.ToUpper() == "SALES ADMIN" || ControlMgr.GroupName.ToUpper() == "PURCHASE MANAGER")
                {
                    F.PricelistTypes("Jual");
                    F.Text = "Pricelist Jual Inquiry";
                    F.Show();
                }
                else
                {
                    F.Close();
                    MessageBox.Show(ControlMgr.PermissionDenied);
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void notaReturBeliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.NotaReturBeli.InqNRBApproval F = new Purchase.NotaReturBeli.InqNRBApproval();
            
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName == "Purchase Manager")
                {
                    F.Show();
                }
                else
                {
                    MessageBox.Show("Maaf approval hanya bisa dilakukan oleh:\nPurchase Manager", "Warning!!!", MessageBoxButtons.OK);
                    return;
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void deliveryOrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskList.GlobalTasklist F = new ISBS_New.TaskList.GlobalTasklist();
           // TaskList.DeliveryOrder.TaskListDeliveryOrder F = new TaskList.DeliveryOrder.TaskListDeliveryOrder();
            //string fieldName = "a.DeliveryOrderId 'DO No', a.DeliveryOrderDate 'DO Date', a.DeliveryDate 'Delivery Date', a.CustID 'Customer', a.InventSiteID 'Warehouse',a.DeliveryOrderStatus, b.Deskripsi, a.SalesOrderId 'SO No', a.SalesOrderDate 'SO Date', a.CreatedBy, a.CreatedDate,a.UpdatedBy,a.UpdatedDate ";
            string fieldName = "a.DeliveryOrderId, a.DeliveryOrderDate, a.DeliveryDate, a.CustID, a.CustName, a.InventSiteID,a.DeliveryOrderStatus, b.Deskripsi, a.SalesOrderId, a.SalesOrderDate, a.CreatedBy, a.CreatedDate,a.UpdatedBy, a.UpdatedDate ";
            List<string> headerText = new List<string> { "No", "DO No", "DO Date", "Delivery Date", "Customer DI","Customer Name", "Warehouse", "Status ID", "Status", "SO No", "SO Date", "Created By", "Created Date", "Updated By", "Updated Date", "Preview", "Send Email" };
            F.SetMode("DeliveryOrderId", "dbo", "DeliveryOrderH", "[dbo].[DeliveryOrderH] a left join TransStatusTable b on a.DeliveryOrderStatus = b.StatusCode", fieldName, "1=1 and a.DeliveryOrderStatus != '07' and b.TransCode = 'DO'", "Delivery Order", "DOInq", headerText);
            //begin
            //updated by : joshua
            //updated date : 22 feb 2018
            //description : check permission access
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end 
        }

        private void approvalSODPRequiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AccountsReceivable.Transaction.AprrovalSODPReq.frmT_FlagSO2DO F = new AccountsReceivable.Transaction.AprrovalSODPReq.frmT_FlagSO2DO();

            //if (ControlMgr.GroupName == "AR Manager")
            //{
            //    F.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Maaf approval hanya bisa dilakukan oleh:\nAR Manager", "Warning!!!", MessageBoxButtons.OK);
            //    return;
            //}
        }

        private void purchaseOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase.PurchaseOrderApproval.POInquiryApproval F = new Purchase.PurchaseOrderApproval.POInquiryApproval();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void purchaseInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskList.AccountPayable.TasklistPurchaseInvoiceAP F = new ISBS_New.TaskList.AccountPayable.TasklistPurchaseInvoiceAP();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }
        //tia edit end
        private void paymentVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void creditLimitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalInquiry F = new GlobalInquiry();
            string fieldName = "  a.[Trans_No],a.[Trans_Date],a.[Customer_Id],a.[CustName] as 'CustomerName',a.[Limit_Temp],a.[StatusCode],a.[ApprovedDate],a.[ApprovedBy],a.CreatedDate,a.CreatedBy,a.UpdatedDate,a.UpdatedBy ";
            string where = "1=1 ";
            List<String> HideField = new List<string> { "Customer_Id", "Preview","Send Email"};
            List<String> headerText = new List<string> { "No","ID", "Date", "Customer ID", "Customer Name", "Limit Temporary", "Status", "ApprovedDate", "ApprovedBy", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
            F.SetMode4("Trans_No", "dbo", "CreditLimit", "(SELECT a.*,b.CustName FROM [dbo].[CreditLimit] a LEFT JOIN [CustTable] b ON a.[Customer_Id]=b.[CustId]) a", fieldName, where, "CreditLimitInq", "CreditLimitInq", headerText, "Credit Limit", HideField);
            F.btnOnProgress.Text = "Pending";
            F.btnCompleted.Text = "Approved";
            F.btnClosed.Visible = false;
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void bentukToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Inventory.Master.Bentuk.FrmL_Bentuk F = new Inventory.Master.Bentuk.FrmL_Bentuk();
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void MenuTransaksiPaymentGiro_Click(object sender, EventArgs e)
        {
            //GlobalInquiry F = new GlobalInquiry();
            //string fieldname = " [PV_No], PV_Date, PaymentMethod, Notes, Total_Payment, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy";
            //string where = " PaymentMethod='CHEQUE' and StatusCode in ('04','05','06')";
            ////List<string> headerText = new List<string> { "No", "NRZ No", "NRZ Date", "GR No", "GR Date", "Warehouse", "Status", "Created Date", "Created By", "Updated Date", "Updated By" };
            //F.SetMode2("PV_No", "dbo", "PaymentVoucher_H", "[dbo].[PaymentVoucher_H] a", fieldname, where, "Payment Voucher Giro", "Payment Voucher Giro");
            //if (F.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    F.Show();
            //}
            //else
            //{
            //    F.Close();
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}

            Variable.Kode = null;
            InquiryV2 tmpSearch = new InquiryV2();
            tmpSearch.InquiryName = "Inquiry_Payment_Voucher_Giro";
            if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            {
                MDI.Add(tmpSearch);
                //Digunakan untuk menampilkan data grid
                tmpSearch.Text = "Inquiry Payment Voucher Giro";
                tmpSearch.PrimaryKey = "PV_No";
                tmpSearch.Table = "[dbo].[PaymentVoucher_H] a ";
                tmpSearch.Fields = "[PV_No], Convert(varchar,[PV_Date],103) as PV_Date, [PaymentMethod],(SELECT [Deskripsi] FROM [ISBS-NEW4].[dbo].[TransStatusTable] b WHERE [TransCode] = 'PaymentVoucher' AND b.[StatusCode] = a.[StatusCode]) as Deskripsi, FORMAT(Total_Payment, 'N2') Total_Payment,Convert(varchar,[Payment_DueDate],103) as Payment_DueDate,CreatedDate, CreatedBy, UpdatedDate, UpdatedBy ";
                //tmpSearch.QuerySearch += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
                tmpSearch.FilterText = new string[] { "PV_No", "PV_Date", "PaymentMethod", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy" };
                tmpSearch.FilterDate = new string[] { "PV_Date", "CreatedDate", "UpdatedDate" };
                tmpSearch.Data2 = new string[] { "Deskripsi" };
                tmpSearch.Select = new string[] { "PV_No" };
                tmpSearch.Parent = this;
                //tmpSearch.Notes = "NT adalah Nota Transfer. \n Step approval : \n 1. Sales Manager. \n 2. Purchase Manager.";
                tmpSearch.TabPageName = new string[] { "OnProgress", "Completed" };
                tmpSearch.WhereTabPage = new string[] { "PaymentMethod='CHEQUE' and StatusCode in ('04','06')", "PaymentMethod='CHEQUE' and StatusCode in ('05','07')" };
                tmpSearch.btnApproval.Visible = false;
                tmpSearch.btnDelete.Visible = false;

                //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
                tmpSearch.FormName = "Inquiry_Payment_Voucher_Giro";

                //Digunakan untuk menentukan form yang akan dideletes
                tmpSearch.Delete = new string[] { "PV_No" };

                //tmpSearch.WherePlus = "";
                //tmpSearch.SetSchemaTable(SchemaName, TableName);
                tmpSearch.Show();
                tmpSearch.Location = new Point(187, 62);
            }
            else
            {
                tmpSearch.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void customerInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaskList.AccountsReceivable.TasklistCustomerInvoice F = new TaskList.AccountsReceivable.TasklistCustomerInvoice();
            
            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                if (ControlMgr.GroupName.ToUpper() == "AR ADMIN" || ControlMgr.GroupName.ToUpper() == "AR MANAGER" || ControlMgr.GroupName.ToUpper() == "TAX ADMIN" || ControlMgr.GroupName.ToUpper() == "TAX MANAGER" || ControlMgr.GroupName.ToUpper() == "ADMINISTRATOR")
                {
                    F.Show();
                }
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void llPA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            purchaseAgreementToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void llPOAproval_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            approveToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void llRO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            receiptOrderToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void llPO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            createToolStripMenuItem1_Click(new object(), new EventArgs());
        }

        private void llCreateGR_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            goodsReceiptToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void requestCancelDOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ISBS_New.Sales.DeliveryOrder.RequestCancelDO F = new ISBS_New.Sales.DeliveryOrder.RequestCancelDO();
            F.Show();
        }

        private void requestCancelDOToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ISBS_New.TaskList.TaskListApproval F = new ISBS_New.TaskList.TaskListApproval();

            Query = "SELECT [DeliveryOrderId], [DeliveryOrderDate], [CustName] FROM [DeliveryOrderH] WHERE [DeliveryOrderId] IN (";

            Query += "SELECT a.DeliveryOrderId FROM DeliveryOrderH a ";
            Query += "LEFT JOIN DeliveryOrderD b ON a.DeliveryOrderId = b.DeliveryOrderId ";           
            Query += "WHERE [DeliveryOrderStatus] IN ";
            if (ControlMgr.UserId == "Sales Manager")
                Query += "('11') ";
            if (ControlMgr.UserId == "Expedisi Manager")
                Query += "('12')";
            if (ControlMgr.GroupName == "Administrator") //cuma buat ngecek
                Query += "('11','12') ";
            Query += "GROUP BY a.DeliveryOrderId ";
            Query += "HAVING SUM(b.Qty) = SUM(b.RemainingQty) ";
            Query += ")";

            F.SetMode("ApproveReject", "Request Cancel DO", Query, "DeliveryOrderId");
            F.FilterText = new string[] { "DeliveryOrderId", "CustName" ,"DeliveryOrderDate"};
            F.Show();
        }

        private void dOCancelledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ISBS_New.TaskList.TaskListApproval F = new ISBS_New.TaskList.TaskListApproval();

            Query = "SELECT [DeliveryOrderId], [DeliveryOrderDate], [CustName],b.[Deskripsi] ";
            Query += "FROM [DeliveryOrderH] a ";
            Query += "LEFT JOIN [TransStatusTable] b ON a.[DeliveryOrderStatus] = b.[StatusCode] ";
            Query += "WHERE [DeliveryOrderStatus] IN ('11','12','13') ";
            Query += "AND [TransCode] = 'DO'";

            F.SetMode("View", "DO Cancelled", Query, "DeliveryOrderId");
            F.FilterText = new string[] { "DeliveryOrderId", "CustName", "Deskripsi","DeliveryOrderDate" };
            F.Show();
        }

        private void eFakturToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EFaktur.EFaktur F = new EFaktur.EFaktur();

            if (F.PermissionAccess(ControlMgr.View) > 0)
            {
                F.Show();
            }
            else
            {
                F.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void CreateNewPaymentInvoice_Click(object sender, EventArgs e)
        {
            Variable.Kode = null;
            InquiryV2 tmpSearch = new InquiryV2();
            tmpSearch.InquiryName = "Inquiry Purchase Invoice";
            if (tmpSearch.PermissionAccess(ControlMgr.View) > 0)
            {
                MDI.Add(tmpSearch);
                //Digunakan untuk menampilkan data grid
                tmpSearch.Text = "Inquiry Purchase Invoice";
                tmpSearch.PrimaryKey = "InvoiceId";
                tmpSearch.Table = "[dbo].[VendInvoiceH] a Left join TransStatusTable b on b.StatusCode = a.TransStatus and b.TransCode='VendInvoice'";
                tmpSearch.Fields = "InvoiceDate,InvoiceId, CASE WHEN (Settle_Amount = 0) THEN 'Unpaid' WHEN (Settle_Amount + [Additional_Disc] = ";
                tmpSearch.Fields += "[InvoiceAmount]) THEN 'Paid' ELSE 'Paid-Outstanding' END Paid_Status,[InvoiceType],VendId, VendName,DueDate, ";
                tmpSearch.Fields += "b.Deskripsi, TransStatus, CreatedDate, CreatedBy,UpdatedDate, UpdatedBy, ApprovedBy ";
                //tmpSearch.QuerySearch += "Left join TransStatusTable b on a.TransStatus = b.StatusCode and TransCode='PR' ";
                tmpSearch.FilterText = new string[] { "InvoiceId", "InvoiceType", "VendId", "VendName", "TransStatus", "CreatedBy", "UpdatedBy" };
                tmpSearch.FilterDate = new string[] { "InvoiceDate", "DueDate", "CreatedDate", "UpdatedDate" };
                tmpSearch.Data2 = new string[] { "Deskripsi" };
                tmpSearch.Select = new string[] { "InvoiceId" };
                tmpSearch.Parent = this;
                //tmpSearch.Notes = "NT adalah Nota Transfer. \n Step approval : \n 1. Sales Manager. \n 2. Purchase Manager.";
                tmpSearch.TabPageName = new string[] { "OnProgress", "Completed" };
                tmpSearch.WhereTabPage = new string[] { " 1=1 ", "1=1" };
                tmpSearch.btnApproval.Visible = false;
                tmpSearch.btnDelete.Visible = false;
                tmpSearch.btnPreview.Visible = true;

                //Digunakan untuk menentukan form yang akan dibuka (New) dan (Select).
                tmpSearch.FormName = "Inquiry Purchase Invoice";

                //Digunakan untuk menentukan form yang akan dideletes
                tmpSearch.Delete = new string[] { "InvoiceId" };

                //tmpSearch.WherePlus = "";
                //tmpSearch.SetSchemaTable(SchemaName, TableName);
                tmpSearch.Show();
                tmpSearch.Location = new Point(187, 62);
            }
            else
            {
                tmpSearch.Close();
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
        }

        private void bankToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form FrmL_Bank = new Master.Bank.FrmL_Bank();
            FrmL_Bank.Show();
        }

        private void itemDPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form FrmL_ItemDP = new Master.ItemDP.FrmL_ItemDP();
            FrmL_ItemDP.Show();
        }            
    }
}
