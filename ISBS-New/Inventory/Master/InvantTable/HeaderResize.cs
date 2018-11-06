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
    public partial class HeaderResize : MetroFramework.Forms.MetroForm
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
        private string Mode;
        private string transID;
        public string ResizeType;

        int count; string id;

        private static string itemID;
        public static string ItemID { get { return itemID; } set { itemID = value; } }

        public static List<string> gvFullItemID = new List<string>();

        InquiryResize Parent = new InquiryResize();

        //begin
        //created by : joshua
        //created date : 24 feb 2018
        //description : Check Permission Access
        public int PermissionAccess(string Authority)
        {
            return ConnectionString.CheckPermissionAccess(this.GetType().Name, Authority);
        }
        //end

        public void SetParent(InquiryResize F)
        {
            Parent = F;
        }

        public HeaderResize()
        {
            InitializeComponent();
        }

        private void ResizeForm_Load(object sender, EventArgs e)
        {
            GetDataHeader();
            if (Mode == "New")
                ModeNew();
            else if (Mode == "Edit")
                ModeEdit();
            else if (Mode == "BeforeEdit")
                ModeBeforeEdit();
            else if (Mode == "ModeView")
                ModeView();
        }

        private string GenerateID()
        {
            Conn = ConnectionString.GetConnection();
            count = 0;
            Cmd = new SqlCommand("Select count(*) from [dbo].[ResizeTableH]", Conn);
            count = (Int32)Cmd.ExecuteScalar();
            if (count == 0)
                count++;
            else
            {
                Cmd = new SqlCommand("SELECT TOP 1 [TransId] FROM [dbo].[ResizeTableH] order by [CreatedDate] desc", Conn);
                string[] lastID = Cmd.ExecuteScalar().ToString().Split('-');
                if (lastID[1] != DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM"))
                    count = 1;
                else
                    count = Convert.ToInt32(lastID[2]) + 1;
            }
            if (count.ToString().Length == 1)
                id += "0000" + count;
            else if (count.ToString().Length == 2)
                id += "000" + count;
            else if (count.ToString().Length == 3)
                id += "00" + count;
            else if (count.ToString().Length == 4)
                id += "0" + count;
            else if (count.ToString().Length == 5)
                id += "" + count;
            Conn.Close();
            return "RSZI-" + DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") + "-" + id;
        }

        private void GetDataHeader()
        {
            if (tbxID.Text == "")
            {
                Conn = ConnectionString.GetConnection();
                dataGridView1.Rows.Clear();
                if (dataGridView1.RowCount - 1 <= 0)
                {
                    dataGridView1.ColumnCount = 3;
                    dataGridView1.Columns[0].Name = "No";
                    dataGridView1.Columns[1].Name = "FullItemID";
                    dataGridView1.Columns[2].Name = "ItemName";
                }
                if (Mode == "New")
                {
                    if (itemID != null)
                    {
                        tbxID.Text = itemID;
                        Cmd = new SqlCommand("select [ItemDeskripsi] from [dbo].[InventTable] where [FullItemID] = '" + tbxID.Text + "'; ", Conn);
                        tbxName.Text = Cmd.ExecuteScalar().ToString();

                        btnAdd.Enabled = true; btnDelete.Enabled = true;
                    }
                }
                else if (Mode == "BeforeEdit" || Mode == "Edit")
                {
                    Query = "Select [ToFullItemId], [ToItemName] from [dbo].[ResizeTableD] where [TransId] ='" + transID + "' and status != '02'; ";
                    Cmd = new SqlCommand(Query, Conn);
                    Dr = Cmd.ExecuteReader();
                    Index = 1;
                    while (Dr.Read())
                    {
                        this.dataGridView1.Rows.Add(Index.ToString(), Dr["ToFullItemId"], Dr["ToItemName"]);
                        Index++;
                    }
                    Dr.Close();
                }
                else if (Mode == "Edit")
                {

                }
                Conn.Close();

                dataGridView1.ReadOnly = true;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.DefaultCellStyle.BackColor = Color.LightGray;
                dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
       
            if (ResizeType == "A" && dataGridView1.RowCount >0)
            {
                MessageBox.Show("Data yang bisa dipilih hanya 1, karena Resize Type = A.");
                goto Outer;
            
            }
            PopUpInvent f = new PopUpInvent();
            PopUpInvent.ItemID = tbxID.Text;
            gvFullItemID.Clear();
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                gvFullItemID.Add(dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString());
            }
            f.ResizeType = txtResizeType.Text.Trim();
            f.ShowDialog();
            if (PopUpInvent.FullItemID.Count != 0 && PopUpInvent.exit != 'X')
            {
                Conn = ConnectionString.GetConnection();
                Query = "Select FullItemID, ItemDeskripsi from InventTable where ";
                for (int i = 0; i < PopUpInvent.FullItemID.Count; i++)
                {
                    Query += "FullItemID = '" + PopUpInvent.FullItemID[i] + "' ";
                    if (PopUpInvent.FullItemID.Count > 1 && i != Convert.ToInt32(PopUpInvent.FullItemID.Count - 1))
                        Query += "or ";
                }
                Cmd = new SqlCommand(Query, Conn);
                Dr = Cmd.ExecuteReader();
                while (Dr.Read())
                {
                    this.dataGridView1.Rows.Add((dataGridView1.Rows.Count + 1).ToString(), Dr["FullItemID"], Dr["ItemDeskripsi"]);
                }
                Dr.Close();
                Conn.Close();
            }

            Outer:;
        }

        private string Validation()
        {
            string check = "";
            if (tbxID.Text == String.Empty || tbxName.Text == String.Empty || dataGridView1.RowCount == 0)
            {
                MetroFramework.MetroMessageBox.Show(this, "Must fill all the data!", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                check = "X";
            }
            return check;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Validation() != "X")
            {
                Conn = ConnectionString.GetConnection();

                Cmd = new SqlCommand("select top 1 TransId from ResizeTableH where TransId = '" + tbxResizeID.Text + "'", Conn);
                Dr = Cmd.ExecuteReader();
                if (Dr.HasRows)
                {
                    //UPDATE QUERY
                    Query = "update ResizeTableH set [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.GroupName + "', ResizeType='" + txtResizeType.Text.Trim() + "' where TransId = '" + tbxResizeID.Text + "'; ";
                    Cmd = new SqlCommand("Select * from ResizeTableD where TransId = '" + tbxResizeID.Text + "' and status = '01'", Conn);
                    Dr = Cmd.ExecuteReader();
                    int found;
                    while (Dr.Read())
                    {
                        found = 0;
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            if (Dr["ToFullItemId"].ToString() == dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString())
                            {
                                Query += "update ResizeTableD set [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.GroupName + "' where TransId = '" + tbxResizeID.Text + "' and ToFullItemId = '" + Dr["ToFullItemId"].ToString() + "'; ";
                                found += 1;
                                break;
                            }
                        }
                        if (found == 0)
                        {
                            Query += "update ResizeTableD set status = '02', [UpdatedDate] = getdate(), [UpdatedBy] = '" + ControlMgr.GroupName + "' where TransId = '" + tbxResizeID.Text + "' and ToFullItemId = '" + Dr["ToFullItemId"].ToString() + "'; ";
                        }
                    }
                }
                else
                {
                    //INSERT QUERY
                    Query = "insert into [dbo].[ResizeTableH] values ('" + tbxResizeID.Text + "', '" + dateTimePicker1.Value + "', '" + tbxDescription.Text + "', '1', getdate(), '" + ControlMgr.GroupName + "', NULL, NULL, '" + txtResizeType.Text.Trim() + "'); ";
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        Query += "insert into [dbo].[ResizeTableD] ([TransId], [SeqNo], FullItemId, ItemName, ToFullItemId, ToItemName, CreatedBy, CreatedDate, [UpdatedBy],[UpdatedDate], status) values ('" + tbxResizeID.Text + "', '" + Convert.ToInt32(i + 1) + "', '" + tbxID.Text + "', '" + tbxName.Text + "', '" + dataGridView1.Rows[i].Cells["FullItemID"].Value.ToString() + "', '" + dataGridView1.Rows[i].Cells["ItemName"].Value.ToString() + "', '" + ControlMgr.GroupName + "', '" + DateTime.Now + "', NULL, NULL, '01'); ";
                    }
                }
                Cmd = new SqlCommand(Query, Conn);
                int result = Cmd.ExecuteNonQuery();
                if (result == 0)
                    MetroFramework.MetroMessageBox.Show(this, "Cannot save data!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    MetroFramework.MetroMessageBox.Show(this, "Save successful!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ModeBeforeEdit();
                }
                Parent.RefreshGrid();
            }
        }

        private void ResizeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PopUpInvent.FullItemID.Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Search f = new Search();
            f.SetSchemaTable("dbo", "InventTable");
            f.ShowDialog();
            tbxID.Text = ConnectionString.Kode;

            if (tbxID.Text != String.Empty)
            {
                Conn = ConnectionString.GetConnection();
                Cmd = new SqlCommand("select [ItemDeskripsi] from [dbo].[InventTable] where [FullItemID] = '" + tbxID.Text + "'; ", Conn);
                tbxName.Text = Cmd.ExecuteScalar().ToString();
                Conn.Close();

                btnAdd.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void ModeNew()
        {
            tbxResizeID.Text = GenerateID();
            dateTimePicker1.Enabled = false;
            btnAdd.Enabled = false; btnDelete.Enabled = false;
            btnEdit.Enabled = false; btnCancel.Enabled = false; btnExit.Enabled = true; btnSave.Enabled = true;
            if (tbxID.Text != String.Empty && tbxName.Text != String.Empty)
            {
                btnAdd.Enabled = true; btnDelete.Enabled = true;
            }
        }

        private void ModeView()
        {
            btnAdd.Enabled = false; btnDelete.Enabled = false;
            btnSave.Enabled = false; btnCancel.Enabled = false; btnEdit.Enabled = true; btnExit.Enabled = true;
        }

        private void ModeBeforeEdit()
        {
            //tbxResizeID.Text = transID;
            Conn = ConnectionString.GetConnection();
            Query = "select a.[TransDate], a.[Deskripsi], b.FullItemId, b.ItemName, a.ResizeType from ResizeTableH as a left join ResizeTableD as b on a.TransId = b.TransId where a.TransId = '" + tbxResizeID.Text + "'; ";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                dateTimePicker1.Value = Convert.ToDateTime(Dr["TransDate"]);
                tbxDescription.Text = Dr["Deskripsi"].ToString();
                tbxID.Text = Dr["FullItemId"].ToString();
                tbxName.Text = Dr["ItemName"].ToString();
                txtResizeType.Text = Dr["ResizeType"].ToString();
            }
            button1.Enabled = false;
            tbxDescription.Enabled = false; dateTimePicker1.Enabled = false;
            btnAdd.Enabled = false; btnDelete.Enabled = false;
            btnSave.Enabled = false; btnCancel.Enabled = false; btnEdit.Enabled = true; btnExit.Enabled = true;
        }

        private void ModeEdit()
        {
            button1.Enabled = false;
            tbxDescription.Enabled = true;
            btnAdd.Enabled = true; btnDelete.Enabled = true;
            btnSave.Enabled = true; btnCancel.Enabled = true; btnExit.Enabled = true; btnEdit.Enabled = false;
        }

        #region SetMode
        public void SetMode(string tmpMode, string tmpNumber, string tmpResizeType)
        {
            Mode = tmpMode;
            transID = tmpNumber;
            ResizeType = tmpResizeType;
            txtResizeType.Text = tmpResizeType;
        }
        #endregion

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //begin
            //updated by : joshua
            //updated date : 24 feb 2018
            //description : check permission access
            if (this.PermissionAccess(ControlMgr.Edit) > 0)
            {
                Mode = "Edit";
                ModeEdit();
                GetDataHeader();
            }
            else
            {
                MessageBox.Show(ControlMgr.PermissionDenied);
            }
            //end            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Mode = "BeforeEdit";
            ModeBeforeEdit();
            GetDataHeader();
        }
    }
}
