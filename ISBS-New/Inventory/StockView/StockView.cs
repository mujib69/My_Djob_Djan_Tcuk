using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Data.SqlClient;
using System.Collections.Generic;
using CrystalDecisions.ReportSource;
using System.IO;
using CrystalDecisions.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.ComponentModel;
using System.Transactions;

namespace ISBS_New.Inventory.StockView
{
    public partial class StockView : MetroFramework.Forms.MetroForm
    {      
        public StockView()
        {
            InitializeComponent();
        }

        private void StockView_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet1.View_Stock_OnHand' table. You can move, or remove it, as needed.
            this.view_Stock_OnHandTableAdapter.Fill(this.dataSet1.View_Stock_OnHand);
            // TODO: This line of code loads data into the 'dataSet1.View_Stock_OnHand' table. You can move, or remove it, as needed.
            this.view_Stock_OnHandTableAdapter.Fill(this.dataSet1.View_Stock_OnHand);
            
        }     
    }
}
