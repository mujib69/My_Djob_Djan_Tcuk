using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Purchase.RFQ
{
    public partial class InfoRFQ : MetroFramework.Forms.MetroForm
    {
        Purchase.RFQ.RFQForm Parent;

        public InfoRFQ()
        {
            InitializeComponent();
        }

        private void Info_Load(object sender, EventArgs e)
        {
            //lblForm.Location = new Point(16, 11);
        }

        private void Info_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(800, 20);
        }

        //public void SetParent(Purchase.PurchaseRequisition.HeaderPR F)
        //{
        //    Parent = F;
        //}

    }
}
