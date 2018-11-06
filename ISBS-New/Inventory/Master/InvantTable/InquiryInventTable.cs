using System;
using System.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Inventory.Master.InvantTable
{
    public partial class InquiryInventTable : MetroFramework.Forms.MetroForm
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
        String crit;

        List<FormInventTable> ListFormInventTable = new List<FormInventTable>();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //timer autorefresh
        private void setTimer()
        {
            Timer timerRefresh = new Timer();
            timerRefresh.Interval = (10*1000);//milisecond
            timerRefresh.Tick += new EventHandler(timerRefresh_Tick);
            timerRefresh.Start();
        }

        private void InquiryInventTable_Load(object sender, EventArgs e)
        {
            addCmbCrit();
            ModeLoad();
            gvSetting();
            //lblForm.Location = new Point(16, 11);
            //setTimer();
        }

        private void gvSetting()
        {
            #region Header
            dgvConfigItem.Columns["No"].HeaderText = "No";
            dgvConfigItem.Columns["FullItemId"].HeaderText = "FullItemId";
            dgvConfigItem.Columns["GroupID"].HeaderText = "Group";
            dgvConfigItem.Columns["SubGroup1ID"].HeaderText = "Sub Group";
            dgvConfigItem.Columns["SubGroup2ID"].HeaderText = "Sub Group 2";
            dgvConfigItem.Columns["ItemDeskripsi"].HeaderText = "Item Name";
            dgvConfigItem.Columns["InventTypeID"].HeaderText = "Type";
            dgvConfigItem.Columns["ManufacturerID"].HeaderText = "Manufacturer";
            dgvConfigItem.Columns["MerekID"].HeaderText = "Merek";
            dgvConfigItem.Columns["GolonganID"].HeaderText = "Golongan";
            dgvConfigItem.Columns["KodeBerat"].HeaderText = "Kode Berat";
            dgvConfigItem.Columns["SpecID"].HeaderText = "Spec";
            dgvConfigItem.Columns["UoM"].HeaderText = "UoM";
            dgvConfigItem.Columns["UoMAlt"].HeaderText = "UoM Alt";
            dgvConfigItem.Columns["CreatedBy"].HeaderText = "Created By";
            dgvConfigItem.Columns["CreatedDate"].HeaderText = "Created Date";
            dgvConfigItem.Columns["UpdatedBy"].HeaderText = "Updated By";
            dgvConfigItem.Columns["UpdatedDate"].HeaderText = "Updated Date";
            #endregion Header


            #region DefaultFormat
            //dgvConfigItem.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            //dgvConfigItem.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvConfigItem.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            dgvConfigItem.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss";
            #endregion DefaultFormat

            #region Width
            dgvConfigItem.Columns["GroupID"].Width = 50;
            dgvConfigItem.Columns["SubGroup1ID"].Width = 50;
            dgvConfigItem.Columns["SubGroup2ID"].Width = 50;
            dgvConfigItem.Columns["InventTypeID"].Width = 50;
            dgvConfigItem.Columns["ManufacturerID"].Width = 50;
            dgvConfigItem.Columns["UoM"].Width = 50;
            dgvConfigItem.Columns["UoMAlt"].Width = 50;
            dgvConfigItem.Columns["CreatedDate"].Width = 150;
            dgvConfigItem.Columns["UpdatedDate"].Width = 150;
            #endregion Width
        }

        public InquiryInventTable()
        {
            InitializeComponent();
        }

        private void addCmbCrit()
        {
            cmbCriteria.Items.Add("All");
            Conn = ConnectionString.GetConnection();
            Query = "Select DisplayName From [User].[Table] Where SchemaName = 'dbo' And TableName = 'InventTableInquiry' order by [OrderNo]";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                cmbCriteria.Items.Add(Dr[0]);
            }
            Conn.Close();
            cmbCriteria.SelectedIndex = 0;            
        }

        public void RefreshGrid()
        {
            //Menampilkan data
            Conn = ConnectionString.GetConnection();
            if (cmbCriteria.Text == null)
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY FullItemID) No, FullItemID, GroupID,GroupDeskripsi, SubGroup1ID, SubGroup1Deskripsi, SubGroup2ID,SubGroup2Deskripsi,ItemDeskripsi,InventTypeID,ManufacturerID,MerekID,GolonganID,KodeBerat,SpecID,UoM,UoMAlt,CreatedDate,CreatedBy,UpdatedDate, UpdatedBy  From [dbo].[InventTable] ) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text =="All")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY FullItemID) No, FullItemID, GroupID,GroupDeskripsi, SubGroup1ID, SubGroup1Deskripsi, SubGroup2ID,SubGroup2Deskripsi,ItemDeskripsi,InventTypeID,ManufacturerID,MerekID,GolonganID,KodeBerat,SpecID,UoM,UoMAlt,CreatedDate,CreatedBy,UpdatedDate, UpdatedBy  From [dbo].[InventTable] Where ";
                Query += "FullItemID like @search or UoMQty like @search or GroupID like @search or GroupDeskripsi like @search or SubGroup1ID like @search or SubGroup1Deskripsi like @search or SubGroup2ID like @search or SubGroup2Deskripsi like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY FullItemID) No, FullItemID, GroupID,GroupDeskripsi, SubGroup1ID, SubGroup1Deskripsi, SubGroup2ID,SubGroup2Deskripsi,ItemDeskripsi,InventTypeID,ManufacturerID,MerekID,GolonganID,KodeBerat,SpecID,UoM,UoMAlt,CreatedDate,CreatedBy,UpdatedDate, UpdatedBy  From [dbo].[InventTable] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (FullItemID like @search or UoMQty like @search or GroupID like @search or GroupDeskripsi like @search or SubGroup1ID like @search or SubGroup1Deskripsi like @search or SubGroup2ID like @search or SubGroup2Deskripsi like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY FullItemID) No, FullItemID, GroupID,GroupDeskripsi, SubGroup1ID, SubGroup1Deskripsi, SubGroup2ID,SubGroup2Deskripsi,ItemDeskripsi,InventTypeID,ManufacturerID,MerekID,GolonganID,KodeBerat,SpecID,UoM,UoMAlt,CreatedDate,CreatedBy,UpdatedDate, UpdatedBy  From [dbo].[InventTable] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (FullItemID like @search or UoMQty like @search or GroupID like @search or GroupDeskripsi like @search or SubGroup1ID like @search or SubGroup1Deskripsi like @search or SubGroup2ID like @search or SubGroup2Deskripsi like @search)) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }
            else
            {
                Query = "Select FieldName From [User].[Table] where DisplayName = '"+cmbCriteria.Text+"'";
                Cmd = new SqlCommand(Query, Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select * From (Select ROW_NUMBER() OVER (ORDER BY FullItemID) No, FullItemID, GroupID,GroupDeskripsi, SubGroup1ID, SubGroup1Deskripsi, SubGroup2ID,SubGroup2Deskripsi,ItemDeskripsi,InventTypeID,ManufacturerID,MerekID,GolonganID,KodeBerat,SpecID,UoM,UoMAlt,CreatedDate,CreatedBy,UpdatedDate, UpdatedBy  From [dbo].[InventTable] Where ";
                //Query += cmbCriteria.Text + " Like @search) a ";
                Query += crit + " Like @search) a ";
                Query += "Where No Between " + Limit1 + " and " + Limit2 + " ;";
            }

            Da = new SqlDataAdapter(Query, Conn);
            Da.SelectCommand.Parameters.Add(new SqlParameter
            {
                ParameterName = "@search",
                Value = "%" + txtSearch.Text + "%",
                SqlDbType = SqlDbType.NVarChar,
                Size = 2000  // Assuming a 2000 char size of the field annotation (-1 for MAX)
            });
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvConfigItem.AutoGenerateColumns = true;
            dgvConfigItem.DataSource = Dt;
            dgvConfigItem.Refresh();
            dgvConfigItem.AutoResizeColumns();
            dgvSetting();
            Conn.Close();
            dgvPaging();
        }

        private void dgvPaging()
        {
            //Mengambil nilai total paging
            Conn = ConnectionString.GetConnection();

            if (cmbCriteria.Text == null)
            {
                Query = "Select Count(FullItemID) From [dbo].[InventTable];";
            }
            else if (cmbCriteria.Text == "All")
            {
                Query = "Select Count(FullItemID) From [dbo].[InventTable] Where ";
                Query += "FullItemID like @search or UoMQty like @search or GroupID like @search or GroupDeskripsi like @search or SubGroup1ID like @search or SubGroup1Deskripsi like @search or SubGroup2ID like @search or SubGroup2Deskripsi like @search";

            }
            else if (cmbCriteria.Text == "Created Date")
            {
                Query = "Select Count(FullItemID) From [dbo].[InventTable] Where ";
                Query += "(CONVERT(VARCHAR(10),CreatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),CreatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (FullItemID like @search or UoMQty like @search or GroupID like @search or GroupDeskripsi like @search or SubGroup1ID like @search or SubGroup1Deskripsi like @search or SubGroup2ID like @search or SubGroup2Deskripsi like @search)";
            }
            else if (cmbCriteria.Text == "Updated Date")
            {
                Query = "Select Count(FullItemID) From [dbo].[InventTable] Where ";
                Query += "(CONVERT(VARCHAR(10),UpdatedDate,120) >= '" + dtFrom.Value.Date.ToString("yyyy-MM-dd") + "' And CONVERT(VARCHAR(10),UpdatedDate,120) <= '" + dtTo.Value.Date.ToString("yyyy-MM-dd") + "') And (FullItemID like @search or UoMQty like @search or GroupID like @search or GroupDeskripsi like @search or SubGroup1ID like @search or SubGroup1Deskripsi like @search or SubGroup2ID like @search or SubGroup2Deskripsi like @search)";
            }
            else
            {
                Query = "Select FieldName From [User].[Table] where DisplayName = '" + cmbCriteria.Text + "'";
                Cmd.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
                Cmd = new SqlCommand(Query, Conn);
                string crit = Cmd.ExecuteScalar().ToString();

                Query = "Select Count(FullItemID) From [dbo].[InventTable] Where ";
                //Query += cmbCriteria.Text + " Like '%" + txtSearch.Text + "%'";
                Query += crit + " Like @search";

            }

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@search","%"+txtSearch.Text+"%");
            Total = Int32.Parse(Cmd.ExecuteScalar().ToString());
            Conn.Close();

            lblTotal.Text = "Total Rows : " + Total.ToString();
            Page2 = (int)Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text));
            lblPage.Text = "/ " + Page2;
        }

        private void dgvSetting()
        {
            dgvConfigItem.Columns["GroupID"].Visible = false;
            dgvConfigItem.Columns["SubGroup1ID"].Visible = false;
            dgvConfigItem.Columns["SubGroup2ID"].Visible = false;

            dgvConfigItem.Columns["CreatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
            dgvConfigItem.Columns["UpdatedDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";

            dgvConfigItem.Columns["GroupDeskripsi"].HeaderText = "Group";
            dgvConfigItem.Columns["SubGroup1Deskripsi"].HeaderText = "Sub Group 1";
            dgvConfigItem.Columns["SubGroup2Deskripsi"].HeaderText = "Sub Group 2";
        }      

        private void ModeLoad()
        {
            cmbShowLoad();
            Limit1 = 1;
            Limit2 = Int32.Parse(cmbShow.Text);
            Page1 = 1;
            txtPage.Text = "1";
            cmbCriteria.SelectedItem = "All";

            dtFrom.Value = DateTime.Today.Date;
            dtTo.Value = DateTime.Today.Date;

            RefreshGrid();
        }        

        private void cmbShowLoad()
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select CmbValue From [Setting].[CmbBox] ";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            cmbShow.Items.Clear();
            while (Dr.Read())
            {
                cmbShow.Items.Add(Dr.GetInt32(0));
            }
            Conn.Close();

            Conn = ConnectionString.GetConnection();
            SqlCommand Cmd1 = new SqlCommand(Query, Conn);
            Total = Int32.Parse(Cmd1.ExecuteScalar().ToString());
            Conn.Close();

            cmbShow.SelectedIndex = 0;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {             
            InventTableHeader ITH = new InventTableHeader();
            ITH.SetParent(this);           
            ITH.Show();
            ITH.ModeNew();

            //////begin
            //////updated by : joshua
            //////updated date : 24 feb 2018
            //////description : check permission access
            ////if (this.PermissionAccess(ControlMgr.New) > 0)
            ////{
            ////    FormInventTable InventTable = new FormInventTable();
            ////    ListFormInventTable.Add(InventTable);
            ////    InventTable.SetParent(this);
            ////    InventTable.ModeNew();
            ////    InventTable.Show();
            ////}
            ////else
            ////{
            ////    MessageBox.Show(ControlMgr.PermissionDenied);
            ////}
            //////end        
        }

        private void dgvPR_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                FormInventTable Type = new FormInventTable();
                Type.SetParent(this);
                Type.GetDataHeader(dgvConfigItem.CurrentRow.Cells["FullItemID"].Value.ToString());
                Type.Show();
                Type.ModeBeforeEdit();
                RefreshGrid();
            }
        }

        private Boolean ValidGeneral(string FullItemId)
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (vBol == true)
            {
                try
                {
                    Query = "Select * From [dbo].[InventTable] Where FullItemID='" + FullItemId + "'";
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
                            ErrMsg = "Item tidak ditemukan..";
                            vBol = false;
                            RefreshGrid();
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

            if (vBol == true)
            {
                string strSql = "SELECT FullItemId FROM Invent_Purchase_Qty WHERE FullItemId='" + FullItemId + "' AND ";
                strSql += "(PR_Issued_UoM<>0 OR PR_Approved_UoM<>0 OR PR_Approved2_UoM<>0 OR PR_CS_Issued_UoM<>0 OR PR_CS_Approved_UoM<>0 OR ";
                strSql += "PR_CS_Approved2_UoM<>0 OR PO_Issued_Outstanding_UoM<>0 OR PO_From_PA_Issued_UoM<>0 OR PO_From_PA_Approved_UoM<>0 OR ";
                strSql += "PO_From_PA_Approved2_UoM<>0 OR RO_Issued_UoM<>0 OR Retur_Beli_In_Progress_UoM<>0) ";
                strSql += "UNION ALL SELECT FullItemId FROM Invent_Sales_Qty WHERE FullItemId='" + FullItemId + "' AND ";
                strSql += "(SO_Preordered_UoM<>0 OR SO_From_SA_Issued_UoM<>0 OR SO_Confirmed_Outstanding_UoM<>0 OR DO_Issued_Outstanding_UoM<>0 OR ";
                strSql += "Retur_Jual_Created_UoM<>0 OR Retur_Jual_Approved_Oustanding_UoM<>0 OR Retur_Jual_GR_In_Progress_UoM<>0) ";
                strSql += "UNION ALL SELECT FullItemId FROM Invent_Movement_Qty WHERE FullItemId='" + FullItemId + "' AND ";
                strSql += "(GR_In_Progress_UoM<>0 OR Resize_In_Progress_UoM<>0 OR Parked_For_Action_Outstanding_UoM<>0 OR Transfer_In_Progress_UoM<>0 OR ";
                strSql += "Transfer_Masuk_In_Progress_UoM<>0 OR Transfer_Keluar_In_Progress_UoM<>0 OR Adjustment_In_Progress_UoM<>0 OR Disposed_In_Progress_UoM<>0) ";
                strSql += "UNION ALL SELECT FullItemId FROM Invent_OnHand_Qty WHERE InventSiteId<>'' AND FullItemId='" + FullItemId + "' AND ";
                strSql += "(Available_UoM<>0 OR Available_For_Sale_UoM<>0 OR Available_For_Sale_Reserved_UoM<>0) ";
                strSql += "UNION ALL SELECT ItemId FROM InventGelombangD WHERE ItemId='" + FullItemId + "' ";
                strSql += "UNION ALL SELECT FullItemId FROM Pricelist_Dtl WHERE FullItemId='" + FullItemId + "' ";
                Conn = ConnectionString.GetConnection();
                using (SqlCommand Cmd = new SqlCommand(strSql, Conn))
                {
                    Dr = Cmd.ExecuteReader();
                    if (Dr.HasRows)
                    {
                        ErrMsg = "Tidak dapat hapus, Item Sudah ada transaksi..";
                        vBol = false;
                    }
                    else
                    {
                        vBol = true;
                    }
                }
                Dr.Close();
                Conn.Close();
            }

            if (vBol == false) { MessageBox.Show(ErrMsg); }
            return vBol;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Delete) > 0)
            {
                try
                {
                    if (dgvConfigItem.RowCount > 0)
                    {
                        Index = dgvConfigItem.CurrentRow.Index;
                        string FullItemID = dgvConfigItem.Rows[Index].Cells["FullItemID"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["FullItemID"].Value.ToString();
                        string No = dgvConfigItem.Rows[Index].Cells["No"].Value == null ? "" : dgvConfigItem.Rows[Index].Cells["No"].Value.ToString();
                        //String VendName = dgvPR.Rows[Index].Cells["VendName"].Value == null ? "" : dgvPR.Rows[Index].Cells["VendName"].Value.ToString();

                        if (!ValidGeneral(FullItemID))
                        {
                            return;
                        }

                        DialogResult dr = MessageBox.Show("FullItemID = " + FullItemID + "\n" + Environment.NewLine + "Apakah data diatas akan dihapus ?", "Konfirmasi", MessageBoxButtons.YesNo);
                        if (dr == DialogResult.Yes)
                        {
                            //delete header
                            Conn = ConnectionString.GetConnection();
                            Query = "Delete from [dbo].[InventTable] where FullItemID='" + FullItemID + "'";
                            Query += " Delete from [dbo].[InventConversion] where FullItemID='" + FullItemID + "'";
                            Query += " Delete from [dbo].[Invent_Purchase_Qty] where FullItemID='" + FullItemID + "'";
                            Query += " Delete from [dbo].[Invent_Sales_Qty] where FullItemID='" + FullItemID + "'";
                            Query += " Delete from [dbo].[Invent_Movement_Qty] where FullItemID='" + FullItemID + "'";
                            Query += " Delete from [dbo].[Invent_OnHand_Qty] where FullItemID='" + FullItemID + "'";

                            Cmd = new SqlCommand(Query, Conn);
                            Cmd.ExecuteNonQuery();

                            MessageBox.Show("No = " + No + "\n" + " Data berhasil dihapus.");

                            ////Query += "insert into [Master].[Logs] (logstablename,logsquery,createddate,createdby) values ('Master.City','" & Query.Replace("'", "''") & "', getdate(),'" & ControlMgr.UserId & "');";
                            Index = 0;
                            Conn.Close();
                            RefreshGrid();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ConnectionString.GlobalException(ex));
                }
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end             
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            InventTableHeader ITH = new InventTableHeader();
            ITH.SetParent(this);
            ITH.Show();
            ITH.ModeBeforeEdit();
            ITH.GetDataHeader(dgvConfigItem.CurrentRow.Cells["FullItemID"].Value.ToString());
            RefreshGrid();


            ////begin
            ////updated by : joshua
            ////updated date : 24 feb 2018
            ////description : check permission access
            //FormInventTable Type = new FormInventTable();                  
            //if (Type.PermissionAccess(ControlMgr.View) > 0)
            //{
            //    //Simpen HeaderId
            //    if (dgvConfigItem.RowCount > 0)
            //    {
            //        Type.SetParent(this);
            //        Type.Show();
            //        Type.ModeBeforeEdit();
            //        Type.GetDataHeader(dgvConfigItem.CurrentRow.Cells["FullItemID"].Value.ToString());
            //        RefreshGrid();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show(ControlMgr.PermissionDenied);
            //}
            ////end              
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            
            txtSearch.Text = "";
            ModeLoad();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
       {
            if (timerRefresh == null)
            {

            }
            else
            {
                RefreshGrid();
            }
        }

        private void cmbCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }

        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                RefreshGrid();
            }
        }

        private void InquiryInventTable_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerRefresh = null;
            for (int i = 0; i < ListFormInventTable.Count(); i++)
            {
                ListFormInventTable[i].Close();
            }
        }

        private void InquiryInventTable_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void cmbCriteria_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbCriteria.Text.Contains("Date"))
            {
                dtFrom.Enabled = true;
                dtTo.Enabled = true;
            }
            else
            {
                dtFrom.Enabled = false;
                dtTo.Enabled = false;
            }
        }

        private void dgvConfigItem_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Simpen HeaderId
            if (dgvConfigItem.RowCount > 0)
            {
                //FormInventTable Type = new FormInventTable();
                //Type.SetParent(this);
                //Type.Show();
                //Type.ModeBeforeEdit();
                //Type.GetDataHeader(dgvConfigItem.CurrentRow.Cells["FullItemID"].Value.ToString());
                //RefreshGrid();

                InventTableHeader ITH = new InventTableHeader();
                ITH.SetParent(this);
                ITH.Show();
                ITH.ModeBeforeEdit();
                ITH.GetDataHeader(dgvConfigItem.CurrentRow.Cells["FullItemID"].Value.ToString());
                RefreshGrid();
            }
        }

        private void btnNext_Click_1(object sender, EventArgs e)
        {
            if (Limit1 + Int32.Parse(cmbShow.Text) <= Total)
            {
                Limit1 += Int32.Parse(cmbShow.Text);
                Limit2 += Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) + 1).ToString();
            }

            RefreshGrid();
        }

        private void btnPrev_Click_1(object sender, EventArgs e)
        {
            if (Limit2 - Int32.Parse(cmbShow.Text) >= 1)
            {
                Limit1 -= Int32.Parse(cmbShow.Text);
                Limit2 -= Int32.Parse(cmbShow.Text);
                txtPage.Text = (Int32.Parse(txtPage.Text) - 1).ToString();
            }
            RefreshGrid();
        }    

        private void txtPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                RefreshGrid();
            }
        }

        private void cmbShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
                Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
                txtPage.Text = "1";
                RefreshGrid();
            }
        }

        private void cmbShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnMNext_Click(object sender, EventArgs e)
        {
            txtPage.Text = Math.Ceiling((decimal)Total / Int32.Parse(cmbShow.Text)).ToString();
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void btnMPrev_Click_1(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }

        private void txtSearch_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (txtSearch.Text == null || txtSearch.Text.Equals(""))
                {
                    MessageBox.Show("Masukkan Kata Kunci");
                }
                else if (cmbCriteria.SelectedIndex == -1)
                {
                    crit = "All";
                }
                else
                {
                    crit = cmbCriteria.SelectedItem.ToString();
                }

                RefreshGrid();
            }
        }

        private void cmbShow_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            txtPage.Text = "1";
            Limit1 = (Int32.Parse(txtPage.Text) - 1) * Int32.Parse(cmbShow.Text) + 1;
            Limit2 = Int32.Parse(txtPage.Text) * Int32.Parse(cmbShow.Text);
            RefreshGrid();
        }
    }
}
