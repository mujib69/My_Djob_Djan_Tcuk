using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ISBS_New.PopUp.Item
{
    public partial class Item : MetroFramework.Forms.MetroForm
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

        public Item()
        {
            InitializeComponent();
        }

        private void Item_Load(object sender, EventArgs e)
        {
            this.Location = new Point(885, 29);
        }

        #endregion

        #region Funtion
        public void GetData(string FullItemID)
        {
            Conn = ConnectionString.GetConnection();
            Query = "Select GroupID,GroupDeskripsi,SubGroup1ID,SubGroup1Deskripsi,SubGroup2ID,SubGroup2Deskripsi,ItemId,FullItemID,ItemDeskripsi From [dbo].[InventTable] where FullItemID='" + FullItemID + "'";
            Cmd = new SqlCommand(Query, Conn);
            Dr = Cmd.ExecuteReader();
            while (Dr.Read())
            {
                lblGroupId.Text = Dr[0].ToString();
                lblGroupName.Text = Dr[1].ToString();
                lblSubGroup1Id.Text = Dr[2].ToString();
                lblSubGroup1Name.Text = Dr[3].ToString();
                lblSubGroup2Id.Text = Dr[4].ToString();
                lblSubGroup2Name.Text = Dr[5].ToString();
                lblItemId.Text = Dr[6].ToString();
                lblFullItemId.Text = Dr[7].ToString();
                lblFullItemName.Text = Dr[8].ToString();
            }
        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
