using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//BY: HC
namespace ISBS_New
{
    public partial class Bookmark : Form
    {
        string[] data1 = new string[] {"Create PR", 
                                        "Generate RFQ", 
                                        "Create Quotation",
                                        "Create Canvas Sheet",
                                        "List PA"};

        public Bookmark()
        {
            InitializeComponent();
            gvData();
        }

        private void gvData()
        {
            //GV FORMAT
            dataGridView1.ColumnCount = 2;
            dataGridView1.ReadOnly = true;

            dataGridView2.ColumnCount = 2;
            dataGridView2.ReadOnly = true;

            //GV HEADER
            dataGridView1.Columns[0].HeaderText = "No";
            dataGridView1.Columns[0].Name = "No";
            dataGridView1.Columns[1].HeaderText = "Description";
            dataGridView1.Columns[1].Name = "Description";

            dataGridView2.Columns[0].HeaderText = "No";
            dataGridView2.Columns[0].Name = "No";
            dataGridView2.Columns[1].HeaderText = "Description";
            dataGridView2.Columns[1].Name = "Description";

            //GV 1 VALUE
            for (int i = 0; i < data1.Length; i++)
            {
                dataGridView1.Rows.Add();
                //dataGridView1.Rows.Add(data1[i]);
                dataGridView1.Rows[i].Cells["Description"].Value = data1[i];
            }

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {

                dataGridView2.Rows.Add();
                dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells["Description"].Value = dataGridView1.Rows[e.RowIndex].Cells["Description"].Value.ToString();
                //dataGridView2.Rows.Add(dataGridView1.Rows[e.RowIndex].Cells["Description"].Value.ToString());
                int row = dataGridView1.CurrentCell.RowIndex;
                dataGridView1.Rows.RemoveAt(row);
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Description"].Value = dataGridView2.Rows[e.RowIndex].Cells["Description"].Value.ToString();
                //dataGridView1.Rows.Add(dataGridView2.Rows[e.RowIndex].Cells["Description"].Value.ToString());
                int row = dataGridView2.CurrentCell.RowIndex;
                dataGridView2.Rows.RemoveAt(row);
            }
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            MainMenu f = new MainMenu();
            f.Show();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView2.Rows.Add();
                dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells["Description"].Value = dataGridView1.CurrentRow.Cells["Description"].Value.ToString();

                //dataGridView2.Rows.Add(dataGridView1.CurrentRow.Cells["Description"].Value.ToString());
                int row = dataGridView1.CurrentCell.RowIndex;
                dataGridView1.Rows.RemoveAt(row);
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count != 0)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Description"].Value = dataGridView2.CurrentRow.Cells["Description"].Value.ToString();
                //dataGridView1.Rows.Add(dataGridView2.CurrentRow.Cells["Description"].Value.ToString());
                int row = dataGridView2.CurrentCell.RowIndex;
                dataGridView2.Rows.RemoveAt(row);
            }
        }

        private void btnARight_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView2.Rows.Add();
                    dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells["Description"].Value = dataGridView1.Rows[i].Cells["Description"].Value.ToString();

                    //dataGridView2.Rows.Add(dataGridView1.Rows[i].Cells["Description"].Value.ToString());
                }
                dataGridView1.Rows.Clear();
            }
        }

        private void btnALeft_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count != 0)
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells["Description"].Value = dataGridView2.Rows[i].Cells["Description"].Value.ToString();
                    //dataGridView1.Rows.Add(dataGridView2.Rows[i].Cells["Description"].Value.ToString());
                }
                dataGridView2.Rows.Clear();
            }
        }

        char flag;
        LinkLabel[] linklabel;
        private void btnSave_Click(object sender, EventArgs e)
        {
            //MainMenu f = new MainMenu();
            if (flag != 'X')
            {
                linklabel = new LinkLabel[dataGridView2.Rows.Count];
            }
            int pos = 0;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                linklabel[i] = new LinkLabel();
                linklabel[i].Text = dataGridView2.Rows[i].Cells["Description"].Value.ToString();
                linklabel[i].Location = new System.Drawing.Point(10, 50 + pos);
                linklabel[i].LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel_LinkClicked);
                linklabel[i].LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                this.groupBox1.Controls.Add(linklabel[i]);//f.groupBox5.Controls.Add(linklabel[i]);
                pos += 28;
            }
            if (flag == 'X')
            {
                for (int i = 0; i <= 4; i++)
                {
                    linklabel[i].Visible = false;
                }
            }
            flag = 'X';
            //this.Close();
            //f.Show();
        }

        private void linkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            if (sender.ToString().Contains("Create PR") == true)
            {
                Purchase.PurchaseRequisition.InquiryPR F = new Purchase.PurchaseRequisition.InquiryPR();
                F.Show();
            }
            if (sender.ToString().Contains("Generate RFQ") == true)
            {
                Purchase.PurchaseRequisition.InquiryPR F = new Purchase.PurchaseRequisition.InquiryPR();
                F.Show();
            }
            if (sender.ToString().Contains("Create Quotation") == true)
            {
                Purchase.PurchaseQuotation.InquiryPQ F = new Purchase.PurchaseQuotation.InquiryPQ();
                F.Show();
            }
            if (sender.ToString().Contains("Create Canvas Sheet") == true)
            {
                Purchase.CanvasSheet.InquiryCanvasSheet F = new Purchase.CanvasSheet.InquiryCanvasSheet();
                F.Show();
            }
            if (sender.ToString().Contains("List PA") == true)
            {
                Purchase.PurchaseAgreement.PAInq F = new Purchase.PurchaseAgreement.PAInq();
                F.Show();
            }
        }

        private void Bookmark_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.dataGridView1.Rows[e.RowIndex].Cells[0].Value = (e.RowIndex + 1).ToString();
        }

        private void dataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            this.dataGridView2.Rows[e.RowIndex].Cells[0].Value = (e.RowIndex + 1).ToString();
        }
    }
}
