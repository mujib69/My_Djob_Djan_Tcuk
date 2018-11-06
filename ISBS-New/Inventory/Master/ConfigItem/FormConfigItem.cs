using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

using System.Transactions;


namespace ISBS_New.Master.ConfigItem
{
    public partial class FormConfigItem : MetroFramework.Forms.MetroForm
    {
        string strSql = "";
        SqlConnection ConMaster;
        SqlDataReader dr;


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

        Boolean vOldChk1, vOldChk2, vOldChk3, vOldChk4, vOldChk5;
        string vOldUkuran1, vOldUkuran2, vOldUkuran3, vOldUkuran4, vOldUkuran5;
        string vOldSatuan1, vOldSatuan2, vOldSatuan3, vOldSatuan4, vOldSatuan5;

        Master.ConfigItem.InquiryConfigItem Parent;

        public FormConfigItem()
        {
            InitializeComponent();
        }

        //Mode
        string Mode = "";

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        private void FormConfigItem_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
            AddCmbType();
            AddCmbMeasurement();
        }

        public void ModeNew()
        {
            Mode = "New";

            btnSearchGroup.Enabled = true;
            btnSearchSubGroup1.Enabled = true;
            btnSearchSubGroup2.Enabled = true;

            chkUkuran1.Enabled = true;
            chkUkuran2.Enabled = true;
            chkUkuran3.Enabled = true;
            chkUkuran4.Enabled = true;
            chkUkuran5.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            btnSearchGroup.Enabled = false;
            btnSearchSubGroup1.Enabled = false;
            btnSearchSubGroup2.Enabled = false;

            chkUkuran1.Enabled = true;
            chkUkuran2.Enabled = true;
            chkUkuran3.Enabled = true;
            chkUkuran4.Enabled = true;
            chkUkuran5.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;

            CheckUkuran();
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            btnSearchGroup.Enabled = false;
            btnSearchSubGroup1.Enabled = false;
            btnSearchSubGroup2.Enabled = false;

            chkUkuran1.Enabled = false;
            chkUkuran2.Enabled = false;
            chkUkuran3.Enabled = false;
            chkUkuran4.Enabled = false;
            chkUkuran5.Enabled = false;

            cmbUkuranName1.Enabled = false;
            cmbUkuranName2.Enabled = false;
            cmbUkuranName3.Enabled = false;
            cmbUkuranName4.Enabled = false;
            cmbUkuranName5.Enabled = false;

            cmbSatuan1.Enabled = false;
            cmbSatuan2.Enabled = false;
            cmbSatuan3.Enabled = false;
            cmbSatuan4.Enabled = false;
            cmbSatuan5.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;

        }

