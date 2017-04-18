using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.IO;

namespace WinExplorer.UI
{
    public class TSQLParser
    {
        public string Parse(string sqlSelect, out string fields, out string from, out string groupby, out string where, out string having, out string orderby)
        {
            TSql100Parser parser = new TSql100Parser(false);
            TextReader rd = new StringReader(sqlSelect);
            IList<ParseError> errors;
            var fragments = parser.Parse(rd, out errors);

            fields = string.Empty;
            from = string.Empty;
            groupby = string.Empty;
            where = string.Empty;
            orderby = string.Empty;
            having = string.Empty;

            if (errors.Count > 0)
            {
                var retMessage = string.Empty;

                return retMessage;
            }

            try
            {
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return string.Empty;
        }
    }
}