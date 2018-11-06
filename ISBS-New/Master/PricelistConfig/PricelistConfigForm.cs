using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Master.PricelistConfig
{
    public partial class PricelistConfigForm : MetroFramework.Forms.MetroForm
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
        String RecId = null;
        String tmpSubGroup2ID=""; //hendry
        String tmpPriceType=""; //hendry
        Master.PricelistConfig.PricelistConfigInquiry Parent;

        //begin
        //created by : joshua
        //created date : 23 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void flag(String prmRecId, String mode)
        {
            RecId = prmRecId;
            Mode = mode;
        }

        public PricelistConfigForm()
        {
            InitializeComponent();
        }

        private void PricelistConfigForm_Load(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                ModeNew();
            }
            if (Mode == "Edit")
            {
                ModeEdit();
                RefreshGrid();
            }
            if (Mode == "BeforeEdit")
            {
                ModeBeforeEdit();
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            Query = "Select SubGroup2Id, SubGroup2Name, PriceType, CONVERT(DECIMAL(5,2),Factor) AS Factor From [dbo].[PricelistConfig] Where RecId = '" + RecId + "' ";

            Conn = ConnectionString.GetConnection();
            using (SqlCommand cmd = new SqlCommand(Query, Conn))
            {
                Dr = cmd.ExecuteReader();

                while (Dr.Read())
                {
                    txtSubGroup2ID.Text = Convert.ToString(Dr["SubGroup2Id"]);
                    txtSubGroup2.Text = Convert.ToString(Dr["SubGroup2Name"]);
                    cmbPriceType.Text = Convert.ToString(Dr["PriceType"]);
                    txtFactor.Text = Convert.ToString(Dr["Factor"]);
                    txtRecId.Text = RecId;                   
                }
            }
            Conn.Close();
        }

        private void ModeNew()
        {
            btnSearchSubGroup2.Enabled = true;     
            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = false;
            cmbPriceType.SelectedIndex = 0;
        }

        private bool Used()
        {
            Boolean vBol = false;
            try
            {
                Query = "Select * From Pricelist_Dtl Where Ref_Config_RecId ='" + txtRecId.Text + "'";
                Conn = ConnectionString.GetConnection();
                using (SqlCommand cmd = new SqlCommand(Query, Conn))
                {
                    Dr = cmd.ExecuteReader();
                    if (Dr.Read())
                    {
                        vBol = true;
                    }
                    else
                    {
                        vBol = false;
                    }
                    Dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                Conn.Close();
            }

            return vBol;
        }

        private void ModeEdit()
        {
            if (Used()) { MessageBox.Show("Tidak boleh Edit, Group Name sudah pernah digunakan..");  return; }
            btnSearchSubGroup2.Enabled = true;
            btnEdit.Visible = false;
            btnSave.Visible = true;
            btnCancel.Visible = true;

            cmbPriceType.Enabled = true;
            txtFactor.Enabled = true;
        }

        private void ModeBeforeEdit()
        {
            btnSearchSubGroup2.Enabled = false;
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnCancel.Visible = false;

            txtSubGroup2.Enabled = false;
            cmbPriceType.Enabled = false;
            txtFactor.Enabled = false;

        }

        public void ParentRefreshGrid(Master.PricelistConfig.PricelistConfigInquiry f)
        {
            Parent = f;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {  
            //begin
            //updated by : joshua
            //updated date : 25 apr 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                tmpSubGroup2ID = txtSubGroup2.Text; //hendry
                tmpPriceType = cmbPriceType.SelectedItem.ToString(); //hendry
                ModeEdit();
                Mode = "Edit";
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end  
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtSubGroup2.Enabled = false;
            cmbPriceType.Enabled = false;
            txtFactor.Enabled = false;    

            btnSearchSubGroup2.Enabled = false;
            btnEdit.Visible = true;
            btnSave.Visible = false;
            btnExit.Visible = true;
            btnCancel.Visible = false;

            RefreshGrid();
        }

        private int cekValidasi(String SubGroup2ID, String PriceType)
        {
            Query = "Select Count(SubGroup2Id) From [dbo].[PricelistConfig] Where SubGroup2Id = '" + SubGroup2ID + "' and PriceType = '" + PriceType + "'";
            //Hendry
            Query += " AND SubGroup2Id <> '" + tmpSubGroup2ID + "' and PriceType <> '" + tmpPriceType + "'";
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand(Query, Conn);
            int CounData = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            return CounData;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtSubGroup2.Text))
            {
                MessageBox.Show("Sub Group 2 harus diisi");
                return;
            }
            else if (String.IsNullOrEmpty(cmbPriceType.Text))
            {
                MessageBox.Show("Price Type harus diisi");
                return;
            }
            else if (String.IsNullOrEmpty(txtFactor.Text))
            {
                MessageBox.Show("Factor harus diisi");
                return;
            }
            else if (cekValidasi(txtSubGroup2ID.Text, cmbPriceType.Text) > 0)
            {
               
                MessageBox.Show("Data dengan\nSub Group 2 : " + txtSubGroup2.Text + "\nPrice Type : " + cmbPriceType.Text + "\nsudah ada di database.");
                return;                             
            }
           
            decimal Factor = 0;
            string PriceType = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT TOP 1  CONVERT(DECIMAL(5,2),Factor)  AS Factor, PriceType FROM PricelistConfig WHERE SubGroup2Id = '" + txtSubGroup2ID.Text + "' AND PriceType < '" + cmbPriceType.Text + "' ORDER BY PriceType DESC";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                Factor = Convert.ToDecimal(Dr[0]);
                PriceType = Convert.ToString(Dr[1]);
            }
            Dr.Close();
            Conn.Close();

            if (PriceType != "")
            {
                if (Convert.ToDecimal(txtFactor.Text) < Factor)
                {
                    MessageBox.Show("Factor harus lebih besar dari " + Factor + ".\nTelah ada data sebelumnya dengan Price Type : " + PriceType + " dan Factor : " + Factor);
                    return;
                }
            }

            Factor = 0;
            PriceType = "";
            Conn = ConnectionString.GetConnection();
            Query = "SELECT TOP 1 CONVERT(DECIMAL(5,2),Factor) AS Factor, PriceType FROM PricelistConfig WHERE SubGroup2Id = '" + txtSubGroup2ID.Text + "' AND PriceType > '" + cmbPriceType.Text + "' ORDER BY PriceType ASC";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                Factor = Convert.ToDecimal(Dr[0]);
                PriceType = Convert.ToString(Dr[1]);
            }
            Dr.Close();
            Conn.Close();

            if (PriceType != "")
            {
                if (Convert.ToDecimal(txtFactor.Text) > Factor)
                {
                    MessageBox.Show("Factor harus lebih kecil dari " + Factor + ".\nTelah ada data sebelumnya dengan Price Type : " + PriceType + " dan Factor : " + Factor);
                    return;
                }  
            }                            
            

            if (Mode == "New")
            {
               
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "insert into [dbo].[PricelistConfig] (SubGroup2Id, SubGroup2Name, PriceType, Factor, CreatedBy, CreatedDate) ";
                    Query += "values ('" + txtSubGroup2ID.Text + "', '" + txtSubGroup2.Text + "', '" + cmbPriceType.Text + "', '" + txtFactor.Text + "', '"+ControlMgr.UserId+"', getdate())";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    
                }
                catch (Exception x)
                {
                    Trans.Rollback();
                    MessageBox.Show(x.Message);
                    return;
                }
                Trans.Commit();
                Conn.Close();
                MessageBox.Show("Data dengan Sub Group 2 " + txtSubGroup2.Text + " dan Price Type " + cmbPriceType.Text + ", berhasil disimpan.");
                Parent.RefreshGrid();
                this.Close();
                
            }
            else if (Mode == "Edit")
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                try
                {
                    Query = "SELECT SubGroup2ID, SubGroup2Name, PriceType, Factor FROM PricelistConfig WHERE RecId='" + txtRecId.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();
                    while (Dr.Read())
                    {
                        string OldSubGroup2ID = Convert.ToString(Dr[0]);
                        string OldSubGroup2Name = Convert.ToString(Dr[1]);
                        string OldPriceType = Convert.ToString(Dr[2]);
                        string OldFactor = Convert.ToString(Dr[3]);

                        if (OldSubGroup2ID != txtSubGroup2ID.Text)
                        {
                            Query = "INSERT INTO PricelistConfig_LogTable (SubGroup2ID, SubGroup2Name, PriceType, Factor, LogDescription, UserId, LogDate) ";
                            Query += "VALUES('" + OldSubGroup2ID + "', '" + OldSubGroup2Name + "', '" + OldPriceType + "', '" + OldFactor + "', 'Sub Group 2 : " + OldSubGroup2ID + "-" + OldSubGroup2Name + " Changed to : " + txtSubGroup2ID.Text + " - " + txtSubGroup2.Text + "', '"+ControlMgr.UserId+"', GETDATE())";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }

                        if (OldPriceType != cmbPriceType.Text)
                        {
                            Query = "INSERT INTO PricelistConfig_LogTable (SubGroup2ID, SubGroup2Name, PriceType, Factor, LogDescription, UserId, LogDate) ";
                            Query += "VALUES('" + txtSubGroup2ID.Text + "', '" + txtSubGroup2.Text + "', '" + OldPriceType + "', '" + OldFactor + "', 'Days : " + OldPriceType + " Changed to : " + cmbPriceType.Text + "', '" + ControlMgr.UserId + "', GETDATE())";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }

                        if (OldFactor != txtFactor.Text)
                        {
                            Query = "INSERT INTO PricelistConfig_LogTable (SubGroup2ID, SubGroup2Name, PriceType, Factor, LogDescription, UserId, LogDate) ";
                            Query += "VALUES('" + txtSubGroup2ID.Text + "', '" + txtSubGroup2.Text + "', '" + cmbPriceType.Text + "', '" + OldFactor + "', 'Factor : " + OldFactor + " Changed to : " + txtFactor.Text + "', '" + ControlMgr.UserId + "', GETDATE())";
                            Cmd = new SqlCommand(Query, Conn, Trans);
                            Cmd.ExecuteNonQuery();
                        }
                    }
                    Dr.Close();

                    Query = "update [dbo].[PricelistConfig] set SubGroup2Name = '"+txtSubGroup2.Text+"', SubGroup2ID = '"+txtSubGroup2ID.Text+"', PriceType='" + cmbPriceType.Text + "', Factor='" + txtFactor.Text + "', UpdatedBy='" + ControlMgr.UserId + "', UpdatedDate=getdate() where RecId='" + txtRecId.Text + "';";
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
                MessageBox.Show("Data berhasil diedit.");
                Parent.RefreshGrid();
                this.Close();
            }  
        }
    
        private void PricelistConfigForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Parent.RefreshGrid();
        }

        private void btnSearchSubGroup2_Click(object sender, EventArgs e)
        {
            LookupSubGroup2ID tmpSearch = new LookupSubGroup2ID();
            tmpSearch.PrimaryKey = "SubGroup2ID";
            tmpSearch.Order = "SubGroup2ID Asc";
            tmpSearch.Table = "[dbo].[InventSubGroup2]";
            tmpSearch.QuerySearch = "SELECT a.SubGroup2ID, a.Deskripsi AS SubGroup2 FROM [dbo].[InventSubGroup2] a";
            tmpSearch.FilterText = new string[] { "SubGroup2ID", "SubGroup2" };
            tmpSearch.Select = new string[] { "SubGroup2ID", "SubGroup2" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtSubGroup2ID.Text = ConnectionString.Kodes[0];
                txtSubGroup2.Text = ConnectionString.Kodes[1];
                ConnectionString.Kodes = null;
            }
        }

        private void txtPriceType_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }
        private void txtFactor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtFactor_Leave(object sender, EventArgs e)
        {
            if (txtFactor.Text != "")
            {
                if (Convert.ToDouble(txtFactor.Text) > 100)
                {
                    txtFactor.Text = "100.00";

                    MessageBox.Show("Factor tidak boleh lebih dari 100");
                    return;
                }
                else
                {
                    txtFactor.Text = Convert.ToString(Convert.ToDecimal(txtFactor.Text).ToString("N2"));                
                }
            }
            else
            {
                txtFactor.Text = "0.00";                
            }
        }
    }
}
