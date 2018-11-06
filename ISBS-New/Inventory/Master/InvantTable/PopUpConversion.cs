using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.Master.InvantTable
{
    public partial class PopUpConversion : MetroFramework.Forms.MetroForm
    {
        //SQL Function
        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;
        public string ResizeType;

        public string FullItemId = "";
        public PopUpConversion()
        {
            InitializeComponent();
        }

        private void PopUpConversion_Load(object sender, EventArgs e)
        {
            GetDataHeader();
        }

        public void GetDataHeader()
        {

            dgvConversion.Rows.Clear();
            if (dgvConversion.RowCount - 1 <= 0)
            {
                dgvConversion.ColumnCount = 4;
                dgvConversion.Columns[0].Name = "No";
                dgvConversion.Columns[1].Name = "FromUnit";
                dgvConversion.Columns[2].Name = "ToUnit";
                dgvConversion.Columns[3].Name = "Ratio";
            }

            Conn = ConnectionString.GetConnection();
            Query = "Select [FullItemID],[ItemDeskripsi],[FromUnit],[ToUnit],[Ratio]";
            Query += " From [InventConversion] Where FullItemId='" + FullItemId + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtFullItemId.Text = Dr["FullItemID"].ToString();
                txtItemDesc.Text = Dr["ItemDeskripsi"].ToString();
                this.dgvConversion.Rows.Add((dgvConversion.Rows.Count + 1).ToString(), Dr["FromUnit"], Dr["ToUnit"], Dr["Ratio"]);
                dgvConversion.AutoResizeColumns();
                dgvConversion.ReadOnly = false;
                dgvConversion.Columns["No"].ReadOnly = true;
                dgvConversion.Columns["FromUnit"].ReadOnly = true;
                dgvConversion.Columns["ToUnit"].ReadOnly = true;
            }
            Dr.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();
                Query = "";
                for (int i = 0; i < dgvConversion.RowCount ; i++)
                {
                    Query += "Update [InventConversion] set [Ratio]='" + dgvConversion.Rows[i].Cells["Ratio"].Value.ToString() + "' where [FullItemID]='" + txtFullItemId.Text.Trim() + "' and [ItemDeskripsi]='" + txtItemDesc.Text.Trim() + "' and [FromUnit] = '" + dgvConversion.Rows[i].Cells["FromUnit"].Value.ToString() + "' and [ToUnit] = '" + dgvConversion.Rows[i].Cells["ToUnit"].Value.ToString() + "';";
                    if (i % 10 == 0)
                    {
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();
                        Query = "";
                    }
                }
                Cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil FullItemId = '" + txtFullItemId.Text.Trim() + "' diupdate.");
            }
            catch(Exception ex)
            { 
                Trans.Rollback();
                MessageBox.Show(ex.Message);
                //MessageBox("Terjadi Error : '" + ex.ToString() + "'");
                return;
            }
            finally
            {
                Trans.Commit();
                Conn.Close();
            }
        }

        private void dgvConversion_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == dgvConversion.Columns["Ratio"].Index)
            {
                double d = double.Parse(e.Value.ToString());
                e.Value = d.ToString("N2");
            }
        }

        private void dgvConversion_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control.AccessibilityObject.Role.ToString() != "ComboBox")
            {
                DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
                tb.KeyPress += new KeyPressEventHandler(dgvConversion_KeyPress);

                e.Control.KeyPress += new KeyPressEventHandler(dgvConversion_KeyPress);
            }
        }

        private void dgvConversion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dgvConversion.Columns[dgvConversion.CurrentCell.ColumnIndex].Name == "Ratio")
            {
           
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
