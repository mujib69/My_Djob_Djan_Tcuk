//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data.SqlClient;
//using System.Drawing;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;

namespace ISBS_New
{
    class ListMethod
    {
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private string Query;

        public static void StatusLogCustomer(string Form, string Transcode, string CustomerId, string Status, string StatusDesc, string PK1, string PK2, string PK3, string PK4)
        {
            string StatusDescription = "";
            string Query = "";
            SqlCommand Cmd;
            if (StatusDesc == "")
            {
                Query = "SELECT [Deskripsi] FROM [TransStatusTable] WHERE [TransCode] = @TransCode AND [StatusCode]=@StatusCode";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@TransCode", Transcode);
                    Cmd.Parameters.AddWithValue("@StatusCode", Status);
                    if (Cmd.ExecuteScalar() != System.DBNull.Value)
                    {
                        StatusDescription = Cmd.ExecuteScalar().ToString();
                    }
                }
            }
            else
            {
                StatusDescription = StatusDesc;
            }

            Query = "INSERT INTO [StatusLog_Customer] ([StatusLog_FormName],[Customer_Id],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]) VALUES (@StatusLog_FormName,@Vendor_Id,@StatusLog_PK1,@StatusLog_PK2,@StatusLog_PK3,@StatusLog_PK4,@StatusLog_Status,@StatusLog_Description,@StatusLog_UserID,getdate())";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@StatusLog_FormName", Form);
                Cmd.Parameters.AddWithValue("@Vendor_Id", CustomerId);
                Cmd.Parameters.AddWithValue("@StatusLog_PK1", PK1);
                Cmd.Parameters.AddWithValue("@StatusLog_PK2", PK2);
                Cmd.Parameters.AddWithValue("@StatusLog_PK3", PK3);
                Cmd.Parameters.AddWithValue("@StatusLog_PK4", PK4);
                Cmd.Parameters.AddWithValue("@StatusLog_Status", Status);
                Cmd.Parameters.AddWithValue("@StatusLog_Description", StatusDescription);
                Cmd.Parameters.AddWithValue("@StatusLog_UserID", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        public static void StatusLogVendor(string Form, string Transcode, string VendorId, string Status, string StatusDesc, string PK1, string PK2, string PK3, string PK4)
        {
            string StatusDescription = "";
            string Query = "";
            SqlCommand Cmd;
            if (StatusDesc == "")
            {
                Query = "SELECT [Deskripsi] FROM [TransStatusTable] WHERE [TransCode] = @TransCode AND [StatusCode]=@StatusCode";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    Cmd.Parameters.AddWithValue("@TransCode", Transcode);
                    Cmd.Parameters.AddWithValue("@StatusCode", Status);
                    if (Cmd.ExecuteScalar() != System.DBNull.Value)
                    {
                        StatusDescription = Cmd.ExecuteScalar().ToString();
                    }
                }
            }
            else
            {
                StatusDescription = StatusDesc;
            }

            Query = "INSERT INTO [StatusLog_Vendor] ([StatusLog_FormName],[Vendor_Id],[StatusLog_PK1],[StatusLog_PK2],[StatusLog_PK3],[StatusLog_PK4],[StatusLog_Status],[StatusLog_Description],[StatusLog_UserID],[StatusLog_Date]) VALUES (@StatusLog_FormName,@Vendor_Id,@StatusLog_PK1,@StatusLog_PK2,@StatusLog_PK3,@StatusLog_PK4,@StatusLog_Status,@StatusLog_Description,@StatusLog_UserID,getdate())";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@StatusLog_FormName", Form);
                Cmd.Parameters.AddWithValue("@Vendor_Id", VendorId);
                Cmd.Parameters.AddWithValue("@StatusLog_PK1", PK1);
                Cmd.Parameters.AddWithValue("@StatusLog_PK2", PK2);
                Cmd.Parameters.AddWithValue("@StatusLog_PK3", PK3);
                Cmd.Parameters.AddWithValue("@StatusLog_PK4", PK4);
                Cmd.Parameters.AddWithValue("@StatusLog_Status", Status);
                Cmd.Parameters.AddWithValue("@StatusLog_Description", StatusDescription);
                Cmd.Parameters.AddWithValue("@StatusLog_UserID", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        public static void restrictedPreviewEmail(string Action, string FormName, string DatabaseName, string PKname, string PKid, string CustomerId)
        {
            if (!checkPreviewEmailC(Action, DatabaseName, PKname, PKid))
                return;

            if (Action == "Preview")
            {
                GlobalPreview f = new GlobalPreview(FormName, PKid);
                f.Show();

                updatePreviewEmailC(Action, DatabaseName, PKname, PKid, "Disable");
            }
            if (Action == "Email")
            {
                GlobalSendEmail f = new GlobalSendEmail(FormName, PKid, CustomerId);
                f.Show();

                //Note : SendEmailC diatur saat di form GlobalSendEmail
            }
        }

        public static Boolean checkPreviewEmailC(string Action, string DatabaseName, string PKname, string PKid)
        {
            SqlCommand Cmd;
            bool vBol = false;

            if (Action == "Preview")
            {
                string Query = "Select PreviewC From " + DatabaseName + " Where " + PKname + " = '" + PKid + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    bool PreviewC = Convert.ToBoolean(Cmd.ExecuteScalar());
                    if (PreviewC == false) //belum di preview
                        vBol = true;
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult dialogBox = MessageBox.Show("Document already previewed!" + Environment.NewLine + "Allow document to be previewed again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dialogBox == DialogResult.Yes)
                            {
                                updatePreviewEmailC(Action, DatabaseName, PKname, PKid, "Enable");
                                vBol = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Document already previewed!" + Environment.NewLine + "Contact Administrator");
                            vBol = false;
                        }
                    }
                }
            }
            if (Action == "Email")
            {
                string Query = "Select SendEmailC From " + DatabaseName + " Where " + PKname + " = '" + PKid + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                {
                    bool SendEmailC = Convert.ToBoolean(Cmd.ExecuteScalar());
                    if (SendEmailC == false) //belum send email
                        vBol = true;
                    else
                    {
                        if (ControlMgr.GroupName == "Administrator")
                        {
                            DialogResult dialogBox = MessageBox.Show("Document already emailed!" + Environment.NewLine + "Allow document to be emailed again?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dialogBox == DialogResult.Yes)
                            {
                                updatePreviewEmailC(Action, DatabaseName, PKname, PKid, "Enable");
                                vBol = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Document already emailed!" + Environment.NewLine + "Contact Administrator");
                            vBol = false;
                        }
                    }
                }
            }
            return vBol;
        }

        public static void updatePreviewEmailC(string Action, string DatabaseName, string PKname, string PKid, string Mode)
        {
            SqlCommand Cmd;
            if (Action == "Preview")
            {
                if (Mode == "Disable")
                {
                    string Query = "Update " + DatabaseName + " Set PreviewC = '1' , PreviewCtr = PreviewCtr + 1 Where " + PKname + " = '" + PKid + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                }
                if (Mode == "Enable")
                {
                    string Query = "Update " + DatabaseName + " Set PreviewC = '0'  Where " + PKname + " = '" + PKid + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                    MessageBox.Show("Document can be previewed again!");
                }
            }
            if (Action == "Email")
            {
                if (Mode == "Disable")
                {
                    string Query = "Update " + DatabaseName + " Set SendEmailC = '1' , SendEmailCtr = SendEmailCtr + 1 Where " + PKname + " = '" + PKid + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                }
                if (Mode == "Enable")
                {
                    string Query = "Update " + DatabaseName + " Set SendEmailC = '0'  Where " + PKname + " = '" + PKid + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();
                    MessageBox.Show("Document can be emailed again!");
                }
            }
        }

        public static Boolean checkCreditLimit(string Action, string CustomerID, decimal TotalNett)
        {
            SqlCommand Cmd;
            string Query;
            SqlDataReader Dr;

            bool vBol = true;

            Query = "SELECT [Sisa_Limit_Total],[Limit_Temp] FROM [dbo].[CustTable] WHERE [CustId] = '" + CustomerID + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    decimal LimitMax = Convert.ToDecimal(Dr["Sisa_Limit_Total"]) + Convert.ToDecimal(Dr["Limit_Temp"]);

                    if (LimitMax < TotalNett)
                    {
                        if (Action == "Ask")
                        {
                            DialogResult dialogResult = MessageBox.Show("Credit untuk cust ini sudah melebihi batas Credit Limit, apakah tetap ingin melanjutkan transaksi? ", "Update Status Confirmation !", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                                vBol = true;
                            else
                                vBol = false;

                        }
                        if (Action == "Stop")
                        {
                            MessageBox.Show("Credit untuk cust ini sudah melebihi batas Credit Limit");
                            vBol = false;
                        }
                    }
                }
                Dr.Close();
            }
            return vBol;
        }

        public static Boolean checkCustomerMoU(string MouNo, string CustomerID, decimal TotalNett)
        {
            SqlCommand Cmd;
            string Query;
            SqlDataReader Dr;
            bool vBol = true;

            Query = "SELECT [Remaining_Amount] FROM [CustMou_Dtl] WHERE [MouNo] = '" + MouNo + "' AND [CustID] = '" + CustomerID + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    decimal RemainingMou = Convert.ToDecimal(Dr["Remaining_Amount"]);

                    if (TotalNett > RemainingMou)
                    {
                        MessageBox.Show("Credit untuk cust ini sudah melebihi batas MOU");
                        vBol = false;
                    }
                }
            }
            return vBol;
        }

