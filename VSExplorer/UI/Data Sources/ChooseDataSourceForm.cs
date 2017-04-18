using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class ChooseDataSourceForm : Form
    {
        [DllImport("odbccp32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SQLGetInstalledDriversW(char[] lpszBuf, ushort cbufMax, out ushort pcbBufOut);

        public ChooseDataSourceForm()
        {
            InitializeComponent();

            listBox1.Items.Add("Microsoft Access Database File");
            listBox1.Items.Add("Microsoft ODBC Data Source");
            listBox1.Items.Add("Microsoft Sql Server");
            listBox1.Items.Add("Microsoft SQLServer Database File");
            listBox1.Items.Add("MySQL database");
            listBox1.Items.Add("Oracle database");
            listBox1.Items.Add(" other >");

            //string[] d = GetOdbcDriverNames();

            //listBox1.Items.AddRange(d);

            //DataTable c = GetProviderFactoryClasses();

            //foreach (DataRow dc in c.Rows)
            //    listBox1.Items.Add(dc[1]);

            //LoadOleDB();

            lb = listBox1;

            cb = comboBox1;

            rb = richTextBox1;

            lb.SelectedIndexChanged += Lb_SelectedIndexChanged;

            OP = GetOracleProviders();

            SP = GetProviders("SQL Server");
        }

        private ArrayList SP { get; set; }

        private ArrayList OP { get; set; }

        public static ArrayList GetProviders(string provider, bool odbc = false)
        {
            ArrayList P = new ArrayList();

            provider = provider.ToLower();

            if (odbc == true)
            {
                string[] c = GetOdbcDriverNames();

                foreach (string cs in c)
                    if (cs.ToLower().Contains(provider))
                        P.Add(cs);
            }

            DataTable d = GetProviderFactoryClasses();

            foreach (DataRow r in d.Rows)
            {
                if (r.ItemArray.Count() >= 2)
                {
                    string s = r[1] as string;
                    if (s.ToLower().Contains(provider))
                    {
                        P.Add(s);
                        continue;
                    }
                }
            }

            return P;
        }

        public static ArrayList GetOracleProviders(bool odbc = false)
        {
            ArrayList P = new ArrayList();

            if (odbc == true)
            {
                string[] c = GetOdbcDriverNames();

                foreach (string cs in c)
                    if (cs.Contains("Oracle"))
                        P.Add(cs);
            }
            DataTable d = GetProviderFactoryClasses();

            foreach (DataRow r in d.Rows)
            {
                if (r.ItemArray.Count() >= 2)
                {
                    string s = r[1] as string;
                    if (s.Contains("Oracle"))
                    {
                        P.Add(s);
                        continue;
                    }
                }
            }

            return P;
        }

        private string[] texts = { "Access", "ODBC", "SQL Server", "SQL Server database file", "MySQL", "Oracle" };

        private void Lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lb.SelectedIndex;
            if (index < 0)
                return;
            if (index == 2)
            {
                ArrayList L = GetSQLServerProviders();

                comboBox1.Items.Clear();

                foreach (string s in L)
                    comboBox1.Items.Add(s);
            }
            else if (index == 4)
            {
                ArrayList L = GetMySQLServerProviders();

                comboBox1.Items.Clear();

                foreach (string s in L)
                    comboBox1.Items.Add(s);
            }
            else if (index == 5)
            {
                ArrayList L = GetOracleServerProviders();

                comboBox1.Items.Clear();

                foreach (string s in L)
                    comboBox1.Items.Add(s);
            }

            if (cb.Items.Count > 0)
                cb.SelectedIndex = 0;

            if (index < texts.Length)
                rb.Text = texts[index];
        }

        private ListBox lb { get; set; }

        private ComboBox cb { get; set; }

        private RichTextBox rb { get; set; }

        /// <summary>
        /// Gets the ODBC driver names from the SQLGetInstalledDrivers function.
        /// </summary>
        /// <returns>a string array containing the ODBC driver names, if the call to SQLGetInstalledDrivers was successfull; null, otherwise.</returns>
        public static string[] GetOdbcDriverNames()
        {
            string[] odbcDriverNames = null;
            char[] driverNamesBuffer = new char[ushort.MaxValue];
            ushort size;

            bool succeeded = SQLGetInstalledDriversW(driverNamesBuffer, ushort.MaxValue, out size);

            if (succeeded == true)
            {
                char[] driverNames = new char[size - 1];
                Array.Copy(driverNamesBuffer, driverNames, size - 1);
                odbcDriverNames = (new string(driverNames)).Split('\0');
            }

            return odbcDriverNames;
        }

        // This example assumes a reference to System.Data.Common.
        public static DataTable GetProviderFactoryClasses()
        {
            // Retrieve the installed providers and factories.
            DataTable table = DbProviderFactories.GetFactoryClasses();

            // Display each row and column value.
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Console.WriteLine(row[column]);
                }
            }
            return table;
        }

        private void LoadOleDB()
        {
            // OleDbEnumerator enumerator = new OleDbEnumerator();
            OleDbEnumerator enumerator = new OleDbEnumerator();
            DataTable d = enumerator.GetElements();
            foreach (DataRow dc in d.Rows)
                listBox1.Items.Add(dc[2]);
        }

        private void DisplayData()
        {
            var reader = OleDbEnumerator.GetRootEnumerator();

            var list = new List<String>();
            while (reader.Read())
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i) == "SOURCES_NAME")
                    {
                        list.Add(reader.GetValue(i).ToString());
                    }
                }
                listBox1.Items.Add(reader.GetName(0) + " - " + reader.GetValue(0));
            }
            reader.Close();
        }

        private ArrayList GetSQLServerProviders()
        {
            ArrayList L = new ArrayList();
            foreach (string s in SP)
                L.Add(s);
            return L;
        }

        private ArrayList GetMySQLServerProviders()
        {
            ArrayList L = new ArrayList();
            L.Add("NET Framework MySQL ODBC Provider");
            L.Add("NET Framework MySQL Server provider");
            return L;
        }

        private ArrayList GetOracleServerProviders()
        {
            ArrayList L = new ArrayList();
            foreach (string s in OP)
                L.Add(s);
            return L;
        }

        public string shortcut { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = lb.SelectedIndex;
            if (index == 2)
            {
                AddConnectionForm acd = new AddConnectionForm();
                DialogResult r = acd.ShowDialog();
                if (r == DialogResult.OK)
                {
                    shortcut = "SQL Server - " + acd.shortcut;
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            else if (index == 4)
            {
                AddConnectionForm_Oracle acd = new AddConnectionForm_Oracle();
                DialogResult r = acd.ShowDialog();
                if (r == DialogResult.OK)
                {
                    shortcut = "Oracle - " + acd.shortcut;
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}