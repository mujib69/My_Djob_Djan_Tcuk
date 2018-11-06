using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ISBS_New.Utility
{
    public partial class combobox : Form
    {
        public combobox()
        {
            InitializeComponent();
        }

        private void combobox_Load(object sender, EventArgs e)
        {
            Conn = new SqlConnection("Data Source=192.168.0.171;Initial Catalog=ISBS-New;Pwd = sql123;USER ID = sa");

            Cmd = new SqlCommand();

            StrSql = "SELECT CityName FROM [Master].City";
            Cmd.CommandText = StrSql;
            Cmd.Connection = Conn;

            Conn.Open();

            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                ((DataGridViewComboBoxColumn)dataGridView1.Columns["City"]).Items.Add(Dr["CityName"].ToString());
            }
            Conn.Close();
        }
    }
}
