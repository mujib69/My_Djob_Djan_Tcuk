using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace ISBS_New.EFaktur
{
    class InvoiceAR
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataReader DrDetail;
        private string Result;
        private string Query;

        public string ExportInvoiceAR(string TanggalDari, string TanggalSampai, string FileName)
        {
            try
            {
                //Set Connection
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                //Set Header
                var HeaderFK = "FK;KD_JENIS_TRANSAKSI;FG_PENGGANTI;NOMOR_FAKTUR;MASA_PAJAK;TAHUN_PAJAK;TANGGAL_FAKTUR;NPWP;NAMA;ALAMAT_LENGKAP;JUMLAH_DPP;JUMLAH_PPN;JUMLAH_PPNBM;ID_KETERANGAN_TAMBAHAN;FG_UANG_MUKA;UANG_MUKA_DPP;UANG_MUKA_PPN;UANG_MUKA_PPNBM;REFERENSI";
                var HeaderLT = "LT;NPWP;NAMA;JALAN;BLOK;NOMOR;RT;RW;KECAMATAN;KELURAHAN;KABUPATEN;PROPINSI;KODE_POS;NOMOR_TELEPON";
                var HeaderOF = "OF;KODE_OBJEK;NAMA;HARGA_SATUAN;JUMLAH_BARANG;HARGA_TOTAL;DISKON;DPP;PPN;TARIF_PPNBM;PPNBM";

                //Create StringBuilder
                StringBuilder StrBuilder = new StringBuilder();

                //Add New Line with Data
                StrBuilder.AppendLine(HeaderFK);
                StrBuilder.AppendLine(HeaderLT);
                StrBuilder.AppendLine(HeaderOF);

                //Inisialisasi Variable
                string INVOICE_ID, INVOICE_TYPE, KD_JENIS_TRANSAKSI = "", FG_PENGGANTI = "", NOMOR_FAKTUR = "", MASA_PAJAK = "", TAHUN_PAJAK = "", TANGGAL_FAKTUR = "", NPWP = "", NAMAFK = "", ALAMAT_LENGKAP = "", JUMLAH_DPP = "", JUMLAH_PPN = "", JUMLAH_PPNBM = "", ID_KETERANGAN_TAMBAHAN = "", FG_UANG_MUKA = "", UANG_MUKA_DPP = "", UANG_MUKA_PPN = "", UANG_MUKA_PPNBM = "", REFERENSI = "";
                string KODE_OBJEK = "", NAMAOF = "", HARGA_SATUAN = "", JUMLAH_BARANG = "", HARGA_TOTAL = "", DISKON = "", DPP = "", PPN = "", TARIF_PPNBM = "", PPNBM = "";

                //Get Data FK
                Query = "SELECT ";
                Query += "C.Invoice_Id AS INVOICE_ID, ";
                Query += "C.Invoice_Type AS INVOICE_TYPE, ";
                Query += "SUBSTRING(C.TaxNum, 1, 2) AS KD_JENIS_TRANSAKSI, ";
                Query += "SUBSTRING(C.TaxNum, 3, 1) AS FG_PENGGANTI, ";
                Query += "SUBSTRING(C.TaxNum, 5, 3) + SUBSTRING(C.TaxNum, 9, 2) + SUBSTRING(C.TaxNum, 12, 8) AS NOMOR_FAKTUR, ";
                Query += "MONTH(C.TaxDate) AS MASA_PAJAK, ";
                Query += "YEAR(C.TaxDate) AS TAHUN_PAJAK, ";
                Query += "CONVERT(VARCHAR, C.TaxDate, 103) AS TANGGAL_FAKTUR, ";
                Query += "REPLACE(REPLACE(C.NPWP, '.', ''), '-', '') AS NPWP, ";
                Query += "C.TaxName AS NAMA, ";
                Query += "C.TaxAddress AS ALAMAT_LENGKAP, ";
                Query += "CONVERT(DECIMAL(22, 0),ROUND(C.Invoice_Tax_Base_Amount, 0)) AS JUMLAH_DPP, ";
                Query += "CONVERT(DECIMAL(22, 0),ROUND(C.Invoice_Tax_Amount, 0)) AS JUMLAH_PPN, ";
                Query += "'0' AS JUMLAh_PPNBM, ";
                Query += "C.KodeKetTambahan AS ID_KETERANGAN_TAMBAHAN, ";
                Query += "CASE WHEN C.DP_Amount != 0 THEN 1 ELSE 0 END AS FG_UANG_MUKA, ";
                Query += "CONVERT(DECIMAL(22, 0),ROUND(C.DP_Amount - (C.DP_Amount * 0.1), 0)) AS UANG_MUKA_DPP, ";
                Query += "CONVERT(DECIMAL(22, 0),ROUND(C.DP_Amount, 0)) AS UANG_MUKA_PPN, ";
                Query += "'0' AS UANG_MUKA_PPNBM, ";
                Query += "CASE WHEN (SELECT S.Referensi FROM SalesOrderH S WHERE S.SalesOrderNo = C.SalesOrderNo) = 'By Phone' THEN '' ELSE (SELECT S.Referensi FROM SalesOrderH S WHERE S.SalesOrderNo = C.SalesOrderNo) END AS REFERENSI ";
                Query += "FROM CustInvoice_H C ";
                Query += "WHERE CONVERT(VARCHAR(10), C.Invoice_Date, 111) >= '" + TanggalDari + "' AND CONVERT(VARCHAR(10), C.Invoice_Date, 111) <= '" + TanggalSampai + "' ";
                Query += "AND C.PPN_Percent != 0 AND C.TransStatus IN ('03', '11', '13') ";
                Query += "AND C.Invoice_Type IN ('Down Payment', 'Invoice') ";

                Cmd = new SqlCommand(Query, Conn, Trans);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    INVOICE_ID = Convert.ToString(Dr["INVOICE_ID"]);
                    INVOICE_TYPE = Convert.ToString(Dr["INVOICE_TYPE"]);
                    KD_JENIS_TRANSAKSI = Convert.ToString(Dr["KD_JENIS_TRANSAKSI"]);
                    FG_PENGGANTI = Convert.ToString(Dr["FG_PENGGANTI"]);
                    NOMOR_FAKTUR = Convert.ToString(Dr["NOMOR_FAKTUR"]); ;
                    MASA_PAJAK = Convert.ToString(Dr["MASA_PAJAK"]);
                    TAHUN_PAJAK = Convert.ToString(Dr["TAHUN_PAJAK"]);
                    TANGGAL_FAKTUR = Convert.ToString(Dr["TANGGAL_FAKTUR"]); ;
                    NPWP = Convert.ToString(Dr["NPWP"]);
                    NAMAFK = Convert.ToString(Dr["NAMA"]);
                    ALAMAT_LENGKAP = Convert.ToString(Dr["ALAMAT_LENGKAP"]);
                    JUMLAH_DPP = Convert.ToString(Dr["JUMLAH_DPP"]);
                    JUMLAH_PPN = Convert.ToString(Dr["JUMLAH_PPN"]);
                    JUMLAH_PPNBM = Convert.ToString(Dr["JUMLAH_PPNBM"]);
                    ID_KETERANGAN_TAMBAHAN = Convert.ToString(Dr["ID_KETERANGAN_TAMBAHAN"]);
                    FG_UANG_MUKA = Convert.ToString(Dr["FG_UANG_MUKA"]);
                    UANG_MUKA_DPP = Convert.ToString(Dr["UANG_MUKA_DPP"]);
                    UANG_MUKA_PPN = Convert.ToString(Dr["UANG_MUKA_PPN"]);
                    UANG_MUKA_PPNBM = Convert.ToString(Dr["UANG_MUKA_PPNBM"]);
                    REFERENSI = Convert.ToString(Dr["REFERENSI"]);

                    //Update Status Efaktur on Table CustInvoice_H
                    Query = "UPDATE CustInvoice_H SET ExportEFaktur = 'SUDAH', UpdatedBy = '" + ControlMgr.UserId + "', UpdatedDate = GETDATE() WHERE Invoice_Id = '" + INVOICE_ID + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    //Set Detail
                    var DetailFK = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18}", "FK", KD_JENIS_TRANSAKSI, FG_PENGGANTI, NOMOR_FAKTUR, MASA_PAJAK, TAHUN_PAJAK, TANGGAL_FAKTUR, NPWP, NAMAFK, ALAMAT_LENGKAP, JUMLAH_DPP, JUMLAH_PPN, JUMLAH_PPNBM, ID_KETERANGAN_TAMBAHAN, FG_UANG_MUKA, UANG_MUKA_DPP, UANG_MUKA_PPN, UANG_MUKA_PPNBM, REFERENSI);

                    //Add New Line with Data
                    StrBuilder.AppendLine(DetailFK);

                    if (INVOICE_TYPE == "Invoice")
                    {
                        //Get Data OF Invoice Type = INVOICE
                        Query = "SELECT ";
                        Query += "D.FullItemId AS KODE_OBJEK, ";
                        Query += "D.Item_Name AS NAMA, ";
                        Query += "FORMAT(CAST(D.Price AS DECIMAL(22,2)), 'g18') AS HARGA_SATUAN, ";
                        Query += "FORMAT(CAST(D.Qty AS DECIMAL(22,2)), 'g18') AS JUMLAH_BARANG, ";
                        Query += "CONVERT(DECIMAL(22, 0),ROUND(D.Total, 0)) AS HARGA_TOTAL, ";
                        Query += "CONVERT(DECIMAL(22, 0),ROUND(D.Total_Disc, 0)) AS DISKON, ";
                        Query += "CONVERT(DECIMAL(22, 0),ROUND(D.Line_Tax_Base_Amount, 0)) AS DPP, ";
                        Query += "CONVERT(DECIMAL(22, 0),ROUND(D.Line_Tax_Amount, 0)) AS PPN, ";
                        Query += "'0' AS TARIF_PPNBM, ";
                        Query += "'0' AS PPNBM ";
                        Query += "FROM CustInvoice_Dtl_SO_Dtl D ";
                        Query += "INNER JOIN CustInvoice_H H ";
                        Query += "ON H.Invoice_Id = D.Invoice_Id ";
                        Query += "WHERE CONVERT(VARCHAR(10), H.Invoice_Date, 111) >= '" + TanggalDari + "' AND CONVERT(VARCHAR(10), H.Invoice_Date, 111) <= '" + TanggalSampai + "' ";
                        Query += "AND H.PPN_Percent != 0 AND H.TransStatus IN ('03', '11', '13') ";
                        Query += "AND H.Invoice_Type = 'Invoice' ";
                        Query += "AND H.Invoice_Id = '" + INVOICE_ID + "'";

                        Cmd = new SqlCommand(Query, Conn, Trans);
                        DrDetail = Cmd.ExecuteReader();
                        while (DrDetail.Read())
                        {
                            KODE_OBJEK = Convert.ToString(DrDetail["KODE_OBJEK"]);
                            NAMAOF = Convert.ToString(DrDetail["NAMA"]);
                            HARGA_SATUAN = Convert.ToString(DrDetail["HARGA_SATUAN"]);
                            JUMLAH_BARANG = Convert.ToString(DrDetail["JUMLAH_BARANG"]);
                            HARGA_TOTAL = Convert.ToString(DrDetail["HARGA_TOTAL"]);
                            DISKON = Convert.ToString(DrDetail["DISKON"]);
                            DPP = Convert.ToString(DrDetail["DPP"]);
                            PPN = Convert.ToString(DrDetail["PPN"]);
                            TARIF_PPNBM = Convert.ToString(DrDetail["TARIF_PPNBM"]);
                            PPNBM = Convert.ToString(DrDetail["PPNBM"]);

                            //Set Detail
                            var DetailOF = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", "OF", KODE_OBJEK, NAMAOF, HARGA_SATUAN, JUMLAH_BARANG, HARGA_TOTAL, DISKON, DPP, PPN, TARIF_PPNBM, PPNBM);

                            //Add New Line with Data
                            StrBuilder.AppendLine(DetailOF);
                        }
                        DrDetail.Close();
                    }
                    else
                    {
                        //Get Data OF Invoice Type = DP
                        Query = "SELECT ";
                        Query += "'UANG MUKA CUSTOMER' AS KODE_OBJEK, ";
                        Query += "H.DPDescription AS NAMA, ";
                        Query += "FORMAT(CAST(H.Invoice_Tax_Base_Amount AS DECIMAL(22,2)), 'g18') AS HARGA_SATUAN, ";
                        Query += "'1.00' AS JUMLAH_BARANG, ";
                        Query += "CONVERT(DECIMAL(22, 0),ROUND(H.Invoice_Tax_Base_Amount, 0)) AS HARGA_TOTAL, ";
                        Query += "'0' AS DISKON, ";
                        Query += "CONVERT(DECIMAL(22, 0),ROUND(H.Invoice_Tax_Base_Amount, 0)) AS DPP, ";
                        Query += "CONVERT(DECIMAL(22, 0),ROUND(H.Invoice_Tax_Amount, 0)) AS PPN, ";
                        Query += "'0' AS TARIF_PPNBM, ";
                        Query += "'0' AS PPNBM ";
                        Query += "FROM CustInvoice_H H ";
                        Query += "WHERE CONVERT(VARCHAR(10), H.Invoice_Date, 111) >= '" + TanggalDari + "' AND CONVERT(VARCHAR(10), H.Invoice_Date, 111) <= '" + TanggalSampai + "' ";
                        Query += "AND H.PPN_Percent != 0 AND H.TransStatus IN ('03', '11', '13') ";
                        Query += "AND H.Invoice_Type = 'Down Payment' ";
                        Query += "AND H.Invoice_Id = '" + INVOICE_ID + "'";

                        Cmd = new SqlCommand(Query, Conn, Trans);
                        DrDetail = Cmd.ExecuteReader();
                        while (DrDetail.Read())
                        {
                            KODE_OBJEK = Convert.ToString(DrDetail["KODE_OBJEK"]);
                            NAMAOF = Convert.ToString(DrDetail["NAMA"]);
                            HARGA_SATUAN = Convert.ToString(DrDetail["HARGA_SATUAN"]);
                            JUMLAH_BARANG = Convert.ToString(DrDetail["JUMLAH_BARANG"]);
                            HARGA_TOTAL = Convert.ToString(DrDetail["HARGA_TOTAL"]);
                            DISKON = Convert.ToString(DrDetail["DISKON"]);
                            DPP = Convert.ToString(DrDetail["DPP"]);
                            PPN = Convert.ToString(DrDetail["PPN"]);
                            TARIF_PPNBM = Convert.ToString(DrDetail["TARIF_PPNBM"]);
                            PPNBM = Convert.ToString(DrDetail["PPNBM"]);

                            //Set Detail
                            var DetailOF = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}", "OF", KODE_OBJEK, NAMAOF, HARGA_SATUAN, JUMLAH_BARANG, HARGA_TOTAL, DISKON, DPP, PPN, TARIF_PPNBM, PPNBM);

                            //Add New Line with Data
                            StrBuilder.AppendLine(DetailOF);
                        }
                        DrDetail.Close();
                    }
                }
                Dr.Close();

                Trans.Commit();

                //Generate CSV
                File.WriteAllText(FileName, StrBuilder.ToString());
                Result = "SUCCESS";
            }
            catch (Exception e)
            {
                Trans.Rollback();
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
