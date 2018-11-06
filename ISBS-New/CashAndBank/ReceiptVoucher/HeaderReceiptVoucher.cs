using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace ISBS_New.CashAndBank.ReceiptVoucher
{
    public partial class HeaderReceiptVoucher : MetroFramework.Forms.MetroForm
    {
        private SqlConnection Conn;
        private SqlCommand Cmd;
        private SqlCommand Cmd2;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;

        string Mode, Query = null;
        string RVNo = "";
        int Index;
        List<string> sSelectedFile, FileName, Extension;
        List<byte[]> attachByte = new List<byte[]>();
      

        CashAndBank.ReceiptVoucher.InquiryReceiptVoucher Parent;

        public void SetParent(CashAndBank.ReceiptVoucher.InquiryReceiptVoucher F)
        {
            Parent = F;
        }

        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }

        public void SetMode(string tmpMode, string tmpRVNo)
        {
            Mode = tmpMode;
            RVNo = tmpRVNo;
        }

        public HeaderReceiptVoucher()
        {
            InitializeComponent();
        }

        private void HeaderReceiptVoucher_Load(object sender, EventArgs e)
        {
            SetHeaderAttachment();
            if (Mode == "New")
            {
                ModeNew();
            }
            else if (Mode == "Edit")
            {
                ModeEdit();
            }
            else if (Mode == "BeforeEdit")
            {
                GetDataHeader();
                ModeBeforeEdit();
            }
        }

        private void ModeBeforeEdit()
        {
            Mode = "BeforeEdit";

            GetDataHeader();

            dtRVDate.Enabled = false;
            txtKetTransferTunai.Enabled = false;
            btnLookupNoRekCust.Enabled = false;
            txtNominal.Enabled = false;
            btnLookupCustomer.Enabled = false;
            btnLookupBank.Enabled = false;
            txtNotes.Enabled = false;

            dtJthTempo.Enabled = false;
            dtTglCair.Enabled = false;
            dtPending.Enabled = false;
            dtTglTolak.Enabled = false;
            txtAlasanPending.Enabled = false;
            txtAlasanTolak.Enabled = false;

            SetHeaderAttachment();
            dgvAttachment.DefaultCellStyle.BackColor = Color.LightGray;           
            btnUpload.Enabled = false;
            btnDelete.Enabled = false;
            btnDownload.Enabled = false;

            btnSave.Visible = false;
            btnExit.Visible = true;
            btnEdit.Visible = true;
            btnCancel.Visible = false;

            if (CheckStatusRV() == "02")
            {
                btnActive.Enabled = true;
                btnInactive.Enabled = false;
            }
            else
            {
                btnActive.Enabled = false;
                btnInactive.Enabled = true;
            }
            
        }

        private void ModeEdit()
        {
            Mode = "Edit";

            btnActive.Enabled = false;
            btnInactive.Enabled = false;

            if (txtPaymentMethod.Text.ToUpper() == "TRANSFER")
            {
                txtKetTransferTunai.Enabled = true;
                btnLookupNoRekCust.Enabled = true;
                txtNominal.Enabled = true;
                btnLookupCustomer.Enabled = true;
                btnLookupBank.Enabled = true;
            }
            else
            {
                dtJthTempo.Enabled = true;
                dtTglCair.Enabled = true;
                dtPending.Enabled = true;
                dtTglTolak.Enabled = true;

                txtAlasanPending.Enabled = true;
                txtAlasanTolak.Enabled = true;
            }
            
            txtNotes.Enabled = true;

            dgvAttachment.DefaultCellStyle.BackColor = Color.White;
            btnUpload.Enabled = true;
            btnDelete.Enabled = true;
            btnDownload.Enabled = true;

            btnSave.Visible = true;
            btnExit.Visible = false;
            btnEdit.Visible = false;
            btnCancel.Visible = true;  
        }
      
        private void ModeNew()
        {
            RVNo = "";

            btnSave.Visible = true;
            btnExit.Visible = true;
            btnEdit.Visible = false;
            btnCancel.Visible = false;

            txtPaymentMethod.Text = "TRANSFER";

            dtTglTolak.CustomFormat = " ";
            dtTglTolak.Format = DateTimePickerFormat.Custom;

            dtPending.CustomFormat = " ";
            dtPending.Format = DateTimePickerFormat.Custom;

            dgvAttachment.Rows.Clear();
        }

        private void GetDataHeader()
        {
            try
            {
                if (RVNo == "")
                {
                    RVNo = txtRevNo.Text.Trim();
                }
                Conn = ConnectionString.GetConnection();
                Query = "SELECT [RV_Date],[RV_No],[Cust_Id],[Cust_Name],[Payment_Method],[Bank_Id],[Bank_Name],[Giro_No] ";
                Query += ",[Receipt_No],[Rek_Cust],[Payment_DueDate],[Tgl_Cair],[Tgl_Tolak],[Tgl_Pending],[Total_Payment] ";
                Query += ",[Signed_Amount],[StatusCode],[Notes],[Notes2],[Notes3]";
                Query += "FROM ReceiptVoucher_H WHERE RV_No='" + RVNo + "';";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                while (Dr.Read())
                {
                    dtRVDate.Text = Convert.ToString(Dr["RV_Date"]);
                    txtRevNo.Text = Convert.ToString(Dr["RV_No"]);
                    txtCustomer.Text = Convert.ToString(Dr["Cust_Id"]);
                    txtCustomerName.Text = Convert.ToString(Dr["Cust_Name"]);
                    txtPaymentMethod.Text = Convert.ToString(Dr["Payment_Method"]);
                    txtBank.Text = Convert.ToString(Dr["Bank_Id"]);
                    txtBankName.Text = Convert.ToString(Dr["Bank_Name"]);
                    txtNoGiro.Text = Convert.ToString(Dr["Giro_No"]);
                    txtKetTransferTunai.Text = Convert.ToString(Dr["Receipt_No"]);
                    txtNoRekCust.Text = Convert.ToString(Dr["Rek_Cust"]);
                    dtJthTempo.Text = Convert.ToString(Dr["Payment_DueDate"]);
                    dtTglCair.Text = Convert.ToString(Dr["Tgl_Cair"]);
                    string TglTolak = Convert.ToString(Dr["Tgl_Tolak"]);
                    if (TglTolak != "")
                    {
                        dtTglTolak.Text = Convert.ToString(Dr["Tgl_Tolak"]);
                    }
                    else
                    {
                        dtTglTolak.CustomFormat = " ";
                        dtTglTolak.Format = DateTimePickerFormat.Custom;
                    }

                    string TglPending = Convert.ToString(Dr["Tgl_Pending"]);
                    if (TglPending != "")
                    {
                        dtPending.Text = Convert.ToString(Dr["Tgl_Pending"]);
                    }
                    else
                    {
                        dtPending.CustomFormat = " ";
                        dtPending.Format = DateTimePickerFormat.Custom;
                    }

                    double Nominal =  Convert.ToDouble(Dr["Total_Payment"]);
                    txtNominal.Text = Nominal.ToString("N2");

                    double TotalPelunasan = Convert.ToDouble(Dr["Signed_Amount"]);
                    txtTotalPelunasan.Text = TotalPelunasan.ToString("N2");

                    double SisaBelumTerpakai = Nominal - TotalPelunasan;
                    txtSisaBelumTerpakai.Text = SisaBelumTerpakai.ToString("N2");

                    txtNotes.Text = Convert.ToString(Dr["Notes"]);
                    txtAlasanTolak.Text = Convert.ToString(Dr["Notes2"]);
                    txtAlasanPending.Text = Convert.ToString(Dr["Notes3"]);

                    Query = "SELECT BankGroupName FROM Rekening WHERE No_Rekening = '" + txtNoRekCust.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    txtRekCustName.Text = Convert.ToString(Cmd.ExecuteScalar());
                 
                }
                Dr.Close();
                

                attachByte.Clear();
                dgvAttachment.Rows.Clear();

                Query = "SELECT Id, FileName, ContentType, fileSize, attachment FROM [tblAttachments] WHERE ReffTableName = 'ReceiptVoucher_H' And ReffTransId = '" + txtRevNo.Text + "'";
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();

                int k = 1;
                while (Dr.Read())
                {
                    this.dgvAttachment.Rows.Add(k, Dr["FileName"], Dr["ContentType"], Dr["fileSize"], Dr["Id"], "");
                    attachByte.Add((byte[])Dr["attachment"]);
                    k++;
                }
                Dr.Close();

                dgvAttachment.AutoResizeColumns();

                Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HeaderReceiptVoucher_Shown(object sender, EventArgs e)
        {
            this.Location = new Point(170, 63);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetHeaderAttachment()
        {
            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 6;
                dgvAttachment.Columns[0].Name = "No";
                dgvAttachment.Columns[1].Name = "FileName";
                dgvAttachment.Columns[2].Name = "ContentType";
                dgvAttachment.Columns[3].Name = "FileSize";
                dgvAttachment.Columns[4].Name = "Id";
                dgvAttachment.Columns[5].Name = "Attachment";

                dgvAttachment.Columns[0].HeaderText = "No";
                dgvAttachment.Columns[1].HeaderText = "File Name";
                dgvAttachment.Columns[2].HeaderText = "Content Type";
                dgvAttachment.Columns[3].HeaderText = "File Size (Kb)";
                dgvAttachment.Columns[4].HeaderText = "Id";
                dgvAttachment.Columns[5].HeaderText = "Attachment";

                dgvAttachment.ReadOnly = true;

                dgvAttachment.Columns[4].Visible = false;
                dgvAttachment.Columns[5].Visible = false;

                dgvAttachment.AutoResizeColumns();
            }
        }

        private void btnLookupNoRekCust_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "NoRekening";
            tmpSearch.Order = "NoRekening ASC";
            tmpSearch.Table = "[dbo].[Rekening]";
            tmpSearch.QuerySearch = "SELECT No_Rekening AS NoRekening, Cabang, BankGroupName, Pemilik FROM [dbo].[Rekening] WHERE Aktif = 1 ";
            tmpSearch.FilterText = new string[] { "NoRekening", "Cabang", "BankGroupName", "Pemilik" };
            tmpSearch.Select = new string[] { "NoRekening", "Cabang", "BankGroupName", "Pemilik" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtNoRekCust.Text = ConnectionString.Kodes[0];
                txtRekCustName.Text = ConnectionString.Kodes[2];                

                ConnectionString.Kodes = null;
            }  
        }

        private void btnLookupCustomer_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "CustId";
            tmpSearch.Order = "CustId ASC";
            tmpSearch.Table = "[dbo].[CustTable]";
            tmpSearch.QuerySearch = "SELECT CustId, CustName, Gol_Prsh AS GolPrsh FROM [dbo].[CustTable]";
            tmpSearch.FilterText = new string[] { "CustId", "CustName", "GolPrsh" };
            tmpSearch.Select = new string[] { "CustId", "CustName", "GolPrsh" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtCustomer.Text = ConnectionString.Kodes[0];
                txtCustomerName.Text = ConnectionString.Kodes[1];

                ConnectionString.Kodes = null;
            }  
        }

        private void btnLookupBank_Click(object sender, EventArgs e)
        {
            SearchQueryV1 tmpSearch = new SearchQueryV1();
            tmpSearch.PrimaryKey = "BankId";
            tmpSearch.Order = "BankId ASC";
            tmpSearch.Table = "[dbo].[BankTable]";
            tmpSearch.QuerySearch = "SELECT BankGroupId, BankId, AccountNo, BankName, AccountID, AccountName  FROM [dbo].[BankTable]";
            tmpSearch.FilterText = new string[] { "BankGroupId", "BankId", "AccountNo", "BankName", "AccountID", "AccountName" };
            tmpSearch.Select = new string[] { "BankGroupId", "BankId", "AccountNo", "BankName", "AccountID", "AccountName" };
            tmpSearch.ShowDialog();
            if (ConnectionString.Kodes != null)
            {
                txtBank.Text = ConnectionString.Kodes[1];
                txtBankName.Text = ConnectionString.Kodes[3];

                ConnectionString.Kodes = null;
            }
        }

        private void dtRVDate_ValueChanged(object sender, EventArgs e)
        {
            dtJthTempo.Value = dtRVDate.Value;
            dtTglCair.Value = dtRVDate.Value;
        }

        private void txtNominal_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtNominal_Leave(object sender, EventArgs e)
        {
            if (txtNominal.Text != "")
            {
                double d = 0;
                if (Convert.ToDecimal(txtNominal.Text) == 0)
                {
                    d = double.Parse("1");
                    txtNominal.Text = d.ToString("N2");

                    MessageBox.Show("Nominal harus lebih dari 0");
                    return;
                }
                else
                {
                    d = double.Parse(txtNominal.Text);
                    txtNominal.Text = d.ToString("N2");

                    double SisaBelumTerpakai = Convert.ToDouble(txtNominal.Text) - Convert.ToDouble(txtTotalPelunasan.Text);
                    txtSisaBelumTerpakai.Text = SisaBelumTerpakai.ToString("N2");

                }    
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Pdf Files (*.pdf)|*.pdf|Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            choofdlog.FilterIndex = 3;
            choofdlog.Multiselect = true;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                FileName = new List<string>();
                Extension = new List<string>();
                sSelectedFile = new List<string>();

                int i = 0;

                foreach (string file in choofdlog.FileNames)
                {
                    FileStream objFileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    int filelength = Convert.ToInt32(objFileStream.Length);
                    byte[] data = new byte[filelength];

                    objFileStream.Read(data, 0, filelength);
                    objFileStream.Close();

                    string tempFullName = Path.GetFileName(file);
                    string[] tempSplit = tempFullName.Split('.');

                    FileName.Add(tempSplit[0]);
                    Extension.Add(tempSplit[tempSplit.Count() - 1]);
                    int filesize = filelength / 1024;

                    this.dgvAttachment.Rows.Add(dgvAttachment.RowCount + 1, FileName[i], Extension[i], filesize.ToString(), "", System.Text.Encoding.UTF8.GetString(data));
                    attachByte.Add(data);
                    i++;
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
                String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
                String ContentType = dgvAttachment.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["ContentType"].Value.ToString();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ContentType;
                sfd.FileName = fileName + "." + ContentType;
                sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                sfd.AddExtension = true;

                if (ContentType == "pdf")
                {
                    sfd.FilterIndex = 1;
                }
                else
                {
                    sfd.FilterIndex = 2;
                }

                if (String.IsNullOrEmpty(fileid))
                {
                    MessageBox.Show("File tidak ada dalam database / belum di masukkan.");
                    return;
                }

                Conn = ConnectionString.GetConnection();
                Query = "Select Attachment From tblAttachments Where Id = '" + fileid + "'";
                Cmd = new SqlCommand(Query, Conn);

                byte[] data = (byte[])Cmd.ExecuteScalar();

                if (sfd.ShowDialog() != DialogResult.Cancel)
                {
                    FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                    objFileStream.Write(data, 0, data.Length);
                    objFileStream.Close();
                    MessageBox.Show("Data tersimpan!");
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk didownload");
                return;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAttachment.RowCount > 0)
            {
                Index = dgvAttachment.CurrentRow.Index;
                DialogResult dialogResult = MessageBox.Show("Apakah data No = " + dgvAttachment.Rows[Index].Cells["No"].Value.ToString() + Environment.NewLine + "FileName = " + dgvAttachment.Rows[Index].Cells["FileName"].Value.ToString() + Environment.NewLine + "Akan dihapus ? ", "Delete Confirmation !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (dgvAttachment.CurrentRow.Index > -1)
                    {
                        attachByte.RemoveAt(Index);
                        dgvAttachment.Rows.RemoveAt(Index);
                        SortNoDataGridAttachment();
                    }
                }
            }
            else
            {
                MessageBox.Show("Silahkan pilih data untuk dihapus");
                return;
            }
        }

        private void SortNoDataGridAttachment()
        {
            for (int i = 0; i < dgvAttachment.RowCount; i++)
            {
                dgvAttachment.Rows[i].Cells["No"].Value = i + 1;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //begin data validation
            if (txtKetTransferTunai.Text == "")
            {
                MessageBox.Show("Ket Transfer/Tunai harus diisi");
                return;
            }
            else if (txtNominal.Text == "")
            {
                MessageBox.Show("Nominal harus diisi");
                return;
            }
            else if (Convert.ToDecimal(txtNominal.Text) == 0)
            {
                MessageBox.Show("Nominal harus lebih dari 0");
                return;
            }
            else if (txtBank.Text == "")
            {
                MessageBox.Show("Bank harus diisi");
                return;
            }
            else if (txtPaymentMethod.Text.ToUpper() == "TRANSFER" || txtPaymentMethod.Text.ToUpper() == "CHEQUE")
            {
                if (dgvAttachment.RowCount == 0)
                {
                    MessageBox.Show("Documents harus diisi");
                    return;
                }
            }

            if (dtTglTolak.Text != " ")
            {
                if (txtAlasanTolak.Text == "")
                {
                    MessageBox.Show("Alasan Tolak harus diisi");
                    return;
                }
            }

            if (dtPending.Text != " ")
            {
                if (txtAlasanPending.Text == "")
                {
                    MessageBox.Show("Alasan Pending harus diisi");
                    return;
                }
            }
            //end data valdation
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                txtNoGiro.Text = txtNoGiro.Text.Replace("'", "").Trim();
                txtPaymentMethod.Text = txtPaymentMethod.Text.Replace("'", "").Trim();
                txtKetTransferTunai.Text = txtKetTransferTunai.Text.Replace("'", "").Trim();
                txtAlasanTolak.Text = txtAlasanTolak.Text.Replace("'", "").Trim();
                txtAlasanPending.Text = txtAlasanPending.Text.Replace("'", "").Trim();
                txtNotes.Text = txtNotes.Text.Replace("'", "").Trim();
                

                if (Mode == "New")
                {
                    string Jenis = "RV", Kode = "RV";
                    RVNo = ConnectionString.GenerateSequenceNo(Jenis, Kode, "", "", Conn, Trans, Cmd);

                    //Begin Insert Header
                    Query = "INSERT INTO [dbo].[ReceiptVoucher_H]([RV_Date],[RV_No],[Cust_Id],[Cust_Name],[Payment_Method], ";
                    Query += "[Bank_Id],[Bank_Name],[Giro_No],[Receipt_No],[Rek_Cust],[Payment_DueDate],[Tgl_Cair], ";
                    Query += "[Total_Payment],[Signed_Amount],[StatusCode],[Notes],[CreatedDate],[CreatedBy]) ";
                    Query += "VALUES('" + dtRVDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + RVNo + "', '" + txtCustomer.Text + "', ";
                    Query += "'" + txtCustomerName.Text + "', '" + txtPaymentMethod.Text + "', '" + txtBank.Text + "', '" + txtBankName.Text + "', ";
                    Query += "'" + txtNoGiro.Text + "', '" + txtKetTransferTunai.Text + "', '" + txtNoRekCust.Text + "', '" + dtJthTempo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                    Query += "'" + dtTglCair.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', " + Convert.ToDecimal(txtNominal.Text) + ", " + Convert.ToDecimal(txtTotalPelunasan.Text) + ", '01', '" + txtNotes.Text + "', GETDATE(), '" + ControlMgr.UserId + "') ";

                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Insert Header

                    //Insert Attachement
                    for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                    {
                        Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment, CreatedBy, CreatedDate, FileType) Values";
                        Query += "( 'ReceiptVoucher_H', '" + RVNo + "', '";
                        Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[i].Cells["FileSize"].Value.ToString();
                        Query += "',@binaryValue, '" + ControlMgr.UserId + "', GETDATE(), '" + txtPaymentMethod.Text + "'";
                        Query += ");";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, attachByte[i].Length).Value = attachByte[i];
                        Cmd.ExecuteNonQuery();
                    }
                    //End Insert Attachement

                    //Insert Bank System

                    Query = "SELECT AccountNo, AccountName FROM BankTable WHERE BankId ='" + txtBank.Text + "';";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    string AccountNo = "", AccountName = "";

                    while (Dr.Read())
                    {
                        AccountNo = Convert.ToString(Dr["AccountNo"]);
                        AccountName = Convert.ToString(Dr["AccountName"]);
                    }
                    Dr.Close();


                    Query = "INSERT INTO [dbo].[BankSystem] ([VoucherNo],[VoucherDate],[VoucherDueDate],[AccountNo],[AccountName],[AccountAmount],[CreatedDate],[CreatedBy]) ";
                    Query += "VALUES('" + RVNo + "', '" + dtRVDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + dtJthTempo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', ";
                    Query += "'" + AccountNo + "', '" + AccountName + "', '" + Convert.ToDecimal(txtNominal.Text) + "', GETDATE(), '" + ControlMgr.UserId + "') ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    InsertLog(RVNo, "ReceiptVoucher", "01", "N", Conn, Trans, Cmd);

                    Trans.Commit();
                    txtRevNo.Text = RVNo;
                    MessageBox.Show("Data RV No : " + RVNo + " berhasil disimpan.");
                }
                else
                {                    
                    //Begin Update Header
                    Query = "UPDATE ReceiptVoucher_H SET [RV_Date] = '" + dtRVDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                    Query += ",[RV_No] = '" + txtRevNo.Text + "', [Cust_Id] = '" + txtCustomer.Text + "',[Cust_Name] = '" + txtCustomerName.Text + "' ";
                    Query += ",[Payment_Method] = '" + txtPaymentMethod.Text + "',[Bank_Id] = '" + txtBank.Text + "',[Bank_Name] = '" + txtBankName.Text + "' ";
                    Query += ",[Giro_No] = '" + txtNoGiro.Text + "',[Receipt_No] = '" + txtKetTransferTunai.Text + "',[Rek_Cust] = '" + txtNoRekCust.Text + "' ";
                    Query += ",[Payment_DueDate] = '" + dtJthTempo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',[Tgl_Cair] = '" + dtTglCair.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                    Query += ",[Total_Payment] = " + Convert.ToDecimal(txtNominal.Text) + ",[Signed_Amount] = " + Convert.ToDecimal(txtTotalPelunasan.Text) + ",[Notes] = '" + txtNotes.Text + "' ";
                    Query += ",[Notes2] = '" + txtAlasanTolak.Text + "',[Notes3] = '" + txtAlasanPending.Text + "',[UpdatedDate] = GETDATE(),[UpdatedBy] = '" + ControlMgr.UserId + "' ";
                   
                  
                    if (dtPending.Text != " ")
                    {
                        Query += ",[Tgl_Pending] = '" + dtPending.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }

                    if (dtTglTolak.Text != " ")
                    {
                        Query += ",[Tgl_Tolak] = '" + dtTglTolak.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }
                  
                    Query += "WHERE RV_No = '" + txtRevNo.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Update Header

                    //Delete Attachment
                    Query = "DELETE FROM tblAttachments WHERE ReffTransID = '" + txtRevNo.Text + "'";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();
                    //End Delete Attachment

                    //Insert Attachement
                    for (int i = 0; i <= dgvAttachment.RowCount - 1; i++)
                    {
                        Query = "Insert tblAttachments (ReffTableName, ReffTransId, fileName, ContentType, fileSize, attachment, CreatedBy, CreatedDate, FileType) Values";
                        Query += "( 'ReceiptVoucher_H', '" + RVNo + "', '";
                        Query += dgvAttachment.Rows[i].Cells["FileName"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[i].Cells["ContentType"].Value.ToString() + "', '";
                        Query += dgvAttachment.Rows[i].Cells["FileSize"].Value.ToString();
                        Query += "',@binaryValue, '" + ControlMgr.UserId + "', GETDATE(), '" + txtPaymentMethod.Text + "'";
                        Query += ");";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.Parameters.Add("@binaryValue", SqlDbType.VarBinary, attachByte[i].Length).Value = attachByte[i];
                        Cmd.ExecuteNonQuery();
                    }
                    //End Insert Attachement

                    //Update Bank System
                    Query = "SELECT AccountNo, AccountName FROM BankTable WHERE BankId ='" + txtBank.Text + "';";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Dr = Cmd.ExecuteReader();

                    string AccountNo = "", AccountName = "";

                    while (Dr.Read())
                    {
                        AccountNo = Convert.ToString(Dr["AccountNo"]);
                        AccountName = Convert.ToString(Dr["AccountName"]);
                    }
                    Dr.Close();


                    Query = "UPDATE [dbo].[BankSystem] ";
                    Query += "SET [VoucherDate] = '" + dtRVDate.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                    Query += ",[VoucherDueDate] = '" + dtJthTempo.Value.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                    Query += ",[AccountNo] = '" + AccountNo + "' ";
                    Query += ",[AccountName] = '" + AccountName + "' ";
                    Query += ",[AccountAmount] = '" + Convert.ToDecimal(txtNominal.Text) + "' ";
                    Query += ",[UpdatedDate] = GETDATE() ";
                    Query += ",[UpdatedBy] = '" + ControlMgr.UserId + "' ";
                    Query += "WHERE VoucherNo = '" + txtRevNo.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    InsertLog(RVNo, "ReceiptVoucher", "01", "E", Conn, Trans, Cmd);

                    Trans.Commit();
                    txtRevNo.Text = RVNo;
                    MessageBox.Show("Data RV No : " + RVNo + " berhasil diubah.");
                }

                Parent.RefreshGrid();
                ModeBeforeEdit();
            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }
        }

        private void InsertLog(string TransaksiID, string TransCode, string StatusCode, string Action, SqlConnection Conn, SqlTransaction Trans, SqlCommand Cmd)
        {
            Query = "SELECT Deskripsi FROM TransStatusTable WHERE TransCode = '" + TransCode + "' AND StatusCode = '" + StatusCode + "'";
            Cmd = new SqlCommand(Query, Conn, Trans);
            string StatusTransaksi = Convert.ToString(Cmd.ExecuteScalar());

            if (Action == "")
            {
                Query = "SELECT TOP 1 Action FROM ReceiptVoucher_LogTable WHERE TransaksiID = '" + TransaksiID + "' ORDER BY LogDatetime DESC";
                Cmd = new SqlCommand(Query, Conn, Trans);
                Action = Convert.ToString(Cmd.ExecuteScalar());
            }

            Query = "INSERT INTO ReceiptVoucher_LogTable (TransaksiID, StatusTransaksi, Action, UserID, LogDatetime) ";
            Query += "VALUES ('" + TransaksiID + "', '" + StatusTransaksi + "', '" + Action + "', '" + ControlMgr.UserId + "', GETDATE())";
            Cmd = new SqlCommand(Query, Conn, Trans);
            Cmd.ExecuteNonQuery();
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 10 Jul 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                string Check = "";
                Conn = ConnectionString.GetConnection();

                if (txtRevNo.Text != "")
                {
                    Query = "SELECT StatusCode FROM ReceiptVoucher_H WHERE RV_No ='" + txtRevNo.Text + "'";
                    Cmd = new SqlCommand(Query, Conn);
                    Check = Cmd.ExecuteScalar().ToString();
                    if (Check == "02")
                    {
                        MessageBox.Show("InvoiceId = " + txtRevNo.Text + ".\n" + "Tidak dapat diedit karena sudah diproses.");
                        Conn.Close();
                        return;
                    }
                }

                Mode = "Edit";
                ModeEdit();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ModeBeforeEdit();
        }

        private void dtTglTolak_ValueChanged(object sender, EventArgs e)
        {
            dtTglTolak.CustomFormat = "dd/MM/yyyy";
            dtTglTolak.Format = DateTimePickerFormat.Custom;           
        }

        private void dtPending_ValueChanged(object sender, EventArgs e)
        {
            dtPending.CustomFormat = "dd/MM/yyyy";
            dtPending.Format = DateTimePickerFormat.Custom;
        }

        private void btnActive_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("RV No = " + txtRevNo.Text + "\n" + "Apakah data diatas akan diaktifkan ?", "Konfirmasi", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {

                    Conn = ConnectionString.GetConnection();
                    Trans = Conn.BeginTransaction();

                    Query = "UPDATE ReceiptVoucher_H SET StatusCode = '01' WHERE RV_No = '" + txtRevNo.Text + "' ";
                    Cmd = new SqlCommand(Query, Conn, Trans);
                    Cmd.ExecuteNonQuery();

                    InsertLog(RVNo, "ReceiptVoucher", "01", "", Conn, Trans, Cmd);

                    Trans.Commit();

                    MessageBox.Show("Data RV No : " + txtRevNo.Text + " berhasil diaktifkan.");
                    Parent.RefreshGrid();
                    ModeBeforeEdit();
                }
            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }

        }

        private void btnInactive_Click(object sender, EventArgs e)
        {
            try
            {
                Conn = ConnectionString.GetConnection();
                Trans = Conn.BeginTransaction();

                Query = "SELECT COUNT(RV_No) FROM ReceiptVoucher_Dtl WHERE RV_No = '" + txtRevNo.Text + "' ";
                Cmd = new SqlCommand(Query, Conn, Trans);
                int CountData = Convert.ToInt32(Cmd.ExecuteScalar());

                if (CountData == 0)
                {
                    DialogResult dr = MessageBox.Show("RV No = " + txtRevNo.Text + "\n" + "Apakah data diatas akan dinonaktifkan ?", "Konfirmasi", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {


                        Query = "UPDATE ReceiptVoucher_H SET StatusCode = '02' WHERE RV_No = '" + txtRevNo.Text + "' ";
                        Cmd = new SqlCommand(Query, Conn, Trans);
                        Cmd.ExecuteNonQuery();

                        InsertLog(RVNo, "ReceiptVoucher", "02", "", Conn, Trans, Cmd);
                        Trans.Commit();

                        MessageBox.Show("Data RV No : " + txtRevNo.Text + " berhasil dinonaktifkan.");
                        Parent.RefreshGrid();
                        ModeBeforeEdit();
                    }
                }
                else
                {
                    MessageBox.Show("Data dengan RV No : " + txtRevNo.Text + " tidak dapat dinonaktifkan karena sudah diproses.");
                }

            }
            catch (Exception)
            {
                Trans.Rollback();
                return;
            }
            finally
            {
                Conn.Close();
            }   
        }

        private string CheckStatusRV()
        {
            Conn = ConnectionString.GetConnection();

            Query = "SELECT StatusCode FROM ReceiptVoucher_H WHERE RV_No = '" + RVNo + "' ";
            Cmd = new SqlCommand(Query, Conn);
            string StatusCode = Convert.ToString(Cmd.ExecuteScalar());

            Conn.Close();

            return StatusCode;
        }
    }
}
