using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.City
{

    public partial class City : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        String Mode, Query;
        int Limit1, Limit2, Index, Total, Page1, Page2;
        int flagRefresh = 0;
        String CityId;

        //begin
        //created by : joshua
        //created date : 26 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public City()
        {
            InitializeComponent();
        }

        public void flag(String Id ,String mode)
        {
            Mode = mode;
            CityId = Id;
        }

        private void City_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private bool cekValidasi(String CityId)
        {
            Query = "Select * From [dbo].[City] Where CityId = '" + CityId + "'";

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
            Query = "Select * From [dbo].[City] c JOIN [dbo].[Province] p ON c.ProvinceId = p.ProvinceId Where CityId = '" + CityId + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtProvinceId.Text = Dr["ProvinceId"].ToString(); ;
                    txtProvinceName.Text = Dr["ProvinceName"].ToString();
                    txtCityId.Text = Dr["CityId"].ToString();
                    txtCityName.Text = Dr["CityName"].ToString();
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
            txtCityId.Text = "";
            txtCityName.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtProvinceId.Enabled = false;
            txtProvinceName.Enabled = false;
            txtCityId.Enabled = true;
            txtCityName.Enabled = true;

            btnSearchProvince.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtProvinceId.Enabled = false;
            txtProvinceName.Enabled = false;
            txtCityId.Enabled = false;
            txtCityName.Enabled = true;

            btnSearchProvince.Enabled = false;
            btnSave.Visible = true;
            btnEdit.Visible = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 26 feb 2018
            //description : check permission access
            if (this.PermissionAccess(Login.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(Login.PermissionDenied);
            }
            //end             
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                if (String.IsNullOrEmpty(txtProvinceId.Text) || String.IsNullOrEmpty(txtCityId.Text) || String.IsNullOrEmpty(txtCityName.Text))
                {
                    MessageBox.Show("Data harus diisi");
                    return;
                }

                if (cekValidasi(txtCityId.Text) == false)
                {
                    MessageBox.Show("Data sudah ada di database.");
                }
                else
                {
                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    try
                    {
                        Query = "insert into [dbo].[City] (CityId, CityName, ProvinceId) ";
                        Query += "values ('" + txtCityId.Text + "', '" + txtCityName.Text + "', '" + txtProvinceId.Text + "');";

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
                    MessageBox.Show("Data " + txtCityId.Text + ", berhasil ditambahkan.");
                    this.Close();
                }
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[City] set CityName='" + txtCityName.Text + "' where CityId='" + txtCityId.Text + "';";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteScalar();
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data " + txtCityId.Text + ", berhasil diupdate.");
                this.Close();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchProvince_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "Province";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtProvinceId.Text = ConnectionString.Kode;
            txtProvinceName.Text = ConnectionString.Kode2;
        }
    }
}
