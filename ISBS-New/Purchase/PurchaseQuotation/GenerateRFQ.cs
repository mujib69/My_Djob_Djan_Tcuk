using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace ISBS_New.Purchase.PurchaseQuotation
{
    public partial class GenerateRFQ : MetroFramework.Forms.MetroForm
    {
        ReportDocument cryRpt;

        public GenerateRFQ()
        {
            InitializeComponent();
        }

        private void GenerateRFQ_Load(object sender, EventArgs e)
        {

            this.crystalReportViewer1.RefreshReport();
        }

        private void ViewerRFQ_Load(object sender, EventArgs e)
        {

        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ReportDocument cryRpt = new ReportDocument();
                cryRpt = reportRFQ1;
                crystalReportViewer1.ReportSource = cryRpt;
                crystalReportViewer1.Refresh();
                cryRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, @"E:\ASD.pdf");

                //cryRpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, HttpContext.Current.Response, true, "Report");
                MessageBox.Show("Exported Successful");
                System.Diagnostics.Process.Start("E:ASD.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {

            

        }
    }
}
