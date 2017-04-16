using AIMS.Libraries.Scripting.ScriptControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MiniSqlQuery.Core.DbModel;
using System.Collections;
using System.Data.SqlClient;

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

        ArrayList C { get; set; }

        DataGridView dg { get; set; }

        public void Load(DbModelObjectBase model)
        {

            if(model.GetType() == typeof(DbModelTable))
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