        private void btnSearchGroup_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventGroup";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtGroupId.Text = ConnectionString.Kode;
            txtSubGroup1Id.Text = "";
            txtSubGroup2Id.Text = "";
        }

        private void btnSearchSubGroup1_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSubGroup1";
            string SWhere = " AND GROUPID='" + txtGroupId.Text + "'";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, SWhere);
            tmpSearch.ShowDialog();
            txtSubGroup1Id.Text = ConnectionString.Kode2;
            txtSubGroup2Id.Text = "";
        }

        private void AmbilDetailGroup()
        {
            try
            {
                ConMaster = ConnectionString.GetConnection();
                strSql = "SELECT G.Deskripsi AS GroupName,SG.Deskripsi AS SubGroupName,SG2.Deskripsi AS SubGroup2Name ";
                strSql += "FROM InventSubGroup2 SG2 ";
                strSql += "LEFT JOIN InventSubGroup1 SG ON SG.SubGroup1ID=SG2.SubGroup1ID ";
                strSql += "LEFT JOIN InventGroup G ON G.GroupID=SG2.GroupID ";
                strSql += "WHERE SG2.SubGroup2ID='" + txtSubGroup2Id.Text + "'";
                using (SqlCommand cmd = new SqlCommand(strSql, ConMaster))
                {
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {                            
                            txtGroupName.Text = Convert.IsDBNull(dr["GroupName"]) ? "" : (string)dr["GroupName"];                         
                            txtSubGroupName.Text = Convert.IsDBNull(dr["SubGroupName"]) ? "" : (string)dr["SubGroupName"];
                            txtSubGroup2Name.Text = Convert.IsDBNull(dr["SubGroup2Name"]) ? "" : (string)dr["SubGroup2Name"];                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                dr.Close();
                ConMaster.Close();
            }
        }

        private void btnSearchGroup2_Click(object sender, EventArgs e)
        {
            string SchemaName = "dbo";
            string TableName = "InventSubGroup2";
            string SWhere = ""; //AND GROUPID='" + txtGroupId.Text + "' AND SUBGROUP1ID='" + txtSubGroup1Id.Text + "'";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName, SWhere);
            tmpSearch.ShowDialog();
            txtGroupId.Text = ConnectionString.Kode;
            txtSubGroup1Id.Text = ConnectionString.Kode2;
            txtSubGroup2Id.Text = ConnectionString.Kode3;

            if(ConnectionString.Kode!="")
            {
                AmbilDetailGroup();
            }
            else
            {
                txtGroupName.Text = "";
                txtSubGroupName.Text = "";
                txtSubGroup2Name.Text = "";
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end      
          
            vOldChk1=chkUkuran1.Checked; 
            vOldChk2=chkUkuran2.Checked;
            vOldChk3=chkUkuran3.Checked;
            vOldChk4=chkUkuran4.Checked;
            vOldChk5=chkUkuran5.Checked;
            //Convert.IsDBNull(Cmb_Purpose.SelectedItem) ? "" : (string)Cmb_Purpose.SelectedItem;
            vOldUkuran1=cmbUkuranName1.SelectedItem.ToString();
            vOldUkuran2=cmbUkuranName2.SelectedItem.ToString();
            vOldUkuran3 = cmbUkuranName3.SelectedItem.ToString();
            vOldUkuran4 = cmbUkuranName4.SelectedItem.ToString();
            vOldUkuran5 = cmbUkuranName5.SelectedItem.ToString();

            vOldSatuan1 = cmbSatuan1.SelectedItem.ToString();
            vOldSatuan2 = cmbSatuan2.SelectedItem.ToString();
            vOldSatuan3 = cmbSatuan3.SelectedItem.ToString();
            vOldSatuan4 = cmbSatuan4.SelectedItem.ToString();
            vOldSatuan5 = cmbSatuan5.SelectedItem.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            chkUkuran1.Checked = vOldChk1;
            chkUkuran2.Checked = vOldChk2;
            chkUkuran3.Checked = vOldChk3;
            chkUkuran4.Checked = vOldChk4;
            chkUkuran5.Checked = vOldChk5;

            cmbUkuranName1.SelectedItem = vOldUkuran1;
            cmbUkuranName2.SelectedItem = vOldUkuran2;
            cmbUkuranName3.SelectedItem = vOldUkuran3;
            cmbUkuranName4.SelectedItem = vOldUkuran4;
            cmbUkuranName5.SelectedItem = vOldUkuran5;

            cmbSatuan1.SelectedItem = vOldSatuan1;
            cmbSatuan2.SelectedItem = vOldSatuan2;
            cmbSatuan3.SelectedItem = vOldSatuan3;
            cmbSatuan4.SelectedItem = vOldSatuan4;
            cmbSatuan5.SelectedItem = vOldSatuan5;

            ModeBeforeEdit();
        }

        private Boolean ValidDetail()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtGroupId.Text == "")
            {
                ErrMsg = "Group Item tidak boleh kosong..";
                vBol=false;
            }
            else if (txtSubGroup1Id.Text == "")
            {
                ErrMsg = "Sub Group Item tidak boleh kosong..";
                vBol = false;
            }
            else if (txtSubGroup2Id.Text == "")
            {
                ErrMsg = "Sub Group 2 Item tidak boleh kosong..";
                vBol = false;
            }

            if (vBol == true)
            {
                if (chkUkuran1.Checked == true && (cmbUkuranName1.SelectedItem.ToString() == "" || cmbSatuan1.SelectedItem.ToString() == ""))
                {
                    MessageBox.Show("Ukuran dan Nama 1 harus dipilih.. ");
                    vBol = false;
                }
                else if (chkUkuran2.Checked == true && (cmbUkuranName2.SelectedItem.ToString() == "" || cmbSatuan2.SelectedItem.ToString() == ""))
                {
                    MessageBox.Show("Ukuran dan Nama 2 harus dipilih.. ");
                    vBol = false;
                }
                else if (chkUkuran3.Checked == true && (cmbUkuranName3.SelectedItem.ToString() == "" || cmbSatuan3.SelectedItem.ToString() == ""))
                {
                    MessageBox.Show("Ukuran dan Nama 3 harus dipilih.. ");
                    vBol = false;
                }
                else if (chkUkuran4.Checked == true && (cmbUkuranName4.SelectedItem.ToString() == "" || cmbSatuan4.SelectedItem.ToString() == ""))
                {
                    MessageBox.Show("Ukuran dan Nama 4 harus dipilih.. ");
                    vBol = false;
                }
                else if (chkUkuran5.Checked == true && (cmbUkuranName5.SelectedItem.ToString() == "" || cmbSatuan5.SelectedItem.ToString() == ""))
                {
                    MessageBox.Show("Ukuran dan Nama 5 harus dipilih.. ");
                    vBol = false;
                }
            }

            if (vBol == true && Mode == "New")
            {
                try
                {
                    Query = "Select * From InventConfig Where SubGroup2ID=@txtSubGroup2Id ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtSubGroup2Id", txtSubGroup2Id.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Item Config sudah ada..";
                            vBol = false;
                        }
                        else
                        {
                            vBol = true;
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
            }

            if (vBol == true && Mode == "Edit")
            {
                try
                {
                    Query = "Select * From InventConfig Where SubGroup2ID=@txtSubGroup2Id ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtSubGroup2Id", txtSubGroup2Id.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Config Item tidak ditemukan..";
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
            }

            if (vBol == false) { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidDetail() == false)
            {
                return;
            }
            //Hendry

            string UkuranName1 = chkUkuran1.Checked == false ? "" : cmbUkuranName1.Text.ToString();
            string UkuranName2 = chkUkuran2.Checked == false ? "" : cmbUkuranName2.Text.ToString();
            string UkuranName3 = chkUkuran3.Checked == false ? "" : cmbUkuranName3.Text.ToString();
            string UkuranName4 = chkUkuran4.Checked == false ? "" : cmbUkuranName4.Text.ToString();
            string UkuranName5 = chkUkuran5.Checked == false ? "" : cmbUkuranName5.Text.ToString();

            string Satuan1 = chkUkuran1.Checked == false ? "" : cmbSatuan1.Text.ToString();
            string Satuan2 = chkUkuran2.Checked == false ? "" : cmbSatuan2.Text.ToString();
            string Satuan3 = chkUkuran3.Checked == false ? "" : cmbSatuan3.Text.ToString();
            string Satuan4 = chkUkuran4.Checked == false ? "" : cmbSatuan4.Text.ToString();
            string Satuan5 = chkUkuran5.Checked == false ? "" : cmbSatuan5.Text.ToString();
            string Msg="";

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ConMaster = ConnectionString.GetConnection();
                    using (ConMaster)
                    {
                        string strSql;
                         
                        if (Mode == "New")
                        {
                            strSql = "Insert into dbo.InventConfig (GroupId,GroupDesc, SubGroup1Id,SubGroup1Desc, SubGroup2Id,SubGroup2Desc, Ukuran1, DimensionID1, Measurement1, Ukuran2, DimensionID2, Measurement2, Ukuran3, DimensionID3, Measurement3, Ukuran4, DimensionID4, Measurement4, Ukuran5, DimensionID5, Measurement5,CreatedBy) values (@txtGroupId,@txtGroupDesc,@txtSubGroup1Id,@txtSubGroup1Desc,@txtSubGroup2Id,@txtSubGroup2Desc,'" + chkUkuran1.Checked.ToString() + "','" + UkuranName1 + "','" + Satuan1 + "','" + chkUkuran2.Checked.ToString() + "','" + UkuranName2 + "','" + Satuan2 + "','" + chkUkuran3.Checked.ToString() + "','" + UkuranName3 + "','" + Satuan3 + "','" + chkUkuran4.Checked.ToString() + "','" + UkuranName4 + "','" + Satuan4 + "','" + chkUkuran5.Checked.ToString() + "','" + UkuranName5 + "','" + Satuan5 + "','" + ControlMgr.UserId + "')";                                            
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {                            
                                cmdCustomer.Parameters.AddWithValue("@txtGroupId", txtGroupId.Text);
                                cmdCustomer.Parameters.AddWithValue("@txtGroupDesc", txtGroupName.Text);   
                                cmdCustomer.Parameters.AddWithValue("@txtSubGroup1Id", txtSubGroup1Id.Text);
                                cmdCustomer.Parameters.AddWithValue("@txtSubGroup1Desc", txtSubGroupName.Text);           
                                cmdCustomer.Parameters.AddWithValue("@txtSubGroup2Id", txtSubGroup2Id.Text);
                                cmdCustomer.Parameters.AddWithValue("@txtSubGroup2Desc", txtSubGroup2Name.Text);
                                cmdCustomer.ExecuteNonQuery();                            
                                Msg="GroupId = " + txtGroupId.Text.Trim() + Environment.NewLine + "SubGroup1Id = " + txtSubGroup1Id.Text.Trim() + Environment.NewLine + "SubGroup2Id = " + txtSubGroup2Id.Text.Trim() + Environment.NewLine + " berhasil ditambahkan..";
                            }
                        }

                        if(Mode=="Edit") 
                        {
                            strSql = "Update dbo.InventConfig set Ukuran1='" + chkUkuran1.Checked.ToString() + "', DimensionID1='" + UkuranName1 + "', Measurement1='" + Satuan1 + "', Ukuran2='" + chkUkuran2.Checked.ToString() + "', DimensionID2='" + UkuranName2 + "', Measurement2='" + Satuan2 + "', Ukuran3='" + chkUkuran3.Checked.ToString() + "', DimensionID3='" + UkuranName3 + "', Measurement3='" + Satuan3 + "', Ukuran4='" + chkUkuran4.Checked.ToString() + "', DimensionID4='" + UkuranName4 + "', Measurement4='" + Satuan4 + "', Ukuran5='" + chkUkuran5.Checked.ToString() + "', DimensionID5='" + UkuranName5 + "', Measurement5='" + Satuan5 + "',UpdatedBy='" + ControlMgr.UserId + "',updatedDate=GETDATE() where GroupId='" + txtGroupId.Text + "' and SubGroup1ID='" + txtSubGroup1Id.Text + "' and SubGroup2ID='" + txtSubGroup2Id.Text + "'";                    
                            using (SqlCommand cmdCustomer = new SqlCommand(strSql, ConMaster))
                            {
                                cmdCustomer.ExecuteNonQuery();                          
                                Msg="GroupId = " + txtGroupId.Text.Trim() + Environment.NewLine + "SubGroup1Id = " + txtSubGroup1Id.Text.Trim() + Environment.NewLine + "SubGroup2Id = " + txtSubGroup2Id.Text.Trim() + Environment.NewLine + " berhasil diedit..";
                            }
                        }
                    }
                    scope.Complete();
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
            finally
            {
                ConMaster.Close();
                
                MessageBox.Show(Msg);

                Form parentform = Application.OpenForms["InquiryConfigItem"];
                if (parentform != null)
                    Parent.RefreshGrid();
                this.Close();
            }
            //Hendry End
        }

        public void SetParent(Master.ConfigItem.InquiryConfigItem F)
        {
            Parent = F;
        }

        public void GetDataHeader(string ConfigItemId)
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select GroupId, SubGroup1Id, SubGroup2Id, Ukuran1, DimensionID1, Measurement1, Ukuran2, DimensionID2, Measurement2, Ukuran3, DimensionID3, Measurement3, Ukuran4, DimensionID4, Measurement4, Ukuran5, DimensionID5, Measurement5 From [dbo].[InventConfig] where GroupId+SubGroup1ID+SubGroup2ID = '" + ConfigItemId + "'";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtGroupId.Text = Dr["GroupId"].ToString();
                txtSubGroup1Id.Text = Dr["SubGroup1Id"].ToString();
                txtSubGroup2Id.Text = Dr["SubGroup2Id"].ToString();
                chkUkuran1.Checked = Dr["Ukuran1"] == null ? false : Convert.ToBoolean(Dr["Ukuran1"]);
                cmbUkuranName1.SelectedItem = Dr["DimensionID1"].ToString();
                cmbSatuan1.SelectedItem = Dr["Measurement1"].ToString();
                chkUkuran2.Checked = Dr["Ukuran2"] == null ? false : Convert.ToBoolean(Dr["Ukuran2"]);
                cmbUkuranName2.SelectedItem = Dr["DimensionID2"].ToString();
                cmbSatuan2.SelectedItem = Dr["Measurement2"].ToString();
                chkUkuran3.Checked = Dr["Ukuran3"] == null ? false : Convert.ToBoolean(Dr["Ukuran3"]);
                cmbUkuranName3.SelectedItem = Dr["DimensionID3"].ToString();
                cmbSatuan3.SelectedItem = Dr["Measurement3"].ToString();
                chkUkuran4.Checked = Dr["Ukuran4"] == null ? false : Convert.ToBoolean(Dr["Ukuran4"]);
                cmbUkuranName4.SelectedItem = Dr["DimensionID4"].ToString();
                cmbSatuan4.SelectedItem = Dr["Measurement4"].ToString();
                chkUkuran5.Checked = Dr["Ukuran5"] == null ? false : Convert.ToBoolean(Dr["Ukuran5"]);
                cmbUkuranName5.SelectedItem = Dr["DimensionID5"].ToString();
                cmbSatuan5.SelectedItem = Dr["Measurement5"].ToString();
            }
            Dr.Close();

            AmbilDetailGroup();
        }

        private void AddCmbType()
        {
            //List<string> Dimension = new List<string>();
            cmbUkuranName1.Items.Add("");
            cmbUkuranName2.Items.Add("");
            cmbUkuranName3.Items.Add("");
            cmbUkuranName4.Items.Add("");
            cmbUkuranName5.Items.Add("");

            Conn = ConnectionString.GetConnection();
            Query = "Select DimensionId From [InventDimension]";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbUkuranName1.Items.Add(Dr[0].ToString().ToUpper());
                cmbUkuranName2.Items.Add(Dr[0].ToString().ToUpper());
                cmbUkuranName3.Items.Add(Dr[0].ToString().ToUpper());
                cmbUkuranName4.Items.Add(Dr[0].ToString().ToUpper());
                cmbUkuranName5.Items.Add(Dr[0].ToString().ToUpper());
            }

            cmbUkuranName1.SelectedIndex = 0;
            cmbUkuranName2.SelectedIndex = 0;
            cmbUkuranName3.SelectedIndex = 0;
            cmbUkuranName4.SelectedIndex = 0;
            cmbUkuranName5.SelectedIndex = 0;

            cmbUkuranName1.Enabled = false;
            cmbUkuranName2.Enabled = false;
            cmbUkuranName3.Enabled = false;
            cmbUkuranName4.Enabled = false;
            cmbUkuranName5.Enabled = false;

            Conn.Close();
        }

        private void AddCmbMeasurement()
        {
            //List<string> Dimension = new List<string>();
            cmbSatuan1.Items.Add("");
            cmbSatuan2.Items.Add("");
            cmbSatuan3.Items.Add("");
            cmbSatuan4.Items.Add("");
            cmbSatuan5.Items.Add("");

            Conn = ConnectionString.GetConnection();
            Query = "Select MeasurementId From [dbo].[InventMeasurement]";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                cmbSatuan1.Items.Add(Dr[0].ToString());
                cmbSatuan2.Items.Add(Dr[0].ToString());
                cmbSatuan3.Items.Add(Dr[0].ToString());
                cmbSatuan4.Items.Add(Dr[0].ToString());
                cmbSatuan5.Items.Add(Dr[0].ToString());
            }

            cmbSatuan1.SelectedIndex = 0;
            cmbSatuan2.SelectedIndex = 0;
            cmbSatuan3.SelectedIndex = 0;
            cmbSatuan4.SelectedIndex = 0;
            cmbSatuan5.SelectedIndex = 0;

            cmbSatuan1.Enabled = false;
            cmbSatuan2.Enabled = false;
            cmbSatuan3.Enabled = false;
            cmbSatuan4.Enabled = false;
            cmbSatuan5.Enabled = false;

            Conn.Close();
        }

        private void chkUkuran1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUkuran1.Checked == true)
            { 
                cmbUkuranName1.Enabled = true;
                cmbSatuan1.Enabled = true;
            }
            else
            {
                cmbUkuranName1.Enabled = false;
                cmbSatuan1.Enabled = false;
            }
        }

        private void chkUkuran2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUkuran2.Checked == true)
            {
                cmbUkuranName2.Enabled = true;
                cmbSatuan2.Enabled = true;
            }
            else
            {
                cmbUkuranName2.Enabled = false;
                cmbSatuan2.Enabled = false;
            }
        }

        private void chkUkuran3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUkuran3.Checked == true)
            {
                cmbUkuranName3.Enabled = true;
                cmbSatuan3.Enabled = true;
            }
            else
            {
                cmbUkuranName3.Enabled = false;
                cmbSatuan3.Enabled = false;
            }
        }

        private void chkUkuran4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUkuran4.Checked == true)
            {
                cmbUkuranName4.Enabled = true;
                cmbSatuan4.Enabled = true;
            }
            else
            {
                cmbUkuranName4.Enabled = false;
                cmbSatuan4.Enabled = false;
            }
        }

        private void chkUkuran5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUkuran5.Checked == true)
            {
                cmbUkuranName5.Enabled = true;
                cmbSatuan5.Enabled = true;
            }
            else
            {
                cmbUkuranName5.Enabled = false;
                cmbSatuan5.Enabled = false;
            }
        }
           
        private void CheckUkuran()
        {
            if (chkUkuran1.Checked == true)
            {
                cmbUkuranName1.Enabled = true;
                cmbSatuan1.Enabled = true;
            }
            else
            {
                cmbUkuranName1.Enabled = false;
                cmbSatuan1.Enabled = false;
            }
            if (chkUkuran2.Checked == true)
            {
                cmbUkuranName2.Enabled = true;
                cmbSatuan2.Enabled = true;
            }
            else
            {
                cmbUkuranName2.Enabled = false;
                cmbSatuan2.Enabled = false;
            }
            if (chkUkuran3.Checked == true)
            {
                cmbUkuranName3.Enabled = true;
                cmbSatuan3.Enabled = true;
            }
            else
            {
                cmbUkuranName3.Enabled = false;
                cmbSatuan3.Enabled = false;
            }
            if (chkUkuran4.Checked == true)
            {
                cmbUkuranName4.Enabled = true;
                cmbSatuan4.Enabled = true;
            }
            else
            {
                cmbUkuranName4.Enabled = false;
                cmbSatuan4.Enabled = false;
            }
            if (chkUkuran5.Checked == true)
            {
                cmbUkuranName5.Enabled = true;
                cmbSatuan5.Enabled = true;
            }
            else
            {
                cmbUkuranName5.Enabled = false;
                cmbSatuan5.Enabled = false;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
