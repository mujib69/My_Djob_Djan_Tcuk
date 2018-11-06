using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ISBS_New.EFaktur
{
    partial class EFaktur : MetroFramework.Forms.MetroForm
    {
        public EFaktur()
        {
            InitializeComponent();

            SetTipe();
            this.CenterToScreen();
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        private void SetTipe()
        { 
            cmbTipe.Items.Clear();
            cmbTipe.Items.Add("Invoice AR");
            cmbTipe.Items.Add("Invoice AP");
            cmbTipe.SelectedIndex = 0;        
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "CSV|*.csv";;
            sfd.Filter = "CSV|*.csv";
            sfd.FilterIndex = 1;

            if(cmbTipe.SelectedIndex == 0)
            {
                string FileName = "InvoiceAR" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                sfd.FileName = FileName;

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    InvoiceAR AR = new InvoiceAR();
                    string result = AR.ExportInvoiceAR(dtDari.Value.ToString("yyyy/MM/dd"), dtSampai.Value.ToString("yyyy/MM/dd"), sfd.FileName);

                    if (result.ToUpper() == "SUCCESS")
                    {
                        MessageBox.Show("Data berhasil diexport pada directory " + sfd.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Data gagal diexport");
                    }
                }                
            }
            if (cmbTipe.SelectedIndex == 1)
            {
                string FileName = "InvoiceAP" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                sfd.FileName = FileName;

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    InvoiceAP AP = new InvoiceAP();
                    string result = AP.ExportInvoiceAP(dtDari.Value.ToString("yyyy/MM/dd"), dtSampai.Value.ToString("yyyy/MM/dd"), sfd.FileName);

                    if (result.ToUpper() == "SUCCESS")
                    {
                        MessageBox.Show("Data berhasil diexport pada directory " + sfd.FileName);
                    }
                    else
                    {
                        MessageBox.Show("Data gagal diexport");
                    }
                } 
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
