using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        public static TSqlScript ParseScript(string script, out IList<string> errorsList)
        {
            IList<ParseError> parseErrors;
            TSql100Parser tsqlParser = new TSql100Parser(true);
            TSqlFragment fragment;
            using (StringReader stringReader = new StringReader(script))
            {
                fragment = (TSqlFragment)tsqlParser.Parse(stringReader, out parseErrors);
            }
            errorsList = new List<string>();
            if (parseErrors.Count > 0)
            {
                var retMessage = string.Empty;
                foreach (var error in parseErrors)
                {
                    retMessage += error.Column + " - " + error.Message + " - position: " + error.Offset + "; ";
                }

            }
            return (TSqlScript)fragment;
        }

        public List<Token> GetTokenCollection(TSqlScript tsqlScript)
        {
            List<Token> lstTokens = new List<Token>();

            if (tsqlScript != null)
            {
                foreach (TSqlBatch batch in tsqlScript.Batches)
                {
                    if (batch.Statements.Count == 0) continue;

                    foreach (TSqlStatement statement in batch.Statements)
                    {

                        foreach (var token in statement.ScriptTokenStream)
                        {
                            lstTokens.Add(new Token { TokenType = token.TokenType.ToString() + (token.IsKeyword() ? "  --> keyword" : ""), Text = token.Text });

                        }
                    }
                }

            }
            return lstTokens;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string filePath = @"Test.sql";
            string Script = string.Empty;

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                Script = streamReader.ReadToEnd();
            }

            IList<string> errorsList;
            var TSqlScript = ParseScript(Script, out errorsList);

            dataGridView1.DataSource = GetTokenCollection(TSqlScript); 
        }
    }

    public class Token
    {
        public string TokenType { get; set; }
        public string Text{ get; set; }
    }
}
