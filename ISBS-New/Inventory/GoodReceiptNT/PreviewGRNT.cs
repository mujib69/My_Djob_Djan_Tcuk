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

namespace ISBS_New.Inventory.GoodReceiptNT
{
    public partial class PreviewGRNT : MetroFramework.Forms.MetroForm
    {
        private string GRId, Mode;

        public PreviewGRNT(String grid, String mode)
        {
            this.GRId = grid;
            this.Mode = mode;
            InitializeComponent();
        }

        private void PreviewGRNT_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            if (Mode == "TT")
            {
                Rpt_GRNT rpt = new Rpt_GRNT();
                rpt.SetParameterValue("@GoodsReceivedId", GRId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBSN";
                    myLogin.ConnectionInfo.ServerName = "(local)";
                    myTable.ApplyLogOnInfo(myLogin);
                }

                crystalReportViewer1.ReportSource = rpt;

                crystalReportViewer1.Refresh();
            }
            else if (Mode == "BM")
            {
                Rpt_TiketBongkarMuatNT rpt = new Rpt_TiketBongkarMuatNT();
                rpt.SetParameterValue("@GoodsReceivedId", GRId);

                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBSN";
                    myLogin.ConnectionInfo.ServerName = "192.168.0.87";
                    myTable.ApplyLogOnInfo(myLogin);
                }

                crystalReportViewer1.ReportSource = rpt;

                crystalReportViewer1.Refresh();
            }

            
        }
    }
}
