using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace ISBS_New.PopUp.Attachment
{
    public partial class Attachment : MetroFramework.Forms.MetroForm
    {

        #region Inisiasi

        private SqlConnection Conn;
        private SqlTransaction Trans;
        private SqlDataReader Dr;
        private SqlDataAdapter Da;
        private DataTable Dt;
        private DataSet Ds;
        private SqlCommand Cmd;
        private string Query;
        private int Index;

        public Attachment()
        {
            InitializeComponent();
        }

        private void Attachment_Load(object sender, EventArgs e)
        {
            this.Location = new Point(885, 29);
        }

        #endregion

        #region Funtion
        public void GetData(string FullItemID)
        {
            Conn = ConnectionString.GetConnection();

            Query = "Select  FileName, ContentType, FileSize, Id From [tblAttachments] Where ReffTableName = 'PurchQuotationH' And ReffTransId = '";// +PQNumber + "'";
            Da = new SqlDataAdapter(Query, Conn);
            Dt = new DataTable();
            Da.Fill(Dt);

            dgvAttachment.AutoGenerateColumns = true;
            dgvAttachment.DataSource = Dt;
            dgvAttachment.Refresh();

            dgvAttachment.AutoResizeColumns();
            dgvAttachment.Columns["Id"].Visible = false;
            Conn.Close();
        }

        public void RefreshGrid(string PQNumber)
        {
            Conn = ConnectionString.GetConnection();

            if (dgvAttachment.RowCount - 1 <= 0)
            {
                dgvAttachment.ColumnCount = 5;
                dgvAttachment.Columns[0].Name = "FileName";
                dgvAttachment.Columns[1].Name = "ContentType";
                dgvAttachment.Columns[2].Name = "File Size (kb)";
                dgvAttachment.Columns[3].Name = "Attachment";
                dgvAttachment.Columns[4].Name = "Id";

                dgvAttachment.Columns["Attachment"].Visible = false;
                dgvAttachment.Columns["Id"].Visible = false;
            }

            Query = "Select * From [tblAttachments] Where ReffTableName = 'PurchQuotationH' And ReffTransId = '" + PQNumber + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();

            while (Dr.Read())
            {
                this.dgvAttachment.Rows.Add(Dr["FileName"], Dr["ContentType"], Dr["FileSize"], "", Dr["Id"]);
                //test.Add((byte[])Dr["Attachment"]);
            }

            dgvAttachment.AutoResizeColumns();
            Conn.Close();

        }
        #endregion

        private void dgvAttachment_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            SaveFile();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
            String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
            String ContentType = dgvAttachment.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["ContentType"].Value.ToString();

            if (ContentType == "jpg" || ContentType == "png" || ContentType == "jpeg" || ContentType == "bmp" || ContentType == "gif" || ContentType == "jpg")
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand(Query, Conn);
                Cmd.CommandText = ("select attachment from [dbo].[tblAttachments] where id ='" + fileid + "'");
                //cmd.Parameters.AddWithValue("@EntryID", Convert.ToInt32(textBox1.Text));

                var da = new SqlDataAdapter(Cmd);
                var ds = new DataSet();
                da.Fill(ds, "attachment");
                int count = ds.Tables["attachment"].Rows.Count;

                if (count > 0)
                {
                    var data = (Byte[])ds.Tables["attachment"].Rows[count - 1]["attachment"];
                    var stream = new MemoryStream(data);
                    pictureBox1.Image = Image.FromStream(stream);
                }

                //optional
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                //REFERENCE : https://stackoverflow.com/questions/10454595/loading-picturebox-image-from-database
            }
            else
            {
                MessageBox.Show("Preview only available for picture!" + Environment.NewLine + "Save the file to preview it");
                SaveFile();
            }
        }

        private void SaveFile()
        {
            String fileid = dgvAttachment.CurrentRow.Cells["Id"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["Id"].Value.ToString();
            String fileName = dgvAttachment.CurrentRow.Cells["FileName"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["FileName"].Value.ToString();
            String ContentType = dgvAttachment.CurrentRow.Cells["ContentType"].Value == null ? "" : dgvAttachment.CurrentRow.Cells["ContentType"].Value.ToString();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ContentType;
            sfd.FileName = fileName + "." + ContentType;
            sfd.Filter = "Pdf Files (*.pdf)|*.pdf|Text files (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.AddExtension = true;

            if (ContentType == "pdf")
            {
                sfd.FilterIndex = 1;
            }
            else if (ContentType == "txt")
            {
                sfd.FilterIndex = 2;
            }
            else
            {
                sfd.FilterIndex = 3;
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

            //string path = System.AppDomain.CurrentDomain.BaseDirectory + fileName + "." + ContentType;
            if (sfd.ShowDialog() != DialogResult.Cancel)
            {
                FileStream objFileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                objFileStream.Write(data, 0, data.Length);
                objFileStream.Close();
                //MessageBox.Show("Data tersimpan!");
            }
            if (sfd.FileName.Contains("\\"))
            {
                System.Diagnostics.Process.Start(sfd.FileName);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

    }
}
