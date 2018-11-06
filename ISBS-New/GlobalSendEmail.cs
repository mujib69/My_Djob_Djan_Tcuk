using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Diagnostics;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace ISBS_New
{
    public partial class GlobalSendEmail : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        String Query;

        String FormName, ParsedID, RecipientID, RecipientName;
        String AttachmentPath = "";
        String EmailPass, decryptedEmailPass;
        String EmailId, DocName , Mode;
        string body = "";

        public GlobalSendEmail(string formname, string parsedid, string recipientid)
        {            
            this.FormName = formname;
            this.ParsedID = parsedid;
            this.RecipientID = recipientid;
            InitializeComponent();
        }

        public void SetMode(string mode)
        {
            this.Mode = mode;
        }

        private void GlobalSendEmail_Load(object sender, EventArgs e)
        {
            ExportPDF();
            Conn = ConnectionString.GetConnection();         
            
            GetEmailTo();
            GetEmailFrom();
            GetEmailPass();
            GetRecipientName();
            GetDefaultBody();
            Conn.Close();
        }

        private void ExportPDF() 
        {
            DocName = ParsedID.Replace(@"/", "-"); //NOTE : FILE PATH DON'T ALLOW SLASH
            AttachmentPath = @"C:\Users\Public\Documents\" + DocName + ".pdf";

            if (FormName == "Sales Agreement")
            {
                ISBS_New.Sales.SalesAgreement.Rpt_SA cryRpt = new ISBS_New.Sales.SalesAgreement.Rpt_SA();
                cryRpt.SetParameterValue("@SalesAgreementNo", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }                
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Goods Receipt")
            {
                ISBS_New.Purchase.GoodsReceipt.Rpt_GR cryRpt = new ISBS_New.Purchase.GoodsReceipt.Rpt_GR();
                cryRpt.SetParameterValue("@GoodsReceivedId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Goods Issued")
            {
                ISBS_New.Sales.BBK.Rpt_GI_SuratJalan cryRpt = new ISBS_New.Sales.BBK.Rpt_GI_SuratJalan();
                cryRpt.SetParameterValue("@GoodsIssuedId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Sales Quotation")
            {
                ISBS_New.Sales.SalesQuotation.Rpt_SQ cryRpt = new ISBS_New.Sales.SalesQuotation.Rpt_SQ();
                cryRpt.SetParameterValue("@SalesQuotationNo", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Delivery Order")
            {
                ISBS_New.Sales.DeliveryOrder.Rpt_DO cryRpt = new ISBS_New.Sales.DeliveryOrder.Rpt_DO();
                cryRpt.SetParameterValue("@DeliveryOrderId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Sales Order")
            {
                ISBS_New.Sales.SalesOrder.Rpt_SO cryRpt = new ISBS_New.Sales.SalesOrder.Rpt_SO();
                cryRpt.SetParameterValue("@SalesOrderNo", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Nota Retur Jual")
            {
                ISBS_New.Sales.NotaReturJual.Rpt_NRJ cryRpt = new ISBS_New.Sales.NotaReturJual.Rpt_NRJ();
                cryRpt.SetParameterValue("@NRJId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Nota Retur Beli")
            {
                ISBS_New.Purchase.NotaReturBeli.Rpt_NRB cryRpt = new ISBS_New.Purchase.NotaReturBeli.Rpt_NRB();
                cryRpt.SetParameterValue("@NRBId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Purchase Agreement")
            {
                if (Mode == "Rinci")
                {
                    ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Rinci cryRpt = new ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Rinci();
                    cryRpt.SetParameterValue("@PAId", ParsedID);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    AttachmentPath = @"C:\Users\Public\Documents\" + DocName + "-Detail.pdf";
                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
                }
                else if (Mode == "Tidak Rinci")
                {
                    ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Ga_Rinci cryRpt = new ISBS_New.Purchase.PurchaseAgreement.Rpt_PA_Ga_Rinci();
                    cryRpt.SetParameterValue("@PAId", ParsedID);

                    CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                    foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                    {
                        myLogin = myTable.LogOnInfo;
                        myLogin.ConnectionInfo.Password = "sql123";
                        myLogin.ConnectionInfo.UserID = "sa";
                        myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                        myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                        myTable.ApplyLogOnInfo(myLogin);
                    }
                    AttachmentPath = @"C:\Users\Public\Documents\" + DocName + "-Simple.pdf";
                    cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
                }
            }
            else if (FormName == "Purchase Order")
            {
                ISBS_New.Purchase.PurchaseOrderNew.POPreview cryRpt = new ISBS_New.Purchase.PurchaseOrderNew.POPreview();
                cryRpt.SetParameterValue("@PurchID", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Receipt Order")
            {
                ISBS_New.Purchase.ReceiptOrder.Rpt_RO cryRpt = new ISBS_New.Purchase.ReceiptOrder.Rpt_RO();
                cryRpt.SetParameterValue("@ReceiptOrderId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "Request For Quotation")
            {
                ISBS_New.Purchase.RFQ.Rpt_RFQ cryRpt = new ISBS_New.Purchase.RFQ.Rpt_RFQ();
                cryRpt.SetParameterValue("@RfqId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else if (FormName == "InvoicePayment")
            {
                ISBS_New.Purchase.GoodsReceipt.Rpt_GR cryRpt = new ISBS_New.Purchase.GoodsReceipt.Rpt_GR();
                cryRpt.SetParameterValue("@GoodsReceivedId", ParsedID);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.30.127";
                    myTable.ApplyLogOnInfo(myLogin);
                }
                cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
            }
            else
                MessageBox.Show("Export belum dibuat");
            lblAttachment.Text = Path.GetFileName(AttachmentPath);
        }        

        private void changeStatus()
        {
            Query = "";
            if (FormName == "Sales Agreement")
            {
                Query = "Update [dbo].[SalesAgreementH] Set EmailId = '" + EmailId + "',[TransStatus] = '05',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where [SalesAgreementNo] = '" + ParsedID + "'";
            }
            else if (FormName == "Goods Receipt")
            {
                Query = "Update [dbo].[GoodsReceivedH] Set EmailId = '" + EmailId + "', [UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where [GoodsReceivedId] = '" + ParsedID + "'";          
            }
            else if (FormName == "Goods Issued")
            {
                Query = "Update [dbo].[GoodsIssuedH] Set EmailId = '" + EmailId + "', [UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where [GoodsIssuedId] = '" + ParsedID + "'";
            }
            else if (FormName == "Sales Quotation")
            {
                Query = "Update [dbo].[SalesQuotationH] Set EmailId = '" + EmailId + "',[TransStatus] = '07',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "'";
                Query += ",SendEmailC = '1',SendEmailCtr = SendEmailCtr + 1 ";
                Query += "Where [SalesQuotationNo] = '" + ParsedID + "'";
            }
            else if (FormName == "Delivery Order")
            {
                Query = "Update [dbo].[DeliveryOrderH] Set EmailId = '" + EmailId + "', [DeliveryOrderStatus] = '05', [UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where [GoodsReceivedId] = '" + ParsedID + "'";
            }
            else if (FormName == "Sales Order")
            {
                Query = "Update [dbo].[SalesOrderH] Set EmailId = '" + EmailId + "',[TransStatus] = '05',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "',";
                Query += ",SendEmailC = '1',SendEmailCtr = SendEmailCtr + 1 ";
                Query += "Where SalesOrderNo = '" + ParsedID + "'";
            }
            else if (FormName == "Nota Retur Jual")
            {
                Query = "Update [dbo].[NotaReturJualH] Set EmailId = '" + EmailId + "',[SendEmailC] = '1',[SendEmailCtr] = [SendEmailCtr] + 1,[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where NRJId = '" + ParsedID + "'";
            }
            else if (FormName == "Nota Retur Beli")
            {
                Query = "Update [dbo].[NotaReturBeliH] Set EmailId = '" + EmailId + "',[SendEmailC] = '1',[SendEmailCtr] = [SendEmailCtr] + 1,[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where NRBId = '" + ParsedID + "'";
            }
            else if (FormName == "Purchase Agreement")
            {
                Query = "Update [dbo].[PurchAgreementH] Set EmailId = '" + EmailId + "',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where [AgreementID] = '" + ParsedID + "'";
            }
            else if (FormName == "Purchase Order")
            {
                Query = "Update [dbo].[PurchH] Set EmailId = '" + EmailId + "',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where PurchID = '" + ParsedID + "'";
            }
            else if (FormName == "Receipt Order")
            {
                Query = "Update [dbo].[ReceiptOrderH] Set EmailId = '" + EmailId + "',[ReceiptOrderStatus] = '09',[UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where ReceiptOrderId = '" + ParsedID + "'";
            }
            else if (FormName == "Request For Quotation")
            {
                Query = "Update [dbo].[RequestForQuotationH] Set EmailId = '" + EmailId + "',[TransStatus] = '02',[SendEmailC] = '1',[SendEmailCtr] = [SendEmailCtr] + 1, [UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.GroupName + "' Where RfqID = '" + ParsedID + "'";
            }
            if (Query != "")
            {
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();
            }
        }

        public void GetDefaultBody()
        {
            txtSubject.Text = FormName;
            body = "Dear " + RecipientName + System.Environment.NewLine + System.Environment.NewLine;
            if (FormName == "Sales Agreement")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your sales agreement of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Sales Order")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your sales agreement of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Goods Receipt")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your sales quotation of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Goods Issued")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your goods issued of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Sales Quotation")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your sales quotation of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Delivery Order")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your delivery order of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Nota Retur Jual")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your NRJ of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Nota Retur Beli")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your NRB of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Purchase Agreement")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your PA of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Purchase Order")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your PO of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Receipt Order")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your Receipt Order of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "Request For Quotation")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your RFQ of the listed items below before the end of the week for our acquisition consideration.";
            }
            else if (FormName == "InvoicePayment")
            {
                body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                body += "attached in this email. We would appreciate your RFQ of the listed items below before the end of the week for our acquisition consideration.";
            }
            else MessageBox.Show("Default Body belum dibuat, silahkan ketik manual");

            body += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
            body += "Yours Sincerely," + System.Environment.NewLine + System.Environment.NewLine;
            body += "________________" + System.Environment.NewLine + System.Environment.NewLine;
            body += ControlMgr.GroupName + System.Environment.NewLine;
            body += "PT INTISUMBER BAJASAKTI" + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
            body += "Cp.People Telp. 021-66675999 Ext. xxxx/xxxx";
            txtBody.Text = body;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMail();
        }

        public void GetEmailTo()
        {
            Query = "SELECT ContactNo FROM Contact where ReffID = '" + RecipientID + "' and ContactType = 'Email' and PrimaryC = 1";
            Cmd = new SqlCommand(Query, Conn);

            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                txtTo.Text = Dr["ContactNo"].ToString();
            }
            txtTo.Text = txtTo.Text + ";";
            Dr.Close();
        }

        public void GetEmailFrom()
        {
            Query = "SELECT Email FROM sysPass WHERE UserID = '" + ControlMgr.UserId + "'";
            Cmd = new SqlCommand(Query, Conn);

            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                txtFrom.Text = Dr["Email"].ToString();
            }
            Dr.Close();
        }

        public void GetEmailPass()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT EmailPass FROM sysPass WHERE UserID = '" + ControlMgr.UserId + "'";
            Cmd = new SqlCommand(Query, Conn);

            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                EmailPass = Dr["EmailPass"].ToString();
            }
            //NOTE : TRY CATCH IF THE EMAIL PASS HAVE NOT BEEN ENCRYPTED BEFORE, THEN USE THE NON-ENCRYPTED ONE
            //DELETE IF ALL EMAIL ALRADY ENCRYPTED
            try
            {
                decryptedEmailPass = AccessRight.StringCipher.Decrypt(EmailPass);
            }
            catch (Exception ex)
            {
                decryptedEmailPass = EmailPass;
            }
            Dr.Close();
        }

        private void btnSelectTo_Click(object sender, EventArgs e)
        {
            var CurrentTo = txtTo.Text;
            var AddTo = "";
            var UpdatedTo = CurrentTo + AddTo;

            string SchemaName = "dbo";
            string TableName = "Contact";
            string Where = "And ReffID = '" + RecipientID + "'";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode != "")
            {
                AddTo = ConnectionString.Kode;
                UpdatedTo = CurrentTo + AddTo + ";";
            }
            txtTo.Text = UpdatedTo.ToString();
        }

        private void btnSelectCC_Click(object sender, EventArgs e)
        {
            var CurrentCC = txtCC.Text;
            var AddCC = "";
            var UpdatedCC = CurrentCC + AddCC;

            string SchemaName = "dbo";
            string TableName = "Contact";
            string Where = "And ReffID = '" + RecipientID + "'";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, Where);
            tmpSearch.ShowDialog();
            if (ConnectionString.Kode != "")
            {
                AddCC = ConnectionString.Kode;
                UpdatedCC = CurrentCC + AddCC + ";";
            }
            txtCC.Text = UpdatedCC.ToString();
        }

        public void SendMail()
        {
            var fromAddress = new MailAddress(txtFrom.Text.Trim());
            var fromPassword = decryptedEmailPass;
            string subject = txtSubject.Text.Trim();
            string body = txtBody.Text.Trim();

            string toAddress = txtTo.Text.Trim();
            string ccAddress = txtCC.Text.Trim();

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };

            MailMessage message = new MailMessage();
            message.From = fromAddress;

            foreach (var address in toAddress.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(address);
            }

            foreach (var address in ccAddress.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.CC.Add(address);
            }

            message.Subject = subject;
            message.Body = body;
            Attachment fileData = new Attachment(AttachmentPath);
            message.Attachments.Add(fileData);

            try
            {
                smtp.Send(message);
                RecordEmail();
                MessageBox.Show("Email sent successfully!");
                this.Close();
            }

            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message); 
                //Note : kalau error "The server response was: 5.5.1 Authentication Required" berarti password salah.
            }
        }

        public void RecordEmail()
        {
            Conn = ConnectionString.GetConnection();
            Trans = Conn.BeginTransaction();

            FileStream objFileStream = new FileStream(AttachmentPath, FileMode.Open, FileAccess.Read);
            int filelength = Convert.ToInt32(objFileStream.Length);
            byte[] data = new byte[filelength];

            objFileStream.Read(data, 0, filelength);
            objFileStream.Close();

            string tempFullName = Path.GetFileName(AttachmentPath);
            string[] tempSplit = tempFullName.Split('.');

            String FileName = tempSplit[0];
            String Extension = tempSplit[tempSplit.Count() - 1];


            Query = "Insert INTO dbo.Email (SendDate, SendFrom, SendTo, SendCC, AttachedFileName, AttachedFileExt, AttachedFile) OUTPUT INSERTED.Id";
            Query += " Values(getDate(),'" + txtFrom.Text + "','" + txtTo.Text + "', '" + txtCC.Text + "', '" + FileName + "', '" + Extension + "', @binaryvalue)";

            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, data.Length).Value = data;

            EmailId = Cmd.ExecuteScalar().ToString();
            changeStatus();
            Trans.Commit();
            Conn.Close();
        }

        private void GetRecipientName()
        {
            Cmd = new SqlCommand("Select ReffTableName FROM [Contact] Where [ReffID] = '" + RecipientID + "'", Conn);
            string contactType = Cmd.ExecuteScalar().ToString();

            if (contactType == "CustTable")
            {
                Cmd = new SqlCommand("Select CustName FROM [CustTable] Where [CustId] = '" + RecipientID + "'", Conn);
                RecipientName = Cmd.ExecuteScalar().ToString();
            }
            else
            {
                Cmd = new SqlCommand("Select VendName FROM [VendTable] Where [VendId] = '" + RecipientID + "'", Conn);
                RecipientName = Cmd.ExecuteScalar().ToString();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTo.Text = "";
            Conn = ConnectionString.GetConnection();
            GetEmailTo();
            Conn.Close();
        }
    }
}
