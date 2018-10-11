using CLCommon;
using Dapper;
using IBM.Data.DB2.iSeries;
using System;
using System.Configuration;
using System.Data;

namespace CLDB2
{
    public class RFNCCABRepository : IRFNCCABRepository
    {
        private IDbConnection db = new iDB2Connection(ConfigurationManager.ConnectionStrings["ConexionDB2"].ToString());
        public string UpdCab(string Prefijo, string Nota)
        {
            string res = "";
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" UPDATE");
            sbF.Add("        RFNCCAB");
            sbF.Add(" SET    FAREF02 = ''");
            sbF.Add(" WHERE");
            sbF.Add(string.Format("        FPREFIJ     = '{0}'", Prefijo));
            sbF.Add(string.Format("        AND FNOTA = {0}", Nota));
            sbF.Add("        AND FAREF02 = 'ENVIANDO'");
            try
            {
                db.Execute(sbF.GetSQL());
                res = "OK";
            }
            catch (iDB2SQLErrorException ex)
            {
                res = "ERROR: " + ex.ToString() + " " + sbF.GetSQL();
            }
            catch (iDB2Exception ex)
            {
                res = "ERROR: " + ex.ToString() + " " + sbF.GetSQL();
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.ToString() + " " + sbF.GetSQL();
            }
            return res;
        }

        public string UpdExistePDF(string Prefijo, string Nota)
        {
            string res = "";
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" UPDATE");
            sbF.Add("        RFNCCAB");
            sbF.Add(string.Format(" SET    FAREF02 = '{0}'", Prefijo + Nota));
            sbF.Add(" , FAFLAG04 = 1");
            sbF.Add(" WHERE");
            sbF.Add(string.Format("        FPREFIJ     = '{0}'", Prefijo));
            sbF.Add(string.Format("        AND FNOTA = {0}", Nota));            
            try
            {
                db.Execute(sbF.GetSQL());
                res = "OK";
            }
            catch (iDB2SQLErrorException ex)
            {
                res = "ERROR: " + ex.ToString() + " " + sbF.GetSQL();
            }
            catch (iDB2Exception ex)
            {
                res = "ERROR: " + ex.ToString() + " " + sbF.GetSQL();
            }
            catch (Exception ex)
            {
                res = "ERROR: " + ex.ToString() + " " + sbF.GetSQL();
            }
            return res;
        }
    }
}
