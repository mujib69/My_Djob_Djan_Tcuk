using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Convertion
{
    public partial class FormConvertion : MetroFramework.Forms.MetroForm
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

        Master.Convertion.InquiryConvertion Parent;

        public FormConvertion()
        {
            InitializeComponent();
        }

        //Mode
        string Mode = "";

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void ModeNew()
        {
            Mode = "New";

            btnSearchItem.Enabled = true;

            txtFullItemId.Enabled = false;
            txtItemDeskripsi.Enabled = false;
            txtFromUnit.Enabled = true;
            txtToUnit.Enabled = true;
            txtRatio.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSearchItem.Enabled = true;

            txtFullItemId.Enabled = false;
            txtItemDeskripsi.Enabled = false;
            txtFromUnit.Enabled = true;
            txtToUnit.Enabled = true;
            txtRatio.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSearchItem.Enabled = false;

            txtFullItemId.Enabled = false;
            txtItemDeskripsi.Enabled = false;
            txtFromUnit.Enabled = false;
            txtToUnit.Enabled = false;
            txtRatio.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            
            //begin
            //updated by : joshua
            //updated date : 23 feb 2018
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Cek Data apakah sudah ada
                string CekData;
                Conn = ConnectionString.GetConnection();

                //Validasi jika kosong
                if (txtFromUnit.Text == "")
                {
                    MessageBox.Show("Data From Unit tidak boleh kosong.");
                    return;
                }
                else if (txtFromUnit.Text == "")
                {
                    MessageBox.Show("Data To Unit tidak boleh kosong.");
                    return;
                }
                else if (txtRatio.Text == "")
                {
                    MessageBox.Show("Data Ratio tidak boleh kosong.");
                    return;
                }

                //Jika New
                if (Mode=="New")
                {
                    Query = "Insert into dbo.InventConversion (FullItemID, ItemDeskripsi, FromUnit, ToUnit, Ratio, CreatedDate, CreatedBy) OUTPUT INSERTED.FullItemID values ";
                    Query += "('" + txtFullItemId.Text.Trim().ToUpper() + "','" + txtItemDeskripsi.Text.Trim().ToUpper() + "','" + txtFromUnit.Text.Trim().ToUpper() + "','" + txtToUnit.Text.Trim().ToUpper() + "','" + txtRatio.Text.Trim().ToUpper() + "',getdate(),'" + Login.Username + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar().ToString();
                    }
                    Trans.Commit();
                    MessageBox.Show("FullItemID = " + txtFullItemId.Text.Trim().ToUpper() + Environment.NewLine + "ItemDeskripsi = " + txtItemDeskripsi.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventConversion set FullItemID='" + txtFullItemId.Text.Trim().ToUpper() + "', ItemDeskripsi='" + txtItemDeskripsi.Text.Trim().ToUpper() + "', FromUnit='" + txtFromUnit.Text.Trim().ToUpper() + "', ToUnit='" + txtToUnit.Text.Trim().ToUpper() + "', Ratio='" + txtRatio.Text.Trim().ToUpper() + "', UpdatedDate=getdate(), UpdatedBy='" + Login.Username + "' where FullItemID='" + txtFullItemId.Text + "'";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("FullItemID = " + txtFullItemId.Text.Trim().ToUpper() + Environment.NewLine + "Berhasil diupdate.");
                }
                Conn.Close();
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Conn.Close();
                this.Close();
            }  
        }

        public void SetParent(Master.Convertion.InquiryConvertion F)
        {
            Parent = F;
        }

        public void GetDataHeader(string FullItemID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select FullItemID, ItemDeskripsi, FromUnit, ToUnit, Ratio, CreatedDate, CreatedBy From [dbo].[InventConversion] where FullItemID='" + FullItemID + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtFullItemId.Text = Dr["FullItemID"].ToString();
                txtItemDeskripsi.Text = Dr["ItemDeskripsi"].ToString();
                txtFromUnit.Text = Dr["FromUnit"].ToString();
                txtToUnit.Text = Dr["ToUnit"].ToString();
                txtRatio.Text = Dr["Ratio"].ToString();
            }
            Dr.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchGroup_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventTable";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtFullItemId.Text = ConnectionString.Kode;
            txtItemDeskripsi.Text = ConnectionString.Kode2;
        }

        private void FormConvertion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        private void FormConvertion_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
            //lblForm.Location = new Point(16, 11);
        }

        private void FormConvertion_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            //lblForm.Location = new Point(16, 11);
        }

        private void txtRatio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtRatio_Layout(object sender, LayoutEventArgs e)
        {
            if (txtRatio.Text != "")
            {
                txtRatio.Text = string.Format("{0:#,##0.00}", double.Parse(txtRatio.Text));
            } 
        }
                        
    }
}
