using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Item
{
    public partial class Item : Form
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
        String ItemId, ItemDeskripsi = null;

        public Item()
        {
            InitializeComponent();
        }

        private String GenerateId()
        {
            Conn = ConnectionString.GetConnection();
            String LastId = "";
            Query = "Select Top 1 (ItemId) From dbo.Item order by ItemId DESC";

            using (SqlCommand Cmd = new SqlCommand(Query, Conn))
            {
                LastId = Cmd.ExecuteScalar().ToString();
            }

            if (String.IsNullOrEmpty(LastId))
            {
                return "00001";
            }
            else
            {
                int temp;
                temp = Int32.Parse(LastId)+1;

                if (temp < 10)
                {
                    return "0000" + temp;
                }
                else if (temp < 100)
                {
                    return "000" + temp;
                }
                else if (temp < 1000)
                {
                    return "00" + temp;
                }
                else if (temp < 10000)
                {
                    return "0" + temp;
                }
                else
                {
                    return temp.ToString();
                }
            }
        }

        public void flag(String itemid, String itemdeskripsi, String mode)
        {
            ItemId = itemid;
            ItemDeskripsi = itemdeskripsi;
            Mode = mode;
        }

        private void Item_Load(object sender, EventArgs e)
        {
            ModeLoad();
            if (Mode == "New")
            {
                ModeNew();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select * From [dbo].[Item] Where ItemId = '" + ItemId + "' And ItemDeskripsi = '" + ItemDeskripsi + "'";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtItemId.Text = Dr["ItemId"].ToString();
                    txtItemDeskripsi.Text = Dr["ItemDeskripsi"].ToString();
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
            txtItemId.Text = "";
            txtItemDeskripsi.Text = "";
        }

        private void ModeNew()
        {
            resetText();

            txtItemId.Enabled = false;
            txtItemDeskripsi.Enabled = true;

            btnEdit.Visible = false;
            btnSave.Visible = true;
        }

        private void ModeEdit()
        {
            txtItemId.Enabled = false;
            txtItemDeskripsi.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            ModeEdit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                if (String.IsNullOrEmpty(txtItemDeskripsi.Text))
                {
                    MessageBox.Show("Item Deskripsi harus diisi.");
                }
                else
                {
                    String TempId = GenerateId();

                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    //int Counter = String.IsNullOrEmpty(txtCounter.Text) ? 1 : Int32.Parse(txtCounter.Text);

                    try
                    {
                        Query = "insert into [dbo].[Item] (ItemId, ItemDeskripsi) ";
                        Query += "values ('" + TempId + "', '" + txtItemDeskripsi.Text + "');";
                        
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
                    MessageBox.Show("Data " + TempId + " " + txtItemDeskripsi.Text + ", berhasil ditambahkan.");
                    this.Close();
                }
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "update [dbo].[Item] set ItemDeskripsi='" + txtItemDeskripsi.Text + "' where ItemId='" + txtItemId.Text + "';";
                    
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
                MessageBox.Show("Data " + txtItemId.Text + ", berhasil diupdate.");
                this.Close();
            }
        
        }

    }
}
