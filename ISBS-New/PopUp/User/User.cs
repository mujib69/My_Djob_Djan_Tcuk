using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.User
{
    public partial class User : MetroFramework.Forms.MetroForm
    {

        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        public string CustId;

        string Query = "";
        DataGridView dgvUserHeader;

        Master.Sales.Sales Parent;

        public User()
        {
            InitializeComponent();
        }

        private void User_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);
            RefreshGrid();
        }

        public void RefreshGrid()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select ROW_NUMBER() OVER (ORDER BY UserID asc) No,* From (Select a.UserID From dbo.[sysPass] a) b ";
          
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            if (dgvUser.Columns.Contains("chk") == false)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                dgvUser.Columns.Add(chk);
                chk.HeaderText = "Check";
                chk.Name = "chk";
            }

            Da.Fill(Dt);

            dgvUser.AutoGenerateColumns = true;
            dgvUser.ReadOnly = false;
            dgvUser.DataSource = Dt;
            dgvUser.Refresh();

            dgvUser.Columns["No"].ReadOnly = true;
            dgvUser.Columns["UserID"].ReadOnly = true;


            string UserIDH = "";
            string UserIDD = "";
            List<string> RemoveUserID = new List<string>();

            for (int i = 0; i < dgvUser.RowCount; i++)
            {
                UserIDD = dgvUser.Rows[i].Cells["UserID"].Value.ToString();

                for (int j = 0; j < dgvUserHeader.RowCount; j++)
                {
                    UserIDH = dgvUserHeader.Rows[j].Cells["UserID"].Value.ToString();

                    if (UserIDH == UserIDD)
                    {
                        RemoveUserID.Add(UserIDD);
                    }
                }
            }

            for (int i = 0; i < RemoveUserID.Count; i++)
            {
                for (int j = 0; j < dgvUser.RowCount; j++)
                {
                    UserIDD = dgvUser.Rows[j].Cells["UserID"].Value.ToString();
                    if (UserIDD == RemoveUserID[i])
                    {
                        dgvUser.Rows.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < dgvUser.RowCount; i++)
            {
                dgvUser.Rows[i].Cells["No"].Value = i + 1;
            }           

            Conn.Close();
           
        }

        public void ParamHeader(DataGridView prmDgvUserFromHeader)
        {
            dgvUserHeader = prmDgvUserFromHeader;
        }

        private void User_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, (63));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            List<string> UserID = new List<string>();

            int CountChk = 0;
            for (int i = 0; i <= dgvUser.RowCount - 1; i++)
            {
                Boolean Check = Convert.ToBoolean(dgvUser.Rows[i].Cells["chk"].Value);
                if (Check == true)
                {
                    CountChk++;
                    UserID.Add(dgvUser.Rows[i].Cells["UserID"].Value.ToString());
                }
            }

            Parent.AddDataGridUser(UserID);
            this.Close();
        }

        public void ParentRefreshGrid(Master.Sales.Sales F)
        {
            Parent = F;
        }

    }
}
