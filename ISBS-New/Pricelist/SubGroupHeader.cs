using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ISBS_New.Pricelist
{
    public partial class SubGroupHeader : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        private int Index;       
        private string Mode, Query, crit = null;
        private string PricelistNo = "", tmpPrType = "", PricelistType = "", Type = "", DeliveryMethod = "", Criteria = "", OldFullItemIDDB = "", OldFullItemIDGrid = "";
        Pricelist.PricelistHeader Parent;
        DateTimePicker dtp;
        Regex strPattern = new Regex("^[0-9.-]*$");

        DataGridView dgvAccountList;
        Dictionary<string, string> DataCheckPricelist = new Dictionary<string, string>();

        //begin
        //created by : joshua
        //created date : 26 apr 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public SubGroupHeader()
        {
            InitializeComponent();

        }

        private void SubGroupHeader_Load(object sender, EventArgs e)
        {
            lblForm.Location = new Point(16, 11);

            if (Mode == "New")
            {
                ModeNew();
            }

            SetCmbManufacture();
            SetCmbMerek();
            SetCmbGolongan();
            SetCmbBerat();
            SetCmbSpec();
            SetCmbBentuk();

            dgvSubGroupPricelistDetails.Controls.Add(dtp);
        }

        private void SetCmbManufacture()
        {
            cmbManufactureID.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT ManufacturerID FROM InventManufacturer";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbManufactureID.DisplayMember = "Text";
            cmbManufactureID.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbManufactureID.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[0] + "" });
            }
            cmbManufactureID.SelectedIndex = 0;
            Conn.Close();
        }

        private void SetCmbMerek()
        {
            cmbMerekID.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT MerekID FROM InventMerek";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbMerekID.DisplayMember = "Text";
            cmbMerekID.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbMerekID.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[0] + "" });
            }
            cmbMerekID.SelectedIndex = 0;
            Conn.Close();
        }

        private void SetCmbGolongan()
        {
            cmbGolonganID.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT GolonganID FROM InventGolongan";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbGolonganID.DisplayMember = "Text";
            cmbGolonganID.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbGolonganID.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[0] + "" });
            }
            cmbGolonganID.SelectedIndex = 0;
            Conn.Close();
        }

        private void SetCmbBerat()
        {
            cmbKodeBerat.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT BeratId FROM InventBerat";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbKodeBerat.DisplayMember = "Text";
            cmbKodeBerat.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbKodeBerat.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[0] + "" });
            }
            cmbKodeBerat.SelectedIndex = 0;
            Conn.Close();
        }

        private void SetCmbSpec()
        {
            cmbSpecID.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT SpecID FROM InventSpec";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbSpecID.DisplayMember = "Text";
            cmbSpecID.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbSpecID.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[0] + "" });
            }
            cmbSpecID.SelectedIndex = 0;
            Conn.Close();
        }

        private void SetCmbBentuk()
        {
            cmbBentuk.Items.Add("-select-");
            Conn = ConnectionString.GetConnection();
            Query = "SELECT Bentuk FROM InventBentuk";

            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            cmbBentuk.DisplayMember = "Text";
            cmbBentuk.ValueMember = "Value";

            while (Dr.Read())
            {
                cmbBentuk.Items.Add(new { Value = "" + Dr[0] + "", Text = "" + Dr[0] + "" });
            }
            cmbBentuk.SelectedIndex = 0;
            Conn.Close();
        }

        public void SetMode(string tmpMode, string tmpPricelistType, string tmpDeliveryMethod, DataGridView tmpDgvAccountList, string tmpCriteria)
        {
            Mode = tmpMode;
            PricelistType = tmpPricelistType;
            DeliveryMethod = tmpDeliveryMethod;
            dgvAccountList = tmpDgvAccountList;
            Criteria = tmpCriteria;
            lblForm.Text = Text = "SubGroup2 " + lblForm.Text + " " + tmpPricelistType;
            if(PricelistType.ToUpper() == "JUAL")
            {               
                Type = "SALES";
            }
            else
            {              
                Type = "PURCHASE";
            }
        }

        public void SetParent(Pricelist.PricelistHeader F)
        {
            Parent = F;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SubGroupHeader_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void SubGroupHeader_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Parent.RefreshGrid();
        }

        public void ModeNew()
        {
            PricelistNo = "";
            btnAdd.Visible = true;
          
           
        }

        private void btnSubGroup2_Click(object sender, EventArgs e)
        {
            //string ExistingSubGroup2ID = "";
            //if (Parent.dgvPricelistDetails.RowCount > 0)
            //{
            //    for (int i = 0; i < Parent.dgvPricelistDetails.RowCount; i++)
            //    {
            //        ExistingSubGroup2ID = ExistingSubGroup2ID + "'" + Parent.dgvPricelistDetails.Rows[i].Cells[22].Value + "',";
            //    }
            //    ExistingSubGroup2ID = ExistingSubGroup2ID.Remove(ExistingSubGroup2ID.Length - 1);
            //}

            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "GroupID";
            tmpSearch.Order = "GroupID Asc";

            if (PricelistType.ToUpper() == "JUAL")
            {
                tmpSearch.Table = "[dbo].[InventSubGroup2]";
                tmpSearch.QuerySearch = "SELECT DISTINCT a.GroupID, a.SubGroup1ID, a.SubGroup2ID, a.Deskripsi AS SubGroup2 FROM [dbo].[InventSubGroup2] a ";
                tmpSearch.QuerySearch += "INNER JOIN PricelistConfig b ON b.SubGroup2Id = a.SubGroup2ID ";
            }
            else
            {
                tmpSearch.Table = "[dbo].[InventSubGroup2]";
                tmpSearch.QuerySearch = "SELECT DISTINCT GroupID, SubGroup1ID, SubGroup2ID, Deskripsi AS SubGroup2 FROM InventSubGroup2 ";
            }
     
            //if (ExistingSubGroup2ID != "")
            //{
            //    tmpSearch.QuerySearch += "WHERE a.SubGroup2ID NOT IN (" + ExistingSubGroup2ID + ")";
            //}
            tmpSearch.FilterText = new string[] { "SubGroup2ID", "SubGroup2" };
            tmpSearch.Select = new string[] { "GroupID", "SubGroup1ID", "SubGroup2ID", "SubGroup2" };
            tmpSearch.Hide = new string[] { "GroupID", "SubGroup1ID" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtGroupID.Text = ConnectionString.Kodes[0];
                txtSubGroup1ID.Text = ConnectionString.Kodes[1];
                txtSubGroup2ID.Text = ConnectionString.Kodes[2];
                txtSubGroup2.Text = ConnectionString.Kodes[3];
                ConnectionString.Kodes = null;
                btnUkuran1From.Enabled = true;
                btnUkuran2From.Enabled = true;
                btnUkuran3From.Enabled = true;
                btnUkuran4From.Enabled = true;
                btnUkuran5From.Enabled = true;

                btnUkuran1To.Enabled = false;
                btnUkuran2To.Enabled = false;
                btnUkuran3To.Enabled = false;
                btnUkuran4To.Enabled = false;
                btnUkuran5To.Enabled = false;

                txtUkuran1From.Text = "";
                txtUkuran2From.Text = "";
                txtUkuran3From.Text = "";
                txtUkuran4From.Text = "";
                txtUkuran5From.Text = "";

                txtUkuran1To.Text = "";
                txtUkuran2To.Text = "";
                txtUkuran3To.Text = "";
                txtUkuran4To.Text = "";
                txtUkuran5To.Text = "";

                txtUkuran1To.Enabled = false;
                txtUkuran2To.Enabled = false;
                txtUkuran3To.Enabled = false;
                txtUkuran4To.Enabled = false;
                txtUkuran5To.Enabled = false;

                txtUkuran1From.Enabled = true;
                txtUkuran2From.Enabled = true;
                txtUkuran3From.Enabled = true;
                txtUkuran4From.Enabled = true;
                txtUkuran5From.Enabled = true;

                ClearDgvPricelistDetails();
            }
        }

        private void btnUkuran1From_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran1Value", "From", "");
        }

        private void btnUkuran1To_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran1Value", "To", txtUkuran1From.Text);
        }

        private void btnUkuran2From_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran2Value", "From", "");
        }

        private void btnUkuran2To_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran2Value", "To", txtUkuran2From.Text);
        }

        private void btnUkuran3From_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran3Value", "From", "");
        }

        private void btnUkuran3To_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran3Value", "To", txtUkuran3From.Text);
        }

        private void btnUkuran4From_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran4Value", "From", "");
        }

        private void btnUkuran4To_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran4Value", "To", txtUkuran4From.Text);
        }

        private void btnUkuran5From_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran5Value", "From", "");
        }

        private void btnUkuran5To_Click(object sender, EventArgs e)
        {
            LookUpUkuran("Ukuran5Value", "To", txtUkuran5From.Text);
        }

        private void LookUpUkuran(string Ukuran, string ValueType, string ValueUkuran)
        {
            string SubGroup2ID = txtSubGroup2ID.Text;

            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "" + SubGroup2ID + "";
            tmpSearch.Order = "" + Ukuran + " ASC";
            tmpSearch.Table = "[dbo].[InventTable]";
            tmpSearch.QuerySearch = "SELECT DISTINCT " + Ukuran + " AS " + Ukuran + " FROM [dbo].[InventTable] WHERE ";
            tmpSearch.QuerySearch += "SubGroup2ID = " + SubGroup2ID + " ";
            if (ValueType == "To")
            {
                tmpSearch.QuerySearch += "AND  CONVERT(decimal, " + Ukuran + ") >= CONVERT(decimal, " + ValueUkuran + ") ";
            }
            tmpSearch.FilterText = new string[] { Ukuran };
            tmpSearch.Select = new string[] { Ukuran };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                if (ValueType == "From")
                {
                    if (Ukuran == "Ukuran1Value")
                    {
                        txtUkuran1From.Text = ConnectionString.Kodes[0];
                        SetUkuranTo(1, "Aktif");
                        txtUkuran1To.Text = "";

                    }
                    else if (Ukuran == "Ukuran2Value")
                    {
                        txtUkuran2From.Text = ConnectionString.Kodes[0];
                        SetUkuranTo(2, "Aktif");
                        txtUkuran2To.Text = "";
                    }
                    else if (Ukuran == "Ukuran3Value")
                    {
                        txtUkuran3From.Text = ConnectionString.Kodes[0];
                        SetUkuranTo(3, "Aktif");
                        txtUkuran3To.Text = "";
                    }
                    else if (Ukuran == "Ukuran4Value")
                    {
                        txtUkuran4From.Text = ConnectionString.Kodes[0];
                        SetUkuranTo(4, "Aktif");
                        txtUkuran4To.Text = "";
                    }
                    else if (Ukuran == "Ukuran5Value")
                    {
                        txtUkuran5From.Text = ConnectionString.Kodes[0];
                        SetUkuranTo(5, "Aktif");
                        txtUkuran5To.Text = "";
                    }
                }
                else
                {
                    if (Ukuran == "Ukuran1Value")
                    {
                        txtUkuran1To.Text = ConnectionString.Kodes[0];
                    }
                    else if (Ukuran == "Ukuran2Value")
                    {
                        txtUkuran2To.Text = ConnectionString.Kodes[0];
                    }
                    else if (Ukuran == "Ukuran3Value")
                    {
                        txtUkuran3To.Text = ConnectionString.Kodes[0]; ;
                    }
                    else if (Ukuran == "Ukuran4Value")
                    {
                        txtUkuran4To.Text = ConnectionString.Kodes[0];
                    }
                    else if (Ukuran == "Ukuran5Value")
                    {
                        txtUkuran5To.Text = ConnectionString.Kodes[0];
                    }
                }

                ConnectionString.Kodes = null;

                ClearDgvPricelistDetails();
            }
        }

        private void SetUkuranTo(int UkuranKe, string Status)
        {
            if (UkuranKe == 1)
            {
                if (Status == "Aktif")
                {
                    txtUkuran1To.Enabled = true;
                    btnUkuran1To.Enabled = true;
                }
                else
                {
                    txtUkuran1To.Enabled = false;
                    txtUkuran1To.Text = "";
                }
            }
            else if (UkuranKe == 2)
            {
                if (Status == "Aktif")
                {
                    txtUkuran2To.Enabled = true;
                    btnUkuran2To.Enabled = true;
                }
                else
                {
                    txtUkuran2To.Enabled = false;
                    txtUkuran2To.Text = "";
                }
            }
            if (UkuranKe == 3)
            {
                if (Status == "Aktif")
                {
                    txtUkuran3To.Enabled = true;
                    btnUkuran3To.Enabled = true;
                }
                else
                {
                    txtUkuran3To.Enabled = false;
                    txtUkuran3To.Text = "";
                }
            }
            if (UkuranKe == 4)
            {
                if (Status == "Aktif")
                {
                    txtUkuran4To.Enabled = true;
                    btnUkuran4To.Enabled = true;
                }
                else
                {
                    txtUkuran4To.Enabled = false;
                    txtUkuran4To.Text = "";
                }
            }
            if (UkuranKe == 5)
            {
                if (Status == "Aktif")
                {
                    txtUkuran5To.Enabled = true;
                    btnUkuran5To.Enabled = true;
                }
                else
                {
                    txtUkuran5To.Enabled = false;
                    txtUkuran5To.Text = "";
                }
            }
        }

        private void CheckDataTypeUkuran(TextBox txtName)
        {
            if (strPattern.IsMatch(txtName.Text))
            {
                if (txtName.Text != "")
                {
                    int UkuranValue = 0;

                    if (txtName.Name.Contains("1"))
                    {
                        if (txtName.Name.Contains("From"))
                        {
                            UkuranValue = CheckUkuranValue("Ukuran1Value", txtName.Text, "");
                        }
                        else
                        {
                            UkuranValue = CheckUkuranValue("Ukuran1Value", txtName.Text, txtUkuran1From.Text);
                        }

                        if (UkuranValue > 0)
                        {
                            SetUkuranTo(1, "Aktif");
                            ClearDgvPricelistDetails();
                        }
                        else
                        {
                            if (txtName.Name.Contains("From"))
                            {
                                MessageBox.Show("Ukuran 1 From Tidak ada di database");
                                SetUkuranTo(1, "Inaktif");
                            }
                            else
                            {
                                MessageBox.Show("Ukuran 1 To Tidak ada di database");
                            }
                            txtName.Text = "";
                        }

                    }
                    else if (txtName.Name.Contains("2"))
                    {
                        if (txtName.Name.Contains("From"))
                        {
                            UkuranValue = CheckUkuranValue("Ukuran2Value", txtName.Text, "");
                        }
                        else
                        {
                            UkuranValue = CheckUkuranValue("Ukuran2Value", txtName.Text, txtUkuran2From.Text);
                        }

                        if (UkuranValue > 0)
                        {
                            SetUkuranTo(2, "Aktif");
                            ClearDgvPricelistDetails();
                        }
                        else
                        {
                            if (txtName.Name.Contains("From"))
                            {
                                MessageBox.Show("Ukuran 2 From Tidak ada di database");
                                SetUkuranTo(2, "Inaktif");
                            }
                            else
                            {
                                MessageBox.Show("Ukuran 2 To Tidak ada di database");
                            }
                            txtName.Text = "";
                        }
                    }
                    else if (txtName.Name.Contains("3"))
                    {
                        if (txtName.Name.Contains("From"))
                        {
                            UkuranValue = CheckUkuranValue("Ukuran3Value", txtName.Text, "");
                        }
                        else
                        {
                            UkuranValue = CheckUkuranValue("Ukuran3Value", txtName.Text, txtUkuran3From.Text);
                        }

                        if (UkuranValue > 0)
                        {
                            SetUkuranTo(3, "Aktif");
                            ClearDgvPricelistDetails();
                        }
                        else
                        {
                            if (txtName.Name.Contains("From"))
                            {
                                MessageBox.Show("Ukuran 3 From Tidak ada di database");
                                SetUkuranTo(3, "Inaktif");
                            }
                            else
                            {
                                MessageBox.Show("Ukuran 3 To Tidak ada di database");
                            }
                            txtName.Text = "";
                        }
                    }
                    else if (txtName.Name.Contains("4"))
                    {
                        if (txtName.Name.Contains("From"))
                        {
                            UkuranValue = CheckUkuranValue("Ukuran4Value", txtName.Text, "");
                        }
                        else
                        {
                            UkuranValue = CheckUkuranValue("Ukuran4Value", txtName.Text, txtUkuran4From.Text);
                        }

                        if (UkuranValue > 0)
                        {
                            SetUkuranTo(4, "Aktif");
                            ClearDgvPricelistDetails();
                        }
                        else
                        {
                            if (txtName.Name.Contains("From"))
                            {
                                MessageBox.Show("Ukuran 4 From Tidak ada di database");
                                SetUkuranTo(4, "Inaktif");
                            }
                            else
                            {
                                MessageBox.Show("Ukuran 4 To Tidak ada di database");
                            }
                            txtName.Text = "";
                        }
                    }
                    else if (txtName.Name.Contains("5"))
                    {
                        if (txtName.Name.Contains("From"))
                        {
                            UkuranValue = CheckUkuranValue("Ukuran5Value", txtName.Text, "");
                        }
                        else
                        {
                            UkuranValue = CheckUkuranValue("Ukuran5Value", txtName.Text, txtUkuran5From.Text);
                        }

                        if (UkuranValue > 0)
                        {

                            SetUkuranTo(5, "Aktif");
                            ClearDgvPricelistDetails();
                        }
                        else
                        {
                            if (txtName.Name.Contains("From"))
                            {
                                MessageBox.Show("Ukuran 5 From Tidak ada di database");
                                SetUkuranTo(5, "Inaktif");
                            }
                            else
                            {
                                MessageBox.Show("Ukuran 5 To Tidak ada di database");
                            }
                            txtName.Text = "";
                        }
                    }
                }
                else
                {
                    if (txtName.Name.Contains("From"))
                    {
                        if (txtName.Name.Contains("1"))
                        {
                            SetUkuranTo(1, "Inaktif");
                        }
                        else if (txtName.Name.Contains("2"))
                        {
                            SetUkuranTo(2, "Inaktif");
                        }
                        else if (txtName.Name.Contains("3"))
                        {
                            SetUkuranTo(3, "Inaktif");
                        }
                        else if (txtName.Name.Contains("4"))
                        {
                            SetUkuranTo(4, "Inaktif");
                        }
                        else if (txtName.Name.Contains("5"))
                        {
                            SetUkuranTo(5, "Inaktif");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Harus angka");
                txtName.Text = "";

                if (txtName.Name.Contains("From"))
                {
                    if (txtName.Name.Contains("1"))
                    {
                        SetUkuranTo(1, "Inaktif");
                    }
                    else if (txtName.Name.Contains("2"))
                    {
                        SetUkuranTo(2, "Inaktif");
                    }
                    else if (txtName.Name.Contains("3"))
                    {
                        SetUkuranTo(3, "Inaktif");
                    }
                    else if (txtName.Name.Contains("4"))
                    {
                        SetUkuranTo(4, "Inaktif");
                    }
                    else if (txtName.Name.Contains("5"))
                    {
                        SetUkuranTo(5, "Inaktif");
                    }
                }
            }
        }

        private int CheckUkuranValue(string FieldName, string txtValue, string txtValueTo)
        {
            int Result = 0;
            Conn = ConnectionString.GetConnection();
            Query = "SELECT COUNT(" + FieldName + ") FROM InventTable WHERE " + FieldName + " = " + txtValue + " AND SubGroup2ID = '" + txtSubGroup2ID.Text + "' ";
            if (txtValueTo != "")
            {
                Query += "AND " + FieldName + " >= " + txtValueTo + " ";
            }

            Cmd = new SqlCommand(Query, Conn);
            Result = Convert.ToInt32(Cmd.ExecuteScalar());
            Conn.Close();

            return Result;
        }

        private void txtUkuran1From_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran1From);
        }

        private void txtUkuran2From_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran2From);
        }

        private void txtUkuran3From_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran3From);
        }

        private void txtUkuran4From_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran4From);
        }

        private void txtUkuran5From_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran5From);
        }

        private void txtUkuran1To_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran1To);
        }

        private void txtUkuran2To_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran2To);
        }

        private void txtUkuran3To_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran3To);
        }

        private void txtUkuran4To_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran4To);
        }

        private void txtUkuran5To_Leave(object sender, EventArgs e)
        {
            CheckDataTypeUkuran(txtUkuran5To);
        }

        private void txtPriceCash_Leave(object sender, EventArgs e)
        {
            if (txtPriceCash.Text != "")
            {
                double d = double.Parse(txtPriceCash.Text);
                txtPriceCash.Text = d.ToString("N2");
            }
        }

        private void txtTolerance_Leave(object sender, EventArgs e)
        {
            if (txtTolerance.Text != "")
            {
                 if (Convert.ToDouble(txtTolerance.Text) > 100)
                {
                    txtTolerance.Text = "100.00";

                    MessageBox.Show("Tolerance tidak boleh lebih dari 100");
                    return;
                }
                double d = double.Parse(txtTolerance.Text);
                txtTolerance.Text = d.ToString("N2");
            }             
        }

        private void txtPriceCash_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtTolerance_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnGenerate_Click(object sender, EventArgs e)
        {   
            if (txtSubGroup2.Text == "")
            {
                MessageBox.Show("Sub Group 2 harus diisi");
                return;
            }
            else if (txtUkuran1From.Text != "" & txtUkuran1To.Text == "")
            {
                MessageBox.Show("Ukuran 1 To harus diisi");
                return;
            }
            else if (txtUkuran2From.Text != "" & txtUkuran2To.Text == "")
            {
                MessageBox.Show("Ukuran 2 To harus diisi");
                return;
            }
            else if (txtUkuran3From.Text != "" & txtUkuran3To.Text == "")
            {
                MessageBox.Show("Ukuran 3 To harus diisi");
                return;
            }
            else if (txtUkuran4From.Text != "" & txtUkuran4To.Text == "")
            {
                MessageBox.Show("Ukuran 4 To harus diisi");
                return;
            }
            else if (txtUkuran5From.Text != "" & txtUkuran5To.Text == "")
            {
                MessageBox.Show("Ukuran 5 To harus diisi");
                return;
            }           
            else if (txtTolerance.Text == "")
            {
                MessageBox.Show("Tolerance harus diisi");
                return;
            }
            else if (txtPriceCash.Text == "")
            {
                MessageBox.Show("Price Cash harus diisi");
                return;
            }
            else if (txtPriceCash.Text == "0.00")
            {
                MessageBox.Show("Price Cash harus lebih besar dari 0");
                return;
            }
            else if(Parent.dgvPricelistDetails.RowCount > 0)
            {
                OldFullItemIDGrid = "";

                //GET DATA GRIDVIEW PRICELIST DETAILS 
                var DistinctPricelistDetails = from Pricelist in Parent.dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
                                               where Pricelist.Cells["SubGroup2ID"].Value.ToString() == txtSubGroup2ID.Text
                                               select Pricelist;   
               
                foreach (var DataPricelist in DistinctPricelistDetails)
                {
                    OldFullItemIDGrid = OldFullItemIDGrid + "" + DataPricelist.Cells["FullItemID"].Value.ToString() + "-";
                }

                if (OldFullItemIDGrid != "")
                {
                    OldFullItemIDGrid = OldFullItemIDGrid.Remove(OldFullItemIDGrid.Length - 1);
                }
                
              
                //END GET DATA GRIDVIEW PRICELIST DETAILS 

                //VALIDASI UKURAN
                //foreach (var DataPricelist in DistinctPricelistDetails)
                //{
                //    decimal Ukuran1_Value_To = DataPricelist.Cells["Ukuran1_Value_To"].Value.ToString() == "" ? 0 : Convert.ToDecimal(DataPricelist.Cells["Ukuran1_Value_To"].Value);
                //    decimal Ukuran2_Value_To = DataPricelist.Cells["Ukuran2_Value_To"].Value.ToString() == "" ? 0 : Convert.ToDecimal(DataPricelist.Cells["Ukuran2_Value_To"].Value);
                //    decimal Ukuran3_Value_To = DataPricelist.Cells["Ukuran3_Value_To"].Value.ToString() == "" ? 0 : Convert.ToDecimal(DataPricelist.Cells["Ukuran3_Value_To"].Value);
                //    decimal Ukuran4_Value_To = DataPricelist.Cells["Ukuran4_Value_To"].Value.ToString() == "" ? 0 : Convert.ToDecimal(DataPricelist.Cells["Ukuran4_Value_To"].Value);
                //    decimal Ukuran5_Value_To = DataPricelist.Cells["Ukuran5_Value_To"].Value.ToString() == "" ? 0 : Convert.ToDecimal(DataPricelist.Cells["Ukuran5_Value_To"].Value);

                //    //if (Ukuran1_Value_To != 0 && txtUkuran1From.Text != "")
                //    //{
                //    //    if (Ukuran1_Value_To >= Convert.ToDecimal(txtUkuran1From.Text == "" ? "0" : txtUkuran1From.Text))
                //    //    {
                //    //        MessageBox.Show("Value range dari Ukuran 1 sudah ada di gridview");
                //    //        return;
                //    //    }
                //    //}
                //    ////else
                //    ////{
                //    ////    MessageBox.Show("Value range dari Ukuran 1 sudah ada di gridview");
                //    ////    return;
                //    ////}

                //    //if (Ukuran2_Value_To != 0 && txtUkuran2From.Text != "")
                //    //{
                //    //    if (Ukuran2_Value_To >= Convert.ToDecimal(txtUkuran2From.Text == "" ? "0" : txtUkuran2From.Text))
                //    //    {
                //    //        MessageBox.Show("Value range dari Ukuran 2 sudah ada di gridview");
                //    //        return;
                //    //    }
                //    //}
                //    ////else
                //    ////{
                //    ////    MessageBox.Show("Value range dari Ukuran 2 sudah ada di gridview");
                //    ////    return;
                //    ////}

                //    //if (Ukuran3_Value_To != 0 && txtUkuran3From.Text != "")
                //    //{
                //    //    if (Ukuran3_Value_To >= Convert.ToDecimal(txtUkuran3From.Text == "" ? "0" : txtUkuran3From.Text))
                //    //    {
                //    //        MessageBox.Show("Value range dari Ukuran 3 sudah ada di gridview");
                //    //        return;
                //    //    }
                //    //}
                //    ////else
                //    ////{
                //    ////    MessageBox.Show("Value range dari Ukuran 3 sudah ada di gridview");
                //    ////    return;
                //    ////}

                //    //if (Ukuran4_Value_To != 0 && txtUkuran4From.Text != "")
                //    //{
                //    //    if (Ukuran4_Value_To >= Convert.ToDecimal(txtUkuran4From.Text == "" ? "0" : txtUkuran4From.Text))
                //    //    {
                //    //        MessageBox.Show("Value range dari Ukuran 4 sudah ada di gridview");
                //    //        return;
                //    //    }
                //    //}
                //    ////else
                //    ////{
                //    ////    MessageBox.Show("Value range dari Ukuran 4 sudah ada di gridview");
                //    ////    return;
                //    ////}

                //    //if (Ukuran5_Value_To != 0 && txtUkuran5From.Text != "")
                //    //{
                //    //    if (Ukuran5_Value_To >= Convert.ToDecimal(txtUkuran5From.Text == "" ? "0" : txtUkuran5From.Text))
                //    //    {
                //    //        MessageBox.Show("Value range dari Ukuran 5 sudah ada di gridview");
                //    //        return;
                //    //    }
                //    //}
                //    ////else
                //    ////{
                //    ////    MessageBox.Show("Value range dari Ukuran 5 sudah ada di gridview");
                //    ////    return;
                //    ////}
                    
                //}
                //END VALIDASI UKURAN                
            }
            
            try
            {
                Dr = GeneratePricelistDetails(OldFullItemIDGrid);
                if (Dr.HasRows)
                {
                    OldFullItemIDDB = "";
                    while (Dr.Read())
                    {
                        OldFullItemIDDB = OldFullItemIDDB + "" + Dr["FullItemID"].ToString() + "-";
                    }
                    OldFullItemIDDB = OldFullItemIDDB.Remove(OldFullItemIDDB.Length - 1);

                    Dictionary<string, string> OldPricelist = CheckOldPricelistNo(OldFullItemIDDB);
                    if(OldPricelist.Count() > 0)
                    {
                        string ErrorMessage = "\nMasih ada data yang aktif pada \n";
                        if (dgvAccountList.RowCount > 0)
                        {
                            string AccountList = "";
                            if (PricelistType.ToUpper() == "JUAL")
                            {
                                AccountList = "Customer ID : ";
                            }
                            else
                            {
                                AccountList = "Vendor ID : ";
                            }

                            foreach (var ExistingData in OldPricelist)
                            {
                                ErrorMessage = ErrorMessage + "PricelistNo : " + ExistingData.Key + "\n" + AccountList + ExistingData.Value + "\n";
                            }                            
                        }
                        else
                        {
                            foreach (var ExistingData in OldPricelist)
                            {
                                ErrorMessage = ErrorMessage + "PricelistNo : " +ExistingData.Key + "\n";                               
                            }                           
                        }

                        MessageBox.Show("Data dengan Sub Group 2 : " + txtSubGroup2.Text + ", Delivery Method : " + DeliveryMethod + ErrorMessage);
                        return;   
                    }
                    else
                    {
                        Dr = GeneratePricelistDetails(OldFullItemIDGrid);

                        Dt = new DataTable();
                        Dt.Load(Dr);

                        dgvSubGroupPricelistDetails.AutoGenerateColumns = true;
                        dgvSubGroupPricelistDetails.DataSource = Dt;
                        dgvSubGroupPricelistDetails.Refresh();
                        dgvSubGroupPricelistDetails.AutoResizeColumns();
                        setHeaderDgvPricelistDetails();
                        FormatDataNumeric();  
                    }                       
                }
                else
                {
                    MessageBox.Show("Data tidak ada di database atau\nsudah ada di gridview");
                    dgvSubGroupPricelistDetails.DataSource = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conn.Close();
            }    
        }

        private SqlDataReader GeneratePricelistDetails(string OldFullItemID)
        {
            Conn = ConnectionString.GetConnection();
            Cmd = new SqlCommand("SP_GeneratePricelistDetails", Conn);
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.Parameters.AddWithValue("@PricelistType", PricelistType);
            Cmd.Parameters.AddWithValue("@PriceCash", Convert.ToDecimal(txtPriceCash.Text));
            Cmd.Parameters.AddWithValue("@SubGroup2Id", txtSubGroup2ID.Text);
            Cmd.Parameters.AddWithValue("@Tolerance", txtTolerance.Text);
            Cmd.Parameters.AddWithValue("@Ukuran1From", Convert.ToDecimal(txtUkuran1From.Text == "" ? "-1" : txtUkuran1From.Text));
            Cmd.Parameters.AddWithValue("@Ukuran1To", Convert.ToDecimal(txtUkuran1To.Text == "" ? "-1" : txtUkuran1To.Text));
            Cmd.Parameters.AddWithValue("@Ukuran2From", Convert.ToDecimal(txtUkuran2From.Text == "" ? "-1" : txtUkuran2From.Text));
            Cmd.Parameters.AddWithValue("@Ukuran2To", Convert.ToDecimal(txtUkuran2To.Text == "" ? "-1" : txtUkuran2To.Text));
            Cmd.Parameters.AddWithValue("@Ukuran3From", Convert.ToDecimal(txtUkuran3From.Text == "" ? "-1" : txtUkuran3From.Text));
            Cmd.Parameters.AddWithValue("@Ukuran3To", Convert.ToDecimal(txtUkuran3To.Text == "" ? "-1" : txtUkuran3To.Text));
            Cmd.Parameters.AddWithValue("@Ukuran4From", Convert.ToDecimal(txtUkuran4From.Text == "" ? "-1" : txtUkuran4From.Text));
            Cmd.Parameters.AddWithValue("@Ukuran4To", Convert.ToDecimal(txtUkuran4To.Text == "" ? "-1" : txtUkuran4To.Text));
            Cmd.Parameters.AddWithValue("@Ukuran5From", Convert.ToDecimal(txtUkuran5From.Text == "" ? "-1" : txtUkuran5From.Text));
            Cmd.Parameters.AddWithValue("@Ukuran5To", Convert.ToDecimal(txtUkuran5To.Text == "" ? "-1" : txtUkuran5To.Text));
            Cmd.Parameters.AddWithValue("OldFullItemID", OldFullItemID);
            Cmd.Parameters.AddWithValue("ManufacturerID", cmbManufactureID.SelectedIndex != 0 ? cmbManufactureID.Text : "");
            Cmd.Parameters.AddWithValue("MerekID", cmbMerekID.SelectedIndex != 0 ? cmbMerekID.Text : "");
            Cmd.Parameters.AddWithValue("GolonganID", cmbGolonganID.SelectedIndex != 0 ? cmbGolonganID.Text : "");
            Cmd.Parameters.AddWithValue("KodeBerat", cmbKodeBerat.SelectedIndex != 0 ? cmbKodeBerat.Text : "");
            Cmd.Parameters.AddWithValue("SpecID", cmbSpecID.SelectedIndex != 0 ? cmbSpecID.Text : "");
            Cmd.Parameters.AddWithValue("Bentuk", cmbBentuk.SelectedIndex != 0 ? cmbBentuk.Text : "");
           
            Dr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);

            return Dr;
        }

        private Dictionary<string, string> CheckOldPricelistNo(string OldFullItemID)
        {
            string AccountList = "";
            if (dgvAccountList.RowCount > 0)
            {
                for (int i = 0; i < dgvAccountList.RowCount; i++)
                {
                    AccountList = AccountList + "" + dgvAccountList.Rows[i].Cells[1].Value + "-";
                }
                AccountList = AccountList.Remove(AccountList.Length - 1);
            }

            try
            {
                DataCheckPricelist.Clear();

                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("SP_CheckPricelist", Conn);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@PricelistType", PricelistType);
                Cmd.Parameters.AddWithValue("@SubGroup2Id", txtSubGroup2ID.Text);
                Cmd.Parameters.AddWithValue("@Ukuran1From", Convert.ToDecimal(txtUkuran1From.Text == "" ? "-1" : txtUkuran1From.Text));
                Cmd.Parameters.AddWithValue("@Ukuran1To", Convert.ToDecimal(txtUkuran1To.Text == "" ? "-1" : txtUkuran1To.Text));
                Cmd.Parameters.AddWithValue("@Ukuran2From", Convert.ToDecimal(txtUkuran2From.Text == "" ? "-1" : txtUkuran2From.Text));
                Cmd.Parameters.AddWithValue("@Ukuran2To", Convert.ToDecimal(txtUkuran2To.Text == "" ? "-1" : txtUkuran2To.Text));
                Cmd.Parameters.AddWithValue("@Ukuran3From", Convert.ToDecimal(txtUkuran3From.Text == "" ? "-1" : txtUkuran3From.Text));
                Cmd.Parameters.AddWithValue("@Ukuran3To", Convert.ToDecimal(txtUkuran3To.Text == "" ? "-1" : txtUkuran3To.Text));
                Cmd.Parameters.AddWithValue("@Ukuran4From", Convert.ToDecimal(txtUkuran4From.Text == "" ? "-1" : txtUkuran4From.Text));
                Cmd.Parameters.AddWithValue("@Ukuran4To", Convert.ToDecimal(txtUkuran4To.Text == "" ? "-1" : txtUkuran4To.Text));
                Cmd.Parameters.AddWithValue("@Ukuran5From", Convert.ToDecimal(txtUkuran5From.Text == "" ? "-1" : txtUkuran5From.Text));
                Cmd.Parameters.AddWithValue("@Ukuran5To", Convert.ToDecimal(txtUkuran5To.Text == "" ? "-1" : txtUkuran5To.Text));
                Cmd.Parameters.AddWithValue("@DeliveryMethod", DeliveryMethod);
                Cmd.Parameters.AddWithValue("@AccountList", AccountList);
                Cmd.Parameters.AddWithValue("@Criteria", Criteria);
                Cmd.Parameters.AddWithValue("@OldFullItemID", OldFullItemID);
                Cmd.Parameters.AddWithValue("ManufacturerID", cmbManufactureID.SelectedIndex != 0 ? cmbManufactureID.Text : "");
                Cmd.Parameters.AddWithValue("MerekID", cmbMerekID.SelectedIndex != 0 ? cmbMerekID.Text : "");
                Cmd.Parameters.AddWithValue("GolonganID", cmbGolonganID.SelectedIndex != 0 ? cmbGolonganID.Text : "");
                Cmd.Parameters.AddWithValue("KodeBerat", cmbKodeBerat.SelectedIndex != 0 ? cmbKodeBerat.Text : "");
                Cmd.Parameters.AddWithValue("SpecID", cmbSpecID.SelectedIndex != 0 ? cmbSpecID.Text : "");
                Cmd.Parameters.AddWithValue("Bentuk", cmbBentuk.SelectedIndex != 0 ? cmbBentuk.Text : "");
           

                Dr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);   
                while (Dr.Read())
                {
                    if (dgvAccountList.RowCount > 0)
                    {
                        DataCheckPricelist = new Dictionary<string, string>();
                        DataCheckPricelist.Add(Convert.ToString(Dr[0]), Convert.ToString(Dr[1]));
                    }
                    else
                    {
                        DataCheckPricelist = new Dictionary<string, string>();
                        DataCheckPricelist.Add(Convert.ToString(Dr[0]), "");                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conn.Close();
            }

            return DataCheckPricelist;
        }      

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgvSubGroupPricelistDetails.RowCount > 0)
            {
                int SeqGroupNo = 0;
                if (Parent.dgvPricelistDetails.RowCount == 0)
                {
                    Parent.setHeaderDgvPricelistDetails();
                    SeqGroupNo = 1;
                }
                else
                {
                    //GET SEQ GROUP NO FROM GRIDVIEW PRICELIST DETAILS                    
                    SeqGroupNo = 1 + (from Pricelist in Parent.dgvPricelistDetails.Rows.Cast<DataGridViewRow>()
                                        select Pricelist).Max(Pricelist => Convert.ToInt32(Pricelist.Cells["SeqGroupNo"].Value));
                    //END SEQ GROUP NO FROM GRIDVIEW PRICELIST DETAILS
                }

                //GENERATE 
                for (int i = 0; i < dgvSubGroupPricelistDetails.RowCount; i++)
                {
                    int No = 0;
                    if (Parent.dgvPricelistDetails.RowCount == 0)
                    {
                        No = Convert.ToInt32(dgvSubGroupPricelistDetails.Rows[i].Cells[0].Value);
                    }
                    else
                    {
                        int LastNoPricelistDetails = Parent.dgvPricelistDetails.RowCount;
                        No = LastNoPricelistDetails + 1;
                    }

                    string FullItemID = Convert.ToString(dgvSubGroupPricelistDetails.Rows[i].Cells[1].Value);
                    string [] SplitFullItemID = FullItemID.Split('.');
                    string ItemID = "";
                    if (SplitFullItemID.Length > 3)
                    {
                        ItemID = Convert.ToString(SplitFullItemID[3]);
                    }

                    string ItemName = Convert.ToString(dgvSubGroupPricelistDetails.Rows[i].Cells[2].Value);
                    string Unit = Convert.ToString(dgvSubGroupPricelistDetails.Rows[i].Cells[3].Value);
                    decimal Tolerance = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[4].Value);
                    decimal Price0 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[5].Value);
                    decimal Price2 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[6].Value);
                    decimal Price3 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[7].Value);
                    decimal Price7 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[8].Value);
                    decimal Price14 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[9].Value);
                    decimal Price21 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[10].Value);
                    decimal Price30 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[11].Value);
                    decimal Price40 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[12].Value);
                    decimal Price45 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[13].Value);
                    decimal Price60 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[14].Value);
                    decimal Price75 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[15].Value);
                    decimal Price90 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[16].Value);
                    decimal Price120 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[17].Value);
                    decimal Price150 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[18].Value);
                    decimal Price180 = Convert.ToDecimal(dgvSubGroupPricelistDetails.Rows[i].Cells[19].Value);
                    Parent.dgvPricelistDetails.Rows.Add(No, FullItemID, ItemName, Unit, Tolerance, Price0, Price2,
                    Price3, Price7, Price14, Price21, Price30, Price40, Price45, Price60, Price75, Price90, Price120,
                    Price150, Price180, txtGroupID.Text, txtSubGroup1ID.Text, txtSubGroup2ID.Text, txtSubGroup2.Text, ItemID,
                    txtUkuran1From.Text, txtUkuran1To.Text, txtUkuran2From.Text, txtUkuran2To.Text, txtUkuran3From.Text, txtUkuran3To.Text,
                    txtUkuran4From.Text, txtUkuran4To.Text, txtUkuran5From.Text, txtUkuran5To.Text, SeqGroupNo, cmbManufactureID.SelectedIndex == 0 ? "" : cmbManufactureID.Text,
                    cmbMerekID.SelectedIndex == 0 ? "" : cmbMerekID.Text, cmbGolonganID.SelectedIndex == 0 ? "" : cmbGolonganID.Text, cmbKodeBerat.SelectedIndex == 0 ? "" : cmbKodeBerat.Text, cmbSpecID.SelectedIndex == 0 ? "" : cmbSpecID.Text, cmbBentuk.SelectedIndex == 0 ? "" : cmbBentuk.Text);                                    
                }
                
                Parent.dgvPricelistDetails.AutoGenerateColumns = true;
                Parent.dgvPricelistDetails.Refresh();
                Parent.dgvPricelistDetails.AutoResizeColumns();
                Parent.FormatDataNumeric();
                //END GENERATE 
            }
            else
            {
                MessageBox.Show("Silahkan generate Pricelist terlebih dahulu");
                return;
            }
            this.Close();
        }

        private void FormatDataNumeric()
        {
            for (int i = 5; i < 20; i++)
            {
                dgvSubGroupPricelistDetails.Columns[i].DefaultCellStyle.Format = "N2";
            }            
        }

        private void ClearDgvPricelistDetails()
        {
            if (dgvSubGroupPricelistDetails.RowCount > 0)
            {
                dgvSubGroupPricelistDetails.DataSource = null;
            }
        }

        private void txtPriceCash_TextChanged(object sender, EventArgs e)
        {
            ClearDgvPricelistDetails();
        }

        private void txtTolerance_TextChanged(object sender, EventArgs e)
        {
            ClearDgvPricelistDetails();
        }
        
        public void setHeaderDgvPricelistDetails()
        {
            if (dgvSubGroupPricelistDetails.RowCount > 0)
            {
                if (PricelistType.ToUpper() == "BELI")
                {
                    for (int i = 6; i <= 19; i++)
                    {
                        dgvSubGroupPricelistDetails.Columns[i].Visible = false;
                    } 
                }                
            }
        }

        private void cmbManufactureID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetGrid();
        }

        private void ResetGrid()
        {
            dgvSubGroupPricelistDetails.DataSource = null;
        }

        private void cmbMerekID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetGrid();
        }

        private void cmbGolonganID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetGrid();
        }

        private void cmbKodeBerat_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetGrid();
        }

        private void cmbSpecID_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetGrid();
        }

        private void cmbBentuk_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
