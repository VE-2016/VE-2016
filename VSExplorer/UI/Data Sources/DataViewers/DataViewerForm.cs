using MiniSqlQuery.Core.DbModel;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class DataViewerForm : Form
    {
        public DataViewerForm()
        {
            InitializeComponent();

            C = DataSourceWizard.GetConnections();

            dg = dataGridView1;
        }

        private ArrayList C { get; set; }

        private DataGridView dg { get; set; }

        public void Load(DbModelObjectBase model)
        {
            if (model.GetType() == typeof(DbModelTable))
            {
                DbModelTable db = model as DbModelTable;
                string tableName = db.Name;

                DataTable datatable = new DataTable();

                string connectionString = C[1] as string;

                DataConnection dc = new DataConnection();
                dc.connectionString = connectionString;
                connectionString = dc.GetConnectionStringr();

                string selectCommand = "SELECT * FROM " + tableName;

                SqlDataAdapter da = new SqlDataAdapter(selectCommand, connectionString);

                da.Fill(datatable);

                dg.DataSource = datatable;
            }
        }
    }
}