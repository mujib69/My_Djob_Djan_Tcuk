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
using System.Transactions;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace ISBS_New.ARCollection
{
    public partial class SendEmail : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        TransactionScope Scope;

        String CLNo, CustId, Mode;
        String ARNo;
        String AttachmentPath;
        String EmailPass, decryptedEmailPass;
        String Query;
        //int TotalSendEmailCtr;
        //Collection.CollectionHeader _owner;
        Collection.CollectionTaskList _owner;

        //public SendEmail(Collection.CollectionHeader owner)
        //{
        //    InitializeComponent();
        //    _owner = owner;
        //    this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SendEmail_FormClosing);
        //}

        //set parent
        Collection.CollectionTaskList Parent;
        public void setParent(Collection.CollectionTaskList PassedForm)
        {
            Parent = PassedForm;
        }

        private void SendEmail_FormClosing(object sender, FormClosingEventArgs e)
        {
            _owner.RefreshAccess();
        }

        public SendEmail()
        {
            InitializeComponent();
        }

        public void flag(String clno,String custid, String mode)
        {
            CLNo = clno;
            CustId = custid;
            Mode = mode;
        }

        public void getARNo(String arno)
        {
            ARNo = arno;
        }

        private void SendEmail_Load(object sender, EventArgs e)
        {
            modeLoad();
        }

        private void modeLoad()
        {
            ExportPDF();
            Conn = ConnectionString.GetConnection();
            lblAttachment.Text = Path.GetFileName(AttachmentPath);
            GetEmailTo();
            GetEmailFrom();
            GetEmailPass();
            GetDefaultBody();
            Conn.Close();
        }

        private void ExportPDF()
        {
            Rpt_CL cryRpt = new Rpt_CL();
            //cryRpt.Load();
            cryRpt.SetParameterValue("@CollectionNo", CLNo);

            CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
            foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
            {
                myLogin = myTable.LogOnInfo;
                myLogin.ConnectionInfo.Password = "sql123";
                myLogin.ConnectionInfo.UserID = "sa";
                myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                myLogin.ConnectionInfo.ServerName = "192.168.0.87";
                myTable.ApplyLogOnInfo(myLogin);
            }

            //NOTE : FILE PATH DIDN'T ALLOW SLASH
            String CLNonoSlash = CLNo.Replace(@"/", "-");
            AttachmentPath = @"C:\Users\Public\Documents\" + CLNonoSlash + ".pdf";
            AttachmentPath = AttachmentPath.Replace(@"\\", @"\");

            cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
        }

        public void SendMail()
        {
            var fromAddress = new MailAddress(txtFrom.Text.Trim());
            var fromPassword = decryptedEmailPass;
            string subject = txtSubject.Text.Trim();
            string body = txtBody.Text.Trim();

            string toAddress = txtTo.Text.Trim();
            string ccAddress = txtCC.Text.Trim();

            try
            {
                using (Scope = new TransactionScope())
                {
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
                        try
                        {
                            message.To.Add(address);
                        }
                        catch
                        {
                            MessageBox.Show("Format tujuan email tidak sesuai untuk pengiriman email.");
                            return;
                        }
                    }

                    foreach (var address in ccAddress.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            message.CC.Add(address);
                        }
                        catch
                        {
                            MessageBox.Show("Format CC tidak sesuai untuk pengiriman email.");
                            return;
                        }
                    }

                    message.Subject = subject;
                    message.Body = body;
                    Attachment fileData = new Attachment(AttachmentPath);
                    message.Attachments.Add(fileData);

                    smtp.Send(message);
                    if (Mode == "New")
                    {
                        saveNewCollection();
                    }
                    RecordEmail();

                    InsertCLRLog();

                    MessageBox.Show("Email Sent!");
                    Scope.Complete();
                }
            }

            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
            finally { }
            Parent.RefreshAccess();
            this.Close();
        }

        public void RecordEmail()
        {
            Conn = ConnectionString.GetConnection();

            FileStream objFileStream = new FileStream(AttachmentPath, FileMode.Open, FileAccess.Read);
            int filelength = Convert.ToInt32(objFileStream.Length);
            byte[] data = new byte[filelength];

            objFileStream.Read(data, 0, filelength);
            objFileStream.Close();

            string tempFullName = Path.GetFileName(AttachmentPath);
            string[] tempSplit = tempFullName.Split('.');

            String FileName = tempSplit[0];
            String Extension = tempSplit[tempSplit.Count() - 1];

            //QUERY
            Query = "Insert INTO dbo.Email (SendDate, SendFrom, SendTo, SendCC, AttachedFileName, AttachedFileExt, AttachedFile) OUTPUT INSERTED.Id";
            Query += " Values(getDate(),'" + txtFrom.Text + "','" + txtTo.Text + "', '" + txtCC.Text + "', '" + FileName + "', '" + Extension + "', @binaryvalue)";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, data.Length).Value = data;

            String EmailId = Cmd.ExecuteScalar().ToString();

            //CHANGE THIS TO RESPECTIVE EMAIL
            Query = "Update [dbo].[Collection_H] Set Email_Id = '" + EmailId + "',[Status_Code] = '03',[Email_C] = '1',[Email_Ctr] = [Email_Ctr] + 1, [UpdatedDate] = getdate(),[UpdatedBy] = '" + ControlMgr.UserId + "' Where CL_No = '" + CLNo + "'";
            Cmd = new SqlCommand(Query, Conn);
            Cmd.ExecuteNonQuery();

            Conn.Close();
        }

        public void GetDefaultBody()
        {
            Query = "Select c.ContactNo, ct.CustName From [Collection_Dtl] h ";
            Query += "JOIN Contact c ON h.CustId = c.ReffId ";
            Query += "JOIN CustTable ct ON h.CustId = ct.CustId ";
            Query += "Where h.CL_No = '" + CLNo + "' And c.PrimaryC = 1";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                string body = "Dear " + Dr["CustName"].ToString() + System.Environment.NewLine + System.Environment.NewLine;
                //body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
                //body += "attached in this email. We would appreciate your sales quotation of the listed items below before the end of the week for our acquisition consideration.";
                //body += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
                //body += "Yours Sincerely," + System.Environment.NewLine + System.Environment.NewLine;
                //body += "________________" + System.Environment.NewLine + System.Environment.NewLine;
                //body += "Purchasing Buyer" + System.Environment.NewLine;
                body += "PT INTISUMBER BAJASAKTI" + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
                body += "Cp.People Telp. 021-66675999 Ext. xxxx/xxxx";
                txtSubject.Text = "Request for quotation letter";
                txtBody.Text = body;
            }
        }

        public void GetEmailTo()
        {
            Conn = ConnectionString.GetConnection();
            Query = "SELECT Contact.ContactNo , [Collection_Dtl].[CL_No] FROM Contact ";
            Query += "INNER JOIN [Collection_Dtl] ON [Collection_Dtl].CustId = Contact.ReffId ";
            Query += "Where [CL_No] = '" + CLNo + "' AND PrimaryC = 1";
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
            Conn = ConnectionString.GetConnection();
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
            //DELETE IF ALL EMAIL START ENCRYPTED
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
            string Where = "And ReffID = '" + CustId + "'";

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
            string Where = "And ReffID = '" + CustId + "'";

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

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMail();
        }

        private void InsertCLRLog()
        {
            string action = "";
            bool print = false;
            DateTime cldate = new DateTime();
            Query = "SELECT TOP 1 CL_Date,Type,[Action],[Email],[Print] FROM [dbo].[Collection_LogTable] WHERE [CL_No]=@CL_No ORDER BY LogDate DESC";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No", CLNo);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    while (Dr.Read())
                    {
                        action = Dr["Action"].ToString();
                        print = Convert.ToBoolean(Dr["Print"]);
                        cldate = Convert.ToDateTime(Dr["CL_Date"]);
                    }
                }
                else
                {
                    action = "N";
                    print = false;
                    cldate = DateTime.Now;
                }
                Dr.Close();
            }

            Query = "INSERT INTO [dbo].[Collection_LogTable] ([CL_No],[CL_Date],[Type],[Email],[Print],[Deskripsi],[Status],[StatusDescription],[Action],[LogDate],[UserID]) VALUES (";
            Query += " @CL_No,getdate(),@Type,@Email,@Print,@Deskripsi,@Status,@StatusDescription,@Action,getdate(),@UserID)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No", CLNo);
                Cmd.Parameters.AddWithValue("@Type", "Email");
                Cmd.Parameters.AddWithValue("@Email", true);
                Cmd.Parameters.AddWithValue("@Print", print);
                Cmd.Parameters.AddWithValue("@Deskripsi", "Email Sent");
                Cmd.Parameters.AddWithValue("@Status", "03");
                Cmd.Parameters.AddWithValue("@StatusDescription", "Completed");
                Cmd.Parameters.AddWithValue("@Action", action);
                Cmd.Parameters.AddWithValue("@UserID", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveNewCollection()
        {
            //getValue
            string InvoiceType="";
            string TandaTerima = "";
            Query = "SELECT * FROM [dbo].[CustInvoice_H] WHERE [Invoice_Id]=@Invoice_Id";
            using(Cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@Invoice_Id",ARNo);
                Dr = Cmd.ExecuteReader();
                while(Dr.Read())
                {
                    InvoiceType = Dr["Invoice_Type"].ToString();
                    TandaTerima = Dr["Kwitansi_No"].ToString();
                }
                Dr.Close();
            }

            string CustName = "";
            Query = "SELECT * FROM [CustTable] WHERE [CustId]=@CustId";
            using(Cmd = new SqlCommand(Query,ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CustId",CustId);
                Dr = Cmd.ExecuteReader();
                while(Dr.Read())
                {
                    CustName = Dr["CustName"].ToString();
                }
                Dr.Close();
            }

            //insert to collectionH
            Query = "INSERT INTO [dbo].[Collection_H] ([CL_Date],[CL_No],[CL_Type],[Collector],[Email_Id],[Email_C],[Email_Ctr],[Mail_Address],[Print_C],[Print_Ctr],[Status_Code],[CreatedDate],[CreatedBy])";
            Query += " VALUES (getdate(),@CL_No,@CL_Type,@Collector,@Email_Id,@Email_C,@Email_Ctr,@Mail_Address,@Print_C,@Print_Ctr,@Status_Code,getdate(),@CreatedBy) ";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No",CLNo);
                Cmd.Parameters.AddWithValue("@CL_Type","Email");
                Cmd.Parameters.AddWithValue("@Collector",txtFrom.Text);
                Cmd.Parameters.AddWithValue("@Email_Id","0");
                Cmd.Parameters.AddWithValue("@Email_C","0");
                Cmd.Parameters.AddWithValue("@Email_Ctr","0");
                Cmd.Parameters.AddWithValue("@Mail_Address",txtTo.Text);
                Cmd.Parameters.AddWithValue("@Print_C","0");
                Cmd.Parameters.AddWithValue("@Print_Ctr","0");
                Cmd.Parameters.AddWithValue("@Status_Code","01");
                Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }

            //insert to collectionDtl
            Query = " INSERT INTO [dbo].[Collection_Dtl] ([CL_No],[SeqNo],[CustId],[Cust_Name],[Invoice_Type],[Invoice_Id],[TT_No],[Comments],[CreatedDate],[CreatedBy]) ";
            Query += " VALUES (@CL_No,@SeqNo,@CustId,@Cust_Name,@Invoice_Type,@Invoice_Id,@TT_No,@Comments,getdate(),@CreatedBy) ;";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@CL_No",CLNo);
                Cmd.Parameters.AddWithValue("@SeqNo",1);
                Cmd.Parameters.AddWithValue("@CustId",CustId);
                Cmd.Parameters.AddWithValue("@Cust_Name",CustName);
                Cmd.Parameters.AddWithValue("@Invoice_Type",InvoiceType);
                Cmd.Parameters.AddWithValue("@Invoice_Id",ARNo);
                Cmd.Parameters.AddWithValue("@TT_No", TandaTerima);
                Cmd.Parameters.AddWithValue("@Comments","");
                Cmd.Parameters.AddWithValue("@CreatedBy",ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }
     
    }
}
