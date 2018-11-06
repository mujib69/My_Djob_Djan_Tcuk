using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Transactions;
using System.Globalization;

namespace ISBS_New.EFaktur
{
    class InvoiceAP
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private string Result;
        private string Query;
        int i = 0;

        public string ExportInvoiceAP(string TanggalDari, string TanggalSampai, string FileName)
        {
           try
           {
               using (TransactionScope scope = new TransactionScope())
               {
                   //Set Connection
                   Conn = ConnectionString.GetConnection();

                   //Create StringBuilder
                   StringBuilder StrBuilder = new StringBuilder();

                   //Set Header
                   StrBuilder.AppendLine("FM;KD_JENIS_TRANSAKSI;FG_PENGGANTI;NOMOR_FAKTUR;MASA_PAJAK;TAHUN_PAJAK;TANGGAL_FAKTUR;NPWP;NAMA;ALAMAT_LENGKAP;JUMLAH_DPP;JUMLAH_PPN;JUMLAH_PPNBM;IS_CREDITABLE");

                   //Inisialisasi Variable
                   string FM = "", KD_JENIS_TRANSAKSI = "", FG_PENGGANTI = "", NOMOR_FAKTUR = "", MASA_PAJAK = "", TAHUN_PAJAK = "", TANGGAL_FAKTUR = "", NPWP = "", NAMA = "", ALAMAT_LENGKAP = "", JUMLAH_PPNBM = "", IS_CREDITABLE = "";
                   decimal JUMLAH_DPP = 0, JUMLAH_PPN = 0;
                   //Get Data FK
                   Query = "Select a.TaxNum,a.TaxDate,a.NPWP,a.VendName,b.TaxAddress,CONVERT(DECIMAL(22, 0),ROUND(a.InvoiceAmount, 0)) InvoiceAmount, CONVERT(DECIMAL(22, 0),ROUND(a.InvoiceTaxAmount, 0)) InvoiceTaxAmount From dbo.VendInvoiceH a ";
                   Query += "inner join VendTable b on a.VendId=b.VendId where a.InvoiceDate>='" + TanggalDari + "' and a.InvoiceDate<='" + TanggalSampai +"' and a.EFAKTUR <> 'SUDAH' and a.TransStatus='11' and TaxPercent>0;";
                   //Update
                   Query += "Update dbo.VendInvoiceH set Efaktur='SUDAH' where InvoiceDate>='" + TanggalDari + "' and InvoiceDate<='" + TanggalSampai + "' and EFAKTUR <> 'SUDAH' and TransStatus='11' and TaxPercent>0;";

                   Cmd = new SqlCommand(Query, Conn);
                   Dr = Cmd.ExecuteReader();
                   while (Dr.Read())
                   {
                       FM = "FM";
                       KD_JENIS_TRANSAKSI = Dr["TaxNum"].ToString().Length >= 2 ? Dr["TaxNum"].ToString().Substring(0, 2): Dr["TaxNum"].ToString();
                       FG_PENGGANTI = Dr["TaxNum"].ToString().Length >= 3 ?  Dr["TaxNum"].ToString().Substring(2, 1):Dr["TaxNum"].ToString();
                       NOMOR_FAKTUR = Dr["TaxNum"].ToString();
                       MASA_PAJAK = Convert.ToDateTime(Dr["TaxDate"]).Month.ToString();
                       TAHUN_PAJAK = Convert.ToDateTime(Dr["TaxDate"]).Year.ToString();
                       TANGGAL_FAKTUR = Convert.ToDateTime(Dr["TaxDate"]).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                       NPWP = Dr["NPWP"].ToString();
                       NAMA = Dr["VendName"].ToString();
                       ALAMAT_LENGKAP = Dr["TaxAddress"].ToString();
                       JUMLAH_DPP = Convert.ToDecimal(Dr["InvoiceAmount"]);
                       JUMLAH_PPN = Convert.ToDecimal(Dr["InvoiceTaxAmount"]);
                       JUMLAH_PPNBM = "0";
                       //if (Dr["TaxNum"].ToString().Substring(1, 2) == "80" || Dr["TaxNum"].ToString().Substring(1, 2) == "10" || Dr["TaxNum"].ToString().Substring(1, 2) == "40")
                       //{
                           IS_CREDITABLE = "1";
                           if (i == 83)
                           {
                               i = 83;
                           }
                       //}
                       //else
                       //{
                       //    IS_CREDITABLE = "0";
                       //}
                       //Set Detail
                       var DetailFK = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13}", FM, KD_JENIS_TRANSAKSI, FG_PENGGANTI, NOMOR_FAKTUR, MASA_PAJAK, TAHUN_PAJAK, TANGGAL_FAKTUR, NPWP, NAMA, ALAMAT_LENGKAP, JUMLAH_DPP, JUMLAH_PPN, JUMLAH_PPNBM, IS_CREDITABLE);

                       //Add New Line with Data
                       StrBuilder.AppendLine(DetailFK);
                       i++;
                   }
                   Dr.Close();

                   //Generate CSV
                   File.WriteAllText(FileName, StrBuilder.ToString());
                   scope.Complete();
                   Result = "SUCCESS";
               }
            }
            catch (Exception e)
            {
                int j = i;
                Result = "FAILED : " + e.Message.ToString();
            }
            finally
            { 
               Conn.Close();
            }
            
            return Result;

        }
    }
}
