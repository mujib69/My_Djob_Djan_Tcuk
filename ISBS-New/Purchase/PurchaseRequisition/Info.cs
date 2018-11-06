using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ISBS_New.Purchase.PurchaseRequisition
{
    public partial class Info : MetroFramework.Forms.MetroForm
    {

        Purchase.PurchaseRequisition.HeaderPR Parent;

        public Info()
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //public void SetParent(Purchase.PurchaseRequisition.HeaderPR F)
        //{
        //    Parent = F;
        //}

    }
}
