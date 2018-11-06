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

namespace ISBS_New.ARCollection.Collection
{
    public partial class PreviewCollectionPrintOut : MetroFramework.Forms.MetroForm
    {
        string CLId;
        string CollectionType;
        string BodyText;
        PrintOut rpt;
        PrintOutMail rpt2;

        public PreviewCollectionPrintOut(string passedCLId, string CollectionTipe)
        {
            InitializeComponent();
            CLId = passedCLId;
            CollectionType = CollectionTipe;
        }
        public PreviewCollectionPrintOut(string passedCLId, string CollectionTipe, string bodytext)
        {
            InitializeComponent();
            CLId = passedCLId;
            CollectionType = CollectionTipe;
            BodyText = bodytext;
        }
        private void PreviewCollectionPrintOut_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            if (CollectionType.ToUpper() == "MANUAL")
            {
                rpt = new PrintOut();
                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.0.87";
                    myTable.ApplyLogOnInfo(myLogin);
                }

                crystalReportViewer1.ReportSource = rpt;

                ParameterDiscreteValue paramDV_CLId = new ParameterDiscreteValue();
                paramDV_CLId.Value = CLId;
                rpt.ParameterFields["@CollectionNo"].CurrentValues.Add(paramDV_CLId);

                crystalReportViewer1.Refresh();
            }
            else if (CollectionType.ToUpper() == "POST")
            {
                rpt2 = new PrintOutMail();
                CrystalDecisions.Shared.TableLogOnInfo myLogin = new TableLogOnInfo();
                foreach (CrystalDecisions.CrystalReports.Engine.Table myTable in rpt2.Database.Tables)
                {
                    myLogin = myTable.LogOnInfo;
                    myLogin.ConnectionInfo.Password = "sql123";
                    myLogin.ConnectionInfo.UserID = "sa";
                    myLogin.ConnectionInfo.DatabaseName = "ISBS-NEW4";
                    myLogin.ConnectionInfo.ServerName = "192.168.0.87";
                    myTable.ApplyLogOnInfo(myLogin);
                }

                crystalReportViewer1.ReportSource = rpt2;

                ParameterDiscreteValue paramDV_CLId = new ParameterDiscreteValue();
                paramDV_CLId.Value = CLId;
                rpt2.ParameterFields["@CollectionNo"].CurrentValues.Add(paramDV_CLId);

                ParameterDiscreteValue Text = new ParameterDiscreteValue();
                Text.Value = BodyText;
                rpt2.ParameterFields["@TextBody"].CurrentValues.Add(Text);

                crystalReportViewer1.Refresh();
            }
        }
    }
}
