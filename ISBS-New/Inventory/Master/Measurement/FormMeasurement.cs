using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.Master.Invent.Measurement
{
    public partial class FormMeasurement : MetroFramework.Forms.MetroForm
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

        string vOldDeskripsi;

        Master.Invent.Measurement.InquiryMeasurement Parent;

        public FormMeasurement()
        {
            InitializeComponent();
        }

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        //Mode
        string Mode = "";

        public void ModeNew()
        {
            Mode = "New";

            txtMeasurementId.Enabled = true;
            txtMeasurementDesc.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;
            btnExit.Visible = true;
        }

        public void ModeEdit()
        {
            Mode = "Edit";

            txtMeasurementId.Enabled = false;
            txtMeasurementDesc.Enabled = true;

            btnSave.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = true;
            btnExit.Visible = false;
        }

        public void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            txtMeasurementId.Enabled = false;
            txtMeasurementDesc.Enabled = false;

            btnSave.Visible = false;
            btnEdit.Visible = true;
            btnCancel.Visible = false;
            btnExit.Visible = true;
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
            vOldDeskripsi = txtMeasurementDesc.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
            txtMeasurementDesc.Text = vOldDeskripsi;
        }

        private bool cekValidasi()
        {
            Boolean vBol = true;
            string ErrMsg = "";

            if (txtMeasurementId.Text == "")
            {
                ErrMsg = "Measurement harus diisi..";
                vBol = false;
            }

            if (vBol == true && txtMeasurementDesc.Text.Trim() == "")
            {
                ErrMsg = "Deskripsi harus diisi..";
                vBol = false;
            }

            if (vBol == true && Mode == "New")
            {
                try
                {
                    Query = "Select * From InventMeasurement Where MeasurementID=@txtMeasurementId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtMeasurementId", txtMeasurementId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            ErrMsg = "Measurement sudah ada..";
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
                    Query = "Select * From InventMeasurement Where MeasurementId=@txtMeasurementId ";
                    Conn = ConnectionString.GetConnection();
                    using (SqlCommand cmd = new SqlCommand(Query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@txtMeasurementId", txtMeasurementId.Text.Trim());
                        Dr = cmd.ExecuteReader();
                        if (Dr.Read())
                        {
                            vBol = true;
                        }
                        else
                        {
                            ErrMsg = "Measurement tidak ditemukan..";
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
            if (cekValidasi() == false)
            {
                return;
            }


            try
            {
                //Cek Data apakah sudah ada
                string CekData;
                Conn = ConnectionString.GetConnection();

                //Jika New
                if (Mode=="New")
                {
                    Query = "Insert into dbo.InventMeasurement (MeasurementID, Deskripsi, CreatedDate, CreatedBy) values ";
                    Query += "(@txtMeasurementId,@txtMeasurementDesc,getdate(),'" + ControlMgr.UserId + "');";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtMeasurementId", txtMeasurementId.Text.Trim());
                        Command.Parameters.AddWithValue("@txtMeasurementDesc", txtMeasurementDesc.Text.Trim());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("MeasurementID = " + txtMeasurementId.Text.Trim() + Environment.NewLine + "Deskripsi = " + txtMeasurementDesc.Text.Trim() + Environment.NewLine + "Berhasil ditambahkan.");
                }

                //Jika Edit
                else if (Mode=="Edit")
                {
                    Query = "Update dbo.InventMeasurement set Deskripsi=@txtMeasurementDesc, UpdatedDate=getdate(), UpdatedBy='" + ControlMgr.UserId + "' where MeasurementID=@txtMeasurementId";
                    Trans = Conn.BeginTransaction();
                    using (SqlCommand Command = new SqlCommand(Query, Conn, Trans))
                    {
                        Command.Parameters.AddWithValue("@txtMeasurementId", txtMeasurementId.Text);
                        Command.Parameters.AddWithValue("@txtMeasurementDesc", txtMeasurementDesc.Text.Trim());
                        Command.ExecuteScalar();
                    }
                    Trans.Commit();
                    MessageBox.Show("MeasurementID = " + txtMeasurementId.Text.Trim() + Environment.NewLine + "Berhasil diupdate.");
                }
                Conn.Close();
                this.Close();                
            }
            catch (Exception Ex)
            {
                Trans.Rollback();
                MessageBox.Show(ConnectionString.GlobalException(Ex));
            }
            finally
            {
                Form parentform = Application.OpenForms["InquiryMeasurement"];
                if (parentform != null)
                    Parent.RefreshGrid();

                Conn.Close();
            }  
        }

        public void SetParent(Master.Invent.Measurement.InquiryMeasurement F)
        {
            Parent = F;
        }

        public void GetDataHeader(string MeasurementID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select MeasurementID, Deskripsi From [dbo].[InventMeasurement] where MeasurementID=@MeasurementID ";

            Cmd = new SqlCommand(Query, Conn);
            Cmd.Parameters.AddWithValue("@MeasurementID", MeasurementID);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                txtMeasurementId.Text = Dr["MeasurementID"].ToString();
                txtMeasurementDesc.Text = Dr["Deskripsi"].ToString();
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
            string TableName = "InventMeasurement";

            Search tmpSearch = new Search();
            tmpSearch.SetSchemaTable(SchemaName, TableName);
            tmpSearch.ShowDialog();
            txtMeasurementId.Text = ConnectionString.Kode;
        }

        private void FormMeasurement_Load(object sender, EventArgs e)
        {
            this.Location = new Point(148, 47);
        }
               
    }
}
