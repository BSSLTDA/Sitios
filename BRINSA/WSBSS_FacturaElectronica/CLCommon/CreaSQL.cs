using System;
using System.Diagnostics;
using System.Text;

namespace CLCommon
{
    public class CreaSQL
    {
        static StringBuilder sbF = new StringBuilder();
        static StringBuilder sbV = new StringBuilder();
        static string query = "";
        private bool mostrarSQL;

        public void Ini(string fSql)
        {
            sbF.Clear();
            sbF.Append(fSql);
            sbV.Clear();
            query = (sbF.ToString() + " " + sbV.ToString()).Trim();
        }

        public void Add(string fSql)
        {
            sbF.Append(" ").Append(fSql.Trim()).Append("\n");
            query = (sbF.ToString() + " " + sbV.ToString()).Trim();
        }

        public void Ini(string fSql, string vSql)
        {
            sbF.Clear();
            sbF.Append(" ").Append(fSql.Trim()).Append("\n");
            sbV.Clear();
            sbV.Append(" ").Append(vSql.Trim()).Append("\n");
            query = (sbF.ToString() + " " + sbV.ToString()).Trim();
        }

        public void Add(string fSql, string vSql)
        {
            sbF.Append(" ").Append(fSql.Trim()).Append("\n");
            sbV.Append(" ").Append(vSql.Trim()).Append("\n");
            query = (sbF.ToString() + " " + sbV.ToString()).Trim();
        }

        public void Add(string sep, string fSql, string vSql)
        {
            sbF.Append(" ").Append(fSql.Trim()).Append("\n");
            sbV.Append(" ").Append(sep);
            sbV.Append(vSql.Trim());
            sbV.Append(sep).Append("\n");
            query = (sbF.ToString() + " " + sbV.ToString()).Trim();
        }
        public string GetSQL()
        {
            if (mostrarSQL)
            {
                Console.WriteLine(sbF.ToString() + " " + sbV.ToString() + "\n\n");
                mostrarSQL = false;
            }

            return query;
        }

        public string CcsId037(string campo)
        {
            return " TRIM(CAST(" + campo + " as VARCHAR(150) CCSID 037)) ";
        }

        public string GetSQL(string tit)
        {
            if (!tit.Equals(""))
            {

                Debug.Print(tit + "\n" + sbF.ToString() + " " + sbV.ToString() + "\n\n");
            }
            return (sbF.ToString() + " " + sbV.ToString()).Trim();
        }

        public void SetMostrarSQL(bool mostrarSQL)
        {
            this.mostrarSQL = mostrarSQL;
        }
    }
}