        public static void updateInventUomAlt(string Action, string DatabaseName, string FullItemId, string UomName, string AltName, decimal UomQty, decimal AltQty)
        {
            SqlCommand Cmd;
            string Query = "";

            if (Action == "Increase")
                Query = "UPDATE " + DatabaseName + " SET " + UomName + " = " + UomName + " + '" + UomQty + "', " + AltName + " = " + AltName + " + '" + AltQty + "' WHERE [FullItemId] = '" + FullItemId + "'";
            if (Action == "Decrease")
                Query = "UPDATE " + DatabaseName + " SET " + UomName + " = " + UomName + " - '" + UomQty + "', " + AltName + " = " + AltName + " - '" + AltQty + "' WHERE [FullItemId] = '" + FullItemId + "'";

            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                Cmd.ExecuteNonQuery();
        }

        public static void updateInventAmount(string Action, string DatabaseName, string FullItemId, string AmountName, decimal AmountQty)
        {
            SqlCommand Cmd;
            string Query = "";

            if (Action == "Increase")
                Query = "UPDATE " + DatabaseName + " SET " + AmountName + " = " + AmountName + " + '" + AmountQty + "' WHERE [FullItemId] = '" + FullItemId + "'";
            if (Action == "Decrease")
                Query = "UPDATE " + DatabaseName + " SET " + AmountName + " = " + AmountName + " - '" + AmountQty + "' WHERE [FullItemId] = '" + FullItemId + "'";

            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                Cmd.ExecuteNonQuery();
        }

