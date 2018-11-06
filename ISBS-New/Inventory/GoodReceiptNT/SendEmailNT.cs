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

namespace ISBS_New.Inventory.GoodReceiptNT
{
    public partial class SendEmailNT : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String GRId, AttachmentPath = "";
        String Query;

        public SendEmailNT()
        {
            InitializeComponent();
        }

        public void flag(String grid)
        {
            GRId = grid;
        }

        private void SendEmailNT_Load(object sender, EventArgs e)
        {
            modeLoad();
        }

        private void ExportPDF()
        {
            Rpt_GRNT cryRpt = new Rpt_GRNT();
            //cryRpt.Load();
            cryRpt.SetParameterValue("@GoodsReceivedId", GRId);

            CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
            foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in cryRpt.Database.Tables)
            {
                myLogin = myTable.LogOnInfo;
                myLogin.ConnectionInfo.Password = "sql123";
                myLogin.ConnectionInfo.UserID = "sa";
                myLogin.ConnectionInfo.DatabaseName = "ISBSN";
                myLogin.ConnectionInfo.ServerName = "192.168.0.87";
                myTable.ApplyLogOnInfo(myLogin);
            }

            AttachmentPath = @"C:\Users\Public\Documents\" + GRId + ".pdf";

            cryRpt.ExportToDisk(ExportFormatType.PortableDocFormat, AttachmentPath);
        }

        public void SendMail(string from, string to, string subject, string body, string attachmentFilePath, string cc)
        {
            bool boo = false;

            MailMessage message = new MailMessage(from, to, subject, body);

            message.Attachments.Add(new Attachment(attachmentFilePath));
            //message.Attachments.Add(new Attachment("C:\\Users\\WIN003OEMM7\\Desktop\\Lampiran.txt"));

            if (!string.IsNullOrEmpty(txtCC.Text))
            {
                MailAddress copy = new MailAddress(cc);
                message.CC.Add(copy);
            }

            NetworkCredential basicCredential = new NetworkCredential(/*Login Email , password*/"surya@intisumberbajasakti.com", "surya123");

            SmtpClient client = new SmtpClient("surya@intisumberbajasakti.com");

            client.Port = 26;
            client.Credentials = basicCredential;
            client.UseDefaultCredentials = false;
            client.Timeout = (60 * 5 * 1000);
            client.Host = "mail.intisumberbajasakti.com";

            try
            {
                client.Send(message);

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
                Query += " Values(getDate(),'" + Login.Email + "','" + txtTo.Text + "', '" + txtCC.Text + "', '" + FileName + "', '" + Extension + "', @binaryvalue)";

                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, data.Length).Value = data;

                String EmailId = Cmd.ExecuteScalar().ToString();

                Query = "Update [dbo].[GoodsReceivedH] Set EmailId = '" + EmailId + "' Where GoodsReceivedId = '" + GRId + "'";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Cmd.ExecuteNonQuery();

                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Email Terkirim.");
                boo = true;
            }
            catch (Exception x)
            {
                Trans.Rollback();
                MessageBox.Show(x.Message);
            }

            if (boo == true)
            {
                this.Close();
                //File.Delete(AttachmentPath);
            }
        }

        private void modeLoad()
        {
            ExportPDF();

            Conn = ConnectionString.GetConnection();
            //Query = "Select c.Email, v.VendName From RequestForQuotationH h JOIN Contact c ON h.VendId = c.ReffRecId JOIN VendTable v ON h.VendId = v.VendId Where h.RFQId = '" + RFQId + "' And c.PrimaryC = 1";
            //Cmd = new SqlCommand(Query, Conn);
            //Dr = Cmd.ExecuteReader();

            //while (Dr.Read())
            //{
            //    string body = "Dear " + Dr["VendName"].ToString() + System.Environment.NewLine + System.Environment.NewLine;
            //    body += "We understand that your company manufactures some products that fit our business requirements and would like to request a quotation on the following items list";
            //    body += "attached in this email. We would appreciate your sales quotation of the listed items below before the end of the week for our acquisition consideration.";
            //    body += System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
            //    body += "Yours Sincerely," + System.Environment.NewLine + System.Environment.NewLine;
            //    body += "________________" + System.Environment.NewLine + System.Environment.NewLine;
            //    body += "Purchasing Buyer" + System.Environment.NewLine;
            //    body += "PT INTISUMBER BAJASAKTI" + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine;
            //    body += "Cp.People Telp. 021-66675999 Ext. xxxx/xxxx";

            //    //txtTo.Text = "surya@intisumberbajasakti.com";
            //    txtTo.Text = Dr["Email"].ToString();
            //    txtSubject.Text = "Request for quotation letter";
            //    txtBody.Text = body;
            //}


            lblAttachment.Text = Path.GetFileName(AttachmentPath);
            Conn.Close();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendMail(Login.Email, txtTo.Text, txtSubject.Text, txtBody.Text, AttachmentPath, txtCC.Text);
        }
    }
}
