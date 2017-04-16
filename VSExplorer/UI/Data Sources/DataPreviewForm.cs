using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class DataPreviewForm : Form
    {
        public DataPreviewForm()
        {
            InitializeComponent();
            cb = comboBox1;
            cb.DropDownStyle = ComboBoxStyle.DropDown;

            cb.Text = "database";
            
            cb.DropDownHeight = 1;

            cb.DropDown += Cb_DropDown;
            cb.DropDownClosed += Cb_DropDownClosed;

            dg = dataGridView1;

            lb = listBox1;

            LoadConnections();
        }

        ListBox lb { get; set; }

        public void LoadConnections()
        {
            ArrayList C = DataSourceWizard.GetConnections();
            foreach(string c in C)
            {
                lb.Items.Add(c);
            }
        }

        public DataGridView dg { get; set; }

        private void Cb_DropDownClosed(object sender, EventArgs e)
        {
            this.Controls.Remove(v);
        }

        TreeView v { get; set; }

        private void Cb_DropDown(object sender, EventArgs e)
        {
           
            vv.Bounds = new Rectangle(cb.Location.X, cb.Location.Y + 20, cb.Width, 200);
                        
            this.Controls.Add(vv);
            this.Controls.SetChildIndex(vv, 0);
        }

        ComboBox cb { get; set; }

        TreeView vv { get; set; }

        TableInfo info { get; set; }
        public void LoadData(TreeView vv)
        {
            this.vv = vv;
            this.info = vv.Tag as TableInfo;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (info == null)
                return;
            
            DataTable datatable = info.GetDataTable();

            if (lb.SelectedIndex < 0)
                return;

            string connectionString = lb.Items[lb.SelectedIndex].ToString();

            DataConnection dc = new DataConnection();
            dc.connectionString = connectionString;
            connectionString = dc.GetConnectionStringr();

            string selectCommand = "SELECT * FROM " + info.tableName;

            SqlDataAdapter da = new SqlDataAdapter(selectCommand, connectionString);

            da.Fill(datatable);

            dg.DataSource = datatable;


        }
    }
}
