using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ISBS_New.AccountPayable.InvoicePayment
{
    public partial class Testing : MetroFramework.Forms.MetroForm
    {
        int HVscroll;

        public Testing()
        {
            InitializeComponent();
            this.metroPanel1.MouseWheel += InvoicePayment_MouseWheel;
        }

        private void InvoicePayment_Load(object sender, EventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Position();
        }

        private void InvoicePayment_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void Position()
        {
            label1.Text = vScrollBar1.Value.ToString();
            if (HVscroll < vScrollBar1.Value)
            {
                button1.Location = new Point(button1.Location.X, 139 + vScrollBar1.Value * 2);
                button2.Location = new Point(button2.Location.X, 169 + vScrollBar1.Value * 2);
            }
            if (HVscroll > vScrollBar1.Value)
            {
                button1.Location = new Point(button1.Location.X, (139 + vScrollBar1.Value * 2) - (HVscroll * 2 - vScrollBar1.Value * 2));
                button2.Location = new Point(button2.Location.X, (169 + vScrollBar1.Value * 2) - (HVscroll * 2 - vScrollBar1.Value * 2));
            }
            HVscroll = vScrollBar1.Value;
        }

        private void InvoicePayment_MouseWheel(object sender, MouseEventArgs e)
        {
            if(e.Delta <0 && vScrollBar1.Value !=0 )
                vScrollBar1.Value -=1;
            else if (e.Delta >0)
                vScrollBar1.Value +=1;
            Position();

            //you can do anything here
        }

    }
}