        public static void revertInventUomAlt(string Action, string DatabaseName, string UomName, string AltName, string PKdatabase, string PKname, string PKid, string PKuomName, string PKaltName)
        {
            SqlCommand Cmd;
            SqlDataReader Dr;
            string Query = "";
            int totalDetail = 0;
            decimal OldUomQty = 0;
            decimal OldAltQty = 0;
            string FullItemId = "";

            Query = "SELECT COUNT(*) FROM " + PKdatabase + " WHERE " + PKname + " = '" + PKid + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                totalDetail = Convert.ToInt32(Cmd.ExecuteScalar());

            for (int i = 1; i < totalDetail + 1; i++)
            {
                Query = "SELECT FullItemId, " + PKuomName + ", " + PKaltName + " FROM " + PKdatabase + " WHERE " + PKname + " = '" + PKid + "' And [SeqNo] = '" + i + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    FullItemId = Dr["FullItemId"].ToString();
                    OldUomQty = Convert.ToDecimal(Dr[PKuomName].ToString());
                    OldAltQty = Convert.ToDecimal(Dr[PKaltName].ToString());
                }
                Dr.Close();

                if (Action == "Increase")
                    Query = "UPDATE " + DatabaseName + " SET " + UomName + " = " + UomName + " + '" + OldUomQty + "', " + AltName + " = " + AltName + " + '" + OldAltQty + "' WHERE [FullItemId] = '" + FullItemId + "'";
                if (Action == "Decrease")
                    Query = "UPDATE " + DatabaseName + " SET " + UomName + " = " + UomName + " - '" + OldUomQty + "', " + AltName + " = " + AltName + " - '" + OldAltQty + "' WHERE [FullItemId] = '" + FullItemId + "'";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    Cmd.ExecuteNonQuery();
            }
        }

        public static void insertLogTable(string Action, string DatabaseName, string TransaksiId, string Deskripsi, string TransName, string TransStatus)
        {
            SqlCommand Cmd;
            string Query = "";
            string TransDeskripsi = "";

            Query = "SELECT [Deskripsi] FROM [TransStatusTable] WHERE [TransCode] = @transname AND [StatusCode] = @transstatus";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@transname", TransName);
                Cmd.Parameters.AddWithValue("@transstatus", TransStatus);
                TransDeskripsi = Cmd.ExecuteScalar().ToString();
            }

            Query = "INSERT INTO " + DatabaseName + " VALUES (@transid, @deskripsi, @transdeskripsi,@action,@userid,getdate())";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@transid", TransaksiId);
                Cmd.Parameters.AddWithValue("@deskripsi", Deskripsi);
                Cmd.Parameters.AddWithValue("@transdeskripsi", TransDeskripsi);
                Cmd.Parameters.AddWithValue("@action", Action);
                Cmd.Parameters.AddWithValue("@userid", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }

        public static void insertLogTableFollowParentAct(string DatabaseName, string TransaksiId, string Deskripsi, string TransName, string TransStatus)
        {
            SqlCommand Cmd;
            string Query = "";
            string TransDeskripsi = "";
            string lastAction = "";

            Query = "SELECT TOP 1 [Action] FROM " + DatabaseName + " WHERE [TransaksiID] = '" + TransaksiId + "' ORDER BY LogDatetime DESC";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                lastAction = Cmd.ExecuteScalar().ToString();

            Query = "SELECT [Deskripsi] FROM [TransStatusTable] WHERE [TransCode] = @transname AND [StatusCode] = @transstatus";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@transname", TransName);
                Cmd.Parameters.AddWithValue("@transstatus", TransStatus);
                TransDeskripsi = Cmd.ExecuteScalar().ToString();
            }

            Query = "INSERT INTO " + DatabaseName + " VALUES (@transid, @deskripsi, @transdeskripsi,@action,@userid,getdate())";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@transid", TransaksiId);
                Cmd.Parameters.AddWithValue("@deskripsi", Deskripsi);
                Cmd.Parameters.AddWithValue("@transdeskripsi", TransDeskripsi);
                Cmd.Parameters.AddWithValue("@action", lastAction);
                Cmd.Parameters.AddWithValue("@userid", ControlMgr.UserId);
                Cmd.ExecuteNonQuery();
            }
        }
    }
}
