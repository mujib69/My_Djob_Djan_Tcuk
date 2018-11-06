using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.MoUCustomerId
{
    public partial class MoUId : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private string MouId;
        PopUp.Vendor.Vendor Vendor = null;
        PopUp.CustomerID.Customer Cust = null;


        public MoUId()
        {
            InitializeComponent();
        }

        private void MoUId_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            foreach (var scrn in Screen.AllScreens)
            {
                if (scrn.Bounds.Contains(this.Location))
                    this.Location = new Point(scrn.Bounds.Right - this.Width, scrn.Bounds.Top);
                dtJatuhTempo.Enabled = false;
                dtLCDate.Enabled = false;
                dtMouDate.Enabled = false;
                dtValidTo.Enabled = false;
            }
        }

        public void GetData(string MouId)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select a.MoUNo, a.MoUDate, a.ValidTo, a.CustID, a.CustName, a.TotalAmount, a.BankGuarantee, a.LC_No, a.LC_Type, a.LC_Date, a.LC_DueDate, a.BankID, b.BankName From [CustMouH] a left join BankTable b on a.BankID=b.BankId ";
            Query += "Where MoUNo = '" + MouId + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                dtMouDate.Text=Dr["MoUDate"].ToString();
                lblMoUNumber.Text = Dr["MoUNo"].ToString();
                dtValidTo.Text = Dr["ValidTo"].ToString();
                lblCustID.Text = Dr["CustID"].ToString();
                lblCustName.Text = Dr["CustName"].ToString();
                lblLimitCredit.Text = Dr["TotalAmount"].ToString();
                lblBankGuarantee.Text = Dr["BankGuarantee"].ToString();
             
                lblLCNo.Text = Dr["LC_No"].ToString();
                lblLCType.Text = Dr["LC_Type"].ToString();
                dtLCDate.Text = Dr["LC_Date"].ToString();
                dtJatuhTempo.Text = Dr["LC_DueDate"].ToString();
                lblBankID.Text = Dr["BankID"].ToString();
                lblBankName.Text = Dr["BankName"].ToString();

            }
            Dr.Close();

            dgvMoU.DataSource = null;
            if (dgvMoU.RowCount==0)
            {
                dgvMoU.Rows.Clear();
                dgvMoU.ColumnCount = 4;
                dgvMoU.Columns[0].Name = "No";
                dgvMoU.Columns[1].Name = "CustID";
                dgvMoU.Columns[2].Name = "CustName";
                dgvMoU.Columns[3].Name = "CreditAmount";
            }
            Query = "Select SeqNo AS No, CustID, CustName, Amount AS CreditAmount From [CustMou_Dtl] Where MoUNo = '" + MouId + "' order by SeqNo asc";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            int i = 1;
            while (Dr.Read())
            {
                this.dgvMoU.Rows.Add(i, Dr["CustID"], Dr["CustName"], Dr["CreditAmount"]);
                i++;
            }
            Dr.Close();

            dgvMoU.ReadOnly = true;
            dgvMoU.AutoResizeColumns();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvMoU_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                if (Cust == null || Cust.Text == "")
                {
                    if (dgvMoU.Columns[e.ColumnIndex].Name.ToString() == "CustID" || dgvMoU.Columns[e.ColumnIndex].Name.ToString() == "CustName")
                    {
                        Cust = new PopUp.CustomerID.Customer();
                        Cust.GetData(dgvMoU.Rows[e.RowIndex].Cells["CustID"].Value.ToString());
                        //itemID = dataGridView1.Rows[e.RowIndex].Cells["FullItemID"].Value.ToString();
                        Cust.Show();
                    }
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.GetData(dgvMoU.Rows[e.RowIndex].Cells["CustID"].Value.ToString());
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }
        private bool CheckOpened(string name)
        {
            FormCollection FC = Application.OpenForms;
            foreach (Form frm in FC)
            {
                if (frm.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void lblCustID_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    lblCustID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(lblCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }

        private void lblCustName_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Cust == null || Cust.Text == "")
                {
                    lblCustID.Enabled = true;
                    Cust = new PopUp.CustomerID.Customer();
                    Cust.GetData(lblCustID.Text);
                    Cust.Show();
                }
                else if (CheckOpened(Cust.Name))
                {
                    Cust.WindowState = FormWindowState.Normal;
                    Cust.Show();
                    Cust.Focus();
                }
            }
        }
    }
}
