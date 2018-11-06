using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportSource;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New
{
    public partial class GlobalPreview : MetroFramework.Forms.MetroForm
    {
        private string FormName, PreviewId, Mode, Param1;
        InquiryV2 Parent1 = new InquiryV2();

        public GlobalPreview(string formname, string parsedID)
        {
            this.FormName = formname;
            this.PreviewId = parsedID;
            InitializeComponent();
        }

        //Add Surya 3 Okt 2018
        public GlobalPreview(string formname, string parsedID, string Param1)
        {
            this.FormName = formname;
            this.PreviewId = parsedID;
            this.Param1 = Param1;
            InitializeComponent();
        }
        //End 

        public void SetMode(string mode)
        {
            this.Mode = mode;
        }

        public void SetParent(InquiryV2 F)
        {
            Parent1 = F;
        }

        private void GlobalPreview_Load(object sender, EventArgs e)
        {
            this.Text = "Preview " + FormName;
            this.WindowState = FormWindowState.Maximized;
            LoadDocument();
        }

        private void LoadDocument()
        {
            if (FormName == "Goods Issued")
            {
                if (Mode == "Surat Jalan")
                {
                    ISBS_New.Sales.BBK.Rpt_GI_SuratJalan rpt = new ISBS_New.Sales.BBK.Rpt_GI_SuratJalan();
                    rpt.SetParameterValue("@GoodsIssuedId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
                else if (Mode == "Ticket")
                {
                    ISBS_New.Sales.BBK.Rpt_GI_Ticket rpt = new ISBS_New.Sales.BBK.Rpt_GI_Ticket();
                    rpt.SetParameterValue("@GoodsIssuedId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
            }
            else if (FormName == "Goods Receipt")
            {
                if (Mode == "TT")
                {
                    ISBS_New.Purchase.GoodsReceipt.Rpt_GR rpt = new ISBS_New.Purchase.GoodsReceipt.Rpt_GR();
                    rpt.SetParameterValue("@GoodsReceivedId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
                else if (Mode == "BM")
                {
                    ISBS_New.Purchase.GoodsReceipt.Rpt_TiketBongkarMuat rpt = new ISBS_New.Purchase.GoodsReceipt.Rpt_TiketBongkarMuat();
                    rpt.SetParameterValue("@GoodsReceivedId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
            }
            else if (FormName == "Delivery Order")
            {                
                ISBS_New.Sales.DeliveryOrder.Rpt_DO rpt = new ISBS_New.Sales.DeliveryOrder.Rpt_DO();
                rpt.SetParameterValue("@DeliveryOrderId", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Sales Agreement")
            {
                ISBS_New.Sales.SalesAgreement.Rpt_SA rpt = new ISBS_New.Sales.SalesAgreement.Rpt_SA();
                rpt.SetParameterValue("@SalesAgreementNo", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Sales Order")
            {
                ISBS_New.Sales.SalesOrder.Rpt_SO rpt = new ISBS_New.Sales.SalesOrder.Rpt_SO();
                rpt.SetParameterValue("@SalesOrderNo", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Receipt Order")
            {
                ISBS_New.Purchase.ReceiptOrder.Rpt_RO rpt = new ISBS_New.Purchase.ReceiptOrder.Rpt_RO();
                rpt.SetParameterValue("@ReceiptOrderId", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Nota Retur Jual")
            {
                ISBS_New.Sales.NotaReturJual.Rpt_NRJ rpt = new ISBS_New.Sales.NotaReturJual.Rpt_NRJ();
                rpt.SetParameterValue("@NRJId", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Nota Retur Beli")
            {
                ISBS_New.Purchase.NotaReturBeli.Rpt_NRB rpt = new ISBS_New.Purchase.NotaReturBeli.Rpt_NRB();
                rpt.SetParameterValue("@NRBId", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Sales Quotation")
            {
                ISBS_New.Sales.SalesQuotation.Rpt_SQ rpt = new ISBS_New.Sales.SalesQuotation.Rpt_SQ();
                rpt.SetParameterValue("@SalesQuotationNo", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Purchase Agreement")
            {
                if (Mode == "Rinci")
                {
                    ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Rinci rpt = new ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Rinci();
                    rpt.SetParameterValue("@PAId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
                else if (Mode == "Tidak Rinci")
                {
                    ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Ga_Rinci rpt = new ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Ga_Rinci();
                    rpt.SetParameterValue("@PAId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
            }
            else if (FormName == "Purchase Order")
            {
                ISBS_New.Purchase.PurchaseOrderNew.POPreview rpt = new ISBS_New.Purchase.PurchaseOrderNew.POPreview();
                rpt.SetParameterValue("@PurchID", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Request For Quotation")
            {
                if (Mode == "Fix")
                {
                    ISBS_New.Purchase.RFQ.Rpt_RFQ rpt = new ISBS_New.Purchase.RFQ.Rpt_RFQ();
                    rpt.SetParameterValue("@RfqId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
                else if (Mode == "Rinci")
                {
                    ISBS_New.Purchase.RFQ.Rpt_RFQ_Rinci rpt = new ISBS_New.Purchase.RFQ.Rpt_RFQ_Rinci();
                    rpt.SetParameterValue("@RfqId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
                else if (Mode == "Tidak Rinci")
                {
                    ISBS_New.Purchase.RFQ.Rpt_RFQ_Ga_Rinci rpt = new ISBS_New.Purchase.RFQ.Rpt_RFQ_Ga_Rinci();
                    rpt.SetParameterValue("@RfqId", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                }
            }
            else if (FormName == "Account Payable")
            {
                ISBS_New.AccountPayable.Rpt_AP rpt = new ISBS_New.AccountPayable.Rpt_AP();
                rpt.SetParameterValue("@InvoiceId", PreviewId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                crystalReportViewer1.ReportSource = rpt;
                crystalReportViewer1.Refresh();
            }
            else if (FormName == "Inquiry_Payment_Voucher")
            {
                //if (Param1.ToUpper() == "CASH")
                //{
                    ISBS_New.AccountPayable.Rpt_PV_Cash rpt = new ISBS_New.AccountPayable.Rpt_PV_Cash();
                    rpt.SetParameterValue("@PVNo", PreviewId);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    crystalReportViewer1.ReportSource = rpt;
                    crystalReportViewer1.Refresh();
                //}
                //if (Param1.ToUpper() == "CHEQUE")
                //{
                //    ISBS_New.AccountPayable.Rpt_PV_Cheque rpt = new ISBS_New.AccountPayable.Rpt_PV_Cheque();
                //    rpt.SetParameterValue("@PVNo", PreviewId);

                //    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                //    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                //    {
                //        myLogin = myTable.LogOnInfo;
                //        myLogin.ConnectionInfo.Password = "sql123";
                //        myLogin.ConnectionInfo.UserID = "sa";
                //        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                //        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                //        myTable.ApplyLogOnInfo(myLogin);
                //    }
                //    crystalReportViewer1.ReportSource = rpt;
                //    crystalReportViewer1.Refresh();
                //}
                //if (Param1.ToUpper() == "TRANSFER")
                //{
                //    ISBS_New.AccountPayable.Rpt_PV_Transfer rpt = new ISBS_New.AccountPayable.Rpt_PV_Transfer();
                //    rpt.SetParameterValue("@PVNo", PreviewId);

                //    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                //    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                //    {
                //        myLogin = myTable.LogOnInfo;
                //        myLogin.ConnectionInfo.Password = "sql123";
                //        myLogin.ConnectionInfo.UserID = "sa";
                //        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                //        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                //        myTable.ApplyLogOnInfo(myLogin);
                //    }
                //    crystalReportViewer1.ReportSource = rpt;
                //    crystalReportViewer1.Refresh();
                //}
                
            }
            else
                MessageBox.Show("Error : Form Name salah");            
        }
    }
}
