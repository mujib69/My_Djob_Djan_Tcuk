using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New
{
    public partial class Filter : MetroFramework.Forms.MetroForm
    {
        /**********SQL*********/
        private SqlConnection Conn;
        private SqlCommand Cmd;
        //private SqlTransaction Trans;
        private SqlDataReader Dr;
        //private SqlDataAdapter Da;
        //private DataTable Dt;
        //private DataSet Ds;
        private string Query;
        /**********SQL*********/

        Label[] label = new Label[100];
        TextBox[] textbox = new TextBox[100];
        DateTimePicker[] dateTimePicker = new DateTimePicker[20];

        //method
        string SchemaName;
        string TableName;
        String WherePlus;

        public string where;
        public List<string> where2 = new List<string>();

        public Filter()
        {
            InitializeComponent();
        }

        public void SetSchemaTable(string Schema, string Table, string Where)
        {
            SchemaName = Schema;
            TableName = Table;
            WherePlus = Where;
        }

        private void Filter_Load(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "select FieldName, DisplayName from [user].[Table] where SchemaName = '" + SchemaName + "' and TableName = '" + TableName + "' and Select1 = 1";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            int j = 0;
            int pos = 0;
            while (Dr.Read())
            {
                createLabel(i, pos, Dr[1].ToString()); //CREATE NEW LABEL TO THE FORM

                if (Dr[1].ToString().Contains("Date") || Dr[1].ToString().Contains("ValidTo"))
                {
                    createDateTimePicker(i, pos, "from"); //CREATE NEW DATETIMEPICKER TO THE FORM
                    i++;
                    createLabel(i, pos, "to");
                    i++;
                    createDateTimePicker(i, pos, "to");
                }
                else
                    createTextBox(i, pos, getMaxLength(Dr["FieldName"].ToString())); //CREATE NEW TEXTBOX TO THE FORM
                pos += 26;
                i++;
            }
            Dr.Close();
            Conn.Close();
        }

        private int getMaxLength(string COLUMN_NAME)
        {
            //GET LENGTH
            //string data_type = "";
            int maxLength = 0;
            Query = "SELECT TABLE_CATALOG, TABLE_NAME, COLUMN_NAME, data_type, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION FROM information_schema.columns where TABLE_NAME = '" + TableName + "' and COLUMN_NAME = '" + COLUMN_NAME + "'";
            Cmd = new SqlCommand(Query, Conn);
            SqlDataReader Dr2 = Cmd.ExecuteReader();
            while (Dr2.Read())
            {
                //data_type = Dr2["data_type"].ToString();
                if (Dr2["CHARACTER_MAXIMUM_LENGTH"].ToString() != String.Empty || Dr2["NUMERIC_PRECISION"].ToString() != String.Empty)
                {
                    if (Dr2["data_type"].ToString() == "varchar" || Dr2["data_type"].ToString() == "nvarchar" || Dr2["data_type"].ToString() == "varbinary" || Dr2["data_type"].ToString() == "text")
                        maxLength = Convert.ToInt32(Dr2["CHARACTER_MAXIMUM_LENGTH"]);
                    else
                        maxLength = Convert.ToInt32(Dr2["NUMERIC_PRECISION"]);
                }
            }
            Dr2.Close();
            return maxLength;
        }

        private void createLabel(int i, int pos, string text)
        {
            label[i] = new Label();
            label[i].Name = "label" + i;
            label[i].Text = text; //#2 String.Format("Label {0}", "AAAAA");
            if (text == "to")
            {
                label[i].Location = new Point(365, 23 + pos);
                label[i].Size = new Size(16, 13);
            }
            else
                label[i].Location = new Point(20, 26 + pos);//#2 label[i].Left = 500 + pos; label[i].Top = 460;
            this.panel1.Controls.Add(label[i]);
        }

        private void createDateTimePicker(int i, int pos, string check)
        {

            dateTimePicker[i] = new DateTimePicker();
            dateTimePicker[i].CustomFormat = "dd/MM/yyyy";
            dateTimePicker[i].Format = DateTimePickerFormat.Custom;
            if (check == "from")
            {
                dateTimePicker[i].Location = new Point(200, 19 + pos);
                dateTimePicker[i].Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, 0, 0, 0, 0);
            }
            else if (check == "to")
            {
                dateTimePicker[i].Location = new Point(400, 19 + pos);
            }
            dateTimePicker[i].Size = new Size(150, 20);
            this.panel1.Controls.Add(dateTimePicker[i]);
        }

        private void createTextBox(int i, int pos, int maxLength)
        {
            textbox[i] = new TextBox();
            textbox[i].Name = "textbox" + i;
            textbox[i].MaxLength = maxLength;
            textbox[i].Width = Convert.ToInt32(maxLength * 14);
            textbox[i].Location = new Point(200, 19 + pos);
            if (textbox[i].Width > 150)
                textbox[i].Width = 150;
            this.panel1.Controls.Add(textbox[i]);
            textbox[i].MouseDown += new MouseEventHandler(a);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Conn = ConnectionString.GetConnection();
            Query = "select FieldName, DisplayName from [user].[Table] where SchemaName = '" + SchemaName + "' and TableName = '" + TableName + "' and Select1 = 1";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            int i = 0;
            where = "";
            while (Dr.Read())
            {
                if (i == 0)
                    where += " where (";//where += " and (";
                //else
                //    where += " or ";
                if (Dr[1].ToString().Contains("Date") || Dr[1].ToString().Contains("ValidTo"))
                {
                    if (i != 0 && where.LastIndexOf("or") != where.Length - 3 && where.LastIndexOf("(") != where.Length - 1)
                        where += " or ";
                    where += Dr["FieldName"] + " between '" + ConvertDateToSqlDate(dateTimePicker[i].Value) + " 00:00:00' and ";
                    where2.Add(ConvertDateToSqlDate(dateTimePicker[i].Value));
                    i += 2;
                    where += "'" + ConvertDateToSqlDate(dateTimePicker[i].Value) + " 23:59:59'";
                    where2.Add(ConvertDateToSqlDate(dateTimePicker[i].Value));
                }
                else
                {
                    if (textbox[i].Text != String.Empty)
                    {
                        if (i != 0 && where.LastIndexOf("or") != where.Length - 3 && where.LastIndexOf("(") != where.Length - 1)
                            where += " or ";
                        where += Dr["FieldName"] + " LIKE '%" + textbox[i].Text + "%'";
                        where2.Add(textbox[i].Text);
                    }
                }
                i++;
            }
            where += ")";
            Conn.Close();
            this.Close();
        }

        private string ConvertDateToSqlDate(DateTime date)
        {
            string day = date.ToShortDateString().Split('/')[1];
            string month = date.ToShortDateString().Split('/')[0];
            string year = date.ToShortDateString().Split('/')[2];

            return year + "-" + month + "-" + day;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void a(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            //{
            //    Control c = (sender as Control);
            //    SearchV2 f = new SearchV2();
            //    f.SetMode("No");
            //    f.SetSchemaTable(SchemaName,TableName, WherePlus);
            //    f.ShowDialog();
            //    if (SearchV2.data.Count != 0)
            //        c.Text = SearchV2.data[0];
            //    else
            //        c.Text = "";
            //}
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
            }
        }
    }
}
