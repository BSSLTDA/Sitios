using CLCommon;
using Dapper;
using IBM.Data.DB2.iSeries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CLDB2
{
    public class RFLOGFERepository : IRFLOGFERepository
    {
        private IDbConnection db = new iDB2Connection(ConfigurationManager.ConnectionStrings["ConexionDB2"].ToString());
        public List<RFLOGFE> SinPDF()
        {
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" SELECT");
            sbF.Add("            CASE WHEN L.DOCTIPO ='FE' THEN TRIM(L.DOCTIPO) ELSE TRIM(L.DOCTIPO) || 'E' END DOCTIPO");
            sbF.Add("          , TRIM(L.DOCPREFI) DOCPREFI");
            sbF.Add("          , L.DOCNUMERO");
            sbF.Add("          , L.FECAPI");
            sbF.Add("          , TIMESTAMPDIFF(4, CHAR(NOW() - L.FECUPD)) MINUTOS");
            sbF.Add("          , L.BUSY");
            sbF.Add(" FROM");
            sbF.Add("            RFLOGFE L");
            sbF.Add("            INNER JOIN RFPARAM P ON P.CCCODE2 = L.DOCPREFI AND L.DOCNUMERO >= P.CCUDV1 AND P.CCTABL = 'FEPARAMG' AND P.CCCODE = 'PREFIJO'");
            sbF.Add(" WHERE");
            sbF.Add("            STSPDF NOT IN(0, -1)");
            sbF.Add("            AND L.DOCTIPO = 'FE' LIMIT 24");
            return db.Query<RFLOGFE>(sbF.GetSQL()).ToList();
        }

        public List<RFLOGFE> SinPDFN()
        {
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" SELECT");
            sbF.Add("            CASE WHEN TRIM(L.DOCTIPO) = 'FE' THEN TRIM(L.DOCTIPO) ELSE CASE WHEN TRIM(L.DOCTIPO) = 'NC' THEN TRIM(L.DOCTIPO) || 'E' ELSE TRIM(L.DOCTIPO) END END DOCTIPO");
            sbF.Add("          , TRIM(L.DOCPREFI) DOCPREFI");
            sbF.Add("          , L.DOCNUMERO");
            sbF.Add("          , L.FECAPI");
            sbF.Add("          , TIMESTAMPDIFF(4, CHAR(NOW() - L.FECUPD)) MINUTOS");
            sbF.Add("          , L.BUSY");
            sbF.Add(" FROM");
            sbF.Add("            RFLOGFE L");
            sbF.Add("            INNER JOIN RFPARAM P ON P.CCCODE2 = L.DOCPREFI AND L.DOCNUMERO >= P.CCUDV1 AND P.CCTABL = 'FEPARAMG' AND P.CCCODE = 'PREFIJO'");
            sbF.Add(" WHERE");
            sbF.Add("            STSPDF NOT IN(0, -1)");
            sbF.Add("            AND L.DOCTIPO <> 'FE' LIMIT 24");
            return db.Query<RFLOGFE>(sbF.GetSQL()).ToList();
        }

        public string UpdBusy(string Prefijo, string Numero)
        {
            string res = "";
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" UPDATE");
            sbF.Add("        RFLOGFE");
            sbF.Add(" SET    BUSY = 0");
            sbF.Add(" WHERE");
            sbF.Add(string.Format("        DOCPREFI      = '{0}'", Prefijo));
            sbF.Add(string.Format("        And DOCNUMERO = {0}", Numero));
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

        public List<RFLOGFE> GetLog(string Prefijo, string Numero)
        {
            CreaSQL sbF = new CreaSQL();
            sbF.Ini(" SELECT");
            sbF.Add("            DOCNUMERO");            
            sbF.Add(" FROM");
            sbF.Add("            RFLOGFE");            
            sbF.Add(" WHERE");
            sbF.Add(string.Format("            DOCPREFI = '{0}'", Prefijo));
            sbF.Add(string.Format("            AND DOCNUMERO = {0}", Numero));
            sbF.Add("            AND (BUSY = 0");
            sbF.Add("            OR ( BUSY = 1 OR FECUPD >= NOW() - 15 MINUTES ))");
            return db.Query<RFLOGFE>(sbF.GetSQL()).ToList();
        }
    }
}
