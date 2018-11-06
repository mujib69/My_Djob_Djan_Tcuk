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
    class TaskListRequestCancelDO
    {
        private SqlCommand Cmd;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private string Query;

        private static Boolean validasi(string DONo)
        {
            SqlCommand Cmd;
            SqlDataReader Dr;
            string Query;
            bool vBol = false;

            Query = "SELECT SUM(Qty) AS TotalQty, SUM(RemainingQty) AS TotalRemaining FROM DeliveryOrderD WHERE DeliveryOrderId = '" + DONo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    decimal TotalQty = Convert.ToDecimal(Dr["TotalQty"].ToString());
                    decimal TotalRemaining = Convert.ToDecimal(Dr["TotalRemaining"].ToString());

                    if (TotalQty != TotalRemaining)
                    {
                        MessageBox.Show("Tidak bisa cancel" + Environment.NewLine + "Qty tidak sama dengan RemainingQty!");
                        return false;
                    }
                    else
                        vBol = true;
                }
            }
            return vBol;
        }

        public static void actionRequestCancelDO(string Action, string DONo)
        {
            SqlCommand Cmd;
            SqlDataReader Dr;
            string Query;

            if (!validasi(DONo))
                return;

            //check sudah dibuat SI atau belum
            string checkSI;
            Query = "SELECT COUNT ([SI_Number]) FROM [DeliveryOrderH] WHERE [DeliveryOrderId] = '" + DONo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                checkSI = Cmd.ExecuteScalar().ToString();

            if (checkSI == "0") //kalau belum ada SI
            {
                if (Action == "Approve")
                {
                    /*jika SM, Yes:
                     * maka status do cancelled. - done
                     * update remaining SO, - done, need check 
                     * create jurnal IN59, 
                     * muncul task list sales admin (inqury DO, status cancelled) - done*/

                    Query = "UPDATE [DeliveryOrderH] SET [DeliveryOrderStatus] = '13',";
                    Query += "[ApproveCancelSMDate] = getdate(),";
                    Query += "[ApproveCancelSMBy] = '" + Login.Username + "' ";
                    Query += "WHERE DeliveryOrderId = '" + DONo + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();

                    updateRemainingSO(DONo);

                    MessageBox.Show(DONo + " cancelled!");
                }
                if (Action == "Reject")
                {
                    /*jika SM. no: 
                     * maka do tidak jadi di cancel, - done
                     * jika DO franco belum tarik SI maka status do update jadi Draft, - done
                     * Jika DO Loco maka status Do update jadi created - done*/

                    string delivMethod;
                    Query = "SELECT [DeliveryMethod] FROM [DeliveryOrderH] WHERE [DeliveryOrderId] = '" + DONo + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        delivMethod = Cmd.ExecuteScalar().ToString();

                    if (delivMethod == "FRANCO")
                    {
                        Query = "UPDATE [DeliveryOrderH] SET [DeliveryOrderStatus] = '03' ";
                        Query += "WHERE DeliveryOrderId = '" + DONo + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            Cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Query = "UPDATE [DeliveryOrderH] SET [DeliveryOrderStatus] = '01' ";
                        Query += "WHERE DeliveryOrderId = '" + DONo + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            Cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show(DONo + " request cancel - rejected!");
                }
            }
            else //kalau sudah tarik SI *******************************************
            {
                if (Action == "Approve")
                {
                    if (Login.UserGroup == "Sales Manager")
                    {
                        /*jika SM, Yes
                         * maka muncul task list di exp manager - done
                         * status do update jadi Cancel – Waiting for Approval EXP - done*/

                        Query = "UPDATE [DeliveryOrderH] SET [DeliveryOrderStatus] = '12',";
                        Query += "[ApproveCancelSMDate] = getdate(),";
                        Query += "[ApproveCancelSMBy] = '" + Login.Username + "' ";
                        Query += "WHERE DeliveryOrderId = '" + DONo + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            Cmd.ExecuteNonQuery();

                        MessageBox.Show(DONo + " cancelled - waiting for approval Exp!");
                    }
                    if (Login.UserGroup == "Expedisi Manager")
                    {
                        /*jika exp man, yes:
                         * maka status do cancelled. - done
                         * update remaining SO, - done, need check 
                         * create jurnal IN59, 
                         * muncul task list sales admin (inqury DO, status cancelled) - done*/

                        Query = "UPDATE [DeliveryOrderH] SET [DeliveryOrderStatus] = '13',";
                        Query += "[ApproveCancelEXPDate] = getdate(),";
                        Query += "[ApproveCancelEXPBy] = '" + Login.Username + "' ";
                        Query += "WHERE DeliveryOrderId = '" + DONo + "'";
                        using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                            Cmd.ExecuteNonQuery();

                        updateRemainingSO(DONo);

                        MessageBox.Show(DONo + " request cancel - approved!");
                    }
                }

                if (Action == "Reject")
                {
                    /*maka do tidak jadi di cancel, status do update jadi created - done*/

                    Query = "UPDATE [DeliveryOrderH] SET [DeliveryOrderStatus] = '01' ";
                    Query += "WHERE DeliveryOrderId = '" + DONo + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();

                    MessageBox.Show(DONo + " request cancel - rejected!");
                }
            }
        }

        private static void updateRemainingSO(string DONo)
        {
            SqlCommand Cmd;
            SqlDataReader Dr;
            string Query;

            //buat return qty ke sales order
            string SONo;
            Query = "SELECT [SalesOrderId] FROM [DeliveryOrderH] WHERE [DeliveryOrderId] = '" + DONo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                SONo = Cmd.ExecuteScalar().ToString();

            int totalDetail;
            Query = "SELECT COUNT([DeliveryOrderId]) FROM [DeliveryOrderD] WHERE [DeliveryOrderId] = '" + DONo + "'";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                totalDetail = Convert.ToInt32(Cmd.ExecuteScalar().ToString());

            string ID = ID = ConnectionString.GenerateSeqID(7, "JN", "JN", ConnectionString.GetConnection(), Cmd);
            insertJournalH(ID, DONo);

            for (int i = 1; i < totalDetail + 1; i++)
            {
                //Query = "SELECT * FROM [DeliveryOrderD] WHERE [DeliveryOrderId] = '" + DONo + "' ";
                Query = "SELECT * FROM [DeliveryOrderD] a ";
                Query += "LEFT JOIN [InventTable] b ON a.FullItemId = b.FullItemId ";
                Query += "WHERE [DeliveryOrderId] = '" + DONo + "' ";
                using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                    Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    decimal DORemainingQty = Convert.ToDecimal(Dr["RemainingQty"].ToString());
                    string SOSeqNo = Dr["SalesOrderSeqNo"].ToString();

                    Query = "UPDATE [SalesOrderD] SET [RemainingQty] = [RemainingQty] + '" + DORemainingQty + "' ";
                    Query += "WHERE [SalesOrderNo] = '" + SONo + "' AND [SeqNo] = '" + SOSeqNo + "'";
                    using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
                        Cmd.ExecuteNonQuery();

                    decimal ConvertionRatio = Convert.ToDecimal(Dr["ConvertionRatio"].ToString());
                    decimal DOQtyAvailableUoM = Convert.ToDecimal(Dr["Qty_Available"].ToString());
                    decimal DOQtyAvailableAlt = DOQtyAvailableUoM * ConvertionRatio;
                    decimal DOQtyReservedUoM = Convert.ToDecimal(Dr["Qty_Reserved"].ToString());
                    decimal DOQtyReservedAlt = DOQtyAvailableAlt * ConvertionRatio;
                    decimal DOQtyUoM = Convert.ToDecimal(Dr["Qty"].ToString());
                    decimal DOQtyAlt = Convert.ToDecimal(Dr["Qty_Alt"].ToString());
                    string FullItemId = Dr["FullItemID"].ToString();

                    decimal UoM_AvgPrice = Convert.ToDecimal(Dr["UoM_AvgPrice"].ToString());
                    if (UoM_AvgPrice == 0)
                        UoM_AvgPrice = 1;
                    decimal DOQtyAmount = DOQtyUoM * UoM_AvgPrice;
                    decimal DOQtyAvailableAmount = DOQtyAvailableUoM * UoM_AvgPrice;
                    decimal DOQtyReservedAmount = DOQtyReservedUoM * UoM_AvgPrice;

                    //Update inventOnHand
                    ListMethod.updateInventUomAlt("Increase", "Invent_OnHand_Qty", FullItemId, "Available_For_Sale_UoM", "Available_For_Sale_Alt", DOQtyAvailableUoM, DOQtyAvailableAlt);
                    ListMethod.updateInventAmount("Increase", "Invent_OnHand_Qty", FullItemId, "Available_For_Sale_Amount", DOQtyAvailableAmount);

                    ListMethod.updateInventUomAlt("Increase", "Invent_OnHand_Qty", FullItemId, "Available_For_Sale_Reserved_UoM", "Available_For_Sale_Reserved_Alt", DOQtyReservedUoM, DOQtyReservedAlt);
                    ListMethod.updateInventAmount("Increase", "Invent_OnHand_Qty", FullItemId, "Available_For_Sale_Reserved_Amount", DOQtyReservedAmount);

                    //update inventSales
                    ListMethod.updateInventUomAlt("Increase", "Invent_Sales_Qty", FullItemId, "SO_Confirmed_Outstanding_UoM", "SO_Confirmed_Outstanding_Alt", DOQtyUoM, DOQtyAlt);
                    ListMethod.updateInventAmount("Increase", "Invent_Sales_Qty", FullItemId, "SO_Confirmed_Outstanding_Amount", DOQtyAmount);

                    ListMethod.updateInventUomAlt("Decrease", "Invent_Sales_Qty", FullItemId, "DO_Issued_Outstanding_UoM", "DO_Issued_Outstanding_Alt", DOQtyUoM, DOQtyAlt);
                    ListMethod.updateInventAmount("Decrease", "Invent_Sales_Qty", FullItemId, "DO_Issued_Outstanding_Amount", DOQtyAmount);

                    insertJournalD(ID, DONo, DOQtyAmount, i);
                }
            }
        }

        private static void insertJournalH(string ID, string DONo)
        {
            SqlCommand Cmd;
            SqlDataReader Dr;
            string Query;

            Query = "INSERT INTO [dbo].[GLJournalH] ([GLJournalHID],[JournalHID],[Notes],[Status],[Posting],[CreatedDate],[CreatedBy],[Referensi],[PostingDate]) ";
            Query += "VALUES(@GLJournalHID,@JournalHID,@Notes,@Status,@Posting,@CreatedDate,@CreatedBy,@Referensi,@PostingDate)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID", ID);
                Cmd.Parameters.AddWithValue("@JournalHID", "IN59");
                Cmd.Parameters.AddWithValue("@Referensi", DONo);
                Cmd.Parameters.AddWithValue("@Status", "Gunakan");
                Cmd.Parameters.AddWithValue("@Posting", 0);
                Cmd.Parameters.AddWithValue("@Notes", "");
                Cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                Cmd.Parameters.AddWithValue("@CreatedBy", Login.Username);
                Cmd.Parameters.AddWithValue("@PostingDate", DateTime.Now);
                Cmd.ExecuteNonQuery();
            }            
        }

        private static void insertJournalD(string ID, string DONo, decimal Amount,int i)
        {
            SqlCommand Cmd;
            SqlDataReader Dr;
            string Query;

            int x = (i * 2) - 1;
            int y = i * 2;

            Query = "INSERT INTO [dbo].[GLJournalDtl] ([GLJournalHID],[SeqNo],[JournalHID],[JournalDType],[FQAID],[FQADesc],[Amount],[Notes],[JournalIDSeqNo],[Auto],[CreatedDate],[CreatedBy]) VALUES(@GLJournalHID,@SeqNo,@JournalHID,@JournalDType,@FQAID,@FQADesc,@Amount,@Notes,@JournalIDSeqNo,@Auto,@CreatedDate,@CreatedBy)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID", ID);
                Cmd.Parameters.AddWithValue("@SeqNo", x);
                Cmd.Parameters.AddWithValue("@JournalHID", "IN59");
                Cmd.Parameters.AddWithValue("@JournalDType", "D");
                Cmd.Parameters.AddWithValue("@FQAID", "1001.003");
                Cmd.Parameters.AddWithValue("@FQADesc", "Inventory Trade – Available");
                Cmd.Parameters.AddWithValue("@Amount", Amount);
                Cmd.Parameters.AddWithValue("@Notes", "");
                Cmd.Parameters.AddWithValue("@Auto", "Auto");
                Cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                Cmd.Parameters.AddWithValue("@CreatedBy", Login.Username);
                Cmd.Parameters.AddWithValue("@JournalIDSeqNo",x);
                Cmd.ExecuteNonQuery();
            }            

            Query = "INSERT INTO [dbo].[GLJournalDtl] ([GLJournalHID],[SeqNo],[JournalHID],[JournalDType],[FQAID],[FQADesc],[Amount],[Notes],[JournalIDSeqNo],[Auto],[CreatedDate],[CreatedBy]) VALUES(@GLJournalHID,@SeqNo,@JournalHID,@JournalDType,@FQAID,@FQADesc,@Amount,@Notes,@JournalIDSeqNo,@Auto,@CreatedDate,@CreatedBy)";
            using (Cmd = new SqlCommand(Query, ConnectionString.GetConnection()))
            {
                Cmd.Parameters.AddWithValue("@GLJournalHID", ID);
                Cmd.Parameters.AddWithValue("@SeqNo", y);
                Cmd.Parameters.AddWithValue("@JournalHID", "IN59");
                Cmd.Parameters.AddWithValue("@JournalDType", "K");
                Cmd.Parameters.AddWithValue("@FQAID", "1001.019");
                Cmd.Parameters.AddWithValue("@FQADesc", "Inventory Trade – DO Issued");
                Cmd.Parameters.AddWithValue("@Amount", Amount);
                Cmd.Parameters.AddWithValue("@Notes", "");
                Cmd.Parameters.AddWithValue("@Auto", "Auto");
                Cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                Cmd.Parameters.AddWithValue("@CreatedBy", Login.Username);
                Cmd.Parameters.AddWithValue("@JournalIDSeqNo", y);
                Cmd.ExecuteNonQuery();
            }
        }
    }
}
