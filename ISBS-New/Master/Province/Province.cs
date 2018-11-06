using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Province
{
    public partial class Province : Form
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Total, Page1, Page2, Index;
        String ProvinceId = null;

        Master.Province.InqProvince Parent;

        public Province()
        {
            InitializeComponent();
        }

        public void flag(String provinceid, String mode)
        {
            ProvinceId = provinceid;
            Mode = mode;
        }

        public void setParent(Master.Province.InqProvince F)
        {
            Parent = F;
        }

        private void Province_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String ProvinceId)
        {
            Query = "Select * From [dbo].[Province] Where ProvinceId = '" + ProvinceId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                if (Dr.Read())//sama dengan di database
                {
                    Conn.Close();
                    return false;
                }
                else
                {
                    Conn.Close();
                    return true;
                }
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[Province] Where ProvinceId = '" + ProvinceId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtProvinceId.Text = Dr["ProvinceId"].ToString(); ;
                    txtProvinceName.Text = Dr["ProvinceName"].ToString();
                }
            }
            Conn.Close();
        }

        private void ModeLoad()
        {
            RefreshGrid();
        }

        private void resetText()
        {
            txtProvinceId.Text = "";
            txtProvinceName.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtProvinceId.Enabled = true;
            txtProvinceName.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtProvinceId.Enabled = false;
            txtProvinceName.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Parent.RefreshGrid();
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                if (String.IsNullOrEmpty(txtProvinceId.Text) || String.IsNullOrEmpty(txtProvinceName.Text))
                {
                    MessageBox.Show("Data harus diisi");
                    return;
                }

                if (cekValidasi(txtProvinceId.Text) == false)
                {
                    MessageBox.Show("Data sudah ada di database.");
                }
                else
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    try
                    {
                        Query = "insert into [dbo].[Province] (ProvinceId, ProvinceName) ";
                        Query += "values ('" + txtProvinceId.Text + "', '" + txtProvinceName.Text + "');";

                        using (SqlCommand Cmd = new SqlCommand(Query, Conn, Trans))
                        {
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception x)
                    {
                        Trans.Rollback();
                        MessageBox.Show(x.Message);
                        return;
                    }
                    Trans.Commit();
                    Conn.Close();
                    MessageBox.Show("Data " + txtProvinceId.Text + ", berhasil ditambahkan.");
                    Parent.RefreshGrid();
                    this.Close();
                }
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[Province] set ProvinceName='" + txtProvinceName.Text + "' where ProvinceId='" + txtProvinceId.Text + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message);
                    Trans.Rollback();
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + txtProvinceId.Text + ", berhasil diupdate.");
                Parent.RefreshGrid();
                this.Close();
            }
        }
    }
}
